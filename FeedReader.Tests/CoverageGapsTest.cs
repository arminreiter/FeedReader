using CodeHollow.FeedReader.Feeds;
using CodeHollow.FeedReader.Feeds.Itunes;
using CodeHollow.FeedReader.Feeds.MediaRSS;
using CodeHollow.FeedReader.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace CodeHollow.FeedReader.Tests
{
    /// <summary>
    /// Parser paths that none of the sample feeds hit: skipHours/skipDays, localized dates,
    /// optional itunes fields, encoding fallback and unsupported feed types.
    /// </summary>
    [TestClass]
    public class CoverageGapsTest
    {
        private static string Rss(string channelContent, string rootAttributes = "") =>
            $"<rss version=\"2.0\" xmlns:itunes=\"http://www.itunes.com/dtds/podcast-1.0.dtd\" {rootAttributes}><channel><title>t</title>{channelContent}</channel></rss>";

        private const string SkipAndLocalizedDate =
            "<language>fr-FR</language>" +
            "<pubDate>3 mars 2025 10:00</pubDate>" +
            "<lastBuildDate>4 avril 2025 11:30</lastBuildDate>" +
            "<skipHours><hour>0</hour><hour>23</hour></skipHours>" +
            "<skipDays><day>Saturday</day><day>Sunday</day></skipDays>";

        [TestMethod]
        public void TestRss20SkipHoursSkipDaysAndLocalizedDates()
        {
            var feed = FeedReader.ReadFromString(Rss(SkipAndLocalizedDate));
            var rss = (Rss20Feed)feed.SpecificFeed;

            CollectionAssert.AreEqual(new[] { "0", "23" }, rss.SkipHours.ToArray());
            CollectionAssert.AreEqual(new[] { "Saturday", "Sunday" }, rss.SkipDays.ToArray());
            Assert.AreEqual(new DateTime(2025, 3, 3), rss.PublishingDate?.Date);
            Assert.AreEqual(new DateTime(2025, 4, 4), rss.LastBuildDate?.Date);
            Assert.AreEqual(feed.OriginalDocument, rss.OriginalDocument);
        }

        [TestMethod]
        public void TestMediaRssSkipHoursSkipDaysAndLocalizedDates()
        {
            var feed = FeedReader.ReadFromString(Rss(SkipAndLocalizedDate, "xmlns:media=\"http://search.yahoo.com/mrss/\""));
            var rss = (MediaRssFeed)feed.SpecificFeed;

            Assert.AreEqual(FeedType.MediaRss, feed.Type);
            CollectionAssert.AreEqual(new[] { "0", "23" }, rss.SkipHours.ToArray());
            CollectionAssert.AreEqual(new[] { "Saturday", "Sunday" }, rss.SkipDays.ToArray());
            Assert.AreEqual(new DateTime(2025, 3, 3), rss.PublishingDate?.Date);
            Assert.AreEqual(new DateTime(2025, 4, 4), rss.LastBuildDate?.Date);
        }

        [TestMethod]
        public void TestUnknownLanguageLeavesUnparseableDateNull()
        {
            // invalid culture name -> CultureNotFoundException is swallowed, date stays null
            var feed = FeedReader.ReadFromString(Rss("<language>!!</language><pubDate>not a date</pubDate>"));
            var mediaFeed = FeedReader.ReadFromString(Rss("<language>!!</language><pubDate>not a date</pubDate>", "xmlns:media=\"http://search.yahoo.com/mrss/\""));

            Assert.IsNull(feed.LastUpdatedDate);
            Assert.IsNull(((Rss20Feed)feed.SpecificFeed).PublishingDate);
            Assert.IsNull(((MediaRssFeed)mediaFeed.SpecificFeed).PublishingDate);
            Assert.AreEqual("not a date", ((Rss20Feed)feed.SpecificFeed).PublishingDateString);
        }

        [TestMethod]
        public void TestItunesOptionalFields()
        {
            var feed = FeedReader.ReadFromString(Rss(
                "<itunes:block>Yes</itunes:block>" +
                "<itunes:complete>yes</itunes:complete>" +
                "<itunes:new-feed-url>https://example.com/new.xml</itunes:new-feed-url>" +
                "<item><title>a</title><itunes:order>7</itunes:order><itunes:block>yes</itunes:block><itunes:duration>3600</itunes:duration></item>" +
                "<item><title>b</title><itunes:duration>1:02:03</itunes:duration></item>" +
                "<item><title>c</title><itunes:duration>1:2:3:4</itunes:duration></item>" +
                "<item><title>d</title><itunes:duration>abc</itunes:duration></item>"));

            var channel = feed.GetItunesChannel();
            Assert.IsTrue(channel.Block);
            Assert.IsTrue(channel.Complete);
            Assert.AreEqual(new Uri("https://example.com/new.xml"), channel.NewFeedUrl);

            var items = feed.Items.Select(i => i.GetItunesItem()).ToArray();
            Assert.AreEqual(7, items[0].Order);
            Assert.IsTrue(items[0].Block);
            Assert.AreEqual(TimeSpan.FromHours(1), items[0].Duration);
            Assert.AreEqual(new TimeSpan(1, 2, 3), items[1].Duration);
            Assert.IsNull(items[2].Duration);
            Assert.IsNull(items[3].Duration);
            Assert.AreEqual(0, items[1].Order);
        }

        [TestMethod]
        public void TestCategoryDetails()
        {
            var rss = FeedReader.ReadFromString(Rss(
                "<category domain=\"https://example.com/tax\">News</category>" +
                "<item><title>a</title><category>Plain</category><category domain=\"d\">Tech</category></item>"));
            var channelCategory = ((Rss20Feed)rss.SpecificFeed).CategoryDetails.Single();
            Assert.AreEqual("News", channelCategory.Name);
            Assert.AreEqual("https://example.com/tax", channelCategory.Scheme);
            var itemCategories = ((Rss20FeedItem)rss.Items.Single().SpecificItem).CategoryDetails.ToArray();
            Assert.AreEqual("Plain", itemCategories[0].Name);
            Assert.IsNull(itemCategories[0].Scheme);
            Assert.AreEqual("d", itemCategories[1].Scheme);

            var atom = FeedReader.ReadFromString(
                "<feed xmlns=\"http://www.w3.org/2005/Atom\"><title>t</title>" +
                "<entry><title>e</title><category term=\"dotnet\" scheme=\"https://example.com/s\" label=\".NET\"/><category term=\"go\"/></entry></feed>");
            var atomItem = (AtomFeedItem)atom.Items.Single().SpecificItem;
            var first = atomItem.CategoryDetails.First();
            Assert.AreEqual(".NET", first.Name);
            Assert.AreEqual("dotnet", first.Term);
            Assert.AreEqual(".NET", first.Label);
            Assert.AreEqual("https://example.com/s", first.Scheme);
            Assert.AreEqual("go", atomItem.CategoryDetails.Last().Name);
            CollectionAssert.AreEqual(atomItem.Categories.ToArray(), atomItem.CategoryDetails.Select(c => c.Name).ToArray());
        }

        [TestMethod]
        public void TestItunesTitle()
        {
            var feed = FeedReader.ReadFromString(Rss(
                "<itunes:title>Show</itunes:title><item><title>a</title><itunes:title>Episode</itunes:title></item>"));
            Assert.AreEqual("Show", feed.GetItunesChannel().Title);
            Assert.AreEqual("Episode", feed.Items.Single().GetItunesItem().Title);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestReadFromStream()
        {
            var bytes = Encoding.UTF8.GetBytes(Rss("<description>Grüße</description>"));
            Assert.AreEqual("Grüße", FeedReader.ReadFromStream(new System.IO.MemoryStream(bytes)).Description);
            var feed = await FeedReader.ReadFromStreamAsync(new System.IO.MemoryStream(bytes));
            Assert.AreEqual("Grüße", feed.Description);
        }

        [TestMethod]
        public void TestUnknownEncodingFallsBackToUtf8()
        {
            var xml = "<?xml version=\"1.0\" encoding=\"no-such-encoding\"?>" + Rss("<description>Grüße</description>");
            var feed = FeedReader.ReadFromByteArray(Encoding.UTF8.GetBytes(xml));

            Assert.AreEqual("Grüße", feed.Description);
        }

        [TestMethod]
        public void TestUnsupportedFeedTypeThrows()
        {
            Assert.ThrowsException<FeedTypeNotSupportedException>(() => FeedReader.ReadFromString("<html><body/></html>"));
        }

        [TestMethod]
        public void TestTryParseMedium()
        {
            Assert.AreEqual(Medium.Unknown, Helpers.TryParseMedium(null));
            Assert.AreEqual(Medium.Image, Helpers.TryParseMedium("IMAGE"));
            Assert.AreEqual(Medium.Audio, Helpers.TryParseMedium("audio"));
            Assert.AreEqual(Medium.Video, Helpers.TryParseMedium("video"));
            Assert.AreEqual(Medium.Document, Helpers.TryParseMedium("document"));
            Assert.AreEqual(Medium.Executable, Helpers.TryParseMedium("executable"));
            Assert.AreEqual(Medium.Unknown, Helpers.TryParseMedium("hologram"));
        }

        [TestMethod]
        public void TestTryParseBool()
        {
            Assert.AreEqual(true, Helpers.TryParseBool("True"));
            Assert.AreEqual(false, Helpers.TryParseBool("FALSE"));
            Assert.IsNull(Helpers.TryParseBool("yes"));
            Assert.IsNull(Helpers.TryParseBool(""));
        }

        [TestMethod]
        public void TestLinkTagWithUnsupportedTypeIsIgnored()
        {
            Assert.IsNull(Helpers.GetFeedLinkFromLinkTag("<link rel=\"alternate\" type=\"text/html\" href=\"/en\" />"));
            Assert.IsNull(Helpers.GetFeedLinkFromLinkTag("<link rel=\"alternate\" type=\"application/json\" href=\"/wp-json/\" />"));
        }
    }
}
