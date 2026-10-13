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
			private const string filename = "sample-presentation.pptx";
			private string region = null;
			private string searchText = "Sample Presentation";

		[Test]
		public void test1() {
			searchText = "Sample Presentation";
			var	powerPointSearch = new PowerPointSearch(filename, searchText);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<PowerPointResult> match  = ( PowerPointResult result) => result.Text.Contains(searchText);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain( "Product A", results.First().Text,"legend is not in slides");
		}

		[Test]
		public void test2() {
			searchText = "Product A";
			region = "chart";
			var	powerPointSearch = new PowerPointSearch(filename, region, searchText);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<PowerPointResult> match  = ( PowerPointResult result) => result.Text.Contains(searchText);
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain search string");
			StringAssert.DoesNotContain( "Sample Presentation", results.First().Text,"slide title is not in chart legend");
		}

		[Test]
		public void test3() {
			var texts = new List<string> {
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
			var	powerPointSearch = new PowerPointSearch(filename, region, texts);
			var results = powerPointSearch.findText();
			Assert.Greater(results.Count, 0, "expect at least one result");
			Predicate<PowerPointResult> match = ( PowerPointResult result) => texts.Exists((string text) => result.Text.Contains(text));
			Assert.AreEqual(results.Count, results.FindAll(match).Count, "expect all results to contain at least one search term");
			var duplicates = results.GroupBy(result => result.Location).Where(group => group.Count() > 1);
			Assert.IsEmpty(duplicates, "Result locations should be unique");
			StringAssert.DoesNotContain("Product A", results.First().Text, "legend is not in slides");
			Assert.IsFalse(
				results.Any(result => result.Text.Contains("Product A")),
				"Chart legend text should not appear in slide results");
		}
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
		public string Region;
		public string Part;
		public int Index;
		public string Text;
		// NOTE: To use later for deduplication
		public string Location { get { return String.Format("{0}:{1}:{2}",  Region, Part, Index );  }}
		// NOTE: the StackOverflowException will not be detected by the compiler
		// a static-analysis tool or IDE inspection may flag self-recursion
		// public string Location { get { return String.Format("{0}:{1}:{2}",  Location, Part, Index );  }}
	}

	public class PowerPointSearch {

		private List<PowerPointResult> results;
		public PowerPointSearch(string filename, string text)
			: this(filename, null, new[] { text }) {
			// NOTE: cannot call a constructor inside the body of another constructor
		}

		public PowerPointSearch(string filename, string region, string text)
			: this(filename, region, new[] { text }) {	}

		public PowerPointSearch(
			string filename, string region, IEnumerable<string> texts) {
			if (String.IsNullOrWhiteSpace(filename)) // better than IsNullOrEmpty
				throw new ArgumentException("Filename cannot be blank.", "filename");
			this.filename = filename;
			if (String.IsNullOrWhiteSpace(region)) {
				// fallback to initial value "slides"
			} else {
				if (!nestedDict.ContainsKey(region))
					throw new ArgumentException(String.Format("Unsupported region: {0}", region), "region");
				this.region = region;
			}

			if (texts == null)
				throw new ArgumentNullException("texts");


			if (texts.Count() == 0)
				throw new ArgumentException(
					"At least one search term is required.", "texts");

			if (texts.ToList().Any(o => String.IsNullOrWhiteSpace(o)))
				throw new ArgumentException(
					"Search terms cannot be blank.", "texts");

			this.texts = new HashSet<string>(
				texts, StringComparer.Ordinal);

		}

		private string filename;
		public string Filename {
			get { return filename; }
			set { filename = value; }
		}
		private HashSet<string> texts;
		public IEnumerable<string> Texts {
			get { return texts; }
			set { texts = value.ToHashSet(); }
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
					getText(documentPart.GetPackagePart().PartName.Name.ToString(), documentPart.GetPackagePart().GetStream(FileMode.Open));
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
						"application/vnd.openxmlformats-officedocument.presentationml.slide+xml" },
					{ "selector", "//a:t" },
					{ "prefix", "a" },
					{ "namespace", "http://schemas.openxmlformats.org/drawingml/2006/main" }
				}
			}, {"chart", new Dictionary<string, string> { {
						"contenttype",
						"application/vnd.openxmlformats-officedocument.drawingml.chart+xml" },
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
			traverseDocumentPart(doc, context);
			return results;
		}

		private void getText(string part, Stream stream) {
			int index = 0;
			Console.Error.WriteLine(String.Format("getText Part: {0}", part));
			var xml = new XmlDocument();
			xml.Load(stream);

			var namespaceManager = new XmlNamespaceManager(xml.NameTable);

			namespaceManager.AddNamespace(nestedDict[region]["prefix"], nestedDict[region]["namespace"]);

			XmlNodeList textNodes = xml.SelectNodes(nestedDict[region]["selector"], namespaceManager);

			foreach (XmlNode node in textNodes) {
				index++;
				var cellText = node.InnerText;
				texts.ToList().ForEach((string text) => {
					if (cellText.Contains(text)) {
						var location = String.Format("{0}:{1}:{2}", region, part, index);
						if (!results.Exists(o => o.Location == location)) {
							results.Add(new PowerPointResult {
								Region = region,
								Part = part,
								Index = index,
								Text = cellText
							});
						}
					}
				});
			}
		}
	}
}
