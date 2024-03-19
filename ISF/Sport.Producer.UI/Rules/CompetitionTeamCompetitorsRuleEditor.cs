using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;

namespace Sport.Producer.UI.Rules
{
	public class CompetitionTeamCompetitorsRuleEditor : RuleTypeEditor
	{
		public CompetitionTeamCompetitorsRuleEditor()
		{
		}

		private Rule _rule;
		private RuleScope _scope;

		public override Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Value = rule[scope];
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(CompetitionTeamCompetitorsSelection);
			bb.ValueChanged += new EventHandler(CompetitionTeamCompetitorsChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void CompetitionTeamCompetitorsChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}


		private static Sport.UI.Dialogs.GenericEditDialog competitionTeamCompetitorsDialog;

		static CompetitionTeamCompetitorsRuleEditor()
		{
			competitionTeamCompetitorsDialog = new
				Sport.UI.Dialogs.GenericEditDialog("משתתפי קבוצה במקצוע");

			competitionTeamCompetitorsDialog.Items.Add("מינמלי:", Sport.UI.Controls.GenericItemType.Number, 
				0, new object[] { 0, 100 });
			competitionTeamCompetitorsDialog.Items.Add("מקסימלי:", Sport.UI.Controls.GenericItemType.Number, 
				0, new object[] { 0, 100 });
			competitionTeamCompetitorsDialog.Items[0].ValueChanged += new EventHandler(MinimumChanged);
			competitionTeamCompetitorsDialog.Items[1].ValueChanged += new EventHandler(MaximumChanged);
		}

		private object CompetitionTeamCompetitorsSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			CompetitionTeamCompetitors ctc = value as CompetitionTeamCompetitors;

			if (ctc != null)
			{
				competitionTeamCompetitorsDialog.Items[0].Value = ctc.Minimum;
				competitionTeamCompetitorsDialog.Items[1].Value = ctc.Maximum;
			}
			else
			{
				competitionTeamCompetitorsDialog.Items[0].Value = 0;
				competitionTeamCompetitorsDialog.Items[1].Value = 0;
			}


			if (competitionTeamCompetitorsDialog.ShowDialog() == DialogResult.OK)
			{
				int min = Sport.Common.Tools.CIntDef(competitionTeamCompetitorsDialog.Items[0].Value, 0);
				int max = Sport.Common.Tools.CIntDef(competitionTeamCompetitorsDialog.Items[1].Value, 0);
				ctc = new CompetitionTeamCompetitors(min, max);
				
				return ctc;
			}

			return value;
		}

		private static void MinimumChanged(object sender, EventArgs e)
		{
			int min = Sport.Common.Tools.CIntDef(competitionTeamCompetitorsDialog.Items[0].Value, 0);
			int max = Sport.Common.Tools.CIntDef(competitionTeamCompetitorsDialog.Items[1].Value, 0);
			
			if (max < min)
				competitionTeamCompetitorsDialog.Items[0].Value = max;
		}

		private static void MaximumChanged(object sender, EventArgs e)
		{
			int min = Sport.Common.Tools.CIntDef(competitionTeamCompetitorsDialog.Items[0].Value, 0);
			int max = Sport.Common.Tools.CIntDef(competitionTeamCompetitorsDialog.Items[1].Value, 0);
			
			if (max < min)
				competitionTeamCompetitorsDialog.Items[1].Value = min;
		}
	}
}
