using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using Database;
using BetAI.Data;
using BetAI.Exceptions;
using System.Data.SQLite;

namespace BetAITestProject.Data
{
    [TestFixture]
    public class QueryMatchesTest
    {
        private string database = "test.db";
        private DB db;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
            db = new DB(database);
            db.CreateDatabase(database);
            db.ExecuteScript("db_schema_dump.sql");
            db.ExecuteScript("db_testdata_dump.sql");
            QueryMatches.SetMatches(database);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            db.DeleteDatabase(database);
        }

        [Test]
        public void test_SelectMatchesWithRowIndex()
        {
            Match m = new Match("ManU", "Cardiff", "England", "2016-2017", new DateTime(2017, 10, 28), 2, 0, 2.2, 3.15, 2.7);
            List<Match> results = QueryMatches.SelectMatchesWithRowIndex(new List<int> { 5 });
            Assert.AreEqual(m, results[0]);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void test_SetMatches_throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => QueryMatches.SetMatches(null));
        }

        [Test]
        public void test_SetMatches_throws_SQLiteException()
        {
            /* If database set does not exist SQLiteException is thrown.*/
            Assert.Throws<SQLiteException>(() => QueryMatches.SetMatches("unexistingDBFile"));
            QueryMatches.SetMatches(database);
        }

        [Test]
        public void test_SelectMatchesWithRowIndex_zero_index()
        {
            Match m = new Match("ManU", "Chelsea", "England", "2016-2017", new DateTime(2017, 09, 23), 2, 1, 2.2, 3.15, 2.7);
            List<Match> results = QueryMatches.SelectMatchesWithRowIndex(new List<int> { 5, 0 });
            Assert.AreEqual(m, results[1]);
            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void test_SelectMatchesByRowIndex_Under_Zero_Throws_IndexOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() => QueryMatches.SelectMatchesWithRowIndex(new List<int> { -1 }));
        }

        [Test]
        public void test_SelectMatchesByRowIndex_Equal_To_Count_Throws_IndexOutOfRangeException()
        {
            Assert.Throws<IndexOutOfRangeException>(() => QueryMatches.SelectMatchesWithRowIndex(new List<int> { 13 }));
        }

        [Test]
        public void test_SelectMatchesByRowIndex_runs_with_more_than_one_value()
        {
            Assert.DoesNotThrow(() => QueryMatches.SelectMatchesWithRowIndex(new List<int> { 1, 2, 5, 6 }));
        }

        [Test]
        public void test_GetMatchCount_returns_13()
        {
            Assert.AreEqual(13, QueryMatches.GetMatchCount());
        }

        [Test]
        public void test_SeasonHomeGoalAvgBeforeDate()
        {
            Match m = db.SelectAllMatchesFromDatabase()[7];
            Assert.AreEqual(1.43, Math.Round((double)QueryMatches.SeasonHomeGoalAvgBeforeDate(m), 2));
        }

        [Test]
        public void test_SeasonHomeGoalAvgBeforeDate_Unexisting_league_NotEnoughDataException()
        {
            Match m = new Match("team1", "team2", "unexistingLeague", "2016-2017", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.SeasonHomeGoalAvgBeforeDate(m));
        }

        [Test]
        public void test_SeasonHomeGoalAvgBeforeDate_Unexisting_season_NotEnoughDataException()
        {
            Match m = new Match("team1", "team2", "England", "2016-2018", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.SeasonHomeGoalAvgBeforeDate(m));
        }
        [Test]
        public void test_SeasonAwayGoalAvgBeforeDate()
        {
            Match m = db.SelectAllMatchesFromDatabase()[6];
            Assert.AreEqual(1.17, Math.Round((double)QueryMatches.SeasonAwayGoalAvgBeforeDate(m), 2));
        }

        [Test]
        public void test_SeasonAwayGoalAvgBeforeDate_Unexisting_league_NotEnoughDataException()
        {
            Match m = new Match("team1", "team2", "unexistingLeague", "2016-2017", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.SeasonAwayGoalAvgBeforeDate(m));
        }

        [Test]
        public void test_SeasonAwayGoalAvgBeforeDate_Unexisting_season_NotEnoughDataException()
        {
            Match m = new Match("team1", "team2", "England", "2016-2018", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.SeasonAwayGoalAvgBeforeDate(m));
        }

        /// <summary>
        /// ManU has over three homematches before date 2017-11-12, so function
        /// should return a list of three previous matches.
        /// </summary>
        [Test]
        public void test_SelectNLastFromTeam_3homeMatches()
        {
            List<Match> matches = QueryMatches.SelectNLastFromTeam(true, 3, new DateTime(2017, 11, 12), "ManU");
            Assert.AreEqual(3, matches.Count);
            Assert.AreEqual(new DateTime(2017, 11, 05), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 10, 28), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 7), matches[2].Date);
        }

        [Test]
        public void test_SelectNLastFromTeam_3_awayMatches()
        {
            List<Match> matches = QueryMatches.SelectNLastFromTeam(false, 3, new DateTime(2017, 12, 03), "Chelsea");
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
            List<Match> matches = QueryMatches.SelectNLastFromTeam(false, 4, new DateTime(2017, 12, 03), "Chelsea");
            Assert.AreEqual(4, matches.Count);
            Assert.AreEqual(new DateTime(2017, 11, 26), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 11, 12), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 21), matches[2].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), matches[3].Date);
        }

        [Test]
        public void test_SelectNLastFromTeam_5Matches()
        {
            List<Match> matches = QueryMatches.SelectNLastFromTeam(false, 5, new DateTime(2017, 12, 03), "Chelsea");
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
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.SelectNLastFromTeam(false, 6, new DateTime(2017, 12, 03), "Chelsea"));
        }
    }
}
