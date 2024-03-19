using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for ChooseCompetitionReportDialog.
	/// </summary>
	public class ChooseCompetitionReportDialog : System.Windows.Forms.Form, ICommandTarget
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		private Sport.Championships.Competition _competition = null;
		private Sport.Championships.CompetitionGroup _group = null;
		private Sport.Championships.CompetitionTeam _team = null;
		private Sport.Championships.Team[] _multipleTeams = null;
		private Sport.Championships.CompetitionPhase _phase = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel pnlReportChoices;
		private Sport.UI.Controls.ThemeButton btnOK;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private Sport.UI.Controls.ThemeButton btnCancel;
		private  Sport.Championships.Competitor _competitor=null;
		
		private Documents.CompetitionReportType _reportType=Documents.CompetitionReportType.Undefined;
		public Documents.CompetitionReportType SelectedReport
		{
			get { return _reportType; }
		}
		
		public ChooseCompetitionReportDialog(
			Sport.Championships.Competition competition, 
			Sport.Championships.CompetitionGroup group,
			Sport.Championships.Competitor competitor, 
			Sport.Championships.CompetitionTeam team, 
			Sport.Championships.CompetitionPhase phase, 
			Sport.Championships.Team[] multipleTeams)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_competition = competition;
			_group = group;
			_competitor = competitor;
			_team = team;
			_multipleTeams = multipleTeams;
			_phase = phase;

			this.Load += new EventHandler(ChooseCompetitionReportDialog_Load);

			BuildChoiceRange();
		}
		
		private void BuildChoiceRange()
		{
			Array arrReportTypes = Enum.GetValues(typeof(Documents.CompetitionReportType));
			bool blnGotCompetition = (_competition != null);
			bool blnGotGroup = (_group != null);
			bool blnGotCompetitor = (_competitor != null);
			bool blnGotTeam = (_team != null);
			bool blnGotMultipleTeams = (_multipleTeams != null && _multipleTeams.Length > 1);
			bool blnGotPhase = (_phase != null);
			bool blnGotResults;
			bool blnMultiCompetitions = false;
			int competitorsWithPosition = 0;
			
			if (_competition != null)
			{
				if (_competition.Competitors != null)
				{
					foreach (Sport.Championships.Competitor competitor in _competition.Competitors)
						if (competitor.ResultPosition >= 0)
							competitorsWithPosition++;
				}
			}
			
			if (_group != null)
			{
				foreach (Sport.Championships.Competition competition in 
					_group.Competitions)
				{
					if (competition.SportField.SportFieldType.CompetitionType == Sport.Types.CompetitionType.MultiCompetition)
					{
						blnMultiCompetitions = true;
						break;
					}
				}
			}
			
			blnGotResults = LookForResults();
			
			while (pnlReportChoices.Controls.Count > 0)
			{
				Control control=pnlReportChoices.Controls[0];
				pnlReportChoices.Controls.Remove(control);
			}
			
			for (int i=arrReportTypes.Length-1; i>=0; i--)
			{
				Documents.CompetitionReportType curType=
					(Documents.CompetitionReportType) arrReportTypes.GetValue(i);
				if (curType == Documents.CompetitionReportType.Undefined)
					continue;
				ReportTypeSelection curSelection = new ReportTypeSelection(curType, 
					blnGotCompetition, blnGotGroup, blnGotCompetitor, 
					blnGotTeam, blnGotMultipleTeams, blnGotPhase, 
					competitorsWithPosition, 
					blnGotResults, 
					blnMultiCompetitions);
				curSelection.Dock = DockStyle.Top;
				curSelection.Height = 25;
				curSelection.SelectionChanged += new EventHandler(ReportSelectionChanged);
				pnlReportChoices.Controls.Add(curSelection);
			}
		}
		
		private bool LookForResults()
		{
			ArrayList arrTeamsToCheck = new ArrayList();

			if (_group != null && _team != null)
				arrTeamsToCheck.Add(_team);

			if (_multipleTeams != null && _multipleTeams.Length > 1)
			{
				for (int i = 0; i < _multipleTeams.Length; i++)
				{
					arrTeamsToCheck.Add(_multipleTeams[i] as Sport.Championships.CompetitionTeam);
				}
			}

			bool gotResults = false;

			for (int i = 0; i < arrTeamsToCheck.Count; i++)
			{
				if (gotResults)
					break;
				Sport.Championships.CompetitionTeam team = (Sport.Championships.CompetitionTeam)arrTeamsToCheck[i];
				Sport.Championships.CompetitionGroup group = team.Group;
				foreach (Sport.Championships.Competition competition in group.Competitions)
				{
					if (gotResults)
						break;
					foreach (Sport.Championships.Competitor competitor in competition.Competitors)
					{
						if ((competitor.Player != null)&&
							(competitor.Player.CompetitionTeam == team)&&
							(competitor.Result > 0))
						{
							gotResults = true;
							break;
						}
					}
				}
			}
			
			return gotResults;
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
			this.pnlReportChoices = new System.Windows.Forms.Panel();
			this.btnOK = new Sport.UI.Controls.ThemeButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnCancel = new Sport.UI.Controls.ThemeButton();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(376, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(125, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "בחר דו\"ח מבוקש:";
			// 
			// pnlReportChoices
			// 
			this.pnlReportChoices.Location = new System.Drawing.Point(8, 32);
			this.pnlReportChoices.Name = "pnlReportChoices";
			this.pnlReportChoices.Size = new System.Drawing.Size(496, 304);
			this.pnlReportChoices.TabIndex = 1;
			// 
			// btnOK
			// 
			this.btnOK.Alignment = System.Drawing.StringAlignment.Center;
			this.btnOK.AutoSize = false;
			this.btnOK.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnOK.Enabled = false;
			this.btnOK.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.btnOK.Hue = 300F;
			this.btnOK.Image = null;
			this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnOK.ImageList = null;
			this.btnOK.ImageSize = new System.Drawing.Size(0, 0);
			this.btnOK.Location = new System.Drawing.Point(439, 0);
			this.btnOK.Name = "btnOK";
			this.btnOK.Saturation = 0.1F;
			this.btnOK.Size = new System.Drawing.Size(75, 32);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "אישור";
			this.btnOK.Transparent = System.Drawing.Color.Black;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.btnOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 352);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(514, 32);
			this.panel1.TabIndex = 3;
			// 
			// btnCancel
			// 
			this.btnCancel.Alignment = System.Drawing.StringAlignment.Center;
			this.btnCancel.AutoSize = false;
			this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.btnCancel.Hue = 300F;
			this.btnCancel.Image = null;
			this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.ImageList = null;
			this.btnCancel.ImageSize = new System.Drawing.Size(0, 0);
			this.btnCancel.Location = new System.Drawing.Point(319, 0);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Saturation = 0.1F;
			this.btnCancel.Size = new System.Drawing.Size(80, 32);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.Transparent = System.Drawing.Color.Black;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Right;
			this.label2.Location = new System.Drawing.Point(399, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 32);
			this.label2.TabIndex = 3;
			// 
			// ChooseCompetitionReportDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.ClientSize = new System.Drawing.Size(514, 384);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pnlReportChoices);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChooseCompetitionReportDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "בחר דו\"ח";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (!btnOK.Enabled)
				return;
			this.DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}
		
		public static string CompetitionReportTypeToString(
			Documents.CompetitionReportType type)
		{
			switch (type)
			{
				case Documents.CompetitionReportType.CompetitionCompetitorsReport:
					return "דו\"ח דירוג אישי למקצוע";
				case Documents.CompetitionReportType.CompetitorVoucher:
					return "תעודת הצטיינות אישית";
				case Documents.CompetitionReportType.GroupTeamsReport:
					return "דו\"ח דירוג קבוצתי לתחרות";
				case Documents.CompetitionReportType.TeamVoucher_School:
					return "תעודה קבוצתית לבית ספר";
				case Documents.CompetitionReportType.TeamVoucher_Student:
					return "תעודה קבוצתית לתלמיד";
				case Documents.CompetitionReportType.RefereeReport:
					return "טופס שיפוט";
				case Documents.CompetitionReportType.TeamFullReport:
					return "סיכום קבוצתי מפורט";
				case Documents.CompetitionReportType.MultiCompetitionReport:
					return "דו\"ח קרב רב";
				case Documents.CompetitionReportType.ClubCompetitionsReport:
					return "דו\"ח דירוג מועדונים";
				case Documents.CompetitionReportType.TeamCompetitorsReport:
					return "דו\"ח שיבוץ למקצוע";
			}
			return "";
		}
		
		private void ReportSelectionChanged(object sender, EventArgs e)
		{
			ReportTypeSelection selection=(ReportTypeSelection) sender;
			if (selection.Selected)
			{
				foreach (Control control in pnlReportChoices.Controls)
				{
					if (control is ReportTypeSelection)
					{
						if (!control.Equals(selection))
							(control as ReportTypeSelection).Selected = false;
					}
				}
				btnOK.Enabled = true;
				_reportType = selection.ReportType;
			}
		}
		
		private class ReportTypeSelection : System.Windows.Forms.Panel
		{
			private RadioButton _radio=null;
			private Documents.CompetitionReportType _type=Documents.CompetitionReportType.Undefined;
			public event EventHandler SelectionChanged=null;
			
			public Documents.CompetitionReportType ReportType
			{
				get { return _type; }
			}
			
			public ReportTypeSelection(Documents.CompetitionReportType type, 
				bool blnGotCompetition, bool blnGotGroup, bool blnGotCompetitor, 
				bool blnGotTeam, bool blnGotMultipleTeams, bool blnGotPhase, int competitorsWithPosition, 
				bool blnGotResults, bool blnMultiCompetitions)
			{
				_type = type;
				
				string strCaption=
					ChooseCompetitionReportDialog.CompetitionReportTypeToString(type);
				
				bool blnEnabled = false;
				bool blnMargins=((type == Documents.CompetitionReportType.CompetitorVoucher)||
							(type == Documents.CompetitionReportType.TeamVoucher_School)||
							(type == Documents.CompetitionReportType.TeamVoucher_Student));
				string strErrorMsg="בבנייה";
				switch (type)
				{
					case Documents.CompetitionReportType.CompetitionCompetitorsReport:
					case Documents.CompetitionReportType.RefereeReport:
					case Documents.CompetitionReportType.CompetitorVoucher:
						blnEnabled = blnGotCompetition;
						strErrorMsg = "לא נבחרה תחרות";
						if ((blnEnabled == true)&&
							((type == Documents.CompetitionReportType.CompetitionCompetitorsReport)||
							(type == Documents.CompetitionReportType.CompetitorVoucher))&&
							(competitorsWithPosition <= 0))
						{
							blnEnabled = false;
							strErrorMsg = "אין דירוג משתתפים";
						}
						break;
					case Documents.CompetitionReportType.TeamVoucher_School:
					case Documents.CompetitionReportType.TeamVoucher_Student:
					case Documents.CompetitionReportType.MultiCompetitionReport:
					case Documents.CompetitionReportType.GroupTeamsReport:
						blnEnabled = blnGotGroup;
						strErrorMsg = "לא נבחר בית";
						if ((blnEnabled)&&(type == Documents.CompetitionReportType.MultiCompetitionReport))
						{
							blnEnabled = blnMultiCompetitions;
							strErrorMsg = "אין תחרויות קרב רב";
						}
						break;
					case Documents.CompetitionReportType.TeamCompetitorsReport:
						blnEnabled = blnGotTeam;
						strErrorMsg = "לא נבחרה קבוצה";
						break;
					case Documents.CompetitionReportType.TeamFullReport:
						blnEnabled = (blnGotTeam || blnGotMultipleTeams);
						strErrorMsg = "לא נבחרו קבוצות";
						if (blnEnabled == true)
						{
							blnEnabled = blnGotResults;
							strErrorMsg = (blnGotMultipleTeams) ? "אין תוצאות עבור הקבוצות שנבחרו" : "אין תוצאות עבור הקבוצה שנבחרה";
						}
						break;
					case Documents.CompetitionReportType.ClubCompetitionsReport:
						blnEnabled = blnGotPhase;
						strErrorMsg = "לא נבחר שלב";
						break;
				}
				
				this.RightToLeft = RightToLeft.Yes;
				
				if (blnMargins)
				{
					Button button = new Button();
					button.Dock = DockStyle.Right;
					button.Width = 65;
					button.Height = this.Height;
					button.Font = new Font(this.Font.FontFamily.Name, this.Font.Size);
					button.Text = "שוליים";
					button.TextAlign = ContentAlignment.MiddleCenter;
					button.Click += new EventHandler(MarginsClick);
					this.Controls.Add(button);
					
					Label label = new Label();
					label.Dock = DockStyle.Right;
					label.AutoSize = false;
					label.Width = 20;
					this.Controls.Add(label);
				}
				
				if (!blnEnabled)
				{
					Label label = new Label();
					label.Dock = DockStyle.Right;
					label.AutoSize = true;
					label.Text = "("+strErrorMsg+")";
					label.TextAlign = ContentAlignment.MiddleLeft;
					this.Controls.Add(label);
					
					label = new Label();
					label.Dock = DockStyle.Right;
					label.AutoSize = false;
					label.Width = 15;
					this.Controls.Add(label);
				}
				
				_radio = new RadioButton();
				_radio.Text = strCaption;
				_radio.Name = "rad_"+((int) type).ToString();
				_radio.Enabled = blnEnabled;
				_radio.Height = this.Height;
				_radio.Dock = DockStyle.Right;
				_radio.Width = 250; //MeasureText(strCaption)+10;
				_radio.TextAlign = ContentAlignment.MiddleLeft;
				_radio.CheckedChanged += new EventHandler(Checked_Changed);
				this.Controls.Add(_radio);
			}
			
			public int MeasureText(string text)
			{
				Label label=new Label();
				label.AutoSize = true;
				label.Text = text;
				this.Controls.Add(label);
				int result=label.Width;
				this.Controls.Remove(label);
				return result;
			}
			
			public bool Selected
			{
				get
				{
					if (!_radio.Enabled)
						return false;
					return _radio.Checked;
				}
				set
				{
					if (_radio.Enabled)
						_radio.Checked = value;
				}
			}

			private void Checked_Changed(object sender, EventArgs e)
			{
				if (_radio.Enabled == false)
					return;
				
				if (SelectionChanged != null)
					SelectionChanged(this, e);
			}

			private void MarginsClick(object sender, EventArgs e)
			{
				Core.Tools.InputMargins(_type.ToString()+"_Margins", 500, 100);
			}
		}

		private void ChooseCompetitionReportDialog_KeyDown(object sender, KeyEventArgs e)
		{
			if (KeyListener.HandleEvent(this, e))
				e.Handled = true;
		}

		#region ICommandTarget Members
		public virtual bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.Cancel)
			{
				this.DialogResult = DialogResult.Cancel;
				return true;
			}
			
			return false;
		}
		#endregion

		private void AttachKeydownDeep(Control parent)
		{
			parent.KeyDown += new KeyEventHandler(ChooseCompetitionReportDialog_KeyDown);
			for (int i = 0; i < parent.Controls.Count; i++)
				AttachKeydownDeep(parent.Controls[i]);
		}

		private void ChooseCompetitionReportDialog_Load(object sender, EventArgs e)
		{
			AttachKeydownDeep(this);
		}
	}
}
