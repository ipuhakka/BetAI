using BetAI.BetSim;
using BetAI.Exceptions;
using BetAI.FileOperations;
using BetAI.Genetics;
using BetAI.Genetics.Crossover;
using BetAI.Genetics.Selection;
using BetAI.Utils;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BetAI
{
    /// <summary>
    /// Master is responsible for running the evolution.
    /// </summary>
    public class Master
    {
        private List<Node> nodes;
        private string Savefile { get; }
        private Values values;
        private CancellationToken cancelToken;
        private ISelection Selection { get; }
        private ICrossover Crossover { get; }

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
            Reproduce reproduce = new Reproduce(Crossover, Selection);
            if (nodes == null || Savefile == null || values == null)
            {
                Console.WriteLine("Initialization failed");
                throw new InitializationException();
            }

            values.LogValues();
            
            while (!cancelToken.IsCancellationRequested)
            {
                List<Match> sample = Sample.CreateSample(values.SampleSize);
                Save.WriteSample(Savefile, sample, nodes[0].Generation);
                int maxSampleSize = nodes.Max(n => n.SimulationSampleSize);
                Matches.CreateMatchDataStructs(sample, maxSampleSize);
                EvaluateNodes(sample);
                Log();
                List<Node> newGeneration = reproduce.CreateNewGeneration(nodes);
                Save.WriteGeneration(Savefile, nodes, nodes[0].Generation);
                Save.WriteGeneration(Savefile, newGeneration, newGeneration[0].Generation);
                nodes = newGeneration;
            }
            Console.WriteLine("Stopping simulation");
        } 

        private void EvaluateNodes(List<Match> sample)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].EvaluateFitness(sample);
            }
        }

        private void Log()
        {
            double worstFitness = FindMinimumFitness();
            double bestFitness = FindMaximumFitness();
            double avgFitness = FindAverageFitness();
            string[] lines = new string[5];
            lines[0] = String.Format("Generation: {0}", nodes[0].Generation);
            lines[1] = String.Format("Minimum fitness: {0}", worstFitness);
            lines[2] = String.Format("Average fitness: {0}", avgFitness);
            lines[3] = String.Format("Maximum fitness: {0}", bestFitness);
            lines[4] = "";

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            Save.Log(Savefile, lines);
        }

        /// <summary>
        /// Uses the random-constructor of Node to create values.NumberOfNodes-amount
        /// of nodes.
        /// </summary>
        private List<Node> RandomiseNodes()
        {
            Random rand = new Random();
            List<Node> generation = new List<Node>();
            for (int i = 0; i < values.NumberOfNodes; i++)
            {
                generation.Add(new Node(rand, values.MinimumStake));
            }
            return generation;
        }

        private double FindMinimumFitness()
        {
            double min = nodes[0].Fitness;
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].Fitness < min)
                    min = nodes[i].Fitness;
            }
            return min;
        }

        private double FindMaximumFitness()
        {
            double max = nodes[0].Fitness;
            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].Fitness > max)
                    max = nodes[i].Fitness;
            }
            return max;
        }

        private double FindAverageFitness()
        {
            double sum = nodes.Sum(n => n.Fitness);
            Console.WriteLine("Fitness sum: " + sum);

            return sum / nodes.Count;
        }
    }
}
