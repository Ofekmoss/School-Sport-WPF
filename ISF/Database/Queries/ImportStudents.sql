DECLARE @id_type int
DECLARE @id_number int
DECLARE @first_name nvarchar(15)
DECLARE @last_name nvarchar(20)
DECLARE @birth_date datetime
DECLARE @school_symbol nvarchar(7)
DECLARE @grade int
DECLARE @current int

DECLARE student_cursor CURSOR FOR
SELECT ID_NUMBER_TYPE, ID_NUMBER, FIRST_NAME, LAST_NAME, BIRTH_DATE, SCHOOL_SYMBOL, GRADE
FROM TEMP_STUDENTS
WHERE ID_NUMBER BETWEEN 11310331 AND 21310331

OPEN student_cursor
FETCH NEXT FROM student_cursor
INTO @id_type,@id_number, @first_name, @last_name, @birth_date, @school_symbol, @grade
	
--loop over the facilities, update one at a time:
SET @current = 0
WHILE @@FETCH_STATUS = 0
BEGIN
	IF @current<50
	BEGIN
		PRINT CONVERT(nvarchar(15), @current) + ': ' + CONVERT(nvarchar(15), @id_number) + ', ' +@first_name + ' ' + @last_name;
		--UPDATE FACILITIES SET CITY_ID=(SELECT CITY_ID FROM SCHOOLS WHERE SCHOOL_ID=@school)
		--WHERE FACILITY_ID=@facility
	END
	
	SET @current = @current+1
	
	--advance to next record:
	FETCH NEXT FROM student_cursor
	INTO @id_type,@id_number, @first_name, @last_name, @birth_date, @school_symbol, @grade
END

CLOSE student_cursor
DEALLOCATE student_cursor 

--WHERE ID_NUMBER BETWEEN 11310331 AND 202000000
--WHERE ID_NUMBER BETWEEN 202000000 AND 203500000
--WHERE ID_NUMBER BETWEEN 203500000 AND 204500000
--WHERE ID_NUMBER BETWEEN 204500000 AND 206000000
--WHERE ID_NUMBER BETWEEN 206000000 AND 207000000

--11,310,331......555,977,503