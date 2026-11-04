namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:alternateEnclosure element - another version of the episode media,
    /// e.g. a different bitrate, a video version or an HLS stream
    /// </summary>
    public class PodcastAlternateEnclosure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastAlternateEnclosure"/> class.
        /// </summary>
        /// <param name="element">the podcast:alternateEnclosure element</param>
        public PodcastAlternateEnclosure(XElement element)
        {
            Type = PodcastNamespace.Attribute(element, "type");
            Length = PodcastNamespace.ParseLong(PodcastNamespace.Attribute(element, "length"));
            Bitrate = PodcastNamespace.ParseDecimal(PodcastNamespace.Attribute(element, "bitrate"));
            Height = PodcastNamespace.ParseInt(PodcastNamespace.Attribute(element, "height"));
            Lang = PodcastNamespace.Attribute(element, "lang");
            Title = PodcastNamespace.Attribute(element, "title");
            Rel = PodcastNamespace.Attribute(element, "rel");
            Codecs = PodcastNamespace.Attribute(element, "codecs");
            Default = PodcastNamespace.Attribute(element, "default").EqualsIgnoreCase("true");
            Sources = PodcastNamespace.Elements(element, "source").Select(x => new PodcastSource(x)).ToArray();
        }

        /// <summary>
        /// The type attribute - mime type of the media
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The length attribute - file size in bytes
        /// </summary>
        public long? Length { get; }

        /// <summary>
        /// The bitrate attribute - average bits per second
        /// </summary>
        public decimal? Bitrate { get; }

        /// <summary>
        /// The height attribute - video height in pixels
        /// </summary>
        public int? Height { get; }

        /// <summary>
        /// The lang attribute (BCP 47)
        /// </summary>
        public string Lang { get; }

        /// <summary>
        /// The title attribute
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The rel attribute - groups alternate versions of the same content
        /// </summary>
        public string Rel { get; }

        /// <summary>
        /// The codecs attribute (RFC 6381)
        /// </summary>
        public string Codecs { get; }

        /// <summary>
        /// The default attribute - true if this is the same media as the enclosure and should be preferred
        /// </summary>
        public bool Default { get; }

        /// <summary>
        /// The podcast:source child elements
        /// </summary>
        public PodcastSource[] Sources { get; }
    }
}
