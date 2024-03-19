using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Sport.Common;
using System.Linq;
using System.Diagnostics;
using System.IO;
using Mir.Common;
using System.Web;

namespace ISF.DataLayer
{
	public class DB
	{
		private static DB instance = null;
		public static DB Instance { get { return instance; } }

		private string logSection = "DB";
		public static string loggerFolder = "C:\\Logger\\DB";
		private readonly int ConnectionTimeoutMinutes = 5;

		public readonly int GlobalConnectionsPoolSize = 20;
		private object selectLocker = new object();
		private List<SqlConnection> globalSelectConnections = new List<SqlConnection>();
		public Dictionary<int, bool> GlobalConnectionsBusyState = new Dictionary<int, bool>();
		public Dictionary<int, long> GlobalConnectionsResetCount = new Dictionary<int, long>();
		public Dictionary<int, DateTime> GlobalConnectionLastAccess = new Dictionary<int, DateTime>();
		public Dictionary<string, int> UserAgents = new Dictionary<string, int>();

		private int selectConnectionCurrentIndex = 0;

		//public Dictionary<string, int> SqlQueries = new Dictionary<string, int>();
		private DB()
		{
			/*
			string moduleName = "";
			try
			{
				moduleName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
			}
			catch
			{ }
			if (!string.IsNullOrEmpty(moduleName))
				logSection += " - " + moduleName;
			*/

			Logger.Instance.WriteLog(LogType.Information, logSection, "Initializing pool with " + GlobalConnectionsPoolSize + " connections");
			for (int i = 0; i < GlobalConnectionsPoolSize; i++)
			{
				ResetGlobalSelectionConnection(i, "create new connection");
			}

			string filePath = Path.Combine(loggerFolder, "UserAgents.txt");
			try
			{
				if (!File.Exists(filePath))
					File.WriteAllText(filePath, "");
				string[] lines = File.ReadAllLines(filePath);
				Logger.Instance.WriteLog(LogType.Information, logSection, "Found " + lines.Length + " lines in user agents file");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (line.Length > 0 && !UserAgents.ContainsKey(line))
						UserAgents.Add(line, i + 1);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteLog(LogType.Warning, logSection, "Error reading user agents file: " + ex.ToString());
			}

			/*
			filePath = Path.Combine(loggerFolder, "Queries.txt");
			try
			{
				if (!File.Exists(filePath))
					File.WriteAllText(filePath, "");
				string[] lines = File.ReadAllLines(filePath);
				Logger.Instance.WriteLog(LogType.Information, logSection, "Found " + lines.Length + " lines in queries file");
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (line.Length > 0 && !SqlQueries.ContainsKey(line))
						SqlQueries.Add(line, i + 1);
				}
			}
			catch (Exception ex)
			{
				Logger.Instance.WriteLog(LogType.Warning, logSection, "Error reading queries file: " + ex.ToString());
			}
			*/

			Logger.Instance.WriteLog(LogType.Debug, logSection, "Initialize done.", 30);
		}

		static DB()
		{
			string strLoggerFilePath = Path.Combine(loggerFolder, "DataLayer.log");
			try
			{
				if (!Directory.Exists(loggerFolder))
					Directory.CreateDirectory(loggerFolder);
				TextFileLogWriter logWriter = new TextFileLogWriter(strLoggerFilePath, false, true);
				logWriter.UseHourlyLog = true;
				Logger.Instance.AddLogWriter(logWriter);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to init logger: " + ex.ToString());
			}

			instance = new DB();
		}

		private string GetUserAgent()
		{
			string userAgent = "";
			lock (this)
			{
				if (HttpContext.Current != null && HttpContext.Current.Request != null)
				{
					userAgent = HttpContext.Current.Request.UserAgent;
					if (!string.IsNullOrEmpty(userAgent))
					{
						if (!UserAgents.ContainsKey(userAgent))
						{
							string filePath = Path.Combine(loggerFolder, "UserAgents.txt");
							bool success = false;
							try
							{
								File.AppendAllLines(filePath, new string[] { userAgent });
								success = true;
							}
							catch
							{
								success = false;
							}
							if (success)
							{
								int count = UserAgents.Count;
								UserAgents.Add(userAgent, count + 1);
								userAgent = "U #" + UserAgents[userAgent];
							}
						}
						else
						{
							userAgent = "U #" + UserAgents[userAgent];
						}
					}
				}
			}
			return userAgent;
		}

