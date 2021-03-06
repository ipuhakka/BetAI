﻿using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Database;
using BetAI.BetSim;

namespace BetSim
{
    [TestFixture]
    public class BetTest
    {
        private DB db;
        private readonly string path = "testi.db";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Directory.SetCurrentDirectory(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\..\Database\db"));
            db = new DB(path);
            db.CreateDatabase(path);
            db.ExecuteScript("db_schema_dump.sql");
            db.ExecuteScript("db_testdata_dump.sql");
            List<Match> matches = db.SelectAllMatchesFromDatabase();
            Matches.SetMatches(path);
            Matches.CreateMatchDataStructs(matches, 7);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            db.DeleteDatabase(path);
        }

        /// <summary>
        /// Whenever betCoefficient is smaller
        /// than playLimit, bet shouldn't be played, and 0 is returned.
        /// </summary>
        [Test]
        public void Test_PlayBet_return0()
        {
            /* Match result = -0.54.
             Estimated result percentage = 0.3147
             betCoefficient = 0.84.
             
             betCoefficient is smaller than playLimit, function returns 0.*/
            Match toPredict = Matches.SelectMatchesWithRowIndex(new List<int>() { 7 })[0];
            double result = (double)Predict.PredictResult(toPredict, 3); 

            Assert.AreEqual(0, Bet.PlayBet(toPredict, result, 1.05, 5, 0.2));
        }

        [Test]
        public void Test_PlayBet()
        {
            /* Match result = -0.54.
             * Predicted result odd = 3.15
             * Prediction correct = 1
             Estimated result percentage = 0.3147
             betCoefficient = 0.991305 
             stake = 5 * (0.991305 / 0.5) = 9.91305 
             result = 9.91305 * 3.15 */
            Match toPredict = Matches.SelectMatchesWithRowIndex(new List<int>() { 7 })[0];
            double result = (double)Predict.PredictResult(toPredict, 3); //result is ~ -0.54

            Assert.AreEqual(21.31, Math.Round(Bet.PlayBet(toPredict, Math.Round(result, 2), 0.5, 5, 1), 2));
        }

        [Test]
        public void Test_PlayBet_lose_return_minus_stake()
        {
            /* Here drawLimit is set lower than the predictedResult, so simulated
             * result will be away win. Therefore PlayBet returns 0, as it wins
             * Match result = -0.54.
             * Predicted result odd = 2.7
             * Prediction correct = 0
             Estimated result percentage = 0.3147
             betCoefficient = 0.8496
             stake = 5 * (0.8496 / 0.5) = 8.496 
             result = -8.496 */
            Match toPredict = Matches.SelectMatchesWithRowIndex(new List<int>() { 7 })[0];
            double result = (double)Predict.PredictResult(toPredict, 3); //result is ~ -0.54

            Assert.AreEqual(-8.5, Math.Round(Bet.PlayBet(toPredict, Math.Round(result, 2), 0.5, 5, 0.53), 2));
        }
    }
}
