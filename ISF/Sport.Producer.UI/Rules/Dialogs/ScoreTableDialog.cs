using System;

using Sport.Rulesets.Rules;
using System.Windows.Forms;
using System.Collections;

namespace Sport.Producer.UI.Rules.Dialogs
{
	public class ScoreTableDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button tbCancel;
		private Sport.UI.Controls.Grid grid;
		private System.Windows.Forms.Label lbTitle;
		private System.Windows.Forms.Button btnExport;
		private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Button tbOk;
		private int _sportField=-1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown nudMaxScore;
		private System.Windows.Forms.Button btnUpdateMaxScore;
		private int _sportFieldType=-1;
		
		private class ScoreTableGridSource : Sport.UI.Controls.IGridSource
		{
			private ScoreTable _scoreTable;

			public ScoreTable ScoreTable
			{
				get { return _scoreTable; }
				set
				{
					_scoreTable = value;
					Rebuild();
					if (_grid != null)
						_grid.Refresh();
				}
			}

			private ResultType _resultType;
			private int[] results;

			public ScoreTableGridSource(ScoreTable scoreTable, ResultType resultType)
			{
				_scoreTable = scoreTable;
				_resultType = resultType;
				results = new int[scoreTable.Count];
				
				Rebuild();
			}
			
			private void Rebuild()
			{
				for (int n = 0; n < results.Length; n++)
					results[n] = _scoreTable.GetResult(n + 1);
			}
			
			public ScoreTable GetScoreTable()
			{
				return new ScoreTable(results, _resultType.Direction);
			}
			
			#region IGridSource Members
			private Sport.UI.Controls.Grid _grid;

			public void SetGrid(Sport.UI.Controls.Grid grid)
			{
				_grid = grid;
				if (_grid != null)
					_grid.SelectOnSpace = true;
			}

			public void Sort(int group, int[] columns)
			{
			}

			public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
			{
				return null;
			}

			public string GetTip(int row)
			{
				return null;
			}

			public int GetGroup(int row)
			{
				return 0;
			}

			private Sport.UI.Controls.TextItemsControl resultControl = null;

			public System.Windows.Forms.Control Edit(int row, int field)
			{
				if (field == 1)
				{
					if (resultControl == null)
					{
						resultControl = new Sport.UI.Controls.TextItemsControl();
						resultControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
						resultControl.ShowSpin = true;
						resultControl.SetFormatterItems(_resultType.ValueFormatter);
						resultControl.KeyDown += new KeyEventHandler(resultControl_KeyDown);
					}
					
					resultControl.Tag = (results.Length - 1) - row;
					resultControl.SetValue(_resultType.ValueFormatter, results[(int) resultControl.Tag]);
					resultControl.SelectionLength = 0;
					resultControl.SelectionStart = 0;

					return resultControl;
				}

				return null;
			}

			public void EditEnded(System.Windows.Forms.Control control)
			{
				results[(int) resultControl.Tag] = resultControl.GetValue(_resultType.ValueFormatter);
			}

			public int GetRowCount()
			{
				return results.Length;
			}

			public int[] GetSort(int group)
			{
				// TODO:  Add ScoreTableGridSource.GetSort implementation
				return null;
			}

			public string GetText(int row, int field)
			{
				switch (field)
				{
					case (0): // score
						return (this.GetRowCount() - row).ToString();
					case (1): // result
						return _resultType.FormatResult(results[(this.GetRowCount() - 1) - row]);
				}

				return null;
			}

			public int GetFieldCount(int row)
			{
				return 2;
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				// TODO:  Add ScoreTableGridSource.Dispose implementation
			}

			#endregion

