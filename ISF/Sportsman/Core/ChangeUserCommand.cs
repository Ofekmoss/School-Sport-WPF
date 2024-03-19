using System;

namespace Sportsman.Core
{
	public class ChangeUserCommand : Sport.UI.Command
	{
		public ChangeUserCommand()
		{
		}
		
		public override void Execute(string param)
		{
			if (Core.UserManager.PerformLogin())
			{
				Sport.Entities.Season season = new Sport.Entities.Season(Sport.Core.Session.Season);
				Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Season, "עונה: " + season.Name);
				
				Sport.Entities.Region region = new Sport.Entities.Region(Core.UserManager.CurrentUser.UserRegion);
				Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.Region, "מחוז: " + region.Name);
				
				Sportsman.Context.SetStatusBar(Forms.MainForm.StatusBarPanels.User, "משתמש: " + Core.UserManager.CurrentUser.Name);
			}
		}

		public override bool IsPermitted(string param)
		{
			return true;
		}
	}
}
