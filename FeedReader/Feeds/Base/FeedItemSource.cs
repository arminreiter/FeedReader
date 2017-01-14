namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// item source object from rss 2.0 according to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class FeedItemSource
    {
        public string Url { get; set; }

        public string Value { get; set; }

        public FeedItemSource(XElement element)
        {
            this.Url = element.GetAttributeValue("url");
            this.Value = element.GetValue();
        }
    }
}
