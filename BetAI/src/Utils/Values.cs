using System;
using System.IO;
using Newtonsoft.Json;

namespace BetAI.Utils
{
    public class Values
    {
        [JsonProperty]
        public double Alpha { get; private set; }
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
            Console.WriteLine("Alpha: " + Alpha);
            Console.WriteLine("Sample size: " + SampleSize);
            Console.WriteLine("Minimum stake: " + MinimumStake);
            Console.WriteLine("Crossover method: " + CrossoverMethod);
            Console.WriteLine("Parent selection method: " + ParentSelectionMethod);
        }
    }
}
