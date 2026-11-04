namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:socialInteract element - the root post for comments, e.g. on ActivityPub
    /// </summary>
    public class PodcastSocialInteract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastSocialInteract"/> class.
        /// </summary>
        /// <param name="element">the podcast:socialInteract element</param>
        public PodcastSocialInteract(XElement element)
        {
            Protocol = PodcastNamespace.Attribute(element, "protocol");
            Uri = PodcastNamespace.Attribute(element, "uri");
            AccountId = PodcastNamespace.Attribute(element, "accountId");
            AccountUrl = PodcastNamespace.Attribute(element, "accountUrl");
            Priority = PodcastNamespace.ParseInt(PodcastNamespace.Attribute(element, "priority"));
        }

        /// <summary>
        /// The protocol attribute, e.g. "activitypub". "disabled" means comments are turned off.
        /// </summary>
        public string Protocol { get; }

        /// <summary>
        /// The uri attribute - the root post
        /// </summary>
        public string Uri { get; }

        /// <summary>
        /// The accountId attribute, e.g. "@dave"
        /// </summary>
        public string AccountId { get; }

        /// <summary>
        /// The accountUrl attribute
        /// </summary>
        public string AccountUrl { get; }

        /// <summary>
        /// The priority attribute - lower number means higher priority
        /// </summary>
        public int? Priority { get; }
    }
}
