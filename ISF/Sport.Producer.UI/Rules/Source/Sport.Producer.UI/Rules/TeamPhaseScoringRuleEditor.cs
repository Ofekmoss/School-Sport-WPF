using System;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using System.Drawing;
using Sport.Producer.UI.Rules.Dialogs;

namespace Sport.Producer.UI.Rules
{
	public class TeamPhaseScoringRuleEditor : RuleTypeEditor
	{
		public TeamPhaseScoringRuleEditor()
		{
		}

		private Rule _rule;
		private RuleScope _scope;

		public override System.Windows.Forms.Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Value = rule[scope];
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(TeamPhaseScoringDialog.ValueSelector);
			bb.ValueChanged += new EventHandler(TeamPhaseScoringChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void TeamPhaseScoringChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}
	}
}
