using System;
using System.Collections.Generic;
using BetAI.Genetics;

namespace BetAI.Genetics.Mutation
{
    public interface IMutation
    {
        List<Node> Mutate(List<Node> generation, double probability);
    }
}
