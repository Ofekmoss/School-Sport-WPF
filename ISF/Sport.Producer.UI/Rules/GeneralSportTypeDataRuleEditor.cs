using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;

namespace Sport.Producer.UI.Rules
{
	public class GeneralSportTypeDataRuleEditor : RuleTypeEditor
	{
		public GeneralSportTypeDataRuleEditor()
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
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(GeneralSportTypeDataSelection);
			bb.ValueChanged += new EventHandler(GeneralSportTypeDataChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void GeneralSportTypeDataChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}


		private static Sport.UI.Dialogs.GenericEditDialog sportTypeDataDialog;

		static GeneralSportTypeDataRuleEditor()
		{
			sportTypeDataDialog =
				new Sport.UI.Dialogs.GenericEditDialog("נתונים כלליים למקצוע");

			sportTypeDataDialog.Items.Add("שימוש במסלולים?",
				Sport.UI.Controls.GenericItemType.Check, true);
			sportTypeDataDialog.Items.Add("ניקוד שווה לדירוג?",
				Sport.UI.Controls.GenericItemType.Check, false);
			sportTypeDataDialog.Items.Add("מספר נסיונות:",
				Sport.UI.Controls.GenericItemType.Number, true);
			sportTypeDataDialog.Items.Add("מספר תוצאות:",
				Sport.UI.Controls.GenericItemType.Number, true);
			sportTypeDataDialog.Items.Add("ניקוד כפול?",
				Sport.UI.Controls.GenericItemType.Check, false);
			sportTypeDataDialog.Items.Add("תוצאות משותפות?",
				Sport.UI.Controls.GenericItemType.Check, false);
			sportTypeDataDialog.Items.Add("פסילות?",
				Sport.UI.Controls.GenericItemType.Check, false);
			sportTypeDataDialog.Items.Add("רוח?",
				Sport.UI.Controls.GenericItemType.Check, false);
		}

		private object GeneralSportTypeDataSelection(
			Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			GeneralSportTypeData data = value as GeneralSportTypeData;
			sportTypeDataDialog.Items[0].Value = (data == null) ? true : data.HasLanes;
			sportTypeDataDialog.Items[1].Value = (data == null) ? false : data.ScoreIsRank;
			sportTypeDataDialog.Items[2].Value = (data == null) ? 1 : data.Tries;
			sportTypeDataDialog.Items[3].Value = (data == null) ? 1 : data.Results;
			sportTypeDataDialog.Items[4].Value = (data == null) ? false : data.DoubleScore;
			sportTypeDataDialog.Items[5].Value = (data == null) ? false : data.SharedResults;
			sportTypeDataDialog.Items[6].Value = (data == null) ? false : data.Disqualifications;
			sportTypeDataDialog.Items[7].Value = (data == null) ? false : data.Wind;

			if (sportTypeDataDialog.ShowDialog() == DialogResult.OK)
			{
				data = new GeneralSportTypeData();
				if (sportTypeDataDialog.Items[0].Value != null)
					data.HasLanes = (bool)sportTypeDataDialog.Items[0].Value;
				if (sportTypeDataDialog.Items[1].Value != null)
					data.ScoreIsRank = (bool)sportTypeDataDialog.Items[1].Value;
				if (sportTypeDataDialog.Items[2].Value != null)
					data.Tries = (int)((double)sportTypeDataDialog.Items[2].Value);
				if (sportTypeDataDialog.Items[3].Value != null)
					data.Results = (int)((double)sportTypeDataDialog.Items[3].Value);
				if (sportTypeDataDialog.Items[4].Value != null)
					data.DoubleScore = (bool)sportTypeDataDialog.Items[4].Value;
				if (sportTypeDataDialog.Items[5].Value != null)
					data.SharedResults = (bool)sportTypeDataDialog.Items[5].Value;
				if (sportTypeDataDialog.Items[6].Value != null)
					data.Disqualifications = (bool)sportTypeDataDialog.Items[6].Value;
				if (sportTypeDataDialog.Items[7].Value != null)
					data.Wind = (bool)sportTypeDataDialog.Items[7].Value;
				return data;
			}

			return value;
		}
	}
}
