using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Mir.Common
{
	public class TextFileLogWriter : ILogWriter
	{
		private string filePath = null;
        private string dailyPathFormat = "";
		private string localIpAddress = null;

		private string lastError = string.Empty;
        private bool dailyLog = false;

		public string TimeStampFormat { get; set; }
		public int MaxSizeBytes { get; set; }
		public bool UseHourlyLog { get; set; }

        public string FilePath
        {
            get
            {
                if (dailyLog && dailyPathFormat.Length > 0)
                {
					string format = "ddMMyyyy";
					if (this.UseHourlyLog)
						format += "_HH";
                    return string.Format(dailyPathFormat, DateTime.Now.ToString(format));
                }
                else
                {
                    return filePath;
                }
            }
        }

        public bool DailyLog { get { return dailyLog; } }

		public TextFileLogWriter(string path, bool mapPath, bool daily)
		{
			this.TimeStampFormat = "dd/MM/yyyy HH:mm:ss";
			this.MaxSizeBytes = 0;
			this.UseHourlyLog = false;
            this.dailyLog = daily;
			if (mapPath)
				this.filePath = Utils.Instance.MapPath(path);
			else
				this.filePath = path;

            if (this.DailyLog)
                this.dailyPathFormat = Path.Combine(Path.GetDirectoryName(this.filePath), Path.GetFileNameWithoutExtension(this.filePath) + "_{0}" + Path.GetExtension(this.filePath));
		}

        public TextFileLogWriter(string path, bool mapPath)
            : this(path, mapPath, false)
        {
        }

		public TextFileLogWriter(string path)
			: this(path, false)
		{
		}

		public Guid WriteToLog(LogType type, string message)
		{
			return WriteToLog(type, message, false); //true
		}

		public void WriteToLog(Guid logItemID, LogType type, string message)
		{
			if (logItemID.Equals(Guid.Empty))
			{
				WriteToLog(type, message);
				return;
			}

			message = "{ in addition to " + logItemID.ToString("N") + " } " + message;
			WriteToLog(type, message, false);
		}

		protected Guid WriteToLog(LogType type, string message, bool appendGuid)
		{
            string path = this.FilePath;

			//maybe no path is defined?
            if (string.IsNullOrEmpty(path))
				return Guid.Empty;

			lastError = string.Empty;
			Guid guid = appendGuid ? Guid.NewGuid() : Guid.Empty;
			try
			{
                if (!this.DailyLog)
                {
                    //maybe file exceeded maximum size?
                    if (this.MaxSizeBytes > 0 && File.Exists(path))
                    {
                        if ((int)(new FileInfo(path)).Length > this.MaxSizeBytes)
                        {
                            //backup and rewrite
                            string newPath = Path.Combine(Path.GetDirectoryName(path)
                                , Path.GetFileNameWithoutExtension(path) + "_prev" + Path.GetExtension(path));
                            File.Copy(path, newPath, true);
                            File.WriteAllText(path, "");
                        }
                    }
                }

				//add stuff as prefix to the message..
				string prefix = string.Empty;

				//time stamp:
				if (!string.IsNullOrEmpty(this.TimeStampFormat))
					prefix += "[" + DateTime.Now.ToString(this.TimeStampFormat) + "] ";

				//machine address in case of storing the log file on external place:
				if (path.StartsWith("\\"))
				{
					if (localIpAddress == null)
						localIpAddress = Utils.GetLocalIpAddress();
					if (localIpAddress.Length > 0)
						prefix += "[" + localIpAddress + "] ";
				}

				//category:
				if (type != LogType.Unspecified)
					prefix += "[" + type.ToString() + "] ";
				message = prefix + MakeFitForLog(message);
				if (appendGuid)
					message += " { event id: " + guid.ToString("N") + " }";

				//append message at end of file, replace new lines to avoid mess:
                File.AppendAllText(path, message + Environment.NewLine);
			}
			catch (Exception ex)
			{
				lastError = "General error during WriteToLog: " + ex.ToString();
			}

			return (lastError.Length > 0) ? Guid.Empty : guid;
		}

		/// <summary>
		/// Replace any newline characters with double tab
		/// </summary>
		protected string MakeFitForLog(string message)
		{
			return message.Replace("\n\r", "\n").Replace("\r\n", "\n").Replace("\r", "").Replace("\n", "\t\t");
		}

		public string GetLastError()
		{
			return lastError;
		}
	}
}
