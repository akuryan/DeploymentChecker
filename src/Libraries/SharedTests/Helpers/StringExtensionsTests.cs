using NUnit.Framework;
using Shared.Helpers;

namespace SharedTests.Helpers
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("dobryak.org", "dobryak.org")]
        [TestCase("http://dobryak.org", "dobryak.org")]
        [TestCase("https://dobryak.org", "dobryak.org")]
        [TestCase("://dobryak.org", "dobryak.org")]
        [TestCase("https://dobryak.org/u/", "dobryak.org")]
        [TestCase("https://dobryak.org/u/?tset", "dobryak.org")]
        [TestCase("https://dobrya?k.org/u/", "dobrya")]
        [TestCase("https://dobrya^k.org/u/", null)]
        [TestCase("", "")]
        [Test, Category("StringExtensions")]
        public void CreateValidHostHeaderTests(string suppliedData, string expectedHeader)
        {
            var header = suppliedData.CreateValidHostHeader();
            Assert.AreEqual(expectedHeader, header);
        }

        [TestCase("dobryak.org", "https://dobryak.org")]
        [TestCase("dobryak.org ", "https://dobryak.org")]
        [TestCase(" dobryak.org ", "https://dobryak.org")]
        [TestCase("http://dobryak.org", "http://dobryak.org")]
        [TestCase("https://dobryak.org", "https://dobryak.org")]
        [TestCase("http://dobryak.org/test", "http://dobryak.org/test")]
        [TestCase("http://dobryak.org/test?tse", "http://dobryak.org/test")]
        [Test, Category("StringExtensions")]
        public void PrepareParceableHostNameTests(string suppliedData, string expectedResult)
        {
            var parceableHost = suppliedData.PrepareParceableHostName();
            Assert.AreEqual(expectedResult, parceableHost);
        }
    }
}
