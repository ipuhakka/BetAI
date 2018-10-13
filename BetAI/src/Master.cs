using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using BetAI.Genetics;
using BetAI.FileOperations;
using BetAI.Utils;
using BetAI.Data;
using BetAI.Exceptions;

namespace BetAI
{
    /// <summary>
    /// Master is responsible for running the evolution.
    /// </summary>
    public class Master
    {
        private List<Node> nodes;
        private List<Match> sample;
        private string savefile;
        private Values values;

        /// <summary>
        /// Constructor for master. Loads latest generation of nodes into memory.
        /// If save does not exist, it is created and first generation of nodes as well.
        /// </summary>
        public Master(string filename, params string[] args)
        {
            savefile = filename;

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
            Matches.SetMatches(values.Database);
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
            Crossover co = new Crossover();
            Selection sel = new Selection();
            if (nodes == null || savefile == null || values == null)
            {
                Console.WriteLine("Initialization failed");
                throw new InitializationException();
            }

            while (true)
            {
                List<Match> sample = Sample.CreateSample(values.SampleSize);
                EvaluateNodes(sample);
                Save.WriteGeneration(savefile, nodes, nodes[0].Generation);
                Log();
                List<Node> toReproduce = sel.SelectForCrossover(nodes);
                nodes = co.Reproduce(toReproduce, values.Alpha);
                Save.WriteGeneration(savefile, nodes, nodes[0].Generation);
            }
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

            Save.Log(savefile, lines);
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
            double sum = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                sum += nodes[i].Fitness;
            }
            return sum / nodes.Count;
        }

    }
}
