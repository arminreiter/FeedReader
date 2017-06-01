namespace CodeHollow.FeedReader.Parser
{
    using System;

    /// <summary>
    /// Exception is thrown if the type of the feed is not supported. Supported feed types
    /// are RSS 0.91, RSS 0.92, RSS 1.0, RSS 2.0 and ATOM
    /// </summary>
    public class FeedTypeNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new FeedTypeNotSupportedException
        /// </summary>
        public FeedTypeNotSupportedException()
        {
        }

        /// <summary>
        /// Initializes a new FeedTypeNotSupportedException with a message
        /// </summary>
        /// <param name="message">custom error message</param>
        public FeedTypeNotSupportedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new FeedTypeNotSupportedException with a message and an innerException
        /// </summary>
        /// <param name="message">custom error message</param>
        /// <param name="innerException">the inner exception</param>
        public FeedTypeNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}