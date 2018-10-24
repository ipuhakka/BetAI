# BetAI

## Use

BetAI can be run after building the program, by opening build directory and running
command:
```
BetAI.exe {filename}
```

This creates a new save named *filename* and starts the simulation with default values.
These are displayed in BetAI\BetAI\Files\defaults.json. If default values are to be changed,
in this folder, program needs to be built before changed are in effect. 

### Program arguments

If user does not want to use default arguments, it is possible to set these by giving them
as parameters when starting the program. 

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

## Code

### BetSim
Contains classes needed for predicting and playing (or not playing) bets.

### Exceptions
Contains project specific exception classes.

### FileOperations
Contains file-I/O actions.


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

