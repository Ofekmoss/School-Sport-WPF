-- ================================================================================
--School Updated - 
--Add club charge if the club status was changed to true,
-- remove club charge if club status was changed to false.
-- ================================================================================ 

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_SCHOOL_UPDATED' AND type = 'TR')
	DROP TRIGGER T_SCHOOL_UPDATED
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