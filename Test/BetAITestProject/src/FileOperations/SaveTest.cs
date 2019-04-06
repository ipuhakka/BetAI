using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using BetAI.FileOperations;
using BetAI.Genetics;
using BetAI.Exceptions;
using Newtonsoft.Json;
using Database;

namespace FileOperations
{
    [TestFixture]
    public class SaveTest
    {
        readonly string testFile = "test";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            var dir = new DirectoryInfo(Path.Combine(@"Files\", testFile));
            if (dir.Exists)
                dir.Delete(true);
        }

        [Test]
        public void Test_InitializeSave_runs()
        {
            Assert.DoesNotThrow(() => Save.InitializeSave(testFile));
        }

        [Test]
        public void Test_InitializeSave_throws_DirectoryExistsException()
        {
            Directory.CreateDirectory(Path.Combine(@"Files\", testFile));
            Assert.Throws<DirectoryExistsException>(() => Save.InitializeSave(testFile));
        }

        [Test]
        public void Test_InitializeSave_sets_defined_minimumStake()
        {
            Save.InitializeSave(testFile, String.Format("minimumStake={0}", 2));
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(@"Files\", testFile, "values.json")));
            Assert.AreEqual("2", data["minimumStake"].ToString());
        }

        [Test]
        public void Test_InitializeSave_throw_ArgumentOutOfRangeException()
        {
            string[] args = { "minimumStake" };
            Assert.Throws<IndexOutOfRangeException>(() => Save.InitializeSave(testFile, args));
        }

        [Test]
        public void Test_InitializeSave_throw_FormatException()
        {
            string[] args = { "minimumStake=not_a_double" };
            Assert.Throws<FormatException>(() => Save.InitializeSave(testFile, args));
        }

        [Test]
        public void Test_InitializeSave_double_with_point_runs()
        {
            string[] args = { "minimumStake=0.5" };
            Assert.DoesNotThrow(() => Save.InitializeSave(testFile, args));
        }

        [Test]
        public void Test_InitializeSave_double_with_comma_runs()
        {
            string[] args = { "minimumStake=0,5" };
            Assert.DoesNotThrow(() => Save.InitializeSave(testFile, args));
        }

        [Test]
        public void Test_InitializeSave_fixes_database_path()
        {
            string[] args = { @"database=backslashes\need\to\be\escaped"};
            Save.InitializeSave(testFile, args);
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(@"Files\", testFile, "values.json")));
            Assert.AreEqual(@"backslashes\\need\\to\\be\\escaped", data["database"].ToString());
        }

        [Test]
        public void Test_WriteGeneration_DirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => Save.WriteGeneration(testFile, new List<Node>(), 0));
        }

        [Test]
        public void Test_WriteGeneration_runs()
        {
            Save.InitializeSave(testFile);
            Assert.DoesNotThrow(() => Save.WriteGeneration(testFile, new List<Node>(), 0));
        }

        [Test]
        public void Test_WriteSample_DirectoryNotFoundException()
        {
            Assert.Throws<DirectoryNotFoundException>(() => Save.WriteSample(testFile, new List<Match>(), 0));
        }

        [Test]
        public void Test_WriteSample_runs()
        {
            Save.InitializeSave(testFile);
            Assert.DoesNotThrow(() => Save.WriteSample(testFile, new List<Match>(), 0));
        }

        [Test]
        public void Test_Log_DoesNotOverWriteData()
        {
            Save.InitializeSave(testFile);
            string[] lines = { "Line 1", "Line 2", "Line 3"};
            Save.Log(testFile, lines);

            string[] newLines = { "Line 4", "Line 5" };
            Save.Log(testFile, newLines);

            string[] allLines = File.ReadAllLines(@"Files\test\log.txt");
            Assert.AreEqual(5, allLines.Length);
        }

        [Test]
        public void Test_Log_throws_DirectoryNotFoundException()
        {
            string[] lines = { "Line 1", "Line 2", "Line 3" };
            Assert.Throws<DirectoryNotFoundException>(() => Save.Log(testFile, lines));
        }
    }
}
