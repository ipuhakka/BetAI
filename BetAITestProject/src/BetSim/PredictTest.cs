﻿using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Database;
using BetAI.BetSim;
using BetAI.Exceptions;
using BetAI.Data;

namespace BetAITestProject.BetSim
{
    [TestFixture]
    public class PredictTest
    {
        private DB db;
        private string path = "testi.db";
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\Database\db"));
            db = new DB(path);
            db.CreateDatabase(path);
            db.ExecuteScript("db_schema_dump.sql");
            db.ExecuteScript("db_testdata_dump.sql");
            Matches.SetMatches(path);
            List<Match> matches = db.SelectAllMatchesFromDatabase();
            Matches.CreateMatchDataStructs(matches, 7);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            db.DeleteDatabase(path);
        }

        [Test]
        public void test_PredictResult()
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

            Match toPredict = Matches.SelectMatchesWithRowIndex(new List<int>() { 7 })[0];
            Predict betSim = new Predict();
            double result = betSim.PredictResult(toPredict, 3);
            Console.WriteLine("Result: " + result);
            Assert.AreEqual(-0.54, Math.Round(result, 2));
        }

        [Test]
        public void test_PredictResult_Throws_NotSimulatedException()
        {
            Match toPredict = Matches.SelectMatchesWithRowIndex(new List<int>() { 6 })[0];
            Predict betSim = new Predict();
            Assert.Throws<NotSimulatedException>(() => betSim.PredictResult(toPredict, 3));
        }
    }
}