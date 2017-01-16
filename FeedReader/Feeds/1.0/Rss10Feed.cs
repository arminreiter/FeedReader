namespace CodeHollow.FeedReader.Feeds
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Rss 1.0 Feed according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public class Rss10Feed : BaseFeed
    {
        /// <summary>
        /// The "about" attribute of the element
        /// </summary>
        public string About { get; set; }

        /// <summary>
        /// All elements starting with "dc:"
        /// </summary>
        public DublinCore DC { get; set; }

        /// <summary>
        /// All elements starting with "sy:"
        /// </summary>
        public Syndication Sy { get; set; }

        /// <summary>
        /// The "description" field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The "image" element
        /// </summary>
        public FeedImage Image { get; set; }

        /// <summary>
        /// The "textInput" element
        /// </summary>
        public FeedTextInput TextInput { get; set; }

        /// <summary>
        /// default constructor (for serialization)
        /// </summary>
        public Rss10Feed()
            : base() { }

        /// <summary>
        /// Reads a rss 1.0 feed based on the xml given in xelement
        /// </summary>
        /// <param name="feedXml"></param>
        /// <param name="xelement"></param>
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

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ToFeed()
        {
            Feed f = new Feed(this);

            if (this.DC != null)
            {
                f.Copyright = this.DC.Rights;
                f.Language = this.DC.Language;
                f.LastUpdatedDate = this.DC.Date;
                f.LastUpdatedDateString = this.DC.DateString;
            }

            f.Description = this.Description;
            f.ImageUrl = this.Image?.Url;
            f.Type = FeedType.Rss_1_0;

            return f;
        }
    }
}
