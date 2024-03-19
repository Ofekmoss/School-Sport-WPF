using System;
using System.Collections;

namespace SportSite.Core
{
	public class FAQ
	{
		public struct Data
		{
			public static Data Empty;
			public string Question;
			public string Answer;
			public string Asker;
			public int Index;
			static Data()
			{
				Empty = new Data();
				Empty.Question = null;
				Empty.Answer = null;
				Empty.Asker = null;
				Empty.Index = 0;
			}
		}
		
		private static readonly string _fileName = "FAQ.xml";
		
		private static FAQ.Data[] GetAllData(bool blnApplyOrder)
		{
			Sport.Common.IniFile ini = GetIniFile();
			ArrayList arrData = new ArrayList();
			int index = 1;
			Data data = GetSingleData(ini, index);
			while (!data.Equals(Data.Empty))
			{
				arrData.Add(data);
				index++;
				data = GetSingleData(ini, index);
			}
			FAQ.Data[] result = (FAQ.Data[]) arrData.ToArray(typeof(FAQ.Data));
			if (blnApplyOrder)
			{
				int[] arrOrder = FAQ.QuestionOrder;
				if (arrOrder != null)
				{
					for (int i=0; i<arrOrder.Length; i++)
					{
						if (i >= result.Length)
							break;
						int curIndex = -1;
						for (int j=i; j<result.Length; j++)
						{
							if (result[j].Index == arrOrder[i])
							{
								curIndex = j;
								break;
							}
						}
						if (curIndex < 0)
							continue;
						FAQ.Data tmp = result[i];
						result[i] = result[curIndex];
						result[curIndex] = tmp;
					}
				}
			}
			return result;
		}
		
		public static FAQ.Data[] GetAllData()
		{
			return GetAllData(true);
		}
		
		public static FAQ.Data GetSingleData(int index)
		{
			Sport.Common.IniFile ini = GetIniFile();
			return GetSingleData(ini, index);
		}
		
		public static string AddQuestion(string strQuestion, string strAsker)
		{
			if ((strQuestion == null)||(strQuestion.Length == 0))
				return "שגיאה כללית: אין שאלה";
			Sport.Common.IniFile ini = GetIniFile();
			Data data = GetExistingData(ini, strQuestion);
			if (data.Index > 0)
				return "שאלה זו כבר קיימת במאגר";
			int index = GetAvailableIndex(ini);
			string section = "data_"+index;
			ini.WriteValue(section, "question", strQuestion);
			ini.WriteValue(section, "asker", strAsker);
			ini.WriteValue(section, "answer", "");
			return "";
		}
		
		public static string UpdateData(string strOriginalQuestion, 
			string strNewQuestion, string strNewAnswer, string strAsker)
		{
			if ((strOriginalQuestion == null)||(strOriginalQuestion.Length == 0))
				return "שגיאה כללית: אין שאלה";
			Sport.Common.IniFile ini = GetIniFile();
			Data data = GetExistingData(ini, strOriginalQuestion);
			if (data.Index < 1)
				return "שאלה זו לא קיימת במאגר: "+strOriginalQuestion;
			int index = data.Index;
			string section = "data_"+index;
			FAQ.Data[] arrAllData = null;
			if ((strNewQuestion == null)||(strNewQuestion.Length == 0))
				arrAllData = FAQ.GetAllData(false);
			ini.WriteValue(section, "answer", strNewAnswer);
			ini.WriteValue(section, "question", strNewQuestion);
			ini.WriteValue(section, "asker", strAsker);
			//System.Web.HttpContext.Current.Response.Write("question: "+strNewQuestion+"<br />");
			if ((strNewQuestion == null)||(strNewQuestion.Length == 0))
			{
				if ((arrAllData != null)&&(arrAllData.Length > 0))
				{
					FAQ.Data lastData = arrAllData[arrAllData.Length-1];
					if (lastData.Index != index)
					{
						ini.WriteValue(section, "question", lastData.Question);
						ini.WriteValue(section, "answer", lastData.Answer);
						ini.WriteValue(section, "asker", lastData.Asker);
						section = "data_"+lastData.Index;
						ini.WriteValue(section, "question", "");
						ini.WriteValue(section, "answer", "");
						ini.WriteValue(section, "asker", "");
					}
				}
			}
			return "";
		}
		
		public static int Supervisor
		{
			get
			{
				Sport.Common.IniFile ini = GetIniFile();
				string strValue = ini.ReadValue("supervisor", "id");
				return Common.Tools.CIntDef(strValue, -1);
			}
			set
			{
				Sport.Common.IniFile ini = GetIniFile();
				ini.WriteValue("supervisor", "id", value.ToString());
			}
		}
		
		public static int DynamicPage
		{
			get
			{
				Sport.Common.IniFile ini = GetIniFile();
				string strValue = ini.ReadValue("dynamicpage", "id");
				return Common.Tools.CIntDef(strValue, -1);
			}
			set
			{
				Sport.Common.IniFile ini = GetIniFile();
				ini.WriteValue("dynamicpage", "id", value.ToString());
			}
		}
		
		public static int[] QuestionOrder
		{
			get
			{
				Sport.Common.IniFile ini = GetIniFile();
				string strValue = ini.ReadValue("general", "order");
				return Sport.Common.Tools.ToIntArray(strValue, ',');
			}
			set
			{
				Sport.Common.IniFile ini = GetIniFile();
				string strOrder = "";
				if (value != null)
				{
					strOrder = 
						String.Join(",", Sport.Common.Tools.ToStringArray(value));
				}
				ini.WriteValue("general", "order", strOrder);
			}
		}
		
		private static FAQ.Data GetExistingData(Sport.Common.IniFile ini, 
			string strQuestion)
		{
			if ((strQuestion == null)||(strQuestion.Length == 0))
				return Data.Empty;
			int index = 1;
			Data data = GetSingleData(ini, index);
			while (!data.Equals(Data.Empty))
			{
				if (data.Question.ToLower() == strQuestion.ToLower())
				{
					return data;
				}
				index++;
				data = GetSingleData(ini, index);
			}
			return Data.Empty;
		}
		
		private static int GetAvailableIndex(Sport.Common.IniFile ini)
		{
			int index = 1;
			Data data = GetSingleData(ini, index);
			while (!data.Equals(Data.Empty))
			{
				index++;
				data = GetSingleData(ini, index);
			}
			return index;
		}
		
		private static FAQ.Data GetSingleData(Sport.Common.IniFile ini, int index)
		{
			Data result = Data.Empty;
			string section = "data_"+index;
			string strQuestion = ini.ReadValue(section, "question");
			if ((strQuestion != null)&&(strQuestion.Length > 0))
			{
				string strAnswer = ini.ReadValue(section, "answer");
				string strAsker = ini.ReadValue(section, "asker");
				result = new Data();
				result.Question = strQuestion;
				result.Answer = Common.Tools.CStrDef(strAnswer, "");
				result.Asker = Common.Tools.CStrDef(strAsker, "");
				result.Index = index;
			}
			return result;
		}
		
		private static Sport.Common.IniFile GetIniFile()
		{
			string strFilePath = System.Web.HttpContext.Current.Server.MapPath(_fileName);
			return new Sport.Common.IniFile(strFilePath);
		}
	}
}
