using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Database;
using BetAI.BetSim;
using BetAI.Exceptions;
using Newtonsoft.Json.Linq;

namespace BetSim
{
    [TestFixture]
    public class PredictTest
    {
        private DB db;
        private readonly string path = "testi.db";
        private readonly string largeDatabase = "data.sqlite3";
        private List<Match> matches;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
            db = new DB(path);
            db.CreateDatabase(path);
            db.ExecuteScript("db_schema_dump.sql");
            db.ExecuteScript("db_testdata_dump.sql");
            matches = db.SelectAllMatchesFromDatabase();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            db.DeleteDatabase(path);
        }

        [Test]
        public void Test_PredictResult()
        {
            /*hometeam avg goals= 1.33
              hometeam conceded avg = 2/3
              awayteam avg goals = 2
              awayteam conceded avg = 2/3
              league home goals avg = 10/7,
              league away goals avg = 8/7

            homeatt = 0.931
            homedef = 0.58333
            awayatt = 1.75
            awaydef = 0.46666

            home estimate =  0.931 * 0.46666 * (10/7)
            away estimate = 1.75 * 0.583333 * (8/7)

            0.620578 - 1.16666
             */
            Matches.SetMatches(path);
            Matches.CreateMatchDataStructs(matches, 7);
            Match toPredict = Matches.SelectMatchesWithRowIndex(new List<int>() { 7 })[0];
            Predict betSim = new Predict();
            double result = betSim.PredictResult(toPredict, 3);
            Console.WriteLine("Result: " + result);
            Assert.AreEqual(-0.54, Math.Round(result, 2));
        }

        /// <summary>
        /// This test involves a situation, where a game has been played that season
        /// but no goals were scored. As league goal averages are then 0, this would produce
        /// a NaN strength value, so NotSimulatedException is thrown so that bet is not played
        /// in such a situation.
        /// </summary>
        [Test]
        public void Test_Predict_LaCoruna_Sociedad_15_16_throws_NotSimulatedException()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"test-files"));
            Matches.SetMatches(largeDatabase);
            JArray matches = JArray.Parse(File.ReadAllText("NaNSample.json"));
            List<Match> sample = new List<Match>();
            foreach (JObject obj in matches)
            {
                string homeT = obj["Hometeam"].ToString();
                string awayT = obj["Awayteam"].ToString();
                string league = obj["League"].ToString();
                string season = obj["Season"].ToString();
                DateTime d = Convert.ToDateTime(obj["Date"].ToString());
                int homeS = Convert.ToInt32(obj["Homescore"].ToString());
                int awayS = Convert.ToInt32(obj["Awayscore"].ToString());
                double homeO = Convert.ToDouble(obj["HomeOdd"].ToString());
                double drawO = Convert.ToDouble(obj["DrawOdd"].ToString());
                double awayO = Convert.ToDouble(obj["AwayOdd"].ToString());
                sample.Add(new Match(homeT, awayT, league, season, d, homeS, awayS, homeO, drawO, awayO));
            }
            Matches.CreateMatchDataStructs(sample, 18);
            Predict betSim = new Predict();
            Assert.Throws<NotSimulatedException>(() => betSim.PredictResult(sample[4], 18));
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
        }

        [Test]
        public void Test_PredictResult_Throws_NotSimulatedException()
        {
            Matches.SetMatches(path);
            Matches.CreateMatchDataStructs(matches, 7);
            Match toPredict = Matches.SelectMatchesWithRowIndex(new List<int>() { 6 })[0];
            Predict betSim = new Predict();
            Assert.Throws<NotSimulatedException>(() => betSim.PredictResult(toPredict, 3));
        }

        /// <summary>
        /// Testing that with minimum set PlayLimit and SimulationSampleSize of 3, 
        /// PredictBets predicts both test matches.
        /// </summary>
        [Test]
        public void Test_PredictBets()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"../../test-files/"));

            List<Match> matches = new List<Match>
            {
                new Match("Everton", "Liverpool", 3.4, 3.3, 2.5, DateTime.Now),
                new Match("Real Madrid", "Valencia", 1.89, 3.14, 4.2, DateTime.Now)
            };

            Predict predict = new Predict();
            Assert.AreEqual(2, predict.PredictBets(matches, Path.GetFullPath(@"Files\test"), "2018-2019").Count);
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
        }
    }
}
