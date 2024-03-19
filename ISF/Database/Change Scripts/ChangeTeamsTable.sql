-- =============================================
-- ChangeTeamsTable.sql
--		Change teams tables:
--			TEAMS
-- =============================================

-- =======    CHAMPIONSHIPS TABLE   ======
ALTER TABLE TEAMS DROP COLUMN TEAM_NAME
GO

ALTER TABLE TEAMS ADD
TEAM_INDEX int DEFAULT 0
GO

ALTER TABLE TEAMS ADD
CONSTRAINT UN_TEAMS_SCHOOL UNIQUE (CHAMPIONSHIP_CATEGORY_ID,SCHOOL_ID,TEAM_INDEX,DATE_DELETED)
GO
 