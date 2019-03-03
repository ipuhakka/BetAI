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

/*result: 0 = no result yet, -1 = Lost, 1=won*/
CREATE TABLE IF NOT EXISTS AI_Wager(
	playedDate DATE,
	result INTEGER,
	bet REAL,
	odd REAL,
	id INTEGER PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS AI_Bet(
	matchDate DATE,
	hometeam TEXT,
	awayteam TEXT,
	result INTEGER,
	wagedResult CHAR,
	odd REAL,
	CONSTRAINT PK_AI_Bet PRIMARY KEY(matchDate, hometeam, awayteam)
);

CREATE TABLE IF NOT EXISTS Bet_Wager(
	wagerId INTEGER,
	matchDate DATE,
	hometeam TEXT,
	awayteam TEXT,
	FOREIGN KEY (matchDate, hometeam, awayteam) REFERENCES AI_Bet(matchDate, hometeam, awayteam) ON DELETE CASCADE,
	FOREIGN KEY (wagerId) REFERENCES AI_Wager(id) ON DELETE CASCADE,
	CONSTRAINT Bet_Wager_Key PRIMARY KEY (matchDate, hometeam, awayteam, wagerId)
);

COMMIT;
PRAGMA foreign_keys=ON;