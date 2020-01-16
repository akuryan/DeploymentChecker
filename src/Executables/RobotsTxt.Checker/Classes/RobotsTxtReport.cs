namespace RobotsTxt.Checker.Classes
{
    public class RobotsTxtReport
    {
        public bool RobotsTxtExists { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// Indicates, if check was succesful or not
        /// </summary>
        public bool CheckStatus { get; set; }

        /// <summary>
        /// Indicates that Sitemaps in robots.txt are valid
        /// </summary>
        public bool SitemapsIsValid { get; set; }

        public Robots Robots { get; set; }
    }
}