		/*
		private string GetQueryIndex(string sql)
		{
			string queryIndex = sql;
			lock (this)
			{
				if (!SqlQueries.ContainsKey(sql))
				{
					string filePath = Path.Combine(loggerFolder, "Queries.txt");
					bool success = false;
					try
					{
						File.AppendAllLines(filePath, new string[] { sql });
						success = true;
					}
					catch
					{
						success = false;
					}
					if (success)
					{
						int count = SqlQueries.Count;
						SqlQueries.Add(sql, count + 1);
						queryIndex = "S #" + SqlQueries[sql];
					}
				}
				else
				{
					queryIndex = "S #" + SqlQueries[sql];
				}
			}
			return queryIndex;
		}
		*/

		private void ResetGlobalSelectionConnection(int index, string reason, bool addToLog)
		{
			lock (this)
			{
				if (addToLog)
				{
					string logMessage = string.Format("Reset connection #{0} called", index + 1);
					if (reason.Length > 0)
						logMessage += ", reason: " + reason;
					Logger.Instance.WriteLog(LogType.Debug, logSection, logMessage, 30);
				}
				if (index < globalSelectConnections.Count && globalSelectConnections[index] != null)
				{
					//Logger.Instance.WriteLog(LogType.Debug, logSection, "Closing and disposing connection #" + (index + 1), 30);
					try
					{
						globalSelectConnections[index].Close();
					}
					catch
					{
					}
					try
					{
						globalSelectConnections[index].Dispose();
					}
					catch
					{
					}
				}

				while (index >= globalSelectConnections.Count)
				{
					Logger.Instance.WriteLog(LogType.Debug, logSection, "Adding new connection to pool...", 30);
					globalSelectConnections.Add(new SqlConnection(this.ConnectionString));
				}

				if (addToLog)
					Logger.Instance.WriteLog(LogType.Information, logSection, "Opening connection #" + (index + 1));

				globalSelectConnections[index] = new SqlConnection(this.ConnectionString);
				globalSelectConnections[index].Open();

				if (addToLog)
					Logger.Instance.WriteLog(LogType.Debug, logSection, "Reset is done, connection #" + (index + 1) + " is open and ready", 30);

				if (!GlobalConnectionsBusyState.ContainsKey(index))
					GlobalConnectionsBusyState.Add(index, false);
				if (!GlobalConnectionsResetCount.ContainsKey(index))
					GlobalConnectionsResetCount.Add(index, 0);
				if (!GlobalConnectionLastAccess.ContainsKey(index))
					GlobalConnectionLastAccess.Add(index, DateTime.MinValue);

				GlobalConnectionsBusyState[index] = false;
				GlobalConnectionsResetCount[index]++;
				GlobalConnectionLastAccess[index] = DateTime.MinValue;
			}
		}

		private void ResetGlobalSelectionConnection(int index, string reason)
		{
			ResetGlobalSelectionConnection(index, reason, true);
		}

