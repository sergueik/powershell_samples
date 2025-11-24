using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;

using Utils;
using TestUtils;

namespace Tests {

	[TestFixture]
	public class TocMatchesTest {
		private List<TocEntry> tokens = new List<TocEntry>();
		private String payload;
		private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "toc.hhc");

		[Test]
		public void test2() {
					// Check for <OBJECT type="text/sitemap"> to confirm it’s a TOC.

	        using (var reader = new StreamReader(filePath)) {
            	// Read the entire stream as a string
	            payload = reader.ReadToEnd();
	        }
					StringAssert.Contains(@"<OBJECT type=""text/sitemap"">", payload, "Expect tag to to confirm it’s a TOC");
		}
		[Ignore]
		[Test]
		[Timeout(120000)]
		// NOTE: may like to upgrade to Nunit 3.x while still under Sharp Develop 5.1
		public void test1() {

			// Open the text file using a StreamReader
	        using (var reader = new StreamReader(filePath)) {
            	// Read the entire stream as a string
	            payload = reader.ReadToEnd();
	        }
			tokens = Chm.parseToc( payload);
			Assert.NotNull(tokens);
			Assert.Greater(1,tokens.Count);
            foreach (var entry in tokens) {
                Assert.IsFalse(string.IsNullOrEmpty(entry.Name), "Entry Name should not be null or empty");
                Assert.IsFalse(string.IsNullOrEmpty(entry.Local), "Entry Local should not be null or empty");
                Console.Error.WriteLine("{0}: {1}", entry.Name, entry.Local);
            }
		}
	}


}
