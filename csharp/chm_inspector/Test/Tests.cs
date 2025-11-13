using System;
using System.Collections.Generic;
using NUnit.Framework;
using Utils;

namespace Tests {
    [TestFixture]
    public class ChmTests  {
        private const string file = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.chm";

        // [TestCase(TestName = "Urls should not throw exception")]
        [Test]
        public void test1() {
            try {
                var urls = Chm.Urls(file);
                Assert.NotNull(urls, "Urls() returned null.");
                Console.WriteLine("Found {0} entries", urls.Count);
            } catch (Exception e) {
                Assert.Fail("Exception thrown: " + e.Message);
            }
        }
    }
}

