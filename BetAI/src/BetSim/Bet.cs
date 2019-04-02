using System;
using Database;


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
            var predictedResultOdd = GetOddForPredictedResult(m, predictedResult, drawLimit);
            var betCoefficient = CalculateBetRisk(m, predictedResult, predictedResultOdd, drawLimit, playLimit);

            if (playLimit > betCoefficient)
                return 0;

            var stake = CalculateStake(baseStake, betCoefficient, playLimit);

            if (GetBetResult(m, predictedResult, drawLimit) == BetResult.Won)
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
            return CalculateExpectedResultPercentage(predictedResult) /
                 (1 / predictedOdd);
        }

        /// <summary>
        /// Returns MatchResult based on result and set drawLimit.
        /// </summary>
        /// <returns></returns>
        public static MatchResult GetMatchResult(double result, double drawLimit)
        {
            if (Math.Abs(result) < drawLimit)
                return MatchResult.Draw;
            else if (result > 0)
                return MatchResult.Homewin;
            else if (result < 0)
                return MatchResult.Awaywin;
            else
                return MatchResult.Draw;
        }

        public static double GetOddForPredictedResult(Match m, double predictedResult, double drawLimit)
        {
            if (Math.Abs(predictedResult) < drawLimit)
                return m.DrawOdd;
            else if (predictedResult > 0)
                return m.HomeOdd;
            else if (predictedResult < 0)
                return m.AwayOdd;
            else 
                return m.DrawOdd;
        }

        /// <summary>
        /// Returns character mark for result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static char GetBetResultMark(MatchResult result)
        {
            switch (result)
            {
                case MatchResult.Homewin:
                    return '1';
                case MatchResult.Draw:
                    return 'X';
                case MatchResult.Awaywin:
                    return '2';
                default:
                    throw new ArgumentException($"Invalid MatchResult: {(int)result} is not" +
                        $" a valid MatchResult");
            }
        }

        /// <summary>
        /// Returns BetResult for predicted match.
        /// </summary>
        private static BetResult GetBetResult(Match m, double predictedResult, double drawLimit)
        {
            var predictedMatchResult = GetMatchResult(predictedResult, drawLimit);
            var actualMatchResult = GetMatchResult(m.Homescore - m.Awayscore, 0);

            if (predictedMatchResult == actualMatchResult)
            {
                return BetResult.Won;
            }
            else
                return BetResult.Lost;
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
