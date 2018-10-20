using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Data;
using BetAI.Utils;
using Database;

namespace BetAITestProject.Genetics
{
    [TestFixture]
    public class WeightedSelectionTest
    {
        List<Node> nodes;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            nodes = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                nodes.Add(new Node(rand, 5));
            }
            string path = "test-files/data.sqlite3";
            Matches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(200);
            int maxSampleSize = nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize;
            Matches.CreateMatchDataStructs(sample, maxSampleSize);

            for (int j = 0; j < nodes.Count; j++)
            {
                nodes[j].EvaluateFitness(sample);
            }
        }

        /// <summary>
        /// This test tests that CrossoverFactors are set as they should.
        /// 
        /// For the node with minimum fitness it is 0, for the rest it is 
        /// node[i].Fitness - minimum fitness.
        /// </summary>
        [Test]
        public void test_Selection_CrossoverFactors()
        {
            WeightedSelection sel = new WeightedSelection();
            double minFitness = nodes.Min(n => n.Fitness);
            sel.SelectForCrossover(nodes);

            foreach (Node n in nodes)
            {
                if (n.Fitness != minFitness)
                    Assert.AreEqual(n.Fitness - minFitness, n.CrossoverValue);
                else
                    Assert.AreEqual(0, n.CrossoverValue);
            }
        }

        [Test]
        public void test_Selection_101_nodes_returns_50_nodes()
        {
            List<Node> gen = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 101; i++)
            {
                gen.Add(new Node(rand, 5));
            }
            WeightedSelection sel = new WeightedSelection();
            List<Node> selectedParents = sel.SelectForCrossover(gen);
            Assert.AreEqual(50, selectedParents.Count);
        }

        [Test]
        public void test_Selection_100_nodes_returns_50_nodes()
        {
            List<Node> gen = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                gen.Add(new Node(rand, 5));
            }
            WeightedSelection sel = new WeightedSelection();
            List<Node> selectedParents = sel.SelectForCrossover(gen);
            Assert.AreEqual(50, selectedParents.Count);
        }

        [Test]
        public void test_Selection_99_nodes_returns_49_nodes()
        {
            List<Node> gen = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 99; i++)
            {
                gen.Add(new Node(rand, 5));
            }
            WeightedSelection sel = new WeightedSelection();
            List<Node> selectedParents = sel.SelectForCrossover(gen);
            Assert.AreEqual(49, selectedParents.Count);
        }
    }
}
