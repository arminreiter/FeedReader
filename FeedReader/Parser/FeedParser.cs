namespace CodeHollow.FeedReader.Parser
{
    using System;
    using System.Xml.Linq;

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

            if (rootElement.Equals("feed", StringComparison.InvariantCultureIgnoreCase))
                return FeedType.Atom;

            if (rootElement.Equals("rdf", StringComparison.InvariantCultureIgnoreCase))
                return FeedType.Rss_1_0;

            if (rootElement.Equals("rss", StringComparison.InvariantCultureIgnoreCase))
            {
                string version = doc.Root.Attribute("version").Value;
                if (version.Equals("2.0", StringComparison.InvariantCultureIgnoreCase))
                    return FeedType.Rss_2_0;

                if (version.Equals("0.91", StringComparison.InvariantCultureIgnoreCase))
                    return FeedType.Rss_0_91;

                if (version.Equals("0.92", StringComparison.InvariantCultureIgnoreCase))
                    return FeedType.Rss_0_92;

                return FeedType.Rss;
            }

            throw new Exception("unknown feed format");
        }

        /// <summary>
        /// Returns the parsed feed
        /// </summary>
        /// <param name="feedContent">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(string feedContent)
        {
            XDocument feedDoc = XDocument.Parse(feedContent);

            var feedType = ParseFeedType(feedDoc);

            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedContent);

            return feed.ToFeed();
        }
    }
}
