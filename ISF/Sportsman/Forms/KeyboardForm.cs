using System;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for KeyboardForm.
	/// </summary>
	public class KeyboardForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbAction;
		private System.Windows.Forms.TextBox tbShortcut;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnSet;
		private Sport.UI.Controls.Grid gridKeyboard;
		private System.Windows.Forms.Label labelCurrentSet;
		private System.Windows.Forms.Button btnClose;

		private void InitializeComponent()
		{
			this.btnClose = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tbAction = new System.Windows.Forms.TextBox();
			this.tbShortcut = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnSet = new System.Windows.Forms.Button();
			this.gridKeyboard = new Sport.UI.Controls.Grid();
			this.labelCurrentSet = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnClose.Location = new System.Drawing.Point(8, 328);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(72, 23);
			this.btnClose.TabIndex = 0;
			this.btnClose.TabStop = false;
			this.btnClose.Text = "סגור";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(318, 303);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "פעולה:";
			// 
			// tbAction
			// 
			this.tbAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbAction.Location = new System.Drawing.Point(200, 298);
			this.tbAction.Name = "tbAction";
			this.tbAction.ReadOnly = true;
			this.tbAction.Size = new System.Drawing.Size(112, 21);
			this.tbAction.TabIndex = 2;
			this.tbAction.TabStop = false;
			this.tbAction.Text = "";
			// 
			// tbShortcut
			// 
			this.tbShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.tbShortcut.Location = new System.Drawing.Point(8, 298);
			this.tbShortcut.Name = "tbShortcut";
			this.tbShortcut.Size = new System.Drawing.Size(140, 21);
			this.tbShortcut.TabIndex = 4;
			this.tbShortcut.TabStop = false;
			this.tbShortcut.Text = "";
			this.tbShortcut.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbShortcut.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbShortcut_KeyDown);
			this.tbShortcut.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbShortcut_KeyPress);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(152, 303);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "קיצור:";
			// 
			// btnSet
			// 
			this.btnSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSet.Location = new System.Drawing.Point(288, 328);
			this.btnSet.Name = "btnSet";
			this.btnSet.Size = new System.Drawing.Size(72, 23);
			this.btnSet.TabIndex = 5;
			this.btnSet.TabStop = false;
			this.btnSet.Text = "הקצה";
			this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
			// 
			// gridKeyboard
			// 
			this.gridKeyboard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.gridKeyboard.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridKeyboard.Editable = true;
			this.gridKeyboard.Location = new System.Drawing.Point(8, 8);
			this.gridKeyboard.Name = "gridKeyboard";
			this.gridKeyboard.SelectedIndex = -1;
			this.gridKeyboard.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridKeyboard.ShowCheckBoxes = false;
			this.gridKeyboard.ShowRowNumber = false;
			this.gridKeyboard.Size = new System.Drawing.Size(352, 280);
			this.gridKeyboard.TabIndex = 0;
			this.gridKeyboard.TabStop = false;
			// 
			// labelCurrentSet
			// 
			this.labelCurrentSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelCurrentSet.Location = new System.Drawing.Point(88, 336);
			this.labelCurrentSet.Name = "labelCurrentSet";
			this.labelCurrentSet.Size = new System.Drawing.Size(192, 16);
			this.labelCurrentSet.TabIndex = 6;
			// 
			// KeyboardForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(368, 358);
			this.Controls.Add(this.labelCurrentSet);
			this.Controls.Add(this.gridKeyboard);
			this.Controls.Add(this.btnSet);
			this.Controls.Add(this.tbShortcut);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbAction);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnClose);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.Name = "KeyboardForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "לוח מקשים";
			this.ResumeLayout(false);

		}
	
		public KeyboardForm()
		{
			InitializeComponent();

            gridKeyboard.Source = new KeyboardGridSource();

			gridKeyboard.Columns.Add(0, "פעולה", 1);
			gridKeyboard.Columns.Add(1, "קיצור", 1, System.Drawing.StringAlignment.Far);

			gridKeyboard.SelectionChanged += new EventHandler(gridKeyboard_SelectionChanged);

			SelectAction(-1);
		}

		private bool IsLegalKey(System.Windows.Forms.Keys keys)
		{
			bool ck = (keys & (System.Windows.Forms.Keys.Shift |
				System.Windows.Forms.Keys.Control |
				System.Windows.Forms.Keys.Alt)) != 0;
			keys &= System.Windows.Forms.Keys.KeyCode;
			if (ck)
			{
				if (((int) keys >= 32 && (int) keys <= 57) ||
					((int) keys >= 65 && (int) keys <= 111) ||
					((int) keys >= 186 && (int) keys <= 222) ||
					keys == System.Windows.Forms.Keys.Back ||
					keys == System.Windows.Forms.Keys.Tab ||
					keys == System.Windows.Forms.Keys.Enter ||
					keys == System.Windows.Forms.Keys.Escape)
					return true;
			}

			if ((int) keys >= 112 && (int) keys <= 135)
				return true;

			return false;
		}

		private void tbShortcut_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = true;

			if (IsLegalKey(e.KeyData))
				SetShortcut(e.KeyData);
		}

		private void gridKeyboard_SelectionChanged(object sender, EventArgs e)
		{
			int action = gridKeyboard.Selection.Size == 1 ? gridKeyboard.Selection.Rows[0] : -1;
			SelectAction(action);
		}

		private int currentAction = -1;

		private void SetShortcut(System.Windows.Forms.Keys keys)
		{
			tbShortcut.Tag = keys;
			tbShortcut.Text = KeyboardGridSource.KeysToString(keys);

			string command = Sport.UI.KeyListener.GetKeyCommand(keys);
			if (command == null)
			{
				labelCurrentSet.Text = "לא מוקצה";
			}
			else
			{
				int index = Sport.UI.KeyListener.GetCommandIndex(command);
				if (index == currentAction)
					labelCurrentSet.Text = null;
				else
					labelCurrentSet.Text = "מוקצה ל\"" + Sport.UI.KeyListener.GetCommandName(index) + "\"";
			}
		}

		private void SelectAction(int action)
		{
			currentAction = action;
			if (action == -1)
			{
				tbAction.Text = null;
				tbShortcut.Text = null;
				tbShortcut.Tag = null;
				tbShortcut.Enabled  = false;
				btnSet.Enabled = false;
			}
			else
			{
				tbAction.Text = Sport.UI.KeyListener.GetCommandName(action);
				SetShortcut(Sport.UI.KeyListener.GetCommendKey(action));
				tbShortcut.Enabled  = true;
				btnSet.Enabled = true;
			}
		}

		private void btnClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnSet_Click(object sender, System.EventArgs e)
		{
			if (currentAction != -1 && tbShortcut.Tag is System.Windows.Forms.Keys)
			{
				System.Windows.Forms.Keys keys = (System.Windows.Forms.Keys) tbShortcut.Tag;
				Sport.UI.KeyListener.SetCommandKey(currentAction, keys);
				SetShortcut(keys);
				Sport.Core.Configuration.WriteString("Keyboard", Sport.UI.KeyListener.GetCommandCommand(currentAction), ((int) keys).ToString());
				gridKeyboard.Refresh();
			}
		}

		private void tbShortcut_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = true;
		}
	}

	public class KeyboardGridSource : Sport.UI.Controls.IGridSource
	{
		public static string KeysToString(System.Windows.Forms.Keys keys)
		{
			string s = "";
			if ((keys & System.Windows.Forms.Keys.Alt) != 0)
				s += "Alt + ";
			if ((keys & System.Windows.Forms.Keys.Control) != 0)
				s += "Control + ";
			if ((keys & System.Windows.Forms.Keys.Shift) != 0)
				s += "Shift + ";
			keys = keys & System.Windows.Forms.Keys.KeyCode;
			s += keys.ToString();
			return s;
		}

		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
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

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			return null;
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
		}

		public int GetRowCount()
		{
			return Sport.UI.KeyListener.Count;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public string GetText(int row, int field)
		{
			switch (field)
			{
				case (0): // action
					return Sport.UI.KeyListener.GetCommandName(row);
				case (1): // shortcut
					return KeysToString(Sport.UI.KeyListener.GetCommendKey(row));
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
			// TODO:  Add KeyboardGridSource.Dispose implementation
		}

		#endregion
	}

}
