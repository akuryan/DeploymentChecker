namespace RobotsTxt.Checker.Classes
{
    public class Constants
    {
        /// <summary>
        /// Holds URL to download sitemap schema
        /// </summary>
        public const string SitemapSchemaUrl = "https://www.sitemaps.org/schemas/sitemap/0.9";

        /// <summary>
        /// If <see cref="SitemapSchemaUrl"/> is not accessible - we could use local backup to validate Sitemap schema
        /// </summary>
        public const string SitemapSchemaBackupName = "sitemap.xsd";

        /// <summary>
        /// Holds relative path to backup of sitemap schema
        /// </summary>
        public static readonly string SitemapSchemaBackupPath = $"Files/{SitemapSchemaBackupName}";

        public const string RobotsTxtName = "robots.txt";
    }
}
