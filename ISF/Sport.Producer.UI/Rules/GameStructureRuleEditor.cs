using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;

namespace Sport.Producer.UI.Rules
{
	public class GameStructureRuleEditor : RuleTypeEditor
	{
		private Rule _rule;
		private RuleScope _scope;

		public override Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Value = rule[scope];
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(GameStructureSelection);
			bb.ValueChanged += new EventHandler(GameStructureChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private void GameStructureChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}

		private static readonly object PointsGameType = "נקודות";
		private static readonly object DurationGameType = "זמן";

		private void SetDialog(Sport.UI.Dialogs.GenericEditDialog ged)
		{
			object type = ged.Items[0].Value;
			int a = (int) (double) ged.Items[1].Value;
			int b = (int) (double) ged.Items[2].Value;
			if (type == PointsGameType)
			{
				ged.Items[1].Title = Sport.Rulesets.Rules.GameStructure.SetsName + ":";
				ged.Items[2].Title = Sport.Rulesets.Rules.GameStructure.GamesName + ":";
				ged.Items[2].Values = Sport.UI.Controls.GenericItem.NumberValues(1, 100, 3, 0);
				if (b < 1)
					ged.Items[2].Value = 1;
			}
			else
			{
				ged.Items[1].Title = Sport.Rulesets.Rules.GameStructure.PartsName + ":";
				ged.Items[2].Title = Sport.Rulesets.Rules.GameStructure.ExtensionsName + ":";
				ged.Items[2].Values = Sport.UI.Controls.GenericItem.NumberValues(0, 100, 3, 0);
			}

			if (a < 1)
				ged.Items[1].Value = 1;
		}

		private void GameStructureTypeChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.GenericItem item = sender as Sport.UI.Controls.GenericItem;
			if (item != null)
			{
				Sport.UI.Dialogs.GenericEditDialog ged = item.Container as Sport.UI.Dialogs.GenericEditDialog;
				if (ged != null)
					SetDialog(ged);
			}
		}

		private object GameStructureSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = new
				Sport.UI.Dialogs.GenericEditDialog("מבנה משחק");

			ged.Items.Add("סוג:", Sport.UI.Controls.GenericItemType.Selection,
				DurationGameType, new object[] { PointsGameType, DurationGameType });
			ged.Items.Add(Sport.UI.Controls.GenericItemType.Number, null,
				Sport.UI.Controls.GenericItem.NumberValues(1, 100, 3, 0));
			ged.Items.Add(Sport.UI.Controls.GenericItemType.Number);

			ged.Items[0].Nullable = false;

			ged.Items[0].ValueChanged += new EventHandler(GameStructureTypeChanged);

			GameStructure gs = value as GameStructure;

			if (gs != null)
			{
				if (gs.GameType == Sport.Rulesets.Rules.GameType.Points)
				{
					ged.Items[0].Value = PointsGameType;
				}

				ged.Items[1].Value = gs.SetPart;
				ged.Items[2].Value = gs.GameExtension;
			}
			else
			{
				ged.Items[0].Value = DurationGameType;
				ged.Items[1].Value = 1;
				ged.Items[2].Value = 0;
			}

			SetDialog(ged);

			if (ged.ShowDialog() == DialogResult.OK)
			{
				Sport.Rulesets.Rules.GameType gameType;

				if (ged.Items[0].Value == DurationGameType)
					gameType = Sport.Rulesets.Rules.GameType.Duration;
				else if (ged.Items[0].Value == PointsGameType)
					gameType = Sport.Rulesets.Rules.GameType.Points;
				else
					return null;

				gs = new GameStructure(gameType);
				gs.SetPart = (int) (double) ged.Items[1].Value;
				gs.GameExtension = (int) (double) ged.Items[2].Value;
					
				return gs;
			}

			return value;
		}
	}
}
