using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Rulesets;
using Sport.Producer.UI.Rules.Dialogs;

namespace Sport.Producer.UI.Rules
{
	public class ScoreByPartRuleEditor : RuleTypeEditor
	{
		public ScoreByPartRuleEditor()
		{
		}

		private Rule _rule;
		private RuleScope _scope;

		public override System.Windows.Forms.Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Tag = rule.Ruleset.SportType;
			bb.Value = rule[scope];
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(ScoreByPartDialog.ValueSelector);
			bb.ValueChanged += new EventHandler(ScoreByPartChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void ScoreByPartChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}
	}
}
