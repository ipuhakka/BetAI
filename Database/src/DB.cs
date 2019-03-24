using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

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
            var sql = File.ReadAllText(path);
            var con = new SQLiteConnection(ConnectionString);

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
            return new DB_Matches(ConnectionString).AddMatches(matches);
        }

        /// <summary>
        /// Selects all matches from database.
        /// </summary>
        /// <returns>List of match objects</returns>
        /// <exception cref="SQLiteException">Thrown if connection to database fails.</exception>
        public List<Match> SelectAllMatchesFromDatabase()
        {
            return new DB_Matches(ConnectionString).SelectAllMatchesFromDatabase();
        }

        /// <summary>
        /// Returns a list of all wagers from specific author.
        /// </summary>
        public List<Wager> GetWagersFromAuthor(string author)
        {
            return new DB_AI_Wager(ConnectionString).GetWagersFromAuthor(author);
        }

        /// <summary>
        /// Updates AI_Wagers. Finds all matches from Bet_Wager in which
        /// the bet is not resolved, checks for these matches from matches-table,
        /// and updates the outcome for matches which it finds.
        /// </summary>
        /// <returns>number of updated wagers.</returns>
        public int UpdateWagers()
        {
            return new DB_Bet_Wager(ConnectionString).UpdateWagers();
        }
        
        /// <summary>
        /// Adds wagers. Creates bets to database for each match and wager
        /// for each wager, and adds a row to junction table for each
        /// bet in wager. Returns number
        /// of wagers added to database.
        /// </summary>
        public int AddWagers(List<Wager> wagers, string author)
        {
            return new DB_Bet_Wager(ConnectionString).AddWagers(wagers, author);
        }
    }
}
