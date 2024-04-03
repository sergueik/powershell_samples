using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Yaml.Tests
{
    [TestFixture]
    public class YamlParserTests{
    	[Ignore]
        [Test]
        public void EmptyValueTest()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            
            writer.WriteLine("database:");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                YamlParser parser = new YamlParser(reader, null);
                Assert.IsTrue(parser.TryParse());
            }                                    
        }
        [Ignore]
        [Test]
        public void BlockValueTest()
        {

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            
            writer.WriteLine("comments:");
            writer.WriteLine("  Late afternoon is best.");
            writer.WriteLine("  Backup contact is Nancy");
            writer.WriteLine("  Billsmer @ 338-4338.");

            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                YamlParser parser = new YamlParser(reader, null);
                Assert.IsTrue(parser.TryParse());
            }  
        }

        [Test]
        public void ParseArrayTest1()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            string expected = "               ";
            writer.WriteLine(expected);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                YamlParser parser = new YamlParser(reader, null);
                Assert.AreEqual(expected,parser.ParseArray(YamlCharacterType.Space));
            }
        }

        [Test]
        public void ParseArrayTest2()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine(".               ");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                YamlParser parser = new YamlParser(reader, null);
                Assert.IsNull(parser.ParseArray(YamlCharacterType.Space));
            }
        }


        [Test]
        public void ParseIndentTest1()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("               ");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                YamlParser parser = new YamlParser(reader, null);
                Assert.AreEqual(15, parser.ParseIndent(),"ParseIndentTest1#100");
                Assert.AreEqual(15, parser.ParseIndent(), "ParseIndentTest1#200");
            }
        }

        [Test]
        public void ParseIndentTest2()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("               ");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                YamlParser parser = new YamlParser(reader, null);
                Assert.IsTrue(parser.ParseIndent(15),"ParseIndentTest2#100");
                Assert.AreEqual(0, parser.ParseIndent(), "ParseIndentTest2#200");
            }
        }

        [Test]
        public void ParseIndentTest3()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("               ");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                YamlParser parser = new YamlParser(reader, null);
                Assert.IsFalse(parser.ParseIndent(5), "ParseIndentTest3#100");
                Assert.IsFalse(parser.ParseIndent(10), "ParseIndentTest3#200");
                Assert.IsTrue(parser.ParseIndent(15), "ParseIndentTest3#300");
            }
        }




    }
}
