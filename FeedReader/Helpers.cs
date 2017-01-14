namespace CodeHollow.FeedReader
{
    using System;

    public static class Helpers
    {
        public static DateTime? TryParse(string datetime)
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

        public static int? TryParseInt(string input)
        {
            int tmp;
            if (!int.TryParse(input, out tmp))
                return null;
            return tmp;
        }
    }
}
