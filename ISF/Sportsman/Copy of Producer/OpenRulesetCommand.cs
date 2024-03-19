using System;
using Sportsman.Views;
using System.Windows.Forms;
using Sport.UI;
using Sport.UI.Dialogs;
using Sport.Rulesets;

namespace Sportsman.Producer
{
	public class OpenRulesetCommand : Sport.UI.Command
	{
		public OpenRulesetCommand()
		{
		}

		public override void Execute(string param)
		{
			Sport.UI.View sv = ViewManager.SelectedView;
			if (sv != null)
				sv.Deactivate();

			TableView tv = new RulesetsTableView();
			EntitySelectionDialog esd = new EntitySelectionDialog(tv, "ערוך", "סגור");
			tv.Editable = true;
			tv.ToolBarEnabled = true;
			tv.FilterBarEnabled = false;

			if (esd.ShowDialog() == DialogResult.OK)
			{
				if (esd.Entity != null)
				{
					// Making sure selected ruleset is loadable
					Sport.UI.Dialogs.WaitForm.ShowWait("טוען תקנון '" + esd.Entity.Name + "'...");
					Ruleset ruleset = Ruleset.LoadRuleset(esd.Entity.Id);
					if (ruleset == null)
					{
						Sport.UI.MessageBox.Show("כשלון בטעינת תקנון");
					}
					else
					{
						ViewManager.OpenView(typeof(RulesetEditorView), "ruleset=" + esd.Entity.Id.ToString());
					}
					Sport.UI.Dialogs.WaitForm.HideWait();
				}
			}
			else if (sv != null)
				sv.Activate();
		}

		public override bool IsPermitted(string param)
		{
			return Sport.UI.View.IsViewPermitted(typeof(Views.RulesetsTableView));
		}
	}
}
