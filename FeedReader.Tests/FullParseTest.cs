using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using CodeHollow.FeedReader.Feeds;
using CodeHollow.FeedReader.Feeds.Itunes;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class FullParseTest
    {
        private void Eq(object expected, object actual)
        {
            Assert.AreEqual(expected, actual);
        }

        #region Synchronous 

        [TestMethod]
        public void TestAtomParseTheVerge()
        {
            var feed = (AtomFeed)FeedReader.ReadFromFile("Feeds/AtomTheVerge.xml").SpecificFeed;

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
        public void TestAtomParseBattleNet()
        {
            var feed = (AtomFeed)FeedReader.ReadFromFile("Feeds/AtomBattleNet.xml").SpecificFeed;

            Eq("StarCraft® II", feed.Title);
            Eq(null, feed.Icon);
            Eq(null, feed.Link);
            Eq("2018-11-20T19:59:19.147Z", feed.UpdatedDateString);
            Eq("3", feed.Id);
        }

        [TestMethod]
        public void TestAtomYouTubeInvestmentPunk()
        {
            var feed = (AtomFeed)FeedReader.ReadFromFile("Feeds/AtomYoutubeInvestmentPunk.xml").SpecificFeed;

            Eq("http://www.youtube.com/feeds/videos.xml?channel_id=UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.First().Href);
            Eq("yt:channel:UCmEN5ZnsHUXIxgpLitRTmWw", feed.Id);
            Eq("Investment Punk Academy by Gerald Hörhan", feed.Title);
            Eq("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.ElementAt(1).Href);
            Eq("Investment Punk Academy by Gerald Hörhan", feed.Author.Name);
            Eq("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Author.Uri);
            var item = (AtomFeedItem)feed.Items.First();
            Eq("yt:video:AFA8ZtMwrvc", item.Id);
            Eq("Zukunft von Vertretern I Kernfusion I Musikgeschäft #ASKTHEPUNK 71", item.Title);
            Eq("alternate", item.Links.First().Relation);
            Eq("2017-01-23T18:14:49+00:00", item.UpdatedDateString);
            Eq("2017-01-20T16:00:00+00:00", item.PublishedDateString);
        }

        [TestMethod]
        public void TestRss091ParseStadtFWeiz()
        {
            var feed = (Rss091Feed)FeedReader.ReadFromFile("Feeds/Rss091Stadtfeuerwehr.xml").SpecificFeed;

            Eq("Stadtfeuerwehr Weiz - Einsätze", feed.Title);
            Eq("http://www.stadtfeuerwehr-weiz.at", feed.Link);
            Eq("Die letzten 15 Einsätze der Stadtfeuerwehr Weiz.", feed.Description);
            Eq("de-de", feed.Language);
            Eq("Stadtfeuerwehr Weiz / Markus Horwath", feed.Copyright);

            var item = (Rss091FeedItem)feed.Items.First();

            Eq(@"[19.08.2018 - 07:08 Uhr] Brandmeldeanlagenalarm", item.Title.Trim());
            Assert.IsTrue(item.Description.Contains("Weitere Informationen"));
            Eq("http://www.stadtfeuerwehr-weiz.at/einsaetze/einsatz-detail/5220/", item.Link);
            Eq("Sun, 19 Aug 2018 07:08:00 +0100", item.PublishingDateString);
            Eq(new DateTime(2018, 8, 19, 6, 08, 0), item.PublishingDate);

            Eq(15, feed.Items.Count);
        }

        [TestMethod]
        public void TestRss091ParseFullSample()
        {
            var feed = (Rss091Feed)FeedReader.ReadFromFile("Feeds/Rss091FullSample.xml").SpecificFeed;
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
        public void TestRss092ParseFullSample()
        {
            var feed = (Rss092Feed)FeedReader.ReadFromFile("Feeds/Rss092FullSample.xml").SpecificFeed;

            Eq("Dave Winer: Grateful Dead", feed.Title);
            Eq("http://www.scripting.com/blog/categories/gratefulDead.html", feed.Link);
            Eq("A high-fidelity Grateful Dead song every day. This is where we're experimenting with enclosures on RSS news items that download when you're not using your computer. If it works (it will) it will be the end of the Click-And-Wait multimedia experience on the Internet. ", feed.Description);
            Eq("Fri, 13 Apr 2001 19:23:02 GMT", feed.LastBuildDateString);
            Eq("http://backend.userland.com/rss092", feed.Docs);
            Eq("dave@userland.com (Dave Winer)", feed.ManagingEditor);
            Eq("dave@userland.com (Dave Winer)", feed.WebMaster);
            Eq("data.ourfavoritesongs.com", feed.Cloud.Domain);
            Eq("80", feed.Cloud.Port);
            Eq("/RPC2", feed.Cloud.Path);
            Eq("ourFavoriteSongs.rssPleaseNotify", feed.Cloud.RegisterProcedure);
            Eq("xml-rpc", feed.Cloud.Protocol);

            Eq(22, feed.Items.Count);
            var item = (Rss092FeedItem)feed.Items.ElementAt(20);
            Eq("A touch of gray, kinda suits you anyway..", item.Description);
            Eq("http://www.scripting.com/mp3s/touchOfGrey.mp3", item.Enclosure.Url);
            Eq(5588242, item.Enclosure.Length);
            Eq("audio/mpeg", item.Enclosure.MediaType);

            var secondItem = (Rss092FeedItem)feed.Items.ElementAt(1);
            Eq("http://scriptingnews.userland.com/xml/scriptingNews2.xml", secondItem.Source.Url);
            Eq("Scripting News", secondItem.Source.Value);
        }

        [TestMethod]
        public void TestRss10ParseFullSample()
        {
            var feed = (Rss10Feed)FeedReader.ReadFromFile("Feeds/Rss10FeedWebResourceSample.xml").SpecificFeed;

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
            var feed = (Rss10Feed)FeedReader.ReadFromFile("Feeds/Rss10OrfAt.xml").SpecificFeed;
            Eq("news.ORF.at", feed.Title);
            Eq("http://orf.at/", feed.Link);
            Eq("2017-01-23T21:54:55+01:00", feed.DC.DateString);
            Eq("Die aktuellsten Nachrichten auf einen Blick - aus Österreich und der ganzen Welt. In Text, Bild und Video.", feed.Description);
            Eq("ORF Österreichischer Rundfunk, Wien", feed.DC.Publisher);
            Eq("ORF Online und Teletext GmbH & Co KG", feed.DC.Creator);
            Eq("de", feed.DC.Language);
            Eq("Copyright © 2017 ORF Online und Teletext GmbH & Co KG", feed.DC.Rights);
            Eq("hourly", feed.Sy.UpdatePeriod);
            Eq("2", feed.Sy.UpdateFrequency);
            Eq("2000-01-01T00:00:00Z", feed.Sy.UpdateBase);
            Eq(50, feed.Items.Count);

            var item = (Rss10FeedItem)feed.Items.ElementAt(4);

            Eq("Feldsperling erstmals häufigster Vogel", item.Title);
            Eq("http://orf.at/stories/2376365/", item.Link);
            Eq("Chronik", item.DC.Subject);
            Eq("2017-01-23T20:51:06+01:00", item.DC.DateString);
        }

        [TestMethod]
        public void TestRss20ParseWebResourceSampleFull()
        {
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20FeedWebResourceSample.xml").SpecificFeed;

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
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20CodeHollowCom.xml").SpecificFeed;

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
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20ContentWindCom.xml").SpecificFeed;
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
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20MoscowTimes.xml").SpecificFeed;
            Eq("The Moscow Times - News, Business, Culture & Multimedia from Russia", feed.Title);
            Eq("https://themoscowtimes.com/", feed.Link);
            Eq("The Moscow Times offers everything you need to know about Russia: Breaking news, top stories, business, analysis, opinion, multimedia, upcoming cultural events", feed.Description);
            Eq("en-us", feed.Language);
            Eq("Mon, 23 Jan 2017 16:45:02 +0000", feed.LastBuildDateString);
            Eq("600", feed.TTL);

            var item = (Rss20FeedItem)feed.Items.First();
            Eq("Russian State TV Praises Trump for Avoiding ‘Democracy’ in Inauguration Speech", item.Title);
            Eq("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Link);
            Eq("Though he welcomed the end of Obama’s presidency as the start of a bright new era, the Kremlin’s “chief propagandist” quickly found himself struggling to find convincing scapegoats for the world’s problems this week.", item.Description);
            Eq("Mon, 23 Jan 2017 16:45:02 +0000", item.PublishingDateString);
            Eq("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Guid);

            item = (Rss20FeedItem)feed.Items.Last();
            Eq("Don’t Say It", item.Title);
            Eq("https://themoscowtimes.com/articles/dont-say-it-56774", item.Link);
            Eq("They say “sex sells,” but don't go peddling it near dinner tables in Russia, where families in an ostensibly conservative society say the subject is too taboo to discuss at home.", item.Description);
            Eq("Tue, 10 Jan 2017 19:58:13 +0000", item.PublishingDateString);
            Eq("https://themoscowtimes.com/articles/dont-say-it-56774", item.Guid);
        }

        [TestMethod]
        public void TestRss20ParseSwedishFeedWithIso8859_1()
        {
            var feed = (Rss20Feed)FeedReader.ReadFromFile("Feeds/Rss20ISO88591Intranet30.xml").SpecificFeed;
            Eq("intranet30", feed.Title);
            Eq("http://www.retriever-info.com", feed.Link);
            Eq("RSS 2.0 News feed from Retriever Norge AS", feed.Description);
            
            var item = (Rss20FeedItem)feed.Items.First();
            Eq("SVART MÅNAD - DÖDSOLYCKA i Vetlanda", item.Title);
            Eq("https://www.retriever-info.com/go/?a=30338&d=00201120180819281555686&p=200108&s=2011&sa=2016177&u=http%3A%2F%2Fwww.hoglandsnytt.se%2Fsvart-manad-dodsolycka-i-vetlanda%2F&x=33d88e677ce6481d9882de22c76e4234", item.Link);
            Eq("Under juli 2018 omkom 39 personer och 1 521 skadades i vägtrafiken. Det visar de preliminära uppgifter som inkommit till Transportstyrelsen fram till den 15 augusti 2018. Det är åtta fler omkomna jämfört med juli månad 2017.", item.Description);
            Eq("Sun, 19 Aug 2018 07:14:00 GMT", item.PublishingDateString);
            Eq("00201120180819281555686", item.Guid);
            Eq("Höglandsnytt", item.Author);

        }

        [TestMethod]
        public void TestRss20CityDogKyrillicNoEncodingDefined()
        {
            var feed = FeedReader.ReadFromFile("Feeds/Rss20CityDog.xml");
            Eq("Новости - citydog.by", feed.Title);
            Eq("Последние обновления - citydog.by", feed.Description);

            var item = feed.Items.First();
            Eq("Группа «Серебряная свадьба» ушла в бессрочный отпуск", item.Title);
            Eq("http://citydog.by/post/zaden-serebrianaya-svadba-v-otpuske/", item.Id);
        }

        [TestMethod]
        public void TestAllFilesForException()
        {
            var linkless = new System.Collections.Generic.List<string>() { "AtomBattleNet.xml" };

            var files = System.IO.Directory.EnumerateFiles("Feeds");
            foreach(var file in files)
            {
                var feed = FeedReader.ReadFromFile(file);
                if (feed != null)
                {
                    string filename = System.IO.Path.GetFileName(file);
                    if (!linkless.Contains(filename))
                        Assert.IsTrue(!string.IsNullOrEmpty(feed.Link));
                    
                    TestItunesParsingForException(feed);
                }
            }
        }

        #endregion Synchronous 

        #region Asynchronous 

        [TestMethod]
        public async Task TestAtomParseTheVerge_Async()
        {
            var feed = (AtomFeed)(await FeedReader.ReadFromFileAsync("Feeds/AtomTheVerge.xml").ConfigureAwait(false)).SpecificFeed;

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
        public async Task TestAtomYouTubeInvestmentPunk_Async()
        {
            var feed = (AtomFeed)(await FeedReader.ReadFromFileAsync("Feeds/AtomYoutubeInvestmentPunk.xml").ConfigureAwait(false)).SpecificFeed;

            Eq("http://www.youtube.com/feeds/videos.xml?channel_id=UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.First().Href);
            Eq("yt:channel:UCmEN5ZnsHUXIxgpLitRTmWw", feed.Id);
            Eq("Investment Punk Academy by Gerald Hörhan", feed.Title);
            Eq("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Links.ElementAt(1).Href);
            Eq("Investment Punk Academy by Gerald Hörhan", feed.Author.Name);
            Eq("http://www.youtube.com/channel/UCmEN5ZnsHUXIxgpLitRTmWw", feed.Author.Uri);
            var item = (AtomFeedItem)feed.Items.First();
            Eq("yt:video:AFA8ZtMwrvc", item.Id);
            Eq("Zukunft von Vertretern I Kernfusion I Musikgeschäft #ASKTHEPUNK 71", item.Title);
            Eq("alternate", item.Links.First().Relation);
            Eq("2017-01-23T18:14:49+00:00", item.UpdatedDateString);
            Eq("2017-01-20T16:00:00+00:00", item.PublishedDateString);
        }

        [TestMethod]
        public async Task TestRss091ParseStadtFWeiz_Async()
        {
            var feed = (Rss091Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss091Stadtfeuerwehr.xml").ConfigureAwait(false)).SpecificFeed;

            Eq("Stadtfeuerwehr Weiz - Einsätze", feed.Title);
            Eq("http://www.stadtfeuerwehr-weiz.at", feed.Link);
            Eq("Die letzten 15 Einsätze der Stadtfeuerwehr Weiz.", feed.Description);
            Eq("de-de", feed.Language);
            Eq("Stadtfeuerwehr Weiz / Markus Horwath", feed.Copyright);

            var item = (Rss091FeedItem)feed.Items.First();

            Eq(@"[19.08.2018 - 07:08 Uhr] Brandmeldeanlagenalarm", item.Title.Trim());
            Assert.IsTrue(item.Description.Contains("Weitere Informationen"));
            Eq("http://www.stadtfeuerwehr-weiz.at/einsaetze/einsatz-detail/5220/", item.Link);
            Eq("Sun, 19 Aug 2018 07:08:00 +0100", item.PublishingDateString);
            Eq(new DateTime(2018, 8, 19, 6, 08, 0), item.PublishingDate);

            Eq(15, feed.Items.Count);
        }

        [TestMethod]
        public async Task TestRss091ParseFullSample_Async()
        {
            var feed = (Rss091Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss091FullSample.xml").ConfigureAwait(false)).SpecificFeed;
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
        public async Task TestRss092ParseFullSample_Async()
        {
            var feed = (Rss092Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss092FullSample.xml").ConfigureAwait(false)).SpecificFeed;

            Eq("Dave Winer: Grateful Dead", feed.Title);
            Eq("http://www.scripting.com/blog/categories/gratefulDead.html", feed.Link);
            Eq("A high-fidelity Grateful Dead song every day. This is where we're experimenting with enclosures on RSS news items that download when you're not using your computer. If it works (it will) it will be the end of the Click-And-Wait multimedia experience on the Internet. ", feed.Description);
            Eq("Fri, 13 Apr 2001 19:23:02 GMT", feed.LastBuildDateString);
            Eq("http://backend.userland.com/rss092", feed.Docs);
            Eq("dave@userland.com (Dave Winer)", feed.ManagingEditor);
            Eq("dave@userland.com (Dave Winer)", feed.WebMaster);
            Eq("data.ourfavoritesongs.com", feed.Cloud.Domain);
            Eq("80", feed.Cloud.Port);
            Eq("/RPC2", feed.Cloud.Path);
            Eq("ourFavoriteSongs.rssPleaseNotify", feed.Cloud.RegisterProcedure);
            Eq("xml-rpc", feed.Cloud.Protocol);

            Eq(22, feed.Items.Count);
            var item = (Rss092FeedItem)feed.Items.ElementAt(20);
            Eq("A touch of gray, kinda suits you anyway..", item.Description);
            Eq("http://www.scripting.com/mp3s/touchOfGrey.mp3", item.Enclosure.Url);
            Eq(5588242, item.Enclosure.Length);
            Eq("audio/mpeg", item.Enclosure.MediaType);

            var secondItem = (Rss092FeedItem)feed.Items.ElementAt(1);
            Eq("http://scriptingnews.userland.com/xml/scriptingNews2.xml", secondItem.Source.Url);
            Eq("Scripting News", secondItem.Source.Value);
        }

        [TestMethod]
        public async Task TestRss10ParseFullSample_Async()
        {
            var feed = (Rss10Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss10FeedWebResourceSample.xml").ConfigureAwait(false)).SpecificFeed;

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
        public async Task TestRss10ParseOrfAt_Async()
        {
            var feed = (Rss10Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss10OrfAt.xml").ConfigureAwait(false)).SpecificFeed;
            Eq("news.ORF.at", feed.Title);
            Eq("http://orf.at/", feed.Link);
            Eq("2017-01-23T21:54:55+01:00", feed.DC.DateString);
            Eq("Die aktuellsten Nachrichten auf einen Blick - aus Österreich und der ganzen Welt. In Text, Bild und Video.", feed.Description);
            Eq("ORF Österreichischer Rundfunk, Wien", feed.DC.Publisher);
            Eq("ORF Online und Teletext GmbH & Co KG", feed.DC.Creator);
            Eq("de", feed.DC.Language);
            Eq("Copyright © 2017 ORF Online und Teletext GmbH & Co KG", feed.DC.Rights);
            Eq("hourly", feed.Sy.UpdatePeriod);
            Eq("2", feed.Sy.UpdateFrequency);
            Eq("2000-01-01T00:00:00Z", feed.Sy.UpdateBase);
            Eq(50, feed.Items.Count);

            var item = (Rss10FeedItem)feed.Items.ElementAt(4);

            Eq("Feldsperling erstmals häufigster Vogel", item.Title);
            Eq("http://orf.at/stories/2376365/", item.Link);
            Eq("Chronik", item.DC.Subject);
            Eq("2017-01-23T20:51:06+01:00", item.DC.DateString);
        }

        [TestMethod]
        public async Task TestRss20ParseWebResourceSampleFull_Async()
        {
            var feed = (Rss20Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss20FeedWebResourceSample.xml").ConfigureAwait(false)).SpecificFeed;

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
        public async Task TestRss20ParseCodeHollow_Async()
        {
            var feed = (Rss20Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss20CodeHollowCom.xml").ConfigureAwait(false)).SpecificFeed;

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
        public async Task TestRss20ParseContentWindGerman_Async()
        {
            var feed = (Rss20Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss20ContentWindCom.xml").ConfigureAwait(false)).SpecificFeed;
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
        public async Task TestRss20ParseMoscowTimes_Async()
        {
            var feed = (Rss20Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss20MoscowTimes.xml").ConfigureAwait(false)).SpecificFeed;
            Eq("The Moscow Times - News, Business, Culture & Multimedia from Russia", feed.Title);
            Eq("https://themoscowtimes.com/", feed.Link);
            Eq("The Moscow Times offers everything you need to know about Russia: Breaking news, top stories, business, analysis, opinion, multimedia, upcoming cultural events", feed.Description);
            Eq("en-us", feed.Language);
            Eq("Mon, 23 Jan 2017 16:45:02 +0000", feed.LastBuildDateString);
            Eq("600", feed.TTL);

            var item = (Rss20FeedItem)feed.Items.First();
            Eq("Russian State TV Praises Trump for Avoiding ‘Democracy’ in Inauguration Speech", item.Title);
            Eq("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Link);
            Eq("Though he welcomed the end of Obama’s presidency as the start of a bright new era, the Kremlin’s “chief propagandist” quickly found himself struggling to find convincing scapegoats for the world’s problems this week.", item.Description);
            Eq("Mon, 23 Jan 2017 16:45:02 +0000", item.PublishingDateString);
            Eq("https://themoscowtimes.com/articles/russian-state-tv-praises-trump-for-avoiding-democracy-in-inauguration-speech-56901", item.Guid);

            item = (Rss20FeedItem)feed.Items.Last();
            Eq("Don’t Say It", item.Title);
            Eq("https://themoscowtimes.com/articles/dont-say-it-56774", item.Link);
            Eq("They say “sex sells,” but don't go peddling it near dinner tables in Russia, where families in an ostensibly conservative society say the subject is too taboo to discuss at home.", item.Description);
            Eq("Tue, 10 Jan 2017 19:58:13 +0000", item.PublishingDateString);
            Eq("https://themoscowtimes.com/articles/dont-say-it-56774", item.Guid);
        }

        [TestMethod]
        public async Task TestRss20ParseSwedishFeedWithIso8859_1_Async()
        {
            var feed = (Rss20Feed)(await FeedReader.ReadFromFileAsync("Feeds/Rss20ISO88591Intranet30.xml").ConfigureAwait(false)).SpecificFeed;
            Eq("intranet30", feed.Title);
            Eq("http://www.retriever-info.com", feed.Link);
            Eq("RSS 2.0 News feed from Retriever Norge AS", feed.Description);
            
            var item = (Rss20FeedItem)feed.Items.First();
            Eq("SVART MÅNAD - DÖDSOLYCKA i Vetlanda", item.Title);
            Eq("https://www.retriever-info.com/go/?a=30338&d=00201120180819281555686&p=200108&s=2011&sa=2016177&u=http%3A%2F%2Fwww.hoglandsnytt.se%2Fsvart-manad-dodsolycka-i-vetlanda%2F&x=33d88e677ce6481d9882de22c76e4234", item.Link);
            Eq("Under juli 2018 omkom 39 personer och 1 521 skadades i vägtrafiken. Det visar de preliminära uppgifter som inkommit till Transportstyrelsen fram till den 15 augusti 2018. Det är åtta fler omkomna jämfört med juli månad 2017.", item.Description);
            Eq("Sun, 19 Aug 2018 07:14:00 GMT", item.PublishingDateString);
            Eq("00201120180819281555686", item.Guid);
            Eq("Höglandsnytt", item.Author);

        }

        [TestMethod]
        public async Task TestRss20CityDogKyrillicNoEncodingDefined_Async()
        {
            var feed = await FeedReader.ReadFromFileAsync("Feeds/Rss20CityDog.xml").ConfigureAwait(false);
            Eq("Новости - citydog.by", feed.Title);
            Eq("Последние обновления - citydog.by", feed.Description);

            var item = feed.Items.First();
            Eq("Группа «Серебряная свадьба» ушла в бессрочный отпуск", item.Title);
            Eq("http://citydog.by/post/zaden-serebrianaya-svadba-v-otpuske/", item.Id);
        }

        [TestMethod]
        public async Task TestAllFilesForException_Async()
        {
            var linkless = new System.Collections.Generic.List<string>() { "AtomBattleNet.xml" };

            var files = System.IO.Directory.EnumerateFiles("Feeds");
            foreach(var file in files)
            {
                var feed = await FeedReader.ReadFromFileAsync(file).ConfigureAwait(false);
                if (feed != null)
                {
                    string filename = System.IO.Path.GetFileName(file);
                    if (!linkless.Contains(filename))
                        Assert.IsTrue(!string.IsNullOrEmpty(feed.Link));

                    TestItunesParsingForException(feed);
                }
            }
        }

        #endregion Asynchronous 

        private static void TestItunesParsingForException(Feed feed)
        {
            Assert.IsNotNull(feed.GetItunesChannel());

            foreach (var item in feed.Items)
                Assert.IsNotNull(item.GetItunesItem());
        }
    }
}
