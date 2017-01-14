namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 1.0 Feed textinput according to specification: http://web.resource.org/rss/1.0/spec
    /// </summary>
    public class Rss10FeedTextInput : FeedTextInput
    {
        public string About { get; set; }

        public Rss10FeedTextInput(XElement element)
            : base(element)
        {
            this.About = element.GetAttribute("rdf:about").GetValue();
        }
    }
}
