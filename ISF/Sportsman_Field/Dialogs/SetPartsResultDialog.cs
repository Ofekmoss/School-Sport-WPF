using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Common;

namespace Sportsman_Field.Dialogs
{
	/// <summary>
	/// Summary description for SetPartsResultDialog.
	/// </summary>
	public class SetPartsResultDialog : System.Windows.Forms.Form
	{
		private readonly int MaxExtensions=5;
		private System.Windows.Forms.Button btnAddPart;
		private System.Windows.Forms.Button btnRemovePart;
		private System.Windows.Forms.ListBox lbGameParts;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.GroupBox gbGameParts;
		private System.Windows.Forms.NumericUpDown nudScore_A;
		private System.Windows.Forms.Label lbTeam_A;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lbTeam_B;
		private System.Windows.Forms.NumericUpDown nudScore_B;
		private System.Windows.Forms.Label label3;

		private int _partsCount=0;
		private MatchData _match=null;
		private bool _changedByCode=false;

		public SetPartsResultDialog(int partsCount, MatchData match)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_partsCount = partsCount;
			_match = match;
		}

		public string PartsResult
		{
			get
			{
				string result="";
				for (int i=0; i<lbGameParts.Items.Count; i++)
				{
					PartData part=(PartData) 
						(lbGameParts.Items[i] as ListItem).Value;
					if ((part.ScoreA < 0)||(part.ScoreB < 0))
						break;
					result += part.ScoreA.ToString()+"-"+part.ScoreB.ToString();
					result += "|";
				}
				if (result.Length > 0)
					result = result.Substring(0, result.Length-1);
				return result;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SetPartsResultDialog));
			this.lbGameParts = new System.Windows.Forms.ListBox();
			this.gbGameParts = new System.Windows.Forms.GroupBox();
			this.btnRemovePart = new System.Windows.Forms.Button();
			this.btnAddPart = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lbTeam_A = new System.Windows.Forms.Label();
			this.nudScore_A = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lbTeam_B = new System.Windows.Forms.Label();
			this.nudScore_B = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.gbGameParts.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudScore_A)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudScore_B)).BeginInit();
			this.SuspendLayout();
			// 
			// lbGameParts
			// 
			this.lbGameParts.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbGameParts.ForeColor = System.Drawing.Color.Blue;
			this.lbGameParts.ItemHeight = 14;
			this.lbGameParts.Location = new System.Drawing.Point(8, 16);
			this.lbGameParts.Name = "lbGameParts";
			this.lbGameParts.Size = new System.Drawing.Size(104, 130);
			this.lbGameParts.TabIndex = 0;
			this.lbGameParts.SelectedIndexChanged += new System.EventHandler(this.lbGameParts_SelectedIndexChanged);
			// 
			// gbGameParts
			// 
			this.gbGameParts.Controls.Add(this.btnRemovePart);
			this.gbGameParts.Controls.Add(this.btnAddPart);
			this.gbGameParts.Controls.Add(this.lbGameParts);
			this.gbGameParts.Location = new System.Drawing.Point(216, 8);
			this.gbGameParts.Name = "gbGameParts";
			this.gbGameParts.Size = new System.Drawing.Size(120, 216);
			this.gbGameParts.TabIndex = 1;
			this.gbGameParts.TabStop = false;
			this.gbGameParts.Text = "חלקי משחק";
			// 
			// btnRemovePart
			// 
			this.btnRemovePart.Enabled = false;
			this.btnRemovePart.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemovePart.ForeColor = System.Drawing.Color.Red;
			this.btnRemovePart.Image = ((System.Drawing.Image)(resources.GetObject("btnRemovePart.Image")));
			this.btnRemovePart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnRemovePart.Location = new System.Drawing.Point(8, 184);
			this.btnRemovePart.Name = "btnRemovePart";
			this.btnRemovePart.Size = new System.Drawing.Size(102, 23);
			this.btnRemovePart.TabIndex = 2;
			this.btnRemovePart.Text = "הסר הארכה";
			this.btnRemovePart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnRemovePart.Click += new System.EventHandler(this.btnRemovePart_Click);
			// 
			// btnAddPart
			// 
			this.btnAddPart.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnAddPart.Image = ((System.Drawing.Image)(resources.GetObject("btnAddPart.Image")));
			this.btnAddPart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnAddPart.Location = new System.Drawing.Point(9, 152);
			this.btnAddPart.Name = "btnAddPart";
			this.btnAddPart.Size = new System.Drawing.Size(102, 23);
			this.btnAddPart.TabIndex = 1;
			this.btnAddPart.Text = "הוסף הארכה";
			this.btnAddPart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnAddPart.Click += new System.EventHandler(this.btnAddPart_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.Color.White;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCancel.ForeColor = System.Drawing.Color.Red;
			this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
			this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.Location = new System.Drawing.Point(8, 200);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(88, 23);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "ביטול";
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.Color.White;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnOK.ForeColor = System.Drawing.Color.Black;
			this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
			this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnOK.Location = new System.Drawing.Point(120, 200);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 23);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "אישור";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.lbTeam_A);
			this.groupBox2.Controls.Add(this.nudScore_A);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(8, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 88);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "קבוצה א\'";
			// 
			// lbTeam_A
			// 
			this.lbTeam_A.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTeam_A.ForeColor = System.Drawing.Color.Blue;
			this.lbTeam_A.Location = new System.Drawing.Point(8, 24);
			this.lbTeam_A.Name = "lbTeam_A";
			this.lbTeam_A.Size = new System.Drawing.Size(184, 23);
			this.lbTeam_A.TabIndex = 12;
			this.lbTeam_A.Text = "עירוני ט\' ראשון לציון";
			// 
			// nudScore_A
			// 
			this.nudScore_A.DecimalPlaces = 1;
			this.nudScore_A.Enabled = false;
			this.nudScore_A.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.nudScore_A.ForeColor = System.Drawing.Color.Blue;
			this.nudScore_A.Location = new System.Drawing.Point(16, 55);
			this.nudScore_A.Maximum = new System.Decimal(new int[] {
																	   999,
																	   0,
																	   0,
																	   0});
			this.nudScore_A.Name = "nudScore_A";
			this.nudScore_A.Size = new System.Drawing.Size(96, 25);
			this.nudScore_A.TabIndex = 11;
			this.nudScore_A.Click += new System.EventHandler(this.nudScore_A_ValueChanged);
			this.nudScore_A.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nudScore_A_KeyUp);
			this.nudScore_A.ValueChanged += new System.EventHandler(this.nudScore_A_ValueChanged);
			this.nudScore_A.Leave += new System.EventHandler(this.nudScore_A_ValueChanged);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 11F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(120, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 23);
			this.label2.TabIndex = 10;
			this.label2.Text = "תוצאה: ";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lbTeam_B);
			this.groupBox1.Controls.Add(this.nudScore_B);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(8, 104);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 88);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "קבוצה ב\'";
			// 
			// lbTeam_B
			// 
			this.lbTeam_B.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbTeam_B.ForeColor = System.Drawing.Color.Blue;
			this.lbTeam_B.Location = new System.Drawing.Point(8, 24);
			this.lbTeam_B.Name = "lbTeam_B";
			this.lbTeam_B.Size = new System.Drawing.Size(184, 23);
			this.lbTeam_B.TabIndex = 12;
			this.lbTeam_B.Text = "עירוני ט\' ראשון לציון";
			// 
			// nudScore_B
			// 
			this.nudScore_B.DecimalPlaces = 1;
			this.nudScore_B.Enabled = false;
			this.nudScore_B.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.nudScore_B.ForeColor = System.Drawing.Color.Blue;
			this.nudScore_B.Location = new System.Drawing.Point(16, 55);
			this.nudScore_B.Maximum = new System.Decimal(new int[] {
																	   999,
																	   0,
																	   0,
																	   0});
			this.nudScore_B.Name = "nudScore_B";
			this.nudScore_B.Size = new System.Drawing.Size(96, 25);
			this.nudScore_B.TabIndex = 11;
			this.nudScore_B.Click += new System.EventHandler(this.nudScore_B_ValueChanged);
			this.nudScore_B.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nudScore_B_KeyUp);
			this.nudScore_B.ValueChanged += new System.EventHandler(this.nudScore_B_ValueChanged);
			this.nudScore_B.Leave += new System.EventHandler(this.nudScore_B_ValueChanged);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 11F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label3.Location = new System.Drawing.Point(120, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(72, 23);
			this.label3.TabIndex = 10;
			this.label3.Text = "תוצאה: ";
			// 
			// SetPartsResultDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(344, 231);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.gbGameParts);
			this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SetPartsResultDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "קביעת ניקוד חלקי משחק";
			this.Load += new System.EventHandler(this.SetPartsResultDialog_Load);
			this.gbGameParts.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudScore_A)).EndInit();
			this.groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudScore_B)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void SetPartsResultDialog_Load(object sender, System.EventArgs e)
		{
			FillTeams();
			FillParts();
		}

		private void FillTeams()
		{
			if ((_match == null)||(_match.TeamA == null)||(_match.TeamB == null))
				throw new Exception("Set Parts Result: match or one of its team empty");

			string teamA=_match.TeamA.School.Name;
			if (_match.TeamA.TeamIndex > 0)
				teamA += " "+Tools.ToHebLetter(_match.TeamA.TeamIndex);
			string teamB=_match.TeamB.School.Name;
			if (_match.TeamB.TeamIndex > 0)
				teamB += " "+Tools.ToHebLetter(_match.TeamB.TeamIndex);

			lbTeam_A.Text = teamA;
			lbTeam_B.Text = teamB;
		}

		private void FillParts()
		{
			lbGameParts.Items.Clear();
			PartData[] parts=GetMatchParts(_match);
			int i;
			for (i=0; i<_partsCount; i++)
			{
				PartData partData=(i < parts.Length)?parts[i]:new PartData();
				lbGameParts.Items.Add(new ListItem(IntToPart(i+1), partData));
			}
			for (i=0; i<parts.Length-(_partsCount); i++)
			{
				PartData partData=parts[i+(_partsCount)];
				lbGameParts.Items.Add(new ListItem(
					"הארכה "+Tools.IntToHebrew(i+1, true), partData));
			}
		}
		
		private void btnAddPart_Click(object sender, System.EventArgs e)
		{
			if (lbGameParts.Items.Count-(_partsCount) >= MaxExtensions)
			{
				Sport.UI.MessageBox.Error("לא ניתן להוסיף עוד הארכות", "שגיאה");
				return;
			}
			int part=lbGameParts.Items.Count-(_partsCount)+1;
			lbGameParts.Items.Add(new ListItem(
				"הארכה "+Tools.IntToHebrew(part, true), new PartData()));
		}
		
		private void lbGameParts_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			nudScore_A.Enabled = false;
			nudScore_B.Enabled = false;
			_changedByCode = true;
			Tools.SetNudValue(nudScore_A, 0);
			_changedByCode = true;
			Tools.SetNudValue(nudScore_B,  0);
			btnRemovePart.Enabled = ((lbGameParts.Items.Count > _partsCount)&&
				(lbGameParts.SelectedIndex == lbGameParts.Items.Count-1));
			int partIndex=lbGameParts.SelectedIndex;
			if (partIndex < 0)
				return;
			
			PartData selPart=(PartData) (lbGameParts.SelectedItem as ListItem).Value;
			if ((selPart.ScoreA >= 0)&&(selPart.ScoreB >= 0))
			{
				_changedByCode = true;
				Tools.SetNudValue(nudScore_A, selPart.ScoreA);
				_changedByCode = true;
				Tools.SetNudValue(nudScore_B, selPart.ScoreB);
			}
			PartData previousPart=null;
			if (partIndex > 0)
			{
				previousPart = (PartData) 
					(lbGameParts.Items[partIndex-1] as ListItem).Value;
			}
			if ((previousPart == null)||
				((previousPart.ScoreA >= 0)&&(previousPart.ScoreB >= 0)))
			{
				nudScore_A.Enabled = true;
				nudScore_B.Enabled = true;
			}
		}
		
		private void btnRemovePart_Click(object sender, System.EventArgs e)
		{
			int selIndex=lbGameParts.SelectedIndex;
			if (selIndex < (lbGameParts.Items.Count-1))
				return;
			
			if (((double) nudScore_A.Value > 0)||(nudScore_B.Value > 0))
			{
				if (!Sport.UI.MessageBox.Ask("פעולה זו תבטל את התוצאה עבור חלק זה. האם להמשיך?", 
						"הסרת הארכה", false))
					return;
			}
			
			lbGameParts.Items.Remove(lbGameParts.SelectedItem);
		}
		
		#region general methods
		private string IntToPart(int part)
		{
			string strFirstWord="";
			switch (_partsCount)
			{
				case 2:
					strFirstWord = "חצי";
					break;
				case 3:
					strFirstWord = "שליש";
					break;
				case 4:
					strFirstWord = "רבע";
					break;
				case 5:
					strFirstWord = "מערכה"; //"חמישית";
					break;
				case 6:
					strFirstWord = "שישית";
					break;
				case 7:
					strFirstWord = "שביעית";
					break;
				case 8:
					strFirstWord = "שמינית";
					break;
				case 9:
					strFirstWord = "תשיעית";
					break;
				default:
					strFirstWord = "חלק";
					break;
			}
			bool blnFemale;
			if (part > _partsCount)
			{
				strFirstWord = "הארכה";
				blnFemale = true;
			}
			else
			{
				string strTemp=strFirstWord.Substring(strFirstWord.Length-2);
				blnFemale=((strTemp == "ית")||(strTemp.Substring(1) == "ה"));
			}
			return strFirstWord+" "+Tools.IntToHebrew(part, blnFemale);
		}
		
		private PartData[] GetMatchParts(MatchData match)
		{
			string strParts=Tools.CStrDef(match.PartsResult, "");
			if ((strParts == null)||(strParts.Length == 0))
				return new PartData[0];
			string[] arrParts=strParts.Split(new char[] {'|'});
			PartData[] result=new PartData[arrParts.Length];
			for (int i=0; i<arrParts.Length; i++)
			{
				result[i]=new PartData(arrParts[i]);
			}
			return result;
		}
		#endregion

		private void nudScore_A_ValueChanged(object sender, System.EventArgs e)
		{
			if (_changedByCode)
			{
				_changedByCode = false;
				return;
			}
			int selIndex=lbGameParts.SelectedIndex;
			if (selIndex < 0)
			{
				nudScore_A.Enabled = false;
				return;
			}

			PartData part=(PartData) (lbGameParts.SelectedItem as ListItem).Value;
			part.ScoreA = (double) nudScore_A.Value;
		}

		private void nudScore_B_ValueChanged(object sender, System.EventArgs e)
		{
			if (_changedByCode)
			{
				_changedByCode = false;
				return;
			}
			int selIndex=lbGameParts.SelectedIndex;
			if (selIndex < 0)
			{
				nudScore_B.Enabled = false;
				return;
			}

			PartData part=(PartData) (lbGameParts.SelectedItem as ListItem).Value;
			part.ScoreB = (double) nudScore_B.Value;
		}

		private void nudScore_A_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//nudScore_A_ValueChanged(sender, EventArgs.Empty);
		}

		private void nudScore_B_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//nudScore_B_ValueChanged(sender, EventArgs.Empty);
		}

		#region Part Data Class
		private class PartData
		{
			public double ScoreA=-1;
			public double ScoreB=-1;
			public PartData()
			{
				ScoreA=-1;
				ScoreB=-1;
			}
			public PartData(double scoreA, double scoreB)
			{
				this.ScoreA = scoreA;
				this.ScoreB = scoreB;
			}
			public PartData(string strPartResult)
			{
				string[] arrTemp=strPartResult.Split(new char[] {'-'});
				this.ScoreA = Double.Parse(arrTemp[0]);
				this.ScoreB = Double.Parse(arrTemp[1]);
			}
		}
		#endregion
	}
}
