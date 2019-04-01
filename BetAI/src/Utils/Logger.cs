using System;
using System.Collections.Generic;
using System.Linq;
using BetAI.Genetics;
using BetAI.FileOperations;

namespace BetAI.Utils
{
    public static class Logger
    {
        /// <summary>
        /// Logs generation data.
        /// </summary>
        public static void Log(List<Node> nodes, string savefile)
        {
            var maxFitnessNode = FindMaximumFitnessNode(nodes);

            var lines = new string[] {
                $"Generation: {nodes[0].Generation}",
                $"Minimum fitness: {FindMinimumFitnessNode(nodes).Fitness}",
                $"Average fitness: {FindAverageFitness(nodes)}",
                $"Maximum fitness: {maxFitnessNode.Fitness}",
                $"Fitness sum: {nodes.Sum(n => n.Fitness)}",
                $"Best node: {maxFitnessNode.BetsWon} won, " +
                    $"{maxFitnessNode.BetsLost} lost, " +
                    $"{maxFitnessNode.BetsNotPlayed} not played, " +
                    $"{maxFitnessNode.BetsSkipped} skipped"
            };

            Array.ForEach(lines, line => Console.WriteLine(line));
            Console.WriteLine();

            Save.Log(savefile, lines);
        }

        private static Node FindMinimumFitnessNode(List<Node> nodes)
        {
            var minNode = nodes[0];

            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].Fitness < minNode.Fitness)
                    minNode = nodes[i];
            }

            return minNode;
        }

        private static Node FindMaximumFitnessNode(List<Node> nodes)
        {
            var maxNode = nodes[0];

            for (int i = 1; i < nodes.Count; i++)
            {
                if (nodes[i].Fitness > maxNode.Fitness)
                    maxNode = nodes[i];
            }

            return maxNode;
        }

        private static double FindAverageFitness(List<Node> nodes)
        {
            var sum = nodes.Sum(n => n.Fitness);

            return sum / nodes.Count;
        }

    }
}
