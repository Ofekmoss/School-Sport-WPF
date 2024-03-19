using System;
using System.Windows.Forms;

namespace Sport.UI
{
	/// <summary>
	/// Summary description for OpenDialogCommand.
	/// </summary>
	public class OpenDialogCommand : Command
	{
		public OpenDialogCommand()
		{
		}

		public void Execute(Type type, string state)
		{
			if (type == null)
				return;
			
			object o = null;
			try
			{
				o = Activator.CreateInstance(type);
			}
			catch
			{
				Sport.UI.MessageBox.Error("שגיאה בעת יצירת מסך אנא דווח אם בעיה זו חוזרת על עצמה" + "\n" + 
					type.FullName + "\n" + state, "שגיאה כללית");
			}

			if (o == null)
				return;
			
			if (o is UI.View)
			{
				View sv = ViewManager.SelectedView;
				if (sv != null)
					sv.Deactivate();

				View v = (View) o;

				if (state != null)
					v.State = new ParameterList(state);

				DialogForm form = new DialogForm(v);

				form.ShowDialog();
					
				if (sv != null)
					sv.Activate();
			}
			else if (o is Form)
			{
				((Form) o).ShowDialog();
			}
		}

		public override void Execute(string param)
		{
			string[] paramArray = param.Split(new char[]{'?'});

			if (paramArray.Length == 0)
				throw new Exception("Invalid dialog string");

			Type type = Type.GetType(paramArray[0]);

			if (type == null)
				throw new Exception("Failed to find view type");

			Execute(type, paramArray.Length > 1 ? paramArray[1] : null);
		}

		public override bool IsPermitted(string param)
		{
			string[] paramArray = param.Split(new char[]{'?'});

			if (paramArray.Length == 0)
				throw new Exception("Invalid dialog string");

			Type type = Type.GetType(paramArray[0]);

			if (type == null)
			{
				System.Diagnostics.Debug.WriteLine("failed to fnd view type: "+paramArray[0]);
				throw new Exception("Failed to find view type");
			}

			return View.IsViewPermitted(type);
		}

	}
}
