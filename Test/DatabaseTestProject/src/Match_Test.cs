using System;
using System.Collections.Generic;
using Database;
using System.IO;
using System.Data.SQLite;
using NUnit.Framework;

namespace DatabaseTestProject
{   [TestFixture]
    public class Match_Test
    {
        [Test]
        public void Test_GetWagerOdd_throws_argumentException()
        {
            Match nullResultMatch = new Match("", "", 1.2, 2, 2, DateTime.Today);
            Assert.Throws<ArgumentException>(() => nullResultMatch.GetWagerOdd());
        }
    }
}
