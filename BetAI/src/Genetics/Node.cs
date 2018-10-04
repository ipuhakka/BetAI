using System.Collections.Generic;
using Database;
using BetAI.BetSim;
using BetAI.Exceptions;

namespace BetAI.Genetics
{
    public class Node
    {
        public double PlayLimit { get; private set; }
        public double DrawLimit { get; private set; }
        public double MinimumStake { get; private set; }
        public double Fitness { get; private set; }
        public double CrossOverProbability { get; private set; }
        public int Generation { get; private set; }
        public int BetsWon { get; private set; }
        public int BetsLost { get; private set; }
        public int BetsNotPlayed { get; private set; }
        public int BetsSkipped { get; private set; }
        public int SimulationSampleSize { get; private set; }

        public Node(double playL, double drawL, double minStake, int gen, int sampleS)
        {
            PlayLimit = playL;
            DrawLimit = drawL;
            MinimumStake = minStake;
            Generation = gen;
            SimulationSampleSize = sampleS;
        }

        /// <summary>
        /// Evaluates the fitness (money won/lost) for the node.
        /// </summary>
        /// <param name="sample">List of matches simulated.</param>
        /// <param name="dbPath">Path to database file which is used.</param>
        /// <param name="evaluationSampleSize">evaluationSampleSize describes
        /// how many matches are used by Predict to make an evaluation of teams 
        /// strength. </param>
        /// <returns>Fitness value for the specific node.</returns>
        public double EvaluateFitness(List<Match> sample)
        {
            Predict predict = new Predict();
            Bet bet = new Bet();
            Fitness = 0;
            foreach (Match m in sample)
            {
                try
                {
                    double predictedResult = predict.PredictResult(m, SimulationSampleSize);
                    double betProfit = bet.PlayBet(m, predictedResult, PlayLimit, MinimumStake, DrawLimit);
                    if (betProfit == 0)
                        BetsNotPlayed++;
                    else if (betProfit > 0)
                        BetsWon++;
                    else
                        BetsLost++;

                    Fitness += betProfit;
                }
                catch (NotSimulatedException)
                {
                    BetsSkipped++;
                    continue;
                }
            }
            return Fitness;
        }

    }
}
