using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeHollow.FeedReader.TestDataCrawler
{
    class Program
    {
        static void Main(string[] args)
        {

            var feeds = System.IO.File.ReadAllLines("feeds.txt");
            Parallel.ForEach<string> (feeds, x =>
               {
                   try
                   {
                       Do(x);
   
                   }
                   catch { }
               }
            );

        }

        static void Do(string url)
        {
            var linksTask = FeedReader.GetFeedUrlsFromUrlAsync(url);

            foreach (var link in linksTask.Result)
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

                    var contentTask = Helpers.DownloadAsync(curl.Url);

                    System.IO.File.WriteAllText("d:\\feeds\\" + title + "_" + Guid.NewGuid().ToString() + ".xml", contentTask.Result);
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
