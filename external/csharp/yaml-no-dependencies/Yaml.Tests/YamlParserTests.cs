using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Yaml.Tests {
	[TestFixture]
	public class YamlParserTests {

		[Test]
		public void EmptyValueTest() {
			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
            
			writer.WriteLine("database:");
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			using (TextReader reader = new StreamReader(stream)) {
				YamlParser parser = new YamlParser(reader, null);
				Assert.IsTrue(parser.TryParse());
			}                                    
		}

		[Test]
		public void BlockValueTest() {

			MemoryStream stream = new MemoryStream();
			StreamWriter writer = new StreamWriter(stream);
            
			writer.WriteLine("comments:");
			writer.WriteLine("  Late afternoon is best.");
			writer.WriteLine("  Backup contact is Nancy");
			writer.WriteLine("  Billsmer @ 338-4338.");

			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			using (TextReader reader = new StreamReader(stream)) {
				YamlParser parser = new YamlParser(reader, null);
				Assert.IsTrue(parser.TryParse());
			}  
		}
	}
}
