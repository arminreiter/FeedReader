namespace CodeHollow.FeedReader
{
    /// <summary>
    /// The type of the feed (Rss 0.91, Rss 2.0, Atom, ...)
    /// </summary>
    public enum FeedType
    {
        /// <summary>
        /// Atom Feed
        /// </summary>
        Atom,

        /// <summary>
        /// Rss 0.91 feed
        /// </summary>
        Rss_0_91,

        /// <summary>
        /// Rss 0.92 feed
        /// </summary>
        Rss_0_92,

        /// <summary>
        /// Rss 1.0 feed
        /// </summary>
        Rss_1_0,

        /// <summary>
        /// Rss 2.0 feed
        /// </summary>
        Rss_2_0,

        /// <summary>
        /// Media Rss feed
        /// </summary>
        MediaRss,


        /// <summary>
        /// Rss feed - is used for <see cref="HtmlFeedLink"/> type
        /// </summary>
        Rss,

        /// <summary>
        /// Unknown - default type
        /// </summary>
        Unknown
    }
}
