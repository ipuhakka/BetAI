using System;
using System.Collections.Generic;
using Database;
using BetAI.BetSim;
using BetAI.Exceptions;
using Newtonsoft.Json;

namespace BetAI.Genetics
{
    public class Node
    {
        const int MaxSimulationSampleSize = 40;
        const double MaxDrawLimit = 5;
        const double MaxPlayLimit = 2;
        const double MinPlayLimit = 0.01;

        [JsonProperty]
        public double PlayLimit { get; private set; }
        [JsonProperty]
        public double DrawLimit { get; private set; }
        [JsonProperty]
        public double MinimumStake { get; private set; }
        [JsonProperty]
        public double Fitness { get; private set; }
        [JsonProperty]
        public double CrossoverValue{ get; set; }
        [JsonProperty]
        public int Generation { get; private set; }
        [JsonProperty]
        public int BetsWon { get; private set; }
        [JsonProperty]
        public int BetsLost { get; private set; }
        [JsonProperty]
        public int BetsNotPlayed { get; private set; }
        [JsonProperty]
        public int BetsSkipped { get; private set; }
        [JsonProperty]
        public int SimulationSampleSize { get; private set; }

        [JsonConstructor]
        public Node()
        {

        }

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
            else if (sampleSize > MaxSimulationSampleSize)
                sampleSize = MaxSimulationSampleSize;

            if (drawLimit < 0)
                drawLimit = 0.0;
            else if (drawLimit > MaxDrawLimit)
                drawLimit = MaxDrawLimit;

            if (playLimit < MinPlayLimit)
                playLimit = MinPlayLimit;
            else if (playLimit > MaxPlayLimit)
                playLimit = MaxPlayLimit;

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
        /// <param name="generation">Optional paremeter, generation number.</param>
        public Node(Random rand, double minimumStake, int generation = 0)
        {
            Generation = generation;
            MinimumStake = minimumStake;
            SimulationSampleSize = rand.Next(1, MaxSimulationSampleSize);
            PlayLimit = rand.NextDouble() * (MaxPlayLimit - MinPlayLimit) + MinPlayLimit;
            DrawLimit = rand.NextDouble() * (MaxDrawLimit - 0) + 0;
        }

        /// <summary>
        /// Function estimates results for each match, and returns a list of 
        /// playable wagers. 
        /// </summary>
        public List<Wager> PlayBets(List<Match> matches)
        {
            List<Wager> wagers = new List<Wager>();

            foreach(Match m in matches)
            {

                double? predictedReturnedValue = Predict.PredictResult(m, SimulationSampleSize);

                if (predictedReturnedValue == null)
                {
                    BetsSkipped++;
                    continue;
                }

                double predictedResult = (double)predictedReturnedValue;

                double predictedOdd = Bet.GetOddForPredictedResult(m, predictedResult, DrawLimit);
                double betRisk = Bet.CalculateBetRisk(m, predictedResult, predictedOdd, DrawLimit, PlayLimit);
                double stake = Bet.CalculateStake(MinimumStake, betRisk, PlayLimit);

                int result = Bet.GetPredictedBetResult(predictedResult, DrawLimit);
                if (result == 1)
                {
                    m.SimulatedResult = '1';
                } else if (result == 0)
                {
                    m.SimulatedResult = 'X';
                } else
                {
                    m.SimulatedResult = '2';
                }

                if (PlayLimit <= betRisk)
                {
                    wagers.Add(new Wager(new List<Match> {m}, stake));
                }
            }
            return wagers;
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
            Fitness = 0;

            foreach (Match m in sample)
            {
                double? predictedReturnedValue = Predict.PredictResult(m, SimulationSampleSize);

                if (predictedReturnedValue == null)
                {
                    BetsSkipped++;
                    continue;
                }

                double predictedResult = (double)predictedReturnedValue;

                double betProfit = Bet.PlayBet(m, predictedResult, PlayLimit, MinimumStake, DrawLimit);
                if (betProfit == 0)
                    BetsNotPlayed++;
                else if (betProfit > 0)
                    BetsWon++;
                else
                    BetsLost++;

                Fitness += betProfit;
            }
            return Fitness;
        }

        public double GetMaxPlayLimit() { return MaxPlayLimit; }
        public double GetMinPlayLimit() { return MinPlayLimit; }
        public double GetMaxDrawLimit() { return MaxDrawLimit; }
        public int GetMaxSimulationSampleSize() { return MaxSimulationSampleSize; }

        public override bool Equals(object obj)
        {
            Node comp = obj as Node;
            return PlayLimit.Equals(comp.PlayLimit) && DrawLimit.Equals(comp.DrawLimit) &&
                MinimumStake.Equals(comp.MinimumStake) && Fitness.Equals(comp.Fitness) &&
                CrossoverValue.Equals(comp.CrossoverValue) && Generation.Equals(comp.Generation)
                && BetsWon.Equals(comp.BetsWon) && BetsLost.Equals(comp.BetsLost) &&
                BetsNotPlayed.Equals(comp.BetsNotPlayed) && BetsSkipped.Equals(comp.BetsSkipped)
                && SimulationSampleSize.Equals(comp.SimulationSampleSize);
        }

        public override int GetHashCode()
        {
            int hash = PlayLimit.GetHashCode() * 12;
            hash = hash * SimulationSampleSize.GetHashCode() * 5;
            hash = hash * DrawLimit.GetHashCode() + 2;
            return hash;
        }
    }
}
