using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Xml.Serialization;
using Sport.Common;
using Sport.Core;
using System.Collections.Generic;
using ISF.DataLayer;

namespace SportServices
{
	/// <summary>
	/// Summary description for DataService.
	/// </summary>
	/*[System.Web.Services.Protocols.SoapDocumentService(RoutingStyle=System.Web.Services.Protocols.SoapServiceRoutingStyle.RequestElement)]*/
	[WebService(Namespace = "http://www.mir.co.il/DataService")]
	public class DataService : System.Web.Services.WebService
	{
		#region structs and private data
		public struct Entity
		{
			public object[] Fields;

			public Entity(object[] fields)
			{
				Fields = fields;
			}
		}

		public struct FunctionaryMatchData
		{
			public int FunctionaryID;
			public int ChampionshipCategoryID;
			public int Phase;
			public int Group;
			public int Round;
			public int Cycle;
			public int Match;
		}

		public struct ReceiptData
		{
			public int ID;
			public string Number;
			public int Account;
			public double Sum;
			public DateTime Date;
			public string Remarks;
		}

		public struct ReceiptData_Basic
		{
			public string ID;
			public string Number;
			public string Account;
			public string Sum;
			public string Date;
			public string Remarks;
		}

		private readonly string[] unsecuredEntities = new string[] {
			Sport.Entities.Season.TypeName, 
			Sport.Entities.Championship.TypeName, 
			Sport.Entities.ChampionshipCategory.TypeName, 
			Sport.Entities.Sport.TypeName,
			Sport.Entities.Region.TypeName,
			Sport.Entities.Ruleset.TypeName,
			Sport.Entities.Team.TypeName,
			Sport.Entities.School.TypeName,
			Sport.Entities.City.TypeName,
			Sport.Entities.Student.TypeName,
			Sport.Entities.ChampionshipRegion.TypeName,
			Sport.Entities.Message.TypeName
		};

		/* private readonly string[] arrSecuredEntities=new string[] {
			Sport.Entities.Season.TypeName, 
			Sport.Entities.Charge.TypeName, 
			Sport.Entities.GameBoard.TypeName, 
			Sport.Entities.Log.TypeName,
			Sport.Entities.Message.TypeName, 
			Sport.Entities.Payment.TypeName,
			Sport.Entities.PhasePattern.TypeName,
			Sport.Entities.Product.TypeName,
			Sport.Entities.User.TypeName }; */
		#endregion

		#region construction
		public DataService()
		{
			InitializeComponent();
		}

		#region Component Designer generated code

		//Required by the Web Services Designer 
		private IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion
		#endregion

		#region web methods
		#region Entity Methods
		#region Get
		[WebMethod(EnableSession = true)]
		public Entity[] GetEntities(string entityName, FilterField[] filterFields, ref decimal timestamp, out int[] deleted)
		{
			deleted = null;
			int userID = Common.ToIntDef(Session[SessionService.SessionKey_UserID], 0);

			List<Entity> newEntities = new List<Entity>();
			List<int> deletedEntities = new List<int>();
			EntityDefinition entityDefinition = EntityDefinition.GetEntityDefinition(entityName);

			if (entityDefinition != null)
			{
				StatementBuilder sb = new StatementBuilder(entityDefinition, Session[SessionService.SessionKey_Season]);
				sb.Filter = filterFields;

				SimpleCommand cmd = sb.GetSelect(timestamp);

				// Reading all entities of bigger timestamp
				SimpleTable table = cmd.GetData();
				table.Rows.ForEach(row =>
				{
					// The last selection field is DATE_DELETED
					// if its not null the row was deleted
					object lastSelectionValue = row[entityDefinition.Fields.Length];
					if ((lastSelectionValue is DateTime) && ((DateTime)lastSelectionValue).Year > 1900)
					{
						// adding to newly deleted entities
						deletedEntities.Add((int)row[entityDefinition.IdField]);
					}
					else
					{
						object[] fields = new object[entityDefinition.Fields.Length];
						for (int f = 0; f < entityDefinition.Fields.Length; f++)
						{
							fields[f] = row[f];
							if (fields[f] is System.DBNull)
								fields[f] = null;
						}

						// adding to newly read entities
						newEntities.Add(new Entity(fields));
					}
				});

				SimpleCommand tsc = sb.GetTimestampSelect();
				table = tsc.GetData();
				if (table.Rows.Count > 0)
					timestamp = Common.ToDecimal(table.Rows[0][0] as System.Array);

				if (deletedEntities.Count > 0)
					deleted = deletedEntities.ToArray();

				return newEntities.ToArray();
			}

			return null;
		}
		#endregion

		#region Update
		[WebMethod]
		public int MarkMessageRead(string username, string password, int messageID)
		{
			if (!Common.IsAuthorized(username, password, false))
				throw new Exception("can't mark message read: not authorized");

			string strSQL = "UPDATE INSTANT_MESSAGES SET DATE_READ=GETDATE() " +
				"WHERE INSTANT_MESSAGE_ID=@id";
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", messageID));
		}

		[WebMethod(EnableSession = true)]
		public bool InsertEntity(string entityName, ref object[] values)
		{
			Common.ApplyNullValues(ref values);

			VerifyAuthorizedUser("Can't insert entity: not authorized. (403)",
				entityName, values);
			return InsertEntity(entityName, values);
		}

		[WebMethod(EnableSession = true)]
		public bool UpdateEntity(string entityName, object[] values)
		{
			Common.ApplyNullValues(ref values);

			if (entityName != Sport.Entities.Message.TypeName)
				VerifyAuthorizedUser("Can't update entity: not authorized. (403)", entityName, values);
			return UpdateEntityInternal(entityName, values) != -1;
		}
		#endregion

		#region Delete
		[WebMethod(EnableSession = true)]
		public int DeleteEntity(string entityName, int id)
		{
			return DeleteEntityInternal(entityName, id);
		}
		#endregion

		#region general
		[WebMethod]
		public void SetTeacherCourseProduct(int productId)
		{
			Common.WriteIniValue("TeacherCourseProduct", productId.ToString(), this.Server);
		}

		[WebMethod]
		public int GetTeacherCourseProduct()
		{
			return Common.ToIntDef(Common.ReadIniValue("TeacherCourseProduct", this.Server), -1);
		}

		[WebMethod]
		public void SetTeacherCourseSports(int[] sports)
		{
			Common.WriteIniValue("TeacherCourseSports",
				String.Join(",", Sport.Common.Tools.ToStringArray(sports)), this.Server);
		}

		[WebMethod]
		public int[] GetTeacherCourseSports()
		{
			string strSports = Common.ReadIniValue("TeacherCourseSports", this.Server);
			if (strSports == null || strSports.Length == 0)
				return null;

			string[] arrSports = strSports.Split(',');
			List<int> sports = new List<int>();
			foreach (string sport in arrSports)
			{
				int id = Common.ToIntDef(sport, -1);
				if (id >= 0 && sports.IndexOf(id) < 0)
					sports.Add(id);
			}
			return sports.ToArray();
		}

		[WebMethod]
		public string GetEntityTypeClass(string entityType)
		{
			string strSQL = "SELECT CLASS FROM ENTITY_TYPES WHERE ENTITY_TYPE = @0";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", entityType));
			string entityTypeClass = table.Rows.Count > 0 ? table.Rows[0][0].ToString() : null;
			return entityTypeClass;
		}

		[WebMethod(EnableSession = true)]
		public FilterField[] LookupEntityFilter(string entityType, int entityId)
		{
			EntityDefinition entityDefinition = EntityDefinition.GetEntityDefinition(entityType);

			if (entityDefinition != null && entityDefinition.DefaultFilter != null)
			{
				FilterField[] resultFilter = new FilterField[entityDefinition.DefaultFilter.Length];
				for (int f = 0; f < entityDefinition.DefaultFilter.Length; f++)
				{
					resultFilter[f] = new FilterField();
					resultFilter[f].Field = entityDefinition.DefaultFilter[f];
				}

				FilterField[] idFilter = new FilterField[1];
				idFilter[0].Field = entityDefinition.IdField;
				idFilter[0].Value = entityId;

				StatementBuilder sb = new StatementBuilder(entityDefinition, Session[SessionService.SessionKey_Season]);
				sb.Filter = idFilter;
				SimpleCommand cmd = sb.GetSelect(0);
				SimpleTable table = cmd.GetData();
				if (table.Rows.Count > 0)
				{
					SimpleRow row = table.Rows[0];
					for (int f = 0; f < entityDefinition.DefaultFilter.Length; f++)
						resultFilter[f].Value = row[resultFilter[f].Field];
				}
				
				//can't contain DbNull or else errors will occur.
				for (int i = 0; i < resultFilter.Length; i++)
				{
					if (resultFilter[i].Value is System.DBNull)
						resultFilter[i].Value = null;
				}

				return resultFilter;
			}
			
			return null;
		}
		#endregion
		#endregion

		#region Data Related Methods
		#region region data methods
		[WebMethod(EnableSession = true)]
		public string GetRegion(int regionID)
		{
			string strSQL = "SELECT REGION_NAME FROM REGIONS " + 
				"WHERE REGION_ID=@1 AND DATE_DELETED IS NULL";
			return DB.Instance.ExecuteScalar(strSQL, "", new SimpleParameter("@1", regionID)).ToString();
		}

		[WebMethod(EnableSession = true)]
		public SimpleData[] GetRegionsData()
		{
			string strSQL = "SELECT REGION_ID, REGION_NAME FROM REGIONS ";
			strSQL += "WHERE DATE_DELETED IS NULL ";
			strSQL += "ORDER BY REGION_ID ASC";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			return table.ToSimpleData().ToArray();
		}
		#endregion

		#region sport data methods
		[WebMethod(EnableSession = true)]
		public SimpleData[] GetSportsData()
		{
			string strSQL = "SELECT SPORT_ID, SPORT_NAME FROM SPORTS " + 
				"WHERE DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			return table.ToSimpleData().ToArray();
		}

