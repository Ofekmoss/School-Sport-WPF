using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsfield
{
	/// <summary>
	/// Summary description for ChampionshipImportForm.
	/// </summary>
	public class ChampionshipImportForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelRegion;
		private System.Windows.Forms.ComboBox cbRegion;
		private System.Windows.Forms.ComboBox cbSport;
		private System.Windows.Forms.Label labelSport;
		private System.Windows.Forms.ComboBox cbChampionship;
		private System.Windows.Forms.Label labelChampionship;
		private System.Windows.Forms.ComboBox cbCategory;
		private System.Windows.Forms.Label labelCategory;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ChampionshipImportForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Sport.Data.Entity[] regions = Sport.Entities.Region.Type.GetEntities(null);
			cbRegion.Items.AddRange(regions);
			cbRegion.SelectedItem = new Sport.Entities.Region(Sport.Core.Session.Region);
			
			Sport.Data.Entity[] sports = Sport.Entities.Sport.Type.GetEntities(null);
			cbSport.Items.AddRange(sports);
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
			this.labelRegion = new System.Windows.Forms.Label();
			this.cbRegion = new System.Windows.Forms.ComboBox();
			this.cbSport = new System.Windows.Forms.ComboBox();
			this.labelSport = new System.Windows.Forms.Label();
			this.cbChampionship = new System.Windows.Forms.ComboBox();
			this.labelChampionship = new System.Windows.Forms.Label();
			this.cbCategory = new System.Windows.Forms.ComboBox();
			this.labelCategory = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelRegion
			// 
			this.labelRegion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRegion.Location = new System.Drawing.Point(184, 13);
			this.labelRegion.Name = "labelRegion";
			this.labelRegion.Size = new System.Drawing.Size(56, 16);
			this.labelRegion.TabIndex = 0;
			this.labelRegion.Text = "מחוז:";
			// 
			// cbRegion
			// 
			this.cbRegion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRegion.Location = new System.Drawing.Point(8, 8);
			this.cbRegion.Name = "cbRegion";
			this.cbRegion.Size = new System.Drawing.Size(168, 21);
			this.cbRegion.TabIndex = 1;
			this.cbRegion.SelectedIndexChanged += new System.EventHandler(this.cbRegion_SelectedIndexChanged);
			// 
			// cbSport
			// 
			this.cbSport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbSport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSport.Location = new System.Drawing.Point(8, 40);
			this.cbSport.Name = "cbSport";
			this.cbSport.Size = new System.Drawing.Size(168, 21);
			this.cbSport.TabIndex = 3;
			this.cbSport.SelectedIndexChanged += new System.EventHandler(this.cbSport_SelectedIndexChanged);
			// 
			// labelSport
			// 
			this.labelSport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSport.Location = new System.Drawing.Point(176, 45);
			this.labelSport.Name = "labelSport";
			this.labelSport.Size = new System.Drawing.Size(64, 16);
			this.labelSport.TabIndex = 2;
			this.labelSport.Text = "ענף ספורט:";
			// 
			// cbChampionship
			// 
			this.cbChampionship.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbChampionship.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbChampionship.Enabled = false;
			this.cbChampionship.Location = new System.Drawing.Point(8, 72);
			this.cbChampionship.Name = "cbChampionship";
			this.cbChampionship.Size = new System.Drawing.Size(168, 21);
			this.cbChampionship.TabIndex = 5;
			this.cbChampionship.SelectedIndexChanged += new System.EventHandler(this.cbChampionship_SelectedIndexChanged);
			// 
			// labelChampionship
			// 
			this.labelChampionship.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelChampionship.Location = new System.Drawing.Point(184, 77);
			this.labelChampionship.Name = "labelChampionship";
			this.labelChampionship.Size = new System.Drawing.Size(56, 16);
			this.labelChampionship.TabIndex = 4;
			this.labelChampionship.Text = "אליפות:";
			// 
			// cbCategory
			// 
			this.cbCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbCategory.Enabled = false;
			this.cbCategory.Location = new System.Drawing.Point(8, 104);
			this.cbCategory.Name = "cbCategory";
			this.cbCategory.Size = new System.Drawing.Size(168, 21);
			this.cbCategory.TabIndex = 7;
			this.cbCategory.SelectedIndexChanged += new System.EventHandler(this.cbCategory_SelectedIndexChanged);
			// 
			// labelCategory
			// 
			this.labelCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCategory.Location = new System.Drawing.Point(184, 109);
			this.labelCategory.Name = "labelCategory";
			this.labelCategory.Size = new System.Drawing.Size(56, 16);
			this.labelCategory.TabIndex = 6;
			this.labelCategory.Text = "קטגוריה:";
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Enabled = false;
			this.btnOk.Location = new System.Drawing.Point(8, 136);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 8;
			this.btnOk.Text = "אישור";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnCancel.Location = new System.Drawing.Point(88, 136);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "ביטול";
			// 
			// ChampionshipImportForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(248, 166);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.cbCategory);
			this.Controls.Add(this.labelCategory);
			this.Controls.Add(this.cbChampionship);
			this.Controls.Add(this.labelChampionship);
			this.Controls.Add(this.cbSport);
			this.Controls.Add(this.labelSport);
			this.Controls.Add(this.cbRegion);
			this.Controls.Add(this.labelRegion);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "ChampionshipImportForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "יבוא אליפות";
			this.ResumeLayout(false);

		}
		#endregion

		private void cbRegion_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnOk.Enabled = false;
			RefreshChampionships();
		}

		private void cbSport_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnOk.Enabled = false;
			RefreshChampionships();
		}

		private void RefreshChampionships()
		{
			Sport.Data.Entity region = cbRegion.SelectedItem as Sport.Data.Entity;
			Sport.Data.Entity sport = cbSport.SelectedItem as Sport.Data.Entity;

			cbChampionship.Items.Clear();
			cbCategory.Items.Clear();
			cbCategory.Enabled = false;

			if (sport == null || region == null)
			{
				cbChampionship.Enabled = false;
			}
			else
			{
				Sport.Data.EntityFilter filter=new Sport.Data.EntityFilter(
					(int) Sport.Entities.Championship.Fields.Sport, sport.Id);
				filter.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.Championship.Fields.Region, region.Id));
				filter.Add(Sport.Entities.Championship.CurrentSeasonFilter());
				Sport.Data.Entity[] championships = 
					Sport.Entities.Championship.Type.GetEntities(filter);
				cbChampionship.Items.AddRange(championships);
				cbChampionship.Enabled = true;
			}
		}

		private void cbChampionship_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnOk.Enabled = false;

			Sport.Data.Entity championship = cbChampionship.SelectedItem as Sport.Data.Entity;

			if (championship == null)
			{
				cbCategory.Enabled = false;
			}
			else
			{
				cbCategory.Enabled = true;
				cbCategory.Items.Clear();

				Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter((int) Sport.Entities.ChampionshipCategory.Fields.Championship, championship.Id);
				Sport.Data.Entity[] categories = Sport.Entities.ChampionshipCategory.Type.GetEntities(filter);
				cbCategory.Items.AddRange(categories);
			}
		}

		private void cbCategory_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnOk.Enabled = cbCategory.SelectedItem != null;
		}

		private Sport.Entities.ChampionshipCategory _championshipCategory;

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			_championshipCategory = new Sport.Entities.ChampionshipCategory(cbCategory.SelectedItem as Sport.Data.Entity);
		}
	
		public Sport.Entities.ChampionshipCategory ChampionshipCategory
		{
			get { return _championshipCategory; }
		}
	}
}
