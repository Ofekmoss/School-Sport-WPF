-- =============================================
ALTER TABLE PLAYERS
ADD CHARGE_ID int
GO
-- =============================================

-- =============================================
ALTER TABLE PLAYERS
ADD CONSTRAINT FK_PLAYERS_CHARGE 
FOREIGN KEY(CHARGE_ID)
REFERENCES CHARGES(CHARGE_ID)
GO
-- =============================================

-- =============================================
ALTER TABLE PLAYERS
ADD STATUS int
GO
-- =============================================

-- =============================================
ALTER TABLE PLAYERS
ADD REMARKS nvarchar(100)
GO
-- =============================================