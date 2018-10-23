﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using BetAI.Exceptions;
using BetAI.Data;

namespace BetAI.BetSim
{
    public class Predict
    {
        /// <summary>
        /// Calculates predicted result for a match. 
        /// </summary>
        /// <exception cref="NotSimulatedException">Thrown when NotEnoughDataException
        /// is thrown by a call to database layer.</exception>
        public double PredictResult(Match toPredict, int sampleSize)
        {
            Match[] hometeamPreviousMatches;
            Match[] awayteamPreviousMatches;
            double homeScoredLeagueAvg = 0;
            double awayScoredLeagueAvg = 0;

            try
            {
                hometeamPreviousMatches = Matches.GetNLastFromTeamBeforeMatch(true, toPredict, sampleSize);
                awayteamPreviousMatches = Matches.GetNLastFromTeamBeforeMatch(false, toPredict, sampleSize);
                homeScoredLeagueAvg = Matches.GetSeasonAverage(true, toPredict);
                awayScoredLeagueAvg = Matches.GetSeasonAverage(false, toPredict);
            }
            catch (NotEnoughDataException)
            {
                throw new NotSimulatedException();
            }

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
        private double CountMeanScoredGoals(Match[] previousMatches, string team)
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
        private double CountMeanConcededGoals(Match[] previousMatches, string team)
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