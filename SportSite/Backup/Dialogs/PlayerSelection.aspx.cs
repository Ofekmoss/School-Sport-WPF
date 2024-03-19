using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SportSite.Core;
using Sport.Data;
using ISF.DataLayer;
using SportCommon = Sport.Common;

namespace SportSite.Dialogs
{
	/// <summary>
	/// Summary description for PlayerSelection.
	/// </summary>
	public class PlayerSelection : System.Web.UI.Page
	{
		public Common.ClientSide clientSide = null;
		protected System.Web.UI.WebControls.Label lblTeamName;
		protected System.Web.UI.WebControls.DropDownList ddlGrades;
		protected System.Web.UI.WebControls.Table TblStudents;
		protected System.Web.UI.HtmlControls.HtmlForm Form1;
		protected System.Web.UI.WebControls.TextBox txtStudentNumber;
		protected LookupType lookupGrades = new Sport.Types.GradeTypeLookup(true);
		private int originalSeason = -1;

		private Entity _school;
		private Entity _category;
		private Entity _team;

		private enum StudentColumns
		{
			Grade = 0,
			LastName,
			FirstName,
			IdNumber,
			Select
		}

		/// <summary>
		/// school from which to select students.
		/// </summary>
		public int SchoolID = -1;
		/// <summary>
		/// championship category for which to register the players.
		/// </summary>
		public int categoryID = -1;
		public int teamID = -1;

		private void Page_Load(object sender, System.EventArgs e)
		{
			originalSeason = Sport.Core.Session.Season;
			Sport.Core.Session.Season = Sport.Entities.Season.GetOpenSeason();
			
			clientSide = new Common.ClientSide(this.Page, null);

			//add hitlog data:
			Common.Tools.AddHitLog(WebSiteServices.WebSitePage.PlayerSelectionDialog,
				this.Request);

			Sport.Core.Session.Cookies = (System.Net.CookieContainer)Session["cookies"];

			txtStudentNumber.Attributes["dir"] = "ltr";
			txtStudentNumber.Attributes["onkeypress"] = "return StudentKeyPress(event, this);";

			CheckFindStudent();

			//get needed info:
			GetAndVerify();

			//getting here without exceptions means we have valid school and category.
			if (_team != null)
				lblTeamName.Text = _team.Name;

			//fill grades list:
			FillGrades();

			//set all students:
			SetStudentsTable();

			AddJavaScript();
		} //end function Page_Load

		private void Page_Unload(object sender, System.EventArgs e)
		{
			Sport.Core.Session.Cookies = null;
			Sport.Core.Session.Season = originalSeason;
		}

