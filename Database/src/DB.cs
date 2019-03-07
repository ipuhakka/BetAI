using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Globalization;

namespace Database
{
    public class DB
    {
        string ConnectionString { get; }

        /// <summary>
        /// Constructor for DB. Creates a connection string based on filePath.
        /// </summary>
        /// <param name="filePath"></param>
        public DB(string filePath)
        {
            ConnectionString = String.Format("Data Source = {0}; Version = 3; foreign keys=true; FailIfMissing=True;", filePath);
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
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
            string query = "DELETE FROM matches; Delete From Bet_Wager; DELETE FROM AI_Wager;" +
                "Delete FROM AI_Bet;";
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
        /// Selects all matches from database.
        /// </summary>
        /// <returns>List of match objects</returns>
        /// <exception cref="SQLiteException">Thrown if connection to database fails.</exception>
        public List<Match> SelectAllMatchesFromDatabase()
        {
            string query = "SELECT * FROM matches;";

            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();     
            
            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();
            List<Match> matches = ParseMatches(reader);

            con.Close();

            return matches;
        }

        /// <summary>
        /// Returns a list of all wagers from specific author.
        /// </summary>
        public List<Wager> GetWagersFromAuthor(string author)
        {
            var wagers = new List<Wager>();
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
                    List<Match> matches = GetMatchesFromWager(id);

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
        /// Updates AI_Wagers. Finds all matches from Bet_Wager in which
        /// the bet is not resolved, checks for these matches from matches-table,
        /// and updates the outcome for matches which it finds.
        /// </summary>
        /// <returns>number of updated wagers.</returns>
        public int UpdateWagers()
        {
            var affectedRows = 0;
            List<Tuple<int, int>> wagerTuples = new List<Tuple<int, int>>(); //Item1: wagerId, Item2: wagerResult
            List<Match> matches = GetUnresolvedBets();
            UpdateBets(matches);

            //Find wagers where wager has result of 0, but all bets have result of 1 or -1.
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                cmd.CommandText = "SELECT DISTINCT wagerId FROM Bet_Wager" +
                    " INNER JOIN AI_Wager w on wagerId=w.id " +
                    "WHERE w.result = 0";

                var reader = cmd.ExecuteReader();

                while (reader.Read()) //get each bet, if all results 1, set result as 1, if even one is -1, set as -1.
                {
                    using (var command = new SQLiteCommand(con))
                    {
                        var wagerResult = 0;
                        var wagerId = reader.GetInt32(0);
                        var betResults = new List<int>();
                        command.CommandText = "SELECT b.result FROM Bet_Wager bw" +
                                " INNER JOIN AI_Bet b ON bw.matchDate = b.matchDate" +
                                " AND bw.hometeam = b.hometeam AND bw.awayteam = b.awayteam " +
                                " WHERE wagerId=@wagerId";
                        command.Parameters.AddWithValue("wagerId", wagerId);

                        var betReader = command.ExecuteReader();
                        while (betReader.Read())
                        {
                            betResults.Add(betReader.GetInt32(0));
                        }

                        if (betResults.Any(i => i == -1))
                        {
                            wagerResult = -1;
                        }
                        else if (betResults.All(i => i == 1))
                        {
                            wagerResult = 1;
                        }

                        if (wagerResult != 0)
                        {
                            wagerTuples.Add(new Tuple<int, int>(wagerId, wagerResult));
                        }
                    }
                }
            }
            con.Close();

            wagerTuples.ForEach(wager => 
            {
                affectedRows += UpdateWager(wager.Item1, wager.Item2);
            });

            return affectedRows;
        }
        
        /// <summary>
        /// Adds wagers. Creates bets to database for each match and wager
        /// for each wager, and adds a row to junction table for each
        /// bet in wager. Returns number
        /// of wagers added to database.
        /// </summary>
        public int AddWagers(List<Wager> wagers, string author)
        {
            int addedRows = 0;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                wagers.ForEach(wager => 
                {
                    if (!WagerExists(wager))
                    {
                        cmd.CommandText = "INSERT INTO AI_Wager " +
                        "(playedDate, result, bet, odd, author)" +
                        " VALUES (@playedDate, 0, @bet, @odd, @author)";
                        cmd.Parameters.AddWithValue(@"playedDate", DateTime.Today);
                        cmd.Parameters.AddWithValue(@"bet", wager.Stake);
                        cmd.Parameters.AddWithValue(@"odd", wager.Matches
                            .Select(match => match.GetWagerOdd())
                            .Aggregate((x, y) => x * y)
                            );
                        cmd.Parameters.AddWithValue(@"author", author);
                        addedRows += cmd.ExecuteNonQuery();

                        int wagerId = (int)con.LastInsertRowId;
                        wager.Matches.ForEach( match => 
                        {
                            cmd.CommandText = "INSERT INTO AI_Bet" +
                                "(matchDate, hometeam, awayteam, result, wagedResult, odd)" +
                                " VALUES (@matchDate, @hometeam, @awayteam, @result, @wagedResult, @odd)";
                            cmd.Parameters.AddWithValue(@"matchDate", match.Date);
                            cmd.Parameters.AddWithValue(@"hometeam", match.Hometeam);
                            cmd.Parameters.AddWithValue(@"awayteam", match.Awayteam);
                            cmd.Parameters.AddWithValue(@"result", 0);
                            cmd.Parameters.AddWithValue(@"wagedResult", match.SimulatedResult);
                            cmd.Parameters.AddWithValue(@"odd", match.GetWagerOdd());

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SQLiteException) { } //Row already exists
                            cmd.CommandText = "INSERT INTO Bet_Wager" +
                                " VALUES (@wagerId, @matchDate, @hometeam, @awayteam)";
                            cmd.Parameters.AddWithValue(@"wagerId", wagerId);
                            cmd.Parameters.AddWithValue(@"matchDate", match.Date);
                            cmd.Parameters.AddWithValue(@"hometeam", match.Hometeam);
                            cmd.Parameters.AddWithValue(@"awayteam", match.Awayteam);

                            cmd.ExecuteNonQuery();
                        });
                    } 
                });  
            }
            con.Close();

