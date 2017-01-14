namespace CodeHollow.FeedReader
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Linq;

    public static class Extensions
    {
        public static string HtmlDecode(this string text)
        {
            return System.Net.WebUtility.HtmlDecode(text);
        }

        public static string ToUtf8(this string text)
        {
            return Encoding.UTF8.GetString(Encoding.Default.GetBytes(text));
        }

        public static string ToUtf8(this string text, Encoding encoding)
        {
            if (encoding == Encoding.UTF8)
                return text.ToUtf8();

            var utf = Encoding.UTF8;
            var convertedBytes = Encoding.Convert(encoding, utf, encoding.GetBytes(text));
            return Encoding.UTF8.GetString(convertedBytes);
        }

        public static string GetValue(this XElement element)
        {
            if (element == null)
                return null;

            string encodingStr = element.Document.Declaration?.Encoding;

            var encoding = Encoding.Default;
            if (!string.IsNullOrEmpty(encodingStr))
                encoding = Encoding.GetEncoding(encodingStr);

            return element?.Value?.ToUtf8(encoding);
        }

        public static string GetValue(this XElement element, string name)
        {
            return element?.GetElement(name).GetValue();
        }

        public static string GetValue(this XElement element, string namespacePrefix, string name)
        {
            return element?.GetElement(namespacePrefix, name).GetValue();
        }

        public static string GetValue(this XAttribute attribute)
        {
            if (attribute == null)
                return null;

            string encodingStr = attribute.Document.Declaration?.Encoding;

            var encoding = Encoding.Default;
            if (!string.IsNullOrEmpty(encodingStr))
                encoding = Encoding.GetEncoding(encodingStr);

            return attribute?.Value?.ToUtf8(encoding);
        }

        public static string GetAttributeValue(this XElement element, string name)
        {
            return element.GetAttribute(name)?.GetValue();
        }

        public static XAttribute GetAttribute(this XElement element, string name)
        {
            var splitted = SplitName(name);
            return element?.GetAttribute(splitted.Item1, splitted.Item2);
        }

        public static XAttribute GetAttribute(this XElement element, string namespacePrefix, string name)
        {
            if (string.IsNullOrEmpty(namespacePrefix))
                return element.Attribute(name);

            var namesp = element.GetNamespacePrefix(namespacePrefix);
            return element.Attribute(namesp + name);
        }

        public static XElement GetElement(this XElement element, string name)
        {
            var splitted = SplitName(name);
            return element?.GetElement(splitted.Item1, splitted.Item2);
        }

        public static XElement GetElement(this XElement element, string namespacePrefix, string name)
        {
            var namesp = element.GetNamespacePrefix(namespacePrefix);
            if (namesp == null) return null;
            return element.Element(namesp + name);
        }

        public static IEnumerable<XElement> GetElements(this XElement element, string name)
        {
            var splitted = SplitName(name);
            return element.GetElements(splitted.Item1, splitted.Item2);
        }

        public static IEnumerable<XElement> GetElements(this XElement element, string namespacePrefix, string name)
        {
            var namesp = element.GetNamespacePrefix(namespacePrefix);
            if (namesp == null) return null;
            return element.Elements(namesp + name);
        }

        public static XNamespace GetNamespacePrefix(this XElement element)
        {
            return element.GetNamespacePrefix(null);
        }

        public static XNamespace GetNamespacePrefix(this XElement element, string namespacePrefix)
        {
            var namesp = string.IsNullOrEmpty(namespacePrefix) ? element.GetDefaultNamespace() : element.GetNamespaceOfPrefix(namespacePrefix);
            return namesp;
        }

        /// <summary>
        /// splits the name into namespace and name if it contains a :
        /// if it does not contain a namespace, item1 is null and item2 is the original name
        /// </summary>
        /// <param name="name">the input name</param>
        /// <returns>splitted namespace and name, item1 is null if namespace is empty</returns>
        private static Tuple<string, string> SplitName(string name)
        {
            string namesp = null;
            if (name.Contains(":"))
            {
                int pos = name.IndexOf(':');
                namesp = name.Substring(0, pos);
                name = name.Substring(pos + 1);
            }

            return new Tuple<string, string>(namesp, name);
        }
    }
}
