namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:image element
    /// </summary>
    public class PodcastImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastImage"/> class.
        /// </summary>
        /// <param name="element">the podcast:image element</param>
        public PodcastImage(XElement element)
        {
            Href = PodcastNamespace.Attribute(element, "href");
            Alt = PodcastNamespace.Attribute(element, "alt");
            AspectRatio = PodcastNamespace.Attribute(element, "aspect-ratio");
            Width = PodcastNamespace.ParseInt(PodcastNamespace.Attribute(element, "width"));
            Height = PodcastNamespace.ParseInt(PodcastNamespace.Attribute(element, "height"));
            Type = PodcastNamespace.Attribute(element, "type");
            Purpose = PodcastNamespace.Attribute(element, "purpose");
        }

        /// <summary>
        /// The href attribute - url of the image
        /// </summary>
        public string Href { get; }

        /// <summary>
        /// The alt attribute - accessibility text
        /// </summary>
        public string Alt { get; }

        /// <summary>
        /// The aspect-ratio attribute, e.g. "16/9". The image is square if it is not set.
        /// </summary>
        public string AspectRatio { get; }

        /// <summary>
        /// The width attribute in pixels
        /// </summary>
        public int? Width { get; }

        /// <summary>
        /// The height attribute in pixels
        /// </summary>
        public int? Height { get; }

        /// <summary>
        /// The type attribute - mime type, e.g. image/webp
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The purpose attribute - space-separated tokens, e.g. "artwork social"
        /// </summary>
        public string Purpose { get; }
    }
}
