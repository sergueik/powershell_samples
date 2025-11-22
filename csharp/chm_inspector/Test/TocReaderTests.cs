using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Utils;
using System.Net;

namespace Tests {
	[TestFixture]
	public class TocReaderTests {
		private const string file = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.chm";
		// see also https://github.com/serilog-contrib/serilog-sinks-elasticsearch/blob/dev/sample/Serilog.Sinks.Elasticsearch.Sample/Program.cs

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
