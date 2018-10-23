using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetAI.Utils
{
    public class Randomise
    {
        public static Random random;

        public static void InitRandom()
        {
            random = new Random();
        }
    }
}
