namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:funding element - a link to donate to or support the podcast
    /// </summary>
    public class PodcastFunding
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastFunding"/> class.
        /// </summary>
        /// <param name="element">the podcast:funding element</param>
        public PodcastFunding(XElement element)
        {
            Url = PodcastNamespace.Attribute(element, "url");
            Text = element.Value;
        }

        /// <summary>
        /// The url attribute
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The text that should be shown next to the link (node value)
        /// </summary>
        public string Text { get; }
    }
}
