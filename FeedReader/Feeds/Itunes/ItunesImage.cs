namespace CodeHollow.FeedReader.Feeds.Itunes
{
    public class ItunesImage
    {
        internal ItunesImage(string href)
        {
            Href = href;
        }

        public string Href { get; }
    }
}
