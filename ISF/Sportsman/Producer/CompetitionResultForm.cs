using System;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using System.Data;
using Sport.UI.Dialogs;
using Sport.UI.Controls;
using System.Drawing;
using System.Collections.Generic;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for CompetitionResultForm.
	/// </summary>
	public class CompetitionResultForm : System.Windows.Forms.Form,
		Sport.UI.Controls.IGridSource
	{
		//////////// added by Lior /////////////////
		//private Sport.UI.Controls.ThemeButton btnUp;
		private Sport.UI.Controls.ThemeButton btnDown;
		////////////////////////////////////////////

		private bool _sharedResults = false;
		private bool _sharedResultsModeActive = false;
		private int _maxSharedResults = 0;
		private int _sharedResultCurrentIndex = 0;
		private int _currentSharedResultRow = -1;

		private bool _disqualifications = false;
		private bool _wind = false;

		private int _disqualificationsField = -1;
		private int _windField = -1;

		private Button _btnActivateSharedResults;
		private Label _lbSharedResultsCurrentStatus;
		private Button _btnFinishSharedResults;

		private Button _btnApplyDisqualifications;

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label labelNumber;
		private System.Windows.Forms.Label labelTeam;
		private System.Windows.Forms.Label labelResult;
		private Sport.UI.Controls.TextControl tcNumber;
		private System.Windows.Forms.TextBox tbTeam;
		private Sport.UI.Controls.TextItemsControl ticResult;
		private System.Windows.Forms.Label labelPlayer;
		private System.Windows.Forms.TextBox tbPlayer;
		private Sport.UI.Controls.Grid gridResults;

		private int _minCompetitorCompetitions = 0;
		private int _maxCompetitorCompetitions = 0;
		private int _minCompetitionTeamCompetitors = 0;

		private CompetitionChampionshipEditorView parentView = null;
		private System.Windows.Forms.Button btnInputResults;
		private System.Windows.Forms.GroupBox gbResults;
		private System.Windows.Forms.Button btnApplyResults;
		private System.Windows.Forms.Panel pnlCurrentResult;
		private System.Windows.Forms.Label label1;
		private int _maxCompetitionTeamCompetitors = 0;

		private int[] _customResults = null;
		private System.Windows.Forms.Label lbCurrentResult;
		private int _customResultIndex = -1;

		private bool _blnChangesOccured = false;
		private bool _forceClose = false;

		public bool ChangesOccured { get { return _blnChangesOccured; } }

		public void ForceCloseWithoutConfirmation()
		{
			_forceClose = true;
		}

		public CompetitionChampionshipEditorView ParentView
		{
			get { return parentView; }
			set { parentView = value; }
		}

		#region designer code
		private void InitializeComponent()
		{
			Sport.UI.Controls.GridDefaultSource gridDefaultSource1 = new Sport.UI.Controls.GridDefaultSource();
			this.btnDown = new Sport.UI.Controls.ThemeButton();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gridResults = new Sport.UI.Controls.Grid();
			this.tcNumber = new Sport.UI.Controls.TextControl();
			this.tbTeam = new System.Windows.Forms.TextBox();
			this.ticResult = new Sport.UI.Controls.TextItemsControl();
			this.labelNumber = new System.Windows.Forms.Label();
			this.labelTeam = new System.Windows.Forms.Label();
			this.labelPlayer = new System.Windows.Forms.Label();
			this.labelResult = new System.Windows.Forms.Label();
			this.tbPlayer = new System.Windows.Forms.TextBox();
			this.btnInputResults = new System.Windows.Forms.Button();
			this.gbResults = new System.Windows.Forms.GroupBox();
			this.btnApplyResults = new System.Windows.Forms.Button();
			this.pnlCurrentResult = new System.Windows.Forms.Panel();
			this.lbCurrentResult = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.gbResults.SuspendLayout();
			this.pnlCurrentResult.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnDown
			// 
			this.btnDown.Alignment = System.Drawing.StringAlignment.Center;
			this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDown.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.btnDown.Hue = 300F;
			this.btnDown.Image = null;
			this.btnDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnDown.ImageList = null;
			this.btnDown.ImageSize = new System.Drawing.Size(0, 0);
			this.btnDown.Location = new System.Drawing.Point(277, 427);
			this.btnDown.Name = "btnDown";
			this.btnDown.Saturation = 0.1F;
			this.btnDown.Size = new System.Drawing.Size(56, 37);
			this.btnDown.TabIndex = 3;
			this.btnDown.Text = "▼";
			this.btnDown.Transparent = System.Drawing.Color.Black;
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Location = new System.Drawing.Point(11, 433);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(105, 28);
			this.btnOK.TabIndex = 3;
			this.btnOK.TabStop = false;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Location = new System.Drawing.Point(123, 433);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(105, 28);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "ביטול";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// gridResults
			// 
			this.gridResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gridResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridResults.Editable = true;
			this.gridResults.ExpandOnDoubleClick = true;
			this.gridResults.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridResults.HeaderHeight = 17;
			this.gridResults.HorizontalLines = true;
			this.gridResults.Location = new System.Drawing.Point(11, 68);
			this.gridResults.Name = "gridResults";
			this.gridResults.SelectedIndex = -1;
			this.gridResults.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridResults.SelectOnSpace = false;
			this.gridResults.ShowCheckBoxes = false;
			this.gridResults.ShowRowNumber = false;
			this.gridResults.Size = new System.Drawing.Size(715, 330);
			this.gridResults.Source = gridDefaultSource1;
			this.gridResults.TabIndex = 26;
			this.gridResults.TabStop = false;
			this.gridResults.VerticalLines = true;
			this.gridResults.VisibleRow = 0;
			// 
			// tcNumber
			// 
			this.tcNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tcNumber.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tcNumber.Controller = null;
			this.tcNumber.Location = new System.Drawing.Point(650, 29);
			this.tcNumber.Name = "tcNumber";
			this.tcNumber.ReadOnly = false;
			this.tcNumber.SelectionLength = 0;
			this.tcNumber.SelectionStart = 0;
			this.tcNumber.ShowSpin = false;
			this.tcNumber.Size = new System.Drawing.Size(79, 26);
			this.tcNumber.TabIndex = 27;
			this.tcNumber.Value = "";
			// 
			// tbTeam
			// 
			this.tbTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbTeam.Location = new System.Drawing.Point(393, 29);
			this.tbTeam.Name = "tbTeam";
			this.tbTeam.ReadOnly = true;
			this.tbTeam.Size = new System.Drawing.Size(246, 24);
			this.tbTeam.TabIndex = 28;
			this.tbTeam.TabStop = false;
			// 
			// ticResult
			// 
			this.ticResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ticResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ticResult.Items = new Sport.UI.Controls.TextItemsControl.TextItem[0];
			this.ticResult.Location = new System.Drawing.Point(23, 29);
			this.ticResult.Name = "ticResult";
			this.ticResult.ReadOnly = true;
			this.ticResult.SelectionLength = 0;
			this.ticResult.SelectionStart = 0;
			this.ticResult.ShowSpin = true;
			this.ticResult.Size = new System.Drawing.Size(146, 24);
			this.ticResult.TabIndex = 29;
			// 
			// labelNumber
			// 
			this.labelNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNumber.Location = new System.Drawing.Point(673, 10);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(56, 19);
			this.labelNumber.TabIndex = 30;
			this.labelNumber.Text = "מספר:";
			// 
			// labelTeam
			// 
			this.labelTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeam.Location = new System.Drawing.Point(572, 10);
			this.labelTeam.Name = "labelTeam";
			this.labelTeam.Size = new System.Drawing.Size(67, 19);
			this.labelTeam.TabIndex = 31;
			this.labelTeam.Text = "קבוצה:";
			// 
			// labelPlayer
			// 
			this.labelPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPlayer.Location = new System.Drawing.Point(303, 10);
			this.labelPlayer.Name = "labelPlayer";
			this.labelPlayer.Size = new System.Drawing.Size(79, 19);
			this.labelPlayer.TabIndex = 32;
			this.labelPlayer.Text = "שחקן:";
			// 
			// labelResult
			// 
			this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelResult.Location = new System.Drawing.Point(102, 10);
			this.labelResult.Name = "labelResult";
			this.labelResult.Size = new System.Drawing.Size(67, 19);
			this.labelResult.TabIndex = 33;
			this.labelResult.Text = "תוצאה:";
			// 
			// tbPlayer
			// 
			this.tbPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbPlayer.Location = new System.Drawing.Point(180, 29);
			this.tbPlayer.Name = "tbPlayer";
			this.tbPlayer.ReadOnly = true;
			this.tbPlayer.Size = new System.Drawing.Size(202, 24);
			this.tbPlayer.TabIndex = 34;
			this.tbPlayer.TabStop = false;
			// 
			// btnInputResults
			// 
			this.btnInputResults.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnInputResults.Location = new System.Drawing.Point(133, 22);
			this.btnInputResults.Name = "btnInputResults";
			this.btnInputResults.Size = new System.Drawing.Size(101, 28);
			this.btnInputResults.TabIndex = 35;
			this.btnInputResults.Text = "קליטה";
			this.btnInputResults.Click += new System.EventHandler(this.btnInputResults_Click);
			// 
			// gbResults
			// 
			this.gbResults.Controls.Add(this.btnApplyResults);
			this.gbResults.Controls.Add(this.btnInputResults);
			this.gbResults.Location = new System.Drawing.Point(469, 505);
			this.gbResults.Name = "gbResults";
			this.gbResults.Size = new System.Drawing.Size(238, 58);
			this.gbResults.TabIndex = 36;
			this.gbResults.TabStop = false;
			this.gbResults.Text = "תוצאות";
			this.gbResults.Visible = false;
			// 
			// btnApplyResults
			// 
			this.btnApplyResults.Enabled = false;
			this.btnApplyResults.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.btnApplyResults.Location = new System.Drawing.Point(14, 22);
			this.btnApplyResults.Name = "btnApplyResults";
			this.btnApplyResults.Size = new System.Drawing.Size(101, 28);
			this.btnApplyResults.TabIndex = 36;
			this.btnApplyResults.Text = "שיוך";
			this.btnApplyResults.Click += new System.EventHandler(this.btnApplyResults_Click);
			// 
			// pnlCurrentResult
			// 
			this.pnlCurrentResult.Controls.Add(this.lbCurrentResult);
			this.pnlCurrentResult.Controls.Add(this.label1);
			this.pnlCurrentResult.Location = new System.Drawing.Point(258, 505);
			this.pnlCurrentResult.Name = "pnlCurrentResult";
			this.pnlCurrentResult.Size = new System.Drawing.Size(190, 58);
			this.pnlCurrentResult.TabIndex = 37;
			this.pnlCurrentResult.Visible = false;
			// 
			// lbCurrentResult
			// 
			this.lbCurrentResult.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lbCurrentResult.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbCurrentResult.ForeColor = System.Drawing.Color.Blue;
			this.lbCurrentResult.Location = new System.Drawing.Point(0, 28);
			this.lbCurrentResult.Name = "lbCurrentResult";
			this.lbCurrentResult.Size = new System.Drawing.Size(190, 30);
			this.lbCurrentResult.TabIndex = 1;
			this.lbCurrentResult.Text = "00:00";
			this.lbCurrentResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(45, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "שיוך נוכחי:";
			// 
			// CompetitionResultForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.ClientSize = new System.Drawing.Size(740, 472);
			this.Controls.Add(this.pnlCurrentResult);
			this.Controls.Add(this.gbResults);
			this.Controls.Add(this.tbPlayer);
			this.Controls.Add(this.tbTeam);
			this.Controls.Add(this.ticResult);
			this.Controls.Add(this.labelResult);
			this.Controls.Add(this.labelPlayer);
			this.Controls.Add(this.labelTeam);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.tcNumber);
			this.Controls.Add(this.gridResults);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnDown);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.MaximizeBox = false;
			this.Name = "CompetitionResultForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "הכנסת תוצאות";
			this.Activated += new System.EventHandler(this.CompetitionResultForm_Activated);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.CompetitionResultForm_Closing);
			this.Closed += new System.EventHandler(this.CompetitionResultForm_Closed);
			this.Load += new System.EventHandler(this.CompetitionResultForm_Load);
			this.gbResults.ResumeLayout(false);
			this.pnlCurrentResult.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private Sport.Championships.Competition _competition = null;
		private Sport.Championships.Heat _heat = null;

		public Sport.Championships.Competition Competition { get { return _competition; } }
		public Sport.Championships.Heat Heat { get { return _heat; } }

		private void SwitchLocation(Control c1, Control c2)
		{
			Point temp = c1.Location;
			c1.Location = c2.Location;
			c2.Location = temp;
		}

		private void ApplySharedResult()
		{
			string text = "נא להזין מספר חזה " + Sport.Common.Tools.IntToHebrew(_sharedResultCurrentIndex + 1, false) + " עבור תוצאה משותפת";
			_lbSharedResultsCurrentStatus.Text = text;
			_lbSharedResultsCurrentStatus.Visible = true;
			_btnFinishSharedResults.Visible = true;
			_btnActivateSharedResults.Visible = false;
			tcNumber.Focus();
		}

		private void ApplySharedResultsControls()
		{
			if (!_sharedResults)
				return;

			_btnActivateSharedResults = new Button();
			_btnActivateSharedResults.AutoSize = true;
			_btnActivateSharedResults.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			_btnActivateSharedResults.Text = "הזנת תוצאה משותפת (עד " + _maxSharedResults + " שחקנים)";
			_btnActivateSharedResults.Top = btnCancel.Top;
			_btnActivateSharedResults.Left = 350;
			_btnActivateSharedResults.Click += new EventHandler(ActivateSharedResultsButtonClicked);
			this.Controls.Add(_btnActivateSharedResults);

			_lbSharedResultsCurrentStatus = new Label();
			_lbSharedResultsCurrentStatus.AutoSize = true;
			_lbSharedResultsCurrentStatus.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			_lbSharedResultsCurrentStatus.Text = "מצב תוצאה משותפת";
			_lbSharedResultsCurrentStatus.Top = btnCancel.Top;
			_lbSharedResultsCurrentStatus.Left = _btnActivateSharedResults.Left + 75;
			_lbSharedResultsCurrentStatus.Visible = false;
			this.Controls.Add(_lbSharedResultsCurrentStatus);

			_btnFinishSharedResults = new Button();
			_btnFinishSharedResults.AutoSize = true;
			_btnFinishSharedResults.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			_btnFinishSharedResults.Text = "סיום";
			_btnFinishSharedResults.Top = btnCancel.Top;
			_btnFinishSharedResults.Left = _btnActivateSharedResults.Left;
			_btnFinishSharedResults.Click += new EventHandler(FinishSharedResultsButtonClicked);
			_btnFinishSharedResults.Visible = false;
			this.Controls.Add(_btnFinishSharedResults);

			//SwitchLocation(tcNumber, tbTeam);
			//SwitchLocation(labelNumber, labelTeam);
		}

		private void ApplyDisqualificationsControls()
		{
			if (!_disqualifications)
				return;

			_btnApplyDisqualifications = new Button();
			_btnApplyDisqualifications.AutoSize = true;
			_btnApplyDisqualifications.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			_btnApplyDisqualifications.Text = "הגדרת פסילות";
			_btnApplyDisqualifications.Top = btnCancel.Top;
			_btnApplyDisqualifications.Left = 350;
			_btnApplyDisqualifications.Enabled = false;
			_btnApplyDisqualifications.Click += new EventHandler(ApplyDisqualifications);
			this.Controls.Add(_btnApplyDisqualifications);
		}

		void ApplyDisqualifications(object sender, EventArgs e)
		{
			if (gridResults.Selection.Size != 1)
				return;

			Row row = results[gridResults.Selection.Rows[0]];
			if (row.PlayerNumber <= 0)
				return;

			using (GenericEditDialog dialog = new GenericEditDialog("הגדרת פסילות עבור מספר חזה " + row.PlayerNumber))
			{
				dialog.Items.Add("פסילות גובה אחרון:", GenericItemType.Number, row.LastResultDisqualifications, new object[] { 0, 100 });
				dialog.Items.Add("סה\"כ פסילות:", GenericItemType.Number, row.TotalDisqualifications, new object[] { 0, 100 });
				dialog.Confirmable = true;
				dialog.Items[0].ValueChanged += new EventHandler(LastResultDisqualificationsChanged);
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					int lastResultDisqualifications = Convert.ToInt32(dialog.Items[0].Value);
					int totalDisqualifications = Convert.ToInt32(dialog.Items[1].Value);
					if (lastResultDisqualifications <= totalDisqualifications)
					{
						row.LastResultDisqualifications = lastResultDisqualifications;
						row.TotalDisqualifications = totalDisqualifications;
						gridResults.RefreshSource();
					}
					else
					{
						Sport.UI.MessageBox.Error("סך כל פסילות לא יכול להיות נמוך יותר מפסילות גובה אחרון", "הגדרת פסילות");
					}
				}
			}
		}

		void LastResultDisqualificationsChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem lastResultItem = (Sport.UI.Controls.GenericItem)sender;
			Sport.UI.Dialogs.GenericEditDialog dialog = (Sport.UI.Dialogs.GenericEditDialog)lastResultItem.Container;
			int lastResultDisqualifications = Convert.ToInt32(lastResultItem.Value);
			int totalDisqualifications = Convert.ToInt32(dialog.Items[1].Value);
			if (lastResultDisqualifications > totalDisqualifications)
				dialog.Items[1].Value = lastResultDisqualifications;
		}

		void ActivateSharedResultsButtonClicked(object sender, EventArgs e)
		{
			_sharedResultsModeActive = true;
			_sharedResultCurrentIndex = 0;
			_currentSharedResultRow = -1;
			ApplySharedResult();
		}

		private void FinishSharedResults()
		{
			_sharedResultsModeActive = false;
			_lbSharedResultsCurrentStatus.Visible = false;
			_btnFinishSharedResults.Visible = false;
			_btnActivateSharedResults.Visible = true;
			ticResult.Focus();
		}

		void FinishSharedResultsButtonClicked(object sender, EventArgs e)
		{
			FinishSharedResults();
		}

		private CompetitionResultForm(Sport.Championships.Competition competition,
			Sport.Championships.Heat heat)
		{
			_competition = competition;
			_heat = heat;
			_maxSharedResults = 0;

			Sport.Rulesets.Rules.GeneralSportTypeData generalSportDataRule = (Sport.Rulesets.Rules.GeneralSportTypeData)_competition.GetRule(
				typeof(Sport.Rulesets.Rules.GeneralSportTypeData), typeof(Sport.Rulesets.Rules.GeneralSportTypeDataRule));
			if (generalSportDataRule != null)
			{
				//Mir.Common.Logger.Instance.WriteLog(Mir.Common.LogType.Debug, "Competition Result Form", "Using general sport data rule: " + generalSportDataRule.ToString());
				_sharedResults = generalSportDataRule.SharedResults;
				if (_sharedResults)
				{
					Sport.Rulesets.Rules.CompetitionTeamCompetitors teamCompetitorsRule = (Sport.Rulesets.Rules.CompetitionTeamCompetitors)_competition.GetRule(
					typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitors), typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitorsRule));
					if (teamCompetitorsRule != null)
						_maxSharedResults = teamCompetitorsRule.Maximum;
					if (_maxSharedResults < 1)
					{
						_sharedResults = false;
						Sport.UI.MessageBox.Warn("לא מוגדר חוק 'משתתפי קבוצה במקצוע', תוצאות משותפות לא פעילות", "קביעת תוצאות תחרות");
					}
				}

				_disqualifications = generalSportDataRule.Disqualifications;
				_wind = generalSportDataRule.Wind;
			}

			InitializeComponent();

			tcNumber.Controller = new Sport.UI.Controls.NumberController(0, 99999, 4, 0);
			tcNumber.LostFocus += new EventHandler(tcNumber_LostFocus);
			tcNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(tcNumber_KeyPress);
			tcNumber.TextChanged += new EventHandler(tcNumber_TextChanged);
			tcNumber.GotFocus += new EventHandler(tcNumber_GotFocus);

			ticResult.SetFormatterItems(competition.ResultType.ValueFormatter);
			ticResult.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ticResult_KeyPress);
			ticResult.GotFocus += new EventHandler(ticResult_GotFocus);
			ticResult.LostFocus += new EventHandler(ticResult_LostFocus);

			gridResults.Groups.Clear();
			int fieldIndex = 0;
			gridResults.Groups[0].Columns.Add(fieldIndex, "מספר", 20);
			fieldIndex++;
			gridResults.Groups[0].Columns.Add(fieldIndex, "קבוצה", 40);
			fieldIndex++;
			gridResults.Groups[0].Columns.Add(fieldIndex, "שחקן", 40);
			fieldIndex++;
			gridResults.Groups[0].Columns.Add(fieldIndex, "תוצאה", 40);
			fieldIndex++;
			if (_disqualifications)
			{
				_disqualificationsField = fieldIndex;
				gridResults.Groups[0].Columns.Add(fieldIndex, "פסילות", 40);
				fieldIndex++;
			}
			if (_wind)
			{
				_windField = fieldIndex;
				gridResults.Groups[0].Columns.Add(fieldIndex, "רוח", 40);
				fieldIndex++;
			}

			ReadResults();

			gridResults.Source = this;

			gridResults.SelectionChanged += new EventHandler(gridResults_SelectionChanged);

			Sport.Rulesets.Rules.CompetitorCompetitions competitorCompetitionsRule =
				(Sport.Rulesets.Rules.CompetitorCompetitions)
				_competition.GetRule(typeof(Sport.Rulesets.Rules.CompetitorCompetitions),
				typeof(Sport.Rulesets.Rules.CompetitorCompetitionsRule));
			if (competitorCompetitionsRule == null)
			{
				_minCompetitorCompetitions = 0;
				_maxCompetitorCompetitions = 99;
				Sport.UI.MessageBox.Warn("לא מוגדר חוק 'מקצועות למשתתף', בדיקת תקנון לא פעילה", "קביעת תוצאות תחרות");
			}
			else
			{
				_minCompetitorCompetitions = competitorCompetitionsRule.Minimum;
				_maxCompetitorCompetitions = competitorCompetitionsRule.Maximum;
			}

			Sport.Rulesets.Rules.CompetitionTeamCompetitors competitionTeamCompetitorsRule =
				(Sport.Rulesets.Rules.CompetitionTeamCompetitors)
				_competition.GetRule(typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitors),
				typeof(Sport.Rulesets.Rules.CompetitionTeamCompetitorsRule));
			if (competitionTeamCompetitorsRule == null)
			{
				_minCompetitionTeamCompetitors = 0;
				_maxCompetitionTeamCompetitors = 999;
				Sport.UI.MessageBox.Warn("לא מוגדר חוק 'משתתפי קבוצה במקצוע' עבור תחרות זו, בדיקת תקנון לא פעילה", "קביעת תוצאות תחרות");
			}
			else
			{
				_minCompetitionTeamCompetitors = competitionTeamCompetitorsRule.Minimum;
				_maxCompetitionTeamCompetitors = competitionTeamCompetitorsRule.Maximum;
			}

			if (_sharedResults)
				ApplySharedResultsControls();

			if (_disqualifications)
				ApplyDisqualificationsControls();

			this.CancelButton = btnCancel;
		}

		public CompetitionResultForm(Sport.Championships.Competition competition)
			: this(competition, null)
		{
		}

		public CompetitionResultForm(Sport.Championships.Heat heat)
			: this(heat.Competition, heat)
		{
		}

		//////////// added by Lior /////////////////////////////////
		//private void btnUp_Click(object sender, System.EventArgs e)
		//{
		//
		//}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			//System.Collections.ArrayList tmp_Results = new System.Collections.ArrayList();

			if (gridResults.SelectedIndex < 0 || gridResults.SelectedIndex == (results.Count - 1))
				return;

			int selectedRow = gridResults.Selection.Rows[0];
			if (selectedRow < 0 || selectedRow >= (results.Count - 1))
				return;

			GenericEditDialog dialog = new GenericEditDialog("הכנס מספר חזה שיקבל את התוצאה המסומנת");
			GenericItem GenericNumericUpDown = new GenericItem("מספר", GenericItemType.Number, null, new object[] { 0, 10000 });
			dialog.Items.Add(GenericNumericUpDown);
			dialog.Confirmable = true;
			dialog.Items[0].Nullable = false;

			//dialog.Items[0].Focus(); // not working...

			int newPlayerNumber = 0;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				if (dialog.Items[0].Value != null)
				{
					newPlayerNumber = Convert.ToInt32(dialog.Items[0].Value);
				}
			}
			dialog.Dispose();

			if (newPlayerNumber <= 0)
				return;

			//search for player with given shirt number in the grid:
			if (results.FindIndex(r => r.PlayerNumber == newPlayerNumber) >= 0)
			{
				Sport.UI.MessageBox.Error("המתחרה כבר קיים ברשימה !", "מספר לא חוקי");
				return;
			}


			string newPlayerTeam = "";
			string newPlayerName = "";

			//look for the player in the whole group:
			Sport.Championships.CompetitionPlayer player = _competition.Group.Players[newPlayerNumber];
			if (player != null)
			{
				if (player.CompetitionTeam != null)
					newPlayerTeam = player.CompetitionTeam.Name;
				if (player.PlayerEntity != null)
					newPlayerName = player.PlayerEntity.Name;
			}

			int wrongResult = results[selectedRow].Result;
			results.Insert(selectedRow, new Row(newPlayerNumber, newPlayerTeam, newPlayerName, wrongResult));
			for (int i = selectedRow + 1; i < results.Count; i++)
			{
				int curResult = (i < (results.Count - 1)) ? (results[i + 1] as Row).Result : 0;
				results[i].Result = curResult;
			}
			
			gridResults.Refresh();
			gridResults.Update();

			tcNumber.Value = results.Last().PlayerNumber;
			SetSelectedRow(results.Count - 1);

			//player has been found or added.

			ticResult.Focus();
		}
		////////////////////////////////////////////////////////////

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			string strError = SetResults();
			if (strError.Length == 0)
			{
				//lower flag to allow close without message
				_blnChangesOccured = false;

				WriteCompetitionResultsToConfig(null);

				DialogResult = System.Windows.Forms.DialogResult.OK;
				if (parentView != null)
				{
					parentView.SortByPosition();
					//parentView.Invalidate();
					//parentView.Rebuild();
				}
				Close();
			}
			else
			{
				Sport.UI.MessageBox.Error("כשלון בשמירת תוצאות: " + "\n" + strError + "\n" + "אנא נסה שנית עוד מספר שניות", "שמירת תוצאות");
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private int _prevEnterPressPlayerNumber = 0;
		private void tcNumber_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			//Enter pressed?
			if (e.KeyChar != (char)System.Windows.Forms.Keys.Enter)
				return;

			//got anything?
			if (tcNumber.Text.Length == 0)
				return;

			//get shirt number:
			int playerNumber = (int)(double)tcNumber.Value;

			if (_sharedResultsModeActive)
			{
				//same number?
				if (_prevEnterPressPlayerNumber == playerNumber && playerNumber > 0)
				{
					FinishSharedResults();
					return;
				}
				_prevEnterPressPlayerNumber = playerNumber;
			}

			//try to find and select proper player:
			string strMessage = SelectPlayer(playerNumber);

			//successful?
			if (strMessage.Length == 0)
			{
				//current result?
				if (_customResultIndex >= 0)
				{
					int curSeconds = _customResults[_customResultIndex];
					SetPlayerResult(curSeconds * 1000);
					_customResultIndex++;
					if (_customResultIndex >= _customResults.Length)
					{
						pnlCurrentResult.Visible = false;
						_customResultIndex = -1;
					}
					else
					{
						ApplyCurrentResult();
						tcNumber.Focus();
					}

					int categoryID = _competition.Group.Phase.Championship.CategoryID;
					string strCurValue = Sport.Core.Configuration.ReadString("General",
						InputResultsForm.GetConfigKey(categoryID, "Results"));
					if (strCurValue != null && strCurValue.Length > 0)
					{
						string[] arrResults = strCurValue.Split('|');
						ArrayList arrNewResults = new ArrayList();
						for (int i = 1; i < arrResults.Length; i++)
							arrNewResults.Add(arrResults[i]);
						Sport.Core.Configuration.WriteString("General",
							InputResultsForm.GetConfigKey(categoryID, "Results"),
							String.Join("|", (string[])arrNewResults.ToArray(typeof(string))));
					}
				}
				else
				{
					//player has been found or added.
					if (_sharedResultsModeActive && gridResults.Selection != null && gridResults.Selection.Size == 1)
					{
						if (_currentSharedResultRow < 0)
							_currentSharedResultRow = gridResults.Selection.Rows[0];
						Row row = results[_currentSharedResultRow];
						bool alreadyShared = row.AddSharedNumber(playerNumber);
						if (!alreadyShared)
						{
							_sharedResultCurrentIndex++;
							if (_sharedResultCurrentIndex >= _maxSharedResults)
							{
								FinishSharedResults();
							}
							else
							{
								ApplySharedResult();
							}
						}
						if (row.SharedNumbersCount > 1)
							gridResults.Refresh();
						tbPlayer.Text = row.PlayerName;
					}
					else
					{
						ticResult.Focus();
					}
				}

				//raise change flag
				_blnChangesOccured = true;

				return;
			}

			//something was wrong...
			tcNumber.SelectionStart = 0;
			tcNumber.SelectionLength = tcNumber.Text.Length;

			//show alert:
			ignoreLostFocus = true;
			Sport.UI.MessageBox.Error(strMessage, "קביעת תוצאות תחרות");
			ignoreLostFocus = false;

			//put focus back:
			tcNumber.Focus();
		}

		private bool ignoreLostFocus = false;
		private void tcNumber_LostFocus(object sender, EventArgs e)
		{
			if (ignoreLostFocus)
				return;

			if (selectedRow == -1)
			{
				if (tcNumber.Text.Length != 0)
				{
					SelectPlayer((int)(double)tcNumber.Value);
				}
			}
		}

		private void tcNumber_GotFocus(object sender, EventArgs e)
		{
			tcNumber.SelectionStart = 0;
			tcNumber.SelectionLength = tcNumber.Text.Length;
		}

		private void tcNumber_TextChanged(object sender, EventArgs e)
		{
			if (!selectingPlayer)
			{
				tbTeam.Text = null;
				tbPlayer.Text = null;
				selectedRow = -1;
				ticResult.ReadOnly = true;
			}
		}

		private void ticResult_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter)
			{
				tcNumber.Focus();
			}
		}

		private void ticResult_GotFocus(object sender, EventArgs e)
		{
			ticResult.SelectionStart = 0;
		}

		private void ticResult_LostFocus(object sender, EventArgs e)
		{
			if (selectedRow != -1)
			{
				SetPlayerResult(ticResult.GetValue(_competition.ResultType.ValueFormatter));
			}
		}


		private void gridResults_SelectionChanged(object sender, EventArgs e)
		{
			bool enableDisqualifications = false;
			if (gridResults.Selection.Size == 1 && !selectingPlayer && gridResults.Selection.Rows.Length > 0)
			{
				selectedRow = gridResults.Selection.Rows[0];
				if (selectedRow >= 0 && selectedRow < results.Count)
				{
					enableDisqualifications = true;
					selectingPlayer = true;
					Row resultRow = results[selectedRow];
					tcNumber.Value = resultRow.PlayerNumber;
					tbTeam.Text = resultRow.TeamName;
					tbPlayer.Text = resultRow.PlayerName;
					int curResult = resultRow.Result;
					ticResult.SetValue(_competition.ResultType.ValueFormatter, curResult);
					selectingPlayer = false;
				}
			}

			if (_disqualifications)
				_btnApplyDisqualifications.Enabled = enableDisqualifications;
		}

		private int selectedRow = -1;

		private void SetPlayerResult(int result, bool blnSaveToConfig)
		{
			if (selectedRow != -1)
			{
				results[selectedRow].Result = result;
				gridResults.Refresh();

				if (blnSaveToConfig)
				{
					//save current state
					int playerNumber = results[selectedRow].PlayerNumber;
					Hashtable tblCurResults = GetResultsFromConfig();
					tblCurResults[playerNumber] = result;
					WriteCompetitionResultsToConfig(tblCurResults);
				}
			}
		}

		private void SetPlayerResult(int result)
		{
			SetPlayerResult(result, true);
		}

		private bool selectingPlayer = false;

		private string GetTeamName(int playerNumber)
		{
			//look for the player in the whole group:
			Sport.Championships.CompetitionPlayer player = _competition.Group.Players[playerNumber];
			
			//found the player?
			if (player != null && player.CompetitionTeam != null)
				return player.CompetitionTeam.Name;

			//did not find the player or team:
			return "";
		}

		private string SelectPlayer(int playerNumber)
		{
			if (playerNumber > 0)
			{
				selectedRow = -1;

				//shared?
				if (_sharedResultsModeActive && _currentSharedResultRow >= 0)
				{
					string playerTeamName = GetTeamName(playerNumber);
					if (playerTeamName == null || playerTeamName.Length == 0)
						return "מספר חזה לא מזוהה";
					string sharedResultsTeamName = results[_currentSharedResultRow].TeamName;
					if (playerTeamName != sharedResultsTeamName)
					{
						return "נא להזין מספר חזה של " + sharedResultsTeamName;
					}
					else
					{
						selectedRow = _currentSharedResultRow;
					}
				}
				
				if (selectedRow < 0)
				{
					//search for player with given shirt number in the grid:
					for (int n = 0; n < results.Count && selectedRow == -1; n++)
					{
						Row currentRow = results[n];
						if (currentRow.PlayerNumber == playerNumber)
						{
							selectedRow = n;
							break;
						}
					}
				}
				
				//player not here?
				if (selectedRow == -1)
				{
					//look for the player in the whole group:
					Sport.Championships.CompetitionPlayer player =
						_competition.Group.Players[playerNumber];
					
					//found the player?
					if (player != null)
					{
						if (player.CompetitionTeam != null)
						{
							//get competitor competitions and team competitors:
							int competitorCompetitions =
								_competition.Group.GetCompetitorCompetitions(playerNumber);
							int teamCompetitors =
								_competition.Group.GetTeamCompetitionCompetitors(player.CompetitionTeam, _competition.Index);
							
							Sport.Championships.CompetitionTeam playerTeam = player.CompetitionTeam;
							if (localTeamCompetitors[playerTeam] == null)
								localTeamCompetitors[playerTeam] = 0;
							
							//add team competitors that were added here and not yet saved
							teamCompetitors += (int)localTeamCompetitors[playerTeam];
							
							if (_competition == null || (_competition != null && !_competition.IsRelayRace()))
							{
								//maybe this player is already playing in maximum competitions?
								if (competitorCompetitions >= _maxCompetitorCompetitions)
									return "מתמודד זה כבר משתתף בכמות מקסימלית של מקצועות המותרת לפי התקנון";
								
								//maybe team already have maximum competitors?
								if (teamCompetitors >= _maxCompetitionTeamCompetitors)
									return "בית הספר כבר משתף את מירב המתמודדים המותרים לפי התקנון";
							}

							results.Add(new Row(playerNumber, playerTeam.Name, player.PlayerEntity == null ? null : player.PlayerEntity.Name, -1));
							selectedRow = results.Count - 1;
							localTeamCompetitors[playerTeam] = ((int)localTeamCompetitors[playerTeam]) + 1;
							gridResults.RefreshSource();
						}
					}
					else
					{
						//maybe offline?
						Sport.Entities.OfflineTeam[] arrOfflineTeams =
							_competition.Group.GetOfflineTeams();
						bool blnOffline = false;
						foreach (Sport.Entities.OfflineTeam offlineTeam in arrOfflineTeams)
						{
							int pNumFrom = offlineTeam.PlayerNumberFrom;
							int pNumTo = offlineTeam.PlayerNumberTo;
							if ((playerNumber >= pNumFrom) && (playerNumber <= pNumTo))
							{
								blnOffline = true;
								break;
							}
						}
						if (blnOffline)
							return "על מנת להוסיף תוצאה לקבוצה לא מקוונת אנא הוסף שחקן";
					}
				}
				
				//add "zombie" competitor if not found
				if (selectedRow < 0 && !_sharedResultsModeActive)
				{
					ignoreLostFocus = true;
					if (Sport.UI.MessageBox.Show("מספר חזה זה לא משוייך לקבוצה.\nהאם להוסיף שורה ריקה?", "הכנסת תוצאות", MessageBoxButtons.OKCancel,
						MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.OK)
					{
						results.Add(new Row(playerNumber, "", "", -1));
						selectedRow = results.Count - 1;
						gridResults.RefreshSource();
					}
					ignoreLostFocus = false;
				}

				//got anything?
				if (selectedRow >= 0)
				{
					//done.
					SetSelectedRow(selectedRow);
					return "";
				} //end if player is in this grid.
			}

			//getting here means player does not exist.
			return "מספר חזה לא מזוהה";
		} //end function SelectPlayer

		private void SetSelectedRow(int selectedRow)
		{
			if (selectedRow < 0 || selectedRow >= results.Count)
				return;

			//raise global flag:
			selectingPlayer = true;

			//select player and scroll to its row
			gridResults.SelectRow(selectedRow);
			gridResults.ScrollToRow(selectedRow);

			//set player and team text:
			tbTeam.Text = results[selectedRow].TeamName;
			tbPlayer.Text = results[selectedRow].PlayerName;

			//set result:
			ticResult.SetValue(_competition.ResultType.ValueFormatter, results[selectedRow].Result);

			//let user change the result:
			ticResult.ReadOnly = false;

			//lower global flag:
			selectingPlayer = false;
		}

		private class Row
		{
			public int PlayerNumber;
			public string TeamName;
			public int Result;
			public int LastResultDisqualifications;
			public int TotalDisqualifications;
			public string Wind;
			private string _playerName = "";
			private List<int> sharedNumbers = null;
			
			public Row(int playerNumber, string teamName, string playerName, int result)
			{
				PlayerNumber = playerNumber;
				TeamName = teamName;
				_playerName = playerName;
				Result = result;
				LastResultDisqualifications = 0;
				TotalDisqualifications = 0;
				Wind = "";
			}

			public string PlayerName
			{
				get
				{
					if (this.SharedNumbersCount > 1)
						return "[משותף] " + string.Join(", ", this.SharedNumbers);
					return _playerName;
				}
				set
				{
					_playerName = value;
				}
			}

			public int SharedNumbersCount
			{
				get
				{
					return (sharedNumbers == null) ? 0 : sharedNumbers.Count;
				}
			}

			public int[] SharedNumbers
			{
				get
				{
					return (sharedNumbers == null) ? new int[] { } : sharedNumbers.ToArray();
				}
			}

			public bool AddSharedNumber(int playerNumber)
			{
				if (sharedNumbers == null)
					sharedNumbers = new List<int>();
				int index = sharedNumbers.IndexOf(playerNumber);
				bool exists = false;
				if (index >= 0)
				{
					exists = true;
					sharedNumbers.RemoveAt(index);
				}
				sharedNumbers.Add(playerNumber);
				return exists;
			}
		}

		private List<Row> results = null;
		private Hashtable localTeamCompetitors = new Hashtable();

		private void ReadResults()
		{
			results = new List<Row>();
			if (_competition == null)
				return;
			System.Collections.ArrayList arrCompetitors =
				new System.Collections.ArrayList();
			if (_competition.Competitors != null)
				arrCompetitors.AddRange(_competition.Competitors);
			arrCompetitors.AddRange(_competition.GetOfflinePlayers());
			if (arrCompetitors.Count == 0)
				return;
			//System.Collections.Hashtable tblOfflineTeams = 
			//	new System.Collections.Hashtable();
			foreach (object oComp in arrCompetitors)
			{
				int curShirtNumber = -1;
				string curPlayerName = null;
				string curTeamName = null;
				int curResult = 0;
				int lastResultDisqualifications = 0;
				int totalDisqualifications = 0;
				int[] curSharedResultNumbers = null;
				string curWind = "";
				if (oComp is Sport.Championships.Competitor)
				{
					Sport.Championships.Competitor competitor =
						(Sport.Championships.Competitor)oComp;
					if (_heat == null || competitor.Heat == _heat.Index)
					{
						curShirtNumber = competitor.PlayerNumber;
						if ((competitor.Player != null) && (competitor.Player.PlayerEntity != null))
							curPlayerName = competitor.Player.PlayerEntity.Name;
						if ((competitor.Team >= 0) && (competitor.Team < _competition.Group.Teams.Count))
						{
							Sport.Championships.CompetitionTeam compTeam =
								_competition.Group.Teams[competitor.Team];
							if (compTeam != null)
							{
								curTeamName = compTeam.Name;
							}
						}
						curResult = competitor.Result;
						curSharedResultNumbers = competitor.SharedResultNumbers;
						lastResultDisqualifications = competitor.LastResultDisqualifications;
						totalDisqualifications = competitor.TotalDisqualifications;
						curWind = competitor.Wind;
					}
				}
				else if (oComp is Sport.Entities.OfflinePlayer)
				{
					Sport.Entities.OfflinePlayer oPlayer =
						(Sport.Entities.OfflinePlayer)oComp;
					curShirtNumber = oPlayer.ShirtNumber;
					curPlayerName = oPlayer.ToString();
					if (oPlayer.Team != null)
						curTeamName = oPlayer.Team.TeamName();
					else if (oPlayer.OfflineTeam != null)
					{
						//int ID = oPlayer.OfflineTeam.OfflineID;
						//Sport.Entities.OfflineTeam oTeam = tblOfflineTeams[
						curTeamName = oPlayer.OfflineTeam.ToString();
					}
					curResult = oPlayer.Result;
				}
				if (curShirtNumber < 0)
					continue;
				Row oRow = new Row(curShirtNumber, curTeamName, curPlayerName, curResult);
				if (curSharedResultNumbers != null)
					curSharedResultNumbers.ToList().ForEach(n => oRow.AddSharedNumber(n));
				oRow.LastResultDisqualifications = lastResultDisqualifications;
				oRow.TotalDisqualifications = totalDisqualifications;
				oRow.Wind = curWind;
				results.Add(oRow);
			}
		}

		public string SetResults()
		{
			Sport.Championships.CompetitorResult[] competitorsResults = results.ConvertAll(r =>
			{
				return new Sport.Championships.CompetitorResult(r.PlayerNumber, r.Result, r.SharedNumbers, r.LastResultDisqualifications, r.TotalDisqualifications, r.Wind);
			}).ToArray();
			return _competition.SetResults(_heat == null ? -1 : _heat.Index, competitorsResults);
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

		private Sport.UI.Controls.TextControl _windControl = null;

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			if (row < results.Count && _wind && field == GetFieldCount(row) - 1)
			{
				Row oRow = results[row];
				if (_windControl == null)
				{
					_windControl = new TextControl();
				}
				_windControl.Tag = row;
				_windControl.Value = oRow.Wind;
				return _windControl;
			}
			return null;
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
			if (control == _windControl)
			{
				int row = (int)_windControl.Tag;
				Row oRow = results[row];
				oRow.Wind = _windControl.Value.ToString();
			}
		}

		public int GetRowCount()
		{
			return results.Count;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public string GetText(int row, int field)
		{
			if (row < results.Count)
			{
				Row oRow = results[row];
				bool gotSharedResult = (oRow.SharedNumbersCount > 1);
				if (field == _disqualificationsField)
					return oRow.TotalDisqualifications > 0 ? string.Format("{0} ({1})", oRow.LastResultDisqualifications, oRow.TotalDisqualifications) : "";
				if (field == _windField)
					return oRow.Wind;
				switch (field)
				{
					case (0):
						return (gotSharedResult) ? string.Join(", ", oRow.SharedNumbers) : oRow.PlayerNumber.ToString();
					case (1):
						return oRow.TeamName;
					case (2):
						return (gotSharedResult) ? "[תוצאה משותפת]" : oRow.PlayerName;
					case (3):
						return _competition.ResultType == null ? null :
							_competition.ResultType.FormatResult(oRow.Result);
				}
			}

			return null;
		}

		public int GetFieldCount(int row)
		{
			int fieldCount = 4;
			if (_disqualifications)
				fieldCount++;
			if (_wind)
				fieldCount++;
			return fieldCount;
		}

		#endregion

		private void CompetitionResultForm_Load(object sender, System.EventArgs e)
		{
			this.Text = "הכנסת תוצאות" + " - " + _competition.Group.Phase.Championship.Name + " - " + _competition.Name;
			if ((_competition.GeneralData != null) && (_competition.GeneralData.ScoreIsRank))
			{
				gbResults.Visible = true;
				this.Height += (gbResults.Height + 50);
			}

			Hashtable tblSavedResults = GetResultsFromConfig();
			if (tblSavedResults.Count > 0)
			{
				foreach (int playerNumber in tblSavedResults.Keys)
				{
					int curResult = (int)tblSavedResults[playerNumber];
					if (SelectPlayer(playerNumber).Length == 0)
					{
						SetPlayerResult(curResult, false);
						_blnChangesOccured = true;
					}
				}
			}
		}

		private InputResultsForm resultForm = null;
		private void btnInputResults_Click(object sender, System.EventArgs e)
		{
			if (!ResultFormOpened())
			{
				resultForm = new InputResultsForm(_competition);
			}
			else
			{
			}
			resultForm.ParentResultForm = this;
			ShowResultForm();
		}

		public void SetCustomResults(int[] customResults)
		{
			_customResults = customResults;
			if ((_customResults != null) && (_customResults.Length > 0))
			{
				btnApplyResults.Enabled = true;
				btnApplyResults.Focus();
			}
			else
			{
				btnApplyResults.Enabled = false;
			}
		}

		private bool ResultFormOpened()
		{
			return (resultForm != null && !resultForm.IsDisposed && resultForm.DialogResult == System.Windows.Forms.DialogResult.None);
		}

		private void ShowResultForm()
		{
			resultForm.WindowState = System.Windows.Forms.FormWindowState.Normal;
			resultForm.Show();
			resultForm.Focus();
		}

		private void btnApplyResults_Click(object sender, System.EventArgs e)
		{
			//got anything?
			if ((_customResults == null) || (_customResults.Length == 0))
				return;
			pnlCurrentResult.Visible = true;
			btnApplyResults.Enabled = false;
			_customResultIndex = 0;
			ApplyCurrentResult();
			tcNumber.Focus();
		}

		private void ApplyCurrentResult()
		{
			int seconds = _customResults[_customResultIndex];
			int minutes = (int)(((double)seconds) / ((double)60));
			seconds %= 60;
			lbCurrentResult.Text = minutes.ToString().PadLeft(2, '0') + ":" +
				seconds.ToString().PadLeft(2, '0');
		}

		private void CompetitionResultForm_Closed(object sender, System.EventArgs e)
		{
			if (ResultFormOpened())
				resultForm.Close();

			if (parentView != null && !parentView.Closing())
			{
				//Sport.UI.ViewManager.SelectedView = parentView;
				parentView.Focus();
			}
		}

		private void CompetitionResultForm_Activated(object sender, System.EventArgs e)
		{
			if (ResultFormOpened())
				ShowResultForm();
		}

		private void CompetitionResultForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_blnChangesOccured && !_forceClose)
				e.Cancel = (MessageBox.Show(this, "סגירת חלון זה תבטל את השינויים שנעשו. האם להמשיך?", "הכנסת תוצאות", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) == System.Windows.Forms.DialogResult.Cancel);

			if (!e.Cancel)
				WriteCompetitionResultsToConfig(null);
		}

		private string GetConfigKey()
		{
			int categoryID = _competition.Group.Phase.Championship.CategoryID;
			string key = InputResultsForm.GetConfigKey(categoryID, "PlayerResults");
			return key;
		}

		private Hashtable GetResultsFromConfig()
		{
			string strCurrentResults = Sport.Core.Configuration.ReadString("General", GetConfigKey());
			if (strCurrentResults == null)
				strCurrentResults = "";

			Hashtable tblResults = new Hashtable();

			if (strCurrentResults.Length > 0)
			{
				string[] arrPlayerResults = strCurrentResults.Split('|');
				for (int i = 0; i < arrPlayerResults.Length; i++)
				{
					string strPlayerResult = arrPlayerResults[i];
					string[] arrTemp = strPlayerResult.Split(',');
					if (arrTemp.Length == 2)
					{
						int curPlayerNumber = Sport.Common.Tools.CIntDef(arrTemp[0], -1);
						int curResult = Sport.Common.Tools.CIntDef(arrTemp[1], 0);
						if (curPlayerNumber >= 0)
						{
							tblResults[curPlayerNumber] = curResult;
						}
					}
				}
			}

			return tblResults;
		}

		private void WriteCompetitionResultsToConfig(Hashtable tblResults)
		{
			StringBuilder sb = new StringBuilder();
			if (tblResults != null)
			{
				foreach (int playerNumber in tblResults.Keys)
				{
					sb.Append(playerNumber).Append(",").Append(tblResults[playerNumber]).Append("|");
				}
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 1, 1);
			Sport.Core.Configuration.WriteString("General", GetConfigKey(), sb.ToString());
		}
	}
}
