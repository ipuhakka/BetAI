using System;
using System.IO;
using System.Collections.Generic;
using BetAI.Exceptions;
using Newtonsoft.Json;

namespace BetAI.FileOperations
{
    public class Save
    {
        /// <summary>
        /// Function creates a new folder named filename to Files-folder.
        /// If no args are given, values from Files\defaults.json are used.
        /// If any of the arguments (alpha, minimumStake, numberOfNodes, sampleSize, database)
        /// are given, they are written to values.json instead of value described in defaults.json.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="args"></param>
        /// <exception cref="DirectoryExistsException">Directory is already in use.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when argument name and value are not separated with '=' character.</exception>
        /// <exception cref="FormatException">Thrown if args contains invalid data</exception>
        public static void InitializeSave(string filename, params string[] args)
        {
            string relativePath = Path.Combine(@"Files\", filename);
            if (Directory.Exists(relativePath))
                throw new DirectoryExistsException();

            Directory.CreateDirectory(relativePath);

            if (args == null || args.Length == 0)
            {
                File.Copy(@"Files\defaults.json", Path.Combine(relativePath, "values.json"));
                return;
            }

            dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(@"Files\defaults.json"));

            foreach (string arg in args)
            {
                string[] argument = arg.Split('=');
                switch (argument[0].Trim().ToLower())
                {
                    case "alpha":
                        json["alpha"] = Convert.ToDouble(argument[1]);
                        break;
                    case "minimumstake":
                        json["minimumStake"] = Convert.ToDouble(argument[1]);
                        break;
                    case "numberofnodes":
                        json["numberOfNodes"] = Convert.ToInt32(argument[1]);
                        break;
                    case "samplesize":
                        json["sampleSize"] = Convert.ToInt32(argument[1]);
                        break;
                    case "database":
                        json["database"] = argument[1].Replace(@"\", @"\\");
                        break;
                    default:
                        break;
                }
            }
            File.WriteAllText(Path.Combine(relativePath, "values.json"), JsonConvert.SerializeObject(json));
        }
    }
}
