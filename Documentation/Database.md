# Database

## Description
Database is a C#/SQLite implementation for storing sports match data. It is designed 
to store data from several different leagues, in a simple table containing all the data needed
to simulate betting results. 

## Table structure
```
Match:
	TEXT playedDate,
	INTEGER league REFER League,
	INTEGER season REFER Season,
	TEXT hometeam,
	TEXT awayteam,
	INTEGER homescore,
	INTEGER awayscore,
	REAL homeOdd,
	REAL drawOdd,
	REAL awayOdd
	
League:
	TEXT name,
	INTEGER id
Season: 
	CHAR(9) season,
	INTEGER id
```

## Requirements
**RQ1**. Database needs to have a method to return N last home- or awaymatches from a given team
starting from a given date. 

**RQ2**. If database doesn't have N previous home/away matches from team A starting from a given date,
It should look for N last total matches. If there is still not enough, it throws an error.

**RQ3**. Database can't have duplicate matches. Duplicate = same date, hometeam and awayteam.

**RQ4**. Database needs to have methods to create a database and set a schema.

**RQ5**. Adding data to table has to be fast enough. Updating 380 matches (standard for 20 team league)
  should take ~ 2 seconds.
  
**RQ6**. Database needs to have method to return a list of matches based on row number. 

**RQ7**. Database needs to have a method for returning count of matches in the database.

**RQ8**. Database needs to have a method to count mean home/away -goals from a league
in a given season, before match n.
  
## Progress
**RQ1**. - Done.

**RQ2**. - Done.

**RQ3**. - Done.

**RQ4**. - Done.

**RQ5**. - Done. Updating 380 matches takes < 0.5 seconds.

**RQ6**. -Done.

**RQ7**. -Done.

**RQ8**. -Done.

## Future changes.
Once BetSimulator is done to the stage that it can be tested, results on how well 
selecting N last from team works in the solution. There is a chance that it is 
too slow, in which case an alternative solution has to be found.

## Made changes
**27.9.2018**: Changed matches table structure to include column for season and league, and corresponding methods. 
Test cases created for new branching.
Tests run OK

**28.9.2018**: Changed the database structure completely. Now there are three tables: matches, seasons, and leagues.
Leagues and seasons make it easier to secure unification of data. Now if some match has incorrect values for these,
database won't allow for adding these matches. Test run OK.

**29.9.2018**: Removed all yesterdays changes to table schema and database layer. Added method to count
home goal average up to date n. Tests run OK. Merge branches.

**2.10.2018**: Changed requirement **RQ6** from returning match in row N to returning list of matches based
on list of index rows. Implemented new transactional match sampling. Tests run OK.

## Bugs discovered
**30.9.2018**: Reading match data in DB.ParseMatches used season and league in the wrong order
in match constructor. This meant using parameters in wrong place in queries, 
when using Match-object data to shape the query. Created test cases to pass after chagnes
have been made, tests run OK.
