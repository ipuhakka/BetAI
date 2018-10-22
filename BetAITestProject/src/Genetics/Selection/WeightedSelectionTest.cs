using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Genetics.Selection;
using BetAI.Data;
using BetAI.Utils;
using Database;
using Newtonsoft.Json;

namespace BetAITestProject.Genetics.Selection
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
            Randomise.InitRandom();
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
        /// This test tests that CrossoverValues are set as they should.
        /// 
        /// For node with the lowest CrossoverValue, CrossoverValue is equal
        /// to its fitness. For the rest it should be fitness - previous node
        /// fitness + running sum of CrossoverValues.
        /// </summary>
        [Test]
        public void test_Selection_CrossoverValues()
        {
            WeightedSelection sel = new WeightedSelection();
            sel.SelectForCrossover(nodes);
            List<Node> sorted = nodes.OrderBy(n => n.CrossoverValue).ToList();
            double runningSum = 0;

            for (int i = 0; i < sorted.Count; i++)
            {
                if (i == 0)
                    Assert.AreEqual(0, sorted[i].CrossoverValue);
                else
                    Assert.AreEqual(runningSum + (sorted[i].Fitness - sorted[i-1].Fitness), sorted[i].CrossoverValue);
                runningSum += sorted[i].CrossoverValue;
            }
        }

        [Test]
        public void test_Selection_returns_2_nodes()
        {
            WeightedSelection sel = new WeightedSelection();
            for (int j = 0; j < 100; j++)
            {
                List<Node> selectedParents = sel.SelectForCrossover(nodes);
                Assert.AreEqual(2, selectedParents.Count);
            }
        }
    }
}
