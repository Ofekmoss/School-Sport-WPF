-- ================================================================================
--Player Updated - 
--Sending message to the Supervisor of the player's team in case the player status
-- has been changed to Not Confirmed. for now, Confirmed won't generate message.
-- ================================================================================   

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_PLAYER_UPDATED' AND type = 'TR')
	DROP TRIGGER T_PLAYER_UPDATED
GO

-- =======    PLAYER_STATUS_CHANGED TRIGGER   ======
CREATE TRIGGER T_PLAYER_UPDATED
ON PLAYERS
AFTER UPDATE AS
BEGIN
	DECLARE @students TABLE (student_id int)
	DECLARE @team int
	DECLARE @player int	
	DECLARE @student_id int
	DECLARE @season int
	DECLARE @team_register_date datetime
	DECLARE @student_count int
	DECLARE @curteam int
	DECLARE @curstudent_count int	
	DECLARE @status int
	DECLARE @user int
	DECLARE @school nvarchar(50)
	DECLARE @remarks nvarchar(100)	
	
	SET @player = 0

	--PRINT 'T_PLAYER_UPDATED executing'	

	--confirmed player (changed from Registered to Confirmed)
	DECLARE confirmed_player_cursor CURSOR FOR
	SELECT DISTINCT ins.PLAYER_ID
	FROM deleted del, inserted ins
	WHERE del.PLAYER_ID=ins.PLAYER_ID 
	AND del.STATUS=1 AND ins.STATUS=2

	OPEN confirmed_player_cursor
	
	FETCH NEXT FROM confirmed_player_cursor
	INTO @player
	
	--loop over the players, take last.
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--advance to next record:
		FETCH NEXT FROM confirmed_player_cursor
		INTO @player
	END
	
	CLOSE confirmed_player_cursor
	DEALLOCATE confirmed_player_cursor
	
	--PRINT 'confirmed player id: ' + CONVERT(nvarchar(50), @player)

	IF (@player>0) BEGIN
		--copy
		select @team=p.TEAM_ID, @student_id=p.STUDENT_ID, @season=c.SEASON, @team_register_date=t.REGISTRATION_DATE
		from PLAYERS p, TEAMS t, CHAMPIONSHIPS c
		where p.TEAM_ID=t.TEAM_ID 
		and t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID 
		and p.PLAYER_ID=@player	
		
		--PRINT CONVERT(nvarchar(50), @team_register_date)
		
		INSERT INTO @students (student_id) (select distinct STUDENT_ID from PLAYERS where TEAM_ID=@team and PLAYER_ID<>@player and DATE_DELETED IS NULL)
		select @student_count=count(*) from @students
		
		--PRINT 'team players: ' + CONVERT(nvarchar(50), @student_count)

		DECLARE team_cursor CURSOR FOR
		select distinct p.TEAM_ID, COUNT(p.STUDENT_ID) from PLAYERS p, TEAMS t, CHAMPIONSHIPS c
		where p.TEAM_ID=t.TEAM_ID and t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID 
		and c.SEASON=@season
		and t.REGISTRATION_DATE>@team_register_date
		and t.DATE_DELETED IS NULL 
		and p.STUDENT_ID in (select student_id from @students) 
		and p.TEAM_ID<>@team
		group by p.TEAM_ID, t.REGISTRATION_DATE
		
		OPEN team_cursor
		FETCH NEXT FROM team_cursor
		INTO @curteam, @curstudent_count
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (@curstudent_count=@student_count)
			BEGIN
				IF NOT EXISTS(SELECT PLAYER_ID FROM PLAYERS WHERE TEAM_ID=@curteam AND STUDENT_ID=@student_id) BEGIN
					--PRINT CONVERT(nvarchar(50), @curteam)
					INSERT INTO PLAYERS (STUDENT_ID, TEAM_ID, TEAM_NUMBER, STATUS, REGISTRATION_DATE)
					(SELECT @student_id, @curteam, TEAM_NUMBER, STATUS, GETDATE() FROM PLAYERS WHERE PLAYER_ID=@player)
				END
			END
			
			FETCH NEXT FROM team_cursor
			INTO @curteam, @curstudent_count
		END
			
		CLOSE team_cursor
		DEALLOCATE team_cursor
	END
	
	--SET @player=43698
	
	--deleted
	DECLARE team_cursor_2 CURSOR FOR
	SELECT del.TEAM_ID FROM deleted del, inserted ins
	WHERE del.PLAYER_ID=ins.PLAYER_ID AND del.DATE_DELETED IS NULL
	AND ins.DATE_DELETED IS NOT NULL 
	
	OPEN team_cursor_2	
	FETCH NEXT FROM team_cursor_2
	INTO @team
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE TEAMS SET PLAYERS_COUNT=PLAYERS_COUNT-1
		WHERE TEAM_ID=@team
		
		--advance to next record:
		FETCH NEXT FROM team_cursor_2
		INTO @team
	END
	CLOSE team_cursor_2
	DEALLOCATE team_cursor_2
	
	--undelete
	DECLARE team_cursor_3 CURSOR FOR
	SELECT del.TEAM_ID FROM deleted del, inserted ins
	WHERE del.PLAYER_ID=ins.PLAYER_ID AND del.DATE_DELETED IS NOT NULL
	AND ins.DATE_DELETED IS NULL
	
	OPEN team_cursor_3	
	FETCH NEXT FROM team_cursor_3
	INTO @team
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE TEAMS SET PLAYERS_COUNT=PLAYERS_COUNT+1
		WHERE TEAM_ID=@team
		
		--advance to next record:
		FETCH NEXT FROM team_cursor_3
		INTO @team
	END
	CLOSE team_cursor_3
	DEALLOCATE team_cursor_3	
	
	IF 	(SELECT COUNT(*) FROM deleted del, inserted ins
		 WHERE del.TEAM_ID=ins.TEAM_ID AND del.CHARGE_ID<>ins.CHARGE_ID)>0
	BEGIN
		RETURN
	END
	
	DECLARE message_cursor CURSOR FOR
	SELECT DISTINCT ins.STATUS, ins.PLAYER_ID,
	t.TEAM_SUPERVISOR, s.SCHOOL_NAME, ins.REMARKS
	FROM deleted del, inserted ins, STUDENTS st, TEAMS t, SCHOOLS s
	WHERE del.PLAYER_ID=ins.PLAYER_ID AND ins.STUDENT_ID=st.STUDENT_ID
	AND ins.TEAM_ID=t.TEAM_ID AND t.SCHOOL_ID=s.SCHOOL_ID 
	AND del.STATUS<>ins.STATUS AND t.TEAM_SUPERVISOR IS NOT NULL
	
	OPEN message_cursor
	
	FETCH NEXT FROM message_cursor
	INTO @status, @player, @user, @school, @remarks
	
	--loop over the players, check status of each player:
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check current status:
		IF @status=3
		BEGIN
			--status 3 means player was rejected.
			--send message to the supervisor:
			INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
					TIME_SENT)
			VALUES(@user, 6, 1, 'השחקן {?player='+CAST(@player AS varchar(10))+'} מבית הספר '+@school+' לא אושר על ידי ההתאחדות. סיבה: '+@remarks, GETDATE())
		END --end if player status is Not Confirmed
		
		--advance to next record:
		FETCH NEXT FROM message_cursor
		INTO @status, @player, @user, @school, @remarks
	END
	
	CLOSE message_cursor
	DEALLOCATE message_cursor
	
	-- =======    delete charge if the player has been marked as deleted   ======	
	--
	--DECLARE @charge int	
	--DECLARE @price float
	--DECLARE @sport int --type of sport: 1 Competition 2 Match
	--DECLARE @region int
	--DECLARE @city int
	--
	--DECLARE delete_charge_cursor CURSOR FOR
	--SELECT ins.CHARGE_ID
	--FROM deleted del, inserted ins
	--WHERE (del.PLAYER_ID=ins.PLAYER_ID) AND (del.DATE_DELETED IS NULL)
	--AND (ins.DATE_DELETED IS NOT NULL)
	--
	--OPEN delete_charge_cursor
	--FETCH NEXT FROM delete_charge_cursor
	--INTO @charge
	--
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
		--charge only if the championship is of type match and price is positive:
	--	UPDATE CHARGES SET DATE_DELETED=GETDATE()
	--	WHERE CHARGE_ID=@charge
	--	
	--	FETCH NEXT FROM delete_charge_cursor
	--	INTO @charge
	--END
	--
	--CLOSE delete_charge_cursor
	--DEALLOCATE delete_charge_cursor	
	--
	-- =======    add charge for the school, only if player is confirmed   ======	
	--DECLARE charge_cursor_2 CURSOR FOR
	--SELECT s.SCHOOL_ID, ins.PLAYER_ID, sp.SPORT_TYPE, cc.REGISTRATION_PRICE, sc.REGION_ID, sc.CITY_ID
	--FROM inserted ins, deleted del, STUDENTS s, SCHOOLS sc, TEAMS t, 
	--	CHAMPIONSHIP_CATEGORIES cc, CHAMPIONSHIPS c, SPORTS sp
	--WHERE ins.PLAYER_ID=del.PLAYER_ID AND ins.STUDENT_ID=s.STUDENT_ID
	--AND ins.TEAM_ID=t.TEAM_ID AND s.SCHOOL_ID=sc.SCHOOL_ID
	--AND del.STATUS<>ins.STATUS AND t.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID
	--AND cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND c.SPORT_ID=sp.SPORT_ID	
	--AND ins.STATUS=2
	--
	--OPEN charge_cursor_2
	--
	--FETCH NEXT FROM charge_cursor_2
	--INTO @school, @player, @sport, @price, @region, @city
	--
	--WHILE @@FETCH_STATUS = 0
	--BEGIN
		--charge only if the championship is of type competition and price is positive:
	--	IF @sport=1 AND @price>0
	--	BEGIN
	--		DECLARE @newcharge int
	--		EXEC SP_INSERT_SCHOOL_CHARGE @school, 2, @price, @player, @newcharge OUTPUT
	--		
	--		UPDATE PLAYERS
	--		SET CHARGE_ID = @newcharge
	--		WHERE PLAYER_ID = @player
	--	END
	--
	--	FETCH NEXT FROM charge_cursor_2
	--	INTO @school, @player, @sport, @price, @region, @city
	--END
	--
	--CLOSE charge_cursor_2
	--DEALLOCATE charge_cursor_2
END
GO
