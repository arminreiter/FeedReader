namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System;
    using System.Globalization;
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:trailer element
    /// </summary>
    public class PodcastTrailer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastTrailer"/> class.
        /// </summary>
        /// <param name="element">the podcast:trailer element</param>
        public PodcastTrailer(XElement element)
        {
            Title = element.Value;
            Url = PodcastNamespace.Attribute(element, "url");
            PubDateString = PodcastNamespace.Attribute(element, "pubdate");
            // RFC 2822 uses english day/month names - parse culture independent
            PubDate = Helpers.TryParseDateTime(PubDateString, CultureInfo.InvariantCulture);
            Length = PodcastNamespace.ParseLong(PodcastNamespace.Attribute(element, "length"));
            Type = PodcastNamespace.Attribute(element, "type");
            Season = PodcastNamespace.ParseInt(PodcastNamespace.Attribute(element, "season"));
        }

        /// <summary>
        /// The title of the trailer (node value)
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The url attribute - the audio or video file
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The pubdate attribute as string
        /// </summary>
        public string PubDateString { get; }

        /// <summary>
        /// The pubdate attribute as DateTime. Null if parsing failed or it is empty.
        /// </summary>
        public DateTime? PubDate { get; }

        /// <summary>
        /// The length attribute - file size in bytes
        /// </summary>
        public long? Length { get; }

        /// <summary>
        /// The type attribute - mime type of the file
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The season attribute - the season this trailer belongs to
        /// </summary>
        public int? Season { get; }
    }
}
