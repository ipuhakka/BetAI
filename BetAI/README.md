# BetAI evolution simulation.

This document describes how the simulation works.

## Master
Simulation is run by Master-class. 

## Start

## Set up for generation fitness evaluation
First, generation nodes are initialized.

Sample with new matches is created 
```
List<Match> sample = Sample.CreateSample(13);
```

Then, MatchData structures in **QueryMatches** are set
```
int maxSampleSize = nodes.OrderBy(node => node.SimulationSampleSize).ToList()[0].SimulationSampleSize);
QueryMatches.CreateMatchDataStructs(sample.Matches, maxSampleSize);
```

Now individual nodes can be evaluated
```
for (int i = 0; i < nodes.Length; i++){
	nodes[i].EvaluateFitness(sample);
}
```