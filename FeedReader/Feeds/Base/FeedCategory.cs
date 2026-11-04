namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// A "category" element with its attributes - rss: &lt;category domain="..."&gt;text&lt;/category&gt;,
    /// atom: &lt;category term="..." scheme="..." label="..." /&gt;
    /// </summary>
    public class FeedCategory
    {
        /// <summary>
        /// Human-readable name, same value as in the Categories string list: atom "label", else "term", else the element text
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Taxonomy the category belongs to: rss "domain" or atom "scheme" attribute
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// The atom "term" attribute
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// The atom "label" attribute
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedCategory"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public FeedCategory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedCategory"/> class.
        /// Reads a rss or atom category element
        /// </summary>
        /// <param name="element">the category element</param>
        public FeedCategory(XElement element)
        {
            this.Term = element.GetAttributeValue("term");
            this.Label = element.GetAttributeValue("label");
            this.Scheme = element.GetAttributeValue("domain") ?? element.GetAttributeValue("scheme");
            this.Name = !string.IsNullOrEmpty(this.Label) ? this.Label
                : !string.IsNullOrEmpty(this.Term) ? this.Term
                : element.Value;
        }
    }
}
