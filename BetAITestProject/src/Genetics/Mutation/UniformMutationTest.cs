using System;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Genetics.Mutation;

namespace BetAITestProject.Genetics.Mutation
{
    [TestFixture]
    public class UniformMutationTest
    {
        private List<Node> nodes;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            nodes = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 50; i++)
            {
                nodes.Add(new Node(rand, 1));
            }
        }
        [Test]
        public void TestMutate_Probability_Less_Than_0_Throws_ArgumentException()
        {
            UniformMutation um = new UniformMutation();
            Assert.Throws<ArgumentException>(() => um.Mutate(nodes, -0.001));
        }

        [Test]
        public void TestMutate_Probability_Over_1_Throws_ArgumentException()
        {
            UniformMutation um = new UniformMutation();
            Assert.Throws<ArgumentException>(() => um.Mutate(nodes, 1.001));
        }
    }
}
