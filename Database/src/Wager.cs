using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

        /// <summary>
        /// Constructor for wager. Calculates odd from bets, and by default, 
        /// sets result to 0 (unresolved).
        /// </summary>
        /// <param name="bets"></param>
        /// <param name="stake"></param>
        /// <param name="result"></param>
        public Wager(List<Match> bets, double stake, int result = 0)
        {
            Matches = bets;
            Stake = stake;
            Odd = bets.Select(match => match.GetWagerOdd())
                      .Aggregate((x, y) => x * y);
            Result = result;
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

        [JsonConstructor]
        public Wager()
        {

        }
    }
}
