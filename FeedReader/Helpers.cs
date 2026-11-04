namespace CodeHollow.FeedReader
{
    using Feeds.MediaRSS;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
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

        // The default HttpClient is a single shared instance (https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/),
        // created on first download only, so parse-only usage never creates it.
        private static readonly Lazy<HttpClient> _defaultHttpClient = new Lazy<HttpClient>(() => new HttpClient(
#if NETCOREAPP
            // recycle pooled connections so a static client picks up DNS changes in long running apps
            new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            }
#else
            new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }
#endif
        ));

        private static HttpClient _httpClient;

        /// <summary>
        /// The HttpClient used for all downloads. Set your own instance once at startup, e.g. to use a proxy,
        /// log requests or change the timeout (default: 100 seconds). Set it to null to use the built-in client again.
        /// The Accept and User-Agent headers are still added per request.
        /// </summary>
        public static HttpClient HttpClient
        {
            get { return _httpClient ?? _defaultHttpClient.Value; }
            set { _httpClient = value; }
        }

        // html is untrusted input, so cap regex time. Declared before LinkTagRegex: static fields init in order.
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

        // <link ... rel=alternate ...> with double, single or no quotes
        private static readonly Regex LinkTagRegex = new Regex(
            // lookbehind rejects data-rel=, lookahead rejects rel=alternate2
            @"<link[^>]*(?<![\w-])rel\s*=\s*(?:""alternate""|'alternate'|alternate(?=[\s>/]))[^>]*>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase,
            RegexTimeout);

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <returns>Content as string</returns>
        [Obsolete("Use the DownloadAsync method")]
        public static string Download(string url)
        {
            return DownloadAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <param name="userAgent">override built-in user-agent header</param>
        /// <returns>Content as byte array</returns>
        public static async Task<byte[]> DownloadBytesAsync(string url, CancellationToken cancellationToken, bool autoRedirect = true, string userAgent = USER_AGENT_VALUE)
        {
            // decoding a well-formed url breaks escaped chars (e.g. ?u=https%3A%2F%2F...), so only
            // decode input that isn't a valid url yet - keeps old behavior for fully encoded urls.
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                url = System.Net.WebUtility.UrlDecode(url);
            userAgent = userAgent ?? USER_AGENT_VALUE;

            using (var response = await SendAsync(url, userAgent, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var statusCode = (int)response.StatusCode;
                    // HttpClientHandler follows most redirects itself; this covers the rest (e.g. https -> http on .net core)
                    // 301 - Moved Permanently, 302 - Moved temporarily, 307 - Temporary redirect, 308 - Permanent redirect
                    var location = response.Headers.Location;
                    if (autoRedirect && location != null && (statusCode == 301 || statusCode == 302 || statusCode == 307 || statusCode == 308))
                        url = (location.IsAbsoluteUri ? location : new Uri(new Uri(url), location)).AbsoluteUri;

                    // some pages return forbidden if a user-agent is set (see commit 351e3ff) - retry once without headers.
                    // redirect targets are requested the same way, like before.
                    using (var retry = await SendAsync(url, userAgent, cancellationToken, withHeaders: false).ConfigureAwait(false))
                    {
                        // ponytail: error responses still return their body (no exception) to stay backward compatible
                        return await retry.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    }
                }

                return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
        }

        private static async Task<HttpResponseMessage> SendAsync(string url, string userAgent, CancellationToken cancellationToken, bool withHeaders = true)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                if (withHeaders)
                {
                    request.Headers.TryAddWithoutValidation(ACCEPT_HEADER_NAME, ACCEPT_HEADER_VALUE);
                    request.Headers.TryAddWithoutValidation(USER_AGENT_NAME, userAgent);
                }
                return await HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as byte array</returns>
        public static Task<byte[]> DownloadBytesAsync(string url, bool autoRedirect = true)
        {
            return DownloadBytesAsync(url, CancellationToken.None, autoRedirect);
        }

        /// <summary>
        /// Download the content from an url and returns it as utf8 encoded string.
        /// Preferred way is to use <see cref="DownloadBytesAsync(string, bool)"/> because it works
        /// better with encoding.
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as string</returns>
        public static async Task<string> DownloadAsync(string url, CancellationToken cancellationToken, bool autoRedirect = true)
        {
            var content = await DownloadBytesAsync(url, cancellationToken, autoRedirect).ConfigureAwait(false);
            return Encoding.UTF8.GetString(content);
        }

        /// <summary>
        /// Download the content from an url and returns it as utf8 encoded string.
        /// Preferred way is to use <see cref="DownloadBytesAsync(string, bool)"/> because it works
        /// better with encoding.
        /// </summary>
        /// <param name="url">correct url</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>Content as string</returns>
        public static Task<string> DownloadAsync(string url, bool autoRedirect = true)
        {
            return DownloadAsync(url, CancellationToken.None, autoRedirect);
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

            datetime = ReplaceRfc2822TimeZone(datetime.Trim());

            var dateTimeFormat = cultureInfo?.DateTimeFormat ?? DateTimeFormatInfo.CurrentInfo;
            bool parseSuccess = DateTimeOffset.TryParse(datetime, dateTimeFormat, DateTimeStyles.None, out var dt);

            if (!parseSuccess)
            {
                // Do, 22 Dez 2016 17:36:00 +0000
                // note - tried ParseExact with diff formats like "ddd, dd MMM yyyy hh:mm:ss K"
                if (datetime.Contains(","))
                {
                    int pos = datetime.IndexOf(',') + 1;
                    string newdtstring = datetime.Substring(pos).Trim();

                    parseSuccess = DateTimeOffset.TryParse(newdtstring, dateTimeFormat, DateTimeStyles.None, out dt);
                }
                // strip the last part (usually an unknown timezone like "CEST") and assume utc
                int lastSpace = datetime.LastIndexOf(' ');
                if (!parseSuccess && lastSpace > 0)
                {
                    string newdtstring = datetime.Substring(0, lastSpace).Trim();

                    parseSuccess = DateTimeOffset.TryParse(newdtstring, dateTimeFormat, DateTimeStyles.AssumeUniversal,
                        out dt);
                }
            }

            if (!parseSuccess)
                return null;

            return dt.UtcDateTime;
        }

        // named zones of RFC 822 / RFC 2822 (US zones, as used in rss pubDate). DateTimeOffset only knows GMT and Z.
        // Other abbreviations (CEST, IST, ...) are ambiguous and still fall back to utc in TryParseDateTime.
        private static readonly Dictionary<string, string> Rfc2822TimeZones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "UT", "+00:00" },
            { "EST", "-05:00" }, { "EDT", "-04:00" },
            { "CST", "-06:00" }, { "CDT", "-05:00" },
            { "MST", "-07:00" }, { "MDT", "-06:00" },
            { "PST", "-08:00" }, { "PDT", "-07:00" }
        };

        /// <summary>
        /// Replaces a trailing RFC 2822 time zone name with its offset, e.g. "08:00:00 EST" -> "08:00:00 -05:00"
        /// </summary>
        private static string ReplaceRfc2822TimeZone(string datetime)
        {
            int lastSpace = datetime.LastIndexOf(' ');
            if (lastSpace > 0 && Rfc2822TimeZones.TryGetValue(datetime.Substring(lastSpace + 1), out var offset))
                return datetime.Substring(0, lastSpace + 1) + offset;
            return datetime;
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

        /// <summary>
        /// Returns a HtmlFeedLink object from a linktag (link href="" type="")
        /// only support application/rss, application/atom and application/feed+json as type
        /// if type is not supported, null is returned
        /// </summary>
        /// <param name="input">link tag, e.g. &lt;link rel="alternate" type="application/rss+xml" title="codehollow &gt; Feed" href="https://codehollow.com/feed/" /&gt;</param>
        /// <returns>Parsed HtmlFeedLink</returns>
        public static HtmlFeedLink GetFeedLinkFromLinkTag(string input)
        {
            string linkTag = input.HtmlDecode();
            string type = GetAttributeFromLinkTag("type", linkTag).ToLower();

            // json feed: only application/feed+json - plain application/json is also used for
            // non-feed links, e.g. the WordPress REST API (<link rel="alternate" type="application/json" href="/wp-json/...">)
            bool isJsonFeed = type.Contains("application/feed+json");
            if (!type.Contains("application/rss") && !type.Contains("application/atom") && !isJsonFeed)
                return null;

            HtmlFeedLink hfl = new HtmlFeedLink();
            string title = GetAttributeFromLinkTag("title", linkTag);
            string href = GetAttributeFromLinkTag("href", linkTag);
            hfl.Title = title;
            hfl.Url = href;
            hfl.FeedType = isJsonFeed ? FeedType.JsonFeed : type.Contains("rss") ? FeedType.Rss : FeedType.Atom;
            return hfl;
        }

        /// <summary>
        /// Parses RSS links from html page and returns all links
        /// </summary>
        /// <param name="htmlContent">the content of the html page</param>
        /// <returns>all RSS/feed links</returns>
        public static IEnumerable<HtmlFeedLink> ParseFeedUrlsFromHtml(string htmlContent)
        {
            // sample link:
            // <link rel="alternate" type="application/rss+xml" title="Microsoft Bot Framework Blog" href="http://blog.botframework.com/feed.xml">
            // <link rel="alternate" type="application/atom+xml" title="Aktuelle News von heise online" href="https://www.heise.de/newsticker/heise-atom.xml">

            List<HtmlFeedLink> result = new List<HtmlFeedLink>();

            try
            {
                foreach (Match m in LinkTagRegex.Matches(htmlContent))
                {
                    var hfl = GetFeedLinkFromLinkTag(m.Value);
                    if (hfl != null)
                        result.Add(hfl);
                }
            }
            catch (RegexMatchTimeoutException) { }

            return result;
        }

        /// <summary>
        /// reads an attribute from an html tag
        /// </summary>
        /// <param name="attribute">name of the attribute, e.g. title</param>
        /// <param name="htmlTag">the html tag, e.g. &lt;link title="my title"&gt;</param>
        /// <returns>the value of the attribute, e.g. my title</returns>
        private static string GetAttributeFromLinkTag(string attribute, string htmlTag)
        {
            // value in double quotes, single quotes or unquoted (stops at whitespace / tag end, per the HTML spec)
            try
            {
                var res = Regex.Match(htmlTag, "(?<![\\w-])" + attribute + "\\s*=\\s*(?:\"(?<val>[^\"]*)\"|'(?<val>[^']*)'|(?<val>[^\\s>\"'`=<]+))", RegexOptions.IgnoreCase, RegexTimeout);
                return res.Success ? res.Groups["val"].Value : string.Empty;
            }
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }
    }
}
