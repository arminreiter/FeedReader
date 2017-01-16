namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Atom 1.0 link according to specification: https://validator.w3.org/feed/docs/atom.html#link
    /// </summary>
    public class AtomLink
    {
        /// <summary>
        /// The "href" element
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// The "rel" element
        /// </summary>
        public string Relation { get; set; } // rel

        /// <summary>
        /// The "type" element
        /// </summary>
        public string LinkType { get; set; } // type

        /// <summary>
        /// The "hreflang" element
        /// </summary>
        public string HrefLanguage { get; set; } // hreflang

        /// <summary>
        /// The "title" element
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The "length" elemnt as int
        /// </summary>
        public int? Length { get; set; }

        /// <summary>
        /// default constructor (for serialization)
        /// </summary>
        public AtomLink()
        { }

        /// <summary>
        /// Reads an atom link based on the xml given in element
        /// </summary>
        /// <param name="element"></param>
        public AtomLink(XElement element)
        {
            this.Href = element.GetAttributeValue("href");
            this.Relation = element.GetAttributeValue("rel");
            this.LinkType = element.GetAttributeValue("type");
            this.HrefLanguage = element.GetAttributeValue("hreflang");
            this.Title = element.GetAttributeValue("title");
            this.Length = Helpers.TryParseInt(element.GetAttributeValue("length"));
        }

        /// <summary>
        /// Returns the Href of the link
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Href;
        }
    }
}
