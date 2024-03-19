using System;

namespace Sportsman.Forms
{
	public class CreateAccountForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RadioButton rbAccountName;
		private System.Windows.Forms.TextBox tbName;
		private Sport.UI.Controls.ButtonBox bbSchool;
		private Sport.UI.Controls.ThemeButton tbCancel;
		private Sport.UI.Controls.ThemeButton tbOk;
		private System.Windows.Forms.RadioButton rbSchool;
	
		private void InitializeComponent()
		{
			this.rbAccountName = new System.Windows.Forms.RadioButton();
			this.rbSchool = new System.Windows.Forms.RadioButton();
			this.tbName = new System.Windows.Forms.TextBox();
			this.bbSchool = new Sport.UI.Controls.ButtonBox();
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.tbOk = new Sport.UI.Controls.ThemeButton();
			this.SuspendLayout();
			// 
			// rbAccountName
			// 
			this.rbAccountName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rbAccountName.Checked = true;
			this.rbAccountName.Location = new System.Drawing.Point(240, 8);
			this.rbAccountName.Name = "rbAccountName";
			this.rbAccountName.Size = new System.Drawing.Size(80, 16);
			this.rbAccountName.TabIndex = 0;
			this.rbAccountName.TabStop = true;
			this.rbAccountName.Text = "שם חשבון:";
			this.rbAccountName.CheckedChanged += new System.EventHandler(this.rbAccountName_CheckedChanged);
			// 
			// rbSchool
			// 
			this.rbSchool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rbSchool.Location = new System.Drawing.Point(240, 56);
			this.rbSchool.Name = "rbSchool";
			this.rbSchool.Size = new System.Drawing.Size(80, 16);
			this.rbSchool.TabIndex = 1;
			this.rbSchool.Text = "בית ספר:";
			this.rbSchool.CheckedChanged += new System.EventHandler(this.rbSchool_CheckedChanged);
			// 
			// tbName
			// 
			this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbName.Location = new System.Drawing.Point(8, 24);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(280, 21);
			this.tbName.TabIndex = 2;
			this.tbName.Text = "";
			this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
			// 
			// bbSchool
			// 
			this.bbSchool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bbSchool.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.bbSchool.Enabled = false;
			this.bbSchool.Location = new System.Drawing.Point(8, 72);
			this.bbSchool.Name = "bbSchool";
			this.bbSchool.Size = new System.Drawing.Size(280, 21);
			this.bbSchool.TabIndex = 3;
			this.bbSchool.Value = null;
			this.bbSchool.ValueSelector = null;
			this.bbSchool.ValueChanged += new System.EventHandler(this.bbSchool_ValueChanged);
			// 
			// tbCancel
			// 
			this.tbCancel.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbCancel.AutoSize = false;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancel.Hue = 300F;
			this.tbCancel.Image = null;
			this.tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbCancel.ImageList = null;
			this.tbCancel.ImageSize = new System.Drawing.Size(0, 0);
			this.tbCancel.Location = new System.Drawing.Point(72, 104);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Saturation = 0.1F;
			this.tbCancel.Size = new System.Drawing.Size(56, 25);
			this.tbCancel.TabIndex = 11;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Transparent = System.Drawing.Color.Black;
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// tbOk
			// 
			this.tbOk.Alignment = System.Drawing.StringAlignment.Center;
			this.tbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbOk.AutoSize = false;
			this.tbOk.Enabled = false;
			this.tbOk.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbOk.Hue = 300F;
			this.tbOk.Image = null;
			this.tbOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbOk.ImageList = null;
			this.tbOk.ImageSize = new System.Drawing.Size(0, 0);
			this.tbOk.Location = new System.Drawing.Point(8, 104);
			this.tbOk.Name = "tbOk";
			this.tbOk.Saturation = 0.1F;
			this.tbOk.Size = new System.Drawing.Size(56, 25);
			this.tbOk.TabIndex = 10;
			this.tbOk.Text = "אישור";
			this.tbOk.Transparent = System.Drawing.Color.Black;
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// CreateAccountForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(328, 134);
			this.Controls.Add(this.tbCancel);
			this.Controls.Add(this.tbOk);
			this.Controls.Add(this.bbSchool);
			this.Controls.Add(this.tbName);
			this.Controls.Add(this.rbSchool);
			this.Controls.Add(this.rbAccountName);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "CreateAccountForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "יצירת חשבון";
			this.ResumeLayout(false);

		}
	
