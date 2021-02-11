namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Media RSS 2.0 feed according to specification: http://www.rssboard.org/media-rss
    /// </summary>
    public class MediaRssFeed : BaseFeed
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
        /// Initializes a new instance of the <see cref="MediaRssFeed"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public MediaRssFeed()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaRssFeed"/> class.
        /// Reads a Media Rss feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml">the entire feed xml as string</param>
        /// <param name="channel">the "channel" element in the xml as XElement</param>
        public MediaRssFeed(string feedXml, XElement channel)
            : base(feedXml, channel)
        {
            this.Description = channel.GetValue("description");
            this.Language = channel.GetValue("language");
            this.Copyright = channel.GetValue("copyright");
            this.ManagingEditor = channel.GetValue("managingEditor");
            this.WebMaster = channel.GetValue("webMaster");
            this.Docs = channel.GetValue("docs");
            this.PublishingDateString = channel.GetValue("pubDate");
            this.LastBuildDateString = channel.GetValue("lastBuildDate");
            this.ParseDates(this.Language, this.PublishingDateString, this.LastBuildDateString);

            var categories = channel.GetElements("category");
            this.Categories = categories.Select(x => x.GetValue()).ToList();

            this.Sy = new Syndication(channel);
            this.Generator = channel.GetValue("generator");
            this.TTL = channel.GetValue("ttl");
            this.Image = new MediaRssFeedImage(channel.GetElement("image"));
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
                this.Items.Add(new MediaRssFeedItem(item));
            }
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
                Type = FeedType.MediaRss
            };
            return f;
        }

        /// <summary>
        /// Sets the PublishingDate and LastBuildDate. If parsing fails, then it checks if the language
        /// is set and tries to parse the date based on the culture of the language.
        /// </summary>
        /// <param name="language">language of the feed</param>
        /// <param name="publishingDate">publishing date as string</param>
        /// <param name="lastBuildDate">last build date as string</param>
        private void ParseDates(string language, string publishingDate, string lastBuildDate)
        {
            this.PublishingDate = Helpers.TryParseDateTime(publishingDate);
            this.LastBuildDate = Helpers.TryParseDateTime(lastBuildDate);

            // check if language is set - if so, check if dates could be parsed or try to parse it with culture of the language
            if (string.IsNullOrWhiteSpace(language))
                return;

            // if publishingDateString is set but PublishingDate is null - try to parse with culture of "Language" property
            bool parseLocalizedPublishingDate = this.PublishingDate == null && !string.IsNullOrWhiteSpace(this.PublishingDateString);

            // if LastBuildDateString is set but LastBuildDate is null - try to parse with culture of "Language" property
            bool parseLocalizedLastBuildDate = this.LastBuildDate == null && !string.IsNullOrWhiteSpace(this.LastBuildDateString);

            // if both dates are set - return
            if (!parseLocalizedPublishingDate && !parseLocalizedLastBuildDate)
                return;

            // dates are set, but one of them couldn't be parsed - so try again with the culture set to the language
            CultureInfo culture;
            try
            {
                culture = new CultureInfo(this.Language);
            }
            catch (CultureNotFoundException)
            {
                // should be replace by a try parse or by getting all cultures and selecting the culture
                // out of the collection. That's unfortunately not available in .net standard 1.3 for now
                // this case should never happen, but in some rare cases it does - so catching the exception
                // is acceptable in that case.
                return; // culture couldn't be found, return as it was already tried to parse with default values
            }

            if (parseLocalizedPublishingDate)
            {
                this.PublishingDate = Helpers.TryParseDateTime(this.PublishingDateString, culture);
            }

            if (parseLocalizedLastBuildDate)
            {
                this.LastBuildDate = Helpers.TryParseDateTime(this.LastBuildDateString, culture);
            }
        }
    }
}
