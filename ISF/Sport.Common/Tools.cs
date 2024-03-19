using System;
using System.Linq;
using System.Collections;
using System.Drawing;
using System.Web.Mail;
using System.IO;
using System.Diagnostics;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
namespace Sport.Common
{
	public static class Tools
	{
		public enum MappingDiffState
		{
			None = 0,
			OnlyFirst,
			OnlySecond,
			Both
		}

		public static string GetHebrewYear(int seasonCode)
		{
			DateTime date = (seasonCode > 0) ? new DateTime(1948 + seasonCode, 12, 1) : DateTime.Now;
			System.Globalization.CultureInfo hebCulture = System.Globalization.CultureInfo.CreateSpecificCulture("he-IL");
			hebCulture.DateTimeFormat.Calendar = new System.Globalization.HebrewCalendar();
			return date.ToString("yyyy", hebCulture);
		}

		public static string GetHebrewYear()
		{
			return GetHebrewYear(0);
		}

		public static Dictionary<int, MappingDiffState> GetMappingDiff<T>(Dictionary<int, T> mapping1, Dictionary<int, T> mapping2)
		{
			Dictionary<int, MappingDiffState> states = new Dictionary<int, MappingDiffState>();
			mapping1.Keys.ToList().ForEach(key =>
			{
				MappingDiffState state = mapping2.ContainsKey(key) ? MappingDiffState.Both : MappingDiffState.OnlyFirst;
				states.Add(key, state);
			});
			mapping2.Keys.ToList().ForEach(key =>
			{
				if (!states.ContainsKey(key))
				{
					MappingDiffState state = mapping1.ContainsKey(key) ? MappingDiffState.Both : MappingDiffState.OnlySecond;
					states.Add(key, state);
				}
			});
			return states;
		}

		public static bool TryReadExcelFile(string filePath, string sheetName,
			out List<List<string>> values, out int sheetCount, out string error)
		{
			values = null;
			error = "";
			sheetCount = 0;

			if (!File.Exists(filePath))
			{
				error = "File '" + filePath + "' does not exist";
				return false;
			}

			//XWPFDocument

			IWorkbook workbook = GetExcelWorkbook(filePath, out error);
			if (workbook == null)
			{
				if (error.Length == 0)
					error = "Unknown error, type not supported";
				return false;
			}

			List<ISheet> sheets = new List<ISheet>();
			if (!string.IsNullOrEmpty(sheetName))
			{
				ISheet sheet = workbook.GetSheet(sheetName);
				if (sheet == null)
				{
					error = "Sheet '" + sheetName + "' does not exist in the workbook";
					return false;
				}
				sheets.Add(sheet);
			}
			else
			{
				for (int i = 0; i < workbook.NumberOfSheets; i++)
				{
					ISheet sheet = workbook.GetSheetAt(i);
					if (sheet != null && sheet.LastRowNum > 0)
						sheets.Add(sheet);
				}
			}

			sheetCount = sheets.Count;
			values = new List<List<string>>();
			foreach (ISheet sheet in sheets)
			{
				for (int i = 0; i <= sheet.LastRowNum; i++)
				{
					IRow row = sheet.GetRow(i);
					if (row != null && row.Cells != null && row.Cells.Count > 0) //null is when the row only contains empty cells 
						values.Add(row.Cells.ToList().ConvertAll(c => GetStringValue(c)));
				}
			}

			//remove rows where all columns are empty:
			values.RemoveAll(cells => cells.TrueForAll(t => t.Length == 0));
			return true;
		}

		private static IWorkbook GetExcelWorkbook(string filePath, out string error)
		{
			error = "";

			//first try as xlsx:
			IWorkbook workbook = null;
			try
			{
				using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					workbook = new XSSFWorkbook(file);
				}
			}
			catch (Exception ex)
			{
				error = "Can't read file, make sure it's not opened in Excel and that it's a valid Excel file\n(Error:\n" + ex.Message + ")";
				workbook = null;
			}

			if (workbook == null)
			{
				//maybe xls will work?
				try
				{
					using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						workbook = new HSSFWorkbook(file);
					}
				}
				catch (Exception ex)
				{
					error = "Can't read file, make sure it's not opened in Excel and that it's a valid Excel file\n(Error:\n" + ex.Message + ")";
					workbook = null;
				}
			}

