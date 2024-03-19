-- Deleting existing triggers
-- ============================================
IF EXISTS(SELECT * FROM sysobjects WHERE name = 'T_MATCH_UPDATED' AND type = 'TR')
	DROP TRIGGER T_MATCH_UPDATED
GO

-- =======    CHAMPIONSHIP_STATUS_CHANGED TRIGGER   ======
CREATE TRIGGER T_MATCH_UPDATED
ON CHAMPIONSHIP_MATCHES
AFTER UPDATE AS
BEGIN
	DECLARE @category int
	DECLARE @phase int
	DECLARE @group int
	DECLARE @round int
	DECLARE @cycle int
	DECLARE @match int
	DECLARE @newtime datetime
	DECLARE @oldtime datetime
	
	IF Cursor_Status('variable', 'match_cursor') <= 0
	BEGIN
		DECLARE match_cursor CURSOR FOR
		SELECT DISTINCT ins.CHAMPIONSHIP_CATEGORY_ID, ins.PHASE, ins.NGROUP, 
		ins.ROUND, ins.CYCLE, ins.MATCH, ins.TIME, del.TIME
		FROM deleted del, inserted ins
		WHERE del.CHAMPIONSHIP_CATEGORY_ID=ins.CHAMPIONSHIP_CATEGORY_ID
		AND del.PHASE=ins.PHASE AND del.NGROUP=ins.NGROUP AND del.ROUND=ins.ROUND
		AND del.CYCLE=ins.CYCLE AND del.MATCH=ins.MATCH
		AND del.TIME<>ins.TIME
		
		OPEN match_cursor
		FETCH NEXT FROM match_cursor
		INTO @category, @phase, @group, @round, @cycle, @match, @newtime, @oldtime
		
		--loop over the championships, check each championship status:
		WHILE @@FETCH_STATUS = 0
		BEGIN
			--check old year:
			IF YEAR(@oldtime)>1900
			BEGIN
				--check new time:
				IF @newtime>getdate()
				BEGIN
					UPDATE CHAMPIONSHIP_MATCHES
					SET DATE_CHANGED_DATE=GETDATE()
					WHERE CHAMPIONSHIP_CATEGORY_ID=@category
					AND PHASE=@phase AND NGROUP=@group AND ROUND=@round
					AND CYCLE=@cycle AND MATCH=@match
				END
			END
			
			--advance to next record:
			FETCH NEXT FROM match_cursor
			INTO @category, @phase, @group, @round, @cycle, @match, @newtime, @oldtime
		END
		
		CLOSE match_cursor
		DEALLOCATE match_cursor
	END
END
GO 