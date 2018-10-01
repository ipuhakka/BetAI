# BetAI

BetAI requires a component for:
1. Calculating a predicted result  
2. Calculating a risk limit for a bet and deciding the stake accordingly
3. Creating a random n-sized sample list of matches
4. Creating an initial n-sized node generation
5. Fitness function for assessing the fitness of each node
6. With weighted probability select nodes based on fitness and reproduce 
with some mutation probability, replace least-fit nodes with new individuals.

## Use cases
User starts the command prompt to run the program with filename argument.
File doesn't exist, so it is created.
Value for mutation probability and percentage of population to go the 
reproduction process are asked to be inputted. These values can later be 
changed from *file\values.json*. Initial population is created, and algorithm 
starts.

User starts the command prompt to run program with filename argument.
File does exist, so program loads the data containing current 
population of nodes, and carries on the algorithm.

## Requirements 

### Predicting results:
1. System needs to get a list of n previous matches for home and away team.
2. System needs to count mean values for goals scored/conceded for both teams.
3. System needs to get league average home and away goals, for season in which
the match predicted is played, up to predicted match.
4. Result is calculated as follows:
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

#### Progress
All requirements done.

### Setting bet
1. Component needs to set a stake according to algorithm
```
estimatedWinPercentage = (e^(-absolute(result))) * (absolute(result)) / 1; 
betCoefficient = estimatedWinPercentage / (1 / odd); 
bool playBet = (playLimit > betCoefficient);
stake = baseStake* (betCoefficient / playLimit);
```
based on given parameters *riskLimit* and *baseStakeCoefficient* and drawLimit.

*estimatedWinPercentage* is poisson distribution probability for 
predicted difference in the two teams goals scored. 

*betCoefficient* is the found value of bet (compared to odd providers assessment).
For example *betCoefficient* = 1, means that
system predicts the likelyhood of a result as the same
as odd providers did. System has not found any value in 
playing the bet.
 
*playLimit* is parameter which sets the minimum playable
value for a bet. 
*baseStake* is the base stake set.

2. Component needs to have a method PlayBet, which returns
money won/lost by the bet. 

3. A bet is played, if betCoefficient is larger 
than playLimit. If bet is not played, 0 is returned
as it is the profit/loss of the bet.

#### Progress
All requirements done.
