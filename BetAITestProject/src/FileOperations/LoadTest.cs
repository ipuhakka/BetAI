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
            Assert.Throws<DirectoryNotFoundException>(() => Load.LoadGeneration("unexistingsave"));
        }

        /// <summary>
        /// If savefile is created but it has no gen{i}.json-files in it, 
        /// </summary>
        [Test]
        public void test_LoadGeneration_return_null()
        {
            Save.InitializeSave(save);
            Assert.IsNull(Load.LoadGeneration(save));
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
            List<Node> loadedGen = Load.LoadGeneration(save);
            Assert.AreEqual(nodes, loadedGen);
        }

    }
}
