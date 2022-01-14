# FeedReader
FeedReader is a .net library used for reading and parsing RSS and ATOM feeds. Supports **RSS 0.91, 0.92, 1.0, 2.0 and ATOM**.
Developed because tested existing libraries do not work with different languages, encodings or have other issues. 
Library tested with multiple languages, encodings and feeds.

FeedReader library is available as NuGet package: https://www.nuget.org/packages/CodeHollow.FeedReader/

## Usage
The simplest way to read a feed and show the information is:
```csharp
    var feed = await FeedReader.ReadAsync("https://arminreiter.com/feed");

    Console.WriteLine("Feed Title: " + feed.Title);
    Console.WriteLine("Feed Description: " + feed.Description);
    Console.WriteLine("Feed Image: " + feed.ImageUrl);
    // ...
    foreach(var item in feed.Items)
    {
        Console.WriteLine(item.Title + " - " + item.Link);
    }
```

There are some properties that are only available in e.g. RSS 2.0. If you want to get those properties, the property "SpecificFeed" is the right one:

```csharp
    var feed = await FeedReader.ReadAsync("https://arminreiter.com/feed");

    Console.WriteLine("Feed Title: " + feed.Title);
            
    if(feed.Type == FeedType.Rss_2_0)
    {
        var rss20feed = (Feeds.Rss20Feed)feed.SpecificFeed;
        Console.WriteLine("Generator: " + rss20feed.Generator);
    }
```

If the url to the feed is not known, then you can use FeedReader.GetFeedUrlsFromUrl(url) to parse the url from the html webpage:

```csharp
    string url = "arminreiter.com";
    var urls = FeedReader.GetFeedUrlsFromUrl(url);
            
    string feedUrl;
    if (urls.Count() < 1) // no url - probably the url is already the right feed url
        feedUrl = url;
    else if (urls.Count() == 1)
        feedUrl = urls.First().Url;
    else if (urls.Count() == 2) // if 2 urls, then its usually a feed and a comments feed, so take the first per default
        feedUrl = urls.First().Url;
    else
    {
        // show all urls and let the user select (or take the first or ...)
        // ...
    }

    var readerTask = FeedReader.ReadAsync(feedUrl);
    readerTask.ConfigureAwait(false);

    foreach (var item in readerTask.Result.Items)
    {
        Console.WriteLine(item.Title + " - " + item.Link);
        // ...
    }
```


The code contains a sample console application: https://github.com/codehollow/FeedReader/tree/master/FeedReader.ConsoleSample


## Specifications
- RSS 0.91: http://www.rssboard.org/rss-0-9-1-netscape
- RSS 0.92: http://backend.userland.com/rss092, http://www.rssboard.org/rss-0-9-2
- RSS 1.0 : http://web.resource.org/rss/1.0/spec
- RSS 2.0 : https://validator.w3.org/feed/docs/rss2.html
- ATOM    : https://validator.w3.org/feed/docs/atom.html
