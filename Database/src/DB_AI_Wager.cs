using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace Database
{
    public class DB_AI_Wager
    {
        private string ConnectionString { get; }
        private DB_AI_Bet AI_Bet { get; }

        public DB_AI_Wager(string connectionString)
        {
            ConnectionString = connectionString;
            AI_Bet = new DB_AI_Bet(connectionString);
        }

        /// <summary>
        /// Returns a list of all wagers from specific author.
        /// </summary>
        public List<Wager> GetWagersFromAuthor(string author)
        {
            var wagers = new List<Wager>();
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "SELECT * FROM AI_Wager " +
                    "WHERE Author = @author";
                cmd.Parameters.AddWithValue("author", author);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var id = Convert.ToInt32(reader["id"].ToString());
                    var matches = AI_Bet.GetMatchesFromWager(id);

                    wagers.Add(new Wager(Convert.ToDouble(reader["bet"].ToString()),
                        Convert.ToDouble(reader["odd"].ToString()),
                        reader["author"].ToString(), Convert.ToInt32(reader["result"].ToString()),
                        Convert.ToDateTime(reader["playedDate"]), matches));
                }
            }
            con.Close();

            return wagers;
        }

        /// <summary>
        /// Sets the result value for specific wager.
        /// Returns number of affected rows.
        /// </summary>
        public int UpdateWager(int id, int newResult)
        {
            var affectedRows = 0;
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "UPDATE AI_Wager SET result=@result " +
                                    " WHERE id=@id";
                cmd.Parameters.AddWithValue("result", newResult);
                cmd.Parameters.AddWithValue("id", id);
                affectedRows = cmd.ExecuteNonQuery();
            }
            con.Close();

            return affectedRows;
        }


        /// <summary>
        /// Returns true if there is a wager with same bet, odd and identical match list.
        /// </summary>
        public bool WagerExists(Wager wager)
        {
            var exists = false;
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "SELECT id " +
                     "FROM AI_Wager " +
                     "WHERE bet=@bet AND odd=@odd";
                cmd.Parameters.AddWithValue("bet", wager.Stake);
                cmd.Parameters.AddWithValue(@"odd", wager.Matches
                    .Select(match => match.GetWagerOdd())
                    .Aggregate((x, y) => x * y));

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (MatchListsIdentical(wager, reader.GetInt32(0)))
                    {
                        exists = true;
                        cmd.Cancel();
                        reader.Close();
                        break;
                    }
                }
            }
            con.Close();

            return exists;
        }

        /// <summary>
        /// Checks if Wager contains all same matches as another wager. By definition,
        /// Wager should not contain same match more than once,
        /// so if comparable wagers differ on any match, they are not the same.
        /// </summary>
        private bool MatchListsIdentical(Wager wager, int wagerId)
        {
            var isMatch = true;
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "SELECT hometeam, awayteam, matchDate" +
                     " FROM Bet_Wager" +
                     " WHERE wagerId=@wagerId";
                cmd.Parameters.AddWithValue(@"wagerId", wagerId);

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (!wager.Matches.Any(match =>
                      match.Hometeam == reader.GetString(0) &&
                      match.Awayteam == reader.GetString(1) &&
                      match.Date.Date == reader.GetDateTime(2).Date))
                    {
                        cmd.Cancel();
                        reader.Close();
                        isMatch = false;
                        break;
                    }
                }
            }
            con.Close();
            return isMatch;
        }
    }
}