		public SqlConnection GetGlobalSelectionConnection(ref int index)
		{
			/*
			for (int i = 0; i < globalSelectConnections.Count; i++)
			{
				if (GlobalConnectionsBusyState[i] == true && 
					GlobalConnectionLastAccess[i].Year > 1900 &&
					(DateTime.Now - GlobalConnectionLastAccess[i]).TotalMinutes >= ConnectionTimeoutMinutes)
				{
					ResetGlobalSelectionConnection(i, "busy more than " + ConnectionTimeoutMinutes + " minutes");
				}
			}
			*/

			/*
			lock (this)
			{
				for (int i = 0; i < globalSelectConnections.Count; i++)
				{
					if (GlobalConnectionsBusyState[i] == false)
					{
						connection = globalSelectConnections[i];
						index = i;
						break;
					}
				}
			}
			*/

			/*
			if (index >= 0)
			{
				if (GlobalConnectionLastAccess[index].Year > 1900 &&
					(DateTime.Now - GlobalConnectionLastAccess[index]).TotalMinutes >= 10)
				{
					ResetGlobalSelectionConnection(index, "routine cleanup", false);
					connection = globalSelectConnections[index];
				}
			}
			*/

			/*
			if (connection == null)
			{
				Logger.Instance.WriteLog(LogType.Warning, logSection, "All connections are busy, working with new connection");
				index = -1;
				return GetOpenConnection();
			}

			if (connection.State != ConnectionState.Open)
			{
				ResetGlobalSelectionConnection(index, "Broken or invalid connection (" + connection.State + ")");
				connection = globalSelectConnections[index];
			}
			*/

			index = selectConnectionCurrentIndex;
			SqlConnection connection = globalSelectConnections[selectConnectionCurrentIndex];
			lock (this)
			{
				GlobalConnectionsBusyState[index] = true;
				GlobalConnectionLastAccess[index] = DateTime.Now;
				selectConnectionCurrentIndex++;
				if (selectConnectionCurrentIndex >= GlobalConnectionsPoolSize)
					selectConnectionCurrentIndex = 0;
			}

			//Logger.Instance.WriteLog(LogType.Debug, logSection, "Connection #" + (index + 1) + " available", 30);
			return connection;

		}
		
		private string connectionString = null;
		public string ConnectionString
		{
			get
			{
				if (connectionString == null)
					connectionString = ConfigurationManager.AppSettings["ConnectionString"] + "";
				return connectionString;
			}
		}

		public SqlConnection GetOpenConnection()
		{
			SqlConnection connection = new SqlConnection(this.ConnectionString);
			connection.Open();
			return connection;
		}

		public SimpleTable GetDataBySQL(string strSQL, params SimpleParameter[] parameters)
		{
			SimpleTable table = SimpleTable.Empty;
			lock (selectLocker)
			{
				/* string strIP = "";
				if (HttpContext.Current != null && HttpContext.Current.Request != null)
					strIP = HttpContext.Current.Request.UserHostAddress + ""; */

				/* bool local = false;
				if (strIP == "213.8.193.147" || strIP == "127.0.0.1" || strIP == "localhost")
				{
					strIP = "L";
					local = true;
				} */

				//string userAgent = local ? "" : GetUserAgent();
				int index = 0;
				SqlConnection connection = GetGlobalSelectionConnection(ref index);
				using (SqlCommand command = new SqlCommand(strSQL, connection))
				{
					if (parameters != null)
						Array.ForEach(parameters, p => command.Parameters.AddWithValue(p.Name, p.Value));
					SqlDataReader reader = null;
					//Sport.Core.Session.User
					/* string sqlText = CommandToString(command);
					//string queryIndex = GetQueryIndex(sqlText);
					string logMessage = string.Format("[C{0}] [{1}]", (index + 1), strIP);
					if (userAgent.Length > 0)
						logMessage += " [" + userAgent + "]";
					logMessage += " [" + sqlText + "]"; //" [" + queryIndex + "]";
					Logger.Instance.WriteLog(LogType.Debug, logSection, logMessage, 30); */
					try
					{
						reader = command.ExecuteReader();
					}
					catch (Exception ex)
					{
						Logger.Instance.WriteLog(LogType.Error, logSection,
							string.Format("Failed executing [{0}]: {1}", CommandToString(command), ex.Message));
						if (index >= 0)
							ResetGlobalSelectionConnection(index, "error executing SQL");
						throw;
					}

					if (reader != null)
					{
						int fieldCount = reader.FieldCount;
						List<string> columns = new List<string>();
						for (int i = 0; i < fieldCount; i++)
							columns.Add(reader.GetName(i));
						table = new SimpleTable(columns.ToArray());
						while (reader.Read())
						{
							object[] values = new object[columns.Count];
							for (int i = 0; i < values.Length; i++)
								values[i] = reader.IsDBNull(i) ? GetDefaultValue(reader.GetFieldType(i)) : reader[i];
							table.AddRow(values);
						}
						reader.Close();
						reader.Dispose();
						//Logger.Instance.WriteLog(LogType.Debug, section, fieldCount + " fields, " + table.Rows.Count + " rows", 30);
					}
				}

				if (index >= 0)
					GlobalConnectionsBusyState[index] = false;

				/*
				if (index < 0)
				{
					Logger.Instance.WriteLog(LogType.Debug, logSection, "Manually closing select connection", 30);
					try
					{
						connection.Close();
						connection.Dispose();
					}
					catch
					{ }
				}
				*/
			}
			return table;
		}

