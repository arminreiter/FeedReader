namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Atom 1.0 feed item object according to specification: https://validator.w3.org/feed/docs/atom.html
    /// </summary>
    public class AtomFeedItem : BaseFeedItem
    {
        public AtomPerson Author { get; set; }

        public ICollection<string> Categories { get; set; }

        public string Content { get; set; }

        public AtomPerson Contributor { get; set; }

        public string Id { get; set; }

        public string PublishedDateString { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string Rights { get; set; }

        public string Source { get; set; }

        public string Summary { get; set; }

        public string UpdatedDateString { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public ICollection<AtomLink> Links { get; set; }

        public AtomFeedItem()
            : base() { }

        public AtomFeedItem(XElement item)
            : base(item)
        {
            this.Link = item.GetElement("link").Attribute("href")?.Value;

            this.Author = new AtomPerson(item.GetElement("author"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();

            this.Content = item.GetValue("content").HtmlDecode();
            this.Contributor = new AtomPerson(item.GetElement("contributor"));
            this.Id = item.GetValue("id");

            this.PublishedDateString = item.GetValue("published");
            this.PublishedDate = Helpers.TryParseDateTime(this.PublishedDateString);
            this.Links = item.GetElements("link").Select(x => new AtomLink(x)).ToList();

            this.Rights = item.GetValue("rights");
            this.Source = item.GetValue("source");
            this.Summary = item.GetValue("summary");

            this.UpdatedDateString = item.GetValue("updated");
            this.UpdatedDate = Helpers.TryParseDateTime(this.UpdatedDateString);
        }

        internal override FeedItem ToFeedItem()
        {
            FeedItem fi = new FeedItem(this);

            fi.Author = this.Author?.ToString();
            fi.Categories = this.Categories;
            fi.Content = this.Content;
            fi.Description = this.Summary;
            fi.Id = this.Id;
            fi.PublishingDate = this.PublishedDate;
            fi.PublishingDateString = this.PublishedDateString;

            return fi;
        }
    }
}
