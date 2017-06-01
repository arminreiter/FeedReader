namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 2.0 Image according to specification: https://validator.w3.org/feed/docs/rss2.html#ltimagegtSubelementOfLtchannelgt
    /// </summary>
    public class Rss20FeedImage : Rss091FeedImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rss20FeedImage"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss20FeedImage()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss20FeedImage"/> class.
        /// Reads a rss 2.0 feed image based on the xml given in element
        /// </summary>
        /// <param name="element">feed image as xml</param>
        public Rss20FeedImage(XElement element)
            : base(element)
        {
        }
    }
}