		public bool GetSingleRow(string strSQL, out SimpleRow row, params SimpleParameter[] parameters)
		{
			row = SimpleRow.Empty;
			SimpleTable table = this.GetDataBySQL(strSQL, parameters);
			if (table.Rows.Count > 0)
			{
				row = table.Rows[0];
				return true;
			}
			return false;
		}

		public int GetMaxValue(string table, string field, string where)
		{
			string strSQL = string.Format("SELECT MAX({0}) FROM {1}", field, table);
			if (!string.IsNullOrEmpty(where))
				strSQL += " WHERE " + where;
			return Int32.Parse(this.ExecuteScalar(strSQL, -1).ToString());
		}

		public int GetMaxValue(string table, string field)
		{
			return GetMaxValue(table, field, null);
		}

		public object ExecuteScalar(string strSQL, object defValue, params SimpleParameter[] parameters)
		{
			if (defValue is SimpleParameter)
				throw new Exception("ExecuteScalar got SimpleParameter as default value");
			SimpleTable table = this.GetDataBySQL(strSQL, parameters);
			return (table.Rows.Count > 0) ? table.Rows[0][0] : defValue;
		}

		public int Execute(string strSQL, params SimpleParameter[] parameters)
		{
			int rowsAffected = 0;
			using (SqlConnection connection = new SqlConnection(this.ConnectionString))
			{
				connection.Open();
				SqlCommand command = new SqlCommand(strSQL, connection);
				HandleNullableParameters(ref strSQL, ref command, parameters);
				try
				{
					rowsAffected = command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					Logger.Instance.WriteLog(LogType.Error, logSection,
						string.Format("Failed to execute [{0}]: {1}", CommandToString(command), ex.Message));
					throw;
				}
				finally
				{
					connection.Close();
				}
				command.Dispose();
			}
			return rowsAffected;
		}

		private void HandleNullableParameters(ref string strSQL, ref SqlCommand command,
			SimpleParameter[] parameters)
		{
			if (parameters == null || parameters.Length == 0)
				return;

			int whereIndex = strSQL.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase);
			for (int i = 0; i < parameters.Length; i++)
			{
				SimpleParameter p = parameters[i];
				command.Parameters.AddWithValue(p.Name, p.Value == null ? DBNull.Value : p.Value);
				if (whereIndex > 0 && (p.Value is Int32) && ((int)p.Value == -1))
				{
					strSQL = command.CommandText.Replace(" = ", "=").Replace(" =", "=").Replace("= ", "=");
					string toLookFor = "=" + p.Name;
					int index = strSQL.IndexOf(toLookFor);
					while (index > 0 && ((index + toLookFor.Length) < (strSQL.Length - 1)) && IsValidFieldCharacter(strSQL[index + toLookFor.Length]))
						index = strSQL.IndexOf(toLookFor, index + 1);
					if (index > whereIndex)
					{
						index--;
						string fieldName = "";
						while (index > 0 && IsValidFieldCharacter(strSQL[index]))
						{
							fieldName = strSQL[index] + fieldName;
							index--;
						}
						if (fieldName.Length > 0)
						{
							string s1 = string.Format("{0}={1}", fieldName, p.Name);
							strSQL = strSQL.Replace(s1, "(" + s1 + " OR " + fieldName + " IS NULL)");
							command.CommandText = strSQL;
							whereIndex = strSQL.IndexOf("WHERE", StringComparison.CurrentCultureIgnoreCase);
						}
					}
				}
			}
		}

		private bool IsValidFieldCharacter(char c)
		{
			return (char.IsLetter(c) || c == '_');
		}

