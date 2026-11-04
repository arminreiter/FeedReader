namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// A JSON Feed author object
    /// </summary>
    public class JsonFeedAuthor
    {
        /// <summary>
        /// The "name" field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The "url" field
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The "avatar" field
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedAuthor"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public JsonFeedAuthor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedAuthor"/> class.
        /// </summary>
        /// <param name="author">the json author object converted to xml</param>
        public JsonFeedAuthor(XElement author)
        {
            this.Name = JsonFeed.GetJsonValue(author, "name");
            this.Url = JsonFeed.GetJsonValue(author, "url");
            this.Avatar = JsonFeed.GetJsonValue(author, "avatar");
        }
    }
}
