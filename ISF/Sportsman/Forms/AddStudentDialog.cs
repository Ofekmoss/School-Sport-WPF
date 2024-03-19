using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for AddStudentDialog.
	/// </summary>
	public class AddStudentDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lbSchoolName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown nudIdNumber;
		private System.Windows.Forms.TextBox tbFirstName;
		private System.Windows.Forms.TextBox tbLastName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.DateTimePicker dtpBirthDate;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnCancel;

		private Sport.Entities.School _school=null;
		private object[] _entityFields=null;
		private bool _ignoreBirthDate=true;

		private System.Windows.Forms.ComboBox cbGrades;
		private System.Windows.Forms.ComboBox cbSexTypes;
	
		public object[] EntityFields
		{
			get {return _entityFields;}
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddStudentDialog(Sport.Entities.School studentSchool)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			dtpBirthDate.MinDate = DateTime.Now.AddYears(-20);
			dtpBirthDate.MaxDate = DateTime.Now.AddYears(-6);
			_entityFields = new object[(int) Sport.Entities.Student.Fields.DataFields];	
			
			_school = studentSchool;
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
			this.lbSchoolName = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbFirstName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.nudIdNumber = new System.Windows.Forms.NumericUpDown();
			this.tbLastName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.dtpBirthDate = new System.Windows.Forms.DateTimePicker();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.cbGrades = new System.Windows.Forms.ComboBox();
			this.cbSexTypes = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudIdNumber)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(224, 8);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "בית ספר: ";
			// 
			// lbSchoolName
			// 
			this.lbSchoolName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbSchoolName.ForeColor = System.Drawing.Color.Blue;
			this.lbSchoolName.Location = new System.Drawing.Point(32, 8);
			this.lbSchoolName.Name = "lbSchoolName";
			this.lbSchoolName.Size = new System.Drawing.Size(184, 23);
			this.lbSchoolName.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(224, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 23);
			this.label3.TabIndex = 2;
			this.label3.Text = "מספר זהות: ";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.ForeColor = System.Drawing.Color.Red;
			this.label4.Location = new System.Drawing.Point(304, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(16, 23);
			this.label4.TabIndex = 4;
			this.label4.Text = "*";
			// 
			// tbFirstName
			// 
			this.tbFirstName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbFirstName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbFirstName.ForeColor = System.Drawing.Color.Blue;
			this.tbFirstName.Location = new System.Drawing.Point(32, 72);
			this.tbFirstName.MaxLength = 15;
			this.tbFirstName.Name = "tbFirstName";
			this.tbFirstName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbFirstName.Size = new System.Drawing.Size(184, 22);
			this.tbFirstName.TabIndex = 2;
			this.tbFirstName.Text = "";
			this.tbFirstName.Enter += new System.EventHandler(this.nudIdNumber_Enter);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Location = new System.Drawing.Point(304, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(16, 23);
			this.label2.TabIndex = 7;
			this.label2.Text = "*";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(224, 72);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(80, 23);
			this.label5.TabIndex = 6;
			this.label5.Text = "שם פרטי: ";
			// 
			// nudIdNumber
			// 
			this.nudIdNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudIdNumber.ForeColor = System.Drawing.Color.Blue;
			this.nudIdNumber.Location = new System.Drawing.Point(32, 40);
			this.nudIdNumber.Maximum = new System.Decimal(new int[] {
																		1410065407,
																		2,
																		0,
																		0});
			this.nudIdNumber.Name = "nudIdNumber";
			this.nudIdNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.nudIdNumber.Size = new System.Drawing.Size(184, 21);
			this.nudIdNumber.TabIndex = 1;
			this.nudIdNumber.Enter += new System.EventHandler(this.nudIdNumber_Enter);
			// 
			// tbLastName
			// 
			this.tbLastName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbLastName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbLastName.ForeColor = System.Drawing.Color.Blue;
			this.tbLastName.Location = new System.Drawing.Point(32, 104);
			this.tbLastName.MaxLength = 15;
			this.tbLastName.Name = "tbLastName";
			this.tbLastName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbLastName.Size = new System.Drawing.Size(184, 22);
			this.tbLastName.TabIndex = 3;
			this.tbLastName.Text = "";
			this.tbLastName.Enter += new System.EventHandler(this.nudIdNumber_Enter);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label6.ForeColor = System.Drawing.Color.Red;
			this.label6.Location = new System.Drawing.Point(304, 104);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(16, 23);
			this.label6.TabIndex = 11;
			this.label6.Text = "*";
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(224, 104);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(80, 23);
			this.label7.TabIndex = 10;
			this.label7.Text = "שם משפחה: ";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(224, 136);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(96, 23);
			this.label8.TabIndex = 13;
			this.label8.Text = "תאריך לידה: ";
			// 
			// dtpBirthDate
			// 
			this.dtpBirthDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.dtpBirthDate.CalendarForeColor = System.Drawing.Color.Blue;
			this.dtpBirthDate.Checked = false;
			this.dtpBirthDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtpBirthDate.Location = new System.Drawing.Point(32, 136);
			this.dtpBirthDate.MinDate = new System.DateTime(1754, 1, 1, 0, 0, 0, 0);
			this.dtpBirthDate.Name = "dtpBirthDate";
			this.dtpBirthDate.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.dtpBirthDate.ShowCheckBox = true;
			this.dtpBirthDate.Size = new System.Drawing.Size(184, 21);
			this.dtpBirthDate.TabIndex = 4;
			this.dtpBirthDate.Click += new System.EventHandler(this.dtpBirthDate_ValueChanged);
			this.dtpBirthDate.ValueChanged += new System.EventHandler(this.dtpBirthDate_ValueChanged);
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label9.ForeColor = System.Drawing.Color.Red;
			this.label9.Location = new System.Drawing.Point(304, 168);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(16, 23);
			this.label9.TabIndex = 16;
			this.label9.Text = "*";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.Location = new System.Drawing.Point(256, 168);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(48, 23);
			this.label10.TabIndex = 15;
			this.label10.Text = "כיתה: ";
			// 
			// cbGrades
			// 
			this.cbGrades.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbGrades.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbGrades.ForeColor = System.Drawing.Color.Blue;
			this.cbGrades.Location = new System.Drawing.Point(187, 168);
			this.cbGrades.Name = "cbGrades";
			this.cbGrades.Size = new System.Drawing.Size(56, 21);
			this.cbGrades.TabIndex = 5;
			this.cbGrades.Enter += new System.EventHandler(this.nudIdNumber_Enter);
			// 
			// cbSexTypes
			// 
			this.cbSexTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSexTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSexTypes.ForeColor = System.Drawing.Color.Blue;
			this.cbSexTypes.Location = new System.Drawing.Point(32, 168);
			this.cbSexTypes.Name = "cbSexTypes";
			this.cbSexTypes.Size = new System.Drawing.Size(56, 21);
			this.cbSexTypes.TabIndex = 6;
			this.cbSexTypes.Enter += new System.EventHandler(this.nudIdNumber_Enter);
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label11.Location = new System.Drawing.Point(102, 168);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(48, 23);
			this.label11.TabIndex = 18;
			this.label11.Text = "מין: ";
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.ForeColor = System.Drawing.Color.Blue;
			this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnClose.Location = new System.Drawing.Point(56, 200);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(96, 23);
			this.btnClose.TabIndex = 19;
			this.btnClose.Text = "אישור";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.Location = new System.Drawing.Point(192, 200);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(96, 23);
			this.btnCancel.TabIndex = 20;
			this.btnCancel.Text = "ביטול";
			// 
			// AddStudentDialog
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(330, 232);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.cbSexTypes);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.cbGrades);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.dtpBirthDate);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.tbLastName);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.nudIdNumber);
			this.Controls.Add(this.tbFirstName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lbSchoolName);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddStudentDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "הוספת תלמיד";
			this.Load += new System.EventHandler(this.AddStudentDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudIdNumber)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			//validate data.

			//student ID number:
			string strIdNumber=nudIdNumber.Value.ToString();
			if (strIdNumber.Length < 5)
			{
				Sport.UI.MessageBox.Error("מספר זהות חייב להיות לפחות חמש ספרות", "הוספת תלמיד");
				nudIdNumber.Focus();
				return;
			}

			//student exists in a different school?
			Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter((int)Sport.Entities.Student.Fields.IdNumber, strIdNumber);
			Sport.Data.Entity[] arrStudents = null;
			try
			{
				arrStudents = Sport.Entities.Student.Type.GetEntities(filter);
			}
			catch
			{
			}
			if (arrStudents != null && arrStudents.Length > 0)
			{
				Sport.Entities.Student student = new Sport.Entities.Student(arrStudents[0]);
				if (student.School.Id != _school.Id)
				{
					string msg = "תלמיד רשום בבית ספר אחר ";
					msg += "(" + student.School.Name + ") " + "לא ניתן להוסיף לקבוצה זו";
					Sport.UI.MessageBox.Error(msg, "הוספת תלמיד");
					nudIdNumber.Focus();
					return;
				}
			}


			//student first name:
			string strFirstName=Sport.Common.Tools.Trim(tbFirstName.Text);
			if (strFirstName.Length < 2)
			{
				Sport.UI.MessageBox.Error("שם פרטי חייב להכיל לפחות שתי אותיות", "הוספת תלמיד");
				tbFirstName.Focus();
				return;
			}

			//student last name:
			string strLastName=Sport.Common.Tools.Trim(tbLastName.Text);
			if (strLastName.Length < 1)
			{
				Sport.UI.MessageBox.Error("אנא הכנס שם משפחה", "הוספת תלמיד");
				tbLastName.Focus();
				return;
			}

			//grade:
			Sport.Data.LookupItem grade=
				(Sport.Data.LookupItem) cbGrades.SelectedItem;
			if (grade == null)
			{
				Sport.UI.MessageBox.Error("אנא בחר כיתה", "הוספת תלמיד");
				cbGrades.Focus();
				return;
			}

			//get non required fields data:
			DateTime birthDate=DateTime.MinValue;
			if (!_ignoreBirthDate)
				birthDate = dtpBirthDate.Value;
			Sport.Data.LookupItem sexType=
				(Sport.Data.LookupItem) cbSexTypes.SelectedItem;
			
			//populate fields:
			_entityFields[(int) Sport.Entities.Student.Fields.School] = _school.Id;
			_entityFields[(int) Sport.Entities.Student.Fields.IdNumber] = strIdNumber;
			_entityFields[(int) Sport.Entities.Student.Fields.FirstName] = strFirstName;
			_entityFields[(int) Sport.Entities.Student.Fields.LastName] = strLastName;
			_entityFields[(int) Sport.Entities.Student.Fields.Grade] = grade.Id;
			if (sexType != null)
				_entityFields[(int) Sport.Entities.Student.Fields.SexType] = sexType.Id;
			if (birthDate.Year >= 1900)
				_entityFields[(int) Sport.Entities.Student.Fields.BirthDate] = birthDate;
			_entityFields[(int) Sport.Entities.Student.Fields.IdNumberType] = 0;
			_entityFields[(int) Sport.Entities.Student.Fields.LastModified] = DateTime.Now;
			
			this.DialogResult = DialogResult.OK;
		}

		private void AddStudentDialog_Load(object sender, System.EventArgs e)
		{
			//stupid control is not working.
			dtpBirthDate.Checked = false;

			//initialize fields:
			Core.Tools.AttachGlobalTooltip(this, "*", "שדה דרוש");
			Sport.Data.LookupItem[] grades=
				(new Sport.Types.GradeTypeLookup(true)).Items;
			cbGrades.Items.Clear();
			bool checkGrades=(_school.ToGrade > _school.FromGrade);
			foreach (Sport.Data.LookupItem grade in grades)
			{
				int iValue=Sport.Core.Session.Season-grade.Id;
				if ((!checkGrades)||
					((iValue >= _school.FromGrade)&&(iValue <= _school.ToGrade)))
				{
					cbGrades.Items.Add(grade);
				}
			}
			Sport.Data.LookupItem[] sexTypes=
				(new Sport.Types.SexTypeLookup()).Items;
			cbSexTypes.Items.Clear();
			foreach (Sport.Data.LookupItem sexType in sexTypes)
			{
				cbSexTypes.Items.Add(sexType);
			}
		}

		private void dtpBirthDate_ValueChanged(object sender, System.EventArgs e)
		{
			_ignoreBirthDate = (!dtpBirthDate.Checked);
		}

		private void nudIdNumber_Enter(object sender, System.EventArgs e)
		{
			if (_ignoreBirthDate)
				dtpBirthDate.Checked = false;
		}
	}
}
