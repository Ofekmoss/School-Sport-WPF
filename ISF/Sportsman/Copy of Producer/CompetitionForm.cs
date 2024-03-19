using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Rulesets;
using Sport.Championships;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for CompetitionForm.
	/// </summary>
	public class CompetitionForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.DateTimePicker dtpTime;
		private System.Windows.Forms.GroupBox gbLocation;
		private System.Windows.Forms.Label labelFacility;
		private System.Windows.Forms.Label label1;
		private Sport.UI.Controls.NullComboBox cbCourt;
		private Sport.UI.Controls.ButtonBox bbFacility=null;
		
		private Sport.Championships.Competition _competition;
		private Sport.Championships.Heat _heat;
		private Sport.UI.EntitySelectionDialog facilityDialog=null;

		public CompetitionForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_competition = null;
			_heat = null;
		}

		public CompetitionForm(Sport.Championships.Competition competition, Sport.Championships.Heat heat)
			: this()
		{
			_competition = competition;
			_heat = heat;

			AddFacilities();

			Sport.Championships.Phase phase = _competition.Group.Phase;

			Text = competition.ToString() + (_heat == null ? null : " - מקצה " + (heat.Index + 1).ToString());

			DateTime time = _heat == null ? _competition.Time : _heat.Time;
			if (Sport.Common.Tools.IsMinDate(time))
			{
				dtpTime.Checked = false;
			}
			else
			{
				dtpTime.Checked = true;
				dtpTime.Value = time;
			}

			Sport.Entities.Facility facility =
				_heat == null ? _competition.Facility : _heat.Facility;
			Sport.Entities.Court court =
				_heat == null ? _competition.Court : _heat.Court;

			if (facility != null)
			{
				facilityDialog.View.State[Sport.Entities.Region.TypeName] = 
					facility.Region.Id.ToString();
				bbFacility.Value = facility.Entity;
				SetCourts();
				if (court != null)
				{
					cbCourt.SelectedItem = court;
				}
			}
		}

		private void AddFacilities()
		{
			Views.FacilitiesTableView facilityTableView=new Views.FacilitiesTableView();
			facilityTableView.State[Sport.UI.View.SelectionDialog] = "1";
			facilityDialog = new Sport.UI.EntitySelectionDialog(facilityTableView);
			
			facilityDialog.View.State[Sport.Entities.Region.TypeName] = _competition.Group.Phase.Championship.ChampionshipCategory.Championship.Region.Id.ToString();
			
			//add the facilities:
			bbFacility.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(
				facilityDialog.ValueSelector);
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
			this.labelTime = new System.Windows.Forms.Label();
			this.dtpTime = new System.Windows.Forms.DateTimePicker();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbLocation = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cbCourt = new Sport.UI.Controls.NullComboBox();
			this.labelFacility = new System.Windows.Forms.Label();
			this.bbFacility = new Sport.UI.Controls.ButtonBox();
			this.gbLocation.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Location = new System.Drawing.Point(64, 8);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(152, 16);
			this.labelTime.TabIndex = 0;
			this.labelTime.Text = "מועד:";
			// 
			// dtpTime
			// 
			this.dtpTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.dtpTime.CustomFormat = "dd/MM/yyyy HH:mm";
			this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpTime.Location = new System.Drawing.Point(8, 24);
			this.dtpTime.Name = "dtpTime";
			this.dtpTime.ShowCheckBox = true;
			this.dtpTime.Size = new System.Drawing.Size(208, 21);
			this.dtpTime.TabIndex = 1;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnOK.Location = new System.Drawing.Point(8, 184);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(96, 184);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "ביטול";
			// 
			// gbLocation
			// 
			this.gbLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbLocation.Controls.Add(this.label1);
			this.gbLocation.Controls.Add(this.cbCourt);
			this.gbLocation.Controls.Add(this.bbFacility);
			this.gbLocation.Controls.Add(this.labelFacility);
			this.gbLocation.Location = new System.Drawing.Point(8, 56);
			this.gbLocation.Name = "gbLocation";
			this.gbLocation.Size = new System.Drawing.Size(208, 120);
			this.gbLocation.TabIndex = 14;
			this.gbLocation.TabStop = false;
			this.gbLocation.Text = "מיקום";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(160, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "מגרש:";
			// 
			// cbCourt
			// 
			this.cbCourt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbCourt.Location = new System.Drawing.Point(8, 88);
			this.cbCourt.Name = "cbCourt";
			this.cbCourt.Size = new System.Drawing.Size(192, 22);
			this.cbCourt.Sorted = true;
			this.cbCourt.TabIndex = 6;
			// 
			// labelFacility
			// 
			this.labelFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFacility.Location = new System.Drawing.Point(160, 28);
			this.labelFacility.Name = "labelFacility";
			this.labelFacility.Size = new System.Drawing.Size(40, 16);
			this.labelFacility.TabIndex = 5;
			this.labelFacility.Text = "מתקן:";
			// 
			// bbFacility
			// 
			this.bbFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bbFacility.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.bbFacility.Location = new System.Drawing.Point(8, 43);
			this.bbFacility.Name = "bbFacility";
			this.bbFacility.Size = new System.Drawing.Size(192, 22);
			this.bbFacility.TabIndex = 4;
			this.bbFacility.Value = null;
			this.bbFacility.ValueSelector = null;
			this.bbFacility.ValueChanged += new System.EventHandler(this.cbFacility_SelectedIndexChanged);
			// 
			// CompetitionForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(226, 216);
			this.Controls.Add(this.gbLocation);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.dtpTime);
			this.Controls.Add(this.labelTime);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "CompetitionForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CompetitionForm";
			this.gbLocation.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetCourts()
		{
			cbCourt.Items.Clear();
			
			if (bbFacility.Value == null)
				return;
			
			Sport.Entities.Facility facility = 
				new Sport.Entities.Facility(bbFacility.Value as Sport.Data.Entity);
			if (facility != null)
			{
				Sport.Entities.Sport sport = _competition.SportField.SportFieldType.Sport;
				cbCourt.Items.AddRange(facility.GetCourts(sport));
				cbCourt.Items.Add(Sport.UI.Controls.NullComboBox.Null);
			}
		}

		private void cbFacility_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetCourts();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			Sport.Entities.Facility facility = null;
			if (bbFacility.Value != null)
				facility = new Sport.Entities.Facility(bbFacility.Value as Sport.Data.Entity);
			Sport.Entities.Court court = cbCourt.SelectedItem as Sport.Entities.Court;
			DateTime time = (dtpTime.Checked) ? dtpTime.Value : DateTime.MinValue;
		
			bool bClose = false;
			try
			{
				if (_heat != null)
					bClose = _heat.Set(time, facility, court);
				else
					bClose = _competition.Set(time, facility, court);
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("Phase not started") >= 0)
				{
					Sport.UI.MessageBox.Error("לא ניתן לערוך את התחרות: יש להגדיר שלב נוכחי", "עריכת תחרות");
					bClose = false;
				}
				else
				{
					throw ex;
				}
			}

			if (bClose)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}
	}
}
