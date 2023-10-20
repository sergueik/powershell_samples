using System;
using NUnit.Framework;
using System.Linq;
using System.Xml.Schema;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

using Program;

namespace Test {

	[TestFixture]
	public class ConsoleFormTests {
		private StringBuilder verificationErrors = new StringBuilder();
		private ConsoleForm splash = null;
		private ConsoleForm login = null;
		private XmlWriter xmlWriter;
		private StringBuilder stringBuilder = new StringBuilder();
		private String result;
		private static string json = null;
		private static bool debug;

		[SetUp]
		public void setUp() {
			splash = ConsoleForm.GetFormInstance(@"Forms\Splash.xml");
			login = ConsoleForm.GetFormInstance(@"Forms\Login.xml");
		}

		[TearDown]
		public void tearDown() {
			// Cannot access protected member 'object.~Object()' 
			// via a qualifier of type 'Program.ConsoleForm'; 
			// the qualifier must be of type 'Test.ConsoleFormTests' (or derived from it) (CS1540)
			// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers#explicit-release-of-resources
			// splash.Finalize();
			// splash.Dispose();
			splash = null;
			login = null;
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test] 
		public void test1() {
			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(6, splash.Lines.Count);			
			Assert.AreEqual(0, splash.Textboxes.Count);
		}

		[Test]
		// https://csharp.hotexamples.com/examples/-/IXmlSerializable/WriteXml/php-ixmlserializable-writexml-method-examples.html
		public void test2()
		{
			// https://learn.microsoft.com/en-us/dotnet/api/system.xml.xmlwritersettings.conformancelevel?view=netframework-4.0
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.ConformanceLevel = ConformanceLevel.Auto;
			settings.CloseOutput = false;
			// System.InvalidOperationException : 
			// Token StartAttribute in state Start would result in an invalid XML document. 
			// Make sure that the ConformanceLevel setting is set to ConformanceLevel.Fragment 
			// or ConformanceLevel.Auto 
			// if you want to write an XML fragment
			
			// System.InvalidOperationException :  
			// Token StartElement in state After Root Level Attribute would result in 
			// an invalid XML document.
			stringBuilder = new StringBuilder();
			xmlWriter = XmlWriter.Create(stringBuilder, settings);
			
			splash.testWriteXml(xmlWriter);
      		
			xmlWriter.Flush();
			
			Assert.AreEqual(1384, stringBuilder.Length);
			result = stringBuilder.ToString();
			StringAssert.Contains("<ConsoleForm", result);
			StringAssert.Contains(@"Name=""Splash""", result);
			Console.Error.WriteLine(stringBuilder.ToString());
			xmlWriter.Close();
		}

	}
}
