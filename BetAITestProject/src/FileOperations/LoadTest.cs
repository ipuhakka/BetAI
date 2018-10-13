using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.FileOperations;
using BetAI.Genetics;

namespace BetAITestProject.FileOperations
{
    [TestFixture]
    public class LoadTest
    {
        private string save = "test";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            var dir = new DirectoryInfo(Path.Combine("Files", save));
            if (dir.Exists)
                dir.Delete(true); 
        }

        /// <summary>
        /// If the savefile is not created, DirectoryNotFoundException should be thrown.
        /// </summary>
        [Test]
        public void test_LoadGeneration_throws_DirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => Load.LoadLatestGeneration("unexistingsave"));
        }

        /// <summary>
        /// If savefile is created but it has no gen{i}.json-files in it, 
        /// </summary>
        [Test]
        public void test_LoadGeneration_return_null()
        {
            Save.InitializeSave(save);
            Assert.IsNull(Load.LoadLatestGeneration(save));
        }

        [Test]
        public void test_LoadGeneration()
        {
            List<Node> nodes = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 3; i++)
            {
                nodes.Add(new Node(rand, 2));
            }
            Save.InitializeSave(save);
            Save.WriteGeneration(save, nodes, nodes[0].Generation);
            List<Node> loadedGen = Load.LoadLatestGeneration(save);
            Assert.AreEqual(nodes, loadedGen);
        }

        /// <summary>
        /// This test checks that correct generation of nodes is returned
        /// when the value indicating generation number consists of more than
        /// 1 digits.
        /// </summary>
        [Test]
        public void test_LoadGeneration_10generations()
        {
            List<Node> nodes = new List<Node>();
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                nodes.Add(new Node(rand, 2));
            }
            Save.InitializeSave(save);
            Save.WriteGeneration(save, nodes, 0);
            for (int j = 0; j < 10; j++)
            {
                Crossover co = new Crossover();
                nodes = co.Reproduce(nodes, 0.2);
                Save.WriteGeneration(save, nodes, nodes[0].Generation);
            }
            List<Node> loadedGen = Load.LoadLatestGeneration(save);
            
            foreach(Node n in loadedGen)
            {
                Assert.AreEqual(10, n.Generation);
            }
        }

    }
}
