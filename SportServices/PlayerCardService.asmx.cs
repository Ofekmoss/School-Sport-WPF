using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using Sport.Core;
using System.Collections.Generic;
using ISF.DataLayer;

namespace SportServices
{
	/// <summary>
	/// Summary description for PlayerCardService.
	/// </summary>
	[WebService(Namespace = "http://www.mir.co.il/PlayerCard")]
	public class PlayerCardService : System.Web.Services.WebService
	{
		public PlayerCardService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		/// <summary>
		/// insert new cards to database for given students and given sport.
		/// update if already exists with current date and time.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int IssuePlayerCards(int[] students, int sportID)
		{
			if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't issue player cards: session expired.");

			//first get list of existing cards:
			string strSQL = "SELECT DISTINCT STUDENT_ID FROM STUDENT_CARDS " +
				"WHERE SPORT_ID=@1";
			List<int> arrAlreadyIssued = new List<int>();
			int seasonID = (int)Session[SessionService.SessionKey_Season];
			DateTime now = DateTime.Now;
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", sportID));
			table.Rows.ForEach(row =>
			{
				arrAlreadyIssued.Add((int)row["STUDENT_ID"]);
			});
			
			//iterate through students and execute proper query:
			SqlConnection connection = DB.Instance.GetOpenConnection();
			SqlTransaction transaction = connection.BeginTransaction();
			SqlCommand command = new SqlCommand();
			command.Connection = connection;
			command.Transaction = transaction;
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@1", -1);
			command.Parameters.AddWithValue("@2", sportID);
			command.Parameters.AddWithValue("@3", now);
			command.Parameters.AddWithValue("@4", seasonID);
			command.Parameters.AddWithValue("@5", now);
			string strError = "";
			string updateSQL = "UPDATE STUDENT_CARDS " + 
				"SET ISSUE_DATE=@3, ISSUE_SEASON=@4, DATE_LAST_MODIFIED=@5 " + 
				"WHERE STUDENT_ID=@1 AND SPORT_ID=@2";
			string insertSQL = "INSERT INTO STUDENT_CARDS " + 
				"(STUDENT_ID, SPORT_ID, ISSUE_DATE, ISSUE_SEASON, DATE_LAST_MODIFIED) " + 
				"VALUES (@1, @2, @3, @4, @5)";
			for (int i = 0; i < students.Length; i++)
			{
				int studentID = students[i];
				strSQL = arrAlreadyIssued.Contains(studentID) ? updateSQL : insertSQL;
				command.CommandText = strSQL;
				command.Parameters["@1"].Value = studentID;
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					strError = "Failed to add or update player card for student " + studentID + ": " + ex.Message;
					break;
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
				throw new Exception("Failed to issue player cards: " + strError);

			return 0;
		}

		/// <summary>
		/// return issue date of player card for given student or minimum if no card exist.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public DateTime CardIssueDate(int studentID, int sportID)
		{
			string strSQL = "SELECT ISSUE_DATE FROM STUDENT_CARDS WHERE ";
			strSQL += "STUDENT_ID=@1 AND SPORT_ID=@2";
			return (DateTime)DB.Instance.ExecuteScalar(strSQL, DateTime.MinValue, 
				new SimpleParameter("@1", studentID),
				new SimpleParameter("@2", sportID));
		}

		/// <summary>
		/// delete card for given student.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int RemovePlayerCard(int studentID, int sportID)
		{
			if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't delete player card: session expired.");

			string strSQL = "DELETE FROM STUDENT_CARDS WHERE STUDENT_ID=@1 AND SPORT_ID=@2";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", studentID),
				new SimpleParameter("@2", sportID));
		}

		/// <summary>
		/// update issue date of player card for given student
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int UpdateIssueDate(int studentID, int sportID, DateTime date)
		{
			string strSQL = "UPDATE STUDENT_CARDS SET ISSUE_DATE=@date WHERE ";
			strSQL += "STUDENT_ID=@student AND SPORT_ID=@sport";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@date", date),
				new SimpleParameter("@student", studentID),
				new SimpleParameter("@sport", sportID));
		}

		/// <summary>
		/// remove issue date of player card for given student
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int RemoveIssueDate(int studentID, int sportID)
		{
			string strSQL = "UPDATE STUDENT_CARDS SET ISSUE_DATE=NULL WHERE ";
			strSQL += "STUDENT_ID=@student AND SPORT_ID=@sport";
			return DB.Instance.Execute(strSQL,
				new SimpleParameter("@student", studentID),
				new SimpleParameter("@sport", sportID));
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
	}
}
