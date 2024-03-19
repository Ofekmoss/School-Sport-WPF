using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mir.Common
{
	public enum LogType
	{
		Unspecified = 0,
        Debug,
		Information,
		Warning,
		Error
	}

	public interface ILogWriter
	{
		string GetLastError();
		Guid WriteToLog(LogType type, string message);
		void WriteToLog(Guid logItemID, LogType type, string message);
	}
}
