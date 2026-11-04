namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:chapters element - link to a chapters file
    /// </summary>
    public class PodcastChapters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastChapters"/> class.
        /// </summary>
        /// <param name="element">the podcast:chapters element</param>
        public PodcastChapters(XElement element)
        {
            Url = PodcastNamespace.Attribute(element, "url");
            Type = PodcastNamespace.Attribute(element, "type");
        }

        /// <summary>
        /// The url attribute
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The type attribute - mime type, usually application/json+chapters
        /// </summary>
        public string Type { get; }
    }
}
