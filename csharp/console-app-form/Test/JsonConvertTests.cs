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
	public class JsonConvertTests {

		private StringBuilder verificationErrors = new StringBuilder();
		private ConsoleForm splash = null;
		private Line line;
		private List<Line> lines;
		private Point location;			 
		private Lines details;
		private static string json = null;

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
			json = JsonConvert.SerializeObject(splash, Newtonsoft.Json.Formatting.Indented);
			Console.Error.WriteLine(json);
			splash = JsonConvert.DeserializeObject<ConsoleForm>(json);

			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(1, splash.Lines.Details.Count);			
		}

		
		[Test]
		// https://www.newtonsoft.com/json/help/html/DeserializeObject.htm
		public void test2()
		{
			json = @"
{
  ""Width"": 80,
  ""Height"": 40,
  ""Name"": ""Splash"",
  ""Lines"": {
    ""Details"": [
      {
        ""Orientation"": 1,
        ""Colour"": 0,
        ""Location"": {
          ""X"": 10,
          ""Y"": 20
        },
        ""Length"": 0
      },
      {
        ""Orientation"": 1,
        ""Colour"": 1,
        ""Location"": {
          ""X"": 44,
          ""Y"": 55
        },
        ""Length"": 100
      }
    ]
  }
}
";
			splash = JsonConvert.DeserializeObject<ConsoleForm>(json);
			
			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(2, splash.Lines.Details.Count);
			lines = splash.Lines.Details;
			Assert.AreEqual(44, lines[1].Location.X);
		}

		[Test]
		public void test3()
		{
			json = @"
{
  ""Width"": 80,
  ""Height"": 40,
  ""Name"": ""Splash"",
  ""Lines"": {
    ""Details"": [
      {
        ""Orientation"": 1,
        ""Colour"": 0,
        ""Location"": {
          ""X"": 10,
          ""Y"": 20
        },
        ""Length"": 0
      },
      {
        ""Orientation"": 1,
        ""Colour"": 1,
        ""Location"": {
          ""X"": 44,
          ""Y"": 55
        },
        ""Length"": 100
      }
    ]
  },
  ""Labels"": {
  ""Details"": [
  {
          ""Foreground"": 10,
          ""Background"": 0,
          ""Location"": {
            ""Location"": {
              ""X"": 14,
              ""Y"": 12
            }
          },
          ""Name"": ""lblMessage"",
          ""Text"": ""Back to the future..."",
         ""Length"": 29
        }
     
]
  },
  ""Textboxes"": {}
}
";
			
			splash = JsonConvert.DeserializeObject<ConsoleForm>(json);
			
			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(2, splash.Lines.Details.Count);
			lines = splash.Lines.Details;
			Assert.AreEqual(44, lines[1].Location.X);
			Assert.AreEqual(1, splash.Labels.Details.Count);
		}


	[Test]
		public void test4()
		{
			json = @"
{
  ""Width"": 80,
  ""Height"": 40,
  ""Name"": ""Splash"",
  ""Lines"": {
    ""Details"": [
      {
        ""Orientation"": 1,
        ""Colour"": 0,
        ""Location"": {
          ""X"": 10,
          ""Y"": 20
        },
        ""Length"": 0
      },
      {
        ""Orientation"": 1,
        ""Colour"": 1,
        ""Location"": {
          ""X"": 44,
          ""Y"": 55
        },
        ""Length"": 100
      }
    ]
  },
  ""Labels"": {
  ""Details"": [
  {
          ""Foreground"": 10,
          ""Background"": 0,
          ""Location"": {
            ""Location"": {
              ""X"": 14,
              ""Y"": 12
            }
          },
          ""Name"": ""lblMessage"",
          ""Text"": ""Back to the future..."",
         ""Length"": 29
        }
     
]
  },
  ""Textboxes"": {""Details"": [
{ ""Name"": ""txtLoginID"", ""Length"": 20, ""ForeColour"": ""DarkGreen"", ""BackColour"":""White"",
      ""Location"": { ""X"":20, ""Y"": 8 } } ]}
}
";
			
			splash = JsonConvert.DeserializeObject<ConsoleForm>(json);
			
			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(2, splash.Lines.Details.Count);
			lines = splash.Lines.Details;
			Assert.AreEqual(44, lines[1].Location.X);
			Assert.AreEqual(1, splash.Labels.Details.Count);
			
			Assert.AreEqual(ConsoleColor.DarkGreen, splash.Textboxes.Details[0].ForeColour);
		}

	[Test]
		public void test5(){
			json = ConsoleForm.getFileContents("JSON\\splash.json");
			splash = JsonConvert.DeserializeObject<ConsoleForm>(json);
			
			StringAssert.Contains("Splash", splash.Name);
			Console.Error.WriteLine(splash.Name);
			Assert.AreEqual(2, splash.Lines.Details.Count);
			lines = splash.Lines.Details;
			Assert.AreEqual(44, lines[1].Location.X);
			Assert.AreEqual(1, splash.Labels.Details.Count);
			
			Assert.AreEqual(ConsoleColor.DarkGreen, splash.Textboxes.Details[0].ForeColour);
		}
				
	}
	
}
