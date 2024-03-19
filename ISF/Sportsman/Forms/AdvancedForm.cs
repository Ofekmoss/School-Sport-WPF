using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for AdvancedForm.
	/// </summary>
	public class AdvancedForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tcOptions;
		private System.Windows.Forms.TabPage tpGrades;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lbStudentsCount1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lbStudentsCount2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lbStudentsCount3;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label lbStudentsCount4;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label lbStudentsCount5;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label lbStudentsCount6;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label lbStudentsCount7;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label lbStudentsCount14;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lbStudentsCount13;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label lbStudentsCount12;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label lbStudentsCount11;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label lbStudentsCount10;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label lbStudentsCount9;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label lbStudentsCount8;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Button btnReload;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabPage tpAlterSQL;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ListBox lbSQL;
		private System.Windows.Forms.TextBox tbSQL;
		private System.Windows.Forms.Button button3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AdvancedForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AdvancedForm));
			this.tcOptions = new System.Windows.Forms.TabControl();
			this.tpGrades = new System.Windows.Forms.TabPage();
			this.btnReload = new System.Windows.Forms.Button();
			this.lbStudentsCount14 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lbStudentsCount13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.lbStudentsCount12 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.lbStudentsCount11 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.lbStudentsCount10 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.lbStudentsCount9 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.lbStudentsCount8 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.label28 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.lbStudentsCount7 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.lbStudentsCount6 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.lbStudentsCount5 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.lbStudentsCount4 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.lbStudentsCount3 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.lbStudentsCount2 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.lbStudentsCount1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.tpAlterSQL = new System.Windows.Forms.TabPage();
			this.button2 = new System.Windows.Forms.Button();
			this.lbSQL = new System.Windows.Forms.ListBox();
			this.tbSQL = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.tcOptions.SuspendLayout();
			this.tpGrades.SuspendLayout();
			this.tpAlterSQL.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcOptions
			// 
			this.tcOptions.Controls.Add(this.tpGrades);
			this.tcOptions.Controls.Add(this.tpAlterSQL);
			this.tcOptions.Location = new System.Drawing.Point(8, 8);
			this.tcOptions.Name = "tcOptions";
			this.tcOptions.SelectedIndex = 0;
			this.tcOptions.Size = new System.Drawing.Size(352, 280);
			this.tcOptions.TabIndex = 0;
			// 
			// tpGrades
			// 
			this.tpGrades.Controls.Add(this.btnReload);
			this.tpGrades.Controls.Add(this.lbStudentsCount14);
			this.tpGrades.Controls.Add(this.label8);
			this.tpGrades.Controls.Add(this.lbStudentsCount13);
			this.tpGrades.Controls.Add(this.label12);
			this.tpGrades.Controls.Add(this.lbStudentsCount12);
			this.tpGrades.Controls.Add(this.label16);
			this.tpGrades.Controls.Add(this.lbStudentsCount11);
			this.tpGrades.Controls.Add(this.label19);
			this.tpGrades.Controls.Add(this.lbStudentsCount10);
			this.tpGrades.Controls.Add(this.label21);
			this.tpGrades.Controls.Add(this.lbStudentsCount9);
			this.tpGrades.Controls.Add(this.label23);
			this.tpGrades.Controls.Add(this.lbStudentsCount8);
			this.tpGrades.Controls.Add(this.label25);
			this.tpGrades.Controls.Add(this.label26);
			this.tpGrades.Controls.Add(this.label27);
			this.tpGrades.Controls.Add(this.label28);
			this.tpGrades.Controls.Add(this.label29);
			this.tpGrades.Controls.Add(this.lbStudentsCount7);
			this.tpGrades.Controls.Add(this.label17);
			this.tpGrades.Controls.Add(this.lbStudentsCount6);
			this.tpGrades.Controls.Add(this.label15);
			this.tpGrades.Controls.Add(this.lbStudentsCount5);
			this.tpGrades.Controls.Add(this.label13);
			this.tpGrades.Controls.Add(this.lbStudentsCount4);
			this.tpGrades.Controls.Add(this.label11);
			this.tpGrades.Controls.Add(this.lbStudentsCount3);
			this.tpGrades.Controls.Add(this.label9);
			this.tpGrades.Controls.Add(this.lbStudentsCount2);
			this.tpGrades.Controls.Add(this.label7);
			this.tpGrades.Controls.Add(this.lbStudentsCount1);
			this.tpGrades.Controls.Add(this.label6);
			this.tpGrades.Controls.Add(this.label4);
			this.tpGrades.Controls.Add(this.label3);
			this.tpGrades.Controls.Add(this.label2);
			this.tpGrades.Controls.Add(this.label1);
			this.tpGrades.Location = new System.Drawing.Point(4, 22);
			this.tpGrades.Name = "tpGrades";
			this.tpGrades.Size = new System.Drawing.Size(344, 254);
			this.tpGrades.TabIndex = 0;
			this.tpGrades.Text = "שכבות בית ספר";
			// 
			// btnReload
			// 
			this.btnReload.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnReload.Location = new System.Drawing.Point(136, 225);
			this.btnReload.Name = "btnReload";
			this.btnReload.TabIndex = 36;
			this.btnReload.Text = "טען נתונים";
			this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
			// 
			// lbStudentsCount14
			// 
			this.lbStudentsCount14.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount14.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount14.Location = new System.Drawing.Point(16, 192);
			this.lbStudentsCount14.Name = "lbStudentsCount14";
			this.lbStudentsCount14.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount14.TabIndex = 35;
			this.lbStudentsCount14.Text = "0";
			this.lbStudentsCount14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label8.ForeColor = System.Drawing.Color.Blue;
			this.label8.Location = new System.Drawing.Point(104, 192);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(50, 20);
			this.label8.TabIndex = 34;
			this.label8.Text = "י\"ד";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount13
			// 
			this.lbStudentsCount13.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount13.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount13.Location = new System.Drawing.Point(16, 168);
			this.lbStudentsCount13.Name = "lbStudentsCount13";
			this.lbStudentsCount13.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount13.TabIndex = 33;
			this.lbStudentsCount13.Text = "0";
			this.lbStudentsCount13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label12.ForeColor = System.Drawing.Color.Blue;
			this.label12.Location = new System.Drawing.Point(104, 168);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(50, 20);
			this.label12.TabIndex = 32;
			this.label12.Text = "י\"ג";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount12
			// 
			this.lbStudentsCount12.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount12.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount12.Location = new System.Drawing.Point(16, 144);
			this.lbStudentsCount12.Name = "lbStudentsCount12";
			this.lbStudentsCount12.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount12.TabIndex = 31;
			this.lbStudentsCount12.Text = "0";
			this.lbStudentsCount12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label16
			// 
			this.label16.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label16.ForeColor = System.Drawing.Color.Blue;
			this.label16.Location = new System.Drawing.Point(104, 144);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(50, 20);
			this.label16.TabIndex = 30;
			this.label16.Text = "י\"ב";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount11
			// 
			this.lbStudentsCount11.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount11.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount11.Location = new System.Drawing.Point(16, 120);
			this.lbStudentsCount11.Name = "lbStudentsCount11";
			this.lbStudentsCount11.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount11.TabIndex = 29;
			this.lbStudentsCount11.Text = "0";
			this.lbStudentsCount11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label19
			// 
			this.label19.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label19.ForeColor = System.Drawing.Color.Blue;
			this.label19.Location = new System.Drawing.Point(104, 120);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(50, 20);
			this.label19.TabIndex = 28;
			this.label19.Text = "י\"א";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount10
			// 
			this.lbStudentsCount10.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount10.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount10.Location = new System.Drawing.Point(16, 96);
			this.lbStudentsCount10.Name = "lbStudentsCount10";
			this.lbStudentsCount10.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount10.TabIndex = 27;
			this.lbStudentsCount10.Text = "0";
			this.lbStudentsCount10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label21
			// 
			this.label21.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label21.ForeColor = System.Drawing.Color.Blue;
			this.label21.Location = new System.Drawing.Point(104, 96);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(50, 20);
			this.label21.TabIndex = 26;
			this.label21.Text = "י\'";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount9
			// 
			this.lbStudentsCount9.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount9.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount9.Location = new System.Drawing.Point(16, 72);
			this.lbStudentsCount9.Name = "lbStudentsCount9";
			this.lbStudentsCount9.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount9.TabIndex = 25;
			this.lbStudentsCount9.Text = "0";
			this.lbStudentsCount9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label23
			// 
			this.label23.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label23.ForeColor = System.Drawing.Color.Blue;
			this.label23.Location = new System.Drawing.Point(104, 72);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(50, 20);
			this.label23.TabIndex = 24;
			this.label23.Text = "ט\'";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount8
			// 
			this.lbStudentsCount8.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount8.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount8.Location = new System.Drawing.Point(16, 48);
			this.lbStudentsCount8.Name = "lbStudentsCount8";
			this.lbStudentsCount8.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount8.TabIndex = 23;
			this.lbStudentsCount8.Text = "0";
			this.lbStudentsCount8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label25
			// 
			this.label25.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label25.ForeColor = System.Drawing.Color.Blue;
			this.label25.Location = new System.Drawing.Point(104, 48);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(50, 20);
			this.label25.TabIndex = 22;
			this.label25.Text = "ח\'";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label26
			// 
			this.label26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label26.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label26.Location = new System.Drawing.Point(16, 40);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(144, 2);
			this.label26.TabIndex = 21;
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label27
			// 
			this.label27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label27.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label27.Location = new System.Drawing.Point(96, 16);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(2, 208);
			this.label27.TabIndex = 20;
			this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label28
			// 
			this.label28.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label28.Location = new System.Drawing.Point(16, 16);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(80, 20);
			this.label28.TabIndex = 19;
			this.label28.Text = "מס\' תלמידים";
			this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label29
			// 
			this.label29.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label29.Location = new System.Drawing.Point(104, 16);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(50, 20);
			this.label29.TabIndex = 18;
			this.label29.Text = "כיתה";
			this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount7
			// 
			this.lbStudentsCount7.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount7.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount7.Location = new System.Drawing.Point(184, 192);
			this.lbStudentsCount7.Name = "lbStudentsCount7";
			this.lbStudentsCount7.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount7.TabIndex = 17;
			this.lbStudentsCount7.Text = "0";
			this.lbStudentsCount7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label17
			// 
			this.label17.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label17.ForeColor = System.Drawing.Color.Blue;
			this.label17.Location = new System.Drawing.Point(272, 192);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(50, 20);
			this.label17.TabIndex = 16;
			this.label17.Text = "ז\'";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount6
			// 
			this.lbStudentsCount6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount6.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount6.Location = new System.Drawing.Point(184, 168);
			this.lbStudentsCount6.Name = "lbStudentsCount6";
			this.lbStudentsCount6.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount6.TabIndex = 15;
			this.lbStudentsCount6.Text = "0";
			this.lbStudentsCount6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label15
			// 
			this.label15.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label15.ForeColor = System.Drawing.Color.Blue;
			this.label15.Location = new System.Drawing.Point(272, 168);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(50, 20);
			this.label15.TabIndex = 14;
			this.label15.Text = "ו\'";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount5
			// 
			this.lbStudentsCount5.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount5.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount5.Location = new System.Drawing.Point(184, 144);
			this.lbStudentsCount5.Name = "lbStudentsCount5";
			this.lbStudentsCount5.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount5.TabIndex = 13;
			this.lbStudentsCount5.Text = "0";
			this.lbStudentsCount5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label13
			// 
			this.label13.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label13.ForeColor = System.Drawing.Color.Blue;
			this.label13.Location = new System.Drawing.Point(272, 144);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(50, 20);
			this.label13.TabIndex = 12;
			this.label13.Text = "ה\'";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount4
			// 
			this.lbStudentsCount4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount4.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount4.Location = new System.Drawing.Point(184, 120);
			this.lbStudentsCount4.Name = "lbStudentsCount4";
			this.lbStudentsCount4.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount4.TabIndex = 11;
			this.lbStudentsCount4.Text = "0";
			this.lbStudentsCount4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label11.ForeColor = System.Drawing.Color.Blue;
			this.label11.Location = new System.Drawing.Point(272, 120);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(50, 20);
			this.label11.TabIndex = 10;
			this.label11.Text = "ד\'";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount3
			// 
			this.lbStudentsCount3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount3.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount3.Location = new System.Drawing.Point(184, 96);
			this.lbStudentsCount3.Name = "lbStudentsCount3";
			this.lbStudentsCount3.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount3.TabIndex = 9;
			this.lbStudentsCount3.Text = "0";
			this.lbStudentsCount3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label9.ForeColor = System.Drawing.Color.Blue;
			this.label9.Location = new System.Drawing.Point(272, 96);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 20);
			this.label9.TabIndex = 8;
			this.label9.Text = "ג\'";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount2
			// 
			this.lbStudentsCount2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount2.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount2.Location = new System.Drawing.Point(184, 72);
			this.lbStudentsCount2.Name = "lbStudentsCount2";
			this.lbStudentsCount2.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount2.TabIndex = 7;
			this.lbStudentsCount2.Text = "0";
			this.lbStudentsCount2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label7.ForeColor = System.Drawing.Color.Blue;
			this.label7.Location = new System.Drawing.Point(272, 72);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(50, 20);
			this.label7.TabIndex = 6;
			this.label7.Text = "ב\'";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbStudentsCount1
			// 
			this.lbStudentsCount1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbStudentsCount1.ForeColor = System.Drawing.Color.Blue;
			this.lbStudentsCount1.Location = new System.Drawing.Point(184, 48);
			this.lbStudentsCount1.Name = "lbStudentsCount1";
			this.lbStudentsCount1.Size = new System.Drawing.Size(80, 20);
			this.lbStudentsCount1.TabIndex = 5;
			this.lbStudentsCount1.Text = "0";
			this.lbStudentsCount1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label6.ForeColor = System.Drawing.Color.Blue;
			this.label6.Location = new System.Drawing.Point(272, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(50, 20);
			this.label6.TabIndex = 4;
			this.label6.Text = "א\'";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label4.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label4.Location = new System.Drawing.Point(184, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(144, 2);
			this.label4.TabIndex = 3;
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label3.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label3.Location = new System.Drawing.Point(265, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(2, 208);
			this.label3.TabIndex = 2;
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(182, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "מס\' תלמידים";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.Location = new System.Drawing.Point(272, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "כיתה";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.button1.Location = new System.Drawing.Point(8, 296);
			this.button1.Name = "button1";
			this.button1.TabIndex = 37;
			this.button1.Text = "סגור";
			// 
			// tpAlterSQL
			// 
			this.tpAlterSQL.Controls.Add(this.button3);
			this.tpAlterSQL.Controls.Add(this.tbSQL);
			this.tpAlterSQL.Controls.Add(this.lbSQL);
			this.tpAlterSQL.Controls.Add(this.button2);
			this.tpAlterSQL.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tpAlterSQL.Location = new System.Drawing.Point(4, 22);
			this.tpAlterSQL.Name = "tpAlterSQL";
			this.tpAlterSQL.Size = new System.Drawing.Size(344, 254);
			this.tpAlterSQL.TabIndex = 1;
			this.tpAlterSQL.Text = "הוספת שדות";
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.button2.Location = new System.Drawing.Point(144, 224);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(72, 24);
			this.button2.TabIndex = 0;
			this.button2.Text = "טען נתונים";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// lbSQL
			// 
			this.lbSQL.ItemHeight = 14;
			this.lbSQL.Location = new System.Drawing.Point(208, 16);
			this.lbSQL.Name = "lbSQL";
			this.lbSQL.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.lbSQL.Size = new System.Drawing.Size(120, 158);
			this.lbSQL.TabIndex = 1;
			this.lbSQL.SelectedIndexChanged += new System.EventHandler(this.lbSQL_SelectedIndexChanged);
			// 
			// tbSQL
			// 
			this.tbSQL.Location = new System.Drawing.Point(8, 16);
			this.tbSQL.Multiline = true;
			this.tbSQL.Name = "tbSQL";
			this.tbSQL.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.tbSQL.Size = new System.Drawing.Size(192, 160);
			this.tbSQL.TabIndex = 2;
			this.tbSQL.Text = "";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(16, 184);
			this.button3.Name = "button3";
			this.button3.TabIndex = 3;
			this.button3.Text = "הרץ שאילתא";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// AdvancedForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 325);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tcOptions);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AdvancedForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "שאילתות מתקדמות";
			this.tcOptions.ResumeLayout(false);
			this.tpGrades.ResumeLayout(false);
			this.tpAlterSQL.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnReload_Click(object sender, System.EventArgs e)
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
			Cursor oldCursor=Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				//initialize service:
				AdvancedServices.AdvancedService _service=new AdvancedServices.AdvancedService();
				_service.CookieContainer = Sport.Core.Session.Cookies;

				//get grade data:
				AdvancedServices.GradeData[] gradesData=_service.GetGradesInfo();

				//iterate over the grades:
				foreach (AdvancedServices.GradeData gradeData in gradesData)
				{
					//calculate current index according to the season:
					int gradeIndex=(Sport.Core.Session.Season-gradeData.grade)+1;

					//look if we have proper control:
					Control objControl=Sport.Common.Tools.FindControl(
						tpGrades, "lbStudentsCount"+gradeIndex.ToString());

					//assign value:
					if (objControl != null)
						(objControl as Label).Text = gradeData.studentsCount.ToString();
				}
				
				Sport.UI.Dialogs.WaitForm.HideWait();
				Cursor.Current = oldCursor;
			}
			catch (Exception err)
			{
				Sport.UI.Dialogs.WaitForm.HideWait();
				Sport.UI.MessageBox.Show("שגיאה בעת טעינת הנתונים: \n"+err.Message, "נתונים מתקדמים", MessageBoxIcon.Error);
				System.Diagnostics.Debug.WriteLine("error loading advanced info: "+err.Message);
				System.Diagnostics.Debug.WriteLine(err.StackTrace);
				Cursor.Current = oldCursor;
			}
		}

		private void lbSQL_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lbSQL.SelectedIndex >= 0)
				tbSQL.Text = lbSQL.Items[lbSQL.SelectedIndex].ToString();
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			AdvancedServices.AdvancedService _service=new AdvancedServices.AdvancedService();
			_service.CookieContainer = Sport.Core.Session.Cookies;
			string[] arrSQL=_service.GetAlterSQL();
			lbSQL.Items.Clear();
			tbSQL.Clear();
			lbSQL.Items.AddRange(arrSQL);
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			if (lbSQL.SelectedIndex >= 0)
			{
				AdvancedServices.AdvancedService _service=new AdvancedServices.AdvancedService();
				_service.CookieContainer = Sport.Core.Session.Cookies;
				try
				{
					_service.ExecuteAlterSQL(lbSQL.SelectedIndex);
					MessageBox.Show("success!", "alter SQL", MessageBoxButtons.OK, MessageBoxIcon.Information); 
				}
				catch (Exception ex)
				{
					MessageBox.Show("error executing SQL:\n"+ex.Message, "alter SQL", MessageBoxButtons.OK, MessageBoxIcon.Error); 
				}
			}
		}
	}
}
