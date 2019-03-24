using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Database
{
    public class DB_AI_Bet
    {
        private string ConnectionString { get; }

        public DB_AI_Bet(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Returns all matches which are linked to a specific wager.
        /// </summary>
        public List<Match> GetMatchesFromWager(int wagerId)
        {
            var matches = new List<Match>();
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "SELECT * " +
                     "FROM AI_Bet b" +
                     " INNER JOIN Bet_Wager bw ON  b.hometeam=bw.hometeam AND" +
                     " bw.awayteam=b.awayteam AND b.matchDate=bw.matchDate " +
                     "WHERE wagerId=@wagerId";
                cmd.Parameters.AddWithValue(@"wagerId", wagerId);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    matches.Add(new Match(
                        reader["hometeam"].ToString(),
                        reader["awayteam"].ToString(),
                        Convert.ToDateTime(reader["matchDate"].ToString()),
                        Convert.ToChar(reader["wagedResult"].ToString()),
                        Convert.ToInt32(reader["result"].ToString()),
                        Convert.ToDouble(reader["homeOdd"].ToString()),
                        Convert.ToDouble(reader["drawOdd"].ToString()),
                        Convert.ToDouble(reader["awayOdd"].ToString())));
                }
            }
            con.Close();
            return matches;
        }

        /// <summary>
        /// Returns a list of matches which have not resolved (result == 0).
        /// </summary>
        /// <returns></returns>
        public List<Match> GetUnresolvedBets()
        {
            var matches = new List<Match>();
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "SELECT * FROM AI_Bet WHERE result=0";
                var reader = cmd.ExecuteReader();
                matches = ParseMatchKeys(reader);
            }
            con.Close();

            return matches;
        }

        /// <summary>
        /// Finds all matches from list which have a result in matches table, and
        /// updates bet results accordingly.
        /// </summary>
        public void UpdateBets(List<Match> matches)
        {
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                matches.ForEach(match =>
                {
                    cmd.CommandText = "SELECT homescore, awayscore FROM matches" +
                    " WHERE hometeam=@hometeam AND awayteam=@awayteam" +
                    " AND playedDate=@playedDate";

                    cmd.Parameters.AddWithValue(@"hometeam", match.Hometeam);
                    cmd.Parameters.AddWithValue(@"awayteam", match.Awayteam);
                    cmd.Parameters.AddWithValue(@"playedDate", match.Date.Date);

                    var scores = new List<Tuple<int, int>>();

                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        scores.Add(new Tuple<int, int>(reader.GetInt32(0), reader.GetInt32(1)));
                    }
                    reader.Close();

                    if (scores.Count > 0)
                    {
                        UpdateBet(match, scores[0].Item1, scores[0].Item2);
                    }
                });
            }
            con.Close();
        }

        /// <summary>
        /// Updates a row in AI_Bet table. 
        /// </summary>
        private void UpdateBet(Match toUpdate, int homescore, int awayscore)
        {
            var result = homescore - awayscore;
            var correctResult = 'X';

            if (result > 0)
            {
                correctResult = '1';
            }
            else if (result < 0)
            {
                correctResult = '2';
            }

            var updatedResult = -1;
            if (toUpdate.SimulatedResult == correctResult)
            {
                updatedResult = 1;
            }

            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "UPDATE AI_Bet SET" +
                " result=@updatedResult" +
                " WHERE hometeam=@hometeam AND awayteam=@awayteam AND" +
                " matchDate=@matchDate";
                cmd.Parameters.AddWithValue(@"updatedResult", updatedResult);
                cmd.Parameters.AddWithValue(@"hometeam", toUpdate.Hometeam);
                cmd.Parameters.AddWithValue(@"awayteam", toUpdate.Awayteam);
                cmd.Parameters.AddWithValue(@"matchDate", toUpdate.Date.Date);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        /// <summary>
        /// Parses Match-objects from AI_Bet table.
        /// </summary>
        private List<Match> ParseMatchKeys(SQLiteDataReader reader)
        {
            var matches = new List<Match>();

            while (reader.Read())
            {
                matches.Add(new Match(
                    reader["hometeam"].ToString(), 
                    reader["awayteam"].ToString(),
                    Convert.ToDateTime(reader["matchDate"]), 
                    Convert.ToChar(reader["wagedResult"].ToString())));
            }

            return matches;
        }
    }
}
