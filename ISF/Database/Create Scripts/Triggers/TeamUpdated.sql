 -- ================================================================================
--Team Updated - 
--Sending message to the Supervisor of the team if the team status has been
-- changed to Confirmed.
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_TEAM_UPDATED' AND type = 'TR')
	DROP TRIGGER T_TEAM_UPDATED
GO
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'TEMP_CHARGE_TEAMS' AND type = 'U')
	DROP TABLE TEMP_CHARGE_TEAMS
GO

CREATE TABLE TEMP_CHARGE_TEAMS (
	CHARGE_TEAM_ID int IDENTITY (1,1), 
	CHARGE_ID      int NOT NULL, 
	TEAM_ID        int NOT NULL
)
GO

-- =======    TEAM_UPDATED TRIGGER   ======
CREATE TRIGGER T_TEAM_UPDATED
ON TEAMS
AFTER UPDATE AS
BEGIN
	DECLARE @charge_team int
	DECLARE @charge int	
	DECLARE @team int
	
	IF 	(SELECT COUNT(*) FROM deleted del, inserted ins
		 WHERE del.TEAM_ID=ins.TEAM_ID AND del.CHARGE_ID<>ins.CHARGE_ID)>0
	BEGIN
		RETURN
	END
	
	-- =======    delete charge if the team has been marked as deleted   ======	
	DECLARE delete_charge_cursor CURSOR FOR
	SELECT ins.CHARGE_ID
	FROM deleted del, inserted ins
	WHERE (del.TEAM_ID=ins.TEAM_ID) AND (del.DATE_DELETED IS NULL)
	AND (ins.DATE_DELETED IS NOT NULL)
	
	OPEN delete_charge_cursor
	FETCH NEXT FROM delete_charge_cursor
	INTO @charge
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--charge only if price is positive:
		UPDATE CHARGES SET DATE_DELETED=GETDATE()
		WHERE CHARGE_ID=@charge
		
		FETCH NEXT FROM delete_charge_cursor
		INTO @charge
	END
	
	DECLARE @status int
	DECLARE @user int
	DECLARE @school int --nvarchar(50)
	DECLARE @price float
	DECLARE @sport int --type of sport: 1 Competition 2 Match
	DECLARE @championship int
	DECLARE @region int
	DECLARE @city int
	DECLARE @userschool int
	DECLARE @isClubsChamp int
	DECLARE @champcategory int
	DECLARE @product int
	
	DECLARE message_cursor CURSOR FOR
	SELECT DISTINCT ins.STATUS, ins.TEAM_SUPERVISOR, s.SCHOOL_ID, 
	u.SCHOOL_ID AS USER_SCHOOL, ins.CHAMPIONSHIP_ID
	FROM deleted del, inserted ins, SCHOOLS s, USERS u
	WHERE del.TEAM_ID=ins.TEAM_ID AND ins.SCHOOL_ID=s.SCHOOL_ID
	AND ins.TEAM_SUPERVISOR=u.USER_ID
	AND del.STATUS<>ins.STATUS AND ins.TEAM_SUPERVISOR IS NOT NULL
	
	OPEN message_cursor
	FETCH NEXT FROM message_cursor
	INTO @status, @user, @school, @userschool, @championship
	
	--loop over the teams, check status of each team:
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check current status:
		IF @status=2
		BEGIN
			--status 2 means team is confirmed.
			--send message to the supervisor:
			IF @userschool=@school
			BEGIN
				INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
					TIME_SENT)
				VALUES(@user, 5, 1, 'הקבוצה אשר שלחת עבור האליפות "{?championship='+CAST(@championship AS varchar(10))+'}" אושרה על ידי ההתאחדות', GETDATE())
			END
			ELSE
			BEGIN
				INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
					TIME_SENT)
				VALUES(@user, 5, 1, 'קבוצה עבור בית הספר "{?school='+CAST(@school AS varchar(10))+'}" אושרה על ידי ההתאחדות', GETDATE())			
			END
		END --end if team status is confirmed team.

		--advance to next record:
		FETCH NEXT FROM message_cursor
		INTO @status, @user, @school, @userschool, @championship
	END
	
	CLOSE message_cursor
	DEALLOCATE message_cursor
	
	-- =======    add charge for the school, only if team is confirmed   ======	
	DECLARE charge_cursor_2 CURSOR FOR
	SELECT ins.TEAM_ID, ins.SCHOOL_ID, s.SPORT_TYPE, cc.REGISTRATION_PRICE, c.CHAMPIONSHIP_ID, cc.CHAMPIONSHIP_CATEGORY_ID, c.IS_CLUBS, sc.REGION_ID, sc.CITY_ID
	FROM deleted del, inserted ins, CHAMPIONSHIP_CATEGORIES cc, CHAMPIONSHIPS c, SPORTS s, SCHOOLS sc
	WHERE del.TEAM_ID=ins.TEAM_ID AND del.STATUS<>ins.STATUS 
	AND ins.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID
	AND cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND c.SPORT_ID=s.SPORT_ID
	And ins.SCHOOL_ID=sc.SCHOOL_ID And ins.STATUS=2
	
	OPEN charge_cursor_2
	FETCH NEXT FROM charge_cursor_2
	INTO @team, @school, @sport, @price, @championship, @champcategory, @isClubsChamp, @region, @city
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--charge only if price is positive:
		IF @price>0
		BEGIN
			DECLARE @newcharge int
			SET @product = 1;
			IF @isClubsChamp=1
				SET @product = 5;			
			EXEC SP_INSERT_SCHOOL_CHARGE @school, @product, @price, @champcategory, @newcharge OUTPUT
			
			INSERT INTO TEMP_CHARGE_TEAMS (CHARGE_ID, TEAM_ID)
			VALUES(@newcharge, @team)

			UPDATE TEAMS
			SET CHARGE_ID = @newcharge
			WHERE TEAM_ID = @team
		END
		
		FETCH NEXT FROM charge_cursor_2
		INTO @team, @school, @sport, @price, @championship, @champcategory, @isClubsChamp, @region, @city
	END
	
	CLOSE charge_cursor_2
	DEALLOCATE charge_cursor_2
	
	CLOSE delete_charge_cursor
	DEALLOCATE delete_charge_cursor
END
GO
