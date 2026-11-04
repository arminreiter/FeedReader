# FeedReader
[![NuGet](https://img.shields.io/nuget/v/CodeHollow.FeedReader.svg)](https://www.nuget.org/packages/CodeHollow.FeedReader/)

FeedReader is a .net library used for reading and parsing RSS, ATOM and JSON feeds. Supports **RSS 0.91, 0.92, 1.0, 2.0, ATOM (1.0 and 0.3) and JSON Feed (1.0 and 1.1)**, plus the iTunes and Podcasting 2.0 (`podcast:`) podcast extensions.
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

Podcast feeds: the iTunes and Podcasting 2.0 tags are available via extension methods:

```csharp
    using CodeHollow.FeedReader.Feeds.Itunes;
    using CodeHollow.FeedReader.Feeds.Podcast;

    var feed = await FeedReader.ReadAsync("https://feeds.podcastindex.org/pc20.xml");
    var podcast = feed.GetPodcastChannel();              // podcast:guid, locked, funding, person, ...
    Console.WriteLine("Guid: " + podcast.Guid);

    foreach (var item in feed.Items)
    {
        var episode = item.GetPodcastItem();             // podcast:transcript, chapters, season, episode, ...
        Console.WriteLine(item.Title + " - transcripts: " + episode.Transcripts.Length);
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

## Build & Test
```bash
dotnet test FeedReader.Tests
```

The library is strong-name signed with `key.snk` (SHA1). Distros that block SHA1 signatures (e.g. Fedora/RHEL) fail the signed build with `invalid digest`. To build the signed NuGet package on Linux (output: `artifacts/`):
```bash
build/pack.sh
```


## Specifications
- RSS 0.91: http://www.rssboard.org/rss-0-9-1-netscape
- RSS 0.92: http://backend.userland.com/rss092, http://www.rssboard.org/rss-0-9-2
- RSS 1.0 : http://web.resource.org/rss/1.0/spec
- RSS 2.0 : https://validator.w3.org/feed/docs/rss2.html
- ATOM    : https://validator.w3.org/feed/docs/atom.html
- JSON Feed: https://jsonfeed.org/version/1.1
- Podcasting 2.0 namespace: https://podcastindex.org/namespace/1.0

## Release
```bash
dotnet test FeedReader.Tests   # workflow doesn't run tests
build/release.sh 2.1.0         # needs clean working tree
```
Bumps `<Version>` in the csproj, commits, tags `v2.1.0` and pushes. The tag push runs `.github/workflows/release.yml`: pack → nuget.org (Trusted Publishing, repo secret `NUGET_USER`) → GitHub release with the `.nupkg` attached.
