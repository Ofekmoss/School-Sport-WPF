using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mir.Common
{
	public class WindowsFormsDialogLogWriter : ILogWriter
	{
		public string GetLastError()
		{
			return string.Empty;
		}

		public Guid WriteToLog(LogType type, string message)
		{
			string caption = type.ToString();
			MessageBoxIcon icon = MessageBoxIcon.None;
			switch (type)
			{
				case LogType.Error:
					icon = MessageBoxIcon.Error;
					break;
				case LogType.Information:
					icon = MessageBoxIcon.Information;
					break;
				case LogType.Warning:
					icon = MessageBoxIcon.Warning;
					break;
			}
			MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
			return Guid.Empty;
		}

		public void WriteToLog(Guid logItemID, LogType type, string message)
		{
			WriteToLog(type, message);
		}
	}
}
