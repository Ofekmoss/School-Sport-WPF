using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for InputMarginsForm.
	/// </summary>
	public class InputMarginsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnConfirm;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.NumericUpDown nudTopMargin;
		private System.Windows.Forms.NumericUpDown nudSideMargin;
		private string _iniKey="";
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public InputMarginsForm(string iniKey)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_iniKey = iniKey;
		}
		
		public int TopMarginValue
		{
			get {return (int) nudTopMargin.Value;}
		}
		
		public int SideMarginValue
		{
			get {return (int) nudSideMargin.Value;}
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
			this.nudTopMargin = new System.Windows.Forms.NumericUpDown();
			this.nudSideMargin = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.btnConfirm = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.nudTopMargin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSideMargin)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(118, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 22);
			this.label1.TabIndex = 0;
			this.label1.Text = "שוליים עליונים:";
			// 
			// nudTopMargin
			// 
			this.nudTopMargin.Location = new System.Drawing.Point(38, 12);
			this.nudTopMargin.Maximum = new System.Decimal(new int[] {
																		 500,
																		 0,
																		 0,
																		 0});
			this.nudTopMargin.Minimum = new System.Decimal(new int[] {
																		 200,
																		 0,
																		 0,
																		 -2147483648});
			this.nudTopMargin.Name = "nudTopMargin";
			this.nudTopMargin.Size = new System.Drawing.Size(72, 26);
			this.nudTopMargin.TabIndex = 1;
			// 
			// nudSideMargin
			// 
			this.nudSideMargin.Location = new System.Drawing.Point(38, 55);
			this.nudSideMargin.Maximum = new System.Decimal(new int[] {
																		  500,
																		  0,
																		  0,
																		  0});
			this.nudSideMargin.Minimum = new System.Decimal(new int[] {
																		  200,
																		  0,
																		  0,
																		  -2147483648});
			this.nudSideMargin.Name = "nudSideMargin";
			this.nudSideMargin.Size = new System.Drawing.Size(72, 26);
			this.nudSideMargin.TabIndex = 3;
			this.nudSideMargin.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(115, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 22);
			this.label2.TabIndex = 2;
			this.label2.Text = "שוליים צדדיים:";
			// 
			// btnConfirm
			// 
			this.btnConfirm.BackColor = System.Drawing.Color.White;
			this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnConfirm.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnConfirm.ForeColor = System.Drawing.Color.Blue;
			this.btnConfirm.Location = new System.Drawing.Point(16, 96);
			this.btnConfirm.Name = "btnConfirm";
			this.btnConfirm.Size = new System.Drawing.Size(88, 24);
			this.btnConfirm.TabIndex = 4;
			this.btnConfirm.Text = "אישור";
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.White;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Location = new System.Drawing.Point(144, 96);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 24);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "ביטול";
			// 
			// InputMarginsForm
			// 
			this.AcceptButton = this.btnConfirm;
			this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(234, 136);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnConfirm);
			this.Controls.Add(this.nudSideMargin);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.nudTopMargin);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InputMarginsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "קביעת שוליים";
			this.Load += new System.EventHandler(this.InputMarginsForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudTopMargin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudSideMargin)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void numericUpDown2_ValueChanged(object sender, System.EventArgs e)
		{
		
		}

		private void InputMarginsForm_Load(object sender, System.EventArgs e)
		{
			if ((_iniKey != null)&&(_iniKey.Length > 0))
			{
				string strTopMargins=
					Sport.Core.Configuration.ReadString(_iniKey, "TopMargin");
				int topMargins=Sport.Common.Tools.CIntDef(strTopMargins, 0);
				string strSideMargins=
					Sport.Core.Configuration.ReadString(_iniKey, "SideMargin");
				int sideMargins=Sport.Common.Tools.CIntDef(strSideMargins, 0);
				if ((topMargins >= nudTopMargin.Minimum)&&(topMargins <= nudTopMargin.Maximum))
					nudTopMargin.Value = topMargins;
				if ((sideMargins >= nudSideMargin.Minimum)&&(sideMargins <= nudSideMargin.Maximum))
					nudSideMargin.Value = sideMargins;
			}
		}
	}
}
