using System.Collections.Generic;

namespace BetAI.Genetics.Crossover
{
    public interface ICrossover
    {
        /// <summary>
        /// Interface method for completing a crossover of two nodes. 
        /// Should always return 2 nodes.
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        /// <returns>2 recombinations of parent nodes.</returns>
        List<Node> Crossover(Node parent1, Node parent2);
    }
}
