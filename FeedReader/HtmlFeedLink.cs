namespace CodeHollow.FeedReader
{
    public class HtmlFeedLink
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public FeedType FeedType { get; set; }

        public HtmlFeedLink()
        {
        }

        public HtmlFeedLink(string title, string url, FeedType feedtype)
        {
            this.Title = title;
            this.Url = url;
            this.FeedType = feedtype;
        }
    }
}
