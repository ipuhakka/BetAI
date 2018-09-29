using System;
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
            string[] lines = FileOperations.ReadFile(inputFile);
            SearchParams sp = new SearchParams(columns);
            List<Tuple<string, int>> indexes = GetColumnIndexes(sp, lines);
            return ParseColumns(indexes, lines, league, season);
        }

        /// <summary>
        /// Parses needed columns from string[] lines into a match list. 
        /// </summary>
        /// <exception cref="FormatException">Thrown if a value conversion fail.</exception>
        private List<Match> ParseColumns(List<Tuple<string, int>> indexes, string[] lines, string league, string season)
        {
            List<Match> matches = new List<Match>();

            for(int i = 1; i < lines.Length; i++)
            {
                string[] splitted = lines[i].Split(',');
                try 
                {
                    string date = Convert.ToDateTime(splitted[indexes[indexes.FindIndex(t => t.Item1 == "Date")].Item2]).ToString("yyyy-MM-dd");
                    string hometeam = splitted[indexes[indexes.FindIndex(t => t.Item1 == "Hometeam")].Item2];
                    string awayteam = splitted[indexes[indexes.FindIndex(t => t.Item1 == "Awayteam")].Item2];
                    int homescore = Convert.ToInt32(splitted[indexes[indexes.FindIndex(t => t.Item1 == "Homescore")].Item2]);
                    int awayscore = Convert.ToInt32(splitted[indexes[indexes.FindIndex(t => t.Item1 == "Awayscore")].Item2]);
                    double homeOdd = Convert.ToDouble(splitted[indexes[indexes.FindIndex(t => t.Item1 == "HomeOdd")].Item2], CultureInfo.InvariantCulture);
                    double drawOdd = Convert.ToDouble(splitted[indexes[indexes.FindIndex(t => t.Item1 == "DrawOdd")].Item2], CultureInfo.InvariantCulture);
                    double awayOdd = Convert.ToDouble(splitted[indexes[indexes.FindIndex(t => t.Item1 == "AwayOdd")].Item2], CultureInfo.InvariantCulture);
                    matches.Add(new Match(hometeam, awayteam, season, league, Convert.ToDateTime(date), homescore,
                        awayscore, i, homeOdd, drawOdd, awayOdd));
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
        private List<Tuple<string, int>> GetColumnIndexes(SearchParams sp, string[] lines)
        {
            List<Tuple<string, int>> tuples = new List<Tuple<string, int>>();
            string[] columnNames = lines[0].Split(',');
            tuples.Add(new Tuple<string, int>("Date", Array.FindIndex(columnNames, x => x.Equals(sp.Date))));
            tuples.Add(new Tuple<string, int>("Hometeam", Array.FindIndex(columnNames, x => x.Equals(sp.Hometeam))));
            tuples.Add(new Tuple<string, int>("Awayteam", Array.FindIndex(columnNames, x => x.Equals(sp.Awayteam))));
            tuples.Add(new Tuple<string, int>("Homescore", Array.FindIndex(columnNames, x => x.Equals(sp.Homescore))));
            tuples.Add(new Tuple<string, int>("Awayscore", Array.FindIndex(columnNames, x => x.Equals(sp.Awayscore))));
            tuples.Add(new Tuple<string, int>("HomeOdd", Array.FindIndex(columnNames, x => x.Equals(sp.HomeOdd))));
            tuples.Add(new Tuple<string, int>("DrawOdd", Array.FindIndex(columnNames, x => x.Equals(sp.DrawOdd))));
            tuples.Add(new Tuple<string, int>("AwayOdd", Array.FindIndex(columnNames, x => x.Equals(sp.AwayOdd))));

            if (tuples.FindIndex(t => t.Item2 == -1) != -1)
                throw new ArgumentException();
            return tuples;
        }
    }
}
