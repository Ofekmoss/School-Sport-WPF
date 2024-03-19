ALTER TABLE CHARGES ADD
CHAMPIONSHIP int
GO

------------------------------------------

ALTER TABLE CHARGES ADD
PAYMENT_ID int
GO

------------------------------------------

ALTER TABLE CHARGES ADD CONSTRAINT
FK_CHARGE_PAYMENT FOREIGN KEY(PAYMENT_ID) REFERENCES PAYMENTS(PAYMENT_ID)
GO 

------------------------------------------

ALTER TABLE CHARGES ADD
REGION_ID int
GO

ALTER TABLE CHARGES ADD CONSTRAINT FK_CHARGE_REGION
FOREIGN KEY(REGION_ID) REFERENCES REGIONS(REGION_ID)
GO
------------------------------------------

ALTER TABLE CHARGES ADD
CITY_ID int
GO

ALTER TABLE CHARGES ADD CONSTRAINT FK_CHARGE_CITY
FOREIGN KEY(CITY_ID) REFERENCES CITIES(CITY_ID)
GO

------------------------------------------

DECLARE @charge int
DECLARE @region int
DECLARE @city int

DECLARE charge_cursor CURSOR FOR
SELECT c.CHARGE_ID, s.REGION_ID, s.CITY_ID
FROM CHARGES c INNER JOIN SCHOOLS s ON c.SCHOOL_ID=s.SCHOOL_ID
WHERE (c.SCHOOL_ID IS NOT NULL)
AND (c.DATE_DELETED IS NULL)

OPEN charge_cursor
FETCH NEXT FROM charge_cursor
INTO @charge, @region, @city
	
--loop over the charges, update one at a time:
WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE CHARGES SET REGION_ID=@region, CITY_ID=@city
	WHERE CHARGE_ID=@charge
	
	--advance to next record:
	FETCH NEXT FROM charge_cursor
	INTO @charge, @region, @city
END

CLOSE charge_cursor
DEALLOCATE charge_cursor

------------------------------------------

ALTER TABLE PAYMENTS ADD
REGION_ID int
GO

ALTER TABLE PAYMENTS ADD CONSTRAINT FK_PAYMENT_REGION
FOREIGN KEY(REGION_ID) REFERENCES REGIONS(REGION_ID)
GO
------------------------------------------

ALTER TABLE PAYMENTS ADD
CITY_ID int
GO

ALTER TABLE PAYMENTS ADD CONSTRAINT FK_PAYMENT_CITY
FOREIGN KEY(CITY_ID) REFERENCES CITIES(CITY_ID)
GO

------------------------------------------

DECLARE @payment int
DECLARE @region int
DECLARE @city int

DECLARE payment_cursor_2 CURSOR FOR
SELECT p.PAYMENT_ID, s.REGION_ID, s.CITY_ID
FROM PAYMENTS p INNER JOIN SCHOOLS s ON p.SCHOOL_ID=s.SCHOOL_ID
WHERE (p.SCHOOL_ID IS NOT NULL)
AND (p.DATE_DELETED IS NULL)

OPEN payment_cursor_2
FETCH NEXT FROM payment_cursor_2
INTO @payment, @region, @city
	
--loop over the charges, update one at a time:
WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE PAYMENTS SET REGION_ID=@region, CITY_ID=@city
	WHERE PAYMENT_ID=@payment
	
	--advance to next record:
	FETCH NEXT FROM payment_cursor_2
	INTO @payment, @region, @city
END

CLOSE payment_cursor_2
DEALLOCATE payment_cursor_2

------------------------------------------

ALTER TABLE CHARGES ADD
CHAMPIONSHIP_CATEGORY int
GO

ALTER TABLE CHARGES ADD
CONSTRAINT FK_CHARGE_CHAMPIONSHIP FOREIGN KEY(CHAMPIONSHIP_CATEGORY)
	REFERENCES CHAMPIONSHIP_CATEGORIES(CHAMPIONSHIP_CATEGORY_ID)
GO

------------------------------------------

DECLARE @account int
DECLARE @school nvarchar(255)

DECLARE account_cursor CURSOR FOR
SELECT a.ACCOUNT_ID, s.SCHOOL_NAME+' (בית ספר)'
FROM ACCOUNTS a INNER JOIN SCHOOLS s ON a.SCHOOL_ID=s.SCHOOL_ID
WHERE a.SCHOOL_ID IS NOT NULL AND a.DATE_DELETED IS NULL

OPEN account_cursor
FETCH NEXT FROM account_cursor
INTO @account, @school
	
--loop over the facilities, update one at a time:
WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE ACCOUNTS SET ACCOUNT_NAME=@school
	WHERE ACCOUNT_ID=@account
	
	--advance to next record:
	FETCH NEXT FROM account_cursor
	INTO @account, @school
END

CLOSE account_cursor
DEALLOCATE account_cursor

------------------------------------------

ALTER TABLE ACCOUNTS ADD
ADDRESS nvarchar(70)
GO

------------------------------------------

DECLARE @account int
DECLARE @address nvarchar(255)

DECLARE account_cursor CURSOR FOR
SELECT a.ACCOUNT_ID, s.ADDRESS
FROM ACCOUNTS a INNER JOIN SCHOOLS s ON a.SCHOOL_ID=s.SCHOOL_ID
WHERE a.SCHOOL_ID IS NOT NULL AND a.DATE_DELETED IS NULL

OPEN account_cursor
FETCH NEXT FROM account_cursor
INTO @account, @address
	
--loop over the facilities, update one at a time:
WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE ACCOUNTS SET ADDRESS=@address
	WHERE ACCOUNT_ID=@account
	
	--advance to next record:
	FETCH NEXT FROM account_cursor
	INTO @account, @address
END

CLOSE account_cursor
DEALLOCATE account_cursor

------------------------------------------

ALTER TABLE RECEIPTS ADD
SEASON int
GO

------------------------------------------

ALTER TABLE PAYMENTS ADD
PAID_BY nvarchar(100)
GO

------------------------------------------

ALTER TABLE PAYMENTS ADD
CREDIT_CARD_TYPE int,
CREDIT_CARD_LAST_DIGITS int,
CREDIT_CARD_EXPIRE_DATE datetime,
CREDIT_CARD_PAYMENTS int
GO

------------------------------------------

ALTER TABLE RECEIPTS ADD CONSTRAINT df_receipts_id
DEFAULT(dbo.GenerateReceiptId())
FOR RECEIPT_ID