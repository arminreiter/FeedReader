namespace CodeHollow.FeedReader.Feeds.Itunes
{
    using System.Xml.Linq;

    /// <summary>
    /// The itunes:image xml element
    /// </summary>
    public class ItunesImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesImage"/> class.
        /// </summary>
        /// <param name="image">The itunes:image element</param>
        public ItunesImage(XElement image)
        {
            Href = image.GetAttributeValue("href");
        }

        /// <summary>
        /// The value of the href attribute
        /// </summary>
        public string Href { get; }
    }
}
