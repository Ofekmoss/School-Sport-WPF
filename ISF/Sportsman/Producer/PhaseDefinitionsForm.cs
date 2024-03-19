using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Rulesets;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for PhaseDefinitionsForm.
	/// </summary>
	public class PhaseDefinitionsForm : System.Windows.Forms.Form,
		Sport.UI.Controls.IGridSource
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private Sport.UI.Controls.Grid _grid;
		private Sport.Championships.Phase _phase;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private struct PhaseDefinition
		{
			public RuleType RuleType;
			public object RuleValue;
			public string Definition;
			public string DefaulValue;
			public string Value;
		}

		private PhaseDefinition[] _definitions;

		public PhaseDefinitionsForm(Sport.Championships.Phase phase)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_phase = phase;

			Ruleset ruleset = _phase.Championship.ChampionshipCategory.GetRuleset();

			ArrayList arrDefinitions = new ArrayList();
			RuleType[] ruleTypes = ruleset.GetRuleTypes();
			foreach (RuleType ruleType in ruleTypes)
			{
				
				string[] ruleTypeDefinitions = ruleType.GetDefinitions();
				if (ruleTypeDefinitions != null)
				{
					for (int n = 0; n < ruleTypeDefinitions.Length; n++)
					{
						PhaseDefinition definition = new PhaseDefinition();
						definition.RuleType = ruleType;
						definition.RuleValue = _phase.Championship.ChampionshipCategory.GetRule(ruleType.GetDataType());
						definition.Definition = ruleTypeDefinitions[n];
						definition.Value = _phase.Definitions.Get(ruleType.Id, definition.Definition);
						definition.DefaulValue = ruleType.GetDefinitionDefaultValue(definition.Definition, definition.RuleValue);
						arrDefinitions.Add(definition);
					}
				}
			}

			_definitions = new PhaseDefinition[arrDefinitions.Count];
			arrDefinitions.CopyTo(_definitions, 0);

			_grid.Source = this;
			_grid.Columns.Add(0, "הגדרה", 1);
			_grid.Columns.Add(1, "ערך", 1);
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
			this._grid = new Sport.UI.Controls.Grid();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(88, 198);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "ביטול";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(8, 198);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// _grid
			// 
			this._grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._grid.Editable = true;
			this._grid.ExpandOnDoubleClick = true;
			this._grid.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this._grid.HeaderHeight = 17;
			this._grid.HorizontalLines = true;
			this._grid.Location = new System.Drawing.Point(8, 8);
			this._grid.Name = "_grid";
			this._grid.SelectedIndex = -1;
			this._grid.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this._grid.SelectOnSpace = false;
			this._grid.ShowCheckBoxes = false;
			this._grid.ShowRowNumber = false;
			this._grid.Size = new System.Drawing.Size(296, 184);
			this._grid.TabIndex = 4;
			this._grid.VerticalLines = true;
			this._grid.VisibleRow = 0;
			// 
			// PhaseDefinitionsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(314, 230);
			this.Controls.Add(this._grid);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PhaseDefinitionsForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הגדרות שלב";
			this.ResumeLayout(false);

		}
		#endregion

		#region IGridSource

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
		}

		public int GetRowCount()
		{
			return _definitions.Length;
		}

		public int GetFieldCount(int row)
		{
			return 2;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		public string GetText(int row, int field)
		{
			if (field == 0) // Definition
			{
				return _definitions[row].Definition;
			}
			else if (field == 1) // Value
			{
				if (_definitions[row].Value == null)
					return _definitions[row].DefaulValue;
				return _definitions[row].Value;
			}
			return null;
		}

		private Sport.UI.Controls.Style _grayed = null;
		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			if (field == 1 && _definitions[row].Value == null)
			{
				if (_grayed == null)
				{
					_grayed = new Sport.UI.Controls.Style();
					_grayed.Foreground = System.Drawing.SystemBrushes.ControlDark;
				}
				return _grayed;
			}
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public void Sort(int group, int[] columns)
		{
		}

		private Sport.UI.Controls.NullComboBox ncbDefinition = null;
		public System.Windows.Forms.Control Edit(int row, int field)
		{
			if (field == 1) // value
			{
				if (ncbDefinition == null)
				{
					ncbDefinition = new Sport.UI.Controls.NullComboBox();
					ncbDefinition.SelectedIndexChanged += new EventHandler(ncbDefinition_SelectedIndexChanged);
				}

				PhaseDefinition definition = _definitions[row];
				ncbDefinition.Tag = row;
				ncbDefinition.Text = definition.DefaulValue;
				ncbDefinition.Items.Clear();
				ncbDefinition.Items.Add(Sport.UI.Controls.NullComboBox.Null);
				string[] values = definition.RuleType.GetDefinitionValues(definition.Definition, definition.RuleValue);
				if (values != null)
				{
					foreach (string value in values)
					{
						ncbDefinition.Items.Add(value);
					}
				}
				ncbDefinition.SelectedItem = definition.Value == null ? Sport.UI.Controls.NullComboBox.Null : definition.Value;

				return ncbDefinition;
			}

			return null;
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
		}

		#endregion

		private void ncbDefinition_SelectedIndexChanged(object sender, EventArgs e)
		{
			int row = (int) ncbDefinition.Tag;
			object selectedItem = ncbDefinition.SelectedItem;
			if (selectedItem == Sport.UI.Controls.NullComboBox.Null)
			{
				_definitions[row].Value = null;
			}
			else
			{
				_definitions[row].Value = selectedItem as string;
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			foreach (PhaseDefinition definition in _definitions)
			{
				_phase.Definitions[new Sport.Championships.Phase.DefinitionKey(definition.RuleType.Id, definition.Definition)] =
					definition.Value;
			}
		}
	}
}
