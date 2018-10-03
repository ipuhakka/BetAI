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
        /// Returns number of loaded matches.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public static int GetMatchCount()
        {
            return matches.Count;
        }


        /// <summary>
        /// Returns an average number of goals 
        /// scored by hometeam,in selected league and season before date d.
        /// </summary>
        /// <param name="d"></param>
        /// <exception cref="NotEnoughDataException">Thrown if no rows where returned.</exception>
        public static double SeasonHomeGoalAvgBeforeDate(Match m)
        {
            List<Match> seasonMatchesBeforeDate = matches.Where(x => x.Season == m.Season && x.Date < m.Date && x.League == m.League).ToList();
            if (seasonMatchesBeforeDate.Count == 0)
                throw new NotEnoughDataException();
            return seasonMatchesBeforeDate.Average(match => match.Homescore);
        }

        /// <summary>
        /// Returns an average number of goals 
        /// scored by awayteam,in selected league and season before date d.
        /// </summary>
        /// <param name="d"></param>
        /// <exception cref="NotEnoughDataException">Thrown if no rows where returned.</exception>
        public static double SeasonAwayGoalAvgBeforeDate(Match m)
        {
            List<Match> seasonMatchesBeforeDate = matches.Where(x => x.Season == m.Season && x.Date < m.Date && x.League == m.League).ToList();
            if (seasonMatchesBeforeDate.Count == 0)
                throw new NotEnoughDataException();
            return seasonMatchesBeforeDate.Average(match => match.Awayscore);
        }

        /// <summary>
        /// Function selects N last home or away matches from team before date.
        /// If not enough homeaway matches are found, all matches are searched. If there
        /// still arent enough matches, throws NotEnoughDataException.
        /// </summary>
        /// <returns>List of matches returned by the statement.</returns>
        /// <exception cref="NotEnoughDataException"></exception>
        public static List<Match> SelectNLastFromTeam(bool searchHomeMatches, int n, DateTime beforeDate, string teamname)
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

            if (nLast.Count < n)
                throw new NotEnoughDataException();
            else
                return nLast.OrderByDescending(m => m.Date).Take(n).ToList();
        }

    }
}
