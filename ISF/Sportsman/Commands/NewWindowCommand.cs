using System;

namespace Sportsman.Commands
{
	/// <summary>
	/// Summary description for NewWindowCommand.
	/// </summary>
	public class NewWindowCommand : Sport.UI.Command
	{
		public NewWindowCommand()
		{
		}

		public override void Execute(string param)
		{
			Forms.MainForm mainForm = new Forms.MainForm();
			mainForm.Show();
			Sportsman.Context.Add(mainForm);
		}

		public override bool IsPermitted(string param)
		{
			return true;
		}

	}
}
