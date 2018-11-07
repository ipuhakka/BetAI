using System;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Genetics.Mutation;

namespace Genetics.Mutation
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
        public void Test_Mutate_Probability_Less_Than_0_Throws_ArgumentException()
        {
            UniformMutation um = new UniformMutation();
            Assert.Throws<ArgumentException>(() => um.Mutate(nodes, -0.001));
        }

        [Test]
        public void Test_Mutate_Probability_Over_1_Throws_ArgumentException()
        {
            UniformMutation um = new UniformMutation();
            Assert.Throws<ArgumentException>(() => um.Mutate(nodes, 1.001));
        }

        [Test]
        public void Test_Mutate_returns_same_amount_of_nodes()
        {
            UniformMutation um = new UniformMutation();
            Assert.AreEqual(nodes.Count, um.Mutate(nodes, 0.03).Count);
        }

        [Test]
        public void Test_Mutate_probability_1_changes_all_nodes()
        {
            UniformMutation um = new UniformMutation();
            List<Node> mutated = um.Mutate(nodes, 1);
            for (int i = 0; i < mutated.Count; i++)
            {
                Assert.IsFalse(mutated[i].Equals(nodes[i]));
            }
        }

        [Test]
        public void Test_Mutate_probability_0_changes_none()
        {
            UniformMutation um = new UniformMutation();
            List<Node> mutated = um.Mutate(nodes, 0);
            Assert.AreEqual(nodes, mutated);
        }
    }
}
