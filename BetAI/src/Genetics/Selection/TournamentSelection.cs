using System;
using System.Collections.Generic;
using System.Linq;
using BetAI.Utils;

namespace BetAI.Genetics.Selection
{
    public class TournamentSelection: ISelection
    {
        public int TournamentSize { get; private set; }
        private int Round { get; set; }

        /// <summary>
        /// Constructor for tournament selection. If tournamentSize
        /// is set to more than generation size, a generation sized tournament is used.
        /// </summary>
        /// <param name="tournamentSize"></param>
        public TournamentSelection(int tournamentSize, int generationSize)
        {
            if (tournamentSize < 1)
                throw new ArgumentException("Tournament size cannot be less than 1");

            if (tournamentSize > generationSize)
                TournamentSize = generationSize;
            else
                TournamentSize = tournamentSize;

            Round = 0;
        }

        /// <summary>
        /// Function creates 2 random n-sized tournament of nodes, 
        /// and returns best fit node from both tournaments. 
        /// Returned nodes are unique.
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        public Parents SelectForCrossover(List<Node> generation)
        {
            var cpyGeneration = generation.Select(node => node.Clone()).ToList();

            List<Node> tournament = CreateTournament(cpyGeneration);
            Node parent1 = MaxNode(tournament);

            cpyGeneration = generation.Select(node => node.Clone()).ToList();
            cpyGeneration.Remove(parent1);

            tournament = CreateTournament(cpyGeneration);
            Node parent2 = MaxNode(tournament);

            Round = 0;
            return new Parents(parent1, parent2);
        }   

        /// <summary>
        /// Creates a TournamentSize-sized random list of nodes, from which the best fit node 
        /// is selected.
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        private List<Node> CreateTournament(List<Node> nodes)
        {
            Node[] tournament = new Node[TournamentSize - Round];
            Round++;
            Randomise.InitRandom();

            for (int i = 0; i < tournament.Length; i++)
            {
                var randomDouble = Randomise.random.Next(0, nodes.Count - 1);
                tournament[i] = nodes[randomDouble];
                nodes.RemoveAt(randomDouble);
            }

            return tournament.ToList();
        }

        private Node MaxNode(List<Node> tournament)
        {
            Node max = tournament[0];

            for (int i = 1; i < tournament.Count; i++)
            {
                if (tournament[i].Fitness > max.Fitness)
                    max = tournament[i];
            }
            return max;
        }
    }
}
