-- =============================================
-- ChampionshipTables.sql
--		Creating championship tables:
--			CHAMPIONSHIPS
--			CHAMPIONSHIP_CATEGORIES
--			games
--			lists
--          scores
-- =============================================

-- Deleting existing tables
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'TEAMS' AND type = 'U')
	DROP TABLE TEAMS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'PLAYERS' AND type = 'U')
	DROP TABLE PLAYERS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'STANDARD_CHAMPIONSHIPS' AND type = 'U')
	DROP TABLE STANDARD_CHAMPIONSHIPS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_COMPETITION_COMPETITORS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_COMPETITION_COMPETITORS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_COMPETITION_HEATS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_COMPETITION_HEATS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_COMPETITIONS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_COMPETITIONS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_MATCHES' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_MATCHES
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_TOURNAMENTS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_TOURNAMENTS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_CYCLES' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_CYCLES
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_ROUNDS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_ROUNDS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_GROUP_TEAMS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_GROUP_TEAMS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_GROUPS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_GROUPS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_PHASES' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_PHASES
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_REGIONS' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_REGIONS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_CATEGORIES' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_CATEGORIES
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIPS' AND type = 'U')
	DROP TABLE CHAMPIONSHIPS
GO

IF EXISTS(SELECT * FROM sysobjects WHERE name = 'STANDARD_CHAMPIONSHIP_CATEGORIES' AND type = 'U')
	DROP TABLE STANDARD_CHAMPIONSHIP_CATEGORIES
GO

IF EXISTS(SELECT * FROM sysobjects WHERE name = 'CHAMPIONSHIP_MATCH_FUNCTIONARIES' AND type = 'U')
	DROP TABLE CHAMPIONSHIP_MATCH_FUNCTIONARIES
GO

-- =======    STANDARD_CHAMPIONSHIPS TABLE   ======
CREATE TABLE STANDARD_CHAMPIONSHIPS (
STANDARD_CHAMPIONSHIP_ID	int				IDENTITY (1,1),
STANDARD_CHAMPIONSHIP_NAME	nvarchar(50),
SPORT_ID					int NOT NULL,
IS_REGIONAL					int NOT NULL, -- 0 no, 1 yes
IS_OPEN						int NOT NULL, -- 0 no, 1 yes for central championships only.
IS_CLUBS					int NOT NULL,
RULESET_ID					int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_STANDARD_CHAMPIONSHIPS PRIMARY KEY(STANDARD_CHAMPIONSHIP_ID),
CONSTRAINT UN_STDCHAMP_NAMEREG UNIQUE(STANDARD_CHAMPIONSHIP_NAME, IS_REGIONAL, DATE_DELETED),
CONSTRAINT FK_STDCHAMP_SPORT FOREIGN KEY(SPORT_ID)
	REFERENCES SPORTS(SPORT_ID),
CONSTRAINT FK_STDCHAMP_RULESET FOREIGN KEY(RULESET_ID)
	REFERENCES RULESETS(RULESET_ID),
CONSTRAINT CHK_STDCHAMP_REGIONAL CHECK (IS_REGIONAL = 0 OR IS_REGIONAL = 1),
CONSTRAINT CHK_STDCHAMP_OPEN CHECK (IS_OPEN = 0 OR IS_OPEN = 1),
CONSTRAINT CHK_STDCHAMP_CLUBS CHECK (IS_CLUBS = 0 OR IS_CLUBS = 1)
)
GO	

-- =======    STANDARD_CHAMPIONSHIP_CATEGORIES TABLE   ======
CREATE TABLE STANDARD_CHAMPIONSHIP_CATEGORIES (
STANDARD_CHAMPIONSHIP_CATEGORY_ID	int		IDENTITY (1,1),
STANDARD_CHAMPIONSHIP_ID			int		NOT NULL,
CATEGORY							int		NOT NULL,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_STANDARD_CHAMPIONSHIP_CATEGORIES PRIMARY KEY(STANDARD_CHAMPIONSHIP_CATEGORY_ID),
CONSTRAINT FK_STANDARD_CHAMPIONCAT_CHAMPIONSHIP FOREIGN KEY(STANDARD_CHAMPIONSHIP_ID)
	REFERENCES STANDARD_CHAMPIONSHIPS(STANDARD_CHAMPIONSHIP_ID)
)
GO

