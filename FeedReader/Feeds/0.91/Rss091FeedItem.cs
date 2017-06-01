namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Rss 0.91 Feed Item according to specification: http://www.rssboard.org/rss-0-9-1-netscape#image
    /// </summary>
    public class Rss091FeedItem : BaseFeedItem
    {
        /// <summary>
        /// The "description" field
        /// </summary>
        public string Description { get; set; } // description

        /// <summary>
        /// The "pubDate" field
        /// </summary>
        public string PublishingDateString { get; set; }

        /// <summary>
        /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
        /// </summary>
        public DateTime? PublishingDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss091FeedItem"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss091FeedItem()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss091FeedItem"/> class.
        /// Creates this object based on the xml in the XElement parameter.
        /// </summary>
        /// <param name="item">feed item as xml</param>
        public Rss091FeedItem(XElement item)
            : base(item)
        {
            this.Description = item.GetValue("description");
            this.PublishingDateString = item.GetValue("pubDate");
            this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString);
        }

        internal override FeedItem ToFeedItem()
        {
            FeedItem fi = new FeedItem(this)
            {
                Description = this.Description,
                PublishingDate = this.PublishingDate,
                PublishingDateString = this.PublishingDateString,
                Id = this.Link
            };
            return fi;
        }
    }
}
