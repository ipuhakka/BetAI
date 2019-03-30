using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Database;
using System.Globalization;

namespace DataParser
{
    public class CSVParser
    {
        /// <summary>
        /// Creates Match-objects from inputFile. Function parses
        /// </summary>
        /// <param name="columns">User given arguments for column names.</param>
        /// <param name="inputFile">File from which data is read.</param>
        /// <param name="league">Name of the league</param>
        /// <param name="season">9 char name for season. 
        /// 'yyyy-yyyy'</param>
        /// <returns></returns>
        /// <exception cref="FormatException">Thrown if any columns any value
        /// conversion fails.</exception>
        /// <exception cref="FileNotFoundException">Thrown if inputFile is not found.</exception>
        /// <exception cref="ArgumentException">Thrown if any of the used column names,
        /// default or user inputted, are not found from the inputFile.</exception>
        public List<Match> Parse(string inputFile, string league, string season, params string[] columns)
        {
            var lines = File.ReadAllLines(inputFile);
            var searchParams = new SearchParams(columns);

            var columnIndexes = GetColumnIndexes(searchParams, lines);

            return ParseColumns(columnIndexes, lines, league, season);
        }

        /// <summary>
        /// Parses needed columns from string[] lines into a match list. 
        /// </summary>
        /// <param name="columnIndexes">Dictionary containing index for each column</param>
        /// <param name="lines"></param>
        /// <param name="league"></param>
        /// <param name="season"></param>
        /// <exception cref="FormatException">Thrown if a value conversion fail.</exception>
        private List<Match> ParseColumns(Dictionary<string, int> columnIndexes, string[] lines, string league, string season)
        {
            var matches = new List<Match>();

            for(int i = 1; i < lines.Length; i++)
            {
                string[] splitted = lines[i].Split(',');
                try 
                {
                    var date = Convert.ToDateTime(splitted[columnIndexes["Date"]]).ToString("yyyy-MM-dd");
                    var hometeam = splitted[columnIndexes["Hometeam"]];
                    var awayteam = splitted[columnIndexes["Awayteam"]];
                    var homescore = Convert.ToInt32(splitted[columnIndexes["Homescore"]]);
                    var awayscore = Convert.ToInt32(splitted[columnIndexes["Awayscore"]]);
                    var homeOdd = Convert.ToDouble(splitted[columnIndexes["HomeOdd"]], CultureInfo.InvariantCulture);
                    var drawOdd = Convert.ToDouble(splitted[columnIndexes["DrawOdd"]], CultureInfo.InvariantCulture);
                    var awayOdd = Convert.ToDouble(splitted[columnIndexes["AwayOdd"]], CultureInfo.InvariantCulture);
                    matches.Add(new Match(hometeam, awayteam, season, league, Convert.ToDateTime(date), homescore,
                        awayscore, homeOdd, drawOdd, awayOdd));
                }
                catch (FormatException)
                {
                    continue;
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
            }

            return matches;
        }

        /// <summary>
        /// Returns a list of tuples, each containing index for data column to be retrieved,
        /// and name for that variable.
        /// These can then be used after splitting each data line in the .CSV-file.
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="lines"></param>
        /// <exception cref="ArgumentException">Thrown if any of the column names
        /// specified in SearchParams-object are not found.</exception>
        private Dictionary<string, int> GetColumnIndexes(SearchParams sp, string[] lines)
        {
            var dictionary = new Dictionary<string, int>();

            var columnNames = lines[0].Split(',');
            dictionary.Add("Date", Array.FindIndex(columnNames, x => x.Equals(sp.Date)));
            dictionary.Add("Hometeam", Array.FindIndex(columnNames, x => x.Equals(sp.Hometeam)));
            dictionary.Add("Awayteam", Array.FindIndex(columnNames, x => x.Equals(sp.Awayteam)));
            dictionary.Add("Homescore", Array.FindIndex(columnNames, x => x.Equals(sp.Homescore)));
            dictionary.Add("Awayscore", Array.FindIndex(columnNames, x => x.Equals(sp.Awayscore)));
            dictionary.Add("HomeOdd", Array.FindIndex(columnNames, x => x.Equals(sp.HomeOdd)));
            dictionary.Add("DrawOdd", Array.FindIndex(columnNames, x => x.Equals(sp.DrawOdd)));
            dictionary.Add("AwayOdd", Array.FindIndex(columnNames, x => x.Equals(sp.AwayOdd)));

            if (dictionary.Any(row => row.Value == -1))
            {
                throw new ArgumentException("Index was not found");
            }

            return dictionary;
        }
    }
}
