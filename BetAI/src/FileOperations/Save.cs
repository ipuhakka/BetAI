using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using BetAI.Exceptions;
using BetAI.Genetics;
using Newtonsoft.Json;
using Database;

namespace BetAI.FileOperations
{
    public class Save
    {
        /// <summary>
        /// Function creates a new folder named filename to Files-folder and creates a file
        /// values.json to that folder.
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
            Directory.CreateDirectory(Path.Combine(relativePath, "gen_data"));
            Directory.CreateDirectory(Path.Combine(relativePath, "sample_data"));

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
                        json["alpha"] = Convert.ToDouble(argument[1], CultureInfo.InvariantCulture);
                        break;
                    case "minimumstake":
                        json["minimumStake"] = Convert.ToDouble(argument[1], CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Function writes a list of nodes into a file named gen{X}.json,
        /// where X is generation number.
        /// </summary>
        /// <param name="filename">Save to which data is appended.</param>
        /// <param name="nodes">List of nodes to write into a file.</param>
        /// <param name="generation">Number of the nodes generation.</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void WriteGeneration(string filename, List<Node> nodes, int generation)
        {
            string directory = Path.Combine(@"Files\", filename, "gen_data");
            string json = JsonConvert.SerializeObject(nodes);
            File.WriteAllText(Path.Combine(directory, String.Format("gen{0}.json", generation)), json);
        }

        /// <summary>
        /// Function writes matches sample to a file in Files\{filename}\sample_data\sample{i},
        /// where i is generation number.
        /// </summary>
        /// <param name="filename">Name of the save file.</param>
        /// <param name="sample">List of matches which are simulated in generation</param>
        /// <param name="generation">Number of the generation to which sample list belongs to.</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void WriteSample(string filename, List<Match> sample, int generation)
        {
            string directory = Path.Combine(@"Files\", filename, "sample_data");
            string json = JsonConvert.SerializeObject(sample);
            File.WriteAllText(Path.Combine(directory, String.Format("sample{0}.json", generation)), json);
        }

        /// <summary>
        /// Function appends lines to BetAI\Files\{savefile}\log.txt.
        /// </summary>
        /// <param name="savefile"></param>
        /// <param name="lines"></param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void Log(string savefile, string[] lines)
        {
            string path = Path.Combine(@"Files\", savefile, "log.txt");
            File.AppendAllLines(path, lines);
        }
    }
}
