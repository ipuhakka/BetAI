# Project diary

**21.9.2018:** Started documentation, outlined project components and
basic functionality.

**22.9.2018:** Made requirements for match database. Created *Database* and *DatabaseTestProject*.
Installed necessary packages for the projects.

Next up -> create database schema, test data and start to code the database layer. 

**23.9.2018:** Updated requirements for the database. Created database schema, access layer methods for creating
and deleting the database, and setting schema.

Added AddMatches method, clearing tables and tests for both. Changed schema: playedDate is TEXT, not DATE.

Added SelectNLastFromTeam, all requirements now fulfilled. Changes are possible once it is noticed how
fast/slow this solution works with BetSimulator.

**24.9.2018:** Made requirements for DataParser. Implemented loading file and error handling for it. Created test cases, all run ok.

**25.9.2018:** Created CSVParser and SearchParams classes for DataParser. Implemented first version and a test case (runs) for CSVParser.

Next up -> Full unit test case design for CSVParser.

**26.9.2018:** Unit tests created, all run ok. Next -> Add as much data as possible to the database. Fixed CSVParser bug about not filtering
linvalid lines out of the data.

**27.9.2018:** Added requirements to Database, implemented new requirements and tests for them.

**28.9.2018**: Changed the database structure completely. Now there are three tables: matches, seasons, and leagues.
Leagues and seasons make it easier to secure unification of data. Now if some match has incorrect values for these,
database won't allow for adding these matches. Test run OK.

**29.9.2018**: Introduced branching to development, so there is always a stable branch and a branch for developemnt changes.
Removed seasons and leagues table, and column matchInSeasonId from matches. These were unnecessary for the system. Tests run OK,
merged development to master.

Added methods for counting season average goals in home and away matches. Tests run OK
