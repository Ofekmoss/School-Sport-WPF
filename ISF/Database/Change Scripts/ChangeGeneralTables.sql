
------------------------------------------
ALTER TABLE BUGS
ALTER COLUMN DESCRIPTION nvarchar(1024) 
GO
------------------------------------------

------------------------------------------
ALTER TABLE SCHOOLS
DROP CONSTRAINT UN_SCHOOL_SYMBOL
GO
------------------------------------------

------------------------------------------
ALTER TABLE SCHOOLS
ALTER COLUMN SYMBOL nvarchar(7) NOT NULL
GO
------------------------------------------

------------------------------------------
ALTER TABLE SCHOOLS ADD
CONSTRAINT UN_SCHOOL_SYMBOL UNIQUE(SYMBOL, DATE_DELETED)
GO
------------------------------------------

------------------------------------------
UPDATE SCHOOLS SET SCHOOL_NAME=REPLACE(SCHOOL_NAME, '(', 'zz')
GO
------------------------------------------

------------------------------------------
UPDATE SCHOOLS SET SCHOOL_NAME=REPLACE(SCHOOL_NAME, ')', '(')
GO
------------------------------------------

------------------------------------------
UPDATE SCHOOLS SET SCHOOL_NAME=REPLACE(SCHOOL_NAME, 'zz', ')')
GO
------------------------------------------

------------------------------------------
UPDATE SCHOOLS SET SYMBOL=SUBSTRING(SYMBOL, 1, 6)
WHERE LEN(SYMBOL)=6
GO
------------------------------------------

------------------------------------------
ALTER TABLE EQUIPMENT ADD
EQUIPMENT_ORDER_DATE datetime NOT NULL default GETDATE()
GO
------------------------------------------

------------------------------------------
ALTER TABLE SPORTS ADD
POINTS_NAME nvarchar(20)
GO
------------------------------------------

------------------------------------------
ALTER TABLE ISF_ARTICLES ADD
ARTICLE_SUB_CAPTION nvarchar(255)
GO
------------------------------------------

------------------------------------------
ALTER TABLE ISF_ARTICLES ADD
IS_PRIMARY_ARTICLE int NOT NULL DEFAULT 0
GO
------------------------------------------

------------------------------------------
ALTER TABLE ISF_ARTICLES ADD
IS_SUB_ARTICLE int NOT NULL DEFAULT 0
GO
------------------------------------------

------------------------------------------
ALTER TABLE ISF_ARTICLES ADD
DATE_DELETED datetime
GO
------------------------------------------

------------------------------------------
ALTER TABLE ISF_ARTICLES
ALTER COLUMN ARTICLE_IMAGE_NAME nvarchar(100)
GO
------------------------------------------

------------------------------------------
ALTER TABLE ISF_ARTICLES ADD
ARTICLE_LINKS nvarchar(1024)
GO
------------------------------------------

------------------------------------------
ALTER TABLE ISF_ARTICLES ADD
ARTICLE_ATTACHMENTS nvarchar(1024)
GO
------------------------------------------

------------------------------------------
ALTER TABLE WEBSITE_PAGES ADD
PAGE_INDEX int NOT NULL DEFAULT 0
GO
------------------------------------------

------------------------------------------
ALTER TABLE USERS ADD
USER_EMAIL nvarchar(200)
GO
------------------------------------------

------------------------------------------
ALTER TABLE REGIONS ADD
COORDINATOR int
GO
------------------------------------------

------------------------------------------
ALTER TABLE REGIONS ADD
CONSTRAINT FK_REGION_COORDINATOR FOREIGN KEY(COORDINATOR) 
REFERENCES USERS(USER_ID)
GO
------------------------------------------

------------------------------------------
ALTER TABLE FACILITIES ADD
CITY_ID			int
GO

ALTER TABLE FACILITIES ADD CONSTRAINT FK_FACILITIES_CITY
FOREIGN KEY(CITY_ID) REFERENCES CITIES(CITY_ID)
GO
------------------------------------------

------------------------------------------
DECLARE @facility int
DECLARE @school int

DECLARE city_cursor CURSOR FOR
SELECT DISTINCT FACILITY_ID, SCHOOL_ID
FROM FACILITIES
WHERE (SCHOOL_ID IS NOT NULL)
AND (DATE_DELETED IS NULL)

OPEN city_cursor
FETCH NEXT FROM city_cursor
INTO @facility, @school
	
--loop over the facilities, update one at a time:
WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE FACILITIES SET CITY_ID=(SELECT CITY_ID FROM SCHOOLS WHERE SCHOOL_ID=@school)
	WHERE FACILITY_ID=@facility
	
	--advance to next record:
	FETCH NEXT FROM city_cursor
	INTO @facility, @school
END

CLOSE city_cursor
DEALLOCATE city_cursor
------------------------------------------

------------------------------------------
DECLARE @facility int
DECLARE @school int

DECLARE address_cursor CURSOR FOR
SELECT DISTINCT FACILITY_ID, SCHOOL_ID
FROM FACILITIES
WHERE (SCHOOL_ID IS NOT NULL)
AND ((ADDRESS IS NULL) OR (ADDRESS=''))
AND (DATE_DELETED IS NULL)

OPEN address_cursor
FETCH NEXT FROM address_cursor
INTO @facility, @school
	
--loop over the facilities, update one at a time:
WHILE @@FETCH_STATUS = 0
BEGIN
	UPDATE FACILITIES SET ADDRESS=(SELECT ADDRESS FROM SCHOOLS WHERE SCHOOL_ID=@school)
	WHERE FACILITY_ID=@facility
	
	--advance to next record:
	FETCH NEXT FROM address_cursor
	INTO @facility, @school
END

CLOSE address_cursor
DEALLOCATE address_cursor

------------------------------------------

ALTER TABLE FUNCTIONARIES
ADD ZIP_CODE nvarchar(15)
GO

ALTER TABLE FUNCTIONARIES
ADD EMAIL nvarchar(100)
GO

------------------------------------------

ALTER TABLE SCHOOLS ADD
MANAGER_CELL_PHONE nvarchar(15)
GO

ALTER TABLE FUNCTIONARIES ADD
CELL_PHONE nvarchar(15)
GO

ALTER TABLE FUNCTIONARIES ADD
FUNCTIONARY_NUMBER int
GO

ALTER TABLE FACILITIES ADD
FACILITY_NUMBER int
GO

------------------------------------------

ALTER TABLE FUNCTIONARIES ADD
ID_NUMBER int,
FUNCTIONARY_STATUS int,
HAS_ANOTHER_JOB int,
WORK_ENVIROMENT int,
SEX_TYPE int,
BIRTH_DATE datetime,
SENIORITY int,
PAYMENT nvarchar(15),
REMARKS nvarchar(50)
GO

------------------------------------------

ALTER TABLE PRACTICE_CAMP_PARTICIPANTS ADD
SEX_TYPE int
GO

------------------------------------------

ALTER TABLE PRACTICE_CAMP_PARTICIPANTS ADD
PARTICIPANT_EMAIL nvarchar(150)
GO

------------------------------------------

ALTER TABLE FUNCTIONARIES ADD
GOT_STICKER int DEFAULT 0
GO

------------------------------------------

ALTER TABLE TEACHER_COURSES ADD
COACH_TRAINING_TYPE nvarchar(255),
COACH_TRAINING_HOURS int
GO