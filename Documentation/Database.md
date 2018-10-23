# Database

## Description
Database is a C#/SQLite library implementation for storing and accessing 
1X2-betting data. It is designed 
to store data from several different leagues, in a simple table containing all the data needed
to simulate betting results. 


## Table structure
```
CREATE TABLE IF NOT EXISTS matches(
	playedDate TEXT,
	hometeam TEXT,
	awayteam TEXT,
	league TEXT,
	season CHAR(9),
	homescore INTEGER,
	awayscore INTEGER,
	homeOdd REAL,
	drawOdd REAL,
	awayOdd REAL,
	CONSTRAINT PK_matches PRIMARY KEY(playedDate, hometeam, awayteam)
);
```

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
on list of index rows. Implemented new transactional match sampling. Tests run OK.'

**3.10.2018**: Working directly with the database is far too slow for BetAI. Therefore, database
is redesigned to only have a query method for returning all matches. Unused query methods were removed from database
layer and requirements.

## Bugs discovered
**30.9.2018**: Reading match data in DB.ParseMatches used season and league in the wrong order
in match constructor. This meant using parameters in wrong place in queries, 
when using Match-object data to shape the query. Created test cases to pass after chagnes
have been made, tests run OK.
