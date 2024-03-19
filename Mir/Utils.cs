using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace Mir.Common
{
	public class Utils
	{
		private static Utils instance;

		public static Utils Instance { get { return instance; } }

		private Utils()
		{
		}

		static Utils()
		{
			instance = new Utils();
		}

		public Dictionary<string, string> ReadMappingFile(string filePath, out string error)
		{
			error = "";
			if (string.IsNullOrEmpty(filePath))
			{
				error = "empty file path";
				return null;
			}

			if (!File.Exists(filePath))
			{
				error = "file '" + filePath + "' does not exist";
				return null;
			}

			string[] lines = null;
			try
			{
				lines = File.ReadAllLines(filePath);
			}
			catch (Exception ex)
			{
				error = "error reading file '" + filePath + "': " + ex.Message;
				return null;
			}

			if (lines == null)
				return null;

			Dictionary<string, string> mapping = new Dictionary<string, string>();
			lines.ToList().ConvertAll(l => l.Trim()).FindAll(l => l.Length > 0).ForEach(currentLine =>
			{
				string[] parts = currentLine.Split('=');
				if (parts.Length == 2)
				{
					string key = parts[0];
					if (key.Length > 0 && !mapping.ContainsKey(key))
						mapping.Add(key, parts[1]);
				}
			});
			return mapping;
		}

		public Type[] LoadTypesFromAssemblies(string[] files, string referer, bool writeToLog)
		{
			List<Type> allTypes = new List<Type>();
			files.ToList().ForEach(f =>
			{
				if (writeToLog) Logger.Instance.WriteLog(LogType.Information, referer, "processing assembly file " + f + "...");

				Assembly assembly = null;
				try
				{
                    assembly = Assembly.LoadFrom(f);
				}
				catch (Exception ex)
				{
					if (writeToLog) Logger.Instance.WriteLog(LogType.Warning, referer, "Failed to load assembly file, error is: " + ex.Message);
				}

				if (assembly != null)
				{
					Type[] types = null;
					try
					{
						types = assembly.GetTypes();
					}
					catch (ReflectionTypeLoadException rtle)
					{
						if (writeToLog) Logger.Instance.WriteLog(LogType.Warning, referer, "Failed to load types: \n" + string.Join("\n", rtle.LoaderExceptions.ToList().ConvertAll(e => { return e.Message; }).ToArray()));
					}
					catch (Exception ex)
					{
						if (writeToLog) Logger.Instance.WriteLog(LogType.Warning, referer, "General error while loading types: " + ex.Message);
					}

					if (types != null)
					{
						if (writeToLog) Logger.Instance.WriteLog(LogType.Information, referer, "assembly file loaded successfully and contains total of " + types.Length + " classes");

						allTypes.AddRange(types);
					}
				}
			});
			return allTypes.ToArray();
		}

		public string GetExternalConfigAppSetting(string filePath, string key, out string errMsg)
		{
			errMsg = "";

			if (!File.Exists(filePath))
			{
				errMsg = "File '" + filePath + "' does not exist";
				return null;
			}
			
			System.Configuration.Configuration config = null;
			try
			{
				config = ConfigurationManager.OpenExeConfiguration(filePath);
			}
			catch (Exception ex)
			{
				errMsg = "Error opening configuration: " + ex.Message;
			}

			if (config != null)
			{
				KeyValueConfigurationElement element = config.AppSettings.Settings[key];
				if (element != null)
				{
					return element.Value;
				}
				else
				{
					errMsg = string.Format("No such key '{0}' in file '{1}'", key, filePath);
				}
			}

			return "";
		}

		public string GetExternalConfigAppSetting(string filePath, string key)
		{
			string dummy;
			return GetExternalConfigAppSetting(filePath, key, out dummy);
		}

		public bool IsValidEmailAddress(string email)
		{
			if (email == null || email.Length == 0)
				return false;
			string[] parts = email.Split('@');
			if (parts.Length != 2 || parts[0].Length == 0 || parts[1].Length == 0)
				return false;
			string domain = parts[1];
			int index = domain.IndexOf(".");
			return index > 0 && index < (domain.Length - 1);
		}

		public bool IsRemoteDirectoryResponding(string path)
		{
			if (!path.StartsWith("\\\\"))
				throw new ArgumentException("Path must be network path", "path");
			int index = path.IndexOf('\\', 2);
			string machineName = (index > 0) ? path.Substring(2, index - 2) : path.Substring(2);
			ProcessStartInfo info = new ProcessStartInfo("ping")
			{
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				Arguments = machineName,
				CreateNoWindow = true
			};
			Process process = Process.Start(info);
			process.EnableRaisingEvents = true;
			bool responding = false;
			int timeOutTryCount = 0;
			int timeOutTryLimit = 3;
			process.OutputDataReceived += (s, drea) =>
			{
				try
				{
					string data = drea.Data + "";
					if (data.IndexOf("bytes=", StringComparison.CurrentCultureIgnoreCase) > 0)
					{
						responding = true;
						process.Kill();
					}
					else if (data.IndexOf("timed out", StringComparison.CurrentCultureIgnoreCase) > 0 ||
						data.IndexOf("unreachable", StringComparison.CurrentCultureIgnoreCase) > 0)
					{
						timeOutTryCount++;
						if (timeOutTryCount >= timeOutTryLimit)
						{
							responding = false;
							process.Kill();
						}
					}
				}
				catch
				{
					//Logger.Instance.WriteLog(LogType.Warning, "IsRemoteDirectoryResponding", "Error upon data receieved: " + ex.ToString());
					responding = true;
				}
			};
			process.BeginOutputReadLine();
			process.WaitForExit();
			return responding;
		}

		public static string GetLocalIpAddress()
		{
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress address in host.AddressList)
			{
				if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
					return address.ToString();
			}

			return string.Empty;
		}

        public static string GetAppDataPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MIR");
        }

		public static Process FindDuplicateProcess()
		{
			Process currentlyRunningProcess = Process.GetCurrentProcess();
			string currentlyRunningFileName = currentlyRunningProcess.MainModule.FileName;
			Process otherProcess = null;
			List<Process> allProcesses = Process.GetProcesses().ToList();
			for (int i = 0; i < allProcesses.Count; i++)
			{
				Process curProcess = allProcesses[i];

				//might be 32 vs. 64 bit issue
				string curFileName = "";
				int curId = 0;
				try
				{
					curFileName = curProcess.MainModule.FileName;
					curId = curProcess.Id;
				}
				catch
				{
					curFileName = "";
				}

				if (curId == currentlyRunningProcess.Id)
					continue;

				if (curFileName.Length > 0 && curFileName.Equals(currentlyRunningFileName, StringComparison.CurrentCultureIgnoreCase))
				{
					otherProcess = curProcess;
					break;
				}
			}
			return otherProcess;
		}

		/// <summary>
		/// add the directory of active process to given file name.
		/// </summary>
		public string MapPath(string fileName)
		{
			return Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), fileName);
		}

        public static string GetTimestamp(DateTime modificationDate)
        {
            return modificationDate.ToString("yyyyMMddHHmmss");
        }

        public static string GetTimestamp(int year, int month, int day, int hour,
            int minute, int second)
        {
            return string.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}",
                year, month, day, hour, minute, second);
        }

        public static string GetTimestamp(int unixtime)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return GetTimestamp(dt.AddSeconds(unixtime));
        }

        public static DateTime? ParseTimestamp(string modificationDate)
        {
            int year, month, day, hour, minute, second;
            if (modificationDate.Length == 14 &&
                int.TryParse(modificationDate.Substring(0, 4), out year) &&
                int.TryParse(modificationDate.Substring(4, 2), out month) &&
                int.TryParse(modificationDate.Substring(6, 2), out day) &&
                int.TryParse(modificationDate.Substring(8, 2), out hour) &&
                int.TryParse(modificationDate.Substring(10, 2), out minute) &&
                int.TryParse(modificationDate.Substring(12, 2), out second))
            {
                try
                {
                    return new DateTime(year, month, day, hour, minute, second);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// \\ => \
        /// \n => LF
        /// \r => CR
        /// \t => TAB
        /// \0 => null
        /// \x##; => Hex ##
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ParseEscapedString(string s)
        {
            if (s == null)
                return null;
            string parsed = string.Empty;
            int i = 0;
            int n = s.IndexOf('\\');
            while (n >= 0)
            {
                parsed += s.Substring(i, n - i);
                n++;
                if (n < s.Length)
                {
                    switch (s[n])
                    {
                        case '\\':
                            parsed += "\\";
                            break;
                        case 'n':
                            parsed += "\n";
                            break;
                        case 'r':
                            parsed += "\r";
                            break;
                        case 't':
                            parsed += "\t";
                            break;
                        case '0':
                            parsed += "\0";
                            break;
                        case 'x':
                            {
                                int e = s.IndexOf(';', n + 1);
                                if (e > n + 1)
                                {
                                    string hex = s.Substring(n + 1, e - n - 1);
                                    int value;
                                    if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out value))
                                    {
                                        parsed += (char)value;
                                        n += hex.Length + 1;
                                    }
                                    else
                                    {
                                        parsed += "\\" + s[n];
                                    }
                                }
                                else
                                {
                                    parsed += "\\" + s[n];
                                }
                            }
                            break;
                        default:
                            parsed += "\\" + s[n];
                            break;
                    }
                }
                else
                {
                    parsed += "\\";
                    break;
                }
                i = n + 1;
                n = s.IndexOf('\\', i);
            }

            if (i < s.Length)
            {
                parsed += s.Substring(i);
            }
            return parsed;
        }

		public static T[] GetEnumItems<T>()
		{
			return Enum.GetNames(typeof(T)).ToList().ConvertAll(r => (T)Enum.Parse(typeof(T), r)).ToArray();
		}
	}
}
