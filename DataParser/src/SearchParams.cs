using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class SearchParams
    {
        public string Date { get; }
        public string Hometeam { get; }
        public string Awayteam { get; }
        public string Homescore { get; }
        public string Drawscore { get; }
        public string Awayscore { get; }
        public string HomeOdd { get; }
        public string DrawOdd { get; }
        public string AwayOdd { get; }

        /// <summary>
        /// Constructor for SearchParams-object. Search parameters are column names which are
        /// parsed from .CSV-file. Parameters are split with '=' char. 
        /// </summary>
        /// <param name="args">User inputted arguments used instead of default values.
        /// Example = "hometeam=alternativeColumnForHometeam"</param>
        public SearchParams(params string[] args)
        {
            Date = "Date";
            Hometeam = "HomeTeam";
            Awayteam = "AwayTeam";
            Homescore = "FTHG";
            Awayscore = "FTAG";
            HomeOdd = "B365H";
            DrawOdd = "B365D";
            AwayOdd = "B365A";

            foreach (string arg in args)
            {
                if (arg != null)
                {
                    string[] split = arg.Split('=');

                    if (split.Length == 2)
                    {
                        switch (split[0].Trim().ToLower())
                        {
                            case "hometeam":
                                Hometeam = split[1].Trim();
                                break;
                            case "awayteam":
                                Awayteam = split[1].Trim();
                                break;
                            case "homescore":
                                Homescore = split[1].Trim();
                                break;
                            case "awayscore":
                                Awayscore = split[1].Trim();
                                break;
                            case "date":
                                Date = split[1].Trim();
                                break;
                            case "homeodd":
                                HomeOdd = split[1].Trim();
                                break;
                            case "drawodd":
                                DrawOdd = split[1].Trim();
                                break;
                            case "awayodd":
                                AwayOdd = split[1].Trim();
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Override done to ease testing.
        /// </summary>
        public override bool Equals(object obj)
        {
            var item = obj as SearchParams;
            return (this.Date.Equals(item.Date) && this.Hometeam.Equals(item.Hometeam) &&
                this.Awayteam.Equals(item.Awayteam) && this.Homescore.Equals(item.Homescore)
                && this.Awayscore.Equals(item.Awayscore) && this.HomeOdd.Equals(item.HomeOdd)
                && this.DrawOdd.Equals(item.DrawOdd) && this.AwayOdd.Equals(item.AwayOdd));
        }

        public override int GetHashCode()
        {
            int hash = 27;
            hash = (hash * 3) + Date.GetHashCode();
            hash = (hash * 3) + Hometeam.GetHashCode();
            hash = (hash * 3) + Awayteam.GetHashCode();
            return hash;
        }
    }
}
