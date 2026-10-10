using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// based on: https://github.com/nissl-lab/npoi/blob/master/testcases/ooxml/TestPOIXMLDocument.cs
namespace Tests {

	[TestFixture]
	public class PowerPointTest {

		private void traverseDocumentPart(POIXMLDocumentPart part, string region, Dictionary<String, POIXMLDocumentPart> context, StringBuilder stringBuilder) {
			// NOTE: deprecated in POI 3.14, scheduled for removal in POI 3.16")]
			Assert.AreEqual(part.GetPackageRelationship().TargetUri.ToString(), part.GetPackagePart().PartName.Name);

			context[part.GetPackagePart().PartName.Name] = part;
			foreach (POIXMLDocumentPart documentPart in part.GetRelations()) {
				Assert.IsNotNull(documentPart);
				Console.Error.WriteLine("Document Part: " + documentPart.ToString());
				// Console.WriteLine("Document Content Type: " + documentPart.GetPackagePart().ContentType.ToString());
				if (nestedDict[region]["contenttype"].Equals(documentPart.GetPackagePart().ContentType.ToString())) {
					getText(documentPart.GetPackagePart().GetStream(FileMode.Open), region, stringBuilder);
					// Console.Error.WriteLine(new StreamReader(documentPart.GetPackagePart().GetStream(FileMode.Open)).ReadToEnd());
				}
				String uri = documentPart.GetPackagePart().PartName.URI.ToString();
				StringAssert.AreEqualIgnoringCase(uri, documentPart.GetPackageRelationship().TargetUri.ToString());
				if (!context.ContainsKey(uri)) {
					traverseDocumentPart(documentPart, region, context, stringBuilder);
				} else {
					POIXMLDocumentPart prev = context[uri];
					Assert.AreSame(prev, documentPart, "Duplicate POIXMLDocumentPart instance for targetURI=" + uri);
				}
			}
		}

		private void traverseDocumentPart(POIXMLDocumentPart part, Dictionary<String, POIXMLDocumentPart> context, StringBuilder stringBuilder){
			
			traverseDocumentPart(part, "slide", context, stringBuilder);
		}

		Dictionary<string, Dictionary<string, string>> nestedDict = new Dictionary<string, Dictionary<string, string>> { {  "slide", new Dictionary<string, string> { {
						"contenttype",
						"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"
					},
					{ "selector", "//a:t" },
					{ "prefix", "a" },
					{ "namespace", "http://schemas.openxmlformats.org/drawingml/2006/main" }
				}
			}, {"chart", new Dictionary<string, string> { {
						"contenttype",
						"application/vnd.openxmlformats-officedocument.drawingml.chart+xml"
					},
					{ "selector", "//c:v" },
					{ "prefix", "c" },
					{ "namespace", "http://schemas.openxmlformats.org/drawingml/2006/chart" }
					
				}
			}
			
		};

		private void getText(Stream stream, string region, StringBuilder stringBuilder) {
			var xml = new XmlDocument();
			xml.Load(stream);

			var namespaceManager = new XmlNamespaceManager(xml.NameTable);

			namespaceManager.AddNamespace(nestedDict[region]["prefix"], nestedDict[region]["namespace"]);

			XmlNodeList textNodes = xml.SelectNodes(nestedDict[region]["selector"], namespaceManager);

			foreach (XmlNode node in textNodes) {
				stringBuilder.Append(node.InnerText).Append(Environment.NewLine);
				System.Diagnostics.Debug.WriteLine(node.InnerText);
			}
		}

