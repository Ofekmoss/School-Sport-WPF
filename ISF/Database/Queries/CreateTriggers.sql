-- =======    CHAMPIONSHIP_ADDED TRIGGER   ======
CREATE TRIGGER T_CHAMPIONSHIP_ADDED
ON CHAMPIONSHIPS
AFTER INSERT AS
BEGIN
	-- Inserting regions (0 is central region)
	INSERT INTO CHAMPIONSHIP_REGIONS(CHAMPIONSHIP_ID, REGION_ID)
	SELECT I.CHAMPIONSHIP_ID, R.REGION_ID
	FROM inserted I, REGIONS R
	WHERE (I.REGION_ID = 0 AND R.REGION_ID <> 0) OR
		(I.REGION_ID <> 0 AND R.REGION_ID = I.REGION_ID)

	-- Inserting categories for standard championships
	INSERT INTO CHAMPIONSHIP_CATEGORIES(CHAMPIONSHIP_ID, CATEGORY)
	SELECT I.CHAMPIONSHIP_ID, S.CATEGORY
	FROM inserted I, STANDARD_CHAMPIONSHIP_CATEGORIES S
	WHERE I.STANDARD_CHAMPIONSHIP_ID = S.STANDARD_CHAMPIONSHIP_ID
END
GO

-- =======    CHAMPIONSHIP_STATUS_CHANGED TRIGGER   ======
CREATE TRIGGER T_CHAMPIONSHIP_UPDATED
ON CHAMPIONSHIPS
AFTER UPDATE AS
BEGIN
	DECLARE message_cursor CURSOR FOR
	SELECT DISTINCT ins.CHAMPIONSHIP_ID, ins.CHAMPIONSHIP_STATUS, ins.REGION_ID,
	ins.CHAMPIONSHIP_NAME, ins.IS_OPEN, ins.IS_CLUBS
	FROM deleted del, inserted ins
	WHERE del.CHAMPIONSHIP_ID=ins.CHAMPIONSHIP_ID
	AND del.CHAMPIONSHIP_STATUS<>ins.CHAMPIONSHIP_STATUS
	
	DECLARE @championship int
	DECLARE @status int
	DECLARE @region int
	DECLARE @name nvarchar(50)
	DECLARE @is_open int
	DECLARE @is_club char(1)
	DECLARE @user int
	
	OPEN message_cursor
	FETCH NEXT FROM message_cursor
	INTO @championship, @status, @region, @name, @is_open, @is_club
	
	--loop over the championships, check each championship status:
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check championship status:
		IF @status=1
		BEGIN
			--status 1 means team register status.
			--need to decide now the users to whom the message will be sent.
			--check the region:
			IF @region=0
			BEGIN
				--central region championship - check if open championship or closed:
				IF @is_open=0
				BEGIN
					--championship is not open and central - send messages to regional users
					--which are defined as internal users
					DECLARE select_users_cursor CURSOR FOR
					SELECT DISTINCT USER_ID FROM USERS
					WHERE REGION_ID>0 AND USER_TYPE=1
				END --end if championship is not open.
				ELSE
				BEGIN
					--open championship - send the message to all regional users with school.
					--check if the championship is defined for clubs only:
					IF @is_club='1'
					BEGIN
						--clubs only, select proper users according to their school:
						DECLARE select_users_cursor CURSOR FOR
						SELECT DISTINCT u.USER_ID FROM USERS u, SCHOOLS s
						WHERE u.SCHOOL_ID=s.SCHOOL_ID AND s.CLUB_STATUS=1
						AND u.REGION_ID>0 AND u.SCHOOL_ID IS NOT NULL AND u.USER_TYPE=2
					END --end if clubs only championship
					ELSE
					BEGIN
						--not for clubs only, select all regional users:
						DECLARE select_users_cursor CURSOR FOR
						SELECT DISTINCT USER_ID FROM USERS
						WHERE REGION_ID>0 AND SCHOOL_ID IS NOT NULL AND USER_TYPE=2
					END --end if championship is not for clubs only.
				END --end if championship is open.
			END --end if central region.
			ELSE
			BEGIN
				--regional championship - send only to proper user of same region.
				--check if the championship is defined for clubs only:
				IF @is_club='1'
				BEGIN
					--clubs only, select proper users according to their school:
					DECLARE select_users_cursor CURSOR FOR
					SELECT DISTINCT u.USER_ID FROM USERS u, SCHOOLS s
					WHERE u.SCHOOL_ID=s.SCHOOL_ID AND s.CLUB_STATUS=1
					AND u.REGION_ID=@region AND u.SCHOOL_ID IS NOT NULL AND u.USER_TYPE=2
				END --end if championship is only for clubs
				ELSE
				BEGIN
					--not for clubs only, select all regional users:
					DECLARE select_users_cursor CURSOR FOR
					SELECT DISTINCT USER_ID FROM USERS
					WHERE REGION_ID=@region AND SCHOOL_ID IS NOT NULL AND USER_TYPE=2
				END --end if championship is not for clubs only.
			END --end if not central region championship.
			
			OPEN select_users_cursor
			
			FETCH NEXT FROM select_users_cursor
			INTO @user
			
			--loop over the users and create message for each of them:
			WHILE @@FETCH_STATUS = 0
			BEGIN
				INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
					TIME_SENT)
				VALUES(@user, 1, 1, 'האליפות "{?championship='+CAST(@championship AS varchar(10))+'}" נפתחה לרישום קבוצות', GETDATE())
				
				FETCH NEXT FROM select_users_cursor
				INTO @user
			END
			
			CLOSE select_users_cursor
			DEALLOCATE select_users_cursor
		END --end if championship status is team register
		
		IF @status=2
		BEGIN
			--status 2 means players register status.
			--need to decide now the users to whom the message will be sent.
			--select proper supervisors:
			DECLARE select_users_cursor CURSOR FOR
			SELECT DISTINCT u.USER_ID FROM USERS u, TEAMS t
			WHERE u.USER_ID=t.TEAM_SUPERVISOR AND t.CHAMPIONSHIP_ID=@championship
			
			OPEN select_users_cursor
			
			FETCH NEXT FROM select_users_cursor
			INTO @user
			
			--loop over the users and create message for each of them:
			WHILE @@FETCH_STATUS = 0
			BEGIN
				INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
					TIME_SENT)
				VALUES(@user, 2, 1, 'האליפות "{?championship='+CAST(@championship AS varchar(10))+'}" נפתחה לרישום שחקנים', GETDATE())
				
				FETCH NEXT FROM select_users_cursor
				INTO @user
			END
			
			CLOSE select_users_cursor
			DEALLOCATE select_users_cursor
		END --end if championship status is players register
		
		--advance to next record:
		FETCH NEXT FROM message_cursor
		INTO @championship, @status, @region, @name, @is_open, @is_club
	END
	
	CLOSE message_cursor
	DEALLOCATE message_cursor
