using RobotsTxt.Checker.Classes;
using Shared.Helpers;
using System;
using System.Linq;

namespace RobotsTxt.Checker.Methods
{
    public static class Checker
    {
        public static RobotsTxtReport CheckRobotsTxt(string url, Options providedOptions)
        {
            var robotsTxtHost = string.IsNullOrWhiteSpace(providedOptions.ServerHostName) ? url : providedOptions.ServerHostName;
            Uri robotsTxtUri = RobotsTxtUri(robotsTxtHost);
            bool robotsTxtExists;
            var robotsContent = string.Empty;

            try
            {
                robotsContent = string.IsNullOrWhiteSpace(providedOptions.ServerHostName) ? Helpers.NetworkHelper.GetString(robotsTxtUri) : Helpers.NetworkHelper.GetString(robotsTxtUri, url);
                //something was downloaded, and we need to check, if it is not empty now
                robotsTxtExists = !string.IsNullOrWhiteSpace(robotsContent);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception received when trying to download {robotsTxtUri.ToString()}");
                Console.WriteLine($"Exception: {exception.Message}");
                Console.WriteLine($"Exception stracktrace {exception.StackTrace}");
                robotsTxtExists = false;
            }

            var robotsFile = robotsTxtExists ? Robots.Load(robotsContent) : Robots.Load(string.Empty);
            var getCheckStatus = GetCheckStatus(robotsFile, providedOptions.CrawlingDenied);
            var sitemapsIsAccessible = CheckSitemapIsAccessible(robotsFile);
            if (HaveSitemaps(robotsFile))
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

        /// <summary>
        /// According to https://support.google.com/webmasters/answer/6062596?hl=en - robots.txt must be located only at root of web app
        /// </summary>
        /// <param name="suppliedHost"></param>
        /// <returns></returns>
        private static Uri RobotsTxtUri(string suppliedHost)
        {
            Uri parsedUri = new Uri(suppliedHost.PrepareParceableUri());

            if (parsedUri == null)
            {
                Console.WriteLine($"Could not parse {suppliedHost} as a hostname.");
                return null;
            }

            var robotsTxtUrl = string.Concat(parsedUri.Scheme, "://", parsedUri.DnsSafeHost, "/", Constants.RobotsTxtName);
            return new Uri(robotsTxtUrl, UriKind.Absolute);
        }

        private static bool HaveSitemaps(Robots robotsFile)
        {
            if (robotsFile.Sitemaps != null)
            {
                return robotsFile.Sitemaps.Any();
            }

            return false;
        }

        private static bool CheckSitemapIsAccessible(Robots robotsFile)
        {
            var isAccessible = false;
            if (HaveSitemaps(robotsFile))
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
