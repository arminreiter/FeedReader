namespace CodeHollow.FeedReader.Parser
{
    using System.Xml.Linq;
    using Feeds;

    public class Rss10Parser : AbstractXmlFeedParser
    {
        public override Feed Parse(string feedXml, XDocument feedDoc)
        {
            var rdf = feedDoc.Root;
            var channel = rdf.GetElement("channel");
            Rss10Feed feed = new Rss10Feed(feedXml, channel);
            return feed;
        }
    }
}