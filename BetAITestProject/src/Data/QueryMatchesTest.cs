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
        private List<Match> matches;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
            db = new DB(database);
            db.CreateDatabase(database);
            db.ExecuteScript("db_schema_dump.sql");
            db.ExecuteScript("db_testdata_dump.sql");
            QueryMatches.SetMatches(database);
            matches = db.SelectAllMatchesFromDatabase();
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
        public void test_SeasonHomeGoalAvgBeforeDate_Unexisting_league_returnMinus1()
        {
            Match m = new Match("team1", "team2", "unexistingLeague", "2016-2017", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.AreEqual(-1, QueryMatches.SeasonHomeGoalAvgBeforeDate(m));
        }

        [Test]
        public void test_SeasonHomeGoalAvgBeforeDate_Unexisting_season_returnMinus1()
        {
            Match m = new Match("team1", "team2", "England", "2016-2018", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.AreEqual(-1, QueryMatches.SeasonHomeGoalAvgBeforeDate(m));
        }
        [Test]
        public void test_SeasonAwayGoalAvgBeforeDate()
        {
            Match m = db.SelectAllMatchesFromDatabase()[6];
            Assert.AreEqual(1.17, Math.Round((double)QueryMatches.SeasonAwayGoalAvgBeforeDate(m), 2));
        }

        [Test]
        public void test_SeasonAwayGoalAvgBeforeDate_Unexisting_league_returnMinus1()
        {
            Match m = new Match("team1", "team2", "unexistingLeague", "2016-2017", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.AreEqual(-1, QueryMatches.SeasonAwayGoalAvgBeforeDate(m));
        }

        [Test]
        public void test_SeasonAwayGoalAvgBeforeDate_Unexisting_season_returnMinus1()
        {
            Match m = new Match("team1", "team2", "England", "2016-2018", new DateTime(2017, 11, 05), 2, 1, 2, 2, 2);
            Assert.AreEqual(-1, QueryMatches.SeasonAwayGoalAvgBeforeDate(m));
        }

        /// <summary>
        /// ManU has over three homematches before date 2017-11-12, so function
        /// should return a list of three previous matches.
        /// </summary>
        [Test]
        public void test_SelectNLastFromTeam_3homeMatches()
        {
            Match[] matches = QueryMatches.SelectNLastFromTeam(true, 3, new DateTime(2017, 11, 12), "ManU");
            Assert.AreEqual(3, matches.Length);
            Assert.AreEqual(new DateTime(2017, 11, 05), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 10, 28), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 7), matches[2].Date);
        }

        [Test]
        public void test_SelectNLastFromTeam_3_awayMatches()
        {
            Match[] matches = QueryMatches.SelectNLastFromTeam(false, 3, new DateTime(2017, 12, 03), "Chelsea");
            Assert.AreEqual(3, matches.Length);
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
            Match[] matches = QueryMatches.SelectNLastFromTeam(false, 4, new DateTime(2017, 12, 03), "Chelsea");
            Assert.AreEqual(4, matches.Length);
            Assert.AreEqual(new DateTime(2017, 11, 26), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 11, 12), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 21), matches[2].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), matches[3].Date);
        }

        [Test]
        public void test_SelectNLastFromTeam_5Matches()
        {
            Match[] matches = QueryMatches.SelectNLastFromTeam(false, 5, new DateTime(2017, 12, 03), "Chelsea");
            Assert.AreEqual(5, matches.Length);
            Assert.AreEqual(new DateTime(2017, 11, 26), matches[0].Date);
            Assert.AreEqual(new DateTime(2017, 11, 12), matches[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 21), matches[2].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), matches[3].Date);
            Assert.AreEqual(new DateTime(2017, 09, 23), matches[4].Date);
        }

        /// <summary>
        /// All matches from team before date are returned, even though
        /// there isn't enough matches for sample size.
        /// </summary>
        [Test]
        public void test_SelectNLastFromTeam_returns5()
        {
            Assert.AreEqual(5, QueryMatches.SelectNLastFromTeam(false, 6, new DateTime(2017, 12, 03), "Chelsea").Length);
        }

        [Test]
        public void test_GetNLastFromTeamBeforeMatch_throws_NotEnoughDataException()
        {
            Match m = new Match("West Ham", "Chelsea", "England", "2016-2017", new DateTime(2017, 12, 03), 0, 2, 2.2, 3.15, 2.7);
            QueryMatches.CreateMatchDataStructs(matches, 6);
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.GetNLastFromTeamBeforeMatch(false, m, 6));
        }

        [Test]
        public void test_GetNLastFromTeamBeforeMatch_throw_NotEnoughDataException()
        {
            /* If max sample size in CreateMatchDataStructs is smaller
             * than the sample size used later in GetNLastFromTeamBeforeMatch, 
             * calling that throws NotEnoughDataException,
              because not enough matches for selected sample size were ever
              set for the structure.*/
            QueryMatches.CreateMatchDataStructs(matches, 4);
            Console.WriteLine(matches[10].Hometeam + matches[10].Awayteam);
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.GetNLastFromTeamBeforeMatch(false, matches[10], 5));
        }

        [Test]
        public void test_GetNLastFromTeamBeforeMatch_returns_5_matches()
        {
            /* Chelsea has 5 matches before game on 2017/12/03, and sampleSize
             of 5 is less than max size of 6, so function call should run and
             return an array of 5 matches.*/
            QueryMatches.CreateMatchDataStructs(matches, 6);
            Console.WriteLine(matches[10].Hometeam + matches[10].Awayteam);
            Match[] results = QueryMatches.GetNLastFromTeamBeforeMatch(false, matches[10], 5);
            Assert.AreEqual(5, results.Length);
            Assert.AreEqual(new DateTime(2017, 11, 26), results[0].Date);
            Assert.AreEqual(new DateTime(2017, 11, 12), results[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 21), results[2].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), results[3].Date);
            Assert.AreEqual(new DateTime(2017, 09, 23), results[4].Date);
        }

        [Test]
        public void test_GetNLastFromTeamBeforeMatch_returns_4_matches()
        {
            /* Chelsea has 5 matches before game on 2017/12/03, and sampleSize
             of 5 is less than max size of 6, so function call should run and
             return an array of 5 matches.*/
            QueryMatches.CreateMatchDataStructs(matches, 6);
            Console.WriteLine(matches[10].Hometeam + matches[10].Awayteam);
            Match[] results = QueryMatches.GetNLastFromTeamBeforeMatch(false, matches[10], 4);
            Assert.AreEqual(4, results.Length);
            Assert.AreEqual(new DateTime(2017, 11, 26), results[0].Date);
            Assert.AreEqual(new DateTime(2017, 11, 12), results[1].Date);
            Assert.AreEqual(new DateTime(2017, 10, 21), results[2].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), results[3].Date);
        }

        [Test]
        public void test_GetNLastFromTeamBeforeMatch_returns_3_awayMatches()
        {
            /* With maxSize of 6, for matches[10] chelsea has been set with 5 matches.
               If GetNLastFromTeamBeforeMatch is called then with sample size n,
               it should return primarily n home- or awaymatches. 
               This test expects that the returned matches are all awaymatches for chelsea.
               If it fails, it means that function most likely returned
               first 3 matches, ignoring whether they are home- or awaymatches.
             */
            QueryMatches.CreateMatchDataStructs(matches, 6);
            Console.WriteLine(matches[10].Hometeam + matches[10].Awayteam);
            Match[] results = QueryMatches.GetNLastFromTeamBeforeMatch(false, matches[10], 3);
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual(new DateTime(2017, 11, 12), results[0].Date);
            Assert.AreEqual(new DateTime(2017, 10, 14), results[1].Date);
            Assert.AreEqual(new DateTime(2017, 09, 23), results[2].Date);
        }

        [Test]
        public void test_GetSeasonAverage_throws_NotEnoughDataException()
        {
            Assert.Throws<NotEnoughDataException>(() => QueryMatches.GetSeasonAverage(true, matches[0]));
        }

        public void test_GetSeasonAverage_runs()
        {
            Assert.DoesNotThrow(() => QueryMatches.GetSeasonAverage(true, matches[1]));
        }

        [Test]
        public void test_CreateMatchDataStructs_Runs()
        {
            /*CreateMatchDataStructs should not throw errors when samplesize is too long.*/
            Assert.DoesNotThrow(() => QueryMatches.CreateMatchDataStructs(matches, 6));
        }
    }
}
