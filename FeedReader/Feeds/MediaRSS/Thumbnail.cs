using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace CodeHollow.FeedReader.Feeds.MediaRSS
{

    /// <summary>
    /// Allows particular images to be used as representative images for the media object. If multiple thumbnails are included, and time coding is not at play, it is assumed that the images are in order of importance. 
    /// </summary>
    public class Thumbnail
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Thumbnail"/> class.
        /// Reads a rss feed item enclosure based on the xml given in element
        /// </summary>
        /// <param name="element">enclosure element as xml</param>
        public Thumbnail(XElement element)
        {
            this.Url = element.GetAttributeValue("url");
            this.Height = Helpers.TryParseInt(element.GetAttributeValue("height"));
            this.Width = Helpers.TryParseInt(element.GetAttributeValue("width"));
            this.Time = element.GetAttributeValue("time");
            
        }

        /// <summary>
        /// The url of the thumbnail. Required attribute
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Height of the object media. Optional attribute
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Width of the object media. Optional attribute
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Specifies the time offset in relation to the media object
        /// </summary>
        public string Time { get; set; }

    }
}
