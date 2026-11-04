namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System;
    using System.Globalization;
    using System.Xml.Linq;

    /// <summary>
    /// The podcast:updateFrequency element
    /// </summary>
    public class PodcastUpdateFrequency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PodcastUpdateFrequency"/> class.
        /// </summary>
        /// <param name="element">the podcast:updateFrequency element</param>
        public PodcastUpdateFrequency(XElement element)
        {
            Text = element.Value;
            Complete = PodcastNamespace.Attribute(element, "complete").EqualsIgnoreCase("true");
            DtStartString = PodcastNamespace.Attribute(element, "dtstart");
            DtStart = Helpers.TryParseDateTime(DtStartString, CultureInfo.InvariantCulture);
            RRule = PodcastNamespace.Attribute(element, "rrule");
        }

        /// <summary>
        /// Free-form text to display, e.g. "Every Monday" (node value)
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// The complete attribute - true if no more episodes will be released
        /// </summary>
        public bool Complete { get; }

        /// <summary>
        /// The dtstart attribute (ISO 8601) as string
        /// </summary>
        public string DtStartString { get; }

        /// <summary>
        /// The dtstart attribute as DateTime (utc). Null if parsing failed or it is empty.
        /// </summary>
        public DateTime? DtStart { get; }

        /// <summary>
        /// The rrule attribute - iCalendar recurrence rule (RFC 5545), e.g. "FREQ=WEEKLY"
        /// </summary>
        public string RRule { get; }
    }
}
