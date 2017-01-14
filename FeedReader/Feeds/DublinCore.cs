namespace CodeHollow.FeedReader.Feeds
{
    public class DublinCore
    {
        public string Title { get; set; }

        public string Creator { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string Publisher { get; set; }

        public string Contributor { get; set; }

        public string Date { get; set; }

        public string Type { get; set; }

        public string Format { get; set; }

        public string Identifier { get; set; }

        public string Source { get; set; }

        public string Language { get; set; }

        public string Relation { get; set; }

        public string Coverage { get; set; }

        public string Rights { get; set; }

        public DublinCore() { }

        public DublinCore(System.Xml.Linq.XElement item)
        {
            this.Title = item.GetValue("dc:title");
            this.Creator = item.GetValue("dc:creator");
            this.Subject = item.GetValue("dc:subject");
            this.Description = item.GetValue("dc:description");
            this.Publisher = item.GetValue("dc:publisher");
            this.Contributor = item.GetValue("dc:contributor");
            this.Date = item.GetValue("dc:date");
            this.Type = item.GetValue("dc:type");
            this.Format = item.GetValue("dc:format");
            this.Identifier = item.GetValue("dc:identifier");
            this.Source = item.GetValue("dc:source");
            this.Language = item.GetValue("dc:language");
            this.Relation = item.GetValue("dc:relation");
            this.Coverage = item.GetValue("dc:coverage");
            this.Rights = item.GetValue("dc:rights");
        }
    }
}
