# DataParser

## Description
DataParser is an application that downloads sports match data from the internet to 
get training data for BetAI. Data downloaded is in .CSV-format. Data is processed by 
the program, and then added to the specified database using Database-library.

This tool is designed
to download data from http://www.football-data.co.uk/data.php. Data format is 
discussed in http://www.football-data.co.uk/notes.txt.

## Data 
Data that has to be gathered:
```
Date
Hometeam
Awayteam
Homescore
Awayscore
HomeOdd
DrawOdd
AwayOdd
```
Data columns can have changing names in each of the files. Data provides
multiple different bookmaker's odds. Bet365 odds will be used as default.

## Program arguments
Argument name / value separator is '=' character.

### Argument order
Necessary arguments should be in the given order as the first arguments:
Database file name, address from where the .csv-file is downloaded, season of the matches in format 'yyyy-yyyy', and league name.
Optional arguments can appear in any order after these, or not at all.

### Arguments
Necessary arguments for launching the program: 
```
database=fileName
address=uriToCSVFile
season=year-year
league=leaguename
```

Optional arguments:
```
date=columnName
hometeam=columnName
awayteam=columnName
homescore=columnName
awayscore=columnName
homeOdd=columnName
drawOdd=columnName
awayOdd=columnName
```

Optional parameters should be given if the file that is trying to be downloaded does not 
follow default naming conventions defined in http://www.football-data.co.uk/notes.txt.

In the batch script these

## Requirements

**RQ1**. All data described in *Data*-section has to be collected for each match.

**RQ2**. Address from which the file is downloaded is given as a parameter for the program.

**RQ3**. Database file is given as a parameter.

**RQ4**. Columns parsed by the program can be given as parameters for the program. 

**RQ5**. Program is used from a batch script.

## Progress

**RQ1**. Described data is collected for each match.

**RQ2**. Done

**RQ3**. Done.

**RQ4**. Done.

**RQ5**. Done. 

## Bugs discovered

**26.9.2018**: CSVParser.cs.ParseColumns(): Improper lines were not filtered out. Fix: catch FormatException, and 
IndexOutOfRangeExceptions and continue loop in the catches. These will fail if line does not contain valid data. 

## Changes made
**27.9.2018** Added season and league parameters as necessary program inputs. 
