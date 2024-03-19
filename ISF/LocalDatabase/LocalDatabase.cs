using System;
using System.Data.OleDb;
using System.Collections;
using Sport.Core;
using Sport.Common;

namespace LocalDatabaseManager
{
	#region local structures
	#region General Structures
	public enum FieldType
	{
		None=-1,
		Text=0,
		Integer=1,
		DateTime=2,
		Identity=3,
		Double=5
	}

	public enum ConditionType
	{
		Equals=0,
		LessThen,
		GreaterThan
	}

	public enum OrderType
	{
		Ascending=0,
		Descending
	}

	public enum ConditionLogic
	{
		None=-1,
		And=0,
		Or
	}

	public struct OrderField
	{
		public string Field;
		public OrderType Order;
	}
	#endregion

	#region Condition
	public struct Condition
	{
		public string Field;
		public ConditionType Type;
		public object Value;

		public static Condition Empty;

		public Condition(string field, ConditionType type, object value)
		{
			this.Field = field;
			this.Type = type;
			this.Value = value;
		}

		static Condition()
		{
			Empty = new Condition("", ConditionType.Equals, null);
		}
	}
	#endregion
	
	#region Field class
	public class Field
	{
		public FieldType Type;
		public string Name;
		public object Value;

		public Field()
		{
			Type = FieldType.None;
			Name = "";
			Value = null;
		}

		public Field(FieldType type, string name)
			: this()
		{
			this.Type = type;
			this.Name = name;
		}

		public Field(string name, object value)
			: this()
		{
			this.Name = name;
			this.Value = value;
		}
		
		public Field(FieldType type, string name, object value)
			: this(type, name)
		{
			this.Value = value;
		}

		public override string ToString()
		{
			return Name+" ("+Type.ToString()+")";
		}

