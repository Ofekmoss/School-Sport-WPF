using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace SportSite.Common
{
	/// <summary>
	/// Summary description for FastControls.
	/// </summary>
	public class FastControls
	{
		public static Label LineBreak()
		{
			return LineBreak(1);
		}

		/// <summary>
		/// returns given amount of line breaks.
		/// </summary>
		public static Label LineBreak(int linesCount)
		{
			Label result=new Label();
			string strHtml="";
			for (int i=0; i<linesCount; i++)
				strHtml += "<br />";
			result.Text = strHtml;
			return result;
		}

		public static Label TextLabel(string text)
		{
			Label result=new Label();
			/* if (SportSite.Common.Style.DefaultFontFamily.Length > 0)
				result.Style.Add("font-family", SportSite.Common.Style.DefaultFontFamily); */
			result.Text = text;
			return result;
		}
		
		public static string InputFile(string strName, bool rtl)
		{
			string result="<input type=\"file\" name=\""+strName+"\"";
			if (rtl)
				result += " dir=\"rtl\"";
			result += " />";
			return result;
		}

		public static string InputBox(string strName, string strValue, int maxChars, int size, bool autoComplete,
			params KeyValuePair<string, string>[] attributes)
		{
			string result = "<input type=\"text\" name=\"" + strName + "\" " +
				"value=\"" + Tools.CStrDef(strValue, "").Replace("\"", "&quot;") + "\"";
			if (maxChars > 0)
				result += " maxlength=\"" + maxChars + "\"";
			if (size > 0)
				result += " size=\"" + size + "\"";
			if (!autoComplete)
				result += " autocomplete=\"off\" ";
			foreach (var attribute in attributes)
				result += string.Format("{0}=\"{1}\" ", attribute.Key, attribute.Value.Replace("\"", "&quot;"));
			result += "/>";
			return result;
		}

		public static string InputBox(string strName, string strValue, int maxChars, int size)
		{
			return InputBox(strName, strValue, maxChars, size, true);
		}
		
		public static string InputBox(string strName, string strValue, int maxChars)
		{
			return InputBox(strName, strValue, maxChars, 0, true);
		}

		public static string InputBox(string strName, string strValue, bool autoComplete)
		{
			return InputBox(strName, strValue, 0, 0, autoComplete);
		}

		public static string InputBox(string strName, string strValue)
		{
			return InputBox(strName, strValue, true);
		}
		
		public static string SubmitButton(string strText, string strOnclickCode)
		{
			string result="";
			result += "<button type=\"submit\"";
			if ((strOnclickCode != null)&&(strOnclickCode.Length > 0))
				result += "onclick=\""+strOnclickCode+"\"";
			result += ">"+strText+"</button>";
			return result;
		}
		
		public static string SubmitButton(string strText)
		{
			return SubmitButton(strText, "");
		}
		
		public static Label HebTextLabel(string text)
		{
			Label result=TextLabel(text);
			result.Attributes.Add("dir", "rtl");
			return result;
		}

		public static DropDownList HebrewCombo()
		{
			DropDownList result=new DropDownList();
			result.Attributes.Add("dir", "rtl");
			return result;
		}

		public static DropDownList HebrewCombo(string id)
		{
			DropDownList result=HebrewCombo();
			result.ID = id;
			return result;
		}
		
		public static string HiddenInputHtml(string strName, string strValue)
		{
			string value = (String.IsNullOrEmpty(strValue)) ? "" : strValue;
			string strHTML="<input type=\"hidden\" name=\""+strName+"\" value=\"" + value.Replace("\"", "&quot;")+"\" />";
			return strHTML;
		}
		
		public static System.Web.UI.LiteralControl HiddenInput(string strName, string strValue)
		{
			string strHtml = HiddenInputHtml(strName, strValue);
			System.Web.UI.LiteralControl result = new System.Web.UI.LiteralControl(strHtml);
			return result;
		}

		public static TableHeaderCell HebHeaderCell(string text)
		{
			TableHeaderCell result=new TableHeaderCell();
			/* if (SportSite.Common.Style.DefaultFontFamily.Length > 0)
				result.Style.Add("font-family", SportSite.Common.Style.DefaultFontFamily); */
			result.Text = text;
			return result;
		}

		public static TableCell HebTableCell(string text, string cssClass)
		{
			TableCell result=new TableCell();
			//result.HorizontalAlign = HorizontalAlign.Center;
			if (cssClass != null)
				result.CssClass = cssClass;
			/* if (SportSite.Common.Style.DefaultFontFamily.Length > 0)
				result.Style.Add("font-family", SportSite.Common.Style.DefaultFontFamily); */
			//result.Attributes.Add("dir", "rtl");
			result.Text = text;
			return result;
		}

		public static TableCell HebTableCell(string text)
		{
			return HebTableCell(text, null);
		}
	}
}
