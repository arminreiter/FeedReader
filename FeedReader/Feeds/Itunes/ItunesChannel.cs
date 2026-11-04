namespace CodeHollow.FeedReader.Feeds.Itunes
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// The basic itunes: elements that are part of the channel xml element of an rss2.0 feed
    /// </summary>
    public class ItunesChannel
    {
        internal const string NAMESPACEPREFIX = "itunes";
        /// <summary>
        /// Initializes a new instance of the <see cref="ItunesChannel"/> class.
        /// </summary>
        /// <param name="channelElement"></param>
        public ItunesChannel(XElement channelElement)
        {
            Author = channelElement.GetValue(NAMESPACEPREFIX, "author");
            Title = channelElement.GetValue(NAMESPACEPREFIX, "title");
            Block = channelElement.GetValue(NAMESPACEPREFIX, "block").EqualsIgnoreCase("yes");
            Categories = GetItunesCategories(channelElement);

            var imageElement = channelElement.GetElement(NAMESPACEPREFIX, "image");
            if (imageElement != null)
            {
                Image = new ItunesImage(imageElement);
            }

            var explicitValue = channelElement.GetValue(NAMESPACEPREFIX, "explicit");
            Explicit = explicitValue.EqualsIgnoreCase("yes", "explicit", "true");
            
            Complete = channelElement.GetValue(NAMESPACEPREFIX, "complete").EqualsIgnoreCase("yes");

            if (Uri.TryCreate(channelElement.GetValue(NAMESPACEPREFIX, "new-feed-url"), UriKind.Absolute, out var newFeedUrl))
            {
                NewFeedUrl = newFeedUrl;
            }

            var ownerElement = channelElement.GetElement(NAMESPACEPREFIX, "owner");

            if (ownerElement != null)
            {
                Owner = new ItunesOwner(ownerElement);
            }

            Subtitle = channelElement.GetValue(NAMESPACEPREFIX, "subtitle");
            Summary = channelElement.GetValue(NAMESPACEPREFIX, "summary");
            Type = channelElement.GetValue(NAMESPACEPREFIX, "type");
            Keywords = ParseKeywords(channelElement.GetValue(NAMESPACEPREFIX, "keywords"));
        }

        internal static string[] ParseKeywords(string keywords)
        {
            if (string.IsNullOrWhiteSpace(keywords))
                return new string[0];
            return keywords.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
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
        /// The itunes:category elements
        /// </summary>
        public ItunesCategory[] Categories { get; }

        /// <summary>
        /// The itunes:image element
        /// </summary>
        public ItunesImage Image { get; }

        /// <summary>
        /// The itunes:explicit element
        /// </summary>
        public bool Explicit { get; }

        /// <summary>
        /// The itunes:complete element
        /// </summary>
        public bool Complete { get; }

        /// <summary>
        /// The itunes:new-feed-url element
        /// </summary>
        public Uri NewFeedUrl { get; }

        /// <summary>
        /// The itunes:owner element
        /// </summary>
        public ItunesOwner Owner { get; }

        /// <summary>
        /// The itunes:subtitle element
        /// </summary>
        public string Subtitle { get; }

        /// <summary>
        /// The itunes:summary element
        /// </summary>
        public string Summary { get; }

        /// <summary>
        /// The itunes:type element (episodic or serial)
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The itunes:title element
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The itunes:keywords element, split by comma
        /// </summary>
        public string[] Keywords { get; }

        /// <summary>
        /// Sets the itunes categories
        /// </summary>
        /// <param name="element">the channel element</param>
        /// <returns>the itunes:categries</returns>
        private ItunesCategory[] GetItunesCategories(XElement element)
        {
            var categoryElements = element.GetElements(NAMESPACEPREFIX, "category");
            if (categoryElements == null)
                return new ItunesCategory[0];

            var query = from categoryElement in categoryElements
                let children = GetItunesCategories(categoryElement)
                select new ItunesCategory(categoryElement.GetAttributeValue("text"), children);

            return query.ToArray();
        }
    }
}
