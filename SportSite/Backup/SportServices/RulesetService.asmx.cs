using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using Sport.Core;
using ISF.DataLayer;
using System.Collections.Generic;

namespace SportServices
{
	/// <summary>
	/// Summary description for RulesetService.
	/// </summary>
	[WebService(Namespace = "http://www.mir.co.il/")]
	public class RulesetService : System.Web.Services.WebService
	{
		public RulesetService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
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


		#region Rulesets

		#region Classes

		public class RuleType
		{
			public int Id;
			public string Class;
		}

		public class Rule
		{
			public int Id;
			public int RuleType;
			public string Value;
			public int SportFieldType;
			public int SportField;
			public int Category;
		}

		public class Ruleset
		{
			public int Id;
			public string Name;
			public int Sport;
			public int SportType;
			public int SportRuleset;
			public Rule[] Rules;
		}

		#endregion

		[WebMethod]
		public object[] GetScoreTablesByFormat(string format)
		{
			string strSQL = "SELECT RULESET_ID, SPORT_FIELD_TYPE_ID, " +
				"SPORT_FIELD_ID, VALUE " +
				"FROM RULES WHERE RULE_TYPE_ID=7 AND DATE_DELETED IS NULL";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			List<int[]> arrData = new List<int[]>();
			table.Rows.ForEach(row =>
			{
				string strCurFormat = row["VALUE"].ToString();
				if (strCurFormat == format)
				{
					int ruleset = row.GetIntOrDefault("RULESET_ID", -1);
					int sportFieldType = row.GetIntOrDefault("SPORT_FIELD_TYPE_ID", -1);
					int sportField = row.GetIntOrDefault("SPORT_FIELD_ID", -1);
					if (ruleset >= 0)
					{
						int[] data = new int[] { ruleset, sportFieldType, sportField };
						arrData.Add(data);
					}
				}
			});

			if (arrData.Count == 0)
				return new object[] { };

			strSQL = "SELECT RULESET_ID, SPORT_FIELD_TYPE_ID, SPORT_FIELD_ID, " +
				"CATEGORY FROM RULES WHERE RULE_TYPE_ID=9 AND " +
				"DATE_DELETED IS NULL AND (";
			ArrayList result = new ArrayList();
			int x = 0;
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			foreach (int[] data in arrData)
			{
				int rulesetID = data[0];
				int sportFieldTypeID = data[1];
				int sportFieldID = data[2];
				if (x > 0)
					strSQL += " OR ";
				strSQL += "(RULESET_ID=@r" + x + " AND SPORT_FIELD_TYPE_ID=@s" + x;
				parameters.Add(new SimpleParameter("@r" + x, rulesetID));
				parameters.Add(new SimpleParameter("@s" + x, sportFieldTypeID));
				if (sportFieldID >= 0)
				{
					strSQL += " AND SPORT_FIELD_ID=@f" + x;
					parameters.Add(new SimpleParameter("@f" + x, sportFieldID));
				}
				strSQL += ")";
				x++;
			}
			strSQL += ")";
			table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			table.Rows.ForEach(row =>
			{
				int ruleset = row.GetIntOrDefault("RULESET_ID", -1);
				int sportFieldType = row.GetIntOrDefault("SPORT_FIELD_TYPE_ID", -1);
				int sportField = row.GetIntOrDefault("SPORT_FIELD_ID", -1);
				int category = row.GetIntOrDefault("CATEGORY", -1);
				if (ruleset >= 0)
				{
					string data = ruleset + "|" + sportFieldType + "|" + sportField + "|" + category;
					result.Add(data);
				}
			});
			
			return result.ToArray();
		}

		[WebMethod]
		public string GetPointsName(int sportID)
		{
			string strSQL = "SELECT POINTS_NAME FROM SPORTS WHERE SPORT_ID=@1";
			return DB.Instance.ExecuteScalar(strSQL, "",
				new SimpleParameter("@1", sportID)).ToString();
		}

		[WebMethod]
		public RuleType[] GetRuleTypes()
		{
			List<RuleType> ruleTypes = new List<RuleType>();
			string strSQL = "SELECT RULE_TYPE_ID, CLASS FROM RULE_TYPES";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			table.Rows.ForEach(row =>
			{
				RuleType ruleType = new RuleType();
				ruleType.Id = (int)row[0];
				ruleType.Class = row[1].ToString();
				ruleTypes.Add(ruleType);
			});
			return ruleTypes.ToArray();
		}