-- =======    CHAMPIONSHIPS TABLE   ======
CREATE TABLE CHAMPIONSHIPS (
CHAMPIONSHIP_ID				int				IDENTITY (1,1),
SEASON						int				NOT NULL,
CHAMPIONSHIP_NAME			nvarchar(50),
REGION_ID					int,
SPORT_ID					int,
IS_CLUBS					int				NOT NULL,
LAST_REGISTRATION_DATE		datetime,
START_DATE					datetime,
END_DATE					datetime,
ALT_START_DATE				datetime,
ALT_END_DATE				datetime,
FINALS_DATE					datetime,
ALT_FINALS_DATE				datetime,
RULESET_ID					int,
IS_OPEN						int,		--0/1 for central championships only.
CHAMPIONSHIP_STATUS			int				NOT NULL	DEFAULT 0,  --0/1/2
CHAMPIONSHIP_SUPERVISOR		int,
STANDARD_CHAMPIONSHIP_ID	int,
REMARKS						nvarchar(255),
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIPS PRIMARY KEY(CHAMPIONSHIP_ID),
CONSTRAINT FK_CHAMPIONSHIPS_SEASON FOREIGN KEY(SEASON)
	REFERENCES SEASONS(SEASON),
CONSTRAINT FK_CHAMPIONSHIPS_REGION FOREIGN KEY(REGION_ID)
	REFERENCES REGIONS(REGION_ID),
CONSTRAINT UN_CHAMPIONSHIPS_NAME UNIQUE(SEASON, REGION_ID, CHAMPIONSHIP_NAME, DATE_DELETED),
CONSTRAINT FK_CHAMPIONSHIPS_SPORT FOREIGN KEY(SPORT_ID)
	REFERENCES SPORTS(SPORT_ID),
CONSTRAINT FK_CHAMPIONSHIPS_RULESET FOREIGN KEY(RULESET_ID)
	REFERENCES RULESETS(RULESET_ID),
CONSTRAINT FK_CHAMPIONSHIPS_SUPERVISOR FOREIGN KEY(CHAMPIONSHIP_SUPERVISOR)
	REFERENCES USERS(USER_ID),
CONSTRAINT FK_CHAMPIONSHIPS_STANDARD FOREIGN KEY(STANDARD_CHAMPIONSHIP_ID)
	REFERENCES STANDARD_CHAMPIONSHIPS(STANDARD_CHAMPIONSHIP_ID),
CONSTRAINT CHK_CHAMPIONSHIP_CLUB CHECK (IS_CLUBS = 0 OR IS_CLUBS = 1)
)
GO

-- =======    CHAMPIONSHIP_CATEGORIES TABLE   ======
CREATE TABLE CHAMPIONSHIP_CATEGORIES (
CHAMPIONSHIP_CATEGORY_ID	int		IDENTITY (1,1),
CHAMPIONSHIP_ID				int		NOT NULL,
CATEGORY					int		NOT NULL,
STATUS						int		NOT NULL	DEFAULT 0, -- 0 not executed, 1 executed
REGISTRATION_PRICE			real	NOT NULL	DEFAULT 0,
MAX_STUDENT_BIRTHDAY		datetime,
CHAMPIONSHIP_CATEGORY_INDEX int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_CATEGORIES PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID),
CONSTRAINT FK_CHAMPIONCAT_CHAMPIONSHIP FOREIGN KEY(CHAMPIONSHIP_ID)
	REFERENCES CHAMPIONSHIPS(CHAMPIONSHIP_ID), 
CONSTRAINT CHK_CHAMPIONSHIP_REG_PRICE CHECK 
	(REGISTRATION_PRICE >= 0 AND REGISTRATION_PRICE <= 999999),
CONSTRAINT CHK_CHAMPIONCAT_STATUS CHECK 
	(STATUS = 0 or STATUS = 1)
)
GO

