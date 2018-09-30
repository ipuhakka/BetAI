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
FIle does exist, so program loads the data containing current 
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



