namespace CodeHollow.FeedReader
{
    /// <summary>
    /// An html feed link, containing the title, the url and the type of the feed
    /// </summary>
    public class HtmlFeedLink
    {
        /// <summary>
        /// The title of the feed
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The url to the feed
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The type of the feed - rss or atom
        /// </summary>
        public FeedType FeedType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlFeedLink"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public HtmlFeedLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlFeedLink"/> class.
        /// Creates an html feed link item
        /// </summary>
        /// <param name="title">title fo the feed</param>
        /// <param name="url">url of the feed</param>
        /// <param name="feedtype">type of the feed (rss 1.0, 2.0, ...)</param>
        public HtmlFeedLink(string title, string url, FeedType feedtype)
        {
            this.Title = title;
            this.Url = url;
            this.FeedType = feedtype;
        }
    }
}
