namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Rss 2.0 Image according to specification: https://validator.w3.org/feed/docs/rss2.html#ltimagegtSubelementOfLtchannelgt
    /// </summary>
    public class Rss20FeedImage : Rss091FeedImage
    {
        public Rss20FeedImage(XElement element)
            : base(element)
        {
        }
    }
}
