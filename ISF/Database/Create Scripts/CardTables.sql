 -- =============================================
-- CardTables.sql
--		STUDENT_CARDS
--
-- =============================================

-- Deleting existing tables
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'STUDENT_CARDS' AND type = 'U')
	DROP TABLE STUDENT_CARDS
GO


-- Creating tables
-- ============================================

-- =======    STUDENT_CARDS TABLE   ======
CREATE TABLE STUDENT_CARDS (
CARD_ID				int			IDENTITY (1,1),		--primary key
STUDENT_ID			int			NOT NULL,			--student to which the card belongs
SPORT_ID			int			NOT NULL,			--type of sport
ISSUE_DATE			datetime	NOT NULL,			--time the card was issued
ISSUE_SEASON		int			NOT NULL,			--season in which card was issued
STICKER_DATE		datetime,						--time the last sticker was given
STICKER_SEASON		int,							--season in which last sticker given
DATE_LAST_MODIFIED DATETIME,
DATE_DELETED datetime,
timestamp,
CONSTRAINT PK_STUDENT_CARDS PRIMARY KEY(CARD_ID),
CONSTRAINT UN_CARD_STUDENT_SPORT UNIQUE(STUDENT_ID,SPORT_ID,DATE_DELETED),
CONSTRAINT FK_STUDENTCARD_STUDENT FOREIGN KEY(STUDENT_ID)
	REFERENCES STUDENTS(STUDENT_ID),
CONSTRAINT FK_STUDENTCARD_SPORT FOREIGN KEY(SPORT_ID)
	REFERENCES SPORTS(SPORT_ID),
CONSTRAINT FK_STUDENTCARD_CARD_SEASON FOREIGN KEY(ISSUE_SEASON)
	REFERENCES SEASONS(SEASON),	
CONSTRAINT FK_STUDENTCARD_STICKER_SEASON FOREIGN KEY(STICKER_SEASON)
	REFERENCES SEASONS(SEASON))
GO