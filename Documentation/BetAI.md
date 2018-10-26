# BetAI
BetAI is a genetic algorithm that samples football match data from an sqlite3-database
and bets the results of these. Aim is to improve amount of money won/lost by the bets.
It is designed to support different parent selection and crossover methods. Supported methods 
in version 0.2 are:
- Crossover
	- Blend crossover alpha (BLX)
	- Uniform alpha crossover (Uni-alpha)
- Parent selection
	- Weighted selection (Weighted)
	- Tournament selection (Tournament)
	
## Parent selection methods
This chapter describes parent selection methods that are available in the program. 
1. Weighted probability selection (parameter name = Weighted)
	- Weighted parent selection gives each node a weight based on its fitness. 
	Then, a randomising process takes part: The better the fitness, the better is the chance
	of an individual to be selected for crossover.
2. Tournament selection (parameter name = Tournament)
	- Tournament selection randomises a tournament of k-nodes, and chooses
	the node with best fitness from tournament to crossover.

## Crossover methods
This chapter describes crossover methods available in the program.
1. BLX-alpha (parameter name = BLX)
- BLX-alpha uses blend-crossover method to produce two child nodes.
2. 
## Use

BetAI can be run after building the program, by opening build directory and running
command:
```
BetAI.exe {filename}
```

This creates a new save named *filename* and starts the simulation with default values.
These are displayed in BetAI\BetAI\Files\defaults.json. If default values are to be changed,
in this folder, program needs to be built before changed are in effect. 

Result data is logged to *buildDir/Files/{filename}/ directory. It contains json-files
for all samples used and all generations of nodes. 
Console log is also found in text format.

### Program arguments

If user does not want to use default arguments, it is possible to set these by giving them
as parameters when starting the program. These parameters need to be given after *filename*.

All parameters are given in format *argumentName=argumentValue*. If
parameter is a string type, it is placed in quotations.

- minimumStake: double
	- Amount of money that is played as a minimum, when a bet is decided to be placed.
- numberOfNodes: int
	- Generation size.
- sampleSize: int
	- Match sample size. Sample is a list of matches that is randomised for each
	generation of nodes for fitness evaluation.
- database: string
	- Path to database file used. 
- parentSelectionMethod: string
	- Indicates what method is used to select nodes for crossover.
	Supported methods:
		- Weighted
		- Tournament
- crossoverMethod: string
	- Indicates the used crossover method.
	Supported:
		- BLX (Blend crossover alpha)
- alpha: double
	- Alpha value for BLX-crossover method. If some other
	crossover method is used, this value does not have any effect.
- tournamentSize: int
	- Size of tournaments if tournament selection is used as parent selection method.
	Otherwise this value does not have any effect.

## Code structure

Main */src*-folder contains Program start point and Master-class, which runs the simulation.

### BetSim
Contains classes needed for predicting and playing (or not playing) bets.

### Exceptions
Contains project specific exception classes.

### FileOperations
Contains file-I/O actions.

### Genetics
Contains all classes that make up the genetic algorithm.

### Utils
Utility classes. 

## Flow
This chapter describes how simulation is run.

1. Initialization
- If save is new, filestructure is created and values.json fileld accordingly. First
generation of nodes is created using random-constructor of Node.
- If Save already exists, json data from gen_data folder is loaded into memory. Newest generation 
is loaded into memory. Values from values.json are loaded.
- All matches in the used database are fetched into memory.
- Randomise.InitRandom() is called. This can be used throughout the program to handle creating of random numbers.

2. Running the simulation
- n-sized sample list of matches is created.
- MatchData structures are set based on sample list.
- Matches-sample is logged to sample{i}.json.
- Generation of nodes is evaluated
- Generation data is written into file gen{i}.json and fitness statistics logged.
- Creating new generation
	- Parents are selected from the generation to produce offspring.
	- Offspring is created. New generation is written to file gen{i}.
- Process is repeated until user interrupts it with esc-button.



