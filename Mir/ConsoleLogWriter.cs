using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mir.Common
{
	public class ConsoleLogWriter : ILogWriter
	{
		public string GetLastError()
		{
			return string.Empty;
		}

		public Guid WriteToLog(LogType type, string message)
		{
			Console.WriteLine("[" + type.ToString() + "] " + message);
			return Guid.Empty;
		}

		public void WriteToLog(Guid logItemID, LogType type, string message)
		{
			WriteToLog(type, message);
		}
	}
}
