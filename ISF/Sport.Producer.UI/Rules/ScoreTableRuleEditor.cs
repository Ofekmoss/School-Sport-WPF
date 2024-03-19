using System;
using Sport.Rulesets;
using System.Windows.Forms;
using Sport.Rulesets.Rules;

namespace Sport.Producer.UI.Rules
{
	public class ScoreTableRuleEditor : RuleTypeEditor
	{

		public ScoreTableRuleEditor()
		{
		}

		public override Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Value = rule[scope];
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(ScoreTableSelection);
			bb.ValueChanged += new EventHandler(ScoreTableChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private Rule _rule;
		private RuleScope _scope;

		private void ScoreTableChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}

		private object ScoreTableSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			ResultType resultType = _rule.Ruleset.GetRule(_scope, 
				typeof(Sport.Rulesets.Rules.ResultType)) as ResultType;

			if (resultType == null)
			{
				Sport.UI.MessageBox.Show("לא נקבע חוק 'סוג תוצאה'. לא ניתן לקבוע טבלת ניקוד תוצאות.");
				return value;
			}

			ScoreTable st = value as ScoreTable;

			if (st == null)
			{
				st = new ScoreTable(null, resultType.Direction);
			}
			else
			{
				if (resultType.Direction != st.Direction)
				{
					if (!Sport.UI.MessageBox.Ask("הכיוון שהוגדר בסוג התוצאה לא מתאים לכיוון טבלת ניקוד התוצאות.\nהטבלה תמחק, האם להמשיך?", true))
					{
						return value;
					}

					st = new ScoreTable(null, resultType.Direction);
				}
			}


			string sportFieldName = "";
			int sportFieldID=-1;
			if (_scope.SportFieldType >= 0)
			{
				Sport.Entities.SportFieldType sportFieldType = new Sport.Entities.SportFieldType(_scope.SportFieldType);
				sportFieldName += sportFieldType.Name;
				if (_scope.SportField >= 0)
				{
					Sport.Entities.SportField sportField = new Sport.Entities.SportField(_scope.SportField);
					sportFieldName += " - " + sportField.Name;
					sportFieldID = sportField.Id;
				}
			}
			Sport.Producer.UI.Rules.Dialogs.ScoreTableDialog scoreTableDialog = 
				new Sport.Producer.UI.Rules.Dialogs.ScoreTableDialog(-1, 
				sportFieldID, sportFieldName, st, resultType);
			
			if (scoreTableDialog.ShowDialog() == DialogResult.OK)
			{
				return scoreTableDialog.ScoreTable;
			}

			return value;
		}
	}
}
