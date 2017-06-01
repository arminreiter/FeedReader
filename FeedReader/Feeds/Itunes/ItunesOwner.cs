namespace CodeHollow.FeedReader.Feeds.Itunes
{
    public class ItunesOwner
    {
        internal ItunesOwner(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Email { get; }
        public string Name { get; }
    }
}
