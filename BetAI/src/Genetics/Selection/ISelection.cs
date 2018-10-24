using System.Collections.Generic;

namespace BetAI.Genetics.Selection
{
    public interface ISelection
    {
        /// <summary>
        /// Selects nodes for crossover. Always returns a list of two nodes.
        /// </summary>
        /// <param name="generation"></param>
        /// <returns>Two nodes, selected for crossover.</returns>
        Parents SelectForCrossover(List<Node> generation);
    }
}
