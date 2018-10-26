# Outline
----------
This document outlines challenges discovered for making an evolutionary
algorithm for 1X2-betting. Its purpose is to take a challenging problem
and split it to comprehensible parts. Document presents solutions for 
tasks identified.

## Data
---------

### Data gathering
http://www.football-data.co.uk/data.php contains football match data from 
~10 european football leagues, for over 20 years time. It contains data for 
each match such as scores, half time scores, shots for both teams, etc.

This data can be used with help of BetSimulator, app for predicting results,
to form the data to work with. 

### Data storage	
For this type of algorithm to work, it needs a vast repository of data for 
training purposes. Data needs to be stored in a place where it's fast to get,
and in a format that allows the algorithm to work correctly.

This problem could be solved by putting the data into a database (SQLite),
and removing all data columns that can't be used. 

### Data structure
Database should not have any columns which are unnecessary. It is essentially
a place to store parsed match data from http://www.football-data.co.uk/data.php,
so that all data is easily accessible in one place for running the engine that
calulcates values for individual matches.

```
CREATE TABLE IF NOT EXISTS matches(
	playedDate TEXT,
	hometeam TEXT,
	awayteam TEXT,
	league TEXT,
	season CHAR(9),
	homescore INTEGER,
	awayscore INTEGER,
	homeOdd REAL,
	drawOdd REAL,
	awayOdd REAL,
	CONSTRAINT PK_matches PRIMARY KEY(playedDate, hometeam, awayteam)
);
```

Table contains all data needed to calculate the predicted winner, and calculating the risk.

## Betting
-------------
This section covers how the algorithm makes bets.

### Predicting single match results

Match results can be predicted by using Poisson distribution calculations 
to calculate values for both teams attack and defense. 

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

if (result > drawLimit)
	betResult = 1; #home win
else if (homeGoalEstimate - awayGoalEstimate) < -drawLimit)
	betResult = -1; #away win
else
	betResult = 0; #draw
```

For predicting Match A vs. B with sampleSize n, program needs to 
have n last homematches from team A, and n last awaymatches from team B.

Also, predicting the winner requires a set limit on what is considered as 
a win. This is because result is a continuous variable. 

Parameters to be controlled by the algorithm in predicting match results would be
*drawLimit* and *sampleSize*. 

### Bet placement

In placing bets, system needs to have a way to analyze if bet is playable. This can 
be achieved by nodes having a *risk-limit*, which decides whether or not to play bet,
and a way to analyze the coefficient for the risk/profit value for each bet.
```
estimatedWinPercentage = (e^(-absolute(result)) * (absolute(result)) / 1; 
betCoefficient = estimatedWinPercentage / (1 / odd); 
bool playBet = (playLimit < betCoefficient);
stake = baseStake * (riskLimit / betCoefficient);
```
Algorithm controls parameter riskLimit.

### Fitness function
-----------------------
Fitness evaluation is based on how much profit is made by playing the bets.
With a standardized minimum stake that nodes use, algorithm rewards
nodes for better evaluation of when to play a bet and how big stake is played.

Challenges of this is that with bad luck it can favour not playing bets at all
(which always has fitness value of 0).