namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:license element
    /// </summary>
    public class PodcastLicense
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastLicense"/> class.
        /// </summary>
        /// <param name="element">the podcast:license element</param>
        public PodcastLicense(XElement element)
        {
            Identifier = element.Value;
            Url = PodcastNamespace.Attribute(element, "url");
        }

        /// <summary>
        /// The license identifier (node value), e.g. "cc-by-4.0"
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// The url attribute - the full license text
        /// </summary>
        public string Url { get; }
    }
}
