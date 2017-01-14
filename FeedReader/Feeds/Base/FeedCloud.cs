namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Cloud object according to Rss 2.0 specification: https://validator.w3.org/feed/docs/rss2.html#ltcloudgtSubelementOfLtchannelgt
    /// </summary>
    public class FeedCloud
    {
        public string Domain { get; set; }

        public string Port { get; set; }

        public string Path { get; set; }

        public string RegisterProcedure { get; set; }

        public string Protocol { get; set; }

        public FeedCloud(XElement element)
        {
            this.Domain = element.GetAttributeValue("domain");
            this.Port = element.GetAttributeValue("port");
            this.Path = element.GetAttributeValue("path");
            this.RegisterProcedure = element.GetAttributeValue("registerProcedure");
            this.Protocol = element.GetAttributeValue("protocol");
        }
    }
}
