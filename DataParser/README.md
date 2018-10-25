# Overview
-----------
This tool adds football match csv-file data from http://www.football-data.co.uk/data.php 
into a database. Tool is run with the use of Run.bat -file, which runs the DataParser.exe 
with different parameters.

## Build
Project does not have any build scripts, so it can to be build using Visual Studio.

## Test
Tests can be run with Visual Studio.

## Use

Run.bat is a script file which can execute DataParser multiple times
with different parameters. 

```
@setlocal enableextensions
@cd /d "%~dp0"
cd bin\debug

//PUT YOUR DATAPARSER CALLS HERE, LIKE THIS
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1718/I1.csv" season="2017-2018" league="Italy"
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1617/I1.csv" season="2016-2017" league="Italy"

cd..\..
PAUSE
EXIT /B 0
```

## Input parameters
Calling DataParser.exe is done in *Run.bat* like this
```
call DataParser.exe database="..\..\..\Database\db\data.sqlite3" address="http://www.football-data.co.uk/mmz4281/1718/I1.csv" season="2017-2018" league="Italy"
```

This calls DataParser.exe with database defined to be in "..\..\..\Database\db\data.sqlite3", relative
to Run.bat-location. Address defines where the csv-file is downloaded. *season* and *league* describe
what season and league the data downloaded belongs to. These are used in the database.

## Optional parameters

Tool uses default parameter names described in http://www.football-data.co.uk/notes.txt. 

These are:
```
HomeTeam = Home Team
AwayTeam = Away Team
FTHG and HG = Full Time Home Team Goals
FTAG and AG = Full Time Away Team Goals
Date = Match Date (dd/mm/yy)
B365H = Bet365 home win odds
B365D = Bet365 draw odds
B365A = Bet365 away win odds
```

If parsing fails, it might be possible that column names differ from these.
In such an event, parameters can be defined after the necessary parameters described
in previous section. These parameters can then be inputted like this:
```
date="columnName"
hometeam="columnName"
awayteam="columnName"
homescore="columnName"
awayscore="columnName"
homeOdd="columnName"
drawOdd="columnName"
awayOdd="columnName"
```

This allows also changing the used odds to those of some other betting company.

## Errors
DataParser.exe errors:
```
"Invalid season parameter"  
```
This means that season parameter inputted is not in format 'yyyy-yyyy'.

```
"Failure in parsing necessary arguments"
```
Mandatory arguments where not inputted correctly. Parameters are inputted like this:
*parameterName="parameterValue"*

```
"Failure in downloading file from address" 
```
Error happened while downloading file. This could mean your not connected to internet, or address
given is invalid.

```
"Connection to the database X failed"
```

Database has not been created, it does not exist or path given to it was invalid. 