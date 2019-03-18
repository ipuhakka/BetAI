using System.IO;
using System.Collections.Generic;
using System.Linq;
using Database;
using BetAI.Exceptions;
using BetAI.FileOperations;
using BetAI.Genetics;

namespace BetAI.BetSim
{
    public static class Predict
    {
        /// <summary>
        /// Predicts all bets and returns a list of Wager-objects which
        /// the algorithm in savefile deems to be playable.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        public static List<Wager> PredictBets(List<Match> matches, string savefile, string season)
        {
            if (Load.SaveExists(savefile, true))
            {
                Values values = Load.LoadValues(savefile, true);
                Matches.SetMatches(values.Database);
                matches.ForEach(m =>  //Set league and season so averages can be counted.
                {
                    m.League = Matches.GetLeagueForTeam(m.Hometeam);
                    m.Season = season;
                });
                Node maxFitness = Load.LoadSecondNewestGeneration(savefile).Aggregate((curMax, newNode) 
                    =>  curMax.Fitness < newNode.Fitness ? newNode : curMax );
                var prunedMatchList = matches
                    .Where(match => Matches.TeamsExist(match.Hometeam,
                        match.Awayteam))
                    .ToList();
                Matches.CreateMatchDataStructs(prunedMatchList, maxFitness.SimulationSampleSize);
                return maxFitness.PlayBets(prunedMatchList);
            } else
            {
                throw new FileNotFoundException(savefile);
            }
        }

        /// <summary>
        /// Calculates predicted result for a match. 
        /// Returns null if match could not be simulated:
        /// In that case, either average goals could not be counted
        /// for the season, or teams did not have enough matches played.
        /// </summary>
        public static double? PredictResult(Match toPredict, int sampleSize)
        {
            Match[] hometeamPreviousMatches;
            Match[] awayteamPreviousMatches;
            double homeScoredLeagueAvg = 0;
            double awayScoredLeagueAvg = 0;

            hometeamPreviousMatches = Matches.GetNLastFromTeamBeforeMatch(true, toPredict, sampleSize);
            awayteamPreviousMatches = Matches.GetNLastFromTeamBeforeMatch(false, toPredict, sampleSize);
            homeScoredLeagueAvg = Matches.GetSeasonAverage(true, toPredict);
            awayScoredLeagueAvg = Matches.GetSeasonAverage(false, toPredict);


            if (homeScoredLeagueAvg <= 0 || awayScoredLeagueAvg <= 0 ||
                hometeamPreviousMatches == null || awayteamPreviousMatches == null)
                return null;

            double homeScoredAvg = CountMeanScoredGoals(hometeamPreviousMatches.ToArray(), toPredict.Hometeam);
            double awayScoredAvg = CountMeanScoredGoals(awayteamPreviousMatches.ToArray(), toPredict.Awayteam);
            double homeConcededAvg = CountMeanConcededGoals(hometeamPreviousMatches.ToArray(), toPredict.Hometeam);
            double awayConcededAvg = CountMeanConcededGoals(awayteamPreviousMatches.ToArray(), toPredict.Awayteam);

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
        private static double CountGoalEstimate(double attackStrength, double defenseStrength, double leagueAvg) 
        {
            return attackStrength * defenseStrength * leagueAvg;
        }

        /// <summary>
        /// Returns the stregth value for team. 
        /// Strength = team average goals / league average goals. 
        /// </summary>
        /// <returns></returns>
        private static double CountStrength(double teamScoredAvg, double leagueAvg)
        {
            return teamScoredAvg / leagueAvg;
        }

        /// <summary>
        /// Calculates average number of goals scored by team.
        /// </summary>
        /// <param name="previousMatches">Matches used to calculate average for team</param>
        /// <param name="team">Team from which average goals is calculated</param>
        /// <returns>Average number of goals scored.</returns>
        private static double CountMeanScoredGoals(Match[] previousMatches, string team)
        {
            double sum = 0;

            for (int i = 0; i < previousMatches.Length; i++)
            {
                if (previousMatches[i].Hometeam.Equals(team))
                    sum = sum + previousMatches[i].Homescore;
                else if (previousMatches[i].Awayteam.Equals(team))
                    sum = sum + previousMatches[i].Awayscore;
            }

            return sum / previousMatches.Length;
        }

        /// <summary>
        /// Calculates average number of goals conceded by team.
        /// </summary>
        /// <param name="previousMatches">Matches used to calculate average for team</param>
        /// <param name="team">Team from which average goals is calculated</param>
        /// <returns>Average number of goals conceded by team.</returns>
        private static double CountMeanConcededGoals(Match[] previousMatches, string team)
        {
            double sum = 0;

            for (int i = 0; i < previousMatches.Length; i++)
            {
                if (previousMatches[i].Hometeam.Equals(team))
                    sum = sum + previousMatches[i].Awayscore;
                else if (previousMatches[i].Awayteam.Equals(team))
                    sum = sum + previousMatches[i].Homescore;
            }

            return sum / previousMatches.Length;
        }
    }
}
