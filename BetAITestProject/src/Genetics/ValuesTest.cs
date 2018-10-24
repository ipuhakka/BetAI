using System;
using System.IO;
using BetAI.FileOperations;
using BetAI.Genetics;
using NUnit.Framework;

namespace BetAITestProject.Genetics
{
    [TestFixture]
    public class ValuesTest
    {
        private string test = "test";
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            var dir = new DirectoryInfo(Path.Combine(@"Files\", test));
            if (dir.Exists)
                dir.Delete(true);

            var dir2 = @"Files";
            if (File.Exists(dir2))
                File.Delete(dir2);
        }

        [Test]
        public void test_Default_Database_Exists()
        {
            Save.InitializeSave(test);
            Values values = Load.LoadValues(test);
            FileInfo file = new FileInfo(values.Database);
            Console.WriteLine(file.FullName);
            Console.WriteLine(values.Database);
            Assert.True(File.Exists(values.Database));
        }
    }
}
