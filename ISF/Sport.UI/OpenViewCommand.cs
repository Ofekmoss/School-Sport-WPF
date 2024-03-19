using System;

namespace Sport.UI
{
	/// <summary>
	/// Summary description for OpenViewCommand.
	/// </summary>
	public class OpenViewCommand : Command
	{
		public OpenViewCommand()
		{
		}
	
		public override void Execute(string param)
		{
			ViewManager.OpenView(param);
		}

		public override bool IsPermitted(string param)
		{
			string[] paramArray = param.Split(new char[]{'?'});

			if (paramArray.Length == 0)
				throw new Exception("Invalid view string");

			Type type = Type.GetType(paramArray[0]);

			if (type == null)
				throw new Exception("Failed to find view type");

			return View.IsViewPermitted(type);
		}

	}
}
