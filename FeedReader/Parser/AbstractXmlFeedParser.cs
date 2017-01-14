namespace CodeHollow.FeedReader.Parser
{
    using System.Xml.Linq;

    public abstract class AbstractXmlFeedParser : IFeedParser
    {
        public Feed Parse(string feedXml)
        {
            XDocument feedDoc = XDocument.Parse(feedXml);

            return this.Parse(feedXml, feedDoc);
        }

        public abstract Feed Parse(string feedXml, XDocument feedDoc);
    }
}
