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
        public double CrossOverProbability { get; private set; }
        public int Generation { get; private set; }
        public int BetsWon { get; private set; }
        public int BetsLost { get; private set; }
        public int BetsNotPlayed { get; private set; }
        public int BetsSkipped { get; private set; }

        public Node(double playL, double drawL, double minStake, int gen)
        {
            PlayLimit = playL;
            DrawLimit = drawL;
            MinimumStake = minStake;
            Generation = gen;
        }

        public double EvaluateFitness(List<Match> sample, string dbPath, int sampleSize)
        {
            Predict predict = new Predict();
            Bet bet = new Bet();
            Fitness = 0;
            foreach (Match m in sample)
            {
                try
                {
                    double predictedResult = predict.PredictResult(m, dbPath, sampleSize);
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
