using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using BetAI.FileOperations;
using BetAI.Genetics;
using BetAI.Genetics.Crossover;
using BetAI.Utils;

namespace BetAITestProject.FileOperations
{
    [TestFixture]
    public class LoadTest
    {
        private readonly string save = "test";

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
        public void Test_LoadGeneration_throws_DirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => Load.LoadLatestGeneration("unexistingsave"));
        }

        /// <summary>
        /// If savefile is created but it has no gen{i}.json-files in it, 
        /// </summary>
        [Test]
        public void Test_LoadGeneration_return_null()
        {
            Save.InitializeSave(save);
            Assert.IsNull(Load.LoadLatestGeneration(save));
        }

        [Test]
        public void Test_LoadGeneration()
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
        public void Test_LoadGeneration_10generations()
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
                BLXAlpha co = new BLXAlpha(0.2);
                nodes = co.Crossover(nodes[0], nodes[1]);
                Save.WriteGeneration(save, nodes, nodes[0].Generation);
            }
            List<Node> loadedGen = Load.LoadLatestGeneration(save);
            
            foreach(Node n in loadedGen)
            {
                Assert.AreEqual(10, n.Generation);
            }
        }

        [Test]
        public void SaveExists_existing_Save_return_true()
        {
            Save.InitializeSave(save);
            Assert.IsTrue(Load.SaveExists(save));
        }

        [Test]
        public void SaveExists_unexisting_Save_return_false()
        {
            Assert.IsFalse(Load.SaveExists(save));
        }

        [Test]
        public void Test_LoadValues_throws_DirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => Load.LoadValues(save));
        }

        [Test]
        public void Test_LoadValues()
        {
            Save.InitializeSave(save);
            Values values = Load.LoadValues(save);
            Assert.AreEqual(0.2, values.Alpha);
            Assert.AreEqual(5, values.MinimumStake);
            Assert.AreEqual(200, values.NumberOfNodes);
            Assert.AreEqual(200, values.SampleSize);
            Assert.AreEqual("../../../Database/db/data.sqlite3", values.Database);
        } 
    }
}
