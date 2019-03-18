using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using System.Data.SQLite;
using BetAI.Exceptions;

namespace BetAI.BetSim
{
    public class Matches
    {
        private static List<Match> Match { get; set; }

        //MatchesData is a dictionary where key is the combination of teamnames and matchdate.
        private static Dictionary<string, MatchData> MatchesData { get; set; }

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
            var db = new DB(path);
            Match = db.SelectAllMatchesFromDatabase();
        }

        /// <summary>
        /// Function creates a dictionary of MatchData-structs for each match in sample.
        /// </summary>
        /// <param name="sample">List of match objects</param>
        /// <param name="maxSampleSize">Sample size which is used
        /// for getting n previous matches for home and awayteam.</param>
        public static void CreateMatchDataStructs(List<Match> sample, int maxSampleSize)
        {
            MatchesData = new Dictionary<string, MatchData>();

            for (int i = 0; i < sample.Count; i++)
            {
                var homeAvg = SeasonHomeGoalAvgBeforeDate(sample[i]);
                var awayAvg = SeasonAwayGoalAvgBeforeDate(sample[i]);
                var hometeamPrevious = SelectNLastFromTeam(
                    true, 
                    maxSampleSize, 
                    sample[i].Date, 
                    sample[i].Hometeam)
                    .ToArray();
                var awayteamPrevious = SelectNLastFromTeam(
                    false, 
                    maxSampleSize, 
                    sample[i].Date, 
                    sample[i].Awayteam)
                    .ToArray();

                MatchesData.Add(
                    $"{sample[i].Hometeam}-{sample[i].Awayteam}-{sample[i].Date}", 
                    new MatchData(homeAvg, awayAvg, hometeamPrevious, awayteamPrevious, 
                        sample[i].Hometeam, sample[i].Awayteam));
            }
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
                return MatchesData[$"{toPredict.Hometeam}-{toPredict.Awayteam}-{toPredict.Date}"]
                    .GetNLastFromSampleFromHometeam(sampleSize);
            else
                return MatchesData[$"{toPredict.Hometeam}-{toPredict.Awayteam}-{toPredict.Date}"]
                    .GetNLastFromSampleFromAwayteam(sampleSize);
        }

