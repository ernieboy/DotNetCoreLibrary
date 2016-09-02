namespace Core.Common.Utilities
{
    public static class StringUtils
    {

        ///https://msdn.microsoft.com/en-us/library/97af8hh4(v=vs.110).aspx
        public static string GenerateLowercase32DigitsGuid()
        {
            return System.Guid.NewGuid().ToString("N");
        }
    }
}