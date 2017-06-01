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
        /// <summary>
        /// The required field "description"
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The required field "language"
        /// /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The "copyright" field
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// The "docs" field
        /// </summary>
        public string Docs { get; set; }

        /// <summary>
        /// The "image" element
        /// </summary>
        public FeedImage Image { get; set; }

        /// <summary>
        /// The "lastBuildDate" element
        /// </summary>
        public string LastBuildDateString { get; set; }

        /// <summary>
        /// The "lastBuildDate" as DateTime. Null if parsing failed or lastBuildDate is empty.
        /// </summary>
        public DateTime? LastBuildDate { get; set; }

        /// <summary>
        /// The "managingEditor" field
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
        /// The "rating" field
        /// </summary>
        public string Rating { get; set; }

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
        /// The "webMaster" element
        /// </summary>
        public string WebMaster { get; set; }

        /// <summary>
        /// All elements that start with "sy:"
        /// </summary>
        public Syndication Sy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss091Feed"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public Rss091Feed()
            : base()
        {
            this.SkipDays = new List<string>();
            this.SkipHours = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rss091Feed"/> class.
        /// Reads a rss 0.91 feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml">the entire feed xml as string</param>
        /// <param name="channel">the "channel" element in the xml as XElement</param>
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

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ToFeed()
        {
            Feed f = new Feed(this)
            {
                Copyright = this.Copyright,
                Description = this.Description,
                ImageUrl = this.Image?.Url,
                Language = this.Language,
                LastUpdatedDate = this.LastBuildDate,
                LastUpdatedDateString = this.LastBuildDateString,
                Type = FeedType.Rss_0_91
            };
            return f;
        }

        /// <summary>
        /// Adds feed items to the items collection
        /// </summary>
        /// <param name="items">feed items as XElements</param>
        internal virtual void AddItems(IEnumerable<XElement> items)
        {
            foreach (var item in items)
            {
                this.Items.Add(new Rss091FeedItem(item));
            }
        }
    }
}
