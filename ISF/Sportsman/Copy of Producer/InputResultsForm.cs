using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for InputResultsForm.
	/// </summary>
	public class InputResultsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button btnFinish;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Button btnNextResult;
		private System.Windows.Forms.Timer tmrClock;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Button btnRemoveResult;
		private System.Windows.Forms.Button btnSetTime;
		private System.ComponentModel.IContainer components;
		private System.DateTime _timerStart=DateTime.MinValue;
		private System.Windows.Forms.Label lbMinutes;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lbSeconds;
		
		private CompetitionResultForm parentForm = null;
		private int _timerSeconds=0;
		private int _lastMinutes=0;
		private System.Windows.Forms.ListBox lbResults;
		private int _lastSeconds=0;
		
		public CompetitionResultForm ParentResultForm
		{
			get { return parentForm; }
			set { parentForm = value; }
		}

		public InputResultsForm()
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

		public int[] GetResults()
		{
			int[] result=new int[lbResults.Items.Count];
			for (int i=0; i<lbResults.Items.Count; i++)
			{
				Sport.Common.ListItem item=(Sport.Common.ListItem) lbResults.Items[i];
				result[i] = (int) item.Value;
			}
			return result;
		}
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(InputResultsForm));
			this.btnStart = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnSetTime = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lbSeconds = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lbMinutes = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.btnFinish = new System.Windows.Forms.Button();
			this.panel4 = new System.Windows.Forms.Panel();
			this.panel6 = new System.Windows.Forms.Panel();
			this.btnRemoveResult = new System.Windows.Forms.Button();
			this.lbResults = new System.Windows.Forms.ListBox();
			this.panel5 = new System.Windows.Forms.Panel();
			this.btnNextResult = new System.Windows.Forms.Button();
			this.tmrClock = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel5.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStart
			// 
			this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnStart.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.btnStart.ForeColor = System.Drawing.Color.White;
			this.btnStart.Location = new System.Drawing.Point(184, 0);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(88, 23);
			this.btnStart.TabIndex = 0;
			this.btnStart.Text = "הרץ שעון";
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnSetTime);
			this.panel1.Controls.Add(this.btnStart);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(298, 24);
			this.panel1.TabIndex = 1;
			// 
			// btnSetTime
			// 
			this.btnSetTime.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnSetTime.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.btnSetTime.ForeColor = System.Drawing.Color.White;
			this.btnSetTime.Location = new System.Drawing.Point(40, 0);
			this.btnSetTime.Name = "btnSetTime";
			this.btnSetTime.Size = new System.Drawing.Size(88, 23);
			this.btnSetTime.TabIndex = 1;
			this.btnSetTime.Text = "קבע זמן";
			this.btnSetTime.Click += new System.EventHandler(this.btnSetTime_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.lbSeconds);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Controls.Add(this.lbMinutes);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(298, 112);
			this.panel2.TabIndex = 2;
			// 
			// lbSeconds
			// 
			this.lbSeconds.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbSeconds.Font = new System.Drawing.Font("Tahoma", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbSeconds.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.lbSeconds.Location = new System.Drawing.Point(168, 0);
			this.lbSeconds.Name = "lbSeconds";
			this.lbSeconds.Size = new System.Drawing.Size(136, 112);
			this.lbSeconds.TabIndex = 2;
			this.lbSeconds.Text = "00";
			this.lbSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Left;
			this.label1.Font = new System.Drawing.Font("Tahoma", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.label1.Location = new System.Drawing.Point(136, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 112);
			this.label1.TabIndex = 1;
			this.label1.Text = ":";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lbMinutes
			// 
			this.lbMinutes.Dock = System.Windows.Forms.DockStyle.Left;
			this.lbMinutes.Font = new System.Drawing.Font("Tahoma", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbMinutes.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.lbMinutes.Location = new System.Drawing.Point(0, 0);
			this.lbMinutes.Name = "lbMinutes";
			this.lbMinutes.Size = new System.Drawing.Size(136, 112);
			this.lbMinutes.TabIndex = 0;
			this.lbMinutes.Text = "00";
			this.lbMinutes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.btnFinish);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel3.Location = new System.Drawing.Point(0, 336);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(298, 24);
			this.panel3.TabIndex = 3;
			// 
			// btnFinish
			// 
			this.btnFinish.BackColor = System.Drawing.SystemColors.Control;
			this.btnFinish.Dock = System.Windows.Forms.DockStyle.Left;
			this.btnFinish.Location = new System.Drawing.Point(0, 0);
			this.btnFinish.Name = "btnFinish";
			this.btnFinish.Size = new System.Drawing.Size(75, 24);
			this.btnFinish.TabIndex = 0;
			this.btnFinish.Text = "סיום";
			this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.panel6);
			this.panel4.Controls.Add(this.lbResults);
			this.panel4.Controls.Add(this.panel5);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(0, 136);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(298, 200);
			this.panel4.TabIndex = 5;
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.btnRemoveResult);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel6.Location = new System.Drawing.Point(0, 176);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(178, 24);
			this.panel6.TabIndex = 4;
			// 
			// btnRemoveResult
			// 
			this.btnRemoveResult.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.btnRemoveResult.Dock = System.Windows.Forms.DockStyle.Right;
			this.btnRemoveResult.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnRemoveResult.ForeColor = System.Drawing.Color.White;
			this.btnRemoveResult.Location = new System.Drawing.Point(90, 0);
			this.btnRemoveResult.Name = "btnRemoveResult";
			this.btnRemoveResult.Size = new System.Drawing.Size(88, 24);
			this.btnRemoveResult.TabIndex = 1;
			this.btnRemoveResult.Text = "הסר תוצאה";
			this.btnRemoveResult.Click += new System.EventHandler(this.btnRemoveResult_Click);
			// 
			// lbResults
			// 
			this.lbResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbResults.Dock = System.Windows.Forms.DockStyle.Top;
			this.lbResults.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbResults.ItemHeight = 19;
			this.lbResults.Location = new System.Drawing.Point(0, 0);
			this.lbResults.Name = "lbResults";
			this.lbResults.Size = new System.Drawing.Size(178, 173);
			this.lbResults.TabIndex = 3;
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.btnNextResult);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel5.Location = new System.Drawing.Point(178, 0);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(120, 200);
			this.panel5.TabIndex = 2;
			// 
			// btnNextResult
			// 
			this.btnNextResult.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.btnNextResult.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnNextResult.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnNextResult.ForeColor = System.Drawing.Color.White;
			this.btnNextResult.Location = new System.Drawing.Point(0, 0);
			this.btnNextResult.Name = "btnNextResult";
			this.btnNextResult.Size = new System.Drawing.Size(120, 32);
			this.btnNextResult.TabIndex = 1;
			this.btnNextResult.Text = "הוסף תוצאה";
			this.btnNextResult.Click += new System.EventHandler(this.btnNextResult_Click);
			this.btnNextResult.KeyUp += new System.Windows.Forms.KeyEventHandler(this.btnNextResult_KeyUp);
			// 
			// tmrClock
			// 
			this.tmrClock.Interval = 500;
			this.tmrClock.Tick += new System.EventHandler(this.tmrClock_Tick);
			// 
			// InputResultsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(298, 360);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "InputResultsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "קליטת תוצאות";
			this.Load += new System.EventHandler(this.InputResultsForm_Load);
			this.LocationChanged += new System.EventHandler(this.InputResultsForm_LocationChanged);
			this.Closed += new System.EventHandler(this.InputResultsForm_Closed);
			this.Activated += new System.EventHandler(this.InputResultsForm_Activated);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		private void tmrClock_Tick(object sender, System.EventArgs e)
		{
			//get difference:
			int seconds=GetCurrentSeconds();
			
			//calculate minutes and seconds:
			_lastMinutes = ((int) (((double) seconds)/((double) 60)));
			_lastSeconds = (seconds % 60);
			
			//display:
			lbMinutes.Text = _lastMinutes.ToString().PadLeft(2, '0');
			lbSeconds.Text = _lastSeconds.ToString().PadLeft(2, '0');
		}

		private void InputResultsForm_Load(object sender, System.EventArgs e)
		{
			//get last seconds:
			_timerSeconds = Sport.Common.Tools.CIntDef(
				Sport.Core.Configuration.ReadString("General", "InputResultSeconds"), 0);
			
			//got anything?
			if (_timerSeconds > 0)
			{
				//calculate minutes and seconds:
				_lastMinutes = ((int) (((double) _timerSeconds)/((double) 60)));
				_lastSeconds = (_timerSeconds % 60);
				
				//display:
				lbMinutes.Text = _lastMinutes.ToString().PadLeft(2, '0');
				lbSeconds.Text = _lastSeconds.ToString().PadLeft(2, '0');
			}
			
			//get last results:
			string strLastResults=Sport.Common.Tools.CStrDef(
				Sport.Core.Configuration.ReadString("General", "InputResultResults"), "");
			
			//got anything?
			if (strLastResults.Length > 0)
			{
				//split:
				string[] arrLastResults=strLastResults.Split(new char[] {'|'});

				//add to list:
				for (int i=0; i<arrLastResults.Length; i++)
				{
					int curSeconds=Sport.Common.Tools.CIntDef(arrLastResults[i], 0);
					if (curSeconds > 0)
						AddResult(curSeconds);
				}
			}
		}

		private void btnStart_Click(object sender, System.EventArgs e)
		{
			btnStart.Enabled = false;
			
			_timerStart = DateTime.Now;
			tmrClock.Enabled = true;
			
			btnNextResult.Focus();
		}

		private void btnSetTime_Click(object sender, System.EventArgs e)
		{
			tmrClock.Enabled = false;
			Sport.UI.Dialogs.GenericEditDialog objDialog=
				new Sport.UI.Dialogs.GenericEditDialog("קביעת זמן");
			objDialog.Items.Add("דקות", Sport.UI.Controls.GenericItemType.Number, 
				_lastMinutes, new object[] {0, 99});
			objDialog.Items.Add("שניות", Sport.UI.Controls.GenericItemType.Number, 
				_lastSeconds, new object[] {0, 59});
			if (objDialog.ShowDialog(this) == DialogResult.OK)
			{
				_timerSeconds = (int) ((((double) objDialog.Items[0].Value)*60)+
					((double) objDialog.Items[1].Value));
				_timerStart = DateTime.Now;
			}
			tmrClock.Enabled = true;
			btnStart.Enabled = false;
			btnNextResult.Focus();
		}
		
		private void btnNextResult_Click(object sender, System.EventArgs e)
		{
			if (tmrClock.Enabled == false)
				return;
			AddResult(GetCurrentSeconds());
		}
		
		private void AddResult(int totalSeconds)
		{
			int minutes=((int) (((double) totalSeconds)/((double) 60)));
			int seconds=(totalSeconds % 60);
			string text=Sport.Common.Tools.Pad(
				minutes.ToString().PadLeft(2, '0')+":"+
				seconds.ToString().PadLeft(2, '0'), ' ', 12);				
			lbResults.Items.Add(new Sport.Common.ListItem(text, totalSeconds));
		}
		
		private void btnRemoveResult_Click(object sender, System.EventArgs e)
		{
			if (lbResults.SelectedIndex >= 0)
			{
				lbResults.Items.RemoveAt(lbResults.SelectedIndex);
			}
			btnNextResult.Focus();
		}

		private void InputResultsForm_Closed(object sender, System.EventArgs e)
		{
			if (tmrClock.Enabled)
			{
				int seconds=GetCurrentSeconds();
				Sport.Core.Configuration.WriteString("General", "InputResultSeconds", 
					seconds.ToString());
			}
			
			if (lbResults.Items.Count > 0)
			{
				string results=String.Join("|", 
					Sport.Common.Tools.ToStringArray(GetResults()));
				Sport.Core.Configuration.WriteString("General", "InputResultResults", results);
			}
		}
		
		private int GetCurrentSeconds()
		{
			return _timerSeconds+((int) (DateTime.Now-_timerStart).TotalSeconds);
		}

		private void btnNextResult_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Tab)
			{
				btnNextResult_Click(sender, EventArgs.Empty);
				e.Handled = true;
				return;
			}
			e.Handled = false;
		}

		private void btnFinish_Click(object sender, System.EventArgs e)
		{
			if (parentForm != null)
			{
				parentForm.SetCustomResults(this.GetResults());
			}
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		private void InputResultsForm_Activated(object sender, System.EventArgs e)
		{
			if (parentForm != null)
			{
				parentForm.WindowState = System.Windows.Forms.FormWindowState.Normal;
				parentForm.Show();
			}
		}

		private void InputResultsForm_LocationChanged(object sender, System.EventArgs e)
		{
			if (this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
			{
				if (parentForm != null)
				{
					parentForm.WindowState = System.Windows.Forms.FormWindowState.Minimized;
				}
			}
		}
	}
}
