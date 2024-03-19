using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;
using Sport.Championships;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for ExportDialog.
	/// </summary>
	public class ExportDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lbRegions;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListBox lbSports;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.ListBox lbChamps;
		private System.Windows.Forms.Button btnExport;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Sport.Common.IniFile _iniFile=null;

		public ExportDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_iniFile = new IniFile(MainForm.IniFileName);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ExportDialog));
			this.lbRegions = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lbSports = new System.Windows.Forms.ListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.lbChamps = new System.Windows.Forms.ListBox();
			this.btnExport = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// lbRegions
			// 
			this.lbRegions.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbRegions.ItemHeight = 14;
			this.lbRegions.Location = new System.Drawing.Point(8, 24);
			this.lbRegions.Name = "lbRegions";
			this.lbRegions.Size = new System.Drawing.Size(136, 102);
			this.lbRegions.TabIndex = 0;
			this.lbRegions.SelectedIndexChanged += new System.EventHandler(this.lbRegions_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lbRegions);
			this.groupBox1.Location = new System.Drawing.Point(320, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(148, 135);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "מחוז";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.lbSports);
			this.groupBox2.Location = new System.Drawing.Point(320, 152);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(148, 135);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "ענף ספורט";
			// 
			// lbSports
			// 
			this.lbSports.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSports.ItemHeight = 14;
			this.lbSports.Location = new System.Drawing.Point(8, 24);
			this.lbSports.Name = "lbSports";
			this.lbSports.Size = new System.Drawing.Size(136, 102);
			this.lbSports.TabIndex = 0;
			this.lbSports.SelectedIndexChanged += new System.EventHandler(this.lbRegions_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.btnExport);
			this.groupBox3.Controls.Add(this.lbChamps);
			this.groupBox3.Location = new System.Drawing.Point(8, 8);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(304, 279);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "אליפויות";
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.Blue;
			this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnClose.Location = new System.Drawing.Point(184, 304);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(96, 23);
			this.btnClose.TabIndex = 4;
			this.btnClose.Text = "אישור";
			// 
			// lbChamps
			// 
			this.lbChamps.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbChamps.ForeColor = System.Drawing.Color.Blue;
			this.lbChamps.ItemHeight = 14;
			this.lbChamps.Items.AddRange(new object[] {
														  "אליפות אתלטיקה קלה גילה - בדיקה"});
			this.lbChamps.Location = new System.Drawing.Point(8, 24);
			this.lbChamps.Name = "lbChamps";
			this.lbChamps.Size = new System.Drawing.Size(288, 214);
			this.lbChamps.TabIndex = 1;
			this.lbChamps.SelectedIndexChanged += new System.EventHandler(this.lbChamps_SelectedIndexChanged);
			// 
			// btnExport
			// 
			this.btnExport.Enabled = false;
			this.btnExport.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnExport.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.Image")));
			this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnExport.Location = new System.Drawing.Point(56, 240);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(184, 24);
			this.btnExport.TabIndex = 2;
			this.btnExport.Text = "העבר אליפות אל שרת מרכזי";
			this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// ExportDialog
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(474, 335);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExportDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ייצוא נתונים";
			this.Load += new System.EventHandler(this.ExportDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ExportDialog_Load(object sender, System.EventArgs e)
		{
			FillRegions();
			FillSports();
			lbChamps.Items.Clear();
			
			//read ini file:
			string strLastSport=_iniFile.ReadValue("ExportSettings", "LastSport");
			string strLastRegion=_iniFile.ReadValue("ExportSettings", "LastRegion");
			string strLastChamp=_iniFile.ReadValue("ExportSettings", "LastChampionship");
			if ((strLastRegion != null)&&(strLastRegion.Length > 0))
				AutoSelect(lbRegions, Tools.CIntDef(strLastRegion, -1));
			if ((strLastSport != null)&&(strLastSport.Length > 0))
				AutoSelect(lbSports, Tools.CIntDef(strLastSport, -1));
			if ((strLastChamp != null)&&(strLastChamp.Length > 0))
				AutoSelect(lbChamps, Tools.CIntDef(strLastChamp, -1));
		}

		private void AutoSelect(ListBox combo, object value)
		{
			for (int i=0; i<combo.Items.Count; i++)
			{
				if ((combo.Items[i] as ListItem).Value.Equals(value))
				{
					combo.SelectedIndex = i;
					break;
				}
			}
		}

		private void FillRegions()
		{
			lbRegions.Items.Clear();
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען רשימת מחוזות אנא המתן...");
			SimpleData[] regions=LocalDatabaseManager.LocalDatabase.GetRegions();
			Sport.UI.Dialogs.WaitForm.HideWait();
			foreach (SimpleData region in regions)
			{
				lbRegions.Items.Add(new ListItem(
					region.Name, region.ID));
			}
		}

		private void FillSports()
		{
			lbSports.Items.Clear();
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען רשימת ענפי ספורט אנא המתן...");
			SimpleData[] sports=LocalDatabaseManager.LocalDatabase.GetSports();
			Sport.UI.Dialogs.WaitForm.HideWait();
			foreach (SimpleData sport in sports)
			{
				lbSports.Items.Add(new ListItem(
					sport.Name, sport.ID));
			}
		}

		private Match[] GetServerMatches(MatchChampionship champ)
		{
			ArrayList arrMatches=new ArrayList();
			foreach (MatchPhase phase in champ.Phases)
			{
				foreach (MatchGroup group in phase.Groups)
				{
					foreach (Round round in group.Rounds)
					{
						foreach (Cycle cycle in round.Cycles)
						{
							foreach (Match match in cycle.Matches)
							{
								arrMatches.Add(match);
							}
						}
					}
				}
			}
			return (Match[]) arrMatches.ToArray(typeof(Match));
		}

		private MatchData[] GetLocalMatches(CategoryData category)
		{
			ArrayList arrMatches=new ArrayList();
			foreach (PhaseData phase in category.Phases)
			{
				foreach (GroupData group in phase.Groups)
				{
					foreach (RoundData round in group.Rounds)
					{
						foreach (MatchData match in round.Matches)
						{
							match.PhaseIndex = phase.PhaseIndex;
							match.GroupIndex = group.GroupIndex;
							match.Round = round;
							arrMatches.Add(match);
						}
					}
				}
			}
			return (MatchData[]) arrMatches.ToArray(typeof(MatchData));
		}

		/// <summary>
		/// looks for the proper local match according to server match data.
		/// </summary>
		private MatchData FindLocalMatch(MatchData[] matches, Match serverMatch)
		{
			foreach (MatchData localMatch in matches)
			{
				if ((localMatch.PhaseIndex == serverMatch.Cycle.Round.Group.Phase.Index)&&
					(localMatch.GroupIndex == serverMatch.Cycle.Round.Group.Index)&&
					(localMatch.Round.RoundIndex == serverMatch.Cycle.Round.Index)&&
					(localMatch.MatchIndex == serverMatch.Index))
				{
					return localMatch;
				}
			}
			return null;
		}

		private void lbRegions_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ((lbRegions.SelectedIndex < 0)||(lbSports.SelectedIndex < 0))
				return;

			int regionID=(int) (lbRegions.SelectedItem as ListItem).Value;
			int sportID=(int) (lbSports.SelectedItem as ListItem).Value;
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען רשימת אליפויות אנא המתן...");
			SimpleData[] champs=
				LocalDatabaseManager.LocalDatabase.GetChampionshipsData(regionID, sportID);
			lbChamps.Items.Clear();
			foreach (SimpleData champ in champs)
			{
				lbChamps.Items.Add(new ListItem(
					champ.Name, champ.ID));
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
			btnExport.Enabled = false;
		}

		private void lbChamps_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnExport.Enabled = (lbChamps.SelectedIndex >= 0);
		}

		private void btnExport_Click(object sender, System.EventArgs e)
		{
			//verify we have selected championship:
			if (lbChamps.SelectedIndex < 0)
				return;

			//get championship and sport basic data:
			int champID=(int) (lbChamps.SelectedItem as ListItem).Value;
			Sport.Types.SportType sportType=Sport.Types.SportType.None;
			Sport.UI.Dialogs.WaitForm.ShowWait("בודק סוג ענף ספורט אנא המתן...");
			SportData sport=
				LocalDatabaseManager.LocalDatabase.GetChampionshipSport(champID);
			if (sport.Type >= 0)
				sportType = (Sport.Types.SportType) sport.Type;
			Sport.UI.Dialogs.WaitForm.HideWait();

			//get full championship data:
			Cursor oldCursor=Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני אליפות אנא המתן...");
			ChampionshipData champ=
				LocalDatabaseManager.LocalDatabase.GetFullChampionship(champID);
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			//save the results:
			Sport.UI.Dialogs.WaitForm.ShowWait("שומר תוצאות אליפות, אנא המתן...");
			int failCount=0;
			string strError="";
			switch (sportType)
			{
				case Sport.Types.SportType.Match:
					int matchCount=0;
					foreach (CategoryData category in champ.Categories)
					{
						MatchChampionship matchChamp=(MatchChampionship)
							Championship.GetChampionship(category.ID);
						Match[] serverMatches=GetServerMatches(matchChamp);
						MatchData[] localMatches=GetLocalMatches(category);
						foreach (Match match in serverMatches)
						{
							MatchData localMatch=FindLocalMatch(localMatches, match);
							if (localMatch != null)
							{
								Status oldStatus=match.Cycle.Round.Group.Phase.Status;
								//match.Cycle.Round.Group.Phase.Status = Status.Started;
								MatchOutcome outcome=MatchOutcome.None;
								if (localMatch.Result >= 0)
									outcome = (MatchOutcome) localMatch.Result;
								double scoreA=localMatch.TeamA_Score;
								double scoreB=localMatch.TeamB_Score;
								string partsResult=Tools.CStrDef(
									localMatch.PartsResult, "");
								if (match.SetResult(
									outcome, scoreA, scoreB, partsResult) == false)
								{
									failCount++;
								}
								matchCount++;
								//match.Cycle.Round.Group.Phase.Status = oldStatus;
							}
						}
					}
					
					if (matchCount == 0)
					{
						strError = "לא נמצאו משחקים מתאימים באליפות זו, תוצאות לא עודכנו";
					}
					else
					{
						if (failCount > 0)
							strError = "כשלון בשמירת נתוני "+failCount+" משחקים";
					}
					break;
				case Sport.Types.SportType.Competition:
					ChampionshipServices.ChampionshipService champService=
						new ChampionshipServices.ChampionshipService();
					champService.CookieContainer = Sport.Core.Session.Cookies;
					int competitorsCount=0;
					foreach (CategoryData category in champ.Categories)
					{
						foreach (PhaseData phase in category.Phases)
						{
							foreach (GroupData group in phase.Groups)
							{
								foreach (CompetitionData competition in group.Competitions)
								{
									foreach (CompetitorData competitor in 
										competition.Competitors)
									{
										if (!champService.SetCompetitorResult(
											category.ID, phase.PhaseIndex, 
											group.GroupIndex, 
											competition.CompetitionIndex, 
											competitor.CompetitorIndex, 
											competitor.Result,
											competitor.Score))
										{
											failCount++;
										}
										competitorsCount++;
									} //end loop over competitors
								} //end loop over competitions
							} //end loop over groups
						} //end loop over phases
					} //end loop over categories
					
					if (competitorsCount == 0)
					{
						strError = "לא נמצאו מתחרים באליפות זו, תוצאות לא עודכנו";
					}
					else
					{
						if (failCount > 0)
							strError = "כשלון בשמירת נתוני "+failCount+" מתמודדים";
					}
					break;
				default:
					strError = "סוג ספורט לא מוגדר עבור אליפות זו";
					break;
			}
			Sport.UI.Dialogs.WaitForm.HideWait();

			Cursor.Current = oldCursor;
			if (strError.Length > 0)
			{
				Sport.UI.MessageBox.Error(strError, "ייצוא נתונים");
			}
			else
			{
				Sport.UI.MessageBox.Success("תוצאות אליפות עודכנו בהצלחה", "ייצוא נתונים");
			}
			
			//store last settings:
			int sportID=(int) (lbSports.SelectedItem as ListItem).Value;
			int regionID=(int) (lbRegions.SelectedItem as ListItem).Value;
			_iniFile.WriteValue("ExportSettings", "LastSport", sportID.ToString());
			_iniFile.WriteValue("ExportSettings", "LastRegion", regionID.ToString());
			_iniFile.WriteValue("ExportSettings", "LastChampionship", champID.ToString());
			
			//add user action:
			try
			{
				SessionServices.SessionService _service=
					new SessionServices.SessionService();
				string description="Championship: "+champID.ToString();
				_service.AddUserAction_2(Core.UserManager.CurrentUser.ID, 
					SessionServices.Action.Field_Export, description, Sport.Core.Data.Field_CurrentVersion);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to add Field Export action: "+ex.Message);
			}
		}
	}
}
