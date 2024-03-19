using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for SetResultsDialog.
	/// </summary>
	public class SetResultsDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ListBox lbLocalChamps;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListBox lbSports;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox lbRegions;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnSetResults;

		private Sport.Common.IniFile _iniFile=null;

		public SetResultsDialog()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SetResultsDialog));
			this.btnClose = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnSetResults = new System.Windows.Forms.Button();
			this.lbLocalChamps = new System.Windows.Forms.ListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lbSports = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lbRegions = new System.Windows.Forms.ListBox();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.ForeColor = System.Drawing.Color.Blue;
			this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnClose.Location = new System.Drawing.Point(151, 282);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(96, 23);
			this.btnClose.TabIndex = 12;
			this.btnClose.Text = "אישור";
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.Color.White;
			this.groupBox3.Controls.Add(this.btnSetResults);
			this.groupBox3.Controls.Add(this.lbLocalChamps);
			this.groupBox3.Location = new System.Drawing.Point(7, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(220, 264);
			this.groupBox3.TabIndex = 11;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "אליפויות";
			// 
			// btnSetResults
			// 
			this.btnSetResults.BackColor = System.Drawing.Color.White;
			this.btnSetResults.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnSetResults.ForeColor = System.Drawing.Color.Black;
			this.btnSetResults.Image = ((System.Drawing.Image)(resources.GetObject("btnSetResults.Image")));
			this.btnSetResults.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSetResults.Location = new System.Drawing.Point(56, 232);
			this.btnSetResults.Name = "btnSetResults";
			this.btnSetResults.Size = new System.Drawing.Size(120, 23);
			this.btnSetResults.TabIndex = 9;
			this.btnSetResults.Text = "קביעת תוצאות";
			this.btnSetResults.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnSetResults.Click += new System.EventHandler(this.btnSetResults_Click);
			// 
			// lbLocalChamps
			// 
			this.lbLocalChamps.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbLocalChamps.ForeColor = System.Drawing.Color.Blue;
			this.lbLocalChamps.Location = new System.Drawing.Point(5, 24);
			this.lbLocalChamps.Name = "lbLocalChamps";
			this.lbLocalChamps.Size = new System.Drawing.Size(210, 199);
			this.lbLocalChamps.TabIndex = 4;
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.lbSports);
			this.groupBox2.Location = new System.Drawing.Point(231, 146);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(144, 128);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "ענף ספורט";
			// 
			// lbSports
			// 
			this.lbSports.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSports.ForeColor = System.Drawing.Color.Black;
			this.lbSports.Location = new System.Drawing.Point(8, 24);
			this.lbSports.Name = "lbSports";
			this.lbSports.Size = new System.Drawing.Size(130, 95);
			this.lbSports.TabIndex = 1;
			this.lbSports.SelectedIndexChanged += new System.EventHandler(this.lbRegions_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.lbRegions);
			this.groupBox1.Location = new System.Drawing.Point(231, 10);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(144, 128);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "מחוז";
			// 
			// lbRegions
			// 
			this.lbRegions.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbRegions.ForeColor = System.Drawing.Color.Black;
			this.lbRegions.Location = new System.Drawing.Point(8, 24);
			this.lbRegions.Name = "lbRegions";
			this.lbRegions.Size = new System.Drawing.Size(130, 95);
			this.lbRegions.TabIndex = 1;
			this.lbRegions.SelectedIndexChanged += new System.EventHandler(this.lbRegions_SelectedIndexChanged);
			// 
			// SetResultsDialog
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(382, 315);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetResultsDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "קביעת תוצאות אליפות";
			this.Load += new System.EventHandler(this.SetResultsDialog_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetResultsDialog_Load(object sender, System.EventArgs e)
		{
			FillRegions();
			FillSports();

			//read selected championship from ini file:
			string lastSelectedRegion=_iniFile.ReadValue("SetResultsDialog", "LastSelectedRegion");
			string lastSelectedSport=_iniFile.ReadValue("SetResultsDialog", "LastSelectedSport");
			string lastSelectedChamp=_iniFile.ReadValue("SetResultsDialog", "LastSelectedChampionsip");
			if ((lastSelectedRegion != null)&&(lastSelectedRegion.Length > 0))
			{
				int regionID=Tools.CIntDef(lastSelectedRegion, -1);
				AutoSelect(lbRegions, regionID);
			}
			if ((lastSelectedSport != null)&&(lastSelectedSport.Length > 0))
			{
				int sportID=Tools.CIntDef(lastSelectedSport, -1);
				AutoSelect(lbSports, sportID);
			}
			if ((lastSelectedChamp != null)&&(lastSelectedChamp.Length > 0))
			{
				int champID=Tools.CIntDef(lastSelectedChamp, -1);
				AutoSelect(lbLocalChamps, champID);
			}
		}
		
		private void FillRegions()
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני מחוזות אנא המתן...");
			SimpleData[] regions=LocalDatabaseManager.LocalDatabase.GetRegions();
			lbRegions.Items.Clear();
			foreach (SimpleData region in regions)
			{
				lbRegions.Items.Add(
					new ListItem(region.Name, region.ID));
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
		}
		
		private void FillSports()
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני ענפי ספורט אנא המתן...");
			SimpleData[] sports=LocalDatabaseManager.LocalDatabase.GetSports();
			lbSports.Items.Clear();
			lbSports.Items.Add(new ListItem("<כל ענפי הספורט>", -1));
			foreach (SimpleData sport in sports)
			{
				lbSports.Items.Add(
					new ListItem(sport.Name, sport.ID));
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		private void lbRegions_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int selRegionIndex=lbRegions.SelectedIndex;
			int selSportIndex=lbSports.SelectedIndex;
			
			if ((selRegionIndex < 0)||(selSportIndex < 0))
				return;
			
			FillChampionships();
		}

		private void FillChampionships()
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני אליפות אנא המתן...");

			int regionID=(int) (lbRegions.SelectedItem as ListItem).Value;
			int sportID=(int) (lbSports.SelectedItem as ListItem).Value;
			
			lbLocalChamps.Items.Clear();
			SimpleData[] championships=
				LocalDatabaseManager.LocalDatabase.GetChampionshipsData(
				regionID, sportID);
			foreach (SimpleData championship in championships)
			{
				lbLocalChamps.Items.Add(new ListItem(
					championship.Name, championship.ID));
			}
			
			Sport.UI.Dialogs.WaitForm.HideWait();
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

		private void btnSetResults_Click(object sender, System.EventArgs e)
		{
			int selChampIndex=lbLocalChamps.SelectedIndex;
			if (selChampIndex < 0)
				return;
			
			int champID=(int) (lbLocalChamps.SelectedItem as ListItem).Value;
			SportData sport=
				LocalDatabaseManager.LocalDatabase.GetChampionshipSport(champID);
			switch (sport.Type)
			{
				case (int) Sport.Types.SportType.Match:
					MatchChampionshipDialog _matchDialog=
						new MatchChampionshipDialog(champID, true);
					_matchDialog.ShowDialog(this);
					break;
				case (int) Sport.Types.SportType.Competition:
					CompetitionChampionshipDialog _competitionDialog=
						new CompetitionChampionshipDialog(champID, true);
					_competitionDialog.ShowDialog(this);
					break;
			}

			//store the selected championship:
			int regionID=(int) (lbRegions.SelectedItem as ListItem).Value;
			int sportID=(int) (lbSports.SelectedItem as ListItem).Value;
			_iniFile.WriteValue("SetResultsDialog", "LastSelectedRegion", regionID.ToString());
			_iniFile.WriteValue("SetResultsDialog", "LastSelectedSport", sportID.ToString());
			_iniFile.WriteValue("SetResultsDialog", "LastSelectedChampionsip", champID.ToString());
		}
	}
}
