using System;
using System.Collections.Generic;
using BetAI.Utils;

namespace BetAI.Genetics.Crossover
{
    /// <summary>
    /// UniformAlpha is a crossover method which combines uniform crossover to using an
    /// alpha value which describes the boundaries of the created value. 
    /// For real numbers, it selects a value x for a gene randomly from either parent, 
    /// and then selects a random value from range 
    /// [x - α, x + α].
    /// 
    /// For integers, it selects a value x randomly from either parent,
    /// and then randomises a new value for child in range
    /// [x - (x * α), x + (x * α)].
    /// </summary>
    public class UniformAlpha: ICrossover
    {
        public double Alpha { get; }

        public UniformAlpha(double alpha)
        {
            if (alpha < 0)
                throw new ArgumentException("Alpha cannot be less than 0");
            Alpha = alpha;
        }

        /// <summary>
        /// Returns two child nodes, with values
        /// created using an uniform selection and alpha parameter
        /// to make adjustments to these values.
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns>Two child nodes created.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<Node> Crossover(Node parent1, Node parent2)
        {
            if (parent1 == null || parent2 == null)
                throw new ArgumentNullException("Parent nodes cannot be null");
            Randomise.InitRandom();

            double[] drawLimits = CreateUniformAlphaValues(parent1.DrawLimit, parent2.DrawLimit);
            double[] playLimits = CreateUniformAlphaValues(parent1.PlayLimit, parent2.PlayLimit);
            int[] sampleSize = CreateUniformAlphaValues(parent1.SimulationSampleSize, parent2.SimulationSampleSize);

            return new List<Node>
            {
                new Node(playLimits[0], drawLimits[0], parent1.MinimumStake, parent1.Generation + 1, sampleSize[0]),
                new Node(playLimits[1], drawLimits[1], parent1.MinimumStake, parent1.Generation + 1, sampleSize[1])
            };
        }

        /// <summary>
        /// returns two new double values, which are in range 
        /// [x - α, x + α], where α is Alpha and x is either val1 or val2, randomly
        /// selected for both returned values.
        /// </summary>        
        /// <param name="values">Parameter values from both parents.</param>
        private double[] CreateUniformAlphaValues(params double[] values)
        {
            Random rand = Randomise.random;
            double[] childValues = new double[2];

            for (int i = 0; i < 2; i++)
            {
                double nextVal = values[rand.Next(0, 1)];
                double min = nextVal - Alpha;
                double max = nextVal + Alpha;
                childValues[i] = rand.NextDouble() * (max - min) + min;
            }
            return childValues;
        }

        /// <summary>
        /// Returns two new int-values, both in range
        /// [x - (x * α), x + (x * α)], where α is Alpha, and
        /// x is randomly selected parent value, selected separately for 
        /// both new values.
        /// </summary>
        /// <param name="values">Parameter values from both parents.</param>
        private int[] CreateUniformAlphaValues(params int[] values)
        {
            Random rand = Randomise.random;
            int[] childValues = new int[2];

            for (int i = 0; i < 2; i++)
            {
                int nextVal = values[rand.Next(0, 1)];
                int min = (int)Math.Round(nextVal - (nextVal * Alpha), 0);
                int max = (int)Math.Round(nextVal + (nextVal * Alpha), 0);
                childValues[i] = rand.Next(min, max);
            }
            return childValues;
        }
    }
}