		public SimpleData[] GetRegionsData()
		{
			string strSQL = "SELECT REGION_ID, REGION_NAME FROM REGIONS WHERE DATE_DELETED IS NULL ORDER BY REGION_ID ASC";
			SimpleTable table = GetDataBySQL(strSQL);
			List<SimpleData> regions = new List<SimpleData>();
			table.Rows.ForEach(row =>
			{
				string name = row["REGION_NAME"].ToString();
				if (name.Length > 0)
					regions.Add(new SimpleData { ID = (int)row["REGION_ID"], Name = name });
			});

			return regions.ToArray();
		}

		public int GetSchoolId(string symbol)
		{
			string strSQL = "SELECT SCHOOL_ID " +
						"FROM SCHOOLS " +
						"WHERE SYMBOL=@1 AND DATE_DELETED IS NULL";
			SimpleRow row;
			int schoolID = -1;
			if (GetSingleRow(strSQL, out row, new SimpleParameter("@1", symbol)))
			{
				schoolID = (int)row["SCHOOL_ID"];
			}
			return schoolID;
		}

		public Dictionary<string, SimpleData> GetClubDetails(int schoolID)
		{
			string strSQL = "SELECT REGION_ID " + 
						"FROM SCHOOLS " +
						"WHERE SCHOOL_ID=@1 AND DATE_DELETED IS NULL";
			SimpleRow row;
			int regionID = -1;
			if (GetSingleRow(strSQL, out row, new SimpleParameter("@1", schoolID)))
			{
				regionID = (int)row["REGION_ID"];
			}

			Dictionary<string, SimpleData> clubDetails = new Dictionary<string, SimpleData>();
			clubDetails.Add("region", new SimpleData { ID = regionID, Name = "" });
			return clubDetails;
		}

		public SimpleData GetRegionData(int regionID)
		{
			return this.GetRegionsData().ToList().DefaultIfEmpty(new SimpleData { ID = -1, Name = string.Empty }).FirstOrDefault(r => r.ID.Equals(regionID));
		}

		public StudentData GetStudentByNumber(string id_number)
		{
			string strSQL = "SELECT STUDENT_ID, FIRST_NAME, LAST_NAME, BIRTH_DATE, SCHOOL_ID, GRADE FROM STUDENTS WHERE ID_NUMBER=@1 AND DATE_DELETED IS NULL";
			SimpleRow row;
			if (GetSingleRow(strSQL, out row, new SimpleParameter("@1", id_number)))
			{
				return new StudentData
				{
					IdNumber = id_number,
					ID = (int)row["STUDENT_ID"],
					Birthdate = (DateTime)row["BIRTH_DATE"],
					FirstName = row["FIRST_NAME"].ToString(),
					LastName = row["LAST_NAME"].ToString(),
					Grade = (int)row["GRADE"]
				};
			}
			return new StudentData { IdNumber = string.Empty, ID = -1, FirstName = string.Empty, LastName = string.Empty, Grade = 0 };
		}

		public string UpdateSchoolDetails(int schoolID, string schoolManager, string schoolPhone, string schoolFax, string schoolEmail)
		{
			string strSQL = "UPDATE SCHOOLS SET MANAGER_NAME=@manager, PHONE=@phone, FAX=@fax, EMAIL=@email WHERE SCHOOL_ID=@school";
			SimpleParameter[] parameters = new SimpleParameter[] { new SimpleParameter { Name = "@school", Value = schoolID }, 
				new SimpleParameter { Name = "@manager", Value = schoolManager }, 
				new SimpleParameter { Name = "@phone", Value = schoolPhone }, 
				new SimpleParameter { Name = "@fax", Value = schoolFax }, 
				new SimpleParameter { Name = "@email", Value = schoolEmail } };
			string strError = string.Empty;
			int rowsAffected = 0;
			try
			{
				rowsAffected = this.Execute(strSQL, parameters);
			}
			catch (Exception ex)
			{
				strError = "general error: " + ex.Message;
			}

			//	if (rowsAffected != 1 && strError.Length == 0)
			//		strError = "Affected " + rowsAffected + " rows instead of 1";

			return strError;
		}

