DECLARE mycursor CURSOR FOR
SELECT NAME FROM sysobjects
WHERE TYPE='U'

DECLARE @tablename nvarchar(50)

OPEN mycursor

FETCH NEXT FROM mycursor
INTO @tablename

--loop over the supervisors and send messages:
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT 'ALTER TABLE '+@tablename
	PRINT 'ADD DATE_LAST_MODIFIED DATETIME'
	PRINT 'GO'
	
	--advance to next record:
	FETCH NEXT FROM mycursor
	INTO @tablename
END

CLOSE mycursor
DEALLOCATE mycursor