END
GO

-- =======    TEAM_UNCHARGE TRIGGER   ======
CREATE TRIGGER T_TEAM_UNCHARGE
ON TEAMS
AFTER DELETE AS
BEGIN
  DELETE CHARGES
  WHERE CHARGE_ID IN (SELECT CHARGE_ID FROM deleted)
END
GO

-- =======    PLAYER_UNCHARGE TRIGGER   ====== 
CREATE TRIGGER T_PLAYER_UNCHARGE
ON PLAYERS
AFTER DELETE AS
BEGIN
  DELETE CHARGES
  WHERE CHARGE_ID IN (SELECT CHARGE_ID FROM deleted)
END
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
	DECLARE @product int
	DECLARE @price float
	
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
	
	-- =======    add charge for the school   ======	
	DECLARE charge_cursor CURSOR FOR
	SELECT s.SCHOOL_ID, p.PLAYER_ID
	FROM inserted p, STUDENTS s 
	WHERE p.STUDENT_ID=s.STUDENT_ID
	
	DECLARE product_cursor CURSOR FOR
	SELECT DISTINCT PRODUCT_ID, PRICE
	FROM PRODUCTS WHERE PRODUCT_ID=2
	
	--get price for the club charge  	
	OPEN product_cursor
	FETCH NEXT FROM product_cursor
	INTO @product, @price
	CLOSE product_cursor
	DEALLOCATE product_cursor
	
	OPEN charge_cursor
		
	FETCH NEXT FROM charge_cursor
	INTO @school, @player
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO CHARGES(SCHOOL_ID, PRODUCT_ID, STATUS, CHARGE_PRICE)
		VALUES(@school, @product, 1, @price)
		
		UPDATE PLAYERS
		SET CHARGE_ID = IDENT_CURRENT('CHARGES')
		WHERE PLAYER_ID = @player
		
		FETCH NEXT FROM charge_cursor
		INTO @school, @player
	END
	
	CLOSE charge_cursor
	DEALLOCATE charge_cursor	
END
GO

