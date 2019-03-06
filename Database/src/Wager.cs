using System;
using System.Collections.Generic;

namespace Database
{
    public class Wager
    {
        public List<Match> Matches { get; set; }
        public double Stake { get; set; }
        public string Author { get; set; }
        public double Odd { get; set; }
        public int Result { get; set; } //-1 = lost, 0 = not resolved, 1 = won.
        public DateTime Date { get; set; }

        public Wager(List<Match> bets, double stake)
        {
            Matches = bets;
            Stake = stake;
        }

        public Wager(double stake, double odd, string author, int result, DateTime date, List<Match> matches)
        {
            Stake = stake;
            Odd = odd;
            Author = author;
            Result = result;
            Date = date;
            Matches = matches;
        }
    }
}
