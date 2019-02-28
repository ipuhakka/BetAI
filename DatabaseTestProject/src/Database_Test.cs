using System;
using System.Collections.Generic;
using NUnit.Framework;
using Database;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace DatabaseTestProject
{
    [TestFixture]
    public class Database_Test
    {
        readonly string path = "Database_testi.db";
        DB db;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
            db = new DB(path);
            db.CreateDatabase(path);
            db.ExecuteScript("db_schema_dump.sql");
        }

        [SetUp]
        public void SetUp()
        {
            db.ExecuteScript("db_testdata_dump.sql");
        }

        [TearDown]
        public void TearDown()
        {
            db.ClearDatabase();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            db.DeleteDatabase(path);
        }

        [Test]
        public void Test_ExecuteScript_return_1()
        {
            DB database = new DB("testDB.db");
            database.CreateDatabase("testDB.db");
            int result = database.ExecuteScript("db_schema_dump.sql");
            database.DeleteDatabase("testDB.db");
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Trying to get schema from file with incorrect SQLite syntax throws an SQLiteException.
        /// </summary>
        [Test]
        public void Test_ExecuteScript_throw_SQLiteException()
        {
            DB database = new DB("testDB.db");
            database.CreateDatabase("testDB.db");
            Assert.Throws<SQLiteException>(() => database.ExecuteScript(@"..\..\DatabaseTestProject\test-files\db_schema_throws_SQLiteE_dump.sql"));
            database.DeleteDatabase("testDB.db");
        }

        [Test]
        public void Test_ExecuteScript_Unexisting_File_throws_SQLiteException()
        {
            DB database = new DB("unusedDatabase.db");
            Assert.Throws<SQLiteException>(() => database.ExecuteScript(@"..\..\DatabaseTestProject\test-files\db_schema_throws_SQLiteE_dump.sql"));
        }

        [Test]
        public void Test_AddMatches_Invalid_format_throws_ArgumentException()
        {
            List<Match> matches = new List<Match>
            {
                new Match("ManU", "Nor", "England", "2016/2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2)
            };
            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        [Test]
        public void Test_AddMatches_Season_TooLong_throws_ArgumentException()
        {
            List<Match> matches = new List<Match>
            {
                new Match("ManU", "Nor", "England", "20162-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2)
            };
            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        [Test]
        public void Test_AddMatches_Season_TooShort_throws_ArgumentException()
        {
            List<Match> matches = new List<Match>
            {
                new Match("ManU", "Nor", "England", "201-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2)
            };
            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        /// <summary>
        /// Adding a same match (same date, hometeam and away team)
        /// more than once should produce only one added row.
        /// </summary>
        [Test]
        public void Test_AddMatches_return1()
        {
            List<Match> matches = new List<Match>();
            for (int i = 0; i < 3; i++)
            {
                matches.Add(new Match("ManU", "Nor", "England", "2016-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            }
            int result = db.AddMatches(matches);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Adding matches to file which doesn't exist should throw SQLiteException
        /// </summary>
        [Test]
        public void Test_AddMatches_throws_SQLiteException()
        {
            DB database = new DB("noExist.db");
            List<Match> matches = new List<Match>();
            for (int i = 0; i < 3; i++)
            {
                matches.Add(new Match("ManU", "Nor", "England", "2016-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            }
            Assert.Throws<SQLiteException>(() => database.AddMatches(matches));
        }

        /// <summary>
        /// 380 matches is standard for 20 team football league, so 
        /// it is used to test performance. Ideally, transaction takes no more than 2 seconds.
        /// </summary>
        [Test]
        public void Test_AddMatches_380()
        {
            List<Match> matches = new List<Match>();
            for (int i = 0; i < 380; i++)
            {
                matches.Add(new Match(i.ToString(), (i + 1).ToString(), "England", "2016-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int count = db.AddMatches(matches);
            sw.Stop();
            Console.WriteLine("took " + sw.ElapsedMilliseconds + " milliseconds");
            Assert.LessOrEqual(sw.ElapsedMilliseconds, 2000);
            Assert.AreEqual(380, count);
        }

        [Test]
        public void Test_SelectAllFromDatabase()
        {
            Assert.AreEqual(13, db.SelectAllMatchesFromDatabase().Count);
        }

        [Test]
        public void Test_SelectAllFromDatabase_throws_SQLiteException()
        {
            DB test = new DB("unexistingfile");
            Assert.Throws<SQLiteException>(() => test.SelectAllMatchesFromDatabase());
        }

        /// <summary>
        /// Tests that firstNotUpdatedDate is updated.
        /// </summary>
        [Test]
        public void Test_UpdateFirstNotUpdated()
        {
            DateTime previous = db.GetFirstNotUpdated();
            int affected = db.UpdateFirstNotUpdated(new DateTime(2019, 12, 12));
            Assert.AreEqual(1, affected);
            Assert.AreNotEqual(previous, db.GetFirstNotUpdated());
        }

    }
}
