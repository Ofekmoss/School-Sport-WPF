using System;
using System.Windows.Forms;
using Sport.Rulesets.Rules;
using Sport.Rulesets;
using Sport.UI.Dialogs;

namespace Sport.Producer.UI.Rules
{
	public class ResultEdit
	{
		public static bool EditResult(ResultType resultType, ref float result)
		{
			GenericEditDialog ged = new GenericEditDialog("הכנס תוצאה");
			if (resultType.Value == Sport.Core.Data.ResultValue.Distance)
			{
				int me;
				int km = Math.DivRem((int) result, 1000, out me);
				int mm;
				int cm = Math.DivRem((int) ((result - km * 1000 - me) * 1000), 10, out mm);

				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Kilometers) != 0)
					ged.Items.Add("ק\"מ", Sport.UI.Controls.GenericItemType.Number, km, new object[] { 0, 1000 });
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Meters) != 0)
					ged.Items.Add("מטר", Sport.UI.Controls.GenericItemType.Number, me, new object[] { 0, 999 });
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Centimeters) != 0)
					ged.Items.Add("ס\"מ", Sport.UI.Controls.GenericItemType.Number, cm, new object[] { 0, 99 });
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Milimeters) != 0)
					ged.Items.Add("מ\"מ", Sport.UI.Controls.GenericItemType.Number, mm, new object[] { 0, 9 });

				if (ged.ShowDialog() == DialogResult.OK)
				{
					result = 0;
					int i = 0;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Kilometers) != 0)
						result = (int) (double) ged.Items[i++].Value * 1000;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Meters) != 0)
						result += (int) (double) ged.Items[i++].Value;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Centimeters) != 0)
						result += (float) (double) ged.Items[i++].Value / 100;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Milimeters) != 0)
						result += (float) (double) ged.Items[i++].Value / 1000;
					return true;

				}
			}
			else if (resultType.Value == Sport.Core.Data.ResultValue.Time)
			{
				int se = (int) result;
				int da = Math.DivRem(se, 86400, out se);
				int ho = Math.DivRem(se, 3600, out se);
				int mi = Math.DivRem(se, 60, out se);
				int ms = (int) ((result - Math.Floor(result)) * 1000);

				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Days) != 0)
					ged.Items.Add("ימים", Sport.UI.Controls.GenericItemType.Number, da, new object[] { 0, 1000 });
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Hours) != 0)
					ged.Items.Add("שעות", Sport.UI.Controls.GenericItemType.Number, ho, new object[] { 0, 24 });
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Minutes) != 0)
					ged.Items.Add("דקות", Sport.UI.Controls.GenericItemType.Number, mi, new object[] { 0, 59 });
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Seconds) != 0)
					ged.Items.Add("שניות", Sport.UI.Controls.GenericItemType.Number, se, new object[] { 0, 59 });
				if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Miliseconds) != 0)
					ged.Items.Add("אלפיות", Sport.UI.Controls.GenericItemType.Number, ms, new object[] { 0, 999 });

				if (ged.ShowDialog() == DialogResult.OK)
				{
					result = 0;
					int i = 0;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Days) != 0)
						result = (int) ged.Items[i++].Value * 86400;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Hours) != 0)
						result += (int) ged.Items[i++].Value * 3600;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Minutes) != 0)
						result += (float) ged.Items[i++].Value * 60;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Seconds) != 0)
						result += (float) ged.Items[i++].Value;
					if ((resultType.Measures & Sport.Core.Data.ResultMeasure.Miliseconds) != 0)
						result += (float) ged.Items[i++].Value / 1000;
					return true;

				}
			}


			return false;
		}
	}

	public class ResultTypeRuleEditor : RuleTypeEditor
	{
		private static Sport.Data.LookupItem[] valueItems;
		private static Sport.Data.LookupItem[] directionItems;
		private static Sport.UI.Dialogs.GenericEditDialog resultTypeDialog;
		static ResultTypeRuleEditor()
		{
			valueItems = new Sport.Data.LookupItem[3];
			valueItems[(int) Sport.Core.Data.ResultValue.Points] = new Sport.Data.LookupItem(
				(int) Sport.Core.Data.ResultValue.Points, 
				Sport.Rulesets.Data.ValueNames[(int) Sport.Core.Data.ResultValue.Points]);
			valueItems[(int) Sport.Core.Data.ResultValue.Distance] = new Sport.Data.LookupItem(
				(int) Sport.Core.Data.ResultValue.Distance, 
				Sport.Rulesets.Data.ValueNames[(int) Sport.Core.Data.ResultValue.Distance]);
			valueItems[(int) Sport.Core.Data.ResultValue.Time] = new Sport.Data.LookupItem(
				(int) Sport.Core.Data.ResultValue.Time, 
				Sport.Rulesets.Data.ValueNames[(int) Sport.Core.Data.ResultValue.Time]);
			directionItems = new Sport.Data.LookupItem[2];
			directionItems[(int) Sport.Core.Data.ResultDirection.Most] = new Sport.Data.LookupItem(
				(int) Sport.Core.Data.ResultDirection.Most, 
				Sport.Rulesets.Data.DirectionNames[(int) Sport.Core.Data.ResultDirection.Most]);
			directionItems[(int) Sport.Core.Data.ResultDirection.Least] = new Sport.Data.LookupItem(
				(int) Sport.Core.Data.ResultDirection.Least, 
				Sport.Rulesets.Data.DirectionNames[(int) Sport.Core.Data.ResultDirection.Least]);

			resultTypeDialog = new Sport.UI.Dialogs.GenericEditDialog("סוג תוצאה");

			resultTypeDialog.Items.Add("ערך:", Sport.UI.Controls.GenericItemType.Selection, null, valueItems);
			resultTypeDialog.Items.Add("כיוון:", Sport.UI.Controls.GenericItemType.Selection, null, directionItems);
			resultTypeDialog.Items.Add("תבנית:", Sport.UI.Controls.GenericItemType.Button, null, 
				new object[] {
								 new Sport.UI.Controls.ButtonBox.SelectValue(Rules.Dialogs.ValueFormatterDialog.ValueSelector)
							 });
			
			resultTypeDialog.Confirmable = false;
			
			resultTypeDialog.Items[0].ValueChanged += new EventHandler(ResultValueChanged);
			resultTypeDialog.Items[1].ValueChanged += new EventHandler(ResultDirectionChanged);
			resultTypeDialog.Items[2].RightToLeft = RightToLeft.No;
		}

		public ResultTypeRuleEditor()
		{
		}

		public override Control Edit(Rule rule, RuleScope scope)
		{
			_rule = rule;
			_scope = scope;

			Sport.UI.Controls.ButtonBox bb = new Sport.UI.Controls.ButtonBox();
			bb.Value = rule[scope];
			bb.ValueSelector = new Sport.UI.Controls.ButtonBox.SelectValue(ResultTypeSelection);
			bb.ValueChanged += new EventHandler(ResultTypeChanged);

			return bb;
		}

		public override void EndEdit()
		{
			_rule = null;
		}

		private Rule _rule;
		private RuleScope _scope;

		private void ResultTypeChanged(object sender, EventArgs e)
		{
			Sport.UI.Controls.ButtonBox bb = sender as Sport.UI.Controls.ButtonBox;
			if (bb != null && _rule != null)
			{
				_rule.Set(_scope, bb.Value);
			}
		}

		private object ResultTypeSelection(Sport.UI.Controls.ButtonBox buttonBox, object value)
		{
			ResultType st = value as ResultType;

			if (st == null)
			{
				resultTypeDialog.Items[0].Value = null;
				resultTypeDialog.Items[1].Value = null;
				resultTypeDialog.Items[2].Value = null;
				resultTypeDialog.Items[2].Enabled = false;
			}
			else
			{
				resultTypeDialog.Items[0].Value = valueItems[(int) st.Value];
				resultTypeDialog.Items[1].Value = directionItems[(int) st.Direction];
				resultTypeDialog.Items[2].Value = st.ValueFormatter;
				resultTypeDialog.Items[2].Enabled = true;
			}


			ResetConfirmable();

			if (resultTypeDialog.ShowDialog() == DialogResult.OK)
			{
				Sport.Core.Data.ResultValue v = (Sport.Core.Data.ResultValue) 
					((Sport.Data.LookupItem) resultTypeDialog.Items[0].Value).Id;
				Sport.Core.Data.ResultDirection d = (Sport.Core.Data.ResultDirection) 
					((Sport.Data.LookupItem) resultTypeDialog.Items[1].Value).Id;
				string resultFormat = resultTypeDialog.Items[2].Value == null ? null : resultTypeDialog.Items[2].Value.ToString();

				return new ResultType(v, d, resultFormat);
			}

			return value;
		}

		private static void ResetConfirmable()
		{
			resultTypeDialog.Confirmable = resultTypeDialog.Items[0].Value != null &&
				resultTypeDialog.Items[1].Value != null;
		}

		private static void ResultValueChanged(object sender, EventArgs e)
		{
			if (resultTypeDialog.Items[0].Value == null)
			{
				resultTypeDialog.Items[2].Value = null;
				resultTypeDialog.Items[2].Enabled = false;
			}
			else
			{
				Sport.Core.Data.ResultValue v = (Sport.Core.Data.ResultValue) 
					((Sport.Data.LookupItem) resultTypeDialog.Items[0].Value).Id;

				Sport.Common.ValueFormatter vf = resultTypeDialog.Items[2].Value as Sport.Common.ValueFormatter;

				switch (v)
				{
					case (Sport.Core.Data.ResultValue.Time):
						vf = new Sport.Common.ValueFormatter(Sport.Common.ValueFormatter.TimeValueFormatters, 
							vf == null ? null : vf.Format);
						break;
					case (Sport.Core.Data.ResultValue.Distance):
						vf = new Sport.Common.ValueFormatter(Sport.Common.ValueFormatter.DistanceValueFormatters, 
							vf == null ? null : vf.Format);
						break;
					case (Sport.Core.Data.ResultValue.Points):
						vf = new Sport.Common.ValueFormatter(Sport.Common.ValueFormatter.PointValueFormatters, 
							vf == null ? null : vf.Format);
						break;
				}

				resultTypeDialog.Items[2].Value = vf;
				resultTypeDialog.Items[2].Enabled = true;
			}

			ResetConfirmable();
		}

		private static void ResultDirectionChanged(object sender, EventArgs e)
		{
			ResetConfirmable();
		}
	}
}
