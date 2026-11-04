namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:block element - asks a platform (or all platforms) not to import the feed
    /// </summary>
    public class PodcastBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastBlock"/> class.
        /// </summary>
        /// <param name="element">the podcast:block element</param>
        public PodcastBlock(XElement element)
        {
            Id = PodcastNamespace.Attribute(element, "id");
            Blocked = element.Value.Trim().EqualsIgnoreCase("yes");
        }

        /// <summary>
        /// The id attribute - service slug of the platform, null means all platforms
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// True if the node value is "yes"
        /// </summary>
        public bool Blocked { get; }
    }
}
