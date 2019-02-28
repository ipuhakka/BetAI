PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;

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

CREATE TABLE IF NOT EXISTS AI_Wager(
	playedDate TEXT,
	result INTEGER,
	bet REAL,
	odd REAL,
	id INTEGER PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS AI_Bet(
	hometeam TEXT,
	awayteam TEXT,
	betDate TEXT,
	wagerId INTEGER,
	result INTEGER,
	odd REAL,
	CONSTRAINT PK_matches PRIMARY KEY(betDate, hometeam, awayteam),
	FOREIGN KEY (wagerID) REFERENCES AI_Wager(id)
);

/* firstNotUpdatedDate is the date from which up to present
day, bets should be checked for result updates. This is done
by checking if matches table contains bets which have not finished
(starting from firstNotUpdatedDate to present day).
If it does, check if the result matches and set result accordingly.
Also once all bets in a single wager are settled, set result for wager.*/
CREATE TABLE IF NOT EXISTS Log_Table(
	id INTEGER PRIMARY KEY,
	firstNotUpdatedDate TEXT
);

COMMIT;
PRAGMA foreign_keys=ON;