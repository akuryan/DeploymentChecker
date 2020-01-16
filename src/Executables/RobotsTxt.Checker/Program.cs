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
            IEnumerable<RobotsTxtReport> checkResults = from url in options.WebAppUrls.AsParallel().WithDegreeOfParallelism(ProcessorsCount) select Methods.Checker.CheckRobotsTxt(url, options.CrawlingDenied);

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
                exitWithError = !exitWithError ? report.CheckStatus : exitWithError;
                if (report.CheckStatus)
                {
                    reportString.AppendLine($"Robots.txt at {report.Url} is correct.");
                    if (report.Robots.Sitemaps.Any())
                    {
                        var sitemapReportString = " ";
                        if (!report.SitemapsIsValid)
                        {
                            sitemapReportString = " not ";
                        }
                        reportString.AppendLine($"Sitemaps, defined at robots.txt at {report.Url} is{sitemapReportString}valid");
                    }
                    else
                    {
                        reportString.AppendLine($"There is no sitemaps defined at robots.txt at {report.Url}");
                    }
                }
                else
                {
                    reportString.AppendLine($"Robots.txt at {report.Url} is not correct.");
                }
            }

            Console.WriteLine(reportString.ToString());
            if (exitWithError)
            {
                Environment.Exit(-1);
            }
        }
    }
}
