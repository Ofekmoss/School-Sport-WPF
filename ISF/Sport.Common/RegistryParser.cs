using System;
using Microsoft.Win32;

namespace Sport.Common
{
	/// <summary>
	/// Used to read and write to the registry
	/// </summary>
	/// 

    public class RegistryParser
	{
		/***** MOVED TO SPORT.CORE namespace *****/
		/*
		public const int REG_ERR_NULL_REFERENCE = 12;
		public const int REG_ERR_SECURITY = 13; 
		public const int REG_ERR_UNKNOWN = 14; 
		public enum REGUTIL_KEYS
		{
			HKCR = 0,
			HKCU = 1,
			HKLM = 2,
			HKU = 3,
			HKCC = 4
		}
      
		char[] Delimit = {'\x005c'};  //Hex for '\'

		public RegistryParser()
		{
		}

		public string getValue(REGUTIL_KEYS Key, string RegPath, string KeyName, string DefaultValue)
		{
			string[] RegString;
			string Result = ""; 
			RegString = RegPath.Split(Delimit);
			RegistryKey[] RegKeys = new RegistryKey[RegPath.Length + 1];
			RegKeys[0] = SelectKey(Key);

			for(int i = 0;i < RegString.Length;i++)
			{
				RegKeys[i + 1] = RegKeys[i].OpenSubKey(RegString[i]);
				if (RegKeys[i + 1] == null)
					return DefaultValue;
				if (i  == RegString.Length - 1 )
					Result = (string)RegKeys[i + 1].GetValue(KeyName, DefaultValue); 
			}
			return Result;
		}
		public int setValue(REGUTIL_KEYS Key, string RegPath, string KeyName, string KeyValue)
		{
			string[] RegString;
   			RegString = RegPath.Split(Delimit);
			RegistryKey[] RegKeys = new RegistryKey[RegPath.Length + 1];
			RegKeys[0] = SelectKey(Key);
			try
			{
				for(int i = 0;i < RegString.Length;i++)
				{
					RegKeys[i + 1] = RegKeys[i].OpenSubKey(RegString[i], true);
					if (RegKeys[i + 1] == null) 
						RegKeys[i + 1] = RegKeys[i].CreateSubKey(RegString[i]);
				}
				RegKeys[RegString.Length].SetValue(KeyName, KeyValue);   
			}
			catch (System.NullReferenceException)
			{
				return REG_ERR_NULL_REFERENCE;
			}
			catch (System.UnauthorizedAccessException)
			{
				return REG_ERR_SECURITY;
			}
			return 0;
		}

		private RegistryKey SelectKey(REGUTIL_KEYS Key)
		{
			switch (Key)
			{
				case REGUTIL_KEYS.HKCR:
					return Registry.ClassesRoot;
				case REGUTIL_KEYS.HKCU:
					return Registry.CurrentUser;
				case REGUTIL_KEYS.HKLM:
					return Registry.LocalMachine;
				case REGUTIL_KEYS.HKU:
					return Registry.Users;
				case REGUTIL_KEYS.HKCC:
					return Registry.CurrentConfig;
				default:
					return Registry.CurrentUser;
			}
		}
		*/
	}
}
