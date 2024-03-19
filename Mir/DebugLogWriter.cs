using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mir.Common
{
	public class DebugLogWriter : ILogWriter
	{
		public string GetLastError()
		{
			return string.Empty;
		}

		public Guid WriteToLog(LogType type, string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
			return Guid.Empty;
		}

		public void WriteToLog(Guid logItemID, LogType type, string message)
		{
			WriteToLog(type, message);
		}
	}
}
