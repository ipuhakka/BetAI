using System;
using BetAI.Genetics;

namespace BetAI.Genetics
{
    public class Parents
    {
        public Node Parent1 { get; private set; }
        public Node Parent2 { get; private set; }

        public Parents(Node p1, Node p2)
        {
            Parent1 = p1;
            Parent2 = p2;
        }
    }
}
