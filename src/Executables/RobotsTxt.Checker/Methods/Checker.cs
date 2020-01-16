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
        public static RobotsTxtReport CheckRobotsTxt(string url, bool isNoAllowRuleExpected, bool validateSitemaps)
        {
            var report = new RobotsTxtReport();
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
            //robotsFile.AllowRuleImplementation = AllowRuleImplementation.AllowOverrides;

            report.Url = url;
            report.CheckStatus = GetCheckStatus(robotsFile, isNoAllowRuleExpected);
            report.RobotsTxtExists = robotsTxtExists;
            report.SitemapsIsValid = validateSitemaps ? CheckSitemapIsValid(robotsFile) : false;
            report.Robots = robotsFile;
            return report;
        }

        //decision making on robots.txt
        private static bool GetCheckStatus(Robots robotsFile, bool isNoAllowRuleExpected)
        {
            bool checkStatus;
            if (!robotsFile.Malformed && robotsFile.HasRules && (robotsFile.HaveNoAllowRules && isNoAllowRuleExpected))
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

        private static bool CheckSitemapIsValid(Robots robotsFile)
        {
            var sitemapIsValid = false;
            if (robotsFile.Sitemaps != null && robotsFile.Sitemaps.Any())
            {
                foreach (Sitemap sitemapUrl in robotsFile.Sitemaps)
                {
                    //if at least one of sitemaps provided is invalid - let us invalidate all
                    sitemapIsValid = ValidateSitemap(sitemapUrl.Url);
                }
            }
            return sitemapIsValid;
        }

        private static bool ValidateSitemap(Uri sitemapUri)
        {
            string sitemapXml;
            string sitemapXsd;
            //let us download
            using (var client = new WebClient())
            {
                try
                {
                    sitemapXml = client.DownloadString(sitemapUri);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Exception received when trying to download sitemap {sitemapUri}");
                    Console.WriteLine($"Exception: {exception.Message}");
                    Console.WriteLine($"Exception stracktrace {exception.StackTrace}");
                    return false;
                }
                var sitemapXsdUrl = string.Concat(Constants.SitemapSchemaUrl, '/', Constants.SitemapSchemaBackupName);
                try
                {
                    //check if we could download Sitemap Schema
                    sitemapXsd = client.DownloadString(sitemapXsdUrl);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Exception received when trying to download XSD from {sitemapXsdUrl}");
                    Console.WriteLine($"Exception: {exception.Message}");
                    Console.WriteLine($"Exception stracktrace {exception.StackTrace}");
                    Console.WriteLine($"Will proceed on backup XSD");
                    sitemapXsd = LoadBackupXsd();
                }
            }
            bool errors = false;
            try
            {
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add("http://www.sitemaps.org/schemas/sitemap/0.9", XmlReader.Create(new StringReader(sitemapXsd)));
                XDocument doc = XDocument.Parse(sitemapXml);
                doc.Validate(schemas, (o, e) =>
                {
                    Console.WriteLine(e.Message);
                    errors = true;
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception received when trying to validate sitemap {sitemapUri}");
                Console.WriteLine($"Exception: {exception.Message}");
                Console.WriteLine($"Exception stracktrace {exception.StackTrace}");
                errors = true;
            }

            return !errors;
        }

        private static string LoadBackupXsd()
        {
            var sitemapXsd = string.Empty;
            if (File.Exists(Constants.SitemapSchemaBackupPath))
            {
                using (var myFile = new StreamReader(Constants.SitemapSchemaBackupPath))
                {
                    sitemapXsd = myFile.ReadToEnd();
                }
            }
            return sitemapXsd;
        }
    }
}