-- =======    CHAMPIONSHIP_REGIONS TABLE   ======
CREATE TABLE CHAMPIONSHIP_REGIONS (
CHAMPIONSHIP_REGION_ID	int		IDENTITY (1,1),
CHAMPIONSHIP_ID			int		NOT NULL,
REGION_ID				int		NOT NULL,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_REGIONS PRIMARY KEY(CHAMPIONSHIP_REGION_ID),
CONSTRAINT FK_CHAMPIONREGION_CHAMPIONSHIP FOREIGN KEY(CHAMPIONSHIP_ID)
	REFERENCES CHAMPIONSHIPS(CHAMPIONSHIP_ID),
CONSTRAINT FK_CHAMPIONREGION_REGION FOREIGN KEY(REGION_ID)
	REFERENCES REGIONS(REGION_ID)
)
GO

-- =======    CHAMPIONSHIP_PHASES   ======
CREATE TABLE CHAMPIONSHIP_PHASES (
CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
PHASE						int				NOT NULL,
PHASE_NAME					nvarchar(100)	NOT NULL,
STATUS						int				NOT NULL, -- 0 plan, 1 ready, 2 started, 3 ended
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_PHASES PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE),
CONSTRAINT FK_CHAMPPHASE_CATEGORY FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID)
	REFERENCES CHAMPIONSHIP_CATEGORIES(CHAMPIONSHIP_CATEGORY_ID),
CONSTRAINT UN_CHAMPPHASE_NAME UNIQUE(CHAMPIONSHIP_CATEGORY_ID, PHASE_NAME, DATE_DELETED),
CONSTRAINT CHK_CHAMPPHASE_STATUS CHECK (STATUS IN (0, 1, 2, 3))
)
GO

-- =======    CHAMPIONSHIP_GROUPS   ======
CREATE TABLE CHAMPIONSHIP_GROUPS (
CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
PHASE						int				NOT NULL,
NGROUP						int				NOT NULL,
GROUP_NAME					nvarchar(100)	NOT NULL,
STATUS						int				NOT NULL, -- 0 plan, 1 ready, 2 started, 3 ended
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_GROUPS PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP),
CONSTRAINT FK_CHAMPGROUP_PHASE FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE)
	REFERENCES CHAMPIONSHIP_PHASES(CHAMPIONSHIP_CATEGORY_ID, PHASE),
CONSTRAINT UN_CHAMPGROUP_NAME UNIQUE(CHAMPIONSHIP_CATEGORY_ID, PHASE, GROUP_NAME, DATE_DELETED),
CONSTRAINT CHK_CHAMPGROUP_STATUS CHECK (STATUS IN (0, 1, 2, 3))
)
GO

-- =======    TEAMS TABLE   ======
CREATE TABLE TEAMS (
TEAM_ID						int				IDENTITY (1,1),
SCHOOL_ID					int				NOT NULL,
CHAMPIONSHIP_ID				int				NOT NULL,
CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
STATUS						int				NOT NULL,
TEAM_INDEX					int				DEFAULT 0, 
CHARGE_ID					int,
TEAM_SUPERVISOR				int,
PLAYER_NUMBER_FROM			int,
PLAYER_NUMBER_TO			int,
REGISTRATION_DATE			datetime	NOT NULL DEFAULT GETDATE(),
PLAYERS_COUNT				int				DEFAULT 0,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_TEAMS PRIMARY KEY(TEAM_ID),
CONSTRAINT UN_TEAMS_SCHOOL UNIQUE (CHAMPIONSHIP_CATEGORY_ID,SCHOOL_ID,TEAM_INDEX,DATE_DELETED),
CONSTRAINT FK_TEAMS_SCHOOL FOREIGN KEY(SCHOOL_ID)
	REFERENCES SCHOOLS(SCHOOL_ID),
CONSTRAINT FK_TEAMS_CHAMPION FOREIGN KEY(CHAMPIONSHIP_ID)
	REFERENCES CHAMPIONSHIPS(CHAMPIONSHIP_ID),
CONSTRAINT FK_TEAMS_CATEGORY FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID)
	REFERENCES CHAMPIONSHIP_CATEGORIES(CHAMPIONSHIP_CATEGORY_ID),
CONSTRAINT FK_TEAMS_CHARGE FOREIGN KEY(CHARGE_ID)
	REFERENCES CHARGES(CHARGE_ID),
CONSTRAINT FK_TEAMS_SUPERVISOR FOREIGN KEY(TEAM_SUPERVISOR)
	REFERENCES USERS(USER_ID)
)
GO

