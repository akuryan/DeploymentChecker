using CommandLine;
using CommandLine.Text;
using System;

namespace RobotsTxt.Checker
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);

            if (result.Tag.Equals(ParserResultType.NotParsed))
            {
                HelpText.AutoBuild(result);
                Environment.Exit(-1);
            }
        }
    }
}
