using System;
using System.Collections.Generic;
using System.Linq;
using BetAI.Utils;

namespace BetAI.Genetics.Selection
{
    public class TournamentSelection: ISelection
    {
        public int TournamentSize { get; private set; }

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
            var gen = generation.ToArray();
            var tournament = CreateTournament(gen);
            var parent1 = MaxNode(tournament);

            gen = gen.ToList().Where(n => !n.Equals(parent1)).ToArray();
            tournament = CreateTournament(gen);
            var parent2 = MaxNode(tournament);

            return new Parents(parent1, parent2);
        }   

        /// <summary>
        /// Creates a TournamentSize-sized random list of nodes, from which the best fit node 
        /// is selected.
        /// </summary>
        /// <param name="generation"></param>
        /// <returns></returns>
        private List<Node> CreateTournament(Node[] generation)
        {
            var tournament = new Node[TournamentSize];
            Randomise.InitRandom();

            for (int i = 0; i < tournament.Length; i++)
            {
                tournament[i] = generation[Randomise.random.Next(0, generation.Length)];
            }

            return tournament.ToList();
        }

        private Node MaxNode(List<Node> tournament)
        {
            var max = tournament[0];

            for (int i = 1; i < tournament.Count; i++)
            {
                if (tournament[i].Fitness > max.Fitness)
                    max = tournament[i];
            }

            return max;
        }
    }
}
