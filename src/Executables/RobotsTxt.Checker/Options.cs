using CommandLine;
using System.Collections.Generic;

namespace RobotsTxt.Checker
{
    class Options
    {
        [Option(Required = true, HelpText = "Please, provide comma-separated list of web app root URLs, where robots.txt shall be checked. Example: https://www.google.com/, https://www.gmail.com/")]
        public IEnumerable<string> WebAppUrls { get; set; }

        [Option(Default = false, HelpText = "Define, if robots.txt shall deny all crawling of web app. Default value is false, e.g. crawling shall be allowed.")]
        public bool CrawlingDenied { get; set; }
    }
}
