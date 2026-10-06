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

// origin: https://github.com/nissl-lab/npoi/blob/master/testcases/ooxml/TestPOIXMLDocument.cs
namespace Tests
{

	[TestFixture]
	public class POIXMLDocumentTest
	{

		private void Traverse(POIXMLDocumentPart part, Dictionary<String, POIXMLDocumentPart> context, StringBuilder stringBuilder)
		{

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
					Traverse(documentPart, context, stringBuilder);
				} else {
					POIXMLDocumentPart prev = context[uri];
					Assert.AreSame(prev, documentPart, "Duplicate POIXMLDocumentPart instance for targetURI=" + uri);
				}
			}
		}

		private void PrintSlideText(Stream stream, StringBuilder stringBuilder)
		{
			var xml = new XmlDocument();
			xml.Load(stream);

			var namespaceManager = new XmlNamespaceManager(xml.NameTable);

			namespaceManager.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

			XmlNodeList textNodes = xml.SelectNodes("//a:t", namespaceManager);

			foreach (XmlNode node in textNodes) {
				stringBuilder.Append(node.InnerText).Append(Environment.NewLine);
				System.Diagnostics.Debug.WriteLine(node.InnerText);
			}
		}

		[Test]
		public void test1()
		{
			var context = new Dictionary<String, POIXMLDocumentPart>();
			var stringBuilder = new StringBuilder(); 
			// NOTE: on C#, StringBuilder is a reference type
			// When passed a StringBuilder arg into a method, a reference to that object is 
			// passed by value meaning callee can modify its internal contents inside the method without needing 
			// the ref keyword - explicit is dicouraged
			var doc = new OPCParser(PackageHelper.Open(File.OpenRead("sample-presentation.pptx")));
			doc.Parse(new TestFactory());
			Traverse(doc, context, stringBuilder);
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
			StringAssert.DoesNotContain( "Product A", result,"legend");
			doc.Close();
			
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
	// some method not implemeted
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
}
