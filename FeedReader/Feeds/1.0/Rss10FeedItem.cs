namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Rss 1.0 Feed Item according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public class Rss10FeedItem : BaseFeedItem
    {
        /// <summary>
        /// The "about" attribute of the element
        /// </summary>
        public string About { get; set; }

        /// <summary>
        /// The "description" field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// All elements starting with "dc:"
        /// </summary>
        public DublinCore DC { get; set; }

        /// <summary>
        /// All elements starting with "sy:"
        /// </summary>
        public Syndication Sy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss10FeedItem"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss10FeedItem()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss10FeedItem"/> class.
        /// Reads a rss 1.0 feed item based on the xml given in item
        /// </summary>
        /// <param name="item">feed item as xml</param>
        public Rss10FeedItem(XElement item)
            : base(item)
        {
            this.DC = new DublinCore(item);
            this.Sy = new Syndication(item);

            this.About = item.GetAttribute("rdf:about").GetValue();
            this.Description = item.GetValue("description");
        }

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            FeedItem f = new FeedItem(this);

            if (this.DC != null)
            {
                f.Author = this.DC.Creator;
                f.Content = this.DC.Description;
                f.PublishingDate = this.DC.Date;
                f.PublishingDateString = this.DC.DateString;
            }

            f.Description = this.Description;
            if (string.IsNullOrEmpty(f.Content))
                f.Content = this.Description;
            f.Id = this.Link;

            return f;
        }
    }
}
