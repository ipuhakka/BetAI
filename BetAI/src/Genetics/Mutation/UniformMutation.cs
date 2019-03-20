using System;
using System.Collections.Generic;

namespace BetAI.Genetics.Mutation
{
    public class UniformMutation: IMutation
    {
        /// <summary>
        /// Goes through a list of nodes, and mutates nodes randomly based 
        /// on probability: each node has a {probability} chance of mutation.
        /// UniformMutation uses Node-objects random constructor, since this essentially does
        /// uniform mutation: it assigns values within set limits to all genes.
        /// </summary>
        /// <param name="generation">List of nodes in a generation.</param>
        /// <param name="probability">Probability of a single mutation. Between 0 and 1.</param>
        /// <returns>Generation after mutation is complete.</returns>
        /// <exception cref="ArgumentException">Thrown if probability is not in range 0-1.</exception>
        public List<Node> Mutate(List<Node> generation, double probability)
        {
            if (probability < 0 || probability > 1)
                throw new ArgumentException("probability is not in valid range: 0-1");

            var random = new Random();
            var mutated = new List<Node>();

            foreach(Node n in generation)
            {
                if (random.NextDouble() < probability)
                    mutated.Add(new Node(random, n.MinimumStake, n.Generation));
                else
                    mutated.Add(n);     
            }

            return mutated;
        }
    }
}