		[Test]
		public void test1() {
			var context = new Dictionary<String, POIXMLDocumentPart>();
			var stringBuilder = new StringBuilder();
			// NOTE: on C#, StringBuilder is a reference type
			// When passed a StringBuilder arg into a method, a reference to that object is
			// passed by value meaning callee can modify its internal contents inside the method without needing
			// the ref keyword - explicit is dicouraged
			var doc = new OPCParser(PackageHelper.Open(File.OpenRead("sample-presentation.pptx")));
			doc.Parse(new TestFactory());
			traverseDocumentPart(doc, context, stringBuilder);
			var fragments = new List<string> {
				"Sample Presentation",
				"Agenda",
				"Roadmap",
				"Project Goals",
				"Reduce churn",
				"Quarterly Revenue",
				"Sales by Quarter",
				"Embedded Image",
				"Trade-offs"
			};
			Assert.AreNotEqual("",stringBuilder.ToString() );
			var result = stringBuilder.ToString();
			foreach (string fragment in fragments)
				StringAssert.Contains(fragment, result, String.Format("{0} not found", fragment));
			StringAssert.DoesNotContain( "Product A", result,"legend is not in slides");
			doc.Close();
			
		}
		[Test]
		public void test() {
			var filename = "sample-presentation.pptx";
			var searchText = "Sample Presentation";
			var	powerPointSearch = new PowerPointSearch(filename, null, searchText);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");

		}
		[Test]
		public void test2() {
			var context = new Dictionary<String, POIXMLDocumentPart>();
			var stringBuilder = new StringBuilder();
			// NOTE: on C#, StringBuilder is a reference type
			// When passed a StringBuilder arg into a method, a reference to that object is
			// passed by value meaning callee can modify its internal contents inside the method without needing
			// the ref keyword - explicit is dicouraged
			var doc = new OPCParser(PackageHelper.Open(File.OpenRead("sample-presentation.pptx")));
			doc.Parse(new TestFactory());
			traverseDocumentPart(doc, "chart", context, stringBuilder);
			var fragments = new List<string> {
				"Product A",
				"Product B"
			};
			Assert.AreNotEqual("",stringBuilder.ToString() );
			var result = stringBuilder.ToString();
			foreach (string fragment in fragments)
				StringAssert.Contains(fragment, result, String.Format("{0} not found", fragment));
			StringAssert.DoesNotContain( "Project Goals", result,"slide title is not in chart");
			doc.Close();
			
		}
		
