using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetAI.Genetics
{
    public class Node
    {
        private double riskLimit, drawLimit, minimumStake, fitness, crossoverProbability;
        private int sampleSize, generation;
        public int BetsWon { get; set; }
        public int BetsLost { get; set; }
        public int BetsNotPlayed { get; set; }
        public int BetsSkipped { get; set; }

    }
}
