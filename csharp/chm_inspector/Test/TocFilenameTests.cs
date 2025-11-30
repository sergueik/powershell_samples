using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Utils;

namespace Tests {
    [TestFixture]
    public class TocFilenameTests  {
    	private const string fileName ="api.chm";
    	private string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName );

       [Test]
        public void test1() {
            try {
                var tocFilename = Chm.tocfilename_7zip(file);
                Assert.NotNull(tocFilename, "tocfilename_7zip() should not be returning null.");
                // Assert.Greater(urls.Count,0,"Expect at least one file");
                Console.WriteLine("Found tocFilename: {0}", tocFilename);
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }

        [Test]
        public void test2() {
            try {
                var tocFilename = Chm.tocfilename_structured(file);
                Assert.NotNull(tocFilename, "tocfilename_structured() should not be returning null.");
                // Assert.Greater(urls.Count,0,"Expect at least one file");
                Console.WriteLine("Found tocFilename: {0}", tocFilename);
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }
    }
}

