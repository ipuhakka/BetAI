PRAGMA foreign_keys=OFF;
BEGIN TRASACTION;

CREATE TABLENOT EXISTS matches(
	playedDate DATE,
	hometeam TEXT,
	awayteam TEXT
	homescore INTEGER,
	awayscore INTEGER,
	homeOdd REAL,
	drawOdd REAL,
	awayOdd REAL
);

COMMIT;
PRAGA foreign_keys=ON;