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
            List<Match> hometeamPreviousMatches = new List<Match>();
            List<Match> awayteamPreviousMatches = new List<Match>();
            double homeScoredLeagueAvg = 0;
            double awayScoredLeagueAvg = 0;

            DB db = new DB(databasePath);
            try
            {
                hometeamPreviousMatches = db.SelectNLastFromTeam(true, sampleSize, toPredict.Date, toPredict.Hometeam);
                awayteamPreviousMatches = db.SelectNLastFromTeam(false, sampleSize, toPredict.Date, toPredict.Awayteam);
                homeScoredLeagueAvg = db.LeagueHomeAVGBeforeDate(toPredict.Date, toPredict.Season, toPredict.League);
                awayScoredLeagueAvg = db.LeagueAwayAVGBeforeDate(toPredict.Date, toPredict.Season, toPredict.League);
            }
            catch (NotEnoughDataException)
            {
                throw new NotSimulatedException();
            }

            double homeScoredAvg = CountMeanScoredGoals(hometeamPreviousMatches, toPredict.Hometeam);
            double awayScoredAvg = CountMeanScoredGoals(awayteamPreviousMatches, toPredict.Awayteam);
            double homeConcededAvg = CountMeanConcededGoals(hometeamPreviousMatches, toPredict.Hometeam);
            double awayConcededAvg = CountMeanConcededGoals(awayteamPreviousMatches, toPredict.Awayteam);

            double homeAttStrength = CountStrength(homeScoredAvg, homeScoredLeagueAvg);
            double homeDefStrength = CountStrength(homeConcededAvg, awayScoredLeagueAvg);
            double awayAttStrength = CountStrength(awayScoredAvg, awayScoredLeagueAvg);
            double awayDefStrength = CountStrength(awayConcededAvg, homeScoredLeagueAvg);

            double homeGoalEstimate = CountGoalEstimate(homeAttStrength, awayDefStrength, homeScoredLeagueAvg);
            double awayGoalEstimate = CountGoalEstimate(awayAttStrength, homeDefStrength, awayScoredLeagueAvg);

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
