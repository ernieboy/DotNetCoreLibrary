using System;
using System.Data.SqlTypes;
using Core.Common.Utilities;

namespace Core.Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Determines if a given string is null, empty or consists only of white space. 
        /// </summary>
        /// <param name="targetString">The string to test</param>
        /// <returns>True if the string is null, empty or consists of only whitespace</returns>
        public static bool IsNullOrWhiteSpace(this string targetString)
        {
            bool result = string.IsNullOrWhiteSpace(targetString);
            return result;
        }

        /// <summary>
        /// Determines if a given string is parsable as a valid Microsoft SQL Server date within the range 1753 and 9999.
        /// </summary>
        /// <param name="stringValue">The string to test</param>
        /// <returns>True if the string can be converted to a valid MS SQL Server date.</returns>
        public static bool IsParsableAsValidMsSqlServerDate(this string stringValue)
        {
            if (stringValue.IsNullOrWhiteSpace()) throw Error.ArgumentNull(nameof(stringValue));
            DateTime testDate;
            bool returnValue;
            if (!DateTime.TryParse(stringValue, out testDate)) return false;
            try
            {
                var sdt = new SqlDateTime(testDate);
                returnValue = ((sdt.Value >= SqlDateTime.MinValue.Value) && (sdt.Value <= SqlDateTime.MaxValue.Value));
            }
            catch (SqlTypeException)
            {
                return false;
            }
            return returnValue;
        }
    }
}
