-- =============================================
-- School Club Charge
-- =============================================
DROP TRIGGER T_CLUB_CHARGE
GO

CREATE TRIGGER T_CLUB_CHARGE
ON SCHOOLS
AFTER UPDATE AS
BEGIN
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
  
  OPEN product_cursor
  FETCH NEXT FROM product_cursor
  INTO @product, @price
  CLOSE product_cursor
  DEALLOCATE product_cursor
  
  OPEN update_cursor
  
  FETCH NEXT FROM update_cursor
  INTO @school, @club, @charge
  
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
END
GO

DROP TRIGGER T_SCHOOL_DELETED
GO

CREATE TRIGGER T_SCHOOL_DELETED
ON SCHOOLS
FOR DELETE AS
BEGIN
  DECLARE charge_cursor CURSOR FOR
  SELECT DISTINCT CLUB_CHARGE_ID 
  FROM deleted
  
  DECLARE @charge int
  
  OPEN charge_cursor
  
  FETCH NEXT FROM charge_cursor
  INTO @charge
  
  WHILE @@FETCH_STATUS = 0
  BEGIN
     DELETE FROM CHARGES WHERE CHARGE_ID=@charge
     FETCH NEXT FROM charge_cursor
     INTO @charge
  END
  
  CLOSE charge_cursor
  DEALLOCATE charge_cursor
END
GO

DROP TRIGGER T_CHARGE_DELETED
GO

/*
CREATE TRIGGER T_CHARGE_DELETED
ON CHARGES
INSTEAD OF DELETE AS
BEGIN
  UPDATE SCHOOLS SET CLUB_CHARGE_ID=NULL 
  WHERE CLUB_CHARGE_ID IN (SELECT DISTINCT CHARGE_ID FROM deleted)

  DELETE FROM CHARGES
  WHERE CHARGE_ID IN (SELECT DISTINCT CHARGE_ID FROM deleted)
END
GO
*/