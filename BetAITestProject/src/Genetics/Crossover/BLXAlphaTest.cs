using System;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Genetics.Crossover;
using FluentAssertions;

namespace Genetics.Crossover
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
    public class BLXAplhaTest
    {
        [Test]
        public void Test_Crossover_alpha_less_than_0_throws_ArgumentException()
        {
            List<Node> nodes = new List<Node>();

            for (int i = 0; i < 100; i++)
            {
                nodes.Add(new Node(1, 0.3, 5, 0, 5));
            }
            Assert.Throws<ArgumentException>(() => new BLXAlpha(-0.001));
        }

        [Test]
        public void Test_Crossover_alpha_0_runs()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1, 0.3, 5, 0, 5),
                new Node(1, 0.3, 5, 0, 5)
            };
            Assert.DoesNotThrow(() => new BLXAlpha(0));
        }

        [Test]
        public void Test_Crossover_throws_ArgumentNullExcpetion()
        {
            BLXAlpha co = new BLXAlpha(1);
            Assert.Throws<ArgumentNullException>(() => co.Crossover(null, new Node()));
        }

        [Test]
        public void Test_Crossover_Generation_Is_Incremented_by_1()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1, 0.3, 5, 0, 5),
                new Node(1, 0.3, 5, 0, 5)
            };

            BLXAlpha co = new BLXAlpha(1);
            List<Node> gen1 = co.Crossover(nodes[0], nodes[1]);
            gen1.Should().OnlyContain(node => node.Generation == 1);
            List<Node> gen2 = co.Crossover(gen1[0], gen1[1]);
            gen2.Should().OnlyContain(node => node.Generation == 2);
        }

        /// <summary>
        /// 2 children should be returned by Crossover().
        /// </summary>
        [Test]
        public void Test_Crossover_returns_2_children()
        {
            List<Node> nodes = new List<Node>
            {
                new Node(1, 0.3, 5, 0, 5),
                new Node(1, 0.3, 5, 0, 5)
            };

            BLXAlpha co = new BLXAlpha(0.5);
            Assert.AreEqual(2, co.Crossover(nodes[0], nodes[1]).Count);
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

        [Test]
        public void Test_Crossover_PlayLimit_InRange()
        {
            // PlayLimit should always with these arguments be
            //in range 0.5 - 2.5 with PlayLimits of 1 and 2 and an alpha value of 0.5.
            List<Node> nodes;

            BLXAlpha co = new BLXAlpha(0.5);
            for (int i = 0; i < 100; i++)
            {
                nodes = new List<Node>
                {
                    new Node(1, 0.3, 5, 0, 5),
                    new Node(2, 0.3, 5, 0, 5)
                };
                Console.WriteLine(nodes.Count);
                List<Node> children = co.Crossover(nodes[0], nodes[1]);
                children[0].PlayLimit.Should().BeInRange(0.5, 2.5);
            }
        }

        [Test]
        public void Test_Crossover_DrawLimit_InRange()
        {
            /* DrawLimit should always with these arguments be
            in range 0.27 - 0.63 with alpha value of 0.1 and 
            parent drawLimits of 0.3 and 0.6. */
            List<Node> nodes;

            BLXAlpha co = new BLXAlpha(0.1);
            for (int i = 0; i < 100; i++)
            {
                nodes = new List<Node>
                {
                    new Node(1, 0.3, 5, 0, 5),
                    new Node(2, 0.6, 5, 0, 5)
                };
                Console.WriteLine(nodes.Count);
                List<Node> children = co.Crossover(nodes[0], nodes[1]);
                children[0].DrawLimit.Should().BeInRange(0.27, 0.63);
            }
        }

        [Test]
        public void Test_Crossover_SimulationSampleSize_InRange()
        {
            // SimulationSampleSize should always with these arguments be
            //in range 4 - 9 with alpha value of 0.2.
            List<Node> nodes;

            BLXAlpha co = new BLXAlpha(0.2);
            for (int i = 0; i < 100; i++)
            {
                nodes = new List<Node>
                {
                    new Node(1, 0.3, 5, 0, 5),
                    new Node(2, 0.6, 5, 0, 8)
                };
                List<Node> children = co.Crossover(nodes[0], nodes[1]);
                children[0].SimulationSampleSize.Should().BeInRange(4, 9);
            }
        }

        [Test]
        public void Test_Crossover_SimulationSampleSize_1_does_not_throw_ArgumentException()
        {
            /* SimulationSampleSize can never be less than 1 or larger than 100. This tests that
                child node will never have a SimulationSampleSize less than limit, by checking
                that crossover does not throw ArgumentException when creating new nodes.
             */
            List<Node> nodes;

            BLXAlpha co = new BLXAlpha(5);
            for (int i = 0; i < 100; i++)
            {
                nodes = new List<Node>
                {
                    new Node(1, 0.3, 5, 0, 1),
                    new Node(2, 0.6, 5, 0, 2)
                };
                Assert.DoesNotThrow(() => co.Crossover(nodes[0], nodes[1]));              
            }
        }
    }
}
