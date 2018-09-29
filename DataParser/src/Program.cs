using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.IO;
using Database;

namespace DataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            FileLoader fl = new FileLoader();
            CSVParser csv = new CSVParser();
            string address = "";
            string database = "";
            string tempFile = "tempfile.csv";

            try
            {
                database = args[0].Split('=')[1];
                address = args[1].Split('=')[1];
                string season = args[2].Split('=')[1];
                string league = args[3].Split('=')[1];
                List<string> list = args.ToList();
                list.RemoveRange(0, 4);
                string[] optionalParams = list.ToArray();

                Console.WriteLine("Loading file from " + address);
                fl.LoadFile(address, tempFile);

                Console.WriteLine("Parsing csv data");
                List<Match> matches = csv.Parse(tempFile, season, league, optionalParams);
                DB db = new DB(database);
                db.AddLeague(league);
                db.AddSeason(season);
                int addedRows = db.AddMatches(matches);

                Console.WriteLine(addedRows + " added to database at " + database);
                File.Delete(tempFile);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid season parameter");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Failure in parsing necessary arguments");
                return;
            }
            catch (LoadException)
            {
                Console.WriteLine("Failure in downloading file from address " + address);
                return;
            }
            catch (SQLiteException)
            {
                Console.WriteLine("Connection to the database at " + database + " failed");
            }
            
        }
    }
}
