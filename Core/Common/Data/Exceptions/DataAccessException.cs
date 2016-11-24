using System;

namespace Core.Common.Data.Exceptions
{
    /// <summary>
    /// A common exception which is thrown when data access problems occur 
    /// </summary>
    public class DataAccessException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DataAccessException()
        {
        }

        /// <summary>
        /// Constructor 1 - Initialises the exception with a message
        /// </summary>
        /// <param name="message">Message to indicate reason why exception occurred</param>
        public DataAccessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor 2 - Initialises the exception with a message and an inner exception
        /// </summary>
        /// <param name="message">Message to indicate reason why exception occurred</param>
        /// <param name="innerException">Inner exception</param>
        public DataAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}