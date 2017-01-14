namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// feed image object that is used in feed (rss 0.91, 2.0, atom, ...)
    /// </summary>
    public class FeedImage
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string Link { get; set; }

        public FeedImage(XElement element)
        {
            this.Title = element.GetValue("title");
            this.Url = element.GetValue("url");
            this.Link = element.GetValue("link");
        }
    }
}
