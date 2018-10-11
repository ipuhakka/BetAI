using System;
using NUnit.Framework;
using BetAI.FileOperations;
using System.IO;
using BetAI.Exceptions;
using Newtonsoft.Json;

namespace BetAITestProject.FileOperations
{
    [TestFixture]
    public class SaveTest
    {
        string testFile = "test";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            var dir = new DirectoryInfo(Path.Combine(@"Files\", testFile));
            dir.Delete(true);
        }

        [Test]
        public void test_InitializeSave_runs()
        {
            Assert.DoesNotThrow(() => Save.InitializeSave(testFile));
        }

        [Test]
        public void test_InitializeSave_throws_DirectoryExistsException()
        {
            Directory.CreateDirectory(Path.Combine(@"Files\", testFile));
            Assert.Throws<DirectoryExistsException>(() => Save.InitializeSave(testFile));
        }

        [Test]
        public void test_InitializeSave_sets_defined_minimumStake()
        {
            Save.InitializeSave(testFile, String.Format("minimumStake={0}", 2));
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(@"Files\", testFile, "values.json")));
            Assert.AreEqual("2", data["minimumStake"].ToString());
        }

        [Test]
        public void test_InitializeSave_throw_ArgumentOutOfRangeException()
        {
            string[] args = { "minimumStake" };
            Assert.Throws<IndexOutOfRangeException>(() => Save.InitializeSave(testFile, args));
        }

        [Test]
        public void test_InitializeSave_throw_FormatException()
        {
            string[] args = { "minimumStake=not_a_double" };
            Assert.Throws<FormatException>(() => Save.InitializeSave(testFile, args));
        }

        [Test]
        public void test_InitializeSave_fixes_database_path()
        {
            string[] args = { @"database=backslashes\need\to\be\escaped"};
            Save.InitializeSave(testFile, args);
            dynamic data = JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(@"Files\", testFile, "values.json")));
            Assert.AreEqual(@"backslashes\\need\\to\\be\\escaped", data["database"].ToString());
        }
    }
}
