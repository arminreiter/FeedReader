namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// A JSON Feed hub object (real-time notification endpoint, e.g. WebSub)
    /// </summary>
    public class JsonFeedHub
    {
        /// <summary>
        /// The "type" field, e.g. WebSub or rssCloud
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The "url" field
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedHub"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public JsonFeedHub()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedHub"/> class.
        /// </summary>
        /// <param name="hub">the json hub object converted to xml</param>
        public JsonFeedHub(XElement hub)
        {
            this.Type = JsonFeed.GetJsonValue(hub, "type");
            this.Url = JsonFeed.GetJsonValue(hub, "url");
        }
    }
}
