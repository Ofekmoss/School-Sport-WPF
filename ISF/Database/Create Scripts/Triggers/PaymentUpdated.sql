-- ================================================================================
--Payment Updated - 
--Update the status of payment's charge to paid if sum of payments exceeds
--the charge price or to not paid if sum of payments is less than price.
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_PAYMENT_UPDATED' AND type = 'TR')
	DROP TRIGGER T_PAYMENT_UPDATED
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
	DECLARE payment_update_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.PAYMENT_SUM<>del.PAYMENT_SUM 
	AND ins.CHARGE_ID=p.CHARGE_ID AND ins.CHARGE_ID=c.CHARGE_ID 
	AND ins.CHARGE_ID IS NOT NULL AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_update_cursor
	
	FETCH NEXT FROM payment_update_cursor
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
		FETCH NEXT FROM payment_update_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_update_cursor
	DEALLOCATE payment_update_cursor
	
	-- =======    get deleted charges  ======
	DECLARE payment_update_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.CHARGE_ID<>del.CHARGE_ID
	AND del.CHARGE_ID=p.CHARGE_ID AND del.CHARGE_ID=c.CHARGE_ID 
	AND del.CHARGE_ID IS NOT NULL AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_update_cursor
	
	FETCH NEXT FROM payment_update_cursor
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
		FETCH NEXT FROM payment_update_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_update_cursor
	DEALLOCATE payment_update_cursor	
	
	-- =======    get inserted charges  ======
	DECLARE payment_update_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.CHARGE_ID<>del.CHARGE_ID
	AND ins.CHARGE_ID=p.CHARGE_ID AND ins.CHARGE_ID=c.CHARGE_ID 
	AND ins.CHARGE_ID IS NOT NULL AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_update_cursor
	
	FETCH NEXT FROM payment_update_cursor
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
		FETCH NEXT FROM payment_update_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_update_cursor
	DEALLOCATE payment_update_cursor		

	-- =======    get all charges for deleted payments ======
	DECLARE payment_update_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.DATE_DELETED IS NOT NULL
	AND del.DATE_DELETED IS NULL AND ins.CHARGE_ID=p.CHARGE_ID 
	AND ins.CHARGE_ID=c.CHARGE_ID AND ins.CHARGE_ID IS NOT NULL 
	AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_update_cursor
	
	FETCH NEXT FROM payment_update_cursor
	INTO @charge, @price, @payments
	
	--PRINT 'iterating through the charges...'
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--PRINT 'charge: '+CONVERT(varchar(20), @charge);
		--PRINT 'payments: '+CONVERT(varchar(20), @payments);
		--PRINT 'price: '+CONVERT(varchar(20), @price);
		
		--check if total payments is less than the charge price:
		IF @payments<@price
		BEGIN
			--change charge status to not paid:
			UPDATE CHARGES SET STATUS=1
			WHERE CHARGE_ID=@charge
		END
		
		--advance to next record:
		FETCH NEXT FROM payment_update_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_update_cursor
	DEALLOCATE payment_update_cursor
	
	-- =======    get all charges for un-deleted payments ======
	DECLARE payment_update_cursor CURSOR FOR
	SELECT c.CHARGE_ID, c.CHARGE_PRICE, SUM(p.PAYMENT_SUM)
	FROM PAYMENTS p, inserted ins, deleted del, CHARGES c
	WHERE ins.PAYMENT_ID=del.PAYMENT_ID AND ins.DATE_DELETED IS NULL
	AND del.DATE_DELETED IS NOT NULL AND ins.CHARGE_ID=p.CHARGE_ID 
	AND ins.CHARGE_ID=c.CHARGE_ID AND ins.CHARGE_ID IS NOT NULL 
	AND p.DATE_DELETED IS NULL
	GROUP BY c.CHARGE_ID, c.CHARGE_PRICE
	
	-- =======    loop over charges   ======	
	OPEN payment_update_cursor
	
	FETCH NEXT FROM payment_update_cursor
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
		FETCH NEXT FROM payment_update_cursor
		INTO @charge, @price, @payments
	END
	
	CLOSE payment_update_cursor
	DEALLOCATE payment_update_cursor
END
GO 

---UPDATE PAYMENTS SET PAYMENT_SUM=0
---WHERE PAYMENT_ID=17
---GO 

---UPDATE PAYMENTS SET CHARGE_ID=44
---WHERE PAYMENT_ID=5
---GO 

---UPDATE PAYMENTS SET DATE_DELETED=GETDATE()
---WHERE PAYMENT_ID=18
---GO