using System;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sport.Data;
using System.IO;
using Sport.Types;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using ISF.DataLayer;
using System.Net.Mail;
using System.Configuration;

namespace SportSite.Common
{
	/// <summary>
	/// Provides static tools for various needs.
	/// </summary>
	public static class Tools
	{
		#region Public Methods
		#region string manipulation methods
		#region Split
		/// <summary>
		/// split given string according to delimeter character, also 
		/// allow escape character by double character.
		/// for example SplitWithEscape("A,B,,C,D", ',') would return array with
		/// three items: "A", "B,C" and "D".
		/// </summary>
		/// <param name="str">string to split</param>
		/// <param name="c">delimeter character</param>
		/// <returns>array of sub strings</returns>
		public static string[] SplitWithEscape(string str, char c)
		{
			if (str == null || str.Length == 0)
				return null;
			string strTemp = str;
			string strChar = c.ToString();
			string escapeString = "[ESCAPE]";
			strTemp = strTemp.Replace(strChar + strChar, escapeString);
			string[] result = strTemp.Split(new char[] { c });
			for (int i = 0; i < result.Length; i++)
			{
				if (result[i].IndexOf(escapeString) >= 0)
					result[i] = result[i].Replace(escapeString, strChar);
			}
			return result;
		}
		#endregion

		#region Conversion
		/// <summary>
		/// try to convert given object into string, if null or fails return default.
		/// </summary>
		public static string CStrDef(object obj, string def, int maxChars)
		{
			if ((obj == null) || (obj == DBNull.Value))
				return def;
			string result = obj.ToString();
			if ((maxChars < 0) || (maxChars >= result.Length))
				return result;
			return obj.ToString().Substring(0, maxChars);
		}

		public static string CStrN(object obj, string def)
		{
			if ((obj == null) || (obj == DBNull.Value) || (obj.ToString().Length == 0))
				return def;
			return obj.ToString();
		}

		public static string CStrDef(object obj, string def)
		{
			return CStrDef(obj, def, -1);
		}
		#endregion

		#region general string manipulation
		public static string SplitString(string value, string delimeter)
		{
			if (value == null || value.Length == 0)
				return "";

			return string.Join(delimeter, value.ToList().ConvertAll(c => ((int)c).ToString()));
		}

		/// <summary>
		/// returns the given string multiplied by the given amount.
		/// </summary>
		public static string MultiString(string str, int mult)
		{
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			for (int i = 1; i <= mult; i++)
			{
				builder.Append(str);
			}
			return builder.ToString();
		}

		/// <summary>
		/// truncate the string after given amount of characters and add "..." instead
		/// </summary>
		public static string TruncateString(string str, int charsCount)
		{
			if (charsCount >= str.Length)
				return ToHTML(str);

			//get first characters:
			string result = str.Substring(0, charsCount);

			//return if it's all the string or if ends with word:
			if ((charsCount >= str.Length) || (str[charsCount] == ' '))
				return result + "...";

			if (str.IndexOf(" ") < 0)
				return str.Substring(0, charsCount - 3) + "...";

			//truncate until previous word:
			while ((result.Length > 0) && (result[result.Length - 1] != ' ') &&
				(result[result.Length - 1] != '\n'))
			{
				result = result.Substring(0, result.Length - 1);
			}

			if (result.Length > 0)
				result = result.Substring(0, result.Length - 1);
			result = result.Replace("\n", "<br />");

			return result + "...";
		}

		public static string TruncateURL(string strURL, int maxChars)
		{
			if ((strURL == null) || (strURL.Length <= maxChars))
				return strURL;

			string[] arrParts = strURL.Split(new char[] { '/' });
			bool blnFound = false;
			string result = "";
			for (int i = 0; i < arrParts.Length; i++)
			{
				string curPart = arrParts[i];
				if (!blnFound)
				{
					string[] arrTmp = curPart.Split(new char[] { '.' });
					if (arrTmp.Length >= 3)
					{
						blnFound = true;
						result += curPart + "/";
					}
				}
				if (blnFound)
				{
					if (i == (arrParts.Length - 1))
						result += curPart;
				}
				else
				{
					result += curPart + "/";
				}
			}
			return result;
		}
		#endregion

		#region array related methods
		/// <summary>
		/// returns the amount of empty items (null or empty string) in given array.
		/// </summary>
		public static int CountEmptyItems(string[] arr)
		{
			int result = 0;
			if (arr == null)
				return result;
			foreach (string s in arr)
				result += (Tools.CStrDef(s, "").Length == 0) ? 1 : 0;
			return result;
		}

		/// <summary>
		/// add the given string as new item in the array and returns the new array.
		/// </summary>
		public static string[] AddToArray(string[] arr, string item)
		{
			string[] result = new string[arr.Length + 1];
			for (int i = 0; i < arr.Length; i++)
				result[i] = arr[i];
			result[arr.Length] = item;
			return result;
		}

