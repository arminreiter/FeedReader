namespace CodeHollow.FeedReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CodeHollow.FeedReader.Parser;

    public static class FeedReader
    {
        public static Feed Read(string url)
        {
            string feedContent = Download(url);

            return ReadFromString(feedContent);
        }

        public static Feed ReadFromFile(string filePath)
        {
            string feedContent = System.IO.File.ReadAllText(filePath);

            return ReadFromString(feedContent);
        }

        public static Feed ReadFromString(string feedContent)
        {
            FeedParser parser = new FeedParser();
            var feed = parser.GetFeed(feedContent);

            return feed;
        }

        /// <summary>
        /// Returns the absolute url of a link on a page
        /// </summary>
        /// <param name="pageUrl">the original url to the page</param>
        /// <param name="feedLink">a referenced feed (link)</param>
        /// <returns>a feed link</returns>
        public static HtmlFeedLink GetAbsoluteFeedUrl(string pageUrl, HtmlFeedLink feedLink)
        {
            string tmpUrl = feedLink.Url.HtmlDecode();
            pageUrl = GetAbsoluteUrl(pageUrl);

            if (tmpUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                || tmpUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                return feedLink;

            if (tmpUrl.StartsWith("//", StringComparison.InvariantCultureIgnoreCase)) // special case
                tmpUrl = "http:" + tmpUrl;

            Uri finalUri;
            if (Uri.TryCreate(tmpUrl, UriKind.RelativeOrAbsolute, out finalUri))
            {
                if (finalUri.IsAbsoluteUri)
                {
                    return new HtmlFeedLink(feedLink.Title.HtmlDecode(), finalUri.ToString(), feedLink.FeedType);
                }
                else if (Uri.TryCreate(pageUrl + '/' + tmpUrl.TrimStart('/'), UriKind.Absolute, out finalUri))
                    return new HtmlFeedLink(feedLink.Title.HtmlDecode(), finalUri.ToString(), feedLink.FeedType);
            }

            throw new Exception($"Could not get the absolute url out of {pageUrl} and {feedLink.Url}");
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <returns>a list of links, an empty list if no links are found</returns>
        public static string[] ParseFeedUrlsAsString(string url)
        {
            return GetFeedUrlsFromUrl(url).Select(x => x.Url).ToArray();
        }

        /// <summary>
        /// Opens a webpage and reads all feed urls from it (link rel="alternate" type="application/...")
        /// </summary>
        /// <param name="url">the url of the page</param>
        /// <returns>a list of links including the type and title, an empty list if no links are found</returns>
        public static IEnumerable<HtmlFeedLink> GetFeedUrlsFromUrl(string url)
        {
            url = GetAbsoluteUrl(url);
            string pageContent = Download(url);
            return ParseFeedUrlsFromHtml(pageContent);
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
            var htmlDoc = new HtmlAgilityPack.HtmlDocument()
            {
                OptionAutoCloseOnEnd = true,
                OptionFixNestedTags = true
            };

            htmlDoc.LoadHtml(htmlContent);

            if (htmlDoc.DocumentNode != null)
            {
                var nodes = htmlDoc.DocumentNode.SelectNodes("//link").Where(
                    x => x.Attributes["type"] != null &&
                    (x.Attributes["type"].Value.Contains("application/rss") || x.Attributes["type"].Value.Contains("application/atom")));

                foreach (var node in nodes)
                {
                    yield return new HtmlFeedLink()
                    {
                        Title = node.Attributes["title"]?.Value,
                        Url = node.Attributes["href"]?.Value,
                        FeedType = GetFeedTypeFromLinkType(node.Attributes["type"].Value)
                    };
                }
            }
        }

        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <returns>content as string</returns>
        public static string Download(string url)
        {
            url = System.Web.HttpUtility.UrlDecode(url);
            using (var webclient = new System.Net.WebClient())
            {
                // header required - without it, some pages return a bad request (e.g. http://www.methode.at/blog?format=RSS)
                // see: https://msdn.microsoft.com/en-us/library/system.net.webclient(v=vs.110).aspx
                webclient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                return webclient.DownloadString(url);
            }
        }

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
        /// read the rss feed type from the type statement of an html link
        /// </summary>
        /// <param name="linkType">application/rss+xml or application/atom+xml or ...</param>
        /// <returns>the feed type</returns>
        private static FeedType GetFeedTypeFromLinkType(string linkType)
        {
            if (linkType.Contains("application/rss"))
                return FeedType.Rss;

            if (linkType.Contains("application/atom"))
                return FeedType.Atom;

            throw new Exception($"The link type '{linkType}' is not a valid feed link!");
        }
    }
}
