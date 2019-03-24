using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using Database;
using BetAI.BetSim;
using BetAI.Exceptions;

namespace BetAI.Utils
{
    public class Sample
    {
        /// <summary>
        /// Creates an n-sized sample of matches, if n is less
        /// than count of matches-table rows in path.
        /// </summary>
        /// <param name="sampleSize">Sample size</param>
        /// <param name="path">Path to the used database file.</param>
        /// <exception cref="NotEnoughDataException">Thrown if sampleSize is less
        /// than amount of matches in database.</exception>
        /// <exception cref="SQLiteException"></exception>
        public static List<Match> CreateSample(int sampleSize)
        {
            var indexes = new List<int>();

            var matchCount = Matches.GetMatchCount();
            if (sampleSize > matchCount)
                throw new NotEnoughDataException();

            var possibleIndexes = Enumerable.Range(0, matchCount).ToList();
            var rand = new Random();

            for (int i = 0; i < sampleSize; i++)
            {
                var newIndex = rand.Next(0, possibleIndexes.Count - 1);
                indexes.Add(possibleIndexes[newIndex]);
                possibleIndexes.RemoveAt(newIndex);
            }

            return Matches.SelectMatchesWithRowIndex(indexes);
        }
    }
}