-- =======    PLAYER_STATUS_CHANGED TRIGGER   ======
CREATE TRIGGER T_PLAYER_UPDATED
ON PLAYERS
AFTER UPDATE AS
BEGIN
	DECLARE message_cursor CURSOR FOR
	SELECT DISTINCT ins.STATUS, ins.PLAYER_ID,
	t.TEAM_SUPERVISOR, s.SCHOOL_NAME, ins.REMARKS
	FROM deleted del, inserted ins, STUDENTS st, TEAMS t, SCHOOLS s
	WHERE del.PLAYER_ID=ins.PLAYER_ID AND ins.STUDENT_ID=st.STUDENT_ID
	AND ins.TEAM_ID=t.TEAM_ID AND t.SCHOOL_ID=s.SCHOOL_ID 
	AND del.STATUS<>ins.STATUS AND t.TEAM_SUPERVISOR IS NOT NULL
	
	DECLARE @status int
	DECLARE @player int --nvarchar(100)
	DECLARE @user int
	DECLARE @school nvarchar(50)
	DECLARE @remarks nvarchar(100)
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
END
GO

-- =======    SCHOOL_UPDATED TRIGGER   ======
CREATE TRIGGER T_SCHOOL_UPDATED
ON SCHOOLS
AFTER UPDATE AS
BEGIN
	-- =======    check if club status has been changed   ======
	DECLARE update_cursor CURSOR FOR
	SELECT DISTINCT ins.SCHOOL_ID, ins.CLUB_STATUS, ins.CLUB_CHARGE_ID 
	FROM deleted del, inserted ins
	WHERE del.SCHOOL_ID=ins.SCHOOL_ID
	AND del.CLUB_STATUS<>ins.CLUB_STATUS
	
	DECLARE product_cursor CURSOR FOR
	SELECT DISTINCT PRODUCT_ID, PRICE
	FROM PRODUCTS WHERE PRODUCT_ID=3
	
	DECLARE @school int
	DECLARE @club int  
	DECLARE @charge int
	DECLARE @product int
	DECLARE @price float
	
	--get price for the club charge
	OPEN product_cursor
	FETCH NEXT FROM product_cursor
	INTO @product, @price
	CLOSE product_cursor
	DEALLOCATE product_cursor
	
	OPEN update_cursor
	FETCH NEXT FROM update_cursor
	INTO @school, @club, @charge
	
	--iterate over the schools, add or delete the club charge
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @club=1
		BEGIN
			INSERT INTO CHARGES(SCHOOL_ID, PRODUCT_ID, STATUS, CHARGE_PRICE)
			VALUES(@school, @product, 1, @price)

			UPDATE SCHOOLS
			SET CLUB_CHARGE_ID = IDENT_CURRENT('CHARGES')
			WHERE SCHOOL_ID=@school
		END
		ELSE
		BEGIN
			UPDATE SCHOOLS
			SET CLUB_CHARGE_ID = NULL
			WHERE SCHOOL_ID=@school       
			
			--PRINT @charge
			DELETE FROM CHARGES WHERE CHARGE_ID=@charge
		END
		
		FETCH NEXT FROM update_cursor
		INTO @school, @club, @charge
	END
	
	CLOSE update_cursor
	DEALLOCATE update_cursor
	
	-- =======    find the schools that were deleted   ======
	DECLARE charge_cursor CURSOR FOR
	SELECT DISTINCT ins.CLUB_CHARGE_ID, ins.DATE_DELETED
	FROM deleted del, inserted ins
	WHERE del.SCHOOL_ID=ins.SCHOOL_ID
	AND del.DATE_DELETED<>ins.DATE_DELETED
	
	DECLARE @date datetime
	
	OPEN charge_cursor
	FETCH NEXT FROM charge_cursor
	INTO @charge, @date
	
	--iterate over the schools, remove charges for deleted schools
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @date<>NULL
		BEGIN
			DELETE FROM CHARGES WHERE CHARGE_ID=@charge
		END
		
		FETCH NEXT FROM charge_cursor
		INTO @charge, @date
	END
	
	CLOSE charge_cursor
	DEALLOCATE charge_cursor	
END
GO

