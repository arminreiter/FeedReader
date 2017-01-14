namespace CodeHollow.FeedReader.Parser
{
    using System.Xml.Linq;
    using Feeds;

    public class Rss091Parser : AbstractXmlFeedParser
    {
        public override Feed Parse(string feedXml, XDocument feedDoc)
        {
            var rss = feedDoc.Root;
            var channel = rss.GetElement("channel");
            Rss091Feed feed = new Rss091Feed(feedXml, channel);
            return feed;
        }
    }
}
