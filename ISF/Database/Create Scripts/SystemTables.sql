 -- =============================================
-- SystemTables.sql
--		Creating system tables:
--			USERS
--			SEASONS
--			MESSAGES
-- =============================================

-- Deleting existing tables
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'USERS' AND type = 'U')
	DROP TABLE USERS
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'SEASONS' AND type = 'U')
	DROP TABLE SEASONS
GO

IF EXISTS(SELECT * FROM sysobjects WHERE name = 'MESSAGES' AND type = 'U')
	DROP TABLE MESSAGES
GO

-- Creating tables
-- ============================================

-- =======    USERS TABLE   ======
CREATE TABLE  USERS (
USER_ID				int				IDENTITY (1,1),
USER_LOGIN			nvarchar(20)	NOT NULL,
USER_FIRST_NAME		nvarchar(30)	 NOT NULL,
USER_LAST_NAME		nvarchar(30),
REGION_ID			int				 NOT NULL, 
SCHOOL_ID			int, 
USER_TYPE			int				 NOT NULL, -- 1 Application user, 2 Web user
USER_PERMISSIONS	int				 NOT NULL, --0 to 10, higher value ->more permissions
USER_PASSWORD		nvarchar(200), 
USER_EMAIL			nvarchar(200),
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_USERS PRIMARY KEY(USER_ID),
CONSTRAINT UN_USER_LOGIN UNIQUE(USER_LOGIN, DATE_DELETED),
CONSTRAINT CHK_USERS_TYPE CHECK (USER_TYPE IN (1, 2)), 
CONSTRAINT FK_USERS_REGION FOREIGN KEY(REGION_ID)
	REFERENCES REGIONS(REGION_ID),
CONSTRAINT FK_USERS_SCHOOL FOREIGN KEY(SCHOOL_ID)
	REFERENCES SCHOOLS(SCHOOL_ID))
GO

-- =======    SEASONS TABLE   ======
CREATE TABLE  SEASONS (
SEASON				int NOT NULL,
NAME				nvarchar(50) NOT NULL, 
STATUS				int NOT NULL, -- 0 new, 1 opened, 2 closed
START_DATE			datetime,
END_DATE			datetime,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_SEASONS PRIMARY KEY(SEASON),
CONSTRAINT CHK_SEASONS_STATUS CHECK (STATUS IN (0, 1, 2))
)
GO

-- =======    MESSAGES TABLE   ======
CREATE TABLE  MESSAGES (
MESSAGE_ID		int	IDENTITY(1, 1), 
USER_ID			int	NOT NULL,
MESSAGE_TYPE	int, 
MESSAGE_STATUS	int	NOT NULL, 
MESSAGE_TEXT	nvarchar(255) NOT NULL, 
TIME_SENT		datetime	NOT NULL,
TIME_READ		datetime,
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_MESSAGES PRIMARY KEY(MESSAGE_ID),
CONSTRAINT FK_MESSAGES_USER FOREIGN KEY(USER_ID)
	REFERENCES USERS(USER_ID))
GO