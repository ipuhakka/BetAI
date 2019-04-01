using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BetAI.BetSim;
using BetAI.Exceptions;
using BetAI.FileOperations;
using BetAI.Genetics;
using BetAI.Genetics.Crossover;
using BetAI.Genetics.Selection;
using BetAI.Genetics.Mutation;
using BetAI.Utils;

namespace BetAI
{
    /// <summary>
    /// Master is responsible for running the simulation.
    /// </summary>
    public class Master
    {
        private List<Node> nodes;
        private string Savefile { get; }
        private Values values;
        private CancellationToken cancelToken;
        private ISelection Selection { get; }
        private ICrossover Crossover { get; }
        private IMutation Mutation { get; }

        /// <summary>
        /// Constructor for master. Loads latest generation of nodes into memory.
        /// If save does not exist, it is created and first generation of nodes as well.
        /// </summary>
        /// <param name="args">Optional arguments that replace values from defaults.json
        /// in values.json and are used in simulation.</param>
        /// <param name="filename">Name of the savefile.</param>
        /// <param name="cancel">CancellationToken used by the main thread to stop simulation.</param>
        public Master(string filename, CancellationToken cancel, params string[] args)
        {
            Savefile = filename;
            cancelToken = cancel;

            if (Load.SaveExists(filename))
            {
                nodes = Load.LoadLatestGeneration(filename);
                values = Load.LoadValues(filename);
                if (nodes == null)
                {
                    nodes = RandomiseNodes();
                }
            }
            else
            {
                Save.InitializeSave(filename, args);
                values = Load.LoadValues(filename);
                nodes = RandomiseNodes();
            }

            Selection = Values.InitializeSelectionMethod(values, nodes.Count);
            Crossover = Values.InitializeCrossoverMethod(values);
            Mutation = Values.InitializeMutationMethod(values);
            Randomise.InitRandom();
            Matches.SetMatches(values.Database);
            Console.WriteLine("Found " + Matches.GetMatchCount() + " matches in database");
        }

        /// <summary>
        /// Function runs the simulation forwards. 
        /// It loops creating a sample of matches, evaluating nodes fitness, 
        /// selecting nodes for crossover, and reproducing a new generation of nodes.
        /// Generational data is written to file after fitness evaluation, and after 
        /// creating the generation.
        /// </summary>
        /// <exception cref="InitializationException"></exception>
        public void Run()
        {
            var reproduce = new Reproduce(Crossover, Selection);

            if (nodes == null || Savefile == null || values == null)
            {
                Console.WriteLine("Initialization failed");
                throw new InitializationException();
            }

            values.LogValues();
            
            while (!cancelToken.IsCancellationRequested)
            {
                var sample = Sample.CreateSample(values.SampleSize);
                Save.WriteSample(Savefile, sample, nodes[0].Generation);

                var maxSampleSize = nodes.Max(n => n.SimulationSampleSize);
                Matches.CreateMatchDataStructs(sample, maxSampleSize);

                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].EvaluateFitness(sample);
                }

                Logger.Log(nodes, Savefile);
                var newGeneration = reproduce.CreateNewGeneration(nodes);
                newGeneration = Mutation.Mutate(newGeneration, values.MutationProbability);

                Save.WriteGeneration(Savefile, nodes, nodes[0].Generation);
                Save.WriteGeneration(Savefile, newGeneration, newGeneration[0].Generation);
                nodes = newGeneration;
            }
            Console.WriteLine("Stopping simulation");
        } 

        /// <summary>
        /// Uses the random-constructor of Node to create values.NumberOfNodes-amount
        /// of nodes.
        /// </summary>
        private List<Node> RandomiseNodes()
        {
            var rand = new Random();
            var generation = new List<Node>();

            for (int i = 0; i < values.NumberOfNodes; i++)
            {
                generation.Add(new Node(rand, values.MinimumStake));
            }

            return generation;
        }
    }
}
