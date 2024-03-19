using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;

namespace Sport.Producer.UI.Rules
{
	public class PlayersAmountRuleEditor : RuleTypeEditor
	{
		public PlayersAmountRuleEditor()
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
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(PlayersAmountSelection);
			bb.ValueChanged += new EventHandler(PlayersAmountChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void PlayersAmountChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}


		private static Sport.UI.Dialogs.GenericEditDialog playersAmountDialog;

		static PlayersAmountRuleEditor()
		{
			playersAmountDialog = new
				Sport.UI.Dialogs.GenericEditDialog("כמות שחקנים");

			playersAmountDialog.Items.Add("מינמלי:", Sport.UI.Controls.GenericItemType.Number, 
				0, new object[] { 0, 10000 });
			playersAmountDialog.Items.Add("מקסימלי:", Sport.UI.Controls.GenericItemType.Number, 
				0, new object[] { 0, 10000 });
			playersAmountDialog.Items[0].ValueChanged += new EventHandler(MinimumChanged);
			playersAmountDialog.Items[1].ValueChanged += new EventHandler(MaximumChanged);
		}

		private object PlayersAmountSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			PlayersAmount pa = value as PlayersAmount;

			if (pa != null)
			{
				playersAmountDialog.Items[0].Value = pa.Minimum;
				playersAmountDialog.Items[1].Value = pa.Maximum;
			}
			else
			{
				playersAmountDialog.Items[0].Value = 0;
				playersAmountDialog.Items[1].Value = 0;
			}


			if (playersAmountDialog.ShowDialog() == DialogResult.OK)
			{
				int min = Sport.Common.Tools.CIntDef(playersAmountDialog.Items[0].Value, 0);
				int max = Sport.Common.Tools.CIntDef(playersAmountDialog.Items[1].Value, 0);
				pa = new PlayersAmount(min, max);
				
				return pa;
			}

			return value;
		}

		private static void MinimumChanged(object sender, EventArgs e)
		{
			int min = Sport.Common.Tools.CIntDef(playersAmountDialog.Items[0].Value, 0);
			int max = Sport.Common.Tools.CIntDef(playersAmountDialog.Items[1].Value, 0);
			
			if (max < min)
				playersAmountDialog.Items[0].Value = max;
		}

		private static void MaximumChanged(object sender, EventArgs e)
		{
			int min = Sport.Common.Tools.CIntDef(playersAmountDialog.Items[0].Value, 0);
			int max = Sport.Common.Tools.CIntDef(playersAmountDialog.Items[1].Value, 0);
			
			if (max < min)
				playersAmountDialog.Items[1].Value = min;
		}
	}
}
