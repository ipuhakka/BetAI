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

Next up -> Design tests for Node, start working on initializing generation 0, Crossover and Mutation. 

**3.10.2018**: Yesterdays performance test for Node suggests that an in memory access for match-data
is required. This means removing all database query matches methods and handling querying the
results in BetAI.

Change was implemented, unused functions in DB-class deleted, tests ported for new class,
and tests run OK. 

**4.10.2018**: Although performance was raised yesterday, it still needs honing
in order for the simulation to be faster. 

Created Master-class, which runs the simulation. 

Implemented MatchData structure to match simulation: Now, instead of 
getting previous matches and goal averages for each simulated match
individually, they are searched only once, according to the needs 
of the node with biggest SimulationSampleSize. Individual nodes
then cut these readily made arrays of matches into sized chunks that 
they need. Now, matchData is not searched by lists but by structure. 
Many of the list operations were also converted for array use instead 
of lists. 

Now, from initial 13-15 second runtime for node, algorithm got down to
1.5 seconds time by yesterdays changes, and after this performance refactoring 
it now takes anything from 0.2ms - 30ms. This cuts the performance of
fitness evaluation for 2000 nodes from 7-9 hours to < 1 minute. Goal set
acceptable was one evalution per second, but now the system is clearly above that.

**8.10.2018**: 
Started working on crossover of nodes to produce new nodes. Implemented Crossover.

Changed Outline.md to say that minimumStake is not controlled
by algorithm: It is not used as an optimized parameter. This is
because changing stake would produce results that vary from the
goal of the program: Goal is to analyze risks better, and 
if the stake is same for all nodes, this can be achieved better.

Changed Node constructor to throw ArgumentException on invalid parameters given.

Created tests for Crossover and added new tests for Node to test ArgumentException being
thrown. All tests run OK.

**9.10.2018**: 
Created a new Constructor for node which provides the possibility
to generate random first generation for the simulation.
Created test cases to verify that produces values are valid.
Modified Node constructor to make sure that values are within the set limits. 

Next up -> Create method to choose Nodes for Crossover and start to implement Master.cs. 

**10.10.2018**: 
Implemented selection of parents and test cases. Tests run ok.

Next up -> Design starting in Master.

**11.10.2018**:
Implemented initializing a save. Created tests, they run OK.

Fixed documentation error in BetAI.README.

Implemented writing generation data to Files\savefile\gen_data\gen{i}.


**12.10.2018**: Implemented loading a generation of nodes.
 Next up -> create test cases.
 
 **13.10.2018**: 
 Created test cases for loading a generation. Created a JsonConstructor
 for node. All tests run OK. Added a function to see if a save exists.
 
 Added Values.cs and implemented loading values from values.json. Implemented
 Master constructor.
 
 Next up -> Implement a function that returns the values from *values.json* for a save.
 
