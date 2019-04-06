using System;
using System.IO;
using BetAI.FileOperations;
using BetAI.Genetics;
using BetAI.Genetics.Mutation;
using NUnit.Framework;

namespace Genetics
{
    [TestFixture]
    public class ValuesTest
    {
        private readonly string test = "test";
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
        public void Test_InitializeMutationMethod_defaults_return_UniformMutation()
        {
            Save.InitializeSave(test);
            Values values = Load.LoadValues(test);
            IMutation mutate = Values.InitializeMutationMethod(values);
            Assert.IsInstanceOf(typeof(UniformMutation), mutate);
        }

        [Test]
        public void Test_InitializeMutationMethod_runs()
        {
            Save.InitializeSave(test);
            Values values = Load.LoadValues(test);
            Assert.DoesNotThrow(() => Values.InitializeMutationMethod(values));
        }
    }
}
