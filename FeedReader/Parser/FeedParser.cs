namespace CodeHollow.FeedReader.Parser
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Internal FeedParser - returns the type of the feed or the parsed feed.
    /// </summary>
    internal static class FeedParser
    {
        /// <summary>
        /// Returns the feed type - rss 1.0, rss 2.0, atom, ...
        /// </summary>
        /// <param name="doc">the xml document</param>
        /// <returns>the feed type</returns>
        public static FeedType ParseFeedType(XDocument doc)
        {
            string rootElement = doc.Root.Name.LocalName;

            if (rootElement.EqualsIgnoreCase("feed"))
                return FeedType.Atom;

            if (rootElement.EqualsIgnoreCase("rdf"))
                return FeedType.Rss_1_0;

            if (rootElement.EqualsIgnoreCase("rss"))
            {
                string version = doc.Root.Attribute("version").Value;
                if (version.EqualsIgnoreCase("2.0"))
                    return FeedType.Rss_2_0;

                if (version.EqualsIgnoreCase("0.91"))
                    return FeedType.Rss_0_91;

                if (version.EqualsIgnoreCase("0.92"))
                    return FeedType.Rss_0_92;

                return FeedType.Rss;
            }
            
            throw new FeedTypeNotSupportedException($"unknown feed type {rootElement}");
        }

        /// <summary>
        /// Returns the parsed feed
        /// </summary>
        /// <param name="feedContent">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(string feedContent)
        {
            feedContent = feedContent.Replace(((char)0x1C).ToString(), string.Empty); // replaces special char 0x1C, fixes issues with at least one feed
            feedContent = feedContent.Replace(((char)65279).ToString(), string.Empty); // replaces special char, fixes issues with at least one feed

            XDocument feedDoc = XDocument.Parse(feedContent);

            var feedType = ParseFeedType(feedDoc);

            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedContent);

            return feed.ToFeed();
        }
    }
}
