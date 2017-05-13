namespace CodeHollow.FeedReader
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// static class with helper functions
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Download the content from an url
        /// </summary>
        /// <param name="url">correct url</param>
        /// <returns>content as string</returns>
        public static string Download(string url)
        {
            url = System.Net.WebUtility.UrlDecode(url);

            using (var httpclient = new HttpClient())
            {
                //webclient.Encoding = System.Text.Encoding.UTF8; // TODO
                // header required - without it, some pages return a bad request (e.g. http://www.methode.at/blog?format=RSS)
                // see: https://msdn.microsoft.com/en-us/library/system.net.webclient(v=vs.110).aspx
                httpclient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0");
                httpclient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                
                string result = string.Empty;
                Task.Run(async () =>
                    {
                        var response = await httpclient.GetAsync(url);
                        
                        if(!response.IsSuccessStatusCode)
                        {
                            httpclient.DefaultRequestHeaders.Clear();
                            // httpclient.Headers are now empty. Some pages return forbidden if user-agent is set.
                            response = await httpclient.GetAsync(url);
                        }

                        // ReadAsByteArray avoids encoding issues that probably occur by using ReadAsStringAsync()
                        //  - this issue is captured by testcase TestReadRss20FeedCharter97Handle403Forbidden
                        var resultAsByteArray = await response.Content.ReadAsByteArrayAsync();
                        result = System.Text.Encoding.UTF8.GetString(resultAsByteArray);
                    }).Wait();

                return result;
            }

        }

        /// <summary>
        /// Tries to parse the string as datetime and returns null if it fails
        /// </summary>
        /// <param name="datetime">datetime as string</param>
        /// <returns>datetime or null</returns>
        public static DateTime? TryParseDateTime(string datetime)
        {
            if (string.IsNullOrEmpty(datetime))
                return null;
            DateTimeOffset dt;
            if (!DateTimeOffset.TryParse(datetime, out dt))
            {
                // Do, 22 Dez 2016 17:36:00 +0000
                // note - tried ParseExact with diff formats like "ddd, dd MMM yyyy hh:mm:ss K"
                if (datetime.Contains(","))
                {
                    int pos = datetime.IndexOf(',') + 1;
                    string newdtstring = datetime.Substring(pos).Trim();

                    DateTimeOffset.TryParse(newdtstring, out dt);
                }
            }

            if (dt == default(DateTimeOffset))
                return null;

            return dt.UtcDateTime;
        }

        /// <summary>
        /// Tries to parse the string as int and returns null if it fails
        /// </summary>
        /// <param name="input">int as string</param>
        /// <returns>integer or null</returns>
        public static int? TryParseInt(string input)
        {
            int tmp;
            if (!int.TryParse(input, out tmp))
                return null;
            return tmp;
        }
    }
}
