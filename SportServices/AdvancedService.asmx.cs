using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using Sport.Core;
using Sport.Common;
using ISF.DataLayer;
using System.Collections.Generic;
using System.Linq;

namespace SportServices
{
	/// <summary>
	/// Advanced and general queries over the database.
	/// </summary>
	public class AdvancedService : System.Web.Services.WebService
	{
		#region structs
		public struct GradeData
		{
			public int grade;
			public int studentsCount;
		}
		#endregion
		
		private string[] _alterSQL=new string[]
		{
			"ALTER TABLE CHAMPIONSHIPS ADD REMARKS nvarchar(255)",
			"ALTER TABLE FUNCTIONARIES ADD ZIP_CODE nvarchar(15)",
			"ALTER TABLE FUNCTIONARIES ADD EMAIL nvarchar(100)"
		};
		
		[WebMethod]
		public void ReportUnhandledError(string strExceptionDescription, 
			string strStackTrace, int userID, DateTime time, string strAppPath)
		{
			List<string> lines = new List<string>();
			lines.Add("Error occured in " + strAppPath);
			lines.Add("User ID: " + userID);
			lines.Add("local time: " + time.ToString("dd/MM/yyyy HH:mm:ss"));
			lines.Add("server time: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
			lines.Add("Description: " + strExceptionDescription);
			lines.Add("Stack Trace:");
			if (strStackTrace == null)
				lines.Add("N/A");
			else
				lines.AddRange(strStackTrace.Split('\n'));
			try
			{
				EventLog.WriteEntry("SchoolSport", string.Join("\n", lines), EventLogEntryType.Error);
			}
			catch
			{
			}

			string strSQL = "Insert Into [UnhandledProgramExceptions] ([Description], [StackTrace], [UserId], [LocalTime], [LocalPath]) Values " +
				"(@description, @stacktrace, @user, @time, @path)";
			using (SqlConnection connection = DB.Instance.GetOpenConnection())
			{
				using (SqlCommand command = new SqlCommand(strSQL, connection))
				{
					command.Parameters.AddWithValue("@description", strExceptionDescription);
					command.Parameters.AddWithValue("@stacktrace", strStackTrace);
					command.Parameters.AddWithValue("@user", userID);
					command.Parameters.AddWithValue("@time", time);
					command.Parameters.AddWithValue("@path", strAppPath);
					command.ExecuteNonQuery();
				}
			}

			/* no longer working after server upgrade
			string strFrom = "Sportsman Error<error@sportsman.err>";
			string strTo = "yahavbr@gmail.com";
			string strSubject = "Error occured in sportsman application";
			lines.Add("---end of report---");
			Sport.Common.Tools.sendEmail(strFrom, strTo, strSubject, string.Join("<br />", lines));
			*/
		}
		
		[WebMethod]
		public string GetOpenConnections()
		{
			return "";
		}
		
		[WebMethod(EnableSession=true)]
		public bool AddToSecurityLog(string message)
		{
			string strIP=Common.ToStringDef(
				Context.Request.ServerVariables["REMOTE_ADDR"], "");
			if (strIP.Length == 0)
			{
				strIP = Common.ToStringDef(
					Context.Request.ServerVariables["REMOTE_HOST"], "");
			}
			Sport.Core.LogFiles.AppendToLogFile(Server.MapPath("SecurityLog.txt"), 
				"["+strIP+"] "+message);
			return true;
		}
		
		[WebMethod(EnableSession=true)]
		public string[] GetAlterSQL()
		{
			if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't get alter SQL: session expired.");
			return _alterSQL;
		}
		
		[WebMethod(EnableSession=true)]
		public void ExecuteAlterSQL(int index)
		{
			if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't execute alter SQL: session expired.");
			
			//build sql statement:
			string strSQL=_alterSQL[index];
			
			//apply command and execute query:
			DB.Instance.Execute(strSQL);
		}
		
		/// <summary>
		/// returns advanced information about every grade.
		/// </summary>
		[WebMethod(EnableSession=true)]
		public GradeData[] GetGradesInfo()
		{
			if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't get grade info: session expired.");
			
			
			//build sql statement:
			string strSQL="SELECT GRADE, COUNT(STUDENT_ID) AS STUDENTS_COUNT FROM STUDENTS GROUP BY GRADE";

			//read results:
			SimpleTable table =  DB.Instance.GetDataBySQL(strSQL);
			List<GradeData> grades = new List<GradeData>();
			table.Rows.ForEach(row =>
			{
				grades.Add(new GradeData()
				{
					grade = (int)row["GRADE"],
					studentsCount = (int)row["STUDENTS_COUNT"]
				});
			});

			return grades.ToArray();
		}
		
		[WebMethod]
		public int[] GetDifferentSeasonCharges(int[] charges, int season)
		{
			if (charges == null || charges.Length == 0)
				return null;
			
			//build select statement:
			string strSQL="SELECT CHARGE_ID, CHAMPIONSHIP_CATEGORY FROM CHARGES " +
				"WHERE CHARGE_ID IN (" + String.Join(", ", Tools.ToStringArray(charges)) + ") " + 
				"AND DATE_DELETED IS NULL " + 
				"AND CHAMPIONSHIP_CATEGORY IS NOT NULL";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL);
			Dictionary<int, int> chargeChamps = new Dictionary<int, int>();
			table.Rows.ForEach(row =>
			{
				int chargeId = (int)row["CHARGE_ID"];
				int champCategoryId = (int)row["CHAMPIONSHIP_CATEGORY"];
				if (chargeId >= 0 && champCategoryId >= 0 && !chargeChamps.ContainsKey(chargeId))
					chargeChamps.Add(chargeId, champCategoryId);
			});

			List<int> result = new List<int>();
			if (chargeChamps.Count > 0)
			{
				strSQL = "SELECT c.SEASON, cc.CHAMPIONSHIP_CATEGORY_ID FROM CHAMPIONSHIPS c, CHAMPIONSHIP_CATEGORIES cc " + 
					"WHERE cc.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID " + 
					"AND cc.CHAMPIONSHIP_CATEGORY_ID IN " + 
					"(" + String.Join(", ", chargeChamps.Values.ToList().ConvertAll(c => c.ToString()).ToArray())  + ")";
				table = DB.Instance.GetDataBySQL(strSQL);
				table.Rows.ForEach(row =>
				{
					int curSeason = (int)row["SEASON"];
					if (curSeason != season)
					{
						int champCategoryId = (int)row["CHAMPIONSHIP_CATEGORY_ID"];
						int chargeId = -1;
						foreach (int curChargeId in chargeChamps.Keys)
						{
							if (chargeChamps[curChargeId] == champCategoryId)
							{
								chargeId = curChargeId;
								break;
							}
						}
						if (chargeId >= 0)
							result.Add(chargeId);
					}
				});
			}
			
			return result.ToArray();
		}
		
