using System;
using System.Xml.Linq;

namespace CodeHollow.FeedReader.Feeds.Itunes
{
    public class ItunesItem
    {
        public ItunesItem(XElement itemElement)
        {
            Author = itemElement.GetValue("itunes", "author");
            Block = itemElement.GetValue("itunes", "block") == "Yes";
            var imageElement = itemElement.GetElement("itunes", "image");

            if (imageElement != null)
            {
                Image = new ItunesImage(imageElement.GetAttributeValue("href"));
            }

            var duration = itemElement.GetValue("itunes", "duration");

            if(duration != null)
            {
                try
                {
                    var durationArray = duration.Split(':');

                    if (durationArray.Length == 1)
                    {
                        Duration = TimeSpan.FromSeconds(long.Parse(durationArray[0]));
                    }
                    else if (durationArray.Length == 2)
                    {
                        Duration = new TimeSpan(0, int.Parse(durationArray[0]), int.Parse(durationArray[1]));
                    }
                    else if (durationArray.Length == 3)
                    {
                        Duration = new TimeSpan(int.Parse(durationArray[0]), int.Parse(durationArray[1]), int.Parse(durationArray[2]));
                    }
                }
                catch
                {
                }
            }

            var explicitValue = itemElement.GetValue("itunes", "explicit");
            Explicit = explicitValue == "Yes" || explicitValue == "Explicit" || explicitValue == "True";

            IsClosedCaptioned = itemElement.GetValue("itunes", "isClosedCaptioned") == "Yes";
            
            if(int.TryParse(itemElement.GetValue("itunes", "order"), out var order))
            {
                Order = order;
            }

            Subtitle = itemElement.GetValue("itunes", "subtitle");
            Summary = itemElement.GetValue("itunes", "summary");
        }

        public string Author { get; }
        public bool Block { get; }
        public ItunesImage Image { get; }
        public TimeSpan? Duration { get; }
        public bool Explicit { get; }
        public bool IsClosedCaptioned { get; }
        public int Order { get; }
        public string Subtitle { get; }
        public string Summary { get; }
    }
}
