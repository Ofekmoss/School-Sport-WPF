-- ================================================================================
--Payment Deleted - 
--Update the status of payment's charge to not paid if sum of remaining payments is
--less than the price of the charge.
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_PAYMENT_DELETED' AND type = 'TR')
	DROP TRIGGER T_PAYMENT_DELETED
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

---DELETE * FROM PAYMENTS
---WHERE PAYMENT_ID=5
---GO 