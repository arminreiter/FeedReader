using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeHollow.FeedReader.TestDataCrawler
{
    class Program
    {
        static void Main(string[] args)
        {


            var contents = FeedReader.ReadAsync("https://www.telegraaf.nl/rss.xml").Result;

            var x = contents.Items.Count;

            //var feeds = System.IO.File.ReadAllLines("feeds.txt");
            //Parallel.ForEach<string>(feeds, x =>
            //{
            //    try
            //    {
            //        Do(x);
            //    }
            //    catch { }
            //});
            
        }

        static void Do(string url)
        {
            var links = FeedReader.GetFeedUrlsFromUrlAsync(url).Result;
            
            foreach (var link in links)
            {
                try
                {
                    string title = link.Title;
                    if (string.IsNullOrEmpty(title))
                    {
                        title = url.Replace("https", "").Replace("http", "").Replace("www.","");
                       
                    }
                    title = Regex.Replace(title.ToLower(), "[^a-z]*", "");
                    var curl = FeedReader.GetAbsoluteFeedUrl(url, link);

                    string content = Helpers.DownloadAsync(curl.Url).Result;
                    System.IO.File.WriteAllText("c:\\data\\feeds\\" + title + "_" + Guid.NewGuid().ToString() + ".xml", content);
                    Console.Write("+");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(link.Title + " - " + link.Url + ": " + ex.ToString());
                }
            }
        }
    }
}