		[WebMethod(EnableSession = true)]
		public SimpleData[] GetSportFieldsHavingPlayers(int season)
		{
			string strSQL = "Select Distinct s.SPORT_ID, s.SPORT_NAME " +
				"From SPORTS s Inner Join CHAMPIONSHIPS c On c.SPORT_ID=s.SPORT_ID " +
				"Inner Join TEAMS t On t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID " +
				"Inner Join PLAYERS p On p.TEAM_ID=t.TEAM_ID " +
				"Where c.SEASON=@season And s.DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@season", season));
			return table.ToSimpleData().ToArray();
		}
		#endregion

		#region championship data methods
		#region get competitions by some parameter
		[WebMethod]
		public ChampionshipService.Competition[] GetSportFieldCompetitions(int sportFieldID)
		{
			return _GetCompetitions(sportFieldID, -1, -1);
		}

		[WebMethod]
		public ChampionshipService.Competition[] GetCourtCompetitions(int courtID)
		{
			return _GetCompetitions(-1, courtID, -1);
		}

		[WebMethod]
		public ChampionshipService.Competition[] GetFacilityCompetitions(int facilityID)
		{
			return _GetCompetitions(-1, -1, facilityID);
		}
		#endregion

		#region get matches by some parameter
		[WebMethod(EnableSession = true)]
		public MatchData[] GetRawMatchesByDate(DateTime start, DateTime end, int region,
			int sport, int championship, int category, int facility)
		{
			return _GetMatchData(start, end, facility, -1, -1, false, region, sport, championship, category);
		}

		[WebMethod(EnableSession = true)]
		public MatchData[] GetMatchesByDate(DateTime start, DateTime end)
		{
			return _GetMatchData(start, end, -1, -1, -1);
		}

		[WebMethod(EnableSession = true)]
		public MatchData[] GetMatchesByFacility(int facility)
		{
			return _GetMatchData(DateTime.MinValue, DateTime.MinValue, facility, -1, -1);
		}

		[WebMethod(EnableSession = true)]
		public MatchData[] GetMatchesByCourt(int court)
		{
			return _GetMatchData(DateTime.MinValue, DateTime.MinValue, -1, court, -1);
		}

		[WebMethod(EnableSession = true)]
		public MatchData[] GetMatchesByTeam(int team)
		{
			return _GetMatchData(DateTime.MinValue, DateTime.MinValue, -1, -1, team);
		}

		[WebMethod(EnableSession = true)]
		public CompetitionData[] GetCompetitionsByDate(DateTime start, DateTime end)
		{
			return _GetCompetitionData(start, end, true, -1, -1, -1);
		}

		[WebMethod(EnableSession = true)]
		public CompetitionData[] GetRawCompetitionsByDate(DateTime start, DateTime end,
			int region, int sport, int championship, int facility, int category)
		{
			return _GetCompetitionData(start, end, false, region, sport, championship, facility, category);
		}
		#endregion

		#region get championships by some parameter
		[WebMethod]
		public int[] GetContainingChampionships(int sportFieldID, int facilityID, int courtID)
		{
			return _GetChampionshipCategories(sportFieldID, courtID, facilityID);
		}
		#endregion

		#region get championship data
		[WebMethod(EnableSession = true)]
		public ChampionshipData[] GetChampionshipData(int champID, int regionID)
		{
			int season = GetLatestSeason();

			//build select statement:
			System.Text.StringBuilder strSQL = new System.Text.StringBuilder();
			strSQL.Append("SELECT c.CHAMPIONSHIP_ID, c.SEASON, c.CHAMPIONSHIP_NAME, c.REGION_ID, ");
			strSQL.Append("r.REGION_NAME, r.ADDRESS, r.PHONE, r.FAX, s.SPORT_NAME, ");
			strSQL.Append("s.SPORT_TYPE, c.SPORT_ID, c.IS_CLUBS, ");
			strSQL.Append("c.CHAMPIONSHIP_STATUS, c.LAST_REGISTRATION_DATE, c.START_DATE, ");
			strSQL.Append("c.END_DATE, c.ALT_START_DATE, c.ALT_END_DATE, c.FINALS_DATE, ");
			strSQL.Append("c.ALT_FINALS_DATE, c.RULESET_ID, rs.RULESET_NAME, c.IS_OPEN, ");
			strSQL.Append("c.CHAMPIONSHIP_SUPERVISOR, u.USER_FIRST_NAME, u.USER_LAST_NAME, ");
			strSQL.Append("c.STANDARD_CHAMPIONSHIP_ID, sc.STANDARD_CHAMPIONSHIP_NAME, ");
			strSQL.Append("rs.SPORT_ID AS RS_SPORT_ID, rs.REGION_ID AS RS_REGION_ID ");
			strSQL.Append("FROM ((((CHAMPIONSHIPS c LEFT JOIN REGIONS r ON c.REGION_ID=r.REGION_ID) ");
			strSQL.Append("LEFT JOIN SPORTS s ON c.SPORT_ID=s.SPORT_ID) ");
			strSQL.Append("LEFT JOIN RULESETS rs ON c.RULESET_ID=rs.RULESET_ID) ");
			strSQL.Append("LEFT JOIN USERS u ON c.CHAMPIONSHIP_SUPERVISOR=u.USER_ID) ");
			strSQL.Append("LEFT JOIN STANDARD_CHAMPIONSHIPS sc ON c.STANDARD_CHAMPIONSHIP_ID=sc.STANDARD_CHAMPIONSHIP_ID ");
			strSQL.Append("WHERE c.SEASON=@season");
			if (champID >= 0)
				strSQL.Append(" AND c.CHAMPIONSHIP_ID=@champ");
			if (regionID >= 0)
				strSQL.Append(" AND c.REGION_ID=@region");
			strSQL.Append(" AND c.DATE_DELETED IS NULL AND s.DATE_DELETED IS NULL");
			strSQL.Append(" AND r.DATE_DELETED IS NULL AND u.DATE_DELETED IS NULL ");
			strSQL.Append(" AND rs.DATE_DELETED IS NULL AND sc.DATE_DELETED IS NULL");

			//initialize data object:
			List<ChampionshipData> result = new List<ChampionshipData>();

			//build parameters:
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@season", season));
			if (champID >= 0)
				parameters.Add(new SimpleParameter("@champ", champID));
			if (regionID >= 0)
				parameters.Add(new SimpleParameter("@region", regionID));

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(), parameters.ToArray());
			table.Rows.ForEach(row =>
			{
				ChampionshipData data = new ChampionshipData();
				data.ID = (int)row["CHAMPIONSHIP_ID"];
				data.Season = row.GetIntOrDefault("SEASON", -1);
				data.Name = row["CHAMPIONSHIP_NAME"].ToString();
				data.Region = new RegionData((int)row["REGION_ID"], row["REGION_NAME"].ToString());
				data.Region.Address = row["ADDRESS"].ToString();
				data.Region.Phone = row["PHONE"].ToString();
				data.Region.Fax = row["FAX"].ToString();
				data.Sport = new SportData((int)row["SPORT_ID"], row["SPORT_NAME"].ToString());
				data.Sport.Type = row.GetIntOrDefault("SPORT_TYPE", -1);
				data.IsClubs = ((int)row["IS_CLUBS"] == 1);
				data.Status = (int)row["CHAMPIONSHIP_STATUS"];
				data.LastRegistrationDate = (DateTime)row["LAST_REGISTRATION_DATE"];
				data.StartDate = (DateTime)row["START_DATE"];
				data.EndDate = (DateTime)row["END_DATE"];
				data.AltStartDate = (DateTime)row["ALT_START_DATE"];
				data.AltEndDate = (DateTime)row["ALT_END_DATE"];
				data.FinalsDate = (DateTime)row["FINALS_DATE"];
				data.AltFinalsDate = (DateTime)row["ALT_FINALS_DATE"];
				data.Ruleset = new RulesetData(row.GetIntOrDefault("RULESET_ID", -1), row["RULESET_NAME"].ToString());
				data.Ruleset.Region = new RegionData(row.GetIntOrDefault("RS_REGION_ID", -1), "");
				data.Ruleset.Sport = new SportData(row.GetIntOrDefault("RS_SPORT_ID", -1), "");
				data.IsOpen = ((int)row["IS_OPEN"] == 1);
				data.Supervisor = row["USER_FIRST_NAME"].ToString() + " " + row["USER_LAST_NAME"].ToString();
				data.StandardChampionship = new SimpleData(row.GetIntOrDefault("STANDARD_CHAMPIONSHIP_ID", -1), row["STANDARD_CHAMPIONSHIP_NAME"].ToString());
				result.Add(data);
			});

			//done.
			return result.ToArray();
		}

		[WebMethod(EnableSession = true)]
		public SimpleData[] GetChampionshipsData(int region, int sport)
		{
			string strSQL = "SELECT CHAMPIONSHIP_ID, CHAMPIONSHIP_NAME ";
			strSQL += "FROM CHAMPIONSHIPS WHERE REGION_ID=@1 AND SPORT_ID=@2 ";
			strSQL += "AND DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", region),
				new SimpleParameter("@2", sport));
			return table.ToSimpleData().ToArray();
		}

		[WebMethod(EnableSession = true)]
		public SimpleData[] GetChampionshipsBySeason(int region, int sport, int season)
		{
			string strSQL = "SELECT CHAMPIONSHIP_ID, CHAMPIONSHIP_NAME ";
			strSQL += "FROM CHAMPIONSHIPS WHERE REGION_ID=@1 AND SPORT_ID=@2 AND SEASON=@3 ";
			strSQL += "AND DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", region),
				new SimpleParameter("@2", sport),
				new SimpleParameter("@3", season));
			return table.ToSimpleData().ToArray();
		}

		[WebMethod(EnableSession = true)]
		public PlayerCardData GetPlayerCardFullData(int playerId)
		{
			string strSQL = "Select st.STUDENT_ID, st.ID_NUMBER, st.FIRST_NAME, st.LAST_NAME, st.BIRTH_DATE, st.GRADE, " +
				"st.SEX_TYPE, sc.SYMBOL, sc.SCHOOL_NAME, c.CITY_NAME, sp.SPORT_NAME, cc.MAX_STUDENT_BIRTHDAY " +
				"From PLAYERS p Inner Join STUDENTS st On p.STUDENT_ID=st.STUDENT_ID " +
				"Inner Join SCHOOLS sc On st.SCHOOL_ID=sc.SCHOOL_ID " +
				"Inner Join TEAMS t On p.TEAM_ID=t.TEAM_ID " +
				"Inner Join CHAMPIONSHIPS ch On t.CHAMPIONSHIP_ID=ch.CHAMPIONSHIP_ID " +
				"Inner Join CHAMPIONSHIP_CATEGORIES cc On t.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID " +
				"Inner Join SPORTS sp On ch.SPORT_ID=sp.SPORT_ID " +
				"Left Join CITIES c On sc.CITY_ID=c.CITY_ID " +
				"Where p.PLAYER_ID=@player";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@player", playerId));
			if (table.Rows.Count == 0)
				return null;

			SimpleRow row = table.Rows[0];
			string studentIdNumber = row["ID_NUMBER"] + "";
			object rawBirthdate = row["BIRTH_DATE"];
			DateTime birthdate = (rawBirthdate == null || rawBirthdate is DBNull) ? DateTime.MinValue : (DateTime)rawBirthdate;
			object rawGrade = row["GRADE"];
			int gradeIndex = (rawGrade == null || rawGrade is DBNull) ? 0 : (int)rawGrade;
			Sport.Types.GradeTypeLookup gradeLookup = new Sport.Types.GradeTypeLookup(true);
			if (Sport.Core.Session.Season <= 0)
			{
				Sport.Core.Session.Season = GetLatestSeason();
				Sport.Types.GradeTypeLookup.ResetRelativeGrades();

			}
			string grade = (gradeIndex <= 0) ? "" : gradeLookup.Lookup(gradeIndex);
			bool isOverAge = false;
			if (birthdate.Year > 1900)
			{
				object rawMaxBirthday = row["MAX_STUDENT_BIRTHDAY"];
				DateTime maxBirthday = (rawMaxBirthday == null || rawMaxBirthday is DBNull) ? DateTime.MinValue : (DateTime)rawMaxBirthday;
				if (maxBirthday.Year > 1900)
					isOverAge = birthdate < maxBirthday;
			}
			byte[] rawPicture = Common.BuildStudentPicture(Server, studentIdNumber);
			bool gotValidPicture = true;
			if (rawPicture == null)
			{
				gotValidPicture = false;
				bool blnFemale = (row["SEX_TYPE"] + "").Equals("2");
				string fileName = string.Format("Pictures/{0}.jpg", (blnFemale ? "anon_female" : "anon_male"));
				string filePath = Server.MapPath(fileName);
				if (System.IO.File.Exists(filePath))
					rawPicture = System.IO.File.ReadAllBytes(filePath);
			}
			return new PlayerCardData
			{
				PlayerId = playerId, 
				StudentEntityId = (int)row["STUDENT_ID"], 
				StudentIdNumber = Crypto.Encode(studentIdNumber),
				FirstName = row["FIRST_NAME"] + "",
				LastName = row["LAST_NAME"] + "", 
				Birthdate = Crypto.Encode(birthdate.Ticks.ToString()),
				Grade = grade,
				SchoolSymbol = row["SYMBOL"] + "",
				SchoolName = row["SCHOOL_NAME"] + "",
				SportName = row["SPORT_NAME"] + "", 
				IsOverAge = isOverAge, 
				RawPicture = rawPicture, 
				GotValidPicture = gotValidPicture
			};
		}

		[WebMethod(EnableSession = true)]
		public PlayerCountData[] GetPlayersCountBySeason(int season)
		{
			string strSQL = "Select t.TEAM_ID, t.CHAMPIONSHIP_ID, t.CHAMPIONSHIP_CATEGORY_ID, " +
							"	(Count(Distinct p1.PLAYER_ID) + Count(Distinct p2.PLAYER_ID)) As TotalPlayerCount, " +
							"	Count(Distinct p1.PLAYER_ID) As ConfirmedPlayersCount, Count(Distinct p2.PLAYER_ID) As UnconfirmedPlayersCount " +
							"From Teams t Inner Join CHAMPIONSHIPS c On t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID " +
							"	Left Join Players p1 On p1.TEAM_ID=t.TEAM_ID And p1.[STATUS]=2 " +
							"	Left Join Players p2 On p2.TEAM_ID=t.TEAM_ID And p2.[STATUS]<>2 " +
							"Where c.SEASON=@season And t.DATE_DELETED Is Null And p1.DATE_DELETED Is Null And p2.DATE_DELETED Is Null " +
							"Group By t.TEAM_ID, t.CHAMPIONSHIP_ID, t.CHAMPIONSHIP_CATEGORY_ID Having (Count(p1.PLAYER_ID)>0 Or Count(p2.PLAYER_ID)>0) " +
							"Order By TotalPlayerCount Desc";
			List<PlayerCountData> playerMapping = new List<PlayerCountData>();
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@season", season));
			table.Rows.ForEach(row => playerMapping.Add(new PlayerCountData
			{
				TeamId = (int)row[0], 
				ChampionshipId = (int)row[1],
				ChampionshipCategoryId = (int)row[2], 
				TotalPlayersCount = (int)row[3], 
				ConfirmedPlayersCount = (int)row[4], 
				UnconfirmedPlayersCount = (int)row[5]
			}));
			return playerMapping.ToArray();
		}

		[WebMethod(EnableSession = true)]
		public SimpleData[] GetTeamsByChampionshipCategory(int champCategoryId)
		{
			string strSQL = "Select Distinct t.TEAM_ID, t.TEAM_INDEX, s.SCHOOL_NAME, c.CITY_NAME " + 
				"From Teams t Inner Join SCHOOLS s On t.SCHOOL_ID=s.SCHOOL_ID " + 
				"Left Join CITIES c On s.CITY_ID=c.CITY_ID " + 
				"Where t.CHAMPIONSHIP_CATEGORY_ID=@category And t.DATE_DELETED Is Null And c.DATE_DELETED Is Null";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@category", champCategoryId));
			List<SimpleData> data = new List<SimpleData>();
			table.Rows.ForEach(row =>
			{
				int teamId = (int)row["TEAM_ID"];
				int teamIndex = row.GetIntOrDefault("TEAM_INDEX", 0);
				string schoolName = (row["SCHOOL_NAME"] + "").Trim();
				string cityName = (row["CITY_NAME"] + "").Trim();
				data.Add(new Sport.Common.SimpleData(teamId, Common.BuildTeamName(schoolName, cityName, teamIndex)));
			});
			return data.ToArray();
		}
		#endregion

		#region championship categories
		[WebMethod(EnableSession = true)]
		public CategoryData[] GetChampionshipCategories(int champID)
		{
			//build select statement:
			System.Text.StringBuilder strSQL = new System.Text.StringBuilder();
			strSQL.Append("SELECT cc.CHAMPIONSHIP_CATEGORY_ID, cc.CATEGORY ");
			strSQL.Append("FROM CHAMPIONSHIPS c LEFT JOIN CHAMPIONSHIP_CATEGORIES cc ");
			strSQL.Append("ON c.CHAMPIONSHIP_ID=cc.CHAMPIONSHIP_ID ");
			strSQL.Append("WHERE c.CHAMPIONSHIP_ID=@1 ");
			strSQL.Append("AND cc.DATE_DELETED IS NULL AND c.DATE_DELETED IS NULL");

			//initialize data object:
			List<CategoryData> arrCategories = new List<CategoryData>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(),
				new SimpleParameter("@1", champID));
			table.Rows.ForEach(row =>
			{
				CategoryData data = new CategoryData((int)row["CHAMPIONSHIP_CATEGORY_ID"], 
					champID, (int)row["CATEGORY"]);
				arrCategories.Add(data);
			});

			//done.
			return arrCategories.ToArray();
		}

		[WebMethod(EnableSession = true)]
		public int[] GetChampionshipCategoriesBySeason(int season)
		{
			//build SQL statement:
			string strSQL = "SELECT DISTINCT cc.CHAMPIONSHIP_CATEGORY_ID ";
			strSQL += "FROM CHAMPIONSHIPS c, CHAMPIONSHIP_CATEGORIES cc ";
			strSQL += "WHERE cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID ";
			strSQL += "AND c.SEASON=@1 AND c.DATE_DELETED IS NULL ";
			strSQL += "AND cc.DATE_DELETED IS NULL";

			//initialize data object:
			List<int> result = new List<int>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(),
				new SimpleParameter("@1", season));
			table.Rows.ForEach(row =>
			{
				result.Add((int)row["CHAMPIONSHIP_CATEGORY_ID"]);
			});
			
			//return the array:
			return result.ToArray();
		}
		#endregion
		#endregion

		#region user data methods
		[WebMethod(EnableSession = true)]
		public string GetUserName(int userID)
		{
			string strSQL = "SELECT USER_FIRST_NAME, USER_LAST_NAME ";
			strSQL += "FROM USERS WHERE USER_ID=@1 ";
			strSQL += "WHERE DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", userID));
			string result = "";
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				result = row["USER_FIRST_NAME"].ToString() + " " + row["USER_LAST_NAME"].ToString();
			}
			return result;
		}
		#endregion

		#region general data methods

		[WebMethod(EnableSession = true)]
		public SimplePlayerData[] GetSimplePlayerData(int[] championships)
		{
			if (championships.Length == 0)
				return null;
			
			//build select statement:
			string strSQL = "SELECT t.CHAMPIONSHIP_ID, t.TEAM_ID, p.PLAYER_ID, IsNull(p.[STATUS], 0) As PLAYER_STATUS, st.FIRST_NAME, st.LAST_NAME, st.BIRTH_DATE, st.ID_NUMBER, cat.CATEGORY, sc.SCHOOL_NAME, cit.CITY_NAME " +
				"FROM ((((PLAYERS p INNER JOIN STUDENTS st ON p.STUDENT_ID=st.STUDENT_ID) " +
				"INNER JOIN TEAMS t ON p.TEAM_ID=t.TEAM_ID) " +
				"INNER JOIN SCHOOLS sc ON t.SCHOOL_ID=sc.SCHOOL_ID) " +
				"LEFT JOIN CITIES cit ON sc.CITY_ID=cit.CITY_ID) " +
				"INNER JOIN CHAMPIONSHIP_CATEGORIES cat ON t.CHAMPIONSHIP_CATEGORY_ID=cat.CHAMPIONSHIP_CATEGORY_ID " +
				"WHERE t.CHAMPIONSHIP_ID IN (" + string.Join(", ", championships) + ") " +
				"AND p.DATE_DELETED IS NULL AND st.DATE_DELETED IS NULL AND t.DATE_DELETED IS NULL AND sc.DATE_DELETED IS NULL AND cit.DATE_DELETED IS NULL AND cat.DATE_DELETED IS NULL"; //ORDER BY p.PLAYER_ID

			//initialize data object:
			List<SimplePlayerData> arrPlayers = new List<SimplePlayerData>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			table.Rows.ForEach(row =>
			{
				string cityName = row["CITY_NAME"] + "";
				string schoolName = row["SCHOOL_NAME"] + "";
				if (cityName.Length > 0)
				{
					string[] possibilities = new string[] { " " + cityName, "-" + cityName, " -" + cityName, " - " + cityName };
					foreach (string p in possibilities)
						schoolName = schoolName.Replace(p, "");
				}
				arrPlayers.Add(new SimplePlayerData
				{
					ChampionshipId = (int)row["CHAMPIONSHIP_ID"],
					TeamId = (int)row["TEAM_ID"], 
					PlayerId = (int)row["PLAYER_ID"],
					PlayerStatus = (int)row["PLAYER_STATUS"], 
					PlayerName = row["FIRST_NAME"] + " " + row["LAST_NAME"],
					BirthDate = (DateTime)row["BIRTH_DATE"], 
					IdNumber = row["PLAYER_ID"] + "",
					IsMale = ((int)row["CATEGORY"] & 0xFFFF) != 0, 
					CityName = cityName,
					TeamName = schoolName
				});
			});

			//done.
			return arrPlayers.ToArray();
		}

		[WebMethod]
		public TeamPlayerNumbers[] GetPlayerNumbers(int[] teamIDs)
		{
			if (teamIDs == null || teamIDs.Length == 0)
				return null;

			//build select statement:
			string strSQL = "SELECT DISTINCT TEAM_ID, TEAM_NUMBER " +
				"FROM PLAYERS WHERE TEAM_ID IN (" + string.Join(", ", teamIDs) + ") AND DATE_DELETED IS NULL";

			//get database data:
			Dictionary<int, List<int>> dicPlayerNumbers = new Dictionary<int, List<int>>();
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			table.Rows.ForEach(row =>
			{
				int teamID = row.GetIntOrDefault("TEAM_ID", -1);
				int playerNumber = row.GetIntOrDefault("TEAM_NUMBER", -1);
				if (!dicPlayerNumbers.ContainsKey(teamID))
					dicPlayerNumbers.Add(teamID, new List<int>());
				dicPlayerNumbers[teamID].Add(playerNumber);
			});
			
			//build response array:
			TeamPlayerNumbers[] arrPlayerNumbers = new TeamPlayerNumbers[dicPlayerNumbers.Count];
			int index = 0;
			foreach (int teamId in dicPlayerNumbers.Keys)
			{
				arrPlayerNumbers[index] = new TeamPlayerNumbers(teamId, dicPlayerNumbers[teamId].ToArray());
				index++;
			}

			//done.
			return arrPlayerNumbers;
		}

		[WebMethod]
		public int[] GetFunctionaryGames(int functionaryID)
		{
			//build select statement:
			string strSQL = "SELECT DISTINCT CHAMPIONSHIP_CATEGORY_ID " +
				"FROM CHAMPIONSHIP_MATCH_FUNCTIONARIES " +
				"WHERE FUNCTIONARY_ID=@id " +
				"AND DATE_DELETED IS NULL";

			//initialize data object:
			List<int> result = new List<int>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(),
							new SimpleParameter("@id", functionaryID));
			table.Rows.ForEach(row =>
			{
				int categoryID = row.GetIntOrDefault("CHAMPIONSHIP_CATEGORY_ID", -1);
				result.Add(categoryID);
			});
			
			//done.
			return result.ToArray();
		}

		[WebMethod]
		public FunctionaryMatchData[] GetFunctionaryGamesByDate(int functionaryID,
			DateTime dtStart, DateTime dtEnd)
		{
			//build select statement:
			string strSQL = "SELECT f.CHAMPIONSHIP_CATEGORY_ID, f.PHASE, f.NGROUP, " +
				"f.ROUND, f.CYCLE, f.MATCH " +
				"FROM CHAMPIONSHIP_MATCH_FUNCTIONARIES f, CHAMPIONSHIP_MATCHES m " +
				"WHERE (f.CHAMPIONSHIP_CATEGORY_ID=m.CHAMPIONSHIP_CATEGORY_ID) " +
				"AND (f.PHASE=m.PHASE) AND (f.NGROUP=m.NGROUP) " +
				"AND (f.ROUND=m.ROUND) AND (f.CYCLE=m.CYCLE) AND (f.MATCH=m.MATCH) " +
				"AND (f.FUNCTIONARY_ID=@id) AND (m.TIME IS NOT NULL) " +
				"AND (m.TIME BETWEEN @start AND @end) AND (f.DATE_DELETED IS NULL)";

			//initialize data object:
			List<FunctionaryMatchData> result = new List<FunctionaryMatchData>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(),
							new SimpleParameter("@id", functionaryID),
							new SimpleParameter("@start", dtStart),
							new SimpleParameter("@end", dtEnd));
			table.Rows.ForEach(row =>
			{
				FunctionaryMatchData data = new FunctionaryMatchData();
				data.FunctionaryID = functionaryID;
				data.ChampionshipCategoryID = row.GetIntOrDefault("CHAMPIONSHIP_CATEGORY_ID", -1);
				data.Phase = row.GetIntOrDefault("PHASE", -1);;
				data.Group = row.GetIntOrDefault("NGROUP", -1);;
				data.Round = row.GetIntOrDefault("ROUND", -1);;
				data.Cycle = row.GetIntOrDefault("CYCLE", -1);;
				data.Match = row.GetIntOrDefault("MATCH", -1);;
				result.Add(data);
			});

			//done.
			return result.ToArray();
		}

		[WebMethod]
		public SeasonData[] GetSeasonData(int season)
		{
			List<SeasonData> arrSeasons = new List<SeasonData>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			string strSQL = "SELECT SEASON, [NAME], [STATUS], [START_DATE], [END_DATE] FROM SEASONS";
			if (season > 0)
			{
				strSQL += " WHERE SEASON=@season";
				parameters.Add(new SimpleParameter("@season", season));
			}
			
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(), parameters.ToArray());
			table.Rows.ForEach(row =>
			{
				object rawValue = row["START_DATE"];
				DateTime start = (rawValue == null || rawValue is DBNull) ? DateTime.MinValue : (DateTime)rawValue;
				rawValue = row["END_DATE"];
				DateTime end = (rawValue == null || rawValue is DBNull) ? DateTime.MinValue : (DateTime)rawValue;
				arrSeasons.Add(new SeasonData
				{
					Season = (int)row["SEASON"], 
					Name = row["NAME"] + "", 
					Status = row.GetIntOrDefault("STATUS", 0), 
					Start = start, 
					End = end
				});
			});
			return arrSeasons.ToArray();
		}

		[WebMethod(EnableSession = true)]
		public RuleData[] GetRulesetRules(int rulesetID)
		{
			//build select statement:
			System.Text.StringBuilder strSQL = new System.Text.StringBuilder();
			strSQL.Append("SELECT r.RULE_ID, r.RULE_TYPE_ID, r.VALUE, ");
			strSQL.Append("r.SPORT_FIELD_TYPE_ID, r.SPORT_FIELD_ID, ");
			strSQL.Append("r.CATEGORY, rt.CLASS ");
			strSQL.Append("FROM RULES r INNER JOIN RULE_TYPES rt ");
			strSQL.Append("ON r.RULE_TYPE_ID=rt.RULE_TYPE_ID ");
			strSQL.Append("WHERE r.RULESET_ID=@1 ");
			strSQL.Append("AND r.DATE_DELETED IS NULL");

			//initialize data object:
			List<RuleData> arrRules = new List<RuleData>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(),
							new SimpleParameter("@1", rulesetID));
			table.Rows.ForEach(row =>
			{
				RuleData data = new RuleData();
				data.Ruleset = new RulesetData(rulesetID, "");
				data.RuleID = (int)row["RULE_ID"];
				data.RuleType = new SimpleData((int)row["RULE_TYPE_ID"], row["CLASS"].ToString());
				data.SportFieldType = new SportFieldTypeData(
					row.GetIntOrDefault("SPORT_FIELD_TYPE_ID", -1), "");
				data.Value = row["VALUE"].ToString();
				data.SportField = new SportFieldData(
					row.GetIntOrDefault("SPORT_FIELD_ID", -1), "");
				data.Category = row.GetIntOrDefault("CATEGORY", Sport.Types.CategoryTypeLookup.All);
				arrRules.Add(data);
			});

			//done.
			return arrRules.ToArray();
		}

		/// <summary>
		/// returns list of ID numbers of students having the desired name.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public string[] GetStudentsByName(string firstName, string lastName)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't get students: Session Expired."); */

			string strFirstName = Common.ToStringDef(firstName, "");
			string strLastName = Common.ToStringDef(lastName, "");

			if ((strFirstName.Length == 0) && (strLastName.Length == 0))
				throw new Exception("can't get students: no name given.");

			string strSQL = "SELECT ID_NUMBER FROM STUDENTS WHERE ";
			if (strFirstName.Length > 0)
			{
				strSQL += "FIRST_NAME=@fn";
				if (strLastName.Length > 0)
					strSQL += " AND LAST_NAME=@ln";
			}
			else
			{
				strSQL += "LAST_NAME=@ln";
			}
			strSQL += " AND DATE_DELETED IS NULL";
			List<string> names = new List<string>();
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@fn", strFirstName),
				new SimpleParameter("@ln", strLastName));
			table.Rows.ForEach(row =>
			{
				names.Add(row["ID_NUMBER"].ToString());
			});

			return names.ToArray();
		}

		[WebMethod(EnableSession = true)]
		public StudentData GetStudentByNumber(string id_number)
		{
			string strSQL = "SELECT STUDENT_ID, FIRST_NAME, LAST_NAME, BIRTH_DATE, ";
			strSQL += "SCHOOL_ID, GRADE FROM STUDENTS WHERE ID_NUMBER=@1 ";
			strSQL += "AND DATE_DELETED IS NULL";
			StudentData result = new StudentData();
			int schoolID = -1;
			result.ID = -1;
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", id_number));
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				result.IdNumber = id_number;
				result.ID = (int)row["STUDENT_ID"];
				result.Birthdate = (DateTime)row["BIRTH_DATE"];
				result.FirstName = row["FIRST_NAME"].ToString();
				result.LastName = row["LAST_NAME"].ToString();
				result.Grade = row.GetIntOrDefault("GRADE", -1);
				schoolID = row.GetIntOrDefault("SCHOOL_ID", -1);
			}

			if (schoolID >= 0)
				result.School = GetSchoolData(schoolID);
			
			return result;
		}

		[WebMethod(EnableSession = true)]
		public SchoolData GetSchoolData(int school_ID)
		{
			string strSQL = "SELECT SYMBOL, SCHOOL_NAME, CITY_ID, ADDRESS, MAIL_ADDRESS, ";
			strSQL += "MAIL_CITY_ID, ZIP_CODE, EMAIL, PHONE, FAX, MANAGER_NAME, ";
			strSQL += "FROM_GRADE, TO_GRADE, SUPERVISION_TYPE, SECTOR_TYPE, REGION_ID, ";
			strSQL += "CLUB_STATUS, DATE_LAST_MODIFIED ";
			strSQL += "FROM SCHOOLS WHERE SCHOOL_ID=@1 AND DATE_DELETED IS NULL";
			SchoolData result = new SchoolData();
			int cityID = -1;
			int regionID = -1;
			result.ID = -1;
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", school_ID));
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				result.ID = school_ID;
				result.FromGrade = (int)row["FROM_GRADE"];
				result.ToGrade = (int)row["TO_GRADE"];
				result.IsClub = ((int)row["CLUB_STATUS"] == 1);
				result.Name = row["SCHOOL_NAME"].ToString();
				result.Symbol = row["SYMBOL"].ToString();
				cityID = row.GetIntOrDefault("CITY_ID", -1);
				regionID = row.GetIntOrDefault("REGION_ID", -1);
			}

			if (cityID >= 0)
				result.City = GetSimpleData(cityID, "CITIES", "CITY_ID", "CITY_NAME").Name;

			if (regionID >= 0)
				result.Region = GetRegionData(regionID);

			return result;
		}

		[WebMethod(EnableSession = true)]
		public RegionData GetRegionData(int region_id)
		{
			string strSQL = "SELECT REGION_NAME, ADDRESS, PHONE, FAX, COORDINATOR ";
			strSQL += "FROM REGIONS WHERE REGION_ID=@1 AND DATE_DELETED IS NULL";
			RegionData result = new RegionData();
			result.ID = -1;
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", region_id));
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				result.ID = region_id;
				result.Address = row["ADDRESS"].ToString();
				result.Fax = row["FAX"].ToString();
				result.Name = row["REGION_NAME"].ToString();
				result.Phone = row["PHONE"].ToString();
			}

			return result;
		}

		[WebMethod(EnableSession = true)]
		public SimpleData GetSimpleData(int ID,
			string table_name, string id_field, string name_field)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't get data: Session Expired."); */
			return _GetSimpleData(ID, table_name, id_field, name_field);
		}
		#endregion

		#region school data methods
		[WebMethod]
		public string UpdateSchoolDetails(int schoolID, string schoolManager, string schoolPhone,
			string schoolFax, string schoolEmail, string userName, string encryptedPassword)
		{
			//verify authorized user
			string strSQL = "SELECT * FROM USERS WHERE USER_LOGIN=@login AND USER_PASSWORD=@password AND DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@login", userName),
				new SimpleParameter("@password",encryptedPassword));
			bool blnUserExists = (table.Rows.Count > 0);
			if (!blnUserExists)
				return "unauthorized user";

			strSQL = "UPDATE SCHOOLS SET MANAGER_NAME=@manager, PHONE=@phone, FAX=@fax, EMAIL=@email " +
				"WHERE SCHOOL_ID=@school";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@school", schoolID));
			parameters.Add(new SimpleParameter("@manager", Tools.IIF((schoolManager == null), DBNull.Value, schoolManager)));
			parameters.Add(new SimpleParameter("@phone", Tools.IIF((schoolPhone == null), DBNull.Value, schoolPhone)));
			parameters.Add(new SimpleParameter("@fax", Tools.IIF((schoolFax == null), DBNull.Value, schoolFax)));
			parameters.Add(new SimpleParameter("@email", Tools.IIF((schoolEmail == null), DBNull.Value, schoolEmail)));
			string strError = "";
			try
			{
				DB.Instance.Execute(strSQL, parameters.ToArray());
			}
			catch (Exception ex)
			{
				strError = "general error: " + ex.Message;
			} 

			return strError;
		}
		#endregion

		#region Receipt data
		[WebMethod]
		public ReceiptData[] GetReceiptData(int receiptID_start, int receiptID_end)
		{
			string strSQL = "SELECT RECEIPT_ID, NUMBER, ACCOUNT_ID, RECEIPT_SUM, RECEIPT_DATE, REMARKS ";
			strSQL += "FROM RECEIPTS WHERE RECEIPT_ID>=@1";
			if (receiptID_end > 0 && receiptID_end >= receiptID_start)
				strSQL += " AND RECEIPT_ID<=@2";
			strSQL += " AND DATE_DELETED IS NULL";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", receiptID_start));
			if (receiptID_end > 0 && receiptID_end >= receiptID_start)
				parameters.Add(new SimpleParameter("@2", receiptID_end));
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<ReceiptData> result = new List<ReceiptData>();
			table.Rows.ForEach(row =>
			{
				ReceiptData data = new ReceiptData();
				data.ID = (int)row["RECEIPT_ID"];
				data.Number = row["NUMBER"].ToString();
				data.Account = (int)row["ACCOUNT_ID"];
				data.Sum = (int)row["RECEIPT_SUM"];
				data.Date = (DateTime)row["RECEIPT_DATE"];
				data.Remarks = row["REMARKS"].ToString();
				result.Add(data);
			});
			
			return result.ToArray();
		}

		[WebMethod]
		public ReceiptData_Basic[] GetReceiptData_Basic(int receiptID_start, int receiptID_end)
		{
			string strSQL = "SELECT RECEIPT_ID, NUMBER, ACCOUNT_ID, RECEIPT_SUM, RECEIPT_DATE, REMARKS ";
			strSQL += "FROM RECEIPTS WHERE RECEIPT_ID>=@1";
			if (receiptID_end > 0 && receiptID_end >= receiptID_start)
				strSQL += " AND RECEIPT_ID<=@2";
			strSQL += " AND DATE_DELETED IS NULL";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", receiptID_start));
			if (receiptID_end > 0 && receiptID_end >= receiptID_start)
				parameters.Add(new SimpleParameter("@2", receiptID_end));
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			List<ReceiptData_Basic> result = new List<ReceiptData_Basic>();
			table.Rows.ForEach(row =>
			{
				ReceiptData_Basic data = new ReceiptData_Basic();
				data.ID = row["RECEIPT_ID"].ToString();
				data.Number = row["NUMBER"].ToString();
				data.Account = row["ACCOUNT_ID"].ToString();
				data.Sum = row["RECEIPT_SUM"].ToString();
				data.Date = ((DateTime)row["RECEIPT_DATE"]).ToString("dd/MM/yyyy");
				data.Remarks = row["REMARKS"].ToString();
				result.Add(data);
			});
			
			return result.ToArray();
		}
		#endregion
		#endregion

		#region General Methods
		#region Website Permanent Championship Methods
		[WebMethod]
		public WebsitePermanentChampionship[] GetWebsitePermanentChampionships()
		{
			List<WebsitePermanentChampionship> permanentChamps = new List<WebsitePermanentChampionship>();
			string strSQL = "SELECT CHAMPIONSHIP_INDEX, CHAMPIONSHIP_CATEGORY_ID, CHAMPIONSHIP_TITLE " +
					"FROM WEBSITE_PERMANENT_CHAMPIONSHIPS " +
					"ORDER BY CHAMPIONSHIP_INDEX ASC";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			table.Rows.ForEach(row =>
			{
				permanentChamps.Add(new WebsitePermanentChampionship
				{
					Index = (int)row["CHAMPIONSHIP_INDEX"],
					ChampionshipCategoryId = (int)row["CHAMPIONSHIP_CATEGORY_ID"], 
					Title = row["CHAMPIONSHIP_TITLE"].ToString()
				});
			});
			return permanentChamps.ToArray();
		}

		[WebMethod]
		public int AddWebsitePermanentChampionship(WebsitePermanentChampionship championship)
		{
			List<WebsitePermanentChampionship> existingChamps = GetWebsitePermanentChampionships().ToList();
			if (existingChamps.Exists(c => c.ChampionshipCategoryId == championship.ChampionshipCategoryId))
				return -1;

			string strSQL = "INSERT INTO WEBSITE_PERMANENT_CHAMPIONSHIPS (CHAMPIONSHIP_CATEGORY_ID, CHAMPIONSHIP_TITLE, CHAMPIONSHIP_INDEX) " +
				"VALUES (@id, @title, @index)";
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", championship.ChampionshipCategoryId), 
				new SimpleParameter("@title", championship.Title), 
				new SimpleParameter("@index", championship.Index));
		}

		[WebMethod]
		public int UpdateWebsitePermanentChampionship(WebsitePermanentChampionship championship)
		{
			string strSQL = "UPDATE WEBSITE_PERMANENT_CHAMPIONSHIPS SET CHAMPIONSHIP_INDEX=@index, CHAMPIONSHIP_TITLE=@title " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@id";
			return DB.Instance.Execute(strSQL, new SimpleParameter("@index", championship.Index),
				new SimpleParameter("@title", championship.Title), 
				new SimpleParameter("@id", championship.ChampionshipCategoryId));
		}

		[WebMethod]
		public int DeleteWebsitePermanentChampionship(int championshipCategoryId)
		{
			string strSQL = "DELETE FROM WEBSITE_PERMANENT_CHAMPIONSHIPS " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@id";
			return DB.Instance.Execute(strSQL, new SimpleParameter("@id", championshipCategoryId));
		}
		#endregion

		[WebMethod]
		public int[] GetClubCategoriesBySeason(int school, int season)
		{
			//build select statement:
			string strSQL = "SELECT cc.CHAMPIONSHIP_CATEGORY_ID " +
					"FROM (CHAMPIONSHIP_CATEGORIES cc INNER JOIN CHAMPIONSHIPS c ON cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID) " +
					"INNER JOIN TEAMS t ON cc.CHAMPIONSHIP_CATEGORY_ID=t.CHAMPIONSHIP_CATEGORY_ID " +
					"WHERE c.SEASON=@season AND t.SCHOOL_ID=@school AND c.IS_CLUBS=1 " +
					"AND c.DATE_DELETED IS NULL AND cc.DATE_DELETED IS NULL AND t.DATE_DELETED IS NULL";

			//read result:
			List<int> result = new List<int>();
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@season", season),
				new SimpleParameter("@school", school));
			table.Rows.ForEach(row =>
			{
				result.Add(row.GetIntOrDefault("CHAMPIONSHIP_CATEGORY_ID", -1));
			});
			
			return result.ToArray();
		}

		[WebMethod]
		public bool HasPassword(int userid)
		{
			//build select statement:
			string strSQL = "SELECT USER_PASSWORD FROM USERS " +
				"WHERE USER_ID=@id AND DATE_DELETED IS NULL";

			//read result:
			return (DB.Instance.ExecuteScalar(strSQL, "", new SimpleParameter("@id", userid)).ToString().Length > 0);
		}

		[WebMethod]
		public int[] UsersWithoutPassword()
		{
			//build select statement:
			string strSQL = "SELECT DISTINCT USER_ID FROM USERS " +
				"WHERE (USER_PASSWORD='' OR USER_PASSWORD IS NULL) AND DATE_DELETED IS NULL";

			//read results:
			List<int> result = new List<int>();
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			table.Rows.ForEach(row =>
			{
				result.Add(row.GetIntOrDefault("USER_ID", -1));
			});
			
			return result.ToArray();
		}

		[WebMethod(EnableSession = true)]
		public void SetClientVersion(double clientVersion)
		{
			//_clientVersion = clientVersion;
			Session["ClientVersion"] = clientVersion;
		}

		[WebMethod]
		public int GetChampionshipCurrentPhase(int champCategoryID)
		{
			//build select statement:
			string strSQL = "SELECT PHASE FROM CHAMPIONSHIP_PHASES " +
				"WHERE CHAMPIONSHIP_CATEGORY_ID=@id " +
				"AND STATUS=1 " +
				"AND DATE_DELETED IS NULL";

			//read result:
			return (int)DB.Instance.ExecuteScalar(strSQL, -1,
				new SimpleParameter("@id", champCategoryID));
		}

		[WebMethod]
		public int GetLatestSeason()
		{
			//build select statement:
			string strSQL = "SELECT MAX(SEASON) FROM SEASONS WHERE STATUS=@status AND DATE_DELETED IS NULL";

			//read result:
			return Int32.Parse(DB.Instance.ExecuteScalar(strSQL, Sport.Core.Session.Season,
				new SimpleParameter("@status", (int)Sport.Types.SeasonStatus.Opened)).ToString());
		}
		#endregion
		#endregion

		#region private methods
		#region User Data

		private void VerifyAuthorizedUser(string strErrMessage,
			string entityName, object[] values)
		{
			if (entityName == Sport.Entities.PracticeCampParticipant.TypeName)
				return;

			if (entityName == Sport.Entities.TeacherCourse.TypeName)
				return;

			if (entityName == Sport.Entities.Functionary.TypeName)
				return;

			if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception(strErrMessage);

			/*
			int userID=Common.ToIntDef(Session[SessionService.SessionKey_UserID], 0);
			int userPermissions=Common.GetUserPermissions(userID);
			
			if (userPermissions <= 0)
				throw new Exception("you do not have permissions to perform this action.");
			
			if ((entityName == Sport.Entities.User.TypeName)&&
				(!Sport.Core.PermissionsManager.IsSuperUser(userID)))
			{
				int fieldIndex=(int) Sport.Entities.User.Fields.Permissions;
				int otherPermissions=Common.ToIntDef(values[fieldIndex], 0);
				int otherID=Common.ToIntDef(values[(int) Sport.Entities.User.Fields.Id], -1);
				if ((otherPermissions > userPermissions)||
					((otherPermissions == userPermissions)&&(otherID != userID)))
					throw new Exception("you do not have permission to access user with higher permission.");
				if ((otherID == userID)&&(otherPermissions != userPermissions))
					throw new Exception("can't update your own permissions");
			}
			*/
		}
		#endregion

		#region Entitities
		#region InsertEntity
		public bool InsertEntity(string entityName, object[] values)
		{
			EntityDefinition entityDefinition = EntityDefinition.GetEntityDefinition(entityName);

			StatementBuilder sb = new StatementBuilder(entityDefinition, Session[SessionService.SessionKey_Season]);
			SimpleCommand cmd = sb.GetInsert(values);

			string strSQL = "";
			if (entityName == Sport.Entities.Season.TypeName)
			{
				strSQL = "INSERT INTO SEASONS (SEASON, ";
				strSQL += "NAME, STATUS, START_DATE, END_DATE, DATE_LAST_MODIFIED) ";
				strSQL += "VALUES (@E0, @E1, @E2, @E3, @E4, @E5)";
				cmd.CommandText = strSQL;
				cmd.AddParameter("@E0", values[(int)Sport.Entities.Season.Fields.Season]);
			}

			cmd.Execute();

			if (entityName == Sport.Entities.Season.TypeName)
			{
				strSQL = "SELECT MAX(SEASON) AS MaxSeason FROM SEASONS WHERE DATE_DELETED IS NULL";
			}
			else if (entityName == Sport.Entities.Receipt.TypeName)
			{
				strSQL = "SELECT MAX(RECEIPT_ID) AS MaxReceiptId FROM RECEIPTS WHERE DATE_DELETED IS NULL";
			}
			else
			{
				strSQL = "SELECT IDENT_CURRENT('" + entityDefinition.TableName + "')";
			}
			int id = Int32.Parse(DB.Instance.ExecuteScalar(strSQL, -1).ToString());
			
			// Reading the entity, some values might have changed in triggers
			if (id != -1)
			{
				FilterField[] idFilter = new FilterField[1];
				idFilter[0].Field = entityDefinition.IdField;
				idFilter[0].Value = id;
				sb.Filter = idFilter;
				cmd = sb.GetSelect(0);
				SimpleTable table = cmd.GetData();
				if (table.Rows.Count > 0)
				{
					SimpleRow row = table.Rows[0];
					for (int f = 0; f < values.Length; f++)
					{
						values[f] = row[f];
						if (values[f] is System.DBNull)
							values[f] = null;
					}
				}
			}
			
			return true;
		}
		#endregion

		#region UpdateEntity
		private int UpdateEntityInternal(string entityName, object[] values)
		{
			EntityDefinition entityDefinition = EntityDefinition.GetEntityDefinition(entityName);

			StatementBuilder sb = new StatementBuilder(entityDefinition, Session[SessionService.SessionKey_Season]);
			SimpleCommand cmd = sb.GetUpdate(values);
			int result = cmd.Execute();

			// Reading the entity, some values might have changed in triggers
			if (result != -1)
			{
				FilterField[] idFilter = new FilterField[1];
				idFilter[0].Field = entityDefinition.IdField;
				idFilter[0].Value = values[entityDefinition.IdField];
				sb.Filter = idFilter;
				cmd = sb.GetSelect(0);
				SimpleTable table = cmd.GetData();
				if (table.Rows.Count > 0)
				{
					SimpleRow row = table.Rows[0];
					for (int f = 0; f < values.Length; f++)
					{
						values[f] = row[f];
						if (values[f] is System.DBNull)
							values[f] = null;
					}
				}
			}
			
			return result;
		}
		#endregion

		#region DeleteEntity
		public int DeleteEntityInternal(string entityName, int id)
		{
			SimpleCommand cmd = new SimpleCommand();
			if (entityName == Sport.Entities.Season.TypeName)
			{
				SimpleParameter seasonParam = new SimpleParameter("@season", id);

				string strSQL = "UPDATE STUDENT_CARDS SET ISSUE_SEASON=" +
					"(SELECT MAX(SEASON) FROM SEASONS WHERE SEASON<>@season) " +
					"WHERE ISSUE_SEASON=@season";
				DB.Instance.Execute(strSQL, seasonParam);
				
				strSQL = "UPDATE STUDENT_CARDS SET STICKER_SEASON=" +
					"(SELECT MAX(SEASON) FROM SEASONS WHERE SEASON<>@season) " +
					"WHERE STICKER_SEASON=@season";
				DB.Instance.Execute(strSQL, seasonParam);
				
				strSQL = "UPDATE CHAMPIONSHIPS SET SEASON=" +
					"(SELECT MAX(SEASON) FROM SEASONS WHERE SEASON<>@season) " +
					"WHERE SEASON=@season";
				DB.Instance.Execute(strSQL, seasonParam);

				strSQL = "DELETE FROM SEASONS WHERE SEASON=@season";
				cmd.CommandText = strSQL;
				cmd.AddParameter(seasonParam);
			}
			else
			{
				EntityDefinition entityDefinition = EntityDefinition.GetEntityDefinition(entityName);
				StatementBuilder sb = new StatementBuilder(entityDefinition, Session[SessionService.SessionKey_Season]);
				object[] entity = new object[entityDefinition.Fields.Length];
				entity[entityDefinition.IdField] = id;
				cmd = sb.GetDelete(entity);
			}

			return cmd.Execute();
		}
		#endregion
		#endregion

		#region General
		#region Simple Data
		private SimpleData _GetSimpleData(int ID,
			string table_name, string id_field, string name_field)
		{
			string strSQL = "SELECT " + name_field + " FROM " + table_name + " ";
			strSQL += "WHERE " + id_field + "=@1 AND DATE_DELETED IS NULL";
			string strName = DB.Instance.ExecuteScalar(strSQL, "",
				new SimpleParameter("@1", ID)).ToString();
			return new SimpleData(ID, strName);
		}
		#endregion

		#region Championship Data
		private int[] _GetChampionshipCategories(int sportFieldID,
			int courtID, int facilityID)
		{
			//get key field
			string strKeyField = "";
			int keyValue = -1;
			if (sportFieldID >= 0)
			{
				strKeyField = "SPORT_FIELD_ID";
				keyValue = sportFieldID;
			}
			else if (courtID >= 0)
			{
				strKeyField = "COURT_ID";
				keyValue = courtID;
			}
			else if (facilityID >= 0)
			{
				strKeyField = "FACILITY_ID";
				keyValue = facilityID;
			}

			//got anything?
			if (strKeyField.Length == 0)
				return null;

			//initialize result array:
			List<int> arrCategories = new List<int>();

			//get data from all possible tables:
			_GetChampionshipCategories(ref arrCategories,
				"CHAMPIONSHIP_COMPETITIONS", strKeyField, keyValue);
			if ((courtID >= 0) || (facilityID >= 0))
			{
				_GetChampionshipCategories(ref arrCategories,
					"CHAMPIONSHIP_COMPETITION_HEATS", strKeyField, keyValue);
				_GetChampionshipCategories(ref arrCategories,
					"CHAMPIONSHIP_TOURNAMENTS", strKeyField, keyValue);
				_GetChampionshipCategories(ref arrCategories,
					"CHAMPIONSHIP_MATCHES", strKeyField, keyValue);
			}

			//done.
			return arrCategories.ToArray();
		}

		private void _GetChampionshipCategories(ref List<int> categories,
			string strTableName, string strKeyField, int keyValue)
		{
			//build SQL statement:
			string strSQL = "SELECT DISTINCT CHAMPIONSHIP_CATEGORY_ID " +
				"FROM " + strTableName + " WHERE " + strKeyField + "=@0";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@0", keyValue));
			for (int i = 0; i < table.Rows.Count; i++)
			{
				SimpleRow row = table.Rows[i];

				//get current category:
				int categoryID = row.GetIntOrDefault("CHAMPIONSHIP_CATEGORY_ID", -1);

				//add to result array if valid:
				if (categoryID >= 0 && categories.IndexOf(categoryID) < 0)
					categories.Add(categoryID);
			}
		}
		#endregion

		#region Team Data
		private TeamData _GetTeamData(int teamID, int categoryID, int phase, int prevGroup, int prevPos)
		{
			//build select statement and parameters:
			string strSQL = "";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			if (teamID < 0)
			{
				strSQL = "SELECT GROUP_NAME FROM CHAMPIONSHIP_GROUPS " + 
					"WHERE CHAMPIONSHIP_CATEGORY_ID=@category AND PHASE=@phase AND NGROUP=@group";
				parameters.Add(new SimpleParameter("@category", categoryID));
				parameters.Add(new SimpleParameter("@phase", phase - 1));
				parameters.Add(new SimpleParameter("@group", prevGroup));
			}
			else
			{
				strSQL = "SELECT t.STATUS, t.TEAM_INDEX, t.SCHOOL_ID, s.SCHOOL_NAME " + 
					"FROM TEAMS t LEFT JOIN SCHOOLS s ON t.SCHOOL_ID=s.SCHOOL_ID " + 
					"WHERE t.TEAM_ID=@id " + 
					"AND t.DATE_DELETED IS NULL AND s.DATE_DELETED IS NULL";
				parameters.Add(new SimpleParameter("@id", teamID));
			}

			//initialize data object:
			TeamData result = new TeamData();
			result.ID = -1;

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			if (table.Rows.Count > 0)
			{
				SimpleRow row = table.Rows[0];
				result.ID = teamID;
				if (teamID < 0)
				{
					string strGroupName = row["GROUP_NAME"].ToString();
					if (!strGroupName.StartsWith("בית"))
						strGroupName = "בית " + strGroupName;
					result.School = new SchoolData();
					result.School.ID = -1;
					result.School.Name = strGroupName + " מיקום " + (prevPos + 1).ToString();
				}
				else
				{
					result.Status = (int)row["STATUS"];
					result.TeamIndex = row.GetIntOrDefault("TEAM_INDEX", -1);
					result.School = new SchoolData();
					result.School.ID =row.GetIntOrDefault("SCHOOL_ID", -1);
					result.School.Name = row["SCHOOL_NAME"].ToString();
				}
			}

			//done.
			return result;
		}

		private TeamData _GetTeamData(int teamID)
		{
			return _GetTeamData(teamID, -1, -1, -1, -1);
		}
		#endregion

		#region Match Data
		private MatchData[] _GetMatchData(DateTime start, DateTime end, int facility_id,
			int court_id, int team_id, bool blnFullDetails, int region, int sport,
			int championship, int category)
		{
			//build select statement and parameters:
			System.Text.StringBuilder strSQL = new System.Text.StringBuilder();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			strSQL.Append("SELECT m.CHAMPIONSHIP_CATEGORY_ID, \n");
			strSQL.Append("m.[TIME], \n");
			strSQL.Append("m.TEAM_A_SCORE, \n");
			strSQL.Append("m.TEAM_B_SCORE, m.[RESULT], m.PHASE, m.[ROUND], \n");
			strSQL.Append("m.NGROUP, m.MATCH, m.CYCLE, m.FACILITY_ID, m.COURT_ID, \n");
			strSQL.Append("c.CHAMPIONSHIP_ID, \n");
			strSQL.Append("p.PHASE_NAME, g.GROUP_NAME, r.ROUND_NAME, \n");
			strSQL.Append("t1.TEAM_ID AS TEAM_A, t2.TEAM_ID AS TEAM_B, \n");
			strSQL.Append("t1.PREVIOUS_GROUP AS PREV_GROUP1, t2.PREVIOUS_GROUP AS PREV_GROUP2, \n");
			strSQL.Append("t1.PREVIOUS_POSITION AS PREV_POS1, t2.PREVIOUS_POSITION AS PREV_POS2 \n");
			strSQL.Append("FROM ((((((CHAMPIONSHIP_MATCHES m LEFT JOIN CHAMPIONSHIP_CATEGORIES cc ON m.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIPS c ON cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIP_PHASES p ON m.CHAMPIONSHIP_CATEGORY_ID=p.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=p.PHASE) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIP_GROUPS g ON m.CHAMPIONSHIP_CATEGORY_ID=g.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=g.PHASE AND m.NGROUP=g.NGROUP) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIP_ROUNDS r ON m.CHAMPIONSHIP_CATEGORY_ID=r.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=r.PHASE AND m.NGROUP=r.NGROUP AND m.ROUND=r.ROUND) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIP_GROUP_TEAMS t1 ON m.CHAMPIONSHIP_CATEGORY_ID=t1.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=t1.PHASE AND m.NGROUP=t1.NGROUP AND m.TEAM_A=t1.POSITION) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIP_GROUP_TEAMS t2 ON m.CHAMPIONSHIP_CATEGORY_ID=t2.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=t2.PHASE AND m.NGROUP=t2.NGROUP AND m.TEAM_B=t2.POSITION ");
			strSQL.Append("WHERE m.DATE_DELETED IS NULL AND c.DATE_DELETED IS NULL");
			if (start.Year > 1900)
			{
				strSQL.Append(" AND m.TIME>=@start");
				parameters.Add(new SimpleParameter("@start", start));
			}
			if (end.Year < 2100 && end.Year > 1900)
			{
				strSQL.Append(" AND m.TIME<=@end");
				parameters.Add(new SimpleParameter("@end", end));
			}
			if (facility_id >= 0)
			{
				strSQL.Append(" AND m.FACILITY_ID=@facility");
				parameters.Add(new SimpleParameter("@facility", facility_id));
			}
			if (court_id >= 0)
			{
				strSQL.Append(" AND m.COURT_ID=@court");
				parameters.Add(new SimpleParameter("@court", court_id));
			}
			if (team_id >= 0)
			{
				strSQL.Append(" AND (t1.TEAM_ID=@team OR t2.TEAM_ID=@team)");
				parameters.Add(new SimpleParameter("@team", team_id));
			}
			if (region >= 0)
			{
				strSQL.Append(" AND c.REGION_ID=@region");
				parameters.Add(new SimpleParameter("@region", region));
			}
			if (sport >= 0)
			{
				strSQL.Append(" AND c.SPORT_ID=@sport");
				parameters.Add(new SimpleParameter("@sport", sport));
			}
			if (championship >= 0)
			{
				strSQL.Append(" AND c.CHAMPIONSHIP_ID=@championship");
				parameters.Add(new SimpleParameter("@championship", championship));
			}
			if (category >= 0)
			{
				strSQL.Append(" AND cc.CHAMPIONSHIP_CATEGORY_ID=@category");
				parameters.Add(new SimpleParameter("@category", category));
			}

			//initialize data object:
			List<MatchData> result = new List<MatchData>();
			Dictionary<int, TeamData> teams = new Dictionary<int, TeamData>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(), parameters.ToArray());
			for (int i = 0; i < table.Rows.Count; i++)
			{
				SimpleRow row = table.Rows[i];
				MatchData data = new MatchData();

				int championshipID = (int)row["CHAMPIONSHIP_ID"];
				if (blnFullDetails)
				{
					ChampionshipData[] arrChamps = GetChampionshipData(championshipID, -1);
					if (arrChamps == null || arrChamps.Length == 0)
						continue;
					data.Championship = arrChamps[0];
				}
				else
				{
					data.Championship = new ChampionshipData();
					data.Championship.ID = championshipID;
				}
				data.CategoryID = (int)row["CHAMPIONSHIP_CATEGORY_ID"];
				data.MatchIndex = (int)row["MATCH"];
				int courtID = row.GetIntOrDefault("COURT_ID", -1);
				if (courtID >= 0)
				{
					data.Court = new CourtData();
					data.Court.ID = courtID;
					if (blnFullDetails)
						data.Court.Name = _GetSimpleData(courtID, "COURTS", "COURT_ID", "COURT_NAME").Name;
				}
				int facilityID = row.GetIntOrDefault("FACILITY_ID", -1);
				if (facilityID >= 0)
				{
					data.Facility = new FacilityData();
					data.Facility.ID = facilityID;
					if (blnFullDetails)
						data.Facility.Name = _GetSimpleData(facilityID, "FACILITIES", "FACILITY_ID", "FACILITY_NAME").Name;
				}
				data.Cycle = (int)row["CYCLE"];
				data.Result = row.GetIntOrDefault("RESULT", -1);
				data.Round = new RoundData();
				data.Round.RoundIndex = (int)row["ROUND"];
				data.Round.RoundName = row["ROUND_NAME"].ToString();
				data.Round.Group = new GroupData();
				data.Round.Group.GroupIndex = (int)row["NGROUP"];
				data.Round.Group.GroupName = row["GROUP_NAME"].ToString();
				data.Round.Group.Phase = new PhaseData();
				data.Round.Group.Phase.PhaseIndex = (int)row["PHASE"];
				data.Round.Group.Phase.PhaseName = row["PHASE_NAME"].ToString();
				data.TeamA_Score = Double.Parse(row["TEAM_A_SCORE"].ToString());
				data.TeamB_Score = Double.Parse(row["TEAM_B_SCORE"].ToString());
				data.Time = (DateTime)row["TIME"];
				int teamA_ID = row.GetIntOrDefault("TEAM_A", -1);
				int teamB_ID = row.GetIntOrDefault("TEAM_B", -1);
				if (teamA_ID < 0)
				{
					int prevGroup = row.GetIntOrDefault("PREV_GROUP1", -1);
					int prevPos = row.GetIntOrDefault("PREV_POS1", -1);
					data.TeamA = _GetTeamData(teamA_ID, data.CategoryID, data.Round.Group.Phase.PhaseIndex, prevGroup, prevPos);
				}
				else
				{
					if (!teams.ContainsKey(teamA_ID))
					{
						TeamData team = null;
						if (blnFullDetails)
						{
							team = _GetTeamData(teamA_ID);
						}
						else
						{
							team = new TeamData();
							team.ID = teamA_ID;
						}
						teams.Add(teamA_ID, team);
					}
					data.TeamA = teams[teamA_ID];
				}
				if (teamB_ID < 0)
				{
					int prevGroup = row.GetIntOrDefault("PREV_GROUP2", -1);
					int prevPos = row.GetIntOrDefault("PREV_POS2", -1);
					data.TeamB = _GetTeamData(teamB_ID, data.CategoryID, data.Round.Group.Phase.PhaseIndex, prevGroup, prevPos);
				}
				else
				{
					if (!teams.ContainsKey(teamB_ID))
					{
						TeamData team = null;
						if (blnFullDetails)
						{
							team = _GetTeamData(teamB_ID);
						}
						else
						{
							team = new TeamData();
							team.ID = teamB_ID;
						}
						teams.Add(teamB_ID, team);
					}
					data.TeamB = teams[teamB_ID];
				}
				result.Add(data);
			};

			//read supervisors:
			string sql = "SELECT f.FUNCTIONARY_NAME, f.FUNCTIONARY_TYPE, f.CELL_PHONE " +
				"FROM CHAMPIONSHIP_MATCH_FUNCTIONARIES cmf, FUNCTIONARIES f " +
				"WHERE cmf.FUNCTIONARY_ID=f.FUNCTIONARY_ID AND " +
				"cmf.CHAMPIONSHIP_CATEGORY_ID = @category AND " +
				"cmf.PHASE = @phase AND " +
				"cmf.NGROUP = @group AND " +
				"cmf.ROUND = @round AND " +
				"cmf.CYCLE = @cycle AND " +
				"cmf.MATCH = @match";

			foreach (MatchData data in result)
			{
				table = DB.Instance.GetDataBySQL(sql,
					new SimpleParameter("@category", data.CategoryID),
					new SimpleParameter("@phase", data.Round.Group.Phase.PhaseIndex),
					new SimpleParameter("@group", data.Round.Group.GroupIndex),
					new SimpleParameter("@round", data.Round.RoundIndex),
					new SimpleParameter("@cycle", data.Cycle),
					new SimpleParameter("@match", data.MatchIndex));
				data.Supervisor = "";
				data.Referee = "";
				table.Rows.ForEach(row =>
				{
					int funcType = row.GetIntOrDefault("FUNCTIONARY_TYPE", -1);
					string funcName = row["FUNCTIONARY_NAME"].ToString();
					string cellPhone = row["CELL_PHONE"].ToString();
					switch (funcType)
					{
						case (int)Sport.Types.FunctionaryType.Coordinator:
							data.Supervisor += funcName;
							if (cellPhone.Length > 0)
								data.Supervisor += " (" + cellPhone + ")";
							data.Supervisor += ",";
							break;
						case (int)Sport.Types.FunctionaryType.Referee:
							data.Referee += funcName;
							if (cellPhone.Length > 0)
								data.Referee += " (" + cellPhone + ")";
							data.Referee += ",";
							break;
					}
				});
				if (data.Supervisor.Length > 0)
					data.Supervisor = data.Supervisor.Substring(0, data.Supervisor.Length - 1);
				if (data.Referee.Length > 0)
					data.Referee = data.Referee.Substring(0, data.Referee.Length - 1);
			}

			//done.
			return result.ToArray();
		}

		private MatchData[] _GetMatchData(DateTime start, DateTime end, int facility_id, int court_id,
			int team_id, bool blnFullDetails, int region, int sport, int championship)
		{
			return _GetMatchData(start, end, facility_id, court_id, team_id, blnFullDetails, region, sport, championship, -1);
		}

		private MatchData[] _GetMatchData(DateTime start, DateTime end, int facility_id, int court_id,
			int team_id)
		{
			return _GetMatchData(start, end, facility_id, court_id, team_id, true, -1, -1, -1);
		}
		#endregion

		#region Competition Data
		private ChampionshipService.Competition[] _GetCompetitions(int sportFieldID,
			int courtID, int facilityID)
		{
			//get key field
			string strKeyField = "";
			int keyValue = -1;
			if (sportFieldID >= 0)
			{
				strKeyField = "SPORT_FIELD_ID";
				keyValue = sportFieldID;
			}
			else if (courtID >= 0)
			{
				strKeyField = "COURT_ID";
				keyValue = courtID;
			}
			else if (facilityID >= 0)
			{
				strKeyField = "FACILITY_ID";
				keyValue = facilityID;
			}

			//got anything?
			if (strKeyField.Length == 0)
				return null;

			//build SQL statement:
			string strSQL = "SELECT CHAMPIONSHIP_CATEGORY_ID, SPORT_FIELD_ID, " +
				"COURT_ID, FACILITY_ID, TIME, LANE_COUNT " +
				"FROM CHAMPIONSHIP_COMPETITIONS " +
				"WHERE " + strKeyField + "=@0";

			//initialize result array:
			List<ChampionshipService.Competition> arrCompetitions = new List<ChampionshipService.Competition>();

			//iterate over the record and read results:
			SimpleParameter parameter = new SimpleParameter("@0", keyValue);
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameter);
			table.Rows.ForEach(row =>
			{
				//create current competition:
				ChampionshipService.Competition competition =
					new ChampionshipService.Competition(row);

				//add to result array if does not exist yet:
				if (arrCompetitions.FindIndex(c => c.championshipCategory.Equals(competition.championshipCategory)) < 0)
					arrCompetitions.Add(competition);
			});
			
			//need to read from heats table as well?
			if (courtID >= 0 || facilityID >= 0)
			{
				//build new SQL statement:
				strSQL = "SELECT CHAMPIONSHIP_CATEGORY_ID, FACILITY_ID, TIME " +
					"FROM CHAMPIONSHIP_COMPETITION_HEATS WHERE " + strKeyField + "=@0";
				//iterate over the record and read results:
				table = DB.Instance.GetDataBySQL(strSQL, parameter);
				table.Rows.ForEach(row =>
				{
					//create current competition:
					ChampionshipService.Competition competition =
						new ChampionshipService.Competition(row);

					//add to result array:
					if (arrCompetitions.FindIndex(c => c.championshipCategory.Equals(competition.championshipCategory)) < 0)
						arrCompetitions.Add(competition);
				});
			}

			//done.
			return arrCompetitions.ToArray();
		}

		private CompetitionData[] _GetCompetitionData(DateTime start, DateTime end, bool blnFullDetails,
			int region, int sport, int championship, int facility, int category)
		{
			//build select statement and parameters:
			System.Text.StringBuilder strSQL = new System.Text.StringBuilder();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			strSQL.Append("SELECT m.CHAMPIONSHIP_CATEGORY_ID, \n");
			strSQL.Append("m.[TIME], m.SPORT_FIELD_ID, m.FACILITY_ID, m.COURT_ID, \n");
			strSQL.Append("m.PHASE, m.NGROUP, m.COMPETITION, \n");
			strSQL.Append("c.CHAMPIONSHIP_ID, \n");
			strSQL.Append("p.PHASE_NAME, g.GROUP_NAME, \n");
			strSQL.Append("sf.SPORT_FIELD_NAME, sft.SPORT_FIELD_TYPE_NAME ");
			strSQL.Append("FROM (((((CHAMPIONSHIP_COMPETITIONS m LEFT JOIN CHAMPIONSHIP_CATEGORIES cc ON m.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIPS c ON cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIP_PHASES p ON m.CHAMPIONSHIP_CATEGORY_ID=p.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=p.PHASE) ");
			strSQL.Append("LEFT JOIN CHAMPIONSHIP_GROUPS g ON m.CHAMPIONSHIP_CATEGORY_ID=g.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=g.PHASE AND m.NGROUP=g.NGROUP) ");
			strSQL.Append("LEFT JOIN SPORT_FIELDS sf ON m.SPORT_FIELD_ID=sf.SPORT_FIELD_ID) ");
			strSQL.Append("LEFT JOIN SPORT_FIELD_TYPES sft ON sf.SPORT_FIELD_TYPE_ID=sft.SPORT_FIELD_TYPE_ID ");
			strSQL.Append("WHERE m.DATE_DELETED IS NULL AND c.DATE_DELETED IS NULL");
			if (start.Year > 1900)
			{
				strSQL.Append(" AND m.TIME>=@start");
				parameters.Add(new SimpleParameter("@start", start));
			}
			if (end.Year < 2100)
			{
				strSQL.Append(" AND m.TIME<=@end");
				parameters.Add(new SimpleParameter("@end", end));
			}
			if (region >= 0)
			{
				strSQL.Append(" AND c.REGION_ID=@region");
				parameters.Add(new SimpleParameter("@region", region));
			}
			if (sport >= 0)
			{
				strSQL.Append(" AND c.SPORT_ID=@sport");
				parameters.Add(new SimpleParameter("@sport", sport));
			}
			if (championship >= 0)
			{
				strSQL.Append(" AND c.CHAMPIONSHIP_ID=@championship");
				parameters.Add(new SimpleParameter("@championship", championship));
			}
			if (facility >= 0)
			{
				strSQL.Append(" AND m.FACILITY_ID=@facility");
				parameters.Add(new SimpleParameter("@facility", facility));
			}
			if (category >= 0)
			{
				strSQL.Append(" AND cc.CHAMPIONSHIP_CATEGORY_ID=@category");
				parameters.Add(new SimpleParameter("@category", category));
			}

			//initialize data object:
			List<CompetitionData> result = new List<CompetitionData>();

			//get database data:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL.ToString(), parameters.ToArray());
			table.Rows.ForEach(row =>
			{
				CompetitionData data = new CompetitionData();
				data.CategoryID = (int)row["CHAMPIONSHIP_CATEGORY_ID"];
				data.CompetitionIndex = (int)row["COMPETITION"];
				data.Group = new GroupData();
				data.GroupIndex = (int)row["NGROUP"];
				data.Group.GroupIndex = data.GroupIndex;
				data.Group.GroupName = row["GROUP_NAME"].ToString();
				data.Group.Phase = new PhaseData();
				data.Group.Phase.PhaseIndex = (int)row["PHASE"];
				data.Group.Phase.PhaseName = row["PHASE_NAME"].ToString();
				data.PhaseIndex = data.Group.Phase.PhaseIndex;
				data.SportField = new SportFieldData();
				data.SportField.ID = row.GetIntOrDefault("SPORT_FIELD_ID", -1);
				data.SportField.Name = row["SPORT_FIELD_NAME"].ToString();
				data.SportField.SportFieldType = new SportFieldTypeData();
				data.SportField.SportFieldType.Name = row["SPORT_FIELD_TYPE_NAME"].ToString();
				data.Time = (DateTime)row["TIME"];
				int courtID = row.GetIntOrDefault("COURT_ID", -1);
				if (courtID >= 0)
				{
					data.Court = new CourtData();
					data.Court.ID = courtID;
					if (blnFullDetails)
						data.Court.Name = _GetSimpleData(courtID, "COURTS", "COURT_ID", "COURT_NAME").Name;
				}
				int facilityID = row.GetIntOrDefault("FACILITY_ID", -1);
				if (facilityID >= 0)
				{
					data.Facility = new FacilityData();
					data.Facility.ID = facilityID;
					if (blnFullDetails)
						data.Facility.Name = _GetSimpleData(facilityID, "FACILITIES", "FACILITY_ID", "FACILITY_NAME").Name;
				}
				result.Add(data);
			});
			
			//done.
			return result.ToArray();
		}

		private CompetitionData[] _GetCompetitionData(DateTime start, DateTime end, bool blnFullDetails,
			int region, int sport, int championship, int facility)
		{
			return _GetCompetitionData(start, end, blnFullDetails, region, sport, championship, facility, -1);
		}

		private CompetitionData[] _GetCompetitionData(DateTime start, DateTime end, bool blnFullDetails,
			int region, int sport, int championship)
		{
			return _GetCompetitionData(start, end, blnFullDetails, region, sport, championship, -1);
		}
		#endregion
		#endregion
		#endregion
	}

	#region data classes
	public class TeamPlayerNumbers
	{
		private int teamId = -1;
		private int[] playerNumbers = null;

		public int TeamId
		{
			get { return teamId; }
			set { teamId = value; }
		}

		public int[] PlayerNumbers
		{
			get { return playerNumbers; }
			set { playerNumbers = value; }
		}

		public TeamPlayerNumbers()
		{
		}

		public TeamPlayerNumbers(int teamId)
			: this()
		{
			this.TeamId = teamId;
		}

		public TeamPlayerNumbers(int teamId, int[] numbers)
			: this(teamId)
		{
			this.PlayerNumbers = numbers;
		}
	}
	#endregion
}
