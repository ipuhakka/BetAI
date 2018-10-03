using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using BetAI.Genetics;
using BetAI.Utils;
using BetAI.Data;

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
            Sample sample = new Sample(200);
            Node node = new Node(2.8, 3.14, 5, 0);
            Assert.DoesNotThrow(() => node.EvaluateFitness(sample.Matches, 5));
        }

        /*[Test]
        public void test_EvaluateFitness_performance()
        {
            List<Node> nodes = new List<Node>();
            string path = "test-files/data.sqlite3";
            Sample sample = new Sample(path, 200);
            for (var i = 0; i < 10; i++)
            {
                nodes.Add(new Node(2.8, 3.14, 5, 0));
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            foreach(Node n in nodes)
            {
                n.EvaluateFitness(sample.Matches, path, 5);
            }
            sw.Stop();
            Console.WriteLine("Took " + sw.ElapsedMilliseconds);
            Assert.LessOrEqual(10000, sw.ElapsedMilliseconds);
        } */
    }
}
