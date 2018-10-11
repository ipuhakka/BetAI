using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using BetAI.Genetics;

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
        /// Constructor for master. If BetAI\Files does not containg a folder named
        /// filename, a new folder is created.
        /// </summary>
        public Master(string filename, params string[] args)
        {
            nodes = new List<Node>();
            sample = new List<Match>();
        }

        /// <summary>
        /// Function creates a sample list of Match-objects to be used in simulation.
        /// </summary>
        private void SetUpGeneration()
        {

        }
    }
}