		public int[] GetClubCategoriesBySeason(int school, int season)
		{
			string strSQL = "SELECT cc.CHAMPIONSHIP_CATEGORY_ID " +
					"FROM (CHAMPIONSHIP_CATEGORIES cc INNER JOIN CHAMPIONSHIPS c ON cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID) " +
					"INNER JOIN TEAMS t ON cc.CHAMPIONSHIP_CATEGORY_ID=t.CHAMPIONSHIP_CATEGORY_ID " +
					"WHERE c.SEASON=@season AND t.SCHOOL_ID=@school AND c.IS_CLUBS=1 " +
					"AND c.DATE_DELETED IS NULL AND cc.DATE_DELETED IS NULL AND t.DATE_DELETED IS NULL";
			SimpleTable table = GetDataBySQL(strSQL, 
				new SimpleParameter("@season", season),  
				new SimpleParameter("@school", school));
			return table.Rows.ConvertAll(r => (int)r[0]).ToArray();
		}

		public int[] GetChampionshipCategoriesBySeason(int season)
		{
			string strSQL = "SELECT DISTINCT cc.CHAMPIONSHIP_CATEGORY_ID FROM CHAMPIONSHIPS c, CHAMPIONSHIP_CATEGORIES cc " + 
				"WHERE cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID AND c.SEASON=@1 AND c.DATE_DELETED IS NULL AND cc.DATE_DELETED IS NULL";
			SimpleTable table = GetDataBySQL(strSQL, new SimpleParameter("@1", season));
			return table.Rows.ConvertAll(r => (int)r[0]).ToArray();
		}

		public int GetChampionshipCurrentPhase(int champCategoryID)
		{
			string strSQL = "SELECT PHASE FROM CHAMPIONSHIP_PHASES WHERE CHAMPIONSHIP_CATEGORY_ID=@id AND STATUS=1 AND DATE_DELETED IS NULL";
			return (int)ExecuteScalar(strSQL, -1, new SimpleParameter("@id", champCategoryID));
		}

		public MatchData[] GetMatchesByTeam(int team)
		{
			return GetMatchData(DateTime.MinValue, DateTime.MinValue, -1, -1, team, false, -1, -1, -1, -1);
		}

		public MatchData[] GetMatchesByFacility(int facility)
		{
			return GetMatchData(DateTime.MinValue, DateTime.MinValue, facility, -1, -1, false, -1, -1, -1, -1);
		}

		public MatchData[] GetMatchesByCourt(int court)
		{
			return GetMatchData(DateTime.MinValue, DateTime.MinValue, -1, court, -1, false, -1, -1, -1, -1);
		}

