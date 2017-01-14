namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Atom 1.0 link according to specification: https://validator.w3.org/feed/docs/atom.html#link
    /// </summary>
    public class AtomLink
    {
        public string Href { get; set; }

        public string Relation { get; set; } // rel

        public string LinkType { get; set; } // type

        public string HrefLanguage { get; set; } // hreflang

        public string Title { get; set; }

        public int? Length { get; set; }

        public AtomLink(XElement element)
        {
            this.Href = element.GetAttributeValue("href");
            this.Relation = element.GetAttributeValue("rel");
            this.LinkType = element.GetAttributeValue("type");
            this.HrefLanguage = element.GetAttributeValue("hreflang");
            this.Title = element.GetAttributeValue("title");
            this.Length = Helpers.TryParseInt(element.GetAttributeValue("length"));
        }

        public override string ToString()
        {
            return this.Href;
        }
    }
}
