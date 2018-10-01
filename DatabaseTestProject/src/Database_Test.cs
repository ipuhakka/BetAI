using System;
using System.Collections.Generic;
using NUnit.Framework;
using Database;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace DatabaseTestProject
{
    [Category("DatabaseTests")]
    [TestFixture]
    public class Database_Test
    {
        string path = "Database_testi.db";
        DB db;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
            db = new DB(path);
            db.CreateDatabase(path);
            db.ExecuteScript("db_schema_dump.sql");
        }

        [Category("use_testdatabase")]
        [SetUp]
        public void SetUp()
        {
            db.ExecuteScript("db_testdata_dump.sql");
        }

        [Category("use_testdatabase")]
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
        public void test_ExecuteScript_return_1()
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
        public void test_ExecuteScript_throw_SQLiteException()
        {
            DB database = new DB("testDB.db");
            database.CreateDatabase("testDB.db");
            Assert.Throws<SQLiteException>(() => database.ExecuteScript(@"..\..\DatabaseTestProject\test-files\db_schema_throws_SQLiteE_dump.sql"));
            database.DeleteDatabase("testDB.db");
        }

        [Test]
        public void test_ExecuteScript_Unexisting_File_throws_SQLiteException()
        {
            DB database = new DB("unusedDatabase.db");
            Assert.Throws<SQLiteException>(() => database.ExecuteScript(@"..\..\DatabaseTestProject\test-files\db_schema_throws_SQLiteE_dump.sql"));
        }

        [Test]
        public void test_AddSeason_Invalid_format_throws_ArgumentException()
        {
            List<Match> matches = new List<Match>();
            matches.Add(new Match("ManU", "Nor", "England", "2016/2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        [Test]
        public void test_AddMatches_Season_TooLong_throws_ArgumentException()
        {
            List<Match> matches = new List<Match>();
            matches.Add(new Match("ManU", "Nor", "England", "20162-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        [Test]
        public void test_AddMatches_Season_TooShort_throws_ArgumentException()
        {
            List<Match> matches = new List<Match>();
            matches.Add(new Match("ManU", "Nor", "England", "201-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        /// <summary>
        /// Adding a same match (same date, hometeam and away team)
        /// more than once should produce only one added row.
        /// </summary>
        [Test]
        public void test_AddMatches_return1()
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
        public void test_AddMatches_throws_SQLiteException()
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
        public void test_AddMatches_380()
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

        /// <summary>
        /// ManU has over three homematches before date 2017-11-12, so function
        /// should return a list of three previous matches.
        /// </summary>
        [Test]
        public void test_SelectNLastFromTeam_3homeMatches()
        {
            List<Match> matches = db.SelectNLastFromTeam(true, 3, new DateTime(2017, 11, 12), "ManU");
            Assert.AreEqual(3, matches.Count);
            Assert.AreEqual(new DateTime(2017, 11, 05), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 10, 28), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 7), matches[2].Date);
        }

        [Test]
        public void test_SelectNLastFromTeam_3_awayMatches()
        {
            List<Match> matches = db.SelectNLastFromTeam(false, 3, new DateTime(2017, 12, 03), "Chelsea");
            Assert.AreEqual(3, matches.Count);
            Assert.AreEqual(new DateTime(2017, 11, 12), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 09, 23), matches[2].Date);
        }

        /// <summary>
        /// When a team doesn't have N previous home/away matches before specified date,
        /// N previous total matches are searched for.
        /// </summary>
        [Test]
        public void test_SelectNLastFromTeam_4Matches()
        {
            List<Match> matches = db.SelectNLastFromTeam(false, 4, new DateTime(2017, 12, 03), "Chelsea");
            Assert.AreEqual(4, matches.Count);
            Assert.AreEqual(new DateTime(2017, 11, 26), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 11, 12), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 21), matches[2].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), matches[3].Date);
        }

        [Test]
        public void test_SelectNLastFromTeam_5Matches()
        {
            List<Match> matches = db.SelectNLastFromTeam(false, 5, new DateTime(2017, 12, 03), "Chelsea");
            Assert.AreEqual(5, matches.Count);
            Assert.AreEqual(new DateTime(2017, 11, 26), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 11, 12), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 21), matches[2].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), matches[3].Date);
            Assert.AreEqual(new DateTime(2017, 09, 23), matches[4].Date);
        }

        /// <summary>
        /// If not enough matches before specified date are not found, exception
        /// is thrown.
        /// </summary>
        [Test]
        public void test_SelectNLastFromTeam_throws_NotEnoughDataException()
        {
            Assert.Throws<NotEnoughDataException>(() => db.SelectNLastFromTeam(false, 6, new DateTime(2017, 12, 03), "Chelsea"));
        }

        [Test]
        public void test_SelectNthRow()
        {
            Match m = new Match("ManU", "Cardiff", "England", "2016-2017", new DateTime(2017, 10, 28), 2, 0, 2.2, 3.15, 2.7);
            Match compared = db.SelectNthRow(5);
            Assert.AreEqual(m.Hometeam, compared.Hometeam);
            Assert.AreEqual(m.Awayteam, compared.Awayteam);
            Assert.AreEqual(m.Date, compared.Date);
        }

        [Test]
        public void test_SelectNthRow_zero_index()
        {
            Match m = new Match("ManU", "Chelsea", "England", "2016-2017",new DateTime(2017, 09, 23), 2, 1, 2.2, 3.15, 2.7);
            Match compared = db.SelectNthRow(0);
            Assert.AreEqual(m.Hometeam, compared.Hometeam);
            Assert.AreEqual(m.Awayteam, compared.Awayteam);
            Assert.AreEqual(m.Date, compared.Date);
        }

        [Test]
        public void test_SelectNthRow_Under_Zero_Throws_IndexOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() => db.SelectNthRow(-1));
        }

        [Test]
        public void test_SelectNthRow_Over_Count_Throws_IndexOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() => db.SelectNthRow(13));
        }

        [Test]
        public void test_SelectCount_emptyTable()
        {
            db.ClearDatabase();
            Assert.AreEqual(0, db.SelectCount());
        }

        [Test]
        public void test_SelectCount()
        {
            Assert.AreEqual(13, db.SelectCount());
        }

        [Test]
        public void test_HomeAVGBeforeDate()
        {
            Assert.AreEqual(1.5, Math.Round((double) db.LeagueHomeAVGBeforeDate(new DateTime(2017, 11, 05), "2016-2017", "England"), 2));
        }

        [Test]
        public void test_HomeAVGBeforeDate_Match_object()
        {
            Match m = db.SelectNthRow(7);
            Assert.AreEqual(1.43, Math.Round((double)db.LeagueHomeAVGBeforeDate(m.Date, m.Season, m.League), 2));
        }

        [Test]
        public void test_HomeAVGBeforeDate_Unexisting_league_NotEnoughDataException()
        {
            Assert.Throws<NotEnoughDataException>(() => db.LeagueHomeAVGBeforeDate(new DateTime(2017, 11, 05), "2016-2017", "Unexisting league"));
        }

        [Test]
        public void test_HomeAVGBeforeDate_Unexisting_season_NotEnoughDataException()
        {
            Assert.Throws<NotEnoughDataException>(() => db.LeagueHomeAVGBeforeDate(new DateTime(2017, 11, 05), "2016-2014", "England"));
        }

        [Test]
        public void test_AwayAVGBeforeDate()
        {
            Assert.AreEqual(1.17, Math.Round((double)db.LeagueAwayAVGBeforeDate(new DateTime(2017, 11, 05), "2016-2017", "England"), 2));
        }

        [Test]
        public void test_AwayAVGBeforeDate_Unexisting_league_throws_NotEnoughDataException()
        {
            Assert.Throws<NotEnoughDataException>(() => db.LeagueAwayAVGBeforeDate(new DateTime(2017, 11, 05), "2016-2017", "Unexisting league"));
        }

        [Test]
        public void test_AwayAVGBeforeDate_Unexisting_season_NotEnoughDataException()
        {
            Assert.Throws<NotEnoughDataException>(() => db.LeagueAwayAVGBeforeDate(new DateTime(2017, 11, 05), "2016-2014", "England"));
        }
    }
}
