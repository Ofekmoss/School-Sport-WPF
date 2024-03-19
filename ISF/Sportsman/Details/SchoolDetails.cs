using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Data;
using Sport.Common;

namespace Sportsman.Details
{
	/// <summary>
	/// Summary description for SchoolDetails.
	/// </summary>
	public class SchoolDetails : System.Windows.Forms.Form
	{
		private int _schoolID=-1;
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox edSchoolNumber;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private Sport.UI.Controls.ThemeButton btnTableView;
		private Sport.UI.Controls.ThemeButton btnClose;
		private System.Windows.Forms.TabControl tcDetails;
		private System.Windows.Forms.TabPage tpGeneral;
		private System.Windows.Forms.TabPage tpCharge;
		private System.Windows.Forms.TabPage tpTeams;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lbSchoolsCount;
		private System.Windows.Forms.ListBox lstTeams;
		private Sport.UI.Controls.ThemeButton btnTeamsView;
		private System.Windows.Forms.Label lbNumOfStudents;
		private System.Windows.Forms.Label lbPrinciple;
		private System.Windows.Forms.Label lbName;
		private System.Windows.Forms.Label lbTotalCharges;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lbTotalPayments;
		private System.Windows.Forms.Label lbBalance;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SchoolDetails()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Text = "פרטי בית הספר";
			
			System.Windows.Forms.ToolTip toolTip=new ToolTip();
			toolTip.SetToolTip(this.lbTotalCharges, "עבור אל מסך חיובים");
			
			toolTip=new ToolTip();
			toolTip.SetToolTip(this.lbTotalPayments, "עבור אל מסך תשלומים");
			
			if (Sport.UI.ViewManager.SelectedView is Sport.UI.TableView)
			{
				Sport.UI.TableView tableView=(Sport.UI.TableView) Sport.UI.ViewManager.SelectedView;
				if (tableView.EntityListView.EntityName == Sport.Entities.School.TypeName)
				{
					if (tableView.Current != null)
						this.EntityID = tableView.Current.Id;
				}
			}
		}
		
