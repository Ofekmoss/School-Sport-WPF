using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Championships;
using Sport.Rulesets;
using Sport.Rulesets.Rules;
using System.Collections.Generic;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for MatchesTimeFacilityForm.
	/// </summary>
	public class MatchesTimeFacilityForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.DateTimePicker dtpTime;

		private Sport.Championships.Match[] _matches;
		private Sport.Entities.Championship _championship;

		private LocationsGridSource sourceLocations;

		private System.Windows.Forms.Label labelMatchDuration;
		private System.Windows.Forms.NumericUpDown nudMatchDuration;
		private System.Windows.Forms.CheckBox cbByLocation;
		private System.Windows.Forms.Button btnAddLocation;
		private System.Windows.Forms.Button btnRemoveLocation;
		private System.Windows.Forms.GroupBox gbFunctionaries;
		private System.Windows.Forms.Label lbFuncError;
		private Sport.UI.Controls.Grid gdFunctionaries;
		private Sport.UI.Controls.Grid gridLocations;
		private System.Windows.Forms.CheckBox cbSetFunctionaries;

		private FunctionaryGridSource _funcSource = null;

		public MatchesTimeFacilityForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			gridLocations.Groups.Add();
			gridLocations.Groups[0].Columns.Add(0, "מיקומים", 1);
			gridLocations.Groups[1].Columns.Add(0, "מיקומים", 1);

			gridLocations.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.List;

			gdFunctionaries.Columns.Add(0, "תיאור", 145);
			gdFunctionaries.Columns.Add(1, "שם", 200);
		}

		public void SetMatches(string owner, Sport.Championships.Match[] matches)
		{
			string title = "קביעת מועדים, מתקנים, ובעלי תפקידים";
			if (owner.Length > 0)
				title += "- " + owner;
			this.Text = title;
			_matches = matches;

			_championship = _matches[0].Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship;

			if (sourceLocations == null)
			{
				sourceLocations = new LocationsGridSource(_championship.Sport);
				gridLocations.Source = sourceLocations;
			}

			SetFunctionaries();

			if (matches != null && matches.Length > 0)
			{
				DateTime dtStart = matches[0].Time;
				int gameDuration = 0;
				int[] functionaries = matches[0].Functionaries;
				ArrayList facilities = new ArrayList();
				ArrayList courts = new ArrayList();
				bool blnFoundAllFacilities = false;
				for (int i = 0; i < matches.Length; i++)
				{
					Sport.Championships.Match match = matches[i];
					if (match.Time.Year < 1900)
					{
						gameDuration = -1;
					}
					else
					{
						if (gameDuration >= 0 && i > 0)
						{
							int curDuration = GetGameDuration(match.Time, matches[i - 1].Time);
							if (gameDuration > 0 && gameDuration != curDuration)
								gameDuration = -1;
							else
								gameDuration = curDuration;
						}
					}
					if (functionaries != null && !Sport.Common.Tools.SameArrays(match.Functionaries, functionaries))
						functionaries = null;
					if (facilities != null)
					{
						if (match.Facility == null)
						{
							facilities = null;
							courts = null;
						}
						else
						{
							if (!blnFoundAllFacilities)
							{
								if (facilities.Count > 0 && match.Facility.Id == (facilities[0] as Sport.Entities.Facility).Id)
								{
									blnFoundAllFacilities = true;
								}
								else
								{
									facilities.Add(match.Facility);
									if (match.Court != null)
										courts.Add(match.Court);
								}
							}
						}
					}
				}

				if (dtStart.Year > 1900 && gameDuration >= 0)
				{
					dtpTime.Checked = true;
					dtpTime_ValueChanged(null, EventArgs.Empty);
					dtpTime.Value = dtStart;
					if (gameDuration < (int)nudMatchDuration.Maximum)
						nudMatchDuration.Value = gameDuration;
				}

				if (functionaries != null && _funcSource != null)
					_funcSource.SetFunctionaries(functionaries);

				if (facilities != null)
				{
					foreach (Sport.Entities.Facility curFacility in facilities)
					{
						sourceLocations.Add(curFacility);
						foreach (Sport.Entities.Court curCourt in courts)
						{
							if (curCourt.Facility != null && curCourt.Facility.Id == curFacility.Id)
							{
								sourceLocations.Add(curCourt);
								break;
							}
						}
					}
					gridLocations.Refresh();
				}
			}
		}

		private int GetGameDuration(DateTime time1, DateTime time2)
		{
			DateTime t1 = Sport.Common.Tools.SetTime(time1, time1.Hour, time1.Minute, 0);
			DateTime t2 = Sport.Common.Tools.SetTime(time2, time2.Hour, time2.Minute, 0);
			return (int)((t1 - t2).TotalMinutes);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Sport.UI.Controls.GridDefaultSource gridDefaultSource1 = new Sport.UI.Controls.GridDefaultSource();
			Sport.UI.Controls.GridDefaultSource gridDefaultSource2 = new Sport.UI.Controls.GridDefaultSource();
			this.labelTime = new System.Windows.Forms.Label();
			this.dtpTime = new System.Windows.Forms.DateTimePicker();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.labelMatchDuration = new System.Windows.Forms.Label();
			this.nudMatchDuration = new System.Windows.Forms.NumericUpDown();
			this.cbByLocation = new System.Windows.Forms.CheckBox();
			this.gridLocations = new Sport.UI.Controls.Grid();
			this.btnAddLocation = new System.Windows.Forms.Button();
			this.btnRemoveLocation = new System.Windows.Forms.Button();
			this.gbFunctionaries = new System.Windows.Forms.GroupBox();
			this.lbFuncError = new System.Windows.Forms.Label();
			this.gdFunctionaries = new Sport.UI.Controls.Grid();
			this.cbSetFunctionaries = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.nudMatchDuration)).BeginInit();
			this.gbFunctionaries.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Location = new System.Drawing.Point(460, 24);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(67, 19);
			this.labelTime.TabIndex = 0;
			this.labelTime.Text = "ממועד:";
			// 
			// dtpTime
			// 
			this.dtpTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dtpTime.Checked = false;
			this.dtpTime.CustomFormat = "dd/MM/yyyy HH:mm";
			this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpTime.Location = new System.Drawing.Point(11, 19);
			this.dtpTime.Name = "dtpTime";
			this.dtpTime.ShowCheckBox = true;
			this.dtpTime.Size = new System.Drawing.Size(415, 24);
			this.dtpTime.TabIndex = 1;
			this.dtpTime.ValueChanged += new System.EventHandler(this.dtpTime_ValueChanged);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnOK.Location = new System.Drawing.Point(11, 456);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(101, 28);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(123, 456);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(101, 28);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// labelMatchDuration
			// 
			this.labelMatchDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMatchDuration.Location = new System.Drawing.Point(426, 57);
			this.labelMatchDuration.Name = "labelMatchDuration";
			this.labelMatchDuration.Size = new System.Drawing.Size(101, 20);
			this.labelMatchDuration.TabIndex = 15;
			this.labelMatchDuration.Text = "משך משחק:";
			// 
			// nudMatchDuration
			// 
			this.nudMatchDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudMatchDuration.Enabled = false;
			this.nudMatchDuration.Location = new System.Drawing.Point(363, 52);
			this.nudMatchDuration.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
			this.nudMatchDuration.Name = "nudMatchDuration";
			this.nudMatchDuration.Size = new System.Drawing.Size(63, 24);
			this.nudMatchDuration.TabIndex = 16;
			// 
			// cbByLocation
			// 
			this.cbByLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbByLocation.Enabled = false;
			this.cbByLocation.Location = new System.Drawing.Point(263, 52);
			this.cbByLocation.Name = "cbByLocation";
			this.cbByLocation.Size = new System.Drawing.Size(94, 28);
			this.cbByLocation.TabIndex = 17;
			this.cbByLocation.Text = "לפי מיקום";
			// 
			// gridLocations
			// 
			this.gridLocations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridLocations.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridLocations.Editable = true;
			this.gridLocations.ExpandOnDoubleClick = true;
			this.gridLocations.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridLocations.HeaderHeight = 17;
			this.gridLocations.HorizontalLines = true;
			this.gridLocations.Location = new System.Drawing.Point(11, 237);
			this.gridLocations.Name = "gridLocations";
			this.gridLocations.SelectedIndex = -1;
			this.gridLocations.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.gridLocations.SelectOnSpace = false;
			this.gridLocations.ShowCheckBoxes = false;
			this.gridLocations.ShowRowNumber = false;
			this.gridLocations.Size = new System.Drawing.Size(513, 213);
			this.gridLocations.Source = gridDefaultSource1;
			this.gridLocations.TabIndex = 18;
			this.gridLocations.VerticalLines = true;
			this.gridLocations.VisibleRow = 0;
			// 
			// btnAddLocation
			// 
			this.btnAddLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddLocation.Location = new System.Drawing.Point(479, 456);
			this.btnAddLocation.Name = "btnAddLocation";
			this.btnAddLocation.Size = new System.Drawing.Size(67, 28);
			this.btnAddLocation.TabIndex = 19;
			this.btnAddLocation.Text = "הוסף";
			this.btnAddLocation.Click += new System.EventHandler(this.btnAddLocation_Click);
			// 
			// btnRemoveLocation
			// 
			this.btnRemoveLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveLocation.Location = new System.Drawing.Point(400, 456);
			this.btnRemoveLocation.Name = "btnRemoveLocation";
			this.btnRemoveLocation.Size = new System.Drawing.Size(68, 28);
			this.btnRemoveLocation.TabIndex = 20;
			this.btnRemoveLocation.Text = "הסר";
			this.btnRemoveLocation.Click += new System.EventHandler(this.btnRemoveLocation_Click);
			// 
			// gbFunctionaries
			// 
			this.gbFunctionaries.Controls.Add(this.lbFuncError);
			this.gbFunctionaries.Controls.Add(this.gdFunctionaries);
			this.gbFunctionaries.Location = new System.Drawing.Point(7, 85);
			this.gbFunctionaries.Name = "gbFunctionaries";
			this.gbFunctionaries.Size = new System.Drawing.Size(539, 146);
			this.gbFunctionaries.TabIndex = 21;
			this.gbFunctionaries.TabStop = false;
			this.gbFunctionaries.Text = "בעלי תפקידים";
			// 
			// lbFuncError
			// 
			this.lbFuncError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbFuncError.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbFuncError.ForeColor = System.Drawing.Color.Red;
			this.lbFuncError.Location = new System.Drawing.Point(36, 36);
			this.lbFuncError.Name = "lbFuncError";
			this.lbFuncError.Size = new System.Drawing.Size(497, 49);
			this.lbFuncError.TabIndex = 6;
			this.lbFuncError.Text = "שגיאה";
			this.lbFuncError.Visible = false;
			// 
			// gdFunctionaries
			// 
			this.gdFunctionaries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gdFunctionaries.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gdFunctionaries.Editable = true;
			this.gdFunctionaries.ExpandOnDoubleClick = true;
			this.gdFunctionaries.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.gdFunctionaries.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gdFunctionaries.HeaderHeight = 17;
			this.gdFunctionaries.HorizontalLines = true;
			this.gdFunctionaries.Location = new System.Drawing.Point(4, 36);
			this.gdFunctionaries.Name = "gdFunctionaries";
			this.gdFunctionaries.SelectedIndex = -1;
			this.gdFunctionaries.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gdFunctionaries.SelectOnSpace = false;
			this.gdFunctionaries.ShowCheckBoxes = false;
			this.gdFunctionaries.ShowRowNumber = false;
			this.gdFunctionaries.Size = new System.Drawing.Size(535, 110);
			this.gdFunctionaries.Source = gridDefaultSource2;
			this.gdFunctionaries.TabIndex = 6;
			this.gdFunctionaries.VerticalLines = true;
			this.gdFunctionaries.VisibleRow = 0;
			// 
			// cbSetFunctionaries
			// 
			this.cbSetFunctionaries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSetFunctionaries.Location = new System.Drawing.Point(82, 51);
			this.cbSetFunctionaries.Name = "cbSetFunctionaries";
			this.cbSetFunctionaries.Size = new System.Drawing.Size(179, 28);
			this.cbSetFunctionaries.TabIndex = 22;
			this.cbSetFunctionaries.Text = "קבע בעלי תפקידים";
			// 
			// MatchesTimeFacilityForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.ClientSize = new System.Drawing.Size(560, 495);
			this.Controls.Add(this.cbSetFunctionaries);
			this.Controls.Add(this.gbFunctionaries);
			this.Controls.Add(this.btnRemoveLocation);
			this.Controls.Add(this.btnAddLocation);
			this.Controls.Add(this.gridLocations);
			this.Controls.Add(this.cbByLocation);
			this.Controls.Add(this.nudMatchDuration);
			this.Controls.Add(this.labelMatchDuration);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.dtpTime);
			this.Controls.Add(this.labelTime);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "MatchesTimeFacilityForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "קבע מועדים, מתקנים, ובעלי תפקידים";
			this.Load += new System.EventHandler(this.MatchesTimeFacilityForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.nudMatchDuration)).EndInit();
			this.gbFunctionaries.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			object[] locations = sourceLocations.GetLocations();
			bool gotLocations = locations.Length > 0;

			/*
			if (!setLocation && !setTime && !setFunc)
			{
				Sport.UI.MessageBox.Error("אנא בחר קביעת מועדים, מתקנים או בעלי תפקידים", "עריכת משחקים");
				return ;
			}
			*/

			List<int> arrFunctionaries = new List<int>();
			if (_funcSource != null)
			{
				Functionaries func = _funcSource.GetFunctionaries();
				arrFunctionaries.AddRange(func.Fields.OfType<FunctionaryValue>().ToList().ConvertAll(f => f.Id));
			}
			int matchDuration = (int)nudMatchDuration.Value;
			DateTime curMatchTime = dtpTime.Checked ? dtpTime.Value : DateTime.MinValue;
			int locationIndex = 0;
			for (int n = 0; n < _matches.Length; n++)
			{
				Match match = _matches[n];
				DateTime time = dtpTime.Checked ? curMatchTime : match.Time;
				Sport.Entities.Facility facility;
				Sport.Entities.Court court;
				int[] functionaries = cbSetFunctionaries.Checked ? arrFunctionaries.ToArray() : match.Functionaries;
				if (gotLocations)
				{
					court = locations[locationIndex] as Sport.Entities.Court;
					facility = (court == null) ? locations[locationIndex] as Sport.Entities.Facility : court.Facility;
				}
				else
				{
					facility = null;
					court = null;
				}
				match.Set(match.Number, time, facility, court, functionaries);
				locationIndex++;
				if (!cbByLocation.Checked || !gotLocations || locationIndex >= locations.Length)
					curMatchTime = curMatchTime.AddMinutes(matchDuration);
				if (locationIndex >= locations.Length)
					locationIndex = 0;
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private Sport.UI.Dialogs.GenericEditDialog locationDialog = null;
		private Sport.UI.EntitySelectionDialog facilityDialog = null;
		private void btnAddLocation_Click(object sender, System.EventArgs e)
		{
			if (locationDialog == null)
			{
				locationDialog = new Sport.UI.Dialogs.GenericEditDialog("בחר מיקום");
				locationDialog.Items.Add("מתקן", Sport.UI.Controls.GenericItemType.Button);
				locationDialog.Items.Add("מגרש", Sport.UI.Controls.GenericItemType.Selection);

				Views.FacilitiesTableView facilityView = new Views.FacilitiesTableView();
				facilityView.State[Sport.UI.TableView.SelectionDialog] = "1";
				facilityDialog = new Sport.UI.EntitySelectionDialog(facilityView);

				locationDialog.Items[0].Values =
					Sport.UI.Controls.GenericItem.ButtonValues(
					new Sport.UI.Controls.ButtonBox.SelectValue(facilityDialog.ValueSelector));
				//_championship.GetFacilities();
				locationDialog.Items[0].Nullable = false;
				locationDialog.Items[0].ValueChanged += new EventHandler(MatchesTimeFacilityForm_FacilityChanged);
				locationDialog.Confirmable = false;
			}

			if (locationDialog.ShowDialog() == DialogResult.OK)
			{
				Sport.Entities.Court court = locationDialog.Items[1].Value as Sport.Entities.Court;
				if (court != null)
				{
					sourceLocations.Add(court);
				}
				else
				{
					Sport.Data.Entity facilityEnt =
						locationDialog.Items[0].Value as Sport.Data.Entity;
					if (facilityEnt != null)
					{
						Sport.Entities.Facility facility =
							new Sport.Entities.Facility(facilityEnt);
						sourceLocations.Add(facility);
					}
				}
				cbByLocation.Enabled = dtpTime.Checked &&
					sourceLocations.GetRowCount() > 0;
				if (!cbByLocation.Enabled)
					cbByLocation.Checked = false;
			}
		}

		private void MatchesTimeFacilityForm_FacilityChanged(object sender, EventArgs e)
		{
			Sport.Data.Entity facilityEnt =
				locationDialog.Items[0].Value as Sport.Data.Entity;
			if (facilityEnt == null)
			{
				locationDialog.Items[1].Values = null;
				locationDialog.Confirmable = false;
			}
			else
			{
				Sport.Entities.Facility facility = new Sport.Entities.Facility(facilityEnt);
				locationDialog.Items[1].Values = facility.GetCourts(_championship.Sport);
				locationDialog.Confirmable = true;
			}
		}

		private void btnRemoveLocation_Click(object sender, System.EventArgs e)
		{
			if (gridLocations.Selection.Size == 1)
			{
				sourceLocations.RemoveAt(gridLocations.Selection.Rows[0]);
				cbByLocation.Enabled = dtpTime.Checked &&
					sourceLocations.GetRowCount() > 0;
				if (!cbByLocation.Enabled)
					cbByLocation.Checked = false;
			}
		}

		private void dtpTime_ValueChanged(object sender, System.EventArgs e)
		{
			bool setTime = dtpTime.Checked;
			nudMatchDuration.Enabled = setTime;
			// must have locations to set by location
			cbByLocation.Enabled = setTime && sourceLocations.GetRowCount() > 0;
			if (!cbByLocation.Enabled)
				cbByLocation.Checked = false;
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void SetFunctionaries()
		{
			Sport.Entities.Ruleset rulesetEnt = _championship.Ruleset;

			if (rulesetEnt == null)
			{
				//maybe sport has default ruleset?
				rulesetEnt = _championship.Sport.Ruleset;
			}

			if (rulesetEnt == null)
			{
				SetError("לא מוגדר תקנון עבור האליפות");
				return;
			}

			Functionaries func = null;
			if (Sport.Core.Session.Connected)
			{
				// Creating a clone of the loaded rule set
				Ruleset ruleset = new Ruleset(Ruleset.LoadRuleset(rulesetEnt.Id));
				func = (Functionaries)ruleset.GetRule(RuleScope.AnyScope, typeof(Functionaries));
			}
			else
			{
				object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(Sport.Rulesets.Rules.FunctionariesRule),
					_matches[0].Cycle.Round.Group.Phase.Championship.CategoryID, -1);
				if (rule != null)
					func = (Functionaries)rule;
			}

			//got anything?
			if (func == null)
			{
				SetError("לא מוגדר חוק בעלי תפקידים בתקנון האליפות");
				return;
			}

			//reset:
			for (int i = 0; i < func.Fields.Count; i++)
				func.Fields[i].Id = -1;

			int[] matchFuncs = _matches[0].Functionaries;

			if (matchFuncs != null)
			{
				for (int i = 0; i < func.Fields.Count; i++)
				{
					if (i >= matchFuncs.Length)
						break;
					func.Fields[i].Id = matchFuncs[i];
				}
			}

			_funcSource = new FunctionaryGridSource(func);
			gdFunctionaries.Source = _funcSource;
		}

		private void SetError(string strMsg)
		{
			lbFuncError.Text = strMsg;
			lbFuncError.Visible = true;
			gdFunctionaries.Visible = false;
		}

		private void MatchesTimeFacilityForm_Load(object sender, System.EventArgs e)
		{
			cbSetFunctionaries.Checked = true;
		}

		#region Functionary Grid Source
		public class FunctionaryGridSource : Sport.UI.Controls.IGridSource
		{
			private Sport.UI.EntitySelectionDialog functionaryDialog;
			private Sport.UI.Controls.Grid _grid;
			private Functionaries _functionaries;

			public Functionaries GetFunctionaries()
			{
				return _functionaries;
			}

			public void SetFunctionaries(int[] funcs)
			{
				if (funcs != null)
				{
					for (int i = 0; i < _functionaries.Fields.Count; i++)
					{
						if (i >= funcs.Length)
							break;
						_functionaries.Fields[i].Id = funcs[i];
					}
					this._grid.Refresh();
				}
			}

			public FunctionaryGridSource(Functionaries functionaries)
			{
				_functionaries = functionaries;
				functionaryDialog = new Sport.UI.EntitySelectionDialog(
					new Views.FunctionariesTableView());
				functionaryDialog.View.State["type"] = ((int)Sport.Types.FunctionaryType.Coordinator).ToString();
				functionaryDialog.View.State[Sport.UI.View.SelectionDialog] = "1";
			}

			#region IGridSource Members
			public void SetGrid(Sport.UI.Controls.Grid grid)
			{
				_grid = grid;
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

			public Control Edit(int row, int field)
			{
				FunctionaryValue funcValue = _functionaries.Fields[row];

				if (field == 1) // Value
				{
					Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
					functionaryDialog.View.State["type"] =
						((int)funcValue.Type).ToString();
					int funcID = funcValue.Id;
					if (funcID < 0)
					{
						bb.Value = null;
					}
					else
					{
						bb.Value = new Sport.Entities.Functionary(funcID).Entity;
					}
					bb.Tag = funcValue;
					bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(functionaryDialog.ValueSelector);
					bb.ValueChanged += new EventHandler(FunctionaryChanged);
					return bb;
				}

				return null;
			}

			public void EditEnded(Control control)
			{
			}

			public int GetRowCount()
			{
				return _functionaries.Fields.Count;
			}

			public int[] GetSort(int group)
			{
				return null;
			}

			public string GetText(int row, int field)
			{
				FunctionaryValue funcValue = _functionaries.Fields[row];
				switch (field)
				{
					case 0:
						return funcValue.Description;
					case 1:
						if (funcValue.Id < 0)
							return "";
						return (new Sport.Entities.Functionary(funcValue.Id)).Name;
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
				//rows = null;
			}
			#endregion

			private void FunctionaryChanged(object sender, EventArgs e)
			{
				Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
				Sport.Data.Entity funcEnt = bb.Value as Sport.Data.Entity;
				FunctionaryValue funcValue = bb.Tag as FunctionaryValue;
				funcValue.Id = (funcEnt == null) ? -1 : funcEnt.Id;
			}
		}
		#endregion

		private void btnRemoveFunctionaries_Click(object sender, EventArgs e)
		{
			if (_funcSource != null)
			{
				cbSetFunctionaries.Checked = true;
				Functionaries func = _funcSource.GetFunctionaries();
				for (int n = 0; n < func.Fields.Count; n++)
					func.Fields[n].Id = -1;
				gdFunctionaries.Refresh();
			}
			
		}
	}

	#region Location Grid Source
	public class LocationsGridSource : Sport.UI.Controls.IGridSource
	{
		public object SelectedLocation
		{
			get
			{
				if (_grid.Selection.Size == 1)
				{
					return locations[_grid.Selection.Rows[0]];
				}

				return null;
			}
		}

		private Sport.UI.Controls.Grid _grid;
		private ArrayList locations;
		private Sport.Entities.Sport _sport;

		public void Add(Sport.Entities.Facility facility)
		{
			for (int n = 0; n < locations.Count; n++)
			{
				if (locations[n].Equals(facility))
					return; // facility already there
			}

			locations.Add(facility);

			_grid.RefreshSource();
		}

		public void Add(Sport.Entities.Court court)
		{
			Sport.Entities.CourtType[] courtTypes = _sport.GetCourtTypes();

			bool courtTypeOk = false;

			for (int n = 0; n < courtTypes.Length && !courtTypeOk; n++)
			{
				if (court.CourtType.Equals(courtTypes[n]))
					courtTypeOk = true;
			}

			if (courtTypeOk)
			{
				Sport.Entities.Facility facility = court.Facility;
				int fi = -1;
				for (int n = 0; n < locations.Count; n++)
				{
					if (locations[n].Equals(court))
						return; // court already there
					if (locations[n].Equals(facility))
						fi = n;
				}

				if (fi == -1)
				{
					fi = locations.Add(facility);
				}

				locations.Insert(fi + 1, court);

				_grid.RefreshSource();
			}
		}

		public void RemoveAt(int index)
		{
			if ((locations == null) || (index < 0) || (index >= locations.Count))
			{
				System.Diagnostics.Debug.WriteLine("invalid index for LocationGridsource RemoveAt: " + index);
				System.Diagnostics.Debug.WriteLine("locations: " + ((locations == null) ? ("NULL") : (locations.Count.ToString())));
				return;
			}
			if (locations[index] is Sport.Entities.Court)
			{
				locations.RemoveAt(index); // removing only court
			}
			else
			{
				// removing the facility and its courts
				locations.RemoveAt(index);
				while (index < locations.Count && locations[index] is Sport.Entities.Court)
					locations.RemoveAt(index);
			}
			_grid.RefreshSource();
		}

		public object[] GetLocations()
		{
			ArrayList al = new ArrayList();
			bool lastWasFacility = false;
			for (int n = 0; n < locations.Count; n++)
			{
				if (locations[n] is Sport.Entities.Facility)
				{
					al.Add(locations[n]);
					lastWasFacility = true;
				}
				else
				{
					if (lastWasFacility)
					{
						// if last one was a facility and current one
						// is a court - removing the facility because
						// using its courts instead
						al[al.Count - 1] = locations[n];
						lastWasFacility = false;
					}
					else
					{
						al.Add(locations[n]);
					}
				}
			}

			return al.ToArray();
		}

		public LocationsGridSource(Sport.Entities.Sport sport)
		{
			_sport = sport;
			locations = new ArrayList();
		}

		#region IGridSource Members

		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
			_grid = grid;
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

		public Control Edit(int row, int field)
		{
			return null;
		}

		public void EditEnded(Control control)
		{
		}


		public int[] GetSort(int group)
		{
			return null;
		}

		public int GetGroup(int row)
		{
			if (locations[row] is Sport.Entities.Facility)
				return 0;
			return 1;
		}

		public int GetRowCount()
		{
			return locations.Count;
		}

		public string GetText(int row, int field)
		{
			return ((Sport.Data.EntityBase)locations[row]).Name;
		}

		public int GetFieldCount(int row)
		{
			return 1;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
	#endregion
}

