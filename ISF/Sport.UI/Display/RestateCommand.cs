using System;

namespace Sport.UI.Display
{
	/// <summary>
	/// Summary description for RestateCommand.
	/// </summary>
	public class RestateCommand : Command
	{
		public RestateCommand()
		{
		}

		public override void Execute(string param)
		{
			StateItem si = StateManager.States[param];
			
			if (si != null)
				si.Checked = !si.Checked;
		}

		public override bool IsPermitted(string param)
		{
			return true;
		}

	}
}