		public int EntityID
		{
			get { return _schoolID; }
			set
			{
				try
				{
					Sport.Entities.School school=new Sport.Entities.School(value);
					edSchoolNumber.Text = school.Symbol;
					//_schoolID = value;
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to create school details. ID: "+value.ToString());
					System.Diagnostics.Debug.WriteLine("error: "+e.Message);
					_schoolID = -1;
				}
			}
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
			this.edSchoolNumber = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnClose = new Sport.UI.Controls.ThemeButton();
			this.btnTableView = new Sport.UI.Controls.ThemeButton();
			this.tcDetails = new System.Windows.Forms.TabControl();
			this.tpGeneral = new System.Windows.Forms.TabPage();
			this.lbNumOfStudents = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbPrinciple = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tpCharge = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.lbBalance = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lbTotalPayments = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbTotalCharges = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.tpTeams = new System.Windows.Forms.TabPage();
			this.btnTeamsView = new Sport.UI.Controls.ThemeButton();
			this.lstTeams = new System.Windows.Forms.ListBox();
			this.lbSchoolsCount = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tcDetails.SuspendLayout();
			this.tpGeneral.SuspendLayout();
			this.tpCharge.SuspendLayout();
			this.tpTeams.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Right;
			this.label1.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(383, 0);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(33, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "סמל:";
			// 
			// edSchoolNumber
			// 
			this.edSchoolNumber.Location = new System.Drawing.Point(320, 0);
			this.edSchoolNumber.MaxLength = 7;
			this.edSchoolNumber.Name = "edSchoolNumber";
			this.edSchoolNumber.Size = new System.Drawing.Size(64, 20);
			this.edSchoolNumber.TabIndex = 0;
			this.edSchoolNumber.Text = "";
			this.edSchoolNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edSchoolNumber_KeyDown);
			this.edSchoolNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.edSchoolNumber_KeyPress);
			this.edSchoolNumber.TextChanged += new System.EventHandler(this.edSchoolNumber_TextChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.edSchoolNumber);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(416, 32);
			this.panel1.TabIndex = 2;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnClose);
			this.panel2.Controls.Add(this.btnTableView);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 197);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(416, 32);
			this.panel2.TabIndex = 3;
			// 
			// btnClose
			// 
			this.btnClose.Alignment = System.Drawing.StringAlignment.Center;
			this.btnClose.AutoSize = false;
			this.btnClose.Dock = System.Windows.Forms.DockStyle.Left;
			this.btnClose.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClose.Hue = 220F;
			this.btnClose.Image = null;
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnClose.ImageList = null;
			this.btnClose.ImageSize = new System.Drawing.Size(0, 0);
			this.btnClose.Location = new System.Drawing.Point(0, 0);
			this.btnClose.Name = "btnClose";
			this.btnClose.Saturation = 0.9F;
			this.btnClose.Size = new System.Drawing.Size(75, 32);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "סגור מסך פרטים";
			this.btnClose.Transparent = System.Drawing.Color.Black;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnTableView
			// 
			this.btnTableView.Alignment = System.Drawing.StringAlignment.Center;
			this.btnTableView.AutoSize = false;
			this.btnTableView.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnTableView.Enabled = false;
			this.btnTableView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnTableView.Hue = 220F;
			this.btnTableView.Image = null;
			this.btnTableView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnTableView.ImageList = null;
			this.btnTableView.ImageSize = new System.Drawing.Size(0, 0);
			this.btnTableView.Location = new System.Drawing.Point(341, 0);
			this.btnTableView.Name = "btnTableView";
			this.btnTableView.Saturation = 0.9F;
			this.btnTableView.Size = new System.Drawing.Size(75, 32);
			this.btnTableView.TabIndex = 2;
			this.btnTableView.Text = "עבור למסך בתי ספר";
			this.btnTableView.Transparent = System.Drawing.Color.Black;
			this.btnTableView.Click += new System.EventHandler(this.btnTableView_Click);
			// 
			// tcDetails
			// 
			this.tcDetails.Controls.Add(this.tpGeneral);
			this.tcDetails.Controls.Add(this.tpCharge);
			this.tcDetails.Controls.Add(this.tpTeams);
			this.tcDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcDetails.Location = new System.Drawing.Point(0, 32);
			this.tcDetails.Name = "tcDetails";
			this.tcDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tcDetails.SelectedIndex = 0;
			this.tcDetails.Size = new System.Drawing.Size(416, 165);
			this.tcDetails.TabIndex = 4;
			// 
			// tpGeneral
			// 
			this.tpGeneral.Controls.Add(this.lbNumOfStudents);
			this.tpGeneral.Controls.Add(this.label5);
			this.tpGeneral.Controls.Add(this.lbPrinciple);
			this.tpGeneral.Controls.Add(this.label4);
			this.tpGeneral.Controls.Add(this.lbName);
			this.tpGeneral.Controls.Add(this.label2);
			this.tpGeneral.Location = new System.Drawing.Point(4, 22);
			this.tpGeneral.Name = "tpGeneral";
			this.tpGeneral.Size = new System.Drawing.Size(408, 139);
			this.tpGeneral.TabIndex = 0;
			this.tpGeneral.Text = "כללי";
			// 
			// lbNumOfStudents
			// 
			this.lbNumOfStudents.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbNumOfStudents.ForeColor = System.Drawing.Color.Blue;
			this.lbNumOfStudents.Location = new System.Drawing.Point(216, 64);
			this.lbNumOfStudents.Name = "lbNumOfStudents";
			this.lbNumOfStudents.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbNumOfStudents.Size = new System.Drawing.Size(88, 19);
			this.lbNumOfStudents.TabIndex = 6;
			this.lbNumOfStudents.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label5.Location = new System.Drawing.Point(312, 64);
			this.label5.Name = "label5";
			this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label5.Size = new System.Drawing.Size(94, 19);
			this.label5.TabIndex = 5;
			this.label5.Text = "מספר תלמידים:";
			// 
			// lbPrinciple
			// 
			this.lbPrinciple.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbPrinciple.ForeColor = System.Drawing.Color.Blue;
			this.lbPrinciple.Location = new System.Drawing.Point(56, 64);
			this.lbPrinciple.Name = "lbPrinciple";
			this.lbPrinciple.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbPrinciple.Size = new System.Drawing.Size(88, 19);
			this.lbPrinciple.TabIndex = 4;
			this.lbPrinciple.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(152, 64);
			this.label4.Name = "label4";
			this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label4.Size = new System.Drawing.Size(39, 19);
			this.label4.TabIndex = 3;
			this.label4.Text = "מנהל:";
			// 
			// lbName
			// 
			this.lbName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbName.ForeColor = System.Drawing.Color.Blue;
			this.lbName.Location = new System.Drawing.Point(56, 16);
			this.lbName.Name = "lbName";
			this.lbName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbName.Size = new System.Drawing.Size(248, 19);
			this.lbName.TabIndex = 2;
			this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(376, 16);
			this.label2.Name = "label2";
			this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label2.Size = new System.Drawing.Size(28, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "שם:";
			// 
			// tpCharge
			// 
			this.tpCharge.Controls.Add(this.label3);
			this.tpCharge.Controls.Add(this.lbBalance);
			this.tpCharge.Controls.Add(this.label8);
			this.tpCharge.Controls.Add(this.lbTotalPayments);
			this.tpCharge.Controls.Add(this.label7);
			this.tpCharge.Controls.Add(this.lbTotalCharges);
			this.tpCharge.Controls.Add(this.label6);
			this.tpCharge.Location = new System.Drawing.Point(4, 22);
			this.tpCharge.Name = "tpCharge";
			this.tpCharge.Size = new System.Drawing.Size(408, 139);
			this.tpCharge.TabIndex = 1;
			this.tpCharge.Text = "חיובים";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label3.Location = new System.Drawing.Point(184, 112);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label3.Size = new System.Drawing.Size(202, 19);
			this.label3.TabIndex = 9;
			this.label3.Text = "(כל המחירים נקובים בשקלים חדשים)";
			// 
			// lbBalance
			// 
			this.lbBalance.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbBalance.ForeColor = System.Drawing.Color.Blue;
			this.lbBalance.Location = new System.Drawing.Point(224, 80);
			this.lbBalance.Name = "lbBalance";
			this.lbBalance.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbBalance.Size = new System.Drawing.Size(80, 19);
			this.lbBalance.TabIndex = 8;
			this.lbBalance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label8.Location = new System.Drawing.Point(360, 80);
			this.label8.Name = "label8";
			this.label8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label8.Size = new System.Drawing.Size(38, 19);
			this.label8.TabIndex = 7;
			this.label8.Text = "יתרה:";
			// 
			// lbTotalPayments
			// 
			this.lbTotalPayments.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lbTotalPayments.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTotalPayments.ForeColor = System.Drawing.Color.Blue;
			this.lbTotalPayments.Location = new System.Drawing.Point(224, 48);
			this.lbTotalPayments.Name = "lbTotalPayments";
			this.lbTotalPayments.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbTotalPayments.Size = new System.Drawing.Size(80, 19);
			this.lbTotalPayments.TabIndex = 6;
			this.lbTotalPayments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbTotalPayments.Click += new System.EventHandler(this.lbTotalPayments_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label7.Location = new System.Drawing.Point(304, 48);
			this.label7.Name = "label7";
			this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label7.Size = new System.Drawing.Size(94, 19);
			this.label7.TabIndex = 5;
			this.label7.Text = "סה\"כ תשלומים:";
			// 
			// lbTotalCharges
			// 
			this.lbTotalCharges.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lbTotalCharges.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTotalCharges.ForeColor = System.Drawing.Color.Blue;
			this.lbTotalCharges.Location = new System.Drawing.Point(224, 16);
			this.lbTotalCharges.Name = "lbTotalCharges";
			this.lbTotalCharges.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbTotalCharges.Size = new System.Drawing.Size(80, 19);
			this.lbTotalCharges.TabIndex = 4;
			this.lbTotalCharges.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lbTotalCharges.Click += new System.EventHandler(this.lbTotalCharges_Click);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label6.Location = new System.Drawing.Point(320, 16);
			this.label6.Name = "label6";
			this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label6.Size = new System.Drawing.Size(79, 19);
			this.label6.TabIndex = 3;
			this.label6.Text = "סה\"כ חיובים:";
			// 
			// tpTeams
			// 
			this.tpTeams.Controls.Add(this.btnTeamsView);
			this.tpTeams.Controls.Add(this.lstTeams);
			this.tpTeams.Location = new System.Drawing.Point(4, 22);
			this.tpTeams.Name = "tpTeams";
			this.tpTeams.Size = new System.Drawing.Size(408, 139);
			this.tpTeams.TabIndex = 2;
			this.tpTeams.Text = "קבוצות";
			// 
			// btnTeamsView
			// 
			this.btnTeamsView.Alignment = System.Drawing.StringAlignment.Center;
			this.btnTeamsView.AutoSize = false;
			this.btnTeamsView.Enabled = false;
			this.btnTeamsView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnTeamsView.Hue = 160F;
			this.btnTeamsView.Image = null;
			this.btnTeamsView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnTeamsView.ImageList = null;
			this.btnTeamsView.ImageSize = new System.Drawing.Size(0, 0);
			this.btnTeamsView.Location = new System.Drawing.Point(320, 48);
			this.btnTeamsView.Name = "btnTeamsView";
			this.btnTeamsView.Saturation = 0.6F;
			this.btnTeamsView.Size = new System.Drawing.Size(75, 32);
			this.btnTeamsView.TabIndex = 3;
			this.btnTeamsView.Text = "עבור למסך קבוצה";
			this.btnTeamsView.Transparent = System.Drawing.Color.Black;
			this.btnTeamsView.Click += new System.EventHandler(this.btnTeamsView_Click);
			// 
			// lstTeams
			// 
			this.lstTeams.Location = new System.Drawing.Point(16, 16);
			this.lstTeams.Name = "lstTeams";
			this.lstTeams.Size = new System.Drawing.Size(288, 95);
			this.lstTeams.TabIndex = 0;
			this.lstTeams.SelectedIndexChanged += new System.EventHandler(this.lstTeams_SelectedIndexChanged);
			// 
			// lbSchoolsCount
			// 
			this.lbSchoolsCount.Location = new System.Drawing.Point(0, 0);
			this.lbSchoolsCount.Name = "lbSchoolsCount";
			this.lbSchoolsCount.TabIndex = 0;
			// 
			// SchoolDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 229);
			this.Controls.Add(this.tcDetails);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "SchoolDetails";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SchoolDetails";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.tcDetails.ResumeLayout(false);
			this.tpGeneral.ResumeLayout(false);
			this.tpCharge.ResumeLayout(false);
			this.tpTeams.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void edSchoolNumber_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//e.Handled = true;
		}

		private void edSchoolNumber_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

		}

		private void RefreshDetails()
		{
			//decide what to do based on the current id
			btnTableView.Enabled = (_schoolID >= 0);
			btnTeamsView.Enabled = false;
			Sport.Entities.School school=null;
			if (_schoolID >= 0)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
				school=new Sport.Entities.School(_schoolID);
				lbName.Text = school.Name;
				lbPrinciple.Text = school.ManagerName;
				lbNumOfStudents.Text = school.StudentsCount.ToString();
				double totCharges=school.TotalCharges;
				double totPayments=school.TotalPayments;
				double balance=totPayments-totCharges;
				lbTotalCharges.Text = 
					Sport.Common.Tools.FormatNumber(totCharges, 2, true);
				lbTotalPayments.Text = 
					Sport.Common.Tools.FormatNumber(totPayments, 2, true);
				lbBalance.Text = 
					Sport.Common.Tools.FormatNumber(balance, 2, true);
				if (balance > 0)
				{
					lbBalance.ForeColor = Color.LightGreen;
				}
				else
				{
					if (balance < 0)
					{
						lbBalance.ForeColor = Color.Red;
					}
					else
					{
						lbBalance.ForeColor = Color.Black;
					}
				}
				FillTeams(school);
				Sport.UI.Dialogs.WaitForm.HideWait();
			}
			else
			{
				lbName.Text = "";
				lbPrinciple.Text = "";
				lbNumOfStudents.Text = "";
				lbTotalCharges.Text = "";
				lbTotalPayments.Text = "";
				lbBalance.Text = "";
				lstTeams.Items.Clear();
			}
			string caption="פרטי בית הספר";
			if (_schoolID >= 0)
			{
				caption += " - "+school.Name;
			}
			this.Text = caption;
		}
		
		private void FillTeams(Sport.Entities.School school)
		{
			lstTeams.Items.Clear();
			EntityFilter filter=new EntityFilter((int) Sport.Entities.Team.Fields.School, school.Id);
			Entity[] teams=Sport.Entities.Team.Type.GetEntities(filter);
			foreach (Entity ent in teams)
			{
				Sport.Entities.Team team=new Sport.Entities.Team(ent);
				lstTeams.Items.Add(new Sport.Common.ListItem(team.Name, team.Id));
			}
		}
		
		private void edSchoolNumber_TextChanged(object sender, System.EventArgs e)
		{
			if (edSchoolNumber.Text.Length >= 6)
			{
				string strSymbol=edSchoolNumber.Text;
				EntityFilter filter=new EntityFilter(
					(int) Sport.Entities.School.Fields.Symbol, strSymbol);
				Sport.Data.Entity[] schools=Sport.Entities.School.Type.GetEntities(filter);
				if (schools.Length > 0)
				{
					_schoolID = schools[0].Id;	
				}
				else
				{
					_schoolID = -1;
				}
			}
			else
			{
				_schoolID = -1;
			}
			
			RefreshDetails();
		}
		
		private void lstTeams_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnTeamsView.Enabled = (lstTeams.SelectedIndex >= 0);
		}

		private void btnTeamsView_Click(object sender, System.EventArgs e)
		{
			int selIndex=lstTeams.SelectedIndex;
			if (selIndex < 0)
				return;
			ListItem selectedItem=(ListItem) lstTeams.Items[selIndex];
			int teamID=(int) selectedItem.Value;
			Sport.UI.ViewManager.OpenView(typeof(Views.TeamsTableView), "team="+teamID.ToString());
			this.DialogResult = DialogResult.OK;
		}

		private void btnTableView_Click(object sender, System.EventArgs e)
		{
			if (_schoolID >= 0)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.SchoolsTableView), "school="+_schoolID.ToString());
				this.DialogResult = DialogResult.OK;
			}
		}

		private void lbTotalCharges_Click(object sender, System.EventArgs e)
		{
			if (_schoolID >= 0)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.ChargesTableView), "school="+_schoolID.ToString());
				this.DialogResult = DialogResult.OK;
			}
		}

		private void lbTotalPayments_Click(object sender, System.EventArgs e)
		{
			if (_schoolID >= 0)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.PaymentsTableView), "school="+_schoolID.ToString());
				this.DialogResult = DialogResult.OK;
			}
		}
	} //end class SchoolDetails
}
