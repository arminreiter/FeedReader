using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CodeHollow.FeedReader.Feeds.Itunes
{
    public class ItunesChannel
    {
        public ItunesChannel(XElement channelElement)
        {

            Author = channelElement.GetValue("itunes", "author");
            Block = channelElement.GetValue("itunes", "block") == "Yes";
            Categories = GetItunesCategories(channelElement);

            var imageElement = channelElement.GetElement("itunes", "image");

            if (imageElement != null)
            {
                Image = new ItunesImage(imageElement.GetAttributeValue("href"));
            }

            var explicitValue = channelElement.GetValue("itunes", "explicit");

            Explicit = explicitValue == "Yes" || explicitValue == "Explicit" || explicitValue == "True";
            Complete = channelElement.GetValue("itunes", "complete") == "Yes";

            if (Uri.TryCreate(channelElement.GetValue("itunes", "new-feed-url"), UriKind.Absolute, out var newFeedUrl))
            {
                NewFeedUrl = newFeedUrl;
            }

            var ownerElement = channelElement.GetElement("itunes", "owner");

            if(ownerElement != null)
            {
                Owner = new ItunesOwner(ownerElement.GetValue("itunes", "name"), ownerElement.GetValue("itunes", "email"));
            }

            Subtitle = channelElement.GetValue("itunes", "subtitle");
            Summary = channelElement.GetValue("itunes", "summary");
        }

        public string Author { get; }
        public bool Block { get; }
        public ItunesCategory[] Categories { get; }
        public ItunesImage Image { get; }
        public bool Explicit { get; }
        public bool Complete { get; }
        public Uri NewFeedUrl { get; }
        public ItunesOwner Owner { get; }
        public string Subtitle { get; }
        public string Summary { get; }

        private ItunesCategory[] GetItunesCategories(XElement element)
        {
            var query = from categoryElement in element.GetElements("itunes", "category")
                        let children = GetItunesCategories(categoryElement)
                        select new ItunesCategory(categoryElement.GetAttributeValue("text"), children);

            return query.ToArray();
        }
    }
}
