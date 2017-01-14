namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 1.0 Feed image according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public class Rss10FeedImage : FeedImage
    {
        public string About { get; set; }

        public Rss10FeedImage(XElement element)
            : base(element)
        {
            this.About = element.GetAttribute("rdf:about").GetValue();
        }
    }
}
