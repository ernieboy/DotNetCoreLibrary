using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Common.Utilities
{
    public static class SecurityUtils
    {
        /// <summary>
        ///     Returns the md5 hash of an input string.
        /// </summary>
        /// <param name="inputString">The input string</param>
        /// <returns>The MD5 hash value of the input string</returns>
        public static string GetMd5HashValueOfString(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString)) throw new ArgumentException("Input string must not be null", inputString);
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(inputString.Trim());
            var hash = md5.ComputeHash(inputBytes);

            //Convert byte array to hex string
            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

/*
        /// <summary>
        /// Returns the SHA256 hash of an input string.
        /// Thanks goes to: http://stackoverflow.com/questions/12416249/hashing-a-string-with-sha256
        /// </summary>
        /// <param name="inputString">The input string</param>
        /// <returns>The SHA256 hash value of the input string</returns>
        public static string GetSha256HashValueOfString(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString)) throw new ArgumentException("Input string must not be null", inputString);
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            var instance = new S SHA256Managed();
            byte[] hash = instance.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:X2}", x);
            }
            return hashString;
        }
        */
    }
}