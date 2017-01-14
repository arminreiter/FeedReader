namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// default text input object for Rss 0.91, 0.92, 1.0, 2.0 and ATOM
    /// </summary>
    public class FeedTextInput
    {
        public string Title { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public FeedTextInput(XElement element)
        {
            this.Title = element.GetValue("title");
            this.Link = element.GetValue("link");
            this.Description = element.GetValue("description");
            this.Name = element.GetValue("name");
        }
    }
}
