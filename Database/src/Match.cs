using System;

namespace Database
{
    public class Match
    {
        public string Hometeam { get; }
        public string Awayteam { get; }
        public string Season { get; }
        public string League { get; }
        public DateTime Date { get; }
        public int Homescore { get; }
        public int Awayscore { get; }
        public double HomeOdd { get; }
        public double DrawOdd { get; }
        public double AwayOdd { get; }

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
    }
}
