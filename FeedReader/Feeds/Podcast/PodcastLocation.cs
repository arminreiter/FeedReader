namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:location element
    /// </summary>
    public class PodcastLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastLocation"/> class.
        /// </summary>
        /// <param name="element">the podcast:location element</param>
        public PodcastLocation(XElement element)
        {
            Name = element.Value;
            Rel = PodcastNamespace.Attribute(element, "rel") ?? "subject";
            Geo = PodcastNamespace.Attribute(element, "geo");
            Osm = PodcastNamespace.Attribute(element, "osm");
            Country = PodcastNamespace.Attribute(element, "country");
        }

        /// <summary>
        /// Human readable location (node value), e.g. "Austin, TX"
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The rel attribute: "subject" (what the content is about, default) or "creator" (where it was recorded)
        /// </summary>
        public string Rel { get; }

        /// <summary>
        /// The geo attribute - a geo URI (RFC 5870), e.g. "geo:30.2672,97.7431"
        /// </summary>
        public string Geo { get; }

        /// <summary>
        /// The osm attribute - OpenStreetMap identifier, e.g. "R113314"
        /// </summary>
        public string Osm { get; }

        /// <summary>
        /// The country attribute - ISO 3166-1 alpha-2 code
        /// </summary>
        public string Country { get; }
    }
}
