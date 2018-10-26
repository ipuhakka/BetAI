using System;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.Genetics.Crossover;
using BetAI.Genetics.Selection;
using BetAI.Genetics;
using BetAI.Utils;

namespace BetAITestProject.Genetics
{
    [TestFixture]
    public class ReproduceTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Randomise.InitRandom();
        }

        [Test]
        public void Test_Reproduce_WeightedSelection_BLXAlpha_returns_100_nodes()
        {
            ICrossover co = new BLXAlpha(0.5);
            ISelection selection = new WeightedSelection();
            List<Node> nodes = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 101; i++)
            {
                nodes.Add(new Node(rand, 5));
            }

            Reproduce reproduce = new Reproduce(co, selection);
            List<Node> children = reproduce.CreateNewGeneration(nodes);
            Assert.AreEqual(100, children.Count);
        }

        [Test]
        public void Test_Reproduce_WeightedSelection_BLXAlpha_returns_98_nodes()
        {
            ICrossover co = new BLXAlpha(0.5);
            ISelection selection = new WeightedSelection();
            List<Node> nodes = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 99; i++)
            {
                nodes.Add(new Node(rand, 5));
            }

            Reproduce reproduce = new Reproduce(co, selection);
            List<Node> children = reproduce.CreateNewGeneration(nodes);
            Assert.AreEqual(98, children.Count);
        }
    }
}
