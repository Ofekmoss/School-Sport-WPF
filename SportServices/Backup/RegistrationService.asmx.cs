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
	/// Summary description for RegistrationService.
	/// </summary>
	public class RegistrationService : System.Web.Services.WebService
	{
		public RegistrationService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Classes
		/// <summary>
		/// data for team to register.
		/// </summary>
		public class TeamData
		{
			public int id;
			public int user_id;
			public int school_id;
			public int championship_category_id;
			public int team_index;
		}

		public class PlayerData
		{
			public int id;
			public int user_id;
			public int team_id;
			public int student_id;
			public int player_number;
		}
		#endregion

		#region Get
		/// <summary>
		/// get list of all pending teams for given school.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public TeamData[] GetPendingTeams(int school_id)
		{
			string strSQL = "Select PENDING_TEAM_ID, SCHOOL_ID, CHAMP_CATEGORY_ID, " + 
				"TEAM_INDEX, USER_ID From PENDING_TEAMS Where SCHOOL_ID=@1";
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL,
				new SimpleParameter("@1", school_id));
			List<TeamData> pendingTeamsList = new List<TeamData>();
			table.Rows.ForEach(row =>
			{
				TeamData team = new TeamData();
				team.id = (int)row["PENDING_TEAM_ID"];
				team.school_id = (int)row["SCHOOL_ID"];
				team.championship_category_id = (int)row["CHAMP_CATEGORY_ID"];
				team.team_index = (int)row["TEAM_INDEX"];
				team.user_id = (int)row["USER_ID"];
				pendingTeamsList.Add(team);
			});
			
			return pendingTeamsList.ToArray();
		}

		/// <summary>
		/// get list of all pending players for given school and team.
		/// to get list of all pending players for specific school,
		/// put 0 in the team_id parameter.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public PlayerData[] GetPendingPlayers(int school_id, int team_id)
		{
			string strSQL = "SELECT p.PENDING_PLAYER_ID, p.TEAM_ID, p.STUDENT_ID, " + 
				"p.PLAYER_NUMBER, p.USER_ID " + 
				"FROM PENDING_PLAYERS p, TEAMS t, STUDENTS s " + 
				"WHERE p.TEAM_ID=t.TEAM_ID AND p.STUDENT_ID=s.STUDENT_ID " + 
				"AND t.DATE_DELETED IS NULL AND s.DATE_DELETED IS NULL " + 
				"AND t.SCHOOL_ID=@1 ";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", school_id));
			if (team_id > 0)
			{
				strSQL += "AND p.TEAM_ID=@2 ";
				parameters.Add(new SimpleParameter("@2", team_id));
			}
			strSQL += "ORDER BY p.TEAM_ID ASC";
			List<PlayerData> pendingPlayersList = new List<PlayerData>();
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			table.Rows.ForEach(row =>
			{
				PlayerData player = new PlayerData();
				player.id = (int)row["PENDING_PLAYER_ID"];
				player.team_id = (int)row["TEAM_ID"];
				player.student_id = (int)row["STUDENT_ID"];
				player.player_number = (int)row["PLAYER_NUMBER"];
				player.user_id = (int)row["USER_ID"];
				pendingPlayersList.Add(player);
			});
			
			return pendingPlayersList.ToArray();
		}
		#endregion

		#region Register
		/// <summary>
		/// add given team to the pending teams table. team is registered for given
		/// school and championship category, by the given user.
		/// the same user can register several teams for different schools.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int RegisterTeam(int user_id, int school_id, int championship_category_id, int team_index)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't register team: session expired."); */

			//first check if team with all these details exist:
			string strSQL = "Select * From PENDING_TEAMS Where SCHOOL_ID=@1 ";
			strSQL += "And CHAMP_CATEGORY_ID=@2 And TEAM_INDEX=@3";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", school_id));
			parameters.Add(new SimpleParameter("@2", championship_category_id));
			parameters.Add(new SimpleParameter("@3", team_index));
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			bool exists = table.Rows.Count > 0;
			if (exists)
				throw new Exception("can't register team, duplicate team already exists");

			strSQL = "Insert Into PENDING_TEAMS (USER_ID,SCHOOL_ID,CHAMP_CATEGORY_ID,";
			strSQL += "TEAM_INDEX) Values (@5, @1, @2, @3)";
			parameters.Add(new SimpleParameter("@5", user_id));
			DB.Instance.Execute(strSQL, parameters.ToArray());

			//read id from database:
			strSQL = "SELECT MAX(PENDING_TEAM_ID) FROM PENDING_TEAMS";
			return Int32.Parse(DB.Instance.ExecuteScalar(strSQL, -1).ToString());
		}

		/// <summary>
		/// register player with given details to the pending players database table.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int RegisterPlayer(int user_id, int team_id, int student_id, int player_number)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't register player: session expired."); */

			PlayerData player = new PlayerData();
			player.user_id = user_id;
			player.team_id = team_id;
			player.student_id = student_id;
			player.player_number = player_number;

			return _RegisterPlayer(player);
		}

		[WebMethod(EnableSession = true)]
		public int[] RegisterPlayers(int user_id, PlayerData[] players)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't register players: session expired."); */

			//initialize array to be returned:
			int[] result = new int[players.Length];

			//iterate over each player and register:
			string strError = "";
			for (int i = 0; i < players.Length; i++)
			{
				try
				{
					result[i] = _RegisterPlayer(players[i]);
				}
				catch (Exception ex)
				{
					strError = "error while registering player with student id: " + players[i].student_id.ToString() + ": " + ex.ToString();
					break;
				}
			} //end loop over players.

			if (strError.Length > 0)
				throw new Exception("Failed to register players: " + strError);

			return result;
		}

		/// <summary>
		/// the actual function to register player.
		/// register player to given team, by given user.
		/// </summary>
		private int _RegisterPlayer(PlayerData player)
		{
			//get data:
			int user_id = player.user_id;
			int team_id = player.team_id;
			int student_id = player.student_id;
			int player_number = player.player_number;

			//first check if player with all these details exist:
			string strSQL = "Select * From PENDING_PLAYERS Where " + 
				"TEAM_ID=@1 And STUDENT_ID=@2";
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@1", team_id));
			parameters.Add(new SimpleParameter("@2", student_id));
			SimpleTable table = DB.Instance.GetDataBySQL(strSQL, parameters.ToArray());
			bool exists = table.Rows.Count > 0;
			if (exists)
				throw new Exception("can't register player, duplicate player already exists in this team.");
			
			//build sql statement and execute:
			strSQL = "Insert Into PENDING_PLAYERS (USER_ID, TEAM_ID, STUDENT_ID, " +
				"PLAYER_NUMBER) Values (@3, @1, @2, @5)";
			parameters.Add(new SimpleParameter("@3", user_id));
			parameters.Add(new SimpleParameter("@5", player_number));
			DB.Instance.Execute(strSQL, parameters.ToArray());
			
			//read automatic id from database:
			strSQL = "SELECT MAX(PENDING_PLAYER_ID) FROM PENDING_PLAYERS";
			return Int32.Parse(DB.Instance.ExecuteScalar(strSQL, -1).ToString());
		}
		#endregion

		#region Remove
		/// <summary>
		/// remove pending team with given id from the pending teams table.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public bool RemovePendingTeam(int pending_team_id)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't delete pending team: session expired."); */

			string strSQL = "Delete From PENDING_TEAMS Where PENDING_TEAM_ID=@1";
			int rowsAffected = DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", pending_team_id));
			return (rowsAffected == 1);
		}

		/// <summary>
		/// remove pending player with given id from pending players table.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public bool RemovePendingPlayer(int pending_player_id)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't delete pending player: session expired."); */

			string strSQL = "Delete From PENDING_PLAYERS Where PENDING_PLAYER_ID=@1";
			int rowsAffected = DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", pending_player_id));
			return (rowsAffected == 1);
		}
		#endregion

		#region Update
		/// <summary>
		/// update the index of given team.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public bool UpdatePendingTeam(int pending_team_id, int team_index)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't update pending team: session expired."); */
			
			string strSQL = "Update PENDING_TEAMS Set TEAM_INDEX=@1 ";
			strSQL += "Where PENDING_TEAM_ID=@2";
			int rowsAffected = DB.Instance.Execute(strSQL, 
				new SimpleParameter("@1", team_index), 
				new SimpleParameter("@2", pending_team_id));
			return (rowsAffected == 1);
		}

		/// <summary>
		/// update player number of given player.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public bool UpdatePendingPlayer(int pending_player_id, int player_number)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't update pending player: session expired."); */

			string strSQL = "Update PENDING_PLAYERS Set PLAYER_NUMBER=@1 ";
			strSQL += "Where PENDING_PLAYER_ID=@2";
			int rowsAffected = DB.Instance.Execute(strSQL, 
				new SimpleParameter("@1", player_number), 
				new SimpleParameter("@2", pending_player_id));
			return (rowsAffected == 1);
		}
		#endregion

		#region Commit
		/// <summary>
		/// commit all pending teams of given school to the main database.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public bool CommitPendingTeams(int school_id)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't commit pending teams: session expired."); */

			//build sql statement:
			string sqlInsert =
				"INSERT INTO TEAMS(SCHOOL_ID, CHAMPIONSHIP_ID, CHAMPIONSHIP_CATEGORY_ID, " +
				"STATUS, TEAM_INDEX, TEAM_SUPERVISOR) " +
				"SELECT P.SCHOOL_ID, C.CHAMPIONSHIP_ID, C.CHAMPIONSHIP_CATEGORY_ID, " +
				" 1, P.TEAM_INDEX, P.USER_ID " +
				"FROM PENDING_TEAMS P, CHAMPIONSHIP_CATEGORIES C " +
				"WHERE P.CHAMP_CATEGORY_ID = C.CHAMPIONSHIP_CATEGORY_ID AND " +
				"P.SCHOOL_ID = @school ";

			//build parameter:
			SimpleParameter schoolParam = new SimpleParameter("@school", school_id);

			//try to execute:
			DB.Instance.Execute(sqlInsert, schoolParam);

			//delete pending teams for that school:
			string sqlDelete = "DELETE FROM PENDING_TEAMS WHERE SCHOOL_ID = @school ";
			DB.Instance.Execute(sqlDelete, schoolParam);

			return true;
		} //end function CommitPendingTeams

		/// <summary>
		/// commit all pending players of given school to the main database.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public bool CommitPendingPlayers(int school_id, int team_id)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't commit pending players: session expired."); */

			//build sql statement:
			string sqlInsert =
				"INSERT INTO PLAYERS(STUDENT_ID, TEAM_ID, TEAM_NUMBER, STATUS) " +
				"SELECT P.STUDENT_ID, P.TEAM_ID, P.PLAYER_NUMBER, P.STATUS " +
				"FROM PENDING_PLAYERS P, TEAMS t " +
				"WHERE P.TEAM_ID=t.TEAM_ID AND t.SCHOOL_ID = @school " +
				"AND P.STUDENT_ID NOT IN (SELECT DISTINCT STUDENT_ID FROM " +
				"PLAYERS WHERE TEAM_ID=@team AND DATE_DELETED IS NULL) " +
				"AND p.TEAM_ID=@team";

			//build parameters:
			List<SimpleParameter> parameters = new List<SimpleParameter>();
			parameters.Add(new SimpleParameter("@school", school_id));
			parameters.Add(new SimpleParameter("@team", team_id));

			//try to execute:
			DB.Instance.Execute(sqlInsert, parameters.ToArray());

			//delete pending players for that school and team:
			string sqlDelete = "DELETE FROM PENDING_PLAYERS " + 
				"WHERE PENDING_PLAYER_ID IN " + 
				"(SELECT DISTINCT P.PENDING_PLAYER_ID FROM " + 
				"PENDING_PLAYERS p, TEAMS t WHERE " + 
				"p.TEAM_ID=t.TEAM_ID AND t.SCHOOL_ID=@school " + 
				"AND p.TEAM_ID=@team)";
			DB.Instance.Execute(sqlDelete, parameters.ToArray());
			
			return true;
		} //end function CommitPendingPlayers
		#endregion

		#region General
		/// <summary>
		/// mark given message as read.
		/// </summary>
		[WebMethod(EnableSession = true)]
		public int MarkMessageRead(int message_id)
		{
			/* if (Session[SessionService.SessionKey_UserID] == null)
				throw new Exception("can't mark message as read: session expired."); */

			string strSQL = "UPDATE MESSAGES SET MESSAGE_STATUS=@2 WHERE MESSAGE_ID=@1";
			int rowsAffected = DB.Instance.Execute(strSQL,
				new SimpleParameter("@1", message_id),
				new SimpleParameter("@2", (int)Sport.Types.MessageStatus.Read));
			Sport.Entities.Message.Type.Reset(null);
			return rowsAffected;
		}

		[WebMethod]
		public int GetStudentsCount(int school)
		{
			string strSQL = "SELECT COUNT(STUDENT_ID) AS TOTAL " +
				"FROM STUDENTS " +
				"WHERE SCHOOL_ID = @0";
			return (int)DB.Instance.ExecuteScalar(strSQL, 0,
				new SimpleParameter("@0", school));
		}
		#endregion

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