		public override bool Equals(object obj)
		{
			return base.Equals (obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
	}
	#endregion

	#region Condition Collection
	public class ConditionCollection
	{
		private ArrayList _conditions;
		private ArrayList _conditionsLogic;
		private int _index;
		
		public ConditionCollection()
		{
			_conditions = new ArrayList();
			_conditionsLogic = new ArrayList();
			_index = 0;
		}

		public ConditionCollection(Condition condition, ConditionLogic logic)
			: this()
		{
			_conditions.Add(condition);
			_conditionsLogic.Add(logic);
		}

		public ConditionCollection(Condition[] conditions, ConditionLogic[] logics)
			: this()
		{
			if (conditions.Length != logics.Length)
				throw new ArgumentException("Error creating Condition Collection: must have the same amount of conditions and logics");
			
			for (int i=0; i<conditions.Length; i++)
				Add(conditions[i], logics[i]);
		}

		public void Add(Condition condition, ConditionLogic logic)
		{
			_conditions.Add(condition);
			_conditionsLogic.Add(logic);
		}

		public int Count
		{
			get {return _conditions.Count;}
		}

		public void MoveFirst()
		{
			_index = 0;
		}

		public bool MoveNext()
		{
			_index++;
			return (_index >= _conditions.Count);
		}

		public bool EOF
		{
			get {return _index >= _conditions.Count;}
		}
		
		public Condition CurrentCondition
		{
			get
			{
				if ((_index >= 0)&&(_index < _conditions.Count))
					return (Condition) _conditions[_index];
				return Condition.Empty;
			}
		}
		
		public ConditionLogic CurrentConditionLogic
		{
			get
			{
				if ((_index >= 0)&&(_index < _conditionsLogic.Count))
					return (ConditionLogic) _conditionsLogic[_index];
				return ConditionLogic.None;
			}
		}
	}
	#endregion
	#endregion
	

	
	/// <summary>
	/// Summary description for LocalDatabase.
	/// </summary>
	public class LocalDatabase
	{
		#region general decleration and constructor
		private static readonly string DATABASE_FILE="LocalDatabase.mdb";
		private static string _dbFilePath=null;

		static LocalDatabase()
		{
			string strFolderPath=Sport.Common.Tools.GetApplicationFolder();
			_dbFilePath = strFolderPath+"\\"+DATABASE_FILE;
		}
		
		private static string ConnectionString()
		{
			if (_dbFilePath == null)
			{
				string strFolderPath=Sport.Common.Tools.GetApplicationFolder();
				_dbFilePath = strFolderPath+"\\"+DATABASE_FILE;
			}
			return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+_dbFilePath+";";
		}
		#endregion

		#region General Methods
		#region database related methods
		private static int TryExecuteNonQuery(string strSQL, 
			OleDbParameter[] parameters)
		{
			//build connection using the connection string and open it:
			OleDbConnection connection=new OleDbConnection(ConnectionString());
			connection.Open();
			
			//build the command using the given sql statement:
			OleDbCommand command=new OleDbCommand(strSQL, connection);
			
			//add the parameters:
			if (parameters != null)
			{
				foreach (OleDbParameter parameter in parameters)
					command.Parameters.Add(parameter);
			}
			
			int result=-1;
			try
			{
				result = command.ExecuteNonQuery();
			}
			catch (Exception err)
			{
				connection.Close();
				string strError="Failed to execute sql statement.\nError: "+err.Message;
				strError += "\nSQL: "+GetSql(command);
				throw new Exception(strError);
			}
			connection.Close();
			return result;
		}
		
		private static int TryExecuteNonQuery(string strSQL)
		{
			return TryExecuteNonQuery(strSQL, null);
		}

		private static MirDataReader TryExecuteReader(string strSQL, 
			OleDbParameter[] parameters)
		{
			//build connection using the connection string and open it:
			OleDbConnection connection=new OleDbConnection(ConnectionString());
			connection.Open();
			
			//build the command using the given sql statement:
			OleDbCommand command=new OleDbCommand(strSQL, connection);

			//add the parameters:
			if (parameters != null)
			{
				foreach (OleDbParameter parameter in parameters)
					command.Parameters.Add(parameter);
			}
			
			MirDataReader result=null;
			try
			{
				OleDbDataReader reader=command.ExecuteReader();
				result=new MirDataReader(reader, connection);
			}
			catch (Exception err)
			{
				connection.Close();
				string strError="Failed to execute sql statement.\nError: "+err.Message;
				strError += "\nSQL: "+GetSql(command);
				throw new Exception(strError);
			}
			//connection.Close();
			return result;
		}

		private static string[] FindSqlValues(Field[] fields)
		{
			string[] result=new string[fields.Length];
			for (int i=0; i<fields.Length; i++)
			{
				string currentValue="";
				object value=fields[i].Value;
				string strValue=((value == null)||(value is System.DBNull))?"NULL":
					value.ToString();
				strValue = strValue.Replace("'", "''");
				if ((value == null)||(value is System.DBNull)||
					((value is DateTime)&&(((DateTime) value).Year < 1900)))
				{
					currentValue = "NULL";
				} //end if null or empty
				else
				{
					if ((value is Int32)||(value is Int64)||(value is Double))
					{
						currentValue = strValue;
					} //end if integer
					else
					{
						if (value is DateTime)
						{
							currentValue = "#"+strValue+"#";
						} //end if datetime
						else
						{
							if (value is Boolean)
							{
								bool blnTemp=(bool) value;
								currentValue = (blnTemp)?"1":"0";
							} //end if boolean
							else
							{
								if (value is string)
								{
									currentValue = "'"+strValue+"'";
								} //end if string
								else
								{
									currentValue = "'"+strValue+"'";
								} //end if not familiar type
							} //end if not boolean
						} //end if not datetime
					} //end if not integer
				} //end if not null or emptty

				result[i] = currentValue;
			} //end main loop
			
			return result;
		}
		#endregion

		#region CREATE and DROP
		public static int CreateTable(string tableName, Field[] fields)
		{
			string strSQL="";
			strSQL += "CREATE TABLE ["+tableName+"]";
			if (fields.Length > 0)
			{
				strSQL += " (";
				for (int i=0; i<fields.Length; i++)
				{
					Field field=fields[i];
					strSQL += "["+field.Name+"] ";
					switch (field.Type)
					{
						case FieldType.Text:
							strSQL += "VARCHAR(255)";
							break;
						case FieldType.Integer:
							strSQL += "INTEGER";
							break;
						case FieldType.DateTime:
							strSQL += "DATETIME";
							break;
						case FieldType.Identity:
							strSQL += "INT IDENTITY(1,1)";
							break;
						case FieldType.Double:
							strSQL += "DOUBLE";
							break;
					}
					if (i < (fields.Length-1))
						strSQL += ", ";
				}
				strSQL += ")";
			}
			return TryExecuteNonQuery(strSQL);
		}

		/// <summary>
		/// try to drop table then creates it.
		/// </summary>
		public static int ReCreateTable(string tableName, Field[] fields)
		{
			try
			{
				DropTable(tableName);
			}
			catch
			{}
			return CreateTable(tableName, fields);
		}
		
		public static int DropTable(string tableName)
		{
			string strSQL="";
			strSQL += "DROP TABLE ["+tableName+"]";
			return TryExecuteNonQuery(strSQL);
		}
		#endregion
		
		#region SELECT
		private static MirDataReader GetTableData(string tableName, 
			string[] fieldsToGet, ConditionCollection conditions, 
			OrderField[] orderBy)
		{
			string strSQL="";
			int i;
			
			//SELECT statement
			strSQL += "SELECT ";
			if ((fieldsToGet == null)||(fieldsToGet.Length == 0))
			{
				strSQL += "*";
			}
			else
			{
				for (i=0; i<fieldsToGet.Length; i++)
				{
					strSQL += "["+fieldsToGet[i]+"]";
					if (i < (fieldsToGet.Length-1))
						strSQL += ", ";
				}
			}

			//FROM statement
			strSQL += " FROM ["+tableName+"]";

			//WHERE clause
			OleDbParameter[] parameters=ApplyWhereClause(ref strSQL, conditions);
			
			//ORDER BY statement
			if ((orderBy != null)&&(orderBy.Length > 0))
			{
				strSQL += " ORDER BY ";
				for (i=0; i<orderBy.Length; i++)
				{
					strSQL += "["+orderBy[i].Field+"] ";
					switch (orderBy[i].Order)
					{
						case OrderType.Ascending:
							strSQL += "ASC";
							break;
						case OrderType.Descending:
							strSQL += "DESC";
							break;
					}
					if (i < (orderBy.Length-1))
						strSQL += ", ";
				}
			} //end if there are any order by fields
			
			MirDataReader reader=TryExecuteReader(strSQL, parameters);
			return reader;
		}
		#endregion

		#region UPDATE
		public static int UpdateTableData(string tableName, Field[] updateFields,
			ConditionCollection conditions)
		{
			string strSQL="";
			int i;
			
			//UPDATE statement
			strSQL += "UPDATE ["+tableName+"] SET ";
			OleDbParameter[] valueParams=null; //new OleDbParameter[updateFields.Length];
			valueParams=new OleDbParameter[0];
			string[] values=FindSqlValues(updateFields);
			for (i=0; i<updateFields.Length; i++)
			{
				Field field=updateFields[i];
				string value=values[i];
				strSQL += "["+field.Name+"]="+value;
				if (i < (updateFields.Length-1))
					strSQL += ", ";
				/*
				string paramName="@v"+(i+1).ToString();
				strSQL += "["+field.Name+"]="+paramName;
				object paramValue=field.Value;
				if ((field.Value == null)||
					((field.Value is DateTime)&&(((DateTime) field.Value).Year < 1900))||
					((field.Value is string)&&(field.Value.ToString().Length == 0)))
				{
					if (field.Value is string)
					{
						paramValue = "aaa";
					}
					else
					{
						paramValue = System.DBNull.Value;
					}
				}
				valueParams[i] = new OleDbParameter(paramName, paramValue);
				*/
			}
			
			//WHERE clause
			OleDbParameter[] condParams=ApplyWhereClause(ref strSQL, conditions);
			
			//create parameters:
			OleDbParameter[] parameters=new OleDbParameter[valueParams.Length+condParams.Length];
			int index=0;
			for (i=0; i<valueParams.Length; i++)
			{
				parameters[index] = valueParams[i];
				index++;
			}
			for (i=0; i<condParams.Length; i++)
			{
				parameters[index] = condParams[i];
				index++;
			}
			
			//execute statement:
			int result=TryExecuteNonQuery(strSQL, parameters);
			return result;
		}
		#endregion

		#region DELETE
		public static int DeleteTableData(string tableName, 
			ConditionCollection conditions, bool jumpToFirst)
		{
			string strSQL="";
			strSQL += "DELETE FROM ["+tableName+"]";
			
			//WHERE clause
			OleDbParameter[] condParams=ApplyWhereClause(ref strSQL, conditions);
			
			int result=TryExecuteNonQuery(strSQL, condParams);
			if (jumpToFirst)
				conditions.MoveFirst();

			return result;
		}
		
		public static int DeleteTableData(string tableName, 
			ConditionCollection conditions)
		{
			return DeleteTableData(tableName, conditions, false);
		}
		#endregion

		#region INSERT
		public static int InsertTableData(string tableName, Field[] insertFields)
		{
			string strSQL="";
			int i;
			
			//INSERT INTO statement
			strSQL += "INSERT INTO ["+tableName+"] (";
			for (i=0; i<insertFields.Length; i++)
			{
				Field field=insertFields[i];
				strSQL += "["+field.Name+"]";
				if (i < (insertFields.Length-1))
				{
					strSQL += ", ";
				}
			}
			OleDbParameter[] valueParams=null; //new OleDbParameter[insertFields.Length];
			valueParams = new OleDbParameter[0];
			string[] values=FindSqlValues(insertFields);
			strSQL += ") VALUES (";
			for (i=0; i<insertFields.Length; i++)
			{
				strSQL += values[i];
				if (i < (insertFields.Length-1))
					strSQL += ", ";
				/*
				string paramName="@"+(i+1).ToString();
				Field field=insertFields[i];
				strSQL += paramName;
				object paramValue=field.Value;
				if ((field.Value == null)||
					((field.Value is DateTime)&&(((DateTime) field.Value).Year < 1900))||
					((field.Value is string)&&(field.Value.ToString().Length == 0)))
				{
					paramValue = System.DBNull.Value;
				}
				valueParams[i] = new OleDbParameter(paramName, paramValue);
				*/
			}
			strSQL += ")";
			
			//execute statement:
			int result=TryExecuteNonQuery(strSQL, valueParams);
			return result;
		}
		#endregion

		#region General
		private static OleDbParameter[] ApplyWhereClause(
			ref string strSQL, ConditionCollection conditions)
		{
			OleDbParameter[] parameters=null;
			int i;
			
			//stupid Access throw non relevant error messages. wish I could throw Access to the nearest trash can
			
			if ((conditions != null)&&(conditions.Count > 0))
			{
				parameters = new OleDbParameter[conditions.Count];
				strSQL += " WHERE (";
				i = 1;
				while (!conditions.EOF)
				{
					Condition condition=conditions.CurrentCondition;
					ConditionLogic logic=conditions.CurrentConditionLogic;
					if (logic != ConditionLogic.None)
					{
						switch (logic)
						{
							case ConditionLogic.And:
								strSQL += " AND (";
								break;
							case ConditionLogic.Or:
								strSQL += " OR (";
								break;
						}
					}
					strSQL += "["+condition.Field+"]";
					switch (condition.Type)
					{
						case ConditionType.Equals:
							strSQL += "=";
							break;
						case ConditionType.GreaterThan:
							strSQL += ">";
							break;
						case ConditionType.LessThen:
							strSQL += "<";
							break;
					}
					strSQL += "@"+i.ToString();
					
					//string[] sqlValues=FindSqlValues(new Field[] 
					//	{ new Field(condition.Field, condition.Value) });
					//strSQL += sqlValues[0];
					
					if (logic != ConditionLogic.None)
						strSQL += ")";

					parameters[i-1] = 
						new OleDbParameter("@"+i.ToString(), condition.Value);
					
					conditions.MoveNext();
					i++;
				} //end loop over the conditions
				strSQL += ")";
			} //end if there are any conditions

			return parameters;
		}

		private static string GetSql(OleDbCommand command)
		{
			string result=command.CommandText;
			for (int i=0; i<command.Parameters.Count; i++)
			{
				OleDbParameter param=command.Parameters[i];
				string paramValue="";
				if ((param.Value == null)||(param.Value is System.DBNull))
				{
					paramValue = "[null]";
				}
				else
				{
					paramValue = param.Value.ToString();
				}
				result = result.Replace(param.ParameterName, paramValue);
			}
			return result;
		}

		/// <summary>
		/// returns error message if can't connect, or null on success.
		/// </summary>
		public static string TestConnection()
		{
			//look for file:
			if (System.IO.File.Exists(_dbFilePath) == false)
				return "Database file ["+_dbFilePath+"] not found.";
			
			//drop existing table:
			try
			{
				DropTable("TMP_TEST_CONNECTION");
			}
			catch
			{}

			//try to create:
			try
			{
				CreateTable("TMP_TEST_CONNECTION", 
					new Field[] {new Field(FieldType.Integer, "field1")});
			}
			catch (Exception ex)
			{
				return "Failed to connect with database ["+ex.Message+"]";
			}
			
			//drop existing table:
			try
			{
				DropTable("TMP_TEST_CONNECTION");
			}
			catch
			{}

			return null;
		}
		
		private static int[] GetCategoriesChampionships(ArrayList arrCategories)
		{
			ArrayList result=new ArrayList();
			string strTableName="CATEGORIES";
			string[] fieldsToSelect=new string[] { "CHAMPIONSHIP_ID" };
			foreach (int categoryID in arrCategories)
			{
				ConditionCollection conditions=new ConditionCollection(
					new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
					ConditionLogic.None);
				MirDataReader reader = GetTableData(
					strTableName, fieldsToSelect, conditions, null);
				if (reader.Read())
				{
					int champID=(int) reader["CHAMPIONSHIP_ID"];
					if (result.IndexOf(champID) < 0)
						result.Add(champID);
				}
				reader.Close();
			}
			
			return (int[]) result.ToArray(typeof(int));
		}

		#region Get Group Courts
		private static CourtData[] GetGroupCourts(GroupData group)
		{
			ArrayList result=new ArrayList();

			//matches
			foreach (RoundData round in group.Rounds)
			{
				foreach (MatchData match in round.Matches)
				{
					if ((match.Court != null)&&(match.Court.ID >= 0))
					{
						if (result.IndexOf(match.Court) < 0)
							result.Add(match.Court);
					}
				}
			}

			//competitions
			foreach (CompetitionData competition in group.Competitions)
			{
				if ((competition.Court != null)&&(competition.Court.ID >= 0))
				{
					if (result.IndexOf(competition.Court) < 0)
						result.Add(competition.Court);
				}
				
				//heats:
				foreach (HeatData heat in competition.Heats)
				{
					if ((heat.Court != null)&&(heat.Court.ID >= 0))
					{
						if (result.IndexOf(heat.Court) < 0)
							result.Add(heat.Court);
					}
				}
			}
			
			return (CourtData[]) result.ToArray(typeof(CourtData));
		}

		#region Get Court Championships
		private static int[] GetCourtChampionships(int courtID)
		{
			ArrayList arrCategories=new ArrayList();
			
			//competition:
			CompetitionData[] competitions=GetCourtCompetitions(courtID);
			foreach (CompetitionData competition in competitions)
			{
				if (arrCategories.IndexOf(competition.CategoryID) < 0)
					arrCategories.Add(competition.CategoryID);
			}
			
			//matches
			MatchData[] matches=GetCourtMatches(courtID);
			foreach (MatchData match in matches)
			{
				if (arrCategories.IndexOf(match.CategoryID) < 0)
					arrCategories.Add(match.CategoryID);
			}

			//heats:
			HeatData[] heats=GetCourtHeats(courtID);
			foreach (HeatData heat in heats)
			{
				if (arrCategories.IndexOf(heat.CategoryID) < 0)
					arrCategories.Add(heat.CategoryID);
			}
			
			return GetCategoriesChampionships(arrCategories);
		}

		#region Get Court Competitions
		private static CompetitionData[] GetCourtCompetitions(int courtID)
		{
			ArrayList result=new ArrayList();
			string strTableName="COMPETITIONS";
			string[] fieldsToSelect=new string[] { "CATEGORY_ID", 
						 "PHASE", "NGROUP", "COMPETITION" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("COURT_ID", ConditionType.Equals, courtID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				CompetitionData data=new CompetitionData();
				data.CategoryID = (int) reader["CATEGORY_ID"];
				data.PhaseIndex = (int) reader["PHASE"];
				data.GroupIndex = (int) reader["NGROUP"];
				data.CompetitionIndex = (int) reader["COMPETITION"];
				result.Add(data);
			}
			reader.Close();
			
			return (CompetitionData[]) result.ToArray(typeof(CompetitionData));
		}
		#endregion
		
		#region Get Court Matches
		private static MatchData[] GetCourtMatches(int courtID)
		{
			ArrayList result=new ArrayList();
			string strTableName="MATCHES";
			string[] fieldsToSelect=new string[] { "CATEGORY_ID", 
						 "PHASE", "NGROUP", "MATCH" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("COURT_ID", ConditionType.Equals, courtID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				MatchData data=new MatchData();
				data.CategoryID = (int) reader["CATEGORY_ID"];
				data.PhaseIndex = (int) reader["PHASE"];
				data.GroupIndex = (int) reader["NGROUP"];
				data.MatchIndex = (int) reader["MATCH"];
				result.Add(data);
			}
			reader.Close();
			
			return (MatchData[]) result.ToArray(typeof(MatchData));
		}
		#endregion
		
		#region Get Court Heats
		private static HeatData[] GetCourtHeats(int courtID)
		{
			ArrayList result=new ArrayList();
			string strTableName="HEATS";
			string[] fieldsToSelect=new string[] { "CATEGORY_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("COURT_ID", ConditionType.Equals, courtID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				HeatData data=new HeatData();
				data.CategoryID = (int) reader["CATEGORY_ID"];
				result.Add(data);
			}
			reader.Close();
			
			return (HeatData[]) result.ToArray(typeof(HeatData));
		}
		#endregion
		#endregion
		#endregion

		#region Get Group Facilities
		private static FacilityData[] GetGroupFacilities(GroupData group)
		{
			ArrayList result=new ArrayList();

			//matches
			foreach (RoundData round in group.Rounds)
			{
				foreach (MatchData match in round.Matches)
				{
					if ((match.Facility != null)&&(match.Facility.ID >= 0))
					{
						if (result.IndexOf(match.Facility) < 0)
							result.Add(match.Facility);
					}
				}
			}

			//competitions
			foreach (CompetitionData competition in group.Competitions)
			{
				if ((competition.Facility != null)&&(competition.Facility.ID >= 0))
				{
					if (result.IndexOf(competition.Facility) < 0)
						result.Add(competition.Facility);
				}
				
				//heats:
				foreach (HeatData heat in competition.Heats)
				{
					if ((heat.Facility != null)&&(heat.Facility.ID >= 0))
					{
						if (result.IndexOf(heat.Facility) < 0)
							result.Add(heat.Facility);
					}
				}
			}
			
			return (FacilityData[]) result.ToArray(typeof(FacilityData));
		}
		
		#region Get Facility Championships
		private static int[] GetFacilityChampionships(int facilityID)
		{
			ArrayList arrCategories=new ArrayList();
			
			//competition:
			CompetitionData[] competitions=GetFacilityCompetitions(facilityID);
			foreach (CompetitionData competition in competitions)
			{
				if (arrCategories.IndexOf(competition.CategoryID) < 0)
					arrCategories.Add(competition.CategoryID);
			}
			
			//matches
			MatchData[] matches=GetFacilityMatches(facilityID);
			foreach (MatchData match in matches)
			{
				if (arrCategories.IndexOf(match.CategoryID) < 0)
					arrCategories.Add(match.CategoryID);
			}

			//heats:
			HeatData[] heats=GetFacilityHeats(facilityID);
			foreach (HeatData heat in heats)
			{
				if (arrCategories.IndexOf(heat.CategoryID) < 0)
					arrCategories.Add(heat.CategoryID);
			}
			
			return GetCategoriesChampionships(arrCategories);
		}
		
		#region Get Facility Competitions
		private static CompetitionData[] GetFacilityCompetitions(int facilityID)
		{
			ArrayList result=new ArrayList();
			string strTableName="COMPETITIONS";
			string[] fieldsToSelect=new string[] { "CATEGORY_ID", 
													 "PHASE", "NGROUP", "COMPETITION" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("FACILITY_ID", ConditionType.Equals, facilityID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				CompetitionData data=new CompetitionData();
				data.CategoryID = (int) reader["CATEGORY_ID"];
				data.PhaseIndex = (int) reader["PHASE"];
				data.GroupIndex = (int) reader["NGROUP"];
				data.CompetitionIndex = (int) reader["COMPETITION"];
				result.Add(data);
			}
			reader.Close();
			
			return (CompetitionData[]) result.ToArray(typeof(CompetitionData));
		}
		#endregion
		
		#region Get Facility Matches
		private static MatchData[] GetFacilityMatches(int facilityID)
		{
			ArrayList result=new ArrayList();
			string strTableName="MATCHES";
			string[] fieldsToSelect=new string[] { "CATEGORY_ID", 
													 "PHASE", "NGROUP", "MATCH" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("FACILITY_ID", ConditionType.Equals, facilityID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				MatchData data=new MatchData();
				data.CategoryID = (int) reader["CATEGORY_ID"];
				data.PhaseIndex = (int) reader["PHASE"];
				data.GroupIndex = (int) reader["NGROUP"];
				data.MatchIndex = (int) reader["MATCH"];
				result.Add(data);
			}
			reader.Close();
			
			return (MatchData[]) result.ToArray(typeof(MatchData));
		}
		#endregion
		
		#region Get Facility Heats
		private static HeatData[] GetFacilityHeats(int facilityID)
		{
			ArrayList result=new ArrayList();
			string strTableName="HEATS";
			string[] fieldsToSelect=new string[] { "CATEGORY_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("FACILITY_ID", ConditionType.Equals, facilityID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				HeatData data=new HeatData();
				data.CategoryID = (int) reader["CATEGORY_ID"];
				result.Add(data);
			}
			reader.Close();
			
			return (HeatData[]) result.ToArray(typeof(HeatData));
		}
		#endregion
		#endregion
		#endregion
		
		#region Get Ruleset Championships
		private static int[] GetRulesetChampionships(int rulesetID)
		{
			ArrayList result=new ArrayList();
			string strTableName="CHAMPIONSHIPS";
			string[] fieldsToSelect=new string[] { "CHAMPIONSHIP_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("RULESET_ID", ConditionType.Equals, rulesetID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				int champID=(int) reader["CHAMPIONSHIP_ID"];
				if (result.IndexOf(champID) < 0)
					result.Add(champID);
			}
			reader.Close();
			
			return (int[]) result.ToArray(typeof(int));
		}
		#endregion
		#endregion
		#endregion

		#region Get methods
		#region get simple championships
		public static SimpleData[] GetChampionshipsData(int regionID, int sportID)
		{
			string strTableName="CHAMPIONSHIPS";
			string[] fieldsToSelect=
				new string[] { "CHAMPIONSHIP_ID", "CHAMPIONSHIP_NAME" };
			ArrayList arrConditions=new ArrayList();
			arrConditions.Add(
				new Condition("REGION_ID", ConditionType.Equals, regionID));
			if (sportID >= 0)
			{
				arrConditions.Add(
					new Condition("SPORT_ID", ConditionType.Equals, sportID));
			}
			ConditionLogic[] arrLogics=new ConditionLogic[arrConditions.Count];
			for (int i=0; i<arrConditions.Count; i++)
			{
				ConditionLogic logic=(i == 0)?ConditionLogic.None:ConditionLogic.And;
				arrLogics[i] = logic;
			}
			ConditionCollection conditions=
				new ConditionCollection(
				(Condition[]) arrConditions.ToArray(typeof(Condition)), 
				arrLogics);
			MirDataReader reader=GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList arrChamps=new ArrayList();
			while (reader.Read())
			{
				SimpleData data=new SimpleData();
				data.ID = Tools.CIntDef(reader["CHAMPIONSHIP_ID"], -1);
				data.Name = reader["CHAMPIONSHIP_NAME"].ToString();
				arrChamps.Add(data);
			}
			reader.Close();
			return (SimpleData[]) arrChamps.ToArray(typeof(SimpleData));
		}
		#endregion

		#region Get Full Championship
		public static ChampionshipData GetFullChampionship(int champID)
		{
			ChampionshipData champData=new ChampionshipData();
			string strTableName="";
			string[] fieldsToSelect=null;
			ConditionCollection conditions=null;
			MirDataReader reader=null;
			int i=0;
			
			/* keep the SQL simple, in the cost of less efficiency. */
			
			//raw championship data:
			strTableName="CHAMPIONSHIPS";
			fieldsToSelect=new string[] {"CHAMPIONSHIP_ID", "CHAMPIONSHIP_NAME",
				"SEASON", "REGION_ID", "SPORT_ID", "IS_CLUBS", "START_DATE",
				"LAST_REGISTRATION_DATE", "CHAMPIONSHIP_STATUS", "ALT_START_DATE", 
				"END_DATE", "ALT_END_DATE", "FINALS_DATE", "RULESET_ID", "IS_OPEN", 
				"ALT_FINALS_DATE", "CHAMPIONSHIP_SUPERVISOR", "STANDARD_CHAMPIONSHIP"
				};
			conditions=new ConditionCollection(
				new Condition("CHAMPIONSHIP_ID", ConditionType.Equals, champID), 
				ConditionLogic.None);
			reader=GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				champData.ID = (int) reader["CHAMPIONSHIP_ID"];
				champData.Name = reader["CHAMPIONSHIP_NAME"].ToString();
				champData.AltEndDate = Tools.CDateTimeDef(
					reader["ALT_END_DATE"], DateTime.MinValue);
				champData.AltFinalsDate = Tools.CDateTimeDef(
					reader["ALT_FINALS_DATE"], DateTime.MinValue);
				champData.AltStartDate = Tools.CDateTimeDef(
					reader["ALT_START_DATE"], DateTime.MinValue);
				champData.EndDate = Tools.CDateTimeDef(
					reader["END_DATE"], DateTime.MinValue);
				champData.FinalsDate = Tools.CDateTimeDef(
					reader["FINALS_DATE"], DateTime.MinValue);
				champData.IsClubs = (reader["IS_CLUBS"].ToString() == "1")?true:false;
				champData.IsOpen = (reader["IS_OPEN"].ToString() == "1")?true:false;
				champData.LastRegistrationDate = Tools.CDateTimeDef(
					reader["LAST_REGISTRATION_DATE"], DateTime.MinValue);
				champData.Region = new RegionData((int) reader["REGION_ID"], "");
				champData.Ruleset = new RulesetData(
					Tools.CIntDef(reader["RULESET_ID"], -1), "");
				champData.Season = Tools.CIntDef(reader["SEASON"], -1);
				champData.Sport = new SportData((int) reader["SPORT_ID"], "");
				champData.StandardChampionship = new SimpleData(
					Tools.CIntDef(reader["STANDARD_CHAMPIONSHIP"], -1), "");
				champData.StartDate = Tools.CDateTimeDef(
					reader["START_DATE"], DateTime.MinValue);
				champData.Status = (int) reader["CHAMPIONSHIP_STATUS"];
				champData.Supervisor = Tools.CStrDef(
					reader["CHAMPIONSHIP_SUPERVISOR"], "");
			}
			reader.Close();
			//maybe does not exist...
			if (champData.ID != champID)
				return champData;
			
			//championship categories:
			strTableName="CATEGORIES";
			fieldsToSelect=new string[] {
				"CATEGORY_ID", "CHAMPIONSHIP_ID", "CATEGORY"};
			conditions=new ConditionCollection(
				new Condition("CHAMPIONSHIP_ID", ConditionType.Equals, champID), 
				ConditionLogic.None);
			reader=GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList categories=new ArrayList();
			while (reader.Read())
			{
				CategoryData category=new CategoryData();
				category.ID = (int) reader["CATEGORY_ID"];
				category.Championship = champID;
				category.Category = (int) reader["CATEGORY"];
				categories.Add(category);
			}
			reader.Close();
			champData.Categories = new CategoryData[categories.Count];
			for (i=0; i<categories.Count; i++)
			{
				champData.Categories[i] = (CategoryData) categories[i];
			}

			//phases:
			for (i=0; i<champData.Categories.Length; i++)
			{
				int categoryID=champData.Categories[i].ID;
				PhaseData[] phases=GetCategoryPhases(categoryID);
				champData.Categories[i].Phases = phases;

				//competitor rules:
				int category=champData.Categories[i].Category;
				foreach (PhaseData phase in champData.Categories[i].Phases)
				{
					foreach (GroupData group in phase.Groups)
					{
						foreach (CompetitionData competition in group.Competitions)
						{
							string competitorRule=GetCompetitorRule(champID, 
								competition.SportField, category);
							foreach (CompetitorData competitor in 
								competition.Competitors)
							{
								competitor.Rule = competitorRule;
							}
						}
					}
				}
			}

			//game structure rule:
			champData.GameStructureRule = GetGameStructureRule(champID);
			
			return champData;
		}

		#region Get Phases
		private static PhaseData[] GetCategoryPhases(int categoryID)
		{
			string strTableName="PHASES";
			string[] fieldsToSelect=new string[] {
					"PHASE", "PHASE_NAME", "STATUS"	};
			ConditionCollection conditions=new ConditionCollection(
				new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
				ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList phases=new ArrayList();
			while (reader.Read())
			{
				PhaseData phase=new PhaseData();
				phase.CategoryID = categoryID;
				phase.PhaseIndex = (int) reader["PHASE"];
				phase.PhaseName = Tools.CStrDef(reader["PHASE_NAME"], "");
				phase.Status = Tools.CIntDef(reader["STATUS"], -1);
				phases.Add(phase);
			}
			reader.Close();

			//groups:
			foreach (PhaseData phase in phases)
				phase.Groups = GetPhaseGroups(categoryID, phase.PhaseIndex);
			
			return (PhaseData[]) phases.ToArray(typeof(PhaseData));
		}

		#region Get Groups
		private static GroupData[] GetPhaseGroups(int categoryID, int phaseIndex)
		{
			string strTableName="GROUPS";
			string[] fieldsToSelect=new string[] {
						 "NGROUP", "GROUP_NAME", "STATUS"	};
			ConditionCollection conditions=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
					new Condition("PHASE", ConditionType.Equals, phaseIndex) 
								}, 
				new ConditionLogic[] {ConditionLogic.None, ConditionLogic.And});
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList groups=new ArrayList();
			while (reader.Read())
			{
				GroupData group=new GroupData();
				group.GroupIndex = (int) reader["NGROUP"];
				group.GroupName = Tools.CStrDef(reader["GROUP_NAME"], "");
				group.Status = Tools.CIntDef(reader["STATUS"], -1);
				groups.Add(group);
			}
			reader.Close();

			//rounds and competitions:
			foreach (GroupData group in groups)
			{
				group.Rounds = GetGroupRounds(categoryID, phaseIndex, group.GroupIndex);
				group.Competitions = GetGroupCompetitions(
					categoryID, phaseIndex, group.GroupIndex);
			}
			
			return (GroupData[]) groups.ToArray(typeof(GroupData));
		}
		
		#region Get Rounds
		private static RoundData[] GetGroupRounds(int categoryID, int phaseIndex, 
			int groupIndex)
		{
			string strTableName="ROUNDS";
			string[] fieldsToSelect=new string[] {
						 "ROUND", "ROUND_NAME", "STATUS" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
					new Condition("PHASE", ConditionType.Equals, phaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, groupIndex)
								}, 
				new ConditionLogic[] {ConditionLogic.None, ConditionLogic.And, 
					ConditionLogic.And});
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList rounds=new ArrayList();
			while (reader.Read())
			{
				RoundData round=new RoundData();
				round.RoundIndex = (int) reader["ROUND"];
				round.RoundName = Tools.CStrDef(reader["ROUND_NAME"], "");
				round.Status = Tools.CIntDef(reader["STATUS"], -1);
				rounds.Add(round);
			}
			reader.Close();
			
			//matches:
			foreach (RoundData round in rounds)
			{
				round.Matches = GetRoundMatches(categoryID, 
					phaseIndex, groupIndex, round.RoundIndex);
			}
			
			return (RoundData[]) rounds.ToArray(typeof(RoundData));
		}

		#region Get Matches
		private static MatchData[] GetRoundMatches(int categoryID, int phaseIndex, 
			int groupIndex, int roundIndex)
		{
			string strTableName="MATCHES";
			string[] fieldsToSelect=new string[] {"MATCH", "CYCLE", 
						"TEAM_A", "TEAM_B", "TIME", "FACILITY_ID", "COURT_ID",
						"TEAM_A_SCORE", "TEAM_B_SCORE", "RESULT", "PARTS_RESULT" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
					new Condition("PHASE", ConditionType.Equals, phaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, groupIndex), 
					new Condition("ROUND", ConditionType.Equals, roundIndex)
								}, 
				new ConditionLogic[] {ConditionLogic.None, ConditionLogic.And, 
					 ConditionLogic.And, ConditionLogic.And});
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList matches=new ArrayList();
			while (reader.Read())
			{
				MatchData match=new MatchData();
				match.MatchIndex = (int) reader["MATCH"];
				match.Cycle = Tools.CIntDef(reader["CYCLE"], 0);
				match.TeamA = new TeamData();
				match.TeamA.ID = (int) reader["TEAM_A"];
				match.TeamB = new TeamData();
				match.TeamB.ID = (int) reader["TEAM_B"];
				match.TeamA_Score = Tools.CDblDef(reader["TEAM_A_SCORE"], -1);
				match.TeamB_Score = Tools.CDblDef(reader["TEAM_B_SCORE"], -1);
				match.Time = Tools.CDateTimeDef(reader["TIME"], DateTime.MinValue);
				match.Facility = new FacilityData(
					Tools.CIntDef(reader["FACILITY_ID"], -1), "");
				match.Court = new CourtData(
					Tools.CIntDef(reader["COURT_ID"], -1), "");
				match.Result = Tools.CIntDef(reader["RESULT"], -1);
				match.PartsResult = Tools.CStrDef(reader["PARTS_RESULT"], "");
				matches.Add(match);
			}
			reader.Close();

			//teams, facility, courts:
			foreach (MatchData match in matches)
			{
				match.TeamA = GetTeamData(match.TeamA.ID);
				match.TeamB = GetTeamData(match.TeamB.ID);

				if ((match.Facility != null)&&(match.Facility.ID >= 0))
				{
					match.Facility = GetFacilityData(match.Facility.ID);
				}
				
				if ((match.Court != null)&&(match.Court.ID >= 0))
				{
					match.Court = GetCourtData(match.Court.ID);
				}
			}
			
			return (MatchData[]) matches.ToArray(typeof(MatchData));
		}
		#endregion
		#endregion
		
		#region Get Competitions
		private static CompetitionData[] GetGroupCompetitions(int categoryID, 
			int phaseIndex, int groupIndex)
		{
			string strTableName="COMPETITIONS";
			string[] fieldsToSelect=new string[] {
						"COMPETITION", "SPORT_FIELD_ID", "TIME", 
						"FACILITY_ID", "COURT_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition[] {
									new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
									new Condition("PHASE", ConditionType.Equals, phaseIndex), 
									new Condition("NGROUP", ConditionType.Equals, groupIndex)
								}, 
				new ConditionLogic[] {ConditionLogic.None, ConditionLogic.And, 
					 ConditionLogic.And});
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList competitions=new ArrayList();
			while (reader.Read())
			{
				CompetitionData competition=new CompetitionData();
				competition.CompetitionIndex = (int) reader["COMPETITION"];
				competition.Time = Tools.CDateTimeDef(
					reader["TIME"], DateTime.MinValue);
				competition.SportField = new SportFieldData(
					Tools.CIntDef(reader["SPORT_FIELD_ID"], -1), "");
				competition.Facility = new FacilityData(
					Tools.CIntDef(reader["FACILITY_ID"], -1), "");
				competition.Court = new CourtData(
					Tools.CIntDef(reader["COURT_ID"], -1), "");
				competitions.Add(competition);
			}
			reader.Close();

			//heats, competitors, facilities, courts and sport fields:
			foreach (CompetitionData competition in competitions)
			{
				competition.Heats = 
					GetCompetitionHeats(categoryID, phaseIndex, groupIndex, 
					competition.CompetitionIndex);
				competition.Competitors = 
					GetCompetitionCompetitors(categoryID, phaseIndex, groupIndex, 
					competition.CompetitionIndex);
				if ((competition.Facility != null)&&(competition.Facility.ID >= 0))
					competition.Facility = GetFacilityData(competition.Facility.ID);
				if ((competition.Court != null)&&(competition.Court.ID >= 0))
					competition.Court = GetCourtData(competition.Court.ID);
				if ((competition.SportField != null)&&(competition.SportField.ID >= 0))
					competition.SportField = GetSportFieldData(competition.SportField.ID);
			}
			
			return (CompetitionData[]) competitions.ToArray(typeof(CompetitionData));
		}

		#region Get Heats
		private static HeatData[] GetCompetitionHeats(int categoryID, 
			int phaseIndex, int groupIndex, int competitionIndex)
		{
			string strTableName="HEATS";
			string[] fieldsToSelect=new string[] {
						"HEAT", "TIME", "FACILITY_ID", 
						"COURT_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
					new Condition("PHASE", ConditionType.Equals, phaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, groupIndex), 
					new Condition("COMPETITION", ConditionType.Equals, competitionIndex)
								}, 
				new ConditionLogic[] {ConditionLogic.None, ConditionLogic.And, 
					 ConditionLogic.And, ConditionLogic.And});
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList heats=new ArrayList();
			while (reader.Read())
			{
				HeatData heat=new HeatData();
				heat.HeatIndex = (int) reader["HEAT"];
				heat.Time = Tools.CDateTimeDef(
					reader["TIME"], DateTime.MinValue);
				heat.Facility = new FacilityData(
					Tools.CIntDef(reader["FACILITY_ID"], -1), "");
				heat.Court = new CourtData(
					Tools.CIntDef(reader["COURT_ID"], -1), "");
				heats.Add(heat);
			}
			reader.Close();
			
			//facilities, courts:
			foreach (HeatData heat in heats)
			{
				if ((heat.Facility != null)&&(heat.Facility.ID >= 0))
				{
					heat.Facility = GetFacilityData(heat.Facility.ID);
				}
				
				if ((heat.Court != null)&&(heat.Court.ID >= 0))
				{
					heat.Court = GetCourtData(heat.Court.ID);
				}
			}

			return (HeatData[]) heats.ToArray(typeof(HeatData));
		}
		#endregion
		
