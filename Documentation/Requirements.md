This document describes all requirements that were identified in designing the system.
These were documented to this file so they can be explicitly tested against in making changes
to project.

# Database

**RQ1**. Database can't have duplicate matches. Duplicate = same date, hometeam and awayteam.

**RQ2**. Database needs to have methods to create a database and set a schema.

**RQ3**. Adding data to table has to be fast enough. Updating 380 matches (standard for 20 team league)
  should take ~ 2 seconds.
  
**RQ4**. Database needs a method to return all matches in the database.

# DataParser

**RQ5**. Foreach match, DataParser needs to collect following data:
date played, home- and awayteam name, home- and awayteam goals scored, and odds
for homewin, draw, and awaywin. 

**RQ6**. Program needs to be given following parameters on start:
*database, address, season and league*. Database indicates to what file new matches are added, 
address tells the uri from which new match data file is downloaded, season tells which season matches
were played, and league is a name used to identify these matches. 

**RQ7**. Season is a 9 character string in format yyyy-yyyy. 


**RQ8**. Columns parsed by the program can be given as parameters for the program. These
indicate what columns are parsed from input data file. 
If these are not given as arguments for program, default values are used. Default values are:
1. HomeTeam: Hometeam name.
2. AwayTeam: Awayteam name.
3. FTHG: Goals scored by hometeam.
4. FTAG: Goals scroed by awayteam.
5. B365H: Homewin odd provided by Bet365.
6. B365D: Draw result odd provided by Bet365.
7. B365A: Awaywin odd provided by Bet365. 

**RQ9**. Program arguments are given in format: argumentname=argumentvalue.

**RQ10**. Mandatory arguments are given as the first arguments, in order: 
*database, address, season, league*. Optional arguments used to define what columns are searched
from input file can then follow in any order. 

# BetAI

## Creating match sample for simulation
**RQ11**. n-sized sample list of matches is created based on parameter *sampleSize* 
defined in *values.json*. 
**RQ12**. Matches are selected for sample based on their row number: that means 
matches from 0 to (database match count - 1) can belong to sample.
**RQ13**. Sample can't have a same match more than once.
**RQ14**. Sample size cannot be larger than number of matches in the used database.

## Using matches from database
1. method to return a list of matches based on row number
2. method for returning count of matches in memory
3. method to count mean home/away -goals in league
in a given season, before match n. 
4. method to return N last home- or awaymatches from a given team
starting from a given date. If team doesn't have n home/awaymatches,
n previous all matches are returned.

**RQ15**. System needs a method to return a list of matches
based on row number. It takes an n-sized list of index numbers ranging from 0 to 
(database match count - 1), and returns a list of matches which resided in the parameter indexes.

**RQ16**. System needs a method to count the number of matches in the used database.

**RQ17**. System needs to be able to calculate average number of goals scored in home/awaygames, 
in a specified league and season, up to match m. Matches after m and m are not calculated into this.

**RQ18**. System needs to get N last home/awaymatches from team before a given date. If there aren't
n home or awaymatches before a given date, both are searched. If there still aren't enough matches,
a match cannot be simulated.

