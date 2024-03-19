using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Championships;
using Sport.Rulesets;
using Sport.Rulesets.Rules;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for MatchForm.
	/// </summary>
	public class MatchForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelTime;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.DateTimePicker dtpTime;
		private System.Windows.Forms.GroupBox gbLocation;
		private System.Windows.Forms.Label labelFacility;
		private Sport.UI.Controls.ButtonBox bbFacility = null;
		private System.Windows.Forms.Label label1;
		private Sport.UI.Controls.NullComboBox cbCourt;
		private System.Windows.Forms.GroupBox gbScore;
		private System.Windows.Forms.NumericUpDown nudTeamBScore;
		private System.Windows.Forms.NumericUpDown nudTeamAScore;
		private System.Windows.Forms.Label labelTeamB;
		private System.Windows.Forms.Label labelTeamA;

		private Sport.Championships.Match _match;
		private Sport.Rulesets.Rules.TechnicalResult technicalResult;

		private System.Windows.Forms.Button btnResult;
		private System.Windows.Forms.CheckBox cbTechnicalA;
		private System.Windows.Forms.CheckBox cbTechnicalB;
		private System.Windows.Forms.Button btnParts;
		private Sport.UI.Controls.Grid gdFunctionaries;
		private FunctionaryGridSource _funcSource = null;

		private bool _result;

		private Sport.Championships.GameResult gameResult = null;
		private System.Windows.Forms.NumericUpDown nudNumber;
		private System.Windows.Forms.Label labelNumber;
		private System.Windows.Forms.GroupBox gbFunctionaries;
		private System.Windows.Forms.Label lbFuncError;

		private Sport.Rulesets.Rules.GameStructure gameStructure;
		private Button btnRemoveLocation;
		private Button btnRemoveFunctionaries;
		private Sport.UI.EntitySelectionDialog facilityDialog = null;

		public MatchForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_match = null;
		}

		public MatchForm(Sport.Championships.Match match)
			: this()
		{
			_match = match;


			Sport.Championships.Phase phase = _match.Cycle.Round.Group.Phase;

			technicalResult = null;
			gameStructure = null;

			object objTechnilcalResult = null;
			object objGameStructure = null;

			if (Sport.Core.Session.Connected)
			{
				objTechnilcalResult = phase.Championship.ChampionshipCategory.GetRule(typeof(Sport.Rulesets.Rules.TechnicalResult));
				objGameStructure = phase.Championship.ChampionshipCategory.GetRule(typeof(Sport.Rulesets.Rules.GameStructure));
			}
			else
			{
				int categoryID = phase.Championship.CategoryID;
				objTechnilcalResult = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(Sport.Rulesets.Rules.TechnicalResultRule),
					categoryID, -1);
				objGameStructure = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(Sport.Rulesets.Rules.GameStructureRule),
					categoryID, -1);
			}

			if (objTechnilcalResult is Sport.Rulesets.Rules.TechnicalResult)
			{
				technicalResult = (Sport.Rulesets.Rules.TechnicalResult)objTechnilcalResult;
			}
			else
			{
				if (objTechnilcalResult != null)
					Sport.UI.MessageBox.Warn("�����: ��� ����� ����� ��� �� �� ����� ����", "����� ����");
			}

			if (objGameStructure is Sport.Rulesets.Rules.GameStructure)
			{
				gameStructure = (Sport.Rulesets.Rules.GameStructure)objGameStructure;
			}
			else
			{
				if (objGameStructure != null)
					Sport.UI.MessageBox.Warn("�����: ��� ���� ���� ��� �� �� ����� ����", "����� ����");
			}

			btnResult.Enabled = true; //(phase.Status != Status.Planned);

			if (gameStructure == null)
			{
				btnParts.Visible = false;
			}
			else
			{
				if (gameStructure.GameType == Sport.Rulesets.Rules.GameType.Duration)
					btnParts.Text = Sport.Rulesets.Rules.GameStructure.PartsName;
				else
					btnParts.Text = Sport.Rulesets.Rules.GameStructure.SetsName;
			}

			Text = match.ToString();

			if (Sport.Common.Tools.IsMinDate(match.Time))
			{
				dtpTime.Checked = false;
			}
			else
			{
				dtpTime.Checked = true;
				dtpTime.Value = match.Time;
			}

			labelTeamA.Text = match.GetTeamAName();
			labelTeamB.Text = match.GetTeamBName();
			_result = true;
			switch (match.Outcome)
			{
				case (MatchOutcome.None):
					_result = false;
					break;
				case (MatchOutcome.TechnicalA):
					cbTechnicalA.Checked = true;
					break;
				case (MatchOutcome.TechnicalB):
					cbTechnicalB.Checked = true;
					break;
				default:
					nudTeamAScore.Value = (int)match.TeamAScore;
					nudTeamBScore.Value = (int)match.TeamBScore;
					break;
			}

			SetResultControls();

			nudNumber.Value = match.Number;

			Views.FacilitiesTableView facilityTableView = new Views.FacilitiesTableView();
			facilityTableView.State[Sport.UI.View.SelectionDialog] = "1";
			facilityDialog = new Sport.UI.EntitySelectionDialog(facilityTableView);
			Sport.Entities.Championship cc =
				_match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship;
			if (cc.Region != null)
				facilityDialog.View.State[Sport.Entities.Region.TypeName] = cc.Region.Id.ToString();
			bbFacility.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(
				facilityDialog.ValueSelector);

			if (_match.Facility != null)
			{
				if (_match.Facility.Region != null)
				{
					facilityDialog.View.State[Sport.Entities.Region.TypeName] =
						_match.Facility.Region.Id.ToString();
				}
				bbFacility.Value = _match.Facility.Entity;
				SetCourts();
				if (_match.Court != null)
					cbCourt.SelectedItem = match.Court;
			}
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
			this.labelTime = new System.Windows.Forms.Label();
			this.dtpTime = new System.Windows.Forms.DateTimePicker();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbLocation = new System.Windows.Forms.GroupBox();
			this.btnRemoveLocation = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.cbCourt = new Sport.UI.Controls.NullComboBox();
			this.labelFacility = new System.Windows.Forms.Label();
			this.bbFacility = new Sport.UI.Controls.ButtonBox();
			this.gbScore = new System.Windows.Forms.GroupBox();
			this.btnParts = new System.Windows.Forms.Button();
			this.cbTechnicalB = new System.Windows.Forms.CheckBox();
			this.cbTechnicalA = new System.Windows.Forms.CheckBox();
			this.btnResult = new System.Windows.Forms.Button();
			this.nudTeamBScore = new System.Windows.Forms.NumericUpDown();
			this.nudTeamAScore = new System.Windows.Forms.NumericUpDown();
			this.labelTeamB = new System.Windows.Forms.Label();
			this.labelTeamA = new System.Windows.Forms.Label();
			this.nudNumber = new System.Windows.Forms.NumericUpDown();
			this.labelNumber = new System.Windows.Forms.Label();
			this.gbFunctionaries = new System.Windows.Forms.GroupBox();
			this.btnRemoveFunctionaries = new System.Windows.Forms.Button();
			this.lbFuncError = new System.Windows.Forms.Label();
			this.gdFunctionaries = new Sport.UI.Controls.Grid();
			this.gbLocation.SuspendLayout();
			this.gbScore.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudTeamBScore)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTeamAScore)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).BeginInit();
			this.gbFunctionaries.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Location = new System.Drawing.Point(565, 14);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(54, 20);
			this.labelTime.TabIndex = 0;
			this.labelTime.Text = "����:";
			// 
			// dtpTime
			// 
			this.dtpTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.dtpTime.CustomFormat = "dd/MM/yyyy HH:mm";
			this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpTime.Location = new System.Drawing.Point(379, 12);
			this.dtpTime.Name = "dtpTime";
			this.dtpTime.ShowCheckBox = true;
			this.dtpTime.Size = new System.Drawing.Size(180, 24);
			this.dtpTime.TabIndex = 1;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnOK.Location = new System.Drawing.Point(11, 440);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(105, 28);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "�����";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(134, 440);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(105, 28);
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "�����";
			// 
			// gbLocation
			// 
			this.gbLocation.Controls.Add(this.btnRemoveLocation);
			this.gbLocation.Controls.Add(this.label1);
			this.gbLocation.Controls.Add(this.cbCourt);
			this.gbLocation.Controls.Add(this.labelFacility);
			this.gbLocation.Controls.Add(this.bbFacility);
			this.gbLocation.Location = new System.Drawing.Point(258, 42);
			this.gbLocation.Name = "gbLocation";
			this.gbLocation.Size = new System.Drawing.Size(358, 191);
			this.gbLocation.TabIndex = 14;
			this.gbLocation.TabStop = false;
			this.gbLocation.Text = "�����";
			// 
			// btnRemoveLocation
			// 
			this.btnRemoveLocation.Location = new System.Drawing.Point(6, 23);
			this.btnRemoveLocation.Name = "btnRemoveLocation";
			this.btnRemoveLocation.Size = new System.Drawing.Size(338, 29);
			this.btnRemoveLocation.TabIndex = 19;
			this.btnRemoveLocation.Text = "��� �����";
			this.btnRemoveLocation.Click += new System.EventHandler(this.btnRemoveLocation_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(291, 126);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 20);
			this.label1.TabIndex = 7;
			this.label1.Text = "����:";
			// 
			// cbCourt
			// 
			this.cbCourt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbCourt.Location = new System.Drawing.Point(6, 146);
			this.cbCourt.Name = "cbCourt";
			this.cbCourt.Size = new System.Drawing.Size(341, 25);
			this.cbCourt.Sorted = true;
			this.cbCourt.TabIndex = 6;
			// 
			// labelFacility
			// 
			this.labelFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFacility.Location = new System.Drawing.Point(291, 67);
			this.labelFacility.Name = "labelFacility";
			this.labelFacility.Size = new System.Drawing.Size(56, 19);
			this.labelFacility.TabIndex = 5;
			this.labelFacility.Text = "����:";
			// 
			// bbFacility
			// 
			this.bbFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bbFacility.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.bbFacility.Location = new System.Drawing.Point(6, 90);
			this.bbFacility.Name = "bbFacility";
			this.bbFacility.Size = new System.Drawing.Size(341, 27);
			this.bbFacility.TabIndex = 4;
			this.bbFacility.Value = null;
			this.bbFacility.ValueSelector = null;
			this.bbFacility.ValueChanged += new System.EventHandler(this.bbFacility_ValueChanged);
			// 
			// gbScore
			// 
			this.gbScore.Controls.Add(this.btnParts);
			this.gbScore.Controls.Add(this.cbTechnicalB);
			this.gbScore.Controls.Add(this.cbTechnicalA);
			this.gbScore.Controls.Add(this.btnResult);
			this.gbScore.Controls.Add(this.nudTeamBScore);
			this.gbScore.Controls.Add(this.nudTeamAScore);
			this.gbScore.Controls.Add(this.labelTeamB);
			this.gbScore.Controls.Add(this.labelTeamA);
			this.gbScore.Location = new System.Drawing.Point(11, 10);
			this.gbScore.Name = "gbScore";
			this.gbScore.Size = new System.Drawing.Size(235, 223);
			this.gbScore.TabIndex = 15;
			this.gbScore.TabStop = false;
			this.gbScore.Text = "�����";
			// 
			// btnParts
			// 
			this.btnParts.Location = new System.Drawing.Point(11, 193);
			this.btnParts.Name = "btnParts";
			this.btnParts.Size = new System.Drawing.Size(213, 24);
			this.btnParts.TabIndex = 21;
			this.btnParts.Text = "������";
			this.btnParts.Click += new System.EventHandler(this.btnParts_Click);
			// 
			// cbTechnicalB
			// 
			this.cbTechnicalB.Location = new System.Drawing.Point(123, 171);
			this.cbTechnicalB.Name = "cbTechnicalB";
			this.cbTechnicalB.Size = new System.Drawing.Size(101, 20);
			this.cbTechnicalB.TabIndex = 20;
			this.cbTechnicalB.Text = "����";
			this.cbTechnicalB.CheckedChanged += new System.EventHandler(this.cbTechnicalB_CheckedChanged);
			// 
			// cbTechnicalA
			// 
			this.cbTechnicalA.Location = new System.Drawing.Point(123, 103);
			this.cbTechnicalA.Name = "cbTechnicalA";
			this.cbTechnicalA.Size = new System.Drawing.Size(101, 20);
			this.cbTechnicalA.TabIndex = 19;
			this.cbTechnicalA.Text = "����";
			this.cbTechnicalA.CheckedChanged += new System.EventHandler(this.cbTechnicalA_CheckedChanged);
			// 
			// btnResult
			// 
			this.btnResult.Location = new System.Drawing.Point(11, 22);
			this.btnResult.Name = "btnResult";
			this.btnResult.Size = new System.Drawing.Size(213, 29);
			this.btnResult.TabIndex = 18;
			this.btnResult.Text = "���� �����";
			this.btnResult.Click += new System.EventHandler(this.btnResult_Click);
			// 
			// nudTeamBScore
			// 
			this.nudTeamBScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudTeamBScore.Location = new System.Drawing.Point(11, 165);
			this.nudTeamBScore.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nudTeamBScore.Name = "nudTeamBScore";
			this.nudTeamBScore.Size = new System.Drawing.Size(101, 24);
			this.nudTeamBScore.TabIndex = 17;
			// 
			// nudTeamAScore
			// 
			this.nudTeamAScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudTeamAScore.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.nudTeamAScore.Location = new System.Drawing.Point(11, 109);
			this.nudTeamAScore.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.nudTeamAScore.Name = "nudTeamAScore";
			this.nudTeamAScore.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.nudTeamAScore.Size = new System.Drawing.Size(101, 24);
			this.nudTeamAScore.TabIndex = 16;
			// 
			// labelTeamB
			// 
			this.labelTeamB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeamB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelTeamB.Location = new System.Drawing.Point(11, 137);
			this.labelTeamB.Name = "labelTeamB";
			this.labelTeamB.Size = new System.Drawing.Size(213, 39);
			this.labelTeamB.TabIndex = 15;
			this.labelTeamB.Text = "label2";
			// 
			// labelTeamA
			// 
			this.labelTeamA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTeamA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelTeamA.Location = new System.Drawing.Point(11, 69);
			this.labelTeamA.Name = "labelTeamA";
			this.labelTeamA.Size = new System.Drawing.Size(213, 39);
			this.labelTeamA.TabIndex = 14;
			this.labelTeamA.Text = "label2";
			// 
			// nudNumber
			// 
			this.nudNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudNumber.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.nudNumber.Location = new System.Drawing.Point(258, 12);
			this.nudNumber.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
			this.nudNumber.Name = "nudNumber";
			this.nudNumber.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.nudNumber.Size = new System.Drawing.Size(51, 24);
			this.nudNumber.TabIndex = 17;
			// 
			// labelNumber
			// 
			this.labelNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNumber.Location = new System.Drawing.Point(318, 12);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(55, 19);
			this.labelNumber.TabIndex = 18;
			this.labelNumber.Text = "����:";
			// 
			// gbFunctionaries
			// 
			this.gbFunctionaries.Controls.Add(this.btnRemoveFunctionaries);
			this.gbFunctionaries.Controls.Add(this.lbFuncError);
			this.gbFunctionaries.Controls.Add(this.gdFunctionaries);
			this.gbFunctionaries.Location = new System.Drawing.Point(11, 237);
			this.gbFunctionaries.Name = "gbFunctionaries";
			this.gbFunctionaries.Size = new System.Drawing.Size(605, 196);
			this.gbFunctionaries.TabIndex = 19;
			this.gbFunctionaries.TabStop = false;
			this.gbFunctionaries.Text = "���� �������";
			// 
			// btnRemoveFunctionaries
			// 
			this.btnRemoveFunctionaries.Location = new System.Drawing.Point(340, 33);
			this.btnRemoveFunctionaries.Name = "btnRemoveFunctionaries";
			this.btnRemoveFunctionaries.Size = new System.Drawing.Size(259, 29);
			this.btnRemoveFunctionaries.TabIndex = 20;
			this.btnRemoveFunctionaries.Text = "��� ���� �������";
			this.btnRemoveFunctionaries.Click += new System.EventHandler(this.btnRemoveFunctionaries_Click);
			// 
			// lbFuncError
			// 
			this.lbFuncError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbFuncError.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.lbFuncError.ForeColor = System.Drawing.Color.Red;
			this.lbFuncError.Location = new System.Drawing.Point(127, 33);
			this.lbFuncError.Name = "lbFuncError";
			this.lbFuncError.Size = new System.Drawing.Size(471, 49);
			this.lbFuncError.TabIndex = 6;
			this.lbFuncError.Text = "�����";
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
			this.gdFunctionaries.HeaderHeight = 19;
			this.gdFunctionaries.HorizontalLines = true;
			this.gdFunctionaries.Location = new System.Drawing.Point(11, 77);
			this.gdFunctionaries.Name = "gdFunctionaries";
			this.gdFunctionaries.SelectedIndex = -1;
			this.gdFunctionaries.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gdFunctionaries.SelectOnSpace = false;
			this.gdFunctionaries.ShowCheckBoxes = false;
			this.gdFunctionaries.ShowRowNumber = false;
			this.gdFunctionaries.Size = new System.Drawing.Size(584, 100);
			this.gdFunctionaries.Source = gridDefaultSource1;
			this.gdFunctionaries.TabIndex = 6;
			this.gdFunctionaries.VerticalLines = true;
			this.gdFunctionaries.VisibleRow = 0;
			// 
			// MatchForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 17);
			this.ClientSize = new System.Drawing.Size(630, 478);
			this.Controls.Add(this.gbFunctionaries);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.nudNumber);
			this.Controls.Add(this.gbScore);
			this.Controls.Add(this.gbLocation);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.dtpTime);
			this.Controls.Add(this.labelTime);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "MatchForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MatchForm";
			this.Load += new System.EventHandler(this.MatchForm_Load);
			this.gbLocation.ResumeLayout(false);
			this.gbScore.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudTeamBScore)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudTeamAScore)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).EndInit();
			this.gbFunctionaries.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetCourts()
		{
			cbCourt.Items.Clear();

			Sport.Data.Entity ent = (Sport.Data.Entity)bbFacility.Value;
			Sport.Entities.Facility facility = null;
			if (ent != null)
			{
				facility = new Sport.Entities.Facility(ent);
			}

			if (facility != null)
			{
				MatchChampionship champ = _match.Cycle.Round.Group.Phase.Championship;
				Sport.Entities.Sport sport = champ.ChampionshipCategory.Championship.Sport;
				cbCourt.Items.AddRange(facility.GetCourts(sport));
			}

			cbCourt.Items.Add(Sport.UI.Controls.NullComboBox.Null);
		}

		private void cbFacility_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SetCourts();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (gameResult == null)
			{
				if (Sport.Common.Tools.IsEmpty(_match.PartsResult))
					gameResult = new Sport.Championships.GameResult();
				else
					gameResult = new Sport.Championships.GameResult(_match.PartsResult);
			}

			Sport.Entities.Facility facility = null;
			if (bbFacility.Value != null)
				facility = new Sport.Entities.Facility(bbFacility.Value as Sport.Data.Entity);
			Sport.Entities.Court court = cbCourt.SelectedItem as Sport.Entities.Court;
			DateTime matchDate =
				(dtpTime.Checked) ? dtpTime.Value : DateTime.MinValue;
			int number = (int)nudNumber.Value;

			bool blnSet = false;

			double teamAScore = 6;
			double teamBScore = 6;
			string partsResult = null;
			Sport.Championships.MatchOutcome mo = Sport.Championships.MatchOutcome.None;
			if (_result)
			{
				teamAScore = (double)nudTeamAScore.Value;
				teamBScore = (double)nudTeamBScore.Value;

				if (gameResult == null)
				{
					partsResult = _match.PartsResult;
				}
				else
				{
					partsResult = gameResult.ToString();
				}


				if (cbTechnicalA.Checked)
				{
					mo = Sport.Championships.MatchOutcome.TechnicalA;
				}
				else if (cbTechnicalB.Checked)
				{
					mo = Sport.Championships.MatchOutcome.TechnicalB;
				}
				else
				{
					double overrideScore_A, overrideScore_B;
					if (TryOverrideScore(partsResult, out overrideScore_A, out overrideScore_B))
					{
						teamAScore = overrideScore_A;
						teamBScore = overrideScore_B;
					}
					if (teamAScore > teamBScore)
						mo = Sport.Championships.MatchOutcome.WinA;
					else if (teamAScore < teamBScore)
						mo = Sport.Championships.MatchOutcome.WinB;
					else
						mo = Sport.Championships.MatchOutcome.Tie;
				}
			}

			bool bClose = false;
			int[] arrFunctionaries = new int[0];
			if (_funcSource != null)
			{
				Functionaries func = _funcSource.GetFunctionaries();
				arrFunctionaries = new int[func.Fields.Count];
				for (int n = 0; n < func.Fields.Count; n++)
				{
					arrFunctionaries[n] = func.Fields[n].Id;
				}
			}

			try
			{
				blnSet = _match.Set(number, matchDate, facility, court, arrFunctionaries);
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("Phase not started") >= 0)
				{
					Sport.UI.MessageBox.Error("�� ���� ����� �� ������: �� ������ ��� �����", "����� �����");
					blnSet = false;
				}
				else
				{
					throw ex;
				}
			}
			if (blnSet)
			{
				if ((this.btnResult.Enabled == false) ||
					((mo == _match.Outcome) && (teamAScore == _match.TeamAScore) &&
					(teamBScore == _match.TeamBScore) &&
					(partsResult == _match.PartsResult)))
				{
					bClose = true;
				}
				else
				{
					try
					{
						blnSet = _match.SetResult(mo, teamAScore, teamBScore, partsResult);
					}
					catch (Exception ex)
					{
						if (ex.Message.IndexOf("Phase not started") >= 0)
						{
							Sport.UI.MessageBox.Error("�� ���� ����� �� ������: �� ������ ��� �����", "����� �����");
							blnSet = false;
						}
						else
						{
							throw ex;
						}
					}
					if (blnSet)
					{
						bClose = true;
					}
					else
					{
						Sport.UI.MessageBox.Show("������ ������ ������ ����");
					}
				}
			}
			else
			{
				Sport.UI.MessageBox.Show("������ ������ ����� ����");
			}

			if (bClose)
			{
				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
		}

		private bool TryOverrideScore(string partsResult, out double scoreA, out double scoreB)
		{
			scoreA = 0;
			scoreB = 0;

			PartScore partScoreRule = _match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.GetRule(typeof(PartScore)) as PartScore;
			if (partScoreRule == null || partScoreRule.Win == 0 || partsResult == null || partsResult.Length == 0)
				return false;

			GameResult gameResult = new GameResult(partsResult);
			if (gameResult == null || gameResult.Games == 0 || gameResult[0].Count == 0)
				return false;

			GameResult.PartResultCollection resultCollection = gameResult[0];
			for (int i = 0; i < resultCollection.Count; i++)
			{
				int currentPartResult_A = resultCollection[i][0];
				int currentPartResult_B = resultCollection[i][1];
				if (currentPartResult_A > currentPartResult_B)
				{
					//WinA
					scoreA += partScoreRule.Win;
					scoreB += partScoreRule.Lose;
				}
				else if (currentPartResult_B > currentPartResult_A)
				{
					//WinB
					scoreA += partScoreRule.Lose;
					scoreB += partScoreRule.Win;
				}
				else
				{
					//Tie
					scoreA += partScoreRule.Draw;
					scoreB += partScoreRule.Draw;
				}
			}

			return true;
		}

		private void SetResultControls()
		{
			if (_result)
			{
				btnResult.Text = "��� �����";
				cbTechnicalA.Enabled = true;
				cbTechnicalB.Enabled = true;
				nudTeamAScore.Enabled = true;
				nudTeamBScore.Enabled = true;
				btnParts.Enabled = true;
			}
			else
			{
				btnResult.Text = "���� �����";
				cbTechnicalA.Enabled = false;
				cbTechnicalB.Enabled = false;
				nudTeamAScore.Enabled = false;
				nudTeamBScore.Enabled = false;
				btnParts.Enabled = false;
			}
		}

		private void btnResult_Click(object sender, System.EventArgs e)
		{
			_result = !_result;
			SetResultControls();
		}

		private string originalPartsResults = null;
		private bool partsResultsChanged = false;
		private void cbTechnicalA_CheckedChanged(object sender, System.EventArgs e)
		{
			if (cbTechnicalA.Checked)
			{
				cbTechnicalB.Checked = false;

				if (technicalResult == null)
				{
					nudTeamAScore.Value = 0;
					nudTeamBScore.Value = 0;
				}
				else
				{
					nudTeamAScore.Value = technicalResult.Winner;
					nudTeamBScore.Value = technicalResult.Loser;
					if (technicalResult.SmallPoints != null && technicalResult.SmallPoints.Length > 0)
					{
						if (originalPartsResults == null)
							originalPartsResults = _match.PartsResult + "";
						_match.PartsResult = string.Join("|", technicalResult.SmallPoints.ToList().
							ConvertAll(p => string.Format("{0}-{1}", p, 0)));
						partsResultsChanged = true;
						gameResult = null;
					}
				}
			}
			else if (partsResultsChanged)
			{
				_match.PartsResult = originalPartsResults;
				partsResultsChanged = false;
			}

			SetResultControls();
		}

		private void cbTechnicalB_CheckedChanged(object sender, System.EventArgs e)
		{
			if (cbTechnicalB.Checked)
			{
				cbTechnicalA.Checked = false;

				if (technicalResult == null)
				{
					nudTeamAScore.Value = 0;
					nudTeamBScore.Value = 0;
				}
				else
				{
					nudTeamAScore.Value = technicalResult.Loser;
					nudTeamBScore.Value = technicalResult.Winner;
					if (technicalResult.SmallPoints != null && technicalResult.SmallPoints.Length > 0)
					{
						if (originalPartsResults == null)
							originalPartsResults = _match.PartsResult + "";
						_match.PartsResult = string.Join("|", technicalResult.SmallPoints.ToList().
							ConvertAll(p => string.Format("{0}-{1}", 0, p)));
						partsResultsChanged = true;
						gameResult = null;
					}
				}
			}
			else if (partsResultsChanged)
			{
				_match.PartsResult = originalPartsResults;
				partsResultsChanged = false;
			}

			SetResultControls();
		}

		private void btnParts_Click(object sender, System.EventArgs e)
		{
			if (gameResult == null)
			{
				if (Sport.Common.Tools.IsEmpty(_match.PartsResult))
					gameResult = new Sport.Championships.GameResult();
				else
					gameResult = new Sport.Championships.GameResult(_match.PartsResult);
			}

			MatchPartsResultForm mprf = new MatchPartsResultForm(_match, gameResult);

			if (mprf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				gameResult = (Sport.Championships.GameResult)mprf.GameResult.Clone();
				string partsResult = gameResult == null ? _match.PartsResult : gameResult.ToString();
				double overrideScore_A, overrideScore_B;
				double scoreA = mprf.TeamAScore;
				double scoreB = mprf.TeamBScore;
				if (TryOverrideScore(partsResult, out overrideScore_A, out overrideScore_B))
				{
					scoreA = overrideScore_A;
					scoreB = overrideScore_B;
				}
				nudTeamAScore.Value = (int)scoreA;
				nudTeamBScore.Value = (int)scoreB;
			}
		}

		private void MatchForm_Load(object sender, System.EventArgs e)
		{
			//functionaryDialog.View.State["UserType"] = ((int) Sport.Types.UserType.Internal).ToString();
			//coordinator_A.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(
			//	functionaryDialog.ValueSelector);

			SetFunctionaries();
		}

		private void SetFunctionaries()
		{
			gdFunctionaries.Columns.Add(0, "�����", 145);
			gdFunctionaries.Columns.Add(1, "��", 200);
			//gdFunctionaries.Groups[0].RowHeight = 30;

			Sport.Entities.Ruleset rulesetEnt =
				_match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship.Ruleset;

			if (rulesetEnt == null)
			{
				//maybe sport has default ruleset?
				rulesetEnt = _match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship.Sport.Ruleset;
			}

			if (rulesetEnt == null)
			{
				SetError("�� ����� ����� ���� �������");
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
					_match.Cycle.Round.Group.Phase.Championship.CategoryID, -1);
				if (rule != null)
					func = (Functionaries)rule;
			}

			//got anything?
			if (func == null)
			{
				SetError("�� ����� ��� ���� ������� ������ �������");
				return;
			}

			//reset:
			for (int i = 0; i < func.Fields.Count; i++)
				func.Fields[i].Id = -1;

			for (int i = 0; i < func.Fields.Count; i++)
			{
				if (i >= _match.Functionaries.Length)
					break;
				func.Fields[i].Id = _match.Functionaries[i];
			}

			_funcSource = new FunctionaryGridSource(func);
			gdFunctionaries.Source = _funcSource;

			//label3.Text = "";
			//foreach (FunctionaryValue funcValue in func.Fields)
			//	label3.Text += funcValue.Description+" , ";
		}

		private void SetError(string strMsg)
		{
			lbFuncError.Text = strMsg;
			lbFuncError.Visible = true;
			gdFunctionaries.Visible = false;
			btnRemoveFunctionaries.Visible = false;
		}

		public class FunctionaryGridSource : Sport.UI.Controls.IGridSource
		{
			private Sport.UI.EntitySelectionDialog functionaryDialog;
			private Sport.UI.Controls.Grid _grid;
			private Functionaries _functionaries;

			public Functionaries GetFunctionaries()
			{
				return _functionaries;
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

		private void bbFacility_ValueChanged(object sender, EventArgs e)
		{

		}

		private void btnRemoveLocation_Click(object sender, EventArgs e)
		{
			bbFacility.Value = null;
			cbCourt.SelectedItem = null;
		}

		private void btnRemoveFunctionaries_Click(object sender, EventArgs e)
		{
			//reset:
			if (_funcSource != null)
			{
				Functionaries func = _funcSource.GetFunctionaries();
				for (int n = 0; n < func.Fields.Count; n++)
					func.Fields[n].Id = -1;
			}
			gdFunctionaries.Refresh();
		}
	}
}
