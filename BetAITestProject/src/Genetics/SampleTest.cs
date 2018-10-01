using System.IO;
using System.Linq;
using System.Collections.Generic;
using BetAI.Genetics;
using NUnit.Framework;
using Database;
using System.Data.SQLite;
using FluentAssertions;

namespace BetAITestProject.Genetics
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
            Assert.Throws<SQLiteException>(() => new Sample("unexistingFile", 13));
        }

        [Test]
        public void test_NoDuplicatesInSample()
        {
            List<Sample> samples = new List<Sample>();
            
            for (int i = 0; i < 100; i++)
            {
                Sample sample = new Sample(file, 13);
                sample.Points.Should().OnlyHaveUniqueItems();
            }
        }
    }
}
