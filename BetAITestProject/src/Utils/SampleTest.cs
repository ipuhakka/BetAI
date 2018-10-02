using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using BetAI.Utils;
using NUnit.Framework;
using Database;
using System.Data.SQLite;
using FluentAssertions;
using System.Linq;

namespace BetAITestProject.Utils
{
    [TestFixture]
    public class SampleTest
    {
        private DB db;
        private string file = "testi.db";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
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
        public void test_Sample_sampleSizeLargerThanMatchCount()
        {
            Assert.Throws<NotEnoughDataException>(() => new Sample(file, 14));
        }

        [Test]
        public void test_Sample_sampleSizeEqualToMatchCount()
        {
            Assert.DoesNotThrow(() => new Sample(file, 13));
        }

        [Test]
        public void test_Sample_Unexisting_File_Throws_SQLiteException()
        {
            Assert.Throws<SQLiteException>(() => new Sample("unexistingfile", 13));
        }

        [Test]
        public void test_Sample_Invalid_File_Throws_SQLiteException()
        {
            Assert.Throws<SQLiteException>(() => new Sample("db_schema_dump.sql", 13));
        }

        [Test]
        public void test_NoDuplicatesInSample()
        {
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 100; i++)
            {
                sw.Start();
                Sample sample = new Sample(file, 13);
                sw.Stop();
                Console.WriteLine("Took " + sw.ElapsedMilliseconds);
                sw.Reset();
                sample.Matches.Should().OnlyHaveUniqueItems();
            }
        }

        [Test]
        public void test_Sample_Performance()
        {
            List<long> runTimes = new List<long>();
            string pathToTestFile = Path.Combine(TestContext.CurrentContext.TestDirectory, @"test-files\data.sqlite3");
            
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 10; i++)
            {
                sw.Start();
                Sample sample = new Sample(pathToTestFile, 2000);
                sw.Stop();
                runTimes.Add(sw.ElapsedMilliseconds);
                sw.Reset();
            }
            Console.WriteLine(runTimes.Average());
            Assert.LessOrEqual(runTimes.Average(), 2000);
        }
    }
}
