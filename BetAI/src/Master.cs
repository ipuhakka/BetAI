using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using BetAI.Genetics;
using BetAI.FileOperations;

namespace BetAI
{
    /// <summary>
    /// Master is responsible for running the evolution.
    /// </summary>
    public class Master
    {
        private List<Node> nodes;
        private List<Match> sample;

        /// <summary>
        /// Constructor for master. Loads latest generation of nodes into memory.
        /// If save does not exist, it is created and first generation of nodes as well.
        /// </summary>
        public Master(string filename, params string[] args)
        {
            if (Load.SaveExists(filename))
            {
                nodes = Load.LoadLatestGeneration(filename);
                // Load values.json to find numberOfNodes
                if (nodes == null)
                {
                    Random rand = new Random();

                }
            }
            else
            {
                Save.InitializeSave(filename, args);
                ///Load values from values.json to find numberOfNodes.
            }
        }

    }
}
