using System.Collections.Generic;
using BetAI.Genetics.Crossover;
using BetAI.Genetics;
using NUnit.Framework;
using FluentAssertions;

namespace Genetics.Crossover
{
    [TestFixture]
    public class UniformTest
    {
        [Test]
        public void Test_Crossover_returns_2_children()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1, 0.3, 5, 0, 5),
                new Node(2, 0.6, 2, 0, 6)
            };

            Uniform uniform = new Uniform();
            Assert.AreEqual(2, uniform.Crossover(nodes[0], nodes[1]).Count);
        }

        [Test]
        public void Test_Crossover_node_values_from_either_parent()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1, 0.3, 5, 0, 5),
                new Node(2, 0.6, 2, 0, 6)
            };

            Uniform uniform = new Uniform();
            List<Node> children = uniform.Crossover(nodes[0], nodes[1]);

            foreach(Node n in children)
            {
                n.PlayLimit.Should().BeOneOf(1, 2);
                n.DrawLimit.Should().BeOneOf(0.3, 0.6);
                Assert.AreEqual(1, n.Generation);
                n.MinimumStake.Should().BeOneOf(2, 5);
                n.SimulationSampleSize.Should().BeOneOf(5, 6);
            }
        }
    }
}
