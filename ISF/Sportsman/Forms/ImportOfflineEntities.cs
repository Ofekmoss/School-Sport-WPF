using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for ImportOfflineEntities.
	/// </summary>
	public class ImportOfflineEntities : System.Windows.Forms.Form
	{
		private enum OfflineStep
		{
			School = 0,
			Student,
			Team, 
			Player
		}
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbCurrentStep;
		private System.Windows.Forms.Label lbTotalSteps;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnContinue;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListBox lbData;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbMessages;
		private System.Windows.Forms.Button btnImport;
		private System.Type[] _offlineTypes = new System.Type[] {
			typeof(Sport.Entities.OfflineSchool), typeof(Sport.Entities.OfflineStudent),
			typeof(Sport.Entities.OfflineTeam), typeof(Sport.Entities.OfflinePlayer) };
		private string[] _captions = new string[] { "בתי ספר", "תלמידים", 
			"קבוצות", "שחקנים" };
		private ArrayList _data = new ArrayList();
		private int _currentStep = 0;
		private int _totalSteps = 0;
		private System.Windows.Forms.GroupBox gbEntity;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ImportOfflineEntities()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...", true);
			Sport.UI.Dialogs.WaitForm.SetProgress(0);
			int typeProgrss = (int) ((((double) 100)/((double) _offlineTypes.Length)));
			int curProgress = 0;
			for (int i=0; i<_offlineTypes.Length; i++)
			{
				_data.Add(Sport.Data.OfflineEntity.LoadAllEntities(_offlineTypes[i]));
				curProgress += typeProgrss;
				Sport.UI.Dialogs.WaitForm.SetProgress(curProgress);
			}
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			foreach (Sport.Data.OfflineEntity[] arrEntities in _data)
				if (arrEntities.Length > 0)
					_totalSteps++;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lbCurrentStep = new System.Windows.Forms.Label();
			this.lbTotalSteps = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.gbEntity = new System.Windows.Forms.GroupBox();
			this.btnContinue = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.lbData = new System.Windows.Forms.ListBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbMessages = new System.Windows.Forms.TextBox();
			this.btnImport = new System.Windows.Forms.Button();
			this.gbEntity.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(408, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(192, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "ייבוא נתונים לא מקוונים";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(344, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 22);
			this.label2.TabIndex = 1;
			this.label2.Text = "צעד";
			// 
			// lbCurrentStep
			// 
			this.lbCurrentStep.AutoSize = true;
			this.lbCurrentStep.ForeColor = System.Drawing.Color.Blue;
			this.lbCurrentStep.Location = new System.Drawing.Point(320, 16);
			this.lbCurrentStep.Name = "lbCurrentStep";
			this.lbCurrentStep.Size = new System.Drawing.Size(15, 22);
			this.lbCurrentStep.TabIndex = 2;
			this.lbCurrentStep.Text = "0";
			// 
			// lbTotalSteps
			// 
			this.lbTotalSteps.AutoSize = true;
			this.lbTotalSteps.ForeColor = System.Drawing.Color.Blue;
			this.lbTotalSteps.Location = new System.Drawing.Point(248, 16);
			this.lbTotalSteps.Name = "lbTotalSteps";
			this.lbTotalSteps.Size = new System.Drawing.Size(15, 22);
			this.lbTotalSteps.TabIndex = 4;
			this.lbTotalSteps.Text = "0";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(272, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 22);
			this.label3.TabIndex = 3;
			this.label3.Text = "מתוך";
			// 
			// gbEntity
			// 
			this.gbEntity.Controls.Add(this.btnImport);
			this.gbEntity.Controls.Add(this.tbMessages);
			this.gbEntity.Controls.Add(this.label5);
			this.gbEntity.Controls.Add(this.lbData);
			this.gbEntity.Controls.Add(this.label4);
			this.gbEntity.ForeColor = System.Drawing.Color.Blue;
			this.gbEntity.Location = new System.Drawing.Point(8, 48);
			this.gbEntity.Name = "gbEntity";
			this.gbEntity.Size = new System.Drawing.Size(592, 256);
			this.gbEntity.TabIndex = 5;
			this.gbEntity.TabStop = false;
			this.gbEntity.Text = "יישות";
			// 
			// btnContinue
			// 
			this.btnContinue.Enabled = false;
			this.btnContinue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnContinue.Location = new System.Drawing.Point(8, 304);
			this.btnContinue.Name = "btnContinue";
			this.btnContinue.TabIndex = 6;
			this.btnContinue.Text = "המשך>>";
			this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
			// 
			// label4
			// 
			this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label4.Location = new System.Drawing.Point(310, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(276, 22);
			this.label4.TabIndex = 2;
			this.label4.Text = "רשימת נתונים";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbData
			// 
			this.lbData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbData.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbData.ItemHeight = 16;
			this.lbData.Location = new System.Drawing.Point(310, 48);
			this.lbData.Name = "lbData";
			this.lbData.Size = new System.Drawing.Size(275, 178);
			this.lbData.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label5.Location = new System.Drawing.Point(16, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(276, 22);
			this.label5.TabIndex = 4;
			this.label5.Text = "תיבת הודעות";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// tbMessages
			// 
			this.tbMessages.AutoSize = false;
			this.tbMessages.Cursor = System.Windows.Forms.Cursors.Default;
			this.tbMessages.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbMessages.Location = new System.Drawing.Point(8, 48);
			this.tbMessages.Multiline = true;
			this.tbMessages.Name = "tbMessages";
			this.tbMessages.ReadOnly = true;
			this.tbMessages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbMessages.Size = new System.Drawing.Size(288, 200);
			this.tbMessages.TabIndex = 5;
			this.tbMessages.Text = "";
			// 
			// btnImport
			// 
			this.btnImport.Enabled = false;
			this.btnImport.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnImport.Location = new System.Drawing.Point(392, 230);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(184, 23);
			this.btnImport.TabIndex = 6;
			this.btnImport.Text = "העבר לשרת מרכזי";
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// ImportOfflineEntities
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
			this.ClientSize = new System.Drawing.Size(614, 336);
			this.ControlBox = false;
			this.Controls.Add(this.btnContinue);
			this.Controls.Add(this.gbEntity);
			this.Controls.Add(this.lbTotalSteps);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lbCurrentStep);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImportOfflineEntities";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ייבוא נתונים לא מקוונים";
			this.Load += new System.EventHandler(this.ImportOfflineEntities_Load);
			this.gbEntity.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ImportOfflineEntities_Load(object sender, System.EventArgs e)
		{
			lbTotalSteps.Text = _totalSteps.ToString();
			_currentStep = 0;
			ApplyCurrentStep();
		}
		
		private void ApplyCurrentStep()
		{
			lbData.Items.Clear();
			tbMessages.Text = "";
			if (_currentStep >= _data.Count)
			{
				lbCurrentStep.Text = _totalSteps.ToString();
				btnContinue.Text = "סיום";
				btnContinue.Enabled = true;
				btnImport.Enabled = false;
				gbEntity.Text = "";
				tbMessages.Text = "לחץ על כפתור \"סיום\" כדי לסיים את התהליך.";
				return;
			}
			lbCurrentStep.Text = (_currentStep+1).ToString();
			gbEntity.Text = _captions[_currentStep];
			Sport.Data.OfflineEntity[] arrEntities = 
				(Sport.Data.OfflineEntity[]) _data[_currentStep];
			if (arrEntities.Length == 0)
			{
				_currentStep++;
				ApplyCurrentStep();
				return;
			}
			Sport.Common.ListItem[] arrItems = new Sport.Common.ListItem[arrEntities.Length];
			string strText = "";
			for (int i=0; i<arrEntities.Length; i++)
			{
				OfflineStep oStep = (OfflineStep) _currentStep;
				Sport.Data.OfflineEntity curEntity = arrEntities[i];
				strText = "";
				switch (oStep)
				{
					case OfflineStep.Student:
						Sport.Entities.OfflineStudent oStudent = 
							(Sport.Entities.OfflineStudent) curEntity;
						strText = oStudent.FirstName+" "+oStudent.LastName+" "+
							"ת.ז. "+oStudent.IdNumber;
						break;
					case OfflineStep.School:
						Sport.Entities.OfflineSchool oSchool = 
							(Sport.Entities.OfflineSchool) curEntity;
						strText = oSchool.Name+" "+
							"סמל בי\"ס "+oSchool.Symbol;
						break;
					case OfflineStep.Player:
						Sport.Entities.OfflinePlayer oPlayer = 
							(Sport.Entities.OfflinePlayer) curEntity;
						strText = oPlayer.ToString()+" "+
							"קבוצה "+oPlayer.GetTeamName();
						break;
					case OfflineStep.Team:
						Sport.Entities.OfflineTeam oTeam = 
							(Sport.Entities.OfflineTeam) curEntity;
						strText = oTeam.ToString();
						break;
				}
				arrItems[i] = new Sport.Common.ListItem();
				arrItems[i].Value = arrEntities[i];
				arrItems[i].Text = strText;
			}
			lbData.Items.AddRange(arrItems);
			strText = "נמצאו "+arrEntities.Length+" "+_captions[_currentStep]+" במאגר הלא מקוון.\n"+
				"\nלחץ על כפתור \"העבר לשרת מרכזי\" כדי להמשיך בתהליך הייבוא.";
			tbMessages.Lines = strText.Split(new char[] {'\n'});
			btnImport.Enabled = true;
		}

		private void btnImport_Click(object sender, System.EventArgs e)
		{
			string strMessage = "";
			if (lbData.Items.Count == 0)
			{
				strMessage = "לא נמצאו נתונים במאגר הלא מקוון";
			}
			else
			{
				OfflineStep oStep = (OfflineStep) _currentStep;
				Sport.Data.OfflineEntity[] arrEntities = 
					new Sport.Data.OfflineEntity[lbData.Items.Count];
				for (int i=0; i<lbData.Items.Count; i++)
				{
					arrEntities[i] = (Sport.Data.OfflineEntity)
						(lbData.Items[i] as Sport.Common.ListItem).Value;
				}
				Sport.UI.Dialogs.WaitForm.ShowWait("מייבא נתונים אנא המתן...", true);
				btnImport.Enabled = false;
				Sport.UI.Dialogs.WaitForm.SetProgress(0);
				double entProgress = 0;
				if (arrEntities.Length > 0)
					entProgress = (((double) 100)/((double) arrEntities.Length));
				double curProgress = 0;
				foreach (Sport.Data.OfflineEntity oEntity in arrEntities)
				{
					strMessage += ImportSingleEntity(oEntity)+"\n";
					curProgress += entProgress;
					Sport.UI.Dialogs.WaitForm.SetProgress((int) curProgress);
				}
				Sport.UI.Dialogs.WaitForm.SetProgress(100);
				Sport.UI.Dialogs.WaitForm.HideWait();
				
				if (oStep == OfflineStep.Team)
				{
					string[] arrImportLines = ImportChampionshipTeams(arrEntities);
					strMessage += String.Join("\n", arrImportLines)+"\n";
				}
				else if (oStep == OfflineStep.Player)
				{
					string[] arrImportLines = ImportChampionshipPlayers(arrEntities);
					strMessage += String.Join("\n", arrImportLines)+"\n";
				}
				btnImport.Enabled = true;
			}
			strMessage += "\nלחץ על כפתור \"המשך\" כדי לעבור לשלב הבא";
			tbMessages.Lines = strMessage.Split(new char[] {'\n'});
			btnImport.Enabled = false;
			btnContinue.Enabled = true;
		}

		private void btnContinue_Click(object sender, System.EventArgs e)
		{
			if (btnContinue.Enabled == false)
				return;
			
			if (btnContinue.Text == "סיום")
			{
				DeleteOfflineData();
				this.DialogResult = DialogResult.OK;
				this.Close();
				return;
			}
			
			btnContinue.Enabled = false;
			_currentStep++;
			ApplyCurrentStep();
		}
		
		private string ImportSingleEntity(Sport.Data.OfflineEntity oEntity)
		{
			string result = "סוג יישות לא מזוהה: "+oEntity.GetType().FullName;
			if (oEntity is Sport.Entities.OfflineSchool)
			{
				Sport.Entities.OfflineSchool oSchool = 
					(Sport.Entities.OfflineSchool) oEntity;
				result = ImportSingleSchool(ref oSchool);
			}
			else if (oEntity is Sport.Entities.OfflineStudent)
			{
				Sport.Entities.OfflineStudent oStudent = 
					(Sport.Entities.OfflineStudent) oEntity;
				result = ImportSingleStudent(ref oStudent);
			}
			else if (oEntity is Sport.Entities.OfflineTeam)
			{
				Sport.Entities.OfflineTeam oTeam = 
					(Sport.Entities.OfflineTeam) oEntity;
				result = ImportSingleTeam(ref oTeam);
			}
			else if (oEntity is Sport.Entities.OfflinePlayer)
			{
				Sport.Entities.OfflinePlayer oPlayer = 
					(Sport.Entities.OfflinePlayer) oEntity;
				result = ImportSinglePlayer(ref oPlayer);
			}
			
			return result; //"נתוני "+strName+" עברו בהצלחה אל השרת.";
		}
		
		private string ImportSingleSchool(ref Sport.Entities.OfflineSchool oSchool)
		{
			string strSymbol = oSchool.Symbol;
			Sport.Entities.School school = Sport.Entities.School.FromSymbol(strSymbol);
			if (school != null)
			{
				string strPureName = school.Entity.Fields[(int) Sport.Entities.School.Fields.Name].ToString();
				if (strPureName == oSchool.ToString())
					return "בית הספר '"+strPureName+"' כבר קיים במאגר הנתונים, אין צורך לייבא";
				string strError = "'"+oSchool.Name+"': "+
					"בית ספר עם אותו סימול כבר קיים במאגר הנתונים המרכזי.\n"+
					"בית ספר זה הוא '"+school.Name+"'\n";
				string strQuestion = 
					"האם ברצונך להגדיר סמל בית ספר חדש, או לבחור בית ספר קיים?\n"+
					"לחץ כן על מנת להגדיר סמל חדש, לחץ לא כדי לבחור מרשימה";
				
				Views.SchoolsTableView schoolView = new Views.SchoolsTableView();
				schoolView.State[Sport.UI.View.SelectionDialog] = "1";
				schoolView.State[Sport.Entities.School.TypeName] = school.Id.ToString();
				schoolView.State[Sport.Entities.Region.TypeName] = oSchool.Region.ToString();
				Sport.UI.EntitySelectionDialog schoolDialog = 
					new Sport.UI.EntitySelectionDialog(schoolView);
				
				Sport.UI.Controls.ButtonBox.SelectValue selector = 
					new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector);
				
				while (1 == 1)
				{
					Sport.UI.Dialogs.GenericEditDialog objDialog = 
						new Sport.UI.Dialogs.GenericEditDialog("בחירת בית ספר");
					Sport.UI.Controls.GenericItem item = null;
					if (Sport.UI.MessageBox.Ask(strError+"\n"+strQuestion, 
						"ייבוא בית ספר", MessageBoxIcon.Question, true) == true)
					{
						item = new Sport.UI.Controls.GenericItem("סמל בית ספר", 
							Sport.UI.Controls.GenericItemType.Text);
					}
					else
					{
						item = new Sport.UI.Controls.GenericItem("בית ספר", 
							Sport.UI.Controls.GenericItemType.Button);
						item.Values = Sport.UI.Controls.GenericItem.ButtonValues(selector);
					}
					objDialog.Items.Add(item);
					if (objDialog.ShowDialog() == DialogResult.OK)
					{
						school = null;
						object value = objDialog.Items[0].Value;
						strSymbol = "";
						switch (item.ItemType)
						{
							case Sport.UI.Controls.GenericItemType.Text:
								strSymbol = value.ToString();
								school = Sport.Entities.School.FromSymbol(strSymbol);
								break;
							case Sport.UI.Controls.GenericItemType.Button:
								if (value != null)
									school = new Sport.Entities.School(value as Sport.Data.Entity);
								break;
						}
						if (school == null)
						{
							if (strSymbol.Length < 6)
							{
								strError = "סמל בית ספר לא חוקי או שלא נבחר בית ספר\n";
							}
							else
							{
								oSchool.Symbol = strSymbol;
								oSchool.Save();
								ApplyOfflineSymbol(oSchool.OfflineID, strSymbol);
								break;
							}
						}
						else
						{
							strPureName = school.Entity.Fields[(int) Sport.Entities.School.Fields.Name].ToString();
							int diff = Sport.Common.Tools.CountDifferentCharacters(
								strPureName, oSchool.Name);
							bool blnUseSchool = true;
							if (diff > 2)
							{
								string strCurMessage = "אזהרה: שם בית הספר שנבחר '"+
									strPureName+"' שונה משם בית הספר הלא מקוון.\n"+
									"האם אתה בטוח שזהו בית הספר הנכון?";
								blnUseSchool = Sport.UI.MessageBox.Ask(strCurMessage, 
									"ייבוא בית ספר", MessageBoxIcon.Warning, false);
							}
							if (blnUseSchool)
							{
								oSchool.Symbol = school.Symbol;
								oSchool.City = -1;
								if (school.City != null)
									oSchool.City = school.City.Id;
								oSchool.Region = -1;
								if (school.Region != null)
									oSchool.Region = school.Region.Id;
								oSchool.Name = strPureName;
								oSchool.Save();
								ApplyOfflineSymbol(oSchool.OfflineID, oSchool.Symbol);
								return "משתמש בבית ספר קיים '"+school.Name+"' אין צורך לייבא";
							}
							strError = "";
						}
					}
				}
			}
			strSymbol = oSchool.Symbol;
			Sport.Entities.Region region = null;
			try
			{
				region = new Sport.Entities.Region(oSchool.Region);
			}
			catch
			{
				return "לא ניתן להוסיף את בית הספר '"+oSchool.Name+"': זיהוי מחוז שגוי";
			}
			Sport.Entities.City city = null;
			if (oSchool.City >= 0)
			{
				try
				{
					city = new Sport.Entities.City(oSchool.City);
				}
				catch
				{
					city = null;
				}
			}
			Sport.Data.EntityEdit entEdit = Sport.Entities.School.Type.New();
			entEdit.Fields[(int) Sport.Entities.School.Fields.Symbol] = 
				strSymbol;
			entEdit.Fields[(int) Sport.Entities.School.Fields.Region] = region.Id;
			if (city != null)
				entEdit.Fields[(int) Sport.Entities.School.Fields.City] = city.Id;
			entEdit.Fields[(int) Sport.Entities.School.Fields.Name] = oSchool.Name;
			entEdit.Fields[(int) Sport.Entities.School.Fields.ClubStatus] = 0;
			entEdit.Fields[(int) Sport.Entities.School.Fields.LastModified] = DateTime.Now;
			Sport.Data.EntityResult result = entEdit.Save();
			if (!result.Succeeded)
				return "כשלון בעת שמירת בית הספר '"+oSchool.Name+"' למאגר הנתונים: "+result.ResultCode.ToString();
			return "בית הספר '"+oSchool.Name+"' נוסף בהצלחה למאגר הנתונים המרכזי";
		}
		
		private string ImportSingleStudent(ref Sport.Entities.OfflineStudent oStudent)
		{
			string strIdNumber = oStudent.IdNumber;
			Sport.Entities.Student student = Sport.Entities.Student.FromIdNumber(strIdNumber);
			if (student != null)
			{
				string strExistingName = student.FirstName+" "+student.LastName;
				if (strExistingName == oStudent.ToString())
					return "התלמיד '"+strExistingName+"' כבר קיים במאגר הנתונים, אין צורך לייבא";
				
				string strError = "'"+oStudent.ToString()+"': "+
					"תלמיד עם אותו מספר זהות כבר קיים במאגר הנתונים המרכזי.\n"+
					"תלמיד זה הוא '"+strExistingName+"'\n";
				string strQuestion = 
					"האם ברצונך להגדיר מספר זהות חדש, או לבחור תלמיד קיים?\n"+
					"לחץ כן על מנת להגדיר מספר זהות חדש, לחץ לא כדי לבחור מרשימה";
				
				Views.StudentsTableView studentView = new Views.StudentsTableView();
				studentView.State[Sport.UI.View.SelectionDialog] = "1";
				studentView.State[Sport.Entities.Region.TypeName] = 
					oStudent.GetRegion().ToString();
				if (oStudent.School != null)
					studentView.State[Sport.Entities.School.TypeName] = oStudent.School.Id.ToString();
				Sport.UI.EntitySelectionDialog studentDialog = 
					new Sport.UI.EntitySelectionDialog(studentView);
				
				Sport.UI.Controls.ButtonBox.SelectValue selector = 
					new Sport.UI.Controls.ButtonBox.SelectValue(studentDialog.ValueSelector);
				
				while (1 == 1)
				{
					Sport.UI.Dialogs.GenericEditDialog objDialog = 
						new Sport.UI.Dialogs.GenericEditDialog("בחירת תלמיד");
					Sport.UI.Controls.GenericItem item = null;
					if (Sport.UI.MessageBox.Ask(strError+"\n"+strQuestion, 
						"ייבוא תלמיד", MessageBoxIcon.Question, true) == true)
					{
						item = new Sport.UI.Controls.GenericItem("מספר זהות", 
							Sport.UI.Controls.GenericItemType.Text);
					}
					else
					{
						item = new Sport.UI.Controls.GenericItem("תלמיד", 
							Sport.UI.Controls.GenericItemType.Button);
						item.Values = Sport.UI.Controls.GenericItem.ButtonValues(selector);
					}
					objDialog.Items.Add(item);
					if (objDialog.ShowDialog() == DialogResult.OK)
					{
						student = null;
						object value = objDialog.Items[0].Value;
						strIdNumber = "";
						switch (item.ItemType)
						{
							case Sport.UI.Controls.GenericItemType.Text:
								strIdNumber = value.ToString();
								student = Sport.Entities.Student.FromIdNumber(strIdNumber);
								break;
							case Sport.UI.Controls.GenericItemType.Button:
								if (value != null)
									student = new Sport.Entities.Student(value as Sport.Data.Entity);
								break;
						}
						if (student == null)
						{
							if (!Sport.Common.Tools.IsInteger(strIdNumber))
							{
								strError = "מספר זהות לא חוקי או שלא נבחר תלמיד\n";
							}
							else
							{
								oStudent.IdNumber = strIdNumber;
								oStudent.Save();
								ApplyOfflineIdNumber(oStudent.OfflineID, strIdNumber);
								break;
							}
						}
						else
						{
							strExistingName = student.FirstName+" "+student.LastName;
							int diff = Sport.Common.Tools.CountDifferentCharacters(
								strExistingName, oStudent.ToString());
							bool blnUseStudent = true;
							if (diff > 2)
							{
								string strCurMessage = "אזהרה: שם התלמיד שנבחר '"+
									strExistingName+"' שונה משם התלמיד הלא מקוון.\n"+
									"האם אתה בטוח שזהו התלמיד הנכון?";
								blnUseStudent = Sport.UI.MessageBox.Ask(strCurMessage, 
									"ייבוא תלמיד", MessageBoxIcon.Warning, false);
							}
							if (blnUseStudent)
							{
								oStudent.IdNumber = student.IdNumber;
								oStudent.FirstName = student.FirstName;
								oStudent.LastName = student.LastName;
								oStudent.Grade = student.Grade;
								oStudent.OfflineSchool = null;
								oStudent.School = student.School;
								oStudent.Save();
								ApplyOfflineIdNumber(oStudent.OfflineID, oStudent.IdNumber);
								return "משתמש בתלמיד קיים '"+oStudent.ToString()+"' אין צורך לייבא";
							}
							strError = "";
						}
					}
				}
			}
			strIdNumber = oStudent.IdNumber;
			Sport.Entities.School school = null;
			if (oStudent.School != null)
				school = oStudent.School;
			else if (oStudent.OfflineSchool != null)
				school = Sport.Entities.School.FromSymbol(oStudent.OfflineSchool.Symbol);
			if (school == null)
				return "לא ניתן להוסיף את התלמיד '"+oStudent.ToString()+"': לא מוגדר בית ספר";
			Sport.Data.EntityEdit entEdit = Sport.Entities.Student.Type.New();
			entEdit.Fields[(int) Sport.Entities.Student.Fields.IdNumber] = strIdNumber;
			entEdit.Fields[(int) Sport.Entities.Student.Fields.School] = school.Id;
			entEdit.Fields[(int) Sport.Entities.Student.Fields.Grade] = oStudent.Grade;
			entEdit.Fields[(int) Sport.Entities.Student.Fields.FirstName] = oStudent.FirstName;
			entEdit.Fields[(int) Sport.Entities.Student.Fields.LastName] = oStudent.LastName;
			entEdit.Fields[(int) Sport.Entities.Student.Fields.LastModified] = DateTime.Now;
			Sport.Data.EntityResult result = entEdit.Save();
			if (!result.Succeeded)
				return "כשלון בעת שמירת התלמיד '"+oStudent.ToString()+"' למאגר הנתונים: "+result.ResultCode.ToString();
			return "התלמיד '"+oStudent.ToString()+"' נוסף בהצלחה למאגר הנתונים המרכזי";
		}
		
		private string ImportSingleTeam(ref Sport.Entities.OfflineTeam oTeam)
		{
			Sport.Entities.ChampionshipCategory champCategory = null;
			try
			{
				champCategory= new Sport.Entities.ChampionshipCategory(oTeam.ChampionshipCategory);
			}
			catch
			{}
			
			if ((champCategory == null)||(champCategory.Id < 0))
				return "לא ניתן להוסיף את הקבוצה '"+oTeam.ToString()+"': לא מוגדרת קטגורית אליפות";
			
			Sport.Entities.School school = null;
			if (oTeam.School != null)
				school = oTeam.School;
			else if (oTeam.OfflineSchool != null)
				school = Sport.Entities.School.FromSymbol(oTeam.OfflineSchool.Symbol);
			if (school == null)
				return "לא ניתן להוסיף את הקבוצה '"+oTeam.ToString()+"': לא מוגדר בית ספר";
			
			Sport.Data.EntityEdit entEdit = Sport.Entities.Team.Type.New();
			entEdit.Fields[(int) Sport.Entities.Team.Fields.Category] = champCategory.Id;
			entEdit.Fields[(int) Sport.Entities.Team.Fields.Championship] = champCategory.Championship.Id;
			entEdit.Fields[(int) Sport.Entities.Team.Fields.School] = school.Id;
			entEdit.Fields[(int) Sport.Entities.Team.Fields.PlayerNumberFrom] = oTeam.PlayerNumberFrom;
			entEdit.Fields[(int) Sport.Entities.Team.Fields.PlayerNumberTo] = oTeam.PlayerNumberTo;
			entEdit.Fields[(int) Sport.Entities.Team.Fields.Status] = (int) Sport.Types.TeamStatusType.Confirmed;
			entEdit.Fields[(int) Sport.Entities.Team.Fields.RegisterDate] = DateTime.Now;
			entEdit.Fields[(int) Sport.Entities.Team.Fields.LastModified] = DateTime.Now;
			Sport.Data.EntityResult result = entEdit.Save();
			if (!result.Succeeded)
				return "כשלון בעת שמירת הקבוצה '"+oTeam.ToString()+"' למאגר הנתונים: "+result.ResultCode.ToString();
			return "הקבוצה '"+oTeam.ToString()+"' נוספה בהצלחה למאגר הנתונים המרכזי";
		}
		
		private string ImportSinglePlayer(ref Sport.Entities.OfflinePlayer oPlayer)
		{
			Sport.Entities.Student student = null;
			if (oPlayer.Student != null)
				student = oPlayer.Student;
			else if (oPlayer.OfflineStudent != null)
				student = Sport.Entities.Student.FromIdNumber(oPlayer.OfflineStudent.IdNumber);
			if (student == null)
				return "לא ניתן להוסיף את השחקן '"+oPlayer.ToString()+"': לא מוגדר תלמיד";
			
			Sport.Entities.Team team = oPlayer.GetPlayerTeam();
			if (team == null)
				return "לא ניתן להוסיף את השחקן '"+oPlayer.ToString()+"': לא מוגדרת קבוצה";
			
			Sport.Data.EntityEdit entEdit = Sport.Entities.Player.Type.New();
			entEdit.Fields[(int) Sport.Entities.Player.Fields.Student] = student.Id;
			entEdit.Fields[(int) Sport.Entities.Player.Fields.Team] = team.Id;
			entEdit.Fields[(int) Sport.Entities.Player.Fields.Number] = oPlayer.ShirtNumber;
			entEdit.Fields[(int) Sport.Entities.Player.Fields.Status] = (int) Sport.Types.PlayerStatusType.Confirmed;
			entEdit.Fields[(int) Sport.Entities.Player.Fields.RegisterDate] = DateTime.Now;
			entEdit.Fields[(int) Sport.Entities.Player.Fields.LastModified] = DateTime.Now;
			Sport.Data.EntityResult result = entEdit.Save();
			if (!result.Succeeded)
				return "כשלון בעת שמירת השחקן '"+oPlayer.ToString()+"' למאגר הנתונים: "+result.ResultCode.ToString();
			return "השחקן '"+oPlayer.ToString()+"' נוסף בהצלחה למאגר הנתונים המרכזי";
		}
		
		private void ApplyOfflineSymbol(int offlineSchoolID, string strSymbol)
		{
			Sport.Data.OfflineEntity[] arrStudents = 
				(Sport.Data.OfflineEntity[]) _data[(int) OfflineStep.Student];
			foreach (Sport.Entities.OfflineStudent oStudent in arrStudents)
			{
				if ((oStudent.OfflineSchool != null)&&
					(oStudent.OfflineSchool.OfflineID == offlineSchoolID))
				{
					oStudent.OfflineSchool.Symbol = strSymbol;
				}
			}
			
			Sport.Data.OfflineEntity[] arrTeams = 
				(Sport.Data.OfflineEntity[]) _data[(int) OfflineStep.Team];
			foreach (Sport.Entities.OfflineTeam oTeam in arrTeams)
			{
				if ((oTeam.OfflineSchool != null)&&
					(oTeam.OfflineSchool.OfflineID == offlineSchoolID))
				{
					oTeam.OfflineSchool.Symbol = strSymbol;
				}
			}
		}
		
		private void ApplyOfflineIdNumber(int offlineStudentID, string strIdNumber)
		{
			Sport.Data.OfflineEntity[] arrPlayers = 
				(Sport.Data.OfflineEntity[]) _data[(int) OfflineStep.Player];
			foreach (Sport.Entities.OfflinePlayer oPlayer in arrPlayers)
			{
				if ((oPlayer.OfflineStudent != null)&&
					(oPlayer.OfflineStudent.OfflineID == offlineStudentID))
				{
					oPlayer.OfflineStudent.IdNumber = strIdNumber;
				}
			}
		}
		
		private string[] ImportChampionshipTeams(Sport.Data.OfflineEntity[] arrTeams)
		{
			Hashtable tblChampTeams = new Hashtable();
			ArrayList arrLines = new ArrayList();
			foreach (Sport.Entities.OfflineTeam oTeam in arrTeams)
			{
				int categoryID = oTeam.ChampionshipCategory;
				if (tblChampTeams[categoryID] == null)
				{
					tblChampTeams[categoryID] = new ArrayList();
				}
				(tblChampTeams[categoryID] as ArrayList).Add(oTeam);
			}
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען קבוצות לתוך האליפות אנא המתן...", true);
			
			foreach (int champCategoryID in tblChampTeams.Keys)
			{
				ArrayList arrChampTeams = (ArrayList) tblChampTeams[champCategoryID];
				if (arrChampTeams.Count == 0)
					continue;
				Sport.Championships.Championship champ = null;
				try
				{
					champ = Sport.Championships.Championship.GetChampionship(champCategoryID);
				}
				catch {}
				if (champ == null)
				{
					arrLines.Add("זיהוי אליפות שגוי: "+champCategoryID);
					continue;
				}
				string strChampName = champ.Name;
				champ.Edit();
				if ((champ.Phases == null)||(champ.Phases.Count == 0))
				{
					arrLines.Add("לאליפות '"+strChampName+"' אין שלבים");
					continue;
				}
				arrLines.Add("מייבא "+Sport.Common.Tools.BuildOneOrMany(
					"קבוצה", "קבוצות", arrChampTeams.Count, false)+" לאליפות '"+
					strChampName+"'");
				string strAddedTeams = "";
				foreach (Sport.Entities.OfflineTeam oTeam in arrChampTeams)
				{
					string strTeamName = oTeam.ToString();
					if ((oTeam.Phase < 0)||(oTeam.Phase >= champ.Phases.Count))
					{
						arrLines.Add("לקבוצה '"+strTeamName+"' מוגדר שלב לא חוקי: "+oTeam.Phase);
						continue;
					}
					Sport.Championships.CompetitionPhase phase = 
						(Sport.Championships.CompetitionPhase) champ.Phases[oTeam.Phase];
					if ((phase.Groups == null)||(phase.Groups.Count == 0))
					{
						arrLines.Add("לשלב '"+phase.Name+"' אין בתים");
						continue;
					}
					if ((oTeam.Group < 0)||(oTeam.Group >= phase.Groups.Count))
					{
						arrLines.Add("לקבוצה '"+strTeamName+"' מוגדר בית לא חוקי: "+oTeam.Group);
						continue;
					}
					Sport.Championships.CompetitionGroup group = 
						phase.Groups[oTeam.Group];
					Sport.Entities.Team team = null;
					Sport.Entities.School school = null;
					if (oTeam.School != null)
						school = oTeam.School;
					else if (oTeam.OfflineSchool != null)
						school = Sport.Entities.School.FromSymbol(oTeam.OfflineSchool.Symbol);
					if (school == null)
						continue;
					team = Sport.Entities.Team.FromSchoolAndCategory(school.Id, champCategoryID);
					if (team == null)
					{
						arrLines.Add("לקבוצה '"+strTeamName+"' אין נתונים במאגר המרכזי");
						continue;
					}
					bool blnExists = false;
					foreach (Sport.Championships.CompetitionTeam compTeam in group.Teams)
					{
						if ((compTeam.TeamEntity != null)&&(compTeam.TeamEntity.Id == team.Id))
						{
							blnExists = true;
							break;
						}
					}
					if (blnExists)
					{
						arrLines.Add("הקבוצה '"+strTeamName+"' כבר רשומה באליפות זו");
						continue;
					}
					Sport.Championships.CompetitionTeam objCompTeam = 
						new Sport.Championships.CompetitionTeam(team);
					group.Teams.Add(objCompTeam);
					strAddedTeams += strTeamName+" שלב '"+phase.Name+"' בית '"+group.Name+"'"+"\n";
				}
				champ.Save();
				if (strAddedTeams.Length > 0)
				{
					arrLines.Add("הקבוצות הבאות נוספו בהצלחה לאליפות:");
					arrLines.Add(strAddedTeams);
				}
				else
				{
					arrLines.Add("לא נוספו קבוצות לאליפות זו");
				}
			}
			
			Sport.UI.Dialogs.WaitForm.HideWait();
			return (string[]) arrLines.ToArray(typeof(string));
		}
		
		private string[] ImportChampionshipPlayers(Sport.Data.OfflineEntity[] arrPlayers)
		{
			Hashtable tblChampPlayers = new Hashtable();
			ArrayList arrLines = new ArrayList();
			foreach (Sport.Entities.OfflinePlayer oPlayer in arrPlayers)
			{
				Sport.Entities.Team team = oPlayer.GetPlayerTeam();
				if (team == null)
					continue;
				int categoryID = team.Category.Id;
				if (tblChampPlayers[categoryID] == null)
					tblChampPlayers[categoryID] = new ArrayList();
				(tblChampPlayers[categoryID] as ArrayList).Add(oPlayer);
			}
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען מתמודדים לתוך האליפות אנא המתן...", true);
			
			foreach (int champCategoryID in tblChampPlayers.Keys)
			{
				ArrayList arrChampPlayers = (ArrayList) tblChampPlayers[champCategoryID];
				if (arrChampPlayers.Count == 0)
					continue;
				Sport.Championships.Championship champ = null;
				try
				{
					champ = Sport.Championships.Championship.GetChampionship(champCategoryID);
				}
				catch {}
				if (champ == null)
				{
					arrLines.Add("זיהוי אליפות שגוי: "+champCategoryID);
					continue;
				}
				string strChampName = champ.Name;
				champ.Edit();
				if ((champ.Phases == null)||(champ.Phases.Count == 0))
				{
					arrLines.Add("לאליפות '"+strChampName+"' אין שלבים");
					continue;
				}
				arrLines.Add("מייבא "+Sport.Common.Tools.BuildOneOrMany(
					"שחקן", "שחקנים", arrChampPlayers.Count, true)+" לאליפות '"+
					strChampName+"'");
				string strAddedPlayers = "";
				foreach (Sport.Entities.OfflinePlayer oPlayer in arrChampPlayers)
				{
					Sport.Entities.Team team = oPlayer.GetPlayerTeam();
					int phaseIndex = -1;
					int groupIndex = -1;
					int teamIndex = -1;
					string strPlayerName = oPlayer.ToString();
					for (int p=0; p<champ.Phases.Count; p++)
					{
						Sport.Championships.CompetitionPhase phase = 
							(Sport.Championships.CompetitionPhase) champ.Phases[p];
						if (phase.Groups == null)
							continue;
						for (int g=0; g<phase.Groups.Count; g++)
						{
							Sport.Championships.CompetitionGroup group = 
								phase.Groups[g];
							if (group.Teams == null)
								continue;
							foreach (Sport.Championships.CompetitionTeam compTeam 
										 in group.Teams)
							{
								if (compTeam.TeamEntity == null)
									continue;
								if (compTeam.TeamEntity.Id == team.Id)
								{
									phaseIndex = p;
									groupIndex = g;
									teamIndex = compTeam.Index;
									break;
								}
							}
							if (groupIndex >= 0)
								break;
						}
						if (phaseIndex >= 0)
							break;
					}
					if ((phaseIndex < 0)||(groupIndex < 0))
					{
						arrLines.Add("לשחקן '"+strPlayerName+"' לא נמצאו שלב ובית מתאימים");
						continue;
					}
					Sport.Championships.CompetitionGroup compGroup = 
						(champ.Phases[phaseIndex] as Sport.Championships.CompetitionPhase).Groups[groupIndex];
					if ((oPlayer.Competition < 0)||(oPlayer.Competition >= compGroup.Competitions.Count))
					{
						arrLines.Add("לשחקן '"+strPlayerName+"' מוגדרת תחרות לא חוקית: "+oPlayer.Competition);
						continue;
					}
					Sport.Championships.Competition competition = 
						compGroup.Competitions[oPlayer.Competition];
					Sport.Entities.Player player = oPlayer.GetPlayerEntity();
					if (player == null)
					{
						arrLines.Add("לשחקן '"+strPlayerName+"' אין נתונים במאגר המרכזי");
						continue;
					}
					arrLines.Add("מוסיף את השחקן '"+strPlayerName+"' לתחרות '"+competition.Name+"'");
					bool blnExists = false;
					foreach (Sport.Championships.Competitor competitor in competition.Competitors)
					{
						if ((competitor.Player != null)&&
							(competitor.Player.PlayerEntity != null)&&
							(competitor.Player.PlayerEntity.Id == player.Id))
						{
							blnExists = true;
							break;
						}
					}
					if (blnExists)
					{
						arrLines.Add("השחקן '"+strPlayerName+"' כבר רשום לתחרות זו");
						continue;
					}
					Sport.Championships.Competitor objCompetitor = 
						new Sport.Championships.Competitor(oPlayer.ShirtNumber);
					objCompetitor.Result = oPlayer.Result;
					objCompetitor.Score = oPlayer.Score;
					objCompetitor.Team = teamIndex;
					competition.Competitors.Add(objCompetitor);
					strAddedPlayers += strPlayerName+" שלב '"+
						champ.Phases[phaseIndex].Name+"' בית '"+
						champ.Phases[phaseIndex].Groups[groupIndex].Name+"'"+"\n";
				}
				champ.Save();
				if (strAddedPlayers.Length > 0)
				{
					arrLines.Add("השחקנים הבאים נוספו בהצלחה לאליפות:");
					arrLines.Add(strAddedPlayers);
				}
				else
				{
					arrLines.Add("לא נוספו שחקנים לאליפות זו");
				}
			}
			
			Sport.UI.Dialogs.WaitForm.HideWait();
			return (string[]) arrLines.ToArray(typeof(string));
		}
		
		private void DeleteOfflineData()
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("מוחק נתונים לא מקוונים אנא המתן...", true);
			Sport.UI.Dialogs.WaitForm.SetProgress(0);
			double offlineProgress = (((double) 100)/((double) _data.Count));
			double curProgress = 0;
			foreach (Sport.Data.OfflineEntity[] arrEntities in _data)
			{
				double entityProgress = offlineProgress;
				if (arrEntities.Length > 0)
					entityProgress = (offlineProgress/((double) arrEntities.Length));
				foreach (Sport.Data.OfflineEntity oEntity in arrEntities)
				{
					oEntity.Delete();
					curProgress += entityProgress;
					Sport.UI.Dialogs.WaitForm.SetProgress((int) curProgress);
				}
				if (arrEntities.Length == 0)
				{
					curProgress += entityProgress;
					Sport.UI.Dialogs.WaitForm.SetProgress((int) curProgress);
				}
			}
			Sport.UI.Dialogs.WaitForm.SetProgress(100);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
	}
}
