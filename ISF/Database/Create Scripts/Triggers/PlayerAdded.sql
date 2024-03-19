 -- ================================================================================
--Player Added (Registered) - 
--Sending message to the Supervisor of the championship, if the player's team
--Supervisor is not also the championship supervisor.
--Also add charge to the school for which the player has been registered.
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_PLAYER_ADDED' AND type = 'TR')
	DROP TRIGGER T_PLAYER_ADDED
GO

-- =======    PLAYER_ADDED TRIGGER   ======
CREATE TRIGGER T_PLAYER_ADDED
ON PLAYERS
AFTER INSERT AS
BEGIN
	DECLARE @user int	
	DECLARE @team int
	DECLARE @count int
	DECLARE @school int
	DECLARE @player int
	DECLARE @price float
	DECLARE @sport int --type of sport: 1 Competition 2 Match
	DECLARE @region int
	DECLARE @city int
	
	DECLARE team_cursor_1 CURSOR FOR
	SELECT DISTINCT TEAM_ID FROM inserted ins
	
	OPEN team_cursor_1	
	FETCH NEXT FROM team_cursor_1
	INTO @team
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE TEAMS SET PLAYERS_COUNT=PLAYERS_COUNT+(SELECT COUNT(PLAYER_ID) FROM inserted WHERE TEAM_ID=@team)
		WHERE TEAM_ID=@team
		
		--advance to next record:
		FETCH NEXT FROM team_cursor_1
		INTO @team
	END
	CLOSE team_cursor_1
	DEALLOCATE team_cursor_1
	
	-- =======    send messages to team supervisors   ======
	DECLARE message_cursor CURSOR FOR
	SELECT c.CHAMPIONSHIP_SUPERVISOR, t.TEAM_ID, COUNT(ins.PLAYER_ID) As Total
	FROM inserted ins, TEAMS t, CHAMPIONSHIPS c
	WHERE ins.TEAM_ID=t.TEAM_ID AND t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID
	AND t.TEAM_SUPERVISOR<>c.CHAMPIONSHIP_SUPERVISOR
	AND c.CHAMPIONSHIP_SUPERVISOR IS NOT NULL
	GROUP BY c.CHAMPIONSHIP_SUPERVISOR, t.TEAM_ID	
	
	OPEN message_cursor
	
	FETCH NEXT FROM message_cursor
	INTO @user, @team, @count
	
	--loop over the supervisors and send messages:
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
			TIME_SENT)
			VALUES(@user, 4, 1, 'בית הספר "{?team='+CAST(@team AS varchar(10))+'}" שלח בקשה לרישום '+CAST(@count AS varchar(5))+' שחקנים', GETDATE())
		
		--advance to next record:
		FETCH NEXT FROM message_cursor
		INTO @user, @team, @count
	END
	
	CLOSE message_cursor
	DEALLOCATE message_cursor
	
	-- =======    add charge for the school, only if player is confirmed   ======	
	--DECLARE charge_cursor CURSOR FOR
	--SELECT s.SCHOOL_ID, ins.PLAYER_ID, sp.SPORT_TYPE, cc.REGISTRATION_PRICE, sc.REGION_ID, sc.CITY_ID
	--FROM inserted ins, STUDENTS s, SCHOOLS sc, TEAMS t, CHAMPIONSHIP_CATEGORIES cc, 
	--	CHAMPIONSHIPS c, SPORTS sp
	--WHERE ins.STUDENT_ID=s.STUDENT_ID AND ins.TEAM_ID=t.TEAM_ID
	--AND s.SCHOOL_ID=sc.SCHOOL_ID
	--AND t.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID
	--AND cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND c.SPORT_ID=sp.SPORT_ID	
	--AND ins.STATUS=2
	--
	--OPEN charge_cursor
	--
	--FETCH NEXT FROM charge_cursor
	--INTO @school, @player, @sport, @price, @region, @city
	--
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
		--charge only if the championship is of type competition and price is positive:
	--	IF @sport=1 AND @price>0
	--	BEGIN
	--		DECLARE @charge int
	--		EXEC SP_INSERT_SCHOOL_CHARGE @school, 2, @price, @player, @charge OUTPUT
	--		
	--		UPDATE PLAYERS
	--		SET CHARGE_ID = @charge
	--		WHERE PLAYER_ID = @player
	--	END
	--	
	--	FETCH NEXT FROM charge_cursor
	--	INTO @school, @player, @sport, @price, @region, @city
	--END
	--
	--CLOSE charge_cursor
	--DEALLOCATE charge_cursor
END
GO
