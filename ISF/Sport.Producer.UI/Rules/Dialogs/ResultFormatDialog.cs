using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Data;
using Sport.Types;

namespace Sport.Producer.UI.Rules.Dialogs
{
	/// <summary>
	/// Summary description for ResultFormatDialog.
	/// </summary>
	public class ResultFormatDialog : System.Windows.Forms.Form
	{
		private static readonly string BlankSpaceText="[רווח]";
		private static readonly string BackspaceText="<";
		private static readonly string ClearAllText="<<";
		
		private string _resultFormat;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox grpButtons;
		public System.Windows.Forms.Button btnClearAll;
		public System.Windows.Forms.Button btnBackspace;
		public System.Windows.Forms.Button btnDigit;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.TextBox edResultFormat;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public string ResultFormat
		{
			get
			{
				if (edResultFormat.Text == null)
					return "";
				return edResultFormat.Text;
			}
		}

		public ResultFormatDialog(string resultFormat)
		{
			_resultFormat = resultFormat;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			edResultFormat.Text = resultFormat;
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
			this.panel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.grpButtons = new System.Windows.Forms.GroupBox();
			this.btnClearAll = new System.Windows.Forms.Button();
			this.btnBackspace = new System.Windows.Forms.Button();
			this.btnDigit = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.edResultFormat = new System.Windows.Forms.TextBox();
			this.panel.SuspendLayout();
			this.grpButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.label1);
			this.panel.Controls.Add(this.grpButtons);
			this.panel.Controls.Add(this.btnOK);
			this.panel.Controls.Add(this.btnClose);
			this.panel.Controls.Add(this.edResultFormat);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(136, 136);
			this.panel.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.ForeColor = System.Drawing.Color.Blue;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 22);
			this.label1.TabIndex = 19;
			this.label1.Text = "תבנית תוצאה";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// grpButtons
			// 
			this.grpButtons.Controls.Add(this.btnClearAll);
			this.grpButtons.Controls.Add(this.btnBackspace);
			this.grpButtons.Controls.Add(this.btnDigit);
			this.grpButtons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grpButtons.Location = new System.Drawing.Point(7, 53);
			this.grpButtons.Name = "grpButtons";
			this.grpButtons.Size = new System.Drawing.Size(120, 48);
			this.grpButtons.TabIndex = 18;
			this.grpButtons.TabStop = false;
			// 
			// btnClearAll
			// 
			this.btnClearAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnClearAll.Location = new System.Drawing.Point(24, 26);
			this.btnClearAll.Name = "btnClearAll";
			this.btnClearAll.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnClearAll.Size = new System.Drawing.Size(25, 20);
			this.btnClearAll.TabIndex = 13;
			this.btnClearAll.Tag = "";
			this.btnClearAll.Text = "<<";
			this.btnClearAll.Click += new System.EventHandler(this.FormatButtonClick);
			// 
			// btnBackspace
			// 
			this.btnBackspace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnBackspace.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnBackspace.Location = new System.Drawing.Point(0, 26);
			this.btnBackspace.Name = "btnBackspace";
			this.btnBackspace.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnBackspace.Size = new System.Drawing.Size(20, 20);
			this.btnBackspace.TabIndex = 12;
			this.btnBackspace.Tag = "";
			this.btnBackspace.Text = "<";
			this.btnBackspace.Click += new System.EventHandler(this.FormatButtonClick);
			// 
			// btnDigit
			// 
			this.btnDigit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDigit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnDigit.Location = new System.Drawing.Point(0, 5);
			this.btnDigit.Name = "btnDigit";
			this.btnDigit.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.btnDigit.Size = new System.Drawing.Size(20, 20);
			this.btnDigit.TabIndex = 11;
			this.btnDigit.Tag = "";
			this.btnDigit.Click += new System.EventHandler(this.FormatButtonClick);
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnOK.Location = new System.Drawing.Point(71, 109);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(54, 21);
			this.btnOK.TabIndex = 17;
			this.btnOK.Text = "אישור";
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnClose.Location = new System.Drawing.Point(7, 109);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(54, 21);
			this.btnClose.TabIndex = 16;
			this.btnClose.Text = "ביטול";
			// 
			// edResultFormat
			// 
			this.edResultFormat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.edResultFormat.Location = new System.Drawing.Point(7, 29);
			this.edResultFormat.Name = "edResultFormat";
			this.edResultFormat.ReadOnly = true;
			this.edResultFormat.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.edResultFormat.Size = new System.Drawing.Size(120, 20);
			this.edResultFormat.TabIndex = 15;
			this.edResultFormat.Text = "";
			// 
			// ResultFormatDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(136, 136);
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ResultFormatDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "תבנית תוצאה";
			this.Load += new System.EventHandler(this.ResultFormatDialog_Load);
			this.panel.ResumeLayout(false);
			this.grpButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public static bool EditResultFormat(ref string format)
		{
			ResultFormatDialog objDialog = new ResultFormatDialog(format);
			if (objDialog.ShowDialog() == DialogResult.OK)
			{
				format = objDialog.ResultFormat;
				return true;
			}
			
			return false;
		}		

