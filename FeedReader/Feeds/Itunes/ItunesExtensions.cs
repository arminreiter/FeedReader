namespace CodeHollow.FeedReader.Feeds.Itunes
{
    public static class ItunesExtensions
    {
        public static ItunesChannel GetItunesChannel(this Feed feed)
        {
            return new ItunesChannel(feed.SpecificFeed.Element);
        }

        public static ItunesItem GetItunesItem(this FeedItem item)
        {
            return new ItunesItem(item.SpecificItem.Element);
        }
    }
}
