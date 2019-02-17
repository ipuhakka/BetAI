@setlocal enableextensions
@cd /d "%~dp0"
cd bin\debug

call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1819/I1.csv" season="2018-2019" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1819/SP1.csv" season="2018-2019" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1819/E0.csv" season="2018-2019" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1819/SC0.csv" season="2018-2019" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1819/D1.csv" season="2018-2019" league="Germany"

cd..\..
PAUSE
EXIT /B 0

REM anything under EXIT line will not be performed
REM Italy Serie A:
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1718/I1.csv" season="2017-2018" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1617/I1.csv" season="2016-2017" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1516/I1.csv" season="2015-2016" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1415/I1.csv" season="2014-2015" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1314/I1.csv" season="2013-2014" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1213/I1.csv" season="2012-2013" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1112/I1.csv" season="2011-2012" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1011/I1.csv" season="2010-2011" league="Italy"

REM Spain La Liga:
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1718/SP1.csv" season="2017-2018" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1617/SP1.csv" season="2016-2017" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1516/SP1.csv" season="2015-2016" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1415/SP1.csv" season="2014-2015" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1314/SP1.csv" season="2013-2014" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1213/SP1.csv" season="2012-2013" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1112/SP1.csv" season="2011-2012" league="Spain"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1011/SP1.csv" season="2010-2011" league="Spain"

REM England premier league:
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1718/E0.csv" season="2017-2018" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1617/E0.csv" season="2016-2017" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1516/E0.csv" season="2015-2016" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1415/E0.csv" season="2014-2015" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1314/E0.csv" season="2013-2014" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1213/E0.csv" season="2012-2013" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1112/E0.csv" season="2011-2012" league="England"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1011/E0.csv" season="2010-2011" league="England"

REM Scotland Premier league:
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1718/SC0.csv" season="2017-2018" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1617/SC0.csv" season="2016-2017" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1516/SC0.csv" season="2015-2016" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1415/SC0.csv" season="2014-2015" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1314/SC0.csv" season="2013-2014" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1213/SC0.csv" season="2012-2013" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1112/SC0.csv" season="2011-2012" league="Scotland"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1011/SC0.csv" season="2010-2011" league="Scotland"

REM Germany bundesliga:
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1718/D1.csv" season="2017-2018" league="Germany"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1617/D1.csv" season="2016-2017" league="Germany"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1516/D1.csv" season="2015-2016" league="Germany"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1415/D1.csv" season="2014-2015" league="Germany"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1314/D1.csv" season="2013-2014" league="Germany"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1213/D1.csv" season="2012-2013" league="Germany"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1112/D1.csv" season="2011-2012" league="Germany"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1011/D1.csv" season="2010-2011" league="Germany"