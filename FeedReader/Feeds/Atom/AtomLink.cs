namespace CodeHollow.FeedReader.Feeds
{
    using System.Collections.Generic;
    using System.Linq;
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
        /// Initializes a new instance of the <see cref="AtomLink"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public AtomLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomLink"/> class.
        /// Reads an atom link based on the xml given in element
        /// </summary>
        /// <param name="element">link as xml</param>
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
        /// Returns the href of the main link: rel="alternate" (or no rel, which defaults to alternate per spec),
        /// preferring type text/html. Falls back to the first link.
        /// </summary>
        internal static string GetMainHref(ICollection<AtomLink> links)
        {
            var alternates = links.Where(x => string.IsNullOrEmpty(x.Relation) || x.Relation.EqualsIgnoreCase("alternate")).ToList();
            var main = alternates.FirstOrDefault(x => x.LinkType.EqualsIgnoreCase("text/html"))
                ?? alternates.FirstOrDefault()
                ?? links.FirstOrDefault();
            return main?.Href;
        }

        /// <summary>
        /// Returns the Href of the link
        /// </summary>
        /// <returns>the href of the link</returns>
        public override string ToString()
        {
            return this.Href;
        }
    }
}
