using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeHollow.FeedReader.Tests
{
    [TestClass]
    public class IssueFixesTest
    {
        // #49: code pages must work without the app registering CodePagesEncodingProvider itself
        [TestMethod]
        public void TestWindows1251FeedIsDecoded()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"windows-1251\"?><rss version=\"2.0\"><channel><title>Новости</title>"
                + "<item><title>Привет</title></item></channel></rss>";
            // take the encoding from the provider directly - registering it here would hide the bug
            byte[] bytes = CodePagesEncodingProvider.Instance.GetEncoding(1251).GetBytes(xml);

            var feed = FeedReader.ReadFromByteArray(bytes);

            Assert.AreEqual("Новости", feed.Title);
            Assert.AreEqual("Привет", feed.Items.Single().Title);
        }

        // #56: atom categories prefer label, then term, then element text
        [TestMethod]
        public void TestAtomCategoryUsesLabelThenTermThenText()
        {
            var feed = FeedReader.ReadFromString(@"<feed xmlns=""http://www.w3.org/2005/Atom""><title>t</title>
<category term=""tech"" label=""Technology""/>
<entry><title>e</title><category term=""dotnet"" label=""""/><category term=""x"" label=""Label X""/><category>plain</category></entry></feed>");

            Assert.AreEqual("Technology", ((Feeds.AtomFeed)feed.SpecificFeed).Categories.Single());
            CollectionAssert.AreEqual(new[] { "dotnet", "Label X", "plain" }, feed.Items.Single().Categories.ToArray());
        }

        // #55 / #61: a custom HttpClient (proxy, logging, timeout) is used for downloads
        [TestMethod]
        public async Task TestCustomHttpClientIsUsed()
        {
            var handler = new FakeHandler("<rss version=\"2.0\"><channel><title>from custom client</title></channel></rss>");
            try
            {
                Helpers.HttpClient = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(5) };

                var feed = await FeedReader.ReadAsync("https://example.test/feed").ConfigureAwait(false);

                Assert.AreEqual("from custom client", feed.Title);
                Assert.AreEqual(1, handler.Calls);
                Assert.IsTrue(handler.LastRequest.Headers.UserAgent.Any(), "default user-agent is still sent");
            }
            finally
            {
                Helpers.HttpClient = null;
            }
            Assert.IsNotNull(Helpers.HttpClient, "null falls back to the built-in client");
        }

        private sealed class FakeHandler : HttpMessageHandler
        {
            private readonly string _content;
            public int Calls;
            public HttpRequestMessage LastRequest;

            public FakeHandler(string content) { _content = content; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Calls++;
                LastRequest = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(_content) });
            }
        }
    }
}
