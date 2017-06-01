namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// item source object from rss 2.0 according to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class FeedItemSource
    {
        /// <summary>
        /// The "url" element
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The value of the element
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItemSource"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public FeedItemSource()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedItemSource"/> class.
        /// Reads a rss feed item based on the xml given in element
        /// </summary>
        /// <param name="element">item source element as xml</param>
        public FeedItemSource(XElement element)
        {
            this.Url = element.GetAttributeValue("url");
            this.Value = element.GetValue();
        }
    }
}
