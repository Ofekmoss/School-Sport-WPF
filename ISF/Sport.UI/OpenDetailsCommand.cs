using System;
using System.Windows.Forms;

namespace Sport.UI
{
	/// <summary>
	/// Summary description for OpenDetailsCommand.
	/// </summary>
	public class OpenDetailsCommand : Command
	{
		public OpenDetailsCommand()
		{
		}
		
		public override void Execute(string param)
		{
			string[] paramArray = param.Split(new char[]{'?'});

			if (paramArray.Length == 0)
				throw new Exception("Invalid dialog string");
			
			Type type = Type.GetType(paramArray[0]);

			if (type == null)
				throw new Exception("Failed to find details type");

			object o = Activator.CreateInstance(type);
			if (o is System.Windows.Forms.Form)
			{
				Form objForm = (Form) o;
				objForm.ShowDialog();
			}
		}
	}
}
