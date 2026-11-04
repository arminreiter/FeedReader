using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CodeHollow.FeedReader.Feeds;
using CodeHollow.FeedReader.Feeds.Itunes;
using CodeHollow.FeedReader.Feeds.Podcast;
using CodeHollow.FeedReader.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class JsonFeedTest
    {
        private static void Eq(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        private static DateTime Utc(int year, int month, int day, int hour, int minute, int second)
        {
            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }

        [TestMethod]
        public void TestJsonFeed11AllFeedFields()
        {
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeed11Sample.json");
            var json = (JsonFeed)feed.SpecificFeed;

            Eq("https://jsonfeed.org/version/1.1", json.Version);
            Eq("My Example Feed – äöü", json.Title);
            Eq("https://example.org/", json.HomePageUrl);
            Eq("https://example.org/feed.json", json.FeedUrl);
            Eq("A feed with all fields", json.Description);
            Eq("Hand-written sample covering every JSON Feed 1.1 field and some edge cases.", json.UserComment);
            Eq("https://example.org/feed.json?page=2", json.NextUrl);
            Eq("https://example.org/icon.png", json.Icon);
            Eq("https://example.org/favicon.png", json.Favicon);
            Eq("de-AT", json.Language);
            Eq(true, json.Expired);

            // "authors" wins over the deprecated "author"; the empty author object is skipped
            Eq(2, json.Authors.Count);
            var jane = json.Authors.First();
            Eq("Jane Doe", jane.Name);
            Eq("https://example.org/jane", jane.Url);
            Eq("https://example.org/jane.png", jane.Avatar);
            Eq("John Roe", json.Authors.Last().Name);
            Assert.IsNull(json.Authors.Last().Url);

            Eq(1, json.Hubs.Count);
            Eq("WebSub", json.Hubs.First().Type);
            Eq("https://websub.example.org/", json.Hubs.First().Url);

            // generic feed
            Eq(FeedType.JsonFeed, feed.Type);
            Eq("My Example Feed – äöü", feed.Title);
            Eq("https://example.org/", feed.Link);
            Eq("A feed with all fields", feed.Description);
            Eq("de-AT", feed.Language);
            Eq("https://example.org/icon.png", feed.ImageUrl);
            Assert.IsNull(feed.Copyright);
            Assert.IsNull(feed.LastUpdatedDate);
            Assert.IsTrue(feed.OriginalDocument.StartsWith("{"));
        }

        [TestMethod]
        public void TestJsonFeed11Items()
        {
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeed11Sample.json");

            // items without id or with a blank id are discarded (spec)
            Eq(2, feed.Items.Count);

            var item = (JsonFeedItem)feed.Items[0].SpecificItem;
            Eq("https://example.org/2", item.Id);
            Eq("https://example.org/2", item.Url);
            Eq("https://example.org/2", item.Link);
            Eq("https://other.example.com/linked", item.ExternalUrl);
            Eq("Second post", item.Title);
            Eq("<p>Hello, <b>world</b>!</p>", item.ContentHtml);
            Eq("Hello, world!", item.ContentText);
            Eq("A short summary", item.Summary);
            Eq("https://example.org/2/image.png", item.Image);
            Eq("https://example.org/2/banner.png", item.BannerImage);
            Eq("2010-02-07T14:04:00-05:00", item.DatePublishedString);
            Eq(Utc(2010, 2, 7, 19, 4, 0), item.DatePublished);
            Eq(DateTimeKind.Utc, item.DatePublished.Value.Kind);
            Eq(Utc(2010, 2, 8, 9, 0, 0), item.DateModified);
            Eq("en-US", item.Language);

            // item authors override the feed authors
            Eq(1, item.Authors.Count);
            Eq("Guest Author", item.Authors.First().Name);

            // tags are strings - the number is not a valid tag
            CollectionAssert.AreEqual(new[] { "json", "feeds" }, item.Tags.ToArray());

            // the attachment without url is skipped
            Eq(2, item.Attachments.Count);
            var mp3 = item.Attachments.First();
            Eq("https://example.org/2/episode.mp3", mp3.Url);
            Eq("audio/mpeg", mp3.MimeType);
            Eq("Episode 2", mp3.Title);
            Eq(89970236L, mp3.SizeInBytes);
            Eq(1234.5, mp3.DurationInSeconds);
            var m4a = item.Attachments.Last();
            Assert.IsNull(m4a.SizeInBytes);
            Assert.IsNull(m4a.DurationInSeconds);

            // generic item
            var generic = feed.Items[0];
            Eq("Second post", generic.Title);
            Eq("https://example.org/2", generic.Link);
            Eq("https://example.org/2", generic.Id);
            Eq("A short summary", generic.Description);
            Eq("<p>Hello, <b>world</b>!</p>", generic.Content); // html wins over text
            Eq("Guest Author", generic.Author);
            Eq(Utc(2010, 2, 7, 19, 4, 0), generic.PublishingDate);
            Eq("2010-02-07T14:04:00-05:00", generic.PublishingDateString);
            CollectionAssert.AreEqual(new[] { "json", "feeds" }, generic.Categories.ToArray());
        }

        [TestMethod]
        public void TestJsonFeedMicroblogItemAndNulls()
        {
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeed11Sample.json");
            var item = (JsonFeedItem)feed.Items[1].SpecificItem;

            Eq("1", item.Id); // numeric ids are coerced to string
            Assert.IsNull(item.Title); // json null is null - not an empty string
            Assert.IsNull(item.Summary);
            Assert.IsNull(item.Url);
            Eq(0, item.Tags.Count);
            Eq(0, item.Attachments.Count);
            Eq("not a date", item.DatePublishedString);
            Assert.IsNull(item.DatePublished);

            // no own authors -> the authors of the feed apply (spec)
            CollectionAssert.AreEqual(new[] { "Jane Doe", "John Roe" }, item.Authors.Select(x => x.Name).ToArray());

            var generic = feed.Items[1];
            Eq("A microblog post without title, url and authors.", generic.Content);
            Eq("Jane Doe, John Roe", generic.Author);
            Assert.IsNull(generic.Title);
            Assert.IsNull(generic.Link);
        }

        [TestMethod]
        public void TestJsonFeedExtensionsAreReadableViaElement()
        {
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeed11Sample.json");

            var custom = feed.SpecificFeed.Element.Element("_custom");
            Assert.IsNotNull(custom);
            Eq("https://example.org/ext", custom.Element("about").Value);
            Eq("5", custom.Element("rating").Value);

            Eq("true", feed.Items[0].SpecificItem.Element.Element("_custom").Element("flag").Value);
        }

        [TestMethod]
        public void TestJsonFeed10DeprecatedAuthor()
        {
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeed10Sample.json");
            var json = (JsonFeed)feed.SpecificFeed;

            Eq("https://jsonfeed.org/version/1", json.Version);
            Eq(FeedType.JsonFeed, feed.Type);
            Eq(1, json.Authors.Count);
            Eq("Old Style Author", json.Authors.First().Name);
            Eq("https://example.com/about", json.Authors.First().Url);

            Eq(2, feed.Items.Count);
            Eq("Item Author", feed.Items[0].Author);
            Eq("Old Style Author", feed.Items[1].Author);
        }

        [TestMethod]
        public void TestJsonFeedRealWorldJsonfeedOrg()
        {
            // jsonfeed.org's own feed (version 1), saved 2026-11-04
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeedJsonfeedOrg.json");

            Eq(FeedType.JsonFeed, feed.Type);
            Eq("JSON Feed", feed.Title);
            Eq("https://www.jsonfeed.org/", feed.Link);
            Eq("https://cdn.micro.blog/jsonfeed/avatar.jpg", feed.ImageUrl);
            Eq(2, feed.Items.Count);

            var item = feed.Items[0];
            Eq("JSON Feed version 1.1", item.Title);
            Eq("http://jsonfeed.micro.blog/2020/08/07/json-feed-version.html", item.Id);
            Eq("https://www.jsonfeed.org/2020/08/07/json-feed-version.html", item.Link);
            Eq(Utc(2020, 8, 7, 16, 44, 36), item.PublishingDate);
            Assert.IsTrue(item.Content.StartsWith("<p>We&rsquo;ve updated the spec"));
        }

        [TestMethod]
        public void TestJsonFeedRealWorldDaringFireball()
        {
            // daringfireball.net (version 1.1), saved 2026-11-04 and trimmed to 4 items
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeedDaringFireball.json");
            var json = (JsonFeed)feed.SpecificFeed;

            Eq("Daring Fireball", feed.Title);
            Eq("https://daringfireball.net/feeds/json", json.FeedUrl);
            Eq("https://daringfireball.net/graphics/favicon-64.png", json.Favicon);
            Eq("John Gruber", json.Authors.Single().Name);
            Eq(4, feed.Items.Count);

            var item = (JsonFeedItem)feed.Items[0].SpecificItem;
            Eq("This Election Will Not Look Surprising in History", item.Title);
            Eq(Utc(2026, 11, 4, 15, 15, 51), item.DatePublished);
            Eq(Utc(2026, 11, 4, 15, 41, 40), item.DateModified);
            Assert.IsTrue(item.ExternalUrl.StartsWith("https://apple.news/"));

            Eq("‘Political Reckoning Shatters Trump’s Grip on Washington’", feed.Items[1].Title);
            Assert.IsTrue(feed.Items.All(x => x.Author == "John Gruber"));
            Assert.IsTrue(feed.Items.All(x => !string.IsNullOrEmpty(x.Content)));
        }

        [TestMethod]
        public void TestJsonFeedFromStringAndBytesWithBom()
        {
            var text = File.ReadAllText("Feeds/JsonFeed11Sample.json");

            var fromString = FeedReader.ReadFromString(text);
            Eq("My Example Feed – äöü", fromString.Title);

            var bytesWithBom = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes("\r\n  " + text)).ToArray();
            var fromBytes = FeedReader.ReadFromByteArray(bytesWithBom);
            Eq("My Example Feed – äöü", fromBytes.Title);
            Eq(2, fromBytes.Items.Count);
        }

        private const string JsonHead = "{\"version\":\"https://jsonfeed.org/version/1.1\",\"title\":\"t\",";

        [TestMethod]
        public void TestJsonStringContentIsNotCleanedLikeXml()
        {
            // U+FEFF and DEL are valid inside json strings - the xml cleanup must not remove them,
            // otherwise "a﻿b" and "ab" would collapse to the same id
            var json = "﻿ \r\n" + JsonHead + "\"items\":[{\"id\":\"a﻿b\",\"title\":\"x\u007Fy\"},{\"id\":\"ab\"}]}";

            foreach (var feed in new[] { FeedReader.ReadFromString(json), FeedReader.ReadFromByteArray(Encoding.UTF8.GetBytes(json)) })
            {
                Eq(2, feed.Items.Count);
                Eq("a﻿b", feed.Items[0].Id);
                Eq("x\u007Fy", feed.Items[0].Title);
                Eq("ab", feed.Items[1].Id);
                Assert.AreNotEqual(feed.Items[0].Id, feed.Items[1].Id);
            }
        }

        [TestMethod]
        public void TestJsonFeedExplicitlyEmptyAuthors()
        {
            var feed = FeedReader.ReadFromString(JsonHead +
                "\"authors\":[],\"author\":{\"name\":\"Deprecated\"}," +
                "\"items\":[{\"id\":\"1\",\"authors\":[]},{\"id\":\"2\"}]}");

            // an explicitly empty "authors" wins over the deprecated "author"
            Eq(0, ((JsonFeed)feed.SpecificFeed).Authors.Count);
            Eq(0, ((JsonFeedItem)feed.Items[0].SpecificItem).Authors.Count);
            Assert.IsNull(feed.Items[0].Author);
            Assert.IsNull(feed.Items[1].Author); // inherits the (empty) feed authors
        }

        [TestMethod]
        public void TestJsonFeedItemAuthorInheritance()
        {
            var feed = FeedReader.ReadFromString(JsonHead +
                "\"authors\":[{\"name\":\"Feed Author\"}]," +
                "\"items\":[{\"id\":\"1\",\"authors\":[]},{\"id\":\"2\"},{\"id\":\"3\",\"author\":{\"name\":\"Old Item Author\"}},{\"id\":\"4\",\"authors\":null}]}");

            Assert.IsNull(feed.Items[0].Author);                // explicitly empty - no inheritance
            Eq("Feed Author", feed.Items[1].Author);            // not specified - inherited
            Eq("Old Item Author", feed.Items[2].Author);        // deprecated item field is a specification too
            Eq("Feed Author", feed.Items[3].Author);            // json null counts as not specified
        }

        [TestMethod]
        public void TestJsonFeedLongContentIsNotTruncated()
        {
            // the default reader quota would fail on strings > 8 KB
            var longHtml = new string('x', 100000);
            var feed = FeedReader.ReadFromString("{\"version\":\"https://jsonfeed.org/version/1.1\",\"title\":\"t\",\"items\":[{\"id\":\"1\",\"content_html\":\"" + longHtml + "\"}]}");

            Eq(100000, feed.Items[0].Content.Length);
        }

        [TestMethod]
        public void TestJsonFeedEscapes()
        {
            var feed = FeedReader.ReadFromString("{\"version\":\"https://jsonfeed.org/version/1.1\",\"title\":\"Tab\\tQuote\\\"\\u00e4\\ud83d\\ude00 <&>\",\"items\":[]}");

            Eq("Tab\tQuote\"ä😀 <&>", feed.Title);
            Eq(0, feed.Items.Count);
        }

        [TestMethod]
        public void TestJsonFeedNumbersAndDatesIndependentOfCulture()
        {
            // de-DE uses "," as decimal separator, th-TH the buddhist calendar
            foreach (var cultureName in new[] { "de-DE", "th-TH", "ar-SA" })
            {
                var original = CultureInfo.CurrentCulture;
                try
                {
                    CultureInfo.CurrentCulture = new CultureInfo(cultureName);
                    var feed = FeedReader.ReadFromFile("Feeds/JsonFeed11Sample.json");
                    var item = (JsonFeedItem)feed.Items[0].SpecificItem;

                    Eq(1234.5, item.Attachments.First().DurationInSeconds);
                    Eq(89970236L, item.Attachments.First().SizeInBytes);
                    Eq(Utc(2010, 2, 7, 19, 4, 0), item.DatePublished);
                }
                finally
                {
                    CultureInfo.CurrentCulture = original;
                }
            }
        }

        [TestMethod]
        public void TestJsonThatIsNotAJsonFeedThrows()
        {
            Assert.ThrowsException<FeedTypeNotSupportedException>(() => FeedReader.ReadFromString("{\"title\":\"no version\",\"items\":[]}"));
            Assert.ThrowsException<FeedTypeNotSupportedException>(() => FeedReader.ReadFromString("{\"version\":\"https://example.com/other\",\"items\":[]}"));
            Assert.ThrowsException<FeedTypeNotSupportedException>(() => FeedReader.ReadFromString("[1,2,3]"));
        }

        [TestMethod]
        public void TestInvalidJsonThrows()
        {
            Assert.ThrowsException<System.Xml.XmlException>(() => FeedReader.ReadFromString("{\"version\":\"https://jsonfeed.org/version/1.1\",\"title\":"));
        }

        [TestMethod]
        public void TestJsonFeedDeepNestingIsRejected()
        {
            var deep = string.Concat(Enumerable.Repeat("{\"a\":", 200)) + "1" + new string('}', 200);
            var json = "{\"version\":\"https://jsonfeed.org/version/1.1\",\"title\":\"t\",\"items\":[],\"_x\":" + deep + "}";

            Assert.ThrowsException<System.Xml.XmlException>(() => FeedReader.ReadFromString(json));
        }

        [TestMethod]
        public void TestXmlFeedsStillWork()
        {
            // the json detection must not affect xml feeds, including ones with leading whitespace
            var feed = FeedReader.ReadFromString("  \r\n<?xml version=\"1.0\"?><rss version=\"2.0\"><channel><title>x</title><item><title>i</title></item></channel></rss>");
            Eq(FeedType.Rss_2_0, feed.Type);
            Eq("i", feed.Items[0].Title);
        }

        [TestMethod]
        public void TestPodcastAndItunesExtensionsOnJsonFeedAreEmpty()
        {
            var feed = FeedReader.ReadFromFile("Feeds/JsonFeed11Sample.json");

            var podcast = feed.GetPodcastChannel();
            Eq(0, podcast.Persons.Length);
            Assert.IsNull(podcast.Guid);
            Eq(0, feed.Items[0].GetPodcastItem().Transcripts.Length);

            var itunes = feed.GetItunesChannel();
            Assert.IsNull(itunes.Author);
        }

        [TestMethod]
        public void TestFeedTypeValuesAreUnchanged()
        {
            // JsonFeed was appended - existing (serialized) numeric values must not change
            Eq(0, (int)FeedType.Atom);
            Eq(4, (int)FeedType.Rss_2_0);
            Eq(7, (int)FeedType.Unknown);
            Eq(8, (int)FeedType.JsonFeed);
        }

        [TestMethod]
        public void TestHtmlDiscoveryFindsJsonFeed()
        {
            var html = "<html><head>" +
                "<link rel=\"alternate\" type=\"application/rss+xml\" title=\"RSS\" href=\"https://example.org/feed.xml\">" +
                "<link rel=\"alternate\" type=\"application/feed+json\" title=\"JSON Feed\" href=\"https://example.org/feed.json\">" +
                // WordPress REST API discovery - plain application/json is not a feed
                "<link rel=\"alternate\" type=\"application/json\" href=\"https://example.org/wp-json/wp/v2/pages/1\">" +
                "</head></html>";

            var links = FeedReader.ParseFeedUrlsFromHtml(html).ToList();

            Eq(2, links.Count);
            Eq(FeedType.Rss, links[0].FeedType);
            Eq(FeedType.JsonFeed, links[1].FeedType);
            Eq("JSON Feed", links[1].Title);
            Eq("https://example.org/feed.json", links[1].Url);
        }
    }
}
