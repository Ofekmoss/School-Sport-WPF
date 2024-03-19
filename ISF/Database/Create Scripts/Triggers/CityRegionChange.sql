-- ================================================================================
-- City Region Change
-- Moving all city's schools and facilities to the new region
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_CITY_REGION_CHANGE' AND type = 'TR')
	DROP TRIGGER T_CITY_REGION_CHANGE
GO

-- =======    T_CITY_REGION_CHANGE TRIGGER   ======
CREATE TRIGGER T_CITY_REGION_CHANGE
ON CITIES
AFTER UPDATE AS
IF UPDATE (REGION_ID)
DECLARE
  @cityId int,
  @regionId int
BEGIN
  SELECT @cityId = CITY_ID, @regionId = REGION_ID
  FROM inserted

  UPDATE SCHOOLS
  SET REGION_ID = @regionId
  WHERE CITY_ID = @cityId
  
  UPDATE FACILITIES
  SET REGION_ID = @regionId
  WHERE CITY_ID = @cityId
END
GO