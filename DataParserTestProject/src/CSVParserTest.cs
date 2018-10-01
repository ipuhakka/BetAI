using System;
using System.Collections.Generic;
using NUnit.Framework;
using DataParser;
using System.IO;
using Database;

namespace DataParserTestProject
{ 
    [TestFixture]
    public class CSVParserTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Test]
        public void test_Parse_default_values_return_380_matches()
        {
            CSVParser csv = new CSVParser();
            List<Match> matches = csv.Parse(@"test-files\England17-18.csv", "England", "2016-2017");
            Assert.AreEqual(380, matches.Count);
            Assert.AreEqual(new DateTime(2018, 5, 13), matches[372].Date);
            Assert.AreEqual("Huddersfield", matches[372].Hometeam);
            Assert.AreEqual("Arsenal", matches[372].Awayteam);
        }
        
        /// <summary>
        /// Test uses a .csv-file with modified column names
        /// for homescore, awayscore and HomeOdd, DrawOdd and AwayOdd.
        /// </summary>
        [Test]
        public void test_Parse_modified_column_names()
        {
            CSVParser csv = new CSVParser();
            string[] args = new string[] {"homescore=HomeGoals", "awayscore=AwayGoals", "homeOdd=HomeOdd", "drawOdd=DrawOdd", "awayOdd=AwayOdd"};
            List<Match> matches = csv.Parse(@"test-files\England17-18_ModifiedColumnNames.csv", "England", "2016-2017", args);
            Assert.AreEqual(380, matches.Count);
            Assert.AreEqual(new DateTime(2018, 5, 13), matches[371].Date);
            Assert.AreEqual("Crystal Palace", matches[371].Hometeam);
            Assert.AreEqual("West Brom", matches[371].Awayteam);
        }

        /// <summary>
        /// If any of the column name parameters given for SearchParams.cs are invalid (dont exist
        /// in .csv-file), Parse operation throws ArgumentException.
        /// </summary>
        [Test]
        public void test_Parse_incorrect_column_names_throws_ArgumentException()
        {
            CSVParser csv = new CSVParser();
            string[] args = new string[] { "HomeScore=Homescore" };
            Assert.Throws<ArgumentException>(() => csv.Parse(@"test-files\England17-18.csv", "England", "2016-2017", args));
        }

        /// <summary>
        /// Test uses a .csv-file with modified column names
        /// for homescore, awayscore and HomeOdd, DrawOdd and AwayOdd with not giving these 
        /// successfully as parameters. Withouth '=' character, these values are never used.
        /// But since default values for column names don't all exist in modified .csv-file,
        /// this call should throw ArgumentException to indicate that Arguments were invalid.
        /// </summary>
        [Test]
        public void test_Parse_invalid_column_params_throws_ArgumentException()
        {
            CSVParser csv = new CSVParser();
            string[] args = new string[] { "HomeScore", "AwayScore", "HomeOdd", "DrawOdd", "awayOdd=AwayOdd" };
            Assert.Throws<ArgumentException>(() => csv.Parse(@"test-files\England17-18_ModifiedColumnNames.csv", "England", "2016-2017", args));
        }
    }
}
