-- =============================================
--Team Charge
-- =============================================
DROP TRIGGER T_TEAM_CHARGE
GO

CREATE TRIGGER T_TEAM_CHARGE
ON TEAMS
AFTER INSERT AS
BEGIN
  DECLARE insert_cursor CURSOR FOR
  SELECT TEAM_ID, SCHOOL_ID
  FROM inserted
  
  DECLARE product_cursor CURSOR FOR
  SELECT DISTINCT PRODUCT_ID, PRICE
  FROM PRODUCTS WHERE PRODUCT_ID=1
  
  DECLARE @school int
  DECLARE @team int
  
  DECLARE @product int
  DECLARE @price float
  
  OPEN product_cursor
  FETCH NEXT FROM product_cursor
  INTO @product, @price
  CLOSE product_cursor
  DEALLOCATE product_cursor
  
  OPEN insert_cursor
  
  FETCH NEXT FROM insert_cursor
  INTO @team, @school
  
  WHILE @@FETCH_STATUS = 0
  BEGIN
    INSERT INTO CHARGES(SCHOOL_ID, PRODUCT_ID, STATUS, CHARGE_PRICE)
    VALUES(@school, @product, 1, @price)
    
    UPDATE TEAMS
    SET CHARGE_ID = IDENT_CURRENT('CHARGES')
    WHERE TEAM_ID = @team
    
    FETCH NEXT FROM insert_cursor
    INTO @team, @school
  END
  
  CLOSE insert_cursor
  DEALLOCATE insert_cursor
END
GO
 
CREATE TRIGGER T_TEAM_UNCHARGE
ON TEAMS
AFTER DELETE AS
BEGIN
  DELETE CHARGES
  WHERE CHARGE_ID IN (SELECT CHARGE_ID FROM deleted)
END
GO

-- =============================================
--Player Charge
-- =============================================
DROP TRIGGER T_PLAYER_CHARGE
GO

CREATE TRIGGER T_PLAYER_CHARGE
ON PLAYERS
AFTER INSERT AS
BEGIN
  DECLARE p_insert_cursor CURSOR FOR
  SELECT s.SCHOOL_ID, p.PLAYER_ID
  FROM inserted p, STUDENTS s 
  WHERE p.STUDENT_ID=s.STUDENT_ID
  
  DECLARE product_cursor CURSOR FOR
  SELECT DISTINCT PRODUCT_ID, PRICE
  FROM PRODUCTS WHERE PRODUCT_ID=2
  
  DECLARE @school int
  DECLARE @player int
  
  DECLARE @product int
  DECLARE @price float
  
  OPEN product_cursor
  FETCH NEXT FROM product_cursor
  INTO @product, @price
  CLOSE product_cursor
  DEALLOCATE product_cursor
  
  OPEN p_insert_cursor
  
  FETCH NEXT FROM p_insert_cursor
  INTO @school, @player
  
  WHILE @@FETCH_STATUS = 0
  BEGIN
    INSERT INTO CHARGES(SCHOOL_ID, PRODUCT_ID, STATUS, CHARGE_PRICE)
    VALUES(@school, @product, 1, @price)
    
    UPDATE PLAYERS
    SET CHARGE_ID = IDENT_CURRENT('CHARGES')
    WHERE PLAYER_ID = @player
    
    FETCH NEXT FROM p_insert_cursor
    INTO @school, @player
  END
  
  CLOSE p_insert_cursor
  DEALLOCATE p_insert_cursor
END
GO
 
DROP TRIGGER T_PLAYER_UNCHARGE
GO
 
CREATE TRIGGER T_PLAYER_UNCHARGE
ON PLAYERS
AFTER DELETE AS
BEGIN
  DELETE CHARGES
  WHERE CHARGE_ID IN (SELECT CHARGE_ID FROM deleted)
END
GO