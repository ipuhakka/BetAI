﻿using System;
using System.Collections.Generic;
using BetAI.Exceptions;
using BetAI.Genetics.Crossover;
using BetAI.Genetics.Selection;

namespace BetAI.Genetics
{
    public class Reproduce
    {
        private ICrossover crossover;
        private ISelection parentSelection;

        public Reproduce(ICrossover crossoverMethod, ISelection parentSelectionMethod)
        {
            crossover = crossoverMethod;
            parentSelection = parentSelectionMethod;
        }

        /// <summary>
        /// Creates a new generation of nodes, equal in count to 
        /// Math.Floor(generation.Count / 2) * 2.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SelectionException">Thrown if parentSelection returns an incorrect
        /// amount of parent nodes.</exception>
        public List<Node> CreateNewGeneration(List<Node> generation)
        {
            int numberOfCrossovers = (int)Math.Floor((double) generation.Count / 2);
            List<Node> newGeneration = new List<Node>();

            for (int i = 0; i < numberOfCrossovers; i++)
            {
                List<Node> parents = parentSelection.SelectForCrossover(generation);

                if (parents.Count != 2)
                    throw new SelectionException("Parent selection returned incorrect amount of parents");

                 newGeneration.AddRange(crossover.Crossover(parents[0], parents[1]));
            }

            return newGeneration;
        }
    }
}