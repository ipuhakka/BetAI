using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Database;
using System.Data.SQLite;

namespace BetAI.Genetics
{
    public class Sample
    {
        public List<int> Points { get; }

        /// <summary>
        /// Constructor for Sample. Creates an n-sized sample, if n is less
        /// than count of matches-table rows in path.
        /// </summary>
        /// <param name="sampleSize">Sample size</param>
        /// <param name="path">Path to the used database file.</param>
        /// <exception cref="NotEnoughDataException">Thrown if sampleSize is less
        /// than amount of matches in database.</exception>
        /// <exception cref="SQLiteException"></exception>
        public Sample(string path, int sampleSize)
        {
            DB db = new DB(path);
            int matchCount = db.SelectCount();

            if (sampleSize > matchCount)
                throw new NotEnoughDataException("Sample size less than match count");

            Points = new List<int>();
            List<int> samplePoints = Enumerable.Range(0, sampleSize).ToList();
            Random rand = new Random();
            for (int i = 0; i < sampleSize; i++)
            {
                int newPoint = rand.Next(0, samplePoints.Count);
                Points.Add(samplePoints[newPoint]);
                samplePoints.RemoveAt(newPoint);
            }

        }
    }
}
