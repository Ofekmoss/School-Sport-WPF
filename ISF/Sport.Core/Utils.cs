using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace Sport.Core
{
	public enum PasswordStrength
	{
		None = 0,
		Trivial,
		Weak,
		Strong,
		Pentagon
	}

	public enum CharacterType
	{
		BigCase = 1,
		LowerCase,
		Digit,
		Punctuation,
		Other
	}

	public class CharacterOccuranceData
	{
		public int Count { get; set; }
		public List<int> Indices { get; set; }
		public CharacterType Type { get; set; }

		public int MaxAdjacent
		{
			get
			{
				int maxAdjacent = 0;
				if (Indices != null)
				{
					int currentAdjacentCount = 1;
					for (var i = 1; i < Indices.Count; i++)
					{
						if (Indices[i] == (Indices[i - 1] + 1))
						{
							currentAdjacentCount++;
						}
						else
						{
							if (currentAdjacentCount > maxAdjacent)
								maxAdjacent = currentAdjacentCount;
							currentAdjacentCount = 1;
						}
					}
					if (currentAdjacentCount > maxAdjacent)
						maxAdjacent = currentAdjacentCount;
				}
				return maxAdjacent;
			}
		}
	}

	public static class Extensions
	{
		public static List<T> AggregateAll<T>(this List<List<T>> list)
		{
			List<T> aggregated = new List<T>();
			list.ForEach(a => aggregated.AddRange(a));
			return aggregated;
		}

		public static string FirstWord(this string value)
		{
			if (value != null && value.Length > 0)
			{
				int index = value.IndexOf(" ");
				if (index > 0)
					return value.Substring(0, index);
			}
			return "";
		}
	}

	public static class Utils
	{
		private static bool _isRunningLocal = false;
		private static bool _isRunningLocalRead = false;

		public static char[] PunctuationCharacters = new char[] { ' ', '-', '.', '"', '\'', '`', ':', ',', '.' };

		public static bool IsRunningLocal()
		{
			if (!_isRunningLocalRead)
			{
				_isRunningLocal = Utils.GetAppSetting("local").Equals("1");
				_isRunningLocalRead = true;
			}
			return _isRunningLocal;
		}

		public static List<string> GetInstalledPrinters()
		{
			List<string> installedPrinters = new List<string>();
			try
			{
				foreach (string printerName in PrinterSettings.InstalledPrinters)
				{
					installedPrinters.Add(printerName);
				}
			}
			catch
			{
				
			}
			return installedPrinters;
		}

		public static FileStream GetCacheFile(string path, string name)
		{
			DirectoryInfo di = new DirectoryInfo(path);
			if (!di.Exists)
				di.Create();

			return new FileStream(path + "\\" + name, FileMode.OpenOrCreate);
		}

		public static string GetAppSetting(string key)
		{
			return ConfigurationManager.AppSettings[key] + "";
		}

		public static string JoinNonEmpty(string separator, string[] values)
		{
			List<string> arrNonEmptyValues = new List<string>();
			foreach (string value in values)
			{
				if (!string.IsNullOrEmpty(value))
				{
					arrNonEmptyValues.Add(value);
				}
			}
			return string.Join(separator, arrNonEmptyValues.ToArray());
		}

		public static CharacterType GetCharacterType(char character)
		{
			if (character >= 'A' && character <= 'Z')
				return CharacterType.BigCase;
			if (character >= 'a' && character <= 'z')
				return CharacterType.LowerCase;
			if (character >= '0' && character <= '9')
				return CharacterType.Digit;
			if (Array.IndexOf<char>(PunctuationCharacters, character) >= 0)
				return CharacterType.Punctuation;
			return CharacterType.Other;
		}

		public static Dictionary<char, CharacterOccuranceData> GetCharacterMapping(string value)
		{
			Dictionary<char, CharacterOccuranceData> mapping = new Dictionary<char, CharacterOccuranceData>();
			if (value != null)
			{
				for (var index = 0; index < value.Length; index++)
				{
					char character = value[index];
					if (!mapping.ContainsKey(character))
					{
						mapping.Add(character, new CharacterOccuranceData()
						{
							Count = 0,
							Indices = new List<int>(),
							Type = GetCharacterType(character)
						});
					}
					mapping[character].Count++;
					mapping[character].Indices.Add(index);
				}
			}
			return mapping;
		}

		public static PasswordStrength GetPasswordStrength(string password, string username)
		{
			if (password == null)
				password = "";
			password = password.Trim();
			while (password.IndexOf("  ") >= 0)
				password = password.Replace("  ", " ");
			if (password.Length == 0)
				return PasswordStrength.None;
			if (password.Length < 6)
				return PasswordStrength.Trivial;
			Dictionary<char, CharacterOccuranceData> characterMapping = GetCharacterMapping(password);
			if (characterMapping.Count < 3)
				return PasswordStrength.Trivial;
			Dictionary<CharacterType, int> typeMapping = new Dictionary<CharacterType, int>();
			foreach (CharacterOccuranceData cod in characterMapping.Values)
			{
				if (!typeMapping.ContainsKey(cod.Type))
					typeMapping.Add(cod.Type, 0);
				typeMapping[cod.Type]++;
			}
			if (typeMapping.Count < 2 && typeMapping.Keys.Cast<CharacterType>().ToList()[0] != CharacterType.Other)
				return PasswordStrength.Trivial;
			if (password.Length < 8 || typeMapping.Count < 3)
				return PasswordStrength.Weak;
			if (username != null && username.Length > 0)
			{
				if (password.ToLower().IndexOf(username.ToLower()) >= 0 || username.ToLower().IndexOf(password.ToLower()) >= 0)
					return PasswordStrength.Weak;
				if (username.Length >= 3)
				{
					for (int i = 0; i < password.Length - 3; i++)
					{
						if (username.ToLower().IndexOf(password.Substring(i, 3).ToLower()) >= 0)
						{
							return PasswordStrength.Weak;
						}
					}
				}
			}
			if (typeMapping.Count < 4)
				return PasswordStrength.Strong;
			foreach (CharacterOccuranceData cod in characterMapping.Values)
			{
				if (cod.MaxAdjacent >= 3)
				{
					return PasswordStrength.Strong;
				}
			}
			return PasswordStrength.Pentagon;
		}

		public static PasswordStrength GetPasswordStrength(string password)
		{
			return GetPasswordStrength(password, null);
		}
	}
}
