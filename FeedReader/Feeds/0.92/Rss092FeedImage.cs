namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 0.92 Feed Image according to specification: http://backend.userland.com/rss092
    /// </summary>
    public class Rss092FeedImage : Rss091FeedImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rss092FeedImage"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss092FeedImage()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss092FeedImage"/> class.
        /// Creates this object based on the xml in the XElement parameter.
        /// </summary>
        /// <param name="element">rss 0.92 image as xml</param>
        public Rss092FeedImage(XElement element)
            : base(element)
        {
        }
    }
}
