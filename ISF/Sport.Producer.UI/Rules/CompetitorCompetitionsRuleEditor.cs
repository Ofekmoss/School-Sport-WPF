using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;

namespace Sport.Producer.UI.Rules
{
	public class CompetitorCompetitionsRuleEditor : RuleTypeEditor
	{
		public CompetitorCompetitionsRuleEditor()
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
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(CompetitorCompetitionsSelection);
			bb.ValueChanged += new EventHandler(CompetitorCompetitionsChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void CompetitorCompetitionsChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}


		private static Sport.UI.Dialogs.GenericEditDialog competitorCompetitionsDialog;

		static CompetitorCompetitionsRuleEditor()
		{
			competitorCompetitionsDialog = new
				Sport.UI.Dialogs.GenericEditDialog("מקצועות למשתתף");

			competitorCompetitionsDialog.Items.Add("מינמלי:", Sport.UI.Controls.GenericItemType.Number, 
				0, new object[] { 0, 100 });
			competitorCompetitionsDialog.Items.Add("מקסימלי:", Sport.UI.Controls.GenericItemType.Number, 
				0, new object[] { 0, 100 });
			competitorCompetitionsDialog.Items[0].ValueChanged += new EventHandler(MinimumChanged);
			competitorCompetitionsDialog.Items[1].ValueChanged += new EventHandler(MaximumChanged);
		}

		private object CompetitorCompetitionsSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			CompetitorCompetitions cc = value as CompetitorCompetitions;

			if (cc != null)
			{
				competitorCompetitionsDialog.Items[0].Value = cc.Minimum;
				competitorCompetitionsDialog.Items[1].Value = cc.Maximum;
			}
			else
			{
				competitorCompetitionsDialog.Items[0].Value = 0;
				competitorCompetitionsDialog.Items[1].Value = 0;
			}


			if (competitorCompetitionsDialog.ShowDialog() == DialogResult.OK)
			{
				int min = Sport.Common.Tools.CIntDef(competitorCompetitionsDialog.Items[0].Value, 0);
				int max = Sport.Common.Tools.CIntDef(competitorCompetitionsDialog.Items[1].Value, 0);
				cc = new CompetitorCompetitions(min, max);
				
				return cc;
			}

			return value;
		}

		private static void MinimumChanged(object sender, EventArgs e)
		{
			int min = Sport.Common.Tools.CIntDef(competitorCompetitionsDialog.Items[0].Value, 0);
			int max = Sport.Common.Tools.CIntDef(competitorCompetitionsDialog.Items[1].Value, 0);
			
			if (max < min)
				competitorCompetitionsDialog.Items[0].Value = max;
		}

		private static void MaximumChanged(object sender, EventArgs e)
		{
			int min = Sport.Common.Tools.CIntDef(competitorCompetitionsDialog.Items[0].Value, 0);
			int max = Sport.Common.Tools.CIntDef(competitorCompetitionsDialog.Items[1].Value, 0);
			
			if (max < min)
				competitorCompetitionsDialog.Items[1].Value = min;
		}
	}
}
