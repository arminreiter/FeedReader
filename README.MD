# FeedReader
FeedReader is a .net library used for reading and parsing RSS and ATOM feeds. Supports RSS 0.91, 0.92, 1.0, 2.0 and ATOM.
Developed because tested existing libraries do not work with different languages or have other issues. 
Library tested with multiple languages and feeds.

FeedReader library is **currently under development**

## Usage
The simplest way to read a feed and show the information is:
```cs
    var feed = FeedReader.Read("https://codehollow.com/feed");

    Console.WriteLine("Feed Title: " + feed.Title);
    Console.WriteLine("Feed Description: " + feed.Description);
    Console.WriteLine("Feed Image: " + feed.ImageUrl);
    // ...
    foreach(var item in feed.Items)
    {
        Console.WriteLine(item.Title + " - " + item.Link);
    }
```

If you want to get all properties, then you have to know the type:
```cs
    var feed = FeedReader.Read("https://codehollow.com/feed");

    Console.WriteLine("Feed Title: " + feed.Title);
            
    if(feed.Type == FeedType.Rss_2_0)
    {
        var rss20feed = (Feeds.Rss20Feed)feed.SpecificFeed;
        Console.WriteLine("Generator: " + rss20feed.Generator);
    }
```



## Specifications
- RSS 0.91: http://www.rssboard.org/rss-0-9-1-netscape
- RSS 0.92: http://backend.userland.com/rss092, http://www.rssboard.org/rss-0-9-2
- RSS 1.0 : http://web.resource.org/rss/1.0/spec
- RSS 2.0 : https://validator.w3.org/feed/docs/rss2.html
- ATOM    : https://validator.w3.org/feed/docs/atom.html
