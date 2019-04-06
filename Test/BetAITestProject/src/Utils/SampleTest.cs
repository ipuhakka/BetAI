using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using BetAI.Utils;
using BetAI.BetSim;
using BetAI.Exceptions;
using NUnit.Framework;
using Database;
using FluentAssertions;

namespace Utils
{
    [TestFixture]
    public class SampleTest
    {
        private DB db;
        private readonly string file = @"testi.db";
        private readonly string largeFile = @"..\test-files\data.sqlite3";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\Database\db"));
            db = new DB(file);
            db.CreateDatabase(file);
            db.ExecuteScript("db_schema_dump.sql");
            db.ExecuteScript("db_testdata_dump.sql");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            db.DeleteDatabase(file);
        }

        [Test]
        public void Test_Sample_sampleSizeLargerThanMatchCount()
        {
            Assert.Throws<NotEnoughDataException>(() => Sample.CreateSample(14));
        }

        [Test]
        public void Test_Sample_sampleSizeEqualToMatchCount()
        {
            Matches.SetMatches(file);
            Assert.DoesNotThrow(() => Sample.CreateSample(13));
        }

        [Test]
        public void Test_NoDuplicatesInSample()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\"));
            Matches.SetMatches(largeFile);
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 100; i++)
            {
                sw.Start();
                List<Match> sample = Sample.CreateSample(10);
                sw.Stop();
                Console.WriteLine("Took " + sw.ElapsedMilliseconds);
                sw.Reset();
                sample.Should().OnlyHaveUniqueItems();
            }
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\Database\db"));
        }

        /// <summary>
        /// As sampling is only done once in generation, it is not necessary
        /// for it to be handled in times such as 100ms. Handling a sample
        /// of 2000 matches in less than 2 seconds is a minimum goal.
        /// </summary>
        [Test]
        public void Test_Sample_Performance()
        {
            List<long> runTimes = new List<long>();
            string pathToTestFile = Path.Combine(TestContext.CurrentContext.TestDirectory, @"test-files\data.sqlite3");
            Matches.SetMatches(pathToTestFile);
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 10; i++)
            {
                sw.Start();
                List<Match> sample = Sample.CreateSample(2000);
                sw.Stop();
                runTimes.Add(sw.ElapsedMilliseconds);
                sw.Reset();
            }
            Console.WriteLine(runTimes.Average());
            Assert.LessOrEqual(runTimes.Average(), 2000);
        }
    }
}
