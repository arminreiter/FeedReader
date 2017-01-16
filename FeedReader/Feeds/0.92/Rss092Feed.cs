using System.Collections.Generic;
using System.Xml.Linq;

namespace CodeHollow.FeedReader.Feeds
{
    public class Rss092Feed : Rss091Feed
    {
        public FeedCloud Cloud { get; set; }

        public Rss092Feed(string feedXml, XElement channel) : base(feedXml, channel)
        {
            this.Cloud = new FeedCloud(channel.GetElement("cloud"));
        }

        internal override void AddItems(IEnumerable<XElement> items)
        {
            foreach (var item in items)
            {
                this.Items.Add(new Rss092FeedItem(item));
            }
        }

        public override Feed ToFeed()
        {
            var feed = base.ToFeed();
            feed.SpecificFeed = this;
            feed.Type = FeedType.Rss_0_92;
            return feed;
        }
    }
}