			private void resultControl_KeyDown(object sender, KeyEventArgs e)
			{
				if (_grid == null)
					return;
				
				if ((e.KeyData == Keys.Enter)||(e.KeyData == Keys.Tab)||
					(e.KeyData == Keys.Space))
				{
					int count = _grid.Source.GetRowCount();
					int row = (count - 1) - (int) resultControl.Tag;
					if (row < (count - 1))
					{
						if (e.KeyData == Keys.Space)
						{
							_grid.EditField(row, 1);
						}
						else
						{
							_grid.EditField(row + 1, 1);
						}
						e.Handled = true;
					}
				}
			}
		}
	
		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.Panel();
			this.btnUpdateMaxScore = new System.Windows.Forms.Button();
			this.nudMaxScore = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.btnCopy = new System.Windows.Forms.Button();
			this.btnImport = new System.Windows.Forms.Button();
			this.btnExport = new System.Windows.Forms.Button();
			this.lbTitle = new System.Windows.Forms.Label();
			this.grid = new Sport.UI.Controls.Grid();
			this.tbOk = new System.Windows.Forms.Button();
			this.tbCancel = new System.Windows.Forms.Button();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxScore)).BeginInit();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.btnUpdateMaxScore);
			this.panel.Controls.Add(this.nudMaxScore);
			this.panel.Controls.Add(this.label1);
			this.panel.Controls.Add(this.btnCopy);
			this.panel.Controls.Add(this.btnImport);
			this.panel.Controls.Add(this.btnExport);
			this.panel.Controls.Add(this.lbTitle);
			this.panel.Controls.Add(this.grid);
			this.panel.Controls.Add(this.tbOk);
			this.panel.Controls.Add(this.tbCancel);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(272, 376);
			this.panel.TabIndex = 8;
			this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			// 
			// btnUpdateMaxScore
			// 
			this.btnUpdateMaxScore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUpdateMaxScore.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnUpdateMaxScore.Location = new System.Drawing.Point(48, 30);
			this.btnUpdateMaxScore.Name = "btnUpdateMaxScore";
			this.btnUpdateMaxScore.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.btnUpdateMaxScore.Size = new System.Drawing.Size(48, 20);
			this.btnUpdateMaxScore.TabIndex = 19;
			this.btnUpdateMaxScore.Text = "עדכן";
			this.btnUpdateMaxScore.Click += new System.EventHandler(this.btnUpdateMaxScore_Click);
			// 
			// nudMaxScore
			// 
			this.nudMaxScore.Location = new System.Drawing.Point(104, 30);
			this.nudMaxScore.Maximum = new System.Decimal(new int[] {
																		9999,
																		0,
																		0,
																		0});
			this.nudMaxScore.Minimum = new System.Decimal(new int[] {
																		100,
																		0,
																		0,
																		0});
			this.nudMaxScore.Name = "nudMaxScore";
			this.nudMaxScore.Size = new System.Drawing.Size(64, 21);
			this.nudMaxScore.TabIndex = 18;
			this.nudMaxScore.Value = new System.Decimal(new int[] {
																	  100,
																	  0,
																	  0,
																	  0});
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(176, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 17);
			this.label1.TabIndex = 17;
			this.label1.Text = "ניקוד מקסימלי:";
			// 
			// btnCopy
			// 
			this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnCopy.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnCopy.Location = new System.Drawing.Point(200, 344);
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.btnCopy.Size = new System.Drawing.Size(56, 20);
			this.btnCopy.TabIndex = 16;
			this.btnCopy.Text = "העתק";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			// 
			// btnImport
			// 
			this.btnImport.BackColor = System.Drawing.Color.White;
			this.btnImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnImport.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnImport.ForeColor = System.Drawing.Color.Red;
			this.btnImport.Location = new System.Drawing.Point(80, 312);
			this.btnImport.Name = "btnImport";
			this.btnImport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.btnImport.Size = new System.Drawing.Size(120, 20);
			this.btnImport.TabIndex = 15;
			this.btnImport.Text = "טעינה מקובץ טקסט";
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// btnExport
			// 
			this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnExport.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.btnExport.Location = new System.Drawing.Point(192, 296);
			this.btnExport.Name = "btnExport";
			this.btnExport.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.btnExport.Size = new System.Drawing.Size(32, 20);
			this.btnExport.TabIndex = 14;
			this.btnExport.Text = "יצא";
			this.btnExport.Visible = false;
			this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
			// 
			// lbTitle
			// 
			this.lbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lbTitle.Location = new System.Drawing.Point(8, 7);
			this.lbTitle.Name = "lbTitle";
			this.lbTitle.Size = new System.Drawing.Size(256, 16);
			this.lbTitle.TabIndex = 13;
			this.lbTitle.Text = "ניקוד תוצאות";
			this.lbTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseUp);
			this.lbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseMove);
			this.lbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbTitle_MouseDown);
			// 
			// grid
			// 
			this.grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.grid.Editable = true;
			this.grid.ExpandOnDoubleClick = true;
			this.grid.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.grid.HeaderHeight = 17;
			this.grid.HorizontalLines = true;
			this.grid.Location = new System.Drawing.Point(8, 56);
			this.grid.Name = "grid";
			this.grid.SelectedIndex = -1;
			this.grid.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.grid.SelectOnSpace = false;
			this.grid.ShowCheckBoxes = false;
			this.grid.ShowRowNumber = false;
			this.grid.Size = new System.Drawing.Size(256, 248);
			this.grid.TabIndex = 12;
			this.grid.VerticalLines = true;
			this.grid.VisibleRow = 0;
			// 
			// tbOk
			// 
			this.tbOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbOk.Location = new System.Drawing.Point(8, 344);
			this.tbOk.Name = "tbOk";
			this.tbOk.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.tbOk.Size = new System.Drawing.Size(68, 20);
			this.tbOk.TabIndex = 11;
			this.tbOk.Text = "אישור";
			this.tbOk.Click += new System.EventHandler(this.tbOk_Click);
			// 
			// tbCancel
			// 
			this.tbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.tbCancel.Location = new System.Drawing.Point(80, 344);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Size = new System.Drawing.Size(68, 20);
			this.tbCancel.TabIndex = 10;
			this.tbCancel.Text = "ביטול";
			this.tbCancel.Click += new System.EventHandler(this.tbCancel_Click);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Comma Delimiter|*.csv";
			this.saveFileDialog.Title = "יצא לקובץ";
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Tab Delimeted Text File|*.txt";
			this.openFileDialog.ShowReadOnly = true;
			this.openFileDialog.Title = "יבא מקובץ";
			// 
			// ScoreTableDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(272, 376);
			this.ControlBox = false;
			this.Controls.Add(this.panel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ScoreTableDialog";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ניקוד תוצאות";
			this.panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudMaxScore)).EndInit();
			this.ResumeLayout(false);

		}

		private ResultType _resultType;

		private ScoreTable _scoreTable;
		public ScoreTable ScoreTable
		{
			get { return _scoreTable; }
		}
	
		public ScoreTableDialog(int sportFieldType, int sportField,  
			string sportFieldName, ScoreTable scoreTable, ResultType resultType)
		{
			InitializeComponent();

			if (sportFieldName.Length > 0)
				lbTitle.Text = "ניקוד תוצאות - " + sportFieldName;

			_scoreTable = scoreTable;
			_resultType = resultType;
			_sportFieldType = sportFieldType;
			_sportField = sportField;
			grid.Source = new ScoreTableGridSource(scoreTable, resultType);
			nudMaxScore.Value = Math.Max(scoreTable.Count, 100);

			grid.Columns.Add(0, "ניקוד", 40, System.Drawing.StringAlignment.Center);
			grid.Columns.Add(1, "תוצאה", 80);
		}

		private void tbOk_Click(object sender, System.EventArgs e)
		{
			ScoreTableGridSource stgd = (ScoreTableGridSource) grid.Source;

			_scoreTable = stgd.GetScoreTable();

			if (_scoreTable != null)
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void tbCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private object hotSpot;

		private void lbTitle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			System.Drawing.Point pt = lbTitle.PointToScreen(new System.Drawing.Point(e.X, e.Y));
			hotSpot = new System.Drawing.Point(pt.X - Left, pt.Y - Top);
		}

		private void lbTitle_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (hotSpot != null)
			{
				System.Drawing.Point pt = lbTitle.PointToScreen(new System.Drawing.Point(e.X, e.Y));
				System.Drawing.Point hs = (System.Drawing.Point) hotSpot;

				Location = new System.Drawing.Point(pt.X - hs.X, pt.Y - hs.Y);
			}
		}

		private void lbTitle_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			hotSpot = null;
		}

		protected override void OnLostFocus(EventArgs e)
		{
			hotSpot = null;
			base.OnLostFocus (e);
		}

		private void btnExport_Click(object sender, System.EventArgs e)
		{
			if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.Unicode);
				streamWriter.WriteLine(lbTitle.Text);
				streamWriter.WriteLine("\"ניקוד\"\t\"תוצאה\"");

				ScoreTableGridSource stgd = (ScoreTableGridSource) grid.Source;
				ScoreTable scoreTable = stgd.GetScoreTable();
				for (int n = grid.Source.GetRowCount(); n > 0; n--)
				{
					streamWriter.WriteLine("\"{0}\"\t\"{1}\"", new object[] {n, _resultType.FormatResult(scoreTable.GetResult(n))});
				}

				streamWriter.Close();
			}
		}

		private void btnImport_Click(object sender, System.EventArgs e)
		{
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				string strFilePath = openFileDialog.FileName;
				Hashtable tblResults = null;
				try
				{
					tblResults = ImportFileResults(strFilePath);
				}
				catch (Exception ex)
				{
					if (ex.Message.IndexOf("cannot access the file") >= 0)
					{
						Sport.UI.MessageBox.Error("לא ניתן לייבא תוצאות: אנא סגור את תוכנת אקסל ואחרי כן נסה שנית", "ייבוא תוצאות");
						return;
					}
					else
					{
						throw ex;
					}
				}
				
				if (tblResults == null || tblResults.Count == 0)
				{
					Sport.UI.MessageBox.Error("לא ניתן לייבא תוצאות, אנא וודא פורמט קובץ תקין", "ייבוא טבלת ניקוד");
				}
				else
				{
					int maxScore = 0;
					foreach (int score in tblResults.Values)
						if (score > maxScore)
							maxScore = score;
					int[] results = new int[maxScore];
					foreach (int result in tblResults.Keys)
					{
						int score = (int) tblResults[result];
						if (score <= 0)
							continue;
						if (results[score-1] > 0)
						{
							if (_resultType.Direction == Sport.Core.Data.ResultDirection.Most && results[score-1] > result)
								continue;
							if (_resultType.Direction == Sport.Core.Data.ResultDirection.Least && results[score-1] < result)
								continue;
						}
						results[score-1] = result;
					}
					for (int i = results.Length-2; i >= 0; i--)
						if (results[i] == 0)
							results[i] = results[i+1];
					ScoreTable scoreTable = new ScoreTable(results, _scoreTable.Direction);
					_scoreTable = scoreTable;
					grid.Source = new ScoreTableGridSource(_scoreTable, _resultType);
				}
			}
		}

		private void btnCopy_Click(object sender, System.EventArgs e)
		{
			//got result type?
			if (_resultType == null)
			{
				Sport.UI.MessageBox.Error("לא מוגדר חוק סוג תוצאה", "העתקת טבלת ניקוד");
				return;
			}
			
			//get current score table:
			ScoreTable objScoreTable = ((ScoreTableGridSource) grid.Source).GetScoreTable();
			
			//get all possible score tables:
			Sport.Rulesets.Rules.ScoreTable[] tables=
				_resultType.GetScoreTables();
			
			//initialize list of valid tables.
			System.Collections.ArrayList arrScoreTables=new System.Collections.ArrayList();
			if ((tables != null)&&(tables.Length > 0))
			{
				foreach (Sport.Rulesets.Rules.ScoreTable scoreTable in tables)
				{
					bool blnSameTables=true;
					for (int score = grid.Source.GetRowCount(); score > 0; score--)
					{
						int result1 = 0;
						int result2 = 0;
						try
						{
							result1 = scoreTable.GetResult(score);
							result2 = objScoreTable.GetResult(score);
						}
						catch
						{}
						if (result1 != result2)
						{
							blnSameTables = false;
							break;
						}
					}
					if (!blnSameTables)
						arrScoreTables.Add(scoreTable);
				}
			}
			//got anything?
			if (arrScoreTables.Count == 0)
			{
				Sport.UI.MessageBox.Warn("לא נמצאו טבלאות ניקוד מתאימות", "העתקת טבלת ניקוד");
				return;
			}
			
			//build items:
			Sport.Common.ListItem[] items=new Sport.Common.ListItem[arrScoreTables.Count];
			Sport.Types.CategoryTypeLookup catLookup=
				new Sport.Types.CategoryTypeLookup();
			for (int i=0; i<arrScoreTables.Count; i++)
			{
				Sport.Rulesets.Rules.ScoreTable scoreTable=
					(Sport.Rulesets.Rules.ScoreTable) arrScoreTables[i];
				int[] data=(int[]) scoreTable.Tag;
				Sport.Entities.Ruleset ruleset=new Sport.Entities.Ruleset(data[0]);
				string strTitle="";
				if (data[1] >= 0)
				{
					Sport.Entities.SportFieldType sportFieldType=
						new Sport.Entities.SportFieldType(data[1]);
					strTitle += sportFieldType.Name;
				}
				if (data[2] >= 0)
				{
					Sport.Entities.SportField sportField=
						new Sport.Entities.SportField(data[2]);
					strTitle += " - "+sportField.Name;
				}
				if (data[3] != Sport.Types.CategoryTypeLookup.All)
				{
					string strCategory=catLookup.Lookup(data[3]);
					strTitle += " - "+strCategory;
				}
				
				items[i] = new Sport.Common.ListItem(strTitle, scoreTable);
			}
			
			//let user select score table:
			Sport.UI.Dialogs.GenericEditDialog objDialog=
				new Sport.UI.Dialogs.GenericEditDialog("בחר טבלת ניקוד");
			objDialog.Items.Add(Sport.UI.Controls.GenericItemType.Selection, null, 
				items, new System.Drawing.Size(580, 25));
			if (objDialog.ShowDialog() == DialogResult.OK)
			{
				if (objDialog.Items[0].Value != null)
				{
					Sport.Common.ListItem item=
						(Sport.Common.ListItem) objDialog.Items[0].Value;
					Sport.Rulesets.Rules.ScoreTable scoreTable=
						(Sport.Rulesets.Rules.ScoreTable) item.Value;
					(grid.Source as ScoreTableGridSource).ScoreTable = scoreTable;
				}
			}

		}

		private void panel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			btnCopy.Enabled = (_sportField >= 0);
		}

		private void btnUpdateMaxScore_Click(object sender, System.EventArgs e)
		{
			int maxScore = (int) nudMaxScore.Value;
			_scoreTable.UpdateCapacity(maxScore);
			grid.Source = new ScoreTableGridSource(_scoreTable, _resultType);
		}

		private Hashtable ImportFileResults(string strFilePath)
		{
			System.IO.StreamReader reader = System.IO.File.OpenText(strFilePath);
			string strCurLine = reader.ReadLine();
			Hashtable tblResults = new Hashtable();
			int curResult = 0;
			int curScore = 0;
			while (strCurLine != null)
			{
				if (strCurLine.Length > 0)
				{
					string[] arrTemp = strCurLine.Split('\t');
					curResult = 0;
					curScore = 0;
					for (int i = 0; i < arrTemp.Length; i++)
					{
						curResult = GetResult(arrTemp[i]);
						if (curResult > 0)
						{
							if (i > 0)
								curScore = GetScore(arrTemp[i-1]);
							if (curScore == 0 && i < arrTemp.Length-1)
								curScore = GetScore(arrTemp[i+1]);
							break;
						}
					}
					if (curResult > 0 && curScore > 0)
						tblResults[curResult] = curScore;
				}
				strCurLine = reader.ReadLine();
			}
			reader.Close();
			return tblResults;
		}

		private int GetResult(string s)
		{
			if (s == null || s.Length == 0)
				return 0;
			
			string[] parts = null;
			if (s.IndexOf(".") > 0 || s.IndexOf(":") > 0)
				parts = s.Split('.', ':');
			
			if (parts == null || parts.Length == 0)
				return 0;
			
			int itemsLength = _resultType.ValueFormatter.FormatItems.Length;
			
			if (parts.Length < itemsLength)
			{
				ArrayList arrNewParts = new ArrayList(parts);
				for (int i = 0; i < (itemsLength - parts.Length); i++)
				{
					arrNewParts.Insert(0, "0");
				}
				parts = (string[]) arrNewParts.ToArray(typeof(string));
			}

			if (parts.Length > itemsLength)
			{
				ArrayList arrNewParts = new ArrayList();
				for (int i = parts.Length - 1; i >= (parts.Length - itemsLength); i--)
				{
					arrNewParts.Add(parts[i]);
				}
				arrNewParts.Reverse();
				parts = (string[]) arrNewParts.ToArray(typeof(string));
			}

			int[] values = new int[parts.Length];
			for (int i = 0; i < parts.Length; i++)
			{
				if (!Sport.Common.Tools.IsInteger(parts[i]))
					return 0;
				values[i] = Int32.Parse(parts[i]);
			}
			
			return _resultType.ValueFormatter.GetValue(values);
		}

		private int GetScore(string s)
		{
			if (!Sport.Common.Tools.IsInteger(s))
				return 0;
			return Int32.Parse(s);
		}
	}
}
