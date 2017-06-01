namespace CodeHollow.FeedReader.Feeds
{
    using System.Xml.Linq;

    /// <summary>
    /// Atom 1.0 person element according to specification: https://validator.w3.org/feed/docs/atom.html
    /// </summary>
    public class AtomPerson
    {
        /// <summary>
        /// The "name" element
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The "email" element
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// The "uri" element
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomPerson"/> class.
        /// default constructor (for serialization)
        /// </summary>
        public AtomPerson()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomPerson"/> class.
        /// Reads an atom person based on the xml given in element
        /// </summary>
        /// <param name="element">person element as xml</param>
        public AtomPerson(XElement element)
        {
            this.Name = element.GetValue("name");
            this.EMail = element.GetValue("email");
            this.Uri = element.GetValue("uri");
        }

        /// <summary>
        /// returns the name of the author including the email if available
        /// </summary>
        /// <returns>name of the author with email if available</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.EMail))
                return this.Name;

            return $"{this.Name} <{this.EMail}>".Trim();
        }
    }
}
