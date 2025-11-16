using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Utils;

namespace Tests {
	[TestFixture]
	public class TocReaderTests {
		private const string file = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.chm";

		[Test]
		public void test1()
		{
			var entries = Chm.toc_structured(file);

            // Assert
            Assert.IsNotNull(entries, "The entries list should not be null");
            Assert.IsNotEmpty(entries, "The entries list should not be empty");

            foreach (var entry in entries) {
                Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
                Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
            }
		}

		[Test]
		public void test2() {
			Dictionary<string,string> toc = Chm.toc_7zip(file);

			// Assert
            Assert.IsNotNull(toc, "The dictionary should not be null");
            Assert.IsNotEmpty(toc, "The dictionary should not be empty");

            foreach (var kvp in toc) {
                Assert.IsFalse(string.IsNullOrEmpty(kvp.Key), "Key (Name) should not be null or empty");
                Assert.IsFalse(string.IsNullOrEmpty(kvp.Value), "Value (Local) should not be null or empty");
                Console.Error.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            }		
		}
	}
}