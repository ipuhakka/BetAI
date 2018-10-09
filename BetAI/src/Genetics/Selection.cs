using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetAI.Genetics;

namespace BetAI.Genetics
{
    public class Selection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="generation">Set of nodes from which the ones
        /// to crossover are selected.</param>
        /// <returns></returns>
        public List<Node> Select(List<Node> generation)
        {
            int toSelect = (int)Math.Floor((double)generation.Count / 2);
            return generation;
        }

        private List<Node> SetWeights(List<Node> generation)
        {
            return generation;
        }
    }
}