-- =======    PLAYERS TABLE    ======
CREATE TABLE  PLAYERS (
PLAYER_ID			int		IDENTITY (1,1),
STUDENT_ID			int		NOT NULL,
TEAM_ID				int		NOT NULL,
TEAM_NUMBER			int,
CHARGE_ID			int,
STATUS				int,
REMARKS				nvarchar(100), 
REGISTRATION_DATE	datetime	NOT NULL DEFAULT GETDATE(),
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_PLAYERS PRIMARY KEY(PLAYER_ID),
CONSTRAINT FK_PLAYER_STUDENT FOREIGN KEY(STUDENT_ID)
	REFERENCES STUDENTS(STUDENT_ID),
CONSTRAINT FK_PLAYER_TEAM FOREIGN KEY(TEAM_ID)
	REFERENCES TEAMS(TEAM_ID),
CONSTRAINT FK_PLAYERS_CHARGE FOREIGN KEY(CHARGE_ID)
	REFERENCES CHARGES(CHARGE_ID),
CONSTRAINT UN_PLAYER_TEAM_STUDENT UNIQUE(TEAM_ID, STUDENT_ID, DATE_DELETED)
	)
GO

-- =======    CHAMPIONSHIP_GROUP_TEAMS   ======
CREATE TABLE CHAMPIONSHIP_GROUP_TEAMS (
CHAMPIONSHIP_CATEGORY_ID	int NOT NULL,
PHASE						int	NOT NULL,
NGROUP						int	NOT NULL,
POSITION					int NOT NULL,
PREVIOUS_GROUP				int,
PREVIOUS_POSITION			int,
TEAM_ID						int,
GAMES						int,
POINTS						int,
POINTS_AGAINST				int,
SMALL_POINTS				int,
SMALL_POINTS_AGAINST		int,
SCORE						real,
RESULT_POSITION				int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_GROUP_TEAMS PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, POSITION),
CONSTRAINT FK_CHAMPGROUPTEAM_GROUP FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP)
	REFERENCES CHAMPIONSHIP_GROUPS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP),
CONSTRAINT FK_CHAMPGROUPTEAM_TEAM FOREIGN KEY(TEAM_ID)
	REFERENCES TEAMS(TEAM_ID),
CONSTRAINT CHK_CHAMPGROUP_TEAM CHECK (
	(PREVIOUS_GROUP IS NOT NULL AND PREVIOUS_POSITION IS NOT NULL) OR
	(TEAM_ID IS NOT NULL)),
CONSTRAINT CHK_CHAMPGROUP_SCORE CHECK (
	(TEAM_ID IS NOT NULL) OR (GAMES IS NULL AND SCORE IS NULL))
)
GO

-- =======    CHAMPIONSHIP_ROUNDS   ======
-- The table devides the championship matches to rounds
-- The matches are for match type sports only
CREATE TABLE CHAMPIONSHIP_ROUNDS (
CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
PHASE						int				NOT NULL,
NGROUP						int				NOT NULL,
ROUND						int				NOT NULL,
ROUND_NAME					nvarchar(100)	NOT NULL,
STATUS						int				NOT NULL, -- 0 plan, 1 ready, 2 started, 3 ended
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_ROUNDS PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND),
CONSTRAINT FK_CHAMPROUND_GROUP FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP)
	REFERENCES CHAMPIONSHIP_GROUPS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP),
CONSTRAINT UN_CHAMPROUND_NAME UNIQUE(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND_NAME, DATE_DELETED),
CONSTRAINT CHK_CHAMPROUND_STATUS CHECK (STATUS IN (0, 1, 2, 3))
)
GO

-- =======    CHAMPIONSHIP_CYCLES   ======
-- The table devides the championship rounds to cycle
-- The cycles are for match type sports only
CREATE TABLE CHAMPIONSHIP_CYCLES (
CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
PHASE						int				NOT NULL,
NGROUP						int				NOT NULL,
ROUND						int				NOT NULL,
CYCLE						int				NOT NULL,
CYCLE_NAME					nvarchar(100)	NOT NULL,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_CYCLES PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE),
CONSTRAINT FK_CHAMPCYCLE_ROUND FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND)
	REFERENCES CHAMPIONSHIP_ROUNDS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND),
