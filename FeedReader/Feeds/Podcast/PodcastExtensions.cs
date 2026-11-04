namespace CodeHollow.FeedReader.Feeds.Podcast
{
    /// <summary>
    /// Extension methods to access the podcast: (Podcasting 2.0) tags of a feed
    /// </summary>
    public static class PodcastExtensions
    {
        /// <summary>
        /// Returns the podcast: elements of a rss feed. Properties are empty if the feed doesn't use the namespace.
        /// </summary>
        /// <param name="feed">the rss feed</param>
        /// <returns>PodcastChannel object which contains podcast:... properties</returns>
        public static PodcastChannel GetPodcastChannel(this Feed feed)
        {
            return new PodcastChannel(feed.SpecificFeed.Element);
        }

        /// <summary>
        /// Returns the podcast: elements of a rss feed item. Properties are empty if the item doesn't use the namespace.
        /// </summary>
        /// <param name="item">the rss feed item</param>
        /// <returns>PodcastItem object which contains podcast:... properties</returns>
        public static PodcastItem GetPodcastItem(this FeedItem item)
        {
            return new PodcastItem(item.SpecificItem.Element);
        }
    }
}
