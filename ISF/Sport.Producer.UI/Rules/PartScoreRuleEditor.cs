using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sport.Rulesets;
using System.Windows.Forms;
using Sport.Rulesets.Rules;

namespace Sport.Producer.UI.Rules
{
	public class PartScoreRuleEditor : RuleTypeEditor
	{
		public PartScoreRuleEditor()
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
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(PartScoreSelection);
			bb.ValueChanged += new EventHandler(PartScoreChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void PartScoreChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}

		private object PartScoreSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = new
				Sport.UI.Dialogs.GenericEditDialog("ניקוד מערכות");

			PartScore ps = value as PartScore;

			ged.Items.Add("ניצחון:", Sport.UI.Controls.GenericItemType.Number,
				ps == null ? 0 : ps.Win, new object[] { 0d, 1000d });
			ged.Items.Add("תיקו:", Sport.UI.Controls.GenericItemType.Number,
				ps == null ? 0 : ps.Draw, new object[] { 0d, 1000d });
			ged.Items.Add("הפסד:", Sport.UI.Controls.GenericItemType.Number,
				ps == null ? 0 : ps.Lose, new object[] { 0d, 1000d });

			if (ged.ShowDialog() == DialogResult.OK)
			{
				ps = new PartScore();
				ps.Win = (int)(double)ged.Items[0].Value;
				ps.Draw = (int)(double)ged.Items[1].Value;
				ps.Lose = (int)(double)ged.Items[2].Value;
				return ps;
			}

			return value;
		}
	}
}
