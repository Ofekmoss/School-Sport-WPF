using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Mir.Common
{
	public class Logger
	{
		private static Logger instance;

		private static object locker = new object();

        public static Logger Instance { get { return instance; } }
        
        private List<ILogWriter> writers = new List<ILogWriter>();
		private int configLogReportLevel = 0;
        private readonly Dictionary<string, int> m_CategoryReportLevel = new Dictionary<string, int>();


		private Logger()
		{
			string sConfigValue = ConfigurationManager.AppSettings["LogReportLevel"] + "";
			int reportLevel;
			if (sConfigValue.Length > 0 && Int32.TryParse(sConfigValue, out reportLevel) && reportLevel > 0)
				configLogReportLevel = reportLevel;
			else
				configLogReportLevel = 40;

            string[] categoryLogLevels = ConfigurationManager.AppSettings.AllKeys.Where(x => x.StartsWith("LogLevel:")).ToArray();
            foreach (string categoryLogLevel in categoryLogLevels)
            {
                int s = categoryLogLevel.IndexOf(':');
                string levelString = ConfigurationManager.AppSettings[categoryLogLevel];
                int level;
                if (levelString != null &&
                    int.TryParse(levelString, out level) &&
                    s > 0)
                {
                    m_CategoryReportLevel[categoryLogLevel.Substring(s + 1)] = level;
                }
            }
		}

		static Logger()
		{
			instance = new Logger();
        }

		public int LogLevel
		{
            get
            {
                return configLogReportLevel;
            }
            set
            {
                if (value > 0)
                    configLogReportLevel = value;
            }
		}

        public IEnumerable<KeyValuePair<string, int>> CategoryLogLevels
        {
            get { return m_CategoryReportLevel; }
        }

        public void SetCategoryLogLevel(string category, int? level)
        {
            if (level.HasValue)
            {
                if (level.Value > 0)
                {
                    m_CategoryReportLevel[category] = level.Value;
                }
            }
            else
            {
                m_CategoryReportLevel.Remove(category);
            }
        }

		public void AddLogWriter(ILogWriter writer)
		{
			if (!writers.Contains(writer))
				writers.Add(writer);
		}

		public void RemoveLogWriter(ILogWriter writer)
		{
			if (writers.Contains(writer))
				writers.Remove(writer);
		}

		public int LogWriterCount
		{
			get
			{
				return writers.Count;
			}
		}

		/// <summary>
		/// Write the message with given type to all registered ILogWriter instances
		/// </summary>
		/// <param name="type"></param>
		/// <param name="message"></param>
		public void WriteLog(LogType type, string category, string message, int reportLevel)
		{
            lock (locker)
            {
                //check report level
                if (configLogReportLevel > 0 && reportLevel > configLogReportLevel)
                {
                    // Checking that this report level is not available for this category
                    int categoryLevel;
                    if (!m_CategoryReportLevel.TryGetValue(category, out categoryLevel) ||
                        reportLevel > categoryLevel)
                    {
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(category))
                    message = "[" + category + "] " + message;
                writers.ForEach(w => { w.WriteToLog(type, message); });
            }
		}

        /// <summary>
        /// Write the message with given type to all registered ILogWriter instances
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void WriteLog(LogType type, string category, Func<string> messageFunc, int reportLevel)
        {
            lock (locker)
            {
                //check report level
                if (configLogReportLevel > 0 && reportLevel > configLogReportLevel)
                {
                    // Checking that this report level is not available for this category
                    int categoryLevel;
                    if (!m_CategoryReportLevel.TryGetValue(category, out categoryLevel) ||
                        reportLevel > categoryLevel)
                    {
                        return;
                    }
                }

                string message = messageFunc();
                if (!string.IsNullOrEmpty(category))
                    message = "[" + category + "] " + message;
                writers.ForEach(w => { w.WriteToLog(type, message); });
            }
        }

		public void WriteLog(LogType type, string category, string message)
		{
			int reportLevel = 0;
			switch (type)
			{
                case LogType.Debug:
                    reportLevel = 100;
                    break;
				case LogType.Information:
					reportLevel = 20;
					break;
				case LogType.Warning:
					reportLevel = 10;
					break;
				case LogType.Error:
					reportLevel = 0;
					break;
			}
			WriteLog(type, category, message, reportLevel);
		}

		public void WriteLog(LogType type, string message, int reportLevel)
		{
			WriteLog(type, string.Empty, message, reportLevel);
		}

		public void WriteLog(LogType type, string message)
		{
			WriteLog(type, string.Empty, message);
		}
	}
}
