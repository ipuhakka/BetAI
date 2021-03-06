﻿using System;
using System.Collections.Generic;
using System.Linq;
using BetAI.Utils;

namespace BetAI.Genetics.Selection
{
    public class WeightedSelection: ISelection
    {
        /// <summary>
        /// Uses a probability based randomisation process
        /// to select 2 nodes for crossover. Weights for each node in generation are calculated
        /// based on fitness, and then 2 nodes are selected randomly by using this weighted
        /// probability model. Nodes cannot be the same, meaning that two different nodes
        /// are always returned.
        /// </summary>
        /// <param name="generation">Set of nodes from which nodes for crossover
        /// are selected.</param>
        public Parents SelectForCrossover(List<Node> generation)
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
        private Parents ProbabilityRandomise(List<Node> generation)
        {
            generation = generation.OrderBy(n => n.CrossoverValue).ToList();
            var toReproduce = new List<Node>();

            for (int i = 0; i < 2; i++)
            {
                var minimumCrossoverValue = generation.Min(n => n.CrossoverValue);
                var maximumCrossoverValue = generation.Max(n => n.CrossoverValue);
                var next = Randomise.random.NextDouble() * (maximumCrossoverValue - minimumCrossoverValue) + minimumCrossoverValue;

                var selected = generation.FirstOrDefault(n => n.CrossoverValue > next) ?? 
                    generation[0];

                toReproduce.Add(selected);
                generation.Remove(selected);
            }

            return new Parents(toReproduce[0], toReproduce[1]);
        }

        /// <summary>
        /// Sets the probability for each node to be selected for 
        /// crossover. This is done as follows:
        /// 
        /// K-nodes are ordered by fitness from lowest to highest.
        /// Lowest fitness node has CrossoverValue of 0.
        /// Node n CrossoverValue is (n.Fitness - [n-1].Fitness) + runningSum.
        /// This way, CrossoverValue gives node a unique CrossoverValue.
        /// CrossoverValues range from
        /// 0 to
        /// (n[k].Fitness - n[k-1].Fitness) + runningCrossoverSum, where k-nodes are ordered
        /// from lowest fitness to highest.
        /// <param name="generation"></param>
        /// <returns></returns>
        private List<Node> SetWeights(Node[] generation)
        {
            var crossoverValuesCalculated = generation.ToList().Any(node => node.CrossoverValue != 0);

            if (crossoverValuesCalculated) //CrossoverValues already calculated, no excess calculation should be done
            {
                return generation.ToList();
            }

            var generationSortedByFitness = generation.ToList().OrderBy(n => n.Fitness).ToList();
            var runningSum = 0.0;
            for (int i = 0; i < generationSortedByFitness.Count; i++)
            {
                if (i == 0)
                    generationSortedByFitness[i].CrossoverValue = 0;
                else
                    generationSortedByFitness[i].CrossoverValue = runningSum + 
                        (generationSortedByFitness[i].Fitness - generationSortedByFitness[i - 1].Fitness);
                
                
                runningSum += generationSortedByFitness[i].CrossoverValue;
            }

            return generationSortedByFitness;
        }

        /// <summary>
        /// Returns the minimum fitness value in a group of nodes.
        /// </summary>
        /// <param name="generation"></param>
        private double MinimumFitness(Node[] generation)
        {
            var minimumFitness = generation[0].Fitness;

            for (var i = 1; i < generation.Length; i++)
            {
                if (generation[i].Fitness < minimumFitness)
                    minimumFitness = generation[i].Fitness;
            }

            return minimumFitness;
        }
    }
}
