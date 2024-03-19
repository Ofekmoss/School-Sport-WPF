using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Mir.Common
{
	public class WindowsEventLogLogWriter : ILogWriter
	{
		private string lastError = string.Empty;

		public string GetLastError()
		{
			return lastError;
		}

		public Guid WriteToLog(LogType type, string message)
		{
			WriteToLog(Guid.Empty, type, message);
			return Guid.Empty;
		}

		public void WriteToLog(Guid logItemID, LogType type, string message)
		{
			lastError = string.Empty;

			try
			{
				if (!EventLog.SourceExists("DNC2010"))
					EventLog.CreateEventSource("DNC2010", "Application");
			}
			catch (Exception ex)
			{
				lastError = "Error creating event source: " + ex.ToString();
				return;
			}

			EventLog eventLog = new EventLog();
			//eventLog.RegisterDisplayName(
			eventLog.Source = "DNC2010";
			EventLogEntryType entryType = EventLogEntryType.FailureAudit;
			switch (type)
			{
				case LogType.Error:
					entryType = EventLogEntryType.Error;
					break;
				case LogType.Information:
					entryType = EventLogEntryType.Information;
					break;
				case LogType.Warning:
					entryType = EventLogEntryType.Warning;
					break;
			}
			try
			{
				eventLog.WriteEntry(message, entryType);
			}
			catch (Exception ex)
			{
				lastError = "Error writing log entry: " + ex.ToString();
			}
		}
	}
}
