using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Utils;
using BetAI.Data;
using Database;

namespace BetAITestProject.Genetics
{
    [TestFixture]
    public class NodeTest
    {
        private string path = "test-files/data.sqlite3";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory));
            
        }

        [Test]
        public void test_EvaluateFitness_runs()
        {
            QueryMatches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(13);
            Node node = new Node(2.8, 3.14, 5, 0, 5);
            QueryMatches.CreateMatchDataStructs(sample, node.SimulationSampleSize);
            Assert.DoesNotThrow(() => node.EvaluateFitness(sample));
        }

        [Test]
        public void test_EvaluateFitness_performance_Average_LessThan1000ms()
        {
            List<Node> nodes = new List<Node>();
            List<long> runtimes = new List<long>();
            string path = "test-files/data.sqlite3";
            QueryMatches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(13);

            for (var i = 0; i < 100; i++)
            {
                nodes.Add(new Node(2.8, 3.14, 5, 0, 5));
            }
            Stopwatch sw = new Stopwatch();
            QueryMatches.CreateMatchDataStructs(sample, nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize);
            foreach (Node n in nodes)
            {
                sw.Start();
                n.EvaluateFitness(sample);
                sw.Stop();
                runtimes.Add(sw.ElapsedMilliseconds);
                sw.Reset();
            }
            
            Console.WriteLine("Average: " + runtimes.Average());
            Assert.LessOrEqual(runtimes.Average(), 1000);
        }

        [Test]
        public void test_EvaluateFitness_performance_Average_LessThan50ms()
        {
            List<Node> nodes = new List<Node>();
            List<long> runtimes = new List<long>();
            string path = "test-files/data.sqlite3";
            QueryMatches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(13);

            for (var i = 0; i < 100; i++)
            {
                nodes.Add(new Node(2.8, 3.14, 5, 0, 5));
            }
            Stopwatch sw = new Stopwatch();
            QueryMatches.CreateMatchDataStructs(sample, nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize);
            foreach (Node n in nodes)
            {
                sw.Start();
                n.EvaluateFitness(sample);
                sw.Stop();
                runtimes.Add(sw.ElapsedMilliseconds);
                sw.Reset();
            }

            Console.WriteLine("Average: " + runtimes.Average());
            Assert.LessOrEqual(runtimes.Average(),50);
        }
    }
}
