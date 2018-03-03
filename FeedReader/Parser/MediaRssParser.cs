namespace CodeHollow.FeedReader.Parser
{
    using System.Xml.Linq;
    using Feeds;

    internal class MediaRssParser : AbstractXmlFeedParser
    {
        public override BaseFeed Parse(string feedXml, XDocument feedDoc)
        {
            var rss = feedDoc.Root;
            var channel = rss.GetElement("channel");
            MediaRssFeed feed = new MediaRssFeed(feedXml, channel);
            return feed;
        }
    }
}