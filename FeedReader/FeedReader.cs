namespace CodeHollow.FeedReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Parser;

    /// <summary>
    /// The static FeedReader class which allows to read feeds from a given url. Use it to
    /// parse a feed from an url <see cref="Read(string)"/>, a file <see cref="ReadFromFile(string)"/> or <see cref="ReadFromFileAsync(string)"/>, a byte array <see cref="ReadFromByteArray(byte[])"/>
    /// or a string <see cref="ReadFromString(string)"/>. If the feed url is not known, <see cref="ParseFeedUrlsFromHtml(string)"/>
    /// returns all feed links on a given page.
    /// </summary>
    /// <example>
    /// var links = FeedReader.ParseFeedUrlsFromHtml("https://codehollow.com");
    /// var firstLink = links.First();
    /// var feed = FeedReader.Read(firstLink.Url);
    /// Console.WriteLine(feed.Title);
    /// </example>
    public static class FeedReader
    {
        /// <summary>
        /// gets a url (with or without http) and returns the full url
        /// </summary>
        /// <param name="url">url with or without http</param>
        /// <returns>full url</returns>
        /// <example>GetUrl("codehollow.com"); => returns https://codehollow.com</example>
        public static string GetAbsoluteUrl(string url)
        {
            return new UriBuilder(url).ToString();
        }

        /// <summary>
        /// Returns the absolute url of a link on a page. If you got the feed links via
        /// GetFeedUrlsFromUrl(url) and the url is relative, you can use this method to get the full url.
        /// </summary>
        /// <param name="pageUrl">the original url to the page</param>
        /// <param name="feedLink">a referenced feed (link)</param>
        /// <returns>a feed link</returns>
        /// <example>GetAbsoluteFeedUrl("codehollow.com", myRelativeFeedLink);</example>
        public static HtmlFeedLink GetAbsoluteFeedUrl(string pageUrl, HtmlFeedLink feedLink)
        {
            string tmpUrl = feedLink.Url.HtmlDecode();
            pageUrl = GetAbsoluteUrl(pageUrl);

            if (tmpUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || tmpUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return feedLink;

            if (tmpUrl.StartsWith("//", StringComparison.OrdinalIgnoreCase)) // special case
                tmpUrl = "http:" + tmpUrl;

            if (Uri.TryCreate(tmpUrl, UriKind.RelativeOrAbsolute, out Uri finalUri))
            {
                if (finalUri.IsAbsoluteUri)
                {
                    return new HtmlFeedLink(feedLink.Title.HtmlDecode(), finalUri.ToString(), feedLink.FeedType);
                }
                else if (Uri.TryCreate(pageUrl + '/' + tmpUrl.TrimStart('/'), UriKind.Absolute, out finalUri))
                    return new HtmlFeedLink(feedLink.Title.HtmlDecode(), finalUri.ToString(), feedLink.FeedType);
            }
            
            throw new UrlNotFoundException($"Could not get the absolute url out of {pageUrl} and {feedLink.Url}");
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <returns>a list of links including the type and title, an empty list if no links are found</returns>
        /// <example>FeedReader.GetFeedUrlsFromUrl("codehollow.com"); // returns a list of all available feeds at
        /// https://codehollow.com </example>
        [Obsolete("Use GetFeedUrlsFromUrlAsync method")]
        public static IEnumerable<HtmlFeedLink> GetFeedUrlsFromUrl(string url)
        {
            return GetFeedUrlsFromUrlAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>a list of links including the type and title, an empty list if no links are found</returns>
        /// <example>FeedReader.GetFeedUrlsFromUrl("codehollow.com"); // returns a list of all available feeds at
        /// https://codehollow.com </example>
        public static async Task<IEnumerable<HtmlFeedLink>> GetFeedUrlsFromUrlAsync(string url, CancellationToken cancellationToken, bool autoRedirect = true)
        {
            url = GetAbsoluteUrl(url);
            string pageContent = await Helpers.DownloadAsync(url, cancellationToken, autoRedirect).ConfigureAwait(false);
            var links = ParseFeedUrlsFromHtml(pageContent);
            return links;
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <returns>a list of links including the type and title, an empty list if no links are found</returns>
        /// <example>FeedReader.GetFeedUrlsFromUrl("codehollow.com"); // returns a list of all available feeds at
        /// https://codehollow.com </example>
        public static Task<IEnumerable<HtmlFeedLink>> GetFeedUrlsFromUrlAsync(string url, bool autoRedirect = true)
        {
            return GetFeedUrlsFromUrlAsync(url, CancellationToken.None, autoRedirect);
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <returns>a list of links, an empty list if no links are found</returns>
        [Obsolete("Use the ParseFeedUrlsAsStringAsync method")]
        public static string[] ParseFeedUrlsAsString(string url)
        {
            return ParseFeedUrlsAsStringAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// <returns>a list of links, an empty list if no links are found</returns>
        public static async Task<string[]> ParseFeedUrlsAsStringAsync(string url, CancellationToken cancellationToken)
        {
            return (await GetFeedUrlsFromUrlAsync(url, cancellationToken).ConfigureAwait(false)).Select(x => x.Url).ToArray();
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <returns>a list of links, an empty list if no links are found</returns>
        public static Task<string[]> ParseFeedUrlsAsStringAsync(string url)
        {
            return ParseFeedUrlsAsStringAsync(url, CancellationToken.None);
        }

        /// <summary>
        /// Parses RSS links from html page and returns all links
        /// </summary>
        /// <param name="htmlContent">the content of the html page</param>
        /// <returns>all RSS/feed links</returns>
        public static IEnumerable<HtmlFeedLink> ParseFeedUrlsFromHtml(string htmlContent)
        {
            // left the method here for downward compatibility
            return Helpers.ParseFeedUrlsFromHtml(htmlContent);
        }

        /// <summary>
        /// reads a feed from an url. the url must be a feed. Use ParseFeedUrlsFromHtml to
        /// parse the feeds from a url which is not a feed.
        /// </summary>
        /// <param name="url">the url to a feed</param>
        /// <returns>parsed feed</returns>
        [Obsolete("Use ReadAsync method")]
        public static Feed Read(string url)
        {
            return ReadAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// reads a feed from an url. the url must be a feed. Use ParseFeedUrlsFromHtml to
        /// parse the feeds from a url which is not a feed.
        /// </summary>
        /// <param name="url">the url to a feed</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <param name="userAgent">override built-in user-agent header</param>
        /// <returns>parsed feed</returns>
        public static async Task<Feed> ReadAsync(string url, CancellationToken cancellationToken, bool autoRedirect = true, string userAgent=null)
        {
            var feedContent = await Helpers.DownloadBytesAsync(GetAbsoluteUrl(url), cancellationToken, autoRedirect, userAgent).ConfigureAwait(false);
            return ReadFromByteArray(feedContent);
        }

        /// <summary>
        /// reads a feed from an url. the url must be a feed. Use ParseFeedUrlsFromHtml to
        /// parse the feeds from a url which is not a feed.
        /// </summary>
        /// <param name="url">the url to a feed</param>
        /// <param name="autoRedirect">autoredirect if page is moved permanently</param>
        /// <param name="userAgent">override built-in user-agent header</param>
        /// <returns>parsed feed</returns>
        public static Task<Feed> ReadAsync(string url, bool autoRedirect = true, string userAgent=null)
        {
            return ReadAsync(url, CancellationToken.None, autoRedirect, userAgent);
        }

        /// <summary>
        /// reads a feed from a file
        /// </summary>
        /// <param name="filePath">the path to the feed file</param>
        /// <returns>parsed feed</returns>
        public static Feed ReadFromFile(string filePath)
        {
            var feedContent = System.IO.File.ReadAllBytes(filePath);
            return ReadFromByteArray(feedContent);
        }

        /// <summary>
        /// reads a feed from a file
        /// </summary>
        /// <param name="filePath">the path to the feed file</param>
        /// <param name="cancellationToken">token to cancel operation</param>
        /// <returns>parsed feed</returns>
        public static async Task<Feed> ReadFromFileAsync(string filePath, CancellationToken cancellationToken)
        {
            byte[] result;
#if NETCOREAPP2_1
            {
                result = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
            }
#else
            {
                using (var stream = System.IO.File.Open(filePath, System.IO.FileMode.Open))
                {
                    result = new byte[stream.Length];
                    await stream.ReadAsync(result, 0, (int)stream.Length, cancellationToken).ConfigureAwait(false);
                }
            }
            #endif
            return ReadFromByteArray(result);
        }

        /// <summary>
        /// reads a feed from a file
        /// </summary>
        /// <param name="filePath">the path to the feed file</param>
        /// <returns>parsed feed</returns>
        public static Task<Feed> ReadFromFileAsync(string filePath)
        {
            return ReadFromFileAsync(filePath, CancellationToken.None);
        }

        /// <summary>
        /// reads a feed from the <paramref name="feedContent" />
        /// </summary>
        /// <param name="feedContent">the feed content (xml)</param>
        /// <returns>parsed feed</returns>
        public static Feed ReadFromString(string feedContent)
        {
            return FeedParser.GetFeed(feedContent);
        }

        /// <summary>
        /// reads a feed from the bytearray <paramref name="feedContent"/>
        /// This could be useful if some special encoding is used.
        /// </summary>
        /// <param name="feedContent"></param>
        /// <returns></returns>
        public static Feed ReadFromByteArray(byte[] feedContent)
        {
            return FeedParser.GetFeed(feedContent);
        }
    }
}
