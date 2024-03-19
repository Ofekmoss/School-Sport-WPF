using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;
using Sport.Core;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for ImportDialog.
	/// </summary>
	public class ImportDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbRegions;
		private System.Windows.Forms.ComboBox cbSports;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox lbServerChamps;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListBox lbLocalChamps;
		private System.Windows.Forms.Button btnImport;
		private System.ComponentModel.IContainer components;
		private DataServices.DataService _dataService=null;
		private ArrayList _importedCourtTypes=null;
		private ArrayList _importedCourts=null;
		private ArrayList _importedFacilities=null;
		private ArrayList _importedPlayers=null;
		private ArrayList _importedTeams=null;
		private ArrayList _importedSportFields=null;
		private ArrayList _importedRules=null;
		private Sport.Common.IniFile _iniFile=null;
		
		public ImportDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_dataService = new DataServices.DataService();
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ImportDialog));
			this.btnClose = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.cbRegions = new System.Windows.Forms.ComboBox();
			this.cbSports = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnImport = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.lbLocalChamps = new System.Windows.Forms.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.lbServerChamps = new System.Windows.Forms.ListBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.Blue;
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnClose.ImageIndex = 0;
			this.btnClose.ImageList = this.imageList1;
			this.btnClose.Location = new System.Drawing.Point(224, 248);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(96, 23);
			this.btnClose.TabIndex = 0;
			this.btnClose.Text = "אישור";
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(536, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 18);
			this.label1.TabIndex = 1;
			this.label1.Text = "מחוז:";
			// 
			// cbRegions
			// 
			this.cbRegions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRegions.Location = new System.Drawing.Point(384, 15);
			this.cbRegions.Name = "cbRegions";
			this.cbRegions.Size = new System.Drawing.Size(144, 22);
			this.cbRegions.TabIndex = 2;
			this.cbRegions.SelectedIndexChanged += new System.EventHandler(this.cbRegions_SelectedIndexChanged);
			// 
			// cbSports
			// 
			this.cbSports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSports.Location = new System.Drawing.Point(8, 15);
			this.cbSports.Name = "cbSports";
			this.cbSports.Size = new System.Drawing.Size(168, 22);
			this.cbSports.TabIndex = 4;
			this.cbSports.SelectedIndexChanged += new System.EventHandler(this.cbRegions_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(184, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 18);
			this.label2.TabIndex = 3;
			this.label2.Text = "ענף ספורט:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnImport);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.lbLocalChamps);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.lbServerChamps);
			this.groupBox1.Location = new System.Drawing.Point(8, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(560, 195);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "אליפויות";
			this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
			// 
			// btnImport
			// 
			this.btnImport.Enabled = false;
			this.btnImport.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnImport.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(64)), ((System.Byte)(64)), ((System.Byte)(64)));
			this.btnImport.Image = ((System.Drawing.Image)(resources.GetObject("btnImport.Image")));
			this.btnImport.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnImport.Location = new System.Drawing.Point(224, 56);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(112, 23);
			this.btnImport.TabIndex = 5;
			this.btnImport.Text = "העבר אליפות";
			this.btnImport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(0, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(208, 18);
			this.label4.TabIndex = 4;
			this.label4.Text = "אליפויות קיימות";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbLocalChamps
			// 
			this.lbLocalChamps.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbLocalChamps.ForeColor = System.Drawing.Color.Blue;
			this.lbLocalChamps.Location = new System.Drawing.Point(6, 42);
			this.lbLocalChamps.Name = "lbLocalChamps";
			this.lbLocalChamps.Size = new System.Drawing.Size(210, 147);
			this.lbLocalChamps.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label3.Location = new System.Drawing.Point(344, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(208, 18);
			this.label3.TabIndex = 2;
			this.label3.Text = "אליפויות זמינות";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbServerChamps
			// 
			this.lbServerChamps.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbServerChamps.ForeColor = System.Drawing.Color.Blue;
			this.lbServerChamps.Location = new System.Drawing.Point(342, 42);
			this.lbServerChamps.Name = "lbServerChamps";
			this.lbServerChamps.Size = new System.Drawing.Size(210, 147);
			this.lbServerChamps.TabIndex = 0;
			this.lbServerChamps.SelectedIndexChanged += new System.EventHandler(this.lbServerChamps_SelectedIndexChanged);
			// 
			// ImportDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(578, 279);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cbSports);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cbRegions);
			this.Controls.Add(this.btnClose);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ImportDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ייבוא נתונים";
			this.Load += new System.EventHandler(this.ImportDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ImportDialog_Load(object sender, System.EventArgs e)
		{
			//fill the regions and sports:
			FillRegions();
			FillSports();

			//auto select region of current logged in user:
			for (int i=0; i<cbRegions.Items.Count; i++)
			{
				if ((cbRegions.Items[i] as ListItem).Text == 
					Core.UserManager.CurrentUser.Region)
				{
					cbRegions.SelectedIndex = i;
					break;
				}
			}

			//read ini file:
			string strLastSport=_iniFile.ReadValue("Settings", "LastSport");
			string strLastRegion=_iniFile.ReadValue("Settings", "LastRegion");
			if ((strLastRegion != null)&&(strLastRegion.Length > 0))
				AutoSelect(cbRegions, Tools.CIntDef(strLastRegion, -1));
			if ((strLastSport != null)&&(strLastSport.Length > 0))
				AutoSelect(cbSports, Tools.CIntDef(strLastSport, -1));
		}

		private void AutoSelect(ComboBox combo, object value)
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

		private void groupBox1_Enter(object sender, System.EventArgs e)
		{
		
		}

		private void FillRegions()
		{
			cbRegions.Items.Clear();
			
			Sport.UI.Dialogs.WaitForm.ShowWait("אנא המתן טוען מחוזות...");
			DataServices.SimpleData[] regions=_dataService.GetRegionsData();
			foreach (DataServices.SimpleData region in regions)
			{
				cbRegions.Items.Add(
					new ListItem(region.Name, region.ID));
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		
		private void FillSports()
		{
			cbSports.Items.Clear();
			
			Sport.UI.Dialogs.WaitForm.ShowWait("אנא המתן טוען ענפי ספורט...");
			DataServices.SimpleData[] sports=_dataService.GetSportsData();
			foreach (DataServices.SimpleData sport in sports)
			{
				cbSports.Items.Add(
					new ListItem(sport.Name, sport.ID));
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		/// <summary>
		/// fills the championships based on selected region and sport.
		/// </summary>
		private void FillChampionships()
		{
			int selRegion=cbRegions.SelectedIndex;
			int selSport=cbSports.SelectedIndex;
			
			if ((selRegion < 0)||(selSport < 0))
				return;
			
			int regionID=(int) (cbRegions.Items[selRegion] as ListItem).Value;
			int sportID=(int) (cbSports.Items[selSport] as ListItem).Value;

			//clear items:
			lbServerChamps.Items.Clear();
			lbLocalChamps.Items.Clear();
			
			Sport.UI.Dialogs.WaitForm.ShowWait("אנא המתן טוען אליפויות...");

			//remote data:
			DataServices.SimpleData[] championships=_dataService.GetChampionshipsData(regionID, sportID);
			foreach (DataServices.SimpleData championship in championships)
			{
				lbServerChamps.Items.Add(
					new ListItem(championship.Name, championship.ID));
			}
			
			//local data:
			Sport.Common.SimpleData[] localChamps=
				LocalDatabaseManager.LocalDatabase.GetChampionshipsData(regionID, sportID);
			foreach (Sport.Common.SimpleData championship in localChamps)
			{
				lbLocalChamps.Items.Add(
					new ListItem(championship.Name, championship.ID));
			}

			Sport.UI.Dialogs.WaitForm.HideWait();
			
			btnImport.Enabled = false;
		}

		private void cbRegions_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if ((cbRegions.SelectedIndex >= 0)&&(cbSports.SelectedIndex >= 0))
			{
				FillChampionships();
			}
			else
			{
				lbServerChamps.Items.Clear();
				lbLocalChamps.Items.Clear();
				btnImport.Enabled = false;
			}
		}

		private void lbServerChamps_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lbServerChamps.SelectedIndex >= 0)
			{
				btnImport.Enabled = true;
			}
		}

		private void btnImport_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbServerChamps.SelectedIndex;
			if (selIndex < 0)
				return;
			int selRegion=cbRegions.SelectedIndex;
			if (selRegion < 0)
				return;

			int champID=(int) (lbServerChamps.Items[selIndex] as ListItem).Value;
			int regionID=(int) (cbRegions.Items[selRegion] as ListItem).Value;
			int sportID=(int) (cbSports.Items[cbSports.SelectedIndex] as ListItem).Value;
			
			_iniFile.WriteValue("Settings", "LastSport", sportID.ToString());
			_iniFile.WriteValue("Settings", "LastRegion", regionID.ToString());
			
			_importedCourtTypes = new ArrayList();
			_importedCourts = new ArrayList();
			_importedFacilities = new ArrayList();
			_importedPlayers = new ArrayList();
			_importedTeams = new ArrayList();
			_importedSportFields = new ArrayList();
			_importedRules = new ArrayList();
			
			Cursor oldCursor=Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			//get championship data:
			DataServices.ChampionshipData[] champs=
				_dataService.GetChampionshipData(champID, regionID);
			DataServices.ChampionshipData champData=new DataServices.ChampionshipData();
			if ((champs != null)&&(champs.Length > 0))
				champData = champs[0];

			//first delete current data if exists:
			Sport.UI.Dialogs.WaitForm.ShowWait("מוחק נתונים קיימים אנא המתן...");
			LocalDatabaseManager.LocalDatabase.DeleteChampionshipData(
				champID);
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			//import.....
			ImportRegions(champData);
			ImportSports(champData);
			ImportRulesets(champData);
			ImportChampionship(champData);
			ImportCategories(champData);

			foreach (Sport.Entities.Facility facility in _importedFacilities)
				ImportFacility(facility);
			foreach (Sport.Entities.CourtType courtType in _importedCourtTypes)
				ImportCourtType(courtType);
			foreach (Sport.Entities.Court court in _importedCourts)
				ImportCourt(court);
			foreach (Sport.Entities.Player player in _importedPlayers)
				ImportPlayer(player);
			foreach (Sport.Entities.Team team in _importedTeams)
				ImportTeam(team);
			foreach (Sport.Entities.SportField sportField in _importedSportFields)
				ImportSportField(sportField);
			foreach (Sport.Common.RuleData rule in _importedRules)
				ImportRule(rule);
			
			Cursor.Current = oldCursor;
			
			Sport.UI.MessageBox.Success("אליפות הועברה בהצלחה", "ניהול אליפויות");
			FillChampionships();
			
			//add user action:
			try
			{
				SessionServices.SessionService _service=
					new SessionServices.SessionService();
				string description="Championship: "+champID.ToString();
				_service.AddUserAction_2(Core.UserManager.CurrentUser.ID, 
					SessionServices.Action.Field_Import, description, Sport.Core.Data.Field_CurrentVersion);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to add Field Import action: "+ex.Message);
			}
		}

		#region Import Methods
		#region Import Regions
		private void ImportRegions(DataServices.ChampionshipData champData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני מחוזות אנא המתן...");
			Sport.Common.RegionData region=new Sport.Common.RegionData(
				champData.Region.ID, champData.Region.Name);
			region.Address = champData.Region.Address;
			region.Phone = champData.Region.Phone;
			region.Fax = champData.Region.Fax;
			LocalDatabaseManager.LocalDatabase.UpdateRegionData(region);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		
		#region Import Sports
		private void ImportSports(DataServices.ChampionshipData champData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני ענפי ספורט אנא המתן...");
			Sport.Common.SportData sport=new Sport.Common.SportData(
				champData.Sport.ID, champData.Sport.Name);
			sport.Type = champData.Sport.Type;
			Sport.Entities.Sport sportEnt=
				new Sport.Entities.Sport(champData.Sport.ID);
			sport.Ruleset = new Sport.Common.RulesetData(-1, "");
			if (sportEnt.Ruleset != null)
			{
				sport.Ruleset.ID = sportEnt.Ruleset.Id;
				sport.Ruleset.Name = sportEnt.Ruleset.Name;
			}
			LocalDatabaseManager.LocalDatabase.UpdateSportData(sport);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion

		#region Import Rule Sets
		private void ImportRulesets(DataServices.ChampionshipData champData)
		{
			if ((champData.RuleSet == null)||(champData.RuleSet.ID < 0))
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני תקנונים אנא המתן...");
			Sport.Common.RulesetData ruleSet=new Sport.Common.RulesetData(
				champData.RuleSet.ID, champData.RuleSet.Name);
			ruleSet.Region = new Sport.Common.RegionData(champData.RuleSet.ID, "");
			ruleSet.Sport = new Sport.Common.SportData(champData.Sport.ID, "");
			LocalDatabaseManager.LocalDatabase.UpdateRulesetData(ruleSet);
			Sport.UI.Dialogs.WaitForm.HideWait();

			//import rules:
			int rulesetID=champData.RuleSet.ID;
			DataServices.RuleData[] rules=_dataService.GetRulesetRules(rulesetID);
			foreach (DataServices.RuleData rule in rules)
			{
				Sport.Common.RuleData ruleData=new Sport.Common.RuleData();
				ruleData.RuleID = rule.RuleID;
				ruleData.Ruleset = new Sport.Common.RulesetData(
					rule.Ruleset.ID, rule.Ruleset.Name);
				ruleData.RuleType = new Sport.Common.SimpleData(
					rule.RuleType.ID, rule.RuleType.Name);
				ruleData.Value = rule.Value;
				ruleData.SportField = new Sport.Common.SportFieldData(
					rule.SportField.ID, rule.SportField.Name);
				ruleData.SportFieldType = new Sport.Common.SportFieldTypeData(
					rule.SportFieldType.ID, rule.SportFieldType.Name);
				ruleData.Category = rule.Category;
				_importedRules.Add(ruleData);
			}
		}
		#endregion
		
		#region Import Championship
		private void ImportChampionship(DataServices.ChampionshipData champData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני אליפות אנא המתן...");
			Sport.Common.ChampionshipData champ=
				new Sport.Common.ChampionshipData();
			champ.AltEndDate = champData.AltEndDate;
			champ.AltFinalsDate = champData.AltFinalsDate;
			champ.AltStartDate = champData.AltStartDate;
			champ.EndDate = champData.EndDate;
			champ.FinalsDate = champData.FinalsDate;
			champ.ID = champData.ID;
			champ.IsClubs = champData.IsClubs;
			champ.IsOpen = champData.IsOpen;
			champ.LastRegistrationDate = champData.LastRegistrationDate;
			champ.Name = champData.Name;
			champ.Region = new Sport.Common.RegionData(champData.Region.ID, "");
			champ.Ruleset = new Sport.Common.RulesetData(champData.RuleSet.ID, "");
			champ.Season = champData.Season;
			champ.Sport = new Sport.Common.SportData(champData.Sport.ID, "");
			champ.StandardChampionship = 
				new Sport.Common.SimpleData(champData.StandardChampionship.ID, "");
			champ.StartDate = champData.StartDate;
			champ.Status = champData.Status;
			champ.Supervisor = champData.Supervisor;
			LocalDatabaseManager.LocalDatabase.UpdateChampionshipData(champ);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		
		#region Import Categories
		private void ImportCategories(DataServices.ChampionshipData champData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען רשימת קטגוריות אליפות אנא המתן...");
			int champID=champData.ID;
			DataServices.CategoryData[] categories=
				_dataService.GetChampionshipCategories(champID);
			Sport.UI.Dialogs.WaitForm.HideWait();
			foreach (DataServices.CategoryData category in categories)
			{
				Sport.Common.CategoryData catData=new Sport.Common.CategoryData();
				catData.ID = category.ID;
				catData.Championship = category.Championship;
				catData.Category = category.Category;
				ImportCategory(catData);
			}
		}

		#region Import Single Category
		private void ImportCategory(Sport.Common.CategoryData category)
		{
			//first import category itself into local database:
			if ((category == null)||(category.ID < 0))
				throw new Exception("Import Category: invalid category given.");
			
			string strCategory=Sport.Types.CategoryTypeLookup.ToString(category.Category);
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני קטגורית אליפות "+strCategory+" אנא המתן...");
			LocalDatabaseManager.LocalDatabase.UpdateChampionshipCategory(category);
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			//get championship information:
			Sport.Championships.Championship champ=
				Sport.Championships.Championship.GetChampionship(category.ID);

			//import each phase:
			if ((champ != null)&&(champ.Phases != null))
			{
				foreach (Sport.Championships.Phase phase in champ.Phases)
				{
					ImportPhase(phase);
				}
			}
		}

		#region Import Phase
		private void ImportPhase(Sport.Championships.Phase phase)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני שלב "+(phase.Index+1).ToString()+" אנא המתן...");
			Sport.Common.PhaseData phaseData=new PhaseData();
			phaseData.CategoryID = phase.Championship.ChampionshipCategory.Id;
			phaseData.PhaseIndex = phase.Index;
			phaseData.PhaseName = phase.Name;
			phaseData.Status = (int) phase.Status;
			LocalDatabaseManager.LocalDatabase.UpdatePhaseData(phaseData);
			Sport.UI.Dialogs.WaitForm.HideWait();
			foreach (Sport.Championships.Group group in phase.Groups)
			{
				ImportGroup(group, phaseData);
			}
		}

		#region Import Group
		private void ImportGroup(Sport.Championships.Group group, 
			Sport.Common.PhaseData phaseData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני בית "+(group.Index+1).ToString()+" אנא המתן...");
			Sport.Common.GroupData groupData=new Sport.Common.GroupData();
			groupData.Phase = phaseData;
			groupData.GroupIndex = group.Index;
			groupData.GroupName = group.Name;
			LocalDatabaseManager.LocalDatabase.UpdateGroupData(groupData);
			Sport.UI.Dialogs.WaitForm.HideWait();
			if (group is Sport.Championships.MatchGroup)
			{
				foreach (Sport.Championships.Round round in
					(group as Sport.Championships.MatchGroup).Rounds)
				{
					ImportRound(round, groupData);
				}
			}
			else
			{
				foreach (Sport.Championships.Competition competition in
					(group as Sport.Championships.CompetitionGroup).Competitions)
				{
					ImportCompetition(competition, groupData);
				}
				foreach (Sport.Championships.CompetitionTeam compTeam in
					(group as Sport.Championships.CompetitionGroup).Teams)
				{
					if (compTeam.TeamEntity != null)
					{
						if (_importedTeams.IndexOf(compTeam.TeamEntity) < 0)
							_importedTeams.Add(compTeam.TeamEntity);
						Sport.Entities.Player[] players=compTeam.TeamEntity.GetPlayers();
						foreach (Sport.Entities.Player player in players)
						{
							if (_importedPlayers.IndexOf(player) < 0)
								_importedPlayers.Add(player);
						}
					}
				}
			}
		}
		#region Import Round
		private void ImportRound(Sport.Championships.Round round, 
			Sport.Common.GroupData groupData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני סיבוב "+(round.Index+1).ToString()+" אנא המתן...");
			Sport.Common.RoundData roundData=new Sport.Common.RoundData();
			roundData.Group = groupData;
			roundData.RoundIndex = round.Index;
			roundData.RoundName = round.Name;
			LocalDatabaseManager.LocalDatabase.UpdateRoundData(roundData);
			Sport.UI.Dialogs.WaitForm.HideWait();

			foreach (Sport.Championships.Cycle cycle in round.Cycles)
			{
				foreach(Sport.Championships.Match match in cycle.Matches)
				{
					ImportMatch(match, roundData);

					//import match teams as well:
					Sport.Championships.MatchTeam teamA=match.GroupTeamA;
					Sport.Championships.MatchTeam teamB=match.GroupTeamB;
					if ((teamA != null)&&(teamA.TeamEntity != null))
					{
						if (_importedTeams.IndexOf(teamA.TeamEntity) < 0)
							_importedTeams.Add(teamA.TeamEntity);
						Sport.Entities.Player[] players=teamA.TeamEntity.GetPlayers();
						foreach (Sport.Entities.Player player in players)
						{
							if (_importedPlayers.IndexOf(player) < 0)
								_importedPlayers.Add(player);
						}
					}
					if ((teamB != null)&&(teamB.TeamEntity != null))
					{
						if (_importedTeams.IndexOf(teamB.TeamEntity) < 0)
							_importedTeams.Add(teamB.TeamEntity);
						Sport.Entities.Player[] players=teamB.TeamEntity.GetPlayers();
						foreach (Sport.Entities.Player player in players)
						{
							if (_importedPlayers.IndexOf(player) < 0)
								_importedPlayers.Add(player);
						}
					}
				}
			}
		}

		#region Import Match
		private void ImportMatch(Sport.Championships.Match match, 
			Sport.Common.RoundData roundData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני התמודדות "+(match.Index+1).ToString()+" אנא המתן...");
			Sport.Common.MatchData matchData=new Sport.Common.MatchData();
			matchData.Round = roundData;
			matchData.MatchIndex = match.Index;
			if ((match.Facility != null)&&(match.Facility.Id >= 0))
			{
				matchData.Facility = new Sport.Common.FacilityData(
					match.Facility.Id, match.Facility.Name);
				if (_importedFacilities.IndexOf(match.Facility) < 0)
				{
					_importedFacilities.Add(match.Facility);
				}
			}
			if ((match.Court != null)&&(match.Court.Id >= 0))
			{
				matchData.Court = new Sport.Common.CourtData(
					match.Court.Id, match.Court.Name);
				if (_importedCourts.IndexOf(match.Court) < 0)
				{
					_importedCourts.Add(match.Court);
				}
				if (match.Court.CourtType != null)
				{
					if (_importedCourtTypes.IndexOf(match.Court.CourtType) < 0)
					{
						_importedCourtTypes.Add(match.Court.CourtType);
					}
				}
			}
			matchData.Result = (int) match.Outcome;
			matchData.PartsResult = match.PartsResult;
			matchData.Time = match.Time;
			Sport.Championships.MatchTeam teamA=match.Cycle.Round.Group.Teams[match.TeamA];
			Sport.Championships.MatchTeam teamB=match.Cycle.Round.Group.Teams[match.TeamB];
			matchData.TeamA = new Sport.Common.TeamData();
			matchData.TeamA.ID = teamA.TeamEntity.Id;
			matchData.TeamA_Score = teamA.Score;
			matchData.TeamB = new Sport.Common.TeamData();
			matchData.TeamB.ID = teamB.TeamEntity.Id;
			matchData.TeamB_Score = teamB.Score;
			LocalDatabaseManager.LocalDatabase.UpdateMatchData(matchData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		#endregion
		
		#region Import Competition
		private void ImportCompetition(Sport.Championships.Competition competition, 
			Sport.Common.GroupData groupData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני תחרות "+(competition.Index+1).ToString()+" אנא המתן...");
			Sport.Common.CompetitionData compData=new Sport.Common.CompetitionData();
			compData.Group = groupData;
			compData.CompetitionIndex = competition.Index;
			if ((competition.Facility != null)&&(competition.Facility.Id >= 0))
			{
				compData.Facility = new Sport.Common.FacilityData(
					competition.Facility.Id, competition.Facility.Name);
				if (_importedFacilities.IndexOf(competition.Facility) < 0)
				{
					_importedFacilities.Add(competition.Facility);
				}
			}
			if ((competition.Court != null)&&(competition.Court.Id >= 0))
			{
				compData.Court = new Sport.Common.CourtData(
					competition.Court.Id, competition.Court.Name);
				if (_importedCourts.IndexOf(competition.Court) < 0)
				{
					_importedCourts.Add(competition.Court);
				}
				if (competition.Court.CourtType != null)
				{
					if (_importedCourtTypes.IndexOf(competition.Court.CourtType) < 0)
					{
						_importedCourtTypes.Add(competition.Court.CourtType);
					}
				}
			}
			compData.SportField = new Sport.Common.SportFieldData(
				competition.SportField.Id, competition.SportField.Name);
			compData.Time = competition.Time;
			LocalDatabaseManager.LocalDatabase.UpdateCompetitionData(compData);
			Sport.UI.Dialogs.WaitForm.HideWait();
			foreach (Sport.Championships.Heat heat in competition.Heats)
			{
				ImportHeat(heat, compData);
			}
			foreach (Sport.Championships.Competitor competitor in 
				competition.Competitors)
			{
				ImportCompetitor(competitor, compData);
			}
			if (competition.SportField != null)
			{
				_importedSportFields.Add(competition.SportField);
			}
		}
		
		#region Import Heat
		private void ImportHeat(Sport.Championships.Heat heat,
			Sport.Common.CompetitionData compData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני מקצה "+(heat.Index+1).ToString()+" אנא המתן...");
			Sport.Common.HeatData heatData=new Sport.Common.HeatData();
			heatData.Competition = compData;
			heatData.HeatIndex = heat.Index;
			if ((heat.Facility != null)&&(heat.Facility.Id >= 0))
			{
				heatData.Facility = new Sport.Common.FacilityData(
					heat.Facility.Id, heat.Facility.Name);
				if (_importedFacilities.IndexOf(heat.Facility) < 0)
				{
					_importedFacilities.Add(heat.Facility);
				}
			}
			if ((heat.Court != null)&&(heat.Court.Id >= 0))
			{
				heatData.Court = new Sport.Common.CourtData(
					heat.Court.Id, heat.Court.Name);
				if (_importedCourts.IndexOf(heat.Court) < 0)
				{
					_importedCourts.Add(heat.Court);
				}
				if (heat.Court.CourtType != null)
				{
					if (_importedCourtTypes.IndexOf(heat.Court.CourtType) < 0)
					{
						_importedCourtTypes.Add(heat.Court.CourtType);
					}
				}
			}
			heatData.Time = heat.Time;
			LocalDatabaseManager.LocalDatabase.UpdateHeatData(heatData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		
		#region Import Competitor
		private void ImportCompetitor(Sport.Championships.Competitor competitor, 
			Sport.Common.CompetitionData compData)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני מתחרה "+(competitor.Index+1).ToString()+" אנא המתן...");
			Sport.Common.CompetitorData competitorData=
				new Sport.Common.CompetitorData();
			competitorData.Competition = compData;
			competitorData.CompetitorIndex = competitor.Index;
			competitorData.Heat = new Sport.Common.HeatData();
			competitorData.Heat.HeatIndex = competitor.Heat;
			competitorData.Player = new Sport.Common.PlayerData();
			competitorData.Player.ID = competitor.Player.PlayerEntity.Id;
			competitorData.Player.ShirtNumber = Tools.CIntDef(
				competitor.PlayerNumber, 0);
			competitorData.Position = competitor.Position;
			competitorData.Result = competitor.Result.ToString();
			competitorData.ResultPosition = competitor.ResultPosition;
			competitorData.Score = competitor.Score;
			LocalDatabaseManager.LocalDatabase.UpdateCompetitorData(competitorData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#endregion
		#endregion
		
		#region Import Court Type
		private void ImportCourtType(Sport.Entities.CourtType courtType)
		{
			if (courtType.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני סוג מגרש "+courtType.Name+" אנא המתן...");
			Sport.Common.SimpleData courtTypeData=
				new Sport.Common.SimpleData(courtType.Id, courtType.Name);
			LocalDatabaseManager.LocalDatabase.UpdateCourtType(courtTypeData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		
		#region Import Court
		private void ImportCourt(Sport.Entities.Court court)
		{
			if (court.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני מגרש "+court.Name+" אנא המתן...");
			Sport.Common.CourtData courtData=
				new Sport.Common.CourtData(court.Id, court.Name);
			if (court.CourtType != null)
			{
				courtData.CourtType = new Sport.Common.SimpleData(
					court.CourtType.Id, court.CourtType.Name);
			}
			else
			{
				courtData.CourtType = new Sport.Common.SimpleData(-1, "");
			}
			if ((court.Facility != null)&&(court.Facility.Id >= 0))
			{
				courtData.Facility = new Sport.Common.FacilityData();
				courtData.Facility.ID = court.Facility.Id;
				courtData.Facility.Name = court.Facility.Name;
			}
			LocalDatabaseManager.LocalDatabase.UpdateCourtData(courtData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		
		#region Import Facility
		private void ImportFacility(Sport.Entities.Facility facility)
		{
			if (facility.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני מתקן "+facility.Name+" אנא המתן...");
			Sport.Common.FacilityData facilityData=
				new Sport.Common.FacilityData(facility.Id, facility.Name);
			facilityData.Address = facility.Address;
			facilityData.Fax = facility.Fax;
			facilityData.Phone = facility.Phone;
			facilityData.Region = new Sport.Common.RegionData(
				facility.Region.Id, facility.Region.Name);
			if ((facility.School != null)&&(facility.School.Id >= 0))
			{
				facilityData.School = new Sport.Common.SchoolData(
					facility.School.Id, facility.School.Name);
			}
			LocalDatabaseManager.LocalDatabase.UpdateFacilityData(facilityData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		
		#region Import Player
		private void ImportPlayer(Sport.Entities.Player player)
		{
			if (player.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני שחקן "+player.Number+" אנא המתן...");
			Sport.Common.PlayerData playerData=new Sport.Common.PlayerData();
			playerData.ID = player.Id;
			playerData.Student = new Sport.Common.StudentData(
				player.Student.Id, player.Student.FirstName, player.Student.LastName);
			playerData.RegistrationDate = DateTime.MinValue;
			if (player.Entity.Fields[(int) Sport.Entities.Player.Fields.RegisterDate] != null)
			{
				playerData.RegistrationDate = (DateTime) 
					player.Entity.Fields[(int) Sport.Entities.Player.Fields.RegisterDate];
			}
			playerData.ShirtNumber = Tools.CIntDef(player.Number, 0);
			playerData.Status = 
				(int) player.Entity.Fields[(int) Sport.Entities.Player.Fields.Status];
			playerData.Remarks = Tools.CStrDef(
				player.Entity.Fields[(int) Sport.Entities.Player.Fields.Remarks], "");
			playerData.Team = new Sport.Common.TeamData();
			playerData.Team.ID = player.Team.Id;
			LocalDatabaseManager.LocalDatabase.UpdatePlayerData(playerData);
			Sport.UI.Dialogs.WaitForm.HideWait();

			//import student as well:
			ImportStudent(player.Student);
		}
		
		#region Import Student
		private void ImportStudent(Sport.Entities.Student student)
		{
			if (student.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני תלמיד ("+student.FirstName+" "+student.LastName+") אנא המתן...");
			Sport.Common.StudentData studentData=new Sport.Common.StudentData(
				student.Id, student.FirstName, student.LastName);
			studentData.Birthdate = student.BirthDate;
			studentData.Grade = student.Grade;
			studentData.IdNumber = student.IdNumber;
			studentData.School = new Sport.Common.SchoolData(
				student.School.Id, student.School.Name);
			studentData.SexType = (int) student.SexType;
			LocalDatabaseManager.LocalDatabase.UpdateStudentData(studentData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		#endregion
		
		#region Import Team
		private void ImportTeam(Sport.Entities.Team team)
		{
			if (team.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני קבוצה "+team.Index+" אנא המתן...");
			Sport.Common.TeamData teamData=new Sport.Common.TeamData();
			teamData.ID = team.Id;
			teamData.School = new Sport.Common.SchoolData(
				team.School.Id, team.School.Name);
			teamData.TeamIndex = Tools.CIntDef(team.Index, 0);
			teamData.Status = (int) team.Status;
			teamData.RegistrationDate = team.RegisterDate;
			int userID=Tools.CIntDef(
				team.Entity.Fields[(int) Sport.Entities.Team.Fields.Supervisor], -1);
			if (userID >= 0)
			{
				teamData.Supervisor = _dataService.GetUserName(userID);
			}
			teamData.Championship = new Sport.Common.ChampionshipData();
			teamData.Championship.ID = team.Championship.Id;
			teamData.ChampionshipCategory = new Sport.Common.CategoryData(
				team.Category.Id, teamData.Championship.ID, team.Category.Category);
			LocalDatabaseManager.LocalDatabase.UpdateTeamData(teamData);
			Sport.UI.Dialogs.WaitForm.HideWait();

			//import school as well:
			ImportSchool(team.School);
		}
		
		#region Import School
		private void ImportSchool(Sport.Entities.School school)
		{
			if (school.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני בית ספר "+school.Name+" אנא המתן...");
			//string schoolName=school.Name;
			//schoolName = schoolName.Replace(" - ", "[minus_sign]");
			//schoolName = schoolName.Replace("-", " ");
			//schoolName = schoolName.Replace("[minus_sign]", " - ");
			//schoolName = Tools.RemoveDuplicateWords(schoolName);
			string schoolName=school.Entity.Fields[(int) Sport.Entities.School.Fields.Name].ToString();
			Sport.Common.SchoolData schoolData=new Sport.Common.SchoolData(
				school.Id, schoolName);
			schoolData.City = school.City.Name;
			schoolData.FromGrade = school.FromGrade;
			schoolData.ToGrade = school.ToGrade;
			schoolData.IsClub = (school.ClubStatus == 1)?true:false;
			schoolData.Region = new Sport.Common.RegionData(
				school.Region.Id, school.Region.Name);
			schoolData.Symbol = school.Symbol;
			LocalDatabaseManager.LocalDatabase.UpdateSchoolData(schoolData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		#endregion
		
		#region Import Sport Field
		private void ImportSportField(Sport.Entities.SportField sportField)
		{
			if (sportField.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני סוג מקצוע "+sportField.Name+" אנא המתן...");
			Sport.Common.SportFieldData sportFieldData=
				new Sport.Common.SportFieldData(sportField.Id, sportField.Name);
			sportFieldData.SportFieldType = new Sport.Common.SportFieldTypeData(
				sportField.SportFieldType.Id, sportField.SportFieldType.Name);
			LocalDatabaseManager.LocalDatabase.UpdateSportField(sportFieldData);
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			//import sport field type as well:
			ImportSportFieldType(sportField.SportFieldType);
		}
		
		#region Import Sport Field Type
		private void ImportSportFieldType(Sport.Entities.SportFieldType sportFieldType)
		{
			if (sportFieldType.Id < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני מקצוע "+sportFieldType.Name+" אנא המתן...");
			Sport.Common.SportFieldTypeData fieldTypeData=new Sport.Common.SportFieldTypeData(
				sportFieldType.Id, sportFieldType.Name);
			fieldTypeData.Sport = new Sport.Common.SportData(
				sportFieldType.Sport.Id, sportFieldType.Sport.Name);
			LocalDatabaseManager.LocalDatabase.UpdateSportFieldType(fieldTypeData);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		#endregion
		
		#region Import Rule
		private void ImportRule(Sport.Common.RuleData rule)
		{
			if (rule.RuleID < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען חוק אנא המתן...");
			LocalDatabaseManager.LocalDatabase.UpdateRuleData(rule);
			Sport.UI.Dialogs.WaitForm.HideWait();
			
			//import rule type as well:
			ImportRuleType(rule.RuleType);
		}
		
		#region Import Rule Type
		private void ImportRuleType(Sport.Common.SimpleData ruleType)
		{
			if (ruleType.ID < 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען סוג חוק אנא המתן...");
			LocalDatabaseManager.LocalDatabase.UpdateRuleType(ruleType);
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		#endregion
		#endregion
		#endregion
	}
}
