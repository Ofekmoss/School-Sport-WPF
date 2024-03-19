IF EXISTS(SELECT * FROM sysobjects WHERE name = 'ENTITY_TYPES' AND type = 'U')
	DROP TABLE ENTITY_TYPES
GO

-- =======    ENTITY_TYPES TABLE ======
CREATE TABLE ENTITY_TYPES (
ENTITY_TYPE			nvarchar(100)	NOT NULL,
CLASS				nvarchar(500)	NOT NULL,
CONSTRAINT PK_ENTITY_TYPES PRIMARY KEY(ENTITY_TYPE)
)
GO

INSERT INTO ENTITY_TYPES(ENTITY_TYPE, CLASS)
VALUES('championship', 'Sport.Entities.Championship,Sport.Entities');
INSERT INTO ENTITY_TYPES(ENTITY_TYPE, CLASS)
VALUES('championshipcategory', 'Sport.Entities.ChampionshipCategory,Sport.Entities');
INSERT INTO ENTITY_TYPES(ENTITY_TYPE, CLASS)
VALUES('championshipregion', 'Sport.Entities.ChampionshipRegion,Sport.Entities');
INSERT INTO ENTITY_TYPES(ENTITY_TYPE, CLASS)
VALUES('school', 'Sport.Entities.School,Sport.Entities');
INSERT INTO ENTITY_TYPES(ENTITY_TYPE, CLASS)
VALUES('team', 'Sport.Entities.Team,Sport.Entities');
GO

