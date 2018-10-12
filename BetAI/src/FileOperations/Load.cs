using System;
using System.IO;
using System.Collections.Generic;
using BetAI.Genetics;
using Newtonsoft.Json;

namespace BetAI.FileOperations
{
    public class Load
    {
        /// <summary>
        /// Function returns the latest generation of nodes 
        /// </summary>
        /// <param name="savefile">Name of the savefile.</param>
        public static List<Node> LoadGeneration(string savefile)
        {
            string directory = Path.Combine(@"Files\", savefile, "gen_data");
            int latestGeneration = LatestGeneration(savefile);
            string latestFile = Path.Combine(directory, String.Format("gen{0}.json", latestGeneration));
            string json = File.ReadAllText(latestFile);
            return JsonConvert.DeserializeObject(json) as List<Node>;
        }

        /// <summary>
        /// Returns the integer number of latest generation of nodes
        /// written to gen_data folder.
        /// Generation file names are in format 'gen{i}.json'.
        /// These are parsed so that only the number of generation
        /// is left.
        /// </summary>
        /// <param name="path">Directory from which generations are searched.</param>
        /// <returns>-1 if no generations are yet created or exist in the folder
        /// BetAI\Files\savefile\gen_data\, otherwise number
        /// representing the latest generation written.</returns>
        private static int LatestGeneration(string path)
        {
            string[] generations = Directory.GetFiles(path);

            int max = -1;
            for (int i = 0; i < generations.Length; i++)
            {
                string splitted = generations[i].Split('.')[0]; //Filter out extension
                splitted = splitted.Remove(0, 3); //remove "gen".
                try
                {
                    int generation = Convert.ToInt32(splitted);
                    if (generation > max)
                        max = generation;
                }
                catch (FormatException)
                {
                    continue;
                }
            }
            return max;
        }
    }
}
