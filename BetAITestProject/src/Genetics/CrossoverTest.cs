using System;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.Genetics;
using FluentAssertions;

namespace BetAITestProject.Genetics
{
    /// <summary>
    /// This class contains test cases for BetAI.Genetics.Crossover.cs.
    /// 
    /// It should be tested that alpha less than 0 throws ArgumentException,
    /// that (floor(selectedCrossoverNodes / 2) * 2) children are always created, 
    /// and that SimulationSampleSize, DrawLimit, and PlayLimit are always 
    /// in between (minimum parent parameter value - alpha * parent parameter difference,
    /// maximum parent parameter value + alpha * parent parameter difference).
    /// Generation should always be added by one per generation, and minimumStake
    /// should not change ever.
    /// </summary>
    [TestFixture]
    public class CrossoverTest
    {
        [Test]
        public void test_Reproduce_alpha_less_than_0_throws_ArgumentException()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 100; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }
            Crossover co = new Crossover();
            Assert.Throws<ArgumentException>(() => co.Reproduce(nodes, -0.001));
        }

        [Test]
        public void test_Reproduce_alpha_0_runs()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 100; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }

            Crossover co = new Crossover();
            Assert.DoesNotThrow(() => co.Reproduce(nodes, 0.0));
        }

        [Test]
        public void test_Reproduce_2_nodes_runs()
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(new Node(1, 0.5, 3, 0, 4));
            nodes.Add(new Node(1, 0.5, 3, 0, 4));
            Crossover co = new Crossover();
            Assert.DoesNotThrow(() => co.Reproduce(nodes, 0.5));
        }

        [Test]
        public void test_Reproduce_1_node_throws_ArgumentExcpetion()
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(new Node(1, 0.5, 3, 0, 4));
            Crossover co = new Crossover();
            Assert.Throws<ArgumentException>(() => co.Reproduce(nodes, 0.5));
        }

        [Test]
        public void test_Reproduce_throws_ArgumentNullExcpetion()
        {
            Crossover co = new Crossover();
            Assert.Throws<ArgumentNullException>(() => co.Reproduce(null, 0.5));
        }

        [Test]
        public void test_Reproduce_Generation_Is_Incremented_by_1()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 100; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }

            Crossover co = new Crossover();
            List<Node> gen1 = co.Reproduce(nodes, 0.0);
            gen1.Should().OnlyContain(node => node.Generation == 1);
            List<Node> gen2 = co.Reproduce(gen1, 0.0);
            gen2.Should().OnlyContain(node => node.Generation == 2);
        }

        /// <summary>
        /// floor(selectedCrossoverNodes / 2) * 2 -children should be returned.
        /// </summary>
        [Test]
        public void test_Reproduce_100_nodes_returns_100_children()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 100; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }

            Crossover co = new Crossover();
            Assert.AreEqual(100, co.Reproduce(nodes, 0.0).Count);
        }

        /// <summary>
        /// floor(selectedCrossoverNodes / 2) * 2 -children should be returned.
        /// </summary>
        [Test]
        public void test_Reproduce_101_nodes_returns_100_children()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 101; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }

            Crossover co = new Crossover();
            Assert.AreEqual(100, co.Reproduce(nodes, 0.0).Count);
        }

        /// <summary>
        /// floor(selectedCrossoverNodes / 2) * 2 -children should be returned.
        /// </summary>
        [Test]
        public void test_Reproduce_99_nodes_returns_100_children()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 99; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }

            Crossover co = new Crossover();
            Assert.AreEqual(98, co.Reproduce(nodes, 0.0).Count);
        }

        [Test]
        public void test_Reproduce_MinimumStake_does_not_change()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 100; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }

            Crossover co = new Crossover();
            List<Node> children = co.Reproduce(nodes, 0.0);
            children.Should().OnlyContain(n => n.MinimumStake == 5);
        }

        [Test]
        public void test_Reproduce_PlayLimit_InRange()
        {
            // PlayLimit should always with these arguments be
            //in range 0.5 - 2.5 with PlayLimits of 1 and 2.
            List<Node> nodes;

            Crossover co = new Crossover();
            for (int i = 0; i < 100; i++)
            {
                nodes = new List<Node>();
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
                nodes.Add(new Node(2, 0.3, 5, 0, 5));
                Console.WriteLine(nodes.Count);
                List<Node> children = co.Reproduce(nodes, 0.5);
                children[0].PlayLimit.Should().BeInRange(0.5, 2.5);
            }
        }

        [Test]
        public void test_Reproduce_DrawLimit_InRange()
        {
            /* DrawLimit should always with these arguments be
            in range 0.27 - 0.63 with alpha value of 0.1 and 
            parent drawLimits of 0.3 and 0.6. */
            List<Node> nodes;

            Crossover co = new Crossover();
            for (int i = 0; i < 100; i++)
            {
                nodes = new List<Node>();
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
                nodes.Add(new Node(2, 0.6, 5, 0, 5));
                Console.WriteLine(nodes.Count);
                List<Node> children = co.Reproduce(nodes, 0.1);
                children[0].DrawLimit.Should().BeInRange(0.27, 0.63);
            }
        }

        [Test]
        public void test_Reproduce_SimulationSampleSize_InRange()
        {
            // SimulationSampleSize should always with these arguments be
            //in range 4 - 9 with alpha value of 0.2.
            List<Node> nodes;

            Crossover co = new Crossover();
            for (int i = 0; i < 100; i++)
            {
                nodes = new List<Node>();
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
                nodes.Add(new Node(2, 0.6, 5, 0, 8));
                Console.WriteLine(nodes.Count);
                List<Node> children = co.Reproduce(nodes, 0.2);
                children[0].SimulationSampleSize.Should().BeInRange(4, 9);
            }
        }
    }
}
