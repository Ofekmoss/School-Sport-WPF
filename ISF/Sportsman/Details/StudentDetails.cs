using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Data;
using Sport.Common;

//208896944
//201581352
//302471958

namespace Sportsman.Details
{
	/// <summary>
	/// Summary description for StudentDetails.
	/// </summary>
	public class StudentDetails : System.Windows.Forms.Form
	{
		private int _studentID=-1;
		private string[] _searchResults=null;
		private int _currentIndex=-1;
		
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox edStudentNumber;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private Sport.UI.Controls.ThemeButton btnTableView;
		private Sport.UI.Controls.ThemeButton btnClose;
		private System.Windows.Forms.TabControl tcDetails;
		private System.Windows.Forms.TabPage tpGeneral;
		private System.Windows.Forms.TabPage tpSchool;
		private System.Windows.Forms.TabPage tpTeams;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label lbFirstName;
		private System.Windows.Forms.Label lbLastName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lbBirthDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lbSchoolSymbol;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lbSchoolName;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lbStudentsCount;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ListBox lstTeams;
		private Sport.UI.Controls.ThemeButton btnTeamsView;
		private System.Windows.Forms.TextBox edFirstName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Panel pnlMultiply;
		private Sport.UI.Controls.ThemeButton btnPrevious;
		private System.Windows.Forms.Label lbCurrentIndex;
		private Sport.UI.Controls.ThemeButton btnNext;
		private Sport.UI.Controls.ThemeButton btnSearch;
		private System.Windows.Forms.TextBox edLastName;
		private System.Windows.Forms.Label lbSchoolRegion;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label lbSchoolCity;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label lbGrade;
		private System.Windows.Forms.Label label13;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StudentDetails()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			/*
			foreach (string caption in captions)
			{
				TabPage page=new TabPage(caption);
				tcDetails.TabPages.Add(page);
			}
			*/

			if (Sport.UI.ViewManager.SelectedView is Sport.UI.TableView)
			{
				Sport.UI.TableView tableView=(Sport.UI.TableView) Sport.UI.ViewManager.SelectedView;
				if (tableView.EntityListView.EntityName == Sport.Entities.Student.TypeName)
				{
					if (tableView.Current != null)
						this.EntityID = tableView.Current.Id;
				}
			}

			this.Text = "פרטי תלמיד";
		}
		
