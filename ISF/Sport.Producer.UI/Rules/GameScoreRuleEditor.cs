using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;

namespace Sport.Producer.UI.Rules
{
	public class GameScoreRuleEditor : RuleTypeEditor
	{
		public GameScoreRuleEditor()
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
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(GameScoreSelection);
			bb.ValueChanged += new EventHandler(GameScoreChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void GameScoreChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}

		private object GameScoreSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = new
				Sport.UI.Dialogs.GenericEditDialog("ניקוד משחק");
			
			GameScore gs = value as GameScore;
			
			for (int n = 0; n < 5; n++)
			{
				ged.Items.Add(Sport.Rulesets.Data.ScoreNames[n] + ":", 
					Sport.UI.Controls.GenericItemType.Number, 
					gs == null ? 0 : gs[(GameScoreType) n], new object[] { -100d, 100d, (byte) 6, (byte) 1 });
			}
			
			ged.Items.Add("ניקוד שווה למערכות?", Sport.UI.Controls.GenericItemType.Check, 
				gs == null ? false : gs.ScoreEqualsPoints);
			
			string strSportName = GetSportName();
			bool blnVolleyball = (strSportName != null && strSportName.IndexOf("כדורעף") >= 0);
			if (blnVolleyball)
			{
				ged.Items.Add("משחק מקוצר?", Sport.UI.Controls.GenericItemType.Check, 
					gs == null ? false : gs.VolleyballShortGame);
			}

			if (ged.ShowDialog() == DialogResult.OK)
			{
				gs = new GameScore();
				for (int n = 0; n < 5; n++)
				{
					object curValue = ged.Items[n].Value;
					gs[(GameScoreType) n] = (curValue == null) ? 0.0 : (double) curValue;
				}
				if (blnVolleyball)
				{
					gs.ScoreEqualsPoints = (bool) ged.Items[ged.Items.Count - 2].Value;
					gs.VolleyballShortGame = (bool) ged.Items[ged.Items.Count - 1].Value;
				}
				else
				{
					gs.ScoreEqualsPoints = (bool) ged.Items[ged.Items.Count - 1].Value;
				}
					
				return gs;
			}

			return value;
		}

		private string GetSportName()
		{
			if (_rule != null)
			{
				if (_rule.Ruleset != null && _rule.Ruleset.Sport >= 0)
				{
					Sport.Entities.Sport sport = null;
					try
					{
						sport = new Sport.Entities.Sport(_rule.Ruleset.Sport);
					}
					catch
					{}
					if (sport != null)
						return sport.Name;
				}
			}
			return String.Empty;
		}
	}
}
