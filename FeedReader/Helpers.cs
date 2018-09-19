namespace CodeHollow.FeedReader
{
    using Feeds.MediaRSS;
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// static class with helper functions
    /// </summary>
    public static class Helpers
    {
        private const string ACCEPT_HEADER_NAME = "Accept";
        private const string ACCEPT_HEADER_VALUE = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        private const string USER_AGENT_NAME = "User-Agent";
        private const string USER_AGENT_VALUE = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0";

        // The HttpClient instance must be a static field
        // https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <returns>Content as string</returns>
        [Obsolete("Use the DownloadAsync method")]
        public static string Download(string url)
        {
            var downloadTask = DownloadAsync(url);
            downloadTask.ConfigureAwait(false);

            return downloadTask.Result;
        }

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as byte array</returns>
        public static async Task<byte[]> DownloadBytesAsync(string url, bool autoRedirect = true)
        {
            url = System.Net.WebUtility.UrlDecode(url);
            HttpResponseMessage response;
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.TryAddWithoutValidation(ACCEPT_HEADER_NAME, ACCEPT_HEADER_VALUE);
                request.Headers.TryAddWithoutValidation(USER_AGENT_NAME, USER_AGENT_VALUE);

                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
            }
            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                // redirect if statuscode = 301 - Moved Permanently, 308 - Permanent redirect
                if (autoRedirect && (statusCode == 301 || statusCode == 308)) 
                {
                    url = response.Headers?.Location?.AbsoluteUri ?? url;
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                }
            }
            var content = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return content;
        }

        /// <summary>
        /// Download the content from an url and returns it as utf8 encoded string.
        /// Preferred way is to use <see cref="DownloadBytesAsync(string, bool)"/> because it works
        /// better with encoding.
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as string</returns>
        public static async Task<string> DownloadAsync(string url, bool autoRedirect = true)
        {
            var content = await DownloadBytesAsync(url, autoRedirect).ConfigureAwait(false);
            return Encoding.UTF8.GetString(content);
        }

        /// <summary>
        /// Tries to parse the string as datetime and returns null if it fails
        /// </summary>
        /// <param name="datetime">datetime as string</param>
        /// <param name="cultureInfo">The cultureInfo for parsing</param>
        /// <returns>datetime or null</returns>
        public static DateTime? TryParseDateTime(string datetime, CultureInfo cultureInfo = null)
        {
            if (string.IsNullOrWhiteSpace(datetime))
                return null;

            var dateTimeFormat = cultureInfo?.DateTimeFormat ?? DateTimeFormatInfo.CurrentInfo;

            if (!DateTimeOffset.TryParse(datetime, dateTimeFormat, DateTimeStyles.None, out var dt))
            {
                // Do, 22 Dez 2016 17:36:00 +0000
                // note - tried ParseExact with diff formats like "ddd, dd MMM yyyy hh:mm:ss K"
                if (datetime.Contains(","))
                {
                    int pos = datetime.IndexOf(',') + 1;
                    string newdtstring = datetime.Substring(pos).Trim();

                    DateTimeOffset.TryParse(newdtstring, dateTimeFormat, DateTimeStyles.None, out dt);
                }
            }

            if (dt == default(DateTimeOffset))
                return null;

            return dt.UtcDateTime;
        }

        /// <summary>
        /// Tries to parse the string as int and returns null if it fails
        /// </summary>
        /// <param name="input">int as string</param>
        /// <returns>integer or null</returns>
        public static int? TryParseInt(string input)
        {
            if (!int.TryParse(input, out int tmp))
                return null;
            return tmp;
        }

        /// <summary>
        /// Tries to parse a string and returns the media type
        /// </summary>
        /// <param name="medium">media type as string</param>
        /// <returns><see cref="Medium"/></returns>
        public static Medium TryParseMedium(string medium)
        {
            if (string.IsNullOrEmpty(medium))
            {
                return Medium.Unknown;
            }

            switch (medium.ToLower())
            {
                case "image":
                    return Medium.Image;
                case "audio":
                    return Medium.Audio;
                case "video":
                    return Medium.Video;
                case "document":
                    return Medium.Document;
                case "executable":
                    return Medium.Executable;
                default:
                    return Medium.Unknown;
            }
        }

        /// <summary>
        /// Tries to parse the string as int and returns null if it fails
        /// </summary>
        /// <param name="input">int as string</param>
        /// <returns>integer or null</returns>
        public static bool? TryParseBool(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.ToLower();

                if (input == "true")
                {
                    return true;
                }
                else if (input == "false")
                {
                    return false;
                }
            }

            return null;
        }
    }
}
