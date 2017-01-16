using System.Collections.Generic;
using System.Xml.Linq;

namespace CodeHollow.FeedReader.Feeds
{
    /// <summary>
    /// Rss 0.92 feed according to specification: http://backend.userland.com/rss092
    /// </summary>
    public class Rss092Feed : Rss091Feed
    {
        /// <summary>
        /// The "cloud" field
        /// </summary>
        public FeedCloud Cloud { get; set; }

        /// <summary>
        /// default constructor (for serialization)
        /// </summary>
        public Rss092Feed() : base()
        { }

        /// <summary>
        /// Reads a rss 0.92 feed based on the xml given in channel
        /// </summary>
        /// <param name="feedXml"></param>
        /// <param name="channel"></param>
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

        /// <summary>
        /// Creates the base <see cref="Feed"/> element out of this feed.
        /// </summary>
        /// <returns>feed</returns>
        public override Feed ToFeed()
        {
            var feed = base.ToFeed();
            feed.SpecificFeed = this;
            feed.Type = FeedType.Rss_0_92;
            return feed;
        }
    }
}
