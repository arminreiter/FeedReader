namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Atom 1.0 person element according to specification: https://validator.w3.org/feed/docs/atom.html
    /// </summary>
    public class AtomPerson
    {
        public string Name { get; set; }

        public string EMail { get; set; }

        public string Uri { get; set; }

        public AtomPerson() { }

        public AtomPerson(XElement element)
        {
            this.Name = element.GetValue("name");
            this.EMail = element.GetValue("email");
            this.Uri = element.GetValue("uri");
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.EMail))
                return this.Name;

            return $"{this.Name} <{this.EMail}>".Trim();
        }
    }
}
