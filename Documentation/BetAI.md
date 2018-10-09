# BetAI

BetAI requires a component for:
1. Calculating a predicted result  
2. Calculating a risk limit for a bet and deciding the stake accordingly
3. Creating a random n-sized sample list of matches
4. Creating an initial n-sized node generation
5. Fitness function for assessing the fitness of each node
6. With weighted probability select nodes based on fitness and reproduce 
with some mutation probability, replace least-fit nodes with new individuals.
7. Handling of matches:
	- method to return a list of matches based on row number
	- method for returning count of matches in memory
	- method to count mean home/away -goals in league
in a given season, before match n. 
	- method to return N last home- or awaymatches from a given team
starting from a given date. If team doesn't have n home/awaymatches,
n previous all matches are returned.

## Use cases
User starts the command prompt to run the program with filename argument.
File doesn't exist, so it is created.
Value for mutation probability and percentage of population to go the 
reproduction process are asked to be inputted along with minimumStake. These values can later be 
changed from *file\values.json*. Initial population is created, and algorithm 
starts.

User starts the command prompt to run program with filename argument.
File does exist, so program loads the data containing current 
population of nodes, and carries on the algorithm.

## Requirements 

### Data handling
Data handling is now done as follows: When the program starts, all matches
in database are loaded into QueryMatches. Then, for each generation
as Sample is created, sample matches are loaded into QueryMatches as an 
array of MatchData-structures, under conditions set by the node with maximum
sample size. Each node then in its fitness evaluation takes the chunks that it
needs. Array and structure use, together with algorithm change so that
simulation data is only loaded once per generation makes this functionality meet 
the performance requirements it needs has.

Now, when Master creates the sample, it also calls QueryMatches.CreateMatchDataStructs. 
One MatchData structures have been created, they can be called from Predict to get
match specific data. Call methods in predict change to GetNLastFromTeamBeforeMatch, and
GetSeasonAverage. NotEnoughDataException still needs to be caught. 

1. method to return a list of matches based on row number
2. method for returning count of matches in memory
3. method to count mean home/away -goals in league
in a given season, before match n. 
4. method to return N last home- or awaymatches from a given team
starting from a given date. If team doesn't have n home/awaymatches,
n previous all matches are returned.

#### Progress
All done.

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

2. Component needs to have a method PlayBet, which returns
profit of the bet. 

3. A bet is played, if betCoefficient is larger 
than playLimit. If bet is not played, 0 is returned
as it is the profit/loss of the bet.

#### Progress
All requirements done.

### Sampling
To get the matches used as a sample, a list of random integers
needs to be created. This list is called Points. Points has 
following properties:
1. Sample can contain values from 0 to match count.
2. Sample can't contain more than one of any value.
3. Sample cant have more values than match count.

#### Progress
Done.

### Node 
Node is an individual node of the whole population. It has variables 
for predicting a match result and making a bet. 

Each node has a fitness value. It is the profit made
in current match sample. 

Each node has a probability of being selected for crossover,
based on fitness value. A higher fitness = Higher chance of
being selected for crossover.

Node also collects statistics:
 Won / played / not played / skipped bets, and generation. 
All variables are written to a file at generation,
change with other nodes of the same generation.
 
1. Node has variables for riskLimit, drawLimit, evaluationSampleSize and minimumStake.
EvaluationSampleSize tells how many matches are looked for assessing strength values for 
teams.
 
2. Node has a variable for fitness value. Fitness is the profit made.
 
3. Node has a variable for probability of being
selected for crossover. CrossoverFactor contains a
weighted factor for that node to be selected for crossover.

4. Node has variables for amount of bets won, lost
 and skipped, and its generation.
 
5. All node data is written in JSON-format
to a file, with other members of its generation.

6. Node has a method for calculating fitness value. Fitness evaluation
should not last for more than 50ms as average.

7. SimulationSampleSize cant be less than 1.

### Crossover
Crossover creates new nodes based on the genetic data of parents. 
Values for child nodes are generated using BLX-alpha algorithm. 
Each parent creates two children.

1. Childrens PlayLimit, DrawLimit, and SimulationSampleSize variable values are always between 
```
(min - alpha * d ,max + alpha * d)
```
where d is the difference in value between the parents,
min is the minimum of the two parents value for parameter, max
is the maximum value for parameter between two parents, and alpha
is the set parameter for BLX.

2. Number of children created is always 
```
floor(selectedCrossoverNodes / 2) * 2
```

3. Created children should have the same minimumstake as their parents
and Generation should be added by one per generation.

4. SimulationSampleSize cant be less than 1.

#### Progress
All requirements are met in tests.


### Master
Master is the driver of simulation. When program is started witha argument file,
new Master is created with parameter filename. It checks whether *BetAI\Files* contains a
folder named filename. If not, this folder is created, and *values.json*-file is created.
This is filled with a json object containing simulation data, either with user given
parameters or values from *BetAI\Files\default.json*. 

```
{
	"alpha": "0.5",
	"minimumStake": "3.0",
	"numberOfNodes": "2000",
	"sampleSize": "200",
	"database": "path\to\db\file"
}
```