using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml;

using NPOI;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.Util;

using NUnit.Framework;

// origin: https://github.com/nissl-lab/npoi/blob/master/testcases/ooxml/TestPOIXMLDocument.cs
namespace Tests {

	[TestFixture]
	public class POIXMLDocumentTest {

		private static readonly Dictionary<String, POIXMLDocumentPart> context = new Dictionary<String, POIXMLDocumentPart>();
		private static readonly StringBuilder stringBuilder = new StringBuilder(); 

		private void Traverse(POIXMLDocumentPart part, Dictionary<String, POIXMLDocumentPart> context) {

			// NOTE: deprecated in POI 3.14, scheduled for removal in POI 3.16")]
			Assert.AreEqual(part.GetPackageRelationship().TargetUri.ToString(), part.GetPackagePart().PartName.Name);

			context[part.GetPackagePart().PartName.Name] = part;
			foreach (POIXMLDocumentPart documentPart in part.GetRelations()) {
				Assert.IsNotNull(documentPart);
				Console.Error.WriteLine("Document Part: " + documentPart.ToString());
				// Console.WriteLine("Document Content Type: " + documentPart.GetPackagePart().ContentType.ToString());
				if ("application/vnd.openxmlformats-officedocument.presentationml.slide+xml".Equals(documentPart.GetPackagePart().ContentType.ToString())) {
					PrintSlideText(documentPart.GetPackagePart().GetStream(FileMode.Open), stringBuilder);
					// Console.Error.WriteLine(new StreamReader(documentPart.GetPackagePart().GetStream(FileMode.Open)).ReadToEnd());
				}
				String uri = documentPart.GetPackagePart().PartName.URI.ToString();
				StringAssert.AreEqualIgnoringCase(uri, documentPart.GetPackageRelationship().TargetUri.ToString());
				if (!context.ContainsKey(uri)) {
					Traverse(documentPart, context);
				} else {
					POIXMLDocumentPart prev = context[uri];
					Assert.AreSame(prev, documentPart, "Duplicate POIXMLDocumentPart instance for targetURI=" + uri);
				}
			}
		}
		private void PrintSlideText(Stream stream, StringBuilder stringBuilder) {
		    var xml = new XmlDocument();
		    xml.Load(stream);

		    var namespaceManager = new XmlNamespaceManager(xml.NameTable);

		    namespaceManager.AddNamespace( "a", "http://schemas.openxmlformats.org/drawingml/2006/main");

		    XmlNodeList textNodes = xml.SelectNodes("//a:t", namespaceManager);

		    foreach (XmlNode node in textNodes) {
		    	stringBuilder.Append(node.InnerText).Append(Environment.NewLine);
		    }
		}

		private void PrintSlideText(Stream stream) {
		    var xml = new XmlDocument();
		    xml.Load(stream);

		    var namespaceManager = new XmlNamespaceManager(xml.NameTable);

		    namespaceManager.AddNamespace( "a", "http://schemas.openxmlformats.org/drawingml/2006/main");

		    XmlNodeList textNodes = xml.SelectNodes("//a:t", namespaceManager);

		    foreach (XmlNode node in textNodes) {
		        Console.WriteLine(node.InnerText);
		    }
		}
    [TearDown]
    public void AfterEachTest() {
			stringBuilder.Clear();
			context.Clear();
    }

    [TestFixtureSetUp]
    public void BeforeFixture()
    {
        // Runs once before any tests in the class
    }
		[Test]
		public void test1() {
			// TODO: instantiate stringBuilder locally and pass it through Traverse 
			POIDataSamples pds = POIDataSamples.GetSlideShowInstance();
			OPCPackage pkg = PackageHelper.Open(pds.OpenResourceAsStream("sample-presentation.pptx"));
			var doc = new OPCParser(pkg);
			doc.Parse(new TestFactory());
			Traverse(doc, context);
			StringAssert.Contains("Project Goals",stringBuilder.ToString(),"not found expected content");
		}

		// simply calling an NPOI TextShape Text convenience method is not possible with NPOI:
		// XMLSlideShow is an Apache POI Java class, not the NPOI in C# project. 
		// Apache POI 3.17 does indeed have org.apache.poi.xslf.usermodel.XMLSlideShow.
		// but NPOI is a .NET port of POI, but its API is not necessarily a 1:1 namespace/type translation
		/*
		[Test]
		public void test2() {
			POIDataSamples pds = POIDataSamples.GetSlideShowInstance();

			using (Stream stream =
				        pds.OpenResourceAsStream("sample-presentation.pptx")) {
				var slideshow = new XMLSlideShow(stream);

				foreach (XSLFSlide slide in slideshow.GetSlides()) {
					foreach (XSLFShape shape in slide.GetShapes()) {
						var textShape = shape as XSLFTextShape;
						if (textShape != null) {
							Console.WriteLine(textShape.Text);
						}
					}
				}
			}
		}
		*/
	}

	// https://github.com/nissl-lab/npoi/blob/master/testcases/main/POIDataSamples.cs
	public class POIDataSamples {
		// unused
		public static String TEST_PROPERTY = "POI.testdata.path";

		private static POIDataSamples _instSlideshow;

		private string _resolvedDataDir;
		/** <c>true</c> if standard system propery is not set,
         * but the data is available on the test runtime classpath */
		private bool _sampleDataIsAvaliableOnClassPath;
		private String _moduleDir;

		private POIDataSamples(String moduleDir) {
			_moduleDir = moduleDir;
			Initialise();
		}

