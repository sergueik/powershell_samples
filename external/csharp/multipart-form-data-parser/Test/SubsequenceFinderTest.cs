using HttpMultipartParser;
using NUnit.Framework;

namespace HttpMultipartParserUnitTest {
    /// <summary>
    ///     Summary description for SubsequenceFinderUnitTest
    /// </summary>
    [TestFixture]
    public class SubsequenceFinderTest {
        [Test]
        public void SmokeTest()
        {
            var A = new byte[] {0x1, 0x2, 0x3, 0x4};
            var B = new byte[] {0x3, 0x4};

            Assert.AreEqual(SubsequenceFinder.Search(A, B, A.Length), 2);
        }
    }
}