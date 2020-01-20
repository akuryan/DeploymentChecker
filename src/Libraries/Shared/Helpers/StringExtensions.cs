using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Parses supplied string and extracts host header from it
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static string CreateValidHostHeader(this string hostname)
        {
            if (string.IsNullOrWhiteSpace(hostname))
            {
                return string.Empty;
            }

            Uri uri;
            Uri.TryCreate(hostname.PrepareParceableHostName(), UriKind.Absolute, out uri);

            return uri?.DnsSafeHost;
        }

        public static string PrepareParceableHostName(this string hostname)
        {
            var url = hostname.Trim();
            if (url.StartsWith("://"))
            {
                url = string.Concat("https", url);
            }
            if (!url.ToLowerInvariant().StartsWith("http"))
            {
                url = string.Concat("https://", url);
            }

            return url;
        }
    }
}
