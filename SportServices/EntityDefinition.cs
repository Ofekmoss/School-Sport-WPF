using System;
using System.Collections;

namespace SportServices
{
	public struct FilterField
	{
		public int		Field;
		public object	Value;
		public bool		Not;
	}

	public class EntityDefinition
	{
		// Database table name that stores entities
		public string	TableName;
		// Fields of the table name ordered as entity fields
		public string[]	Fields;
		// Index of Id field in fields
		public int		IdField;

		// Season clause
		public string	SeasonClause;
		
		public int[]	DefaultFilter;
		public string	StaticWhere;

		public EntityDefinition(string tableName, string[] fields, 
			int idField)
		{
			TableName = tableName;
			Fields = fields;
			IdField = idField;
			DefaultFilter = null;
			StaticWhere = null;
			SeasonClause = null;
		}


		private static Hashtable entities;
		static EntityDefinition()
		{
			entities = new Hashtable();

			EntityDefinition ed;
			
			ed = new EntityDefinition("ACCOUNTS",
				new string[] {"ACCOUNT_ID", "REGION_ID", "ACCOUNT_NAME", "SCHOOL_ID", "BALANCE", 
								"ADDRESS", "DATE_LAST_MODIFIED" }, 
				0);
			ed.DefaultFilter = new int[] { 1 /* regionid */ };
			entities[Sport.Entities.Account.TypeName] = ed;
			
			ed = new EntityDefinition("ACCOUNT_ENTRIES",
				new string[] { "ACCOUNT_ENTRY_ID", "REGION_ID", "ACCOUNT_ID", 
								 "ENTRY_TYPE", "SUM", "DESCRIPTION", "ADDITIONAL", 
								 "ENTRY_DATE", "SEASON", "DATE_LAST_MODIFIED"},
				0);
			//ed.SeasonClause = "(SEASON IS NULL OR SEASON = @SEASON)";
			ed.DefaultFilter = new int[] { 2 /* accountid */ };
			entities[Sport.Entities.AccountEntry.TypeName] = ed;
			
			ed = new EntityDefinition("BUGS",
				new string[] { "BUG_ID", "BUG_DATE", "TITLE", "DESCRIPTION", 
					"STATUS", "USER_ID", "TYPE", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Bug.TypeName] = ed;
			
			ed = new EntityDefinition("CITIES",
				new string[] { "CITY_ID", "CITY_NAME", "REGION_ID", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.City.TypeName] = ed;
			
			ed = new EntityDefinition("CHAMPIONSHIPS",
				new string[] { "CHAMPIONSHIP_ID", "SEASON", "CHAMPIONSHIP_NAME", "REGION_ID",
					 "SPORT_ID", "IS_CLUBS", "CHAMPIONSHIP_STATUS", "LAST_REGISTRATION_DATE",
					 "START_DATE", "END_DATE", "ALT_START_DATE", "ALT_END_DATE",
					 "FINALS_DATE", "ALT_FINALS_DATE", "RULESET_ID", "IS_OPEN",
					 "CHAMPIONSHIP_SUPERVISOR", "STANDARD_CHAMPIONSHIP_ID", "CHAMPIONSHIP_NUMBER", 
					 "REMARKS", "DATE_LAST_MODIFIED"},
				0);
			ed.SeasonClause = "SEASON = @SEASON";
			ed.DefaultFilter = new int[] { 0 };
			entities[Sport.Entities.Championship.TypeName] = ed;
			
			ed = new EntityDefinition("CHAMPIONSHIP_CATEGORIES",
				new string[] { "CHAMPIONSHIP_ID", "CHAMPIONSHIP_CATEGORY_ID", "CATEGORY", 
					"STATUS", "REGISTRATION_PRICE", "MAX_STUDENT_BIRTHDAY", 
					"CHAMPIONSHIP_CATEGORY_INDEX", "DATE_LAST_MODIFIED" },
				1);
			ed.SeasonClause = "CHAMPIONSHIP_ID IN (SELECT CHAMPIONSHIP_ID FROM CHAMPIONSHIPS WHERE SEASON = @SEASON)";
			ed.DefaultFilter = new int[] { 1 };
			entities[Sport.Entities.ChampionshipCategory.TypeName] = ed;
			
			ed = new EntityDefinition("CHAMPIONSHIP_REGIONS",
				new string[] { "CHAMPIONSHIP_REGION_ID", "CHAMPIONSHIP_ID", "REGION_ID", 
								"DATE_LAST_MODIFIED"},
				0);
			ed.SeasonClause = "CHAMPIONSHIP_ID IN (SELECT CHAMPIONSHIP_ID FROM CHAMPIONSHIPS WHERE SEASON = @SEASON)";
			entities[Sport.Entities.ChampionshipRegion.TypeName] = ed;
			
			ed = new EntityDefinition("CHARGES",
				new string[] {"CHARGE_ID", "REGION_ID", "ACCOUNT_ID", "PRODUCT_ID", 
							"AMOUNT", "PRICE", "CHARGE_DATE", "STATUS", "ADDITIONAL", 
							"CHAMPIONSHIP_CATEGORY", "DATE_LAST_MODIFIED"}, 
				0);
			ed.SeasonClause = "(CHAMPIONSHIP_CATEGORY IS NULL OR (CHAMPIONSHIP_CATEGORY IN (SELECT DISTINCT cc.CHAMPIONSHIP_CATEGORY_ID FROM CHAMPIONSHIP_CATEGORIES cc INNER JOIN CHAMPIONSHIPS c ON cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID WHERE c.SEASON = @SEASON)))";
				//(CHAMPIONSHIP_CATEGORY IS NULL OR (CHAMPIONSHIP_CATEGORY IS NOT NULL AND ";
			ed.DefaultFilter = new int[] { 1 /* regionid */ };
			entities[Sport.Entities.Charge.TypeName] = ed;
			
			ed = new EntityDefinition("COURTS",
				new string[] { "COURT_ID", "COURT_NAME", "FACILITY_ID", 
								"COURT_TYPE_ID", "DATE_LAST_MODIFIED" },
				0);
			ed.DefaultFilter = new int[] { 2 /* facilityid */ };
			entities[Sport.Entities.Court.TypeName] = ed;
			
			ed = new EntityDefinition("COURT_TYPES",
				new string[] { "COURT_TYPE_ID", "COURT_TYPE_NAME", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.CourtType.TypeName] = ed;

			ed = new EntityDefinition("COURT_TYPE_SPORTS",
				new string[] { "COURT_TYPE_SPORT_ID", "COURT_TYPE_ID", "SPORT_ID", "SPORT_FIELD_TYPE_ID",
								 "SPORT_FIELD_ID", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.CourtTypeSport.TypeName] = ed;
			
			ed = new EntityDefinition("CREDITS",
				new string[] { "CREDIT_ID", "REGION_ID", "RECEIPT_ID",
								 "ACCOUNT_ID", "CREDIT", "DATE_LAST_MODIFIED"},
				0);
			ed.DefaultFilter = new int[] { 2 /* receiptid */ };
			entities[Sport.Entities.Credit.TypeName] = ed;
			
			ed = new EntityDefinition("EQUIPMENT",
				new string[] { "EQUIPMENT_ID", "EQUIPMENT_TYPE_ID", "EQUIPMENT_REGION_ID", 
							"EQUIPMENT_SPORT_ID", "EQUIPMENT_CHAMPIONSHIP_ID", "EQUIPMENT_CATEGORY_ID", 
							"EQUIPMENT_AMOUNT", "EQUIPMENT_PRICE", "EQUIPMENT_ORDER_DATE", 
							"DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Equipment.TypeName] = ed;

			ed = new EntityDefinition("EQUIPMENT_CATEGORIES",
				new string[] { "EQUIPMENT_CATEGORY_ID", "EQUIPMENT_TYPE_ID",
								 "CHAMPIONSHIP_CATEGORY_ID", "EQUIPMENT_PRICE",
								 "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.EquipmentCategory.TypeName] = ed;
			
			ed = new EntityDefinition("EQUIPMENT_CHAMPIONSHIPS",
				new string[] { "EQUIPMENT_CHAMPIONSHIP_ID", "EQUIPMENT_TYPE_ID",
								 "CHAMPIONSHIP_ID", "EQUIPMENT_PRICE",
								 "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.EquipmentChampionship.TypeName] = ed;
			
			ed = new EntityDefinition("EQUIPMENT_REGIONS",
				new string[] { "EQUIPMENT_REGION_ID", "EQUIPMENT_TYPE_ID",
								 "REGION_ID", "EQUIPMENT_PRICE",
								 "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.EquipmentRegion.TypeName] = ed;
			
			ed = new EntityDefinition("EQUIPMENT_SPORTS",
				new string[] { "EQUIPMENT_SPORT_ID", "EQUIPMENT_TYPE_ID",
								 "SPORT_ID", "EQUIPMENT_PRICE",
								 "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.EquipmentSport.TypeName] = ed;
			
			ed = new EntityDefinition("EQUIPMENT_TYPES",
				new string[] { "EQUIPMENT_TYPE_ID", "EQUIPMENT_NAME", "EQUIPMENT_BASE_PRICE", 
								 "EQUIPMENT_TYPE_TYPE", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.EquipmentType.TypeName] = ed;
			
			// Facilities
			ed = new EntityDefinition("FACILITIES",
				new string[] { "FACILITY_ID", "FACILITY_NAME", "REGION_ID", "SCHOOL_ID", 
								"ADDRESS", "PHONE", "FAX", "CITY_ID", "FACILITY_NUMBER", 
								"DATE_LAST_MODIFIED" },
				0);
			ed.DefaultFilter = new int[] { 2 /* regionid */ };
			entities[Sport.Entities.Facility.TypeName] = ed;
			
			// Functionaries
			ed = new EntityDefinition("FUNCTIONARIES",
				new string[] { "FUNCTIONARY_ID", "FUNCTIONARY_NAME", "FUNCTIONARY_TYPE", 
								"REGION_ID", "CITY_ID", "SCHOOL_ID", "ADDRESS", "PHONE", 
								"FAX", "ZIP_CODE", "EMAIL", "CELL_PHONE", 
								"FUNCTIONARY_NUMBER", "ID_NUMBER", "FUNCTIONARY_STATUS", 
								"HAS_ANOTHER_JOB", "WORK_ENVIROMENT", "SEX_TYPE", 
								"BIRTH_DATE", "SENIORITY", "PAYMENT", "REMARKS", "GOT_STICKER", 
								"DATE_LAST_MODIFIED" },
				0);
			//ed.DefaultFilter = new int[] { 2 /* type */ };
			entities[Sport.Entities.Functionary.TypeName] = ed;
			
			// Functionary Sport
			ed = new EntityDefinition("FUNCTIONARY_SPORT",
				new string[] { "FUNCTIONARY_SPORT_ID", 
								"FUNCTIONARY_ID", 
								"SPORT_ID", 
								"DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.FunctionarySport.TypeName] = ed;
			
			ed = new EntityDefinition("GAME_BOARDS",
				new string[] { "GAME_BOARD_ID", "GAME_BOARD_NAME", "RANGE", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.GameBoard.TypeName] = ed;
			
			ed = new EntityDefinition("INSTANT_MESSAGES",
				new string[] {"INSTANT_MESSAGE_ID", "SENDER", "RECIPIENT", "DATE_SENT", 
								 "CONTENTS", "DATE_READ", "DATE_LAST_MODIFIED"}, 
				0);
			//ed.DefaultFilter = new int[] { 1 /* sender */ };
			entities[Sport.Entities.InstantMessage.TypeName] = ed;
			
			ed = new EntityDefinition("ISF_PAYMENTS",
				new string[] { "ISF_PAYMENT_ID", "EQUIPMENT_ID", "PAYMENT_SUM", 
								"PAYMENT_DESCRIPTION", "PAYMENT_DATE", "PAYMENT_TYPE", 
								"PAID_BY", "DATE_LAST_MODIFIED"},
				0);
			entities[Sport.Entities.IsfPayment.TypeName] = ed;
			
			ed = new EntityDefinition("LOG_TABLE",
				new string[] { "LOG_ID", "USER_ID", "LOG_DATE", "DESCRIPTION", 
								"VERSION_USED", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Log.TypeName] = ed;
			
			ed = new EntityDefinition("MESSAGES",
				new string[] { "MESSAGE_ID", "USER_ID", "MESSAGE_TYPE", "MESSAGE_STATUS", 
								 "MESSAGE_TEXT", "TIME_SENT",  "TIME_READ", "DATE_LAST_MODIFIED"},
				0);
			//ed.DefaultFilter = new int[] { 1 /* userid */ };
			entities[Sport.Entities.Message.TypeName] = ed;
			
			ed = new EntityDefinition("PAYMENTS",
				new string[] { "PAYMENT_ID", "RECEIPT_ID", "PAYMENT_TYPE", 
							"PAYMENT_SUM", "BANK", "BANK_BRANCH", "BANK_ACCOUNT",
							"REFERENCE", "PAYMENT_DATE", "CREDIT_CARD_TYPE", 
							"CREDIT_CARD_LAST_DIGITS", "CREDIT_CARD_EXPIRE_DATE", 
							"CREDIT_CARD_PAYMENTS", "DATE_LAST_MODIFIED"},
				0);
			ed.DefaultFilter = new int[] { 1 /* receiptid */ };
			entities[Sport.Entities.Payment.TypeName] = ed;
			
			ed = new EntityDefinition("PHASE_PATTERNS",
				new string[] { "PHASE_PATTERN_ID", "PHASE_PATTERN_NAME", "RANGE", 
								"DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.PhasePattern.TypeName] = ed;
			
			ed = new EntityDefinition("PLAYERS",
				new string[] {"PLAYER_ID", "STUDENT_ID", "TEAM_ID", "TEAM_NUMBER", 
							"CHARGE_ID", "STATUS", "REMARKS", 
							"REGISTRATION_DATE", "GOT_STICKER", "DATE_LAST_MODIFIED" }, 
				0);
			ed.DefaultFilter = new int[] { 2 /* teamid */ };
			ed.SeasonClause = "TEAM_ID IN (SELECT TEAM_ID FROM TEAMS WHERE CHAMPIONSHIP_ID IN (SELECT CHAMPIONSHIP_ID FROM CHAMPIONSHIPS WHERE SEASON = @SEASON))";
			entities[Sport.Entities.Player.TypeName] = ed;
			
			ed = new EntityDefinition("PRACTICE_CAMPS",
				new string[] {"PRACTICE_CAMP_ID", "SPORT_ID", "DATE_START", "DATE_FINISH", 
								 "BASE_PRICE", "REMARKS", "DATE_LAST_MODIFIED" }, 
				0);
			entities[Sport.Entities.PracticeCamp.TypeName] = ed;
			
			ed = new EntityDefinition("PRACTICE_CAMP_PARTICIPANTS",
				new string[] {"PARTICIPANT_ID", "PRACTICE_CAMP_ID", "PARTICIPANT_NAME", "PARTICIPANT_ADDRESS", 
								 "PARTICIPANT_SCHOOL", "PARTICIPANT_BIRTHDAY", "PARTICIPANT_PHONE", "PARTICIPANT_CELL_PHONE", 
								 "REMARKS", "CHARGE_ID", "IS_CONFIRMED", "SEX_TYPE", "PARTICIPANT_EMAIL", "DATE_LAST_MODIFIED"}, 
				0);
			entities[Sport.Entities.PracticeCampParticipant.TypeName] = ed;
			
			ed = new EntityDefinition("PRODUCT_AREAS",
				new string[] {"PRODUCT_AREA_ID", "PRODUCT_AREA_NAME", "DATE_LAST_MODIFIED" }, 
				0);
			entities[Sport.Entities.ProductArea.TypeName] = ed;

			ed = new EntityDefinition("PRODUCTS",
				new string[] {"PRODUCT_ID", "PRODUCT_AREA_ID", "PRODUCT_NAME", "PRICE", "DATE_LAST_MODIFIED" }, 
				0);
			entities[Sport.Entities.Product.TypeName] = ed;
			
			ed = new EntityDefinition("RECEIPTS",
				new string[] {"RECEIPT_ID", "NUMBER", "REGION_ID", "ACCOUNT_ID", 
								 "RECEIPT_SUM", "RECEIPT_DATE", "REMARKS", "SEASON", 
								 "DATE_LAST_MODIFIED"}, 
				0);
			ed.SeasonClause = "(SEASON IS NULL OR SEASON = @SEASON)";
			ed.DefaultFilter = new int[] { 2 /* regionid */ };
			entities[Sport.Entities.Receipt.TypeName] = ed;
			
			ed = new EntityDefinition("REFEREES",
				new string[] { "REFEREE_ID", "REFEREE_NAME", "REFEREE_TYPE", 
								"DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Referee.TypeName] = ed;
			
			ed = new EntityDefinition("REGIONS",
				new string[] { "REGION_ID", "NUMBER", "REGION_NAME", "ADDRESS", "PHONE", 
								"FAX", "COORDINATOR", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Region.TypeName] = ed;
			
			ed = new EntityDefinition("RULESETS",
				new string[] { "RULESET_ID", "RULESET_NAME", "SPORT_ID", 
								"REGION_ID", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Ruleset.TypeName] = ed;
			
			ed = new EntityDefinition("SCHOOLS",
				new string[] { "SCHOOL_ID", "SYMBOL", "SCHOOL_NAME", "CITY_ID", "ADDRESS", 
								"MAIL_ADDRESS", "MAIL_CITY_ID", "ZIP_CODE", "EMAIL",
								"PHONE", "FAX", "MANAGER_NAME", "FROM_GRADE", "TO_GRADE", 
								"SUPERVISION_TYPE", "SECTOR_TYPE", "REGION_ID", 
								"CLUB_STATUS", "PLAYER_NUMBER_FROM", "PLAYER_NUMBER_TO",
								"MANAGER_CELL_PHONE", "DATE_LAST_MODIFIED" },
				0);
			ed.DefaultFilter = new int[] { 16 /* regionid */ };
			entities[Sport.Entities.School.TypeName] = ed;
			
			ed = new EntityDefinition("SEASONS",
				new string[] { "SEASON", "NAME", "STATUS", "START_DATE", 
								"END_DATE", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Season.TypeName] = ed;
			
			ed = new EntityDefinition("SPORTS",
				new string[] { "SPORT_ID", "SPORT_NAME", "SPORT_TYPE", 
								"RULESET_ID", "POINTS_NAME", "CENTRAL_REGION_ONLY", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.Sport.TypeName] = ed;
			
			ed = new EntityDefinition("SPORT_FIELDS",
				new string[] { "SPORT_FIELD_ID", "SPORT_FIELD_TYPE_ID", "SPORT_FIELD_NAME", 
								"DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.SportField.TypeName] = ed;
			
			ed = new EntityDefinition("SPORT_FIELD_TYPES",
				new string[] { "SPORT_FIELD_TYPE_ID", "SPORT_ID", "SPORT_FIELD_TYPE_NAME", "COMPETITION_TYPE", 
								"DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.SportFieldType.TypeName] = ed;
			
			ed = new EntityDefinition("STANDARD_CHAMPIONSHIPS",
				new string[] { "STANDARD_CHAMPIONSHIP_ID", "STANDARD_CHAMPIONSHIP_NAME", "SPORT_ID", 
								 "IS_REGIONAL", "IS_OPEN", "IS_CLUBS", 
								 "RULESET_ID", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.StandardChampionship.TypeName] = ed;
			
			ed = new EntityDefinition("STANDARD_CHAMPIONSHIP_CATEGORIES",
				new string[] { "STANDARD_CHAMPIONSHIP_ID", "STANDARD_CHAMPIONSHIP_CATEGORY_ID", 
								"CATEGORY", "DATE_LAST_MODIFIED" },
				1);
			entities[Sport.Entities.StandardChampionshipCategory.TypeName] = ed;
			
			ed = new EntityDefinition("STUDENTS",
				new string[] { "STUDENT_ID", "ID_NUMBER_TYPE", "ID_NUMBER", "FIRST_NAME",
								 "LAST_NAME", "BIRTH_DATE", "SCHOOL_ID", "GRADE",
								 "SEX_TYPE", "DATE_LAST_MODIFIED" },
				0);
			ed.DefaultFilter = new int[] { 6 /* schoolid *//* , 7 grade */ };
			entities[Sport.Entities.Student.TypeName] = ed;

			ed = new EntityDefinition("TEAMS",
				new string[] { "TEAM_ID", "SCHOOL_ID", "CHAMPIONSHIP_CATEGORY_ID", 
								 "CHAMPIONSHIP_ID",  "STATUS", "TEAM_INDEX", 
								 "TEAM_SUPERVISOR", "PLAYER_NUMBER_FROM", "PLAYER_NUMBER_TO",
								 "REGISTRATION_DATE", "DATE_LAST_MODIFIED", "CHARGE_ID", "PLAYERS_COUNT"},
				0);
			ed.SeasonClause = "CHAMPIONSHIP_ID IN (SELECT CHAMPIONSHIP_ID FROM CHAMPIONSHIPS WHERE SEASON = @SEASON)";
			entities[Sport.Entities.Team.TypeName] = ed;

			ed = new EntityDefinition("TEACHER_COURSES",
				new string[] { "TEACHER_COURSE_ID", "TEACHER_FIRST_NAME", "TEACHER_LAST_NAME", 
					"TEACHER_ID_NUMBER",  "TEACHER_BIRTHDAY", "TEACHER_ADDRESS", "TEACHER_CITY", 
					"TEACHER_ZIP_CODE", "TEACHER_EMAIL", "TEACHER_CELL_PHONE", "TEACHER_HOME_PHONE", 
					"TEACHER_FAX_NUMBER", "TEACHER_GENDER", "TEACHER_SCHOOL_NAME", "TEACHER_CITY_NAME", 
					"TEACHER_SCHOOL_ADDRESS", "TEACHER_SPORT1", "TEACHER_SPORT2", "TEACHER_VETERANSHIP", 
					"TEACHER_EXPERTISE_TYPE", "TEACHER_TEAM_AGE_TYPE", "COURSE_HOLIDAY_TYPE", "COURSE_YEAR", 
					"COURSE_SPORT", "CHARGE_ID", "IS_CONFIRMED", "COACH_TRAINING_TYPE", "COACH_TRAINING_HOURS", "DATE_LAST_MODIFIED" },
				0);
			entities[Sport.Entities.TeacherCourse.TypeName] = ed;
			
			ed = new EntityDefinition("USERS",
				new string[] { "USER_ID", "USER_LOGIN", "USER_FIRST_NAME", 
								 "USER_LAST_NAME", "REGION_ID",  "SCHOOL_ID", "USER_TYPE", 
								 "USER_PERMISSIONS", "USER_EMAIL", "DATE_LAST_MODIFIED" },
				0);
			ed.DefaultFilter = new int[] { 0  };
			entities[Sport.Entities.User.TypeName] = ed;
		}

		public static EntityDefinition GetEntityDefinition(string entityName)
		{
			return (EntityDefinition) entities[entityName];
		}

	}
}