		[WebMethod]
		public int[] GetChargesChampionships(int[] charges, int season)
		{
			if (charges == null || charges.Length == 0)
				return null;
			
			//build select statement:
			string strSQL="SELECT t.CHARGE_ID, t.CHAMPIONSHIP_CATEGORY_ID " + 
				"FROM TEAMS t, CHAMPIONSHIPS c " + 
				"WHERE t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID " + 
				"AND c.SEASON=@season "+
				"AND t.CHARGE_ID IN (" + String.Join(", ", Tools.ToStringArray(charges)) + ") " + 
				"AND t.DATE_DELETED IS NULL AND c.DATE_DELETED IS NULL";

			//read results:
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, new SimpleParameter("@season", season));
			Dictionary<int, int> chargeChamps = new Dictionary<int, int>();
			table.Rows.ForEach(row =>
			{
				int chargeId = (int)row["CHARGE_ID"];
				int champCategoryId = (int)row["CHAMPIONSHIP_CATEGORY_ID"];
				if (chargeId >= 0 && champCategoryId >= 0 && !chargeChamps.ContainsKey(chargeId))
					chargeChamps.Add(chargeId, champCategoryId);
			});
			
			int[] result = new int[charges.Length];
			for (int i = 0; i < charges.Length; i++)
			{
				int curCharge = charges[i];
				int curCategoryId = chargeChamps.ContainsKey(curCharge) ? chargeChamps[curCharge] : -1;
				result[i] = curCategoryId;
			}
			
			return result;
		}
		
		public AdvancedService()
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
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		// WEB SERVICE EXAMPLE
		// The HelloWorld() example service returns the string Hello World
		// To build, uncomment the following lines then save and build the project
		// To test this web service, press F5

//		[WebMethod]
//		public string HelloWorld()
//		{
//			return "Hello World";
//		}
	}
}
