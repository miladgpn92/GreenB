using Microsoft.AspNetCore.Http;

namespace Common.Utilities
{
    /// <summary>
    /// the implement <see cref="HttpContext"/> extensions method.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Returns the combined components of the request URL in a fully un-escaped form(except for QueryStrings) with Route Path
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns> The combined components of the request URL in a fully un-escaped form(except for QueryStrings) with Route Path</returns>
        public static string GetFullUrl(this HttpRequest request)
        {
            var scheme = $"{request.Scheme}://";
            var domain = request.Host.Value;
            string link = request.Path.Value;

            return scheme + domain + link;
        }

        /// <summary>
        /// Returns the combined components of the request URL in a fully un-escaped form for api operations(except for QueryStrings)
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns> The combined components of the request URL in a fully un-escaped form for api operations(except for QueryStrings) </returns>
        public static string GetDomainWithScheme(this HttpRequest request)
        {
            string scheme = $"{request.Scheme}://";
            string domain = request.Host.Value;

            return scheme + domain;
        }

        /// <summary>
        /// Returns the combined components of the request URL in a fully un-escaped form for api operations(except for QueryStrings)
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <param name="url">the custom url append to domain url.</param>
        /// <returns> The combined components of the request URL in a fully un-escaped form for api operations(except for QueryStrings) </returns>
        public static string GetDomainWithScheme(this HttpRequest request, string url)
        {
            string domainWithScheme = GetDomainWithScheme(request);

            url = url.Trim();
            url = url.StartsWith("/") ? url : $"/{url}";

            return domainWithScheme + url;
        }

        /// <summary>
        /// Returns the Remote Ip Address of the Connection.
        /// </summary>
        /// <param name="connection">the connection to assemble the client pieces from.</param>
        /// <returns>Returns the Remote Ip Address of the Connection.</returns>
        public static string GetRemoteIp(this ConnectionInfo connection)
        {
            return connection.RemoteIpAddress.ToString();
        }
    }
}