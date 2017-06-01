namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// default text input object for Rss 0.91, 0.92, 1.0, 2.0 and ATOM
    /// </summary>
    public class FeedTextInput
    {
        /// <summary>
        /// The "title" element
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The "link" element
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// The "description" field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The "name" element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedTextInput"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public FeedTextInput()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedTextInput"/> class.
        /// Reads a rss textInput element based on the xml given in element
        /// </summary>
        /// <param name="element">text input element as xml</param>
        public FeedTextInput(XElement element)
        {
            this.Title = element.GetValue("title");
            this.Link = element.GetValue("link");
            this.Description = element.GetValue("description");
            this.Name = element.GetValue("name");
        }
    }
}
