namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// RSS 2.0 feed item accoring to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class Rss20FeedItem : FeedItem
    {
        public string Description { get; set; } // description

        public string Author { get; set; } // author

        public string Comments { get; set; } // comments

        public FeedItemEnclosure Enclosure { get; set; } // enclosure

        public string Guid { get; set; } // guid

        public string PublishingDateString { get; set; } // pubDate

        public DateTime? PublishingDate { get; set; } // pubDate

        public FeedItemSource Source { get; set; } // source

        public ICollection<string> Categories { get; set; } // category

        public string Content { get; set; } // content:encoded

        public DublinCore DC { get; set; }

        public Rss20FeedItem()
            : base() { }

        public Rss20FeedItem(XElement item)
            : base(item)
        {
            this.Comments = item.GetValue("comments");
            this.Author = item.GetValue("author");
            this.Enclosure = new FeedItemEnclosure(item.GetElement("enclosure"));
            this.PublishingDateString = item.GetValue("pubDate");
            this.PublishingDate = Helpers.TryParse(this.PublishingDateString);
            this.DC = new DublinCore(item);
            this.Source = new FeedItemSource(item.GetElement("source"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();

            this.Guid = item.GetValue("guid");
            this.Description = item.GetValue("description");
            this.Content = item.GetValue("content:encoded")?.HtmlDecode();
        }
    }
}
