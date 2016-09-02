namespace Core.Common.Utilities
{
    /// <summary>
    /// Class that contains date formats constants 
    /// </summary>
    public static class DateFormat
    {
        /// <summary>
        /// British date format using slash as separator (dd/MM/yyyy) e.g. 23/06/1977 
        /// </summary>
        public static readonly string BritishWithSlashSparator  = "dd/MM/yyyy";

        /// <summary>
        /// British date format using hyphen as separator (dd-MM-yyyy) e.g. 23-06-1977
        /// </summary>
        public static readonly string BritishWithHyphenSeparator = "dd-MM-yyyy";

        /// <summary>
        /// American date format with two digits for the year using slash separator (MM/dd/yy) e.g 01/23/61 
        /// </summary>
        public static readonly string AmericanTwoDigitsYearWithSlashSparator = "MM/dd/yy";  

        /// <summary>
        /// American date format with two digits for the year using hyphen separator (MM-dd-yy) e.g 01-23-61 
        /// </summary>
        public static readonly string AmericanTwoDigitsYearWithHyphenSparator = "MM-dd-yy";
        

        /// <summary>
        /// American date format with four digits for the year using slash separator (MM/dd/yyyy) e.g 01/23/1961 
        /// </summary>
        public static readonly string AmericanFourDigitsYearWithSlashSparator = "MM/dd/yyyy";

        /// <summary>
        /// American date format with four digits for the year using hyphen separator (MM-dd-yyyy) e.g 01-23-1961 
        /// </summary>
        public static readonly string AmericanFourDigitsYearWithHyphenSparator = "MM-dd-yyyy";
    }
}