CONSTRAINT UN_CHAMPCYCLE_NAME UNIQUE(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE_NAME, DATE_DELETED)
)
GO

-- =======    CHAMPIONSHIP_TOURNAMENTS   ======
-- The table adds tournaments to cycles
-- The tournaments are for match type sports only
CREATE TABLE CHAMPIONSHIP_TOURNAMENTS (
CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
PHASE						int				NOT NULL,
NGROUP						int				NOT NULL,
ROUND						int				NOT NULL,
CYCLE						int				NOT NULL,
TOURNAMENT					int				NOT NULL,
NUMBER						int				NOT NULL,
TIME						datetime,
FACILITY_ID					int,
COURT_ID					int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_TOURNAMENTS PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, TOURNAMENT),
CONSTRAINT FK_CHAMPTOUR_CYCLE FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE)
	REFERENCES CHAMPIONSHIP_CYCLES(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE),
CONSTRAINT UN_CHAMPTOUR_NUMBER UNIQUE(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, NUMBER, DATE_DELETED)
)
GO

-- =======    CHAMPIONSHIP_MATCHES TABLE   ======
-- The table contains all matches for a group in a phase
-- The matches are for match type sports only
CREATE TABLE CHAMPIONSHIP_MATCHES (
CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
PHASE						int	NOT NULL,
NGROUP						int	NOT NULL,
ROUND						int	NOT NULL,
CYCLE						int	NOT NULL,
MATCH						int NOT NULL,
TOURNAMENT					int,
MATCH_NUMBER				int NOT NULL,
TEAM_A						int,-- NOT NULL, adding match-relative team
TEAM_B						int,-- NOT NULL,
RELATIVE_TEAM_A				int,
RELATIVE_TEAM_B				int,
TIME						datetime,
FACILITY_ID					int,
COURT_ID					int,
TEAM_A_SCORE				real,
TEAM_B_SCORE				real,
RESULT						int, -- 0 tie, 1 teamA win, 2 teamB win, 3 teamA technical win, 4 teamB technical win
PARTS_RESULT				nvarchar(2000),
REFEREE_COUNT				int NOT NULL DEFAULT 0,
CUSTOM_TEAM_A				nvarchar(50),
CUSTOM_TEAM_B				nvarchar(50),
DATE_CHANGED_DATE			datetime,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_MATCHES PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, MATCH),
CONSTRAINT FK_CHAMPMATCH_CYCLE FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE)
	REFERENCES CHAMPIONSHIP_CYCLES(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE),
CONSTRAINT FK_CHAMPMATCH_TEAMA FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, TEAM_A)
	REFERENCES CHAMPIONSHIP_GROUP_TEAMS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, POSITION),
CONSTRAINT FK_CHAMPMATCH_TEAMB FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, TEAM_B)
	REFERENCES CHAMPIONSHIP_GROUP_TEAMS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, POSITION),
CONSTRAINT FK_CHAMPMATCH_FACILITY FOREIGN KEY(FACILITY_ID)
	REFERENCES FACILITIES(FACILITY_ID),
CONSTRAINT FK_CHAMPMATCH_COURT FOREIGN KEY(COURT_ID)
	REFERENCES COURTS(COURT_ID),
CONSTRAINT FK_CHAMPMATCH_TOUR FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, TOURNAMENT)
	REFERENCES CHAMPIONSHIP_TOURNAMENTS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, TOURNAMENT),
CONSTRAINT CHK_CHAMPMATCH_NUMBER CHECK (MATCH_NUMBER > 0),
CONSTRAINT CHK_CHAMPMATCH_COURFAC CHECK (FACILITY_ID IS NOT NULL OR COURT_ID IS NULL),
CONSTRAINT CHK_CHAMPMATCH_RESULT CHECK (RESULT IN (0, 1, 2, 3, 4))
)
GO

-- =======    CHAMPIONSHIP_COMPETITIONS TABLE   ======
-- The table contains all competitions in a group in a phase
-- The competitions are for competition type sports only
CREATE TABLE CHAMPIONSHIP_COMPETITIONS (
CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
PHASE						int	NOT NULL,
NGROUP						int	NOT NULL,
COMPETITION					int	NOT NULL,
SPORT_FIELD_ID				int NOT NULL,
TIME						datetime,
FACILITY_ID					int,
COURT_ID					int,
LANE_COUNT					int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_COMPETITIONS PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION),
CONSTRAINT FK_CHAMPCOMPT_GROUP FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP)
	REFERENCES CHAMPIONSHIP_GROUPS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP),
