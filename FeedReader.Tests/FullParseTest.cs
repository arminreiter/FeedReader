using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CodeHollow.FeedReader.Feeds;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class FullParseTest
    {
        private void Eq(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestAtomParseTheVerge()
        {
            var feed = (AtomFeed)FeedReader.ReadFromFile("Feeds/AtomTheVerge.xml");

            Eq("The Verge -  Front Pages", feed.Title);
            Eq("https://cdn2.vox-cdn.com/community_logos/34086/verge-fv.png", feed.Icon);
            Eq("2017-01-07T09:00:01-05:00", feed.UpdatedDateString);
            Eq(new DateTime(2017,1,7,14,0,1), feed.UpdatedDate);
            Eq("http://www.theverge.com/rss/group/front-page/index.xml", feed.Id);

            var item = (AtomFeedItem)feed.Items.First();

            Eq("2017-01-07T09:00:01-05:00", item.UpdatedDateString);
            Eq(new DateTime(2017, 1, 7, 14, 0, 1), item.UpdatedDate);
            Eq("2017-01-07T09:00:01-05:00", item.PublishedDateString);
            Eq(new DateTime(2017, 1, 7, 14, 0, 1), item.PublishedDate);
            Eq("This is the new Hulu experience with live TV", item.Title);
            Eq("http://www.theverge.com/ces/2017/1/7/14195588/hulu-live-tv-streaming-internet-ces-2017", item.Id);
            Eq("http://www.theverge.com/ces/2017/1/7/14195588/hulu-live-tv-streaming-internet-ces-2017", item.Link);

            Assert.IsTrue(item.Content.Trim().StartsWith("<img alt=\"\""));

            Eq("Chris Welch", item.Author.Name);
        }

        [TestMethod]
        public void TestAtomYouTubeInvestmentPunk()
        {
            var feed = (AtomFeed)FeedReader.ReadFromFile("Feeds/AtomYoutubeInvestmentPunk.xml");

            Eq("http://www.youtube.com/feeds/videos.xml?channel_id=UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.First().Href);
            Eq("yt:channel:UCmEN5ZnsHUXIxgpLitRTmWw", feed.Id);
            Eq("Investment Punk Academy by Gerald Hörhan", feed.Title);
            Eq("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.ElementAt(1).Href);
            Eq("Investment Punk Academy by Gerald Hörhan", feed.Author.Name);
            Eq("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Author.Uri);
            var item = (AtomFeedItem)feed.Items.First();
            Eq("yt:video:-qF_ABYEHxQ", item.Id);
            Eq("Sanierung I Blockchain-Technologie I Daytrading #ASKTHEPUNK 69", item.Title);
            Eq("alternate", item.Links.First().Relation);
            Eq("2017-01-07T15:20:24+00:00", item.UpdatedDateString);
            Eq("2017-01-06T16:00:00+00:00", item.PublishedDateString);
        }

        [TestMethod]
        public void TestRss091ParseStadtFWeiz()
        {
            var feed = (Rss091Feed)FeedReader.ReadFromFile("Feeds/Rss091Stadtfeuerwehr.xml");

            Eq("Stadtfeuerwehr Weiz - Einsätze", feed.Title);
            Eq("http://www.stadtfeuerwehr-weiz.at", feed.Link);
            Eq("Die letzten 15 Einsätze der Stadtfeuerwehr Weiz.", feed.Description);
            Eq("de-de", feed.Language);
            Eq("Stadtfeuerwehr Weiz / Markus Horwath", feed.Copyright);

            var item = (Rss091FeedItem)feed.Items.First();

            Eq(@"[06.01.2017 - 06:59 Uhr] Baum-Bergung", item.Title.Trim());
            Assert.IsTrue(item.Description.Contains("Weitere Informationen"));
            Eq("http://www.stadtfeuerwehr-weiz.at/einsaetze/einsatz-detail/4565/", item.Link);
            Eq("Fri, 06 Jan 2017 06:59:00 +0100", item.PublishingDateString);
            Eq(new DateTime(2017, 1, 6, 5, 59, 0), item.PublishingDate);

            Eq(15, feed.Items.Count);
        }

        [TestMethod]
        public void TestRss091ParseFullSample()
        {
            var feed = (Rss091Feed)FeedReader.ReadFromFile("Feeds/Rss091FullSample.xml");
            Eq("Copyright 1997-1999 UserLand Software, Inc.", feed.Copyright);
            Eq("Thu, 08 Jul 1999 07:00:00 GMT", feed.PublishingDateString);
            Eq("Thu, 08 Jul 1999 16:20:26 GMT", feed.LastBuildDateString);
            Eq("http://my.userland.com/stories/storyReader$11", feed.Docs);
            Eq("News and commentary from the cross-platform scripting community.", feed.Description);
            Eq("http://www.scripting.com/", feed.Link);
            Eq("Scripting News", feed.Title);
            Eq("http://www.scripting.com/", feed.Image.Link);
            Eq("Scripting News", feed.Image.Title);
            Eq("http://www.scripting.com/gifs/tinyScriptingNews.gif", feed.Image.Url);
            Eq(40, ((Rss091FeedImage)feed.Image).Height);
            Eq(78, ((Rss091FeedImage)feed.Image).Width);
            Eq("What is this used for?", ((Rss091FeedImage)feed.Image).Description);
            Eq("dave@userland.com (Dave Winer)", feed.ManagingEditor);
            Eq("dave@userland.com (Dave Winer)", feed.WebMaster);
            Eq("en-us", feed.Language);
            Assert.IsTrue(feed.SkipHours.Contains("6"));
            Assert.IsTrue(feed.SkipHours.Contains("7"));
            Assert.IsTrue(feed.SkipHours.Contains("8"));
            Assert.IsTrue(feed.SkipHours.Contains("9"));
            Assert.IsTrue(feed.SkipHours.Contains("10"));
            Assert.IsTrue(feed.SkipHours.Contains("11"));
            Assert.IsTrue(feed.SkipDays.Contains("Sunday"));
            Eq("(PICS-1.1 \"http://www.rsac.org/ratingsv01.html\" l gen true comment \"RSACi North America Server\" for \"http://www.rsac.org\" on \"1996.04.16T08:15-0500\" r (n 0 s 0 v 0 l 0))", feed.Rating);

            Eq(1, feed.Items.Count);
            var item = (Rss091FeedItem)feed.Items.First();
            Eq("stuff", item.Title);
            Eq("http://bar", item.Link);
            Eq("This is an article about some stuff", item.Description);

            Eq("Search Now!", feed.TextInput.Title);
            Eq("Enter your search terms", feed.TextInput.Description);
            Eq("find", feed.TextInput.Name);
            Eq("http://my.site.com/search.cgi", feed.TextInput.Link);
        }

        [TestMethod]
        public void TestRss10ParseFullSample()
        {
            var feed = (Rss10Feed)FeedReader.ReadFromFile("Feeds/Rss10FeedWebResourceSample.xml");

            Eq("XML.com", feed.Title);
            Eq("http://xml.com/pub", feed.Link);
            Eq("\n      XML.com features a rich mix of information and services\n      for the XML community.\n    ", feed.Description);
            var image = (Rss10FeedImage)feed.Image;
            Eq("http://xml.com/universal/images/xml_tiny.gif", image.About);
            Eq("XML.com", image.Title);
            Eq("http://www.xml.com", image.Link);
            Eq("http://xml.com/universal/images/xml_tiny.gif", image.Url);
            Eq("Search XML.com", feed.TextInput.Title);
            Eq("Search XML.com's XML collection", feed.TextInput.Description);
            Eq("s", feed.TextInput.Name);
            Eq("http://search.xml.com", ((Rss10FeedTextInput)feed.TextInput).About);
            Eq("http://search.xml.com", feed.TextInput.Link);

            var item = (Rss10FeedItem)feed.Items.Last();

            Eq("http://xml.com/pub/2000/08/09/rdfdb/index.html", item.About);
            Eq("Putting RDF to Work", item.Title);
            Eq("http://xml.com/pub/2000/08/09/rdfdb/index.html", item.Link);
            Eq(186, item.Description.Length);
        }

        [TestMethod]
        public void TestRss10ParseOrfAt()
        {
            var feed = (Rss10Feed)FeedReader.ReadFromFile("Feeds/Rss10OrfAt.xml");
            Eq("news.ORF.at", feed.Title);
            Eq("http://orf.at/", feed.Link);
            Eq("2017-01-07T15:57:36+01:00", feed.DC.Date);
            Eq("Die aktuellsten Nachrichten auf einen Blick - aus Österreich und der ganzen Welt. In Text, Bild und Video.", feed.Description);
            Eq("ORF Österreichischer Rundfunk, Wien", feed.DC.Publisher);
            Eq("ORF Online und Teletext GmbH & Co KG", feed.DC.Creator);
            Eq("de", feed.DC.Language);
            Eq("Copyright � 2017 ORF Online und Teletext GmbH & Co KG", feed.DC.Rights);
            Eq("hourly", feed.Sy.UpdatePeriod);
            Eq("2", feed.Sy.UpdateFrequency);
            Eq("2000-01-01T00:00:00Z", feed.Sy.UpdateBase);
            Eq(49, feed.Items.Count);

            var item = (Rss10FeedItem)feed.Items.ElementAt(4);

            Eq("Irak: Einigung über Abzug türkischer Truppen", item.Title);
            Eq("http://orf.at/stories/2374136/", item.Link);
            Eq("Ausland", item.DC.Subject);
            Eq("2017-01-07T15:03:35+01:00", item.DC.Date);
        }

        [TestMethod]
        public void TestRss20ParseWebResourceSampleFull()
        {
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20FeedWebResourceSample.xml");

            Eq("Scripting News", feed.Title);
            Eq("http://www.scripting.com/", feed.Link);
            Eq("A weblog about scripting and stuff like that.", feed.Description);
            Eq("en-us", feed.Language);
            Eq("Copyright 1997-2002 Dave Winer", feed.Copyright);
            Eq("Mon, 30 Sep 2002 11:00:00 GMT", feed.LastBuildDateString);
            Eq("http://backend.userland.com/rss", feed.Docs);
            Eq("Radio UserLand v8.0.5", feed.Generator);
            Eq("1765", feed.Categories.First());
            Eq("dave@userland.com", feed.ManagingEditor);
            Eq("dave@userland.com", feed.WebMaster);
            Eq("40", feed.TTL);
            Eq(9, feed.Items.Count);

            var item = (Rss20FeedItem)feed.Items.Last();
            Eq("Really early morning no-coffee notes", item.Title);
            Eq("http://scriptingnews.userland.com/backissues/2002/09/29#reallyEarlyMorningNocoffeeNotes", item.Link);
            Assert.IsTrue(item.Description.Contains("<p>One of the lessons I've learned"));
            Eq("Sun, 29 Sep 2002 11:13:10 GMT", item.PublishingDateString);
            Eq(new DateTime(2002, 09, 29, 11, 13, 10), item.PublishingDate);
            Eq("http://scriptingnews.userland.com/backissues/2002/09/29#reallyEarlyMorningNocoffeeNotes", item.Guid);
        }

        [TestMethod]
        public void TestRss20ParseCodeHollow()
        {
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20CodeHollowCom.xml");

            Eq("codehollow", feed.Title);
            Eq("https://codehollow.com", feed.Link);
            Eq("Azure, software engineering/architecture, Scrum, SharePoint, VSTS/TFS, .NET and other funny things", feed.Description);
            Eq("Fri, 23 Dec 2016 09:01:55 +0000", feed.LastBuildDateString);
            Eq(new DateTime(2016, 12, 23, 09, 01, 55), feed.LastBuildDate);
            Eq("en-US", feed.Language);
            Eq("hourly", feed.Sy.UpdatePeriod);
            Eq("1", feed.Sy.UpdateFrequency);
            Eq("https://wordpress.org/?v=4.7", feed.Generator);
            
            var item = (Rss20FeedItem)feed.Items.First();

            Eq("Export Azure RateCard data to CSV with C# and Billing API", item.Title);
            Eq("https://codehollow.com/2016/12/export-azure-ratecard-data-csv-csharp-billing-api/", item.Link);
            Eq("https://codehollow.com/2016/12/export-azure-ratecard-data-csv-csharp-billing-api/#respond", item.Comments);
            Eq("Thu, 22 Dec 2016 07:00:28 +0000", item.PublishingDateString);
            Eq(new DateTime(2016, 12, 22, 7, 0, 28), item.PublishingDate);
            Eq("Armin Reiter", item.DC.Creator);
            Eq(4, item.Categories.Count);
            Assert.IsTrue(item.Categories.Contains("BillingAPI"));
            Eq("https://codehollow.com/?p=749", item.Guid);
            Assert.IsTrue(item.Description.StartsWith("<p>The Azure Billing API allows to programmatically read Azure"));
            Assert.IsTrue(item.Content.Contains("<add key=\"Tenant\" "));

        }

        [TestMethod]
        public void TestRss20ParseContentWindGerman()
        {
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20ContentWindCom.xml");
            Eq("ContentWind", feed.Title);
            Eq("http://content-wind.com", feed.Link);
            Eq("Do, 22 Dez 2016 17:36:00 +0000", feed.LastBuildDateString);
            Eq(new DateTime(2016, 12, 22, 17, 36, 00), feed.LastBuildDate);
            Eq("de-DE", feed.Language);
            Eq("hourly", feed.Sy.UpdatePeriod);
            Eq("1", feed.Sy.UpdateFrequency);
            Eq("https://wordpress.org/?v=4.7", feed.Generator);

            var item = (Rss20FeedItem)feed.Items.First();
            Eq("Wachstum Influencer Marketing", item.Title);
            Eq("http://content-wind.com/2016/12/22/wachstum-influencer-marketing/", item.Link);
            Eq("http://content-wind.com/2016/12/22/wachstum-influencer-marketing/#respond", item.Comments);
            Eq("Thu, 22 Dec 2016 13:09:51 +0000", item.PublishingDateString);
            Eq(new DateTime(2016, 12, 22, 13, 09, 51), item.PublishingDate);
            Eq("Harald Schaffernak", item.DC.Creator);

        }

        [TestMethod]
        public void TestRss20ParseMoscowTimes()
        {
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20MoscowTimes.xml");
            Eq("The Moscow Times - News, Business, Culture & Multimedia from Russia", feed.Title);
            Eq("https://themoscowtimes.com/", feed.Link);
            Eq("The Moscow Times offers everything you need to know about Russia: Breaking news, top stories, business, analysis, opinion, multimedia, upcoming cultural events", feed.Description);
            Eq("en-us", feed.Language);
            Eq("Sat, 07 Jan 2017 07:02:27 +0000", feed.LastBuildDateString);
            Eq("600", feed.TTL);

            var item = (Rss20FeedItem)feed.Items.First();
            Eq("American Unintelligence on Russia (Op-ed)", item.Title);
            Eq("https://themoscowtimes.com/articles/american-unintelligence-on-russia-op-ed-56746", item.Link);
            Eq("America’s case against the Kremlin suffers from major flaws that should be acknowledged, even by those who argue that Russia hacked U.S. democratic institutions, says Kevin Rothrock.", item.Description);
            Eq("Sat, 07 Jan 2017 07:02:27 +0000", item.PublishingDateString);
            Eq("https://themoscowtimes.com/articles/american-unintelligence-on-russia-op-ed-56746", item.Guid);

            item = (Rss20FeedItem)feed.Items.Last();
            Eq("Russian Man-Hating Plants and Other Wonderful Creatures", item.Title);
            Eq("https://themoscowtimes.com/articles/russian-man-hating-plants-and-other-wonderful-creatures-56557", item.Link);
            Eq("If misogyny is hatred or prejudice against women, what’s hatred or prejudice against men? This word is much less common in English: misandry. And how do you say “misandry” in Russian? Simple! Феминизм (feminism).", item.Description);
            Eq("Fri, 16 Dec 2016 11:00:00 +0000", item.PublishingDateString);
            Eq("https://themoscowtimes.com/articles/russian-man-hating-plants-and-other-wonderful-creatures-56557", item.Guid);

        }

        [TestMethod]
        public void TestAllFilesForException()
        {
            var files = System.IO.Directory.EnumerateFiles("Feeds");
            foreach(var file in files)
            {
                var feed = FeedReader.ReadFromFile(file);
                if (feed != null)
                    Assert.IsTrue(!string.IsNullOrEmpty(feed.Link));
            }
        }
    }
}
