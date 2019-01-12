using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            List<Node> children = new List<Node>();
            Randomise.InitRandom();
            for (int i = 0; i < 2; i++)
            {
                double rand = Randomise.random.NextDouble();
                double playLimit = rand > 0.5 ? parent1.PlayLimit : parent2.PlayLimit;
                rand = Randomise.random.NextDouble();
                double drawLimit = rand > 0.5 ? parent1.DrawLimit : parent2.DrawLimit;
                rand = Randomise.random.NextDouble();
                double minStake = rand > 0.5 ? parent1.MinimumStake : parent2.MinimumStake;
                int gen = parent1.Generation + 1;
                rand = Randomise.random.NextDouble();
                int sampleSize = rand > 0.5 ? parent1.SimulationSampleSize : parent2.SimulationSampleSize;
                children.Add(new Node(playLimit, drawLimit, minStake, gen, sampleSize));
            }
            return children;
        }
    }
}
