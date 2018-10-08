using System;
using System.Collections.Generic;

namespace BetAI.Genetics
{
    /// <summary>
    /// Class is responsible for arranging all nodes selected for reproduction
    /// in pairs and perform crossover operation for each pair. 
    /// </summary>
    public class Crossover
    {
        /// <summary>
        /// Pairs individual nodes and uses an BLX-α algorithm as crossover
        /// algorithm to produce new offspring.
        /// </summary>
        /// <param name="toReproduce">Nodes selected for reproduction</param>
        /// <param name="alpha">Alpha parameter used in BLX-α, defining
        /// possible values for offspring.</param>
        /// <returns>Array of nodes created in crossover.</returns>
        /// <exception cref="ArgumentException">Thrown if alpha is less than 0 or toReproduce
        /// has less than 2 nodes.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public List<Node> Reproduce(List<Node> toReproduce, double alpha)
        {
            if (toReproduce == null)
                throw new ArgumentNullException();
            if (alpha < 0 || toReproduce.Count < 2)
                throw new ArgumentException();

            Pair[] pairs = CreatePairs(toReproduce);
            return CreateChildren(pairs, alpha);
        }

        /// <summary>
        /// Function creates as many pairs as can be produced from parameter nodes,
        /// and returns them.
        /// </summary>
        /// <param name="toReproduce"></param>
        private Pair[] CreatePairs(List<Node> toReproduce)
        {
            int numberOfPairs = (int) Math.Floor((double)toReproduce.Count / 2);
            Pair[] pairs = new Pair[numberOfPairs];
            Random rand = new Random();
            for (int i = 0; i < numberOfPairs; i++)
            {
                Node parent1 = toReproduce[rand.Next(0, toReproduce.Count)];
                toReproduce.Remove(parent1);
                Node parent2 = toReproduce[rand.Next(0, toReproduce.Count)];
                toReproduce.Remove(parent2);
                pairs[i] = new Pair(parent1, parent2);
            }

            return pairs;
        }

        private List<Node> CreateChildren(Pair[] pairs, double alpha)
        {
            List<Node> children = new List<Node>();

            for (int i = 0; i < pairs.Length; i++)
            {
                children.AddRange(pairs[i].BLX_alpha(alpha));
            }
            return children;
        }

        private struct Pair
        {
            Node parent1, parent2;

            public Pair(Node p1, Node p2)
            {
                parent1 = p1;
                parent2 = p2;
            }

            public List<Node> BLX_alpha(double alpha)
            {
                Random rand = new Random();
                double[] drawLimits = BlendDoubles(rand, alpha, parent1.DrawLimit, parent2.DrawLimit);
                double[] playLimits = BlendDoubles(rand, alpha, parent1.PlayLimit, parent2.PlayLimit);
                int[] simulationSampleSizes = BlendInts(rand, alpha, parent1.SimulationSampleSize, parent2.SimulationSampleSize);

                Node child1 = new Node(playLimits[0], drawLimits[0], parent1.MinimumStake, parent1.Generation + 1, simulationSampleSizes[0]);
                Node child2 = new Node(playLimits[1], drawLimits[1], parent2.MinimumStake, parent2.Generation + 1, simulationSampleSizes[1]);
                return new List<Node> { child1, child2 };
            }

            /// <summary>
            /// Function performs a crossover of values. 
            /// Newly created values should always be in between 
            /// (min - alpha * d ,max + alpha * d). Function
            /// returns two values, producing two different
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
                double[] newValues = new double[2];
                double d = Math.Abs(val1 - val2);
                double min = Math.Min(val1, val2);
                double max = Math.Max(val1, val2);
                newValues[0] = rand.NextDouble() *  (max + alpha * d - min - alpha * d) + min - alpha * d;
                newValues[1] = rand.NextDouble() * (max + alpha * d - min - alpha * d) + min - alpha * d;
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
                int[] newValues = new int[2];
                int d = Math.Abs(val1 - val2);
                int min = Math.Min(val1, val2);
                int max = Math.Max(val1, val2);
                newValues[0] = rand.Next((int) Math.Round(min - alpha * d), (int) Math.Round(max + alpha * d));
                newValues[1] = rand.Next((int)Math.Round(min - alpha * d), (int)Math.Round(max + alpha * d));
                return newValues;
            }
        }
    }
}
