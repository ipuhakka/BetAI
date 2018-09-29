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
                string fullPath = Path.GetFullPath(filepath);
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid filepath " + filepath);
                return false;
            }
        }

        /// <summary>
        /// Returns all lines in a file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>string[] containing all lines in the file.</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static string[] ReadFile(string path)
        {
            if (File.Exists(path))
                return File.ReadAllLines(path);

            throw new FileNotFoundException();
        }
    }
}
