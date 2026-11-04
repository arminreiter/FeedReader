namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// JSON Feed 1.0 / 1.1 according to specification: https://jsonfeed.org/version/1.1
    /// The json document is converted to xml (see <see cref="BaseFeed.Element"/>), so extensions
    /// (keys starting with "_") can be read the same way as for xml feeds,
    /// e.g. <c>feed.SpecificFeed.Element.Element("_custom")</c>.
    /// </summary>
    public class JsonFeed : BaseFeed
    {
        /// <summary>
        /// The "version" field, e.g. https://jsonfeed.org/version/1.1
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The "home_page_url" field. Also available as <see cref="BaseFeed.Link"/>.
        /// </summary>
        public string HomePageUrl { get; set; }

        /// <summary>
        /// The "feed_url" field
        /// </summary>
        public string FeedUrl { get; set; }

        /// <summary>
        /// The "description" field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The "user_comment" field
        /// </summary>
        public string UserComment { get; set; }

        /// <summary>
        /// The "next_url" field (pagination)
        /// </summary>
        public string NextUrl { get; set; }

        /// <summary>
        /// The "icon" field
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The "favicon" field
        /// </summary>
        public string Favicon { get; set; }

        /// <summary>
        /// The "authors" field. Falls back to the deprecated version 1.0 "author" field.
        /// </summary>
        public ICollection<JsonFeedAuthor> Authors { get; set; }

        /// <summary>
        /// The "language" field
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The "expired" field. True if the feed will never be updated again.
        /// </summary>
        public bool Expired { get; set; }

        /// <summary>
        /// The "hubs" field
        /// </summary>
        public ICollection<JsonFeedHub> Hubs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeed"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public JsonFeed()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeed"/> class.
        /// </summary>
        /// <param name="feedJson">the entire feed json as string</param>
        /// <param name="root">the json document converted to xml (root element)</param>
        public JsonFeed(string feedJson, XElement root)
            : base(feedJson, root)
        {
            this.Version = GetJsonValue(root, "version");
            this.Title = GetJsonValue(root, "title");
            this.HomePageUrl = GetJsonValue(root, "home_page_url");
            this.Link = this.HomePageUrl;
            this.FeedUrl = GetJsonValue(root, "feed_url");
            this.Description = GetJsonValue(root, "description");
            this.UserComment = GetJsonValue(root, "user_comment");
            this.NextUrl = GetJsonValue(root, "next_url");
            this.Icon = GetJsonValue(root, "icon");
            this.Favicon = GetJsonValue(root, "favicon");
            this.Authors = GetAuthors(root) ?? new List<JsonFeedAuthor>();
            this.Language = GetJsonValue(root, "language");
            this.Expired = GetJsonValue(root, "expired").EqualsIgnoreCase("true");
            this.Hubs = GetJsonArray(root, "hubs").Select(x => new JsonFeedHub(x)).ToList();

            foreach (var item in GetJsonArray(root, "items"))
            {
                var feedItem = new JsonFeedItem(item, this.Authors);

                // spec: items without (or with a blank) id must be discarded
                if (!string.IsNullOrWhiteSpace(feedItem.Id))
                    this.Items.Add(feedItem);
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
                Description = this.Description,
                ImageUrl = this.Icon ?? this.Favicon,
                Language = this.Language,
                Type = FeedType.JsonFeed
            };
            return f;
        }

        /// <summary>
        /// Returns the value of a string, number or boolean json field, null if it is missing,
        /// null, an object or an array.
        /// </summary>
        internal static string GetJsonValue(XElement element, string name)
        {
            var field = element?.Element(name);
            if (field == null)
                return null;

            var type = field.Attribute("type")?.Value;
            if (type == "null" || type == "object" || type == "array")
                return null;

            return field.Value;
        }

        /// <summary>
        /// Returns the entries of a json array field, empty if the field is missing or not an array.
        /// </summary>
        internal static IEnumerable<XElement> GetJsonArray(XElement element, string name)
        {
            var field = element?.Element(name);
            if (field?.Attribute("type")?.Value != "array")
                return Enumerable.Empty<XElement>();

            return field.Elements();
        }

        /// <summary>
        /// Returns the json number as double, null if it isn't a number
        /// </summary>
        internal static double? GetJsonNumber(XElement element, string name)
        {
            // json numbers always use "." - never parse them with the current culture
            if (double.TryParse(GetJsonValue(element, name), NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                return number;
            return null;
        }

        /// <summary>
        /// Returns the json date (RFC 3339) as DateTime (utc), null if parsing failed
        /// </summary>
        internal static DateTime? ParseJsonDate(string date)
        {
            return Helpers.TryParseDateTime(date, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Reads the "authors" array. Only if "authors" is missing, the deprecated json feed 1.0 "author" object is used.
        /// Returns null if neither is set, so callers can tell "not specified" (inherit) from an explicitly empty list.
        /// Author objects without name, url and avatar are skipped.
        /// </summary>
        internal static ICollection<JsonFeedAuthor> GetAuthors(XElement element)
        {
            IEnumerable<XElement> authors;
            var authorsField = element.Element("authors");
            var author = element.Element("author");

            if (authorsField?.Attribute("type")?.Value == "array") // also if empty: "authors": [] is a valid value
                authors = authorsField.Elements();
            else if (author?.Attribute("type")?.Value == "object")
                authors = new[] { author };
            else
                return null; // missing (or json null)

            return authors
                .Where(x => x.Attribute("type")?.Value == "object")
                .Select(x => new JsonFeedAuthor(x))
                .Where(x => x.Name != null || x.Url != null || x.Avatar != null)
                .ToList();
        }
    }
}
