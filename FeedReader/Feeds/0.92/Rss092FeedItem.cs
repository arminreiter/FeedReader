namespace CodeHollow.FeedReader.Feeds
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class Rss092FeedItem : Rss091FeedItem
    {
        public ICollection<string> Categories { get; set; }

        public FeedItemEnclosure Enclosure { get; set; }

        public FeedItemSource Source { get; set; }
        
        public Rss092FeedItem(XElement item) : base(item)
        {
            this.Enclosure = new FeedItemEnclosure(item.GetElement("enclosure"));
            this.Source = new FeedItemSource(item.GetElement("source"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();
        }
    }
}
