namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 0.91 Feed Image according to specification: http://www.rssboard.org/rss-0-9-1-netscape#image
    /// </summary>
    public class Rss091FeedImage : FeedImage
    {
        /// <summary>
        /// The "description" element
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The "width" element
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// The "height" element
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss091FeedImage"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss091FeedImage()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss091FeedImage"/> class.
        /// Creates this object based on the xml in the XElement parameter.
        /// </summary>
        /// <param name="element">feed image as xml</param>
        public Rss091FeedImage(XElement element)
            : base(element)
        {
            this.Description = element.GetValue("description");
            this.Width = Helpers.TryParseInt(element.GetValue("width"));
            this.Height = Helpers.TryParseInt(element.GetValue("height"));
        }
    }
}
