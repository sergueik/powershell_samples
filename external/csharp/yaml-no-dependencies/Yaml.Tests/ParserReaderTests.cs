using System;
using System.Collections.Generic;
using System.Text;
using Yaml;
using NUnit.Framework;
using System.IO;

namespace Yaml.Tests
{
    [TestFixture]
    public class ParserReaderTests
    {

        [Test,ExpectedException(typeof(InvalidOperationException),ExpectedMessage="Cannot create more then 32 marks deep.")]
        public void MarkDepthTest()
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            
            writer.Write("hi pals");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                
                ParserReader parser = new ParserReader(reader);
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();//5
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();//10
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();//15
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();//20
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();//25
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();
                parser.Mark();//30
                parser.Mark();
                parser.Mark();//32 we should get here and throw on the next mark.
                parser.Mark();//
            }
        }

        [Test]
        public void MarkTest()
        {

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            
            writer.Write("hi pals");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                
                ParserReader parser = new ParserReader(reader);
                Assert.AreEqual('h', (char)parser.GetCurrent(), "MarkTest#100");
                parser.Mark(); //mark a save point
                Assert.AreEqual('h', (char)parser.Read(), "MarkTest#200");
                Assert.AreEqual('i', (char)parser.Read(), "MarkTest#300");
                Assert.AreEqual(' ', (char)parser.Read(), "MarkTest#400");
                Assert.AreEqual('p', (char)parser.GetCurrent(), "MarkTest#500");
                Assert.AreEqual('p', (char)parser.Read(), "MarkTest#550");
                Assert.AreEqual("hi p", parser.GetString(), "MarkTest#575");
                parser.Reset(); // reset to the last save point
                Assert.AreEqual('h', (char)parser.GetCurrent(), "MarkTest#600");
                parser.Mark();
                Assert.AreEqual('h', (char)parser.GetCurrent(), "MarkTest#700");
                parser.Unmark();
                Assert.AreEqual('h', (char)parser.GetCurrent(), "MarkTest#800");
                Assert.AreEqual('h', (char)parser.Read(), "MarkTest#900");
                Assert.AreEqual('i', (char)parser.Read(), "MarkTest#925");
                Assert.AreEqual(' ', (char)parser.GetCurrent(), "MarkTest#950");
                Assert.AreEqual('h', (char)parser.GetPrevious(), "MarkTest#1000");
            }

        }

        [Test]
        public void EmptyStreamTests()
        {
            MemoryStream stream = new MemoryStream();

            using (TextReader reader = new StreamReader(stream))
            {
                ParserReader parser = new ParserReader(reader);
                Assert.AreEqual(-1, parser.Read(), "EmptyStreamTests#100");
            }
                
        }

        [Test]
        public void GetStringLargerThenBuffer()
        { 
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            
            writer.Write("hi pals");
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            using (TextReader reader = new StreamReader(stream))
            {
                ParserReader parser = new ParserReader(reader,5);
                parser.Mark();
                Assert.AreEqual('h',(char)parser.Read(), "GetStringLargerThenBuffer#100");
                Assert.AreEqual('i', (char)parser.Read(), "GetStringLargerThenBuffer#200");
                Assert.AreEqual(' ', (char)parser.Read(), "GetStringLargerThenBuffer#300");
                Assert.AreEqual('p', (char)parser.Read(), "GetStringLargerThenBuffer#400");
                Assert.AreEqual('a', (char)parser.Read(), "GetStringLargerThenBuffer#500");
                Assert.AreEqual('l', (char)parser.Read(), "GetStringLargerThenBuffer#600");
                Assert.AreEqual('s', (char)parser.Read(), "GetStringLargerThenBuffer#700");
                Assert.AreEqual("hi pals", parser.GetString(), "GetStringLargerThenBuffer#800");
                parser.Unmark();
            }
        }
    }
}
