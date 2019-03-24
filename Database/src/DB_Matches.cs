using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Database
{
    public class DB_Matches
    {
        private string ConnectionString { get; }

        public DB_Matches(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Selects all matches from database.
        /// </summary>
        /// <returns>List of match objects</returns>
        /// <exception cref="SQLiteException">Thrown if connection to database fails.</exception>
        public List<Match> SelectAllMatchesFromDatabase()
        {
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            var command = new SQLiteCommand("SELECT * FROM matches;", con);
            var reader = command.ExecuteReader();
            var matches = ParseMatches(reader);

            con.Close();

            return matches;
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
            var addedMatches = 0;
            var con = new SQLiteConnection(ConnectionString);
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
        /// Returns a list of Match-objects parsed from SQLiteDataReader.
        /// </summary>
        private List<Match> ParseMatches(SQLiteDataReader reader)
        {
            var matches = new List<Match>();

            while (reader.Read())
            {
                matches.Add(new Match(reader["hometeam"].ToString(), reader["awayteam"].ToString(),
                    reader["league"].ToString(), reader["season"].ToString(),
                    Convert.ToDateTime(reader["playedDate"]), Convert.ToInt32(reader["homescore"].ToString()),
                    Convert.ToInt32(reader["awayscore"].ToString()), Convert.ToDouble(reader["homeOdd"].ToString()),
                    Convert.ToDouble(reader["drawOdd"].ToString()), Convert.ToDouble(reader["awayOdd"].ToString())));
            }

            return matches;
        }
    }
}
