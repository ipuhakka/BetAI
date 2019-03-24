using System;

namespace BetAI.Utils
{
    public class Randomise
    {
        public static Random random;

        public static void InitRandom()
        {
            if (random == null)
                random = new Random();
        }
    }
}