		public static object ValueSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			string format="";
			if (value != null)
				format = value.ToString();
			if (EditResultFormat(ref format))
			{
				return new Sport.Rulesets.ResultFormat(format);
			}

			return value;
		}

		private void ResultFormatDialog_Load(object sender, System.EventArgs e)
		{
			btnDigit.Text = Sport.Rulesets.ResultFormat.DigitCharacter.ToString();
			
			Sport.Common.Tools.AttachTooltip(btnDigit, "לחץ כדי להוסיף סיפרה (0-9) לתבנית");
			Sport.Common.Tools.AttachTooltip(btnBackspace, "לחץ כדי למחוק תו אחרון");
			Sport.Common.Tools.AttachTooltip(btnClearAll, "לחץ כדי למחוק את כל תווי התבנית");

			//build seperator buttons:
			int initialLeft=btnDigit.Left+btnDigit.Width;
			int initialTop=btnDigit.Top;
			int buttonWidth=btnDigit.Width;
			int spaceWidth=35;
			int hDiff=1;
			int buttonHeight=btnDigit.Height;
			for (int i=0; i<Sport.Rulesets.ResultFormat.Delimeters.Length; i++)
			{
				int curLeft=initialLeft+GetSeperatorsWidth()+((i+1)*hDiff);
				string delimeter=Sport.Rulesets.ResultFormat.Delimeters[i].ToString();
				Button objButton=new Button();
				objButton.Name = "btnSeperator_"+i.ToString();
				objButton.FlatStyle = FlatStyle.Flat;
				objButton.Left = curLeft;
				objButton.Top = initialTop;
				objButton.Width = buttonWidth;
				if (delimeter == " ")
					objButton.Width = spaceWidth;
				objButton.Height = buttonHeight;
				objButton.Text = delimeter;
				if (delimeter == " ")
				{
					Sport.Common.Tools.AssignFontSize(objButton, 6.8f);
					objButton.Text = "[רווח]";
				}
				Sport.Common.Tools.AttachTooltip(objButton, "לחץ כדי להוסיף תו זה לתבנית");
				objButton.Click += new EventHandler(FormatButtonClick);
				grpButtons.Controls.Add(objButton);
			}
		}

		private int GetSeperatorsWidth()
		{
			int result=0;
			foreach (Control control in grpButtons.Controls)
			{
				if (control.Name.IndexOf("btnSeperator_") == 0)
				{
					result += control.Width;
				}
			}
			return result;
		}

		private void FormatButtonClick(object sender, EventArgs e)
		{
			if (sender == null)
				return;
			if (!(sender is Button))
				return;
			Button objButton=(Button) sender;
			string caption=objButton.Text;
			
			if (caption == BackspaceText)
			{
				if (edResultFormat.Text.Length > 0)
				{
					edResultFormat.Text = edResultFormat.Text.Substring(0, 
						edResultFormat.Text.Length-1);
				}
				return;
			}
			
			if (caption == ClearAllText)
			{
				edResultFormat.Text = "";
				return;
			}
			
			if (caption == BlankSpaceText)
			{
				edResultFormat.Text += " ";
				return;
			}

			//some other seperator...
			edResultFormat.Text += caption;
		}
	}
}
