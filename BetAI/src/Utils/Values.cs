using System;
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
    }
}
