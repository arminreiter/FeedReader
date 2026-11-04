namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:person element. If an item has persons, they replace the persons of the channel for that episode.
    /// </summary>
    public class PodcastPerson
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastPerson"/> class.
        /// </summary>
        /// <param name="element">the podcast:person element</param>
        public PodcastPerson(XElement element)
        {
            Name = element.Value;
            // spec defaults if the attribute is missing
            Role = PodcastNamespace.Attribute(element, "role") ?? "host";
            Group = PodcastNamespace.Attribute(element, "group") ?? "cast";
            Img = PodcastNamespace.Attribute(element, "img");
            Href = PodcastNamespace.Attribute(element, "href");
        }

        /// <summary>
        /// The full name or alias of the person (node value)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The role attribute (case-insensitive, Podcast Taxonomy Project). "host" if not set.
        /// </summary>
        public string Role { get; }

        /// <summary>
        /// The group attribute (case-insensitive, Podcast Taxonomy Project). "cast" if not set.
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// The img attribute - url of a picture or avatar of the person
        /// </summary>
        public string Img { get; }

        /// <summary>
        /// The href attribute - url with information about the person
        /// </summary>
        public string Href { get; }
    }
}
