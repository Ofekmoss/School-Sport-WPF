using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.Championships;
using Sport.Rulesets;
using Sport.Rulesets.Rules;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for TournamentForm.
	/// </summary>
	public class TournamentForm : System.Windows.Forms.Form
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
		private Sport.UI.Controls.NullComboBox cbFacility;
		private Sport.UI.Controls.NullComboBox cbCourt;

		private Sport.Championships.Tournament _tournament;

		private System.Windows.Forms.NumericUpDown nudNumber;
		private System.Windows.Forms.Label labelNumber;
		private System.Windows.Forms.GroupBox gbFunctionaries;
		private System.Windows.Forms.Label lbFuncError;
		private Sport.UI.Controls.Grid gdFunctionaries;
		private System.Windows.Forms.Label labelCourt;
		
		private FunctionaryGridSource _funcSource=null;
		
		public TournamentForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_tournament = null;
		}

		public TournamentForm(Sport.Championships.Tournament tournament)
			: this()
		{
			_tournament = tournament;

			AddFacilities();

			Text = tournament.ToString();

			if (Sport.Common.Tools.IsMinDate(tournament.Time))
			{
				dtpTime.Checked = false;
			}
			else
			{
				dtpTime.Checked = true;
				dtpTime.Value = tournament.Time;
			}

			nudNumber.Value = tournament.Number;

			if (tournament.Facility != null)
			{
				cbFacility.SelectedItem = tournament.Facility;
				SetCourts();
				if (tournament.Court != null)
				{
					cbCourt.SelectedItem = tournament.Court;
				}
			}
		}

		private void AddFacilities()
		{
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען מתקנים אנא המתן...");

			Sport.Entities.Championship cc = 
				_tournament.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship;

			//add the facilities:
			cbFacility.Items.Add(Sport.UI.Controls.NullComboBox.Null);
			cbFacility.Items.AddRange(cc.GetFacilities());

			Sport.UI.Dialogs.WaitForm.HideWait();
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
			this.labelTime = new System.Windows.Forms.Label();
			this.dtpTime = new System.Windows.Forms.DateTimePicker();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.gbLocation = new System.Windows.Forms.GroupBox();
			this.labelCourt = new System.Windows.Forms.Label();
			this.cbCourt = new Sport.UI.Controls.NullComboBox();
			this.labelFacility = new System.Windows.Forms.Label();
			this.cbFacility = new Sport.UI.Controls.NullComboBox();
			this.nudNumber = new System.Windows.Forms.NumericUpDown();
			this.labelNumber = new System.Windows.Forms.Label();
			this.gbFunctionaries = new System.Windows.Forms.GroupBox();
			this.lbFuncError = new System.Windows.Forms.Label();
			this.gdFunctionaries = new Sport.UI.Controls.Grid();
			this.gbLocation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).BeginInit();
			this.gbFunctionaries.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelTime
			// 
			this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTime.Location = new System.Drawing.Point(216, 56);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(152, 16);
			this.labelTime.TabIndex = 0;
			this.labelTime.Text = "מועד:";
			// 
			// dtpTime
			// 
			this.dtpTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.dtpTime.CustomFormat = "dd/MM/yyyy HH:mm";
			this.dtpTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dtpTime.Location = new System.Drawing.Point(184, 72);
			this.dtpTime.Name = "dtpTime";
			this.dtpTime.ShowCheckBox = true;
			this.dtpTime.Size = new System.Drawing.Size(184, 21);
			this.dtpTime.TabIndex = 1;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnOK.Cursor = System.Windows.Forms.Cursors.Default;
			this.btnOK.Location = new System.Drawing.Point(8, 256);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "אישור";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(96, 256);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 8;
			this.btnCancel.Text = "ביטול";
			// 
			// gbLocation
			// 
			this.gbLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gbLocation.Controls.Add(this.labelCourt);
			this.gbLocation.Controls.Add(this.cbCourt);
			this.gbLocation.Controls.Add(this.labelFacility);
			this.gbLocation.Controls.Add(this.cbFacility);
			this.gbLocation.Location = new System.Drawing.Point(0, 8);
			this.gbLocation.Name = "gbLocation";
			this.gbLocation.Size = new System.Drawing.Size(184, 112);
			this.gbLocation.TabIndex = 14;
			this.gbLocation.TabStop = false;
			this.gbLocation.Text = "מיקום";
			// 
			// labelCourt
			// 
			this.labelCourt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCourt.Location = new System.Drawing.Point(136, 66);
			this.labelCourt.Name = "labelCourt";
			this.labelCourt.Size = new System.Drawing.Size(40, 16);
			this.labelCourt.TabIndex = 7;
			this.labelCourt.Text = "מגרש:";
			// 
			// cbCourt
			// 
			this.cbCourt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbCourt.Location = new System.Drawing.Point(8, 82);
			this.cbCourt.Name = "cbCourt";
			this.cbCourt.Size = new System.Drawing.Size(168, 22);
			this.cbCourt.Sorted = true;
			this.cbCourt.TabIndex = 6;
			// 
			// labelFacility
			// 
			this.labelFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFacility.Location = new System.Drawing.Point(136, 22);
			this.labelFacility.Name = "labelFacility";
			this.labelFacility.Size = new System.Drawing.Size(40, 16);
			this.labelFacility.TabIndex = 5;
			this.labelFacility.Text = "מתקן:";
			// 
			// cbFacility
			// 
			this.cbFacility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbFacility.Location = new System.Drawing.Point(8, 38);
			this.cbFacility.Name = "cbFacility";
			this.cbFacility.Size = new System.Drawing.Size(168, 22);
			this.cbFacility.Sorted = true;
			this.cbFacility.TabIndex = 4;
			this.cbFacility.SelectedIndexChanged += new System.EventHandler(this.cbFacility_SelectedIndexChanged);
			// 
			// nudNumber
			// 
			this.nudNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nudNumber.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.nudNumber.Location = new System.Drawing.Point(184, 16);
			this.nudNumber.Maximum = new System.Decimal(new int[] {
																	  999999999,
																	  0,
																	  0,
																	  0});
			this.nudNumber.Name = "nudNumber";
			this.nudNumber.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.nudNumber.TabIndex = 17;
			// 
			// labelNumber
			// 
			this.labelNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNumber.Location = new System.Drawing.Point(312, 24);
			this.labelNumber.Name = "labelNumber";
			this.labelNumber.Size = new System.Drawing.Size(56, 16);
			this.labelNumber.TabIndex = 18;
			this.labelNumber.Text = "מספר:";
			// 
			// gbFunctionaries
			// 
			this.gbFunctionaries.Controls.Add(this.lbFuncError);
			this.gbFunctionaries.Controls.Add(this.gdFunctionaries);
			this.gbFunctionaries.Location = new System.Drawing.Point(8, 128);
			this.gbFunctionaries.Name = "gbFunctionaries";
			this.gbFunctionaries.Size = new System.Drawing.Size(360, 120);
			this.gbFunctionaries.TabIndex = 20;
			this.gbFunctionaries.TabStop = false;
			this.gbFunctionaries.Text = "בעלי תפקידים";
			// 
			// lbFuncError
			// 
			this.lbFuncError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lbFuncError.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.lbFuncError.ForeColor = System.Drawing.Color.Red;
			this.lbFuncError.Location = new System.Drawing.Point(16, 32);
			this.lbFuncError.Name = "lbFuncError";
			this.lbFuncError.Size = new System.Drawing.Size(336, 40);
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
			this.gdFunctionaries.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.gdFunctionaries.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gdFunctionaries.HeaderHeight = 17;
			this.gdFunctionaries.HorizontalLines = true;
			this.gdFunctionaries.Location = new System.Drawing.Point(8, 24);
			this.gdFunctionaries.Name = "gdFunctionaries";
			this.gdFunctionaries.SelectedIndex = -1;
			this.gdFunctionaries.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gdFunctionaries.ShowCheckBoxes = false;
			this.gdFunctionaries.ShowRowNumber = false;
			this.gdFunctionaries.Size = new System.Drawing.Size(345, 91);
			this.gdFunctionaries.TabIndex = 6;
			this.gdFunctionaries.VerticalLines = true;
			this.gdFunctionaries.VisibleRow = 0;
			// 
			// TournamentForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(370, 288);
			this.Controls.Add(this.gbFunctionaries);
			this.Controls.Add(this.labelNumber);
			this.Controls.Add(this.nudNumber);
			this.Controls.Add(this.gbLocation);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.dtpTime);
			this.Controls.Add(this.labelTime);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "TournamentForm";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TournamentForm";
			this.Load += new System.EventHandler(this.TournamentForm_Load);
			this.gbLocation.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudNumber)).EndInit();
			this.gbFunctionaries.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void SetCourts()
		{
			cbCourt.Items.Clear();

			Sport.Entities.Facility facility = cbFacility.SelectedItem as Sport.Entities.Facility;
			if (facility != null)
			{
				MatchChampionship champ=_tournament.Cycle.Round.Group.Phase.Championship;
				Sport.Entities.Sport sport=champ.ChampionshipCategory.Championship.Sport;
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
			Sport.Entities.Facility facility = cbFacility.SelectedItem as Sport.Entities.Facility;
			Sport.Entities.Court court = cbCourt.SelectedItem as Sport.Entities.Court;
			DateTime tournamentDate=
				(dtpTime.Checked)?dtpTime.Value:DateTime.MinValue;
			int number = (int) nudNumber.Value;

			Tournament t = _tournament.Cycle.Round.Group.GetTournamentByNumber(number);

			if (t != null && t != _tournament)
			{
				Sport.UI.MessageBox.Show("מספר טורניר קיים");
				nudNumber.Focus();
				return ;
			}
			
			int[] arrFunctionaries=new int[0];
			if (_funcSource != null)
			{
				Functionaries func=_funcSource.GetFunctionaries();
				arrFunctionaries = new int[func.Fields.Count];
				for (int n=0; n<func.Fields.Count; n++)
				{
					arrFunctionaries[n] = func.Fields[n].Id;
				}
			}
			
			try
			{
				if (_tournament.Set(number, tournamentDate, facility, court))
				{
					//set functionaries for all matches
					foreach (Sport.Championships.Match match in _tournament.Cycle.Matches)
					{
						if (match.Tournament == _tournament.Index)
						{
							try
							{
								match.Set(match.Number, match.Time, match.Facility, match.Court, arrFunctionaries);
							}
							catch {}
						}
					}
					DialogResult = System.Windows.Forms.DialogResult.OK;
					Close();
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("Phase not started") >= 0)
				{
					Sport.UI.MessageBox.Error("לא ניתן לערוך את התחרות: יש להגדיר שלב נוכחי", "עריכת תחרות");
				}
				else
				{
					throw ex;
				}
			}
		}
		
		private void SetFunctionaries()
		{
			gdFunctionaries.Columns.Add(0, "תיאור", 145);
			gdFunctionaries.Columns.Add(1, "שם", 200);
			//gdFunctionaries.Groups[0].RowHeight = 30;
			
			Sport.Entities.Ruleset rulesetEnt=
				_tournament.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship.Ruleset;
			
			if (rulesetEnt == null)
			{
				//maybe sport has default ruleset?
				rulesetEnt = _tournament.Cycle.Round.Group.Phase.Championship.ChampionshipCategory.Championship.Sport.Ruleset;
			}
			
			if (rulesetEnt == null)
			{
				SetError("לא מוגדר תקנון עבור האליפות");
				return;
			}
			
			Functionaries func=null;
			if (Sport.Core.Session.Connected)
			{
				// Creating a clone of the loaded rule set
				Ruleset ruleset = new Ruleset(Ruleset.LoadRuleset(rulesetEnt.Id));
				func = (Functionaries) ruleset.GetRule(RuleScope.AnyScope, typeof(Functionaries));
			}
			else
			{
				object rule = Sport.Rulesets.Ruleset.LoadOfflineRule(
					typeof(Sport.Rulesets.Rules.FunctionariesRule), 
					_tournament.Cycle.Round.Group.Phase.Championship.CategoryID, -1);
				if (rule != null)
					func = (Functionaries) rule;
			}
			
			//got anything?
			if (func == null)
			{
				SetError("לא מוגדר חוק בעלי תפקידים בתקנון האליפות");
				return;
			}
			
			//reset:
			for (int i=0; i<func.Fields.Count; i++)
				func.Fields[i].Id = -1;
			
			_funcSource = new FunctionaryGridSource(func);
			gdFunctionaries.Source = _funcSource;
		}

		private void SetError(string strMsg)
		{
			lbFuncError.Text = strMsg;
			lbFuncError.Visible = true;
			gdFunctionaries.Visible = false;
		}

		private void TournamentForm_Load(object sender, System.EventArgs e)
		{
			SetFunctionaries();
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
				functionaryDialog.View.State["type"] = ((int) Sport.Types.FunctionaryType.Coordinator).ToString();
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
				FunctionaryValue funcValue=_functionaries.Fields[row];
				
				if (field == 1) // Value
				{
					Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
					functionaryDialog.View.State["type"] = 
						((int) funcValue.Type).ToString();
					int funcID=funcValue.Id;
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
				FunctionaryValue funcValue=_functionaries.Fields[row];
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
				Sport.Data.Entity funcEnt=bb.Value as Sport.Data.Entity;
				FunctionaryValue funcValue = bb.Tag as FunctionaryValue;
				funcValue.Id = (funcEnt == null)?-1:funcEnt.Id;
			}
		}
	}
}
