using System;
using System.Collections.Generic;
using System.Linq;
using Database;

namespace BetAI.Genetics
{
    /// <summary>
    /// Master is responsible for running the evolution.
    /// </summary>
    public class Master
    {
        private List<Node> nodes;
        private List<Match> sample;

        /// <summary>
        /// Constructor for master. If generation is 0, nodes should be initialized
        /// randomly, otherwise they are loaded from memory.
        /// </summary>
        public Master()
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