		private MatchData[] GetMatchData(DateTime start, DateTime end, int facility_id, int court_id, int team_id, 
			bool blnFullDetails, int region, int sport,	int championship, int category)
		{
			//read supervisors:
			string strSQL = "SELECT cmf.CHAMPIONSHIP_CATEGORY_ID, cmf.PHASE, cmf.NGROUP, cmf.ROUND, cmf.CYCLE, cmf.MATCH, " + 
				"f.FUNCTIONARY_NAME, f.FUNCTIONARY_TYPE, f.CELL_PHONE " +
				"FROM CHAMPIONSHIP_MATCH_FUNCTIONARIES cmf INNER JOIN FUNCTIONARIES f ON cmf.FUNCTIONARY_ID=f.FUNCTIONARY_ID";
			SimpleTable table = GetDataBySQL(strSQL);
			Dictionary<string, List<FunctionaryData>> functionaries = new Dictionary<string, List<FunctionaryData>>();
			string funcKeyFormat = "{0}_{1}_{2}_{3}_{4}_{5}";
			table.Rows.ForEach(row => {
				string funcKey = string.Format(funcKeyFormat, 
				row["CHAMPIONSHIP_CATEGORY_ID"], row["PHASE"], row["NGROUP"], row["ROUND"], row["CYCLE"], row["MATCH"]);
				if (!functionaries.ContainsKey(funcKey))
					functionaries.Add(funcKey, new List<FunctionaryData>());
				functionaries[funcKey].Add(new FunctionaryData { Name = row["FUNCTIONARY_NAME"].ToString(), Type = (int)row["FUNCTIONARY_TYPE"], Phone = row["CELL_PHONE"].ToString() });
			});

			List<string> filters = new List<string>();
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			AddFilterIfValid(start, "@start", "m.TIME", ">=", ref filters, ref parameters);
			AddFilterIfValid(end, "@end", "m.TIME", "<=", ref filters, ref parameters);
			AddFilterIfValid(facility_id, "@facility", "m.FACILITY_ID", "=", ref filters, ref parameters);
			AddFilterIfValid(court_id, "@court", "m.COURT_ID", "=", ref filters, ref parameters);
			AddFilterIfValid(team_id, "@team", new string[] { "t1.TEAM_ID", "t2.TEAM_ID" }, "=", ref filters, ref parameters);
			AddFilterIfValid(region, "@region", "c.REGION_ID", "=", ref filters, ref parameters);
			AddFilterIfValid(sport, "@sport", "c.SPORT_ID", "=", ref filters, ref parameters);
			AddFilterIfValid(championship, "@championship", "c.CHAMPIONSHIP_ID", "=", ref filters, ref parameters);
			AddFilterIfValid(category, "@category", "cc.CHAMPIONSHIP_CATEGORY_ID", "=", ref filters, ref parameters);

			//build select statement:
			strSQL = "SELECT m.CHAMPIONSHIP_CATEGORY_ID, m.[TIME], m.TEAM_A_SCORE, m.TEAM_B_SCORE, m.[RESULT], m.PHASE, m.[ROUND], " + 
				"m.NGROUP, m.MATCH, m.CYCLE, m.FACILITY_ID, m.COURT_ID, c.CHAMPIONSHIP_ID, p.PHASE_NAME, g.GROUP_NAME, r.ROUND_NAME, " + 
				"t1.TEAM_ID AS TEAM_A, t2.TEAM_ID AS TEAM_B, t1.PREVIOUS_GROUP AS PREV_GROUP1, t2.PREVIOUS_GROUP AS PREV_GROUP2, " + 
				"t1.PREVIOUS_POSITION AS PREV_POS1, t2.PREVIOUS_POSITION AS PREV_POS2 " + 
				"FROM ((((((CHAMPIONSHIP_MATCHES m LEFT JOIN CHAMPIONSHIP_CATEGORIES cc ON m.CHAMPIONSHIP_CATEGORY_ID=cc.CHAMPIONSHIP_CATEGORY_ID) " + 
				"LEFT JOIN CHAMPIONSHIPS c ON cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID) " + 
				"LEFT JOIN CHAMPIONSHIP_PHASES p ON m.CHAMPIONSHIP_CATEGORY_ID=p.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=p.PHASE) " + 
				"LEFT JOIN CHAMPIONSHIP_GROUPS g ON m.CHAMPIONSHIP_CATEGORY_ID=g.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=g.PHASE AND m.NGROUP=g.NGROUP) " + 
				"LEFT JOIN CHAMPIONSHIP_ROUNDS r ON m.CHAMPIONSHIP_CATEGORY_ID=r.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=r.PHASE AND m.NGROUP=r.NGROUP AND m.ROUND=r.ROUND) " + 
				"LEFT JOIN CHAMPIONSHIP_GROUP_TEAMS t1 ON m.CHAMPIONSHIP_CATEGORY_ID=t1.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=t1.PHASE AND m.NGROUP=t1.NGROUP AND m.TEAM_A=t1.POSITION) " + 
				"LEFT JOIN CHAMPIONSHIP_GROUP_TEAMS t2 ON m.CHAMPIONSHIP_CATEGORY_ID=t2.CHAMPIONSHIP_CATEGORY_ID AND m.PHASE=t2.PHASE AND m.NGROUP=t2.NGROUP AND m.TEAM_B=t2.POSITION " + 
				"WHERE m.DATE_DELETED IS NULL AND c.DATE_DELETED IS NULL";
			if (filters.Count > 0)
				strSQL += " AND " + string.Join(" AND ", filters.ToArray());

			//System.Collections.Hashtable teams = new Hashtable();
			List<MatchData> matches = new List<MatchData>();
			table = GetDataBySQL(strSQL, parameters.ToArray());
			table.Rows.ForEach(row =>
			{
				MatchData match = new MatchData();
				match.Championship = new ChampionshipData { ID = (int)row["CHAMPIONSHIP_ID"] };
				match.CategoryID = (int)row["CHAMPIONSHIP_CATEGORY_ID"];
				match.MatchIndex = (int)row["MATCH"];
				match.Court = new CourtData { ID = (int)row["COURT_ID"] };
				match.Facility = new FacilityData { ID = (int)row["FACILITY_ID"] };
				match.Cycle = row.GetIntOrDefault("CYCLE", -1);
				match.Result = (int)row["RESULT"];
				match.Round = new RoundData { RoundIndex = row.GetIntOrDefault("ROUND", -1), RoundName = row["ROUND_NAME"].ToString() };
				match.Round.Group = new GroupData { GroupIndex = row.GetIntOrDefault("NGROUP", -1), GroupName = row["GROUP_NAME"].ToString() };
				match.Round.Group.Phase = new PhaseData { PhaseIndex = row.GetIntOrDefault("PHASE", -1), PhaseName = row["PHASE_NAME"].ToString() };
				match.TeamA_Score = Double.Parse(row["TEAM_A_SCORE"].ToString());
				if (match.TeamA_Score < 0)
					match.TeamA_Score = 0;
				match.TeamB_Score = Double.Parse(row["TEAM_B_SCORE"].ToString());
				if (match.TeamB_Score < 0)
					match.TeamB_Score = 0;
				match.Time = (DateTime)row["TIME"];
				match.TeamA = new TeamData { ID = (int)row["TEAM_A"] };
				match.TeamB = new TeamData { ID = (int)row["TEAM_B"] };
				string funcKey = string.Format(funcKeyFormat, 
					match.CategoryID, match.Round.Group.Phase.PhaseIndex, match.Round.Group.GroupIndex, match.Round.RoundIndex, match.Cycle, match.MatchIndex);
				if (functionaries.ContainsKey(funcKey))
				{
					List<FunctionaryData> matchFuncs = functionaries[funcKey];
					List<FunctionaryData> coordinators = matchFuncs.FindAll(f => f.Type.Equals((int)Sport.Types.FunctionaryType.Coordinator));
					List<FunctionaryData> referees = matchFuncs.FindAll(f => f.Type.Equals((int)Sport.Types.FunctionaryType.Referee));
					match.Supervisor = string.Join(",", coordinators.ConvertAll(f => f.ToString()).ToArray());
					match.Referee = string.Join(",", referees.ConvertAll(f => f.ToString()).ToArray());
				}
				matches.Add(match);
			});

			//done.
			return matches.ToArray();
		}