		private void SetStudentsTable()
		{
			TblStudents.CssClass = SportSite.Common.Style.PlayersTableCss;
			TblStudents.Attributes["fixedrows"] = "1";
			TblStudents.BorderWidth = new Unit((double)1);

			//get form data:
			int gradeFilter = Sport.Common.Tools.CIntDef(Request.Form[ddlGrades.UniqueID], -1);

			//get list of students for the given school, in proper grades.
			EntityType studentType = Sport.Entities.Student.Type;

			//define filters:
			EntityFilter filter = new EntityFilter();
			Sport.Entities.ChampionshipCategory cc = new Sport.Entities.ChampionshipCategory(categoryID);
			if (cc.IsValid())
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.Student.Fields.School, SchoolID));
				if (gradeFilter >= 0)
					filter.Add(new EntityFilterField((int)Sport.Entities.Student.Fields.Grade, gradeFilter));
				filter.Add(new Sport.Types.CategoryFilterField(
					(int)Sport.Entities.Student.Fields.Grade, cc.Category, Sport.Types.CategoryCompareType.Grade));
			}

			//get list of already-registered players:
			ArrayList registeredPlayers = GetRegisteredPlayers(_team.Id);

			//get list of students:
			ArrayList students =
				new ArrayList(Sport.Entities.Student.Type.GetEntities(filter));

			//add the registered players who are not part of that school...
			foreach (int student_id in registeredPlayers)
			{
				Entity student = null;
				try
				{
					student = Sport.Entities.Student.Type.Lookup(student_id);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("failed to create student: " + ex.Message);
					System.Diagnostics.Debug.WriteLine(ex.Message);
					student = null;
				}
				if (student != null)
				{
					if (students.IndexOf(student) == -1)
						students.Add(student);
				}
			}

			//fill the table:
			foreach (Entity student in students)
			{
				//extract details:
				string grade = lookupGrades.Lookup((int)student.Fields[(int)Sport.Entities.Student.Fields.Grade]);
				string lastName = student.Fields[(int)Sport.Entities.Student.Fields.LastName].ToString();
				string firstName = student.Fields[(int)Sport.Entities.Student.Fields.FirstName].ToString();
				string id_number = student.Fields[(int)Sport.Entities.Student.Fields.IdNumber].ToString();
				string studentID = student.Id.ToString();
				bool disabled = registeredPlayers.Contains(Int32.Parse(studentID));

				//initialize row and its properties:
				TableRow objRow = new TableRow();

				if (disabled)
					objRow.BackColor = Color.White;

				//populate cells.

				//1st cell - Grade
				objRow.Cells.Add(BuildTableCell(grade, null, disabled));

				//2nd cell - Family name.
				objRow.Cells.Add(BuildTableCell(lastName, null, disabled));

				//3rd cell - First name.
				objRow.Cells.Add(BuildTableCell(firstName, null, disabled));

				//4th cell - id number.
				objRow.Cells.Add(BuildTableCell(id_number, null, disabled));

				//5th cell - checkbox.
				TableCell objCell = Common.FastControls.HebTableCell("");
				string strHtml = "<input type=\"checkbox\" name=\"selected_players\" ";
				if (disabled)
					strHtml += "disabled=\"disabled\" ";
				strHtml += "value=\"" + student.Id.ToString() + "\" />";
				objCell.Controls.Add(new System.Web.UI.LiteralControl(strHtml));
				objRow.Cells.Add(objCell);

				TblStudents.Rows.Add(objRow);
			}

			TblStudents.Attributes["colors"] = Sport.Common.Tools.ColorsToHex(
				Common.Style.ChoosePlayerColors, ",");
			TblStudents.Attributes["addlight"] = "20";
		}

		private TableCell BuildTableCell(string text, string id, bool disabled)
		{
			TableCell result = Common.FastControls.HebTableCell(text);
			//result.CssClass = SportSite.Common.Style.ChoosePlayerCss;
			if (disabled)
				result.ForeColor = System.Drawing.SystemColors.GrayText;
			if (id != null)
				result.Attributes.Add("id", id);
			return result;
		}

		/// <summary>
		/// get list of already-registered players for given team.
		/// check both Players and Pending Players tables.
		/// every item in the list is the student_id from Students table.
		/// </summary>
		private ArrayList GetRegisteredPlayers(int teamid)
		{
			ArrayList result = new ArrayList();
			int i;

			//get list of players from database:
			EntityType playerType = Sport.Entities.Player.Type;
			Entity[] players = playerType.GetEntities(new EntityFilter((int)Sport.Entities.Player.Fields.Team, teamid));

			//put student id in the list:
			for (i = 0; i < players.Length; i++)
				result.Add((int)players[i].Fields[(int)Sport.Entities.Player.Fields.Student]);

			//get list of pending players:
			SportSite.RegistrationService.RegistrationService regService =
				new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer)Session["cookies"];
			int school_id = ((UserData)Session[UserManager.SessionKey]).School.ID;
			SportSite.RegistrationService.PlayerData[] arrPendingPlayers =
				regService.GetPendingPlayers(school_id, teamid);

			//put student id in the list:
			for (i = 0; i < arrPendingPlayers.Length; i++)
				result.Add(arrPendingPlayers[i].student_id);

			//done.
			return result;
		}

		/// <summary>
		/// read information from querystring if needed and verify it exists.
		/// </summary>
		private void GetAndVerify()
		{
			//get school from querystring:
			if (SchoolID < 0)
				SchoolID = Sport.Common.Tools.CIntDef(Request.QueryString["school"], -1);

			//verify we have school:
			if (SchoolID < 0)
				throw new Exception("לא ניתן לבחור תלמידים, לא הוגדר בית ספר");

			//get team from querystring:
			if (teamID < 0)
				teamID = Sport.Common.Tools.CIntDef(Request.QueryString["team"], -1);

			//get category from querystring:
			if (categoryID < 0)
				categoryID = Sport.Common.Tools.CIntDef(Request.QueryString["category"], -1);

			//verify we have category:
			if (categoryID < 0)
				throw new Exception("לא ניתן לבחור תלמידים, לא הוגדרה קטגוריית אליפות");

			//get database data:
			_school = Sport.Entities.School.Type.Lookup(SchoolID);
			_category = Sport.Entities.ChampionshipCategory.Type.Lookup(categoryID);
			_team = Sport.Entities.Team.Type.Lookup(teamID);
			if ((_school == null) || (_category == null))
				throw new Exception("מידע לא קיים במאגר הנתונים לא ניתן לבחור שחקנים");
		}

		/// <summary>
		/// fill all grades matching the current category.
		/// </summary>
		/// <param name="categotyID"></param>
		private void FillGrades()
		{
			int i;

			//clear combo:
			ddlGrades.Items.Clear();

			//get category value:
			int categoryValue = (int)_category.Fields[(int)Sport.Entities.ChampionshipCategory.Fields.Category];

			//filter the grades to those matching the championship category:
			LookupItem[] arrGrades = null;
			ArrayList gradesList = new ArrayList();
			for (i = 0; i < lookupGrades.Items.Length; i++)
			{
				//LookupItem grade=(LookupItem) lookupGrades.Items[i];
				if (Sport.Types.CategoryTypeLookup.Contains(categoryValue, i))
					gradesList.Add(lookupGrades.Items[i]);
			}
			arrGrades = new LookupItem[gradesList.Count];
			for (i = 0; i < gradesList.Count; i++)
				arrGrades[i] = (LookupItem)gradesList[i];

			//fill only the proper grades:
			for (i = 0; i < arrGrades.Length; i++)
				ddlGrades.Items.Add(new ListItem(arrGrades[i].Text, arrGrades[i].Id.ToString()));

			//add the default:
			ddlGrades.Items.Insert(0, new ListItem("<כל הכיתות>", ""));

			if (Request.Form[ddlGrades.UniqueID] != null)
				ddlGrades.SelectedValue = Request.Form[ddlGrades.UniqueID].ToString();
		}

		/// <summary>
		/// add all javascript related code.
		/// </summary>
		private void AddJavaScript()
		{
			clientSide.RegisterDebugArea(this.Page);

			string strCode = "<script type=\"text/javascript\" src=\"" + Common.Data.AppPath + "/Common/Common.js\"></script>\n";
			strCode += "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + Common.Data.AppPath + "/SportSite.css\" />";
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "global", strCode, false);

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("<script type=\"text/javascript\" language=\"javascript\">\n");
			builder.Append(" var ModalResult=" + ((int)SportSite.Common.ModalResults.Undefined).ToString() + ";\n");
			builder.Append(" var txtStudentNumberID=\"" + txtStudentNumber.UniqueID + "\";\n");
			builder.Append(" var tblStudentsID=\"" + TblStudents.UniqueID + "\";\n");
			builder.Append(" var idNumberIndex=" + ((int)StudentColumns.IdNumber) + ";\n");
			builder.Append(" var selectIndex=" + ((int)StudentColumns.Select) + ";\n");
			builder.Append(" var firstNameIndex=" + ((int)StudentColumns.FirstName) + ";\n");
			builder.Append(" var lastNameIndex=" + ((int)StudentColumns.LastName) + ";\n");
			builder.Append(" var strAlreadyExistErr=\"תלמיד זה כבר רשום כשחקן בקבוצה\";\n");
			builder.Append(" var strDoesNotExistErr=\"תלמיד/ה בעלי מספר זהות זה לא קיימים במאגר הנתונים\";\n");
			builder.Append(" var strHighlightColor=\"" + Sport.Common.Tools.ColorToHex(
				SportSite.Common.Style.PlayerSelectHighlight) + "\";");
			builder.Append(" var lblTeamNameID=\"" + lblTeamName.UniqueID + "\";\n");
			builder.Append(" var ddlGradesID=\"" + ddlGrades.UniqueID + "\";\n");
			builder.Append(" var modalResultOK=" + ((int)SportSite.Common.ModalResults.OK) + ";\n");
			builder.Append(" var modalResultCancel=" + ((int)SportSite.Common.ModalResults.Cancel) + ";\n");
			builder.Append("</script>\n");
			builder.Append("\n");
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "GlobalVariables", builder.ToString(), false);
			Page.ClientScript.RegisterClientScriptInclude("local", "PlayerSelection.js");

			clientSide.RegisterAddLight(this.Page);
		}

		private void CheckFindStudent()
		{
			if (Request.QueryString["action"] == "findstudent")
			{
				string id_number = Common.Tools.CStrDef(Request.QueryString["num"], "");
				if (id_number.Length > 1)
				{
					//initialize service:
					//DataServices.DataService _service = new DataServices.DataService();
					//_service.CookieContainer = Sport.Core.Session.Cookies;

					//get student details:
					//DataServices.StudentData student = _service.GetStudentByNumber(id_number);
					SportCommon.StudentData student = null;
					try
					{
						student = DB.Instance.GetStudentByNumber(id_number);
					}
					catch (Exception ex)
					{
						Response.Write("<script type=\"text/javascript\">");
						Response.Write(" parent.window.alert('שגיאה כללית בעת טעינת מידע:\\n" + ex.Message.Replace("'", "\\'") + "');\n");
						Response.Write(" parent.document.body.style.cursor = \"default\";");
						Response.Write("</script>");
					}

					if (student != null)
					{
						string strGrade = "";
						if (student.ID >= 0)
							strGrade = lookupGrades.Lookup(student.Grade);

						Response.Write("<script type=\"text/javascript\">");
						//Response.Write(" parent.alert(\"hello\");");
						Response.Write("</script>");

						Response.Write("<script type=\"text/javascript\" src=\"" + Common.Data.AppPath + "/Common/Common.js\">");
						Response.Write("</script>");
						Response.Write("<script type=\"text/javascript\">");
						Response.Write(" var tblStudentsID=\"" + TblStudents.UniqueID + "\";");
						Response.Write(" parent.FindStudentResults('" + student.ID + "', " +
							"'" + student.FirstName.Replace("'", "\\'") + "', " +
							"'" + student.LastName.Replace("'", "\\'") + "', " +
							"'" + student.IdNumber + "', " +
							"'" + strGrade.Replace("'", "\\'") + "');");
						Response.Write("");
						Response.Write("</script>");
					}
				}
				Sport.Core.Session.Cookies = null;
				Response.End();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
			this.Unload += new System.EventHandler(this.Page_Unload);

		}
		#endregion
	}
}