		public static POIDataSamples GetSlideShowInstance() {
			if (_instSlideshow == null)
				_instSlideshow = new POIDataSamples("");
			return _instSlideshow;
		}

		private Stream OpenClasspathResource(String sampleFileName) {
			var file = new FileStream(_resolvedDataDir + sampleFileName, FileMode.Open, FileAccess.Read);
			return file;
		}

		private void Initialise() {
			//  Some of the tests are locale dependent
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

			String dataDirName = AppDomain.CurrentDomain.BaseDirectory; // TestContext.Parameters[TEST_PROPERTY];

			if (dataDirName == null)
				throw new Exception("Must set system property '"
				+ TEST_PROPERTY
				+ "' before running tests");

			string dataDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, dataDirName, _moduleDir));
			if (!Directory.Exists(dataDir)) {
				throw new IOException("Data dir '" + dataDir
				+ "' specified by system property '"
				+ TEST_PROPERTY + "' does not exist");
			}

			_sampleDataIsAvaliableOnClassPath = true;
			_resolvedDataDir = dataDir + Path.DirectorySeparatorChar;
		}

		public string ResolvedDataDir {
			get { return _resolvedDataDir; }
		}

		/**
* Opens a sample file from the standard HSSF test data directory
*
* @return an Open <c>Stream</c> for the specified sample file
*/
		public Stream OpenResourceAsStream(String sampleFileName)
		{
			Initialise();

			if (_sampleDataIsAvaliableOnClassPath) {
				Stream result = OpenClasspathResource(sampleFileName);
				if (result == null) {
					throw new Exception("specified test sample file '" + sampleFileName
					+ "' not found on the classpath");
				}
				//			System.out.println("Opening cp: " + sampleFileName);
				// wrap to avoid temp warning method about auto-closing input stream
				return new NonSeekableStream(result);
			}
			if (_resolvedDataDir == "") {
				throw new Exception("Must set system property '"
				+ TEST_PROPERTY
				+ "' properly before running tests");
			}


			if (!File.Exists(_resolvedDataDir + sampleFileName)) {
				throw new Exception("Sample file '" + sampleFileName
				+ "' not found in data dir '" + _resolvedDataDir + "'");
			}


			//		System.out.println("Opening " + f.GetAbsolutePath());
			try {
				return new FileStream(_resolvedDataDir + sampleFileName, FileMode.Open, FileAccess.Read);
			} catch (FileNotFoundException) {
				throw;
			}
		}

		public FileInfo GetFileInfo(String sampleFileName)
		{
			string path = _resolvedDataDir + sampleFileName;
			if (!File.Exists(path)) {
				throw new Exception("Sample file '" + sampleFileName
				+ "' not found in data dir '" + _resolvedDataDir + "'");
			}
			return new FileInfo(path);
		}

		/**
         *
         * @param sampleFileName    the name of the test file
         * @return
         * @throws RuntimeException if the file was not found
         */
		public FileStream GetFile(String sampleFileName) {
			string path = _resolvedDataDir + sampleFileName;
			if (!File.Exists(path)) {
				throw new Exception("Sample file '" + sampleFileName
				+ "' not found in data dir '" + _resolvedDataDir + "'");
			}
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}

        public string[] GetFiles() {
			return Directory.GetFiles(_resolvedDataDir);
		}

        public string[] GetFiles(string searchPattern) {
			return Directory.GetFiles(_resolvedDataDir, searchPattern);
		}

        public byte[] ReadFile(String fileName) {
			var memoryStream = new MemoryStream();

			try {
				Stream fileStream = OpenResourceAsStream(fileName);

				var buffer = new byte[512];
				while (true) {
					int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
					if (bytesRead < 1) {
						break;
					}
					memoryStream.Write(buffer, 0, bytesRead);
				}
				fileStream.Close();
			} catch (IOException) {
				throw;
			}
			return memoryStream.ToArray();
		}

		private class NonSeekableStream : Stream {

			private Stream _is;

			public NonSeekableStream(Stream is1) {
				_is = is1;
			}

			public int Read() {
				return _is.ReadByte();
			}

			public override int Read(byte[] b, int off, int len) {
				return _is.Read(b, off, len);
			}

			public bool markSupported() {
				return false;
			}

			public override void Close() {
				_is.Close();
			}

			public override bool CanRead {
				get { return _is.CanRead; }
			}

			public override bool CanSeek {
				get { return false; }
			}

			public override bool CanWrite {
				get { return _is.CanWrite; }
			}

			public override long Length {
				get { return _is.Length; }
			}

			public override long Position {
				get { return _is.Position; }
				set { _is.Position = value; }
			}

			public override void Write(byte[] buffer, int offset, int count) {
				_is.Write(buffer, offset, count);
			}

			public override void Flush() {
				_is.Flush();
			}

			public override long Seek(long offset, SeekOrigin origin) {
				return _is.Seek(offset, origin);
			}

			public override void SetLength(long value) {
				_is.SetLength(value);
			}
		}
	}

	public class OPCParser : POIXMLDocument {

		public OPCParser(OPCPackage pkg) : base(pkg) {
		}

		public override List<PackagePart> GetAllEmbedds() {
			throw new NotSupportedException();
		}

		public void Parse(POIXMLFactory factory) {
			Load(factory);
		}
	}

	public class TestFactory : POIXMLFactory {

		public TestFactory(){
			//
		}
		protected override POIXMLRelation GetDescriptor(String relationshipType) {
			return null;
		}
		protected override POIXMLDocumentPart CreateDocumentPart(Type cls, Type[] classes, Object[] values) {
			return null;
		}
	}
}
