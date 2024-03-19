using System;

namespace Sport.UI
{
	/// <summary>
	/// Summary description for Command.
	/// </summary>
	public abstract class Command
	{
		public Command()
		{
		}

		public abstract void Execute(string param);
		public abstract bool IsPermitted(string param);
	}
}
