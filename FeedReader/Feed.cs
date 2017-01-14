namespace CodeHollow.FeedReader
{
    using System.Collections.Generic;

    public class Feed
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public ICollection<FeedItem> Items { get; set; }

        /// <summary>
        /// Gets the whole, original feed as string
        /// </summary>
        public string OriginalDocument { get; private set; }

        public Feed()
        {
            this.Items = new List<FeedItem>();
        }

        public Feed(string feedXml, System.Xml.Linq.XElement xelement)
            : this()
        {
            this.OriginalDocument = feedXml;

            this.Title = xelement.GetValue("title");
            this.Link = xelement.GetValue("link");
        }
    }
}