		public int EntityID
		{
			get { return _studentID; }
			set
			{
				try
				{
					Sport.Entities.Student student=new Sport.Entities.Student(value);
					edStudentNumber.Text = student.IdNumber;
					//_studentID = value;
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to create student details. ID: "+value.ToString());
					System.Diagnostics.Debug.WriteLine("error: "+e.Message);
					_studentID = -1;
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
			this.edStudentNumber = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnlMultiply = new System.Windows.Forms.Panel();
			this.btnNext = new Sport.UI.Controls.ThemeButton();
			this.lbCurrentIndex = new System.Windows.Forms.Label();
			this.btnPrevious = new Sport.UI.Controls.ThemeButton();
			this.btnSearch = new Sport.UI.Controls.ThemeButton();
			this.edLastName = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.edFirstName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnClose = new Sport.UI.Controls.ThemeButton();
			this.btnTableView = new Sport.UI.Controls.ThemeButton();
			this.tcDetails = new System.Windows.Forms.TabControl();
			this.tpGeneral = new System.Windows.Forms.TabPage();
			this.lbGrade = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.lbBirthDate = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbLastName = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbFirstName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tpSchool = new System.Windows.Forms.TabPage();
			this.lbSchoolCity = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.lbSchoolRegion = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.lbStudentsCount = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbSchoolSymbol = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lbSchoolName = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.tpTeams = new System.Windows.Forms.TabPage();
			this.btnTeamsView = new Sport.UI.Controls.ThemeButton();
			this.lstTeams = new System.Windows.Forms.ListBox();
			this.panel1.SuspendLayout();
			this.pnlMultiply.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tcDetails.SuspendLayout();
			this.tpGeneral.SuspendLayout();
			this.tpSchool.SuspendLayout();
			this.tpTeams.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(349, 0);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(67, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "מספר ת\"ז:";
			// 
			// edStudentNumber
			// 
			this.edStudentNumber.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.edStudentNumber.ForeColor = System.Drawing.Color.Blue;
			this.edStudentNumber.Location = new System.Drawing.Point(253, 0);
			this.edStudentNumber.MaxLength = 10;
			this.edStudentNumber.Name = "edStudentNumber";
			this.edStudentNumber.Size = new System.Drawing.Size(96, 21);
			this.edStudentNumber.TabIndex = 1;
			this.edStudentNumber.Text = "";
			this.edStudentNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edStudentNumber_KeyDown);
			this.edStudentNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.edStudentNumber_KeyPress);
			this.edStudentNumber.TextChanged += new System.EventHandler(this.edStudentNumber_TextChanged);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.pnlMultiply);
			this.panel1.Controls.Add(this.btnSearch);
			this.panel1.Controls.Add(this.edLastName);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.edFirstName);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.edStudentNumber);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(416, 64);
			this.panel1.TabIndex = 2;
			// 
			// pnlMultiply
			// 
			this.pnlMultiply.Controls.Add(this.btnNext);
			this.pnlMultiply.Controls.Add(this.lbCurrentIndex);
			this.pnlMultiply.Controls.Add(this.btnPrevious);
			this.pnlMultiply.Location = new System.Drawing.Point(64, 8);
			this.pnlMultiply.Name = "pnlMultiply";
			this.pnlMultiply.Size = new System.Drawing.Size(176, 16);
			this.pnlMultiply.TabIndex = 6;
			this.pnlMultiply.Visible = false;
			// 
			// btnNext
			// 
			this.btnNext.Alignment = System.Drawing.StringAlignment.Center;
			this.btnNext.AutoSize = false;
			this.btnNext.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnNext.Enabled = false;
			this.btnNext.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnNext.Hue = 10F;
			this.btnNext.Image = null;
			this.btnNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnNext.ImageList = null;
			this.btnNext.ImageSize = new System.Drawing.Size(0, 0);
			this.btnNext.Location = new System.Drawing.Point(56, 0);
			this.btnNext.Name = "btnNext";
			this.btnNext.Saturation = 0.9F;
			this.btnNext.Size = new System.Drawing.Size(48, 16);
			this.btnNext.TabIndex = 8;
			this.btnNext.Text = "הבא";
			this.btnNext.Transparent = System.Drawing.Color.Black;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// lbCurrentIndex
			// 
			this.lbCurrentIndex.AutoSize = true;
			this.lbCurrentIndex.Dock = System.Windows.Forms.DockStyle.Right;
			this.lbCurrentIndex.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbCurrentIndex.ForeColor = System.Drawing.Color.Blue;
			this.lbCurrentIndex.Location = new System.Drawing.Point(104, 0);
			this.lbCurrentIndex.Name = "lbCurrentIndex";
			this.lbCurrentIndex.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbCurrentIndex.Size = new System.Drawing.Size(24, 19);
			this.lbCurrentIndex.TabIndex = 7;
			this.lbCurrentIndex.Text = "0/0";
			// 
			// btnPrevious
			// 
			this.btnPrevious.Alignment = System.Drawing.StringAlignment.Center;
			this.btnPrevious.AutoSize = false;
			this.btnPrevious.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnPrevious.Enabled = false;
			this.btnPrevious.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnPrevious.Hue = 10F;
			this.btnPrevious.Image = null;
			this.btnPrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnPrevious.ImageList = null;
			this.btnPrevious.ImageSize = new System.Drawing.Size(0, 0);
			this.btnPrevious.Location = new System.Drawing.Point(128, 0);
			this.btnPrevious.Name = "btnPrevious";
			this.btnPrevious.Saturation = 0.9F;
			this.btnPrevious.Size = new System.Drawing.Size(48, 16);
			this.btnPrevious.TabIndex = 6;
			this.btnPrevious.Text = "הקודם";
			this.btnPrevious.Transparent = System.Drawing.Color.Black;
			this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
			// 
			// btnSearch
			// 
			this.btnSearch.Alignment = System.Drawing.StringAlignment.Center;
			this.btnSearch.AutoSize = false;
			this.btnSearch.Enabled = false;
			this.btnSearch.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnSearch.Hue = 220F;
			this.btnSearch.Image = null;
			this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSearch.ImageList = null;
			this.btnSearch.ImageSize = new System.Drawing.Size(0, 0);
			this.btnSearch.Location = new System.Drawing.Point(64, 32);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Saturation = 0.9F;
			this.btnSearch.Size = new System.Drawing.Size(48, 24);
			this.btnSearch.TabIndex = 5;
			this.btnSearch.Text = "חיפוש";
			this.btnSearch.Transparent = System.Drawing.Color.Black;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// edLastName
			// 
			this.edLastName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.edLastName.ForeColor = System.Drawing.Color.Blue;
			this.edLastName.Location = new System.Drawing.Point(120, 32);
			this.edLastName.MaxLength = 10;
			this.edLastName.Name = "edLastName";
			this.edLastName.Size = new System.Drawing.Size(72, 21);
			this.edLastName.TabIndex = 3;
			this.edLastName.Text = "";
			this.edLastName.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label8.Location = new System.Drawing.Point(200, 32);
			this.label8.Name = "label8";
			this.label8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label8.Size = new System.Drawing.Size(75, 19);
			this.label8.TabIndex = 4;
			this.label8.Text = "שם משפחה:";
			// 
			// edFirstName
			// 
			this.edFirstName.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.edFirstName.ForeColor = System.Drawing.Color.Blue;
			this.edFirstName.Location = new System.Drawing.Point(280, 32);
			this.edFirstName.MaxLength = 10;
			this.edFirstName.Name = "edFirstName";
			this.edFirstName.Size = new System.Drawing.Size(64, 21);
			this.edFirstName.TabIndex = 2;
			this.edFirstName.Text = "";
			this.edFirstName.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label3.Location = new System.Drawing.Point(344, 32);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label3.Size = new System.Drawing.Size(59, 19);
			this.label3.TabIndex = 2;
			this.label3.Text = "שם פרטי:";
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnClose);
			this.panel2.Controls.Add(this.btnTableView);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 253);
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
			this.btnTableView.Text = "עבור למסך תלמידים";
			this.btnTableView.Transparent = System.Drawing.Color.Black;
			this.btnTableView.Click += new System.EventHandler(this.btnTableView_Click);
			// 
			// tcDetails
			// 
			this.tcDetails.Controls.Add(this.tpGeneral);
			this.tcDetails.Controls.Add(this.tpSchool);
			this.tcDetails.Controls.Add(this.tpTeams);
			this.tcDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcDetails.Location = new System.Drawing.Point(0, 64);
			this.tcDetails.Name = "tcDetails";
			this.tcDetails.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tcDetails.SelectedIndex = 0;
			this.tcDetails.Size = new System.Drawing.Size(416, 189);
			this.tcDetails.TabIndex = 4;
			// 
			// tpGeneral
			// 
			this.tpGeneral.Controls.Add(this.lbGrade);
			this.tpGeneral.Controls.Add(this.label13);
			this.tpGeneral.Controls.Add(this.lbBirthDate);
			this.tpGeneral.Controls.Add(this.label5);
			this.tpGeneral.Controls.Add(this.lbLastName);
			this.tpGeneral.Controls.Add(this.label4);
			this.tpGeneral.Controls.Add(this.lbFirstName);
			this.tpGeneral.Controls.Add(this.label2);
			this.tpGeneral.Location = new System.Drawing.Point(4, 22);
			this.tpGeneral.Name = "tpGeneral";
			this.tpGeneral.Size = new System.Drawing.Size(408, 163);
			this.tpGeneral.TabIndex = 0;
			this.tpGeneral.Text = "כללי";
			// 
			// lbGrade
			// 
			this.lbGrade.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbGrade.ForeColor = System.Drawing.Color.Blue;
			this.lbGrade.Location = new System.Drawing.Point(216, 112);
			this.lbGrade.Name = "lbGrade";
			this.lbGrade.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbGrade.Size = new System.Drawing.Size(88, 19);
			this.lbGrade.TabIndex = 8;
			this.lbGrade.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label13.Location = new System.Drawing.Point(344, 112);
			this.label13.Name = "label13";
			this.label13.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label13.Size = new System.Drawing.Size(41, 19);
			this.label13.TabIndex = 7;
			this.label13.Text = "כיתה: ";
			// 
			// lbBirthDate
			// 
			this.lbBirthDate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbBirthDate.ForeColor = System.Drawing.Color.Blue;
			this.lbBirthDate.Location = new System.Drawing.Point(216, 64);
			this.lbBirthDate.Name = "lbBirthDate";
			this.lbBirthDate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbBirthDate.Size = new System.Drawing.Size(88, 19);
			this.lbBirthDate.TabIndex = 6;
			this.lbBirthDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label5.Location = new System.Drawing.Point(312, 64);
			this.label5.Name = "label5";
			this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label5.Size = new System.Drawing.Size(75, 19);
			this.label5.TabIndex = 5;
			this.label5.Text = "תאריך לידה:";
			// 
			// lbLastName
			// 
			this.lbLastName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbLastName.ForeColor = System.Drawing.Color.Blue;
			this.lbLastName.Location = new System.Drawing.Point(24, 16);
			this.lbLastName.Name = "lbLastName";
			this.lbLastName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbLastName.Size = new System.Drawing.Size(88, 19);
			this.lbLastName.TabIndex = 4;
			this.lbLastName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(120, 16);
			this.label4.Name = "label4";
			this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label4.Size = new System.Drawing.Size(75, 19);
			this.label4.TabIndex = 3;
			this.label4.Text = "שם משפחה:";
			// 
			// lbFirstName
			// 
			this.lbFirstName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbFirstName.ForeColor = System.Drawing.Color.Blue;
			this.lbFirstName.Location = new System.Drawing.Point(216, 16);
			this.lbFirstName.Name = "lbFirstName";
			this.lbFirstName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbFirstName.Size = new System.Drawing.Size(88, 19);
			this.lbFirstName.TabIndex = 2;
			this.lbFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(328, 16);
			this.label2.Name = "label2";
			this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label2.Size = new System.Drawing.Size(59, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "שם פרטי:";
			// 
			// tpSchool
			// 
			this.tpSchool.Controls.Add(this.lbSchoolCity);
			this.tpSchool.Controls.Add(this.label12);
			this.tpSchool.Controls.Add(this.lbSchoolRegion);
			this.tpSchool.Controls.Add(this.label11);
			this.tpSchool.Controls.Add(this.lbStudentsCount);
			this.tpSchool.Controls.Add(this.label7);
			this.tpSchool.Controls.Add(this.lbSchoolSymbol);
			this.tpSchool.Controls.Add(this.label6);
			this.tpSchool.Controls.Add(this.lbSchoolName);
			this.tpSchool.Controls.Add(this.label10);
			this.tpSchool.Location = new System.Drawing.Point(4, 22);
			this.tpSchool.Name = "tpSchool";
			this.tpSchool.Size = new System.Drawing.Size(408, 163);
			this.tpSchool.TabIndex = 1;
			this.tpSchool.Text = "בית ספר";
			// 
			// lbSchoolCity
			// 
			this.lbSchoolCity.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSchoolCity.ForeColor = System.Drawing.Color.Blue;
			this.lbSchoolCity.Location = new System.Drawing.Point(16, 96);
			this.lbSchoolCity.Name = "lbSchoolCity";
			this.lbSchoolCity.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbSchoolCity.Size = new System.Drawing.Size(120, 19);
			this.lbSchoolCity.TabIndex = 18;
			this.lbSchoolCity.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label12.Location = new System.Drawing.Point(144, 96);
			this.label12.Name = "label12";
			this.label12.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label12.Size = new System.Drawing.Size(42, 19);
			this.label12.TabIndex = 17;
			this.label12.Text = "יישוב: ";
			// 
			// lbSchoolRegion
			// 
			this.lbSchoolRegion.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSchoolRegion.ForeColor = System.Drawing.Color.Blue;
			this.lbSchoolRegion.Location = new System.Drawing.Point(16, 56);
			this.lbSchoolRegion.Name = "lbSchoolRegion";
			this.lbSchoolRegion.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbSchoolRegion.Size = new System.Drawing.Size(120, 19);
			this.lbSchoolRegion.TabIndex = 16;
			this.lbSchoolRegion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label11.Location = new System.Drawing.Point(144, 56);
			this.label11.Name = "label11";
			this.label11.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label11.Size = new System.Drawing.Size(40, 19);
			this.label11.TabIndex = 15;
			this.label11.Text = "מחוז: ";
			// 
			// lbStudentsCount
			// 
			this.lbStudentsCount.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount.Location = new System.Drawing.Point(232, 96);
			this.lbStudentsCount.Name = "lbStudentsCount";
			this.lbStudentsCount.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbStudentsCount.Size = new System.Drawing.Size(56, 19);
			this.lbStudentsCount.TabIndex = 14;
			this.lbStudentsCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label7.Location = new System.Drawing.Point(304, 96);
			this.label7.Name = "label7";
			this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label7.Size = new System.Drawing.Size(94, 19);
			this.label7.TabIndex = 13;
			this.label7.Text = "מספר תלמידים:";
			// 
			// lbSchoolSymbol
			// 
			this.lbSchoolSymbol.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSchoolSymbol.ForeColor = System.Drawing.Color.Blue;
			this.lbSchoolSymbol.Location = new System.Drawing.Point(200, 56);
			this.lbSchoolSymbol.Name = "lbSchoolSymbol";
			this.lbSchoolSymbol.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbSchoolSymbol.Size = new System.Drawing.Size(88, 19);
			this.lbSchoolSymbol.TabIndex = 12;
			this.lbSchoolSymbol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label6.Location = new System.Drawing.Point(304, 56);
			this.label6.Name = "label6";
			this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label6.Size = new System.Drawing.Size(86, 19);
			this.label6.TabIndex = 11;
			this.label6.Text = "סמל בית ספר:";
			// 
			// lbSchoolName
			// 
			this.lbSchoolName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSchoolName.ForeColor = System.Drawing.Color.Blue;
			this.lbSchoolName.Location = new System.Drawing.Point(72, 16);
			this.lbSchoolName.Name = "lbSchoolName";
			this.lbSchoolName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.lbSchoolName.Size = new System.Drawing.Size(216, 19);
			this.lbSchoolName.TabIndex = 8;
			this.lbSchoolName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Arial", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label10.Location = new System.Drawing.Point(304, 16);
			this.label10.Name = "label10";
			this.label10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label10.Size = new System.Drawing.Size(80, 19);
			this.label10.TabIndex = 7;
			this.label10.Text = "שם בית ספר:";
			// 
			// tpTeams
			// 
			this.tpTeams.Controls.Add(this.btnTeamsView);
			this.tpTeams.Controls.Add(this.lstTeams);
			this.tpTeams.Location = new System.Drawing.Point(4, 22);
			this.tpTeams.Name = "tpTeams";
			this.tpTeams.Size = new System.Drawing.Size(408, 163);
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
			// StudentDetails
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 285);
			this.Controls.Add(this.tcDetails);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "StudentDetails";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "StudentDetails";
			this.panel1.ResumeLayout(false);
			this.pnlMultiply.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.tcDetails.ResumeLayout(false);
			this.tpGeneral.ResumeLayout(false);
			this.tpSchool.ResumeLayout(false);
			this.tpTeams.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void edStudentNumber_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//e.Handled = true;
		}

		private void edStudentNumber_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

		}

		private void RefreshDetails()
		{
			//decide what to do based on the current id
			btnTableView.Enabled = (_studentID >= 0);
			btnTeamsView.Enabled = false;
			if (_studentID >= 0)
			{
				Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...", true);
				
				Sport.Entities.Student student=new Sport.Entities.Student(_studentID);
				lbFirstName.Text = student.FirstName;
				lbLastName.Text = student.LastName;
				lbBirthDate.Text = student.BirthDate.ToShortDateString();
				lbGrade.Text = (new Sport.Types.GradeTypeLookup(true)).Lookup(student.Grade);
				lbSchoolName.Text = student.School.Name;
				lbSchoolSymbol.Text = student.School.Symbol;
				lbStudentsCount.Text = student.School.StudentsCount.ToString();
				//System.Windows.Forms.MessageBox.Show(lbStudentsCount.Text);
				lbSchoolRegion.Text = student.School.Region.Name;
				lbSchoolCity.Text = student.School.City.Name;
				FillTeams(student);
				
				Sport.UI.Dialogs.WaitForm.HideWait();
			}
			else
			{
				lbFirstName.Text = "";
				lbLastName.Text = "";
				lbBirthDate.Text = "";
				lbSchoolName.Text = "";
				lbSchoolSymbol.Text = "";
				lbStudentsCount.Text = "";
				lstTeams.Items.Clear();
			}
			this.Text = "פרטי תלמיד";
			if (_studentID >= 0)
				this.Text += " - "+Sport.Entities.Student.Type.Lookup(_studentID).Name;
		}

		private void FillTeams(Sport.Entities.Student student)
		{
			lstTeams.Items.Clear();
			EntityFilter filter=new EntityFilter((int) Sport.Entities.Player.Fields.Student, student.Id);
			Entity[] players=Sport.Entities.Player.Type.GetEntities(filter);
			foreach (Entity ent in players)
			{
				Sport.Entities.Player player=new Sport.Entities.Player(ent);
				lstTeams.Items.Add(new Sport.Common.ListItem(player.Team.Name, player.Team.Id));
			}
		}

		private void edStudentNumber_TextChanged(object sender, System.EventArgs e)
		{
			if (edStudentNumber.Text.Length >= 5)
			{
				int studentNumber=Sport.Common.Tools.CIntDef(edStudentNumber.Text, -1);
				if (studentNumber < 0)
				{
					_studentID = -1;
				}
				else
				{
					Cursor oldCursor=Cursor.Current;
					Cursor.Current = Cursors.WaitCursor;
					int carretPos=edStudentNumber.SelectionStart;
					edStudentNumber.Enabled = false;
					Sport.UI.Dialogs.WaitForm.ShowWait("בודק מספר זהות אנא המתן...");
					EntityFilter filter=new EntityFilter((int) Sport.Entities.Student.Fields.IdNumber, studentNumber);
					Sport.Data.Entity[] students=Sport.Entities.Student.Type.GetEntities(filter);
					if (students.Length > 0)
					{
						_studentID = students[0].Id;	
					}
					else
					{
						_studentID = -1;
					}
					Sport.UI.Dialogs.WaitForm.HideWait();
					edStudentNumber.Enabled = true;
					Cursor.Current = oldCursor;
					edStudentNumber.Focus();
					edStudentNumber.SelectionStart = carretPos;
				}
			}
			else
			{
				_studentID = -1;
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
			if (_studentID >= 0)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.StudentsTableView), "student="+_studentID.ToString());
				this.DialogResult = DialogResult.OK;
			}
		}

		private void textBox1_TextChanged(object sender, System.EventArgs e)
		{
			btnSearch.Enabled = ((edFirstName.Text.Length+edLastName.Text.Length)>0);
		}

		public int SearchResultIndex
		{
			get {return _currentIndex;}
			set
			{
				edStudentNumber.Text = _searchResults[value];
				lbCurrentIndex.Text = (value+1).ToString()+"/"+_searchResults.Length;
				btnPrevious.Enabled = (value > 0);
				btnNext.Enabled = (value < (_searchResults.Length-1));
				_currentIndex = value;
			}
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			throw new Exception("Not implemented");
			/*
			string strFirstName=Tools.CStrDef(edFirstName.Text, "");
			string strLastName=Tools.CStrDef(edLastName.Text, "");
			
			if ((strFirstName.Length+strLastName.Length) == 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("מחפש תלמידים בעלי שם זה אנא המתן...", true);
			DataServices.DataService _service=new DataServices.DataService();
			_service.CookieContainer = Sport.Core.Session.Cookies;
			string[] arrStudents=_service.GetStudentsByName(strFirstName, strLastName);
			Sport.UI.Dialogs.WaitForm.HideWait();
			if (arrStudents.Length == 0)
			{
				Sport.UI.MessageBox.Success("אין תלמידים בעלי שם זה", "חיפוש תלמידים");
				return;
			}
			Sport.UI.MessageBox.Success("נמצאו "+arrStudents.Length+" תלמידים בעלי שם זה", "חיפוש תלמידים");
			
			pnlMultiply.Visible = (arrStudents.Length>1);
			_searchResults = arrStudents;
			SearchResultIndex = 0;*/
		}

		private void btnPrevious_Click(object sender, System.EventArgs e)
		{
			if (SearchResultIndex <= 0)
				return;
			btnPrevious.Enabled = false;
			SearchResultIndex -= 1;
		}

		private void btnNext_Click(object sender, System.EventArgs e)
		{
			if (SearchResultIndex >= _searchResults.Length)
				return;
			btnNext.Enabled = false;
			SearchResultIndex += 1;
		}
	} //end class StudentDetails
}
