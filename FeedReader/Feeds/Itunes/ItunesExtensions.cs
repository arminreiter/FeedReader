namespace CodeHollow.FeedReader.Feeds.Itunes
{
    /// <summary>
    /// Extension method that allows to access the itunes tags for a feed
    /// </summary>
    public static class ItunesExtensions
    {
        /// <summary>
        /// Returns the itunes elements of a rss feed
        /// </summary>
        /// <param name="feed">the rss feed</param>
        /// <returns>ItunesChannel object which contains itunes:... properties</returns>
        public static ItunesChannel GetItunesChannel(this Feed feed)
        {
            return new ItunesChannel(feed.SpecificFeed.Element);
        }

        /// <summary>
        /// Returns the itunes element of a rss feeditem
        /// </summary>
        /// <param name="item">the rss feed item</param>
        /// <returns>ItunesItem object which contains itunes:... properties</returns>
        public static ItunesItem GetItunesItem(this FeedItem item)
        {
            return new ItunesItem(item.SpecificItem.Element);
        }
    }
}
