using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeHollow.FeedReader.Feeds.Itunes;
using System.Linq;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class ItunesTest
    {
        private void Eq(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestItunesSampleFeed()
        {
            var feed = FeedReader.ReadFromFile("Feeds/Rss20ItunesSample.xml");

            var itunesChannel = feed.GetItunesChannel();

            Eq("A show about everything", itunesChannel.Subtitle);
            Eq("John Doe", itunesChannel.Author);
            Eq("All About Everything is a show about everything. Each week we dive into any subject known to man and talk about it as much as we can. Look for our podcast in the Podcasts app or in the iTunes Store", itunesChannel.Summary);

            Assert.IsNotNull(itunesChannel.Owner);
            Eq("John Doe", itunesChannel.Owner.Name);
            Eq("john.doe@example.com", itunesChannel.Owner.Email);
            Assert.IsNotNull(itunesChannel.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything.jpg", itunesChannel.Image.Href);
            Assert.IsNotNull(itunesChannel.Categories);
            Eq("Technology", itunesChannel.Categories[0].Text);
            Assert.IsNotNull(itunesChannel.Categories[0].Children);
            Eq("Gadgets", itunesChannel.Categories[0].Children[0].Text);
            Eq("TV & Film", itunesChannel.Categories[1].Text);
            Eq("Arts", itunesChannel.Categories[2].Text);
            Assert.IsNotNull(itunesChannel.Categories[2].Children);
            Eq("Food", itunesChannel.Categories[2].Children[0].Text);
            Eq(false, itunesChannel.Explicit);


            var item1 = feed.Items.ElementAt(0).GetItunesItem();
            var item2 = feed.Items.ElementAt(1).GetItunesItem();
            var item3 = feed.Items.ElementAt(2).GetItunesItem();
            var item4 = feed.Items.ElementAt(3).GetItunesItem();

            Eq("John Doe", item1.Author);
            Eq("A short primer on table spices", item1.Subtitle);
            Eq("This week we talk about <a href=\"https://itunes/apple.com/us/book/antique-trader-salt-pepper/id429691295?mt=11\">salt and pepper shakers</a>, comparing and contrasting pour rates, construction materials, and overall aesthetics. Come and join the party!", item1.Summary);
            Assert.IsNotNull(item1.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode1.jpg", item1.Image.Href);
            Assert.IsNotNull(item1.Duration);
            Eq(4, item1.Duration.Value.Seconds);
            Eq(7, item1.Duration.Value.Minutes);
            Eq(false, item1.Explicit);

            Eq("Jane Doe", item2.Author);
            Eq("Comparing socket wrenches is fun!", item2.Subtitle);
            Eq("This week we talk about metric vs. Old English socket wrenches. Which one is better? Do you really need both? Get all of your answers here.", item2.Summary);
            Assert.IsNotNull(item2.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode2.jpg", item2.Image.Href);
            Assert.IsNotNull(item2.Duration);
            Eq(34, item2.Duration.Value.Seconds);
            Eq(4, item2.Duration.Value.Minutes);
            Eq(false, item2.Explicit);

            Eq("Jane Doe", item3.Author);
            Eq("Jane and Eric", item3.Subtitle);
            Eq("This week we talk about the best Chili in the world. Which chili is better?", item3.Summary);
            Assert.IsNotNull(item3.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode3.jpg", item3.Image.Href);
            Assert.IsNotNull(item3.Duration);
            Eq(34, item3.Duration.Value.Seconds);
            Eq(4, item3.Duration.Value.Minutes);
            Eq(false, item3.Explicit);
            Eq(true, item3.IsClosedCaptioned);

            Eq("Various", item4.Author);
            Eq("Red + Blue != Purple", item4.Subtitle);
            Eq("This week we talk about surviving in a Red state if you are a Blue person. Or vice versa.", item4.Summary);
            Assert.IsNotNull(item4.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode4.jpg", item4.Image.Href);
            Assert.IsNotNull(item4.Duration);
            Eq(59, item4.Duration.Value.Seconds);
            Eq(3, item4.Duration.Value.Minutes);
            Eq(false, item4.Explicit);

        }

        [TestMethod]
        public async Task TestItunesSampleFeed_Async()
        {
            var feed = await FeedReader.ReadFromFileAsync("Feeds/Rss20ItunesSample.xml").ConfigureAwait(false);

            var itunesChannel = feed.GetItunesChannel();

            Eq("A show about everything", itunesChannel.Subtitle);
            Eq("John Doe", itunesChannel.Author);
            Eq("All About Everything is a show about everything. Each week we dive into any subject known to man and talk about it as much as we can. Look for our podcast in the Podcasts app or in the iTunes Store", itunesChannel.Summary);

            Assert.IsNotNull(itunesChannel.Owner);
            Eq("John Doe", itunesChannel.Owner.Name);
            Eq("john.doe@example.com", itunesChannel.Owner.Email);
            Assert.IsNotNull(itunesChannel.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything.jpg", itunesChannel.Image.Href);
            Assert.IsNotNull(itunesChannel.Categories);
            Eq("Technology", itunesChannel.Categories[0].Text);
            Assert.IsNotNull(itunesChannel.Categories[0].Children);
            Eq("Gadgets", itunesChannel.Categories[0].Children[0].Text);
            Eq("TV & Film", itunesChannel.Categories[1].Text);
            Eq("Arts", itunesChannel.Categories[2].Text);
            Assert.IsNotNull(itunesChannel.Categories[2].Children);
            Eq("Food", itunesChannel.Categories[2].Children[0].Text);
            Eq(false, itunesChannel.Explicit);


            var item1 = feed.Items.ElementAt(0).GetItunesItem();
            var item2 = feed.Items.ElementAt(1).GetItunesItem();
            var item3 = feed.Items.ElementAt(2).GetItunesItem();
            var item4 = feed.Items.ElementAt(3).GetItunesItem();

            Eq("John Doe", item1.Author);
            Eq("A short primer on table spices", item1.Subtitle);
            Eq("This week we talk about <a href=\"https://itunes/apple.com/us/book/antique-trader-salt-pepper/id429691295?mt=11\">salt and pepper shakers</a>, comparing and contrasting pour rates, construction materials, and overall aesthetics. Come and join the party!", item1.Summary);
            Assert.IsNotNull(item1.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode1.jpg", item1.Image.Href);
            Assert.IsNotNull(item1.Duration);
            Eq(4, item1.Duration.Value.Seconds);
            Eq(7, item1.Duration.Value.Minutes);
            Eq(false, item1.Explicit);

            Eq("Jane Doe", item2.Author);
            Eq("Comparing socket wrenches is fun!", item2.Subtitle);
            Eq("This week we talk about metric vs. Old English socket wrenches. Which one is better? Do you really need both? Get all of your answers here.", item2.Summary);
            Assert.IsNotNull(item2.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode2.jpg", item2.Image.Href);
            Assert.IsNotNull(item2.Duration);
            Eq(34, item2.Duration.Value.Seconds);
            Eq(4, item2.Duration.Value.Minutes);
            Eq(false, item2.Explicit);

            Eq("Jane Doe", item3.Author);
            Eq("Jane and Eric", item3.Subtitle);
            Eq("This week we talk about the best Chili in the world. Which chili is better?", item3.Summary);
            Assert.IsNotNull(item3.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode3.jpg", item3.Image.Href);
            Assert.IsNotNull(item3.Duration);
            Eq(34, item3.Duration.Value.Seconds);
            Eq(4, item3.Duration.Value.Minutes);
            Eq(false, item3.Explicit);
            Eq(true, item3.IsClosedCaptioned);

            Eq("Various", item4.Author);
            Eq("Red + Blue != Purple", item4.Subtitle);
            Eq("This week we talk about surviving in a Red state if you are a Blue person. Or vice versa.", item4.Summary);
            Assert.IsNotNull(item4.Image);
            Eq("http://example.com/podcasts/everything/AllAboutEverything/Episode4.jpg", item4.Image.Href);
            Assert.IsNotNull(item4.Duration);
            Eq(59, item4.Duration.Value.Seconds);
            Eq(3, item4.Duration.Value.Minutes);
            Eq(false, item4.Explicit);

        }
    }
}
