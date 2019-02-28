﻿using System;
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
            string query = "DELETE FROM matches; DELETE FROM LOG_TABLE; DELETE FROM AI_Wager;" +
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
        /// Updates firstNotUpdatedValue in Log_Table.
        /// Returns the number of affected rows.
        /// </summary>
        public int UpdateFirstNotUpdated(DateTime firstNotUpdated)
        {
            
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(con);
            command.CommandText =  $"UPDATE Log_Table SET firstNotUpdatedDate=@update";
            command.Parameters.AddWithValue(@"update", firstNotUpdated.ToString("yyyy-MM-dd"));
            var affectedRows = command.ExecuteNonQuery();
            con.Close();
            return affectedRows;
        }

        /// <summary>
        /// Returns first firstNotUpdated from Log_Table.
        /// </summary>
        /// <returns></returns>
        public DateTime GetFirstNotUpdated()
        {
            string query = "SELECT * FROM Log_Table;";
            SQLiteConnection con = new SQLiteConnection(ConnectionString);
            con.Open();
            SQLiteCommand command = new SQLiteCommand(query, con);
            SQLiteDataReader reader = command.ExecuteReader();
            List<string> updateList = new List<string>();
            while (reader.Read())
            {
                updateList.Add(reader["firstNotUpdatedDate"].ToString());
            }
            con.Close();
            Console.WriteLine("Trying to parse " + updateList.First());
            return DateTime.ParseExact(updateList.FirstOrDefault().ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
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
