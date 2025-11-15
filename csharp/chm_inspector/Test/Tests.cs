using System;
using System.Collections.Generic;
using NUnit.Framework;
using Utils;

namespace Tests {
    [TestFixture]
    public class ChmTests  {
        private const string file = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.chm";

        // NOTE: uncommenting the attribute break the little “green/red bullet” visual indicator in the SharpDevelop IDE.
        // [TestCase(TestName = "Urls should not throw exception")]
        [Test]
        public void test1() {
            try {
                var urls = Chm.urls_7zip(file);
                Assert.NotNull(urls, "Urls() returned null.");
                Console.WriteLine("Found {0} entries", urls.Count);
                Assert.Greater(urls.Count,0,"Expect at least one file");
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }
        [Test]
        public void test2() {
            try {
                var urls = Chm.urls_structured(file);
                Assert.NotNull(urls, "Urls() returned null.");
                Console.WriteLine("Found {0} entries", urls.Count);
                Assert.Greater(urls.Count,0,"Expect at least one file");
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }
    }
}