CONSTRAINT FK_CHAMPCOMPT_FIELD FOREIGN KEY(SPORT_FIELD_ID)
	REFERENCES SPORT_FIELDS(SPORT_FIELD_ID),
CONSTRAINT FK_CHAMPCOMPT_FACILITY FOREIGN KEY(FACILITY_ID)
	REFERENCES FACILITIES(FACILITY_ID),
CONSTRAINT FK_CHAMPCOMPT_COURT FOREIGN KEY(COURT_ID)
	REFERENCES COURTS(COURT_ID)
)
GO

-- =======    CHAMPIONSHIP_COMPETITION_HEATS TABLE   ======
-- The table devide championship competitions competitors into heats
-- The competition heats are for competition type sports only
CREATE TABLE CHAMPIONSHIP_COMPETITION_HEATS (
CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
PHASE						int	NOT NULL,
NGROUP						int	NOT NULL,
COMPETITION					int	NOT NULL,
HEAT						int NOT NULL,
TIME						datetime,
FACILITY_ID					int,
COURT_ID					int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_COMPETITION_HEATS PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION, HEAT),
CONSTRAINT FK_CHAMPHEAT_COMPETITION FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION)
	REFERENCES CHAMPIONSHIP_COMPETITIONS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION),
CONSTRAINT FK_CHAMPHEAT_FACILITY FOREIGN KEY(FACILITY_ID)
	REFERENCES FACILITIES(FACILITY_ID),
CONSTRAINT FK_CHAMPHEAT_COURT FOREIGN KEY(COURT_ID)
	REFERENCES COURTS(COURT_ID)
)
GO

-- =======    CHAMPIONSHIP_COMPETITION_COMPETITORS TABLE   ======
-- The table contains the compertitors of each heat or competition
-- The competitors are for competition type sports only
CREATE TABLE CHAMPIONSHIP_COMPETITION_COMPETITORS (
CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
PHASE						int	NOT NULL,
NGROUP						int	NOT NULL,
COMPETITION					int	NOT NULL,
COMPETITOR					int NOT NULL,
PLAYER_NUMBER				int,
PLAYER_ID					int,
HEAT						int,
POSITION					int,
RESULT_POSITION				int,
RESULT						int,
SCORE						int,
CUSTOM_POSITION				int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_COMPETITION_COMPETITORS PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION, COMPETITOR),
CONSTRAINT FK_CHAMPCOMPETITOR_COMPETITION FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION)
	REFERENCES CHAMPIONSHIP_COMPETITIONS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION),
CONSTRAINT FK_CHAMPCOMPETITOR_HEAT FOREIGN KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION, HEAT)
	REFERENCES CHAMPIONSHIP_COMPETITION_HEATS(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, COMPETITION, HEAT),
CONSTRAINT FK_CHAMPCOMPETITOR_PLAYER FOREIGN KEY(PLAYER_ID)
	REFERENCES PLAYERS(PLAYER_ID)
)
GO

-- =======    CHAMPIONSHIP_MATCH_FUNCTIONARIES TABLE   ======
-- The table contains the functionaries for each match.
CREATE TABLE CHAMPIONSHIP_MATCH_FUNCTIONARIES (
CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
PHASE						int	NOT NULL,
NGROUP						int	NOT NULL,
ROUND						int	NOT NULL,
CYCLE						int NOT NULL,
MATCH						int NOT NULL,
ROLE						int NOT NULL,
FUNCTIONARY_ID				int,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_CHAMPIONSHIP_MATCH_FUNCTIONARIES PRIMARY KEY(CHAMPIONSHIP_CATEGORY_ID, PHASE, NGROUP, ROUND, CYCLE, MATCH, ROLE),
CONSTRAINT FK_CHAMP_MATCH_FUNCTIONARY FOREIGN KEY(FUNCTIONARY_ID)
	REFERENCES FUNCTIONARIES(FUNCTIONARY_ID))
GO