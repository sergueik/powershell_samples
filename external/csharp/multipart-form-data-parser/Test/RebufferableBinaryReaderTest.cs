﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using HttpMultipartParser;
using NUnit.Framework;

namespace HttpMultipartParserUnitTest {
	[TestFixture]
    public class RebufferableBinaryReaderTest {
         #region Read() Tests

        [Test]
        public void CanReadSingleCharacterBuffer() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("abc"), Encoding.UTF8);

            Assert.AreEqual(reader.Read(), 'a');
            Assert.AreEqual(reader.Read(), 'b');
            Assert.AreEqual(reader.Read(), 'c');
        }

        [Test]
        public void CanReadSingleCharacterOverBuffers() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("def"), Encoding.UTF8);
            reader.Buffer(TestUtil.StringToByteNoBom("abc"));

            Assert.AreEqual(reader.Read(), 'a');
            Assert.AreEqual(reader.Read(), 'b');
            Assert.AreEqual(reader.Read(), 'c');
            Assert.AreEqual(reader.Read(), 'd');
            Assert.AreEqual(reader.Read(), 'e');
            Assert.AreEqual(reader.Read(), 'f');
        }

        [Test]
        public void CanReadMixedAsciiAndUTFCharacters() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("abcdèfg"), Encoding.UTF8);

            Assert.AreEqual(reader.Read(), 'a');
            Assert.AreEqual(reader.Read(), 'b');
            Assert.AreEqual(reader.Read(), 'c');
            Assert.AreEqual(reader.Read(), 'd');
            Assert.AreEqual(reader.Read(), 'è');
            Assert.AreEqual(reader.Read(), 'f');
            Assert.AreEqual(reader.Read(), 'g');
        }

        [Test]
        public void CanReadMixedAsciiAndUTFCharactersOverBuffers()
        {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("dèfg"), Encoding.UTF8);
            reader.Buffer(TestUtil.StringToByteNoBom("abc"));

            Assert.AreEqual(reader.Read(), 'a');
            Assert.AreEqual(reader.Read(), 'b');
            Assert.AreEqual(reader.Read(), 'c');
            Assert.AreEqual(reader.Read(), 'd');
            Assert.AreEqual(reader.Read(), 'è');
            Assert.AreEqual(reader.Read(), 'f');
            Assert.AreEqual(reader.Read(), 'g');
        }

        #endregion

        #region Read(buffer, index, count) Tests

        [Test]
        public void CanReadSingleBuffer() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("6chars"), Encoding.UTF8);

            var buffer = new byte[Encoding.UTF8.GetByteCount("6chars")];
            reader.Read(buffer, 0, buffer.Length);
            string result = Encoding.UTF8.GetString(buffer);
            Assert.AreEqual(result, "6chars");
        }

        [Test]
        public void CanReadAcrossMultipleBuffers() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("ars"), Encoding.UTF8);
            reader.Buffer(TestUtil.StringToByteNoBom("6ch"));

            var buffer = new byte[6];
            reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "6chars");
        }

        [Test]
        public void CanReadMixedAsciiAndUTF8() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("5èats"), Encoding.UTF8);

            var buffer = new byte[Encoding.UTF8.GetByteCount("5èats")];
            reader.Read(buffer, 0, buffer.Length);
            string result = Encoding.UTF8.GetString(buffer);
            Assert.AreEqual(result, "5èats");
        }

        [Test]
        public void CanReadMixedAsciiAndUTF8AcrossMultipleBuffers() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("ts"), Encoding.UTF8);
            reader.Buffer(TestUtil.StringToByteNoBom(("5èa")));

            var buffer = new byte[Encoding.UTF8.GetByteCount("5èats")];
            reader.Read(buffer, 0, buffer.Length);
            string result = Encoding.UTF8.GetString(buffer);
            Assert.AreEqual(result, "5èats");
        }

        [Test]
        public void ReadCorrectlyHandlesSmallerBufferThenStream() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("6chars"), Encoding.UTF8);

            var buffer = new byte[4];
            reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "6cha");

            buffer = new byte[2];
            reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "rs");
        }

        [Test]
        public void ReadCorrectlyHandlesLargerBufferThenStream() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("6chars"), Encoding.UTF8);

            var buffer = new byte[10];
            int amountRead = reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "6chars\0\0\0\0");
            Assert.AreEqual(amountRead, 6);
        }

        [Test]
        public void ReadReturnsZeroOnNoData() {
            var reader = new RebufferableBinaryReader(new MemoryStream(), Encoding.UTF8);

            var buffer = new byte[6];
            int amountRead = reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "\0\0\0\0\0\0");
            Assert.AreEqual(amountRead, 0);
        }

        [Test]
        public void ReadLineReturnsNullOnNoData() {
            var reader = new RebufferableBinaryReader(new MemoryStream(new byte[6]), Encoding.UTF8);

            var s = reader.ReadLine();
            Assert.AreEqual(s, "\0\0\0\0\0\0");
            Assert.Null(reader.ReadLine());
        }

        [Test]
        public void ReadCanResumeInterruptedStream() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("6chars"), Encoding.UTF8);

            var buffer = new byte[4];
            int amountRead = reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "6cha");
            Assert.AreEqual(amountRead, 4);

            reader.Buffer(TestUtil.StringToByteNoBom("14intermission"));
            buffer = new byte[14];
            amountRead = reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "14intermission");
            Assert.AreEqual(amountRead, 14);

            buffer = new byte[2];
            amountRead = reader.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(Encoding.UTF8.GetString(buffer), "rs");
            Assert.AreEqual(amountRead, 2);
        }

        #endregion

        #region ReadByteLine() Tests

        [Test]
        public void CanReadByteLineOnMixedAsciiAndUTF8Text() {
            var reader = new RebufferableBinaryReader(TestUtil.StringToStreamNoBom("Bonjour poignée"), Encoding.UTF8);
            byte[] bytes = reader.ReadByteLine();
            var expected = new byte[] {66, 111, 110, 106, 111, 117, 114, 32, 112, 111, 105, 103, 110, 195, 169, 101};

            foreach (var pair in expected.Zip(bytes, Tuple.Create))
            {
                Assert.AreEqual(pair.Item1, pair.Item2);
            }
        }

        #endregion
    }
}