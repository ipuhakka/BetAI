using System;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using BetAI.Exceptions;
using BetAI.Genetics.Crossover;
using BetAI.Genetics.Selection;
using BetAI.Genetics.Mutation;

namespace BetAI.Genetics
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
        [JsonProperty]
        public string MutationMethod { get; private set; }
        [JsonProperty]
        public double MutationProbability { get; private set; }


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
            Console.WriteLine("Mutation method: " + MutationMethod);
            Console.WriteLine("Mutation probability: " + MutationProbability);

            if (ParentSelectionMethod.Trim().ToLower().Equals("tournament"))
            {
                Console.WriteLine("Tournament size: " + TournamentSize);
            }
            Console.WriteLine("Crossover method: " + CrossoverMethod);

            if (CrossoverMethod.Trim().ToLower().Equals("uni-alpha") || CrossoverMethod.Trim().ToLower().Equals("blx"))
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
                    case "mutationmethod":
                        json["mutationMethod"] = argument[1];
                        break;
                    case "mutationprobability":
                        json["mutationProbability"] = Convert.ToDouble(argument[1], CultureInfo.InvariantCulture);
                        break;
                    default:
                        break;
                }
            }
            return json;
        }

        /// <summary>
        ///  Creates a new instance of ISelection interface, based on the parameter 
        /// parentSelectionMethod in values.json.
        /// </summary>
        /// <exception cref="InitializationException">Parent selection method is not identified.</exception>
        public static ISelection InitializeSelectionMethod(Values values, int nodeCount)
        {
            string method = values.ParentSelectionMethod;

            switch (method.Trim().ToLower())
            {
                case "weighted":
                    return new WeightedSelection();
                case "tournament":
                    return new TournamentSelection(values.TournamentSize, nodeCount);
                default:
                    throw new InitializationException("Parent selection method not identified");
            }
        }

        /// <summary>
        /// Creates a new instance of ICrossover interface, based on the parameter 
        /// crossoverMethod in values.json.
        /// </summary>
        /// <exception cref="InitializationException">Crossover method is not identified.</exception>
        public static ICrossover InitializeCrossoverMethod(Values values)
        {
            string method = values.CrossoverMethod;

            switch (method.Trim().ToLower())
            {
                case "blx":
                    return new BLXAlpha(values.Alpha);
                case "uni-alpha":
                    return new UniformAlpha(values.Alpha);
                case "uniform":
                    return new Uniform();
                default:
                    throw new InitializationException("Crossover method not identified");
            }
        }

        /// <summary>
        /// Creates a new instance of IMutation interface based on the parameter 
        /// mutationMethod in values.json.
        /// </summary>
        /// <param name="values"></param>
        public static IMutation InitializeMutationMethod(Values values)
        {
            string method = values.MutationMethod;

            switch (method.Trim().ToLower())
            {
                case "uniform":
                    return new UniformMutation();
                default:
                    throw new InitializationException("Mutation method not identified");
            }
        }
    }
}
