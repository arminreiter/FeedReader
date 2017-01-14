namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Rss 0.91 Feed Item according to specification: http://www.rssboard.org/rss-0-9-1-netscape#image
    /// </summary>
    public class Rss091FeedItem : FeedItem
    {
        public string Description { get; set; } // description

        public string PublishingDateString { get; set; } // pubDate

        public DateTime? PublishingDate { get; set; } // pubDate

        public Rss091FeedItem()
            : base() { }

        public Rss091FeedItem(XElement item)
            : base(item)
        {
            this.Description = item.GetValue("description");
            this.PublishingDateString = item.GetValue("pubDate");
            this.PublishingDate = Helpers.TryParse(this.PublishingDateString);
        }
    }
}
