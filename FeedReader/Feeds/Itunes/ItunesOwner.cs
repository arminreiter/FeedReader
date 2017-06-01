namespace CodeHollow.FeedReader.Feeds.Itunes
{
    using System.Xml.Linq;

    /// <summary>
    /// The itunes:owner xml element
    /// </summary>
    public class ItunesOwner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesOwner"/> class.
        /// </summary>
        /// <param name="ownerElement">the owner xml element</param>
        public ItunesOwner(XElement ownerElement)
        {
            Name = ownerElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "name");
            Email = ownerElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "email");
        }
        
        /// <summary>
        /// The itunes:email of the owner
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// The itunes:name of the owner
        /// </summary>
        public string Name { get; }
    }
}
