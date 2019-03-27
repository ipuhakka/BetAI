using System;
using System.Collections.Generic;
using NUnit.Framework;
using Database;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

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
            db.ExecuteScript("db_testdata_dump.sql");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            db.DeleteDatabase(path);
        }

        [Test]
        public void Test_ExecuteScript_return_1()
        {
            var database = new DB("testDB.db");
            database.CreateDatabase("testDB.db");

            var result = database.ExecuteScript("db_schema_dump.sql");
            database.DeleteDatabase("testDB.db");
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Trying to get schema from file with incorrect SQLite syntax throws an SQLiteException.
        /// </summary>
        [Test]
        public void Test_ExecuteScript_throw_SQLiteException()
        {
            var database = new DB("testDB.db");
            database.CreateDatabase("testDB.db");

            Assert.Throws<SQLiteException>(() => database.ExecuteScript(@"..\..\DatabaseTestProject\test-files\db_schema_throws_SQLiteE_dump.sql"));
            database.DeleteDatabase("testDB.db");
        }

        [Test]
        public void Test_ExecuteScript_Unexisting_File_throws_SQLiteException()
        {
            var database = new DB("unusedDatabase.db");

            Assert.Throws<SQLiteException>(() => database.ExecuteScript(@"..\..\DatabaseTestProject\test-files\db_schema_throws_SQLiteE_dump.sql"));
        }

        /// <summary>
        /// Checks that DeleteMatches deletes correct amount of matches.
        /// </summary>
        /// <returns></returns>
        [Test]
        public void Test_DeleteMatches()
        {
            var matches = new List<Match>
            {
                new Match("ManU", "Nor", "England", "2016-2017", new DateTime(2018, 1, 1), 2, 1, 1.34, 3.1, 4.2),
                new Match("ManU", "Liverpool", "England", "2016-2017", new DateTime(2018, 1, 2), 2, 1, 1.34, 3.1, 4.2),
                new Match("ManU", "Nor", "England", "2016-2017", new DateTime(2018, 1, 3), 2, 1, 1.34, 3.1, 4.2)
            };

            db.AddMatches(matches);

            Assert.AreEqual(3, db.DeleteMatches(matches));
        }

        [Test]
        public void Test_AddMatches_Invalid_format_throws_ArgumentException()
        {
            var matches = new List<Match>
            {
                new Match("ManU", "Nor", "England", "2016/2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2)
            };

            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        [Test]
        public void Test_AddMatches_Season_TooLong_throws_ArgumentException()
        {
            var matches = new List<Match>
            {
                new Match("ManU", "Nor", "England", "20162-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2)
            };

            Assert.Throws<ArgumentException>(() => db.AddMatches(matches));
        }

        [Test]
        public void Test_AddMatches_Season_TooShort_throws_ArgumentException()
        {
            var matches = new List<Match>
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
            var matches = new List<Match>();

            for (int i = 0; i < 3; i++)
            {
                matches.Add(new Match("ManU", "Nor", "England", "2016-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            }

            var result = db.AddMatches(matches);
            db.DeleteMatches(matches);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Adding matches to file which doesn't exist should throw SQLiteException
        /// </summary>
        [Test]
        public void Test_AddMatches_throws_SQLiteException()
        {
            var database = new DB("noExist.db");
            var matches = new List<Match>();

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
            var matches = new List<Match>();

            for (int i = 0; i < 380; i++)
            {
                matches.Add(new Match(i.ToString(), (i + 1).ToString(), "England", "2016-2017", new DateTime(2018, 9, 23), 2, 1, 1.34, 3.1, 4.2));
            }

            var sw = new Stopwatch();
            sw.Start();
            var count = db.AddMatches(matches);
            sw.Stop();
            db.DeleteMatches(matches);

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
            var test = new DB("unexistingfile");

            Assert.Throws<SQLiteException>(() => test.SelectAllMatchesFromDatabase());
        }

        /// <summary>
        /// Tests that two wagers with identical bet, odd, and match lists (team names, odd)
        /// are not added. 
        /// </summary>
        [Test]
        public void Test_AddWagers_Identical_Match_Lists_and_Wager_Info_Not_Added()
        {
            List<Match> matchList1 = new List<Match>
            {
                new Match("team1", "team2", 2, 3, 2, DateTime.Today){ SimulatedResult = '1' },
                new Match("team3", "team4", 2, 4, 1, DateTime.Today){ SimulatedResult = '2' }
            };
            List<Match> matchList2 = new List<Match>
            {
                new Match("team2", "team3", 5, 3, 1, DateTime.Today){ SimulatedResult = 'X' },
                new Match("team3", "team4", 2, 4, 1, DateTime.Today){ SimulatedResult = '1' }
            };

            List<Wager> wagers = new List<Wager>
            {
                new Wager(matchList1, 2),
                new Wager(matchList2, 2),
                new Wager(matchList1, 2),
            };

            Assert.AreEqual(2, db.AddWagers(wagers, "testAuthor"));
            db.ClearWagersAndBets();
        }

        /// <summary>
        /// Identical wagers should not be addes to database on different updates.
        /// </summary>
        [Test]
        public void Test_AddWagers_Same_Wager_In_Different_Update_Fails()
        {
            List<Match> matchList1 = new List<Match>
            {
                new Match("team1", "team2", 2, 3, 2, DateTime.Today){ SimulatedResult = '1' },
                new Match("team3", "team4", 2, 4, 1, DateTime.Today){ SimulatedResult = '2' }
            };
            List<Match> matchList2 = new List<Match>
            {
                new Match("team2", "team3", 5, 3, 1, DateTime.Today){ SimulatedResult = 'X' },
                new Match("team3", "team4", 2, 4, 1, DateTime.Today){ SimulatedResult = '1' }
            };

            List<Wager> wagers = new List<Wager>
            {
                new Wager(matchList1, 2),
                new Wager(matchList2, 2),
            };

            Assert.AreEqual(2, db.AddWagers(wagers, "testAuthor"));
            Assert.AreEqual(0, db.AddWagers(new List<Wager> { new Wager(matchList1, 2) }, "testAuthor"));
            db.ClearWagersAndBets();
        }

        /// <summary>
        /// Creates six new wagers (3 for testAuthor and 3 for anotherTestAuthor), and
        /// tests that GetWagersFromAuthor returns 3 wagers for testAuthor.
        /// </summary>
        [Test]
        public void Test_GetWagersFromAuthor()
        {
            List<Match> matchList1 = new List<Match>
            {
                new Match("team1", "team2", 2, 3, 2, new DateTime(2019, 3, 3)){ SimulatedResult = '1', Season="2018-2019"},
                new Match("team3", "team4", 2, 4, 1, new DateTime(2019, 3, 3)){ SimulatedResult = '2', Season="2018-2019"}
            };
            List<Match> matchList2 = new List<Match>
            {
                new Match("team2", "team3", 5, 3, 1, new DateTime(2019, 3, 3)){ SimulatedResult = 'X', Season="2018-2019" },
                new Match("team1", "team4", 2, 4, 1, new DateTime(2019, 3, 3)){ SimulatedResult = '1', Season="2018-2019" }
            };
            List<Match> matchList3 = new List<Match>
            {
                new Match("team2", "team6", 5, 3, 1, new DateTime(2019, 3, 3)){ SimulatedResult = 'X', Season="2018-2019" },
                new Match("team5", "team4", 2, 4, 1, new DateTime(2019, 3, 4)){ SimulatedResult = '1', Season="2018-2019" }
            };

            List<Wager> wagers = new List<Wager>
            {
                new Wager(matchList1, 2),
                new Wager(matchList2, 2),
                new Wager(matchList3, 5),
            };

            db.AddWagers(wagers, "testAuthor");
            db.AddWagers(wagers, "anotherTestAuthor");

            List<Wager> receivedWagers = db.GetWagersFromAuthor("testAuthor");
            Assert.AreEqual(3, receivedWagers.Count);
            char[] expectedChars = new char[] { '1', 'X', '2' };
            receivedWagers.ForEach(wager => 
            {
                Assert.AreEqual("testAuthor", wager.Author);
                wager.Matches.ForEach(match => 
                {
                    Assert.IsTrue(expectedChars.Contains(match.SimulatedResult));
                });
            });
            db.ClearWagersAndBets();
        }

        /// <summary>
        /// Creates three wagers: Puts results to bets of two wagers,
        /// and updates wagers. This should then return the number of updated wagers.
        /// TODO: After implementing getting wager info, do a more thorough check, check
        /// if results are updated appropriately.
        /// </summary>
        [Test]
        public void Test_UpdateWagers()
        {
            List<Match> matchList1 = new List<Match>
            {
                new Match("team1", "team2", 2, 3, 2, new DateTime(2019, 3, 3)){ SimulatedResult = '1', Season="2018-2019"},
                new Match("team3", "team4", 2, 4, 1, new DateTime(2019, 3, 3)){ SimulatedResult = '1', Season="2018-2019"}
            };
            List<Match> matchList2 = new List<Match>
            {
                new Match("team2", "team3", 5, 3, 1, new DateTime(2019, 3, 3)){ SimulatedResult = 'X', Season="2018-2019" },
                new Match("team1", "team4", 2, 4, 1, new DateTime(2019, 3, 3)){ SimulatedResult = '1', Season="2018-2019" }
            };
            List<Match> matchList3 = new List<Match>
            {
                new Match("team2", "team6", 5, 3, 1, new DateTime(2019, 3, 3)){ SimulatedResult = 'X', Season="2018-2019" },
                new Match("team5", "team4", 2, 4, 1, new DateTime(2019, 3, 4)){ SimulatedResult = '1', Season="2018-2019" }
            };

            List<Wager> wagers = new List<Wager>
            {
                new Wager(matchList1, 2),
                new Wager(matchList2, 2),
                new Wager(matchList3, 5),
            };

            Assert.AreEqual(3, db.AddWagers(wagers, "testAuthor"));

            matchList1[0] = new Match(matchList1[0].Hometeam, matchList1[0].Awayteam,
                "league", matchList1[0].Season, matchList1[0].Date, 2, 1,
                matchList1[0].HomeOdd, matchList1[0].DrawOdd, matchList1[0].AwayOdd);

            matchList1[1] = new Match(matchList1[1].Hometeam, matchList1[1].Awayteam,
               "league", matchList1[1].Season, matchList1[1].Date, 2, 1,
               matchList1[1].HomeOdd, matchList1[1].DrawOdd, matchList1[1].AwayOdd);

            matchList2[0] = new Match(matchList2[0].Hometeam, matchList2[0].Awayteam,
               "league", matchList2[0].Season, matchList2[0].Date, 2, 0,
               matchList2[0].HomeOdd, matchList2[0].DrawOdd, matchList2[0].AwayOdd);

            matchList2[1] = new Match(matchList2[1].Hometeam, matchList2[1].Awayteam,
               "league", matchList2[1].Season, matchList2[1].Date, 2, 1,
               matchList2[1].HomeOdd, matchList2[1].DrawOdd, matchList2[1].AwayOdd);

            db.AddMatches(matchList1);
            db.AddMatches(matchList2);

            Assert.AreEqual(2, db.UpdateWagers());
            var wagersAfterUpdate = db.GetWagersFromAuthor("testAuthor");
            Assert.AreEqual(1, wagersAfterUpdate //won wager
                .Where(wager => wager.Result == 1)
                .ToList()
                .Count);

            Assert.AreEqual(1, wagersAfterUpdate //unresolved wager
               .Where(wager => wager.Result == 0)
               .ToList()
               .Count);

            Assert.AreEqual(1, wagersAfterUpdate //lost wager
               .Where(wager => wager.Result == -1)
               .ToList()
               .Count);

            db.DeleteMatches(matchList1.Concat(matchList2).ToList());
            db.ClearWagersAndBets();
        }
    }
}
