namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// The podcast: (Podcasting 2.0, https://podcastindex.org/namespace/1.0) elements of the channel element of an rss 2.0 feed.
    /// Not covered: value, valueTimeSplit, liveItem, remoteItem, podroll, publisher, chat, podping and the deprecated images element -
    /// read them via <see cref="BaseFeed.Element"/> if needed.
    /// </summary>
    public class PodcastChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastChannel"/> class.
        /// </summary>
        /// <param name="channelElement">the channel element</param>
        public PodcastChannel(XElement channelElement)
        {
            Guid = PodcastNamespace.Value(channelElement, "guid")?.Trim();

            var locked = PodcastNamespace.Element(channelElement, "locked");
            Locked = locked != null && locked.Value.Trim().EqualsIgnoreCase("yes");
            LockedOwner = PodcastNamespace.Attribute(locked, "owner");

            Medium = PodcastNamespace.Value(channelElement, "medium")?.Trim();

            var license = PodcastNamespace.Element(channelElement, "license");
            License = license == null ? null : new PodcastLicense(license);

            var updateFrequency = PodcastNamespace.Element(channelElement, "updateFrequency");
            UpdateFrequency = updateFrequency == null ? null : new PodcastUpdateFrequency(updateFrequency);

            Funding = PodcastNamespace.Elements(channelElement, "funding").Select(x => new PodcastFunding(x)).ToArray();
            Persons = PodcastNamespace.Elements(channelElement, "person").Select(x => new PodcastPerson(x)).ToArray();
            Locations = PodcastNamespace.Elements(channelElement, "location").Select(x => new PodcastLocation(x)).ToArray();
            Trailers = PodcastNamespace.Elements(channelElement, "trailer").Select(x => new PodcastTrailer(x)).ToArray();
            Blocks = PodcastNamespace.Elements(channelElement, "block").Select(x => new PodcastBlock(x)).ToArray();
            Txts = PodcastNamespace.Elements(channelElement, "txt").Select(x => new PodcastTxt(x)).ToArray();
            Images = PodcastNamespace.Elements(channelElement, "image").Select(x => new PodcastImage(x)).ToArray();
            SocialInteracts = PodcastNamespace.Elements(channelElement, "socialInteract").Select(x => new PodcastSocialInteract(x)).ToArray();
        }

        /// <summary>
        /// The podcast:guid element - globally unique id of the podcast (UUIDv5)
        /// </summary>
        public string Guid { get; }

        /// <summary>
        /// The podcast:locked element - true if other platforms must not import the feed ("yes")
        /// </summary>
        public bool Locked { get; }

        /// <summary>
        /// The owner attribute of podcast:locked - email to verify ownership
        /// </summary>
        public string LockedOwner { get; }

        /// <summary>
        /// The podcast:medium element, e.g. podcast, music, video, audiobook. Null if not set (the spec then assumes "podcast").
        /// </summary>
        public string Medium { get; }

        /// <summary>
        /// The podcast:license element
        /// </summary>
        public PodcastLicense License { get; }

        /// <summary>
        /// The podcast:updateFrequency element
        /// </summary>
        public PodcastUpdateFrequency UpdateFrequency { get; }

        /// <summary>
        /// The podcast:funding elements
        /// </summary>
        public PodcastFunding[] Funding { get; }

        /// <summary>
        /// The podcast:person elements - the regular people of the show
        /// </summary>
        public PodcastPerson[] Persons { get; }

        /// <summary>
        /// The podcast:location elements
        /// </summary>
        public PodcastLocation[] Locations { get; }

        /// <summary>
        /// The podcast:trailer elements
        /// </summary>
        public PodcastTrailer[] Trailers { get; }

        /// <summary>
        /// The podcast:block elements
        /// </summary>
        public PodcastBlock[] Blocks { get; }

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
