using System;
using System.Text;

namespace Core.Common.Utilities
{
    /// <summary>
    /// A utitlity class to help with error handling
    /// </summary>
    public class Error  
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Exception ArgumentNull(string s) => new ArgumentNullException(s);

        /// <summary>
        /// The origin of the error
        /// </summary>
        public enum ErrorOrigin
        {
            /// <summary>
            /// Error that originated from database
            /// </summary>
            Database = 1,
            /// <summary>
            ///  Error that originated from Entity Framework entity validation failure
            /// </summary>
            EntityFrameworkValidationFailure = 2,
            /// <summary>
            ///  Error that originated from trying to access a file
            /// </summary>
            FileAccessFailure = 3,
            /// <summary>
            ///  Any other unspecified error
            /// </summary>
            Other = 4
        };

        /// <summary>
        ///     Returns full details about an Exception object
        /// </summary>
        /// <param name="ex">The Exception object which was thrown when the error occurred</param>
        /// <param name="sb">A string builder object to build the final error message string</param>
        /// <returns>A string builder object which contains the final error message</returns>
        public static StringBuilder BuildExceptionDetail(Exception ex, StringBuilder sb)
        {
            if (ex == null) throw new NullReferenceException("ex");
            if (sb == null) throw new NullReferenceException("sb");
            sb.AppendLine("Message: " + ex.Message);
            sb.AppendLine("Source: " + ex.Source);
            sb.AppendLine("StackTrace: " + ex.StackTrace);

            //Loop recursivly through the inner exceptions if there are any.
            if (ex.InnerException != null)
            {
                sb.AppendLine("InnerException: ");
                BuildExceptionDetail(ex.InnerException, sb);
            }
            return sb;
        }
    }
}
