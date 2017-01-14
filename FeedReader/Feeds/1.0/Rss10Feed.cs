namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 1.0 Feed according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public class Rss10Feed : Feed
    {
        public string About { get; set; }

        public DublinCore DC { get; set; }

        public Syndication Sy { get; set; }

        public string Description { get; set; }

        public FeedImage Image { get; set; }

        public FeedTextInput TextInput { get; set; }

        public Rss10Feed()
            : base() { }

        public Rss10Feed(string feedXml, XElement xelement)
            : base(feedXml, xelement)
        {
            this.About = xelement.GetAttribute("rdf:about").GetValue();
            this.DC = new DublinCore(xelement);
            this.Sy = new Syndication(xelement);
            this.Description = xelement.GetValue("description");

            this.Image = new Rss10FeedImage(xelement.Parent.GetElement("image"));
            this.TextInput = new Rss10FeedTextInput(xelement.Parent.GetElement("textinput"));

            var items = xelement.Parent.GetElements("item");
            foreach (var item in items)
            {
                this.Items.Add(new Rss10FeedItem(item));
            }
        }
    }
}