            return addedRows;
        }

        /// <summary>
        /// Returns true if there is a wager with same bet, odd and identical match list.
        /// </summary>
        public bool WagerExists(Wager wager)
        {
            bool exists = false;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
            bool isMatch = true;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
                      match.Hometeam == reader.GetString(0)
                      && match.Awayteam == reader.GetString(1)
                      && match.Date.Date == reader.GetDateTime(2).Date))
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

        /// <summary>
        /// Sets the result value for specific wager.
        /// Returns number of affected rows.
        /// </summary>
        private int UpdateWager(int id, int newResult)
        {
            var affectedRows = 0;
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
        /// Finds all matches from list which have a result in matches table, and
        /// updates bet results accordingly.
        /// </summary>
        private void UpdateBets(List<Match> matches)
        {
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
            int result = homescore - awayscore;
            char correctResult = 'X';

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

            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
        /// Returns a list of matches which have not resolved (result == 0).
        /// </summary>
        /// <returns></returns>
        private List<Match> GetUnresolvedBets()
        {
            List<Match> matches = new List<Match>();
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
        /// Returns all matches which are linked to a specific wager.
        /// </summary>
        private List<Match> GetMatchesFromWager(int wagerId)
        {
            var matches = new List<Match>();
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
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
                    matches.Add(new Match(reader["hometeam"].ToString(),
                        reader["awayteam"].ToString(),
                        Convert.ToDateTime(reader["matchDate"].ToString()),
                        (char)Convert.ToInt32(reader["wagedResult"].ToString()),
                        Convert.ToInt32(reader["result"].ToString())));
                }
            }
            con.Close();
            return matches;
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

        /// <summary>
        /// Parses Match-objects from AI_Bet table.
        /// </summary>
        private List<Match> ParseMatchKeys(SQLiteDataReader reader)
        {
            List<Match> matches = new List<Match>();

            while (reader.Read())
            {
                Match m = new Match(reader["hometeam"].ToString(), reader["awayteam"].ToString(),
                    Convert.ToDateTime(reader["matchDate"]), (char)Convert.ToInt32(reader["wagedResult"]));

                matches.Add(m);
            }

            return matches;
        }
    }
}
