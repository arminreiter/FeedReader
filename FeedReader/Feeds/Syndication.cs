namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// The parsed syndication elements. Those are all elements that start with "sy:"
    /// </summary>
    public class Syndication
    {
        /// <summary>
        /// The "updatePeriod" element
        /// </summary>
        public string UpdatePeriod { get; set; }

        /// <summary>
        /// The "updateFrequency" element
        /// </summary>
        public string UpdateFrequency { get; set; }

        /// <summary>
        /// The "updateBase" element
        /// </summary>
        public string UpdateBase { get; set; }

        /// <summary>
        /// default constructor (for serialization)
        /// </summary>
        public Syndication() { }

        /// <summary>
        /// Creates the object based on the xml in the given XElement
        /// </summary>
        /// <param name="xelement"></param>
        public Syndication(XElement xelement)
        {
            this.UpdateBase = xelement.GetValue("sy:updateBase");
            this.UpdateFrequency = xelement.GetValue("sy:updateFrequency");
            this.UpdatePeriod = xelement.GetValue("sy:updatePeriod");
        }
    }
}