		private void AddFilterIfValid(object value, string paramName, string[] fieldNames, string strOperator, 
			ref List<string> filters, ref List<SimpleParameter> parameters)
		{
			if ((value is int && ((int)value) >= 0) || (value is DateTime && ((DateTime)value).Year > 1900))
			{
				string filter = string.Empty;
				if (fieldNames.Length > 1)
					filter += "(";
				filter += string.Join(" OR ", fieldNames.ToList().ConvertAll(fieldName =>
					string.Format("{0} {1} {2}", fieldName, strOperator, paramName)).ToArray());
				if (fieldNames.Length > 1)
					filter += ")";
				filters.Add(filter);
				parameters.Add(new SimpleParameter { Name = paramName, Value = value });
			}
		}

		private void AddFilterIfValid(object value, string paramName, string fieldName, string strOperator,
			ref List<string> filters, ref List<SimpleParameter> parameters)
		{
			AddFilterIfValid(value, paramName, new string[] { fieldName }, strOperator, ref filters, ref parameters);
		}

		private object GetDefaultValue(Type type)
		{
			if (type.Equals(typeof(string)))
				return "";
			if (type.Equals(typeof(DateTime)))
				return DateTime.MinValue;
			if (type.Equals(typeof(int)) || type.Equals(typeof(long)) || type.Equals(typeof(double)) || type.Equals(typeof(float)) || type.Equals(typeof(short)))
				return -1;
			return null;
		}

		public string CommandToString(SqlCommand command)
		{
			string text = command.CommandText;
			if (command.Parameters != null && command.Parameters.Count > 0)
			{
				text += " [" + string.Join(", ", command.Parameters.OfType<SqlParameter>().ToList().ConvertAll(p => 
					string.Format("{0}={1}", p.ParameterName, p.Value))) + "]";
			}
			return text;
		}
	}
}
