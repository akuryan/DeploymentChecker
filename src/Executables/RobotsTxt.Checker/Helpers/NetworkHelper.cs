using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace RobotsTxt.Checker.Helpers
{
    /// <summary>
    /// This class is inspired by https://codereview.stackexchange.com/questions/194647/handle-redirect-manually
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        /// Retrieves file at uri, passed as parameter
        /// </summary>
        /// <param name="uri">File to download</param>
        /// <returns></returns>
        public static string GetString(Uri uri)
        {
            return GetString(uri, string.Empty);
        }

        /// <summary>
        /// If hostheader is not empty - constructs request to uri, but replaces Host in request headeres with hostheader value
        /// </summary>
        /// <param name="uri">File to download</param>
        /// <param name="hostHeader">Sets header Host in request to this value</param>
        /// <returns></returns>
        public static string GetString(Uri uri, string hostHeader)
        {
            var data = string.Empty;
            if (uri == null)
            {
                return data;
            }

            var httpHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            using (var httpClient = new HttpClient(httpHandler))
            {
                var request = new HttpRequestMessage { RequestUri = uri, Method = HttpMethod.Get };

                var response = httpClient.SendAsync(request).Result;

                var statusCode = (int)response.StatusCode;

                if (statusCode >= 300 && statusCode <= 399)
                {
                    Uri redirectUri = response.Headers.Location;
                    if (!redirectUri.IsAbsoluteUri)
                    {
                        redirectUri = new Uri(request.RequestUri.GetLeftPart(UriPartial.Authority) + redirectUri);
                    }
                    return GetString(redirectUri);
                }

                if (!string.IsNullOrWhiteSpace(hostHeader))
                {
                    request.Headers.Host = hostHeader.CreateValidHostHeader();
                }

                response = httpClient.SendAsync(request).Result;
                data = response.Content.ToString();
            }

            return data;
        }
    }
}
