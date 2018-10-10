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
using FluentAssertions;

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

        // If minimumstake less than or equal to 0 or generation less than 0
        //is inputted, argumentexception will be thrown.
        [Test]
        public void test_Node_constructor_throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Node(1, 1, 0, 1, 1));
            Assert.Throws<ArgumentException>(() => new Node(1, 1, 1, -1, 1));
        }

        [Test]
        public void test_Node_constructor_corrects_sampleSizes()
        {
            /*sampleSize less than 1 should correct itself to 1, and over 100
             should correct itself to 100.*/
            Node n = new Node(1, 1, 1, 1, 0);
            Assert.AreEqual(n.SimulationSampleSize, 1);
            Node n2 = new Node(1, 1, 1, 1, 101);
            Assert.AreEqual(n2.SimulationSampleSize, 100);
        }

        [Test]
        public void test_Node_constructor_corrects_DrawLimit()
        {
            /*drawLimit less than 0 should correct itself to 0, and over 10
             should correct itself to 10.*/
            Node n = new Node(1, -0.1, 1, 1, 1);
            Assert.AreEqual(n.DrawLimit, 0);
            Node n2 = new Node(1, 10.1, 1, 1, 1);
            Assert.AreEqual(n2.DrawLimit, 10);
        }

        [Test]
        public void test_Node_constructor_corrects_PlayLimit()
        {
            /*playLimit less than 0 should correct itself to 0, and over 5
             should correct itself to 5.*/
            Node n = new Node(-0.1, 1, 1, 1, 1);
            Assert.AreEqual(n.PlayLimit, 0);
            Node n2 = new Node(5.1, 1, 1, 1, 1);
            Assert.AreEqual(n2.PlayLimit, 5);
        }

        [Test]
        public void test_Node_random_constructor_produces_valid_samplesizes()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                Node n = new Node(rand, 1);
                n.SimulationSampleSize.Should().BeInRange(1, 100);
            }

        }

        [Test]
        public void test_Node_random_constructor_produces_valid_PlayLimits()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                Node n = new Node(rand, 1);
                n.PlayLimit.Should().BeInRange(0.0, 5.0);
            }
        }

        [Test]
        public void test_Node_random_constructor_produces_valid_DrawLimits()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                Node n = new Node(rand, 1);
                n.DrawLimit.Should().BeInRange(0.0, 10.0);
            }
        }

        [Test]
        public void test_Node_random_constructor_produces_Generation_is_0()
        {
            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                Node n = new Node(rand, 1);
                n.Generation.Should().Be(0);
            }
        }

        [Test]
        public void test_EvaluateFitness_runs()
        {
            Matches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(13);
            Node node = new Node(2.8, 3.14, 5, 0, 5);
            Matches.CreateMatchDataStructs(sample, node.SimulationSampleSize);
            Assert.DoesNotThrow(() => node.EvaluateFitness(sample));
        }

        [Test]
        public void test_EvaluateFitness_performance_Average_LessThan1000ms()
        {
            List<Node> nodes = new List<Node>();
            List<long> runtimes = new List<long>();
            string path = "test-files/data.sqlite3";
            Matches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(13);

            for (var i = 0; i < 100; i++)
            {
                nodes.Add(new Node(2.8, 3.14, 5, 0, 5));
            }
            Stopwatch sw = new Stopwatch();
            Matches.CreateMatchDataStructs(sample, nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize);
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
            Matches.SetMatches(path);
            List<Match> sample = Sample.CreateSample(13);

            for (var i = 0; i < 100; i++)
            {
                nodes.Add(new Node(2.8, 3.14, 5, 0, 5));
            }
            Stopwatch sw = new Stopwatch();
            Matches.CreateMatchDataStructs(sample, nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize);
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
