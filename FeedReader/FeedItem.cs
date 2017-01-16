namespace CodeHollow.FeedReader
{
    using Feeds;
    using System;
    using System.Collections.Generic;

    public class FeedItem
    {
        public string Title { get; set; } // title

        public string Link { get; set; } // link

        public string Description { get; set; }

        public string PublishingDateString { get; set; }
        public DateTime? PublishingDate { get; set; }

        public string Author { get; set; }

        public string Id { get; set; }

        public ICollection<string> Categories { get; set; }

        public string Content { get; set; }
        
        public BaseFeedItem SpecificItem { get; set; }

        public FeedItem() { } // for (possible) serialization

        public FeedItem(BaseFeedItem feedItem)
        {
            this.Title = feedItem.Title;
            this.Link = feedItem.Link;
            this.Categories = new List<string>();
            this.SpecificItem = feedItem;
        }
    }
}
