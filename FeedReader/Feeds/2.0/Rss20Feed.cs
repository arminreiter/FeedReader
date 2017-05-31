namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// RSS 2.0 feed accoring to specification: https://validator.w3.org/feed/docs/rss2.html
    /// </summary>
    public class Rss20Feed : BaseFeed
    {
        /// <summary>
        /// The "description" element
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The "language" element
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The "copyright" element
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The "docs" element
        /// </summary>
        public string Docs { get; set; }

        /// <summary>
        /// The "image" element
        /// </summary>
        public FeedImage Image { get; set; }

        /// <summary>
        /// The "lastBuildDate" element as string
        /// </summary>
        public string LastBuildDateString { get; set; }

        /// <summary>
        /// The "lastBuildDate" element as DateTime. Null if parsing failed of lastBuildDate is empty.
        /// </summary>
        public DateTime? LastBuildDate { get; set; }

        /// <summary>
        /// The "managingEditor" element
        /// </summary>
        public string ManagingEditor { get; set; }

        /// <summary>
        /// The "pubDate" field
        /// </summary>
        public string PublishingDateString { get; set; }

        /// <summary>
        /// The "pubDate" field as DateTime. Null if parsing failed or pubDate is empty.
        /// </summary>
        public DateTime? PublishingDate { get; set; }

        /// <summary>
        /// The "webMaster" field
        /// </summary>
        public string WebMaster { get; set; }

        /// <summary>
        /// All "category" elements
        /// </summary>
        public ICollection<string> Categories { get; set; } // category

        /// <summary>
        /// The "generator" element
        /// </summary>
        public string Generator { get; set; }

        /// <summary>
        /// The "cloud" element
        /// </summary>
        public FeedCloud Cloud { get; set; }

        /// <summary>
        /// The time to life "ttl" element
        /// </summary>
        public string TTL { get; set; }

        /// <summary>
        /// All "day" elements in "skipDays"
        /// </summary>
        public ICollection<string> SkipDays { get; set; }

        /// <summary>
        /// All "hour" elements in "skipHours"
        /// </summary>
        public ICollection<string> SkipHours { get; set; }

        /// <summary>
        /// The "textInput" element
        /// </summary>
        public FeedTextInput TextInput { get; set; }

        /// <summary>
        /// All elements starting with "sy:"
        /// </summary>
        public Syndication Sy { get; set; }

        /// <summary>
        /// default constructor (for serialization)
        /// </summary>
        public Rss20Feed()
            : base() { }

        /// <summary>
        /// Reads a rss 2.0 feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml"></param>
        /// <param name="channel"></param>
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
            this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString);
            this.LastBuildDateString = channel.GetValue("lastBuildDate");
            this.LastBuildDate = Helpers.TryParseDateTime(this.LastBuildDateString);

            if(this.Language != null && (this.PublishingDate == null || this.LastBuildDate == null))
            {
                CultureInfo culture = null;

                try
                {
                    culture = new CultureInfo(this.Language);
                    if (this.PublishingDate == null)
                    {
                        this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString, culture);
                    }

                    if (this.LastBuildDate == null)
                    {
                        this.LastBuildDate = Helpers.TryParseDateTime(this.LastBuildDateString, culture);
                    }
                }
                catch(CultureNotFoundException)
                {
                }
            }

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

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ToFeed()
        {
            Feed f = new Feed(this);

            f.Copyright = this.Copyright;
            f.Description = this.Description;
            f.ImageUrl = this.Image?.Url;
            f.Language = this.Language;
            f.LastUpdatedDate = this.LastBuildDate;
            f.LastUpdatedDateString = this.LastBuildDateString;
            f.Type = FeedType.Rss_2_0;

            return f;
        }
    }
}
