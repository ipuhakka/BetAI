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
    public static class Bet
    {
        /// <summary>
        /// PlayBet assesses the risk involved in playing bet for Match m,
        /// and plays the bet if it finds value based on predicted result,
        /// given odds and a playLimit. Returns profit of the bet.
        /// </summary>
        /// <param name="m">Match predicted.</param>
        /// <param name="predictedResult">Simulated result for the match.</param>'
        /// <param name="playLimit">Limit which the betValue must be higher to play
        /// the bet.</param>
        /// <param name="baseStake">Lowest stake which is played.</param>
        /// <param name="drawLimit">If result is smaller than absolute(drawLimit) bet
        /// is predicted as a draw.</param>
        /// <returns>Profit of the bet. If bet is lost: -stake,
        /// if bet is won = (stake * predictedResultOdd) - stake</returns>
        public static double PlayBet(Match m, double predictedResult, double playLimit, double baseStake, double drawLimit)
        {
            double predictedResultOdd = GetOddForPredictedResult(m, predictedResult, drawLimit);
            double betCoefficient = CalculateBetRisk(m, predictedResult, predictedResultOdd, drawLimit, playLimit);
            if (playLimit > betCoefficient)
                return 0;

            double stake = CalculateStake(baseStake, betCoefficient, playLimit);

            if (GetBetResult(m, predictedResult, drawLimit) == 1)
                return (stake * predictedResultOdd) - stake;
            else
                return -stake;
        }

        public static double CalculateStake(double baseStake, double risk, double playLimit)
        {
            return baseStake * (risk / playLimit);
        }

        /// <summary>
        /// Calculates a risk for bet, based on its
        /// estimated result, margin for a tie game, and
        /// set limit for when bet is playable.
        /// </summary>
        public static double CalculateBetRisk(Match m, double predictedResult, double predictedOdd, double drawLimit, double playLimit)
        {
            double expectedResultPercentage = CalculateExpectedResultPercentage(predictedResult);
            return expectedResultPercentage / (1 / predictedOdd);
        }

        /// <summary>
        /// Returns 1 if predicted a home win, 0 if draw, and -1
        /// if predicted away win.
        /// </summary>
        /// <returns></returns>
        public static int GetPredictedBetResult(double predictedResult, double drawLimit)
        {
            if (Math.Abs(predictedResult) < drawLimit)
                return 0;
            else if (predictedResult > 0)
                return 1;
            else if (predictedResult < 0)
                return -1;
            else // predictedResult is exactly 0
                return 0;
        }

        public static double GetOddForPredictedResult(Match m, double predictedResult, double drawLimit)
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
        private static int GetBetResult(Match m, double predictedResult, double drawLimit)
        {
            int predictedBetResult = GetPredictedBetResult(predictedResult, drawLimit);
            if (predictedBetResult == m.Homescore - m.Awayscore)
                return 1;
            if (predictedBetResult == 1 && m.Homescore > m.Awayscore)
                return 1;
            if (predictedBetResult == -1 && m.Homescore < m.Awayscore)
                return 1;
            else
                return 0;
        }

        /// <summary>
        /// Calculates the percentage which the simulation sets as 
        /// probability of the result.
        /// e^(-absolute(result)) * (absolute(result)) / 1
        /// </summary>
        /// <param name="expectedResult">Result simulated.</param>
        /// <returns>Calculated probability of the expectedResult</returns>
        private static double CalculateExpectedResultPercentage(double expectedResult)
        {
            return Math.Pow(Math.E, -Math.Abs(expectedResult)) * Math.Abs(expectedResult) / 1;
        }
    }
}
