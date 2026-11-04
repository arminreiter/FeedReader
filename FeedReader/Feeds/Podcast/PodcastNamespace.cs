namespace CodeHollow.FeedReader.Feeds.Podcast
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Helpers to read podcast: elements (https://podcastindex.org/namespace/1.0)
    /// </summary>
    internal static class PodcastNamespace
    {
        internal const string NAMESPACEPREFIX = "podcast";

        // the spec says both uris must be treated as identical
        private static readonly XNamespace[] KnownNamespaces =
        {
            "https://podcastindex.org/namespace/1.0",
            "https://github.com/Podcastindex-org/podcast-namespace/blob/main/docs/1.0.md"
        };

        /// <summary>
        /// All podcast:<paramref name="name"/> children. Matches the "podcast" prefix and both official
        /// namespace uris, so feeds with another prefix or an unofficial uri are read too.
        /// </summary>
        public static IEnumerable<XElement> Elements(XElement parent, string name)
        {
            if (parent == null)
                return Enumerable.Empty<XElement>();

            var byPrefix = parent.GetNamespaceOfPrefix(NAMESPACEPREFIX);
            var namespaces = byPrefix == null ? KnownNamespaces : new[] { byPrefix }.Concat(KnownNamespaces).Distinct();

            return namespaces.SelectMany(ns => parent.Elements(ns + name));
        }

        public static XElement Element(XElement parent, string name)
        {
            return Elements(parent, name).FirstOrDefault();
        }

        public static string Value(XElement parent, string name)
        {
            return Element(parent, name)?.Value;
        }

        public static string Attribute(XElement element, string name)
        {
            return element?.Attribute(name)?.Value;
        }

        public static int? ParseInt(string value)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }

        public static long? ParseLong(string value)
        {
            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }

        public static decimal? ParseDecimal(string value)
        {
            // the spec uses "." (e.g. episode 100.5) - never parse with the current culture
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                return result;
            return null;
        }

        public static TimeSpan? ParseSeconds(string value)
        {
            var seconds = ParseDecimal(value);
            // exact decimal bound, so the cast to long ticks below can't overflow
            if (seconds == null || seconds < 0 || seconds > (decimal)TimeSpan.MaxValue.Ticks / TimeSpan.TicksPerSecond)
                return null;
            return TimeSpan.FromTicks((long)(seconds.Value * TimeSpan.TicksPerSecond));
        }
    }
}
