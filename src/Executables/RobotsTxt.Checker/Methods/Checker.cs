using RobotsTxt.Checker.Classes;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace RobotsTxt.Checker.Methods
{
    public static class Checker
    {
        public static RobotsTxtReport CheckRobotsTxt(string url, bool isNoAllowRuleExpected)
        {
            var siteUrl = url.Contains('?') ? url.Split('?')[0] : url;
            siteUrl = siteUrl.Trim();
            Uri uriResult = ResultingUri(siteUrl, Constants.RobotsTxtName);
            bool robotsTxtExists;
            var robotsContent = string.Empty;

            try
            {
                robotsContent = Helpers.NetworkHelper.GetString(uriResult);
                //something was downloaded, and we need to check, if it is not empty now
                robotsTxtExists = !string.IsNullOrWhiteSpace(robotsContent);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception received when trying to download {uriResult}");
                Console.WriteLine($"Exception: {exception.Message}");
                Console.WriteLine($"Exception stracktrace {exception.StackTrace}");
                robotsTxtExists = false;
            }

            var robotsFile = robotsTxtExists ? Robots.Load(robotsContent) : Robots.Load(string.Empty);
            var getCheckStatus = GetCheckStatus(robotsFile, isNoAllowRuleExpected);
            var sitemapsIsAccessible = CheckSitemapIsAccessible(robotsFile);
            if (robotsFile.Sitemaps.Any())
            {
                //if robots.txt check is OK, but sitemaps, defined in it is not accessible - we shall fail the check
                getCheckStatus = getCheckStatus ? sitemapsIsAccessible : getCheckStatus;
            }

            var report = new RobotsTxtReport
            {
                Url = url,
                CheckStatus = getCheckStatus,
                RobotsTxtExists = robotsTxtExists,
                SitemapsIsAccessible = sitemapsIsAccessible,
                Robots = robotsFile
            };
            return report;
        }

        //decision making on robots.txt
        private static bool GetCheckStatus(Robots robotsFile, bool isNoAllowRuleExpected)
        {
            bool checkStatus;
            if (!robotsFile.Malformed && robotsFile.HasRules && robotsFile.HaveNoAllowRules && isNoAllowRuleExpected)
            {
                //if robots.txt exist, formed correctly, have some rules and our expectation that it have no allow rules is fulfilled - we are ok
                checkStatus = true;
            }
            else if (!robotsFile.Malformed && robotsFile.HasRules && !robotsFile.HaveNoAllowRules && !isNoAllowRuleExpected)
            {
                //if robots.txt exist, formed correctly, have some rules and there is allow rules present
                checkStatus = true;
            }
            else
            {
                checkStatus = false;
            }
            return checkStatus;
        }

        private static Uri ResultingUri(string urlName, string append)
        {
            Uri uriResult;
            if (!urlName.Trim().StartsWith("http"))
            {
                Console.WriteLine($"url {urlName} does not start with http or https.");
                Uri.TryCreate(UriName(string.Concat("https://", urlName), append), UriKind.Absolute, out uriResult);
            }
            else
            {
                Uri.TryCreate(UriName(urlName, append), UriKind.Absolute, out uriResult);
            }

            return uriResult;
        }

        private static string UriName(string siteUrlToTest, string append)
        {
            return siteUrlToTest.EndsWith("/", StringComparison.InvariantCultureIgnoreCase) ? string.Concat(siteUrlToTest, append) : string.Concat(siteUrlToTest, "/", append);
        }

        private static bool CheckSitemapIsAccessible(Robots robotsFile)
        {
            var isAccessible = false;
            if (robotsFile.Sitemaps != null && robotsFile.Sitemaps.Any())
            {
                foreach (Sitemap sitemapUrl in robotsFile.Sitemaps)
                {
                    var sitemapContent = string.Empty;
                    //if at least one of sitemaps provided is invalid - let us invalidate all
                    try
                    {
                        sitemapContent = Helpers.NetworkHelper.GetString(sitemapUrl.Url);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Exception received when trying to download {sitemapUrl.Url}");
                        Console.WriteLine($"Exception: {exception.Message}");
                        Console.WriteLine($"Exception stracktrace {exception.StackTrace}");
                    }
                    isAccessible = !string.IsNullOrWhiteSpace(sitemapContent);
                }
            }
            return isAccessible;
        }
    }
}
