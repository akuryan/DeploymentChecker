using NUnit.Framework;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedTests.Helpers
{
    public class StringExtensionsTests
    {
        [TestCase("dobryak.org", "dobryak.org")]
        [TestCase("http://dobryak.org", "dobryak.org")]
        [TestCase("https://dobryak.org", "dobryak.org")]
        [TestCase("://dobryak.org", "dobryak.org")]
        [TestCase("https://dobryak.org/u/", "dobryak.org")]
        [TestCase("https://dobryak.org/u/?tset", "dobryak.org")]
        [TestCase("https://dobrya?k.org/u/", null)]
        [Test, Category("StringExtensions")]
        public void CreateValidHostHeaderTests(string suppliedData, string expectedHeader)
        {
            var header = suppliedData.CreateValidHostHeader();
            Assert.AreEqual(header, expectedHeader);
        }
    }
}
