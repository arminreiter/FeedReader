namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// RSS 2.0 feed accoring to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class Rss20Feed : Feed
    {
        public string Description { get; set; }

        public string Language { get; set; }

        public string Copyright { get; set; }

        public string Docs { get; set; }

        public FeedImage Image { get; set; }

        public string LastBuildDateString { get; set; }

        public DateTime? LastBuildDate { get; set; }

        public string ManagingEditor { get; set; }

        public string PublishingDateString { get; set; }

        public DateTime? PublishingDate { get; set; }

        public string WebMaster { get; set; }

        public ICollection<string> Categories { get; set; } // category

        public string Generator { get; set; }

        public FeedCloud Cloud { get; set; }

        public string TTL { get; set; }

        public FeedTextInput TextInput { get; set; }

        public ICollection<string> SkipDays { get; set; }

        public ICollection<string> SkipHours { get; set; }

        public Syndication Sy { get; set; }

        public Rss20Feed()
            : base() { }

        public Rss20Feed(string feedXml, XElement channel)
            : base(feedXml, channel)
        {
            this.Description = channel.GetValue("description");
            this.Language = channel.GetValue("language");
            this.Copyright = channel.GetValue("copyright");
            this.ManagingEditor = channel.GetValue("managingEditor");
            this.WebMaster = channel.GetValue("webMaster");
            this.Docs = channel.GetValue("docs");
            this.PublishingDateString = channel.GetValue("pubDate");
            this.PublishingDate = Helpers.TryParse(this.PublishingDateString);
            this.LastBuildDateString = channel.GetValue("lastBuildDate");
            this.LastBuildDate = Helpers.TryParse(this.LastBuildDateString);

            var categories = channel.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();

            this.Sy = new Syndication(channel);
            this.Generator = channel.GetValue("generator");
            this.TTL = channel.GetValue("ttl");
            this.Image = new Rss20FeedImage(channel.GetElement("image"));
            this.Cloud = new FeedCloud(channel.GetElement("cloud"));
            this.TextInput = new FeedTextInput(channel.GetElement("textinput"));

            var skipHours = channel.GetElement("skipHours");
            if (skipHours != null)
                this.SkipHours = skipHours.GetElements("hour")?.Select(x => x.GetValue()).ToList();

            var skipDays = channel.GetElement("skipDays");
            if (skipDays != null)
                this.SkipDays = skipDays.GetElements("day")?.Select(x => x.GetValue()).ToList();

            var items = channel.GetElements("item");

            foreach (var item in items)
            {
                this.Items.Add(new Rss20FeedItem(item));
            }
        }
    }
}
