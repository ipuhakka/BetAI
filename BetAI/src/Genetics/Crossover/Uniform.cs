using System.Collections.Generic;
using BetAI.Utils;

namespace BetAI.Genetics.Crossover
{

    /// <summary>
    /// Uniform crossover select all new node values randomly from either of the parents, except
    /// generation, which should still incremented by 1.
    /// </summary>
    public class Uniform: ICrossover
    {
        public List<Node> Crossover(Node parent1, Node parent2)
        {
            var childNodes = new List<Node>();
            Randomise.InitRandom();

            for (int i = 0; i < 2; i++)
            {
                var randomDouble = Randomise.random.NextDouble();
                var playLimit = randomDouble > 0.5 ? parent1.PlayLimit : parent2.PlayLimit;

                randomDouble = Randomise.random.NextDouble();
                var drawLimit = randomDouble > 0.5 ? parent1.DrawLimit : parent2.DrawLimit;

                randomDouble = Randomise.random.NextDouble();
                var minStake = randomDouble > 0.5 ? parent1.MinimumStake : parent2.MinimumStake;

                var gen = parent1.Generation + 1;

                randomDouble = Randomise.random.NextDouble();
                var sampleSize = randomDouble > 0.5 ? parent1.SimulationSampleSize : parent2.SimulationSampleSize;
                childNodes.Add(new Node(playLimit, drawLimit, minStake, gen, sampleSize));
            }
            return childNodes;
        }
    }
}
