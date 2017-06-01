namespace CodeHollow.FeedReader.Feeds.Itunes
{
    public class ItunesCategory
    {
        internal ItunesCategory(string text, ItunesCategory[] children)
        {
            Text = text;
            Children = children;
        }

        public string Text { get; }
        public ItunesCategory[] Children { get; }
    }
}
