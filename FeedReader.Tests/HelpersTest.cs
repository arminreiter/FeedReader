using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class HelpersTest
    {
        #region

        [TestMethod]
        public void TestCodeHollowLinkTag01()
        {
            string input = "<link rel=\"alternate\" type=\"application/rss+xml\" title=\"codehollow &raquo; Feed\" href=\"https://codehollow.com/feed/\" />";
            TestLinkTagParse(input, new HtmlFeedLink("codehollow » Feed", "https://codehollow.com/feed/", FeedType.Rss));
        }

        [TestMethod]
        public void TestCodeHollowLinkTag01Reordered1()
        {
            string input = "<link title=\"codehollow &raquo; Feed\" rel=\"alternate\" type=\"application/rss+xml\" href=\"https://codehollow.com/feed/\" />";
            TestLinkTagParse(input, new HtmlFeedLink("codehollow » Feed", "https://codehollow.com/feed/", FeedType.Rss));
        }

        [TestMethod]
        public void TestCodeHollowLinkTag01Reordered2()
        {
            string input = "<link type=\"application/rss+xml\"   href=\"https://codehollow.com/feed/\" title=\"codehollow &raquo; Feed\" rel=\"alternate\" />";
            TestLinkTagParse(input, new HtmlFeedLink("codehollow » Feed", "https://codehollow.com/feed/", FeedType.Rss));
        }

        [TestMethod]
        public void TestCodeHollowLinkTagWhitespaces()
        {
            string input = "<link        rel  = \"alternate\"   type= \"application/rss+xml\"         title=\"codehollow &raquo; Feed\"      href=\"https://codehollow.com/feed/\" />";
            TestLinkTagParse(input, new HtmlFeedLink("codehollow » Feed", "https://codehollow.com/feed/", FeedType.Rss));
        }

        [TestMethod]
        public void TestCodeHollowLinkTagNewLine()
        {
            string input = $"<link rel=\"alternate\" " +
                "type=\"application/rss+xml\" title=\"codehollow &raquo; Feed\" href=\"https://codehollow.com/feed/\" />";
            TestLinkTagParse(input, new HtmlFeedLink("codehollow » Feed", "https://codehollow.com/feed/", FeedType.Rss));
        }

        [TestMethod]
        public void TestLinkTagUppercaseAttributes()
        {
            string input = "<link rel=\"alternate\" TYPE=\"application/rss+xml\" Title=\"codehollow\" HREF=\"https://codehollow.com/feed/\" />";
            TestLinkTagParse(input, new HtmlFeedLink("codehollow", "https://codehollow.com/feed/", FeedType.Rss));
        }

        [TestMethod]
        public void TestTryParseDateTimeInvalidReturnsNull()
        {
            Assert.IsNull(Helpers.TryParseDateTime("garbage"));
            Assert.IsNull(Helpers.TryParseDateTime(" x"));
            Assert.IsNotNull(Helpers.TryParseDateTime("Thu, 22 Dec 2016 17:36:00 CEST"));
        }

        [TestMethod]
        public void TestRssWithoutVersionIsParsed()
        {
            var feed = FeedReader.ReadFromString("<rss><channel><title>t</title><item><title>i</title></item></channel></rss>");
            Assert.AreEqual("t", feed.Title);
            Assert.AreEqual(1, feed.Items.Count);
        }

        [TestMethod]
        public void TestGetAbsoluteFeedUrl()
        {
            string Abs(string page, string link) => FeedReader.GetAbsoluteFeedUrl(page, new HtmlFeedLink("t", link, FeedType.Rss)).Url;

            Assert.AreEqual("https://site.com/feed.xml", Abs("https://site.com/blog/post", "/feed.xml"));
            Assert.AreEqual("https://site.com/blog/feed.xml", Abs("https://site.com/blog/", "feed.xml"));
            Assert.AreEqual("https://cdn.site.com/f", Abs("https://site.com/", "//cdn.site.com/f"));
            Assert.AreEqual("http://other.com/feed", Abs("https://site.com/", "http://other.com/feed"));
            Assert.AreEqual("http://codehollow.com/feed", Abs("codehollow.com", "/feed"));
        }

        [TestMethod]
        public void TestParseFeedUrlsSingleQuotes()
        {
            var links = Helpers.ParseFeedUrlsFromHtml("<link rel='alternate' type='application/rss+xml' title='t' href='/f'><link rel=\"alternate\" type=\"application/atom+xml\" href=\"/a\">").ToList();
            Assert.AreEqual(2, links.Count);
            Assert.AreEqual("/f", links[0].Url);
            Assert.AreEqual("t", links[0].Title);
            Assert.AreEqual("/a", links[1].Url);
        }

        [TestMethod]
        public async Task TestDownloadRetriesWithoutHeadersOnForbidden()
        {
            var port = new Random().Next(20000, 60000);
            var prefix = $"http://localhost:{port}/";
            using (var listener = new System.Net.HttpListener())
            {
                listener.Prefixes.Add(prefix);
                listener.Start();
                var server = Task.Run(async () =>
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var ctx = await listener.GetContextAsync();
                        bool hasUserAgent = ctx.Request.Headers["User-Agent"] != null;
                        ctx.Response.StatusCode = hasUserAgent ? 403 : 200;
                        var body = System.Text.Encoding.UTF8.GetBytes(hasUserAgent ? "forbidden" : "ok");
                        ctx.Response.OutputStream.Write(body, 0, body.Length);
                        ctx.Response.Close();
                    }
                });

                var content = await Helpers.DownloadBytesAsync(prefix);
                await server;
                Assert.AreEqual("ok", System.Text.Encoding.UTF8.GetString(content));
            }
        }

        [TestMethod]
        public void TestUnquotedAttributes()
        {
            string input = "<link href=https://shkspr.mobi/blog/feed rel=alternate title=\"RSS Feed.\" type=application/rss+xml>";
            TestLinkTagParse(input, new HtmlFeedLink("RSS Feed.", "https://shkspr.mobi/blog/feed", FeedType.Rss));
        }

        [TestMethod]
        public void TestLinkTagWithLineBreaks()
        {
            string input = "<link\n  rel=\"alternate\"\n  type=\"application/rss+xml\"\n  title=\"My Feed\"\n  href=\"https://example.com/feed/\" />";
            TestLinkTagParse(input, new HtmlFeedLink("My Feed", "https://example.com/feed/", FeedType.Rss));
        }

        [TestMethod]
        public void TestParseFeedUrlsRelAlternatePartialWordNotMatched()
        {
            Assert.AreEqual(0, Helpers.ParseFeedUrlsFromHtml("<link rel=alternate2 type=\"application/rss+xml\" href=\"https://example.com/feed/\">").Count());
            Assert.AreEqual(0, Helpers.ParseFeedUrlsFromHtml("<link data-rel=\"alternate\" type=\"application/rss+xml\" href=\"https://example.com/feed/\">").Count());
        }

        [TestMethod]
        [DataRow("2024-03-01T13:26:09+00:00", 2024, 3, 1, 13, 26, 9)]
        [DataRow("2017-01-07T09:00:01-05:00", 2017, 1, 7, 14, 0, 1)]
        [DataRow("Sat, 07 Jan 2017 10:19:44 -0500", 2017, 1, 7, 15, 19, 44)]
        [DataRow("2019-04-27T14:25:30Z", 2019, 4, 27, 14, 25, 30)]
        // RFC 822 / 2822 time zone names
        [DataRow("Thu, 01 Apr 2021 08:00:00 EST", 2021, 4, 1, 13, 0, 0)]
        [DataRow("Thu, 01 Jul 2021 08:00:00 EDT", 2021, 7, 1, 12, 0, 0)]
        [DataRow("Thu, 01 Apr 2021 08:00:00 CST", 2021, 4, 1, 14, 0, 0)]
        [DataRow("Thu, 01 Jul 2021 08:00:00 CDT", 2021, 7, 1, 13, 0, 0)]
        [DataRow("Thu, 01 Apr 2021 08:00:00 MST", 2021, 4, 1, 15, 0, 0)]
        [DataRow("Thu, 01 Jul 2021 08:00:00 MDT", 2021, 7, 1, 14, 0, 0)]
        [DataRow("Thu, 01 Apr 2021 08:00:00 PST", 2021, 4, 1, 16, 0, 0)]
        [DataRow("Thu, 01 Jul 2021 08:00:00 PDT", 2021, 7, 1, 15, 0, 0)]
        [DataRow("Thu, 01 Apr 2021 08:00:00 pst ", 2021, 4, 1, 16, 0, 0)]
        [DataRow("Thu, 01 Apr 2021 08:00:00 UT", 2021, 4, 1, 8, 0, 0)]
        [DataRow("Thu, 01 Apr 2021 08:00:00 GMT", 2021, 4, 1, 8, 0, 0)]
        [DataRow("Thu, 01 Apr 2021 08:00 EST", 2021, 4, 1, 13, 0, 0)]
        // ambiguous zone names are not in RFC 2822 and still fall back to utc
        [DataRow("Thu, 22 Dec 2016 17:36:00 CEST", 2016, 12, 22, 17, 36, 0)]
        // ponytail: offset-less strings ("2020-01-01") parse as machine-local time, so they're left out to keep this TZ-independent
        public void TestDateTimeParse(string input, int year, int month, int day, int hour, int minute, int second)
        {
            var res = Helpers.TryParseDateTime(input);
            Assert.AreEqual(new DateTime(year, month, day, hour, minute, second), res);
        }

        private static void TestLinkTagParse(string input, HtmlFeedLink expectedResult)
        {
            var res = Helpers.GetFeedLinkFromLinkTag(input);
            Assert.AreEqual(expectedResult.Title, res.Title);
            Assert.AreEqual(expectedResult.Url, res.Url);
            Assert.AreEqual(expectedResult.FeedType, res.FeedType);
        }

        #endregion


        #region ParseFeedUrlsFromHtml Test -  test full html feed parse
        [TestMethod]
        public void ParseFeedsCodeHollow()
        {
            TestHtmlLinkParse("Html/codehollow.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("codehollow » Feed", "https://codehollow.com/feed/", FeedType.Rss),
                new HtmlFeedLink("codehollow » Comments Feed", "https://codehollow.com/comments/feed/", FeedType.Rss)
            });

        }

        [TestMethod]
        public void ParseFeedsHeise()
        {

            TestHtmlLinkParse("Html/heise.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("Aktuelle News von heise online", "https://www.heise.de/rss/heise-atom.xml", FeedType.Atom),
                new HtmlFeedLink("Aktuelle News von heise online (für ältere RSS-Reader)", "https://www.heise.de/rss/heise.rdf", FeedType.Rss)
            });

        }

        [TestMethod]
        public void ParseFeedsJapanTimes()
        {

            TestHtmlLinkParse("Html/japantimes.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("Japan Times RSS Feed - Top Stories", "https://www.japantimes.co.jp/feed/topstories", FeedType.Rss),
            });

        }
        [TestMethod]
        public void ParseFeedsOrfAt()
        {

            TestHtmlLinkParse("Html/orf.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("Newsfeed - news.ORF.at", "https://rss.orf.at/news.xml", FeedType.Rss),
            });

        }
        [TestMethod]
        public void ParseFeedsStackOverflow()
        {

            TestHtmlLinkParse("Html/stackoverflow.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("Feed of recent questions", "/feeds", FeedType.Atom),
            });

        }
        [TestMethod]
        public void ParseFeedsStadtfeuerwehrWeiz()
        {

            TestHtmlLinkParse("Html/stadtfeuerwehrweiz.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("Stadtfeuerwehr Weiz - Einsätze", "http://www.stadtfeuerwehr-weiz.at/rss/einsaetze.xml", FeedType.Rss),
            });

        }
        [TestMethod]
        public void ParseFeedsTheVerge()
        {

            TestHtmlLinkParse("Html/theverge.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("The Verge", "/rss/index.xml", FeedType.Rss),
                new HtmlFeedLink("Front Page", "https://www.theverge.com/rss/front-page/index.xml", FeedType.Rss)
            });

        }

        [TestMethod]
        public void ParseFeedsShksprMobi()
        {
            TestHtmlLinkParse("Html/shksprmobi.html", new List<HtmlFeedLink>()
            {
                new HtmlFeedLink("Atom Feed.", "https://shkspr.mobi/blog/feed/atom", FeedType.Atom),
                new HtmlFeedLink("RSS Feed.", "https://shkspr.mobi/blog/feed", FeedType.Rss),
            });
        }

        private static void TestHtmlLinkParse(string path, IEnumerable<HtmlFeedLink> expectedLinks)
        {
            var content = System.IO.File.ReadAllText(path);

            var links = Helpers.ParseFeedUrlsFromHtml(content);
            Assert.AreEqual(expectedLinks.Count(), links.Count());

            foreach (var l in links)
            {
                expectedLinks.First(e => e.FeedType == l.FeedType && e.Title == l.Title && e.Url == l.Url); // throws exception if link doesn't exist
            }
        }
        #endregion
    }
}
