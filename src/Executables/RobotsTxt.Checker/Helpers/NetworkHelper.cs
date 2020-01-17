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
        public static string GetString(Uri uri)
        {
            var httpHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            var httpClient = new HttpClient(httpHandler);

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

            var data = string.Empty;
            using (var client = new WebClient())
            {
                data = client.DownloadString(uri);
            }
            return data;
        }
    }
}