		private Sport.Entities.Region _region;

		private Sport.UI.EntitySelectionDialog schoolDialog;

		private Sport.Entities.Account _account;
		public Sport.Entities.Account Account
		{
			get { return _account; }
		}

		public CreateAccountForm(Sport.Entities.Region region)
		{
			InitializeComponent();

			Text = "יצירת חשבון - מחוז " + region.Name;

			_region = region;
			_account = null;


			Sport.UI.GeneralTableView schoolsTableView = 
				new Sport.UI.GeneralTableView("AccountSchoolSelection", new Entities.SchoolView(), 
				new int[] {
							  (int) Sport.Entities.School.Fields.Symbol,
							  (int) Sport.Entities.School.Fields.Name,
							  (int) Sport.Entities.School.Fields.City
						  },
				new int[] {
							  (int) Sport.Entities.School.Fields.Name
						  });

			schoolsTableView.EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.School.Fields.Region, region);
			schoolsTableView.EntityListView.EntityQuery.Parameters.Add((int) Sport.Entities.School.Fields.City, false);

			Sport.UI.TableView2.ComboBoxFilter filterCity = new Sport.UI.TableView2.ComboBoxFilter("עיר:", region.GetCities(), null, "<כל הערים>", 120);
			filterCity.Parameters.Add(schoolsTableView.EntityListView.EntityQuery[1]);
			schoolsTableView.Filters.Add(filterCity);
			schoolsTableView.Searchers.Add("סמל:", Sport.Entities.School.Type.Fields[(int) Sport.Entities.School.Fields.Symbol], 80);
			schoolsTableView.Searchers.Add("שם:", Sport.Entities.School.Type.Fields[(int) Sport.Entities.School.Fields.Name], 180);

			schoolDialog = new Sport.UI.EntitySelectionDialog(schoolsTableView);
			schoolDialog.View.State[Sport.Entities.Region.TypeName] = _region.Id.ToString();

			bbSchool.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(
				schoolDialog.ValueSelector);
		}

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			_account = null;
			Sport.Entities.School school = null;
			if (rbAccountName.Checked)
			{
				string strAccountName = tbName.Text;
				if (strAccountName == null || strAccountName.Length == 0)
				{
					Sport.UI.MessageBox.Error("אנא הזן שם חשבון", "יצירת חשבון");
					return;
				}
				Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter(
					(int) Sport.Entities.Account.Fields.Name, strAccountName);
				Sport.Data.Entity[] accounts = null;
				try
				{
					accounts = Sport.Entities.Account.Type.GetEntities(filter);
				}
				catch
				{}
				if (accounts != null && accounts.Length > 0)
				{
					Sport.UI.MessageBox.Error("חשבון עם זה זהה כבר קיים, לא ניתן ליצור חשבון חדש", "יצירת חשבון");
					return;
				}
			}
			
			if (rbSchool.Checked)
			{
				school = new Sport.Entities.School((Sport.Data.Entity) bbSchool.Value);

				_account = school.GetAccount();
			}

			if (_account == null)
			{
				_account = new Sport.Entities.Account(Sport.Entities.Account.Type.New());
				_account.AccountName = (school == null)?tbName.Text:school.Name+" (בית ספר)";
				_account.Region = _region;
				_account.School = school;
				if (school != null)
					_account.Address = school.Address;
				_account.Save();
			}

			DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			_account = null;
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}

		private void rbSchool_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbSchool.Checked)
			{
				tbName.Text = "בית ספר";
				bbSchool.Value = null;
				tbName.Enabled = false;
				bbSchool.Enabled = true;
				tbOk.Enabled = false;
			}
		}

		private void rbAccountName_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rbAccountName.Checked)
			{
				tbName.Text = null;
				bbSchool.Value = null;
				tbName.Enabled = true;
				bbSchool.Enabled = false;
				tbOk.Enabled = false;
			}
		}

		private void tbName_TextChanged(object sender, System.EventArgs e)
		{
			if (rbAccountName.Checked)
				tbOk.Enabled = tbName.Text.Length > 0;
		}

		private void bbSchool_ValueChanged(object sender, System.EventArgs e)
		{
			if (rbSchool.Checked)
				tbOk.Enabled = bbSchool.Value != null;
		}
	}
}
