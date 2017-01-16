namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    public abstract class BaseFeedItem
    {
        public string Title { get; set; } // title

        public string Link { get; set; } // link

        internal abstract FeedItem ToFeedItem();

        public BaseFeedItem() { }

        public BaseFeedItem(XElement item)
        {
            this.Title = item.GetValue("title");
            this.Link = item.GetValue("link");
        }
    }
}
