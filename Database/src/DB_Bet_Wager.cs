using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace Database
{
    public class DB_Bet_Wager
    {
        private string ConnectionString { get; }
        private DB_AI_Bet AI_Bet { get; }
        private DB_AI_Wager AI_Wager { get; }

        public DB_Bet_Wager(string connectionString)
        {
            ConnectionString = connectionString;
            AI_Bet = new DB_AI_Bet(connectionString);
            AI_Wager = new DB_AI_Wager(connectionString);
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
            var wagerTuples = new List<Tuple<int, int>>(); //Item1: wagerId, Item2: wagerResult
            var matches = AI_Bet.GetUnresolvedBets();
            AI_Bet.UpdateBets(matches);

            //Find wagers where wager has result of 0, but all bets have result of 1 or -1.
            var con = new SQLiteConnection(ConnectionString);
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
                affectedRows += AI_Wager.UpdateWager(wager.Item1, wager.Item2);
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
            var addedRows = 0;
            var con = new SQLiteConnection(ConnectionString);
            con.Open();

            using (var cmd = new SQLiteCommand(con))
            {
                wagers.ForEach(wager =>
                {
                    if (!AI_Wager.WagerExists(wager))
                    {
                        cmd.CommandText = "INSERT INTO AI_Wager " +
                        "(playedDate, result, bet, odd, author)" +
                        " VALUES (@playedDate, 0, @bet, @odd, @author)";
                        cmd.Parameters.AddWithValue(@"playedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue(@"bet", wager.Stake);
                        cmd.Parameters.AddWithValue(@"odd", wager.Matches
                            .Select(match => match.GetWagerOdd())
                            .Aggregate((x, y) => x * y)
                            );
                        cmd.Parameters.AddWithValue(@"author", author);
                        addedRows += cmd.ExecuteNonQuery();

                        var wagerId = (int)con.LastInsertRowId;
                        wager.Matches.ForEach(match =>
                        {
                            cmd.CommandText = "INSERT INTO AI_Bet" +
                                "(matchDate, hometeam, awayteam, result, wagedResult, homeOdd, drawOdd, awayOdd)" +
                                " VALUES (@matchDate, @hometeam, @awayteam, @result, @wagedResult" +
                                ", @homeOdd, @drawOdd, @awayOdd)";
                            cmd.Parameters.AddWithValue(@"matchDate", match.Date);
                            cmd.Parameters.AddWithValue(@"hometeam", match.Hometeam);
                            cmd.Parameters.AddWithValue(@"awayteam", match.Awayteam);
                            cmd.Parameters.AddWithValue(@"result", 0);
                            cmd.Parameters.AddWithValue(@"wagedResult", match.SimulatedResult.ToString());
                            cmd.Parameters.AddWithValue(@"homeOdd", match.HomeOdd);
                            cmd.Parameters.AddWithValue(@"drawOdd", match.DrawOdd);
                            cmd.Parameters.AddWithValue(@"awayOdd", match.AwayOdd);

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (SQLiteException e)
                            {
                                System.Diagnostics.Debug.WriteLine($"Exception: {e.Message}");
                            } //Row already exists
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
    }
}