-- =======    TEAM_ADDED TRIGGER   ======
CREATE TRIGGER T_TEAM_ADDED
ON TEAMS
AFTER INSERT AS
BEGIN
	DECLARE @user int
	DECLARE @school int
	DECLARE @team int
	DECLARE @product int
	DECLARE @price float
	
	-- =======    send messages - for teams having supervisor   ======
	DECLARE message_cursor CURSOR FOR
	SELECT DISTINCT c.CHAMPIONSHIP_SUPERVISOR, s.SCHOOL_ID
	FROM inserted ins, CHAMPIONSHIPS c, SCHOOLS s
	WHERE ins.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND ins.SCHOOL_ID=s.SCHOOL_ID
	AND ins.TEAM_SUPERVISOR<>c.CHAMPIONSHIP_SUPERVISOR
	AND c.CHAMPIONSHIP_SUPERVISOR IS NOT NULL
	
	OPEN message_cursor
	
	FETCH NEXT FROM message_cursor
	INTO @user, @school
	
	--loop over the supervisors and send messages:
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
			TIME_SENT)
			VALUES(@user, 3, 1, 'בית הספר "{?school='+CAST(@school AS varchar(10))+'}" שלח בקשה לרישום קבוצה', GETDATE())
		
		--advance to next record:
		FETCH NEXT FROM message_cursor
		INTO @user, @school
	END
	
	CLOSE message_cursor
	DEALLOCATE message_cursor
	
	-- =======    add charge for the school   ======	
	DECLARE charge_cursor CURSOR FOR
	SELECT TEAM_ID, SCHOOL_ID
	FROM inserted
	
	DECLARE product_cursor CURSOR FOR
	SELECT DISTINCT PRODUCT_ID, PRICE
	FROM PRODUCTS WHERE PRODUCT_ID=1
	
	--get price for the club charge  
	OPEN product_cursor
	FETCH NEXT FROM product_cursor
	INTO @product, @price
	CLOSE product_cursor
	DEALLOCATE product_cursor
	
	OPEN charge_cursor
	FETCH NEXT FROM charge_cursor
	INTO @team, @school
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT INTO CHARGES(SCHOOL_ID, PRODUCT_ID, STATUS, CHARGE_PRICE)
		VALUES(@school, @product, 1, @price)
		
		UPDATE TEAMS
		SET CHARGE_ID = IDENT_CURRENT('CHARGES')
		WHERE TEAM_ID = @team
		
		FETCH NEXT FROM charge_cursor
		INTO @team, @school
	END
	
	CLOSE charge_cursor
	DEALLOCATE charge_cursor	
END
GO

-- =======    TEAM_UPDATED TRIGGER   ======
CREATE TRIGGER T_TEAM_UPDATED
ON TEAMS
AFTER UPDATE AS
BEGIN
	DECLARE @status int
	DECLARE @user int
	DECLARE @school int --nvarchar(50)
	
	DECLARE message_cursor CURSOR FOR
	SELECT DISTINCT ins.STATUS, ins.TEAM_SUPERVISOR, s.SCHOOL_ID
	FROM deleted del, inserted ins, SCHOOLS s
	WHERE del.TEAM_ID=ins.TEAM_ID AND ins.SCHOOL_ID=s.SCHOOL_ID
	AND del.STATUS<>ins.STATUS AND ins.TEAM_SUPERVISOR IS NOT NULL
	
	OPEN message_cursor
	FETCH NEXT FROM message_cursor
	INTO @status, @user, @school
	
	--loop over the teams, check status of each team:
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check current status:
		IF @status=2
		BEGIN
			--status 2 means team is confirmed.
			--send message to the supervisor:
			INSERT INTO MESSAGES(USER_ID, MESSAGE_TYPE, MESSAGE_STATUS, MESSAGE_TEXT, 
				TIME_SENT)
			VALUES(@user, 5, 1, 'קבוצה עבור בית הספר "{?school='+CAST(@school AS varchar(10))+'}" אושרה על ידי ההתאחדות', GETDATE())
		END --end if team status is confirmed team.

		--advance to next record:
		FETCH NEXT FROM message_cursor
		INTO @status, @user, @school
	END
	
	CLOSE message_cursor
	DEALLOCATE message_cursor
END
GO

-- =======    PAYMENT_ADDED TRIGGER   ======
CREATE TRIGGER T_PAYMENT_ADDED
ON PAYMENTS
AFTER INSERT AS
BEGIN
	DECLARE @charge int
	DECLARE @price float
	DECLARE @payments float

	-- =======    get all charges   ======
	DECLARE payment_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, CHARGES c
	WHERE ins.CHARGE_ID=p.CHARGE_ID	AND ins.CHARGE_ID=c.CHARGE_ID 
	AND ins.CHARGE_ID IS NOT NULL AND ins.PAYMENT_SUM>0
	AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @charge, @price, @payments
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--PRINT @charge
		--PRINT @price
		--PRINT @payments
		
		--check if total payments is equal or bigger than the charge price:
		IF @payments>=@price
		BEGIN
			--change charge status to paid:
			UPDATE CHARGES SET STATUS=2
			WHERE CHARGE_ID=@charge
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor
END
GO

