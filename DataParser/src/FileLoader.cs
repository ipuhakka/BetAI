using System;
using System.Net;
using System.IO;

namespace DataParser
{
    public class FileLoader
    {
        /// <summary>
        /// Loads a file, using a WebClient-object, from address to filepath. 
        /// Returns 1 on success.
        /// </summary>
        /// <param name="address">Address from which file is loaded.</param>
        /// <param name="filepath">File to which data is loaded.</param>
        /// <exception cref="LoadException">Thrown when file download throws WebException.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if filepath given is not valid.</exception>
        public int LoadFile(string address, string filepath)
        {
            if (!FileOperations.IsValidPath(filepath))
                throw new DirectoryNotFoundException();

            using (var client = new WebClient())
            {
               try
                {
                    Console.WriteLine("Downloading file from " + address + " to path " + filepath);
                    client.DownloadFile(address, filepath);
                }
                catch (WebException)
                {
                    Console.WriteLine("Something went wrong with downloading file; Check connection");
                    throw new LoadException();
                }
            }
            return 1;
        }
    }
}
