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

**30.9.2018**: Added BetAI to project. Made requirements for Match prediction. Refactored method names
in DB.
Implemented predicting matches. It can't yet distinquish situations where DB-calls throw 
NotEnoughDataException. One test case fails.

**1.10.2018**: Implemented PredictMatches throwing NotSimulatedException on catching NotEnoughDataException.
Fixed documentation mistakes for betting algorithm.

Implemented making bets based on requirements and algorithm shown in Documentation\BetAI.md.
Tests run OK. Algorithm leaves room for small performance tweaks if necessary. Testing should
be done more systematically.

Made requirements for individual node.

Documented requirements for sampling, implemented creating a sample and tests for requirements.
All tests run OK.

**2.10.2018**: 
Refactored Sample to create Match-list immediately. Added a performance test, which suggest that
an alteration would be in order. Database should contain a function to get a list of N matches
with row numbers provided by sample. This was implemented and SelectNthRow was deleted. Tests
were designed and run OK. Peformance with creating a sample of 2000 matches was increased,
before it took on average 13 seconds, now < 1 second.

Implemented Node.EvaluateFitness().

Fixed the fitness in requirements and implementation. Now fitness is the profit made by bets.