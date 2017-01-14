namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Atom 1.0 feed object according to specification: https://validator.w3.org/feed/docs/atom.html
    /// </summary>
    public class AtomFeed : Feed
    {
        public AtomPerson Author { get; set; }

        public ICollection<string> Categories { get; set; }

        public AtomPerson Contributor { get; set; }

        public string Generator { get; set; }

        public string Icon { get; set; }

        public string Id { get; set; }

        public string Logo { get; set; }

        public string Rights { get; set; }

        public string Subtitle { get; set; }

        public string UpdatedDateString { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public ICollection<AtomLink> Links { get; set; }

        public AtomFeed()
            : base() { }

        public AtomFeed(string feedXml, XElement feed)
            : base(feedXml, feed)
        {
            this.Link = feed.GetElement("link").Attribute("href")?.Value;

            this.Author = new AtomPerson(feed.GetElement("author"));

            var categories = feed.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();

            this.Contributor = new AtomPerson(feed.GetElement("contributor"));
            this.Generator = feed.GetValue("generator");
            this.Icon = feed.GetValue("icon");
            this.Id = feed.GetValue("id");
            this.Logo = feed.GetValue("logo");
            this.Rights = feed.GetValue("rights");
            this.Subtitle = feed.GetValue("subtitle");

            this.Links = feed.GetElements("link").Select(x => new AtomLink(x)).ToList();

            this.UpdatedDateString = feed.GetValue("updated");
            this.UpdatedDate = Helpers.TryParse(this.UpdatedDateString);

            var items = feed.GetElements("entry");

            foreach (var item in items)
            {
                this.Items.Add(new AtomFeedItem(item));
            }
        }
    }
}
