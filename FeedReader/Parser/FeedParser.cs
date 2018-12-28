namespace CodeHollow.FeedReader.Parser
{
    using System;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Internal FeedParser - returns the type of the feed or the parsed feed.
    /// </summary>
    internal static class FeedParser
    {
        /// <summary>
        /// Returns the feed type - rss 1.0, rss 2.0, atom, ...
        /// </summary>
        /// <param name="doc">the xml document</param>
        /// <returns>the feed type</returns>
        public static FeedType ParseFeedType(XDocument doc)
        {
            string rootElement = doc.Root.Name.LocalName;
            
            if (rootElement.EqualsIgnoreCase("feed"))
                return FeedType.Atom;

            if (rootElement.EqualsIgnoreCase("rdf"))
                return FeedType.Rss_1_0;

            if (rootElement.EqualsIgnoreCase("rss"))
            {
                string version = doc.Root.Attribute("version").Value;
                if (version.EqualsIgnoreCase("2.0")) {
                    if (doc.Root.Attribute(XName.Get("media", XNamespace.Xmlns.NamespaceName)) != null) {
                        return FeedType.MediaRss;
                    } else {
                        return FeedType.Rss_2_0;
                    }
                }

                if (version.EqualsIgnoreCase("0.91"))
                    return FeedType.Rss_0_91;

                if (version.EqualsIgnoreCase("0.92"))
                    return FeedType.Rss_0_92;

                return FeedType.Rss;
            }
            
            throw new FeedTypeNotSupportedException($"unknown feed type {rootElement}");
        }


        /// <summary>
        /// Returns the parsed feed.
        /// This method checks the encoding of the received file
        /// </summary>
        /// <param name="feedContentData">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(byte[] feedContentData)
        {
            string feedContent = Encoding.UTF8.GetString(feedContentData); // 1.) get string of the content
            feedContent = RemoveWrongChars(feedContent);

            XDocument feedDoc = XDocument.Parse(feedContent); // 2.) read document to get the used encoding

            Encoding encoding = GetEncoding(feedDoc); // 3.) get used encoding

            if (encoding != Encoding.UTF8) // 4.) if not UTF8 - reread the data :
                                           // in some cases - ISO-8859-1 - Encoding.UTF8.GetString doesn't work correct, so converting
                                           // from UTF8 to ISO-8859-1 doesn't work and result is wrong. see: FullParseTest.TestRss20ParseSwedishFeedWithIso8859_1
            {
                feedContent = encoding.GetString(feedContentData);
                feedContent = RemoveWrongChars(feedContent);
            }

            var feedType = ParseFeedType(feedDoc);

            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedContent);

            return feed.ToFeed();
        }
        
        /// <summary>
        /// Returns the parsed feed
        /// </summary>
        /// <param name="feedContent">the feed document</param>
        /// <returns>parsed feed</returns>
        public static Feed GetFeed(string feedContent)
        {
            feedContent = RemoveWrongChars(feedContent);

            XDocument feedDoc = XDocument.Parse(feedContent);

            var feedType = ParseFeedType(feedDoc);

            var parser = Factory.GetParser(feedType);
            var feed = parser.Parse(feedContent);

            return feed.ToFeed();
        }

        /// <summary>
        /// reads the encoding from a feed document, returns UTF8 by default
        /// </summary>
        /// <param name="feedDoc">rss feed document</param>
        /// <returns>encoding or utf8 by default</returns>
        private static Encoding GetEncoding(XDocument feedDoc)
        {
            Encoding encoding = Encoding.UTF8;

            try
            {
                var encodingStr = feedDoc.Document.Declaration?.Encoding;
                if (!string.IsNullOrEmpty(encodingStr))
                    encoding = Encoding.GetEncoding(encodingStr);

            }
            catch(Exception) { } // ignore and return default encoding
            return encoding;
        }

        /// <summary>
        /// removes some characters at the beginning of the document. These shouldn't be there,
        /// but unfortunately they are sometimes there. If they are not removed - xml parsing would fail.
        /// </summary>
        /// <param name="feedContent">rss feed content</param>
        /// <returns>cleaned up rss feed content</returns>
        private static string RemoveWrongChars(string feedContent)
        {
            feedContent = feedContent.Replace(((char)0x1C).ToString(), string.Empty); // replaces special char 0x1C, fixes issues with at least one feed
            feedContent = feedContent.Replace(((char)65279).ToString(), string.Empty); // replaces special char, fixes issues with at least one feed

            return feedContent.Trim();
        }
    }
}
