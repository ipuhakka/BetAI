PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;

CREATE TABLE IF NOT EXISTS leagues(
	league TEXT PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS seasons(
	season CHAR(9) PRIMARY KEY
);

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
	matchInSeasonId INTEGER,
	CONSTRAINT PK_matches PRIMARY KEY(playedDate, hometeam, awayteam)
);

COMMIT;
PRAGMA foreign_keys=ON;