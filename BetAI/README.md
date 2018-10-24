This document describes how the simulation works.
Simulation is run by Master.cs. 

## Start

Master.cs is the driver of simulation. When program is started witha argument file,
new Master is created with parameter filename. It checks whether *BetAI\Files* contains a
folder named filename. If not, this folder is created, and *values.json*-file is created inside.
This is filled with a json-object containing simulation data, either with user given
parameters or values from *BetAI\Files\default.json*. 

```
{
	"alpha": "0.5",
	"tournamentSize": "8",
	"minimumStake": "3.0",
	"numberOfNodes": "200",
	"sampleSize": "200",
	"database": "path\to\db\file",
	"parentSelectionMethod": "Weighted",
	"crossoverMethod": "BLX"
}
```

These arguments can be given as program arguments when starting simulation
with a new file, if user doesn't want to use default values. 

```
BetAI.exe mynewfilename alpha=0.4 tournamentSize=16 minimumStake=4 numberOfNodes=1000 sampleSize=100 database="path\to\used\database\file" parentSelectionMethod="Weighted" crossoverMethod="BLX" 
```

## Parent selection methods
This chapter describes parent selection methods that are available in the system. 
1. Weighted probability selection (parameter name = Weighted)
- Weighted parent selection gives each node a weight based on its fitness. 
Then, a randomising process takes part: The better the fitness, the better is the chance
of an individual to be selected for crossover.

## Crossover methods
This chapter describes crossover methods available in the system.
1. BLX-alpha (parameter name = BLX)
- BLX-alpha uses blend-crossover method to produce two child nodes. 

# Flow
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
