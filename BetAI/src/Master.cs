using System;
using System.Collections.Generic;
using Database;
using BetAI.Genetics;
using BetAI.FileOperations;
using BetAI.Utils;

namespace BetAI
{
    /// <summary>
    /// Master is responsible for running the evolution.
    /// </summary>
    public class Master
    {
        private List<Node> nodes;
        private List<Match> sample;
        private Values values;

        /// <summary>
        /// Constructor for master. Loads latest generation of nodes into memory.
        /// If save does not exist, it is created and first generation of nodes as well.
        /// </summary>
        public Master(string filename, params string[] args)
        {
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

    }
}
