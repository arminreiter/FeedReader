namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// The podcast: (Podcasting 2.0, https://podcastindex.org/namespace/1.0) elements of an rss 2.0 item
    /// </summary>
    public class PodcastItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastItem"/> class.
        /// </summary>
        /// <param name="itemElement">the item element</param>
        public PodcastItem(XElement itemElement)
        {
            var season = PodcastNamespace.Element(itemElement, "season");
            Season = PodcastNamespace.ParseInt(season?.Value.Trim());
            SeasonName = PodcastNamespace.Attribute(season, "name");

            var episode = PodcastNamespace.Element(itemElement, "episode");
            Episode = PodcastNamespace.ParseDecimal(episode?.Value.Trim());
            EpisodeDisplay = PodcastNamespace.Attribute(episode, "display");

            var chapters = PodcastNamespace.Element(itemElement, "chapters");
            Chapters = chapters == null ? null : new PodcastChapters(chapters);

            var license = PodcastNamespace.Element(itemElement, "license");
            License = license == null ? null : new PodcastLicense(license);

            Transcripts = PodcastNamespace.Elements(itemElement, "transcript").Select(x => new PodcastTranscript(x)).ToArray();
            Soundbites = PodcastNamespace.Elements(itemElement, "soundbite").Select(x => new PodcastSoundbite(x)).ToArray();
            Persons = PodcastNamespace.Elements(itemElement, "person").Select(x => new PodcastPerson(x)).ToArray();
            Locations = PodcastNamespace.Elements(itemElement, "location").Select(x => new PodcastLocation(x)).ToArray();
            Funding = PodcastNamespace.Elements(itemElement, "funding").Select(x => new PodcastFunding(x)).ToArray();
            AlternateEnclosures = PodcastNamespace.Elements(itemElement, "alternateEnclosure").Select(x => new PodcastAlternateEnclosure(x)).ToArray();
            Txts = PodcastNamespace.Elements(itemElement, "txt").Select(x => new PodcastTxt(x)).ToArray();
            Images = PodcastNamespace.Elements(itemElement, "image").Select(x => new PodcastImage(x)).ToArray();
            SocialInteracts = PodcastNamespace.Elements(itemElement, "socialInteract").Select(x => new PodcastSocialInteract(x)).ToArray();
        }

        /// <summary>
        /// The podcast:season element (number). Null if missing or not an integer.
        /// </summary>
        public int? Season { get; }

        /// <summary>
        /// The name attribute of podcast:season
        /// </summary>
        public string SeasonName { get; }

        /// <summary>
        /// The podcast:episode element. Decimal, as the spec allows numbers like 100.5. Null if missing or not a number.
        /// </summary>
        public decimal? Episode { get; }

        /// <summary>
        /// The display attribute of podcast:episode - shown instead of the number if set
        /// </summary>
        public string EpisodeDisplay { get; }

        /// <summary>
        /// The podcast:chapters element
        /// </summary>
        public PodcastChapters Chapters { get; }

        /// <summary>
        /// The podcast:license element
        /// </summary>
        public PodcastLicense License { get; }

        /// <summary>
        /// The podcast:transcript elements
        /// </summary>
        public PodcastTranscript[] Transcripts { get; }

        /// <summary>
        /// The podcast:soundbite elements
        /// </summary>
        public PodcastSoundbite[] Soundbites { get; }

        /// <summary>
        /// The podcast:person elements. If not empty, they replace the persons of the channel for this episode.
        /// </summary>
        public PodcastPerson[] Persons { get; }

        /// <summary>
        /// The podcast:location elements
        /// </summary>
        public PodcastLocation[] Locations { get; }

        /// <summary>
        /// The podcast:funding elements
        /// </summary>
        public PodcastFunding[] Funding { get; }

        /// <summary>
        /// The podcast:alternateEnclosure elements
        /// </summary>
        public PodcastAlternateEnclosure[] AlternateEnclosures { get; }

        /// <summary>
        /// The podcast:txt elements
        /// </summary>
        public PodcastTxt[] Txts { get; }

        /// <summary>
        /// The podcast:image elements
        /// </summary>
        public PodcastImage[] Images { get; }

        /// <summary>
        /// The podcast:socialInteract elements
        /// </summary>
        public PodcastSocialInteract[] SocialInteracts { get; }
    }
}
