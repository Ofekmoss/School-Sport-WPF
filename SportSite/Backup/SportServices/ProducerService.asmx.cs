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

namespace SportServices
{
	[WebService(Namespace="http://www.mir.co.il/")]
	public class ProducerService : System.Web.Services.WebService
	{
		public ProducerService()
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

		[WebMethod]
		public string GetGameBoard(int gameBoardId)
		{
			string strSQL = "SELECT DATA " +
				"FROM GAME_BOARDS " +
				"WHERE GAME_BOARD_ID = @0";
			string data = DB.Instance.ExecuteScalar(strSQL, "", 
				new SimpleParameter("@0", gameBoardId)).ToString();
			return (data.Length > 0) ? data : null;
		}

		[WebMethod]
		public bool SetGameBoard(int gameBoardId, string range, string data)
		{
			string strSQL = "UPDATE GAME_BOARDS " +
				"SET DATA = @0, RANGE = @1 " +
				"WHERE GAME_BOARD_ID = @2";
			int rowsAffected = DB.Instance.Execute(strSQL,
				new SimpleParameter("@0", data),
				new SimpleParameter("@1", range),
				new SimpleParameter("@2", gameBoardId));
			return (rowsAffected == 1);
		}

		[WebMethod]
		public string GetPhasePattern(int phasePatternId)
		{
			string strSQL = "SELECT DATA " +
				"FROM PHASE_PATTERNS " +
				"WHERE PHASE_PATTERN_ID = @0";
			return DB.Instance.ExecuteScalar(strSQL, "",
				new SimpleParameter("@0", phasePatternId)).ToString();
		}

		[WebMethod]
		public bool SetPhasePattern(int phasePatternId, string range, string data)
		{
			string strSQL = "UPDATE PHASE_PATTERNS " +
				"SET DATA = @0, RANGE = @1 " +
				"WHERE PHASE_PATTERN_ID = @2";
			int rowsAffected = DB.Instance.Execute(strSQL,
				new SimpleParameter("@0", data),
				new SimpleParameter("@1", range),
				new SimpleParameter("@2", phasePatternId));
			return (rowsAffected == 1);
		}
	}
}