		/// <summary>
		/// convert given collection to key=value format string, except for given keys.
		/// </summary>
		public static string CollectionToString(
			System.Collections.Specialized.NameValueCollection collection,
			string seperator, string[] exceptions)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < collection.Keys.Count; i++)
			{
				string key = collection.Keys[i];
				if (Sport.Common.Tools.InArray(exceptions, key) == -1)
				{
					string value = collection[key];
					if ((value != null) && (value.Length > 0))
						sb.Append(key + "=" + value + seperator);
				}
			}
			string result = sb.ToString();
			if ((result != null) && (result.Length > 0))
				result = StringTools.Left(result, result.Length - seperator.Length);
			return result;
		}

		public static string CollectionToString(
			System.Collections.Specialized.NameValueCollection collection,
			string seperator)
		{
			return CollectionToString(collection, seperator, null);
		}
		#endregion

		#region email
		public static void SendEmail(string strFrom, string strTo,
			string strSubject, string strBody)
		{
			/*
			string host = ConfigurationManager.AppSettings["SmtpHost"] + "";
			if (host.Length == 0)
				host = "localhost";
			SmtpClient client = new SmtpClient(host);
			//client.UseDefaultCredentials = true;
			MailMessage message = new MailMessage(strFrom, strTo, strSubject, strBody);
			message.IsBodyHtml = true;
			client.Send(message);
			*/

			Sport.Common.Tools.sendEmail(strFrom, strTo, strSubject, strBody);
		}
		#endregion

		#region cleaning
		public static string CleanForJavascript(string s)
		{
			return s.Replace("\n\r", "\n").Replace("\r\n", "\n").Replace("\n", "").Replace("\"", "\\\"");
		}
		#endregion
		#endregion

		#region entity related methods
		#region instant messages
		public static Sport.Entities.InstantMessage GetPendingMessage(int userid)
		{
			EntityFilter filter = new EntityFilter(
				(int)Sport.Entities.InstantMessage.Fields.Recipient, userid);
			filter.Add(new EntityFilterField(
				(int)Sport.Entities.InstantMessage.Fields.DateRead, null));
			Sport.Data.Entity[] entities =
				Sport.Entities.InstantMessage.Type.GetEntities(filter);
			if ((entities != null) && (entities.Length > 0))
				return new Sport.Entities.InstantMessage(entities[0]);
			return null;
		}

		public static string GetInstantMessageHTML(
			Sport.Entities.InstantMessage message,
			System.Web.HttpServerUtility Server, System.Web.HttpRequest Request)
		{
			//got anything?
			if ((message == null) || (message.Id < 0))
				return "";

			//file path:
			string strFilePath = Server.MapPath(Data.AppPath + "/Common/InstantMessage.html");

			//check file:
			if (!File.Exists(strFilePath))
				return "---Error: template file does not exist!---";

			//read file contents:
			string result = String.Join("\n", Sport.Common.Tools.ReadTextFile(strFilePath, true));

			//replace with proper data:
			result = result.Replace("%sender", message.Sender.FirstName + " " +
				message.Sender.LastName);
			result = result.Replace("%datesent",
				message.DateSent.ToString("dd/MM/yyyy HH:mm:ss"));
			result = result.Replace("%contents",
				Tools.PutHTML_NewLines(message.Contents));
			result = result.Replace("%id", message.Id.ToString());
			result = result.Replace("%server_url", Tools.GetFullURL(Request));

			//done.
			return result;
		}
		#endregion

		public static string BuildNewStudentForm(int category)
		{
			int gradeCounter = 0;
			Sport.Types.GradeTypeLookup.ResetRelativeGrades();
			LookupType lookupGrades = new Sport.Types.GradeTypeLookup(true);
			List<LookupItem> availableGrades = lookupGrades.Items.ToList();
			StringBuilder strHtml = new StringBuilder();
			strHtml.AppendFormat("<span id=\"CategoryAvailableGrades\" style=\"display: none;\">{0}</span>",
				string.Join(",", lookupGrades.Items.ToList().FindAll(l => CategoryTypeLookup.Contains(category, gradeCounter++)).
				ConvertAll(l => l.Text)));
			strHtml.Append("<div id=\"pnlAddNewStudent\" style=\"display: none;\">");
			strHtml.Append("<h2>טופס הוספת תלמיד חדש למאגר</h2>");
			strHtml.Append("מספר זהות: <span class=\"new_student_id_number\"></span><br />");
			strHtml.Append("<label>שם פרטי:</label> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type=\"text\" class=\"new_student_first_name\" size=\"20\" maxlength=\"15\" lang=\"he\" /><br />");
			strHtml.Append("<label>שם משפחה:</label> <input type=\"text\" class=\"new_student_last_name\" size=\"20\" maxlength=\"20\" lang=\"he\" /><br />");
			strHtml.Append("<label>תאריך לידה:</label> &nbsp;&nbsp;<input type=\"text\" class=\"new_student_birthday\" size=\"15\" maxlength=\"10\" placeholder=\"dd/MM/yyyy\" /><br />");
			strHtml.Append("<label>כיתה:</label> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;").AppendFormat("<select class=\"new_student_grade\">{0}</select><br />", 
				string.Join("", availableGrades.ConvertAll(l => BuildOption(l.Id.ToString(), l.Text, false))));
			strHtml.Append("<button type=\"button\" id=\"btnSendNewStudent\">הוסף תלמיד למאגר</button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
			strHtml.Append("<button type=\"button\" id=\"btnCancelNewStudent\">ביטול הוספת תלמיד חדש</button>");
			strHtml.Append("</div>");
			return strHtml.ToString();
		}

		public static bool CheckDisplayStudentPicture()
		{
			HttpContext context = HttpContext.Current;
			string strPictureStudentId;
			if (context.Request.QueryString["action"] == "DisplayStudentPicture" && TryGetStudentNumberFromQueryString(out strPictureStudentId))
			{
				context.Response.Clear();
				SportSite.Core.UserData loggedInUser = SportSite.Core.UserData.Empty;
				if (context.Session[SportSite.Core.UserManager.SessionKey] is SportSite.Core.UserData)
					loggedInUser = (SportSite.Core.UserData)context.Session[SportSite.Core.UserManager.SessionKey];
				if (loggedInUser.Equals(SportSite.Core.UserData.Empty))
				{
					context.Response.Write("<h1 style=\"color: red;\">Not Authorized</h1>");
				}
				else
				{
					string imageFilePath = StudentPictureHelper.GetExistingPicture(strPictureStudentId);
					if (imageFilePath.Length == 0)
					{
						context.Response.Write("<h1 style=\"color: red;\">No Picture</h1>");
					}
					else
					{
						context.Response.ContentType = "image/" + Path.GetExtension(imageFilePath).Replace(".", "").ToLower();
						context.Response.AddHeader("content-disposition", "attachment;filename=" + Path.GetFileName(imageFilePath));
						context.Response.WriteFile(imageFilePath);
					}
				}
				context.Response.End();
				return true;
			}

			return false;
		}

		public static bool CheckGetStudentTeams()
		{
			HttpContext context = HttpContext.Current;
			string studentIdNumber;
			if (context.Request.QueryString["action"] == "GetStudentTeams" && TryGetStudentNumberFromQueryString(out studentIdNumber))
			{
				context.Response.Clear();
				Dictionary<string, string> jsonOutput = new Dictionary<string, string>();
				jsonOutput.Add("success", "false");
				jsonOutput.Add("error", "");
				SportSite.Core.UserData loggedInUser = SportSite.Core.UserData.Empty;
				if (context.Session[SportSite.Core.UserManager.SessionKey] is SportSite.Core.UserData)
					loggedInUser = (SportSite.Core.UserData)context.Session[SportSite.Core.UserManager.SessionKey];
				if (loggedInUser.Equals(SportSite.Core.UserData.Empty))
				{
					jsonOutput["error"] = "Not Authorized";
				}
				else
				{
					Entity student = GetStudentByIdNumber(studentIdNumber);
					if (student != null)
					{
						List<string> teams = new List<string>();
						Sport.Entities.Player[] players = null;
						try
						{
							players = new Sport.Entities.Student(student).GetPlayers();
						}
						catch
						{
						}

						if (players != null)
						{
							players.ToList().ForEach(p =>
							{
								string curName = "";
								try
								{
									curName = p.Team.Name;
								}
								catch
								{
								}
								if (curName.Length > 0 && teams.IndexOf(curName) < 0)
									teams.Add(curName);
							});
						}
						if (teams.Count > 0)
							jsonOutput.Add("teams", "['" + string.Join("', '", teams.ConvertAll(t => t.Replace("'", ""))) + "']");
						else
							jsonOutput.Add("teams", "[]");
						jsonOutput["success"] = "true";
					}
					else
					{
						jsonOutput["error"] = "Student does not exist";
					}
				}
				context.Response.Clear();
				context.Response.Write(BuildJsonString(jsonOutput));
				context.Response.End();
				return true;
			}

			return false;
		}

		public static bool CheckFindStudentByIdNumber()
		{
			HttpContext context = HttpContext.Current;
			Dictionary<string, string> jsonOutput = new Dictionary<string, string>();
			int idNumber;
			if (context.Request.QueryString["action"] == "FindStudentById")
			{
				if (Int32.TryParse(context.Request.QueryString["id_number"], out idNumber) && idNumber > 0 && idNumber.ToString().Length > 5)
				{
					LookupType lookupGrades = new Sport.Types.GradeTypeLookup(true);
					using (DataServices1.DataService service = new DataServices1.DataService())
					{
						DataServices1.StudentData data = service.GetStudentByNumber(idNumber.ToString());
						jsonOutput.Add("exists", "false");
						if (data != null && data.ID > 0)
						{
							jsonOutput["exists"] = "true";
							jsonOutput.Add("internal_id", data.ID.ToString());
							jsonOutput.Add("school_id", data.School.ID.ToString());
							jsonOutput.Add("school_name", new Sport.Entities.School(data.School.ID).Name);
							jsonOutput.Add("first_name", data.FirstName);
							jsonOutput.Add("last_name", data.LastName);
							jsonOutput.Add("grade", lookupGrades.Lookup(data.Grade));
							jsonOutput.Add("birthday", (data.Birthdate.Year > 1900 ? data.Birthdate.ToString("dd/MM/yyyy") : ""));
						}
						else
						{
							string dummy;
							jsonOutput.Add("valid", Sport.Common.Tools.IsValidIdNumber(idNumber.ToString(), out dummy).ToString().ToLower());
						}
					}
				}
			}
			else if (context.Request.QueryString["action"] == "CheckExternalStudent")
			{
				if (Int32.TryParse(context.Request.QueryString["id_number"], out idNumber) && idNumber > 0 && idNumber.ToString().Length > 5)
				{
					using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
					{
						jsonOutput.Add("external", service.IsExternallyAddedStudent(idNumber).ToString().ToLower());
					}
				}
			}

			if (jsonOutput.Count > 0)
			{
				context.Response.Clear();
				context.Response.Write(BuildJsonString(jsonOutput));
				context.Response.End();
				return true;
			}

			return false;
		}

		public static bool CheckAddStudent(int loggedInUserId, int schoolId)
		{
			HttpContext context = HttpContext.Current;
			if (context.Request.Form["action"] == "AddExternalStudent")
			{
				int idNumber, grade;
				string firstName = (context.Request.Form["first_name"] + "").Trim();
				string lastName = (context.Request.Form["last_name"] + "").Trim();
				DateTime? birthDate = ParseRawDate((context.Request.Form["birthdate"] + "").Trim());
				bool existing = false;
				if (Int32.TryParse(context.Request.Form["id_number"], out idNumber) && idNumber > 0 && idNumber.ToString().Length > 5 &&
					Int32.TryParse(context.Request.Form["grade"], out grade) && grade > 0)
				{
					string errorMsg = "";
					try
					{
						Sport.Data.EntityEdit studentEdit = GetStudentEdit(idNumber, out existing, out errorMsg);
						if (errorMsg.Length == 0)
						{
							if (!existing)
							{
								studentEdit.Fields[(int)Sport.Entities.Student.Fields.IdNumber] = idNumber;
								studentEdit.Fields[(int)Sport.Entities.Student.Fields.School] = schoolId; //(new Sport.Entities.School(schoolId)).Entity;
							}
							studentEdit.Fields[(int)Sport.Entities.Student.Fields.FirstName] = firstName;
							studentEdit.Fields[(int)Sport.Entities.Student.Fields.LastName] = lastName;
							studentEdit.Fields[(int)Sport.Entities.Student.Fields.Grade] = grade;
							if (birthDate != null)
								studentEdit.Fields[(int)Sport.Entities.Student.Fields.BirthDate] = birthDate.Value;
							EntityResult result = studentEdit.Save();
							if (!result.Succeeded)
								errorMsg = result.GetMessage();
						}
					}
					catch (Exception ex)
					{
						errorMsg = ex.Message;
					}

					Dictionary<string, string> jsonOutput = new Dictionary<string, string>();
					jsonOutput.Add("success", "false");
					if (errorMsg.Length == 0)
					{
						Entity student = GetStudentByIdNumber(idNumber.ToString());
						if (student != null)
						{
							if (!existing)
							{
								WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService();
								try
								{
									service.MapExternalStudent(student.Id, loggedInUserId, GetIPAddress());
								}
								catch (Exception ex)
								{
									errorMsg = ex.Message;
									if (ex.InnerException != null && (ex.InnerException.Message + "").Length > 0)
										errorMsg = ex.InnerException.Message;
									jsonOutput.Add("error", "שגיאה בעת מיפוי נתונים: " + errorMsg);
									try
									{
										student.Delete();
									}
									catch
									{
									}
								}
								service.Dispose();
							}
							if (errorMsg.Length == 0)
							{
								jsonOutput["success"] = "true";
								jsonOutput.Add("internal_id", student.Id.ToString());
							}
						}
						else
						{
							jsonOutput.Add("error", "שגיאה כללית: נתוני תלמיד לא נשמרו");
						}
					}
					else
					{
						jsonOutput.Add("error", errorMsg);
					}
					context.Response.Clear();
					context.Response.Write(BuildJsonString(jsonOutput));
					context.Response.End();
					return true;
				}
			}

			return false;
		}

		private static Sport.Data.EntityEdit GetStudentEdit(int idNumber, out bool existing, out string errorMsg)
		{
			existing = false;
			errorMsg = "";
			EntityFilter filter = new EntityFilter();
			filter.Add(new EntityFilterField((int)Sport.Entities.Student.Fields.IdNumber, idNumber));
			Entity[] arrExistingStudents = null;
			try
			{
				arrExistingStudents = Sport.Entities.Student.Type.GetEntities(filter);
			}
			catch
			{
			}

			if (arrExistingStudents != null && arrExistingStudents.Length == 1)
			{
				bool blnExternal = false;
				using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
				{
					blnExternal = service.IsExternallyAddedStudent(idNumber);
				}
				if (!blnExternal)
				{
					errorMsg = "לא ניתן לערוך נתונים של תלמיד שלא נוסף דרך האתר";
					return null;
				}
				existing = true;
				return arrExistingStudents[0].Edit();
			}
			
			return Sport.Entities.Student.Type.New();
		}

		public static bool VerifyTeam(int userSchoolId, out Sport.Entities.Team team, out string errorMsg)
		{
			errorMsg = "";
			team = null;
			int teamId;
			if (!Int32.TryParse(HttpContext.Current.Request.QueryString["team"], out teamId) || teamId <= 0)
			{
				errorMsg = "זיהוי קבוצה חסר או שגוי";
				return false;
			}

			try
			{
				team = new Sport.Entities.Team(teamId);
			}
			catch
			{
			}

			if (team == null)
			{
				errorMsg = "זיהוי קבוצה לא חוקי";
				return false;
			}

			if (team.School.Id != userSchoolId)
			{
				errorMsg = "אינך מורשה לרשום שחקנים עבור קבוצה זו";
				return false;
			}

			return true;
		}

		public static int[] ExtractValidStudentsForRegistration(string rawValue, Sport.Entities.Team team)
		{
			List<int> studentIds = new List<int>();
			int currentStudentId;
			List<int> arrAlreadyInTeam = FindAllStudentsInTeam(team);
			rawValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(rawStudentId =>
			{
				if (Int32.TryParse(rawStudentId, out currentStudentId) && currentStudentId > 0)
				{
					Sport.Entities.Student student = null;
					try
					{
						student = new Sport.Entities.Student(currentStudentId);
					}
					catch
					{
					}
					if (student != null && !arrAlreadyInTeam.Contains(currentStudentId) && studentIds.IndexOf(currentStudentId) < 0)
						studentIds.Add(currentStudentId);
				}
			});
			return studentIds.ToArray();
		}
		
		public static List<int> FindAllStudentsInTeam(Sport.Entities.Team team)
		{
			List<int> studentIds = Sport.Entities.Player.Type.GetEntities(new EntityFilter((int)Sport.Entities.Player.Fields.Team, team.Id)).ToList().ConvertAll(playerEntity =>
				(int)playerEntity.Fields[(int)Sport.Entities.Player.Fields.Student]);
			SportSite.RegistrationService.RegistrationService regService = new SportSite.RegistrationService.RegistrationService();
			regService.CookieContainer = (System.Net.CookieContainer)HttpContext.Current.Session["cookies"];
			studentIds.AddRange(regService.GetPendingPlayers(team.School.Id, team.Id).ToList().ConvertAll(p => p.student_id));
			studentIds = studentIds.Distinct().ToList();
			return studentIds;
		}

		public static Table BuildPlayerSelectionTable(WebSiteServices.FullTeamData teamData)
		{
			int category = teamData.Category.Category;
			Table tblStudents = new Table();
			tblStudents.BorderWidth = Unit.Pixel(1);
			LookupType lookupGrades = new Sport.Types.GradeTypeLookup(true);
			tblStudents.Rows.Add(BuildCaptionRow("הערות", "כיתה", "שם משפחה", "שם פרטי", "מספר תעודת זהות", "סמן לבחירה"));
			tblStudents.CssClass = SportSite.Common.Style.PlayersTableCss;
			tblStudents.Attributes["data-fixedrows"] = "1";
			List<WebSiteServices.FullStudentData> allStudents = new List<WebSiteServices.FullStudentData>();
			using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
			{
				allStudents.AddRange(service.GetTeamStudents(teamData.Id));
			}
			//allStudents = allStudents.Distinct((s1, s2) => s1.IdNumber == s2.IdNumber).ToList();
			allStudents = allStudents.GroupBy(student => student.IdNumber).Select(grp => grp.First()).ToList();
			allStudents.RemoveAll(s => CategoryTypeLookup.Compare(category, s.Grade, CategoryCompareType.Grade) == false);
			List<int> arrAlreadyInTeam = allStudents.FindAll(s =>
				s.Type == WebSiteServices.FullStudentType.Pending || s.Type == WebSiteServices.FullStudentType.Player).
				ConvertAll(s => s.Id);
			allStudents.Sort((s1, s2) =>
			{
				int c1 = lookupGrades.Lookup(s1.Grade).CompareTo(lookupGrades.Lookup(s2.Grade));
				if (c1 != 0)
					return c1;
				int c2 = s1.FirstName.CompareTo(s2.FirstName);
				if (c2 != 0)
					return c2;
				return s1.LastName.CompareTo(s2.LastName);
			});
			TableRow blankRow = BuildOneStudentTableRow(false, lookupGrades, null);
			blankRow.CssClass = "student_template_row";
			blankRow.Style["display"] = "none";
			tblStudents.Rows.Add(blankRow);
			tblStudents.Rows.AddRange(allStudents.ConvertAll(student =>
			{
				bool disabled = arrAlreadyInTeam.Contains(student.Id);
				return BuildOneStudentTableRow(disabled, lookupGrades, student);
			}).ToArray());
			tblStudents.Attributes["data-colors"] = Sport.Common.Tools.ColorsToHex(Common.Style.ChoosePlayerColors, ",");
			tblStudents.Attributes["data-addlight"] = "20";
			return tblStudents;
		}

		public static TableRow BuildOneStudentTableRow(bool disabled, LookupType lookupGrades, WebSiteServices.FullStudentData studentData)
		{
			string grade = (studentData == null) ? "&nbsp;" : lookupGrades.Lookup(studentData.Grade);
			string lastName = (studentData == null) ? "&nbsp;" : studentData.LastName;
			string firstName = (studentData == null) ? "&nbsp;" : studentData.FirstName;
			string idNumber = (studentData == null) ? "&nbsp;" : studentData.IdNumber;
			string checkboxHtml = string.Format("<input type=\"checkbox\" name=\"selected_players\"{0} value=\"{1}\" />",
				(disabled) ? " disabled=\"disabled\"" : "", (studentData == null) ? "" : studentData.Id.ToString());
			TableRow row = BuildTableRow(disabled, "&nbsp;", grade, lastName, firstName, idNumber, checkboxHtml);
			if (disabled)
				row.BackColor = Color.White;
			row.Cells[0].CssClass = "student_comments";
			row.Cells[1].CssClass = "student_grade";
			row.Cells[2].CssClass = "student_last_name";
			row.Cells[3].CssClass = "student_first_name";
			row.Cells[4].CssClass = "student_id_cell";
			string birthDay = "";
			if (studentData != null && studentData.BirthDate.Year > 1900)
				birthDay = studentData.BirthDate.ToString("dd/MM/yyyy");
			row.Attributes["data-birthday"] = birthDay;
			return row;
		}

		public static string CreateEntitiesCombo(string strDescription, string strComboName,
			Sport.Data.EntityType type, List<int> exceptions, ref int entitiesCount)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<p>" + strDescription + ": <select name=\"" + strComboName + "\">");
			Sport.Data.Entity[] arrEntities = type.GetEntities(null);
			entitiesCount = 0;
			foreach (Sport.Data.Entity entity in arrEntities)
			{
				if (IsValidEntity(entity))
				{
					if (exceptions != null && exceptions.IndexOf(entity.Id) >= 0)
					{
						//ignore
					}
					else
					{
						entitiesCount++;
						sb.Append("<option value=\"" + entity.Id + "\">" + entity.Name + "</option>");
					}
				}
			}
			sb.Append("</select></p>");
			return sb.ToString();
		}

		public static string CreateEntitiesCombo(string strDescription, string strComboName,
			Sport.Data.EntityType type)
		{
			int dummy = 0;
			return CreateEntitiesCombo(strDescription, strComboName, type, null, ref dummy);
		}

		public static bool IsValidEntity(Sport.Data.Entity entity)
		{
			if (entity.Id < 0)
				return false;

			if (entity.EntityType.Name == Sport.Entities.PracticeCamp.TypeName)
			{
				Sport.Entities.PracticeCamp camp = new Sport.Entities.PracticeCamp(entity);
				if (camp.DateFinish < DateTime.Now)
					return false;
			}
			return true;
		}

		public static string GetSportsCombo(int selSport)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<select name=\"sport\" onchange=\"SportChanged(this);\">");
			sb.Append("<option value=\"-1\"");
			if (selSport == -1)
				sb.Append(" selected=\"selected\"");
			sb.Append(">כל ענפי הספורט</option>");
			Entity[] sportEnts = Sport.Entities.Sport.Type.GetEntities(null);
			foreach (Entity sportEnt in sportEnts)
			{
				int curSport = sportEnt.Id;
				sb.Append("<option value=\"" + curSport + "\"");
				if (selSport == curSport)
					sb.Append(" selected=\"selected\"");
				sb.Append(">" + sportEnt.Name + "</option>");
			}
			sb.Append("</select>");
			return sb.ToString();
		}

		public static Sport.Data.EntityFilter GetChampionshipDefaultFilter(bool blnLatest, bool blnOpen)
		{
			Sport.Data.EntityFilter result = new Sport.Data.EntityFilter();
			result.Add(new Sport.Data.EntityFilterField(
				(int)Sport.Entities.Championship.Fields.Status,
				(int)Sport.Types.ChampionshipType.InitialPlanning, true));
			EntityFilterField seasonField = null;
			if (blnOpen)
			{
				seasonField = Sport.Entities.Championship.OpenSeasonFilter();
			}
			else if (blnLatest)
			{
				seasonField = Sport.Entities.Championship.LatestSeasonFilter();
			}
			else
			{
				int curSeason = Tools.CIntDef(System.Web.HttpContext.Current.Session["season"], -1);
				if (curSeason < 0)
					seasonField = Sport.Entities.Championship.CurrentSeasonFilter();
				else
					seasonField = new EntityFilterField((int)Sport.Entities.Championship.Fields.Season, curSeason);
			}
			result.Add(seasonField);
			return result;
		}

		public static Sport.Data.EntityFilter GetChampionshipDefaultFilter(bool blnLatest)
		{
			return GetChampionshipDefaultFilter(blnLatest, false);
		}

		public static string GetChampionshipCombo(int sport, int selChamp)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<select name=\"championship\" onchange=\"ChampChanged(this);\">");
			sb.Append("<option value=\"-1\"");
			if (selChamp == -1)
				sb.Append(" selected=\"selected\"");
			sb.Append(">כל האליפויות</option>");
			if (sport >= 0)
			{
				EntityFilter filter = Tools.GetChampionshipDefaultFilter(true);
				filter.Add(new EntityFilterField(
					(int)Sport.Entities.Championship.Fields.Sport, sport));
				Entity[] champEnts = Sport.Entities.Championship.Type.GetEntities(filter);
				foreach (Entity champEnt in champEnts)
				{
					int curChamp = champEnt.Id;
					sb.Append("<option value=\"" + curChamp + "\"");
					if (selChamp == curChamp)
						sb.Append(" selected=\"selected\"");
					sb.Append(">" + champEnt.Name + "</option>");
				}
			}
			sb.Append("</select>");
			return sb.ToString();
		}

		public static string GetChampCategoryCombo(int champ, int selCategory)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<select name=\"category\">");
			sb.Append("<option value=\"-1\"");
			if (selCategory == -1)
				sb.Append(" selected=\"selected\"");
			sb.Append(">כל הקטגוריות</option>");
			if (champ >= 0)
			{
				EntityFilter filter = new EntityFilter(
					(int)Sport.Entities.ChampionshipCategory.Fields.Championship, champ);
				Entity[] categoryEnts = Sport.Entities.ChampionshipCategory.Type.GetEntities(filter);
				foreach (Entity categoryEnt in categoryEnts)
				{
					int curCategory = categoryEnt.Id;
					sb.Append("<option value=\"" + curCategory + "\"");
					if (selCategory == curCategory)
						sb.Append(" selected=\"selected\"");
					sb.Append(">" + categoryEnt.Name + "</option>");
				}
			}
			sb.Append("</select>");
			return sb.ToString();
		}

		/// <summary>
		/// return the name of the entity with given id or null if does not exist.
		/// </summary>
		public static string GetEntityName(string entityTypeName, int entID)
		{
			if (entID < 0)
				return "";

			EntityType entType = EntityType.GetEntityType(entityTypeName);
			Entity ent = entType.Lookup(entID);
			return (ent == null) ? null : ent.Name;
		}

		public static Sport.Championships.Match[] GetRoundMatches(Sport.Championships.Round round)
		{
			System.Collections.ArrayList matches = new System.Collections.ArrayList();
			if (round.Cycles != null)
			{
				foreach (Sport.Championships.Cycle cycle in round.Cycles)
				{
					if (cycle.Matches != null)
					{
						foreach (Sport.Championships.Match match in cycle.Matches)
							matches.Add(match);
					}
				}
			}
			return (Sport.Championships.Match[])matches.ToArray(typeof(Sport.Championships.Match));
		}

		#endregion

		#region sorting methods
		public static void SortByValue(ListControl combo)
		{
			SortCombo(combo, new ComboValueComparer());
		}

		public static void SortByText(ListControl combo)
		{
			SortCombo(combo, new ComboTextComparer());
		}
		#endregion

		#region html elements creation
		#region tables
		public static TableRow BuildCaptionRow(params string[] headers)
		{
			TableRow row = new TableRow();
			headers.ToList().ForEach(headerText => row.Cells.Add(BuildHeaderCell(headerText)));
			return row;
		}

		public static TableRow BuildTableRow(bool disabled, params string[] values)
		{
			TableRow row = new TableRow();
			values.ToList().ForEach(cellText => row.Cells.Add(BuildTableCell(cellText, disabled)));
			return row;
		}

		public static TableHeaderCell BuildHeaderCell(string contents)
		{
			TableHeaderCell cell = new TableHeaderCell();
			cell.Text = contents;
			return cell;
		}

		public static TableCell BuildTableCell(string text, bool disabled)
		{
			TableCell result = Common.FastControls.HebTableCell(text);
			if (disabled)
				result.ForeColor = System.Drawing.SystemColors.GrayText;
			return result;
		}

		public static string BuildDetailsTable(string[] captions, string[] texts,
			string strSpecial)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<table class=\"DetailsTable\" border=\"0\">");
			sb.Append("<tr><td style=\"width: 15%;\">&nbsp;</td><td style=\"width: 85%;\">&nbsp;</td></tr>");
			for (int i = 0; i < captions.Length; i++)
			{
				string caption = captions[i];
				string text = texts[i];
				if (caption.Length == 0)
					continue;
				if (text.Length == 0)
					text = "&nbsp;";
				if (caption[caption.Length - 1] != ':')
					caption += ":";
				sb.Append("<tr>");
				sb.Append("<td style=\"font-weight: bold;\">" + caption + "</td>");
				sb.Append("<td>" + text + "</td>");
				sb.Append("</tr>");
			}
			if ((strSpecial != null) && (strSpecial.Length > 0))
			{
				sb.Append("<tr>");
				sb.Append("<td colspan=\"2\" align=\"right\">");
				sb.Append("<span style=\"font-weight: bold;\">");
				sb.Append(strSpecial);
				sb.Append("</span>");
				sb.Append("</td>");
				sb.Append("</tr>");
			}
			sb.Append(Tools.BuildEmptyRow(10, 2));
			sb.Append("</table>");
			return sb.ToString();
		}

		public static string BuildDetailsTable(string[] captions, string[] texts)
		{
			return BuildDetailsTable(captions, texts, null);
		}

		public static string BuildChampTable(string[] captions, ArrayList rows,
			ArrayList arrNoWrap, Hashtable tblCellColors, string title)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<table class=\"ChampTable\" cellspacing=\"0\" cellpadding=\"0\">");
			if ((title != null) && (title.Length > 0))
				sb.Append("<caption>" + title + "</caption>");
			sb.Append("<tr>");
			sb.Append("<th class=\"ChampTableRightHeader\">&nbsp;</th>");
			for (int i = 0; i < captions.Length; i++)
			{
				string caption = captions[i];
				sb.Append("<th class=\"ChampTableHeader\"");
				if (i == 0)
					sb.Append(" style=\"border-right: solid 0px Blue;\"");
				if ((arrNoWrap != null) && (arrNoWrap.IndexOf(i) >= 0))
					sb.Append(" nowrap=\"nowrap\"");
				sb.Append(">" + ((caption.Length > 0) ? caption : "&nbsp;") + "</th>");
			}
			sb.Append("<th class=\"ChampTableLeftHeader\" style=\"border-right: solid 0px Blue;\">&nbsp;</th>");
			sb.Append("</tr>");
			int rowIndex = 0;
			foreach (string[] cells in rows)
			{
				sb.Append("<tr>");
				for (int i = 0; i < cells.Length; i++)
				{
					string text = cells[i];
					int colspan = 0;
					if ((i == (cells.Length - 1)) && (cells.Length < captions.Length))
					{
						colspan = captions.Length - cells.Length + 3;
					}
					else
					{
						if ((i == 0) || (i == cells.Length - 1))
							colspan = 2;
					}
					Color bgColor = Color.Empty;
					if (tblCellColors != null)
					{
						if (tblCellColors[rowIndex] != null)
						{
							Hashtable tblRowColors = (Hashtable)tblCellColors[rowIndex];
							if (tblRowColors[i] != null)
								bgColor = (Color)tblRowColors[i];
						}
					}
					sb.Append("<td");
					if (colspan > 0)
						sb.Append(" colspan=\"" + colspan + "\"");
					if ((arrNoWrap != null) && (arrNoWrap.IndexOf(i) >= 0))
						sb.Append(" nowrap=\"nowrap\"");
					if (!bgColor.Equals(Color.Empty))
					{
						sb.Append(" style=\"background-color: " +
							Sport.Common.Tools.ColorToHex(bgColor) + ";\"");
					}
					sb.Append(">" + ((text.Length > 0) ? text : "&nbsp;") + "</td>");
				}
				sb.Append("</tr>");
				rowIndex++;
			}
			sb.Append("</table>");
			return sb.ToString();
		}

		public static string BuildChampTable(string[] captions, ArrayList rows,
			ArrayList arrNoWrap, Hashtable tblCellColors)
		{
			return BuildChampTable(captions, rows, arrNoWrap, tblCellColors, null);
		}

		public static string BuildChampTable(string[] captions, ArrayList rows)
		{
			return BuildChampTable(captions, rows, null, null);
		}

		public static string BuildChampTable(Sport.Documents.Data.Table table)
		{
			if (table.Headers == null || table.Headers.Length == 0)
				return "";

			string[] captions = new string[table.Headers.Length];
			for (int i = 0; i < table.Headers.Length; i++)
				captions[i] = table.Headers[i].Text;
			ArrayList rows = new ArrayList();
			if (table.Rows != null)
			{
				foreach (Sport.Documents.Data.Row row in table.Rows)
				{
					if (row.Cells == null)
						continue;
					string[] cells = new string[row.Cells.Length];
					for (int i = 0; i < row.Cells.Length; i++)
						cells[i] = row.Cells[i].Text;
					rows.Add(cells);
				}
			}
			return BuildChampTable(captions, rows,
				new ArrayList(new int[] { captions.Length - 1 }), null, table.Caption);
		}

		public static string BuildChampTable(Sport.Documents.Data.Table[] tables)
		{
			if ((tables == null) || (tables.Length == 0))
				return "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < tables.Length; i++)
			{
				sb.Append(BuildChampTable(tables[i]));
				if (i < (tables.Length - 1))
					sb.Append("<hr />");
			}
			return sb.ToString();
		}

		public static string BuildTableRow(int[] cellsWidth, int height)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<tr");
			if (height > 0)
				sb.Append(" style=\"height: " + height + "px;\"");
			sb.Append(">");
			foreach (int width in cellsWidth)
				sb.Append("<td style=\"width: " + width + "px;\"></td>");
			sb.Append("</tr>");
			return sb.ToString();
		}

		public static string BuildTableRow(int[] cellsWidth)
		{
			return BuildTableRow(cellsWidth, -1);
		}

		public static string BuildBlankRow(int colSpan, int height)
		{
			string result = "<tr";
			if (height > 0)
				result += " style=\"height: " + height + "px;\"";
			result += ">";
			result += "<td colspan=\"" + colSpan + "\"></td>";
			result += "</tr>";
			return result;
		}

		private static string BuildAttachmentsTable(ArrayList attachments,
			System.Web.HttpServerUtility Server, bool blnRightToLeft)
		{
			if ((attachments == null) || (attachments.Count == 0))
				return "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<table class=\"AttachmentsTable\" cellspacing=\"0\" " +
				"cellpadding=\"0\"");
			if (!blnRightToLeft)
				sb.Append(" dir=\"ltr\" style=\"text-align: left;\"");
			sb.Append(">");
			string[] arrColors = new string[] { "#F7F7F6", "#EDEDEB" };
			for (int i = 0; i < attachments.Count; i++)
			{
				Core.AttachmentData data = Core.AttachmentData.Empty;
				if (attachments[i] is WebSiteServices.AttachmentData)
				{
					data = new Core.AttachmentData((WebSiteServices.AttachmentData)attachments[i], Server);
				}
				else
				{
					if (attachments[i] is Core.AttachmentData)
						data = (Core.AttachmentData)attachments[i];
				}
				if (data.ID >= 0)
					sb.Append(Core.AttachmentManager.AttachmentHtml_FullRow(data, arrColors[i % arrColors.Length], i, blnRightToLeft));
			}
			sb.Append("</table>");
			return sb.ToString();
		}

		private static string BuildAttachmentsTable(ArrayList attachments,
			System.Web.HttpServerUtility Server)
		{
			return BuildAttachmentsTable(attachments, Server, true);
		}

		public static string BuildAttachmentsTable(WebSiteServices.AttachmentData[] attachments,
			System.Web.HttpServerUtility Server, bool blnRightToLeft)
		{
			return BuildAttachmentsTable(new ArrayList(attachments), Server, blnRightToLeft);
		}

		public static string BuildAttachmentsTable(WebSiteServices.AttachmentData[] attachments,
			System.Web.HttpServerUtility Server)
		{
			return BuildAttachmentsTable(attachments, Server, true);
		}

		public static string BuildAttachmentsTable(Core.AttachmentData[] attachments,
			System.Web.HttpServerUtility Server)
		{
			return BuildAttachmentsTable(new ArrayList(attachments), Server);
		}

		public static string BuildTableRow(string[] captions, bool[] boldTexts, bool ignoreBlank)
		{
			if ((ignoreBlank) && (Tools.CountEmptyItems(captions) > 0))
				return "";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<tr>");
			for (int i = 0; i < captions.Length; i++)
			{
				string caption = captions[i];
				bool boldText = boldTexts[i];
				sb.Append("<td" + ((boldText) ? " style=\"font-weight: bold;\"" : "") + ">");
				sb.Append(caption);
				sb.Append("</td>");
			}
			sb.Append("</tr>");
			return sb.ToString();
		}

		public static string BuildEmptyRow(int height, int colSpan)
		{
			string result = "<tr>";
			result += "<td style=\"height: " + height + "px;\" colspan=\"" + colSpan + "\">";
			result += "&nbsp;";
			result += "</td></tr>";
			return result;
		}
		#endregion

		#region drop down lists
		public static string BuildUpDownCombo(string strHiddenField, string strComboName,
			string[] values, string[] texts, string strSeperator, string strCaption,
			bool blnMale)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int count = 0;
			string strHiddenValue = "";
			if ((values != null) && (values.Length > 0))
			{
				count = values.Length;
				strHiddenValue = String.Join(strSeperator, values);
			}
			if (texts != null)
			{
				count = texts.Length;
				if (strHiddenValue.Length == 0)
					strHiddenValue = String.Join(strSeperator, texts);
			}
			sb.Append("<input type=\"hidden\" name=\"" + strHiddenField + "\" " +
				"value=\"" + strHiddenValue + "\" />");
			sb.Append("<select size=\"" + count + "\" " +
				"name=\"" + strComboName + "\">");
			for (int i = 0; i < count; i++)
			{
				string curValue = "";
				string curText = "";
				if ((values != null) && (i < values.Length))
					curValue = values[i];
				if ((texts != null) && (i < texts.Length))
					curText = texts[i];
				sb.Append("<option value=\"" + curValue + "\">" +
					curText + "</option>");
			}
			sb.Append("</select><br />");
			string strFunctionBase = "MoveSelectedItem(this, '" + strComboName + "', %n, " +
				"'" + strHiddenField + "', '" + strSeperator + "');";
			string strTemp = strCaption + " " + ((blnMale) ? "מסומן" : "מסומנת");
			sb.Append("<button type=\"button\" " +
				"onclick=\"" + strFunctionBase.Replace("%n", "1") + "\">" +
				"הורד " + strTemp + " למטה</button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
			sb.Append("<button type=\"button\" " +
				"onclick=\"" + strFunctionBase.Replace("%n", "-1") + "\">" +
				"העלה " + strTemp + " למעלה</button>");
			return sb.ToString();
		}

		public static string BuildUpDownCombo(string strHiddenField, string strComboName,
			ArrayList values, ArrayList texts, string strSeperator, string strCaption,
			bool blnMale)
		{
			return BuildUpDownCombo(strHiddenField, strComboName,
				Sport.Common.Tools.ToStringArray(values.ToArray()),
				Sport.Common.Tools.ToStringArray(texts.ToArray()),
				strSeperator, strCaption, blnMale);
		}

		public static string BuildOption(string value, string text, bool selected)
		{
			string result = "<option value=\"" + value + "\"";
			if (selected)
				result += " selected=\"selected\"";
			result += ">" + text + "</option>";
			return result;
		}

		public static string BuildOption(string value, bool selected)
		{
			return BuildOption(value, value, selected);
		}

		public static string BuildAutoSubmitCombo(string name)
		{
			return "<select name=\"" + name + "\" onchange=\"this.form.submit();\">";
		}
		#endregion

		#region buttons
		public static string BuildSubmitButton(string strOnclickJS, string strImageName, string id)
		{
			if (strOnclickJS == null)
				strOnclickJS = "";
			if ((strImageName == null) || (strImageName.Length == 0))
				strImageName = "Images/btn_send.gif";
			string result = "<input type=\"image\" ";
			if (id.Length > 0)
				result += "id=\"" + id + "\" ";
			result += "class=\"DefaultSubmitButton\"";
			if (strOnclickJS.Length > 0)
			{
				if (strOnclickJS[strOnclickJS.Length - 1] == ';')
					strOnclickJS = strOnclickJS.Substring(0, strOnclickJS.Length - 1);
				result += " onclick=\"" + strOnclickJS + ";";
				if (strOnclickJS.IndexOf("return") < 0)
					result += " return false;";
				result += "\"";
			}
			result += " src=\"" + Common.Data.AppPath + "/" + strImageName + "\" />";
			return result;
		}

		public static string BuildSubmitButton(string strOnclickJS, string strImageName)
		{
			return BuildSubmitButton(strOnclickJS, strImageName, "");
		}

		public static string BuildSubmitButton(string strOnclickJS)
		{
			return BuildSubmitButton(strOnclickJS, "");
		}

		public static string BuildSubmitButton()
		{
			return BuildSubmitButton("");
		}
		#endregion

		#region links
		public static string BuildLink(string URL, string text, string target,
			string color, string onclick)
		{
			string result = "<a href=\"";
			result += URL + "\"";
			if ((target != null) && (target.Length > 0))
				result += " target=\"" + target + "\"";
			if ((color != null) && (color.Length > 0))
				result += " style=\"color: " + color + "\"";
			if ((onclick != null) && (onclick.Length > 0))
				result += " onclick=\"" + onclick + "\"";
			result += ">" + text + "</a>";
			return result;
		}

		public static string BuildDocumentLink(string strURL, string strText)
		{
			string result = "<a href=\"" + strURL + "\">";
			result += strText + " " + Data.LinkSuffix + "</a>";
			return result;
		}
		#endregion

		#region other elements
		public static string BuildRadioButton(string name, string value, bool selected)
		{
			string result = "<input type=\"radio\" name=\"" + name + "\" ";
			if (selected)
				result += "checked=\"checked\" ";
			result += "value=\"" + value + "\" />";
			return result;
		}

		public static string HtmlHiddenInput(string name, string value)
		{
			string result;
			result = "<input type=\"hidden\" name=\"" + name + "\" value=\"" + value + "\" />";
			return result;
		}
		#endregion

		#region general html
		private static string GetSignatureHtml(string[] captions, string[] values, int[] sizes, int gap, 
			params KeyValuePair<string, string>[] attributes)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.Append("<center>");
			sb.Append("<br /><br />");
			sb.Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\">"); // style=\"page-break-after: always;\"

			//"<u>"+MultiString("&nbsp;", 30)+"</u>"			

			sb.Append("<tr>");
			for (int i = 0; i < captions.Length; i++)
			{
				sb.Append("<td align=\"center\"><input type=\"text\" class=\"InputBottomOnly\" size=\"").Append(sizes[i]).Append("\" ");
				if (values != null && !String.IsNullOrEmpty(values[i]))
					sb.Append("value=\"").Append(values[i].Replace("\"", "&quot;")).Append("\" ");
				foreach (var attribute in attributes)
					sb.AppendFormat("{0}=\"{1}\" ", attribute.Key, attribute.Value.Replace("\"", "&quot;"));
				sb.Append("/></td>");
				if (i < (captions.Length - 1))
					sb.Append("<td align=\"center\">").Append(MultiString("&nbsp;", gap)).Append("</td>");
			}
			sb.Append("</tr>");

			sb.Append("<tr>");
			for (int i = 0; i < captions.Length; i++)
			{
				sb.Append("<td align=\"center\">").Append(captions[i]).Append("</td>");
				if (i < (captions.Length - 1))
					sb.Append("<td align=\"center\">&nbsp;</td>");
			}
			sb.Append("</tr>");

			sb.Append("</table>");
			sb.Append("</center>");

			return sb.ToString();
		}

		public static string GetSchoolSignatureHtml()
		{
			return GetSignatureHtml(new string[] { "חותמת המוסד", "חתימת מנהל בית הספר", "תאריך" },
				new string[] { null, null, DateTime.Now.ToString("dd/MM/yyyy") },
				new int[] { 30, 35, 15 }, 5);
		}

		public static string GetSchoolClubSignatureHtml(Sport.Entities.School school)
		{
			return GetSignatureHtml(new string[] { "שם מועדון הספורט הבית ספרי", "שם הרשות המקומית", "מחוז" },
				new string[] { school.Name, "", school.Region.Name }, new int[] { 35, 30, 15 }, 5,
				new KeyValuePair<string, string>("data-valuekeeper", "txtMunicipalityName"));
		}

		public static string GetClubSchoolExtraText(int regionID, Sport.Entities.School school)
		{
			Dictionary<string, string> dailyGameMapping = new Dictionary<string, string>();
			Dictionary<int, string> facilityHostMapping = new Dictionary<int, string>();
			int[] allSports = new int[] {16, 18, 17, 15, 21, 19};
			using (WebSiteServices.WebsiteService service = new WebSiteServices.WebsiteService())
			{
				var allDailyGames = service.GetSportDailyGames().ToList();
				allDailyGames.FindAll(x => x.RegionId == regionID).ForEach(dailyGameData =>
				{
					string key = dailyGameData.SportId + "_" + dailyGameData.Day;
					dailyGameMapping.Add(key, dailyGameData.Category);
				});

				var allFacilityHostDays = service.GetFacilityHostDays().ToList().FindAll(x => x.RegionId == regionID);
				allFacilityHostDays.ForEach(hostData => facilityHostMapping.Add(hostData.Index, hostData.Day));
			}
			string strFilePath = HttpContext.Current.Server.MapPath("ExtraClubText.html");
			string strExtraText = String.Join("\n", Sport.Common.Tools.ReadTextFile(strFilePath, true));
			for (int i = 1; i <= 10; i++)
				strExtraText = strExtraText.Replace("$sign_form_#" + i + ";", Tools.BuildClubSignForm(i));
			strExtraText = strExtraText.Replace("$school_name", school.Name.Replace("\"", "&quot;"));
			strExtraText = strExtraText.Replace("$region_name", school.Region.Name.Replace("\"", "&quot;"));
			strExtraText = strExtraText.Replace("$city_name", school.City == null ? "" : school.City.Name.Replace("\"", "&quot;"));
			strExtraText = strExtraText.Replace("$current_date", DateTime.Now.ToString("dd/MM/yyyy"));
			strExtraText = strExtraText.Replace("$manager_name", school.ManagerName.Replace("\"", "&quot;"));

			/*
			string curValue;
			foreach (int sportId in allSports)
			{
				for (int day = 1; day <= 5; day++)
				{
					string template = string.Format("$sport_day_{0}_{1}", sportId, day);
					string key = sportId + "_" + day;
					if (!dailyGameMapping.TryGetValue(key, out curValue))
						curValue = "&nbsp;";
					strExtraText = strExtraText.Replace(template, curValue);
				}
			}
			*/

			/*
			for (int index = 12; index >= 1; index--)
			{
				if (!facilityHostMapping.TryGetValue(index, out curValue))
					curValue = "&nbsp;";
				strExtraText = strExtraText.Replace("$facility_host_day_" + index, curValue);
			}
			*/

			return strExtraText;
		}

		public static string BuildClubSignForm(int formNumber)
		{
			string extraText = "יועבר עד לתאריך 15/07/16";
			string rawHtml = @"<div style=""float: left;""><div style=""width: 150px; text-align: center; border: solid 2px black; padding: 5px;"">טופס הרשמה מס' " + 
				formNumber + "<br />" + extraText + "</div></div><div style=\"clear: both;\"></div>";
			return rawHtml;
		}

		/// <summary>
		/// add hidden input if does not exist or update value if already exist.
		/// </summary>
		public static bool UpdateHiddenField(Control parent, string name, string value)
		{
			//create control to be added:
			LiteralControl control = FastControls.HiddenInput(name, value);

			//check if already exists in parent:
			Control tempControl = Tools.FindControl(parent, "name=\"" + name + "\"");
			if (tempControl != null)
			{
				(tempControl as LiteralControl).Text = control.Text;
				return true;
			}
			else
			{
				parent.Controls.Add(control);
				return false;
			}
		}

		public static string ToHTML(string str)
		{
			if (str == null)
				return "&nbsp;";
			string result = str;
			result = result.Replace("\n", "<br />");
			return result;
		}

		public static string BuildTableRow(List<string> cells, bool header, string[] colors, List<string> cellsWidth)
		{
			string strOpeningTag = (header) ? "<th%width%bgcolor>" : "<td%width%bgcolor>";
			string strClosingTag = (header) ? "</th>" : "</td>";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<tr>");
			for (int i = 0; i < cells.Count; i++)
			{
				string text = cells[i] + "";
				if (text.Length == 0)
					text = "&nbsp;";
				string color = "";
				if (colors != null && colors[i].Length > 0)
					color = " style=\"background-color: " + colors[i] + ";\"";
				string strWidth = "";
				if (cellsWidth != null && i < cellsWidth.Count)
					strWidth = " width=\"" + Tools.CStrDef(cellsWidth[i], "100%") + "\"";
				sb.Append(strOpeningTag.Replace("%bgcolor", color).Replace("%width", strWidth));
				sb.Append(text);
				sb.Append(strClosingTag);
			}
			sb.Append("</tr>");
			return sb.ToString();
		}

		public static string BuildTableRow(List<string> cells, bool header, List<string> cellsWidth)
		{
			return BuildTableRow(cells, header, null, cellsWidth);
		}

		public static string BuildTableRow(List<string> cells, bool header, string[] colors)
		{
			return BuildTableRow(cells, header, colors, null);
		}

		public static string BuildTableRow(List<string> cells, bool header)
		{
			return BuildTableRow(cells, header, null, null);
		}

		public static string BuildTableRow(List<string> cells)
		{
			return BuildTableRow(cells, false);
		}

		public static string BuildFilledSquare(string strColor)
		{
			string result = "<table width=\"100%\" height=\"100%\" ";
			result += "style=\"background-color: " + strColor + ";\">";
			result += "<tr><td>&nbsp;</td></tr></table>";
			return result;
		}

		public static string BuildHtmlTable(int borderSize, ArrayList data, string align)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<table");
			if (borderSize > 0)
				sb.Append(" border=\"" + borderSize + "\"");
			sb.Append(">");
			if (data != null)
			{
				foreach (object[] cells in data)
				{
					sb.Append("<tr>");
					for (int i = 0; i < cells.Length; i++)
					{
						string strCellText = Tools.CStrDef(cells[i], "&nbsp;");
						if (strCellText.Length == 0)
							strCellText = "&nbsp;";
						sb.Append("<td");
						if ((align != null) && (align.Length > 0))
							sb.Append(" align=\"" + align + "\"");
						sb.Append(">" + strCellText + "</td>");
					}
					sb.Append("</tr>");
				}
			}
			sb.Append("</table>");
			return sb.ToString();
		}

		public static string BuildHtmlTable(int borderSize, List<string> cellsData, int colCount)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<table{0}>", (borderSize > 0) ? " border=\"" + borderSize + "\"" : "");
			for (int i = 0; i < cellsData.Count; i++)
			{
				if ((i % colCount) == 0)
				{
					if (i > 0)
						sb.Append("</tr>");
					sb.Append("<tr>");
				}

				string value = cellsData[i];
				if (string.IsNullOrEmpty(value) && borderSize > 0)
					value = "&nbsp;";

				sb.AppendFormat("<td>{0}</td>", value);
			}

			if (cellsData.Count > 0)
			{
				int lastIndex = cellsData.Count;
				while ((lastIndex % colCount) != 0)
					lastIndex++;
				for (int i = cellsData.Count; i < lastIndex; i++)
					sb.Append("<td>&nbsp;</td>");
				sb.Append("</tr>");
			}

			sb.Append("</table>");
			return sb.ToString();
		}

		private static int _subCaptionsCount = 0;
		public static string BuildPageSubCaption(string text,
			System.Web.HttpServerUtility Server, string strBgColor,
			Common.ClientSide clientSide)
		{
			if (_subCaptionsCount > 999)
				_subCaptionsCount = 0;
			//string strImageName=Tools.BuildPageSubCaptionImage(text, Server);
			string strContainerID = "PageSubCaption_" + (_subCaptionsCount + 1);
			string strHTML = "<div class=\"PageSubCaption\" id=\"" + strContainerID + "\"";
			/*
			if (strBgColor == "#ffffff")
				strHTML += " style=\"margin-right: -10px;\"";
			*/
			strHTML += "></div>";
			string strNewText = text; //Tools.ReverseNumbers(text);
			clientSide.RegisterFlashMovie(strContainerID, Common.Data.AppPath + "/Flash/blue_title_v" + Data.FlashTitlesVersion + ".swf", 500, 35, "txt=" + strNewText, strBgColor);
			_subCaptionsCount++;
			return strHTML;
		}

		public static string BuildPageSubCaption(string text,
			System.Web.HttpServerUtility Server, Common.ClientSide clientSide)
		{
			return BuildPageSubCaption(text, Server, "#ffffff", clientSide);
		}

		public static string BuildConfirmDeleteHtml(string deleteWhat)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<div dir=\"rtl\">");
			sb.Append("האם אתה בטוח שברצונך למחוק " + deleteWhat + "?<br />");
			sb.Append("<input type=\"hidden\" name=\"Confirm\" />");
			sb.Append("<button onclick=\"this.form.Confirm.value='1'; ");
			sb.Append("this.form.submit();\">אישור</button>");
			sb.Append(Tools.MultiString("&nbsp;", 20));
			sb.Append("<button onclick=\"this.form.Confirm.value='0'; ");
			sb.Append("this.form.submit();\">ביטול</button>");
			sb.Append("</div>");
			return sb.ToString();
		}

		public static string BuildOptionsRange(int minValue, int maxValue, int selValue)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = minValue; i <= maxValue; i++)
			{
				sb.Append("<option value=\"" + i + "\"");
				if (i == selValue)
				{
					sb.Append(" selected=\"selected\"");
				}
				sb.Append(">" + i + "</option>");
			}
			return sb.ToString();
		}
		#endregion
		#endregion

		#region Anti XSS
		public static string GetSafeRequestValue(string key)
		{
			return Tools.PreventXSS((HttpContext.Current.Request.Form[key] + "").Trim());
		}

		public static string PreventXSS(string text)
		{
			return HttpContext.Current.Server.HtmlEncode(text);
		}
		#endregion

		#region JSON
		public static string BuildJsonString(Dictionary<string, string> values)
		{
			StringBuilder jsonString = new StringBuilder();
			jsonString.Append("{");
			if (values.Count > 0)
			{
				List<string> items = values.Keys.ToList().ConvertAll(key => string.Format("\"{0}\": {1}{2}{1}", key,
					values[key].StartsWith("[") ? "" : "\"", 
					values[key].Replace("\"", "\\\"").Replace("<", "&lt;").Replace("\r", "").Replace("\n", "")));
				jsonString.Append(string.Join(", ", items));
			}
			jsonString.Append("}");
			return jsonString.ToString();
		}

		public static string BuildJsonString(params KeyValuePair<string, string>[] pairs)
		{
			Dictionary<string, string> values = new Dictionary<string, string>();
			pairs.ToList().ForEach(p => values.Add(p.Key, p.Value));
			return BuildJsonString(values);
		}
		#endregion

		#region general methods
		#region load controls
		public static void LoadBanner(System.Web.UI.Page Page, Controls.MainView View,
			Controls.BannerType type)
		{
			View.AddContents("<div class=\"BannerPanel\">");
			Controls.Banner objBanner = (Controls.Banner)
				Page.LoadControl(Data.AppPath + "/Controls/Banner.ascx");
			objBanner.Type = type;
			View.AddContents(objBanner);
			View.AddContents("</div>");
		}
		#endregion

		#region Conversion
		public static DateTime GenerateDateTimeDef(System.Web.HttpRequest Request,
			string strPrefix, DateTime defValue)
		{
			DateTime result = defValue;
			try
			{
				result = new DateTime(Int32.Parse(Request.Form[strPrefix + "_year"]),
					Int32.Parse(Request.Form[strPrefix + "_month"]),
					Int32.Parse(Request.Form[strPrefix + "_day"]));
			}
			catch
			{
				result = defValue;
			}
			return result;
		}

		/// <summary>
		/// try to convert given object into integer, if null or fails return default.
		/// </summary>
		public static int CIntDef(object obj, int def)
		{
			if (obj == null)
				return def;
			try
			{
				return System.Convert.ToInt32(obj);
			}
			catch
			{
				return def;
			}
		}

		/// <summary>
		/// try to convert given object into long int, if null or fails return default.
		/// </summary>
		public static long CLongDef(object obj, long def)
		{
			if (obj == null)
				return def;
			try
			{
				return System.Convert.ToInt64(obj);
			}
			catch
			{
				return def;
			}
		}
		#endregion

		#region Random Numbers
		/// <summary>
		/// returns random integer in the given range.
		/// </summary>
		public static int MakeRandom(int min, int max)
		{
			System.Random Rnd = new Random();
			return Rnd.Next(min, max);
		}
		#endregion

		#region thumbnails
		public static int MakeThumbnail(string strImagePath, string strThumbnailPath,
			int thumbWidth, int thumbHeight, bool bruteResize)
		{
			Transformer.BitmapWrapper wrapper = null;
			try
			{
				wrapper = new Transformer.BitmapWrapper();
				wrapper.Load(strImagePath);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to load bitmap: " + strImagePath + ": " + ex.Message);
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
				return 1;
			}
			double scaleFactorX = ((double)thumbWidth) / ((double)wrapper.Bitmap.Width);
			double scaleFactorY = ((double)thumbHeight) / ((double)wrapper.Bitmap.Height);
			if (bruteResize)
			{
				wrapper.Scale(scaleFactorX, scaleFactorY, Transformer.BitmapWrapper.InterpolationType.LinearInterpolation);
			}
			else
			{
				if ((wrapper.Bitmap.Height * scaleFactorX) >= thumbHeight)
				{
					wrapper.Scale(scaleFactorX, scaleFactorX, Transformer.BitmapWrapper.InterpolationType.LinearInterpolation);
				}
				else
				{
					wrapper.Scale(scaleFactorY, scaleFactorY, Transformer.BitmapWrapper.InterpolationType.LinearInterpolation);
				}
			}
			wrapper.Save(strThumbnailPath);
			return 0;
		}

		public static int MakeThumbnail(string strImagePath, string strThumbnailPath,
			int thumbWidth, int thumbHeight)
		{
			return MakeThumbnail(strImagePath, strThumbnailPath, thumbWidth, thumbHeight, false);
		}

		public static string CheckAndCreateThumbnail(string strImageName,
			int width, int height, System.Web.HttpServerUtility Server, bool bruteResize)
		{
			if ((strImageName == null) || (strImageName.Length == 0))
				return "";
			string strThumbFolderPath = Server.MapPath(Common.Data.AppPath + "/" + Common.Data.ThumbnailBaseFolder);
			if (!Directory.Exists(strThumbFolderPath))
				Directory.CreateDirectory(strThumbFolderPath);
			string strSubFolderName = width.ToString() + "_" + height.ToString();
			string strSubFolderPath = Server.MapPath(Common.Data.AppPath + "/" + Common.Data.ThumbnailBaseFolder + "/" + strSubFolderName);
			if (!Directory.Exists(strSubFolderPath))
				Directory.CreateDirectory(strSubFolderPath);
			string strThumbName = Common.Data.AppPath + "/" + Common.Data.ThumbnailBaseFolder + "/" + strSubFolderName + "/" + strImageName.Replace("/", "_").Replace(":", "_").Replace("'", "_");
			string strThumbPath = Server.MapPath(strThumbName);
			if (File.Exists(strThumbPath))
				return strThumbName;
			if (MakeThumbnail(Server.MapPath(strImageName), strThumbPath, width, height, bruteResize) == 0)
			{
				return strThumbName;
			}
			else
			{
				return strImageName;
			}
		}

		public static string CheckAndCreateThumbnail(string strImageName,
			int width, int height, System.Web.HttpServerUtility Server)
		{
			return CheckAndCreateThumbnail(strImageName, width, height, Server, false);
		}

		//public static string SimpleEncode(string s)
		//{
		//	string result="";
		//
		//}

		public static string BuildThumbnailHTML(WebSiteServices.ImageData image,
			System.Web.HttpServerUtility Server, string linkURL, string description,
			string linkTarget, string linkTitle, bool blnEditMode)
		{
			if ((linkURL == null) || (linkURL.Length == 0))
				linkURL = "Gallery.aspx?action=view&id=" + image.ID;
			if ((description == null) || (description.Length == 0))
				description = image.Description;
			string result = "";
			int thumbWidth = 140;
			int thumbHeight = 100;
			string strPictureName = Common.Data.ImageGalleryFolder + "/" + image.PictureName;
			string strThumbName = CheckAndCreateThumbnail(strPictureName, thumbWidth, thumbHeight, Server);
			result += "<center>";
			result += "<div class=\"GalleryPicturePanel\">";
			result += "<a href=\"" + linkURL + "\"";
			if ((linkTarget != null) && (linkTarget.Length > 0))
				result += " target=\"" + linkTarget + "\"";
			if ((linkTitle != null) && (linkTitle.Length > 0))
				result += " title=\"" + linkTitle + "\"";
			result += ">";
			result += "<img src=\"" + strThumbName + "\" width=\"" + thumbWidth + "\" ";
			result += "alt=\"תמונת גלרייה\" ";
			result += "height=\"" + thumbHeight + "\" title=\"" + description + "\" /></a>";
			result += "</div>";
			if (blnEditMode)
			{
				result += "<input type=\"text\" name=\"description_" + image.ID + "\" " +
					"value=\"" + description.Replace("\"", "&quot;") + "\" />";
			}
			else
			{
				result += description;
			}
			result += "</center>";
			return result;
		}

		public static string BuildThumbnailHTML(WebSiteServices.ImageData image,
			System.Web.HttpServerUtility Server, bool blnEditMode)
		{
			return BuildThumbnailHTML(image, Server, null, null, "_blank",
				"לחץ לצפייה בתמונה מלאה", blnEditMode);
		}

		public static string BuildThumbnailHTML(WebSiteServices.ImageData image,
			System.Web.HttpServerUtility Server)
		{
			return BuildThumbnailHTML(image, Server, false);
		}
		#endregion

		#region dynamic images
		private enum CaptionType
		{
			Page = 0,
			Sub
		}

		private static string BuildPageCaption(string caption,
			int maxWidth, int maxHeight, System.Web.HttpServerUtility Server,
			CaptionType type)
		{
			string strBasePath = Server.MapPath(Common.Data.AppPath + "/" + Common.Data.DynamicImagesFolder);
			if (!Directory.Exists(strBasePath))
				Directory.CreateDirectory(strBasePath);
			string strSubFolderName = maxWidth.ToString() + "_" + maxHeight.ToString();
			string strSubFolderPath = Server.MapPath(Common.Data.AppPath + "/" + Common.Data.DynamicImagesFolder + "/" + strSubFolderName);
			if (!Directory.Exists(strSubFolderPath))
				Directory.CreateDirectory(strSubFolderPath);
			string strImageName = Common.Data.AppPath + "/" + Common.Data.DynamicImagesFolder + "/" + strSubFolderName + "/" + caption.Replace("'", "_").Replace("\"", "_").Replace(":", "_").Replace(" ", "_") + ".gif";
			string strImagePath = Server.MapPath(strImageName);
			if (File.Exists(strImagePath))
				return strImageName;
			System.Drawing.Image image = null;
			int fontSize = 12;
			Color bgColor = Color.Empty;
			switch (type)
			{
				case CaptionType.Page:
					fontSize = 25;
					bgColor = Color.FromArgb(204, 0, 0);
					break;
				case CaptionType.Sub:
					fontSize = 20;
					bgColor = Color.FromArgb(20, 121, 191);
					break;
			}
			while ((image == null) || (image.Height > maxHeight))
			{
				if (fontSize < 1)
				{
					break;
				}
				image = Tools.BuildTextGif(caption, Common.Data.CaptionsFontFamily, Color.White, bgColor, fontSize, true, maxWidth, maxHeight, true);
				fontSize--;
			}
			image.Save(Server.MapPath(strImageName));
			return strImageName;
		}

		public static string BuildPageCaptionImage(string caption, System.Web.HttpServerUtility Server)
		{
			return BuildPageCaption(caption, 390, 38, Server, CaptionType.Page);
		}

		public static string BuildPageSubCaptionImage(string caption, System.Web.HttpServerUtility Server)
		{
			return BuildPageCaption(caption, 500, 30, Server, CaptionType.Sub);
		}

		public static System.Drawing.Image BuildTextGif(string text, string fontFamily,
			Color textColor, Color bgColor, int fontSize, bool fontBold,
			int width, int height, bool autoSize)
		{
			System.Drawing.Bitmap result = new System.Drawing.Bitmap(width, height);
			System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result);
			System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
			if (fontBold)
				style = System.Drawing.FontStyle.Bold;
			Font font = new Font(fontFamily, fontSize, style, System.Drawing.GraphicsUnit.Pixel);
			Brush brush = new SolidBrush(textColor);
			if ((autoSize) && (text.Length > 0))
			{
				SizeF size = g.MeasureString(text, font, width);
				try
				{
					result = new System.Drawing.Bitmap((int)size.Width, (int)size.Height);
				}
				catch (Exception ex)
				{
					throw new Exception("Build Text Gif: auto size failed for text '" + text + "' (" + size.Width + "x" + size.Height + ") [" + ex.Message + "]");
				}
				g = System.Drawing.Graphics.FromImage(result);
			}
			g.FillRectangle(new System.Drawing.SolidBrush(bgColor), 0, 0, result.Width, result.Height);
			g.DrawString(text, font, brush, 0f, 0f);
			return result;
		}
		#endregion

		#region ini file
		public static int[] ReadIniOrder(string strSection, string strKey,
			int range, HttpServerUtility Server)
		{
			string strValue = Tools.ReadIniValue(strSection, strKey, Server);
			string[] arrValues = FixNumericArray(
				Sport.Common.Tools.SplitNoBlank(strValue, ','), range);
			int[] result = new int[arrValues.Length];
			for (int i = 0; i < arrValues.Length; i++)
				result[i] = Int32.Parse(arrValues[i]);
			return result;
		}

		public static string ReadIniValue(string strSection, string strKey,
			HttpServerUtility Server)
		{
			Sport.Common.IniFile ini = new Sport.Common.IniFile(Server.MapPath(Data.INI_FILE_NAME));
			string result = "";
			try
			{
				result = Tools.CStrDef(ini.ReadValue(strSection, strKey), "");
			}
			catch
			{ }
			return result;
		}

		public static void WriteIniValue(string strSection, string strKey,
			object value, HttpServerUtility Server)
		{
			Sport.Common.IniFile ini = new Sport.Common.IniFile(Server.MapPath(Data.INI_FILE_NAME));
			try
			{
				ini.WriteValue(strSection, strKey, Tools.CStrDef(value, ""));
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("error writing ini value: " + ex.Message);
			}
		}
		#endregion

		#region players register form
		private static string BuildPlayersRegisterForm(Sport.Entities.Team team,
			ArrayList players, System.Web.HttpServerUtility Server,
			string strFormAction, string strScriptName, bool showConfirm, int page,
			System.Web.SessionState.HttpSessionState Session)
		{
			//got anything?
			if ((team == null) || (players == null))
				return "";

			//declare local vaiables:
			Sport.Entities.School school = null;
			Sport.Entities.Championship championship = null;
			Sport.Entities.ChampionshipCategory category = null;
			Sport.Entities.Sport sport = null;
			Sport.Entities.Region region = null;

			//get data from the team:
			school = team.School;
			championship = team.Championship;
			category = team.Category;
			if (championship != null)
				sport = championship.Sport;
			if (school != null)
				region = school.Region;

			//got anything?
			if ((school == null) || (championship == null) || (category == null) || (sport == null) || (region == null))
				return "";

			//get base html:
			string strHTML = "";
			string strFilePath = Server.MapPath(Common.Data.AppPath + "/PlayersRegisterForm.html");
			using (StreamReader reader = new StreamReader(strFilePath, System.Text.Encoding.GetEncoding("ISO-8859-8")))
			{
				strHTML = reader.ReadToEnd();
				reader.Close();
			}
			
			//put data:
			string strBlank = Tools.MultiString("&nbsp;", 15);
			strHTML = strHTML.Replace("@championship", championship.Name);
			strHTML = strHTML.Replace("@sport", sport.Name);
			strHTML = strHTML.Replace("@grades", Tools.GetOnlyGrades(category.Name));
			strHTML = strHTML.Replace("@sex", Tools.GetOnlySex(category.Name));
			strHTML = strHTML.Replace("@season", (new Sport.Entities.Season(Sport.Entities.Season.GetOpenSeason())).Name);
			strHTML = strHTML.Replace("@region", region.Name);
			strHTML = strHTML.Replace("@symbol", school.Symbol);
			strHTML = strHTML.Replace("@address", Tools.CStrN(school.Address, strBlank));
			strHTML = strHTML.Replace("@zipcode", Tools.CStrN(school.ZipCode, strBlank));
			strHTML = strHTML.Replace("@school_phone", Tools.CStrN(school.Phone, strBlank));
			strHTML = strHTML.Replace("@school_fax", Tools.CStrN(school.Fax, strBlank));
			strHTML = strHTML.Replace("@school_email", Tools.CStrN(school.Email, strBlank));
			strHTML = strHTML.Replace("@teacher_cell_phone", strBlank);
			strHTML = strHTML.Replace("@teacher_email", strBlank);
			strHTML = strHTML.Replace("@teacher", strBlank);
			strHTML = strHTML.Replace("@coach_cell_phone", strBlank);
			strHTML = strHTML.Replace("@coach_email", strBlank);
			strHTML = strHTML.Replace("@coach", strBlank);
			Sport.Types.GradeTypeLookup lookup = new Sport.Types.GradeTypeLookup(true);
			int pagesCount = ((int)(((double)(players.Count - 1)) / ((double)20))) + 1;
			if (page < 1)
				page = 1;
			if (page > pagesCount)
				page = pagesCount;
			for (int i = 20; i >= 1; i--)
			{
				int index = (i - 1) + (20 * (page - 1));
				string strShirtNumber = "&nbsp;";
				string strFirstName = "&nbsp;";
				string strLastName = "&nbsp;";
				string strBirthday = "&nbsp;";
				string strIdNumber = "&nbsp;";
				string strGrade = "&nbsp;";
				if (index < players.Count)
				{
					if (players[index] != null)
					{
						Sport.Entities.Student student = null;
						if (players[index] is RegistrationService.PlayerData)
						{
							RegistrationService.PlayerData curPlayer = (RegistrationService.PlayerData)players[index];
							if (curPlayer.player_number > 0)
								strShirtNumber = curPlayer.player_number.ToString();
							int studentID = curPlayer.student_id;
							if (studentID >= 0)
							{
								try
								{
									student = new Sport.Entities.Student(studentID);
								}
								catch
								{ }
							}
						}
						else
						{
							Sport.Entities.Player curPlayer = (Sport.Entities.Player)players[index];
							if (curPlayer.Number > 0)
								strShirtNumber = curPlayer.Number.ToString();
							student = curPlayer.Student;
						}
						if (student != null)
						{
							strFirstName = student.FirstName;
							strLastName = student.LastName;
							strBirthday = student.BirthDate.ToString("dd/MM/yyyy");
							strIdNumber = student.IdNumber;
							strGrade = lookup.Lookup(student.Grade);
						}
					}
				}
				strHTML = strHTML.Replace("@shirt_" + i, strShirtNumber);
				strHTML = strHTML.Replace("@f_name_" + i, strFirstName);
				strHTML = strHTML.Replace("@l_name_" + i, strLastName);
				strHTML = strHTML.Replace("@birthday_" + i, strBirthday);
				strHTML = strHTML.Replace("@id_num_" + i, strIdNumber);
				strHTML = strHTML.Replace("@grade_" + i, strGrade);
			}
			strHTML = strHTML.Replace("@manager_name", school.ManagerName);
			strHTML = strHTML.Replace("@date", DateTime.Now.ToString("dd/MM/yyyy"));
			strHTML = strHTML.Replace("@form_action", strFormAction);
			strHTML = strHTML.Replace("@action", strScriptName);
			strHTML = strHTML.Replace("@team_id", team.Id.ToString());
			strHTML = strHTML.Replace("@school", school.Name);

			//pages?
			string strPagesHTML = "";
			string strConfirmStyle = "";
			string strConfirmDisabled = "";
			if (!showConfirm)
				strConfirmStyle = "display: none;";
			string strVisitedPages = Tools.CStrDef(Session["VisitedPages_" + team.Id], "");
			ArrayList arrVisitedPages = new ArrayList();
			if ((showConfirm) && (strVisitedPages.Length > 0))
			{
				arrVisitedPages.AddRange(
					Sport.Common.Tools.SplitNoBlank(strVisitedPages, ','));
			}
			if (arrVisitedPages.IndexOf(page.ToString()) < 0)
				arrVisitedPages.Add(page.ToString());
			if (pagesCount > 1)
			{
				if (showConfirm)
				{
					Session["VisitedPages_" + team.Id] =
						String.Join(",",
						(string[])arrVisitedPages.ToArray(typeof(string)));
				}
				strPagesHTML += "עמודים:";
				for (int i = 1; i <= pagesCount; i++)
				{
					strPagesHTML += "&nbsp;";
					strPagesHTML += "[";
					strPagesHTML += (i != page) ?
						"<a href=\"javascript:GoPage(" + i + ");\">" : "<b>";
					strPagesHTML += i.ToString();
					strPagesHTML += (i != page) ?
						"</a>" : "</b>";
					strPagesHTML += "]";
				}
				strPagesHTML += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
				if ((showConfirm) && (arrVisitedPages.Count < pagesCount))
					strConfirmDisabled = "disabled=\"disabled\"";
			}
			strHTML = strHTML.Replace("@pages", strPagesHTML);
			strHTML = strHTML.Replace("@btn_confirm_style", strConfirmStyle);
			strHTML = strHTML.Replace("@btn_confirm_disabled", strConfirmDisabled);

			//done.
			return strHTML;
		}

		public static string BuildPlayersRegisterForm(Sport.Entities.Team team,
			RegistrationService.PlayerData[] players, System.Web.HttpServerUtility Server,
			string strFormAction, string strScriptName, int page,
			System.Web.SessionState.HttpSessionState Session)
		{
			return BuildPlayersRegisterForm(team, new ArrayList(players), Server,
				strFormAction, strScriptName, true, page, Session);
		}

		public static string BuildPlayersRegisterForm(Sport.Entities.Team team,
			Sport.Entities.Player[] players, System.Web.HttpServerUtility Server,
			int page, System.Web.SessionState.HttpSessionState Session)
		{
			return BuildPlayersRegisterForm(team, new ArrayList(players), Server,
				"", "", false, page, Session);
		}
		#endregion

		#region Logos
		#region general
		private static string GetFirstFileInFolder(string strFolderName,
			string strFolderPath)
		{
			if (!System.IO.Directory.Exists(strFolderPath))
				return "";
			string[] arrFileNames = System.IO.Directory.GetFiles(strFolderPath);
			if ((arrFileNames == null) || (arrFileNames.Length == 0))
				return "";
			return strFolderName + "/" + System.IO.Path.GetFileName(arrFileNames[0]);
		}
		#endregion

		#region championship logo
		public static string GetChampionshipLogo(int champID, System.Web.HttpServerUtility Server)
		{
			string strFolderName = Data.AppPath + "/Images/Logos/champ_" + champID;
			string strFolderPath = Server.MapPath(strFolderName);
			return GetFirstFileInFolder(strFolderName, strFolderPath);
		}
		#endregion

		#region page logo
		public static string GetPageLogo(Common.Data.DynamicLinkData linkData,
			System.Web.HttpServerUtility Server)
		{
			string strFolderName = Data.AppPath + "/Images/Logos/page_" +
				Tools.MakeValidFileName(linkData.Text);
			return GetFirstFileInFolder(strFolderName, Server.MapPath(strFolderName));
		}
		#endregion
		#endregion

		#region article groups
		public static List<WebSiteServices.ArticleData[]> BuildArticleGroups(WebSiteServices.ArticleData[] articles)
		{
			List<WebSiteServices.ArticleData[]> result = new List<WebSiteServices.ArticleData[]>();
			List<WebSiteServices.ArticleData> curGroup = new List<WebSiteServices.ArticleData>();
			foreach (WebSiteServices.ArticleData article in articles)
			{
				DateTime curArticleTime = article.Time;
				DateTime firstArticleTime = DateTime.Now;
				if (curGroup.Count > 0)
					firstArticleTime = curGroup[0].Time;
				int daysDiff = (int)(firstArticleTime - curArticleTime).TotalDays;
				if ((daysDiff > 30) && (curGroup.Count >= 20))
				{
					result.Add(curGroup.ToArray());
					curGroup.Clear();
				}
				curGroup.Add(article);
			}

			if (curGroup.Count > 0)
				result.Add(curGroup.ToArray());
			
			return result;
		}

		public static string BuildArticlesGroupCaption(WebSiteServices.ArticleData[] group)
		{
			if (group.Length == 0)
				return "אין כתבות";
			DateTime firstArticleTime = group[0].Time;
			DateTime lastAricleTime = group[group.Length - 1].Time;
			if (((int)(DateTime.Now - firstArticleTime).TotalDays) < 30)
				return "כתבות חדשות";
			string firstArticleMonth = Controls.HebDateTime.HebMonthName(firstArticleTime.Month);
			string lastArticleMonth = Controls.HebDateTime.HebMonthName(lastAricleTime.Month);
			int firstArticleYear = firstArticleTime.Year;
			int lastArticleYear = lastAricleTime.Year;
			string result = "כתבות ";
			result += lastArticleMonth;
			if (lastArticleYear == firstArticleYear)
			{
				if (firstArticleMonth != lastArticleMonth)
					result += "-" + firstArticleMonth;
				result += " " + firstArticleYear;
			}
			else
			{
				result += " " + lastArticleYear + " עד " + firstArticleMonth + " " + firstArticleYear;
			}
			return result;
		}
		#endregion

		#region dates
		public static DateTime? ParseRawDate(string rawValue)
		{
			string[] parts = rawValue.Split('/');
			if (parts.Length == 3)
			{
				int day, month, year;
				if (Int32.TryParse(parts[0], out day) && day > 0 && day < 32 &&
					Int32.TryParse(parts[1], out month) && month > 0 && month < 13 &&
					Int32.TryParse(parts[2], out year) && year > 1900)
				{
					DateTime date = DateTime.MinValue;
					try
					{
						date = new DateTime(year, month, day);
					}
					catch
					{
						return null;
					}
					if (date.Year == year && date.Month == month && date.Day == day)
						return date;
				}
			}
			return null;
		}

		public static DateTime GetOnlyDate(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
		}

		public static string GetHebrewTimeElpased(DateTime date)
		{
			DateTime now = DateTime.Now;
			TimeSpan diff = (now - date);
			int secondsElpased = (int)Math.Abs(diff.TotalSeconds);
			if (secondsElpased < 5)
				return "ברגעים אלו";

			int minutesElpased = (int)Math.Abs(diff.TotalMinutes);
			int hoursElpased = (int)Math.Abs(diff.TotalHours);
			int daysElpased = (int)Math.Abs(diff.TotalDays);

			if (daysElpased == 1)
				return (date > now) ? "מחר" : "אתמול";

			if (daysElpased == 2)
				return (date > now) ? "מחרתיים" : "שלשום";

			int monthsElpased = -1;
			int yearsElpased = -1;
			if (daysElpased > 0 && date < now)
			{
				DateTime dtIndex = date;
				while (dtIndex < now)
				{
					monthsElpased++;
					dtIndex = dtIndex.AddMonths(1);
				}

				if (monthsElpased > 0)
				{
					dtIndex = date;
					while (dtIndex < now)
					{
						yearsElpased++;
						dtIndex = dtIndex.AddYears(1);
					}
				}
			}

			return (date > now) ? "בעוד " : "לפני " + GetHebrewTimeElpased(secondsElpased, minutesElpased, hoursElpased, daysElpased, monthsElpased, yearsElpased);
		}
		#endregion

		#region championship details
		public static string BuildDetailsTable(string strLogo, string strTitle,
			System.Web.HttpServerUtility Server, System.Web.UI.Page Page,
			int categoryID)
		{
			//initialize result:
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			//make output appear in the center:
			builder.Append("<center>");

			//make thumbnail:
			string strLogoImage =
				Tools.CheckAndCreateThumbnail(strLogo, 67, 80, Server);

			//build HTML output:
			builder.Append("<table id=\"ChampionshipDetails\" " +
				"cellpadding=\"0\" cellspacing=\"0\">");
			int[] arrCellWidths = new int[] { 5, 67, 15, 353, 67, 5 };
			builder.Append(Tools.BuildTableRow(arrCellWidths, 6));
			builder.Append("<tr>");
			builder.Append("<td>&nbsp;</td>");
			builder.Append("<td align=\"right\" valign=\"middle\">");
			builder.Append((strLogoImage.Length == 0) ? "&nbsp;" :
				"<img src=\"" + strLogoImage + "\" />");
			builder.Append("</td>");
			builder.Append("<td>&nbsp;</td>");
			builder.Append("<td align=\"center\">" +
				"<div id=\"ChampSectionName\">" + strTitle + "</div></td>");
			builder.Append("<td align=\"right\" valign=\"middle\">");
			string strAmitBB = "&nbsp;";
			if (Tools.IsCategoryLinkedAmitBB(categoryID, Server, Page))
			{
				strAmitBB = "<a href=\"http://www.amitbb.co.il/?go=games\" " +
					"target=\"_blank\" title=\"סטטיסטיקות משחקים\">";
				strAmitBB += "<img src=\"" + Tools.CheckAndCreateThumbnail(
					Data.AppPath + "/Images/amitbb_logo.jpg", 67, 80, Server) + "\" />";
				strAmitBB += "</a>";
			}
			builder.Append(strAmitBB + "</td>");
			builder.Append("<td align=\"right\">&nbsp;</td>");
			builder.Append("</tr>");
			builder.Append(Tools.BuildBlankRow(arrCellWidths.Length, 6));
			builder.Append("</table>");
			builder.Append("</center><br />");

			//done.
			return builder.ToString();
		}
		#endregion

		#region string manipulation
		public static string MakeValidHtml(string s)
		{
			if (s == null)
				return "";
			return s.Replace("\"", "&quot;");
		}

		public static string ReverseNumbers(string s)
		{
			if ((s == null) || (s.Length == 0))
				return "";
			string[] arrWords = s.Split(new char[] { ' ' });
			string result = "";
			for (int i = 0; i < arrWords.Length; i++)
			{
				string word = arrWords[i];
				if (Tools.IsInteger(word))
					word = Tools.ReverseString(word);
				result += word;
				if (i < (arrWords.Length - 1))
					result += " ";
			}
			return result;
		}

		public static string ReverseString(string s)
		{
			if ((s == null) || (s.Length == 0))
				return "";
			string result = "";
			for (int i = s.Length - 1; i >= 0; i--)
				result += s.Substring(i, 1);
			return result;
		}

		public static bool IsInteger(string s)
		{
			if ((s == null) || (s.Length == 0))
				return false;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if ((c < '0') || (c > '9'))
					return false;
			}
			return true;
		}
		#endregion

		#region general stuff
		#region IP address
		public static string GetIPAddress()
		{
			System.Web.HttpContext context = System.Web.HttpContext.Current;
			string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (!string.IsNullOrEmpty(ipAddress))
			{
				string[] addresses = ipAddress.Split(',');
				if (addresses.Length != 0)
					return addresses[0];
			}

			ipAddress = context.Request.ServerVariables["REMOTE_ADDR"] + "";
			if (ipAddress.Length == 0)
				ipAddress = context.Request.ServerVariables["REMOTE_HOST"] + "";
			
			return ipAddress;
		}
		#endregion

		#region from URL
		public static List<Sport.Entities.ChampionshipCategory> GetCategoriesFromURL(string key)
		{
			List<Sport.Entities.ChampionshipCategory> categories = new List<Sport.Entities.ChampionshipCategory>();
			string[] rawValues = (HttpContext.Current.Request.Form[key] + "").Split(',');
			int categoryID;
			Sport.Entities.ChampionshipCategory category;
			for (int i = 0; i < rawValues.Length; i++)
			{
				category = null;
				categoryID = 0;
				if (rawValues[i].Length > 0 && Int32.TryParse(rawValues[i], out categoryID) && categoryID > 0)
				{
					if (!categories.Exists(cat => cat.Id.Equals(categoryID)))
					{
						try
						{
							category = new Sport.Entities.ChampionshipCategory(categoryID);
						}
						catch
						{ }
						if (category != null && category.Id < 0)
							category = null;
					}
				}
				if (category != null)
					categories.Add(category);
			}
			return categories;
		}
		#endregion

		#region visitor action panel
		private class VisitorActionData
		{
			public VisitorAction Action { get; set; }
			public string Title { get; set; }

			private string imageName;
			public string ImageName {
				get
				{
					return imageName;
				}
				set
				{
					imageName = value;
					CalculateCellWidth();
				}
			}
			public int CellWidth { get; private set; }
			public string OnClickTemplate { get; set; }

			public string ImageUrl
			{
				get
				{
					return Common.Data.AppPath + "/Images/" + this.ImageName;
				}
			}

			public void CalculateCellWidth()
			{
				int width = 50;
				if ((this.ImageName + "").Length > 0)
				{
					string strPath = HttpContext.Current.Server.MapPath(this.ImageUrl);
					if (File.Exists(strPath))
					{
						using (Bitmap bitmap = new Bitmap(strPath))
						{
							width = bitmap.Width;
						}
					}
				}
				this.CellWidth = width;
			}

			public string GetOnClick(string elementId, string strTitle, int articleID)
			{
				if (string.IsNullOrEmpty(this.OnClickTemplate))
					return "";
				
				switch (this.Action)
				{
					case VisitorAction.SendToFriend:
						return string.Format(this.OnClickTemplate, elementId);
					case VisitorAction.Print:
						return string.Format(this.OnClickTemplate, elementId, strTitle.Replace("'", "").Replace("\"", "~"));
					case VisitorAction.AddComment:
						if (articleID > 0)
							return string.Format(this.OnClickTemplate, articleID);
						break;
				}
				return "";
			}
		}

		public enum VisitorAction
		{
			SendToFriend = 0,
			Print,
			AddComment
		}

		private static Dictionary<VisitorAction, VisitorActionData> allVisitorActions = new Dictionary<VisitorAction, VisitorActionData>();
		private static void RebuildVisitorActions()
		{
			allVisitorActions.Clear();
			allVisitorActions.Add(VisitorAction.SendToFriend, new VisitorActionData
			{
				Action = VisitorAction.SendToFriend, 
				Title = "שלח לחבר",
				ImageName = "btn_send_freind.jpg",
				OnClickTemplate = "SendToFriend('{0}');"
			});
			allVisitorActions.Add(VisitorAction.Print, new VisitorActionData
			{
				Action = VisitorAction.Print, 
				Title = "הדפס",
				ImageName = "btn_print.jpg",
				OnClickTemplate = "PrintElement('{0}', '{1}');"
			});
			allVisitorActions.Add(VisitorAction.AddComment, new VisitorActionData
			{
				Action = VisitorAction.AddComment, 
				Title = "הוסף תגובה",
				ImageName = "btn_talkback.jpg",
				OnClickTemplate = "AddComment({0});"
			});
		}

		public static void RegisterVisitorActionsJavaScript(Common.ClientSide clientSide)
		{
			clientSide.RegisterJavascript("visitor_actions", "<script type=\"text/javascript\" " +
				"src=\"" + Common.Data.AppPath + "/Common/VisitorActions.js\"></script>", true);
		}

		public static string BuildVisitorActionsPanel(string elementID, string strTitle, int articleID,
			Common.ClientSide clientSide, params VisitorAction[] actions)
		{
			if (actions.Length == 0)
				actions = new VisitorAction[] { VisitorAction.SendToFriend, VisitorAction.Print, VisitorAction.AddComment };

			if (allVisitorActions.Count == 0)
				RebuildVisitorActions();

			RegisterVisitorActionsJavaScript(clientSide);
			
			if (strTitle == null)
				strTitle = "";
			string strSeperatorCell = string.Format("<td style=\"width: 1px;\"><img src=\"{0}\" /></td>",
				Common.Data.AppPath + "/Images/small_seperator.jpg");
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("<script type=\"text/javascript\">");
			result.Append(" var _send_friend_window_title=\"שליחת כתבה לחבר\";");
			result.Append(" var _add_comment_window_title=\"הוספת תגובה לכתבה\";");
			result.Append(" var _send_friend_form_texts=new Array(\"*שם:\", " +
				"\"*אימייל שלי:\", " +
				"\"*אימייל של חבר:\", " +
				"\"טקסט נוסף:\", " +
				"\"שלחו הפנייה לכתבה\", " +
				"\"שלחו כתבה ב-html\");");
			result.Append(" var _add_comment_form_texts=new Array(\"*שם:\", " +
				"\"אימייל שלי:\", " +
				"\"*נושא:\", " +
				"\"תוכן התגובה:\");");
			result.Append(" var _caption_print=\"הדפס\";");
			result.Append(" var _caption_cancel=\"ביטול\";");
			result.Append("</script>");
			result.Append("<div align=\"right\"><br /><br />");
			result.Append("<table class=\"VisitorActionsPanel\" cellpadding=\"1\">");
			result.Append("<tr>");
			List<VisitorActionData> arrVisitorActions = actions.ToList().ConvertAll(a => allVisitorActions[a]).
				FindAll(a => a.GetOnClick(elementID, strTitle, articleID).Length > 0);
			for (int i = 0; i < arrVisitorActions.Count; i++)
			{
				var currentAction = arrVisitorActions[i];
				result.AppendFormat("<td style=\"width: {0}px;\">", currentAction.CellWidth);
				result.AppendFormat("<a href=\"javascript: void(0);\" onclick=\"{0}\">", currentAction.GetOnClick(elementID, strTitle, articleID));
				result.AppendFormat("<img src=\"{0}\" alt=\"{1}\" /></a></td>", currentAction.ImageUrl, currentAction.Title);
				if (i < (actions.Length - 1))
					result.Append(strSeperatorCell);
			}
			if (articleID >= 0)
				result.Append("<td><a name=\"fb_share\" type=\"button_count\" href=\"http://www.facebook.com/sharer.php\">Share</a><script src=\"http://static.ak.fbcdn.net/connect.php/js/FB.Share\" type=\"text/javascript\"></script></td>");
			result.Append("<td width=\"100%\">&nbsp;</td>");
			result.Append("</tr>");
			result.Append("</table></div>");
			return result.ToString();
		}

		public static string BuildVisitorActionsPanel(string elementID, string strTitle, Common.ClientSide clientSide, params VisitorAction[] actions)
		{
			return BuildVisitorActionsPanel(elementID, strTitle, -1, clientSide, actions);
		}

		public static string BuildVisitorActionsPanel(string elementID, Common.ClientSide clientSide, params VisitorAction[] actions)
		{
			return BuildVisitorActionsPanel(elementID, "", clientSide, actions);
		}
		#endregion

		#region small points
		public static int _smallPointsCounter = 0;
		public static string GetSmallPointsHTML(string strSmallPoints)
		{
			if ((strSmallPoints == null) || (strSmallPoints.Length == 0))
				return "";

			Sport.Championships.GameResult gr = null;
			try
			{
				gr = new Sport.Championships.GameResult(strSmallPoints);
			}
			catch
			{ }
			if ((gr == null) || (gr.Games == 0))
				return "";

			int sum = 0;
			for (int game = 0; game < gr.Games; game++)
			{
				for (int part = 0; part < gr[game].Count; part++)
				{
					int[] results = gr[game][part];
					for (int i = 0; i < results.Length; i++)
						sum += results[i];
				}
			}
			if (sum <= 0)
				return "";

			if (_smallPointsCounter > 1000)
				_smallPointsCounter = 0;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			string strContainerID = "small_points_" + (_smallPointsCounter + 1);
			sb.Append("&nbsp;<a href=\"javascript:void(0);\" " +
				"onclick=\"ToggleVisibility('" + strContainerID + "');\" " +
				"class=\"SmallBorderedText\">+</a>");
			sb.Append("<br />");
			sb.Append("<span id=\"" + strContainerID + "\" style=\"display: none;\">");

			ArrayList arrColumns = new ArrayList();
			for (int game = 0; game < gr.Games; game++)
			{
				arrColumns.Clear();
				for (int part = 0; part < gr[game].Count; part++)
				{
					int[] results = gr[game][part];
					int maxResult = Tools.CIntDef(FindMaxObject(results), -1);
					string[] arrCells = new string[results.Length];
					for (int i = 0; i < results.Length; i++)
					{
						int curResult = results[i];
						string strText = "";
						if (curResult == maxResult)
							strText += "<font color=\"#cc0000\">";
						strText += curResult.ToString();
						if (curResult == maxResult)
							strText += "</font>";
						arrCells[i] = strText;
					}
					if (arrCells.Length > 0)
						arrColumns.Add(arrCells);
				}
				if (arrColumns.Count > 0)
				{
					int rowCount = ((string[])arrColumns[0]).Length;
					for (int i = 0; i < rowCount; i++)
					{
						foreach (string[] cells in arrColumns)
						{
							sb.Append("<span class=\"SmallBorderedText\" " +
								"style=\"width: 20px;\">" + cells[i] + "</span>");
						}
						if (i < (rowCount - 1))
							sb.Append("<br />");
					}
				}
			}
			sb.Append("</span>");

			_smallPointsCounter++;
			return sb.ToString();
		}
		#endregion

		#region big links
		public static string BuildBigLinks(Sport.Common.ListItem[] items, ref int curTop)
		{
			StringBuilder sb = new StringBuilder();
			int[] rights = new int[] { 0, 166, 332 };
			for (int i = 0; i < items.Length; i++)
			{
				Sport.Common.ListItem item = items[i];
				string strURL = string.Empty;
				string strOnClick = string.Empty;
				if (item.Value is string[])
				{
					strURL = (item.Value as string[])[0];
					strOnClick = (item.Value as string[])[1];
				}
				else
				{
					strURL = item.Value.ToString();
				}

				int step = (i % 3);
				if (step == 0)
				{
					if (i > 0)
						sb.Append("</div>");
					sb.AppendFormat("<div class=\"big_menu_link_container\" style=\"top: {0}px;\">", curTop);
					curTop += 92;
				}
				sb.AppendFormat("<div class=\"big_menu_link_parent\" style=\"right: {0}px;\">", rights[step]);
				sb.Append("<div class=\"big_menu_link_data\">");
				if (strURL.Length > 0 || strOnClick.Length > 0)
				{
					sb.Append("<a");
					if (strURL.Length > 0)
						sb.AppendFormat(" href=\"{0}\"", strURL);
					if (strOnClick.Length > 0)
						sb.AppendFormat(" onclick=\"{0}\"", strOnClick);
					sb.Append(">");
				}
				sb.Append(item.Text);
				if (strURL.Length > 0 || strOnClick.Length > 0)
					sb.Append("</a>");
				sb.Append("</div></div>");
			}

			if (items.Length > 0)
				sb.Append("</div>");

			return sb.ToString();
		}

		public static string BuildBigLinks(Sport.Common.ListItem[] items)
		{
			int dummy = 0;
			return BuildBigLinks(items, ref dummy);
		}

		public static string BuildBigLinks(SportSite.Controls.LinkBox box, int startIndex, out int totalHeight, out string subCaption)
		{
			List<SportSite.Controls.LinkBox.Link> bigLinks = GetBigLinksBasedOnQuerystring(box, startIndex, out subCaption);
			List<Sport.Common.ListItem> items = new List<Sport.Common.ListItem>();
			int curTop = 0;
			for (int i = 0; i < bigLinks.Count; i++)
			{
				SportSite.Controls.LinkBox.Link curLink = bigLinks[i];
				string url = DecideBigLinkURL(curLink, i + ((curLink.Level == 0) ? startIndex : 0));
				items.Add(new Sport.Common.ListItem(curLink.Text, url));
			}

			string strHTML = BuildBigLinks(items.ToArray(), ref curTop);
			totalHeight = curTop;

			return strHTML;
		}
		#endregion

		#region articles range
		public static Dictionary<string, SportSite.WebSiteServices.ArticleData[]> BuildArticleRanges(List<SportSite.WebSiteServices.ArticleData> articles)
		{
			Dictionary<string, SportSite.WebSiteServices.ArticleData[]> ranges = new Dictionary<string, WebSiteServices.ArticleData[]>();
			List<SportSite.WebSiteServices.ArticleData> buffer = new List<WebSiteServices.ArticleData>();
			string prevTimeStamp = string.Empty;
			string rangeStart = string.Empty;
			articles.Reverse();
			articles.ForEach(currentArticle =>
			{
				string currentTimeStamp = currentArticle.Time.Month.ToString() + currentArticle.Time.Year.ToString();
				if (!currentTimeStamp.Equals(prevTimeStamp))
				{
					if (rangeStart.Length == 0)
						rangeStart = currentTimeStamp;
					if (buffer.Count > 20)
					{
						string key = rangeStart;
						if (!rangeStart.Equals(prevTimeStamp))
							key += "_" + prevTimeStamp;
						ranges.Add(key, buffer.ToArray());
						rangeStart = currentTimeStamp;
						buffer.Clear();
					}
				}

				prevTimeStamp = currentTimeStamp;
				buffer.Add(currentArticle);
			});

			if (buffer.Count > 0)
			{
				string key = rangeStart;
				if (!rangeStart.Equals(prevTimeStamp))
					key += "_" + prevTimeStamp;
				ranges.Add(key, buffer.ToArray());
			}

			return ranges;
		}

		public static string RangeToHebrew(string rangeKey, int count)
		{
			string[] parts = rangeKey.Split('_');
			string startKey = parts[0];
			string endKey = (parts.Length > 1) ? parts[1] : string.Empty;
			int startMonth, startYear;
			int endMonth, endYear;
			RangeKeyToMonthAndYear(startKey, out startMonth, out startYear);
			RangeKeyToMonthAndYear(endKey, out endMonth, out endYear);

			string result = SportSite.Controls.HebDateTime.HebMonthName(startMonth);
			if (endMonth > 0 && endMonth > startMonth && endYear == startYear)
				result += " - " + SportSite.Controls.HebDateTime.HebMonthName(endMonth);
			result += " " + startYear.ToString();
			if (endMonth > 0 && endYear > startYear)
				result += " - " + SportSite.Controls.HebDateTime.HebMonthName(endMonth) + " " + endYear.ToString();
			if (count > 0)
				result += " (" + Sport.Common.Tools.BuildOneOrMany("כתבה", "כתבות", count, false) + ")";
			return result;
		}

		public static string RangeToHebrew(string rangeKey)
		{
			return RangeToHebrew(rangeKey, 0);
		}

		public static void ExtractDateRange(string rangeKey, out DateTime rangeStart, out DateTime rangeEnd)
		{
			string[] parts = rangeKey.Split('_');
			string startKey = parts[0];
			string endKey = (parts.Length > 1) ? parts[1] : string.Empty;
			int startMonth, startYear;
			int endMonth, endYear;
			RangeKeyToMonthAndYear(startKey, out startMonth, out startYear);
			RangeKeyToMonthAndYear(endKey, out endMonth, out endYear);

			if (endMonth <= 0)
				endMonth = startMonth;

			if (endYear <= 0)
				endYear = startYear;

			rangeStart = new DateTime(startYear, startMonth, 1, 0, 0, 0);
			rangeEnd = (new DateTime(endYear, endMonth, 1, 23, 59, 59)).AddMonths(1).AddDays(-1);
		}
		#endregion

		#region logged user
		public static SportSite.Core.UserData GetLoggedInUser()
		{
			object oUser = HttpContext.Current.Session[SportSite.Core.UserManager.SessionKey];
			if (oUser == null)
				return SportSite.Core.UserData.Empty;

			return (SportSite.Core.UserData)oUser;
		}

		public static int GetLoggedInUserID()
		{
			SportSite.Core.UserData data = GetLoggedInUser();
			return data.Id;
		}

		public static UserType? GetLoggedInUserType()
		{
			SportSite.Core.UserData user = GetLoggedInUser();
			return (user.Id < 0) ? null : new UserType?((UserType)user.Type);
		}

		public static bool IsCurrentUserLoggedIn()
		{
			return (GetLoggedInUserID() >= 0);
		}
		#endregion

		#region web requests
		public static string ReadRemotePage(string url)
		{
			// *** Establish the request
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			// *** Retrieve request info headers
			string contents = string.Empty;
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			}
			catch
			{
				//return string.Empty;
			}

			if (response != null)
			{
				Encoding enc = Encoding.GetEncoding(1252);  // Windows default Code Page
				using (StreamReader reader = new StreamReader(response.GetResponseStream(), enc))
				{
					contents = reader.ReadToEnd();
					reader.Close();
				}
				response.Close();
			}

			return contents;
		}

		public static Stream GetRemotePage(string url, out string error)
		{
			error = string.Empty;

			WebRequest request = WebRequest.Create(url);
			WebResponse response = null;
			try
			{
				response = request.GetResponse();
			}
			catch (Exception ex)
			{
				error = "Failed to get response from remote URL: " + ex.Message;
			}

			if (response != null)
				return response.GetResponseStream();

			return null;
		}

		public static void SaveRemoteUrl(string url, string folderPath, out string error)
		{
			Stream remoteStream = GetRemotePage(url, out error);
			if (remoteStream != null)
			{
				string filePath = folderPath;
				if (!filePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
					filePath += Path.DirectorySeparatorChar.ToString();
				filePath += ExtractFileNameFromUrl(url);

				// Create the local file
				using (FileStream localStream = File.Create(filePath))
				{
					// Allocate a 1k buffer
					byte[] buffer = new byte[1024];
					int bytesRead;

					// Simple do/while loop to read from stream until
					// no bytes are returned
					do
					{
						// Read data (up to 1k) from the stream
						bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

						// Write the data to the local file
						localStream.Write(buffer, 0, bytesRead);
					} while (bytesRead > 0);

					localStream.Close();
				}

				remoteStream.Dispose();
			}
		}

		public static string ExtractFileNameFromUrl(string url)
		{
			string retVal = url;
			int index = retVal.IndexOf("?");
			if (index > 0)
				retVal = retVal.Substring(0, index);
			string[] temp = retVal.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			retVal = temp[temp.Length - 1];
			return retVal;
		}
		#endregion

		#region JavaScript helpers
		public static void RegisterJavaScriptVariables(string key, params KeyValuePair<string, string>[] variables)
		{
			if (variables.Length > 0)
			{
				Page page = HttpContext.Current.CurrentHandler as Page;
				page.ClientScript.RegisterClientScriptBlock(page.GetType(), key, string.Join(" ", variables.ToList().ConvertAll(pair =>
				{
					return string.Format("var {0} = \"{1}\";", pair.Key, pair.Value.Replace("\"", "\\\""));
				})) + "; ", true);
			}
		}
		#endregion

		#region other stuff
		public static bool EqualsIgnoreNewLines(string s1, string s2)
		{
			s1 = HttpContext.Current.Server.HtmlDecode(s1);
			s2 = HttpContext.Current.Server.HtmlDecode(s2);
			s1 = s1.Replace("\n", "").Replace("\r", "").Replace("<br />", "");
			s2 = s2.Replace("\n", "").Replace("\r", "").Replace("<br />", "");
			return s1.Equals(s2, StringComparison.CurrentCultureIgnoreCase);
		}

		public static Dictionary<string, string> GetRequestMapping(string prefix)
		{
			Dictionary<string, string> result = new Dictionary<string, string>();
			foreach (string key in HttpContext.Current.Request.Form.Keys)
			{
				if (key.StartsWith(prefix) && key.Length > prefix.Length)
				{
					string mapKey = key.Substring(prefix.Length);
					if (!result.ContainsKey(mapKey))
						result.Add(mapKey, HttpContext.Current.Request.Form[key]);
				}
			}
			return result;
		}

		public static List<Dictionary<string, string>> GetRequestCollection(string keyName, params string[] valueNames)
		{
			var context = HttpContext.Current;
			List<string> matchingKeys = context.Request.Form.Keys.OfType<string>().ToList().FindAll(k => k.StartsWith(keyName + "_"));
			matchingKeys.Sort((k1, k2) =>
			{
				int n1 = Int32.Parse(k1.Substring(k1.IndexOf("_") + 1));
				int n2 = Int32.Parse(k2.Substring(k2.IndexOf("_") + 1));
				return n1.CompareTo(n2);
			});
			List<string> valueNamesList = valueNames.ToList();
			return matchingKeys.ConvertAll(formKey =>
			{
				string keyIndex = formKey.Substring(formKey.IndexOf("_"));
				Dictionary<string, string> curItem = new Dictionary<string,string>();
				curItem.Add(keyName, context.Request.Form[formKey] + "");
				valueNamesList.ForEach(valueName =>
				{
					string value = context.Request.Form[valueName + keyIndex] + "";
					curItem.Add(valueName, value);
				});
				return curItem;
			});
		}

		public static string MapAction(SportSiteAction action)
		{
			return string.Format("{0}?action={1}", HttpContext.Current.Request.FilePath, action.ToString());
		}

		public static string OutputHiddenValues(string[] arrFormElements)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (string strFormElement in arrFormElements)
			{
				string strValue = System.Web.HttpContext.Current.Request.Form[strFormElement];
				sb.Append(FastControls.HiddenInputHtml(strFormElement, strValue));
			}
			return sb.ToString();
		}

		public static string GetFullURL(System.Web.HttpRequest Request)
		{
			string[] arrTemp =
				Request.ServerVariables["SERVER_PROTOCOL"].Split(new char[] { '/' });
			string result = arrTemp[0];
			result += "://";
			result += Request.ServerVariables["Server_Name"];
			result += Common.Data.AppPath;
			return result;
		}

		public static string PutHTML_NewLines(string s)
		{
			s = s.Replace("\r\n", "\n");
			s = s.Replace("\n\r", "\n");
			return s.Replace("\n", "<br />");
		}

		public static object FindMaxObject(int[] arr)
		{
			if ((arr == null) || (arr.Length == 0))
				return null;
			object max = arr[0];
			for (int i = 1; i < arr.Length; i++)
			{
				if (arr[i] > ((int)max))
					max = arr[i];
			}
			return max;
		}

		public static ArrayList ReArrangeArray(ArrayList arr, int[] indices)
		{
			if (arr == null)
				return null;

			ArrayList result = new ArrayList();
			foreach (int index in indices)
			{
				if ((index >= 0) && (index < arr.Count))
				{
					object o = arr[index];
					if (result.IndexOf(o) < 0)
						result.Add(o);
				}
			}

			foreach (object o in arr)
			{
				if (result.IndexOf(o) < 0)
					result.Add(o);
			}

			return result;
		}

		public static string[] FixNumericArray(string[] arrNumbers, int range)
		{
			ArrayList result = new ArrayList(arrNumbers);
			ArrayList arrToRemove = new ArrayList();
			foreach (string s in arrNumbers)
			{
				int num = Tools.CIntDef(s, -1);
				if ((num < 0) || (num >= range))
					arrToRemove.Add(s);
			}
			foreach (string s in arrToRemove)
				result.Remove(s);
			while (result.Count < range)
			{
				int num = 0;
				while (result.IndexOf(num.ToString()) >= 0)
				{
					num++;
				}
				result.Add(num.ToString());
			}
			return (string[])result.ToArray(typeof(string));
		}

		public static long CurrentMilliSeconds()
		{
			TimeSpan ts;
			ts = new TimeSpan(DateTime.Now.Ticks);
			return (long)ts.TotalMilliseconds;
		}

		public static void SortGalleryGroups(ArrayList data,
			System.Web.HttpServerUtility Server)
		{
			string strOrder = Common.Data.GetGalleryGroupsOrder(Server);
			if ((strOrder == null) || (strOrder.Length == 0))
				return;
			string[] arrIndices = strOrder.Split(new char[] { '|' });
			ArrayList arrTemp = new ArrayList();
			for (int i = 0; i < arrIndices.Length; i++)
			{
				int curIndex = Tools.CIntDef(arrIndices[i], -1);
				if ((curIndex >= 0) && (curIndex < data.Count))
				{
					object objToAdd = data[curIndex];
					if (arrTemp.IndexOf(objToAdd) < 0)
						arrTemp.Add(objToAdd);
				}
			}
			ArrayList arrOriginalData = new ArrayList((string[])data.ToArray(typeof(string)));
			data.Clear();
			foreach (object o in arrTemp)
				data.Add(o);
			if (data.Count < arrOriginalData.Count)
			{
				foreach (object obj in arrOriginalData)
				{
					if (data.IndexOf(obj) < 0)
						data.Add(obj);
				}
			}
		}

		public static string MakeValidFileName(string str)
		{
			string result = str.ToLower();
			result = result.Replace("https://", "");
			result = result.Replace("http://", "");
			result = result.Replace("/", "_");
			result = result.Replace("\\", "_");
			result = result.Replace("'", "_");
			result = result.Replace("\"", "_");
			result = result.Replace(":", "_");
			result = result.Replace("&", "_");
			result = result.Replace("?", "_");
			result = result.Replace(".", "_");
			return result;
		}

		public static bool InRange(int num, int min, int max)
		{
			return ((num >= min) && (num <= max));
		}

		/// <summary>
		/// add given child to the parent in given position
		/// </summary>
		public static void PutInPosition(System.Web.UI.Control parent,
			System.Web.UI.Control child, HorizontalAlign position)
		{
			Panel objPanel = new Panel();
			objPanel.HorizontalAlign = position;
			objPanel.Controls.Add(child);
			parent.Controls.Add(objPanel);
		}

		public static string FormatIsfDate(DateTime date)
		{
			if (date.Year < 1900)
				return "-";
			return date.ToString("dd/MM/yyyy");
		}

		/// <summary>
		/// add given page to the general hit log table.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="Request"></param>
		public static void AddHitLog(WebSiteServices.WebSitePage page,
			System.Web.HttpRequest Request)
		{
			//System.Web.HttpContext.Current.Application["test"] = "hello from add to hit log";

			string strUserAgent = Request.ServerVariables["HTTP_USER_AGENT"] + "";

			//ignore bots..
			if (strUserAgent.IndexOf("bot", StringComparison.CurrentCultureIgnoreCase) >= 0)
				return;

			string strServerName = Request.ServerVariables["SERVER_NAME"] + "";
			if (strServerName.Equals("localhost", StringComparison.CurrentCultureIgnoreCase) || strServerName.Equals("127.0.0.1"))
				return;

			//initialize service:
			WebSiteServices.WebsiteService websiteService =
				new WebSiteServices.WebsiteService();

			string strAppKey = "LastHitLogPurge";
			DateTime dtLastPurge = DateTime.MinValue;
			if (System.Web.HttpContext.Current.Application[strAppKey] != null)
				dtLastPurge = (DateTime)System.Web.HttpContext.Current.Application[strAppKey];
			//System.Web.HttpContext.Current.Response.Write("now: " + DateTime.Now + "<br />");
			//System.Web.HttpContext.Current.Response.Write("last purge: " + dtLastPurge + "<br />");
			if ((DateTime.Now - dtLastPurge).TotalDays >= 1)
			{
				string strMessage = websiteService.PurgeOldHitLogData();
				//System.Web.HttpContext.Current.Response.Write("result: "+strMessage+"<br />");
				System.Web.HttpContext.Current.Application[strAppKey] = DateTime.Now;
			}

			//get user information:
			string strIP = Request.ServerVariables["REMOTE_HOST"];
			string strQueryString = CollectionToString(Request.QueryString, "&");
			string strForm = CollectionToString(Request.Form, "&",
				new string[] { "__VIEWSTATE" });
			if (strForm != null)
			{
				strForm = strForm.Replace("IsfMainView:", "");
				if (strForm.Length > 255)
					strForm = strForm.Substring(0, 252) + "...";
			}
			if (strQueryString != null)
			{
				if (strQueryString.Length > 255)
					strQueryString = strQueryString.Substring(0, 252) + "...";
			}

			string strReferer = "";
			if (Request.UrlReferrer != null)
				strReferer = Request.UrlReferrer.ToString();

			//try to add hit log record:
			try
			{
				websiteService.Page_Hit(page, strIP, strQueryString,
					strForm, strUserAgent, strReferer);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to add hit log record: " + ex.Message);
				Sport.Core.LogFiles.AppendToLogFile(
					System.Web.HttpContext.Current.Server.MapPath("HitLogErrorsLog.txt"),
					ex.Message);
			}
		}

		public static string MakeHtmlBold(string text)
		{
			return "<span style=\"font-weight: bold;\">" + text + "</span>";
		}

		public static SportSiteAction StrToAction(string str)
		{
			SportSiteAction result = SportSiteAction.Undefined;
			if ((str == null) || (str.Length == 0))
				return result;
			//string[] arrActions=Enum.GetNames(typeof(SportSiteAction));
			try
			{
				result = (SportSiteAction)Enum.Parse(typeof(SportSiteAction), str, true);
			}
			catch
			{
				result = SportSiteAction.Undefined;
			}
			return result;
		}

		public static NavBarLink StrToNavBarLink(string str)
		{
			NavBarLink result = NavBarLink.MainPage; //default!

			try
			{
				result = (NavBarLink)Enum.Parse(typeof(NavBarLink), str, true);
			}
			catch
			{
				result = NavBarLink.MainPage;
			}
			return result;
		}

		public static int GetLatestSeason()
		{
			return Data.GetCurrentSeason();
		}

		public static Sport.Data.EntityFilterField ChampSeasonField()
		{
			return Sport.Entities.Championship.SeasonFilter(GetLatestSeason());
		}

		public static string FixHebrewPhone(string phone)
		{
			string result = "";
			string[] arrParts = phone.Split(new char[] { '-' });
			if (arrParts.Length > 1)
				result += arrParts[1] + "-";
			result += arrParts[0];
			return result;
		}

		public static string BuildCaption(string caption)
		{
			return "<div class=\"OrangeCaption\">" + caption + "</div>";
		}

		public static string GetOnlyGrades(string strCategory)
		{
			return Sport.Common.Tools.GetOnlyGrades(strCategory);
		}

		public static string GetOnlySex(string strCategory)
		{
			return Sport.Common.Tools.GetOnlySex(strCategory);
		}

		public static T GetElementOrDefault<T>(T[] array, int index, T defValue)
		{
			if (array == null || index < 0 || index >= array.Length)
				return defValue;
			return array[index];
		}
		#endregion

		public static string[] GetFiles(string path, params string[] filters)
		{
			List<string> files = new List<string>();
			foreach (string filter in filters)
			{
				files.AddRange(Directory.GetFiles(path, filter));
			}
			return files.ToArray();
		}

		private static Dictionary<string, int> ballMapping = new Dictionary<string, int>();
		public static string AddBall()
		{
			string key = Guid.NewGuid().ToString("N");
			while (ballMapping.ContainsKey(key))
				key = Guid.NewGuid().ToString("N");
			int value = Tools.MakeRandom(1, 10);
			ballMapping.Add(key, value);
			return key;
		}

		public static int GetBallValue(string key)
		{
			if (key.Length == 0)
				return 0;
			return ballMapping.ContainsKey(key) ? ballMapping[key] : 0;
		}
		#endregion
		#endregion

		#region hebrew related methods
		/// <summary>
		/// returns amount of hebrew letters in the given string.
		/// </summary>
		public static int HebLettersCount(string str)
		{
			int result = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if ((str[i] >= 'א') && (str[i] <= 'ת'))
					result++;
			}
			return result;
		}

		public static string HebrewCount(int number, bool isMale)
		{
			return Sport.Common.Tools.HebrewCount(number, isMale);
		}
		#endregion

		#region web controls methods
		/// <summary>
		/// finds the parent of given child, which is direct son of ancestor.
		/// </summary>
		public static System.Web.UI.Control FindFirstParent(System.Web.UI.Control ancestor,
			System.Web.UI.Control child)
		{
			for (int i = 0; i < ancestor.Controls.Count; i++)
			{
				if (FindControlDeep(ancestor.Controls[i], child))
					return ancestor.Controls[i];
			}
			return null;
		}

		/// <summary>
		/// deep search for child control in ancestor.
		/// </summary>
		public static bool FindControlDeep(System.Web.UI.Control ancestor,
			System.Web.UI.Control child)
		{
			if (ancestor.Equals(child))
				return true;

			for (int i = 0; i < ancestor.Controls.Count; i++)
			{
				if (FindControlDeep(ancestor.Controls[i], child))
					return true;
			}

			return false;
		}

		public static Control FindControlByType(Control control, string strPartialType)
		{
			if (control.GetType().Name.ToLower().IndexOf(strPartialType.ToLower()) >= 0)
				return control;
			foreach (Control child in control.Controls)
			{
				if (child.GetType().Name.ToLower().IndexOf(strPartialType.ToLower()) >= 0)
					return child;
			}
			Control result = null;
			if (control.Parent != null)
				result = FindControlByType(control.Parent, strPartialType);
			return result;
		}

		/// <summary>
		/// search for control with given partial text inside parent control. recursive.
		/// </summary>
		public static Control FindControl(Control parent, string strPartialText)
		{
			//stop condition: parent is null.
			if (parent == null)
				return null;

			//check all children:
			for (int i = 0; i < parent.Controls.Count; i++)
			{
				Control result = FindControl(parent.Controls[i], strPartialText);
				if (result != null)
					return result;
			}

			//check current control. can check only certain controls.
			if (parent is Label)
				return ((parent as Label).Text.ToLower().IndexOf(strPartialText.ToLower()) >= 0) ? parent : null;
			if (parent is TableCell)
				return ((parent as TableCell).Text.ToLower().IndexOf(strPartialText.ToLower()) >= 0) ? parent : null;
			if (parent is LiteralControl)
				return ((parent as LiteralControl).Text.ToLower().IndexOf(strPartialText.ToLower()) >= 0) ? parent : null;

			return null;
		}

		/// <summary>
		/// search for control with given partial text inside parent control. recursive.
		/// </summary>
		public static Control FindControlByID(Control parent, string ID)
		{
			//stop condition: parent is null.
			if (parent == null)
				return null;

			//check all children:
			for (int i = 0; i < parent.Controls.Count; i++)
			{
				Control result = FindControlByID(parent.Controls[i], ID);
				if (result != null)
					return result;
			}

			//check current control.
			if (parent.ID == ID)
				return parent;

			return null;
		}
		#endregion

		#region file related methods
		public static string GetFlashLink(string strFilePath)
		{
			if (!File.Exists(strFilePath))
				return "";
			string[] arrLines = Sport.Common.Tools.ReadTextFile(strFilePath, false);
			string strToSearch = "link=";
			foreach (string curLine in arrLines)
			{
				if (curLine.ToLower().StartsWith(strToSearch))
					return curLine.Substring(strToSearch.Length);
			}
			return "";
		}

		private static bool IsValidImage(Stream stream, string strFilePath)
		{
			System.Drawing.Image image = null;
			try
			{
				if (stream != null)
				{
					image = System.Drawing.Image.FromStream(stream);
				}
				else
				{
					if (strFilePath != null)
						image = System.Drawing.Image.FromFile(strFilePath);
				}
			}
			catch
			{ }

			bool valid = (image != null && image.Width > 0);
			if (image != null)
				image.Dispose();

			return valid;
		}

		public static bool IsValidImage(Stream stream)
		{
			return IsValidImage(stream, null);
		}

		public static bool IsValidImage(string strFilePath)
		{
			return IsValidImage(null, strFilePath);
		}

		public static string DiagnoseTextFile(string strFilePath, string strLineSeperator)
		{
			StreamReader objReader = new StreamReader(strFilePath);
			string strAllData = objReader.ReadToEnd();
			objReader.Close();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < strAllData.Length; i++)
			{
				char c = strAllData[i];
				sb.Append(((int)c).ToString());
				sb.Append(" - ");
				sb.Append(c.ToString());
				if (i < (strAllData.Length - 1))
					sb.Append(strLineSeperator);
			}
			return sb.ToString();
		}

		public static bool IsImageFile(string fileName)
		{
			//extension:
			string extension = GetFileExtension(fileName).ToLower();
			string[] arrValid = new string[] { "jpg", "gif", "bmp", "jpeg", "png" };
			bool result = (Sport.Common.Tools.InArray(arrValid, extension) >= 0);
			return result;
		}

		public static string GetFileExtension(string fileName)
		{
			int index = fileName.LastIndexOf(".");
			if ((index < 0) || (index == fileName.Length - 1))
			{
				return "";
			}
			return fileName.Substring(index + 1);
		}

		/// <summary>
		/// add given string to the file name in given path.
		/// NOTE:
		/// returns only file name!
		/// </summary>
		public static string AddToFilename(string strPath, string strToAdd)
		{
			string result = System.IO.Path.GetFileNameWithoutExtension(strPath);
			result += strToAdd + System.IO.Path.GetExtension(strPath);
			return result;
		}

		/// <summary>
		/// return the lines of the given file.
		/// </summary>
		public static void CreateTextFile(string strFullPath, string strText, bool hebrew)
		{
			StreamWriter writer = null;
			if (hebrew)
				writer = new StreamWriter(strFullPath, false, System.Text.Encoding.GetEncoding("ISO-8859-8"));
			else
				writer = new StreamWriter(strFullPath, false);
			if (strText != null)
				writer.Write(strText);
			writer.Close();
		}
		#endregion

		#region articles and news related methods
		/// <summary>
		/// if any file specified, return its contents.
		/// otherwise, returns the direct contents.
		/// note, the limit of the direct contents is 1024 characters.
		/// </summary>
		public static string GetArticleContents(WebSiteServices.ArticleData article,
			System.Web.HttpServerUtility Server)
		{
			string strFileName = Tools.CStrDef(article.File, "");
			string strFolderName =
				Common.Data.AppPath + "/" + Common.Data.ArticlesFolderName;
			string strFolderPath = Server.MapPath(strFolderName);
			string result = null;

			//create articles folder if needed:
			if (!System.IO.Directory.Exists(strFolderPath))
				System.IO.Directory.CreateDirectory(strFolderPath);

			if (strFileName.Length > 0)
			{
				//read from file:
				string filePath = Server.MapPath(strFolderName + "/" + strFileName);
				try
				{
					System.IO.StreamReader objReader =
						new System.IO.StreamReader(filePath);
					result = objReader.ReadToEnd();
					objReader.Close();
				}
				catch (Exception ex)
				{
					throw new Exception("error while reading " + filePath + ": " +
						ex.Message);
				}
			}
			else
			{
				result = article.Contents;
			}
			return result;
		}

		public static bool IsAuthorized_Pages(int userid)
		{
			return IsUserAuthorized(userid, "SportSite.Pages");
		}

		public static bool IsAuthorized_Articles(int userid)
		{
			return IsUserAuthorized(userid, "SportSite.Articles");
		}

		public static bool IsAuthorized_News(int userid)
		{
			return IsUserAuthorized(userid, "SportSite.FlashNews");
		}

		public static bool IsAuthorized_Polls(int userid)
		{
			return IsUserAuthorized(userid, "SportSite.Polls");
		}

		public static bool IsAuthorized_PermanentChamps(int userid)
		{
			return IsUserAuthorized(userid, "SportSite.PermanentChamps");
		}

		private static int _flashObjectsCount = 0;
		private static System.Web.UI.Page _page = null;
		public static System.Web.UI.Page Page
		{
			get { return _page; }
			set { _page = value; }
		}

		/// <summary>
		/// returns image tag with smart resize, if the image do exists.
		/// </summary>
		public static string BuildArticleImage(string strImageName, string defVal,
			int width, int height, HttpServerUtility Server, string imageID,
			string strAlign, string strImageDescription, Common.ClientSide clientSide, string bgColor)
		{
			if (_flashObjectsCount > 1000)
				_flashObjectsCount = 0;
			_flashObjectsCount++;
			string result = null;
			if ((strImageName != null) && (strImageName.Length > 0))
			{
				int imageWidth = Common.Data.ArticleBigImage.Width;
				int imageHeight = Common.Data.ArticleBigImage.Height;
				strImageName = Data.AppPath + "/" + Data.ArticlesImagesFolder + "/" + strImageName;
				string strOriginalImage = strImageName;
				strImageName = Tools.CheckAndCreateThumbnail(strImageName, imageWidth, imageHeight, Server);
				string strMovie = Common.Data.AppPath + "/Flash/bigPicNew.swf";
				string strContainerID = "ArticleImageFlash_" + _flashObjectsCount;
				result = "<div class=\"ArticlePicFlash\">";
				result += "<div id=\"" + strContainerID + "\"></div>";
				if ((strImageDescription != null) && (strImageDescription.Length > 0))
					result += Tools.MultiString("&nbsp;", 5) + strImageDescription;
				result += "<div class=\"ImageMagnifier\">" +
					"<a href=\"" + strOriginalImage + "\" target=\"_blank\" title=\"לחץ לצפייה בתמונה מלאה\">" +
					"<img src=\"" + Data.AppPath + "/Images/magnifier.gif\"></a></div>";
				result += "</div>";
				if (string.IsNullOrEmpty(bgColor))
					clientSide.RegisterFlashMovie(strContainerID, strMovie, imageWidth, imageHeight, "picUrl=" + strImageName);
				else
					clientSide.RegisterFlashMovie(strContainerID, strMovie, imageWidth, imageHeight, "picUrl=" + strImageName, bgColor);
			}
			else
			{
				result = defVal;
			}
			return result;
		}

		public static string BuildArticleImage(string strImageName, string defVal,
			int width, int height, HttpServerUtility Server, string imageID,
			string strAlign, Common.ClientSide clientSide, string bgColor)
		{
			return BuildArticleImage(strImageName, defVal, width, height, Server, imageID, strAlign, "", clientSide, bgColor);
		}

		public static string BuildArticleImage(string strImageName, string defVal,
			int width, int height, HttpServerUtility Server, string imageID,
			Common.ClientSide clientSide, string bgColor)
		{
			return BuildArticleImage(strImageName, defVal, width, height, Server, imageID, "", clientSide, bgColor);
		}

		public static string BuildArticleImage(string strImageName, string defVal,
			int width, int height, HttpServerUtility Server, Common.ClientSide clientSide, string bgColor)
		{
			return BuildArticleImage(strImageName, defVal, width, height, Server, null, clientSide, bgColor);
		}

		private static string GetArticleImage(int articleID, HttpPostedFile imageFile, string existingImagePath)
		{
			string fileName = (imageFile == null) ? Path.GetFileName(existingImagePath) : imageFile.FileName;
			var server = HttpContext.Current.Server;
			//build proper file name for the new image: (prevent cache)
			int randNum = (new Random()).Next(1, 999999);
			string strExtension = Tools.GetFileExtension(fileName).ToLower();
			string strImageBaseName = string.Format("article_{0}_$rand.{1}", articleID, strExtension);
			string strImageName = strImageBaseName.Replace("$rand", randNum.ToString());
			string strImagePath = server.MapPath(string.Format("{0}/{1}/{2}", Data.AppPath, Data.ArticlesImagesFolder, strImageName));
			while (File.Exists(strImagePath))
			{
				randNum++;
				strImageName = strImageBaseName.Replace("$rand", randNum.ToString());
				strImagePath = server.MapPath(string.Format("{0}/{1}/{2}", Data.AppPath, Data.ArticlesImagesFolder, strImageName));
			}

			//save or copy:
			if (imageFile != null)
			{
				//save
				imageFile.SaveAs(strImagePath);
			}
			else
			{
				//copy
				File.Copy(existingImagePath, strImagePath, true);
			}

			//make virtual:
			string strFullVirtual = Data.AppPath + "/" + Data.ArticlesImagesFolder + "/" + strImageName;

			//build thumbnails:
			string strSmallThumb = Tools.CheckAndCreateThumbnail(strFullVirtual, Common.Data.ArticleSmallImage.Width, Common.Data.ArticleSmallImage.Height, server);
			string strBigThumb = Tools.CheckAndCreateThumbnail(strFullVirtual, Common.Data.ArticleBigImage.Width, Common.Data.ArticleBigImage.Height, server);

			//done.
			return strImageName;
		}

		public static string GetArticleImage(int articleID, HttpPostedFile imageFile)
		{
			return GetArticleImage(articleID, imageFile, null);
		}

		public static string GetArticleImage(int articleID, string existingImagePath)
		{
			return GetArticleImage(articleID, null, existingImagePath);
		}

		public static string GetArticleImageFromRequest(Page page, string requestKey, int articleID)
		{
			HttpPostedFile oImageFile = page.Request.Files[requestKey];
			if (oImageFile == null || oImageFile.ContentLength == 0)
				return string.Empty;

			//maybe not really an image?
			if (!Tools.IsImageFile(oImageFile.FileName))
			{
				string strHtml = "<script type=\"text/javascript\">";
				strHtml += string.Format("alert(\"The file '{0}' is not a valid image\");", oImageFile.FileName);
				strHtml += "</script>";
				page.ClientScript.RegisterClientScriptBlock(page.GetType(), "InvalidFormatAlert", strHtml, false);
				return string.Empty;
			}

			return GetArticleImage(articleID, oImageFile);
		}

		public static string MapVirtualUrl(string url)
		{
			string retVal = HttpContext.Current.Request.ServerVariables["SERVER_PROTOCOL"];
			if (string.IsNullOrEmpty(retVal))
				retVal = "http";
			int index = retVal.IndexOf("/");
			if (index > 0)
				retVal = retVal.Substring(0, index);
			retVal += "://";
			retVal += HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

			string port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
			if (!string.IsNullOrEmpty(port) && port != "80")
				retVal += ":" + port;

			if (!string.IsNullOrEmpty(url))
			{
				if (!url.StartsWith("/"))
					retVal += "/";
				retVal += url;
			}

			return retVal;
		}
		#endregion

		#region cache related methods
		public static bool IsCategoryLinkedAmitBB(int category,
			System.Web.HttpServerUtility Server, System.Web.UI.Page Page)
		{
			if (category < 0)
				return false;
			if (Page.Cache["AmitBB_Categories"] == null)
				RebuildAmitBB_Categories(Server, Page);
			int[] curCategories = (int[])Page.Cache["AmitBB_Categories"];
			if (curCategories != null)
			{
				ArrayList arrCategories = new ArrayList(curCategories);
				return (arrCategories.IndexOf(category) >= 0);
			}
			return false;
		}

		public static void SetLinkAmitBB(int category, bool exists,
			System.Web.HttpServerUtility Server, System.Web.UI.Page Page)
		{
			int[] curCategories = (int[])Page.Cache["AmitBB_Categories"];
			if ((curCategories == null) || (curCategories.Length == 0))
			{
				RebuildAmitBB_Categories(Server, Page);
				curCategories = (int[])Page.Cache["AmitBB_Categories"];
			}
			ArrayList arrCategories = new ArrayList(curCategories);
			if (exists)
			{
				if (arrCategories.IndexOf(category) < 0)
					arrCategories.Add(category);
			}
			else
			{
				if (arrCategories.IndexOf(category) >= 0)
					arrCategories.Remove(category);
			}
			Tools.WriteIniValue("AmitBB", "categories",
				String.Join(",", Sport.Common.Tools.ToStringArray(
				(int[])arrCategories.ToArray(typeof(int)))), Server);
			RebuildAmitBB_Categories(Server, Page);
		}

		public static void RebuildAmitBB_Categories(System.Web.HttpServerUtility Server,
			System.Web.UI.Page Page)
		{
			string strCategories = Tools.ReadIniValue("AmitBB", "categories", Server);
			strCategories = Tools.CStrDef(strCategories, "");
			string[] arrCategories = Sport.Common.Tools.SplitNoBlank(strCategories, ',');
			ArrayList categories = new ArrayList();
			foreach (string strCurCategory in arrCategories)
			{
				if ((strCurCategory == null) || (strCurCategory.Length == 0))
					continue;
				int catID = Tools.CIntDef(strCurCategory, -1);
				if (catID >= 0)
				{
					Sport.Entities.ChampionshipCategory category = null;
					try
					{
						category = new Sport.Entities.ChampionshipCategory(catID);
					}
					catch
					{
						category = null;
					}
					if ((category != null) && (category.Id >= 0) &&
						(categories.IndexOf(catID) < 0))
					{
						categories.Add(catID);
					}
				}
			}
			Page.Cache["AmitBB_Categories"] = (int[])categories.ToArray(typeof(int));
		}
		#endregion

		#region match data
		public static string MatchToString(WebSiteServices.MatchData match)
		{
			string[] arrItems = Sport.Common.Tools.ToStringArray(
				new int[] {match.Category, match.Phase, 
							match.Round, match.Cycle, 
							match.Match});
			return String.Join("_", arrItems);
		}

		public static WebSiteServices.MatchData StringToMatch(string str)
		{
			if ((str == null) || (str.Length == 0))
				return null;
			string[] arrItems = str.Split(new char[] { '_' });
			if (arrItems.Length != 5)
				return null;
			WebSiteServices.MatchData result = new WebSiteServices.MatchData();
			try
			{
				result.Category = Int32.Parse(arrItems[0]);
				result.Phase = Int32.Parse(arrItems[1]);
				result.Round = Int32.Parse(arrItems[2]);
				result.Cycle = Int32.Parse(arrItems[3]);
				result.Match = Int32.Parse(arrItems[4]);
			}
			catch
			{
				return null;
			}
			return result;
		}
		#endregion

		#region not used
		/*
		public static string FormatIsfDate(DateTime date, string format)
		{
			string result=format;
			string strYear=date.Year.ToString();
			string strMonth=date.Month.ToString();
			string strDay=date.Day.ToString();
			string strHour=date.Hour.ToString();
			string strMinute=date.Minute.ToString();
			string strSecond=date.Second.ToString();
			string strMiliSecond=date.Millisecond.ToString();
			result = result.Replace("%dd", strDay.PadLeft(2, '0'));
			result = result.Replace("%d", strDay);
			result = result.Replace("%mm", strMonth.PadLeft(2, '0'));
			result = result.Replace("%m", strMonth);
			result = result.Replace("%yyyy", strYear);
			result = result.Replace("%yy", strYear.Substring(strYear.Length-2, 2));
			result = result.Replace("%hh", strHour.PadLeft(2, '0'));
			result = result.Replace("%h", strHour);
			result = result.Replace("%nn", strMinute.PadLeft(2, '0'));
			result = result.Replace("%n", strMinute);
			result = result.Replace("%ss", strSecond.PadLeft(2, '0'));
			result = result.Replace("%dd", strDay.PadLeft(2, '0'));
			result = result.Replace("%dd", strDay.PadLeft(2, '0'));
			result = result.Replace("%dd", strDay.PadLeft(2, '0'));
			result = result.Replace("%dd", strDay.PadLeft(2, '0'));
			result = result.Replace("%dd", strDay.PadLeft(2, '0'));
		}
		*/
		#endregion
		#endregion

		#region Private Methods - for internal use
		private static bool IsUserAuthorized(int userid, string formName)
		{
			return true;
			/*
			if (Sport.Core.PermissionsManager.IsSuperUser(userid))
				return true;
			try
			{
				PermissionServices.PermissionService permService=
					new PermissionServices.PermissionService();
				permService.CookieContainer = Sport.Core.Session.Cookies;
				PermissionServices.PermissionData[] permissions=
					permService.GetUserPermissions(userid);
				for (int i=0; i<permissions.Length; i++)
				{
					if (permissions[i].FormClassName == formName)
						return (permissions[i].Type == PermissionServices.PermissionType.Full);
				}
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine("failed to get permissions: "+e.Message);
				System.Diagnostics.Debug.WriteLine(e.StackTrace);
			}
			return false;
			*/
		}

		private static bool TryGetStudentNumberFromQueryString(out string idNumber)
		{
			HttpContext context = HttpContext.Current;
			idNumber = context.Request.QueryString["student_id_number"] + "";
			if (idNumber.Length > 0)
			{
				long dummy;
				if (Int64.TryParse(idNumber, out dummy) && dummy > 0)
					return true;
			}
			return false;
		}

		private static Entity GetStudentByIdNumber(string idNumber)
		{
			Entity[] existingStudents = Sport.Entities.Student.Type.GetEntities(
				new EntityFilter((int)Sport.Entities.Student.Fields.IdNumber, idNumber));
			if (existingStudents != null && existingStudents.Length == 1)
				return existingStudents[0];
			return null;
		}

		private static void SortCombo(ListControl combo, IComparer comparer)
		{
			int i;
			if (combo.Items.Count <= 1)
				return;
			ArrayList arrItems = new ArrayList();
			for (i = 0; i < combo.Items.Count; i++)
			{
				ListItem item = combo.Items[i];
				arrItems.Add(item);
				//arrValues.Add(comparer is ComboValueComparer)?item.Value:item.Text;
			}
			arrItems.Sort(comparer);
			combo.Items.Clear();
			for (i = 0; i < arrItems.Count; i++)
			{
				combo.Items.Add((ListItem)arrItems[i]);
			}
		}

		private static List<SportSite.Controls.LinkBox.Link> GetBigLinksBasedOnQuerystring(SportSite.Controls.LinkBox box, int startIndex, out string subCaption)
		{
			subCaption = string.Empty;

			string selIndex = (HttpContext.Current.Request.QueryString["sub"] + "");
			string selIndex2 = (HttpContext.Current.Request.QueryString["sub2"] + "");
			List<SportSite.Controls.LinkBox.Link> links = new List<Controls.LinkBox.Link>();

			foreach (SportSite.Controls.LinkBox.Link link in box.Links)
				links.Add(link);

			links.RemoveAll(l => { return !l.Visible; });
			links.RemoveRange(0, startIndex);

			if (selIndex.Length > 0)
			{
				int nIndex;
				if (Int32.TryParse(selIndex, out nIndex))
				{
					nIndex -= startIndex;
					if (nIndex >= 0 && nIndex < links.Count)
					{
						SportSite.Controls.LinkBox.Link selectedLink = links[nIndex];
						links.Clear();
						subCaption = selectedLink.Text;
						SportSite.Controls.LinkBox.LinkCollection subLinks = selectedLink.Links;
						foreach (SportSite.Controls.LinkBox.Link link in subLinks)
						{
							if (link.Visible)
							{
								link.Level = 1;
								links.Add(link);
							}
						}
						if (selIndex2.Length > 0)
						{
							if (Int32.TryParse(selIndex2, out nIndex))
							{
								if (nIndex >= 0 && nIndex < subLinks.Count)
								{
									selectedLink = subLinks[nIndex];
									links.Clear();
									subCaption = selectedLink.Text;
									foreach (SportSite.Controls.LinkBox.Link link in selectedLink.Links)
									{
										if (link.Visible)
										{
											link.Level = 2;
											links.Add(link);
										}
									}
								}
							}
						}
					}
				}
			}

			return links;
		}

		private static string DecideBigLinkURL(SportSite.Controls.LinkBox.Link link, int index)
		{
			if (!string.IsNullOrEmpty(link.Url) && (link.Links == null || link.Links.Count == 0))
				return link.Url;
			string url = "?sub=";
			if (link.Level > 0)
			{
				url += HttpContext.Current.Request.QueryString["sub"];
				url += "&sub2=";
			}
			url += index.ToString();
			return url;
		}

		private static void RangeKeyToMonthAndYear(string key, out int month, out int year)
		{
			month = 0;
			year = 0;
			if (key.Length > 0)
			{
				year = Int32.Parse(key.Substring(key.Length - 4, 4));
				month = Int32.Parse(key.Substring(0, key.Length - 4), 0);
			}
		}

		public static string GetHebrewTimeElpased(int secondsElpased, int minutesElpased, int hoursElpased,
			int daysElpased, int monthsElpased, int yearsElpased)
		{
			if (secondsElpased < 30)
				return "שניות אחדות";
			
			if (minutesElpased < 1)
				return Sport.Common.Tools.HebrewCount(secondsElpased, false) + " שניות";
			
			if (minutesElpased < 5)
				return "דקות אחדות";

			if (hoursElpased < 1)
				return Sport.Common.Tools.HebrewCount(minutesElpased, false) + " דקות";

			if (hoursElpased < 5)
				return "שעות אחדות";

			if (daysElpased < 1)
				return Sport.Common.Tools.HebrewCount(hoursElpased, false) + " שעות";

			if (daysElpased == 2)
				return "יומיים";

			if (monthsElpased < 1)
				return Sport.Common.Tools.HebrewCount(daysElpased, true) + " ימים";

			if (monthsElpased == 1)
				return "חודש";

			if (monthsElpased == 2)
				return "חודשיים";

			if (yearsElpased < 1)
				return Sport.Common.Tools.HebrewCount(monthsElpased, true) + " חודשים";

			string halfYear = (monthsElpased >= 6) ? " וחצי" : "";
			if (yearsElpased == 1)
				return "שנה" + halfYear;

			if (yearsElpased == 2)
				return "שנתיים" + halfYear;

			return Sport.Common.Tools.HebrewCount(yearsElpased, false) + " שנים";
		}

		public static string KeepOriginalFormat(string text)
		{
			return text.Replace("\n\r", "\n").Replace("\r\n", "\n").Replace("\n", "<br />").Replace("  ", "&nbsp;&nbsp;");
		}

		#region Combo Comparers
		/// <summary>
		/// compare list items by their value
		/// </summary>
		private class ComboValueComparer : IComparer
		{
			public enum SortOrder
			{
				Ascending = 1,
				Descending = -1
			}
			private int _modifier;

			public ComboValueComparer()
			{
				_modifier = (int)SortOrder.Ascending;
			}

			public ComboValueComparer(SortOrder order)
			{
				_modifier = (int)order;
			}
			//sort by value
			public int Compare(Object o1, Object o2)
			{
				ListItem cb1 = (ListItem)o1;
				ListItem cb2 = (ListItem)o2;
				return cb1.Value.CompareTo(cb2.Value) * _modifier;
			}
		} //end class ComboValueComparer

		/// <summary>
		/// compare list items by their text.
		/// </summary>
		private class ComboTextComparer : IComparer
		{
			public enum SortOrder
			{
				Ascending = 1,
				Descending = -1
			}
			private int _modifier;

			public ComboTextComparer()
			{
				_modifier = (int)SortOrder.Ascending;
			}

			public ComboTextComparer(SortOrder order)
			{
				_modifier = (int)order;
			}
			//sort by value
			public int Compare(Object o1, Object o2)
			{
				ListItem cb1 = (ListItem)o1;
				ListItem cb2 = (ListItem)o2;
				return cb1.Text.CompareTo(cb2.Text) * _modifier;
			}
		} //end class ComboTextComparer
		#endregion
		#endregion
	} //end class Tools

	public class DebugTools
	{
		/// <summary>
		/// prints all three collections (QueryString, Form and ServerVariables) of 
		/// the given page.
		/// </summary>
		/// <param name="page"></param>
		public static void PrintRequestCollections(System.Web.UI.Page page)
		{
			page.Response.Write("---debugging page control---<br />");
			page.Response.Write("QueryString collection:<br />");
			foreach (string key in page.Request.QueryString)
				page.Response.Write(key + " = " + page.Request.QueryString[key] + "<br />");
			page.Response.Write("------------------<br />");
			page.Response.Write("Form collection:<br />");
			foreach (string key in page.Request.Form)
				page.Response.Write(key + " = " + page.Request.Form[key] + "<br />");
			page.Response.Write("------------------<br />");
			page.Response.Write("ServerVariables collection:<br />");
			foreach (string key in page.Request.ServerVariables)
				page.Response.Write(key + " = " + page.Request.ServerVariables[key] + "<br />");
			page.Response.Write("------------------<br />");
		}
	}

	public class StringTools
	{
		public static string Left(string str, int charsCount)
		{
			return str.Substring(0, charsCount);
		}

		public static string Right(string str, int charsCount)
		{
			return str.Substring(str.Length - charsCount, charsCount);
		}
	}

	public static class GeneralExtensions
	{
		public static string GetMimeType(this System.Drawing.Imaging.ImageFormat imageFormat)
		{
			System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
			return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
		}

		public static T GetItemOrDefault<T>(this T[] array, int index, T defaultValue)
		{
			return (index >= 0 && index < array.Length) ? array[index] : defaultValue;
		}
	}

	public static class StringExtensions
	{
		public static string[] SplitRemoveBlank(this string s, char c)
		{
			return s.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static int[] SplitToNumbers(this string s, char c)
		{
			return s.SplitRemoveBlank(c).ToList().ConvertAll(n => { return Int32.Parse(n); }).ToArray();
		}

		public static string LastWord(this string s)
		{
			s = s.Trim();
			int index = (s + "").LastIndexOf(" ");
			return (index > 0) ? s.Substring(index + 1) : s;
		}

		public static string GetDefaultValueIfEmpty(this string s, string d)
		{
			return string.IsNullOrEmpty(s) ? d : s;
		}

		public static int ToIntOrDefault(this string s, int d)
		{
			int n;
			if (!Int32.TryParse(s, out n))
				n = d;
			return n;
		}
	}

	public static class StudentPictureHelper
	{
		private static string folderPath = "";
		private static readonly string filePrefix = "st_";
		private static readonly string[] validPicExtensions = new string[] { "jpg", "gif", "bmp", "png" };

		static StudentPictureHelper()
		{
			folderPath = ConfigurationManager.AppSettings["StudentPicturesFolder"] + "";
		}

		public static string Error
		{
			get
			{
				if (folderPath.Length == 0 || !Directory.Exists(folderPath))
					return "Missing folder path or folder does not exist, check server configuration";
				return "";
			}
		}

		public static StudentPictureItem[] GetAllPictures()
		{
			List<StudentPictureItem> items = new List<StudentPictureItem>();
			DataServices1.DataService service = new DataServices1.DataService();
			Directory.GetFiles(folderPath, filePrefix + "*.*").ToList().ForEach(currentPath =>
			{
				if (Array.Exists<string>(validPicExtensions, e => Path.GetExtension(currentPath).Replace(".", "").Equals(e, StringComparison.InvariantCultureIgnoreCase)))
				{
					string idNumber = Path.GetFileNameWithoutExtension(currentPath).Replace(filePrefix, "");
					DataServices1.StudentData student = service.GetStudentByNumber(idNumber);
					if (student != null && student.IdNumber == idNumber)
					{
						items.Add(new StudentPictureItem
						{
							StudentEntityId = student.ID, 
							IdNumber = student.IdNumber, 
							PicturePath = currentPath, 
							FirstName = student.FirstName, 
							LastName = student.LastName, 
							DateLastModified = File.GetLastWriteTime(currentPath)
						});
					}
				}
			});
			service.Dispose();
			return items.ToArray();
		}

		public static string GetExistingPicture(string idNumber)
		{
			if (Error.Length > 0)
				return "";
			return Directory.GetFiles(folderPath, filePrefix + idNumber + ".*").ToList().FirstOrDefault(f => Array.Exists<string>(validPicExtensions, e => Path.GetExtension(f).Replace(".", "").Equals(e, StringComparison.InvariantCultureIgnoreCase))) + "";
		}

		public static string GenerateImageSrc(string idNumber)
		{
			return string.Format("{0}?action=DisplayStudentPicture&student_id_number={1}&ts={2}",
				HttpContext.Current.Request.FilePath, idNumber, DateTime.Now.Ticks);
		}

		public static string GenerateImageTag(string idNumber, int size)
		{
			return string.Format("<img src=\"{0}\" width=\"{1}\" height=\"{1}\" />",
				GenerateImageSrc(idNumber), size);
		}

		public static bool TryGeneratePath(string idNumber, HttpPostedFile uploadedFile, out string path, 
			out string error)
		{
			path = "";
			error = "";
			
			System.Drawing.Image image = null;
			try
			{
				image = System.Drawing.Image.FromStream(uploadedFile.InputStream);
			}
			catch
			{
			}

			if (image == null || image.Width == 0 || image.Height == 0)
			{
				error = "יש להעלות קובץ תמונה בלבד";
				return false;
			}

			string mimeType = image.RawFormat.GetMimeType().ToLower().Replace("jpeg", "jpg");
			string extension = Array.Find<string>(validPicExtensions, e => mimeType.Contains(e));
			if (string.IsNullOrEmpty(extension))
			{
				error = "פורמט תמונה לא תקין. יש לשמור את התמונה באחד מהפורמטים הבאים: " + string.Join(", ", validPicExtensions);
				return false;
			}

			path = Path.Combine(folderPath, string.Format("{0}{1}.{2}", filePrefix, idNumber, extension));
			return true;
		}
	}

	public class StudentPictureItem
	{
		public int StudentEntityId { get; set; }
		public string IdNumber { get; set; }
		public string PicturePath { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime DateLastModified { get; set; }
	}
}
