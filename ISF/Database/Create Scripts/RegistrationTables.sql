-- =============================================
-- RegistrationTables.sql
--		Creating registration tables:
--			PENDING_TEAMS
--			PENDING_PLAYERS
-- =============================================

-- Deleting existing tables
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'PENDING_PLAYERS' AND type = 'U')
	DROP TABLE PENDING_PLAYERS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'PENDING_TEAMS' AND type = 'U')
	DROP TABLE PENDING_TEAMS
GO


-- Creating tables
-- ============================================
-- =======    PENDING_TEAMS  TABLE    ======
CREATE TABLE  PENDING_TEAMS  (
PENDING_TEAM_ID		int		IDENTITY (1,1),
USER_ID				int		NOT NULL,	--user who asked to register the team.
SCHOOL_ID			int		NOT NULL,	--school to which to register the team.
CHAMP_CATEGORY_ID	int		NOT NULL,	--category for which to register the team.
TEAM_INDEX			int,				--index of team to register.
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_PENDING_TEAMS PRIMARY KEY(PENDING_TEAM_ID),
CONSTRAINT FK_PENDING_TEAMS_USER FOREIGN KEY(USER_ID)
	REFERENCES USERS(USER_ID),
CONSTRAINT FK_PENDING_TEAMS_SCHOOL FOREIGN KEY(SCHOOL_ID)
	REFERENCES SCHOOLS(SCHOOL_ID),
CONSTRAINT FK_PENDING_TEAMS_CATEGORY FOREIGN KEY(CHAMP_CATEGORY_ID)
	REFERENCES CHAMPIONSHIP_CATEGORIES(CHAMPIONSHIP_CATEGORY_ID))	
GO

-- =======    PENDING_PLAYERS  TABLE    ======
CREATE TABLE  PENDING_PLAYERS  (
PENDING_PLAYER_ID	int		IDENTITY (1,1),
USER_ID				int		NOT NULL,	--user who asked to register the player.
TEAM_ID				int		NOT NULL,	--team for which to register the player.
STUDENT_ID			int		NOT NULL,	--student id of player to register.
PLAYER_NUMBER		int,				--player number of player to register.
STATUS				int		NOT NULL DEFAULT 1, 
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_PENDING_PLAYERS PRIMARY KEY(PENDING_PLAYER_ID),
CONSTRAINT FK_PENDING_PLAYERS_USER FOREIGN KEY(USER_ID)
	REFERENCES USERS(USER_ID),
CONSTRAINT FK_PENDING_PLAYERS_TEAM FOREIGN KEY(TEAM_ID)
	REFERENCES TEAMS(TEAM_ID),
CONSTRAINT FK_PENDING_PLAYERS_STUDENT FOREIGN KEY(STUDENT_ID)
	REFERENCES STUDENTS(STUDENT_ID))	
GO