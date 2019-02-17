using System.Collections.Generic;

namespace Database
{
    public class Wager
    {
        public List<Match> Matches { get; set; }
        public double Stake { get; set; }

        public Wager(List<Match> bets, double stake)
        {
            Matches = bets;
            Stake = stake;
        }
    }
}
