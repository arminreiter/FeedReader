namespace CodeHollow.FeedReader.Parser
{
    public interface IFeedParser
    {
        Feed Parse(string feedXml);
    }
}
