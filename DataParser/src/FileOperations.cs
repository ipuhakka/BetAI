using System;
using System.IO;

namespace DataParser
{
    public class FileOperations
    {
        /// <summary>
        /// Checks if getting the full path throws any errors. Returns true if path is correct,
        /// false if not.
        /// </summary>
        public static bool IsValidPath(string filepath)
        {
            try
            {
                var fullPath = Path.GetFullPath(filepath);
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid filepath " + filepath);
                return false;
            }
        }
    }
}
