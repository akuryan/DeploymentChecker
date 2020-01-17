using System;
using NUnit.Framework;
using RobotsTxt;

namespace RobotsTxtTests
{
    [TestFixture]
    class CrawlDelayTests
    {
        private readonly string newLine = Environment.NewLine;

        [Test, Category("CrawlDelay")]
        public void CrawlDelayEmptyUserAgentThrowsArgumentException(
            [Values("", " ")] string userAgent // white space considered empty
            )
        {
            Robots r = new Robots(String.Empty);
            Assert.Throws<ArgumentException>(() => r.CrawlDelay(userAgent));
        }

        [Test, Category("CrawlDelay")]
        public void CrawlDelayNoRulesZero()
        {
            Robots r = new Robots(String.Empty);
            Assert.AreEqual(0, r.CrawlDelay("*"));
        }

        [Test, Category("CrawlDelay")]
        public void CrawlDelayNoCrawlDelayRuleZero()
        {
            string s = @"User-agent: *" + this.newLine + "Disallow: /dir/";
            Robots r = new Robots(s);
            Assert.AreEqual(0, r.CrawlDelay("*"));
        }

        [Test, Category("CrawlDelay")]
        public void CrawlDelayNoRuleForRobotZero()
        {
            string s = @"User-agent: Slurp" + this.newLine + "Crawl-delay: 2";
            Robots r = new Robots(s);
            Assert.AreEqual(0, r.CrawlDelay("Google"));
        }

        [Test, Category("CrawlDelay")]
        public void CrawlDelayInvalidRuleZero()
        {
            string s = @"User-agent: *" + this.newLine + "Crawl-delay: foo";
            Robots r = new Robots(s);
            Assert.AreEqual(0, r.CrawlDelay("Google"));
        }

        [Test, Category("CrawlDelay")]
        public void CrawlDelayRuleWithoutUserAgent()
        {
            string s = "Crawl-delay: 1";
            Robots r = Robots.Load(s);
            Assert.AreNotEqual(1000, r.CrawlDelay("Google"));
            Assert.AreEqual(0, r.CrawlDelay("Google"));
        }

        [Test, Category("CrawlDelay"), Sequential]
        public void CrawlDelayValidRule(
            [Values(2000, 2000, 500, 500)] long expected,
            [Values("Google", "google", "Slurp", "slurp")] string userAgent)
        {
            string s = @"User-agent: Google" + this.newLine + "Crawl-delay: 2" + this.newLine +
                "User-agent: Slurp" + this.newLine + "Crawl-delay: 0.5";
            Robots r = new Robots(s);
            Assert.AreEqual(expected, r.CrawlDelay(userAgent));
        }
    }
}