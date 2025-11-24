using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Utils;

namespace Tests {
    [TestFixture]
    public class ChmTest  {
    	private const string fileName ="api.chm";
    	private string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName );

        // NOTE: uncommenting the attribute break the little “green/red bullet” visual indicator in the SharpDevelop IDE.
        // [TestCase(TestName = "Urls should not throw exception")]
        [Test]
        public void test1() {
            try {
                var urls = Chm.urls_7zip(file);
                Assert.NotNull(urls, "Urls() returned null.");
                Assert.Greater(urls.Count,0,"Expect at least one file");
                Console.WriteLine("Found {0} entries", urls.Count);
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }

		[Test]
        public void test2() {
            try {
                var urls = Chm.urls_7zip_alt(file);
                Assert.NotNull(urls, "Urls() returned null.");
                Assert.Greater(urls.Count,0,"Expect at least one file");
                Console.WriteLine("Found {0} entries", urls.Count);
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }

        [Test]
        public void test3() {
            try {
                var urls = Chm.urls_structured(file);
                Assert.NotNull(urls, "Urls() returned null.");
                Assert.Greater(urls.Count,0,"Expect at least one file");
                Console.WriteLine("Found {0} entries", urls.Count);
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }

        [Test]
        public void test4() {
            try {
                var title = Chm.title(file);
                Assert.NotNull(title, "title() should not be returning null.");
                // Assert.Greater(urls.Count,0,"Expect at least one file");
                Console.WriteLine("Found title: {0}", title);
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }
    }
}

