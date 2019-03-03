using System;

namespace Database
{
    public class Match
    {
        public string Hometeam { get; }
        public string Awayteam { get; }
        public string Season { get; set; }
        public string League { get; set; }
        public DateTime Date { get; }
        public int Homescore { get; }
        public int Awayscore { get; }
        public double HomeOdd { get; }
        public double DrawOdd { get; }
        public double AwayOdd { get; }
        public char SimulatedResult { get; set; }

        public Match(string homeT, string awayT, string league, string season, DateTime d, int homeS, int awayS, double homeO, double drawO, double awayO)
        {
            Hometeam = homeT;
            Awayteam = awayT;
            Season = season;
            League = league;
            Date = Convert.ToDateTime(d.ToString("yyyy-MM-dd"));
            Homescore = homeS;
            Awayscore = awayS;
            HomeOdd = homeO;
            DrawOdd = drawO;
            AwayOdd = awayO;
        }

        /// <summary>
        /// Constructor for a Match which is to be predicted (Non-simulation).
        /// </summary>
        public Match(string homeT, string awayT, double homeO, double drawO, double awayO, DateTime date)
        {
            Hometeam = homeT;
            Awayteam = awayT;
            HomeOdd = homeO;
            DrawOdd = drawO;
            AwayOdd = awayO;
            Date = date; 
        }

        /// <summary>
        /// Returns HomeOdd, if SimulatedResult is '1',
        /// DrawOdd if SimulatedResult is 'X',
        /// AwayOdd if SimulatedResult is '2'.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public double GetWagerOdd()
        {
            switch (SimulatedResult)
            {
                case '1':
                    return HomeOdd;
                case '2':
                    return AwayOdd;
                case 'X':
                    return DrawOdd;
                default:
                    throw new ArgumentException("SimulatedResult value was invalid");
            }
        }

        /// <summary>
        /// Two matches are considered equal, if they have the same
        /// Date and Hometeam and Awayteam.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            Match m = obj as Match;
            return Date.Equals(m.Date) && Hometeam.Equals(m.Hometeam) && Awayteam.Equals(m.Awayteam);
        }

        public override int GetHashCode()
        {
            int hash = 9;
            hash = (hash * 3) + Date.GetHashCode();
            hash = (hash * 4) + Hometeam.GetHashCode();
            hash = (hash * 2) + Awayteam.GetHashCode();
            return hash;
        }
    }
}
