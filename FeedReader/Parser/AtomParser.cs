namespace CodeHollow.FeedReader.Parser
{
    using System.Xml.Linq;
    using Feeds;

    public class AtomParser : AbstractXmlFeedParser
    {
        public override Feed Parse(string feedXml, XDocument feedDoc)
        {
            AtomFeed feed = new AtomFeed(feedXml, feedDoc.Root);
            return feed;
        }
    }
}
