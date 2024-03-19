using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ImportBlackListedPlayers
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

		public static void ImportSinglePlayer(PlayerRowItem player, out QueryExecutionResult result)
		{
			result = QueryExecutionResult.None;
			string updateSQL = "Update PlayersBlackList Set [FULL_NAME]=@fullname, [SPORT_NAME]=@sportname " +
				"Where [ID_NUMBER]=@idnumber";
			string insertSQL = "Insert Into PlayersBlackList ([ID_NUMBER], [FULL_NAME], [SPORT_NAME]) " +
				"Values (@idnumber, @fullname, @sportname)";
			using (SqlConnection connection = GetOpenConnection())
			{
				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = connection;
					command.Parameters.AddWithValue("@idnumber", player.IdNumber);
					command.Parameters.AddWithValue("@fullname", player.FullName);
					command.Parameters.AddWithValue("@sportname", player.SportName);
					if (PlayerExists(connection, player.IdNumber))
					{
						command.CommandText = updateSQL;
						command.ExecuteNonQuery();
						result = QueryExecutionResult.Updated;
					}
					else
					{
						command.CommandText = insertSQL;
						command.ExecuteNonQuery();
						result = QueryExecutionResult.Inserted;
					}
				}
			}
		}

		private static bool PlayerExists(SqlConnection connection, string idNumber)
		{
			bool exists = false;
			string strSQL = "Select * From PlayersBlackList Where [ID_NUMBER]=@idnumber";
			using (SqlCommand command = new SqlCommand(strSQL, connection))
			{
				command.Parameters.AddWithValue("@idnumber", idNumber);
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
