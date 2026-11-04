namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:transcript element
    /// </summary>
    public class PodcastTranscript
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastTranscript"/> class.
        /// </summary>
        /// <param name="element">the podcast:transcript element</param>
        public PodcastTranscript(XElement element)
        {
            Url = PodcastNamespace.Attribute(element, "url");
            Type = PodcastNamespace.Attribute(element, "type");
            Language = PodcastNamespace.Attribute(element, "language");
            Rel = PodcastNamespace.Attribute(element, "rel");
        }

        /// <summary>
        /// The url attribute
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The type attribute - mime type, e.g. text/vtt, application/x-subrip, application/json
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The language attribute. If null, the language of the feed applies.
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// The rel attribute. "captions" means the file is a closed captions file.
        /// </summary>
        public string Rel { get; }
    }
}
