UPDATE SCHOOLS SET CLUB_CHARGE_ID=NULL
WHERE CLUB_CHARGE_ID IN 
(SELECT DISTINCT CHARGE_ID FROM CHARGES
 WHERE CHARGE_PRICE=0)
GO

UPDATE PLAYERS SET CHARGE_ID=NULL
WHERE CHARGE_ID IN 
(SELECT DISTINCT CHARGE_ID FROM CHARGES
 WHERE CHARGE_PRICE=0)
GO

UPDATE TEAMS SET CHARGE_ID=NULL
WHERE CHARGE_ID IN 
(SELECT DISTINCT CHARGE_ID FROM CHARGES
 WHERE CHARGE_PRICE=0)
GO

DELETE FROM CHARGES
WHERE CHARGE_PRICE=0
GO 