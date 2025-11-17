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
		public void test1() {
			var toclist = Chm.toc_structured(file);

			// Assert
			Assert.IsNotNull(toclist, "The toclist list should not be null");
			Assert.IsNotEmpty(toclist, "The toclist list should not be empty");

			foreach (var entry in toclist) {
			    Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
			    Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
			    Console.Error.WriteLine("{0}: {1}", entry.Name, entry.Local);
            }
		}

		[Test]
		public void test2() {
			Dictionary<string,string> toc = Chm.tocdict_7zip(file);

			// Assert
			Assert.IsNotNull(toc, "The dictionary should not be null");
			Assert.IsNotEmpty(toc, "The dictionary should not be empty");
			
			foreach (var keyValuePair in toc) {
			    Assert.IsFalse(string.IsNullOrEmpty(keyValuePair.Key), "Key (Name) should not be null or empty");
			    Assert.IsFalse(string.IsNullOrEmpty(keyValuePair.Value), "Value (Local) should not be null or empty");
			    Console.Error.WriteLine("{0}: {1}", keyValuePair.Key, keyValuePair.Value);
			}		
		}
		[Test]
		public void test3() {
			var toclist = Chm.toc_7zip(file);
			
			// Assert
			Assert.IsNotNull(toclist, "The toclist list should not be null");
			Assert.IsNotEmpty(toclist, "The toclist list should not be empty");
			
			foreach (var entry in toclist) {
			    Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
			    Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
			    Console.Error.WriteLine("{0}: {1}", entry.Name, entry.Local);
			}
		}
	}
}