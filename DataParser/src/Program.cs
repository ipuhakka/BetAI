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
            var fl = new FileLoader();
            var csv = new CSVParser();
            var address = "";
            var database = "";
            var tempFile = "tempfile.csv";

            try
            {
                database = args[0].Split('=')[1];
                address = args[1].Split('=')[1];
                var season = args[2].Split('=')[1];
                var league = args[3].Split('=')[1];
                var argumentList = args.ToList();
                argumentList.RemoveRange(0, 4);
                var optionalParams = argumentList.ToArray();

                Console.WriteLine("Loading file from " + address);
                fl.LoadFile(address, tempFile);

                Console.WriteLine("Parsing csv data");
                var matches = csv.Parse(tempFile, season, league, optionalParams);

                var db = new DB(database);
                var addedRows = db.AddMatches(matches);

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
