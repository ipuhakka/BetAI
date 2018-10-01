using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using DataParser;

namespace DataParserTestProject
{
    [TestFixture]
    public class SearchParamsTest
    {
        /// <summary>
        /// Nothing should be set, so Object with no params is equal
        /// to object with arguments which don't contain '=' characters.
        /// </summary>
        [Test]
        public void testSearchParams_NotSplittedStrings()
        {
            string[] args = new string[8];
            args[0] = "NotSplitted";
            args[1] = "NotSplitted";
            args[2] = "NotSplitted";
            args[3] = "NotSplitted";
            args[4] = "NotSplitted";
            args[5] = "NotSplitted";
            args[6] = "NotSplitted";
            args[7] = "NotSplitted";

            SearchParams sp1 = new SearchParams();
            SearchParams sp2 = new SearchParams(args);
            Assert.AreEqual(sp1, sp2);
        }

        [Test]
        public void testSearchParams_SetHomeOddAndDate()
        {
            string[] args = new string[2];
            args[0] = "homeodd=someColumnName";
            args[1] = "date=anothercolumnname";

            SearchParams sp1 = new SearchParams();
            SearchParams sp2 = new SearchParams(args);
            Assert.AreNotEqual(sp1.HomeOdd, sp2.HomeOdd);
            Assert.AreEqual(sp1.AwayOdd, sp2.AwayOdd);
            Assert.AreNotEqual(sp1.Date, sp2.Date);
            Assert.AreEqual(sp1.Hometeam, sp2.Hometeam);
        }

        [Test]
        public void test_SearchParams_Trims_setColumn()
        {
            string[] args = new string[2];
            args[0] = "homeodd= someColumnName";
            args[1] = "date=   anothercolumnname  ";

            SearchParams sp1 = new SearchParams(args);
            Assert.AreEqual(sp1.HomeOdd, "someColumnName");
            Assert.AreEqual(sp1.Date, "anothercolumnname");
        }

        [Test]
        public void test_SearchParams_Argument_name_CaseInsensitive()
        {
            string[] args = new string[2];
            args[0] = "homeOdd=someColumnName";
            args[1] = "DATE=anothercolumnname ";

            SearchParams sp1 = new SearchParams(args);
            Assert.AreEqual(sp1.HomeOdd, "someColumnName");
            Assert.AreEqual(sp1.Date, "anothercolumnname");
        }

        [Test]
        public void test_SearchParams_runs_with_larger_than_2_splittedArrays()
        {
            string[] args = new string[4];
            args[0] = "this=has=many=splits";
            args[1] = "this=has=many=splits";
            args[2] = "this=has=many=splits";
            args[3] = "this=has=many=splits";

            Assert.DoesNotThrow(() => new SearchParams(args));
        }

        [Test]
        public void test_SearchParams_runs_with_null_values()
        {
            string[] args = new string[8];
            Assert.DoesNotThrow(() => new SearchParams(args));
        }


    }
}
