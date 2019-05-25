using System;
using System.Linq;

namespace CodeHollow.FeedReader.ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string hlp = "Please enter feed url or exit to stop the program:";
            Console.WriteLine(hlp);

            while (true)
            {
                try
                {
                    string url = Console.ReadLine();
                    if (url.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                        break;

                    var urlsTask = FeedReader.GetFeedUrlsFromUrlAsync(url);
                    var urls = urlsTask.Result;

                    string feedUrl;
                    if (urls == null || urls.Count() < 1)
                        feedUrl = url;
                    else if (urls.Count() == 1)
                        feedUrl = urls.First().Url;
                    else if (urls.Count() == 2) // if 2 urls, then its usually a feed and a comments feed, so take the first per default
                        feedUrl = urls.First().Url;
                    else
                    {
                        int i = 1;
                        Console.WriteLine("Found multiple feed, please choose:");
                        foreach (var feedurl in urls)
                        {
                            Console.WriteLine($"{i++.ToString()} - {feedurl.Title} - {feedurl.Url}");
                        }
                        var input = Console.ReadLine();

                        if (!int.TryParse(input, out int index) || index < 1 || index > urls.Count())
                        {
                            Console.WriteLine("Wrong input. Press key to exit");
                            Console.ReadKey();
                            return;
                        }
                        feedUrl = urls.ElementAt(index).Url;
                    }

                    var readerTask = FeedReader.ReadAsync(feedUrl);
                    readerTask.ConfigureAwait(false);

                    foreach (var item in readerTask.Result.Items)
                    {
                        Console.WriteLine(item.Title + " - " + item.Link);
                    }
                }             
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.InnerException.Message}{Environment.NewLine}{ex.InnerException.ToString()}");
                }
                finally
                {
                    Console.WriteLine("================================================");
                    Console.WriteLine(hlp);
                }
            }
        }
    }
}
