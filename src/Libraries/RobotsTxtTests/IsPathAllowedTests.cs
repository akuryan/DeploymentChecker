using System;
using NUnit.Framework;
using RobotsTxt;

namespace RobotsTxtTests
{
    [TestFixture]
    class IsPathAllowedTests
    {
        private readonly string newLine = Environment.NewLine;

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedEmptyUserAgentThrowsArgumentException(
            [Values("", " ")]string userAgent, // white space considered empty
            [Values("")]string path)
        {
            string s = "User-agent: *" + this.newLine + "Disallow: /";
            Robots r = new Robots(s);
            Assert.Throws<ArgumentException>(() => r.IsPathAllowed(userAgent, path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedRuleWithoutUserAgentTrue()
        {
            string s = "Disallow: /";
            Robots r = Robots.Load(s);
            Assert.True(r.IsPathAllowed("*", "/foo"));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedWithoutRulesTrue(
            [Values("*", "some robot")] string userAgent,
            [Values("", "/", "/file.html", "/directory/")] string path)
        {
            Robots r = new Robots(String.Empty);
            Assert.True(r.IsPathAllowed(userAgent, path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedWithoutAccessRuleTrue(
            [Values("*", "some robot")] string userAgent,
            [Values("", "/", "/file.html", "/directory/")] string path)
        {
            string s = "User-agent: *" + this.newLine + "Crawl-delay: 5";
            Robots r = new Robots(s);
            Assert.True(r.IsPathAllowed(userAgent, path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedNoRulesForRobotTrue(
            [Values("", "/", "/file.html", "/directory/")] string path)
        {
            string s = "User-agent: Slurp" + this.newLine + "Disallow: /";
            Robots r = new Robots(s);
            Assert.True(r.IsPathAllowed("some robot", path));
        }

        [Test, Category("IsPathAllowed"), Description("User-agent string match should be case-insensitive.")]
        public void IsPathAllowedNoGlobalRulesFalse(
            [Values("Slurp", "slurp", "Exabot", "exabot")] string userAgent,
            [Values("", "/", "/file.html", "/directory/")] string path)
        {
            string s = "User-agent: Slurp" + this.newLine + "Disallow: /" + this.newLine + "User-agent: Exabot" + this.newLine + "Disallow: /";
            Robots r = new Robots(s);
            Assert.False(r.IsPathAllowed(userAgent, path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedUserAgentStringCaseInsensitiveFalse(
            [Values("Slurp", "slurp", "Exabot", "exabot", "FigTree/0.1 Robot libwww-perl/5.04")] string userAgent)
        {
            string s = 
@"User-agent: Slurp
Disallow: /
User-agent: Exabot
Disallow: /
User-agent: Exabot
Disallow: /
User-agent: figtree
Disallow: /";
            Robots r = Robots.Load(s);
            Assert.False(r.IsPathAllowed(userAgent, "/dir"));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowed_OnlyDisallow_False(
            [Values("/help", "/help.ext", "/help/", "/help/file.ext", "/help/dir/", "/help/dir/file.ext")] string path)
        {
            string s = @"User-agent: *" + this.newLine + "Disallow: /help";
            Robots r = new Robots(s);
            Assert.False(r.IsPathAllowed("*", path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedAllowAndDisallowTrue(
            [Values("foo", "/dir/file.ext", "/dir/file.ext1")]string path)
        {
            string s = @"User-agent: *" + this.newLine + "Allow: /dir/file.ext" + this.newLine + "Disallow: /dir/";
            Robots r = new Robots(s);
            Assert.True(r.IsPathAllowed("*", path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedAllowAndDisallowFalse(
            [Values("/dir/file2.ext", "/dir/", "/dir/dir/")] string path)
        {
            string s = @"User-agent: *" + this.newLine + "Allow: /dir/file.ext" + this.newLine + "Disallow: /dir/";
            Robots r = new Robots(s);
            Assert.False(r.IsPathAllowed("*", path));
        }

        [Test, Category("IsPathAllowed"), Sequential]
        public void IsPathAllowedPathShouldBeCaseSensitiveTrue(
            [Values("/dir/file.ext", "/dir/file.ext", "/*/file.html", "/*.gif$")] string rule,
            [Values("/dir/File.ext", "/Dir/file.ext", "/a/File.html", "a.GIF")] string path)
        {
            string s = @"User-agent: *" + this.newLine + "Disallow: " + rule;
            Robots r = Robots.Load(s);
            Assert.True(r.IsPathAllowed("*", path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedDollarWildcardTrue(
            [Values("asd", "a.gifa", "a.gif$")] string path)
        {
            string s = @"User-agent: *" + this.newLine + "Disallow: /*.gif$";
            Robots r = Robots.Load(s);
            Assert.True(r.IsPathAllowed("*", path));
        }

        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedDollarWildcardFalse(
            [Values("a.gif", "foo.gif", "b.a.gif", "a.gif.gif")]string path)
        {
            string s = @"User-agent: *" + this.newLine + "Disallow: /*.gif$";
            Robots r = Robots.Load(s);
            Assert.False(r.IsPathAllowed("*", path));
        }

        [TestCase("/*/file.html", "/foo/", true)]
        [TestCase("/*/file.html", "file.html", true)]
        [TestCase("/*/file.html", "/foo/file2.html", true)]
        [TestCase("/*/file.html", "/a/file.html", false)]
        [TestCase("/*/file.html", "/dir/file.html", false)]
        [TestCase("/*/file.html", "//a//file.html", false, Description = "The path should be normalized to \"/a/file.html\"")]
        [TestCase("/*/file.html", "/a/a/file.html", false)]
        [TestCase("/*/file.html", "/a/a/file.htmlz", false)]
        [TestCase("/*/file.html", "///f.html", true, Description = "The path should be normalized to \"/f.html\"")]
        [TestCase("/*/file.html", "/\\/f.html", true)]
        [TestCase("/*/file.html", "/:/f.html", true)]
        [TestCase("/*/file.html", "/*/f.html", true)]
        [TestCase("/*/file.html", "/?/f.html", true)]
        [TestCase("/*/file.html", "/\"/f.html", true)]
        [TestCase("/*/file.html", "/</f.html", true)]
        [TestCase("/*/file.html", "/>/f.html", true)]
        [TestCase("/*/file.html", "/|/f.html", true)]
        [TestCase("/private*/", "/private/", false)]
        [TestCase("/private*/", "/Private/", true)]
        [TestCase("/private*/", "/private/f.html", false)]
        [TestCase("/private*/", "/private/dir/", false)]
        [TestCase("/private*/", "/private/dir/f.html", false)]
        [TestCase("/private*/", "/private1/", false)]
        [TestCase("/private*/", "/Private1/", true)]
        [TestCase("/private*/", "/private1/f.html", false)]
        [TestCase("/private*/", "/private1/dir/", false)]
        [TestCase("/private*/", "/private1/dir/f.html", false)]
        [TestCase("*/private/", "/private/dir/f.html", false)]
        [Test, Category("IsPathAllowed")]
        public void IsPathAllowedStarWildcard(string rule, string path, bool result)
        {
            string s = @"User-agent: *" + this.newLine + "Disallow: " + rule;
            Robots r = Robots.Load(s);
            r.IsPathAllowed("*", path);
            Assert.AreEqual(result, r.IsPathAllowed("*", path));
        }
    }
}