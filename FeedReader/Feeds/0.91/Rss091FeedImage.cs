namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 0.91 Feed Image according to specification: http://www.rssboard.org/rss-0-9-1-netscape#image
    /// </summary>
    public class Rss091FeedImage : FeedImage
    {
        public string Description { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public Rss091FeedImage(XElement element)
            : base(element)
        {
            this.Description = element.GetValue("description");
            this.Width = Helpers.TryParseInt(element.GetValue("width"));
            this.Height = Helpers.TryParseInt(element.GetValue("height"));
        }
    }
}
