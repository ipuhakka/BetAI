## Build
Project does not contain any scripts to build, so the project needs to be built
by opening it in Visual Studio and building it there.

## Test
Once the program has been built, tests can also be run using Visual Studio.

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
	- Indicates the used crossover method. These are explained in *Documentation\BetAI.md*.

- alpha: double
	- Alpha value for some crossover methods. If another kind of
	crossover method is used, this value does not have any effect.
- tournamentSize: int
	- Size of tournaments if tournament selection is used as parent selection method.
	Otherwise this value does not have any effect.