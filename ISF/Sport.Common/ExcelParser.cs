using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
//using Excel;

namespace Sport.ExcelParser
{
	/// <summary>
	/// Used to Create, Read and Write to and from an Excel file.
	/// </summary>
	/// 
	public class ExcelParser
	{
		private string m_FileName;
		private bool m_ChangeFont;

		private const int colorB = 65536;
		private const int colorG = 256;

		private Worksheet m_oSheet;
		private Workbook m_oWB;
		private ApplicationClass m_oXL;

		public ExcelParser(string fileName, bool changeFont)
		{
			m_FileName = fileName;
			m_ChangeFont = changeFont;
			createExcelFile();
		}

		public ExcelParser(string fileName)
			: this(fileName, true)
		{
		}

		private void createExcelFile()
		{
			try
			{
				// Show the contents in an Excel window
				m_oXL = new ApplicationClass();
				System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
				m_oWB = m_oXL.Workbooks.Add(Missing.Value);

				m_oSheet = (Worksheet)m_oWB.ActiveSheet;

				setSheetProperties();
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to create excel file:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}

		}

		private void setSheetProperties()
		{
			m_oSheet.DisplayRightToLeft = true;

			if (m_ChangeFont)
			{
				m_oSheet.Cells.Font.Bold = true;
				m_oSheet.Cells.Font.Size = 12;
			}
		}

		private void saveFile()
		{
			try
			{
				m_oWB.SaveAs(m_FileName, XlFileFormat.xlWorkbookNormal, Missing.Value, Missing.Value, Missing.Value,
					false, XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value,
					Missing.Value, Missing.Value);
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to save excel file:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}

		}

		public void closeFile(bool blnSave)
		{
			if (blnSave)
				saveFile();
			
			try
			{
				m_oWB.Close(false, Missing.Value, Missing.Value);
			}
			catch
			{ }

			m_oXL.Quit();
		}

		public void closeFile()
		{
			closeFile(true);
		}

		public void insertPicture(int locationX, int locationY, string imagePath)
		{
			try
			{
				Shape newShape;
				newShape = m_oSheet.Shapes.AddPicture(imagePath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, 0, 0, 50, 50);
				//newShape = m_oSheet.Shapes.AddShape(Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle,locationX,locationY,50,50);
				//newShape.Fill.UserPicture(imagePath);

				newShape.ScaleHeight((float)1, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoScaleFrom.msoScaleFromTopLeft);
				newShape.ScaleWidth((float)1, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoScaleFrom.msoScaleFromBottomRight);
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to add image into excel file:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}

		}

		public void changeCellBorders(int row, int column, bool isOn)
		{
			try
			{
				Color borderColor = new Color();
				if (isOn)
					borderColor = Color.Black;
				else
					borderColor = Color.White;
				((Range)m_oSheet.Cells[row, column]).BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, ConvertColor(borderColor));
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to change excel border style:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}
		}

		public void writeToCell(int row, int column, string stringToWrite, Color color)
		{
			ASSERT((row >= 1), "Excel Parser: invalid row: " + row);
			ASSERT((column >= 1), "Excel Parser: invalid column: " + column);
			try
			{
				m_oSheet.Cells[row, column] = stringToWrite;
				if (color != Color.Empty)
				{
					((Range)m_oSheet.Cells[row, column]).Interior.Color =
						ConvertColor(color);
				}
			}

			catch (Exception e)
			{
				MessageBox.Show("Unable to write into excel file:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}
		}

		public void writeToCell(int row, int column, string stringToWrite)
		{
			writeToCell(row, column, stringToWrite, Color.Empty);
		}

		public void refitAll()
		{
			try
			{
				m_oSheet.Columns.AutoFit();
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to refit cells:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}
		}

		public void changeCellFont(int row, int column, int fontSize)
		{
			try
			{
				((Range)m_oSheet.Cells[row, column]).Font.Size = fontSize;
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to change font:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}
		}

		public void changeCellBold(int row, int column, bool isBold)
		{
			try
			{
				((Range)m_oSheet.Cells[row, column]).Font.Bold = isBold;
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to change bold:" + e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				m_oXL.Quit();
			}
		}

		protected int ConvertColor(Color color)
		{
			int result = 0;
			result += (color.B * colorB);
			result += (color.G * colorG);
			result += color.R;
			return result;
		}

		private void ASSERT(bool condition, string message)
		{
			if (condition == false)
				throw new Exception(message);
		}
	}
}
