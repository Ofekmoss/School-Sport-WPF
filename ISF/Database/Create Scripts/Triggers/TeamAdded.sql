-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_TEAM_ADDED' AND type = 'TR')
	DROP TRIGGER T_TEAM_ADDED
GO

-- =======    TEAM_ADDED TRIGGER   ======
CREATE TRIGGER T_TEAM_ADDED
ON TEAMS
AFTER INSERT AS
BEGIN
	DECLARE @user int
	DECLARE @school int
	DECLARE @price float
	DECLARE @sport int --type of sport: 1 Competition 2 Match
	DECLARE @championship int
	DECLARE @team int	
	DECLARE @region int
	DECLARE @city int
	DECLARE @isClubsChamp int
	DECLARE @schoolClubStatus int
	DECLARE @regionSupervisor int
	DECLARE @champcategory int
	DECLARE @product int
	
	-- =======    send messages - for teams having supervisor   ======
	DECLARE message_cursor CURSOR FOR
	SELECT DISTINCT c.CHAMPIONSHIP_SUPERVISOR, c.IS_CLUBS, s.SCHOOL_ID, 
	s.CLUB_STATUS, r.COORDINATOR, ins.CHAMPIONSHIP_CATEGORY_ID
	FROM inserted ins, CHAMPIONSHIPS c, SCHOOLS s, REGIONS r
	WHERE ins.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND ins.SCHOOL_ID=s.SCHOOL_ID
	AND c.REGION_ID=r.REGION_ID 
	AND (((ins.TEAM_SUPERVISOR<>c.CHAMPIONSHIP_SUPERVISOR) 
	AND (c.CHAMPIONSHIP_SUPERVISOR IS NOT NULL)) 
	OR (r.COORDINATOR IS NOT NULL))
	
	OPEN message_cursor
	
	FETCH NEXT FROM message_cursor
	INTO @user, @isClubsChamp, @school, @schoolClubStatus, @regionSupervisor, @champcategory
	
	--loop over the supervisors and send messages:
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @user IS NOT NULL
		BEGIN
			INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
				TIME_SENT)
				VALUES(@user, 3, 1, 'בית הספר "{?school='+CAST(@school AS varchar(10))+'}" שלח בקשה לרישום קבוצה לאליפות {?championshipcategory='+CAST(@champcategory AS varchar(10))+'}', GETDATE())
		END
		
		--PRINT 'user: '+CAST(@user AS varchar(10))
		--PRINT 'is club champ? '+CAST(@isClubsChamp AS varchar(10))
		--PRINT 'school: '+CAST(@school AS varchar(10))
		--PRINT 'school club status: '+CAST(@schoolClubStatus AS varchar(10))
		--PRINT 'region supervisor: '+CAST(@regionSupervisor AS varchar(10))

		--requested to register new club?
		IF (@isClubsChamp=1) AND (@schoolClubStatus=0) AND (@regionSupervisor IS NOT NULL)
		BEGIN
			IF (@regionSupervisor > 0)
			BEGIN
				INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
					TIME_SENT)
					VALUES(@regionSupervisor, 7, 1, 'בית הספר "{?school='+CAST(@school AS varchar(10))+'}" שלח בקשה לרישום מועדון חדש', GETDATE())
			END
		END
		
		--advance to next record:
		FETCH NEXT FROM message_cursor
		INTO @user, @isClubsChamp, @school, @schoolClubStatus, @regionSupervisor, @champcategory
	END
	
	CLOSE message_cursor
	DEALLOCATE message_cursor
	
	-- =======    add charge for the school, only if team is confirmed   ======	
	DECLARE charge_cursor CURSOR FOR
	SELECT ins.TEAM_ID, ins.SCHOOL_ID, s.SPORT_TYPE, cc.REGISTRATION_PRICE, c.CHAMPIONSHIP_ID, cc.CHAMPIONSHIP_CATEGORY_ID, c.IS_CLUBS, c.REGION_ID, sc.CITY_ID
	FROM inserted ins, CHAMPIONSHIP_CATEGORIES cc, CHAMPIONSHIPS c, SPORTS s, SCHOOLS sc
	WHERE ins.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID
	AND cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND c.SPORT_ID=s.SPORT_ID
	AND ins.SCHOOL_ID=sc.SCHOOL_ID AND ins.STATUS=2
	
	OPEN charge_cursor
	FETCH NEXT FROM charge_cursor
	INTO @team, @school, @sport, @price, @championship, @champcategory, @isClubsChamp, @region, @city
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--charge only if price is positive:
		IF @price>0
		BEGIN
			DECLARE @charge int
			IF @isClubsChamp=1 BEGIN
				SET @product = 5;
			END ELSE BEGIN
				IF @region=0 BEGIN
					SET @product = 1;
				END ELSE BEGIN
					SET @product = 7;
				END
			END
			EXEC SP_INSERT_SCHOOL_CHARGE @school, @product, @price, @champcategory, @charge OUTPUT
			
			UPDATE TEAMS
			SET CHARGE_ID = @charge
			WHERE TEAM_ID = @team
		END
		
		FETCH NEXT FROM charge_cursor
		INTO @team, @school, @sport, @price, @championship, @champcategory, @isClubsChamp, @region, @city
	END
	
	CLOSE charge_cursor
	DEALLOCATE charge_cursor	
END
GO
