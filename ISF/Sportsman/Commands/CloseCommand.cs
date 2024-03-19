using System;

namespace Sportsman.Commands
{
	/// <summary>
	/// Summary description for CloseCommand.
	/// </summary>
	public class CloseCommand : Sport.UI.Command
	{
		public CloseCommand()
		{
		}

		public override void Execute(string param)
		{
			int viewsCount=Sport.UI.ViewManager.ViewsCount();
			if (viewsCount > 0)
			{
				if (!Sport.UI.MessageBox.Ask(
					Sportsman.ExitMessage.Replace("%views", viewsCount.ToString()), 
					System.Windows.Forms.MessageBoxIcon.Question, false))
					return;
			}
			//getting here means either there are no views open or user confirmed exit:
			Sportsman.IsClosing = true;
			int vv=0;
			while (Sport.UI.ViewManager.ViewsCount() > 0)
			{
				if (vv > 99)
					break;
				Sport.UI.View view=Sport.UI.ViewManager.GetView(0);
				Sport.UI.ViewManager.CloseView(view);
				vv++;
			}
			int visible=(Sport.UI.Display.StateManager.States["messagebar"].Checked)?1:0;
			int calendar=(Sport.UI.Display.StateManager.States["calendarbar"].Checked)?1:0;
			if (Sport.Core.Configuration.ReadString("General", "MessageBarVisible") != visible.ToString())
				Sport.Core.Configuration.WriteString("General", "MessageBarVisible", visible.ToString());
			if (Sport.Core.Configuration.ReadString("General", "CalendarBarVisible") != calendar.ToString())
				Sport.Core.Configuration.WriteString("General", "CalendarBarVisible", calendar.ToString());
			Sportsman.Context.CloseAll();
			Sportsman.IsClosing = false;
		}

		public override bool IsPermitted(string param)
		{
			return true;
		}

	}
}
