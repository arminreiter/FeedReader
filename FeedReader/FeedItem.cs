namespace CodeHollow.FeedReader
{
    using System.Xml.Linq;

    public class FeedItem
    {
        public string Title { get; set; } // title

        public string Link { get; set; } // link

        public FeedItem() { }

        public FeedItem(XElement item)
        {
            this.Title = item.GetValue("title");
            this.Link = item.GetValue("link");
        }
    }
}
