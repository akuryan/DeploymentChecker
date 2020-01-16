using CommandLine;
using System.Collections.Generic;

namespace RobotsTxt.Checker.Classes
{
    public class Options
    {
        [Option(Required = true, HelpText = "Please, provide comma or space -separated list of web app root URLs, where robots.txt shall be checked. If protocol (http or https) is not provided - I am assuming it is https. Example: https://www.google.com/, https://www.gmail.com/", Separator = ',')]
        public IEnumerable<string> WebAppUrls { get; set; }

        [Option(Default = false, HelpText = "Define, if robots.txt shall deny all crawling of web app. Default value is false, e.g. crawling shall be allowed.")]
        public bool CrawlingDenied { get; set; }
    }
}