			return workbook;
		}

		public static bool IsValidIdNumber(string idNumber, out string error)
		{
			error = "";

			if (idNumber.Length == 0)
			{
				error = "empty";
				return false;
			}

			if (idNumber.Length > 9)
			{
				error = "more than 9 digits";
				return false;
			}

			if (!idNumber.ToList().TrueForAll(c => Char.IsDigit(c)))
			{
				error = "contains invalid characters";
				return false;
			}

			idNumber = idNumber.PadLeft(9, '0');
			int counter = 0;
			int checksum = idNumber.ToList().Sum(c =>
			{
				int curDigitValue = Int32.Parse(c.ToString());
				curDigitValue *= (counter % 2) + 1;
				if (curDigitValue > 9)
					curDigitValue -= 9;
				counter++;
				return curDigitValue;
			});

			if ((checksum % 10) != 0)
			{
				error = "checksum not matching";
				return false;
			}

			return true;
		}

		public static bool TryReadExcelFile(string filePath, out List<List<string>> values,
			out int sheetCount, out string error)
		{
			return TryReadExcelFile(filePath, "", out values, out sheetCount, out error);
		}

		private static string GetStringValue(ICell cell)
		{
			switch (cell.CellType)
			{
				case CellType.BOOLEAN:
					return cell.BooleanCellValue.ToString();
				case CellType.NUMERIC:
					DateTime? dateValue = null;
					try
					{
						dateValue = cell.DateCellValue;
					}
					catch
					{
						dateValue = null;
					}
					if (dateValue != null && (dateValue.Value.Year < 1900 || dateValue.Value.Year > 2200))
						dateValue = null;
					return (dateValue == null) ? cell.NumericCellValue.ToString() : dateValue.ToString();
				case CellType.STRING:
					return cell.StringCellValue;
				case CellType.ERROR:
					return "Error #" + cell.ErrorCellValue;
			}
			return "";
		}

		/// <summary>
		/// returns the numeric value of the object if possible, 
		/// or the given default number if object is not numeric or is null.
		/// </summary>
		public static int CIntDef(Object obj, int def)
		{
			int result = def;

			if (obj != null)
			{
				if (!(obj is System.DBNull))
				{
					try
					{
						if (obj is int)
							return (int)obj;
						result = System.Convert.ToInt32(obj);
					}
					catch (Exception e)
					{
						string dummy = e.Message;
						result = def;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// returns the numeric value of the object if possible, 
		/// or the given default number if object is not numeric or is null.
		/// </summary>
		public static long CLngDef(Object obj, long def)
		{
			long result = def;

			if (obj != null)
			{
				if (!(obj is System.DBNull))
				{
					try
					{
						result = System.Convert.ToInt64(obj);
					}
					catch (Exception e)
					{
						string dummy = e.Message;
						result = def;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// returns the string value of the object if possible, 
		/// or the given default string if object is null.
		/// </summary>
		public static string CStrDef(Object obj, string def)
		{
			if ((obj == null) || (obj is DBNull))
				return def;
			return obj.ToString();
		}

		/// <summary>
		/// returns the value as DateTime if possible, 
		/// or the given default DateTime if object is null.
		/// </summary>
		public static DateTime CDateTimeDef(Object obj, DateTime def)
		{
			if ((obj == null) || (obj is DBNull))
				return def;
			try
			{
				return (DateTime)obj;
			}
			catch
			{
				return def;
			}
		}

		/// <summary>
		/// Returns the matching characters count
		/// </summary>
		public static int MatchStrings(string a, string b)
		{
			int eq = 0;
			if ((a == null) || (b == null))
				return eq;
			while (eq < a.Length && eq < b.Length &&
				a[eq] == b[eq])
				eq++;

			return eq;
		}

		/// <summary>
		/// converts the given number to hebrew letters, for example 1 is א 
		/// and 21 is כ"א
		/// </summary>
		public static string ToHebLetter(int num)
		{
			char[] letters = { 'י', 'כ', 'ל', 'מ', 'נ', 'ס', 'ע', 'פ', 'צ' };
			if ((num < 0) || (num > 99))
				return num.ToString();

			if (num == 0)
				return "";

			if (num <= 10)
				return ((char)(((int)'א') + (num - 1))).ToString();

			if (num == 15)
				return "ט\"ו";

			if (num == 16)
				return "ט\"ז";

			if ((num % 10) == 0)
				return letters[(num / 10) - 1].ToString();

			return ToHebLetter(((int)num / 10) * 10) + "\"" + ToHebLetter(num % 10);
		} //end function ToHebLetter

		public static ArrayList MergeArrays(Object[] arr1, Object[] arr2)
		{
			ArrayList result = new ArrayList(arr1);
			for (int i = 0; i < arr2.Length; i++)
			{
				result.Add(arr2[i]);
			}
			return result;
		}

		/// <summary>
		/// converts given Color to the proper hex string for usage in html.
		/// </summary>
		public static string ColorToHex(Color color)
		{
			string R = IntToHex(color.R);
			string G = IntToHex(color.G);
			string B = IntToHex(color.B);
			R = (R.Length == 1) ? "0" + R : R;
			G = (G.Length == 1) ? "0" + G : G;
			B = (B.Length == 1) ? "0" + B : B;
			return "#" + R + G + B;
		}

		public static string ColorsToHex(Color[] colors, string seperator)
		{
			string result = "";
			for (int i = 0; i < colors.Length; i++)
			{
				result += ColorToHex(colors[i]);
				if (i < (colors.Length - 1))
					result += seperator;
			}
			return result;
		}

		/// <summary>
		/// converts given integer to hex string.
		/// </summary>
		public static string IntToHex(int num)
		{
			//stop condition: the number is less than 10.
			if (num < 10)
				return num.ToString();

			//return one character if the number is 15 or less.
			switch (num)
			{
				case 10: return "a";
				case 11: return "b";
				case 12: return "c";
				case 13: return "d";
				case 14: return "e";
				case 15: return "f";
			}

			//use recursion to convert number:
			return IntToHex((int)(num / 16)) + IntToHex((int)(num % 16));
		} //end function IntToHex

		/// <summary>
		/// add zero to the given number if it's less than 10.
		/// </summary>
		public static string AddZero(int num)
		{
			return (num < 10) ? "0" + num.ToString() : num.ToString();
		}

		public static string AddZero(string strNumber)
		{
			int num = CIntDef(strNumber, Int32.MinValue);
			if (num >= 0)
				return AddZero(num);
			else
				return strNumber;
		}

		/// <summary>
		/// add zero to all array items.
		/// </summary>
		public static string[] AddZero(string[] numbers)
		{
			string[] result = new string[numbers.Length];
			for (int i = 0; i < numbers.Length; i++)
			{
				result[i] = AddZero(numbers[i]);
			}
			return result;
		}

		/// <summary>
		/// returns new array with given array items except any null items.
		/// </summary>
		public static System.Array RemoveNullValues(System.Array array, System.Type type)
		{
			System.Collections.ArrayList list = new ArrayList();
			for (int i = 0; i < array.Length; i++)
			{
				if (array.GetValue(i) != null)
					list.Add(array.GetValue(i));
			}
			return list.ToArray(type);
		}

		/// <summary>
		/// merge all given arrays. all must be of the same type.
		/// </summary>
		public static System.Array MergeArrays(System.Array arrArrays, System.Type type)
		{
			System.Collections.ArrayList list = new ArrayList();
			for (int i = 0; i < arrArrays.Length; i++)
			{
				System.Array arr = (System.Array)arrArrays.GetValue(i);
				for (int j = 0; j < arr.Length; j++)
					list.Add(arr.GetValue(j));
			}
			return list.ToArray(type);
		}

		/// <summary>
		/// returns current date in desired format.
		/// </summary>
		public static string FormatDate(DateTime date, string format)
		{
			string result = format;
			string day = date.Day.ToString().PadLeft(2, '0');
			string month = date.Month.ToString().PadLeft(2, '0');
			string hour = date.Hour.ToString().PadLeft(2, '0');
			string minute = date.Minute.ToString().PadLeft(2, '0');
			string second = date.Second.ToString().PadLeft(2, '0');
			string year = date.Year.ToString();
			if (year.Length > 1)
				year = year.Substring(2);
			result = result.Replace("dd", day);
			result = result.Replace("mm", month);
			result = result.Replace("hh", hour);
			result = result.Replace("nn", minute);
			result = result.Replace("ss", second);
			result = result.Replace("yy", year);
			return result;
		}

		/// <summary>
		/// return array items joined toghether with given delimeter.
		/// </summary>
		public static string ArrayToString(System.Array array, string delimeter)
		{
			string result = "";
			if (array == null)
				return null;

			for (int i = 0; i < array.Length; i++)
			{
				result += array.GetValue(i).ToString();
				if (i < (array.Length - 1))
					result += delimeter;
			}

			return result;
		}

		public static int[] ToIntArray(string str, char delimeter, bool positiveOnly)
		{
			if (str != null && str.Length > 0)
			{
				List<int> array = new List<int>();
				int num;
				str.Split(delimeter).ToList().ForEach(rawValue =>
				{
					if (rawValue.Length > 0)
					{
						if (Int32.TryParse(rawValue, out num))
						{
							if (!positiveOnly || (positiveOnly && num > 0))
							{
								array.Add(num);
							}
						}
					}
				});
				return array.Count == 0 ? null : array.ToArray();
			}
			return null;
		}

		public static int[] ToIntArray(string str, char delimeter)
		{
			return ToIntArray(str, delimeter, false);
		}

		/// <summary>
		/// returns whether the string is null or zero length.
		/// </summary>
		public static bool IsEmpty(string str)
		{
			return ((str == null) || (str.Length == 0));
		}

		/// <summary>
		/// search for given value in given array and return its index.
		/// </summary>
		public static int InArray(System.Array arr, object value)
		{
			if ((value == null) || (arr == null) || (arr.Length == 0))
				return -1;

			for (int i = 0; i < arr.Length; i++)
			{
				object curValue = arr.GetValue(i);
				if (curValue != null)
				{
					if (arr.GetValue(i).Equals(value))
						return i;
				}
			}

			return -1;
		}

		public static double CDblDef(Object obj, double def)
		{
			double result = def;
			if (obj == null)
				return result;
			if (obj.ToString().Length == 0)
				return result;
			try
			{
				result = System.Convert.ToDouble(obj);
			}
			catch
			{
				result = def;
			}
			return result;
		}

		public static DateTime ToDateDef(Object obj, DateTime def)
		{
			DateTime result = def;
			if (obj == null || obj.Equals(DBNull.Value))
				return result;
			try
			{
				result = (DateTime)obj;
			}
			catch
			{
				result = def;
			}
			return result;
		}

		public static int GetNumber(string str, int defVal)
		{
			string digits = string.Empty;
			for (int i = 0; i < str.Length; i++)
			{
				char c = str[i];
				if (!char.IsDigit(c))
					break;
				digits += c;
			}
			return (digits.Length > 0) ? Int32.Parse(digits) : defVal;
		}

		public static bool ToInt(string str, out int value)
		{
			if (str != null && str.Length > 0)
			{
				try
				{
					value = Int32.Parse(str);
					return true;
				}
				catch
				{
				}
			}

			value = 0;

			return false;
		}

		/// <summary>
		/// returns the number with given amount of digits after decimal point.
		/// </summary>
		public static string FormatNumber(double number, int precision, bool clearZeros)
		{
			//lame .NET does not have any clear format strings. no choice but power.
			string strNumber = number.ToString();
			string[] arrParts = strNumber.Split(new char[] { '.' });
			string result = arrParts[0];
			if (precision <= 0)
				return result;
			result += ".";
			for (int i = 0; i < precision; i++)
			{
				if ((arrParts.Length > 1) && (i < arrParts[1].Length))
				{
					result += arrParts[1][i];
				}
				else
				{
					result += "0";
				}
			}

			if (clearZeros)
			{
				while (result[result.Length - 1] == '0')
				{
					result = result.Substring(0, result.Length - 1);
				}
				if (result[result.Length - 1] == '.')
				{
					result = result.Substring(0, result.Length - 1);
				}
			}

			return result;
		}

		public static double ToDoubleFast(object o, bool forceConvert)
		{
			if (o == null)
				throw new Exception("can't convert null to double");
			//weird, but it happens.
			if (o.ToString().Equals("0"))
				return 0;
			if (!forceConvert)
			{
				return (double)o;
			}
			else
			{
				double result = 0;
				try
				{
					result = System.Convert.ToDouble(o);
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to convert value to double: " + o.ToString());
					throw e;
				}
				return result;
			}
		}

		public static System.Windows.Forms.Control FindControl(
			System.Windows.Forms.Control parent, string childName)
		{
			//maybe parent is the child?
			if (parent.Name == childName)
				return parent;

			//search for child:
			foreach (System.Windows.Forms.Control control in parent.Controls)
			{
				//check current:
				if (control.Name == childName)
					return control;
			}

			//not found.
			return null;
		}

		public static void AttachTooltip(
			System.Windows.Forms.Control control, string toolTip)
		{
			System.Windows.Forms.ToolTip objToolTip =
				new System.Windows.Forms.ToolTip();
			objToolTip.SetToolTip(control, toolTip);
		}

		public static void AssignFontSize(
			System.Windows.Forms.Control control, float newSize)
		{
			control.Font =
				new Font(control.Font.FontFamily, newSize, control.Font.Style);
		}

		public static bool IsDigit(char c)
		{
			return ((c == '0') || (c == '1') || (c == '2') || (c == '3') ||
				(c == '4') || (c == '5') || (c == '6') || (c == '7') ||
				(c == '8') || (c == '9'));
		}

		public static bool IsInteger(string s)
		{
			if ((s == null) || (s.Length == 0))
				return false;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (!IsDigit(c))
				{
					if ((i > 0) || (c != '-'))
						return false;
				}
			}
			return true;
		}

		public static bool IsNumeric(string s)
		{
			if ((s == null) || (s.Length == 0))
				return false;
			string[] a = s.Split(new char[] { '.' });
			if (a.Length > 2)
				return false;
			string n1 = a[0];
			string n2 = (a.Length > 1) ? a[1] : "";
			if ((n1.Length == 0) && (n2.Length == 0))
				return false;
			if (!IsInteger(n1))
				return false;
			if (n2.Length > 0)
			{
				if (n2.IndexOf("-") >= 0)
					return false;
				if (!IsInteger(n2))
					return false;
			}
			return true;
		}

		public static void SplitWithDelimeters(string str, char[] delimeters,
			ref System.Collections.Specialized.StringCollection partsCollection,
			ref System.Collections.Specialized.StringCollection delimetersCollection)
		{
			partsCollection = new System.Collections.Specialized.StringCollection();
			delimetersCollection = new System.Collections.Specialized.StringCollection();

			if (str == null)
				return;

			string[] arrParts = str.Split(delimeters);
			for (int partIndex = 0; partIndex < arrParts.Length; partIndex++)
				partsCollection.Add(arrParts[partIndex]);

			for (int charIndex = 0; charIndex < str.Length; charIndex++)
			{
				for (int delimeterIndex = 0; delimeterIndex < delimeters.Length; delimeterIndex++)
				{
					char curDelimeter = delimeters[delimeterIndex];
					if (str[charIndex] == curDelimeter)
					{
						delimetersCollection.Add(curDelimeter.ToString());
						break;
					}
				}
			}
		}

		/// <summary>
		/// returns null if condition is true, the given object otherwise.
		/// </summary>
		public static object Nullify(object obj, bool cond)
		{
			if (cond == true)
				return null;
			return obj;
		}

		public static string BuildOneOrMany(string strOne, string strMany,
			int count, bool blnMale)
		{
			if (count <= 0)
				return "אין " + strMany;
			string strSingle = (blnMale) ? "אחד" : "אחת";
			if (count == 1)
				return strOne + " " + strSingle;
			//return count.ToString()+" "+strMany;
			return NumberToHebrew(count, blnMale) + " " + strMany;
		}

		public static string GetApplicationFolder()
		{
			string strFullPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			return System.IO.Path.GetDirectoryName(strFullPath);
		}

		public static object IIF(bool cond, object objTrue, object objFalse)
		{
			if (cond)
				return objTrue;
			return objFalse;
		}

		public static string Trim(string str)
		{
			if (str == null)
				return "";
			return str.Trim();
		}

		/// <summary>
		/// removes all duplicate words from the string.
		/// </summary>
		public static string RemoveDuplicateWords(string str) //, int duplicateCount
		{
			/*
				/// if duplicateCount is bigger than zero, it would remove
				/// all words which repeat the given count, otherwise all
			*/
			string result = "";
			string[] arrWords = str.Split(new char[] { ' ' });
			ArrayList wordsList = new ArrayList();
			/*
			System.Collections.Specialized.ListDictionary wordsList=
				new System.Collections.Specialized.ListDictionary();
			*/
			for (int i = 0; i < arrWords.Length; i++)
			{
				string strWord = arrWords[i];
				if ((wordsList.IndexOf(strWord) < 0) || (strWord.Length < 2))
				{
					result += strWord + " ";
					wordsList.Add(strWord);
				}
				/*
				if (wordsList[strWord] == null)
				{
					wordsList.Add(strWord, 0);
				}
				wordsList[strWord] = ((int) wordsList[strWord])+1;
				*/
			}

			if (result.Length > 0)
				result = result.Substring(0, result.Length - 1);

			return result;
		}

		public static string RemoveDuplicateWords(string str, string[] arrToIgnore)
		{
			string result = str;
			for (int i = 0; i < arrToIgnore.Length; i++)
			{
				result = result.Replace(arrToIgnore[i], "");
			}
			return RemoveDuplicateWords(result);
		}

		public static string IntToHebrew(int number, bool isFemale)
		{
			if (number < 2)
				return (isFemale) ? "ראשונה" : "ראשון";

			switch (number)
			{
				case 2:
					return (isFemale) ? "שנייה" : "שני";
				case 3:
					return (isFemale) ? "שלישית" : "שלישי";
				case 4:
					return (isFemale) ? "רביעית" : "רביעי";
				case 5:
					return (isFemale) ? "חמישית" : "חמישי";
				case 6:
					return (isFemale) ? "שישית" : "שישי";
				case 7:
					return (isFemale) ? "שביעית" : "שביעי";
				case 8:
					return (isFemale) ? "שמינית" : "שמיני";
				case 9:
					return (isFemale) ? "תשיעית" : "תשיעי";
				case 10:
					return (isFemale) ? "עשירית" : "עשירי";
			}

			if (number < 20)
				return HebrewCount((number % 10), isFemale) + " " + ((isFemale) ? "עשרה" : "עשר");

			return HebrewCount(number, isFemale);
		}

		public static string HebrewCount(int number, bool isMale)
		{
			string[] arrLessThanTen_Female = new string[] {"אפס", "אחת", "שתיים", "שלוש", "ארבע", 
															"חמש", "שש", "שבע", "שמונה", "תשע", "עשר"};
			string[] arrLessThanTen_Male = new string[] {"אפס", "אחד", "שניים", "שלושה", "ארבעה", 
														  "חמישה", "שישה", "שבעה", "שמונה", "תשעה", "עשרה"};
			string[] arrAsarot = new string[] {"עשר", "עשרים", "שלושים", "ארבעים", "חמישים", 
												"שישים", "שבעים", "שמונים", "תשעים"};
			string[] arrMeot = new string[9];
			string[] arrAlafim = new string[9];
			int i = 0;

			if (number < 0)
				return "מינוס " + HebrewCount(number * -1, isMale);

			if (number <= 10)
				return (isMale) ? arrLessThanTen_Male[number] : arrLessThanTen_Female[number];

			if (number < 20)
			{
				return (isMale) ? (arrLessThanTen_Male[number % 10] + " " + arrLessThanTen_Female[arrLessThanTen_Female.Length - 1]) :
					(arrLessThanTen_Female[number % 10] + " " + arrLessThanTen_Male[arrLessThanTen_Male.Length - 1]);
			}

			if (number < 100)
			{
				int asarot = Trunc(number / 10);
				string result = arrAsarot[asarot - 1];
				if ((number % 10) > 0)
					result += " ו" + HebrewCount(number % 10, isMale);
				return result;
			}

			for (i = 0; i < arrMeot.Length; i++)
			{
				switch (i)
				{
					case 0:
						arrMeot[i] = "מאה";
						break;
					case 1:
						arrMeot[i] = "מאתיים";
						break;
					default:
						arrMeot[i] = arrLessThanTen_Female[i + 1] + " מאות";
						break;
				}
			}

			if (number < 1000)
			{
				int meot = Trunc(number / 100);
				string result = arrMeot[meot - 1];
				if ((number % 100) > 0)
					result += " " + HebrewCount(number % 100, isMale);
				return result;
			}

			for (i = 0; i < arrAlafim.Length; i++)
			{
				switch (i)
				{
					case 0:
						arrAlafim[i] = "אלף";
						break;
					case 1:
						arrAlafim[i] = "אלפיים";
						break;
					default:
						if (i == 7)
							arrAlafim[i] = arrLessThanTen_Female[i + 1].Replace("ה", "ת") + " אלפים";
						else
							arrAlafim[i] = arrLessThanTen_Female[i + 1] + "ת" + " אלפים";
						break;
				}
			}

			if (number < 1000000)
			{
				int alafim = Trunc(number / 1000);
				string result = "";
				if ((alafim - 1) < arrAlafim.Length)
					result = arrAlafim[alafim - 1];
				else
					result = HebrewCount(alafim, false) + " אלף";
				if ((number % 1000) > 0)
				{
					if ((number % 1000) < 20)
						result += " ו" + HebrewCount(number % 1000, isMale);
					else
						result += " " + HebrewCount(number % 1000, isMale);
				}
				return result;
			}

			return number.ToString();
		}

		private static int Trunc(double num)
		{
			return (int)Math.Floor(num);
		}

		public static void SetNudValue(System.Windows.Forms.NumericUpDown nud,
			double value)
		{
			//for some weird reason, it won't update unless you Read the Value.
			string dummy = nud.Value.ToString();
			nud.Value = (decimal)value;
		}

		public static System.Collections.ArrayList GetResultMeasures(
			Sport.Core.Data.ResultValue resultValue,
			Sport.Core.Data.ResultMeasure resultMeasure)
		{
			System.Collections.ArrayList measures = new System.Collections.ArrayList();
			if (resultValue == Sport.Core.Data.ResultValue.Distance)
			{
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Kilometers) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Kilometers);
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Meters) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Meters);
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Centimeters) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Centimeters);
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Milimeters) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Milimeters);
			}
			if (resultValue == Sport.Core.Data.ResultValue.Time)
			{
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Days) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Days);
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Hours) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Hours);
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Minutes) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Minutes);
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Seconds) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Seconds);
				if ((resultMeasure & Sport.Core.Data.ResultMeasure.Miliseconds) != 0)
					measures.Add(Sport.Core.Data.ResultMeasure.Miliseconds);
			}
			return measures;
		}

		public static long DateDiffMiliSeconds(DateTime date1, DateTime date2)
		{
			long tpms = System.TimeSpan.TicksPerMillisecond;
			long elpasedMilliSeconds = ((date2.Ticks - date1.Ticks) / tpms);
			return elpasedMilliSeconds;
		}

		/// <summary>
		/// returns the new dimensions of the resized image, fit for maximum square.
		/// </summary>
		public static System.Drawing.Size SmartResize(System.Drawing.Image image,
			int maxValue)
		{
			//read current dimensions:
			int curWidth = image.Width;
			int curHeight = image.Height;

			//initialize new size:
			System.Drawing.Size result = new Size(curWidth, curHeight);

			//check if empty:
			if ((curWidth == 0) || (curHeight == 0))
				return result;

			//calculate ratio:
			double ratio = ((double)maxValue) / ((double)Math.Max(curWidth, curHeight));

			//apply new size:
			result.Width = (int)(result.Width * ratio);
			result.Height = (int)(result.Height * ratio);

			//retur new size:
			return result;
		}

		/// <summary>
		/// returns true if the array is null, has no elements or all elements empty.
		/// </summary>
		public static bool IsArrayEmpty(System.Array arr)
		{
			//null?
			if (arr == null)
				return true;

			//zero length array?
			if (arr.Length == 0)
				return true;

			//empty elements only?
			bool hasValue = false;
			for (int i = 0; i < arr.Length; i++)
			{
				object value = arr.GetValue(i);
				if ((value != null) && (value.ToString().Length > 0))
				{
					hasValue = true;
					break;
				}
			}
			return (!hasValue);
		}

		public static int[] AddIntegerOffset(int[] array, int offset)
		{
			if (array == null)
				return null;
			int[] result = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
				result[i] = (array[i] + offset);
			return result;
		}

		public static string[] ToStringArray(System.Array array)
		{
			if (array == null)
				return null;

			string[] result = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				result[i] = "";
				object value = array.GetValue(i);
				if (value != null)
				{
					result[i] = value.ToString();
				}
			}
			return result;
		}

		/// <summary>
		/// add the "http://" if missing
		/// </summary>
		public static string MakeValidURL(string url)
		{
			if ((url.ToLower().StartsWith("http://")) ||
				(url.ToLower().StartsWith("https://")))
			{
				return url;
			}
			return "http://" + url;
		}

		/// <summary>
		/// ordinary split, but would return only the non-empty items.
		/// </summary>
		public static string[] SplitNoBlank(string str, char delimeter)
		{
			if (str == null)
				str = "";
			string[] arrItems = str.Split(new char[] { delimeter });
			ArrayList result = new ArrayList();
			for (int i = 0; i < arrItems.Length; i++)
			{
				if ((arrItems[i] != null) && (arrItems[i].Length > 0))
					result.Add(arrItems[i]);
			}
			return (string[])result.ToArray(typeof(string));
		}

		public static string GetEqualFile(string strFilePath, string strFolderPath)
		{
			string[] arrFiles = System.IO.Directory.GetFiles(strFolderPath);
			foreach (string strCurFile in arrFiles)
			{
				if (strCurFile.ToLower() == strFilePath.ToLower())
					continue;
				if (Tools.FilesEqual(strFilePath, strCurFile))
					return strCurFile;
			}
			return "";
		}

		/// <summary>
		/// returns whether the files are binary equal i.e. have the same bytes.
		/// </summary>
		public static bool FilesEqual(string strPath1, string strPath2)
		{
			//get files information:
			System.IO.FileInfo file1 = new System.IO.FileInfo(strPath1);
			System.IO.FileInfo file2 = new System.IO.FileInfo(strPath2);

			//maybe different size?
			if (file1.Length != file2.Length)
				return false;

			//need to read the whole file...
			System.IO.StreamReader reader1 = new System.IO.StreamReader(strPath1);
			System.IO.StreamReader reader2 = new System.IO.StreamReader(strPath2);

			//compare the whole binary data:
			bool equals = reader1.ReadToEnd().Equals(reader2.ReadToEnd());

			//close and return:
			reader1.Close();
			reader2.Close();

			return equals;
		}

		/// <summary>
		/// Used to send a message through the default SMTP server.
		/// </summary>
		/// <param name="strFrom">The sender address</param>
		/// <param name="strTo">The recipent address</param>
		/// <param name="strSubject">The subject of the email</param>
		/// <param name="strBody">The actual message</param>
		public static void sendEmail(string strFrom, string strTo,
			string strSubject, string strBody)
		{
			MailMessage messageToSend = new MailMessage();
			messageToSend.BodyFormat = MailFormat.Html;
			//strBody = strBody.Replace("\n","<br />");

			messageToSend.UrlContentBase = "http://213.8.193.147/";
			messageToSend.From = strFrom;
			messageToSend.To = strTo;
			messageToSend.Subject = strSubject;
			messageToSend.Body = "<html><body>" + strBody + "</body></html>";

			SmtpMail.Send(messageToSend);
		}

		public static bool IsSameDate(DateTime date1, DateTime date2)
		{
			return ((date1.Year == date2.Year) && (date1.Month == date2.Month) && (date1.Day == date2.Day));
		}

		public static bool DateInRange(DateTime date, DateTime start, DateTime end)
		{
			return ((date >= start) && (date <= end));
		}

		public static string NumberToHebrew(int number, bool isMale)
		{
			if (number < 0)
				return number.ToString();
			string[] arrMales = new string[] {"אפס", "אחד", "שני", "שלושה", "ארבעה", "חמישה", 
									"שישה", "שבעה", "שמונה", "תשעה", "עשרה"};
			string[] arrFemales = new string[] {"אפס", "אחת", "שתי", "שלוש", "ארבע", "חמש", 
								   "שש", "שבע", "שמונה", "תשע", "עשר"};
			if (isMale)
			{
				return (number < arrMales.Length) ? arrMales[number] : number.ToString();
			}

			return (number < arrFemales.Length) ? arrFemales[number] : number.ToString();
		}

		public static string GetHebDayOfWeek(DateTime date)
		{
			string result = "";
			switch (date.DayOfWeek)
			{
				case DayOfWeek.Sunday:
					result = "א";
					break;
				case DayOfWeek.Monday:
					result = "ב";
					break;
				case DayOfWeek.Tuesday:
					result = "ג";
					break;
				case DayOfWeek.Wednesday:
					result = "ד";
					break;
				case DayOfWeek.Thursday:
					result = "ה";
					break;
				case DayOfWeek.Friday:
					result = "ו";
					break;
				case DayOfWeek.Saturday:
					result = "ש";
					break;
			}
			if (result.Length > 0)
				result += "'";
			return result;
		}

		public static DateTime SetTime(DateTime date, int hour, int minute, int second)
		{
			return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
		}

		public static object[] PartialJoin(object[] arr, int start, int end)
		{
			ArrayList result = new ArrayList();
			for (int i = start; i < end; i++)
			{
				if (i >= arr.Length)
					break;
				result.Add(arr[i]);
			}
			return (object[])result.ToArray(typeof(object));
		}

		public static string MakeInitials(string s)
		{
			if ((s == null) || (s.Length == 0))
				return "";
			s = s.Trim();
			while (s.IndexOf("  ") >= 0)
				s = s.Replace("  ", " ");
			if (s.Length == 0)
				return "";
			string[] arrWords = s.Split(new char[] { ' ' });
			if (arrWords.Length == 1)
				return s;
			string result = "";
			for (int i = 0; i < arrWords.Length; i++)
			{
				string strWord = arrWords[i];
				if (strWord.Length > 0)
				{
					result += strWord[0].ToString();
					if (i == (arrWords.Length - 2))
						result += "\"";
				}
			}
			return result;
		}

		public static string CleanName(string name)
		{
			string result = name;
			result = result.Replace("-", " ").Replace("\"", "").Replace("'", "");
			while (result.IndexOf("וו") >= 0)
				result = result.Replace("וו", "ו");
			return result;
		}

		public static bool FindCityInSchool(string school, string city)
		{
			school = Tools.CleanName(school);
			city = Tools.CleanName(city);
			bool result = (school.IndexOf(city) > 0);
			if ((result == false) && (city.IndexOf(" ") > 0) && (school.IndexOf(" ") > 0))
			{
				string[] arrWords = city.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string word in arrWords)
				{
					if (word.Length > 0)
					{
						if (school.IndexOf(word) >= 0)
						{
							school = school.Replace(word, "");
							result = false;
						}
					}
				}
			}
			return result;
		}

		public static string BuildIdentificationNumber(string strNumber)
		{
			if ((strNumber == null) || (strNumber.Length == 0))
				return "";
			string result = "0-" + strNumber.Substring(0, strNumber.Length - 1);
			result += "-" + strNumber.Substring(strNumber.Length - 1, 1);
			return result;
		}

		public static string GetSeasonYears(DateTime dtDate)
		{
			int year1 = 0;
			int year2 = 0;
			if (dtDate.Month < 9)
			{
				year1 = dtDate.Year - 1;
				year2 = dtDate.Year;
			}
			else
			{
				year1 = dtDate.Year;
				year2 = dtDate.Year + 1;
			}
			return year1.ToString() + "-" + year2.ToString();
		}

		public static string GetOnlyGrades(string strCategory)
		{
			if (strCategory == null || strCategory.Length == 0)
				return "";

			int index = strCategory.LastIndexOf(" ");
			return strCategory.Substring(0, index);
		}

		public static string GetOnlySex(string strCategory, bool blnShort)
		{
			if (strCategory == null || strCategory.Length == 0)
				return "";

			int index = strCategory.LastIndexOf(" ");
			string result = "";
			for (int i = index + 1; i < strCategory.Length; i++)
				result += strCategory.Substring(i, 1);

			if (blnShort)
				result = result.Replace("תלמידים", "בנים").Replace("תלמידות", "בנות");

			return result;
		}

		public static string GetOnlySex(string strCategory)
		{
			return GetOnlySex(strCategory, false);
		}

		public static int GetAvailableNumber(int min, int max, List<int> numbers)
		{
			int result = min;
			while (numbers.IndexOf(result) >= 0)
			{
				result++;
				if (result > max)
					return (min - 1);
			}
			return result;
		}

		public static bool SameArrays(Array arr1, Array arr2)
		{
			if ((arr1 == null) && (arr2 == null))
				return true;
			if ((arr1 == null) || (arr2 == null))
				return false;
			if (arr1.Length != arr2.Length)
				return false;
			for (int i = 0; i < arr1.Length; i++)
			{
				object item1 = arr1.GetValue(i);
				object item2 = arr2.GetValue(i);
				if ((item1 == null) && (item2 == null))
					continue;
				if ((item1 == null) && (item2 != null))
					return false;
				if ((item1 != null) && (item2 == null))
					return false;
				if (!item1.Equals(item2))
					return false;
			}
			return true;
		}

		/// <summary>
		/// this function will take all the items in the given arrays
		/// and scramble them into new set of arrays, equally divided
		/// so that items from the same original array won't appear
		/// next to each other in the new arrays.
		/// </summary>
		/// <param name="arrays">each item is expected to be Array</param>
		/// <param name="newCount">amount of new arrays to generate</param>
		/// <returns>set of new arrays with the scrambles items</returns>
		public static ArrayList Scramble(ArrayList arrays, int newCount)
		{
			//got anything?
			if ((arrays == null) || (arrays.Count == 0))
				return arrays;

			//declare local variables:
			ArrayList arrScrambedArrays = new ArrayList();
			ArrayList result = new ArrayList();
			int itemsCount = 0;

			//calculate how many items we got in the original arrays:
			foreach (Array array in arrays)
				itemsCount += array.Length;

			//get array of random numbers:
			int[] arrIndices = RandomNoDuplicates(0, arrays.Count - 1, arrays.Count);

			//use the array for initial scrambling of the arrays
			for (int i = 0; i < arrIndices.Length; i++)
				arrScrambedArrays.Add(arrays[arrIndices[i]]);

			//build initial arrays
			for (int i = 0; i < newCount; i++)
				result.Add(new ArrayList());

			//fill new arrays
			int scrambled_index = 0;
			int scrambled_array = 0;
			int newItemsCount = 0;
			while (newItemsCount < itemsCount)
			{
				for (int index_new = 0; index_new < newCount; index_new++)
				{
					if (scrambled_array >= arrScrambedArrays.Count)
					{
						newItemsCount = itemsCount;
						break;
					}
					Array curScrambledArray = (Array)arrScrambedArrays[scrambled_array];
					object curItem = curScrambledArray.GetValue(scrambled_index);
					(result[index_new] as ArrayList).Add(curItem);
					scrambled_index++;
					if (scrambled_index >= curScrambledArray.Length)
					{
						scrambled_array++;
						scrambled_index = 0;
					}
					newItemsCount++;
				}
			}

			//sort:
			for (int i = 0; i < result.Count; i++)
			{
				Hashtable curTable = new Hashtable();
				ArrayList curArray = (ArrayList)result[i];
				foreach (object o in curArray)
				{
					int index = -1;
					for (int j = 0; j < arrays.Count; j++)
					{
						if (index >= 0)
							break;
						Array arr = (Array)arrays[j];
						for (int k = 0; k < arr.Length; k++)
						{
							if (arr.GetValue(k).Equals(o))
							{
								index = j;
								break;
							}
						}
					}
					curTable[o] = index;
				}
				for (int j = 1; j < curArray.Count; j++)
				{
					int curIndex = (int)curTable[curArray[j]];
					int previousIndex = (int)curTable[curArray[j - 1]];
					if (curIndex == previousIndex)
					{
						int newIndex = j;
						for (int k = j + 1; k < curArray.Count; k++)
						{
							if (((int)curTable[curArray[k]]) != curIndex)
							{
								newIndex = k;
								break;
							}
						}
						if (newIndex != j)
						{
							object temp = curArray[newIndex];
							curArray[newIndex] = curArray[j];
							curArray[j] = temp;
						}
					}
				}
				if (curArray.Count > 1)
				{
					if (curTable[curArray[curArray.Count - 1]].Equals(
						curTable[curArray[curArray.Count - 2]]))
					{
						object temp = curArray[curArray.Count - 1];
						curArray[curArray.Count - 1] = curArray[0];
						curArray[0] = temp;
					}
				}
				//curArray.Sort(new SpecialComparer(curTable));
				result[i] = curArray;
			}

			//put proper type back:
			for (int i = 0; i < result.Count; i++)
			{
				result[i] = (result[i] as ArrayList).ToArray(
					(arrays[0] as Array).GetValue(0).GetType());
			}

			//done.
			return result;
		} //end function Scramble

		public static int[] RandomNoDuplicates(int min, int max, int count)
		{
			if ((min < 0) || (max < 0))
				throw new Exception("RandomNoDuplicates: both values must be positive");
			if (min > max)
				throw new Exception("RandomNoDuplicates: minimum bigger than maximum");
			if (count > (max - min + 1))
				throw new Exception("RandomNoDuplicates: count bigger than capacity");

			int[] result = new int[count];
			Random random = new Random();
			ArrayList arrAvailableNumbers = new ArrayList();
			for (int i = min; i <= max; i++)
				arrAvailableNumbers.Add(i);
			for (int i = 0; i < count; i++)
			{
				int randIndex = random.Next(0, arrAvailableNumbers.Count);
				result[i] = (int)arrAvailableNumbers[randIndex];
				arrAvailableNumbers.RemoveAt(randIndex);
			}
			return result;
		}

		public static bool IsMinDate(DateTime date)
		{
			return (date.Year < 1900);
		}

		public static string MultiString(string s, int n)
		{
			if (s == null)
				return null;
			string result = "";
			for (int i = 0; i < n; i++)
				result += s;
			return result;
		}

		public static string MultiString(char c, int n)
		{
			return MultiString(c.ToString(), n);
		}

		public static string Pad(string strString, char padChar, int num)
		{
			string strPadding = Tools.MultiString(padChar, num);
			return strPadding + strString + strPadding;
		}

		public static int CountDifferentCharacters(string s1, string s2)
		{
			if ((s1 == null) && (s2 == null))
				return 0;
			if (s1 == null)
				return s2.Length;
			if (s2 == null)
				return s1.Length;
			if (s1.Equals(s2))
				return 0;
			int result = 0;
			for (int i = 0; i < s1.Length; i++)
			{
				if (i >= s2.Length)
					return result + (i - s2.Length);
				result += (s1[i].Equals(s2[i])) ? 0 : 1;
			}
			if (s2.Length > s1.Length)
				result += (s2.Length - s1.Length);
			return result;
		}

		public static string EmptyIfLessThan(int num, int min)
		{
			return (num < min) ? "" : num.ToString();
		}

		public static string HebMonthName(int month)
		{
			switch (month)
			{
				case 1:
					return "ינואר";
				case 2:
					return "פברואר";
				case 3:
					return "מרץ";
				case 4:
					return "אפריל";
				case 5:
					return "מאי";
				case 6:
					return "יוני";
				case 7:
					return "יולי";
				case 8:
					return "אוגוסט";
				case 9:
					return "ספטמבר";
				case 10:
					return "אוקטובר";
				case 11:
					return "נובמבר";
				case 12:
					return "דצמבר";
			}

			return "?לא מוגדר";
		}

		public static string FixHebrewNegativeNumber(double num)
		{
			if (num >= 0)
				return num.ToString();
			string result = num.ToString();
			if (result[0] == '-')
				result = result.Substring(1, result.Length - 1) + "-";
			return result;
		}

		public static string TimeToDatabaseString(DateTime time)
		{
			return time.Day + "/" + time.Month + "/" + time.Year + " " + time.Hour + ":" + time.Minute + ":" + time.Second;
		}

		public static DateTime DatabaseStringToDateTime(string strTime)
		{
			string[] arrTemp = strTime.Split(' ');
			string datePart = arrTemp[0];
			string timePart = arrTemp[1];
			string[] arrDates = datePart.Split('/');
			string[] arrTimes = timePart.Split(':');
			return new DateTime(Int32.Parse(arrDates[2]), Int32.Parse(arrDates[1]), Int32.Parse(arrDates[0]),
				Int32.Parse(arrTimes[0]), Int32.Parse(arrTimes[1]), Int32.Parse(arrTimes[2]));
		}

		/// <summary>
		/// return the lines of the given file.
		/// </summary>
		public static string[] ReadTextFile(string strFullPath, bool hebrew)
		{
			if (!File.Exists(strFullPath))
				return null;
			StreamReader reader = null;
			if (hebrew)
				reader = new StreamReader(strFullPath, System.Text.Encoding.GetEncoding("ISO-8859-8"));
			else
				reader = new StreamReader(strFullPath);
			string strData = reader.ReadToEnd();
			if (strData == null)
				strData = "";
			if (strData.IndexOf("\r\n") >= 0)
				strData = strData.Replace("\r\n", "\n");
			reader.Close();
			return strData.Split('\n');
		}

		public static int CountEmptyItems(string[] items)
		{
			if (items == null)
				return 0;

			int emptyCount = 0;
			foreach (string item in items)
			{
				if (item == null || item.Trim().Length == 0)
					emptyCount++;
			}

			return emptyCount;
		}

		public static int CountPopulatedItems(string[] items)
		{
			if (items == null)
				return 0;
			return (items.Length - CountEmptyItems(items));
		}

		public static DateTime StringToDate(string s)
		{
			if (s != null)
			{
				string[] parts = s.Split('/');
				if (parts.Length > 2)
				{
					int day = Tools.CIntDef(parts[0], 0);
					int month = Tools.CIntDef(parts[1], 0);
					int year = Tools.CIntDef(parts[2], 0);
					if (day > 0 && month > 0 && year > 1900)
						return new DateTime(year, month, day);
				}
			}
			return DateTime.MinValue;
		}

		public static void WriteToLog(string data, string strLogFilePath)
		{
			if (strLogFilePath == null || strLogFilePath.Length == 0)
				strLogFilePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Path.DirectorySeparatorChar + "log.txt";
			string strToWrite = String.Format("[{0}]{1}", System.DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), data);
			try
			{
				using (StreamWriter writer = File.AppendText(strLogFilePath))
				{
					writer.WriteLine(strToWrite);
					writer.Close();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("failed to write to log file '" + strLogFilePath + "': " + ex.Message);
			}
		}

		public static void WriteToLog(string data)
		{
			WriteToLog(data, String.Empty);
		}
	} //end class Tools
}