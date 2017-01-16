namespace CodeHollow.FeedReader
{
    using System;

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
            url = System.Web.HttpUtility.UrlDecode(url);
            using (var webclient = new System.Net.WebClient())
            {
                // header required - without it, some pages return a bad request (e.g. http://www.methode.at/blog?format=RSS)
                // see: https://msdn.microsoft.com/en-us/library/system.net.webclient(v=vs.110).aspx
                webclient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                return webclient.DownloadString(url);
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
