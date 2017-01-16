namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Rss 1.0 Feed Item according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public class Rss10FeedItem : BaseFeedItem
    {
        public string About { get; set; }

        public string Description { get; set; }

        public DublinCore DC { get; set; }

        public Syndication Sy { get; set; }

        public Rss10FeedItem()
            : base() { }

        public Rss10FeedItem(XElement item)
            : base(item)
        {
            this.DC = new DublinCore(item);
            this.Sy = new Syndication(item);

            this.About = item.GetAttribute("rdf:about").GetValue();
            this.Description = item.GetValue("description");
        }

        internal override FeedItem ToFeedItem()
        {
            FeedItem f = new FeedItem(this);

            if (this.DC != null)
            {
                f.Author = this.DC.Publisher;
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
