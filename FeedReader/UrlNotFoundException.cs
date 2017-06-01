namespace CodeHollow.FeedReader
{
    using System;

    /// <summary>
    /// An exception thrown when the given url was not found.
    /// </summary>
    public sealed class UrlNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new UrlNotFoundException
        /// </summary>
        public UrlNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new UrlNotFoundException with a message
        /// </summary>
        /// <param name="message">custom error message</param>
        public UrlNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new UrlNotFoundException with a message and an innerException
        /// </summary>
        /// <param name="message">custom error message</param>
        /// <param name="innerException">the inner exception</param>
        public UrlNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}