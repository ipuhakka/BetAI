using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using System.Data.SQLite;
using BetAI.Exceptions;

namespace BetAI.Data
{
    public class QueryMatches
    {
        private static List<Match> matches;
        private static MatchData[] matchesData;

        /// <summary>
        /// Reads all matches from specified database.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="SQLiteException">Thrown if connection to database fails.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetMatches(string path)
        {
            if (path == null)
                throw new ArgumentNullException();
            DB db = new DB(path);
            matches = db.SelectAllMatchesFromDatabase();
        }

        /// <summary>
        /// Returns a list of matches based on the indexes.
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException">thrown if matches are not set.</exception>
        public static List<Match> SelectMatchesWithRowIndex(List<int> indexes)
        {
            List<Match> matchSample = new List<Match>();

            foreach (int index in indexes)
            {
                if (index < 0 || index >= matches.Count)
                    throw new IndexOutOfRangeException();
                matchSample.Add(matches[index]);
            }
            return matchSample;
        }

        /// <summary>
        /// Function creates a MatchData-struct for each match in sample.
        /// </summary>
        /// <param name="sample">List of match objects</param>
        /// <param name="maxSampleSize">Sample size which is used
        /// for getting n previous matches for home and awayteam.</param>
        public static void CreateMatchDataStructs(List<Match> sample, int maxSampleSize)
        {
            matchesData = new MatchData[sample.Count];

            for (int i = 0; i < sample.Count; i++)
            {
                double homeAvg = SeasonHomeGoalAvgBeforeDate(sample[i]);
                double awayAvg = SeasonAwayGoalAvgBeforeDate(sample[i]);
                Match[] hometeamPrevious = SelectNLastFromTeam(true, maxSampleSize, sample[i].Date, sample[i].Hometeam).ToArray();
                Match[] awayteamPrevious = SelectNLastFromTeam(false, maxSampleSize, sample[i].Date, sample[i].Awayteam).ToArray();
                matchesData[i] = new MatchData(homeAvg, awayAvg, hometeamPrevious, awayteamPrevious, sample[i]);
            }
        }

        /// <summary>
        /// Returns number of loaded matches.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public static int GetMatchCount()
        {
            return matches.Count;
        }

        /// <summary>
        /// Uses MatchData to return N last for either home- or awayteam.
        /// </summary>
        /// <returns></returns>
        /// <param name="hometeam">Indicates whether home or awayteams previous
        /// matches are returned.</param>
        /// <param name="sampleSize">Samplesize which is used in simulating. 
        /// Indicates how many matches are returned.</param>
        /// <param name="toPredict">Match to be predicted.</param>
        /// <exception cref="NotEnoughDataException">Thrown if MatchData doesn't
        /// contain enough matches for sampleSize.</exception>
        /// <exception cref="ArgumentNullException">Thrown if MatchData structures
        /// have not been created using CreateMatchDataStructs-function.</exception>
        public static Match[] GetNLastFromTeamBeforeMatch(bool hometeam, Match toPredict, int sampleSize)
        {
            if (hometeam)
                return matchesData.Where(match => match.toPredict.Equals(toPredict)).ToArray()[0].GetNLastFromSampleFromHometeam(sampleSize);
            else
                return matchesData.Where(match => match.toPredict.Equals(toPredict)).ToArray()[0].GetNLastFromSampleFromAwayteam(sampleSize);
        }

        /// <summary>
        /// returns the average number of goals scored in home- or
        /// awaymatches, depending on boolean parameter homematches.
        /// </summary>
        /// <param name="homematches">Indicates if home or away average is returned.</param>
        /// <returns>Average numebr of goals scored that season, either in
        /// home or away.</returns>
        /// <param name="toPredict">Match to be predicted.</param>
        /// <exception cref="NotEnoughDataException"></exception>
        public static double GetSeasonAverage(bool homematches, Match toPredict)
        {
            if (homematches)
                return matchesData.Where(match => match.toPredict.Equals(toPredict)).ToArray()[0].GetSeasonHomeGoalAverage();
            else
                return matchesData.Where(match => match.toPredict.Equals(toPredict)).ToArray()[0].GetSeasonAwayGoalAverage();
        }

        /// <summary>
        /// Returns an average number of goals 
        /// scored by hometeam,in selected league and season before date d.
        /// </summary>
        /// <param name="d"></param>
        /// <exception cref="ArgumentNullException">Thrown if MatchData structures
        /// have not been created using CreateMatchDataStructs-function.</exception>
        /// <returns>Average goals scored by hometeam up to Match m, -1 if
        /// no matches had been played before Match m.</returns>
        public static double SeasonHomeGoalAvgBeforeDate(Match m)
        {
            List<Match> seasonMatchesBeforeDate = matches.Where(x => x.Season == m.Season && x.Date < m.Date && x.League == m.League).ToList();
            if (seasonMatchesBeforeDate.Count == 0)
                return -1;
            return seasonMatchesBeforeDate.Average(match => match.Homescore);
        }

