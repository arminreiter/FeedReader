namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// JSON Feed item according to specification: https://jsonfeed.org/version/1.1
    /// </summary>
    public class JsonFeedItem : BaseFeedItem
    {
        /// <summary>
        /// The "id" field. Numeric ids are returned as string.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The "url" field. Also available as <see cref="BaseFeedItem.Link"/>.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The "external_url" field
        /// </summary>
        public string ExternalUrl { get; set; }

        /// <summary>
        /// The "content_html" field
        /// </summary>
        public string ContentHtml { get; set; }

        /// <summary>
        /// The "content_text" field
        /// </summary>
        public string ContentText { get; set; }

        /// <summary>
        /// The "summary" field
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The "image" field
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The "banner_image" field
        /// </summary>
        public string BannerImage { get; set; }

        /// <summary>
        /// The "date_published" field as string
        /// </summary>
        public string DatePublishedString { get; set; }

        /// <summary>
        /// The "date_published" field as DateTime (utc). Null if parsing failed or the field is empty.
        /// </summary>
        public DateTime? DatePublished { get; set; }

        /// <summary>
        /// The "date_modified" field as string
        /// </summary>
        public string DateModifiedString { get; set; }

        /// <summary>
        /// The "date_modified" field as DateTime (utc). Null if parsing failed or the field is empty.
        /// </summary>
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// The "authors" field. Falls back to the deprecated "author" field and then, as defined by the spec,
        /// to the authors of the feed - only if the item has neither (an empty "authors" list is kept empty).
        /// </summary>
        public ICollection<JsonFeedAuthor> Authors { get; set; }

        /// <summary>
        /// The "tags" field
        /// </summary>
        public ICollection<string> Tags { get; set; }

        /// <summary>
        /// The "language" field
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The "attachments" field
        /// </summary>
        public ICollection<JsonFeedAttachment> Attachments { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedItem"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public JsonFeedItem()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedItem"/> class.
        /// </summary>
        /// <param name="item">the json item converted to xml</param>
        /// <param name="feedAuthors">authors of the feed, used if the item has no own authors</param>
        public JsonFeedItem(XElement item, ICollection<JsonFeedAuthor> feedAuthors = null)
            : base(item)
        {
            this.Id = JsonFeed.GetJsonValue(item, "id");
            this.Url = JsonFeed.GetJsonValue(item, "url");
            this.Link = this.Url;
            this.ExternalUrl = JsonFeed.GetJsonValue(item, "external_url");
            this.Title = JsonFeed.GetJsonValue(item, "title");
            this.ContentHtml = JsonFeed.GetJsonValue(item, "content_html");
            this.ContentText = JsonFeed.GetJsonValue(item, "content_text");
            this.Summary = JsonFeed.GetJsonValue(item, "summary");
            this.Image = JsonFeed.GetJsonValue(item, "image");
            this.BannerImage = JsonFeed.GetJsonValue(item, "banner_image");

            this.DatePublishedString = JsonFeed.GetJsonValue(item, "date_published");
            this.DatePublished = JsonFeed.ParseJsonDate(this.DatePublishedString);
            this.DateModifiedString = JsonFeed.GetJsonValue(item, "date_modified");
            this.DateModified = JsonFeed.ParseJsonDate(this.DateModifiedString);

            // spec: the feed authors apply only if the item doesn't specify authors - an empty list is specified
            this.Authors = JsonFeed.GetAuthors(item) ?? feedAuthors?.ToList() ?? new List<JsonFeedAuthor>();

            this.Tags = JsonFeed.GetJsonArray(item, "tags")
                .Where(x => x.Attribute("type")?.Value == "string")
                .Select(x => x.Value)
                .ToList();

            this.Language = JsonFeed.GetJsonValue(item, "language");
            this.Attachments = JsonFeed.GetJsonArray(item, "attachments")
                .Select(x => new JsonFeedAttachment(x))
                .Where(x => x.Url != null)
                .ToList();
        }

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            var names = this.Authors.Select(x => x.Name).Where(x => !string.IsNullOrEmpty(x)).ToList();

            FeedItem fi = new FeedItem(this)
            {
                Author = names.Count > 0 ? string.Join(", ", names) : null,
                Categories = this.Tags,
                Content = this.ContentHtml ?? this.ContentText,
                Description = this.Summary,
                Id = this.Id,
                PublishingDate = this.DatePublished,
                PublishingDateString = this.DatePublishedString
            };
            return fi;
        }
    }
}
