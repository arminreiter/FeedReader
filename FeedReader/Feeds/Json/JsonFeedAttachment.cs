namespace CodeHollow.FeedReader.Feeds
{
    using System.Globalization;
    using System.Xml.Linq;

    /// <summary>
    /// A JSON Feed attachment object (e.g. the audio file of a podcast episode)
    /// </summary>
    public class JsonFeedAttachment
    {
        /// <summary>
        /// The "url" field
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The "mime_type" field
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// The "title" field. Attachments with the same title are alternate versions of the same resource.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The "size_in_bytes" field
        /// </summary>
        public long? SizeInBytes { get; set; }

        /// <summary>
        /// The "duration_in_seconds" field
        /// </summary>
        public double? DurationInSeconds { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedAttachment"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public JsonFeedAttachment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFeedAttachment"/> class.
        /// </summary>
        /// <param name="attachment">the json attachment object converted to xml</param>
        public JsonFeedAttachment(XElement attachment)
        {
            this.Url = JsonFeed.GetJsonValue(attachment, "url");
            this.MimeType = JsonFeed.GetJsonValue(attachment, "mime_type");
            this.Title = JsonFeed.GetJsonValue(attachment, "title");
            if (long.TryParse(JsonFeed.GetJsonValue(attachment, "size_in_bytes"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
                this.SizeInBytes = size;
            this.DurationInSeconds = JsonFeed.GetJsonNumber(attachment, "duration_in_seconds");
        }
    }
}
