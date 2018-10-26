using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using BetAI.Genetics;
using BetAI.Genetics.Crossover;

namespace Genetics.Crossover
{
    [TestFixture]
    public class UniformAlphaTest
    {

        [Test]
        public void Test_constructor_negative_alpha_throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new UniformAlpha(-0.01));
        }

        [Test]
        public void Test_constructor_sets_Alpha()
        {
            UniformAlpha uniform = new UniformAlpha(0.8);
            Assert.AreEqual(0.8, uniform.Alpha);
        }

        [Test]
        public void Test_Crossover_generation_inceremented_by1()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1, 0.3, 5, 0, 5),
                new Node(1, 0.3, 5, 0, 5)
            };

            UniformAlpha co = new UniformAlpha(1);
            List<Node> gen1 = co.Crossover(nodes[0], nodes[1]);
            gen1.Should().OnlyContain(node => node.Generation == 1);
            List<Node> gen2 = co.Crossover(gen1[0], gen1[1]);
            gen2.Should().OnlyContain(node => node.Generation == 2);
        }

        [Test]
        public void Test_Crossover_MinimumStake_does_not_change()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1, 0.3, 5, 0, 5),
                new Node(1, 0.3, 5, 0, 5)
            };

            BLXAlpha co = new BLXAlpha(1);
            List<Node> children = co.Crossover(nodes[0], nodes[1]);
            children.Should().OnlyContain(n => n.MinimumStake == 5);
        }

        /// <summary>
        /// If alpha parameter is set to 0, children have always equal values as
        /// either of their parents.
        /// </summary>
        [Test]
        public void Test_Crossover_Alpha0_children_have_samevalues()
        {
            List<Node> nodes = new List<Node>();
            Random rand = new Random();

            nodes.Add(new Node(rand, 1));
            nodes.Add(new Node(rand, 1));
            List<Double> drawLimits = new List<double>{ nodes[0].DrawLimit, nodes[1].DrawLimit };
            List<Double> playLimits = new List<double> { nodes[0].PlayLimit, nodes[1].PlayLimit };
            List<int> sampleSizes = new List<int> { nodes[0].SimulationSampleSize, nodes[1].SimulationSampleSize };

            UniformAlpha uniform = new UniformAlpha(0);
            for (int i = 0; i < 10; i++)
            {
                List<Node> children = uniform.Crossover(nodes[0], nodes[1]);
                children[0].DrawLimit.Should().BeOneOf(drawLimits);
                children[0].PlayLimit.Should().BeOneOf(playLimits);
                children[0].SimulationSampleSize.Should().BeOneOf(sampleSizes);
                children[1].DrawLimit.Should().BeOneOf(drawLimits);
                children[1].PlayLimit.Should().BeOneOf(playLimits);
                children[1].SimulationSampleSize.Should().BeOneOf(sampleSizes);
            }
        }

        /// <summary>
        /// Simulated real values should be in range parent value +- 0.3.
        /// </summary>
        [Test]
        public void Test_Crossover_Alpha_0_3_real_values_in_range()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1.05, 0.25, 3, 0, 5),
                new Node(1.05, 0.25, 3, 0, 5)
            };
            UniformAlpha uniform = new UniformAlpha(0.3);
            for (int i = 0; i < 10; i++)
            {
                List<Node> children = uniform.Crossover(nodes[0], nodes[1]);
                children[0].DrawLimit.Should().BeInRange(nodes[0].DrawLimit - 0.3, nodes[0].DrawLimit + 0.3);
                children[0].PlayLimit.Should().BeInRange(nodes[0].PlayLimit - 0.3, nodes[0].PlayLimit + 0.3);
                children[1].DrawLimit.Should().BeInRange(nodes[0].DrawLimit - 0.3, nodes[0].DrawLimit + 0.3);
                children[1].PlayLimit.Should().BeInRange(nodes[0].PlayLimit - 0.3, nodes[0].PlayLimit + 0.3);
            }
        }

        /// <summary>
        /// Simulated integer values should be in range parent value +- (values * 0.3).
        /// </summary>
        [Test]
        public void Test_Crossover_Alpha_0_3_integer_values_in_range()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1.05, 0.25, 3, 0, 5),
                new Node(1.05, 0.25, 3, 0, 5)
            };
            double alpha = 0.3;
            int sampleSize = nodes[0].SimulationSampleSize;

            UniformAlpha uniform = new UniformAlpha(alpha);
            for (int i = 0; i < 10; i++)
            {
                List<Node> children = uniform.Crossover(nodes[0], nodes[1]);
                children[0].SimulationSampleSize.Should().BeInRange((int)Math.Round(sampleSize - (sampleSize * alpha), 0), (int)Math.Round(sampleSize + (sampleSize * alpha), 0));
                children[1].SimulationSampleSize.Should().BeInRange((int)Math.Round(sampleSize - (sampleSize * alpha), 0), (int)Math.Round(sampleSize + (sampleSize * alpha), 0));
            }
        }

        [Test]
        public void Test_Crossover_returns_2_children()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1.05, 0.25, 3, 0, 5),
                new Node(1.05, 0.25, 3, 0, 5)
            };
            double alpha = 0.3;
            int sampleSize = nodes[0].SimulationSampleSize;

            UniformAlpha uniform = new UniformAlpha(alpha);
            for (int i = 0; i < 10; i++)
            {
                List<Node> children = uniform.Crossover(nodes[0], nodes[1]);
                Assert.AreEqual(2, children.Count);
            }
        }
    }
}
