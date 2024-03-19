using System;

namespace Sportsman.Producer
{
	public class OpenGameBoardCommand : Sport.UI.Command
	{
		public OpenGameBoardCommand()
		{
		}

		public override void Execute(string param)
		{
			Sport.UI.View sv = Sport.UI.ViewManager.SelectedView;
			if (sv != null)
				sv.Deactivate();

			Sport.UI.TableView tv = new Views.GameBoardsTableView();
			Sport.UI.EntitySelectionDialog esd = new Sport.UI.EntitySelectionDialog(tv, "ערוך", "סגור");
			tv.Editable = true;
			tv.ToolBarEnabled = true;
			tv.FilterBarEnabled = false;

			if (esd.ShowDialog() == System.Windows.Forms.DialogResult.OK && esd.Entity != null)
			{
				Sport.UI.ViewManager.OpenView(typeof(GameBoardEditorView), "board=" + esd.Entity.Id.ToString());
			}
			else if (sv != null)
				sv.Activate();
		}

		public override bool IsPermitted(string param)
		{
			return Sport.UI.View.IsViewPermitted(typeof(Views.GameBoardsTableView));
		}

	}
}
