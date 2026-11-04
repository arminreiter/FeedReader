using System;
using System.Globalization;
using System.Linq;
using CodeHollow.FeedReader.Feeds.Itunes;
using CodeHollow.FeedReader.Feeds.Podcast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class PodcastTest
    {
        private static void Eq(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPodcastNamespaceExampleChannel()
        {
            // official example.xml of https://github.com/Podcastindex-org/podcast-namespace, saved 2026-11-04
            var feed = FeedReader.ReadFromFile("Feeds/Rss20PodcastNamespaceExample.xml");
            var channel = feed.GetPodcastChannel();

            Eq("95ba3425-022d-5359-9326-eb77d8bcb15a", channel.Guid);
            Eq(true, channel.Locked);
            Eq("podcastowner@example.com", channel.LockedOwner);
            Eq("podcast", channel.Medium);

            Eq("my-podcast-license-v1", channel.License.Identifier);
            Eq("https://example.org/mypodcastlicense/full.pdf", channel.License.Url);

            Eq(3, channel.Blocks.Length);
            Assert.IsNull(channel.Blocks[0].Id);
            Eq(true, channel.Blocks[0].Blocked);
            Eq("google", channel.Blocks[1].Id);
            Eq(false, channel.Blocks[1].Blocked);

            Eq("https://example.com/donate", channel.Funding.Single().Url);
            Eq("Support the show!", channel.Funding.Single().Text);

            var location = channel.Locations.Single();
            Eq("Austin, TX", location.Name);
            Eq("creator", location.Rel);
            Eq("geo:30.2672,97.7431", location.Geo);
            Eq("R113314", location.Osm);
            Eq("US", location.Country);

            Eq(4, channel.Images.Length);
            var social = channel.Images[1];
            Eq("https://example.com/images/show/pci_avatar-massive_1200x630.webp", social.Href);
            Eq("image description for accesibility and bots", social.Alt);
            Eq("social", social.Purpose);
            Eq("1.9", social.AspectRatio);
            Eq(1200, social.Width);
            Eq(630, social.Height);
            Eq("image/webp", social.Type);
            Assert.IsNull(channel.Images[0].Height);

            var trailer = channel.Trailers.Single();
            Eq("Coming April 1st, 2021", trailer.Title);
            Eq("https://example.org/trailers/teaser", trailer.Url);
            Eq("Thu, 01 Apr 2021 08:00:00 EST", trailer.PubDateString);
            Eq(new DateTime(2021, 4, 1, 13, 0, 0, DateTimeKind.Utc), trailer.PubDate); // EST = -05:00
            Eq(12345678L, trailer.Length);
            Eq("audio/mp3", trailer.Type);
            Assert.IsNull(trailer.Season);

            // tags inside liveItem, podroll and publisher are not channel tags
            Eq(0, channel.SocialInteracts.Length);
            Eq(0, channel.Persons.Length);
            Eq(0, channel.Txts.Length);
            Assert.IsNull(channel.UpdateFrequency);

            // the itunes extension keeps working on the same feed
            Eq("episodic", feed.GetItunesChannel().Type);
        }

        [TestMethod]
        public void TestPodcastNamespaceExampleItem()
        {
            var feed = FeedReader.ReadFromFile("Feeds/Rss20PodcastNamespaceExample.xml");
            Eq(3, feed.Items.Count);
            var item = feed.Items[0].GetPodcastItem();

            Eq(1, item.Season);
            Eq("Podcasting 2.0", item.SeasonName);
            Eq(3m, item.Episode);
            Assert.IsNull(item.EpisodeDisplay);

            Eq("https://example.com/ep3_chapters.json", item.Chapters.Url);
            Eq("application/json", item.Chapters.Type);

            var soundbite = item.Soundbites.Single();
            Eq(TimeSpan.FromMilliseconds(33833), soundbite.StartTime);
            Eq(TimeSpan.FromSeconds(60), soundbite.Duration);
            Eq("", soundbite.Title);

            Eq(2, item.Transcripts.Length);
            Eq("https://example.com/ep3/transcript.txt", item.Transcripts[0].Url);
            Eq("text/plain", item.Transcripts[0].Type);
            Assert.IsNull(item.Transcripts[0].Language);
            Assert.IsNull(item.Transcripts[0].Rel);
            Eq("text/vtt", item.Transcripts[1].Type);
            Eq("es", item.Transcripts[1].Language);
            Eq("captions", item.Transcripts[1].Rel);

            Eq(3, item.Persons.Length);
            var adam = item.Persons[0];
            Eq("Adam Curry", adam.Name);
            Eq("host", adam.Role); // spec default
            Eq("cast", adam.Group); // spec default
            Eq("https://www.podchaser.com/creators/adam-curry-107ZzmWE5f", adam.Href);
            Eq("https://example.com/images/adamcurry.jpg", adam.Img);
            Eq("guest", item.Persons[1].Role);
            Eq("cover art designer", item.Persons[2].Role);
            Eq("visuals", item.Persons[2].Group);
            Assert.IsNull(item.Persons[2].Img);

            Eq("Birmingham Civil Rights Museum", item.Locations.Single().Name);
            Eq("subject", item.Locations.Single().Rel);
            Eq(2, item.Images.Length);

            Eq(5, item.AlternateEnclosures.Length);
            var standard = item.AlternateEnclosures[0];
            Eq("audio/mpeg", standard.Type);
            Eq(43200000L, standard.Length);
            Eq(128000m, standard.Bitrate);
            Eq(true, standard.Default);
            Eq("Standard", standard.Title);
            CollectionAssert.AreEqual(new[] { "https://example.com/file-03.mp3", "ipfs://someRandomMpegFile03" }, standard.Sources.Select(x => x.Uri).ToArray());
            Assert.IsNull(standard.Sources[0].ContentType);

            var video = item.AlternateEnclosures[4];
            Eq("video/mp4", video.Type);
            Eq(511276.52m, video.Bitrate);
            Eq(720, video.Height);
            Eq(false, video.Default);
            Eq(2, video.Sources.Length); // podcast:integrity is not a source

            Eq(2, item.SocialInteracts.Length);
            Eq(1, item.SocialInteracts[0].Priority);
            Eq("activitypub", item.SocialInteracts[0].Protocol);
            Eq("https://podcastindex.social/@dave/105079274766075912", item.SocialInteracts[0].Uri);
            Eq("@dave", item.SocialInteracts[0].AccountId);
            Eq("https://podcastindex.social/@dave", item.SocialInteracts[0].AccountUrl);
            Eq("twitter", item.SocialInteracts[1].Protocol);

            // second item: two locations, no images
            var item2 = feed.Items[1].GetPodcastItem();
            Eq(2m, item2.Episode);
            Eq(2, item2.Locations.Length);
            Eq("Marlow", item2.Locations[0].Name);
            Eq("GB", item2.Locations[0].Country);
            Eq("Dreamworld", item2.Locations[1].Name);
            Eq(0, item2.Images.Length);
            Eq(TimeSpan.FromSeconds(45.4), item2.Soundbites.Single().StartTime);
        }

        [TestMethod]
        public void TestPodcasting20RealWorldFeedWithGithubNamespaceUri()
        {
            // the "Podcasting 2.0" show feed (mp3s.nashownotes.com/pc20rss.xml), saved 2026-11-04 and trimmed to 3 items.
            // It binds the prefix to the alternative namespace uri https://github.com/Podcastindex-org/podcast-namespace/blob/main/docs/1.0.md
            var feed = FeedReader.ReadFromFile("Feeds/Rss20Podcasting20.xml");
            Eq(FeedType.Rss_2_0, feed.Type);
            Eq(3, feed.Items.Count);

            var channel = feed.GetPodcastChannel();
            Eq("917393e3-1b1e-5cef-ace4-edaa54e1f810", channel.Guid);
            Eq(true, channel.Locked);
            Eq("adam@curry.com", channel.LockedOwner);
            Eq("podcast", channel.Medium);
            Eq("Value 4 Value", channel.Funding.Single().Text);
            Eq(4, channel.Blocks.Length);
            Eq(false, channel.Blocks[0].Blocked);
            CollectionAssert.AreEqual(new[] { "apple", "spotify", "amazon" }, channel.Blocks.Skip(1).Select(x => x.Id).ToArray());
            Assert.IsTrue(channel.Blocks.Skip(1).All(x => x.Blocked));
            CollectionAssert.AreEqual(new[] { "Adam Curry", "Dave Jones" }, channel.Persons.Select(x => x.Name).ToArray());
            // the socialInteract of the liveItem is not a channel tag
            Eq(0, channel.SocialInteracts.Length);

            var item = feed.Items[0].GetPodcastItem();
            Eq("https://mp3s.nashownotes.com/PC20-277-Captions.srt", item.Transcripts.Single().Url);
            Eq("application/srt", item.Transcripts.Single().Type);
            Eq("captions", item.Transcripts.Single().Rel);
            Eq("application/json", item.Chapters.Type);
            Eq(2, item.Persons.Length);
            Eq("@adam", item.SocialInteracts.Single().AccountId);
            Eq("Support us with Value 4 Value", item.Funding.Single().Text);

            Assert.IsTrue(feed.Items.All(x => x.GetPodcastItem().Chapters != null));
        }

        [TestMethod]
        public void TestPodcastEdgeCasesNonStandardPrefix()
        {
            // namespace bound to "pc" instead of "podcast"
            var feed = FeedReader.ReadFromFile("Feeds/Rss20PodcastEdgeCases.xml");
            var channel = feed.GetPodcastChannel();

            Eq(false, channel.Locked);
            Assert.IsNull(channel.LockedOwner);
            Eq("cc-by-4.0", channel.License.Identifier);
            Assert.IsNull(channel.License.Url);
            Eq("spotify", channel.Blocks.Single().Id);
            Eq(true, channel.Blocks.Single().Blocked);
            Assert.IsNull(channel.Medium);
            Assert.IsNull(channel.Guid);

            Eq(2, channel.Txts.Length);
            Eq("verify", channel.Txts[0].Purpose);
            Eq("S6lpp-7ZCn8-dZfGc-OoyVG", channel.Txts[0].Value);
            Eq("ai-content", channel.Txts[1].Purpose);
            Eq("false", channel.Txts[1].Value);

            Eq("Weekly", channel.UpdateFrequency.Text);
            Eq(true, channel.UpdateFrequency.Complete);
            Eq("FREQ=WEEKLY", channel.UpdateFrequency.RRule);
            Eq("2023-04-01T00:00:00.000Z", channel.UpdateFrequency.DtStartString);
            Eq(new DateTime(2023, 4, 1, 0, 0, 0, DateTimeKind.Utc), channel.UpdateFrequency.DtStart);

            var person = channel.Persons.Single();
            Eq("Anonymous Host", person.Name);
            Eq("host", person.Role);
            Eq("cast", person.Group);
            Assert.IsNull(person.Href);

            Eq("disabled", channel.SocialInteracts.Single().Protocol);
            Assert.IsNull(channel.SocialInteracts.Single().Uri);
            Assert.IsNull(channel.SocialInteracts.Single().Priority);

            var trailer = channel.Trailers.Single();
            Eq(2, trailer.Season);
            Eq(new DateTime(2021, 4, 1, 6, 0, 0, DateTimeKind.Utc), trailer.PubDate);
            Assert.IsNull(trailer.Length);
            Assert.IsNull(trailer.Type);
        }

        [TestMethod]
        public void TestPodcastEdgeCasesItems()
        {
            var feed = FeedReader.ReadFromFile("Feeds/Rss20PodcastEdgeCases.xml");

            var bonus = feed.Items[0].GetPodcastItem();
            Eq(2, bonus.Season);
            Assert.IsNull(bonus.SeasonName);
            Eq(100.5m, bonus.Episode);
            Eq("Bonus", bonus.EpisodeDisplay);
            Eq(TimeSpan.FromSeconds(12.5), bonus.Soundbites.Single().StartTime);
            Eq(TimeSpan.FromSeconds(1234.25), bonus.Soundbites.Single().Duration);
            Eq("Best part", bonus.Soundbites.Single().Title);

            var hls = bonus.AlternateEnclosures.Single();
            Eq("application/x-mpegURL", hls.Type);
            Eq("video", hls.Rel);
            Eq("avc1.4d401e", hls.Codecs);
            Eq("de", hls.Lang);
            Eq(1080, hls.Height);
            Assert.IsNull(hls.Length);
            Assert.IsNull(hls.Bitrate);
            Eq(false, hls.Default);
            Eq("https://example.com/ep.m3u8", hls.Sources.Single().Uri);
            Eq("application/x-mpegURL", hls.Sources.Single().ContentType);

            Assert.IsNull(bonus.Chapters);
            Assert.IsNull(bonus.License);
            Eq(0, bonus.Transcripts.Length);

            // invalid values become null instead of throwing
            var broken = feed.Items[1].GetPodcastItem();
            Assert.IsNull(broken.Season);
            Assert.IsNull(broken.Episode);
            Assert.IsNull(broken.Soundbites.Single().StartTime);
            Assert.IsNull(broken.Soundbites.Single().Duration); // negative
        }

        [TestMethod]
        public void TestPodcastNumbersIndependentOfCulture()
        {
            // in de-DE "100.5" would be parsed as 1005 with the current culture
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                var feed = FeedReader.ReadFromFile("Feeds/Rss20PodcastEdgeCases.xml");
                var item = feed.Items[0].GetPodcastItem();

                Eq(100.5m, item.Episode);
                Eq(TimeSpan.FromSeconds(12.5), item.Soundbites.Single().StartTime);

                var example = FeedReader.ReadFromFile("Feeds/Rss20PodcastNamespaceExample.xml");
                Eq(511276.52m, example.Items[0].GetPodcastItem().AlternateEnclosures[4].Bitrate);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [TestMethod]
        public void TestFeedsWithoutPodcastNamespaceAreEmpty()
        {
            foreach (var file in new[] { "Feeds/Rss20ItunesSample.xml", "Feeds/AtomTheVerge.xml", "Feeds/Rss10OrfAt.xml", "Feeds/Rss091FullSample.xml" })
            {
                var feed = FeedReader.ReadFromFile(file);
                var channel = feed.GetPodcastChannel();

                Assert.IsNull(channel.Guid, file);
                Eq(false, channel.Locked);
                Assert.IsNull(channel.License, file);
                Assert.IsNull(channel.UpdateFrequency, file);
                Eq(0, channel.Persons.Length);
                Eq(0, channel.Funding.Length);
                Eq(0, channel.Blocks.Length);

                foreach (var feedItem in feed.Items)
                {
                    var item = feedItem.GetPodcastItem();
                    Assert.IsNull(item.Season, file);
                    Assert.IsNull(item.Episode, file);
                    Assert.IsNull(item.Chapters, file);
                    Eq(0, item.Transcripts.Length);
                    Eq(0, item.AlternateEnclosures.Length);
                }
            }
        }
    }
}
