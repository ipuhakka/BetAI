using System;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;

namespace BetAI.Utils
{
    public class Values
    {
        [JsonProperty]
        public double Alpha { get; private set; }
        [JsonProperty]
        public int TournamentSize { get; private set; }
        [JsonProperty]
        public double MinimumStake { get; private set; }
        [JsonProperty]
        public int NumberOfNodes { get; private set; }
        [JsonProperty]
        public int SampleSize { get; private set; }
        [JsonProperty]
        public string Database { get; private set; }
        [JsonProperty]
        public string CrossoverMethod { get; private set; }
        [JsonProperty]
        public string ParentSelectionMethod { get; private set; }


        [JsonConstructor]
        public Values()
        {

        }

        public void LogValues()
        {
            Console.WriteLine("Starting with arguments:");
            FileInfo file = new FileInfo(Database);
            Console.WriteLine("Database: " + file.FullName);
            Console.WriteLine("Number of nodes: " + NumberOfNodes);
            Console.WriteLine("Sample size: " + SampleSize);
            Console.WriteLine("Minimum stake: " + MinimumStake);
            Console.WriteLine("Parent selection method: " + ParentSelectionMethod);

            if (ParentSelectionMethod.Trim().ToLower().Equals("tournament"))
            {
                Console.WriteLine("Tournament size: " + TournamentSize);
            }
            Console.WriteLine("Crossover method: " + CrossoverMethod);

            if (CrossoverMethod.Trim().ToLower().Equals("weighted"))
            {
                Console.WriteLine("Alpha: " + Alpha);
            }
        }

        /// <summary>
        /// Parses dynamic json data into format used by values.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="args"></param>
        /// <returns>Parsed dynamic object containing values.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when argument name and value are not separated with '=' character.</exception>
        /// <exception cref="FormatException">Thrown if args contains invalid data</exception>
        public static dynamic ParseArguments(dynamic json, string[] args)
        {
            foreach (string arg in args)
            {
                string[] argument = arg.Split('=');
                switch (argument[0].Trim().ToLower())
                {
                    case "alpha":
                        json["alpha"] = Convert.ToDouble(argument[1], CultureInfo.InvariantCulture);
                        break;
                    case "tournamentsize":
                        json["tournamentSize"] = Convert.ToInt32(argument[1]);
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
                    case "parentselectionmethod":
                        json["parentSelectionMethod"] = argument[1];
                        break;
                    case "crossovermethod":
                        json["crossoverMethod"] = argument[1];
                        break;
                    default:
                        break;
                }
            }
            return json;
        }
    }
}
