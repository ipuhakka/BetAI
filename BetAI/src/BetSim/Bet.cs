using System;
using System.Collections.Generic;
using Database;
using BetAI.Exceptions;


namespace BetAI.BetSim
{
    /// <summary>
    /// Bet class is responsible for assessing a valid stake for bet, 
    /// and playing the bet accordingly.
    /// </summary>
    public class Bet
    {
        /// <summary>
        /// PlayBet assesses the risk involved in playing bet for Match m,
        /// and plays the bet if it finds value based on predicted result,
        /// given odds and a playLimit. 
        /// </summary>
        /// <param name="m">Match predicted.</param>
        /// <param name="predictedResult">Simulated result for the match.</param>'
        /// <param name="playLimit">Limit which the betValue must be higher to play
        /// the bet.</param>
        /// <param name="baseStake">Lowest stake which is played.</param>
        /// <param name="drawLimit">If result is smaller than absolute(drawLimit) bet
        /// is predicted as a draw.</param>
        /// <returns>Amount of money won/lost by bet</returns>
        public double PlayBet(Match m, double predictedResult, double playLimit, double baseStake, double drawLimit)
        {
            double expectedResultPercentage = CalculateExpectedResultPercentage(predictedResult);
            double predictedResultOdd = GetOddForPredictedResult(m, predictedResult, drawLimit);
            double betCoefficient = expectedResultPercentage / (1 / predictedResultOdd);
            Console.WriteLine(expectedResultPercentage);
            Console.WriteLine(1 / predictedResultOdd);
            Console.WriteLine(betCoefficient);
            if (playLimit > betCoefficient)
                return 0;

            double stake = baseStake * (betCoefficient / playLimit);

            if (GetBetResult(m, predictedResult, drawLimit) == 1)
                return stake * predictedResultOdd;
            else
                return -stake;
        }

        /// <summary>
        /// Calculates the percentage which the simulation sets as 
        /// probability of the result.
        /// e^(-absolute(result)) * (absolute(result)) / 1
        /// </summary>
        /// <param name="expectedResult">Result simulated.</param>
        /// <returns>Calculated probability of the expectedResult</returns>
        private double CalculateExpectedResultPercentage(double expectedResult)
        {
            return Math.Pow(Math.E, -Math.Abs(expectedResult)) * Math.Abs(expectedResult) / 1;
        }

        private double GetOddForPredictedResult(Match m, double predictedResult, double drawLimit)
        {
            if (Math.Abs(predictedResult) < drawLimit)
                return m.DrawOdd;
            else if (predictedResult > 0)
                return m.HomeOdd;
            else if (predictedResult < 0)
                return m.AwayOdd;
            else // predictedResult is exactly 0
                return m.DrawOdd;
        }

        /// <summary>
        /// Returns 1 if simulated bet was correct, 0 if not.
        /// </summary>
        private int GetBetResult(Match m, double predictedResult, double drawLimit)
        {
            int predictedBetResult = 0;
            if (Math.Abs(predictedResult) < drawLimit)
                predictedBetResult = 0;
            else if (predictedResult > 0)
                predictedBetResult =  1;
            else if (predictedResult < 0)
                predictedBetResult = -1;
            else // predictedResult is exactly 0
                predictedBetResult = 0;

            if (predictedBetResult == m.Homescore - m.Awayscore)
                return 1;
            if (predictedBetResult == 1 && m.Homescore > m.Awayscore)
                return 1;
            if (predictedBetResult == -1 && m.Homescore < m.Awayscore)
                return 1;
            else
                return 0;
        }


    }
}
