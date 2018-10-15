using System;
using System.Collections.Generic;
using System.Linq;

namespace BetAI.Genetics
{
    public class Selection
    {
        /// <summary>
        /// Uses a probability based randomisation process
        /// to select Math.Floor(n / 2) nodes for crossover.
        /// </summary>
        /// <param name="generation">Set of nodes from which the ones
        /// to crossover are selected.</param>
        public List<Node> SelectForCrossover(List<Node> generation)
        {
            generation = SetWeights(generation.ToArray());
            return ProbabilityRandomise(generation);
        }

        /// <summary>
        /// Returns Math.Floor(generation / 2) nodes. For every selected node,
        /// function randomises a number
        /// between 0 and sum of CrossoverFactors of still not chosen nodes. 
        /// First variable which has a CrossoverValue value lower than the random value is 
        /// selected. If no variable have a lower CrossoverValue value, first one from
        /// sorted list (has the highest Crossoverfactor) will be selected.
        /// This is then removed from original list and added to the list of nodes
        /// that go to crossover process.
        /// </summary>
        private List<Node> ProbabilityRandomise(List<Node> generation)
        {
            generation = generation.OrderByDescending(n => n.CrossoverValue).ToList();
            double probabilitySum = generation.Sum(n => n.CrossoverValue);
            List<Node> toReproduce = new List<Node>();
            int nodesToCrossover = (int) Math.Floor((double) generation.Count / 2);
            Random rand = new Random();

            for (int i = 0; i < nodesToCrossover; i++)
            {
                double minimumCrossoverFactor = generation.Min(n => n.CrossoverValue);
                double next = rand.NextDouble() * (probabilitySum - minimumCrossoverFactor);
                Node selected = generation.FirstOrDefault(n => n.CrossoverValue < next);
                if (selected == null)
                    selected = generation[0];
                toReproduce.Add(selected);
                generation.Remove(selected);
                probabilitySum -= selected.CrossoverValue;
            }

            return toReproduce;
        }

        /// <summary>
        /// Sets the probability for each node to be selected for 
        /// crossover. This is set as follows:
        /// 
        /// Node[i].CrossoverValue = Node[i].Fitness - Min(Fitness).
        /// This does mean that node with minimum fitness has
        /// crossover probability of 0.
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        private List<Node> SetWeights(Node[] generation)
        {
            double minimumFitness = MinimumFitness(generation);
            for (var i = 0; i < generation.Length; i++)
            {
                generation[i].CrossoverValue = generation[i].Fitness - minimumFitness;
            }

            return generation.ToList();
        }

        /// <summary>
        /// Returns the minimum fitness value in a group of nodes.
        /// </summary>
        /// <param name="generation"></param>
        private double MinimumFitness(Node[] generation)
        {
            double minimumFitness = generation[0].Fitness;

            for (var i = 1; i < generation.Length; i++)
            {
                if (generation[i].Fitness < minimumFitness)
                    minimumFitness = generation[i].Fitness;
            }

            return minimumFitness;
        }
    }
}
