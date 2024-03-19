-- ================================================================================
--Championship Updated - 
--Send proper messages to users if the championship status
-- has been changed.
-- ================================================================================  

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_CHAMPIONSHIP_UPDATED' AND type = 'TR')
	DROP TRIGGER T_CHAMPIONSHIP_UPDATED
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