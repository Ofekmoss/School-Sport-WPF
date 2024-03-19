using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;
using LocalDatabaseManager;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for ManageDialog.
	/// </summary>
	public class ManageDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox lbRegions;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ListBox lbSports;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.ListBox lbLocalChamps;
		private System.Windows.Forms.Button btnDetails;
		private System.Windows.Forms.Button btnDelete;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private StatusType _statusType;
		private Sport.Common.IniFile _iniFile=null;

		public ManageDialog(StatusType statusType)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_statusType = statusType;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ManageDialog));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lbRegions = new System.Windows.Forms.ListBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lbSports = new System.Windows.Forms.ListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.lbLocalChamps = new System.Windows.Forms.ListBox();
			this.btnDetails = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.lbRegions);
			this.groupBox1.Location = new System.Drawing.Point(232, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(144, 128);
			this.groupBox1.TabIndex = 5;
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
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.lbSports);
			this.groupBox2.Location = new System.Drawing.Point(232, 144);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(144, 128);
			this.groupBox2.TabIndex = 6;
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
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.Color.White;
			this.groupBox3.Controls.Add(this.btnDelete);
			this.groupBox3.Controls.Add(this.btnDetails);
			this.groupBox3.Controls.Add(this.lbLocalChamps);
			this.groupBox3.Location = new System.Drawing.Point(8, 8);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(220, 264);
			this.groupBox3.TabIndex = 7;
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
			this.btnClose.Location = new System.Drawing.Point(152, 280);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(96, 23);
			this.btnClose.TabIndex = 8;
			this.btnClose.Text = "אישור";
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
			// btnDetails
			// 
			this.btnDetails.BackColor = System.Drawing.Color.White;
			this.btnDetails.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDetails.ForeColor = System.Drawing.Color.Black;
			this.btnDetails.Image = ((System.Drawing.Image)(resources.GetObject("btnDetails.Image")));
			this.btnDetails.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnDetails.Location = new System.Drawing.Point(120, 232);
			this.btnDetails.Name = "btnDetails";
			this.btnDetails.Size = new System.Drawing.Size(96, 23);
			this.btnDetails.TabIndex = 9;
			this.btnDetails.Text = "נתוני אליפות";
			this.btnDetails.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.BackColor = System.Drawing.Color.White;
			this.btnDelete.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDelete.ForeColor = System.Drawing.Color.Red;
			this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
			this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnDelete.Location = new System.Drawing.Point(8, 232);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(96, 23);
			this.btnDelete.TabIndex = 10;
			this.btnDelete.Text = "מחיקה";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// ManageDialog
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(384, 311);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ManageDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "כלי ניהול";
			this.Load += new System.EventHandler(this.ManageDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void ManageDialog_Load(object sender, System.EventArgs e)
		{
			FillRegions();
			FillSports();

			//read selected championship from ini file:
			string lastSelectedRegion=_iniFile.ReadValue("ManageDialog", "LastSelectedRegion");
			string lastSelectedSport=_iniFile.ReadValue("ManageDialog", "LastSelectedSport");
			string lastSelectedChamp=_iniFile.ReadValue("ManageDialog", "LastSelectedChampionsip");
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

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			int selChampIndex=lbLocalChamps.SelectedIndex;
			if (selChampIndex < 0)
				return;

			int champID=(int) (lbLocalChamps.SelectedItem as ListItem).Value;
			string champName=(lbLocalChamps.SelectedItem as ListItem).Text;
			if (!Sport.UI.MessageBox.Ask("האם למחוק את האליפות '"+champName+"'?", false))
			{
				return;
			}

			if (_statusType == StatusType.Offline)
			{
				if (Sport.UI.MessageBox.Show("הנך עובד במצב לא מקוון. פעולת המחיקה "+
					"תהיה לא הפיכה ולא יהיה ניתן לייבא מחדש עד לעבודה מקוונת.\nהאם להמשיך?", 
					MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
					return;
			}

			Sport.UI.Dialogs.WaitForm.ShowWait("מוחק נתוני אליפות אנא המתן...");
			LocalDatabaseManager.LocalDatabase.DeleteChampionshipData(champID);
			Sport.UI.Dialogs.WaitForm.HideWait();

			Sport.UI.MessageBox.Success("האליפות נמחקה בהצלחה", "ניהול אליפויות");
			FillChampionships();
		}

		private void btnDetails_Click(object sender, System.EventArgs e)
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
						new MatchChampionshipDialog(champID);
					_matchDialog.ShowDialog(this);
					break;
				case (int) Sport.Types.SportType.Competition:
					CompetitionChampionshipDialog _competitionDialog=
						new CompetitionChampionshipDialog(champID);
					_competitionDialog.ShowDialog(this);
					break;
			}

			//store the selected championship:
			int regionID=(int) (lbRegions.SelectedItem as ListItem).Value;
			int sportID=(int) (lbSports.SelectedItem as ListItem).Value;
			_iniFile.WriteValue("ManageDialog", "LastSelectedRegion", regionID.ToString());
			_iniFile.WriteValue("ManageDialog", "LastSelectedSport", sportID.ToString());
			_iniFile.WriteValue("ManageDialog", "LastSelectedChampionsip", champID.ToString());
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
	}
}
