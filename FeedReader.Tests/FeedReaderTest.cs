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
            DownloadTest("http://www.methode.at/blog?format=RSS");
        }

        [TestMethod]
        public void TestAcceptHeaderForbiddenWithParsing()
        {
            // results in 403 Forbidden if webclient does not have the accept header set
            var feed = FeedReader.ReadAsync("http://www.girlsguidetopm.com/feed/").Result;
            string title = feed.Title;
            Assert.IsTrue(feed.Items.Count > 2);
            Assert.IsTrue(!string.IsNullOrEmpty(title));
        }

        [TestMethod]
        public void TestAcceptForbiddenUserAgent()
        {
            // results in 403 Forbidden if webclient does not have the accept header set
            DownloadTest("https://mikeclayton.wordpress.com/feed/");
        }


        [TestMethod]
        public void TestAcceptForbiddenUserAgentWrike()
        {
            // results in 403 Forbidden if webclient does not have the accept header set
            DownloadTest("https://www.wrike.com/blog");
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
            string[] urls = FeedReader.ParseFeedUrlsAsStringAsync(url).Result;
            Assert.AreEqual(expectedNumberOfLinks, urls.Length);
        }

        #endregion

        #region Parse Html and check if it returns absolute urls

        [TestMethod]
        public void TestParseAndAbsoluteUrlDerStandard1()
        {
            string url = "derstandard.at";
            var links = FeedReader.GetFeedUrlsFromUrlAsync(url).Result;

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
            var feed = FeedReader.ReadAsync("https://codehollow.com/feed").Result;
            string title = feed.Title;
            Assert.AreEqual("codehollow", title);
            Assert.AreEqual(10, feed.Items.Count());
        }

        [TestMethod]
        public void TestReadRss20GermanFeed()
        {
            var feed = FeedReader.ReadAsync("http://botential.at/feed").Result;
            string title = feed.Title;
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadRss10GermanFeed()
        {
            var feed = FeedReader.ReadAsync("http://rss.orf.at/news.xml").Result;
            string title = feed.Title;
            Assert.IsTrue(feed.Items.Count > 10);
        }

        [TestMethod]
        public void TestReadAtomFeedHeise()
        {
            var feed = FeedReader.ReadAsync("https://www.heise.de/newsticker/heise-atom.xml").Result;
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
            Assert.IsTrue(feed.Items.Count > 1);
        }

        [TestMethod]
        public void TestReadAtomFeedGitHub()
        {
            var feed = FeedReader.ReadAsync("https://github.com/codehollow/AzureBillingRateCardSample/commits/master.atom").Result;
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        }

        [TestMethod]
        public void TestReadRss20GermanFeedPowershell()
        {
            var feed = FeedReader.ReadAsync("http://www.powershell.co.at/feed/").Result;
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadRss20FeedCharter97Handle403Forbidden()
        {
            var feed = FeedReader.ReadAsync("charter97.org/rss.php").Result;
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
        }

        [TestMethod]
        public void TestReadRssScottHanselmanWeb()
        {
            var feed = FeedReader.ReadAsync("http://feeds.hanselman.com/ScottHanselman").Result;
            Assert.IsTrue(!string.IsNullOrEmpty(feed.Title));
            Assert.IsTrue(feed.Items.Count > 0);
        }
        
        [TestMethod]
        public void TestReadBuildAzure()
        {
            DownloadTest("https://buildazure.com");
        }

        [TestMethod]
        public void TestReadNoticiasCatolicas()
        {
            var feed = FeedReader.ReadAsync("feeds.feedburner.com/NoticiasCatolicasAleteia").Result;
            Assert.AreEqual("Noticias Catolicas", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadTimeDoctor()
        {
            var feed = FeedReader.ReadAsync("https://www.timedoctor.com/blog/feed/").Result;
            Assert.AreEqual("Time Doctor", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadMikeC()
        {
            var feed = FeedReader.ReadAsync("https://mikeclayton.wordpress.com/feed/").Result;
            Assert.AreEqual("Shift Happens!", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadTheLPM()
        {
            var feed = FeedReader.ReadAsync("https://thelazyprojectmanager.wordpress.com/feed/").Result;
            Assert.AreEqual("The Lazy Project Manager's Blog", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }


        [TestMethod]
        public void TestReadStrategyEx()
        {
            var feed = FeedReader.ReadAsync("http://blog.strategyex.com/feed/").Result;
            Assert.AreEqual("Strategy Execution Blog", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadTechRep()
        {
            var feed = FeedReader.ReadAsync("http://www.techrepublic.com/rssfeeds/topic/project-management/").Result;
            Assert.AreEqual("Project Management on TechRepublic", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadAPOD()
        {
            var feed = FeedReader.ReadAsync("https://apod.nasa.gov/apod.rss").Result;
            Assert.AreEqual("APOD", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadThaqafnafsak()
        {
            var feed = FeedReader.ReadAsync("http://www.thaqafnafsak.com/feed").Result;
            Assert.AreEqual("ثقف نفسك", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadTheStudentLawyer()
        {
            var feed = FeedReader.ReadAsync("http://us10.campaign-archive.com/feed?u=8da2e137a07b178e5d9a71c2c&id=9134b0cc95").Result;
            Assert.AreEqual("The Student Lawyer Careers Network Archive Feed", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        [TestMethod]
        public void TestReadLiveBold()
        {
            var feed = FeedReader.ReadAsync("http://feeds.feedburner.com/LiveBoldAndBloom").Result;
            Assert.AreEqual("Live Bold and Bloom", feed.Title);
            Assert.IsTrue(feed.Items.Count > 0);
        }

        #endregion

        #region private helpers

        private void DownloadTest(string url)
        {
            string content = Helpers.DownloadAsync(url).Result;
            Assert.IsTrue(content.Length > 200);
        }

        #endregion
    }
}
