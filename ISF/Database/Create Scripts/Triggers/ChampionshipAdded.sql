-- ================================================================================
--Championship Added - 
--Adding championship's region to its regions list (CHAMPIONSHIP_REGIONS)
-- or all none central regions if its region is the central region
--Adding standard championship categories if the STANDARD_CHAMPIONSHIP_ID is
-- not null
-- ================================================================================

-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_CHAMPIONSHIP_ADDED' AND type = 'TR')
	DROP TRIGGER T_CHAMPIONSHIP_ADDED
GO

-- =======    CHAMPIONSHIP_ADDED TRIGGER   ======
CREATE TRIGGER T_CHAMPIONSHIP_ADDED
ON CHAMPIONSHIPS
AFTER INSERT AS
BEGIN
	-- Inserting regions (0 is central region)
	INSERT INTO CHAMPIONSHIP_REGIONS(CHAMPIONSHIP_ID, REGION_ID)
	SELECT I.CHAMPIONSHIP_ID, R.REGION_ID
	FROM inserted I, REGIONS R
	WHERE ((I.REGION_ID = 0 AND R.REGION_ID <> 0) OR
		(I.REGION_ID <> 0 AND R.REGION_ID = I.REGION_ID)) 
		AND (R.DATE_DELETED IS NULL)

	-- Inserting categories for standard championships
	INSERT INTO CHAMPIONSHIP_CATEGORIES(CHAMPIONSHIP_ID, CATEGORY)
	SELECT I.CHAMPIONSHIP_ID, S.CATEGORY
	FROM inserted I, STANDARD_CHAMPIONSHIP_CATEGORIES S
	WHERE I.STANDARD_CHAMPIONSHIP_ID = S.STANDARD_CHAMPIONSHIP_ID
END
GO