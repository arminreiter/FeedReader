namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:soundbite element - a short clip of the episode, e.g. for previews
    /// </summary>
    public class PodcastSoundbite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastSoundbite"/> class.
        /// </summary>
        /// <param name="element">the podcast:soundbite element</param>
        public PodcastSoundbite(XElement element)
        {
            StartTime = PodcastNamespace.ParseSeconds(PodcastNamespace.Attribute(element, "startTime"));
            Duration = PodcastNamespace.ParseSeconds(PodcastNamespace.Attribute(element, "duration"));
            Title = element.Value;
        }

        /// <summary>
        /// The startTime attribute. Null if it is missing or not a number of seconds.
        /// </summary>
        public TimeSpan? StartTime { get; }

        /// <summary>
        /// The duration attribute. Null if it is missing or not a number of seconds.
        /// </summary>
        public TimeSpan? Duration { get; }

        /// <summary>
        /// The title of the soundbite (node value), may be empty
        /// </summary>
        public string Title { get; }
    }
}
