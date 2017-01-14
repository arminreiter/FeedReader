namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    public class Syndication
    {
        public string UpdatePeriod { get; set; }

        public string UpdateFrequency { get; set; }

        public string UpdateBase { get; set; }

        public Syndication() { }

        public Syndication(XElement xelement)
        {
            this.UpdateBase = xelement.GetValue("sy:updateBase");
            this.UpdateFrequency = xelement.GetValue("sy:updateFrequency");
            this.UpdatePeriod = xelement.GetValue("sy:updatePeriod");
        }
    }
}
