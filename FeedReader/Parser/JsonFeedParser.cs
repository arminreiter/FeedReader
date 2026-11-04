namespace CodeHollow.FeedReader.Parser
{
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Feeds;

    /// <summary>
    /// Parses JSON Feed 1.0 / 1.1. The json is converted to xml by the framework's
    /// JsonReaderWriterFactory (no extra dependency), so the feed classes can use the same
    /// XElement helpers as the xml feeds.
    /// </summary>
    internal class JsonFeedParser : IFeedParser
    {
        // a leading byte order mark and json whitespace (RFC 8259) - nothing else is removed,
        // as any other character can be a valid part of a json string
        private static readonly char[] LeadingChars = { '﻿', ' ', '\t', '\r', '\n' };

        /// <summary>
        /// true if the content looks like json (object or array), false for xml
        /// </summary>
        public static bool IsJson(string feedContent)
        {
            var content = feedContent.TrimStart(LeadingChars);
            return content.Length > 0 && (content[0] == '{' || content[0] == '[');
        }

        public BaseFeed Parse(string feedJson)
        {
            feedJson = feedJson.TrimStart(LeadingChars);

            // json is always utf-8 (RFC 8259). Max quotas: the default string limit (8 KB) is too small for content_html.
            // Max is a shared read-only instance, so copy it before changing MaxDepth.
            var quotas = new XmlDictionaryReaderQuotas();
            XmlDictionaryReaderQuotas.Max.CopyTo(quotas);
            quotas.MaxDepth = 64; // json feed is ~4 levels deep, extensions add a few. Caps nesting of untrusted input.

            XElement root;
            using (var reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(feedJson), quotas))
            {
                root = XElement.Load(reader);
            }

            var version = JsonFeed.GetJsonValue(root, "version");
            if (root.Attribute("type")?.Value != "object" || version == null || !version.Contains("jsonfeed.org/version/"))
                throw new FeedTypeNotSupportedException("json content is not a JSON Feed (missing or unknown \"version\")");

            return new JsonFeed(feedJson, root);
        }
    }
}
