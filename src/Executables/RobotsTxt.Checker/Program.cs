using CommandLine;
using CommandLine.Text;
using RobotsTxt.Checker.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RobotsTxt.Checker
{
    public class Program
    {
        private static readonly int ProcessorsCount = Environment.ProcessorCount;

        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            if (result.Tag.Equals(ParserResultType.NotParsed))
            {
                HelpText.AutoBuild(result);
                Environment.Exit(-1);
            }

            result.WithParsed(Run);
            // if we reached this point - everything is OK
            Environment.Exit(0);
        }

        private static void Run(Options options)
        {
            IEnumerable<RobotsTxtReport> checkResults = from url in options.WebAppUrls.AsParallel().WithDegreeOfParallelism(ProcessorsCount) where !string.IsNullOrWhiteSpace(url) select Methods.Checker.CheckRobotsTxt(url, options.CrawlingDenied, options.PerformSitemapValidation);

            RobotsTxtReport[] reports = checkResults.ToArray();

            var reportString = new StringBuilder();
            if (!reports.Any())
            {
                //something went wrong
                reportString.AppendLine("No robots.txt data have been retrieved. Exiting...");
                Console.WriteLine(reportString.ToString());
                Environment.Exit(-1);
            }
            //flag, that we shall not leave this processing
            bool exitWithError = false;
            foreach (var report in reports)
            {
                exitWithError = !exitWithError ? !report.CheckStatus : exitWithError;
                reportString.AppendLine(CheckStatusReport(report.CheckStatus, report.Url));
                if (options.PerformSitemapValidation)
                {
                    reportString.Append(SitemapValidationStatus(report.Robots.Sitemaps.Any(), report.SitemapsIsValid, report.Url));
                }
            }

            Console.WriteLine(reportString.ToString());
            if (exitWithError)
            {
                Environment.Exit(-1);
            }
        }

        private static string CheckStatusReport(bool status, string url)
        {
            return status ? $"Robots.txt at {url} is correct." : $"Robots.txt at {url} is not correct.";
        }

        private static string SitemapValidationStatus(bool sitemapsPresent, bool sitemapsIsValid, string url)
        {
            if (!sitemapsPresent)
            {
                return $"There is no sitemaps defined at robots.txt at {url}";
            }
            var sitemapReportString = " ";
            if (!sitemapsIsValid)
            {
                sitemapReportString = " not ";
            }
            return $"Sitemaps, defined at robots.txt at {url} is{sitemapReportString}valid";
        }
    }
}
