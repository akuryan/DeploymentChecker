namespace RobotsTxt
{
    /// <summary>
    /// Parses robots.txt
    /// </summary>
    public interface IRobotsParser
    {
        /// <summary>
        /// Checks, if path is allowed
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsPathAllowed(string userAgent, string path);
        /// <summary>
        /// Check delays
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        long CrawlDelay(string userAgent);
    }
}