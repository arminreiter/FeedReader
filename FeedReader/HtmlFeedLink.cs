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
        /// default constructor (for serialization)
        /// </summary>
        public HtmlFeedLink()
        {
        }

        /// <summary>
        /// Creates an html feed link item
        /// </summary>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <param name="feedtype"></param>
        public HtmlFeedLink(string title, string url, FeedType feedtype)
        {
            this.Title = title;
            this.Url = url;
            this.FeedType = feedtype;
        }
    }
}