        /// <summary>
        /// Returns the average number of goals scored by a team in home- or
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
                return MatchesData[$"{toPredict.Hometeam}-{toPredict.Awayteam}-{toPredict.Date}"]
                    .GetSeasonHomeGoalAverage();
            else
                return MatchesData[$"{toPredict.Hometeam}-{toPredict.Awayteam}-{toPredict.Date}"]
                    .GetSeasonAwayGoalAverage();
        }

        /// <summary>
        /// Returns league for a team, null if team name is not found.
        /// </summary>
        public static string GetLeagueForTeam(string team)
        {
            return Match.FirstOrDefault(m => m.Hometeam == team || m.Awayteam == team)
                ?.League ?? null;
        }

        /// <summary>
        /// Returns number of loaded matches.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public static int GetMatchCount()
        {
            return Match.Count;
        }

        /// <summary>
        /// Returns true if Match-list contains both home and away teams.
        /// </summary>
        /// <param name="home"></param>
        /// <param name="away"></param>
        /// <returns></returns>
        public static bool TeamsExist(string home, string away)
        {
            return
                Match
                    .Any(m => m.Hometeam == home ||
                                m.Awayteam == home) &&
                Match
                    .Any(m => m.Hometeam == away || m.Awayteam == away);
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
            var matchSample = new List<Match>();

            foreach (int index in indexes)
            {
                if (index < 0 || index >= Match.Count)
                    throw new IndexOutOfRangeException();
                matchSample.Add(Match[index]);
            }

            return matchSample;
        }

        /// <summary>
        /// Function initializes MatchData averages by returning number of goals 
        /// scored by hometeam,in selected league and season before date d.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if MatchData structures
        /// have not been created using CreateMatchDataStructs-function.</exception>
        /// <returns>Average goals scored by hometeam up to Match m, -1 if
        /// no matches had been played before Match m.</returns>
        private static double SeasonHomeGoalAvgBeforeDate(Match m)
        {
            var seasonMatchesBeforeDate = Match
                .Where(x => x.League == m.League && x.Season == m.Season && x.Date < m.Date)
                .ToList();

            if (seasonMatchesBeforeDate.Count == 0)
                return -1;

            return seasonMatchesBeforeDate.Average(match => match.Homescore);
        }

        /// <summary>
        /// Function initializes MatchData averages by returning an average number of goals 
        /// scored by awayteam,in selected league and season before date d.
        /// </summary>
        /// <param name="d"></param>
        /// <returns>Average goals scored by hometeam up to Match m, -1 if
        /// no matches had been played before Match m.</returns>
        /// <exception cref="ArgumentNullException">Thrown if MatchData structures
        /// have not been created using CreateMatchDataStructs-function.</exception>
        private static double SeasonAwayGoalAvgBeforeDate(Match m)
        {
            var seasonMatchesBeforeDate = Match
                .Where(x => x.League == m.League && x.Season == m.Season && x.Date < m.Date)
                .ToList();

            if (seasonMatchesBeforeDate.Count == 0)
                return -1;

            return seasonMatchesBeforeDate.Average(match => match.Awayscore);
        }

        /// <summary>
        /// Function selects N last home or away matches from team before date,
        /// to initialize MatchData-dictionary.
        /// If not enough homeaway matches are found, both home/away matches are searched,
        /// and returned.
        /// </summary>
        /// <returns>Match-array the size of n or of match count prior to beforeDate.</returns>
        private static Match[] SelectNLastFromTeam(bool searchHomeMatches, int n, DateTime beforeDate, string teamname)
        {
            var nLast = new Match[n];

            if (searchHomeMatches)
            {
                nLast = Match
                    .Where(match => match.Date < beforeDate && match.Hometeam == teamname)
                    .OrderByDescending(m => m.Date)
                    .Take(n)
                    .ToArray();
            }
            else
            {
                nLast = Match
                    .Where(match => match.Date < beforeDate && match.Awayteam == teamname)
                    .OrderByDescending(m => m.Date)
                    .Take(n)
                    .ToArray();
            }

            if (nLast.Length < n)
            {
                return Match
                    .Where(match => 
                        match.Date < beforeDate && (match.Hometeam == teamname || match.Awayteam == teamname))
                    .OrderByDescending(m => m.Date)
                    .Take(n)
                    .ToArray();
            }
          
            return nLast;
        }

        /// <summary>
        /// Structure contains an array of previous matches for hometeam, awayteam,
        /// and average number of goals scored that season in league both home
        /// and away.
        /// </summary>
        protected struct MatchData
        {
            string Hometeam { get; }
            string Awayteam { get; }
            double HomeAvg { get; }
            double AwayAvg { get; }
            Match[] hometeamPreviousMatches;
            Match[] awayteamPreviousMatches;

            public MatchData(double homeA, double awayA, Match[] hometeamPM
                , Match[] awayteamPM, string hometeam, string awayteam)
            {
                HomeAvg = homeA;
                AwayAvg = awayA;
                Hometeam = hometeam;
                Awayteam = awayteam;
                hometeamPreviousMatches = hometeamPM;
                awayteamPreviousMatches = awayteamPM;
            }

            /// <summary>
            /// Returns an array of last matches from team.
            /// If team has n previous homematches in the list, these 
            /// are returned. Otherwise n-last total matches for team are
            /// returned.
            /// If n is larger than array of previous matches, 
            /// null is returned to mark that there are not enough
            /// matches to simulate a match.
            /// </summary>
            /// <param name="n">Number of matches that are looked for.</param>
            /// <returns>n-sized array of teams previous matches.</returns>
            public Match[] GetNLastFromSampleFromHometeam(int n)
            {
                if (n > hometeamPreviousMatches.Length)
                    return null;

                var hometeam = Hometeam;
                var homeMatches = hometeamPreviousMatches
                    .Where(match => match.Hometeam.Equals(hometeam))
                    .OrderByDescending(match => match.Date)
                    .ToArray();

                if (homeMatches.Length == n)
                    return homeMatches;

                return hometeamPreviousMatches
                    .OrderByDescending(match => match.Date)
                    .Take(n)
                    .ToArray(); 
            }

            /// <summary>
            /// Returns an array of last matches from team.
            /// If team has n previous awaymatches in the list, these 
            /// are returned. Otherwise n-last total matches for team are
            /// returned.
            /// If n is larger than array of previous matches, 
            /// null is returned to mark that there are not enough
            /// matches to simulate a match.
            /// </summary>
            /// <param name="n">Number of matches that are looked for.</param>
            /// <returns>n-sized array of teams previous matches.</returns>
            public Match[] GetNLastFromSampleFromAwayteam(int n)
            {
                if (n > awayteamPreviousMatches.Length)
                    return null;

                var awayteam = Awayteam;
                var awayMatches = awayteamPreviousMatches
                    .Where(match => 
                        match.Awayteam.Equals(awayteam))
                    .OrderByDescending(match => match.Date)
                    .ToArray();

                if (awayMatches.Length == n)
                    return awayMatches;

                return awayteamPreviousMatches
                    .OrderByDescending(match => match.Date)
                    .Take(n)
                    .ToArray();
            }

            /// <summary>
            /// Returns homeAvg. If homeAvg is -1,
            /// there are no matches played before, and
            /// match should not be simulated.
            /// </summary>
            /// <returns>Average numebr of goals scored by hometeam in season
            /// of match toPredict.</returns>
            public double GetSeasonHomeGoalAverage()
            {
                return HomeAvg;
            }

            /// <summary>
            /// Returns awayAvg. If awayAvg is -1,
            /// there are no matches played before, and
            /// match should not be simulated.
            /// </summary>
            /// <returns>Average numebr of goals scored by awayteam in season
            /// of match toPredict.</returns>
            public double GetSeasonAwayGoalAverage()
            {
                return AwayAvg;
            }
        }
    }
}
