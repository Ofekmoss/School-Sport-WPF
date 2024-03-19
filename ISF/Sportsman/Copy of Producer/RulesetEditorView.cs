using System;
using System.Windows.Forms;
using System.Diagnostics;
using Sport.UI.Dialogs;
using Sport.UI.Controls;
using Sport.Rulesets;
using Sport.Producer.UI;
using Sport.Types;
using Sport.Data;

namespace Sportsman.Producer
{
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports)]
	public class RulesetEditorView : Sport.UI.View
	{
		private Sport.UI.Controls.ButtonBox bbCategory;
		private System.Windows.Forms.Label labelCategory;
		private System.Windows.Forms.Panel panelGrid;
		private System.Windows.Forms.Panel panelField;
		private System.Windows.Forms.Label labelSportFieldType;
		private Sport.UI.Controls.NullComboBox cbSportFieldType;
		private System.Windows.Forms.Label labelSportField;
		private Sport.UI.Controls.NullComboBox cbSportField;
		private Sport.UI.Controls.Grid grid;
		private Sport.UI.Controls.ThemeButton tbAdd;
		private Sport.UI.Controls.ThemeButton tbRemove;
		private Sport.UI.Controls.ThemeButton tbSave;
		private Sport.UI.Controls.ThemeButton tbCancel;
		private System.Windows.Forms.ImageList buttonImages;
		private System.ComponentModel.IContainer components;

		private Ruleset _ruleset = null;
		private GenericEditDialog _dlgNewRule = null;
		private System.Windows.Forms.Label labelRuleType;
		private Sport.UI.Controls.NullComboBox cbRuleType;
		//private Sport.Entities.PermissionServices.PermissionType _permType;

		public RulesetEditorView()
		{
			InitializeComponent();

			this.bbCategory.ValueSelector = new ButtonBox.SelectValue(Forms.CategorySelectionDialog.ValueSelector);
			this.bbCategory.ValueChanged += new EventHandler(CategoryChanged);
			this.cbSportFieldType.SelectedIndexChanged += new EventHandler(SportFieldTypeChanged);
			this.cbSportField.SelectedIndexChanged += new EventHandler(SportFieldChanged);
			this.cbRuleType.SelectedIndexChanged += new EventHandler(RuleTypeChanged);
		}

		private void SetRuleset(Ruleset ruleset)
		{
			if (_ruleset != null)
			{
				_ruleset.Changed -= new EventHandler(RulesetChanged);
			}
			_ruleset = ruleset;
			if (_ruleset != null)
			{
				_ruleset.Changed += new EventHandler(RulesetChanged);
			}
			((RulesetGridSource) grid.Source).Ruleset = _ruleset;
	}

		#region Initialize Component
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(RulesetEditorView));
			this.bbCategory = new Sport.UI.Controls.ButtonBox();
			this.labelCategory = new System.Windows.Forms.Label();
			this.panelGrid = new System.Windows.Forms.Panel();
			this.grid = new Sport.UI.Controls.Grid();
			this.panelField = new System.Windows.Forms.Panel();
			this.labelSportField = new System.Windows.Forms.Label();
			this.cbSportField = new Sport.UI.Controls.NullComboBox();
			this.labelSportFieldType = new System.Windows.Forms.Label();
			this.cbSportFieldType = new Sport.UI.Controls.NullComboBox();
			this.tbAdd = new Sport.UI.Controls.ThemeButton();
			this.buttonImages = new System.Windows.Forms.ImageList(this.components);
			this.tbRemove = new Sport.UI.Controls.ThemeButton();
			this.tbSave = new Sport.UI.Controls.ThemeButton();
			this.tbCancel = new Sport.UI.Controls.ThemeButton();
			this.labelRuleType = new System.Windows.Forms.Label();
			this.cbRuleType = new Sport.UI.Controls.NullComboBox();
			this.panelGrid.SuspendLayout();
			this.panelField.SuspendLayout();
			this.SuspendLayout();
			// 
			// bbCategory
			// 
			this.bbCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bbCategory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.bbCategory.Location = new System.Drawing.Point(224, 6);
			this.bbCategory.Name = "bbCategory";
			this.bbCategory.Size = new System.Drawing.Size(200, 21);
			this.bbCategory.TabIndex = 0;
			this.bbCategory.Text = "הכל";
			this.bbCategory.Value = null;
			this.bbCategory.ValueSelector = null;
			// 
			// labelCategory
			// 
			this.labelCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCategory.Location = new System.Drawing.Point(432, 11);
			this.labelCategory.Name = "labelCategory";
			this.labelCategory.Size = new System.Drawing.Size(48, 16);
			this.labelCategory.TabIndex = 2;
			this.labelCategory.Text = "קטגוריה:";
			// 
			// panelGrid
			// 
			this.panelGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.panelGrid.Controls.Add(this.grid);
			this.panelGrid.Controls.Add(this.panelField);
			this.panelGrid.Location = new System.Drawing.Point(8, 32);
			this.panelGrid.Name = "panelGrid";
			this.panelGrid.Size = new System.Drawing.Size(698, 272);
			this.panelGrid.TabIndex = 7;
			// 
			// grid
			// 
			this.grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.Editable = true;
			this.grid.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.grid.HorizontalLines = true;
			this.grid.Location = new System.Drawing.Point(0, 40);
			this.grid.Name = "grid";
			this.grid.SelectedIndex = -1;
			this.grid.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.grid.ShowCheckBoxes = false;
			this.grid.ShowRowNumber = false;
			this.grid.Size = new System.Drawing.Size(698, 232);
			this.grid.TabIndex = 2;
			this.grid.VerticalLines = true;
			this.grid.SelectionChanged += new System.EventHandler(this.GridSelectionChanged);
			// 
			// panelField
			// 
			this.panelField.Controls.Add(this.labelSportField);
			this.panelField.Controls.Add(this.cbSportField);
			this.panelField.Controls.Add(this.labelSportFieldType);
			this.panelField.Controls.Add(this.cbSportFieldType);
			this.panelField.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelField.Location = new System.Drawing.Point(0, 0);
			this.panelField.Name = "panelField";
			this.panelField.Size = new System.Drawing.Size(698, 40);
			this.panelField.TabIndex = 1;
			// 
			// labelSportField
			// 
			this.labelSportField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSportField.Location = new System.Drawing.Point(400, 13);
			this.labelSportField.Name = "labelSportField";
			this.labelSportField.Size = new System.Drawing.Size(48, 16);
			this.labelSportField.TabIndex = 8;
			this.labelSportField.Text = "מקצוע:";
			// 
			// cbSportField
			// 
			this.cbSportField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSportField.Location = new System.Drawing.Point(221, 8);
			this.cbSportField.Name = "cbSportField";
			this.cbSportField.Size = new System.Drawing.Size(176, 21);
			this.cbSportField.Sorted = true;
			this.cbSportField.Text = "הכל";
			this.cbSportField.TabIndex = 7;
			// 
			// labelSportFieldType
			// 
			this.labelSportFieldType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSportFieldType.Location = new System.Drawing.Point(632, 13);
			this.labelSportFieldType.Name = "labelSportFieldType";
			this.labelSportFieldType.Size = new System.Drawing.Size(64, 16);
			this.labelSportFieldType.TabIndex = 6;
			this.labelSportFieldType.Text = "סוג מקצוע:";
			// 
			// cbSportFieldType
			// 
			this.cbSportFieldType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSportFieldType.Location = new System.Drawing.Point(462, 8);
			this.cbSportFieldType.Name = "cbSportFieldType";
			this.cbSportFieldType.Size = new System.Drawing.Size(168, 21);
			this.cbSportFieldType.Sorted = true;
			this.cbSportFieldType.Text = "הכל";
			this.cbSportFieldType.TabIndex = 5;
			// 
			// tbAdd
			// 
			this.tbAdd.Alignment = System.Drawing.StringAlignment.Center;
			this.tbAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbAdd.AutoSize = true;
			this.tbAdd.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbAdd.Hue = 220F;
			this.tbAdd.Image = null;
			this.tbAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbAdd.ImageIndex = 0;
			this.tbAdd.ImageList = this.buttonImages;
			this.tbAdd.ImageSize = new System.Drawing.Size(12, 12);
			this.tbAdd.Location = new System.Drawing.Point(8, 312);
			this.tbAdd.Name = "tbAdd";
			this.tbAdd.Saturation = 0.9F;
			this.tbAdd.Size = new System.Drawing.Size(53, 17);
			this.tbAdd.TabIndex = 8;
			this.tbAdd.Text = "הוסף";
			this.tbAdd.Transparent = System.Drawing.Color.Black;
			this.tbAdd.Click += new System.EventHandler(this.AddClicked);
			// 
			// buttonImages
			// 
			this.buttonImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.buttonImages.ImageSize = new System.Drawing.Size(12, 12);
			this.buttonImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("buttonImages.ImageStream")));
			this.buttonImages.TransparentColor = System.Drawing.Color.Black;
			// 
			// tbRemove
			// 
			this.tbRemove.Alignment = System.Drawing.StringAlignment.Center;
			this.tbRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbRemove.AutoSize = true;
			this.tbRemove.Enabled = false;
			this.tbRemove.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbRemove.Hue = 0F;
			this.tbRemove.Image = null;
			this.tbRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbRemove.ImageIndex = 1;
			this.tbRemove.ImageList = this.buttonImages;
			this.tbRemove.ImageSize = new System.Drawing.Size(12, 12);
			this.tbRemove.Location = new System.Drawing.Point(72, 312);
			this.tbRemove.Name = "tbRemove";
			this.tbRemove.Saturation = 0.7F;
			this.tbRemove.Size = new System.Drawing.Size(49, 17);
			this.tbRemove.TabIndex = 9;
			this.tbRemove.Text = "מחק";
			this.tbRemove.Transparent = System.Drawing.Color.Black;
			this.tbRemove.Click += new System.EventHandler(this.RemoveClicked);
			// 
			// tbSave
			// 
			this.tbSave.Alignment = System.Drawing.StringAlignment.Center;
			this.tbSave.AutoSize = true;
			this.tbSave.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbSave.Hue = 120F;
			this.tbSave.Image = null;
			this.tbSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbSave.ImageIndex = 2;
			this.tbSave.ImageList = this.buttonImages;
			this.tbSave.ImageSize = new System.Drawing.Size(12, 12);
			this.tbSave.Location = new System.Drawing.Point(8, 8);
			this.tbSave.Name = "tbSave";
			this.tbSave.Saturation = 0.5F;
			this.tbSave.Size = new System.Drawing.Size(53, 17);
			this.tbSave.TabIndex = 10;
			this.tbSave.Text = "שמור";
			this.tbSave.Transparent = System.Drawing.Color.Black;
			this.tbSave.Click += new System.EventHandler(this.SaveClicked);
			// 
			// tbCancel
			// 
			this.tbCancel.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCancel.AutoSize = true;
			this.tbCancel.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCancel.Hue = 0F;
			this.tbCancel.Image = null;
			this.tbCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tbCancel.ImageIndex = 3;
			this.tbCancel.ImageList = this.buttonImages;
			this.tbCancel.ImageSize = new System.Drawing.Size(12, 12);
			this.tbCancel.Location = new System.Drawing.Point(72, 8);
			this.tbCancel.Name = "tbCancel";
			this.tbCancel.Saturation = 0.7F;
			this.tbCancel.Size = new System.Drawing.Size(48, 17);
			this.tbCancel.TabIndex = 11;
			this.tbCancel.Text = "בטל";
			this.tbCancel.Transparent = System.Drawing.Color.Black;
			this.tbCancel.Click += new System.EventHandler(this.CancelClicked);
			// 
			// labelRuleType
			// 
			this.labelRuleType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelRuleType.Location = new System.Drawing.Point(672, 11);
			this.labelRuleType.Name = "labelRuleType";
			this.labelRuleType.Size = new System.Drawing.Size(32, 16);
			this.labelRuleType.TabIndex = 12;
			this.labelRuleType.Text = "חוק:";
			// 
			// cbRuleType
			// 
			this.cbRuleType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbRuleType.Location = new System.Drawing.Point(488, 6);
			this.cbRuleType.Name = "cbRuleType";
			this.cbRuleType.Size = new System.Drawing.Size(176, 21);
			this.cbRuleType.Sorted = true;
			this.cbRuleType.Text = "הכל";
			this.cbRuleType.TabIndex = 13;
			// 
			// RulesetEditorView
			// 
			this.Controls.Add(this.cbRuleType);
			this.Controls.Add(this.labelRuleType);
			this.Controls.Add(this.panelGrid);
			this.Controls.Add(this.labelCategory);
			this.Controls.Add(this.bbCategory);
			this.Controls.Add(this.tbAdd);
			this.Controls.Add(this.tbRemove);
			this.Controls.Add(this.tbSave);
			this.Controls.Add(this.tbCancel);
			this.Name = "RulesetEditorView";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Size = new System.Drawing.Size(714, 330);
			this.panelGrid.ResumeLayout(false);
			this.panelField.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public override void Open()
		{
			/*
			if (Sport.Entities.User.ViewPermission(this.GetType().Name) == 
				Sport.Entities.PermissionServices.PermissionType.None)
			{
				throw new Exception("Can't edit rulesets: Permission Denied");
			}
			*/
			
			string rsid = State["ruleset"];

			if (rsid == null)
				throw new Exception("No ruleset given");

			// Creating a clone of the loaded rule set
			_ruleset = new Ruleset(Ruleset.LoadRuleset(Int32.Parse(rsid)));

			int sportID=_ruleset.Sport;
			Sport.Entities.Sport sport=new Sport.Entities.Sport(sportID);
			_ruleset.PointsName = sport.PointsName;

			if (_ruleset.SportType == SportType.Match)
			{
				panelField.Visible = false;
			}

			grid.Columns.Add(0, "חוק", 120);
			grid.Columns.Add(1, "תחום", 100);
			grid.Columns.Add(2, "ערך", 200);

			grid.Groups[0].RowHeight = 60;

			//_permType = Sport.Entities.User.ViewPermission(this.GetType().Name);

			grid.Source = new RulesetGridSource(_ruleset, 
				new RuleScope(RuleScope.Any, RuleScope.Any, RuleScope.Any),
				true); //(_permType == Sport.Entities.PermissionServices.PermissionType.Full)

			_ruleset.Changed += new EventHandler(RulesetChanged);

			tbSave.Enabled = false;
			tbCancel.Enabled = false;

			cbSportField.Items.Clear();
			cbSportFieldType.Items.Clear();
			EntityFilter filter=new EntityFilter(
				(int) Sport.Entities.SportFieldType.Fields.Sport, _ruleset.Sport);
			cbSportFieldType.Items.AddRange(Sport.Entities.SportFieldType.Type.GetEntities(filter));
			cbSportFieldType.Items.Add(Sport.UI.Controls.NullComboBox.Null);
			cbSportFieldType.SelectedItem = Sport.UI.Controls.NullComboBox.Null;

			cbRuleType.Items.Clear();
			cbRuleType.Items.AddRange(RuleType.GetRuleTypes(_ruleset.SportType));
			cbRuleType.Items.Add(Sport.UI.Controls.NullComboBox.Null);
			cbRuleType.SelectedItem = Sport.UI.Controls.NullComboBox.Null;

			Title = "תקנונים - " + _ruleset.Name;

			/*
			if (_permType == 
				Sport.Entities.PermissionServices.PermissionType.ReadOnly)
			{
				this.tbAdd.Visible = false;
				this.tbRemove.Visible = false;
				this.tbSave.Visible = false;
			}
			*/

			base.Open ();
		}

		private void AddClicked(object sender, System.EventArgs e)
		{
			/*
			if (_permType !=
				Sport.Entities.PermissionServices.PermissionType.Full)
			{
				return;
			}
			*/
			
			Sport.Rulesets.RuleType[] ruleTypes=RuleType.GetRuleTypes(_ruleset.SportType);
			
			//maybe no more rules available...
			if (ruleTypes.Length == 0)
			{
				Sport.UI.MessageBox.Show("כל החוקים כבר נוספו, ניתן לערוך חוקים קיימים", "הוספת חוק חדש", MessageBoxIcon.Information);
				return;
			}
			
			if (_dlgNewRule == null)
			{
				_dlgNewRule = new GenericEditDialog("חוק חדש");
				_dlgNewRule.Items.Add(new GenericItem("חוק:", GenericItemType.Selection,
					null, ruleTypes));
				_dlgNewRule.Items.Add(new GenericItem("קטגוריה:", GenericItemType.Button,
					null, 
					new object[] {
									 new Sport.UI.Controls.ButtonBox.SelectValue(Forms.CategorySelectionDialog.ValueSelector)
								 }));
				_dlgNewRule.Items[0].ValueChanged += new EventHandler(DlgRuleChanged);

				//add the sport fields and sport field types if competiton:
				if (_ruleset.SportType == SportType.Competition)
				{
					EntityFilter filter=new EntityFilter(
						(int) Sport.Entities.SportFieldType.Fields.Sport, _ruleset.Sport);
					Entity[] sportFieldTypes=
						Sport.Entities.SportFieldType.Type.GetEntities(filter);
					_dlgNewRule.Items.Add(new GenericItem("סוג מקצוע:", GenericItemType.Selection, 
						null, sportFieldTypes));
					_dlgNewRule.Items.Add(new GenericItem("מקצוע:", GenericItemType.Selection));
				
					_dlgNewRule.Items[2].ValueChanged += new EventHandler(DlgSportFieldTypeChanged);
				}
			}

			_dlgNewRule.Confirmable = false;

			_dlgNewRule.Items[0].Value = cbRuleType.SelectedItem as RuleType;
			if (_dlgNewRule.Items[0].Value != null)
				DlgRuleChanged(_dlgNewRule.Items[0], EventArgs.Empty); // To update confirmable
			_dlgNewRule.Items[1].Value = bbCategory.Value;

			if (_ruleset.SportType == SportType.Competition)
			{
				_dlgNewRule.Items[2].Value = cbSportFieldType.SelectedItem as Entity;
				if (_dlgNewRule.Items[2].Value != null)
					DlgSportFieldTypeChanged(_dlgNewRule.Items[2], EventArgs.Empty); // to update sport fields
				_dlgNewRule.Items[3].Value = cbSportField.SelectedItem as Entity;
			}

			
			if (_dlgNewRule.ShowDialog() == DialogResult.OK)
			{
				RuleType ruleType = _dlgNewRule.Items[0].Value as RuleType;
				if (ruleType != null)
				{
					Rule rule = _ruleset.Add(ruleType);
					LookupItem category = _dlgNewRule.Items[1].Value as LookupItem;
					object fieldType = null;
					object field = null;
					if (_ruleset.SportType == SportType.Competition)
					{
						fieldType = _dlgNewRule.Items[2].Value;
						field = _dlgNewRule.Items[3].Value;
					}
					if (category == null &&
						fieldType == null &&
						field == null)
					{
						rule.Set(Rule.Empty);
					}
					else
					{
						int categoryID=-1;
						if ((category != null)&&(category.Id != CategoryTypeLookup.All))
							categoryID = category.Id;
						int fieldTypeID=-1;
						if (fieldType != null)
							fieldTypeID = (fieldType as Entity).Id;
						int fieldID=-1;
						if (field != null)
							fieldID = (field as Entity).Id;
						rule.Set(new RuleScope(categoryID, fieldTypeID, fieldID), Rule.Empty);
					}
					grid.RefreshSource();
				}
			}
		}

		private void DlgSportFieldTypeChanged(object sender, EventArgs e)
		{
			int index=3;
			Sport.Entities.SportFieldType sportFieldType=null;
			
			_dlgNewRule.Items[index].Value = null;
			if (_dlgNewRule.Items[2].Value == null)
			{
				_dlgNewRule.Items[index].Values = null;
			}
			else
			{
				sportFieldType = new Sport.Entities.SportFieldType(
					_dlgNewRule.Items[2].Value as Entity);
				EntityFilter filter=new EntityFilter(
					(int) Sport.Entities.SportField.Fields.SportFieldType, sportFieldType.Id);
				Sport.Data.Entity[] sportFields = Sport.Entities.SportField.Type.GetEntities(filter);
				_dlgNewRule.Items[index].Values = sportFields;
			}
		}

		private void DlgRuleChanged(object sender, EventArgs e)
		{
			_dlgNewRule.Confirmable = (_dlgNewRule.Items[0].Value != null);
		}

		private void RemoveClicked(object sender, System.EventArgs e)
		{
			if (grid.Selection.Size == 1)
			{
				int row = grid.Selection.Rows[0];
				((RulesetGridSource) grid.Source).Remove(row);
			}
		}

		private void RulesetChanged(object sender, EventArgs e)
		{
			tbSave.Enabled = true;
			tbCancel.Enabled = true;
		}

		private void GridSelectionChanged(object sender, EventArgs e)
		{
			if (grid.Selection.Size == 1)
			{
				int row = grid.Selection.Rows[0];
				tbRemove.Enabled = ((RulesetGridSource) grid.Source).CanRemove(row);
			}
			else
			{
				tbRemove.Enabled = false;
			}
		}

		private void SaveClicked(object sender, EventArgs e)
		{
			grid.CancelEdit();
			if (_ruleset.Save())
			{
				tbSave.Enabled = false;
				tbCancel.Enabled = false;
			}
		}

		private void CancelClicked(object sender, EventArgs e)
		{
			// Resetting to original ruleset
			SetRuleset(new Ruleset(Ruleset.LoadRuleset(_ruleset.Id)));
			tbSave.Enabled = false;
			tbCancel.Enabled = false;
			ResetGridSource();
		}

		private void CategoryChanged(object sender, EventArgs e)
		{
			ResetGridSource();
		}

		private void SportFieldTypeChanged(object sender, EventArgs e)
		{
			cbSportField.Items.Clear();
			Sport.Data.Entity entity = cbSportFieldType.SelectedItem as Sport.Data.Entity;

			if (entity != null)
			{
				cbSportField.Items.AddRange(
					Sport.Entities.SportField.Type.GetEntities(
					new EntityFilter((int) Sport.Entities.SportField.Fields.SportFieldType,
					entity.Id)));
				cbSportField.Items.Add(Sport.UI.Controls.NullComboBox.Null);
			}

			ResetGridSource();
		}

		private void SportFieldChanged(object sender, EventArgs e)
		{
			ResetGridSource();
		}

		private void RuleTypeChanged(object sender, EventArgs e)
		{
			ResetGridSource();
		}


		private void ResetGridSource()
		{
			int category = RuleScope.Any;
			int sportfieldtype = RuleScope.Any;
			int sportfield = RuleScope.Any;
			if (bbCategory.Value != null)
			{
				category = ((LookupItem) bbCategory.Value).Id;
			}
			if (cbSportFieldType.SelectedItem != null)
			{
				sportfieldtype = ((Sport.Data.Entity) cbSportFieldType.SelectedItem).Id;
			}
			if (cbSportField.SelectedItem != null)
			{
				sportfield = ((Sport.Data.Entity) cbSportField.SelectedItem).Id;
			}

			((RulesetGridSource) grid.Source).Scope = new RuleScope(category, sportfieldtype, sportfield);
			((RulesetGridSource) grid.Source).RuleType = cbRuleType.SelectedItem as RuleType;
			grid.RefreshSource();
		}
	}

	public class RulesetGridSource : Sport.UI.Controls.IGridSource
	{
		private Sport.UI.Controls.Grid _grid;
		public Sport.UI.Controls.Grid Grid
		{
			get { return _grid; }
		}
		private Ruleset _ruleset;
		public Ruleset Ruleset
		{
			get { return _ruleset; }
			set 
			{ 
				if (_ruleset != null)
				{
					_ruleset.Changed -= new EventHandler(RulesetChanged);
				}

				_ruleset = value; 

				if (_ruleset != null)
				{
					_ruleset.Changed += new EventHandler(RulesetChanged);
				}

				rows = null; 
			}
		}
		private RuleScope _scope;
		public RuleScope Scope
		{
			get { return _scope; }
			set { _scope = value; rows = null; }
		}
		private RuleType _ruleType;
		public RuleType RuleType
		{
			get { return _ruleType; }
			set { _ruleType = value; rows = null; }
		}

		private Style _grayed;

		private bool _editable;

		public RulesetGridSource(Ruleset ruleset, RuleScope scope, bool editable)
		{
			_grid = null;
			_sort = new int[] { 0, 1 }; // Sort by rule and scope
			_ruleset = ruleset;
			_scope = scope;
			_ruleType = null;
			_grayed = new Style();
			_grayed.Foreground = System.Drawing.SystemBrushes.ControlDark;
			_editable = editable;
			_ruleset.Changed += new EventHandler(RulesetChanged);
		}

		public class Row
		{
			public bool SportRuleset;
			public Rule Rule;
			public RuleType RuleType;
			public RuleScope Scope;
			public object Value;
			public Row(bool sportRuleset, Rule rule, RuleType ruleType, RuleScope scope, object value)
			{
				SportRuleset = sportRuleset;
				Rule = rule;
				Scope = scope;
				RuleType = ruleType;
				Value = value;
			}
		}

		private void BuildRows()
		{
			System.Collections.ArrayList al = new System.Collections.ArrayList();

			// Reading ruleset's rules
			Ruleset.RuleEnumerator e = _ruleset.GetRuleEnumerator(_scope);

			while (e.MoveNext())
			{
				if (_ruleType == null || _ruleType.Equals(e.RuleType))
				{
					al.Add(new Row(false, e.Rule, e.RuleType, e.RuleScope, e.Value));
				}
			}

			int ruleCount = al.Count;

			if (_ruleset.SportRuleset != null && _ruleset.SportRuleset != _ruleset)
			{
				// Reading ruleset's sport's ruleset's rules
				e = _ruleset.SportRuleset.GetRuleEnumerator(_scope);

				while (e.MoveNext())
				{
					if (_ruleType == null || _ruleType.Equals(e.RuleType))
					{
						Row row = new Row(true, e.Rule, e.RuleType, e.RuleScope, e.Value);

						bool exist = false;
						for (int n = 0; n < ruleCount && !exist; n++)
						{
							Row r = (Row) al[n];
							if (r.RuleType == row.RuleType &&
								r.Scope == row.Scope)
								exist = true;
						}

						if (!exist)
							al.Add(row);
					}
				}
			}

			rows = (Row[]) al.ToArray(typeof(Row));

			Sort(0, _sort);
		}

		public bool CanRemove(int row)
		{
			Row r = rows[row];
			return !r.SportRuleset;
		}

		public void Remove(int row)
		{
			if (row >= 0 && row < rows.Length)
			{
				Row r = rows[row];
				_ruleset[r.RuleType].Set(r.Scope, null);
			}
		}

		private Row[] rows = null;
		public Row GetRow(int index)
		{
			return rows[index];
		}

		private void RulesetChanged(object sender, EventArgs e)
		{
			rows = null;
			if (_grid != null)
			{
				_grid.RefreshSelection();
				_grid.Refresh();
			}
		}

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
			_grid = grid;
		}

		public int GetRowCount()
		{
			if (rows == null)
				BuildRows();

			return rows.Length;
		}

		public int GetFieldCount(int row)
		{
			return 3;
		}

		public int GetGroup(int row)
		{
			return 0;
		}

		private static string GetScopeText(RuleScope scope)
		{
			string s = "";
			
			if (scope.Category != -1)
				s += CategoryTypeLookup.ToString(scope.Category);
			
			if (scope.SportFieldType >= 0)
			{
				Sport.Entities.SportFieldType sportFieldType=
					new Sport.Entities.SportFieldType(scope.SportFieldType);
				if (s.Length > 0)
					s += " / ";
				s += sportFieldType.Name;
			}
			
			if (scope.SportField >= 0)
			{
				Sport.Entities.SportField sportField=
					new Sport.Entities.SportField(scope.SportField);
				if (s.Length > 0)
					s += " / ";
				s += sportField.Name;
			}
			
			return s;
		}

		private class ScopeText
		{
			private RuleScope _scope;
			public RuleScope Scope
			{
				get { return _scope; }
			}

			public ScopeText(RuleScope scope)
			{
				_scope = scope;
			}

			public override string ToString()
			{
				return GetScopeText(_scope);
			}
		}

		public string GetText(int row, int col)
		{
			Row r = rows[row];

			switch (col)
			{
				case (0):
					return r.RuleType.ToString();
				case (1):
					return GetScopeText(r.Scope);
				case (2):
					return r.Value.ToString();
			}

			return null;
		}

		public Style GetStyle(int row, int field, GridDrawState state)
		{
			if (rows[row].SportRuleset)
				return _grayed;
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public static object ScopeSelector(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			//initialize dialog:
			GenericEditDialog dlgScope = new GenericEditDialog("שינוי תחום");

			Row editRow = buttonBox.Tag as Row;

			Ruleset ruleset = editRow.Rule.Ruleset;

			//add rules combo:
			dlgScope.Items.Add(new GenericItem("חוק:", GenericItemType.Text,
				editRow.RuleType.ToString()));
			dlgScope.Items[0].ReadOnly = true;
			
			dlgScope.Items.Add(new GenericItem("קטגוריה:", GenericItemType.Button,
				CategoryTypeLookup.ToLookupItem(editRow.Scope.Category),
				new object[] { 
								   new Sport.UI.Controls.ButtonBox.SelectValue(Forms.CategorySelectionDialog.ValueSelector)
							}));

			//add the sport fields and sport field types if competiton:
			if (ruleset.SportType == SportType.Competition)
			{
				Sport.Rulesets.RuleScope scope=(value as ScopeText).Scope;
				Sport.Entities.SportFieldType selectedSportFieldType=null;
				Sport.Entities.SportField selectedSportField=null;
				if (!scope.Equals(Sport.Rulesets.RuleScope.Empty))
				{
					if (scope.SportFieldType >= 0)
					{
						selectedSportFieldType = 
							new Sport.Entities.SportFieldType(scope.SportFieldType);
					}
					if (scope.SportField >= 0)
					{
						selectedSportField = 
							new Sport.Entities.SportField(scope.SportField);
					}
				}
				
				EntityFilter filter=new EntityFilter(
					(int) Sport.Entities.SportFieldType.Fields.Sport, ruleset.Sport);
				Entity[] t =
					Sport.Entities.SportFieldType.Type.GetEntities(filter);
				object[] sportFieldTypes = new object[t.Length + 1];
				Array.Copy(t, 0, sportFieldTypes, 1, t.Length);
				sportFieldTypes[0] = "";
				dlgScope.Items.Add(new GenericItem("סוג מקצוע:", GenericItemType.Selection, 
					(selectedSportFieldType == null)?null:selectedSportFieldType.Entity, 
					sportFieldTypes));
				dlgScope.Items.Add(new GenericItem("מקצוע:", GenericItemType.Selection));
				if (selectedSportFieldType != null)
				{
					SportFieldTypeChanged(dlgScope.Items[2], EventArgs.Empty);
					if (selectedSportField != null)
						dlgScope.Items[3].Value = selectedSportField.Entity;
				}
				dlgScope.Items[2].ValueChanged += new EventHandler(SportFieldTypeChanged);
			}

			
			//show the dialog
			if (dlgScope.ShowDialog() == DialogResult.OK)
			{
				LookupItem category = 
					dlgScope.Items[1].Value as LookupItem;
				Sport.Data.Entity entSportFieldType = null;
				Sport.Data.Entity entSportField = null;

				if (ruleset.SportType == SportType.Competition)
				{
					entSportFieldType = dlgScope.Items[2].Value as Sport.Data.Entity;
					entSportField = dlgScope.Items[3].Value as Sport.Data.Entity;
				}

				RuleScope scope = new RuleScope(
					category == null ? -1 : category.Id,
					entSportFieldType == null ? -1 : entSportFieldType.Id,
					entSportField == null ? -1 : entSportField.Id);

				if (scope != editRow.Scope)
				{
					if (editRow.Rule.Get(scope) != null)
					{
						if (!Sport.UI.MessageBox.Ask("קיים ערך עבור תחום זה, האם לדרוס ערך?", false))
							return value;
					}

					return new ScopeText(scope);
				}
			}
			
			return value;
		}

		private void ScopeChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			ScopeText st = bb.Value as ScopeText;
			Row r = bb.Tag as Row;

			object o = r.Rule.Get(r.Scope);
			r.Rule.Set(r.Scope, null);
			r.Rule.Set(st.Scope, o);
			r.Scope = st.Scope;
		}

		private RuleTypeEditor editor;
		public Control Edit(int row, int col)
		{
			if (!_editable)
				return null;

			Row r = rows[row];

			if (r.SportRuleset)
				return null;

			if (col == 1) // Scope
			{
				Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
				bb.Value = new ScopeText(r.Scope);
				bb.Tag = r;
				bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(ScopeSelector);
				bb.ValueChanged += new EventHandler(ScopeChanged);

				return bb;
			}
			else if (col == 2) // Value
			{
				editor = null;
				Type type = r.RuleType.GetType();

				object[] attributes = type.GetCustomAttributes(typeof(Sport.Rulesets.RuleEditorAttribute), false);
				if (attributes != null && attributes.Length > 0)
				{
					Sport.Rulesets.RuleEditorAttribute pea = attributes[0] as Sport.Rulesets.RuleEditorAttribute;
					Type editorType = Type.GetType(pea.EditorTypeName);
					if (editorType == null)
					{
						Debug.WriteLine("Pattern block editor not found");
						return null;
					}

					editor = Activator.CreateInstance(editorType) as RuleTypeEditor;

					if (editor != null)
					{
						return editor.Edit(r.Rule, r.Scope);
					}
				}
			}

			return null;
		}

		public void EditEnded(Control control)
		{
			if (editor != null)
			{
				editor.EndEdit();
				editor = null;
			}
		}

		private int[] _sort;

		public int[] GetSort(int group)
		{
			return _sort;
		}

		#region Rule Fields Comparers

		private class RuleScopeComparer : System.Collections.IComparer
		{
			#region IComparer Members

			private int CompareCategory(int cx, int cy)
			{
				if (cx == -1)
				{
					if (cy == -1)
						return 0;
					return -1;
				}
				else if (cy == -1)
					return 1;

				return CategoryTypeLookup.ToString(cx).CompareTo(CategoryTypeLookup.ToString(cy));
			}

			private int CompareSportFieldType(int sftx, int sfty)
			{
				if (sftx == -1)
				{
					if (sfty == -1)
						return 0;
					return -1;
				}
				else if (sfty == -1)
					return 1;

				return new Sport.Entities.SportFieldType(sftx).Name.CompareTo(
					new Sport.Entities.SportFieldType(sfty).Name);
			}

			private int CompareSportField(int sfx, int sfy)
			{
				if (sfx == -1)
				{
					if (sfy == -1)
						return 0;
					return -1;
				}
				else if (sfy == -1)
					return 1;

				return new Sport.Entities.SportField(sfx).Name.CompareTo(
					new Sport.Entities.SportField(sfy).Name);
			}

			public int Compare(object x, object y)
			{
				Row rx = (Row) x;
				Row ry = (Row) y;

				int r = CompareCategory(rx.Scope.Category, ry.Scope.Category);
				if (r == 0)
				{
					r = CompareSportFieldType(rx.Scope.SportFieldType, ry.Scope.SportFieldType);
					if (r == 0)
						r = CompareSportField(rx.Scope.SportField, ry.Scope.SportField);
				}

				return r;
			}

			#endregion
		}

		private class RuleRuleTypeComparer : System.Collections.IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				Row rx = (Row) x;
				Row ry = (Row) y;
				return rx.RuleType.ToString().CompareTo(ry.RuleType.ToString());
			}

			#endregion
		}

		private class RuleValueComparer : System.Collections.IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				Row rx = (Row) x;
				Row ry = (Row) y;
				return rx.Value.ToString().CompareTo(ry.Value.ToString());
			}

			#endregion
		}

		private System.Collections.IComparer[] ruleFieldComparers =
			new System.Collections.IComparer[]
			{
				new RuleRuleTypeComparer(),
				new RuleScopeComparer(),
				new RuleValueComparer()
			};

		#endregion

		public void Sort(int group, int[] columns)
		{
			if (columns == null)
			{
				_sort = null;
			}
			else
			{
				_sort = (int[]) columns.Clone();

				Sport.Common.ArraySort.Sort(rows, _sort, ruleFieldComparers);
			}
		}

		public void Dispose()
		{
			rows = null;
		}

		private static void SportFieldTypeChanged(object sender, EventArgs e)
		{
			GenericItem gi = sender as GenericItem;
			GenericEditDialog ged = gi.Container as GenericEditDialog;

			Entity en = ged.Items[2].Value as Entity;
			if (en == null)
			{
				ged.Items[3].Values = null;
			}
			else
			{
				Sport.Entities.SportFieldType sportFieldType = new Sport.Entities.SportFieldType(en);

				EntityFilter filter=new EntityFilter(
					(int) Sport.Entities.SportField.Fields.SportFieldType, sportFieldType.Id);
				Sport.Data.Entity[] sportFields = Sport.Entities.SportField.Type.GetEntities(filter);
				ged.Items[3].Values = sportFields;
			}
		}
	}
}
