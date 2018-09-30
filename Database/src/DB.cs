using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace Database
{
    public class DB
    {
        string connectionString = "";

        /// <summary>
        /// Constructor for DB. Creates a connection string based on filePath.
        /// </summary>
        /// <param name="filePath"></param>
        public DB(string filePath)
        {
            connectionString = String.Format("Data Source = {0}; Version = 3; foreign keys=true; FailIfMissing=True", filePath);
        }

        public void CreateDatabase(string filepath)
        {
            SQLiteConnection.CreateFile(filepath);
        }

        /// <summary>
        /// Executes an SQLite statement.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>1 on success</returns>
        /// <exception cref="SQLiteException">Thrown if statement fails.</exception>
        public int ExecuteScript(string path)
        {
            string sql = File.ReadAllText(path);
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand command = new SQLiteCommand(sql, con);
                command.ExecuteNonQuery();
            }
            finally {
                con.Close();
            }
            return 1;
        }

        /// <summary>
        /// Empties matches-table.
        /// </summary>
        /// <returns></returns>
        public void ClearDatabase()
        {
            string query = "DELETE FROM matches;";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.ExecuteNonQuery();
            con.Close();
        }

        /// <summary>Deletes a file given as the parameter.</summary>
        /// <param name="path">path to file to be deleted</param>
        /// <returns>-1 if file wasn't deleted, 1 on success.</returns>
        public int DeleteDatabase(string path)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (File.Exists(path))
                File.Delete(path);
            else
                return -1;

            return 1;
        }

        /// <summary>
        /// Adds matches to the table matches. 
        /// </summary>
        /// <param name="matches">List of matches to be added.</param>
        /// <param name="connectionString">Connectionstring to use for the database connection.</param>
        /// <returns>Number of rows added.</returns>
        /// <exception cref="SQLiteException">Thrown if connection to database fails.</exception>
        /// <exception cref="ArgumentException">Thrown if any match contains
        /// invalid season parameter. </exception>
        public int AddMatches(List<Match> matches)
        {
            int addedMatches = 0;
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            using (var transaction = con.BeginTransaction())
            {
                foreach (Match m in matches)
                {
                    if (m.Season.Length != 9 || !m.Season[4].Equals('-'))
                        throw new ArgumentException("Invalid season format");

                    try
                    {
                            //Add your query here.
                        cmd.CommandText = "INSERT INTO matches VALUES (@playedDate, @hometeam, @awayteam, @league, @season, @homescore, @awayscore, @homeOdd, @drawOdd, @awayOdd);";
                        cmd.Parameters.AddWithValue(@"playedDate", m.Date);
                        cmd.Parameters.AddWithValue(@"hometeam", m.Hometeam);
                        cmd.Parameters.AddWithValue(@"awayteam", m.Awayteam);
                        cmd.Parameters.AddWithValue(@"league", m.League);
                        cmd.Parameters.AddWithValue(@"season", m.Season);
                        cmd.Parameters.AddWithValue(@"homescore", m.Homescore);
                        cmd.Parameters.AddWithValue(@"awayscore", m.Awayscore);
                        cmd.Parameters.AddWithValue(@"homeOdd", m.HomeOdd);
                        cmd.Parameters.AddWithValue(@"drawOdd", m.DrawOdd);
                        cmd.Parameters.AddWithValue(@"awayOdd", m.AwayOdd);
                        addedMatches += cmd.ExecuteNonQuery();
                    }
                     catch (SQLiteException)
                     {
                         continue;
                     }
                }
                transaction.Commit();
            }
            con.Close();
            return addedMatches;
        }

        /// <summary>
        /// Function selects N last home or away matches from team before date.
        /// If not enough homeaway matches are found, all matches are searched. If there
        /// still arent enough matches, throws NotEnoughDataException.
        /// </summary>
        /// <returns>List of matches returned by the statement.</returns>
        /// <exception cref="NotEnoughDataException"></exception>
        public List<Match> SelectNLastFromTeam(bool searchForHomeMatches, int n, DateTime date, string team)
        {
            string query = "";
            List<Match> matches = new List<Match>();

            if (searchForHomeMatches)
                query = "SELECT * FROM matches WHERE (hometeam=@team AND date(playedDate) < date(@date)) ORDER BY date(playedDate) DESC LIMIT @limit;";
            else
                query = "SELECT * FROM matches WHERE (awayteam=@team AND date(playedDate) < date(@date)) ORDER BY date(playedDate) DESC LIMIT @limit;";

            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("team", team);
            command.Parameters.AddWithValue("date", date.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("limit", n);
            SQLiteDataReader reader = command.ExecuteReader();
            matches = ParseMatches(reader);
            con.Close();

            if (matches.Count >= n)
            {
                return matches;
            }

            query = "SELECT * FROM matches WHERE (hometeam=@team OR awayteam=@team) AND date(playedDate) < date(@date) ORDER BY date(playedDate) DESC LIMIT @limit;";
            con = new SQLiteConnection(connectionString);
            con.Open();
            command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("team", team);
            command.Parameters.AddWithValue("date", date);
            command.Parameters.AddWithValue("limit", n);
            reader = command.ExecuteReader();
            matches = ParseMatches(reader);
            con.Close();

            if (matches.Count < n)
                throw new NotEnoughDataException("Too few matches returned by query");
            return matches;
        }

        /// <summary>
        /// Returns a match in selected row. Throws IndexOutOfRangeException
        /// if row searched is less than 0 or at least match-count.
        /// Indexing starts from 0.
        /// </summary>
        /// <param name="rowNumber">Row to be returned</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Match SelectNthRow(int rowNumber)
        {
            if (rowNumber < 0)
                throw new IndexOutOfRangeException();
            string query = "SELECT * FROM matches LIMIT 1 OFFSET @rowNumber;";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("rowNumber", rowNumber);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Match> matches = ParseMatches(reader);
            con.Close();
            if (matches.Count > 0)
                return matches[0];
            else
                throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Returns a count of matches in the database.
        /// </summary>
        /// <returns></returns>
        public int SelectCount()
        {
            int count = 0;
            string query = "SELECT COUNT(*) FROM matches;";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                count = reader.GetInt32(0);
            }
            con.Close();
            return count;
        }

        /// <summary>
        /// Returns an average number of goals 
        /// scored by hometeam,in selected league and season before date d.
        /// </summary>
        /// <param name="d"></param>
        /// <exception cref="NotEnoughDataException">Thrown if no rows where returned.</exception>
        public double LeagueHomeAVGBeforeDate(DateTime d, string season, string league)
        {
            double result = 0;
            string query = "SELECT CASE WHEN COUNT(homescore) > 0 THEN AVG(homescore) ELSE -1 END AS REAL FROM matches WHERE (season=@season AND league=@league AND date(playedDate) < date(@date))";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("season", season);
            command.Parameters.AddWithValue("league", league);
            command.Parameters.AddWithValue("date", d.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetDouble(0);
            }
            con.Close();

            if (result == -1)
                throw new NotEnoughDataException("Average could not be counted, no rows returned");

            return result;
        }

        /// <summary>
        /// Returns an average number of goals 
        /// scored by awayteam,in selected league and season before date d.
        /// </summary>
        /// <param name="d"></param>
        /// <exception cref="NotEnoughDataException">Thrown if no rows where returned.</exception>
        public double LeagueAwayAVGBeforeDate(DateTime d, string season, string league)
        {
            double result = 0;
            string query = "SELECT CASE WHEN COUNT(homescore) > 0 THEN AVG(awayscore) ELSE -1 END AS REAL FROM matches WHERE (season=@season AND league=@league AND date(playedDate) < date(@date))";
            SQLiteConnection con = new SQLiteConnection(connectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            command.Parameters.AddWithValue("season", season);
            command.Parameters.AddWithValue("league", league);
            command.Parameters.AddWithValue("date", d.ToString("yyyy-MM-dd"));
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetDouble(0);
            }
            con.Close();

            if (result == -1)
                throw new NotEnoughDataException("Average could not be counted, no rows returned");
            return result;
        }

        /// <summary>
        /// Returns a list of Match-objects parsed from SQLiteDataReader.
        /// </summary>
        private List<Match> ParseMatches(SQLiteDataReader reader)
        {
            List<Match> matches = new List<Match>();
            while (reader.Read())
            {
                Match m = new Match(reader["hometeam"].ToString(), reader["awayteam"].ToString(),
                    reader["league"].ToString(), reader["season"].ToString(),
                    Convert.ToDateTime(reader["playedDate"]), Convert.ToInt32(reader["homescore"].ToString()), 
                    Convert.ToInt32(reader["awayscore"].ToString()), Convert.ToDouble(reader["homeOdd"].ToString()), 
                    Convert.ToDouble(reader["drawOdd"].ToString()), Convert.ToDouble(reader["awayOdd"].ToString()));

                matches.Add(m);
            }
            return matches;
        }
    }
}
