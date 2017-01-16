namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Rss Feed according to Rss 0.91 specification:
    /// http://www.rssboard.org/rss-0-9-1-netscape
    /// </summary>
    public class Rss091Feed : BaseFeed
    {
        public string Description { get; set; } // required field description

        public string Language { get; set; } // required field language

        public string Copyright { get; set; }

        public string Docs { get; set; }

        public FeedImage Image { get; set; }

        public string LastBuildDateString { get; set; }

        public DateTime? LastBuildDate { get; set; }

        public string ManagingEditor { get; set; }

        public string PublishingDateString { get; set; }

        public DateTime? PublishingDate { get; set; }

        public string Rating { get; set; }

        public ICollection<string> SkipDays { get; set; }

        public ICollection<string> SkipHours { get; set; }

        public FeedTextInput TextInput { get; set; }

        public string WebMaster { get; set; }

        public Syndication Sy { get; set; }

        public Rss091Feed()
            : base()
        {
            this.SkipDays = new List<string>();
            this.SkipHours = new List<string>();
        }

        public Rss091Feed(string feedXml, XElement channel)
            : base(feedXml, channel)
        {
            this.Description = channel.GetValue("description");
            this.Language = channel.GetValue("language");
            this.Image = new Rss091FeedImage(channel.GetElement("image"));
            this.Copyright = channel.GetValue("copyright");
            this.ManagingEditor = channel.GetValue("managingEditor");
            this.WebMaster = channel.GetValue("webMaster");
            this.Rating = channel.GetValue("rating");

            this.PublishingDateString = channel.GetValue("pubDate");
            this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString);

            this.LastBuildDateString = channel.GetValue("lastBuildDate");
            this.LastBuildDate = Helpers.TryParseDateTime(this.LastBuildDateString);

            this.Docs = channel.GetValue("docs");

            this.TextInput = new FeedTextInput(channel.GetElement("textinput"));

            this.Sy = new Syndication(channel);

            var skipHours = channel.GetElement("skipHours");
            if (skipHours != null)
                this.SkipHours = skipHours.GetElements("hour")?.Select(x => x.GetValue()).ToList();

            var skipDays = channel.GetElement("skipDays");
            if (skipDays != null)
                this.SkipDays = skipDays.GetElements("day")?.Select(x => x.GetValue()).ToList();

            var items = channel.GetElements("item");

            AddItems(items);
        }

        public override Feed ToFeed()
        {
            Feed f = new Feed(this);

            f.Copyright = this.Copyright;
            f.Description = this.Description;
            f.ImageUrl = this.Image?.Url;
            f.Language = this.Language;
            f.LastUpdatedDate = this.LastBuildDate;
            f.LastUpdatedDateString = this.LastBuildDateString;
            f.Type = FeedType.Rss_0_91;

            return f;
        }

        internal virtual void AddItems(IEnumerable<XElement> items)
        {
            foreach (var item in items)
            {
                this.Items.Add(new Rss091FeedItem(item));
            }
        }
    }
}
