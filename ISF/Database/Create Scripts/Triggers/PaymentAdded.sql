-- ================================================================================
--Payment Added - 
--Update the status of payment's charge to paid if sum of payments exceeds
--the charge price.
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_PAYMENT_ADDED' AND type = 'TR')
	DROP TRIGGER T_PAYMENT_ADDED
GO

-- =======    PAYMENT_ADDED TRIGGER   ======
CREATE TRIGGER T_PAYMENT_ADDED
ON PAYMENTS
AFTER INSERT AS
BEGIN
	DECLARE @charge int
	DECLARE @price float
	DECLARE @payments float
	DECLARE @payment int
	DECLARE @region int
	DECLARE @city int
	
	-- =======    update region and city of payment according to school   ======
	DECLARE payment_cursor CURSOR FOR
	SELECT ins.PAYMENT_ID, s.REGION_ID, s.CITY_ID
	FROM inserted ins, SCHOOLS s
	WHERE ins.SCHOOL_ID=s.SCHOOL_ID
	AND ins.SCHOOL_ID IS NOT NULL
	
	-- =======    loop over payments   ======	
	OPEN payment_cursor
	
	FETCH NEXT FROM payment_cursor
	INTO @payment, @region, @city
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--update current payment with region and city.
		UPDATE PAYMENTS SET REGION_ID=@region, CITY_ID=@city
		WHERE PAYMENT_ID=@payment
		
		--advance to next record:
		FETCH NEXT FROM payment_cursor
		INTO @payment, @region, @city
	END
	
	CLOSE payment_cursor
	DEALLOCATE payment_cursor
	
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

---INSERT INTO PAYMENTS (SCHOOL_ID, CHARGE_ID, PAYMENT_SUM)
---VALUES (1890, 45, 100)
---GO