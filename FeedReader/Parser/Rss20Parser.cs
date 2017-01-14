namespace CodeHollow.FeedReader.Parser
{
    using System.Xml.Linq;
    using Feeds;

    public class Rss20Parser : AbstractXmlFeedParser
    {
        public override Feed Parse(string feedXml, XDocument feedDoc)
        {
            var rss = feedDoc.Root;
            var channel = rss.GetElement("channel");
            Rss20Feed feed = new Rss20Feed(feedXml, channel);
            return feed;
        }
    }
}