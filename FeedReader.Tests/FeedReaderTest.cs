using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class FeedReaderTest
    {
        #region special cases

        [TestMethod]
        public void TestDownload400BadRequest()
        {
            // results in a 400 BadRequest if webclient is not initialized correctly
            string content = Helpers.Download("http://www.methode.at/blog?format=RSS");
            Assert.IsTrue(content.Length > 200);
        }

        [TestMethod]
        public void TestAcceptHeaderForbiddenWithParsing()
        {
            // results in 403 Forbidden if webclient does not have the accept header set
            var feed = FeedReader.Read("http://www.girlsguidetopm.com/feed/");
            string title = feed.Title;
            Assert.IsTrue(feed.Items.Count > 2);
            Assert.IsTrue(!string.IsNullOrEmpty(title));
        }

        [TestMethod]
        public void TestAcceptForbiddenUserAgent()
        {
            // results in 403 Forbidden if webclient does not have the accept header set
            string content = Helpers.Download("https://mikeclayton.wordpress.com/feed/");
            Assert.IsTrue(content.Length > 200);
        }


        [TestMethod]
        public void TestAcceptForbiddenUserAgentWrike()
        {
            // results in 403 Forbidden if webclient does not have the accept header set
            string content = Helpers.Download("https://www.wrike.com/blog");
            Assert.IsTrue(content.Length > 200);
        }
        

        #endregion

        #region ParseRssLinksFromHTML

        [TestMethod]
        public void TestParseRssLinksCodehollow()
        {
            TestParseRssLinks("https://codehollow.com", 2);
        }

        [TestMethod]
        public void TestParseRssLinksHeise() { TestParseRssLinks("http://heise.de/", 2); }
        [TestMethod]
        public void TestParseRssLinksHeise2() { TestParseRssLinks("heise.de", 2); }
        [TestMethod]
        public void TestParseRssLinksHeise3() { TestParseRssLinks("www.heise.de", 2); }
        [TestMethod]
        public void TestParseRssLinksDerStandard() { TestParseRssLinks("derstandard.at", 15); }
        [TestMethod]
        public void TestParseRssLinksDerStandard1() { TestParseRssLinks("http://www.derstandard.at", 15); }
        [TestMethod]
        public void TestParseRssLinksNYTimes() { TestParseRssLinks("nytimes.com", 1); }

        private void TestParseRssLinks(string url, int expectedNumberOfLinks)
        {
            string[] urls = FeedReader.ParseFeedUrlsAsString(url);
            Assert.AreEqual(expectedNumberOfLinks, urls.Length);
        }

        #endregion

        #region Parse Html and check if it returns absolute urls

        [TestMethod]
        public void TestParseAndAbsoluteUrlDerStandard1()
        {
            string url = "derstandard.at";
            var links = FeedReader.GetFeedUrlsFromUrl(url);

            foreach (var link in links)
            {
                var absoluteUrl = FeedReader.GetAbsoluteFeedUrl(url, link);
                Assert.IsTrue(absoluteUrl.Url.StartsWith("http://"));
            }

        }

        #endregion

        #region Read Feed

        [TestMethod]
        public void TestReadSimpleFeed()
        {
            var feed = FeedReader.Read("https://codehollow.com/feed");
            string title = feed.Title;
            Assert.AreEqual("codehollow", title);
            Assert.AreEqual(10, feed.Items.Count());
        }

        [TestMethod]
        public void TestReadRss20GermanFeed()
        {
            var feed = FeedReader.Read("http://botential.at/feed");
            string title = feed.Title;
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadRss10GermanFeed()
        {
            var feed = FeedReader.Read("http://rss.orf.at/news.xml");
            string title = feed.Title;
            Assert.IsTrue(feed.Items.Count > 10);
        }

        [TestMethod]
        public void TestReadAtomFeedHeise()
        {
            var feed = FeedReader.Read("https://www.heise.de/newsticker/heise-atom.xml");
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
            Assert.IsTrue(feed.Items.Count > 1);
        }

        [TestMethod]
        public void TestReadAtomFeedGitHub()
        {
            var feed = FeedReader.Read("https://github.com/codehollow/AzureBillingRateCardSample/commits/master.atom");
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        }

        [TestMethod]
        public void TestReadRss20GermanFeedPowershell()
        {
            var feed = FeedReader.Read("http://www.powershell.co.at/feed/");
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadRss20FeedCharter97Handle403Forbidden()
        {
            var feed = FeedReader.Read("charter97.org/rss.php");
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        }

        [TestMethod]
        public void TestReadRssScottHanselmanWeb()
        {
            var feed = FeedReader.Read("http://feeds.hanselman.com/ScottHanselman");
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadBuildAzure()
        {
            string content = Helpers.Download("https://buildazure.com");
            Assert.IsTrue(content.Length > 200);
        }

        #endregion
    }
}
