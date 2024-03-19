using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace ImportSportFlowersPlayers
{
	public static class DatabaseLayer
	{
		public static bool VerifyDatabaseConnection(out string errorMsg)
		{
			SqlConnection connection;
			bool result = TryGetConnection(out connection, out errorMsg);
			if (connection != null)
				connection.Dispose();
			return result;
		}

		public static Dictionary<string, int> GetAllMunicipalities()
		{
			Dictionary<string, int> municipalities = new Dictionary<string, int>();
			using (SqlConnection connection = GetOpenConnection())
			{
				string strSQL = "Select [Seq], [Name] From Organizations Where [Type]=2";
				using (SqlCommand command = new SqlCommand(strSQL, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string currentName = reader["Name"] + "";
							if (currentName.Length > 0 && !municipalities.ContainsKey(currentName))
								municipalities.Add(currentName, reader.GetInt32(0));
						}
					}
				}
			}
			return municipalities;
		}

		public static void ImportSinglePlayer(int year, int muniSeq, PlayerRowItem player, out QueryExecutionResult result)
		{
			result = QueryExecutionResult.None;
			string updateSQL = "Update Population Set [FirstName]=@firstname, [LastName]=@lastname, [BirthDate]=@birthdate, [Gender]=@gender " +
				"Where [Year]=@year And MunicipalitySeq=@muni And [Id]=@idnumber";
			string insertSQL = "Insert Into Population ([Seq], [MunicipalitySeq], [Year], [Id], [FirstName], [LastName], [BirthDate], [Gender]) " +
				"Values (@seq, @muni, @year, @idnumber, @firstname, @lastname, @birthdate, @gender)";
			using (SqlConnection connection = GetOpenConnection())
			{
				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = connection;
					command.Parameters.AddWithValue("@year", year);
					command.Parameters.AddWithValue("@muni", muniSeq);
					command.Parameters.AddWithValue("@idnumber", player.IdNumber);
					command.Parameters.AddWithValue("@lastname", player.LastName);
					command.Parameters.AddWithValue("@firstname", player.FirstName);
					command.Parameters.AddWithValue("@birthdate", player.Birthdate);
					command.Parameters.AddWithValue("@gender", (int)player.Gender);
					if (PlayerExists(connection, year, muniSeq, player))
					{
						command.CommandText = updateSQL;
						command.ExecuteNonQuery();
						result = QueryExecutionResult.Updated;
					}
					else
					{
						int newSeq = GetMaxSeq(connection, "Population") + 1;
						command.Parameters.AddWithValue("@seq", newSeq);
						command.CommandText = insertSQL;
						command.ExecuteNonQuery();
						result = QueryExecutionResult.Inserted;
						string strSQL = "Update Sequences Set [Value]=@value Where [TableName]='Population'";
						command.Parameters.Clear();
						command.CommandText = strSQL;
						command.Parameters.AddWithValue("@value", newSeq);
						command.ExecuteNonQuery();
					}
				}
			}
		}

		private static int GetMaxSeq(SqlConnection connection, string tableName)
		{
			int maxSeq = 0;
			string strSQL = "Select [Value] From Sequences Where [TableName]=@table";
			using (SqlCommand command = new SqlCommand(strSQL, connection))
			{
				command.Parameters.AddWithValue("@table", tableName);
				object rawValue = command.ExecuteScalar();
				if (rawValue != null && !(rawValue is DBNull))
					maxSeq = (int)rawValue;
			}
			return maxSeq;
		}

		private static bool PlayerExists(SqlConnection connection, int year, int muniSeq, PlayerRowItem player)
		{
			bool exists = false;
			string strSQL = "Select [Seq] From Population Where [Year]=@year And MunicipalitySeq=@muni And [Id]=@idnumber";
			using (SqlCommand command = new SqlCommand(strSQL, connection))
			{
				command.Parameters.AddWithValue("@year", year);
				command.Parameters.AddWithValue("@muni", muniSeq);
				command.Parameters.AddWithValue("@idnumber", player.IdNumber);
				using (SqlDataReader reader = command.ExecuteReader())
				{
					exists = reader.Read();
				}
			}
			return exists;
		}

		private static SqlConnection GetOpenConnection()
		{
			SqlConnection connection;
			string errorMsg;
			TryGetConnection(out connection, out errorMsg);
			return connection;
		}

		private static bool TryGetConnection(out SqlConnection connection, out string errorMsg)
		{
			connection = null;
			errorMsg = "";
			string strConnectionString = "";
			ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["MainConnection"];
			if (setting != null)
				strConnectionString = setting.ConnectionString + "";
			if (strConnectionString.Length == 0)
			{
				errorMsg = "Missing connection string";
				return false;
			}

			connection = new SqlConnection(strConnectionString);
			try
			{
				connection.Open();
			}
			catch (Exception ex)
			{
				errorMsg = "Error opening database connection: " + ex.Message;
			}
			return (errorMsg.Length == 0);
		}
	}
}
