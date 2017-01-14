namespace CodeHollow.FeedReader.Parser
{
    using System;
    using System.Xml.Linq;

    public class FeedParser
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

                return FeedType.Rss;
            }

            throw new Exception("unknown feed format");
        }

        public Feed GetFeed(string feedXml)
        {
            XDocument feedDoc = XDocument.Parse(feedXml);

            var feedType = ParseFeedType(feedDoc);

            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedXml);

            return feed;
        }
    }
}