		[WebMethod]
		public Ruleset LoadRuleset(int rulesetId)
		{
			string strSQL = "SELECT R.RULESET_NAME, R.SPORT_ID, S.SPORT_TYPE, S.RULESET_ID " +
				"FROM RULESETS R, SPORTS S " +
				"WHERE R.SPORT_ID = S.SPORT_ID AND " +
				" R.RULESET_ID = @0";
			SimpleParameter rulesetParam = new SimpleParameter("@0", rulesetId);
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, rulesetParam);
			if (table.Rows.Count == 0)
				return null;

			Ruleset ruleset = new Ruleset();
			SimpleRow row = table.Rows[0];

			ruleset.Id = rulesetId;
			ruleset.Name = row[0].ToString();
			ruleset.Sport = (int)row[1];
			ruleset.SportType = (int)row[2];
			ruleset.SportRuleset = row.GetIntOrDefault(3, -1);

			strSQL = "SELECT RULE_ID, RULE_TYPE_ID, VALUE, SPORT_FIELD_TYPE_ID, " +
				"SPORT_FIELD_ID, CATEGORY " +
				"FROM RULES " +
				"WHERE RULESET_ID = @0";
			List<Rule> rules = new List<Rule>();
			table = DB.Instance.GetDataBySQL(strSQL, rulesetParam);
			table.Rows.ForEach(dataRow =>
			{
				Rule rule = new Rule();
				rule.Id = (int)dataRow[0];
				rule.RuleType = (int)dataRow[1];
				rule.Value = dataRow[2].ToString();
				rule.SportFieldType = dataRow.GetIntOrDefault(3, -1);
				rule.SportField = dataRow.GetIntOrDefault(4, -1);
				rule.Category = dataRow.GetIntOrDefault(5, -1);
				rules.Add(rule);
			});

			ruleset.Rules = rules.ToArray();
			return ruleset;
		}

		[WebMethod]
		public bool SaveRuleset(Ruleset ruleset)
		{
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			string strError = "";
			string strSQL = "DELETE RULES " +
				"WHERE RULESET_ID = @0";
			command.CommandText = strSQL;
			command.Parameters.AddWithValue("@0", ruleset.Id);

			try
			{
				command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				strError = "Error deleting existing ruleset: " + ex.ToString();
			}

			if (strError.Length == 0)
			{
				strSQL = "INSERT INTO RULES(RULESET_ID, RULE_TYPE_ID, VALUE, " +
					" SPORT_FIELD_TYPE_ID, SPORT_FIELD_ID, CATEGORY) " +
					"VALUES(@0, @1, @2, @3, @4, @5)";
				command.CommandText = strSQL;
				command.Parameters.AddWithValue("@1", -1);
				command.Parameters.AddWithValue("@2", DBNull.Value);
				command.Parameters.AddWithValue("@3", DBNull.Value);
				command.Parameters.AddWithValue("@4", DBNull.Value);
				command.Parameters.AddWithValue("@5", DBNull.Value);
				int counter = 0;
				foreach (Rule rule in ruleset.Rules)
				{
					command.Parameters["@1"].Value = rule.RuleType;
					command.Parameters["@2"].Value = DBNull.Value;
					command.Parameters["@3"].Value = DBNull.Value;
					command.Parameters["@4"].Value = DBNull.Value;
					command.Parameters["@5"].Value = DBNull.Value;
					if (rule.Value != null)
						command.Parameters["@2"].Value = rule.Value;
					if (rule.SportFieldType >= 0)
						command.Parameters["@3"].Value = rule.SportFieldType;
					if (rule.SportField >= 0)
						command.Parameters["@4"].Value = rule.SportField;
					if (rule.Category >= 0)
						command.Parameters["@5"].Value = rule.Category;
					try
					{
						command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						strError = "Failed to insert rule #" + (counter + 1) + ": " + ex.Message;
						break;
					}
					counter++;
				}
			}

			if (strError.Length == 0)
			{
				try
				{
					transaction.Commit();
				}
				catch (Exception ex)
				{
					strError = "Error committing: " + ex.ToString();
				}
			}
			else
			{
				try
				{
					transaction.Rollback();
				}
				catch
				{ }
			}

			command.Dispose();
			transaction.Dispose();
			connection.Close();
			connection.Dispose();

			if (strError.Length > 0)
				throw new Exception("Failed to save ruleset: " + strError);
			
			return true;
		}


		#endregion
	}
}
