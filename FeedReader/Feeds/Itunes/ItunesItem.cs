using System;
using System.Xml.Linq;

namespace CodeHollow.FeedReader.Feeds.Itunes
{
    /// <summary>
    /// The itunes:... elements of an rss 2.0 item
    /// </summary>
    public class ItunesItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesItem"/> class.
        /// </summary>
        /// <param name="itemElement"></param>
        public ItunesItem(XElement itemElement)
        {
            Author = itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "author");
            Block = itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "block").EqualsIgnoreCase("yes");
            var imageElement = itemElement.GetElement(ItunesChannel.NAMESPACEPREFIX, "image");

            if (imageElement != null)
            {
                Image = new ItunesImage(imageElement);
            }

            var duration = itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "duration");
            Duration = ParseDuration(duration);

            var explicitValue = itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "explicit");
            Explicit = explicitValue.EqualsIgnoreCase("yes", "explicit", "true");

            IsClosedCaptioned = itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "isClosedCaptioned").EqualsIgnoreCase("yes");

            if (int.TryParse(itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "order"), out var order))
            {
                Order = order;
            }

            Subtitle = itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "subtitle");
            Summary = itemElement.GetValue(ItunesChannel.NAMESPACEPREFIX, "summary");
        }

        private static TimeSpan? ParseDuration(string duration)
        {
            if (String.IsNullOrWhiteSpace(duration))
                return null;

            var durationArray = duration.Split(':');

            if (durationArray.Length == 1 && long.TryParse(durationArray[0], out long result))
            {
                return TimeSpan.FromSeconds(result);
            }

            if (durationArray.Length == 2 && int.TryParse(durationArray[0], out int minutes) &&
                    int.TryParse(durationArray[1], out int seconds))
            {
                return new TimeSpan(0, minutes, seconds);
            }

            if (durationArray.Length == 3 && int.TryParse(durationArray[0], out int hours) &&
                    int.TryParse(durationArray[1], out int min) &&
                    int.TryParse(durationArray[2], out int sec))
            {
                return new TimeSpan(hours, min, sec);
            }

            return null;
        }

        /// <summary>
        /// The itunes:author element
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// The itunes:block element
        /// </summary>
        public bool Block { get; }

        /// <summary>
        /// The itunes:image element
        /// </summary>
        public ItunesImage Image { get; }

        /// <summary>
        /// The itunes:duration element
        /// </summary>
        public TimeSpan? Duration { get; }

        /// <summary>
        /// The itunes:explicit element
        /// </summary>
        public bool Explicit { get; }

        /// <summary>
        /// The itunes:isClosedCaptioned element
        /// </summary>
        public bool IsClosedCaptioned { get; }

        /// <summary>
        /// The itunes:order element
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// The itunes:subtitle element
        /// </summary>
        public string Subtitle { get; }

        /// <summary>
        /// The itunes:summary element
        /// </summary>
        public string Summary { get; }
    }
}
