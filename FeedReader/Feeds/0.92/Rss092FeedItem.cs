namespace CodeHollow.FeedReader.Feeds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Rss 0.92 feed item according to specification: http://backend.userland.com/rss092
    /// </summary>
    public class Rss092FeedItem : Rss091FeedItem
    {
        /// <summary>
        /// All entries "category" entries
        /// </summary>
        public ICollection<string> Categories { get; set; }

        /// <summary>
        /// The "enclosure" field
        /// </summary>
        public FeedItemEnclosure Enclosure { get; set; }

        /// <summary>
        /// The "source" field
        /// </summary>
        public FeedItemSource Source { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss092FeedItem"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss092FeedItem()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss092FeedItem"/> class.
        /// Creates a new feed item element based on the given xml XELement
        /// </summary>
        /// <param name="item">the xml containing the feed item</param>
        public Rss092FeedItem(XElement item)
            : base(item)
        {
            this.Enclosure = new FeedItemEnclosure(item.GetElement("enclosure"));
            this.Source = new FeedItemSource(item.GetElement("source"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();
        }
    }
}