        /// <summary>
        /// Returns an average number of goals 
        /// scored by awayteam,in selected league and season before date d.
        /// </summary>
        /// <param name="d"></param>
        /// <returns>Average goals scored by hometeam up to Match m, -1 if
        /// no matches had been played before Match m.</returns>
        /// <exception cref="ArgumentNullException">Thrown if MatchData structures
        /// have not been created using CreateMatchDataStructs-function.</exception>
        public static double SeasonAwayGoalAvgBeforeDate(Match m)
        {
            List<Match> seasonMatchesBeforeDate = matches.Where(x => x.Season == m.Season && x.Date < m.Date && x.League == m.League).ToList();
            if (seasonMatchesBeforeDate.Count == 0)
                return -1;
            return seasonMatchesBeforeDate.Average(match => match.Awayscore);
        }

        /// <summary>
        /// Function selects N last home or away matches from team before date.
        /// If not enough homeaway matches are found,  both home/away matches are searched,
        /// and returned.
        /// </summary>
        /// <returns>Match-array the size of n or of match count prior to beforeDate.</returns>
        public static Match[] SelectNLastFromTeam(bool searchHomeMatches, int n, DateTime beforeDate, string teamname)
        {
            List<Match> nLast = new List<Match>();
            if (searchHomeMatches)
            {
                nLast = matches.Where(match => match.Date < beforeDate && match.Hometeam == teamname).ToList();
            }
            else
            {
                nLast = matches.Where(match => match.Date < beforeDate && match.Awayteam == teamname).ToList();
            }

            if (nLast.Count < n)
            {
                nLast = matches.Where(match => match.Date < beforeDate && (match.Hometeam == teamname || match.Awayteam == teamname)).ToList();
            }
          
            return nLast.OrderByDescending(m => m.Date).Take(n).ToArray();
        }

        /// <summary>
        /// Structure contains an array of previous matches for hometeam, awayteam,
        /// and average number of goals scored that season in league both home
        /// and away.
        /// </summary>
        private struct MatchData
        {
            public double homeAvg, awayAvg;
            public Match[] hometeamPreviousMatches;
            public Match[] awayteamPreviousMatches;
            public Match toPredict;

            public MatchData(double homeA, double awayA, Match[] hometeamPM, Match[] awayteamPM, Match toPred )
            {
                homeAvg = homeA;
                awayAvg = awayA;
                hometeamPreviousMatches = hometeamPM;
                awayteamPreviousMatches = awayteamPM;
                toPredict = toPred;                   
            }
            
            /// <summary>
            /// Returns an array of last matches from team.
            /// If team has n previous homematches in the list, these 
            /// are returned. Otherwise n-last total matches for team are
            /// returned.
            /// If n is larger than array of previous matches, 
            /// NotEnoughDataException is thrown.
            /// </summary>
            /// <param name="n">Number of matches that are looked for.</param>
            /// <returns>n-sized array of teams previous matches.</returns>
            /// <exception cref="NotEnoughDataException"></exception>
            public Match[] GetNLastFromSampleFromHometeam(int n)
            {
                if (n > hometeamPreviousMatches.Length)
                   throw new NotEnoughDataException();

                string hometeam = toPredict.Hometeam;
                Match[] homeMatches = hometeamPreviousMatches.Where(match => match.Hometeam.Equals(hometeam)).ToList().OrderByDescending(match => match.Date).ToArray();

                if (homeMatches.Length == n)
                    return homeMatches;

                return hometeamPreviousMatches.ToList().OrderByDescending(match => match.Date).ToList().GetRange(0, n).ToArray(); 
            }

            /// <summary>
            /// Returns an array of last matches from team.
            /// If team has n previous awaymatches in the list, these 
            /// are returned. Otherwise n-last total matches for team are
            /// returned.
            /// If n is larger than array of previous matches, 
            /// NotEnoughDataException is thrown.
            /// </summary>
            /// <param name="n">Number of matches that are looked for.</param>
            /// <returns>n-sized array of teams previous matches.</returns>
            /// <exception cref="NotEnoughDataException"></exception>
            public Match[] GetNLastFromSampleFromAwayteam(int n)
            {
                if (n > awayteamPreviousMatches.Length)
                    throw new NotEnoughDataException();

                string awayteam = toPredict.Awayteam;
                Match[] awayMatches = awayteamPreviousMatches.Where(match => match.Awayteam.Equals(awayteam)).ToList().OrderByDescending(match => match.Date).ToArray();

                if (awayMatches.Length == n)
                    return awayMatches;
                return awayteamPreviousMatches.ToList().OrderByDescending(match => match.Date).ToList().GetRange(0, n).ToArray();
            }

            /// <summary>
            /// Returns homeAvg. If homeAvg is -1,
            /// there are no matches played before, and
            /// NotEnoughDataException is thrown.
            /// </summary>
            /// <returns>Average numebr of goals scored by hometeam in season
            /// of match toPredict.</returns>
            /// <exception cref="NotEnoughDataException"></exception>
            public double GetSeasonHomeGoalAverage()
            {
                if (homeAvg == -1)
                    throw new NotEnoughDataException();
                return homeAvg;
            }

            /// <summary>
            /// Returns awayAvg. If awayAvg is -1,
            /// there are no matches played before, and
            /// NotEnoughDataException is thrown.
            /// </summary>
            /// <returns>Average numebr of goals scored by awayteam in season
            /// of match toPredict.</returns>
            /// <exception cref="NotEnoughDataException"></exception>
            public double GetSeasonAwayGoalAverage()
            {
                if (awayAvg == -1)
                    throw new NotEnoughDataException();
                return awayAvg;
            }
        }
    }
}
