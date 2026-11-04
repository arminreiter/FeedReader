namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:source element inside podcast:alternateEnclosure
    /// </summary>
    public class PodcastSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastSource"/> class.
        /// </summary>
        /// <param name="element">the podcast:source element</param>
        public PodcastSource(XElement element)
        {
            Uri = PodcastNamespace.Attribute(element, "uri");
            ContentType = PodcastNamespace.Attribute(element, "contentType");
        }

        /// <summary>
        /// The uri attribute - where the media file can be downloaded or streamed
        /// </summary>
        public string Uri { get; }

        /// <summary>
        /// The contentType attribute - mime type, if the transport differs from the file (e.g. torrents)
        /// </summary>
        public string ContentType { get; }
    }
}
