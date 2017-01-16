namespace CodeHollow.FeedReader.Feeds
{
    using System.Collections.Generic;

    public abstract class BaseFeed
    {
        public abstract Feed ToFeed();

        public string Title { get; set; }

        public string Link { get; set; }

        public ICollection<BaseFeedItem> Items { get; set; }

        /// <summary>
        /// Gets the whole, original feed as string
        /// </summary>
        public string OriginalDocument { get; private set; }

        public BaseFeed()
        {
            this.Items = new List<BaseFeedItem>();
        }

        public BaseFeed(string feedXml, System.Xml.Linq.XElement xelement)
            : this()
        {
            this.OriginalDocument = feedXml;

            this.Title = xelement.GetValue("title");
            this.Link = xelement.GetValue("link");
        }
    }
}
