using System;
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
        public double CrossoverFactor{ get; set; }
        public int Generation { get; private set; }
        public int BetsWon { get; private set; }
        public int BetsLost { get; private set; }
        public int BetsNotPlayed { get; private set; }
        public int BetsSkipped { get; private set; }
        public int SimulationSampleSize { get; private set; }

        /// <summary>
        /// Constructor for node. If PlayLimit, drawLimit or sampleSize are under/over
        /// the limits, they are corrected to be right at the limit.
        /// </summary>
        /// <param name="playL"></param>
        /// <param name="drawL"></param>
        /// <param name="minStake"></param>
        /// <param name="gen"></param>
        /// <param name="sampleS"></param>
        /// <exception cref="ArgumentException">Thrown if sample size
        /// is less than 1 or if minimum stake is less than 0.</exception>
        public Node(double playLimit, double drawLimit, double minStake, int gen, int sampleSize)
        {
            if (sampleSize < 1)
                sampleSize = 1;
            else if (sampleSize > 100)
                sampleSize = 100;

            if (drawLimit < 0)
                drawLimit = 0.0;
            else if (drawLimit > 10.0)
                drawLimit = 10;

            if (playLimit < 0)
                playLimit = 0.0;
            else if (playLimit > 5)
                playLimit = 5;

            if (minStake <= 0 || gen < 0)
                throw new ArgumentException();

            PlayLimit = playLimit;
            DrawLimit = drawLimit;
            MinimumStake = minStake;
            Generation = gen;
            SimulationSampleSize = sampleSize;
        }

        /// <summary>
        /// Creates a node by initializing random values to the parameters
        /// algorithm optimizes. 
        /// Initializes generation as 0,
        /// PlayLimit as a value between 0.0 - 5.0,
        /// DrawLimit as a value betwen 0.0 - 10.0,
        /// SimulationSampleSize as a value between 1 and 100.
        /// </summary>
        /// <param name="rand">Random-object used to provide randomized values.</param>
        /// <param name="minimumStake">Minimum stake for a bet, set by user.</param>
        public Node(Random rand, double minimumStake)
        {
            Generation = 0;
            MinimumStake = minimumStake;
            SimulationSampleSize = rand.Next(1, 100);
            PlayLimit = rand.NextDouble() * (5 - 0) + 0;
            DrawLimit = rand.NextDouble() * (10 - 0) + 0;
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
