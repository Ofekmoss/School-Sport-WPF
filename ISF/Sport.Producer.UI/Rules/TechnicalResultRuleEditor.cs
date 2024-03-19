using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;

namespace Sport.Producer.UI.Rules
{
	public class TechnicalResultRuleEditor : RuleTypeEditor
	{
		public TechnicalResultRuleEditor()
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
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(TechnicalResultSelection);
			bb.ValueChanged += new EventHandler(TechnicalResultChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void TechnicalResultChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}

		private object TechnicalResultSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = new
				Sport.UI.Dialogs.GenericEditDialog("תוצאת טכני");

			TechnicalResult tr = value as TechnicalResult;

			ged.Items.Add("מנצח:", Sport.UI.Controls.GenericItemType.Number,
				tr == null ? 0 : tr.Winner, new object[] { 0d, 1000d });
			ged.Items.Add("מפסיד:", Sport.UI.Controls.GenericItemType.Number,
				tr == null ? 0 : tr.Loser, new object[] { 0d, 1000d });
			ged.Items.Add("נקודות קטנות:", Sport.UI.Controls.GenericItemType.Text,
				(tr == null || tr.SmallPoints == null) ? "" : string.Join(",", tr.SmallPoints));

			if (ged.ShowDialog() == DialogResult.OK)
			{
				tr = new TechnicalResult();

				tr.Winner = (int) (double) ged.Items[0].Value;
				tr.Loser = (int) (double) ged.Items[1].Value;
				tr.SmallPoints = (ged.Items[2].Value + "").Split(',').ToList().ConvertAll(p =>
				{
					int n;
					if (!Int32.TryParse(p, out n))
						n = -1;
					return n;
				}).FindAll(n => n >= 0).ToArray();
				
				return tr;
			}

			return value;
		}

	}
}
