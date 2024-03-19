 -- ================================================================================
--Practice Camp Participant Updated - 
--Adding charge if the participant has been confirmed.
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_PRACTICE_CAMP_PARTICIPANT_UPDATED' AND type = 'TR')
	DROP TRIGGER T_PRACTICE_CAMP_PARTICIPANT_UPDATED
GO
--IF EXISTS(SELECT * FROM sysobjects WHERE name = 'TEMP_CHARGE_PRACTICE_CAMP_PARTICIPANT' AND type = 'U')
--	DROP TABLE TEMP_CHARGE_PRACTICE_CAMP_PARTICIPANT
--GO

--CREATE TABLE TEMP_CHARGE_PRACTICE_CAMP_PARTICIPANT (
--	CHARGE_PARTICIPANT_ID	int IDENTITY (1,1), 
--	CHARGE_ID				int NOT NULL, 
--	PARTICIPANT_ID			int NOT NULL
--)
--GO

-- =======    TEAM_UPDATED TRIGGER   ======
CREATE TRIGGER T_PRACTICE_CAMP_PARTICIPANT_UPDATED
ON PRACTICE_CAMP_PARTICIPANTS
AFTER UPDATE AS
BEGIN
	DECLARE @charge int	
	
	IF 	(SELECT COUNT(*) FROM deleted del, inserted ins
		 WHERE del.PARTICIPANT_ID=ins.PARTICIPANT_ID AND del.CHARGE_ID<>ins.CHARGE_ID)>0
	BEGIN
		RETURN
	END
	
	-- =======    delete charge if the team has been marked as deleted   ======	
	DECLARE delete_charge_cursor CURSOR FOR
	SELECT ins.CHARGE_ID
	FROM deleted del, inserted ins
	WHERE (del.PARTICIPANT_ID=ins.PARTICIPANT_ID) AND (del.DATE_DELETED IS NULL)
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
	
	DECLARE @participant int
	DECLARE @name nvarchar(255)
	DECLARE @address nvarchar(255)
	DECLARE @price int
	DECLARE @account int
	
	-- =======    add charge for the school, only if team is confirmed   ======	
	DECLARE charge_cursor_2 CURSOR FOR
	SELECT ins.PARTICIPANT_ID, ins.PARTICIPANT_NAME, ins.PARTICIPANT_ADDRESS, pc.BASE_PRICE
	FROM deleted del, inserted ins, PRACTICE_CAMPS pc
	WHERE del.PARTICIPANT_ID=ins.PARTICIPANT_ID AND del.IS_CONFIRMED<>ins.IS_CONFIRMED 
	AND ins.PRACTICE_CAMP_ID=pc.PRACTICE_CAMP_ID AND ins.IS_CONFIRMED=1
	
	OPEN charge_cursor_2
	FETCH NEXT FROM charge_cursor_2
	INTO @participant, @name, @address, @price
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--charge only if price is positive:
		IF @price>0
		BEGIN
			SET @name = @name + ' (משתתף מחנה אימון)'
		
			SELECT @account = ACCOUNT_ID
			FROM ACCOUNTS
			WHERE ACCOUNT_NAME = @name AND DATE_DELETED IS NULL
			
			IF (@account IS NULL)
			BEGIN
				INSERT INTO ACCOUNTS(REGION_ID, ACCOUNT_NAME, ADDRESS)
				VALUES (0, @name, @address)
				SELECT @account = IDENT_CURRENT('ACCOUNTS')
			END
			
			INSERT INTO CHARGES(REGION_ID, ACCOUNT_ID, PRODUCT_ID, AMOUNT, PRICE, CHARGE_DATE, STATUS, ADDITIONAL)
			VALUES (0, @account, 4, 1, @price, GETDATE(), 1, @participant)
			SELECT @charge = IDENT_CURRENT('CHARGES')
			
			UPDATE PRACTICE_CAMP_PARTICIPANTS
			SET CHARGE_ID = @charge
			WHERE PARTICIPANT_ID = @participant
		END
		
		FETCH NEXT FROM charge_cursor_2
		INTO @participant, @name, @address, @price
	END
	
	CLOSE charge_cursor_2
	DEALLOCATE charge_cursor_2
	
	CLOSE delete_charge_cursor
	DEALLOCATE delete_charge_cursor
END
GO
 