using System;
using NUnit.Framework;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;

using Utils;

namespace Test {

	[TestFixture]
	public class ConsoleFormTests {

		private StringBuilder verificationErrors = new StringBuilder();
		private ConsoleForm splash = null;
		private Line line;
		private Point location;
		private Lines details;
		private List<Line> lines;
		// private String result;
		private static string json = null;
		private static bool debug = false;

		[SetUp]
		public void setUp() {
			
		}

		[TearDown]
		public void tearDown()
		{
			splash = null;
			Assert.AreEqual("", verificationErrors.ToString());
		}

		[Test] 
		public void test1() {
			lines = new List<Line>();
			line = new Line();
			line.Orientation = Line.LineOrientation.Horizontal;
			line.Colour = System.ConsoleColor.Black;
			location = new Point();
			location.X = 10;
			location.Y = 20;
			line.Location = location;
			lines.Add(line);
			details = new Lines();
			details.Details = lines;
			splash = new ConsoleForm();
			splash.Lines = details;
			splash.Name = "Splash";
			splash.Debug = debug;
			json = JsonConvert.SerializeObject(splash, Newtonsoft.Json.Formatting.Indented);
			Console.Error.WriteLine(json);
			splash = JsonConvert.DeserializeObject<ConsoleForm>(json);

			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(1, splash.Lines.Details.Count);			
		}

		
	[Test]
		public void test2(){
			splash = ConsoleForm.GetFormInstance(@"JSON\\splash.json");
			splash.Debug = debug;			
			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(2, splash.Lines.Details.Count);
			lines = splash.Lines.Details;
			Assert.AreEqual(44, lines[1].Location.X);
			Assert.AreEqual(1, splash.Labels.Details.Count);
			
			Assert.AreEqual(ConsoleColor.DarkGreen, splash.Textboxes.Details[0].ForeColour);
		}
				
	[Test]
		public void test3(){
			json = ConsoleForm.getFileContents(@"JSON\\splash.json");
			splash = JsonConvert.DeserializeObject<ConsoleForm>(json);
			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(2, splash.Lines.Details.Count);
			lines = splash.Lines.Details;
			Assert.AreEqual(44, lines[1].Location.X);
			Assert.AreEqual(1, splash.Labels.Details.Count);
			
			Assert.AreEqual(ConsoleColor.DarkGreen, splash.Textboxes.Details[0].ForeColour);
			Assert.AreEqual(20, splash.Textboxes.Details[0].Location.X);
			Assert.AreEqual("txtLoginID", splash.Textboxes.Details[0].Name);
		}
	}
	
}
