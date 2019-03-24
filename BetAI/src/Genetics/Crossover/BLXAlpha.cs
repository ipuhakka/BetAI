using System;
using System.Collections.Generic;

namespace BetAI.Genetics.Crossover
{
    /// <summary>
    /// Class is responsible for arranging all nodes selected for reproduction
    /// in pairs and perform crossover operation for each pair. 
    /// </summary>
    public class BLXAlpha: ICrossover
    {
        public double Alpha { get; private set; }

        /// <summary>
        /// Constructor for BLX-α crossover object.
        /// </summary>
        /// <param name="alpha_par">Alpha parameter which defines how far from boundary values
        /// new values can be created.</param>
        /// <exception cref="ArgumentException">Thrown if alpha parameter is less than 0.</exception>
        public BLXAlpha(double alpha_par)
        {
            if (alpha_par < 0)
                throw new ArgumentException("Alpha parameter cannot be less than 0.");

            Alpha = alpha_par;
        }

        /// <summary>
        /// Pairs individual nodes and uses an BLX-α algorithm as crossover
        /// algorithm to produce new offspring.
        /// </summary>
        /// <param name="toReproduce">Nodes selected for reproduction</param>
        /// <param name="alpha">Alpha parameter used in BLX-α, defining
        /// possible values for offspring.</param>
        /// <returns>Array of nodes created in crossover.</returns>
        /// <exception cref="ArgumentNullException">Thrown if either parent node is null.</exception>
        public List<Node> Crossover(Node parent1, Node parent2)
        {
            if (parent1 == null || parent2 == null)
                throw new ArgumentNullException("Parent nodes cannot be null");

            return BLX_alpha(parent1, parent2);
        }

        public List<Node> BLX_alpha(Node parent1, Node parent2)
        {
            var random = new Random();
            var drawLimits = BlendDoubles(random, Alpha, parent1.DrawLimit, parent2.DrawLimit);
            var playLimits = BlendDoubles(random, Alpha, parent1.PlayLimit, parent2.PlayLimit);
            var simulationSampleSizes = BlendInts(random, Alpha, parent1.SimulationSampleSize, parent2.SimulationSampleSize);

            var child1 = new Node(playLimits[0], drawLimits[0], parent1.MinimumStake, parent1.Generation + 1, simulationSampleSizes[0]);
            var child2 = new Node(playLimits[1], drawLimits[1], parent2.MinimumStake, parent2.Generation + 1, simulationSampleSizes[1]);
            return new List<Node> { child1, child2 };
        }

        /// <summary>
        /// Function performs a crossover of values. 
        /// Newly created values should always be in between 
        /// (min - alpha * d ,max + alpha * d). Function
        /// returns 2 values, producing 2 different
        /// values based on parent genes.
        /// </summary>
        /// <param name="rand">Random-object which is used to produce
        /// random values.</param>
        /// <param name="alpha">value broadens/narrows the gap
        /// in which new values can be created.</param>
        /// <param name="val1">Parent 1's value for a parameter.</param>
        /// <param name="val2">Parent 2's value for a parameter.</param>
        /// <returns></returns>
        private double[] BlendDoubles(Random rand, double alpha, double val1, double val2)
        {
            var newValues = new double[2];
            var d = Math.Abs(val1 - val2);
            var min = Math.Min(val1, val2);
            var max = Math.Max(val1, val2);

            newValues[0] = rand.NextDouble() * (max + alpha * d - (min - alpha * d)) + min - alpha * d;
            newValues[1] = rand.NextDouble() * (max + alpha * d - (min - alpha * d)) + min - alpha * d;

            return newValues;
        }

        /// <summary>
        /// Creates two new integer values. 
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="alpha"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        private int[] BlendInts(Random rand, double alpha, int val1, int val2)
        {
            var newValues = new int[2];
            var d = Math.Abs(val1 - val2);
            var min = Math.Min(val1, val2);
            var max = Math.Max(val1, val2);

            newValues[0] = rand.Next((int)Math.Round(min - alpha * d), (int)Math.Round(max + alpha * d));
            newValues[1] = rand.Next((int)Math.Round(min - alpha * d), (int)Math.Round(max + alpha * d));

            return newValues;
        }
    }
}
