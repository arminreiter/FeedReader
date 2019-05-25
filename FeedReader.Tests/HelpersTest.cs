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
