using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Genetics.Selection;
using BetAI.Utils;
using BetAI.BetSim;
using Database;
using Newtonsoft.Json;

namespace Genetics.Selection
{
    [TestFixture]
    public class TournamentSelectionTest
    {
        List<Node> nodes;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            nodes = new List<Node>();
            Random rand = new Random();
            Randomise.InitRandom();
            for (int i = 0; i < 50; i++)
            {
                nodes.Add(new Node(rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), 1, rand.Next()));
            }
            string path = "test-files/data.sqlite3";
            Matches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(20);
            int maxSampleSize = nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize;
            Matches.CreateMatchDataStructs(sample, maxSampleSize);

            for (int j = 0; j < nodes.Count; j++)
            {
                nodes[j].EvaluateFitness(sample);
            }
            Console.WriteLine(JsonConvert.SerializeObject(nodes));
        }

        /// <summary>
        /// If tournament size is less than 1, ArgumentException is thrown.
        /// </summary>
        [Test]
        public void Test_constructor_size_0_throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new TournamentSelection(0, nodes.Count));
        }

        /// <summary>
        /// If generation size is less than tournament size, generation size is set as
        /// tournament size.
        /// </summary>
        [Test]
        public void Test_constructor_tSize_50_gSize_49()
        {
            TournamentSelection ts = new TournamentSelection(50, 49);
            Assert.AreEqual(49, ts.TournamentSize);
        }

        [Test]
        public void Test_constructor_tSize_50_gSize_75()
        {
            TournamentSelection ts = new TournamentSelection(50, 75);
            Assert.AreEqual(50, ts.TournamentSize);
        }

        [Test]
        public void Test_SelectForCrossover_returns_unique_nodes()
        {
            TournamentSelection ts = new TournamentSelection(16, nodes.Count);
            for (int i = 0; i < 1; i++)
            {
                Parents parents = ts.SelectForCrossover(nodes);
                Assert.AreNotEqual(parents.Parent1, parents.Parent2);
            }
        }

        /// <summary>
        /// Tournament should always return node with maximum fitness, so if
        /// tournamentSelection is called with same tournamentSize and number of nodes,
        /// nodes with two maximum fitness values should be returned.
        /// </summary>
        [Test]
        public void Test_SelectForCrossover_EqualTournamentSizeAndGenerationSize_ReturnMaxNodes()
        {
            var tournamentSelection = new TournamentSelection(nodes.Count, nodes.Count);
            var nodesOrdered = nodes.OrderByDescending(n => n.Fitness)
                .ToList();
            
            var parents = tournamentSelection.SelectForCrossover(nodes);

            Assert.AreEqual(parents.Parent1, nodesOrdered[0]);
            Assert.AreEqual(parents.Parent2, nodesOrdered[1]);
        }

    }
}
