# v0.4
 -Implemented
	- Playing bets from external sources
	- Added logging best node statistics
	- Uniform crossover method
- Performance: 
	- Matches uses dictionary instead of linq-queries to search correct MatchData struct.
	- DataParser uses dictionary instead of tuple-list
- Refactoring code

# v0.3
- Implemented
	- Implement uniform alpha crossover
	- Refactor test namespaces for easier identification
	- Set minimumstake in default.json to 1
	- Check code for intelliSense hints on structure
	- Implement a mutation strategy
	
# v0.2
- Implemented
	- Display only valid parameters from values.json to console (all are not used)
	- Sample logging
	- Add tournament selection method
	- Fixed sampling bug
	- Fixed NaN predicted result bug
	- Fixed bracket bug in BLXAlpha
	- Change returning a list of nodes to returning parent-object in selection.
	- Reorganize and clean documentation 