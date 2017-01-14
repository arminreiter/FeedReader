namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Enclosure object of Rss 2.0 according to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class FeedItemEnclosure
    {
        public string Url { get; set; }

        public int? Length { get; set; }

        public string MediaType { get; set; }

        public FeedItemEnclosure(XElement element)
        {
            this.Url = element.GetAttributeValue("url");
            this.Length = Helpers.TryParseInt(element.GetAttributeValue("length"));
            this.MediaType = element.GetAttributeValue("type");
        }
    }
}
