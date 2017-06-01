using System;
using System.Linq;

namespace CodeHollow.FeedReader.ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter feed url:");
            string url = Console.ReadLine();

            var urls = FeedReader.GetFeedUrlsFromUrlAsync(url).Result;
            
            string feedUrl;
            if (urls.Count() < 1)
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

            var reader = FeedReader.ReadAsync(feedUrl).Result;

            foreach (var item in reader.Items)
            {
                Console.WriteLine(item.Title + " - " + item.Link);
            }

            Console.ReadKey();
        }
    }
}
