using System;
using System.Data.SqlTypes;
using System.Globalization;

namespace Core.Common.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="americanDate"></param>
        /// <returns></returns>
        public static DateTime? ConvertAmericanDateToBritishDate(string americanDate)
        {
            if (string.IsNullOrWhiteSpace(americanDate)) return null;
            try
            {
                DateTime dt = DateTime.ParseExact(americanDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                string formattedDate = dt.ToString("dd/MM/yyyy");
                DateTime finalDt = DateTime.Parse(formattedDate, new CultureInfo("en-GB"));
                return finalDt;
            }
            catch (FormatException)
            {
                //Log it
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns></returns>
        public static DateTime?ParseShortBritishDateFromString(string dateString)   
        {
            if (string.IsNullOrWhiteSpace(dateString)) return null;
            try
            {
                DateTime dt = DateTime.ParseExact(dateString, "dd/MM/yyyy", new CultureInfo("en-GB"));
                return dt;
            }
            catch (FormatException)
            {
                //Log it
            }

            return null;
        }

        /// <summary>
        /// An better method to verify whether a value is 
        /// kosher for SQL Server datetime. This uses the native library
        /// for checking range values
        /// Borrowed from http://stackoverflow.com/questions/7054782/validate-datetime-before-inserting-it-into-sql-server-database
        /// </summary>
        /// <param name="someval">A date string that may parse</param>
        /// <returns>true if the parameter is valid for SQL Sever datetime</returns>
        public static bool IsValidSqlServerDate(string someval) 
        {
            bool valid = false;
            DateTime testDate;
            if (DateTime.TryParse(someval, out testDate))
            {
                try
                {
                    // take advantage of the native conversion
                    var sdt = new SqlDateTime(testDate);
                    valid = true;
                }
                catch (SqlTypeException)
                {
                    // no need to do anything, this is the expected out of range error
                }
            }

            return valid;
        }
    }
}
