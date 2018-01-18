namespace CodeHollow.FeedReader.Parser
{
    internal static class Factory
    {
        public static AbstractXmlFeedParser GetParser(FeedType feedType)
        {
            switch (feedType)
            {
                case FeedType.Atom: return new AtomParser();
                case FeedType.Rss_0_91: return new Rss091Parser();
                case FeedType.Rss_0_92: return new Rss092Parser();
                case FeedType.Rss_1_0: return new Rss10Parser();
                case FeedType.MediaRss: return new MediaRssParser();
                default: return new Rss20Parser();
            }
        }
    }
}
