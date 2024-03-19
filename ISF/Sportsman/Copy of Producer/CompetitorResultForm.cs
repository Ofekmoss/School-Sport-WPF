using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for CompetitorResultForm.
	/// </summary>
	public class CompetitorResultForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblPlayerName;
		private System.Windows.Forms.Label lblCaption1;
		private Sport.UI.Controls.TextControl result1;
		private System.Windows.Forms.Label lblCaption2;
		private Sport.UI.Controls.TextControl result2;
		private System.Windows.Forms.Label lblCaption3;
		private Sport.UI.Controls.TextControl result3;
		private System.Windows.Forms.Label lblCaption4;
		private Sport.UI.Controls.TextControl result4;
		private System.Windows.Forms.Label lblCaption5;
		private Sport.UI.Controls.TextControl result5;
		private System.Windows.Forms.Label lblCaption6;
		private Sport.UI.Controls.TextControl result6;

		private Sport.Championships.Competitor _competitor;

		public CompetitorResultForm(Sport.Championships.Competitor competitor, 
			int ruleId, string ruleValue)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			_competitor = competitor;
			this.Text = "קביעת תוצאה - "+competitor.Name;
			lblPlayerName.Text = competitor.Name;
			
			Sport.Rulesets.Rules.ResultTypeRule resultTypeRule=
				new Sport.Rulesets.Rules.ResultTypeRule(ruleId);
			Sport.Rulesets.Rules.ResultType resultType=(Sport.Rulesets.Rules.ResultType)
				resultTypeRule.ParseValue(ruleValue);
			switch ((int) resultType.Value)
			{
				case (int) Sport.Core.Data.ResultValue.Time:
					SetTimeCaptions(resultType);
					break;
				case (int) Sport.Core.Data.ResultValue.Distance:
					SetDistanceCaptions(resultType);
					break;
				case (int) Sport.Core.Data.ResultValue.Points:
					SetPointsCaptions(resultType);
					break;
			}

			/*if (competitor.Result != -1)
			{
				string[] arrResults=competitor.Result.Split(new char[] {'|'});
				for (int i=0; i<arrResults.Length; i++)
				{
					Sport.UI.Controls.TextControl control=GetResultControl(i+1);
					int value=0;
					try
					{
						value = Int32.Parse(arrResults[i]);
					}
					catch
					{
						System.Diagnostics.Debug.WriteLine("invalid result for competitor: "+competitor.Result);
					}
					control.Value = value;
				}
			}*/
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lblPlayerName = new System.Windows.Forms.Label();
			this.lblCaption1 = new System.Windows.Forms.Label();
			this.result1 = new Sport.UI.Controls.TextControl();
			this.lblCaption2 = new System.Windows.Forms.Label();
			this.result2 = new Sport.UI.Controls.TextControl();
			this.lblCaption3 = new System.Windows.Forms.Label();
			this.result3 = new Sport.UI.Controls.TextControl();
			this.lblCaption4 = new System.Windows.Forms.Label();
			this.result4 = new Sport.UI.Controls.TextControl();
			this.lblCaption5 = new System.Windows.Forms.Label();
			this.result5 = new Sport.UI.Controls.TextControl();
			this.lblCaption6 = new System.Windows.Forms.Label();
			this.result6 = new Sport.UI.Controls.TextControl();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(96, 224);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 20;
			this.btnCancel.Text = "ביטול";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnOK.Location = new System.Drawing.Point(8, 224);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 19;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(184, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 19);
			this.label1.TabIndex = 21;
			this.label1.Text = "שם המתמודד:";
			// 
			// lblPlayerName
			// 
			this.lblPlayerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lblPlayerName.ForeColor = System.Drawing.Color.Blue;
			this.lblPlayerName.Location = new System.Drawing.Point(16, 8);
			this.lblPlayerName.Name = "lblPlayerName";
			this.lblPlayerName.Size = new System.Drawing.Size(160, 16);
			this.lblPlayerName.TabIndex = 22;
			// 
			// lblCaption1
			// 
			this.lblCaption1.Location = new System.Drawing.Point(168, 32);
			this.lblCaption1.Name = "lblCaption1";
			this.lblCaption1.Size = new System.Drawing.Size(88, 19);
			this.lblCaption1.TabIndex = 27;
			this.lblCaption1.Text = "כותרת1";
			this.lblCaption1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblCaption1.Visible = false;
			// 
			// result1
			// 
			this.result1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.result1.Controller = null;
			this.result1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.result1.ForeColor = System.Drawing.Color.Blue;
			this.result1.Location = new System.Drawing.Point(72, 32);
			this.result1.Name = "result1";
			this.result1.ReadOnly = false;
			this.result1.ShowSpin = false;
			this.result1.Size = new System.Drawing.Size(88, 16);
			this.result1.TabIndex = 26;
			this.result1.Value = "";
			this.result1.Visible = false;
			// 
			// lblCaption2
			// 
			this.lblCaption2.Location = new System.Drawing.Point(168, 64);
			this.lblCaption2.Name = "lblCaption2";
			this.lblCaption2.Size = new System.Drawing.Size(88, 19);
			this.lblCaption2.TabIndex = 29;
			this.lblCaption2.Text = "כותרת2";
			this.lblCaption2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblCaption2.Visible = false;
			// 
			// result2
			// 
			this.result2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.result2.Controller = null;
			this.result2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.result2.ForeColor = System.Drawing.Color.Blue;
			this.result2.Location = new System.Drawing.Point(72, 64);
			this.result2.Name = "result2";
			this.result2.ReadOnly = false;
			this.result2.ShowSpin = false;
			this.result2.Size = new System.Drawing.Size(88, 16);
			this.result2.TabIndex = 28;
			this.result2.Value = "";
			this.result2.Visible = false;
			// 
			// lblCaption3
			// 
			this.lblCaption3.Location = new System.Drawing.Point(168, 96);
			this.lblCaption3.Name = "lblCaption3";
			this.lblCaption3.Size = new System.Drawing.Size(88, 19);
			this.lblCaption3.TabIndex = 31;
			this.lblCaption3.Text = "כותרת3";
			this.lblCaption3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblCaption3.Visible = false;
			// 
			// result3
			// 
			this.result3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.result3.Controller = null;
			this.result3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.result3.ForeColor = System.Drawing.Color.Blue;
			this.result3.Location = new System.Drawing.Point(72, 96);
			this.result3.Name = "result3";
			this.result3.ReadOnly = false;
			this.result3.ShowSpin = false;
			this.result3.Size = new System.Drawing.Size(88, 16);
			this.result3.TabIndex = 30;
			this.result3.Value = "";
			this.result3.Visible = false;
			// 
			// lblCaption4
			// 
			this.lblCaption4.Location = new System.Drawing.Point(168, 128);
			this.lblCaption4.Name = "lblCaption4";
			this.lblCaption4.Size = new System.Drawing.Size(88, 19);
			this.lblCaption4.TabIndex = 33;
			this.lblCaption4.Text = "כותרת4";
			this.lblCaption4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblCaption4.Visible = false;
			// 
			// result4
			// 
			this.result4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.result4.Controller = null;
			this.result4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.result4.ForeColor = System.Drawing.Color.Blue;
			this.result4.Location = new System.Drawing.Point(72, 128);
			this.result4.Name = "result4";
			this.result4.ReadOnly = false;
			this.result4.ShowSpin = false;
			this.result4.Size = new System.Drawing.Size(88, 16);
			this.result4.TabIndex = 32;
			this.result4.Value = "";
			this.result4.Visible = false;
			// 
			// lblCaption5
			// 
			this.lblCaption5.Location = new System.Drawing.Point(168, 160);
			this.lblCaption5.Name = "lblCaption5";
			this.lblCaption5.Size = new System.Drawing.Size(88, 19);
			this.lblCaption5.TabIndex = 35;
			this.lblCaption5.Text = "כותרת5";
			this.lblCaption5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblCaption5.Visible = false;
			// 
			// result5
			// 
			this.result5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.result5.Controller = null;
			this.result5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.result5.ForeColor = System.Drawing.Color.Blue;
			this.result5.Location = new System.Drawing.Point(72, 160);
			this.result5.Name = "result5";
			this.result5.ReadOnly = false;
			this.result5.ShowSpin = false;
			this.result5.Size = new System.Drawing.Size(88, 16);
			this.result5.TabIndex = 34;
			this.result5.Value = "";
			this.result5.Visible = false;
			// 
			// lblCaption6
			// 
			this.lblCaption6.Location = new System.Drawing.Point(168, 192);
			this.lblCaption6.Name = "lblCaption6";
			this.lblCaption6.Size = new System.Drawing.Size(88, 19);
			this.lblCaption6.TabIndex = 37;
			this.lblCaption6.Text = "כותרת6";
			this.lblCaption6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lblCaption6.Visible = false;
			// 
			// result6
			// 
			this.result6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.result6.Controller = null;
			this.result6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.result6.ForeColor = System.Drawing.Color.Blue;
			this.result6.Location = new System.Drawing.Point(72, 192);
			this.result6.Name = "result6";
			this.result6.ReadOnly = false;
			this.result6.ShowSpin = false;
			this.result6.Size = new System.Drawing.Size(88, 16);
			this.result6.TabIndex = 36;
			this.result6.Value = "";
			this.result6.Visible = false;
			// 
			// CompetitorResultForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 261);
			this.Controls.Add(this.lblCaption6);
			this.Controls.Add(this.result6);
			this.Controls.Add(this.lblCaption5);
			this.Controls.Add(this.result5);
			this.Controls.Add(this.lblCaption4);
			this.Controls.Add(this.result4);
			this.Controls.Add(this.lblCaption3);
			this.Controls.Add(this.result3);
			this.Controls.Add(this.lblCaption2);
			this.Controls.Add(this.result2);
			this.Controls.Add(this.lblCaption1);
			this.Controls.Add(this.result1);
			this.Controls.Add(this.lblPlayerName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Name = "CompetitorResultForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CompetitorResultForm";
			this.Load += new System.EventHandler(this.CompetitorResultForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void CompetitorResultForm_Load(object sender, System.EventArgs e)
		{
		
		}

		private void SetCaptionText(int index, CaptionResultContainer container)
		{
			Sport.UI.Controls.TextControl control=null;
			System.Windows.Forms.Label label=null;
			switch (index)
			{
				case 1:
					control = result1;
					label = lblCaption1;
					break;
				case 2:
					control = result2;
					label = lblCaption2;
					break;
				case 3:
					control = result3;
					label = lblCaption3;
					break;
				case 4:
					control = result4;
					label = lblCaption4;
					break;
				case 5:
					control = result5;
					label = lblCaption5;
					break;
				case 6:
					control = result6;
					label = lblCaption6;
					break;
			}
			if (control != null)
			{
				label.Text = container.text;
				label.Visible = (container.text.Length > 0);
				control.Visible = (container.text.Length > 0);
				control.Controller = container.controller;
				if (container.text.IndexOf("אלפיות") >= 0)
					(control.Controller as Sport.UI.Controls.NumberController).ArrowValue = 10;
				control.ShowSpin = true;
			}
		}
		
		private Sport.UI.Controls.TextControl GetResultControl(int index)
		{
			Sport.UI.Controls.TextControl control=null;
			switch (index)
			{
				case 1:
					control = result1;
					break;
				case 2:
					control = result2;
					break;
				case 3:
					control = result3;
					break;
				case 4:
					control = result4;
					break;
				case 5:
					control = result5;
					break;
				case 6:
					control = result6;
					break;
			}
			return control;
		}

		private void HideCaptions()
		{
			result1.Visible = false;
			lblCaption1.Visible = false;
			result2.Visible = false;
			lblCaption2.Visible = false;
			result3.Visible = false;
			lblCaption3.Visible = false;
			result4.Visible = false;
			lblCaption4.Visible = false;
			result5.Visible = false;
			lblCaption5.Visible = false;
			result6.Visible = false;
			lblCaption6.Visible = false;
		}

		private void SetTimeCaptions(Sport.Rulesets.Rules.ResultType resultType)
		{
			HideCaptions();
			ArrayList arrCaptions=new ArrayList();
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Days) != 0)
				arrCaptions.Add(new CaptionResultContainer("ימים:", 0, 999));
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Hours) != 0)
				arrCaptions.Add(new CaptionResultContainer("שעות:", 0, 23));
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Minutes) != 0)
				arrCaptions.Add(new CaptionResultContainer("דקות:", 0, 59));
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Seconds) != 0)
				arrCaptions.Add(new CaptionResultContainer("שניות:", 0, 59));
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Miliseconds) != 0)
				arrCaptions.Add(new CaptionResultContainer("אלפיות:", 0, 999));
			for (int i=0; i<arrCaptions.Count; i++)
				SetCaptionText(i+1, (arrCaptions[i] as CaptionResultContainer));
		}

		private void SetDistanceCaptions(Sport.Rulesets.Rules.ResultType resultType)
		{
			HideCaptions();
			ArrayList arrCaptions=new ArrayList();
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Kilometers) != 0)
				arrCaptions.Add(new CaptionResultContainer("קילומטרים:", 0, 999));
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Meters) != 0)
				arrCaptions.Add(new CaptionResultContainer("מטרים:", 0, 999));
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Centimeters) != 0)
				arrCaptions.Add(new CaptionResultContainer("סנטימטרים:", 0, 99));
			if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Milimeters) != 0)
				arrCaptions.Add(new CaptionResultContainer("מילימטרים:", 0, 9));
			for (int i=0; i<arrCaptions.Count; i++)
				SetCaptionText(i+1, (arrCaptions[i] as CaptionResultContainer));
		}

		private void SetPointsCaptions(Sport.Rulesets.Rules.ResultType resultType)
		{
			HideCaptions();
			SetCaptionText(1, new CaptionResultContainer("נקודות:", 0, 999));
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			bool bClose = false;
			string result="";
			for (int i=1; i<=6; i++)
			{
				Sport.UI.Controls.TextControl control=GetResultControl(i);
				if (control.Visible == false)
					break;
				if (control.Text.Length == 0)
					control.Value = 0;
				result += control.Text+Sport.Core.Data.ResultSeperator;
			}
			if (result.Length > 0)
				result = result.Substring(0, result.Length-1);
			/*if (_competitor.Set(result))
			{
				bClose = true;
			}
			else
			{
				Sport.UI.MessageBox.Show("כישלון בשמירת התוצאה");
			}*/
			
			if (bClose)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private class CaptionResultContainer
		{
			public string text="";
			public Sport.UI.Controls.NumberController controller=null;

			public CaptionResultContainer(string strText, double min, double max)
			{
				this.text = strText;
				this.controller = new Sport.UI.Controls.NumberController(min, max);
			}
		}
	}
}