		#region Get Competitors
		private static CompetitorData[] GetCompetitionCompetitors(int categoryID, 
			int phaseIndex, int groupIndex, int competitionIndex)
		{
			string strTableName="COMPETITORS";
			string[] fieldsToSelect=new string[] {"COMPETITOR", "PLAYER_ID", 
						"HEAT", "POSITION", "RESULT_POSITION", "RESULT", "SCORE"};
			ConditionCollection conditions=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, categoryID), 
					new Condition("PHASE", ConditionType.Equals, phaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, groupIndex), 
					new Condition("COMPETITION", ConditionType.Equals, competitionIndex)
								}, 
				new ConditionLogic[] {ConditionLogic.None, ConditionLogic.And, 
					ConditionLogic.And, ConditionLogic.And});
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			ArrayList competitors=new ArrayList();
			while (reader.Read())
			{
				CompetitorData competitor=new CompetitorData();
				competitor.CompetitorIndex = (int) reader["COMPETITOR"];
				competitor.Heat = new HeatData();
				competitor.Heat.HeatIndex = Tools.CIntDef(
					reader["HEAT"], -1);
				competitor.Player = new PlayerData();
				competitor.Player.ID = (int) reader["PLAYER_ID"];
				competitor.Position = Tools.CIntDef(reader["POSITION"], -1);
				competitor.Result = Tools.CStrDef(reader["RESULT"], "");
				competitor.ResultPosition = Tools.CIntDef(
					reader["RESULT_POSITION"], -1);
				competitor.Score = Tools.CIntDef(reader["SCORE"], 0);
				competitors.Add(competitor);
			}
			reader.Close();
			
			//players:
			foreach (CompetitorData competitor in competitors)
			{
				if ((competitor.Player != null)&&(competitor.Player.ID >= 0))
				{
					competitor.Player = GetPlayerData(competitor.Player.ID);
				}
			}
			
			return (CompetitorData[]) competitors.ToArray(typeof(CompetitorData));
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#endregion

		#region Get Simple Data (general)
		private static SimpleData GetSimpleData(string tableName, 
			string id_field, string name_field, int id_value)
		{
			SimpleData result=SimpleData.Empty;
			string[] fieldsToSelect=new string[] { id_field, name_field };
			ConditionCollection conditions=new ConditionCollection(
				new Condition(id_field, ConditionType.Equals, id_value), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				tableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result = new SimpleData((int) reader[id_field], 
					Tools.CStrDef(reader[name_field], ""));
			}
			reader.Close();
			
			return result;
		}
		#endregion

		#region Get Simple Regions
		public static SimpleData[] GetRegions()
		{
			string strTableName="REGIONS";
			string[] fieldsToSelect=
				new string[] { "REGION_ID", "REGION_NAME" };
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, null, null);
			ArrayList arrRegions=new ArrayList();
			while (reader.Read())
			{
				SimpleData data=new SimpleData();
				data.ID = Tools.CIntDef(reader["REGION_ID"], -1);
				data.Name = reader["REGION_NAME"].ToString();
				arrRegions.Add(data);
			}
			reader.Close();
			return (SimpleData[]) arrRegions.ToArray(typeof(SimpleData));
		}
		#endregion
		
		#region Get Simple Sports
		public static SimpleData[] GetSports()
		{
			string strTableName="SPORTS";
			string[] fieldsToSelect=
				new string[] { "SPORT_ID", "SPORT_NAME" };
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, null, null);
			ArrayList arrSports=new ArrayList();
			while (reader.Read())
			{
				SimpleData data=new SimpleData();
				data.ID = Tools.CIntDef(reader["SPORT_ID"], -1);
				data.Name = reader["SPORT_NAME"].ToString();
				arrSports.Add(data);
			}
			reader.Close();
			return (SimpleData[]) arrSports.ToArray(typeof(SimpleData));
		}
		#endregion

		#region Get Ruleset Rules
		public static RuleData[] GetRulesetRules(int rulesetID)
		{
			ArrayList rules=new ArrayList();
			string strTableName="RULES";
			string[] fieldsToSelect=new string[] { "RULE_ID", "RULESET_ID", 
						"RULE_TYPE_ID", "VALUE", "SPORT_FIELD_TYPE_ID", 
						"SPORT_FIELD_ID", "CATEGORY"};
			ConditionCollection conditions=new ConditionCollection(
				new Condition("RULESET_ID", ConditionType.Equals, rulesetID), 
				ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				RuleData rule=new RuleData();
				rule.RuleID = (int) reader["RULE_ID"];
				rule.Ruleset = new RulesetData();
				rule.Ruleset.ID = (int) reader["RULESET_ID"];
				rule.Category = Tools.CIntDef(reader["CATEGORY"], -1);
				rule.RuleType = new SimpleData((int) reader["RULE_TYPE_ID"], "");
				rule.SportField = new SportFieldData();
				rule.SportField.ID = Tools.CIntDef(
					reader["SPORT_FIELD_ID"], -1);
				rule.SportFieldType = new SportFieldTypeData();
				rule.SportFieldType.ID = Tools.CIntDef(
					reader["SPORT_FIELD_TYPE_ID"], -1);
				rule.Value = Tools.CStrDef(reader["VALUE"], "");
				rules.Add(rule);
			}
			reader.Close();
			
			foreach (RuleData rule in rules)
			{
				if (rule.RuleType.ID >= 0)
				{
					rule.RuleType = GetSimpleData(
						"RULE_TYPES", "RULE_TYPE_ID", "CLASS", rule.RuleType.ID);
				}
			}
			
			return (RuleData[]) rules.ToArray(typeof(RuleData));
		}
		#endregion

		#region Get Championship Sport
		public static SportData GetChampionshipSport(int champID)
		{
			string strTableName="";
			string[] fieldsToSelect=null;
			ConditionCollection conditions=null;
			MirDataReader reader=null;
			int sportID=-1;
			
			strTableName="CHAMPIONSHIPS";
			fieldsToSelect=
				new string[] { "SPORT_ID" };
			conditions = new ConditionCollection(
				new Condition("CHAMPIONSHIP_ID", ConditionType.Equals, champID), 
				ConditionLogic.None);
			reader=GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				sportID = (int) reader["SPORT_ID"];
			}
			reader.Close();
			if (sportID < 0)
				return null;
			
			return GetSportData(sportID);
		}
		#endregion
		
		#region Get Region Data
		public static RegionData GetRegionData(int regionID)
		{
			RegionData result=new RegionData();
			string strTableName="REGIONS";
			string[] fieldsToSelect=new string[] { "REGION_ID", "REGION_NAME", 
													 "ADDRESS", "PHONE", "FAX" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("REGION_ID", ConditionType.Equals, regionID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["REGION_ID"];
				result.Name = reader["REGION_NAME"].ToString();
				result.Address = Tools.CStrDef(reader["ADDRESS"], "");
				result.Phone = Tools.CStrDef(reader["PHONE"], "");
				result.Fax = Tools.CStrDef(reader["FAX"], "");
			}
			reader.Close();
			
			return result;
		}
		#endregion
		
		#region Get Sport Data
		public static SportData GetSportData(int sportID)
		{
			SportData result=new SportData();
			string strTableName="SPORTS";
			string[] fieldsToSelect=new string[] { "SPORT_ID", 
						 "SPORT_NAME", "SPORT_TYPE", "RULESET_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("SPORT_ID", ConditionType.Equals, sportID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["SPORT_ID"];
				result.Name = reader["SPORT_NAME"].ToString();
				result.Type = Tools.CIntDef(reader["SPORT_TYPE"], 0);
				result.Ruleset = new RulesetData();
				result.Ruleset.ID = Tools.CIntDef(
					reader["RULESET_ID"], -1);
			}
			reader.Close();
			
			return result;
		}
		#endregion
		
		#region Get School Data
		public static SchoolData GetSchoolData(int schoolID)
		{
			SchoolData result=new SchoolData();
			string strTableName="SCHOOLS";
			string[] fieldsToSelect=new string[] { "SCHOOL_ID", "SCHOOL_NAME", 
						 "SYMBOL", "CITY_NAME", "FROM_GRADE", "TO_GRADE", 
						 "REGION_ID", "CLUB_STATUS" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("SCHOOL_ID", ConditionType.Equals, schoolID), 
				ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["SCHOOL_ID"];
				result.Name = reader["SCHOOL_NAME"].ToString();
				result.Symbol = reader["SYMBOL"].ToString();
				result.City = Tools.CStrDef(
					reader["CITY_NAME"], "");
				result.FromGrade = Tools.CIntDef(
					reader["FROM_GRADE"], 0);
				result.ToGrade = Tools.CIntDef(
					reader["TO_GRADE"], 0);
				result.Region = new RegionData();
				result.Region.ID = (int) reader["REGION_ID"];
				result.IsClub = (reader["CLUB_STATUS"].ToString() == "1");
			}
			reader.Close();
			
			//maybe school was not found...
			if ((result.Region != null)&&(result.Region.ID >= 0))
				result.Region = GetRegionData(result.Region.ID);
			
			return result;
		}
		#endregion

		#region Get Student Data
		public static StudentData GetStudentData(int studentID)
		{
			StudentData result=new StudentData();
			string strTableName="STUDENTS";
			string[] fieldsToSelect=new string[] { "STUDENT_ID", "ID_NUMBER", 
						"FIRST_NAME", "LAST_NAME", "BIRTH_DATE", 
						"SCHOOL_ID", "GRADE", "SEX_TYPE" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("STUDENT_ID", ConditionType.Equals, studentID), 
				ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["STUDENT_ID"];
				result.FirstName = Tools.CStrDef(reader["FIRST_NAME"], "");
				result.LastName = Tools.CStrDef(reader["LAST_NAME"], "");
				result.Name = result.FirstName;
				if (result.LastName.Length > 0)
					result.Name += " "+result.LastName;
				result.IdNumber = reader["ID_NUMBER"].ToString();
				result.Birthdate = Tools.CDateTimeDef(
					reader["BIRTH_DATE"], DateTime.MinValue);
				result.School = new SchoolData();
				result.School.ID = (int) reader["SCHOOL_ID"];
				result.Grade = Tools.CIntDef(reader["GRADE"], 0);
				result.SexType = Tools.CIntDef(reader["SEX_TYPE"], -1);
			}
			reader.Close();
			
			if (result.School.ID >= 0)
				result.School = GetSchoolData(result.School.ID);
			
			return result;
		}
		#endregion

		#region Get Team Data
		public static TeamData GetTeamData(int teamID)
		{
			string strTableName="";
			string[] fieldsToSelect=null;
			ConditionCollection conditions=null;
			MirDataReader reader=null;
			TeamData result=new TeamData();
			
			strTableName="TEAMS";
			fieldsToSelect=
				new string[] { "TEAM_ID", "SCHOOL_ID", "CHAMPIONSHIP_ID", 
					"CATEGORY_ID", "STATUS", "TEAM_INDEX", "TEAM_SUPERVISOR", 
					"REGISTRATION_DATE" };
			conditions = new ConditionCollection(
				new Condition("TEAM_ID", ConditionType.Equals, teamID), 
				ConditionLogic.None);
			reader=GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["TEAM_ID"];
				result.Championship = new ChampionshipData();
				result.Championship.ID = (int) reader["CHAMPIONSHIP_ID"];
				result.ChampionshipCategory = new CategoryData();
				result.ChampionshipCategory.ID = (int) reader["CATEGORY_ID"];
				result.RegistrationDate = Tools.CDateTimeDef(
					reader["REGISTRATION_DATE"], DateTime.MinValue);
				result.School = new SchoolData();
				result.School.ID = (int) reader["SCHOOL_ID"];
				result.Status = (int) reader["STATUS"];
				result.Supervisor = Tools.CStrDef(
					reader["TEAM_SUPERVISOR"], "");
				result.TeamIndex = Tools.CIntDef(
					reader["TEAM_INDEX"], 0);
			}
			reader.Close();
			if (result.ID < 0)
				return null;
			
			//maybe team was not found...
			if ((result.School != null)&&(result.School.ID >= 0))
			{
				result.School = GetSchoolData(result.School.ID);
				if (result.School.ID < 0)
					throw new Exception("Team without school: "+teamID);
			}
			
			return result;
		}
		#endregion
		
		#region Get Player Data
		public static PlayerData GetPlayerData(int playerID)
		{
			PlayerData result=new PlayerData();
			string strTableName="PLAYERS";
			string[] fieldsToSelect=new string[] { "PLAYER_ID", "STUDENT_ID", 
						"TEAM_ID", "SHIRT_NUMBER", "CHIP_NUMBER", "STATUS", 
						"REMARKS", "REGISTRATION_DATE" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("PLAYER_ID", ConditionType.Equals, playerID), 
				ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["PLAYER_ID"];
				result.Student = new StudentData();
				result.Student.ID = (int) reader["STUDENT_ID"];
				result.Team = new TeamData();
				result.Team.ID = (int) reader["TEAM_ID"];
				result.ShirtNumber = Tools.CIntDef(reader["SHIRT_NUMBER"], 0);
				result.ChipNumber = Tools.CIntDef(reader["CHIP_NUMBER"], 0);
				result.Status = Tools.CIntDef(reader["STATUS"], 0);
				result.Remarks = Tools.CStrDef(reader["REMARKS"], "");
				result.RegistrationDate = Tools.CDateTimeDef(
					reader["REGISTRATION_DATE"], DateTime.MinValue);
			}
			reader.Close();
			
			if (result.Student.ID >= 0)
				result.Student = GetStudentData(result.Student.ID);

			if (result.Team.ID >= 0)
				result.Team = GetTeamData(result.Team.ID);
			
			return result;
		}
		#endregion
		
		#region Get Facility Data
		public static FacilityData GetFacilityData(int facilityID)
		{
			FacilityData result=new FacilityData();
			string strTableName="FACILITIES";
			string[] fieldsToSelect=new string[] { "FACILITY_ID", "FACILITY_NAME", 
						"REGION_ID", "SCHOOL_ID", "ADDRESS", "PHONE", "FAX" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("FACILITY_ID", ConditionType.Equals, facilityID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["FACILITY_ID"];
				result.Name = reader["FACILITY_NAME"].ToString();
				result.Region = new RegionData();
				result.Region.ID = (int) reader["REGION_ID"];
				result.School = new SchoolData();
				result.School.ID = Tools.CIntDef(reader["SCHOOL_ID"], -1);
				result.Address = Tools.CStrDef(reader["ADDRESS"], "");
				result.Phone = Tools.CStrDef(reader["PHONE"], "");
				result.Fax = Tools.CStrDef(reader["FAX"], "");
			}
			reader.Close();

			if (result.Region.ID >= 0)
				result.Region = GetRegionData(result.Region.ID);
			
			if (result.School.ID >= 0)
				result.School = GetSchoolData(result.School.ID);
			
			return result;
		}
		#endregion
		
		#region Get Court Data
		public static CourtData GetCourtData(int courtID)
		{
			CourtData result=new CourtData();
			string strTableName="COURTS";
			string[] fieldsToSelect=new string[] { "COURT_ID", 
					"COURT_NAME", "FACILITY_ID", "COURT_TYPE_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("COURT_ID", ConditionType.Equals, courtID), 
				ConditionLogic.None);
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["COURT_ID"];
				result.Name = reader["COURT_NAME"].ToString();
				result.Facility = new FacilityData();
				result.Facility.ID = (int) reader["FACILITY_ID"];
				result.CourtType = new SimpleData(
					Tools.CIntDef(reader["COURT_TYPE_ID"], -1), "");
			}
			reader.Close();
			
			if (result.Facility.ID >= 0)
				result.Facility = GetFacilityData(result.Facility.ID);
			
			if (result.CourtType.ID >= 0)
				result.CourtType = GetSimpleData("COURT_TYPES", "COURT_TYPE_ID", 
					"COURT_TYPE_NAME", result.CourtType.ID);
			
			return result;
		}
		#endregion
		
		#region Get Sport Field Type Data
		public static SportFieldTypeData GetSportFieldTypeData(int sportFieldTypeID)
		{
			SportFieldTypeData result=new SportFieldTypeData();
			string strTableName="SPORT_FIELD_TYPES";
			string[] fieldsToSelect=new string[] { "SPORT_FIELD_TYPE_ID", 
						 "SPORT_FIELD_TYPE_NAME", "SPORT_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("SPORT_FIELD_TYPE_ID", ConditionType.Equals, 
					sportFieldTypeID), ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["SPORT_FIELD_TYPE_ID"];
				result.Name = reader["SPORT_FIELD_TYPE_NAME"].ToString();
				result.Sport = new SportData();
				result.Sport.ID = 
					Tools.CIntDef(reader["SPORT_ID"], -1);
			}
			reader.Close();
			
			if (result.Sport.ID >= 0)
				result.Sport = GetSportData(result.Sport.ID);
			
			return result;
		}
		#endregion

		#region Get Sport Field Data
		public static SportFieldData GetSportFieldData(int sportFieldID)
		{
			SportFieldData result=new SportFieldData();
			string strTableName="SPORT_FIELDS";
			string[] fieldsToSelect=new string[] { "SPORT_FIELD_ID", 
						"SPORT_FIELD_NAME", "SPORT_FIELD_TYPE_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("SPORT_FIELD_ID", ConditionType.Equals, sportFieldID), 
				ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				result.ID = (int) reader["SPORT_FIELD_ID"];
				result.Name = reader["SPORT_FIELD_NAME"].ToString();
				result.SportFieldType = new SportFieldTypeData();
				result.SportFieldType.ID = 
					Tools.CIntDef(reader["SPORT_FIELD_TYPE_ID"], -1);
			}
			reader.Close();
			
			if (result.SportFieldType.ID >= 0)
			{
				result.SportFieldType = 
					GetSportFieldTypeData(result.SportFieldType.ID);
			}
			
			return result;
		}
		#endregion
		
		#region Get Competitor Rule
		public static string GetCompetitorRule(int championshipID, 
			SportFieldData sportField, int category)
		{
			//first check if the championship has ruleset defined:
			int rulesetID=-1;
			string strTableName="CHAMPIONSHIPS";
			string[] fieldsToSelect=new string[] { "RULESET_ID" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition("CHAMPIONSHIP_ID", ConditionType.Equals, championshipID), 
				ConditionLogic.None);
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, conditions, null);
			if (reader.Read())
			{
				rulesetID = Tools.CIntDef(reader["RULESET_ID"], -1);
			}
			reader.Close();

			//try to find the default ruleset if not found yet
			if (rulesetID < 0)
			{
				SportData sport=GetChampionshipSport(championshipID);
				if (sport.Ruleset.ID >= 0)
					rulesetID = sport.Ruleset.ID;
			}
			
			//still no luck?
			if (rulesetID < 0)
				return null;
			
			//get all rules of that ruleset:
			RuleData[] rules=GetRulesetRules(rulesetID);
			
			//look for the score type rule:
			int sportFieldID=-1;
			int sportFieldTypeID=-1;
			if (sportField != null)
			{
				sportFieldID=sportField.ID;
				if (sportField.SportFieldType != null)
					sportFieldTypeID = sportField.SportFieldType.ID;
			}
			foreach (RuleData rule in rules)
			{
				if ((rule.SportField.ID >= 0)&&(sportFieldID >= 0)&&
					(rule.SportField.ID != sportFieldID))
					continue;
				if ((rule.SportFieldType.ID >= 0)&&(sportFieldTypeID >= 0)&&
					(rule.SportFieldType.ID != sportFieldTypeID))
					continue;
				if ((rule.Category >= 0)&&(category >= 0)&&
					(rule.Category != Sport.Types.CategoryTypeLookup.All)&&
					(!Sport.Types.CategoryTypeLookup.Compare(category, rule.Category, 
						Sport.Types.CategoryCompareType.Full)))
					continue;
				if (rule.RuleType.ID >= 0)
				{
					string[] arrTmp=rule.RuleType.Name.Split(new char[] {','});
					string strClassName=arrTmp[0];
					if (strClassName == 
						typeof(Sport.Rulesets.Rules.ResultTypeRule).ToString())
					{
						return rule.Value;
					}
				}
			}
			
			return null;
		}
		#endregion

		#region Get Game Structure Rule
		public static string GetGameStructureRule(int championshipID)
		{
			//first check if the sport has default ruleset:
			int rulesetID=-1;
			SportData sport=GetChampionshipSport(championshipID);
			if (sport.Ruleset.ID >= 0)
				rulesetID = sport.Ruleset.ID;
			//look for the championship ruleset if needed:
			if (rulesetID < 0)
			{
				string strTableName="CHAMPIONSHIPS";
				string[] fieldsToSelect=new string[] { "RULESET_ID" };
				ConditionCollection conditions=new ConditionCollection(
					new Condition("CHAMPIONSHIP_ID", ConditionType.Equals, championshipID), 
					ConditionLogic.None);
				MirDataReader reader=
					GetTableData(strTableName, fieldsToSelect, conditions, null);
				if (reader.Read())
				{
					rulesetID = Tools.CIntDef(reader["RULESET_ID"], -1);
				}
				reader.Close();
			}
			
			//still no luck?
			if (rulesetID < 0)
				return null;
			
			//get all rules of that ruleset:
			RuleData[] rules=GetRulesetRules(rulesetID);
			
			//look for the game structure rule:
			foreach (RuleData rule in rules)
			{
				if (rule.RuleType.ID >= 0)
				{
					string[] arrTmp=rule.RuleType.Name.Split(new char[] {','});
					string strClassName=arrTmp[0];
					if (strClassName == 
						typeof(Sport.Rulesets.Rules.GameStructureRule).ToString())
					{
						return rule.Value;
					}
				}
			}
			
			return null;
		}
		#endregion
		
		#region Get Score Range Data
		public static ScoreRangeData[] GetScoreRangeData(int sportField, 
			int sportFieldType)
		{
			ArrayList result=new ArrayList();
			string strTableName="SCORE_RANGES";
			string[] fieldsToSelect=new string[] { "SCORE_RANGE_ID", 
						"CATEGORY", "RANGE_LOWER_LIMIT", "RANGE_UPPER_LIMIT", 
						"SCORE" };
			ConditionCollection conditions=new ConditionCollection(
				new Condition[] {
					new Condition("SPORT_FIELD", ConditionType.Equals, sportField), 
					new Condition("SPORT_FIELD_TYPE", ConditionType.Equals, sportFieldType)
				}, 
				new ConditionLogic[] {ConditionLogic.None, ConditionLogic.And});
			MirDataReader reader = GetTableData(
				strTableName, fieldsToSelect, conditions, null);
			while (reader.Read())
			{
				ScoreRangeData scoreRange=new ScoreRangeData();
				scoreRange.ID = (int) reader["SCORE_RANGE_ID"];
				scoreRange.Category = Tools.CIntDef(reader["CATEGORY"], -1);
				scoreRange.LowerLimit = Tools.CIntDef(reader["RANGE_LOWER_LIMIT"], -1);
				scoreRange.UpperLimit = Tools.CIntDef(reader["RANGE_UPPER_LIMIT"], -1);
				scoreRange.Score = Tools.CIntDef(reader["SCORE"], -1);
				scoreRange.SportField = new SportFieldData();
				scoreRange.SportField.ID = sportField;
				scoreRange.SportFieldType = new SportFieldTypeData();
				scoreRange.SportFieldType.ID = sportFieldType;
				result.Add(scoreRange);
			}
			reader.Close();
			
			return (ScoreRangeData[]) result.ToArray(typeof(ScoreRangeData));
		}
		#endregion
		#endregion

		#region general update methods (update&insert)
		#region Update Regions
		public static bool UpdateRegionData(RegionData region)
		{
			string strTableName="REGIONS";
			string[] fieldsToSelect=new string[] {"REGION_ID"};
			Field[] fields=new Field[]
				{
					new Field("REGION_ID", region.ID),
					new Field("REGION_NAME", region.Name), 
					new Field("ADDRESS", region.Address), 
					new Field("PHONE", region.Phone), 
					new Field("FAX", region.Fax)
				};
			
			ConditionCollection regionCondition=new ConditionCollection(
				new Condition("REGION_ID", ConditionType.Equals, region.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, regionCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			regionCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, regionCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Sports
		public static bool UpdateSportData(SportData sport)
		{
			string strTableName="SPORTS";
			string[] fieldsToSelect=new string[] {"SPORT_ID"};
			Field[] fields=new Field[]
				{
					new Field("SPORT_ID", sport.ID),
					new Field("SPORT_NAME", sport.Name), 
					new Field("SPORT_TYPE", sport.Type),
					new Field("RULESET_ID", sport.Ruleset.ID)
				};
			ConditionCollection sportCondition=new ConditionCollection(
				new Condition("SPORT_ID", ConditionType.Equals, sport.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, sportCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			sportCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, sportCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion 

		#region Update Schools
		public static bool UpdateSchoolData(SchoolData school)
		{
			string strTableName="SCHOOLS";
			string[] fieldsToSelect=new string[] {"SCHOOL_ID"};
			Field[] fields=new Field[]
				{
					new Field("SCHOOL_ID", school.ID),
					new Field("SYMBOL", school.Symbol), 
					new Field("SCHOOL_NAME", school.Name), 
					new Field("CITY_NAME", school.City), 
					new Field("FROM_GRADE", school.FromGrade), 
					new Field("TO_GRADE", school.ToGrade), 
					new Field("REGION_ID", school.Region.ID), 
					new Field("CLUB_STATUS", school.IsClub)
				};
			
			ConditionCollection schoolCondition=new ConditionCollection(
				new Condition("SCHOOL_ID", ConditionType.Equals, school.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, schoolCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			schoolCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, schoolCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion 

		#region Update Students
		public static bool UpdateStudentData(StudentData student)
		{
			string strTableName="STUDENTS";
			string[] fieldsToSelect=new string[] {"STUDENT_ID"};
			Field[] fields=new Field[]
				{
					new Field("STUDENT_ID", student.ID),
					new Field("ID_NUMBER", Int32.Parse(student.IdNumber)), 
					new Field("FIRST_NAME", student.FirstName), 
					new Field("LAST_NAME", student.LastName), 
					new Field("BIRTH_DATE", student.Birthdate), 
					new Field("SCHOOL_ID", student.School.ID), 
					new Field("GRADE", student.Grade), 
					new Field("SEX_TYPE", student.SexType)
				};
			
			ConditionCollection studentCondition=new ConditionCollection(
				new Condition("STUDENT_ID", ConditionType.Equals, student.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, studentCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			studentCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, studentCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion 

		#region Update Facilities
		public static bool UpdateFacilityData(FacilityData facility)
		{
			string strTableName="FACILITIES";
			string[] fieldsToSelect=new string[] {"FACILITY_ID"};
			Field[] fields=new Field[]
				{
					new Field("FACILITY_ID", facility.ID),
					new Field("FACILITY_NAME", facility.Name), 
					new Field("REGION_ID", facility.Region.ID), 
					new Field("SCHOOL_ID", 
						(facility.School == null)?-1:facility.School.ID), 
					new Field("ADDRESS", facility.Address), 
					new Field("PHONE", facility.Phone), 
					new Field("FAX", facility.Fax)
				};
			
			ConditionCollection facilityCondition=new ConditionCollection(
				new Condition("FACILITY_ID", ConditionType.Equals, facility.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, facilityCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			facilityCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, facilityCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion 
		
		#region Update Court Types
		public static bool UpdateCourtType(SimpleData courtType)
		{
			string strTableName="COURT_TYPES";
			string[] fieldsToSelect=new string[] {"COURT_TYPE_ID"};
			Field[] fields=new Field[]
				{
					new Field("COURT_TYPE_ID", courtType.ID),
					new Field("COURT_TYPE_NAME", courtType.Name)
				};
			
			ConditionCollection courtTypeCondition=new ConditionCollection(
				new Condition("COURT_TYPE_ID", ConditionType.Equals, courtType.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, courtTypeCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			courtTypeCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, courtTypeCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		
		#region Update Courts
		public static bool UpdateCourtData(CourtData court)
		{
			string strTableName="COURTS";
			string[] fieldsToSelect=new string[] {"COURT_ID"};
			Field[] fields=new Field[]
				{
					new Field("COURT_ID", court.ID),
					new Field("COURT_NAME", court.Name), 
					new Field("FACILITY_ID", 
						(court.Facility == null)?-1:court.Facility.ID), 
					new Field("COURT_TYPE_ID", court.CourtType.ID)
				};
			
			ConditionCollection courtCondition=new ConditionCollection(
				new Condition("COURT_ID", ConditionType.Equals, court.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, courtCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			courtCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, courtCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion 

		#region Update Rulesets
		public static bool UpdateRulesetData(RulesetData ruleset)
		{
			string strTableName="RULESETS";
			string[] fieldsToSelect=new string[] {"RULESET_ID"};
			Field[] fields=new Field[]
				{
					new Field("RULESET_ID", ruleset.ID),
					new Field("RULESET_NAME", ruleset.Name), 
					new Field("SPORT_ID", 
						(ruleset.Sport == null)?-1:ruleset.Sport.ID), 
					new Field("REGION_ID", 
						(ruleset.Region == null)?-1:ruleset.Region.ID)
				};
			
			ConditionCollection ruleSetCondition=new ConditionCollection(
				new Condition("RULESET_ID", ConditionType.Equals, ruleset.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, ruleSetCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			ruleSetCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, ruleSetCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Players
		public static bool UpdatePlayerData(PlayerData player)
		{
			string strTableName="PLAYERS";
			string[] fieldsToSelect=new string[] {"PLAYER_ID"};
			Field[] fields=new Field[]
				{
					new Field("PLAYER_ID", player.ID),
					new Field("STUDENT_ID", player.Student.ID), 
					new Field("TEAM_ID", player.Team.ID), 
					new Field("SHIRT_NUMBER", player.ShirtNumber), 
					new Field("CHIP_NUMBER", player.ChipNumber), 
					new Field("STATUS", player.Status), 
					new Field("REMARKS", player.Remarks), 
					new Field("REGISTRATION_DATE", player.RegistrationDate)
				};
			
			ConditionCollection playerCondition=new ConditionCollection(
				new Condition("PLAYER_ID", ConditionType.Equals, player.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, playerCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			playerCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, playerCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Teams
		public static bool UpdateTeamData(TeamData team)
		{
			string strTableName="TEAMS";
			string[] fieldsToSelect=new string[] {"TEAM_ID"};
			Field[] fields=new Field[]
				{
					new Field("TEAM_ID", team.ID),
					new Field("SCHOOL_ID", team.School.ID), 
					new Field("CHAMPIONSHIP_ID", team.Championship.ID), 
					new Field("CATEGORY_ID", team.ChampionshipCategory.ID), 
					new Field("STATUS", team.Status), 
					new Field("TEAM_INDEX", team.TeamIndex), 
					new Field("TEAM_SUPERVISOR", team.Supervisor), 
					new Field("REGISTRATION_DATE", team.RegistrationDate)
				};
			
			ConditionCollection teamCondition=new ConditionCollection(
				new Condition("TEAM_ID", ConditionType.Equals, team.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, teamCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			teamCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, teamCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Championships
		public static bool UpdateChampionshipData(ChampionshipData championship)
		{
			string strTableName="CHAMPIONSHIPS";
			string[] fieldsToSelect=new string[] {"CHAMPIONSHIP_ID"};
			Field[] fields=new Field[]
				{
					new Field("SEASON", championship.Season),			//---0
					new Field("CHAMPIONSHIP_NAME", championship.Name),	//---1
					new Field("REGION_ID", championship.Region.ID),		//---2
					new Field("SPORT_ID", championship.Sport.ID),		//---3
					new Field("IS_CLUBS", championship.IsClubs),		//---4
					new Field("LAST_REGISTRATION_DATE", 
						championship.LastRegistrationDate),				//---5
					new Field("START_DATE", championship.StartDate),	//---6
					new Field("END_DATE", championship.EndDate),		//---7
					new Field("ALT_START_DATE", 
						championship.AltStartDate),						//---8
					new Field("ALT_END_DATE", championship.AltEndDate),	//---9
					new Field("FINALS_DATE", championship.FinalsDate),	//---10
					new Field("ALT_FINALS_DATE", 
						championship.AltFinalsDate),					//---11
					new Field("RULESET_ID", championship.Ruleset.ID),	//---12
					new Field("IS_OPEN", championship.IsOpen),			//---13
					new Field("CHAMPIONSHIP_STATUS", 
						championship.Status),							//---14
					new Field("CHAMPIONSHIP_SUPERVISOR", 
						championship.Supervisor),						//---15
					new Field("STANDARD_CHAMPIONSHIP", 
						championship.StandardChampionship.ID),			//---16
					new Field("CHAMPIONSHIP_ID", championship.ID)		//---17
				};

			ConditionCollection champCond=new ConditionCollection(
				new Condition("CHAMPIONSHIP_ID", ConditionType.Equals, championship.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, champCond, null);
			bool blnExists=reader.Read();
			reader.Close();
			champCond.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, champCond);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Category
		public static bool UpdateChampionshipCategory(CategoryData category)
		{
			string strTableName="CATEGORIES";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", category.ID),
					new Field("CHAMPIONSHIP_ID", category.Championship), 
					new Field("CATEGORY", category.Category)
				};
			
			ConditionCollection categoryCondition=new ConditionCollection(
				new Condition("CATEGORY_ID", ConditionType.Equals, category.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, categoryCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			categoryCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, categoryCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update SportFieldType
		public static bool UpdateSportFieldType(SportFieldTypeData sportFieldType)
		{
			string strTableName="SPORT_FIELD_TYPES";
			string[] fieldsToSelect=new string[] {"SPORT_FIELD_TYPE_ID"};
			Field[] fields=new Field[]
				{
					new Field("SPORT_FIELD_TYPE_ID", sportFieldType.ID),
					new Field("SPORT_ID", sportFieldType.Sport.ID), 
					new Field("SPORT_FIELD_TYPE_NAME", sportFieldType.Name)
				};
			
			ConditionCollection sportFieldTypeCondition=new ConditionCollection(
				new Condition("SPORT_FIELD_TYPE_ID", ConditionType.Equals, 
				sportFieldType.ID), ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=GetTableData(
				strTableName, fieldsToSelect, sportFieldTypeCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			sportFieldTypeCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, sportFieldTypeCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		
		#region Update SportField
		public static bool UpdateSportField(SportFieldData sportField)
		{
			string strTableName="SPORT_FIELDS";
			string[] fieldsToSelect=new string[] {"SPORT_FIELD_ID"};
			Field[] fields=new Field[]
				{
					new Field("SPORT_FIELD_ID", sportField.ID),
					new Field("SPORT_FIELD_TYPE_ID", sportField.SportFieldType.ID), 
					new Field("SPORT_FIELD_NAME", sportField.Name)
				};
			
			ConditionCollection sportFieldCondition=new ConditionCollection(
				new Condition("SPORT_FIELD_ID", ConditionType.Equals, sportField.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, sportFieldCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			sportFieldCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, sportFieldCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Rules
		public static bool UpdateRuleData(RuleData rule)
		{
			string strTableName="RULES";
			string[] fieldsToSelect=new string[] {"RULE_ID"};
			Field[] fields=new Field[]
				{
					new Field("RULE_ID", rule.RuleID),
					new Field("RULESET_ID", rule.Ruleset.ID), 
					new Field("RULE_TYPE_ID", rule.RuleType.ID), 
					new Field("VALUE", rule.Value), 
					new Field("SPORT_FIELD_TYPE_ID", 
						(rule.SportFieldType == null)?-1:rule.SportFieldType.ID), 
					new Field("SPORT_FIELD_ID",
						(rule.SportField == null)?-1:rule.SportField.ID), 
					new Field("CATEGORY", rule.Category)
				};
			
			ConditionCollection ruleCondition=new ConditionCollection(
				new Condition("RULE_ID", ConditionType.Equals, rule.RuleID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, ruleCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			ruleCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, ruleCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		
		#region Update Score Ranges
		public static bool UpdateScoreRange(ScoreRangeData scoreRange)
		{
			string strTableName="SCORE_RANGES";
			string[] fieldsToSelect=new string[] {"SCORE_RANGE_ID"};
			Field[] fields=new Field[]
				{
					new Field("SCORE_RANGE_ID", scoreRange.ID), 
					new Field("SPORT_FIELD_TYPE", scoreRange.SportFieldType.ID), 
					new Field("SPORT_FIELD", scoreRange.SportField.ID), 
					new Field("CATEGORY", scoreRange.Category), 
					new Field("RANGE_LOWER_LIMIT", scoreRange.LowerLimit), 
					new Field("RANGE_UPPER_LIMIT", scoreRange.UpperLimit), 
					new Field("SCORE", scoreRange.Score)
				};
			
			ConditionCollection scoreRangeCondition=new ConditionCollection(
				new Condition("SCORE_RANGE_ID", ConditionType.Equals, scoreRange.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, scoreRangeCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			scoreRangeCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, scoreRangeCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		
		#region Update Rule Types
		public static bool UpdateRuleType(SimpleData ruleType)
		{
			string strTableName="RULE_TYPES";
			string[] fieldsToSelect=new string[] {"RULE_TYPE_ID"};
			Field[] fields=new Field[]
				{
					new Field("RULE_TYPE_ID", ruleType.ID),
					new Field("CLASS", ruleType.Name)
				};
			
			ConditionCollection ruleTypeCondition=new ConditionCollection(
				new Condition("RULE_TYPE_ID", ConditionType.Equals, ruleType.ID), 
				ConditionLogic.None);
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, ruleTypeCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			ruleTypeCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, ruleTypeCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		#endregion

		#region complex update methods
		#region Update Phases
		public static bool UpdatePhaseData(PhaseData phase)
		{
			string strTableName="PHASES";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", phase.CategoryID),
					new Field("PHASE", phase.PhaseIndex), 
					new Field("PHASE_NAME", phase.PhaseName), 
					new Field("STATUS", phase.Status)
				};
			
			ConditionCollection phaseCondition=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, phase.CategoryID), 
					new Condition("PHASE", ConditionType.Equals, phase.PhaseIndex)
				}, 
				new ConditionLogic[] {
					ConditionLogic.None, ConditionLogic.And
				 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, phaseCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			phaseCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, phaseCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Groups
		public static bool UpdateGroupData(GroupData group)
		{
			string strTableName="GROUPS";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", group.Phase.CategoryID),
					new Field("PHASE", group.Phase.PhaseIndex), 
					new Field("NGROUP", group.GroupIndex), 
					new Field("GROUP_NAME", group.GroupName), 
					new Field("STATUS", group.Status)
				};
			
			ConditionCollection groupCondition=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, group.Phase.CategoryID), 
					new Condition("PHASE", ConditionType.Equals, group.Phase.PhaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, group.GroupIndex)
				}, 
				new ConditionLogic[] {
					 ConditionLogic.None, ConditionLogic.And, ConditionLogic.And
				 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, groupCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			groupCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, groupCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Rounds
		public static bool UpdateRoundData(RoundData round)
		{
			string strTableName="ROUNDS";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", round.Group.Phase.CategoryID),
					new Field("PHASE", round.Group.Phase.PhaseIndex), 
					new Field("NGROUP", round.Group.GroupIndex), 
					new Field("ROUND", round.RoundIndex), 
					new Field("ROUND_NAME", round.RoundName), 
					new Field("STATUS", round.Status)
				};
			
			ConditionCollection roundCondition=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, round.Group.Phase.CategoryID), 
					new Condition("PHASE", ConditionType.Equals, round.Group.Phase.PhaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, round.Group.GroupIndex), 
					new Condition("ROUND", ConditionType.Equals, round.RoundIndex)
				}, 
				new ConditionLogic[] {
					 ConditionLogic.None, ConditionLogic.And, ConditionLogic.And, 
					ConditionLogic.And
				 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, roundCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			roundCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, roundCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		
		#region Update Matches
		public static bool UpdateMatchData(MatchData match)
		{
			string strTableName="MATCHES";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", match.Round.Group.Phase.CategoryID),
					new Field("PHASE", match.Round.Group.Phase.PhaseIndex), 
					new Field("NGROUP", match.Round.Group.GroupIndex), 
					new Field("ROUND", match.Round.RoundIndex), 
					new Field("CYCLE", match.Cycle), 
					new Field("MATCH", match.MatchIndex), 
					new Field("TEAM_A", match.TeamA.ID), 
					new Field("TEAM_B", match.TeamB.ID), 
					new Field("TIME", match.Time), 
					new Field("FACILITY_ID", 
						(match.Facility == null)?-1:match.Facility.ID), 
					new Field("COURT_ID", 
						(match.Court == null)?-1:match.Court.ID), 
					new Field("TEAM_A_SCORE", match.TeamA_Score), 
					new Field("TEAM_B_SCORE", match.TeamB_Score), 
					new Field("RESULT", match.Result), 
					new Field("PARTS_RESULT", match.PartsResult)
				};
			
			ConditionCollection matchCondition=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, match.Round.Group.Phase.CategoryID), 
					new Condition("PHASE", ConditionType.Equals, match.Round.Group.Phase.PhaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, match.Round.Group.GroupIndex), 
					new Condition("ROUND", ConditionType.Equals, match.Round.RoundIndex), 
					new Condition("MATCH", ConditionType.Equals, match.MatchIndex)
				}, 
				new ConditionLogic[] {
					 ConditionLogic.None, ConditionLogic.And, ConditionLogic.And, 
					 ConditionLogic.And, ConditionLogic.And
				 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, matchCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			matchCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, matchCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Competitions
		public static bool UpdateCompetitionData(CompetitionData competition)
		{
			string strTableName="COMPETITIONS";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", competition.Group.Phase.CategoryID),
					new Field("PHASE", competition.Group.Phase.PhaseIndex), 
					new Field("NGROUP", competition.Group.GroupIndex), 
					new Field("COMPETITION", competition.CompetitionIndex), 
					new Field("SPORT_FIELD_ID", competition.SportField.ID), 
					new Field("TIME", competition.Time), 
					new Field("FACILITY_ID", 
						(competition.Facility == null)?-1:competition.Facility.ID), 
					new Field("COURT_ID", 
						(competition.Court == null)?-1:competition.Court.ID)
				};
			
			ConditionCollection competitionCondition=new ConditionCollection(
				new Condition[] {
									new Condition("CATEGORY_ID", ConditionType.Equals, competition.Group.Phase.CategoryID), 
									new Condition("PHASE", ConditionType.Equals, competition.Group.Phase.PhaseIndex), 
									new Condition("NGROUP", ConditionType.Equals, competition.Group.GroupIndex), 
									new Condition("COMPETITION", ConditionType.Equals, competition.CompetitionIndex)
								},
				new ConditionLogic[] {
										 ConditionLogic.None, 
										 ConditionLogic.And, 
										 ConditionLogic.And, 
										 ConditionLogic.And
									 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, competitionCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			competitionCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, competitionCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Heats
		public static bool UpdateHeatData(HeatData heat)
		{
			string strTableName="HEATS";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", heat.Competition.Group.Phase.CategoryID),
					new Field("PHASE", heat.Competition.Group.Phase.PhaseIndex), 
					new Field("NGROUP", heat.Competition.Group.GroupIndex), 
					new Field("COMPETITION", heat.Competition.CompetitionIndex), 
					new Field("HEAT", heat.HeatIndex), 
					new Field("TIME", heat.Time), 
					new Field("FACILITY_ID", 
						(heat.Facility == null)?-1:heat.Facility.ID), 
					new Field("COURT_ID", 
						(heat.Court == null)?-1:heat.Court.ID)
				};
			
			ConditionCollection heatCondition=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, heat.Competition.Group.Phase.CategoryID), 
					new Condition("PHASE", ConditionType.Equals, heat.Competition.Group.Phase.PhaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, heat.Competition.Group.GroupIndex), 
					new Condition("COMPETITION", ConditionType.Equals, heat.Competition.CompetitionIndex), 
					new Condition("HEAT", ConditionType.Equals, heat.HeatIndex)
				},
				new ConditionLogic[] {
					 ConditionLogic.None, ConditionLogic.And, ConditionLogic.And, 
					 ConditionLogic.And, ConditionLogic.And
				 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, heatCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			heatCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, heatCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion

		#region Update Competitors
		public static bool UpdateCompetitorData(CompetitorData competitor)
		{
			string strTableName="COMPETITORS";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", competitor.Competition.Group.Phase.CategoryID),
					new Field("PHASE", competitor.Competition.Group.Phase.PhaseIndex), 
					new Field("NGROUP", competitor.Competition.Group.GroupIndex), 
					new Field("COMPETITION", competitor.Competition.CompetitionIndex), 
					new Field("COMPETITOR", competitor.CompetitorIndex), 
					new Field("PLAYER_ID", competitor.Player.ID), 
					new Field("HEAT", competitor.Heat.HeatIndex), 
					new Field("POSITION", competitor.Position), 
					new Field("RESULT_POSITION", competitor.ResultPosition), 
					new Field("RESULT", competitor.Result), 
					new Field("SCORE", competitor.Score), 
				};
			
			ConditionCollection competitorCondition=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, competitor.Competition.Group.Phase.CategoryID), 
					new Condition("PHASE", ConditionType.Equals, competitor.Competition.Group.Phase.PhaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, competitor.Competition.Group.GroupIndex), 
					new Condition("COMPETITION", ConditionType.Equals, competitor.Competition.CompetitionIndex), 
					new Condition("HEAT", ConditionType.Equals, competitor.Heat.HeatIndex), 
					new Condition("COMPETITOR", ConditionType.Equals, competitor.CompetitorIndex)
				},
				new ConditionLogic[] {
					 ConditionLogic.None, ConditionLogic.And, ConditionLogic.And, 
					 ConditionLogic.And, ConditionLogic.And, ConditionLogic.And
				 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, competitorCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			competitorCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, competitorCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		
		#region Update GroupTeams
		public static bool UpdateGroupTeamData(GroupTeamData groupTeam)
		{
			string strTableName="GROUP_TEAMS";
			string[] fieldsToSelect=new string[] {"CATEGORY_ID"};
			Field[] fields=new Field[]
				{
					new Field("CATEGORY_ID", groupTeam.Group.Phase.CategoryID),
					new Field("PHASE", groupTeam.Group.Phase.PhaseIndex), 
					new Field("NGROUP", groupTeam.Group.GroupIndex), 
					new Field("POSITION", groupTeam.Position), 
					new Field("PREVIOUS_GROUP", groupTeam.PreviousGroup), 
					new Field("PREVIOUS_POSITION", groupTeam.PreviousPosition), 
					new Field("TEAM_ID", groupTeam.Team.ID), 
					new Field("GAMES", groupTeam.Games), 
					new Field("POINTS", groupTeam.Points), 
					new Field("POINTS_AGAINST", groupTeam.PointsAgainst), 
					new Field("SMALL_POINTS", groupTeam.SmallPoints), 
					new Field("SMALL_POINTS_AGAINST", groupTeam.SmallPointsAgainst), 
					new Field("SCORE", groupTeam.Score), 
					new Field("RESULT_POSITION", groupTeam.ResultPosition)
			};
			
			ConditionCollection groupTeamCondition=new ConditionCollection(
				new Condition[] {
					new Condition("CATEGORY_ID", ConditionType.Equals, groupTeam.Group.Phase.CategoryID), 
					new Condition("PHASE", ConditionType.Equals, groupTeam.Group.Phase.PhaseIndex), 
					new Condition("NGROUP", ConditionType.Equals, groupTeam.Group.GroupIndex), 
					new Condition("TEAM_ID", ConditionType.Equals, groupTeam.Team.ID)
				},
				new ConditionLogic[] {
					ConditionLogic.None, ConditionLogic.And, ConditionLogic.And, 
					ConditionLogic.And
				 });
			
			//check if exists:
			MirDataReader reader=
				GetTableData(strTableName, fieldsToSelect, groupTeamCondition, null);
			bool blnExists=reader.Read();
			reader.Close();
			groupTeamCondition.MoveFirst();
			
			//build proper statement:
			if (blnExists)
			{
				UpdateTableData(strTableName, fields, groupTeamCondition);
			}
			else
			{
				InsertTableData(strTableName, fields);
			}
			
			return blnExists;
		}
		#endregion
		#endregion

		#region Delete methods
		/// <summary>
		/// delete ALL championship data!! 
		/// (including phases, rounds, matches, competitions, heats etc...
		/// returns amount of deleted records.
		/// </summary>
		public static int DeleteChampionshipData(int champID)
		{
			int result=0;
			
			ConditionCollection deleteCondition=new ConditionCollection(
				new Condition("CHAMPIONSHIP_ID", ConditionType.Equals, champID), 
				ConditionLogic.None);
			
			//get full details:
			ChampionshipData champData=GetFullChampionship(champID);

			//championships
			result += DeleteTableData("CHAMPIONSHIPS", deleteCondition);
			
			//categories:
			if (champData.Categories != null)
			{
				foreach (CategoryData category in champData.Categories)
				{
					deleteCondition=new ConditionCollection(
						new Condition("CATEGORY_ID", ConditionType.Equals, category.ID), 
						ConditionLogic.None);
					
					result += DeleteTableData("CATEGORIES", deleteCondition, true);
					
					//phases:
					result += DeleteTableData("PHASES", deleteCondition, true);
					
					//rounds:
					result += DeleteTableData("ROUNDS", deleteCondition, true);
					
					//phases:
					result += DeleteTableData("PHASES", deleteCondition, true);
					
					//matches:
					result += DeleteTableData("MATCHES", deleteCondition, true);
					
					//competitions:
					result += DeleteTableData("COMPETITIONS", deleteCondition, true);
					
					//heats:
					result += DeleteTableData("HEATS", deleteCondition, true);
					
					//competitors:
					result += DeleteTableData("COMPETITORS", deleteCondition, true);
					
					//group teams:
					result += DeleteTableData("GROUP_TEAMS", deleteCondition, true);
					
					//groups:
					result += DeleteTableData("GROUPS", deleteCondition, true);
				}
			}

			if (champData.ID >= 0)
			{
				//ruleset:
				if ((champData.Ruleset != null)&&(champData.Ruleset.ID >= 0))
				{
					//check if used in other championships:
					bool exists=false;
					int[] champs=GetRulesetChampionships(champData.Ruleset.ID);
					for (int i=0; i<champs.Length; i++)
					{
						if (champs[i] != champData.ID)
						{
							exists = true;
							break;
						}
					}
					
					//remove if does not exist in other championship:
					if (!exists)
					{
						deleteCondition=new ConditionCollection(
							new Condition("RULESET_ID", ConditionType.Equals, 
							champData.Ruleset.ID), ConditionLogic.None);
						result += DeleteTableData(
							"RULESETS", deleteCondition, true);

						//rules:
						deleteCondition=new ConditionCollection(
							new Condition("RULESET_ID", ConditionType.Equals, 
							champData.Ruleset.ID), ConditionLogic.None);
						result += DeleteTableData(
							"RULES", deleteCondition, true);
					}
				}

				foreach (CategoryData category in champData.Categories)
				{
					foreach (PhaseData phase in category.Phases)
					{
						foreach (GroupData group in phase.Groups)
						{
							//courts:
							CourtData[] courts=GetGroupCourts(group);
							foreach (CourtData court in courts)
							{
								//check if used in other championships:
								bool exists=false;
								int[] champs=GetCourtChampionships(court.ID);
								for (int i=0; i<champs.Length; i++)
								{
									if (champs[i] != champID)
									{
										exists = true;
										break;
									}
								}
								
								//remove if does not exist in other championship:
								if (!exists)
								{
									deleteCondition=new ConditionCollection(
										new Condition("COURT_ID", ConditionType.Equals, 
										court.ID), ConditionLogic.None);
									result += DeleteTableData(
										"COURTS", deleteCondition, true);
								}
							} //end loop over courts of this group
							
							//facilities:
							FacilityData[] facilities=GetGroupFacilities(group);
							foreach (FacilityData facility in facilities)
							{
								//check if used in other championships:
								bool exists=false;
								int[] champs=GetFacilityChampionships(facility.ID);
								for (int i=0; i<champs.Length; i++)
								{
									if (champs[i] != champID)
									{
										exists = true;
										break;
									}
								} //end if exists
								
								//remove if does not exist in other championship:
								if (!exists)
								{
									deleteCondition=new ConditionCollection(
										new Condition("FACILITY_ID", ConditionType.Equals, 
										facility.ID), ConditionLogic.None);
									result += DeleteTableData(
										"FACILITIES", deleteCondition, true);
								} //end if exists
							} //end loop over facilities of this group
						} //end loop over groups of this phahse
					} //end loop over phases of this category
				} //end loop over categories of this championship
			} //end if championship exists
			
			return result;
		}
		#endregion

		#region create local tables
		/// <summary>
		/// creates all the local tables. drops any table if already exists.
		/// </summary>
		public static void CreateLocalTables()
		{
			ReCreateTable("CATEGORIES", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "CHAMPIONSHIP_ID"),
								new Field(FieldType.Integer, "CATEGORY")
							});
			ReCreateTable("COMPETITORS", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Integer, "NGROUP"),
								new Field(FieldType.Integer, "COMPETITION"),
								new Field(FieldType.Integer, "COMPETITOR"),
								new Field(FieldType.Integer, "PLAYER_ID"),
								new Field(FieldType.Integer, "HEAT"),
								new Field(FieldType.Integer, "POSITION"),
								new Field(FieldType.Integer, "RESULT_POSITION"),
								new Field(FieldType.Text, "RESULT"),
								new Field(FieldType.Integer, "SCORE")
							});
			ReCreateTable("HEATS", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Integer, "NGROUP"),
								new Field(FieldType.Integer, "COMPETITION"),
								new Field(FieldType.Integer, "HEAT"),
								new Field(FieldType.DateTime, "TIME"),
								new Field(FieldType.Integer, "FACILITY_ID"),
								new Field(FieldType.Integer, "COURT_ID")
							});
			ReCreateTable("COMPETITIONS", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Integer, "NGROUP"),
								new Field(FieldType.Integer, "COMPETITION"),
								new Field(FieldType.Integer, "SPORT_FIELD_ID"),
								new Field(FieldType.DateTime, "TIME"),
								new Field(FieldType.Integer, "FACILITY_ID"),
								new Field(FieldType.Integer, "COURT_ID")
							});
			ReCreateTable("GROUP_TEAMS", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Integer, "NGROUP"),
								new Field(FieldType.Integer, "POSITION"),
								new Field(FieldType.Integer, "PREVIOUS_GROUP"),
								new Field(FieldType.Integer, "PREVIOUS_POSITION"),
								new Field(FieldType.Integer, "TEAM_ID"),
								new Field(FieldType.Integer, "GAMES"),
								new Field(FieldType.Integer, "POINTS"),
								new Field(FieldType.Integer, "POINTS_AGAINST"),
								new Field(FieldType.Integer, "SMALL_POINTS"),
								new Field(FieldType.Integer, "SMALL_POINTS_AGAINST"),
								new Field(FieldType.Double, "SCORE"),
								new Field(FieldType.Integer, "RESULT_POSITION")
							});
			ReCreateTable("GROUPS", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Integer, "NGROUP"),
								new Field(FieldType.Text, "GROUP_NAME"),
								new Field(FieldType.Integer, "STATUS") //0 plan, 1 ready, 2 started, 3 ended
							});
			ReCreateTable("MATCHES", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Integer, "NGROUP"),
								new Field(FieldType.Integer, "ROUND"),
								new Field(FieldType.Integer, "CYCLE"),
								new Field(FieldType.Integer, "MATCH"),
								new Field(FieldType.Integer, "TEAM_A"),
								new Field(FieldType.Integer, "TEAM_B"),
								new Field(FieldType.DateTime, "TIME"),
								new Field(FieldType.Integer, "FACILITY_ID"),
								new Field(FieldType.Integer, "COURT_ID"),
								new Field(FieldType.Double, "TEAM_A_SCORE"),
								new Field(FieldType.Double, "TEAM_B_SCORE"),
								new Field(FieldType.Integer, "RESULT"), //0 tie, 1 teamA win, 2 teamB win, 3 teamA technical win, 4 teamB technical win
								new Field(FieldType.Text, "PARTS_RESULT")
							});
			ReCreateTable("PHASES", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Text, "PHASE_NAME"),
								new Field(FieldType.Integer, "STATUS") //0 plan, 1 ready, 2 started, 3 ended
							});
			ReCreateTable("CHAMPIONSHIP_REGIONS", 
				new Field[] {
								new Field(FieldType.Integer, "CHAMPIONSHIP_REGION_ID"),
								new Field(FieldType.Integer, "CHAMPIONSHIP_ID"),
								new Field(FieldType.Integer, "REGION_ID")
							});
			ReCreateTable("ROUNDS", 
				new Field[] {
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "PHASE"),
								new Field(FieldType.Integer, "NGROUP"),
								new Field(FieldType.Integer, "ROUND"),
								new Field(FieldType.Text, "ROUND_NAME"),
								new Field(FieldType.Integer, "STATUS") //0 plan, 1 ready, 2 started, 3 ended
							});
			ReCreateTable("CHAMPIONSHIPS", 
				new Field[] {
								new Field(FieldType.Integer, "CHAMPIONSHIP_ID"),
								new Field(FieldType.Integer, "SEASON"),
								new Field(FieldType.Text, "CHAMPIONSHIP_NAME"),
								new Field(FieldType.Integer, "REGION_ID"),
								new Field(FieldType.Integer, "SPORT_ID"),
								new Field(FieldType.Integer, "IS_CLUBS"),
								new Field(FieldType.DateTime, "LAST_REGISTRATION_DATE"),
								new Field(FieldType.DateTime, "START_DATE"),
								new Field(FieldType.DateTime, "END_DATE"),
								new Field(FieldType.DateTime, "ALT_START_DATE"),
								new Field(FieldType.DateTime, "ALT_END_DATE"),
								new Field(FieldType.DateTime, "FINALS_DATE"),
								new Field(FieldType.DateTime, "ALT_FINALS_DATE"),
								new Field(FieldType.Integer, "RULESET_ID"),
								new Field(FieldType.Integer, "IS_OPEN"), //0 or 1, for central championships only.
								new Field(FieldType.Integer, "CHAMPIONSHIP_STATUS"),
								new Field(FieldType.Text, "CHAMPIONSHIP_SUPERVISOR"),
								new Field(FieldType.Text, "STANDARD_CHAMPIONSHIP")
							});
			ReCreateTable("SCORE_RANGES", 
				new Field[] {
								new Field(FieldType.Integer, "SCORE_RANGE_ID"),
								new Field(FieldType.Integer, "SPORT_FIELD_TYPE"),
								new Field(FieldType.Integer, "SPORT_FIELD"),
								new Field(FieldType.Integer, "CATEGORY"),
								new Field(FieldType.Integer, "RANGE_LOWER_LIMIT"),
								new Field(FieldType.Integer, "RANGE_UPPER_LIMIT"),
								new Field(FieldType.Integer, "SCORE")
							});
			ReCreateTable("COURT_TYPES", 
				new Field[] {
								new Field(FieldType.Integer, "COURT_TYPE_ID"),
								new Field(FieldType.Text, "COURT_TYPE_NAME")
							});
			ReCreateTable("COURTS", 
				new Field[] {
								new Field(FieldType.Integer, "COURT_ID"),
								new Field(FieldType.Text, "COURT_NAME"),
								new Field(FieldType.Integer, "FACILITY_ID"),
								new Field(FieldType.Integer, "COURT_TYPE_ID")
							});
			ReCreateTable("FACILITIES", 
				new Field[] {
								new Field(FieldType.Integer, "FACILITY_ID"),
								new Field(FieldType.Text, "FACILITY_NAME"),
								new Field(FieldType.Integer, "REGION_ID"),
								new Field(FieldType.Integer, "SCHOOL_ID"),
								new Field(FieldType.Text, "ADDRESS"),
								new Field(FieldType.Text, "PHONE"),
								new Field(FieldType.Text, "FAX")
							});
			ReCreateTable("PLAYERS", 
				new Field[] {
								new Field(FieldType.Integer, "PLAYER_ID"),
								new Field(FieldType.Integer, "STUDENT_ID"),
								new Field(FieldType.Integer, "TEAM_ID"),
								new Field(FieldType.Integer, "SHIRT_NUMBER"),
								new Field(FieldType.Integer, "CHIP_NUMBER"),
								new Field(FieldType.Integer, "STATUS"),
								new Field(FieldType.Text, "REMARKS"),
								new Field(FieldType.DateTime, "REGISTRATION_DATE")
			});
			ReCreateTable("REFEREES", 
				new Field[] {
								new Field(FieldType.Integer, "REFEREE_ID"),
								new Field(FieldType.Text, "REFEREE_NAME"),
								new Field(FieldType.Integer, "REFEREE_TYPE")
							});
			ReCreateTable("REGIONS", 
				new Field[] {
								new Field(FieldType.Integer, "REGION_ID"),
								new Field(FieldType.Text, "REGION_NAME"),
								new Field(FieldType.Text, "ADDRESS"),
								new Field(FieldType.Text, "PHONE"),
								new Field(FieldType.Text, "FAX")
							});
			ReCreateTable("RULE_TYPES", 
				new Field[] {
								new Field(FieldType.Integer, "RULE_TYPE_ID"),
								new Field(FieldType.Text, "CLASS")
							});
			ReCreateTable("RULES", 
				new Field[] {
								new Field(FieldType.Integer, "RULE_ID"),
								new Field(FieldType.Integer, "RULESET_ID"),
								new Field(FieldType.Integer, "RULE_TYPE_ID"),
								new Field(FieldType.Text, "VALUE"),
								new Field(FieldType.Integer, "SPORT_FIELD_TYPE_ID"),
								new Field(FieldType.Integer, "SPORT_FIELD_ID"),
								new Field(FieldType.Integer, "CATEGORY")
							});
			ReCreateTable("RULESETS", 
				new Field[] {
								new Field(FieldType.Integer, "RULESET_ID"),
								new Field(FieldType.Text, "RULESET_NAME"),
								new Field(FieldType.Integer, "SPORT_ID"),
								new Field(FieldType.Integer, "REGION_ID")
							});
			ReCreateTable("SCHOOLS", 
				new Field[] {
								new Field(FieldType.Integer, "SCHOOL_ID"),
								new Field(FieldType.Text, "SYMBOL"),
								new Field(FieldType.Text, "SCHOOL_NAME"),
								new Field(FieldType.Text, "CITY_NAME"),
								new Field(FieldType.Integer, "FROM_GRADE"),
								new Field(FieldType.Integer, "TO_GRADE"),
								new Field(FieldType.Integer, "REGION_ID"),
								new Field(FieldType.Integer, "CLUB_STATUS")
							});
			ReCreateTable("SPORT_FIELD_TYPES", 
				new Field[] {
								new Field(FieldType.Integer, "SPORT_FIELD_TYPE_ID"),
								new Field(FieldType.Integer, "SPORT_ID"),
								new Field(FieldType.Text, "SPORT_FIELD_TYPE_NAME")
							});
			ReCreateTable("SPORT_FIELDS", 
				new Field[] {
								new Field(FieldType.Integer, "SPORT_FIELD_ID"),
								new Field(FieldType.Integer, "SPORT_FIELD_TYPE_ID"),
								new Field(FieldType.Text, "SPORT_FIELD_NAME")
							});
			ReCreateTable("SPORTS", 
				new Field[] {
								new Field(FieldType.Integer, "SPORT_ID"),
								new Field(FieldType.Text, "SPORT_NAME"),
								new Field(FieldType.Integer, "SPORT_TYPE"), //1 Competition 2 Match
								new Field(FieldType.Integer, "RULESET_ID")
							});
			ReCreateTable("STUDENTS", 
				new Field[] {
								new Field(FieldType.Integer, "STUDENT_ID"),
								new Field(FieldType.Integer, "ID_NUMBER"),
								new Field(FieldType.Text, "FIRST_NAME"),
								new Field(FieldType.Text, "LAST_NAME"),
								new Field(FieldType.DateTime, "BIRTH_DATE"),
								new Field(FieldType.Integer, "SCHOOL_ID"),
								new Field(FieldType.Integer, "GRADE"),
								new Field(FieldType.Integer, "SEX_TYPE")
							});
			ReCreateTable("TEAMS", 
				new Field[] {
								new Field(FieldType.Integer, "TEAM_ID"),
								new Field(FieldType.Integer, "SCHOOL_ID"),
								new Field(FieldType.Integer, "CHAMPIONSHIP_ID"),
								new Field(FieldType.Integer, "CATEGORY_ID"),
								new Field(FieldType.Integer, "STATUS"),
								new Field(FieldType.Integer, "TEAM_INDEX"),
								new Field(FieldType.Text, "TEAM_SUPERVISOR"),
								new Field(FieldType.DateTime, "REGISTRATION_DATE")
							});
		}

/*
			
			-----------------------------------------------
			
*/

		#region raw SQL
		#region CHAMPIONSHIP_CATEGORIES
			/* CREATE TABLE CHAMPIONSHIP_CATEGORIES (
			CHAMPIONSHIP_CATEGORY_ID	int		IDENTITY (1,1),
			CHAMPIONSHIP_ID				int		NOT NULL,
			CATEGORY					int		NOT NULL,
			REGISTRATION_PRICE			real	NOT NULL	DEFAULT 0 */
		#endregion
		#region CHAMPIONSHIP_COMPETITION_COMPETITORS
		/* CREATE TABLE CHAMPIONSHIP_COMPETITION_COMPETITORS (
		CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
		PHASE						int	NOT NULL,
		NGROUP						int	NOT NULL,
		COMPETITION					int	NOT NULL,
		COMPETITOR					int NOT NULL,
		PLAYER_ID					int,
		HEAT						int,
		POSITION					int,
		RESULT_POSITION				int,
		RESULT						nvarchar(20),
		SCORE						int */
		#endregion
		#region CHAMPIONSHIP_COMPETITION_HEATS
			/* CREATE TABLE CHAMPIONSHIP_COMPETITION_HEATS (
			CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
			PHASE						int	NOT NULL,
			NGROUP						int	NOT NULL,
			COMPETITION					int	NOT NULL,
			HEAT						int NOT NULL,
			TIME						datetime,
			FACILITY_ID					int,
			COURT_ID					int */
		#endregion
		#region CHAMPIONSHIP_COMPETITIONS
		/* CREATE TABLE CHAMPIONSHIP_COMPETITIONS (
			CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
			PHASE						int	NOT NULL,
			NGROUP						int	NOT NULL,
			COMPETITION					int	NOT NULL,
			SPORT_FIELD_ID				int NOT NULL,
			TIME						datetime,
			FACILITY_ID					int,
			COURT_ID					int */
		#endregion
		#region CHAMPIONSHIP_GROUP_TEAMS
		/* 	CREATE TABLE CHAMPIONSHIP_GROUP_TEAMS (
			CHAMPIONSHIP_CATEGORY_ID	int NOT NULL,
			PHASE						int	NOT NULL,
			NGROUP						int	NOT NULL,
			POSITION					int NOT NULL,
			PREVIOUS_GROUP				int,
			PREVIOUS_POSITION			int,
			TEAM_ID						int,
			GAMES						int,
			POINTS						int,
			POINTS_AGAINST				int,
			SMALL_POINTS				int,
			SMALL_POINTS_AGAINST		int,
			SCORE						real,
			RESULT_POSITION				int */
		#endregion
		#region CHAMPIONSHIP_GROUPS
		/* 			CREATE TABLE CHAMPIONSHIP_GROUPS (
			CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
			PHASE						int				NOT NULL,
			NGROUP						int				NOT NULL,
			GROUP_NAME					nvarchar(100)	NOT NULL,
			STATUS						int				NOT NULL -- 0 plan, 1 ready, 2 started, 3 ended */
		#endregion
		#region CHAMPIONSHIP_MATCHES
		/* CREATE TABLE CHAMPIONSHIP_MATCHES (
			CHAMPIONSHIP_CATEGORY_ID	int	NOT NULL,
			PHASE						int	NOT NULL,
			NGROUP						int	NOT NULL,
			ROUND						int	NOT NULL,
			CYCLE						int	NOT NULL DEFAULT 0,
			MATCH						int NOT NULL,
			TEAM_A						int NOT NULL,
			TEAM_B						int NOT NULL,
			TIME						datetime,
			FACILITY_ID					int,
			COURT_ID					int,
			TEAM_A_SCORE				real,
			TEAM_B_SCORE				real,
			RESULT						int, -- 0 tie, 1 teamA win, 2 teamB win, 3 teamA technical win, 4 teamB technical win
			PARTS_RESULT				nvarchar(2000) */
		#endregion
		#region CHAMPIONSHIP_PHASES
		/* CREATE TABLE CHAMPIONSHIP_PHASES (
			CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
			PHASE						int				NOT NULL,
			PHASE_NAME					nvarchar(100)	NOT NULL,
			STATUS						int				NOT NULL -- 0 plan, 1 ready, 2 started, 3 ended */
		#endregion
		#region CHAMPIONSHIP_REGIONS
		/* CREATE TABLE CHAMPIONSHIP_REGIONS (
			CHAMPIONSHIP_REGION_ID	int		IDENTITY (1,1),
			CHAMPIONSHIP_ID			int		NOT NULL,
			REGION_ID				int		NOT NULL */
		#endregion
		#region CHAMPIONSHIP_ROUNDS
		/* 	CREATE TABLE CHAMPIONSHIP_ROUNDS (
			CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
			PHASE						int				NOT NULL,
			NGROUP						int				NOT NULL,
			ROUND						int				NOT NULL,
			ROUND_NAME					nvarchar(100)	NOT NULL,
			STATUS						int				NOT NULL -- 0 plan, 1 ready, 2 started, 3 ended */
		#endregion
		#region CHAMPIONSHIPS
		/* CREATE TABLE CHAMPIONSHIPS (
			CHAMPIONSHIP_ID				int				IDENTITY (1,1),
			SEASON						int				NOT NULL,
			CHAMPIONSHIP_NAME			nvarchar(50),
			REGION_ID					int,
			SPORT_ID					int,
			IS_CLUBS					int				NOT NULL,
			LAST_REGISTRATION_DATE		datetime,
			START_DATE					datetime,
			END_DATE					datetime,
			ALT_START_DATE				datetime,
			ALT_END_DATE				datetime,
			FINALS_DATE					datetime,
			ALT_FINALS_DATE				datetime,
			RULESET_ID					int,
			IS_OPEN						int,		--0/1 for central championships only.
			CHAMPIONSHIP_STATUS			int				NOT NULL	DEFAULT 0,  --0/1/2
			CHAMPIONSHIP_SUPERVISOR		int,
			STANDARD_CHAMPIONSHIP_ID	int */
		#endregion
		#region COMPETITOR_SCORE_RANGES
		/*	CREATE TABLE COMPETITOR_SCORE_RANGES (
			COMPETITOR_SCORE_RANGE_ID	int	IDENTITY (1,1),
			SPORT_FIELD_TYPE			int	NOT NULL,
			SPORT_FIELD					int	NOT NULL,
			CATEGORY					int,
			RANGE_LOWER_LIMIT			int,
			RANGE_UPPER_LIMIT			int,
			SCORE						int NOT NULL */
		#endregion
		#region COURT_TYPES
		/* 	CREATE TABLE COURT_TYPES (
			COURT_TYPE_ID		int				IDENTITY (1,1),
			COURT_TYPE_NAME		nvarchar(30)	NOT NULL */
		#endregion
		#region COURTS
		/* CREATE TABLE COURTS (
			COURT_ID		int				IDENTITY (1,1),
			COURT_NAME		nvarchar(30)	NOT NULL,
			FACILITY_ID		int NOT NULL,
			COURT_TYPE_ID	int */
		#endregion
		#region FACILITIES
		/* CREATE TABLE FACILITIES (
			FACILITY_ID		int				IDENTITY (1,1),
			FACILITY_NAME	nvarchar(30)	NOT NULL,
			REGION_ID		int,
			SCHOOL_ID		int,
			ADDRESS			nvarchar(70),
			PHONE			nvarchar(15),
			FAX				nvarchar(15) */
		#endregion
		#region PLAYERS
		/* CREATE TABLE  PLAYERS (
			PLAYER_ID			int		IDENTITY (1,1),
			STUDENT_ID			int		NOT NULL,
			TEAM_ID				int		NOT NULL,
			TEAM_NUMBER			int,
			CHARGE_ID			int,
			STATUS				int,
			REMARKS				nvarchar(100), 
			REGISTRATION_DATE	datetime	NOT NULL DEFAULT GETDATE() */
		#endregion
		#region REFEREES
		/* CREATE TABLE REFEREES (
			REFEREE_ID		int				IDENTITY (1,1),
			REFEREE_NAME	nvarchar(50)	NOT NULL,
			REFEREE_TYPE	int				NOT NULL	--Minor/Senior */
		#endregion
		#region REGIONS
		/* CREATE TABLE REGIONS (
			REGION_ID	int				IDENTITY (1,1),
			REGION_NAME	nvarchar(15)	NOT NULL,
			ADDRESS		nvarchar(70),
			PHONE		nvarchar(15),
			FAX			nvarchar(15) */
		#endregion
		#region RULE_TYPES
		/* CREATE TABLE RULE_TYPES (
			RULE_TYPE_ID	int,
			CLASS			nvarchar(300)	NOT NULL */
		#endregion
		#region RULES
		/* CREATE TABLE RULES (
			RULE_ID					int		IDENTITY (1,1),
			RULESET_ID				int		NOT NULL,
			RULE_TYPE_ID			int		NOT NULL,
			VALUE					ntext,
			SPORT_FIELD_TYPE_ID		int,
			SPORT_FIELD_ID			int,
			CATEGORY				int */
		#endregion
		#region RULESETS
		/* CREATE TABLE RULESETS (
			RULESET_ID		int				IDENTITY (1,1),
			RULESET_NAME	nvarchar(30)	NOT NULL,
			SPORT_ID		int,
			REGION_ID		int */
		#endregion
		#region SCHOOLS
		/* CREATE TABLE SCHOOLS (
			SCHOOL_ID			int				IDENTITY (1,1),
			SYMBOL				nchar(7)		NOT NULL,
			SCHOOL_NAME			nvarchar(30)	NOT NULL,
			CITY_ID				int,
			ADDRESS				nvarchar(70),
			MAIL_ADDRESS		nvarchar(70),
			MAIL_CITY_ID		int,
			ZIP_CODE			nvarchar(5),
			EMAIL				nvarchar(100),
			PHONE				nvarchar(15),
			FAX					nvarchar(15),
			MANAGER_NAME		nvarchar(20),
			FROM_GRADE			int,
			TO_GRADE			int,
			SUPERVISION_TYPE	int,
			SECTOR_TYPE			int,
			REGION_ID			int				NOT NULL,
			CLUB_STATUS			int				NOT NULL,
			CLUB_CHARGE_ID		int */
		#endregion
		#region SPORT_FIELD_TYPES
		/* CREATE TABLE SPORT_FIELD_TYPES (
			SPORT_FIELD_TYPE_ID		int		IDENTITY (1,1),
			SPORT_ID				int,
			SPORT_FIELD_TYPE_NAME	nvarchar(50) */
		#endregion
		#region SPORT_FIELDS
		/* CREATE TABLE SPORT_FIELDS (
			SPORT_FIELD_ID		int		IDENTITY (1,1),
			SPORT_FIELD_TYPE_ID int,
			SPORT_FIELD_NAME	nvarchar(50) */
		#endregion
		#region SPORTS
		/* CREATE TABLE SPORTS (
			SPORT_ID	int				IDENTITY (1,1),
			SPORT_NAME	nvarchar(50),
			SPORT_TYPE	int				NOT NULL -- 1 Competition 2 Match */
		#endregion
		#region STUDENTS
		/* CREATE TABLE STUDENTS (
			STUDENT_ID	int				IDENTITY (1,1),
			ID_NUMBER_TYPE	int,
			ID_NUMBER	int,
			FIRST_NAME	nvarchar(15)	NOT NULL,
			LAST_NAME	nvarchar(20)	NOT NULL,
			BIRTH_DATE	datetime,
			SCHOOL_ID	int				NOT NULL,
			GRADE		int,
			SEX_TYPE	int */
		#endregion
		#region TEAMS
		/* CREATE TABLE TEAMS (
			TEAM_ID						int				IDENTITY (1,1),
			SCHOOL_ID					int				NOT NULL,
			CHAMPIONSHIP_ID				int				NOT NULL,
			CHAMPIONSHIP_CATEGORY_ID	int				NOT NULL,
			STATUS						int				NOT NULL,
			TEAM_INDEX					int				DEFAULT 0, 
			CHARGE_ID					int,
			TEAM_SUPERVISOR				int,
			REGISTRATION_DATE			datetime	NOT NULL DEFAULT GETDATE() */
		#endregion
		#endregion
		#endregion
	}
}
