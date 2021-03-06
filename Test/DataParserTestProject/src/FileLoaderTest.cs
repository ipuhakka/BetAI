﻿using NUnit.Framework;
using System.IO;
using DataParser;

namespace DataParserTestProject
{
    [TestFixture]
    public class FileLoaderTest
    {
        [Test]
        public void Test_LoadFile_InvalidAddress_throws_LoadException()
        {
            FileLoader fl = new FileLoader();
            Assert.Throws<LoadException>(() => fl.LoadFile("notawebaddress.net/notafile.csv", "testi.csv"));
        }

        [Test]
        public void Test_LoadFile_AddressWithNoCSV_throws_LoadException()
        {
            FileLoader fl = new FileLoader();
            Assert.Throws<LoadException>(() => fl.LoadFile("https://github.com/ipuhakka", "testi.csv"));
        }

        [Test]
        public void Test_LoadFile_InvalidPath_throws_DirectoryNotFoundException()
        {
            FileLoader fl = new FileLoader();
            Assert.Throws<DirectoryNotFoundException>(() => fl.LoadFile("http://www.football-data.co.uk/mmz4281/1819/E0.csv", "notexistingDir\testi.csv"));
        }

        [Test]
        public void Test_LoadFile_return1()
        {
            FileLoader fl = new FileLoader();
            Assert.AreEqual(1, fl.LoadFile("http://www.football-data.co.uk/mmz4281/1819/E0.csv", "testi.csv"));
            Assert.True(File.Exists("testi.csv"));
            File.Delete("testi.csv");
        }
    }
}
