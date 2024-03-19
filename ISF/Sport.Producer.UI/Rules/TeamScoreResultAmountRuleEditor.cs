using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using Sport.Common;

namespace Sport.Producer.UI.Rules
{
	public class TeamScoreCounterRuleEditor : RuleTypeEditor
	{
		public TeamScoreCounterRuleEditor()
		{
		}

		private Rule _rule;
		private RuleScope _scope;
		private TeamPhaseScoring _teamPhaseScoring;

		public override Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			_teamPhaseScoring = _rule.Ruleset.GetRule(scope, typeof(TeamPhaseScoring)) as TeamPhaseScoring;
			if (_teamPhaseScoring == null)
			{
				Sport.UI.MessageBox.Show("לא נמצאו הגדרות ניקוד קבוצות לשלב");
				return null;
			}

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Value = rule[scope];
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(TeamScoreCounterSelection);
			bb.ValueChanged += new EventHandler(TeamScoreCounterChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void TeamScoreCounterChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}

		private object TeamScoreCounterSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			if (_teamPhaseScoring == null)
				return value;
            
			Sport.UI.Dialogs.GenericEditDialog teamScoreCounterDialog =
				new Sport.UI.Dialogs.GenericEditDialog("מונה תוצאה");
			teamScoreCounterDialog.Items.Add(Sport.UI.Controls.GenericItemType.Selection);

			SortedArray counterNames = new SortedArray();
			foreach (ScoringPlan scoringPlan in _teamPhaseScoring.ScoringPlans)
			{
				foreach (ScoringPlan.Counter counter in scoringPlan.Counters)
				{
					if (!counterNames.Contains(counter.Name))
					{
						counterNames.Add(counter.Name);
					}
				}
			}

			string[] counters = new string[counterNames.Count];
			counterNames.CopyTo(counters, 0);

			teamScoreCounterDialog.Items[0].Values = counters;
			teamScoreCounterDialog.Items[0].ValueChanged += new EventHandler(TeamScoreCounterRuleEditor_ValueChanged);

			TeamScoreCounter tsc = value as TeamScoreCounter;

			if (tsc != null)
			{
				teamScoreCounterDialog.Items[0].Value = tsc.Counter;
			}


			if (teamScoreCounterDialog.ShowDialog() == DialogResult.OK)
			{
				tsc = new TeamScoreCounter(
					(string) teamScoreCounterDialog.Items[0].Value);
				
				return tsc;
			}

			return value;
		}

		private void TeamScoreCounterRuleEditor_ValueChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem gi = sender as Sport.UI.Controls.GenericItem;
			if (gi != null)
			{
				((Sport.UI.Dialogs.GenericEditDialog) gi.Container).Confirmable = gi.Value != null;
			}
		}
	}
}
