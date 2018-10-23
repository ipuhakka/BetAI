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

**RQ15**. System needs a method to return a list of matches
based on row number. It takes an n-sized list of index numbers ranging from 0 to 
(database match count - 1), and returns a list of matches which resided in the parameter indexes.

**RQ16**. System needs a method to count the number of matches in the used database.

**RQ17**. System needs to be able to calculate average number of goals scored in home/awaygames, 
in a specified league and season, up to match m. Matches after m and m are not calculated into this.

**RQ18**. System needs to get N last home/awaymatches from team before a given date. If there aren't
n home or awaymatches before a given date, both are searched. If there still aren't enough matches,
a match cannot be simulated.

## Predicting results:
**RQ19**. System needs to get a list of n previous matches for home and away team.

**RQ20**. System needs to count mean values for goals scored/conceded for both teams.

**RQ21**. Result is calculated as follows:
```
homeAttack = (homeGoals / sampleSize) / leagueAvgGoalsHome;
homeDef = (homeConceded/ sampleSize) / leagueAvgGoalsAway;
awayAttack = (awayGoals / sampleSize) / leagueAvgGoalsAway;
awayDef = (awayConceded / sampleSize) / leagueAvgGoalsHome;

#Goal estimates are the poisson distribution expected values
homeGoalEstimate = homeAttack * awayDef * leagueAvgGoalsHome;
awayGoalEstimate = awayAttack * homeAttack * leagueAvgGoalsAway;

#result
result = homeGoalEstimate - awayGoalEstimate;
```

## Setting a bet
**RQ22**. Component needs to set a stake according to algorithm
```
estimatedWinPercentage = (e^(-absolute(result))) * (absolute(result)) / 1; 
betCoefficient = estimatedWinPercentage / (1 / odd); 
bool playBet = (playLimit > betCoefficient);
stake = minimumStake* (betCoefficient / playLimit);
```
based on given parameters *riskLimit* and *minimumStakeCoefficient* and drawLimit.

*estimatedWinPercentage* is poisson distribution probability for 
predicted difference in the two teams goals scored. 

*betCoefficient* is the found value of bet (compared to odd providers assessment).
For example *betCoefficient* = 1, means that
system predicts the likelyhood of a result as the same
as odd providers did. System has not found any value in 
playing the bet.
 
*playLimit* is parameter which sets the minimum playable
value for a bet. 
*minimumStake* is the base stake set.

**RQ23**. Component needs to have a method PlayBet, which returns
profit of the bet. Profit is 0 if bet was not played at all,
its (-stake), if bet was lost, and its stake * odd - stake, if bet was
predicted correctly.

**RQ24**. A bet is played, if betCoefficient is larger 
than playLimit. If bet is not played, 0 is returned
as it is the profit/loss of the bet as bet was not played. 

## Node
**RQ25**. Node holds data for fitness, number of bets won, lost, not played and 
skipped. Skipped are not simulated due to not having enough data to simulate a match.
 
**RQ26**. Node fitness is defined by how much node made profit playing the sample of matches.
 
**RQ27**. Node has a CrossoverValue, which is used to select or not select node to crossover. 

**RQ28**. Fitness evaluation for node should not last more than 50ms, given a match samplesize
of 100. 

**RQ29**. SimulationSampleSize cant be less than 1 or over 40.

**RQ30**. DrawLimit is between 0.0 and 5.

**RQ31**. PlayLimit is between 0.01 and 2.

**RQ32**. Minimumstake is over 0.

## Crossover

**RQ33**. Childrens PlayLimit, DrawLimit, and SimulationSampleSize variable values are always between 
```
(min - alpha * d ,max + alpha * d)
```
where d is the difference in value between the parents,
min is the minimum of the two parents value for parameter, max
is the maximum value for parameter between two parents, and alpha
is the set parameter for BLX.

**RQ34**. Number of children created is always 
```
floor(selectedCrossoverNodes / 2) * 4
```

**RQ35**. Created children should have the same minimumstake as their parents
and Generation should be added by one per generation.'

## Program data

**RQ36**. On creating a new save, new folder named *filename* is created, and inside it, a file *values.json*
and a folder gen_data is created. 


**RQ37**. *values.json* has values from *BetAI\Files\defaults.json*, unless any variable is given a value as
parameter. That value is then written into *values.json*.'

**RQ38**. If program is started with a savefile that exists, that file is loaded. Newest
generation of nodes is loaded from *gen_data* folder into program memory and simulation is continued
there.

## Data reporting

**RQ39**. Each generation of nodes is written as json file with name genX.json, where X is the
current generation. 


**RQ40**. Once a new generation of nodes has been created, it is written into *BetAI\Files\savefile\genX.json*.
This file is overwritten once generation has been evaluated.


**RQ41**. After a generation has been evaluated, results are logged to console:
Fitness sum of generation, worst fitness, average fitness, and maximum fitness are logged to screen.
These are also logged to file *BetAI\Files\savefile\log.txt*.