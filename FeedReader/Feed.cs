namespace CodeHollow.FeedReader
{
    using Feeds;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Feed
    {
        public FeedType Type { get; set; }

        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Copyright { get; set; }
        public string LastUpdatedDateString { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<FeedItem> Items { get; set; }
        
        /// <summary>
        /// Gets the whole, original feed as string
        /// </summary>
        public string OriginalDocument { get { return SpecificFeed.OriginalDocument; } }

        public BaseFeed SpecificFeed { get; set; }

        public Feed(BaseFeed feed)
        {
            SpecificFeed = feed;

            Title = feed.Title;
            Link = feed.Link;

            Items = feed.Items.Select(x => x.ToFeedItem()).ToList();
        }
    }
}