-- =======    PAYMENT_UPDATED TRIGGER   ======
CREATE TRIGGER T_PAYMENT_UPDATED
ON PAYMENTS
AFTER UPDATE AS
BEGIN
	DECLARE @charge int
	DECLARE @price float
	DECLARE @payments float
	
	-- =======    get all charges for payments whose sum has been changed  ======
	DECLARE payment_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.PAYMENT_SUM<>del.PAYMENT_SUM 
	AND ins.CHARGE_ID=p.CHARGE_ID AND ins.CHARGE_ID=c.CHARGE_ID 
	AND ins.CHARGE_ID IS NOT NULL AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @charge, @price, @payments
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--PRINT @charge
		--PRINT @price
		--PRINT @payments
		
		--check if total payments is equal or bigger than the charge price:
		IF @payments>=@price
		BEGIN
			--change charge status to paid:
			UPDATE CHARGES SET STATUS=2
			WHERE CHARGE_ID=@charge
		END
		ELSE
		BEGIN
			--change charge status to not paid:
			UPDATE CHARGES SET STATUS=1
			WHERE CHARGE_ID=@charge		
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor
	
	-- =======    get deleted charges  ======
	DECLARE payment_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.CHARGE_ID<>del.CHARGE_ID
	AND del.CHARGE_ID=p.CHARGE_ID AND del.CHARGE_ID=c.CHARGE_ID 
	AND del.CHARGE_ID IS NOT NULL AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @charge, @price, @payments
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check if total payments is less than the charge price:
		IF @payments<@price
		BEGIN
			--change charge status to not paid:
			UPDATE CHARGES SET STATUS=1
			WHERE CHARGE_ID=@charge
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor	
	
	-- =======    get inserted charges  ======
	DECLARE payment_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.CHARGE_ID<>del.CHARGE_ID
	AND ins.CHARGE_ID=p.CHARGE_ID AND ins.CHARGE_ID=c.CHARGE_ID 
	AND ins.CHARGE_ID IS NOT NULL AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @charge, @price, @payments
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check if total payments is equal or bigger than the charge price:
		IF @payments>=@price
		BEGIN
			--change charge status to paid:
			UPDATE CHARGES SET STATUS=2
			WHERE CHARGE_ID=@charge
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor		

	-- =======    get all charges for deleted payments ======
	DECLARE payment_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.DATE_DELETED IS NOT NULL
	AND del.DATE_DELETED IS NULL AND ins.CHARGE_ID=p.CHARGE_ID 
	AND ins.CHARGE_ID=c.CHARGE_ID AND ins.CHARGE_ID IS NOT NULL 
	AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @charge, @price, @payments
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check if total payments is less than the charge price:
		IF @payments<@price
		BEGIN
			--change charge status to not paid:
			UPDATE CHARGES SET STATUS=1
			WHERE CHARGE_ID=@charge
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor
	
	-- =======    get all charges for un-deleted payments ======
	DECLARE payment_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.DATE_DELETED IS NULL
	AND del.DATE_DELETED IS NOT NULL AND ins.CHARGE_ID=p.CHARGE_ID 
	AND ins.CHARGE_ID=c.CHARGE_ID AND ins.CHARGE_ID IS NOT NULL 
	AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @charge, @price, @payments
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check if total payments is equal or bigger than the charge price:
		IF @payments>=@price
		BEGIN
			--change charge status to paid:
			UPDATE CHARGES SET STATUS=2
			WHERE CHARGE_ID=@charge
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor
END
GO

-- =======    PAYMENT_DELETED TRIGGER   ======
CREATE TRIGGER T_PAYMENT_DELETED
ON PAYMENTS
AFTER DELETE AS
BEGIN
	DECLARE @charge int
	DECLARE @price float
	DECLARE @payments float
	
	DECLARE @counter int

	-- =======    get deleted charges  ======
	DECLARE payment_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, deleted del, CHARGES c
	WHERE del.CHARGE_ID=p.CHARGE_ID AND del.CHARGE_ID=c.CHARGE_ID 
	AND del.CHARGE_ID IS NOT NULL AND del.PAYMENT_SUM>0
	AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @charge, @price, @payments
	
	SET @counter=0
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--check if total payments is less than the charge price:
		IF @payments<@price
		BEGIN
			--change charge status to not paid:
			UPDATE CHARGES SET STATUS=1
			WHERE CHARGE_ID=@charge
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @charge, @price, @payments
		
		SET @counter=@counter+1
	END
	
	IF @counter=0
	BEGIN
		--change charge status to not paid:
		UPDATE CHARGES SET STATUS=1
		WHERE CHARGE_ID IN (SELECT DISTINCT CHARGE_ID FROM deleted)
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor	
END
GO