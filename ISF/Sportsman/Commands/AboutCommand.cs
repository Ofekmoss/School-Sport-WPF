using System;

namespace Sportsman.Commands
{
	/// <summary>
	/// Summary description for AboutCommand.
	/// </summary>
	public class AboutCommand : Sport.UI.Command
	{
		public AboutCommand()
		{
		}

		public override void Execute(string param)
		{
			//Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter((int)Sport.Entities.Player.Fields.Team, new int[] {7221, 9758, 8321});
			//Sport.Data.Entity[] players = Sport.Entities.Player.Type.GetEntities(filter);
			//Sport.UI.MessageBox.Show(players.Length.ToString());


			Sport.UI.Dialogs.AboutForm dlgAbout = new Sport.UI.Dialogs.AboutForm();
			if (Sport.Core.Session.Connected)
			{
				SessionServices.SessionService service = new SessionServices.SessionService();
				dlgAbout.LastestVersion = service.GetLatestVersion().VersionNumber;
				dlgAbout.ServiceUrl = service.Url;
			}
			else
			{
				dlgAbout.LastestVersion = Sport.Common.Tools.CDblDef(
					Sport.Core.Configuration.ReadString("General", "LastVersion"), 0);
				dlgAbout.ServiceUrl = "";
			}
			dlgAbout.CurrentVersion = Sport.Core.Data.CurrentVersion;
			dlgAbout.ShowDialog();
		}

		public override bool IsPermitted(string param)
		{
			return true;
		}

	}
}
