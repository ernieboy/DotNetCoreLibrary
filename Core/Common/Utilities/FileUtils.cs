using System.Collections.Generic;
using System.IO;
using Core.Common.Extensions;

namespace Core.Common.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public  static class FileUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="absoluteFilePath"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadAllLinesFromFile(string absoluteFilePath)
        {
            if(absoluteFilePath.IsNullOrWhiteSpace()) throw Error.ArgumentNull(nameof(absoluteFilePath));            
            using (TextReader reader = File.OpenText(absoluteFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