		// simply calling an NPOI TextShape Text convenience method is not possible with NPOI:
		// XMLSlideShow is an Apache POI Java class, not the NPOI in C# project.
		// Apache POI 3.17 does indeed have org.apache.poi.xslf.usermodel.XMLSlideShow.
		// but NPOI is a .NET port of POI, but its API is not necessarily a 1:1 namespace/type translation
		/*
		[Test]
		public void test4() {
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
	// some method not implemented
	public class OPCParser : POIXMLDocument {

		public OPCParser(OPCPackage pkg) : base(pkg) {}

		public override List<PackagePart> GetAllEmbedds(){
			throw new NotSupportedException();
		}

		public void Parse(POIXMLFactory factory){
			Load(factory);
		}
	}

	public class TestFactory : POIXMLFactory{

		public TestFactory() {}

		protected override POIXMLRelation GetDescriptor(String relationshipType) {
			return null;
		}

		protected override POIXMLDocumentPart CreateDocumentPart(Type cls, Type[] classes, Object[] values) {
			return null;
		}
	}
	
	public struct PowerPointResult {
		public int SlideNumber;
		public string Path; // leave blank, will use later for deduplication
		public string Text;
		public string Location { get { return String.Format("{0}",  SlideNumber );  }}
	}



	public class PowerPointSearch {
		private List<PowerPointResult> results;
		public PowerPointSearch(string filename, string region, string text ){
			if (String.IsNullOrWhiteSpace(filename)) // better than IsNullOrEmpty
				throw new ArgumentException( "Filename cannot be blank.", "filename");
			this.filename = filename;
			if (String.IsNullOrWhiteSpace(region)) {
				// fallback to initial value "slides"
			} else {
				if ( !nestedDict.ContainsKey(region))
					throw new ArgumentException( String.Format("Unsupported region: {0}", region), "region");
				this.region = region;
			}
			if (String.IsNullOrWhiteSpace(text))
				throw new ArgumentException("text cannot be blank", "text");
			this.text = text;
		}

		private string filename;
		public string Filename {
			get { return filename; }
			set { filename = value; }
		}
		private string text;
		public string Text {
			get { return text; }
			set { text = value; }
		}
		private string region = "slide";
		public string Region {
			get { return region; }
			set { region = value; }
		}

		private void traverseDocumentPart(POIXMLDocumentPart part, Dictionary<String, POIXMLDocumentPart> context) {
			// NOTE: deprecated in POI 3.14, scheduled for removal in POI 3.16")]
			Assert.AreEqual(part.GetPackageRelationship().TargetUri.ToString(), part.GetPackagePart().PartName.Name);

			context[part.GetPackagePart().PartName.Name] = part;
			foreach (POIXMLDocumentPart documentPart in part.GetRelations()) {
				Assert.IsNotNull(documentPart);
				Console.Error.WriteLine("Document Part: " + documentPart.ToString());
				// Console.WriteLine("Document Content Type: " + documentPart.GetPackagePart().ContentType.ToString());
				if (nestedDict[region]["contenttype"].Equals(documentPart.GetPackagePart().ContentType.ToString())) {
					getText(documentPart.GetPackagePart().GetStream(FileMode.Open));
					// Console.Error.WriteLine(new StreamReader(documentPart.GetPackagePart().GetStream(FileMode.Open)).ReadToEnd());
				}
				String uri = documentPart.GetPackagePart().PartName.URI.ToString();
				StringAssert.AreEqualIgnoringCase(uri, documentPart.GetPackageRelationship().TargetUri.ToString());
				if (!context.ContainsKey(uri)) {
					traverseDocumentPart(documentPart, context);
				} else {
					POIXMLDocumentPart prev = context[uri];
					Assert.AreSame(prev, documentPart, "Duplicate POIXMLDocumentPart instance for targetURI=" + uri);
				}
			}
		}

		Dictionary<string, Dictionary<string, string>> nestedDict = new Dictionary<string, Dictionary<string, string>> { {  "slide", new Dictionary<string, string> { {
						"contenttype",
						"application/vnd.openxmlformats-officedocument.presentationml.slide+xml"
					},
					{ "selector", "//a:t" },
					{ "prefix", "a" },
					{ "namespace", "http://schemas.openxmlformats.org/drawingml/2006/main" }
				}
			}, {"chart", new Dictionary<string, string> { {
						"contenttype",
						"application/vnd.openxmlformats-officedocument.drawingml.chart+xml"
					},
					{ "selector", "//c:v" },
					{ "prefix", "c" },
					{ "namespace", "http://schemas.openxmlformats.org/drawingml/2006/chart" }
					
				}
			}
			
		};

		public List<PowerPointResult> findText() {
			results = new List<PowerPointResult>();
			var context = new Dictionary<String, POIXMLDocumentPart>();
			// NOTE: on C#, StringBuilder is a reference type
			// When passed a StringBuilder arg into a method, a reference to that object is
			// passed by value meaning callee can modify its internal contents inside the method without needing
			// the ref keyword - explicit is dicouraged
			var doc = new OPCParser(PackageHelper.Open(File.OpenRead(filename)));
			doc.Parse(new TestFactory());
			traverseDocumentPart(doc,context);
			return results;
		}

		private void getText(Stream stream) {			
			var xml = new XmlDocument();
			xml.Load(stream);

			var namespaceManager = new XmlNamespaceManager(xml.NameTable);

			namespaceManager.AddNamespace(nestedDict[region]["prefix"], nestedDict[region]["namespace"]);

			XmlNodeList textNodes = xml.SelectNodes(nestedDict[region]["selector"], namespaceManager);

			foreach (XmlNode node in textNodes) {
				var cellText =  node.InnerText;
				
				if (cellText.Contains(text)) {
					results.Add(new PowerPointResult {
					           	SlideNumber = 0, // need to store it somewhere
					           	Path = null,
					           	Text = cellText
					           });
				}

			}
		}
		
	}
}
