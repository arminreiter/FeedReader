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
        /// <summary>
        /// The "author" element
        /// </summary>
        public AtomPerson Author { get; set; }

        /// <summary>
        /// All "category" elements
        /// </summary>
        public ICollection<string> Categories { get; set; }

        /// <summary>
        /// All "category" elements with their attributes (domain, term, scheme, label)
        /// </summary>
        public ICollection<FeedCategory> CategoryDetails { get; set; }

        /// <summary>
        /// The "content" element
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The "contributor" element
        /// </summary>
        public AtomPerson Contributor { get; set; }

        /// <summary>
        /// The "id" element
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The "published" date as string
        /// </summary>
        public string PublishedDateString { get; set; }

        /// <summary>
        /// The "published" element as DateTime. Null if parsing failed or published is empty.
        /// </summary>
        public DateTime? PublishedDate { get; set; }

        /// <summary>
        /// The "rights" element
        /// </summary>
        public string Rights { get; set; }

        /// <summary>
        /// The "source" element
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The "summary" element
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// The "updated" element
        /// </summary>
        public string UpdatedDateString { get; set; }

        /// <summary>
        /// The "updated" element as DateTime. Null if parsing failed or updated is empty
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// All "link" elements
        /// </summary>
        public ICollection<AtomLink> Links { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomFeedItem"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public AtomFeedItem()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomFeedItem"/> class.
        /// Reads an atom feed based on the xml given in item
        /// </summary>
        /// <param name="item">feed item as xml</param>
        public AtomFeedItem(XElement item)
            : base(item)
        {
            this.Links = item.GetElements("link").Select(x => new AtomLink(x)).ToList();
            this.Link = AtomLink.GetMainHref(this.Links);

            this.Author = new AtomPerson(item.GetElement("author"));

            var categories = item.GetElements("category");
            this.Categories = categories.Select(GetCategoryName).ToList();
            this.CategoryDetails = categories.Select(x => new FeedCategory(x)).ToList();

            this.Content = item.GetValue("content").HtmlDecode();
            this.Contributor = new AtomPerson(item.GetElement("contributor"));
            this.Id = item.GetValue("id");

            // Atom 0.3 fallbacks: issued/created -> published, modified -> updated
            this.PublishedDateString = item.GetValue("published") ?? item.GetValue("issued") ?? item.GetValue("created");
            this.PublishedDate = Helpers.TryParseDateTime(this.PublishedDateString);

            this.Rights = item.GetValue("rights");
            this.Source = item.GetValue("source");
            this.Summary = item.GetValue("summary");

            this.UpdatedDateString = item.GetValue("updated") ?? item.GetValue("modified");
            this.UpdatedDate = Helpers.TryParseDateTime(this.UpdatedDateString);
        }

        /// <summary>
        /// Human-readable name of an atom category: the optional "label", else the "term" identifier
        /// (often already readable), else the element text (non-standard feeds). Issue #56.
        /// </summary>
        internal static string GetCategoryName(XElement category) => new FeedCategory(category).Name;

        /// <inheritdoc/>
        internal override FeedItem ToFeedItem()
        {
            FeedItem fi = new FeedItem(this)
            {
                Author = this.Author?.ToString(),
                Categories = this.Categories,
                Content = this.Content,
                Description = this.Summary,
                Id = this.Id,
                PublishingDate = this.PublishedDate,
                PublishingDateString = this.PublishedDateString
            };
            return fi;
        }
    }
}
