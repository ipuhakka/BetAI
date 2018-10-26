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
