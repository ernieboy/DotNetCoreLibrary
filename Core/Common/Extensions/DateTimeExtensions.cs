using System;
using System.Globalization;
using Core.Common.Utilities;

namespace Core.Common.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeExtensions
    {

        /// <summary>
        /// Constructs a British DateTime instance by parsing a string that contains the date e.g 19/11/2008 or 19/11/08 etc
        /// </summary>
        /// <param name="dateTime">The type we are extending with this method</param>
        /// <param name="dateString">The string to parse in order to build the date</param>
        /// <returns>A nullable DateTime instance</returns>
        public static DateTime ParseBritishDateFromString(this DateTime dateTime, string dateString)
        {
            DateTime toReturn = DateTime.MinValue;
            try
            {
                toReturn = DateTime.Parse(dateString, new CultureInfo("en-GB"));
                return toReturn;
            }
            catch (FormatException)
            {
                return toReturn;
            }
        }

        /// <summary>
        /// Constructs a British DateTime instance by parsing a string that contains a American date format e.g 11/19/2008
        /// </summary>
        /// <param name="dateTime">The type we are extending with this method</param>
        /// <param name="dateString">The string to parse in order to build the date</param>
        /// <param name="sourceStringDateFormat">The date format of the string to be parsed e.g. MM/dd/yyyy or MM-dd-yyyy </param>
        /// <returns>A nullable DateTime instance</returns>
        public static DateTime ParseBritishDateFromAmericanDateString(this DateTime dateTime, string dateString, string sourceStringDateFormat)
        {
            DateTime toReturn = DateTime.MinValue;
            try
            {
                DateTime dt = DateTime.ParseExact(dateString, sourceStringDateFormat, CultureInfo.InvariantCulture);
                string formattedDate = dt.ToString(DateFormat.BritishWithSlashSparator);
                toReturn = DateTime.Parse(formattedDate, new CultureInfo("en-GB"));
                return toReturn;
            }
            catch (FormatException)
            {
                return toReturn;
            }
        }

        /// <summary>
        /// Constructs a British DateTime instance by parsing a string that contains a short American date format e.g 11/19/08
        /// </summary>
        /// <param name="dateTime">The type we are extending with this method</param>
        /// <param name="dateString">The string to parse in order to build the date</param>
        /// <param name="sourceStringDateFormat">The date format of the string to be parsed e.g. MM/dd/yy or MM-dd-yy</param>
        /// <returns>A nullable DateTime instance</returns>
        public static DateTime ParseBritishDateFromShortAmericanDateString(this DateTime dateTime, string dateString, string sourceStringDateFormat)
        {
            DateTime toReturn = DateTime.MinValue;
            try
            {
                DateTime dt = DateTime.ParseExact(dateString, sourceStringDateFormat, CultureInfo.InvariantCulture);
                string formattedDate = dt.ToString(DateFormat.BritishWithSlashSparator);
                toReturn = DateTime.Parse(formattedDate, new CultureInfo("en-GB"));
                return toReturn;
            }
            catch (FormatException)
            {
                return toReturn;
            }
        }


    }
}
