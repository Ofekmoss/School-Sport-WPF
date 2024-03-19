-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_PLAYER_DELETED' AND type = 'TR')
	DROP TRIGGER T_PLAYER_DELETED
GO

-- =======    PLAYER_DELETED TRIGGER   ======
CREATE TRIGGER T_PLAYER_DELETED
ON PLAYERS
AFTER DELETE AS
BEGIN
	DECLARE @team int
	
	--deleted
	DECLARE team_cursor_5 CURSOR FOR
	SELECT del.TEAM_ID FROM deleted del
	
	OPEN team_cursor_5	
	FETCH NEXT FROM team_cursor_5
	INTO @team
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE TEAMS SET PLAYERS_COUNT=PLAYERS_COUNT-1
		WHERE TEAM_ID=@team
		
		--advance to next record:
		FETCH NEXT FROM team_cursor_5
		INTO @team
	END
	CLOSE team_cursor_5
	DEALLOCATE team_cursor_5
END
GO