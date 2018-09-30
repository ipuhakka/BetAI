using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using BetAI.Exceptions;

namespace BetAI.BetSim
{
    public class BetSimulator
    {
        /// <summary>
        /// Calculates predicted result for a match. 
        /// </summary>
        /// <exception cref="NotSimulatedException">Thrown when NotEnoughDataException
        /// is thrown by a call to database layer.</exception>
        public double PredictResult(Match toPredict, string databasePath, int sampleSize)
        {
            DB db = new DB(databasePath);
            List<Match> hometeamPreviousMatches = db.SelectNLastFromTeam(true, sampleSize, toPredict.Date, toPredict.Hometeam);
            List<Match> awayteamPreviousMatches = db.SelectNLastFromTeam(false, sampleSize, toPredict.Date, toPredict.Awayteam);
            double homeScoredAvg = CountMeanScoredGoals(hometeamPreviousMatches, toPredict.Hometeam);
            double awayScoredAvg = CountMeanScoredGoals(awayteamPreviousMatches, toPredict.Awayteam);
            double homeConcededAvg = CountMeanConcededGoals(hometeamPreviousMatches, toPredict.Hometeam);
            double awayConcededAvg = CountMeanConcededGoals(awayteamPreviousMatches, toPredict.Awayteam);
            double homeScoredLeagueAvg = db.LeagueHomeAVGBeforeDate(toPredict.Date, toPredict.Season, toPredict.League);
            double awayScoredLeagueAvg = db.LeagueAwayAVGBeforeDate(toPredict.Date, toPredict.Season, toPredict.League);

            double homeAttStrength = CountStrength(homeScoredAvg, homeScoredLeagueAvg);
            double homeDefStrength = CountStrength(homeConcededAvg, awayScoredLeagueAvg);
            double awayAttStrength = CountStrength(awayScoredAvg, awayScoredLeagueAvg);
            double awayDefStrength = CountStrength(awayConcededAvg, homeScoredLeagueAvg);
            Console.WriteLine(homeConcededAvg + "/" + awayScoredLeagueAvg);

            Console.WriteLine(homeAttStrength);
            Console.WriteLine(homeDefStrength);
            Console.WriteLine(awayAttStrength);
            Console.WriteLine(awayDefStrength);
            double homeGoalEstimate = CountGoalEstimate(homeAttStrength, awayDefStrength, homeScoredLeagueAvg);
            double awayGoalEstimate = CountGoalEstimate(awayAttStrength, homeDefStrength, awayScoredLeagueAvg);
            Console.WriteLine(homeGoalEstimate + "-" + awayGoalEstimate);

            return homeGoalEstimate - awayGoalEstimate;
        }

        /// <summary>
        /// Calculates an estimate for how many goals a team scores.
        /// Estimate is attack * def * leagueAvg.
        /// </summary>
        private double CountGoalEstimate(double attackStrength, double defenseStrength, double leagueAvg) 
        {
            return attackStrength * defenseStrength * leagueAvg;
        }

        /// <summary>
        /// Returns the stregth value for team. 
        /// Strength = team average goals / league average goals. 
        /// </summary>
        /// <returns></returns>
        private double CountStrength(double teamScoredAvg, double leagueAvg)
        {
            return teamScoredAvg / leagueAvg;
        }

        /// <summary>
        /// Calculates average number of goals scored by team.
        /// </summary>
        /// <param name="previousMatches">Matches used to calculate average for team</param>
        /// <param name="team">Team from which average goals is calculated</param>
        /// <returns>Average number of goals scored.</returns>
        private double CountMeanScoredGoals(List<Match> previousMatches, string team)
        {
            double sum = 0;

            foreach(Match m in previousMatches)
            {
                if (m.Hometeam.Equals(team))
                    sum = sum + m.Homescore;
                else if (m.Awayteam.Equals(team))
                    sum = sum + m.Awayscore;
            }

            return sum / previousMatches.Count;
        }

        /// <summary>
        /// Calculates average number of goals conceded by team.
        /// </summary>
        /// <param name="previousMatches">Matches used to calculate average for team</param>
        /// <param name="team">Team from which average goals is calculated</param>
        /// <returns>Average number of goals conceded by team.</returns>
        private double CountMeanConcededGoals(List<Match> previousMatches, string team)
        {
            double sum = 0;

            foreach (Match m in previousMatches)
            {
                if (m.Hometeam.Equals(team))
                    sum = sum + m.Awayscore;
                else if (m.Awayteam.Equals(team))
                    sum = sum + m.Homescore;
            }

            return sum / previousMatches.Count;
        }
    }
}
