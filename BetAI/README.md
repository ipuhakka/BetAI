# BetAI evolution simulation.

This document describes how the simulation works.

Simulation is run by Master.cs. 

## Start

Master is the driver of simulation. When program is started witha argument file,
new Master is created with parameter filename. It checks whether *BetAI\Files* contains a
folder named filename. If not, this folder is created, and *values.json*-file is created inside.
This is filled with a json-object containing simulation data, either with user given
parameters or values from *BetAI\Files\default.json*. 

```
{
	"alpha": "0.5",
	"minimumStake": "3.0",
	"numberOfNodes": "2000",
	"sampleSize": "200",
	"database": "path\to\db\file"
}

These arguments can be given as program arguments when starting simulation
with a new file.

```
BetAI.exe mynewfilename alpha=0.4 minimumStake=4 numberOfNodes=1000 sampleSize=100 database="path\to\used\database\file"
```

```
On starting the program these can be given as parameters. If not, values from *default.json* 
are used.

On start, matches used are set.
```
Matches.SetMatches(pathToDatabaseFile);
```


## Set up for generation fitness evaluation

First, generation nodes are initialized.
```
List<Node> generation = Load.LoadLatestGeneration(savefile);

if (generation == null)
{
	//create random nodes	
}
```

Sample with new matches is created 
```
List<Match> sample = Sample.CreateSample(13);
```

Then, MatchData structures in **QueryMatches** are set
```
int maxSampleSize = nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize;
Matches.CreateMatchDataStructs(sample, maxSampleSize);
```

Now individual nodes can be evaluated
```
for (int i = 0; i < nodes.Length; i++){
	nodes[i].EvaluateFitness(sample);
}
```

Then, parents can be selected
```
Selection sel = new Selection();
List<Node> toReproduce = sel.SelectForCrossover(nodes);
```