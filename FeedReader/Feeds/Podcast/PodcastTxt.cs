namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:txt element - free form text, e.g. ownership verification or the "ai-content" disclosure
    /// </summary>
    public class PodcastTxt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastTxt"/> class.
        /// </summary>
        /// <param name="element">the podcast:txt element</param>
        public PodcastTxt(XElement element)
        {
            Purpose = PodcastNamespace.Attribute(element, "purpose");
            Value = element.Value;
        }

        /// <summary>
        /// The purpose attribute, e.g. "verify", "applepodcastsverify" or "ai-content"
        /// </summary>
        public string Purpose { get; }

        /// <summary>
        /// The node value
        /// </summary>
        public string Value { get; }
    }
}
