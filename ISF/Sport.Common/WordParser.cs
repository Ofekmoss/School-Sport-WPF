using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using Microsoft.Office.Interop.Word;
//using Word;

namespace Sport.WordParser
{
	/// <summary>
	/// Used to Create, Read and Write to and from a Word file.
	/// </summary>
	/// 
	public class WordParser
	{
		private string m_FileName;
		private const int colorB = 65536;
		private const int colorG = 256;
		private int mNumOfTables;

		private Document m_oDocument;
		private	ApplicationClass m_oWD;
	
		public WordParser(string fileName)
		{
			m_FileName = fileName;
			mNumOfTables = 1;
			createWordFile();
		}

		private void createWordFile()
		{
			try
			{
				object fileName = "normal.dot";   // template file name
				object newTemplate = false;
				object docType = 0;
				object isVisible = true;
				
				m_oWD = new ApplicationClass();
				m_oDocument = m_oWD.Documents.Add(ref fileName,ref newTemplate,ref docType,ref isVisible);
				m_oDocument.Activate();
				setDocumentProperties();

			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to create word file:" + e.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				closeFile();
			}
			
		}

		private void setDocumentProperties()
		{
			m_oWD.Selection.Font.Bold = 1;
			m_oWD.Selection.Font.Size = 12;
			m_oWD.Selection.RtlPara();
		}

		private void saveFile()
		{
			object FileName = m_FileName;
			object Format = WdSaveFormat.wdFormatDocument;
			object False = false;
			object True = true;
			object Encode = WdSaveFormat.wdFormatEncodedText;
			object EmptyString = "";
			
			m_oDocument.SaveAs(ref FileName,ref Format,ref False,ref EmptyString,
				ref True,ref EmptyString,ref False,ref False,ref False,ref False,
				ref False,ref Encode,ref False,ref False,ref False,ref False);
		}

		public void saveAndClose()
		{
			saveFile();
			closeFile();
		}

		private void closeFile()
		{
			object saveOnExit = false;
			object originalFormat = false;
			object referenceDocument = false;
			m_oWD.Quit(ref saveOnExit,ref originalFormat,ref referenceDocument);
		}

		public void writeLine(string writeText,int alignment)
		{
			try
			{
				alignSelection(alignment);
				m_oWD.Selection.TypeText(writeText);
				m_oWD.Selection.TypeParagraph();
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to write into word file:" + e.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				closeFile();
			}
		}

		public void Write(string writeText)
		{
			try
			{
				m_oWD.Selection.TypeText(writeText);
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to write into word file:" + e.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				closeFile();
			}
		}
		
		public void insertPicture(string imagePath,int alignment)
		{
			/* Under Construction
			try
			{
		
				object False = false;
				object True = true;
				object Anchor = m_oWD.Selection.Range;
				object Up = locationX;
				object Left = locationY;
				object Width = 50;
				object Height = 50;
				object Start = 0;
				object End = 0;
				//m_oDocument.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekFirstPageHeader;
				
				//InlineShape shape = m_oWD.Selection.InlineShapes.AddPicture(imagePath,ref False,ref True,ref Anchor);
				m_oWD.Selection.Range.SetRange(0,0);
				m_oWD.Selection.Range.Select();
				Shape shape = m_oDocument.Shapes.AddPicture(imagePath,ref False,ref True,ref Left,ref Up,ref Width,ref Height,ref Anchor);
				
				shape.ScaleHeight((float)1,Microsoft.Office.Core.MsoTriState.msoCTrue,Microsoft.Office.Core.MsoScaleFrom.msoScaleFromTopLeft);
				shape.ScaleWidth((float)1,Microsoft.Office.Core.MsoTriState.msoCTrue,Microsoft.Office.Core.MsoScaleFrom.msoScaleFromTopLeft);
				
				

				//m_oWD.Selection.MoveUp(

				shape.RelativeHorizontalPosition = WdRelativeHorizontalPosition.wdRelativeHorizontalPositionMargin;
				shape.Left = ((float)WdShapePosition.wdShapeRight);
				shape.RelativeVerticalPosition = WdRelativeVerticalPosition.wdRelativeVerticalPositionMargin;
				
				m_oDocument.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;	
				
				//m_oWD.Selection.InlineShapes.AddPicture(imagePath,ref False,ref True,ref Anchor);
				//System.IO.File.Delete(tempFileName);
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to insert picture into word file:" + e.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				closeFile();
			}
			*/
		}

		public int insertTable(int numOfRows,int numOfColumns,int alignment)
		{
			try
			{
				alignSelection(alignment);
				object tableBehavior = WdDefaultTableBehavior.wdWord9TableBehavior;
				object autoFitBehavior = WdAutoFitBehavior.wdAutoFitFixed;
				m_oDocument.Tables.Add(m_oWD.Selection.Range,numOfRows,numOfColumns,ref tableBehavior,ref autoFitBehavior);
				
				m_oWD.Selection.Tables[mNumOfTables].ApplyStyleHeadingRows = true;
				m_oWD.Selection.Tables[mNumOfTables].ApplyStyleLastRow = true;
				m_oWD.Selection.Tables[mNumOfTables].ApplyStyleFirstColumn = true;
				m_oWD.Selection.Tables[mNumOfTables].ApplyStyleLastColumn = true;
				mNumOfTables++;
				return mNumOfTables - 1;
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to add table:" + e.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				closeFile();
				return -1;
			}
		}

        public void writeTable(int tableNum,int row,int column,Color backColor,string textToWrite)
		{
			try
			{
				int convertedColor = ConvertColor(backColor);
				m_oWD.Selection.Tables[tableNum].Cell(row,column).Select();
				m_oWD.Selection.Cells.Shading.BackgroundPatternColor = (WdColor)convertedColor;
				m_oWD.Selection.TypeText(textToWrite);

				if (row == m_oWD.Selection.Tables[tableNum].Rows.Count && column == m_oWD.Selection.Tables[tableNum].Columns.Count)
				{
					
					
					object Unit = WdUnits.wdLine;
					object Count = 1;
					object Extend = false;
					m_oWD.Selection.MoveDown(ref Unit,ref Count,ref Extend);
				}
					
				
			}
			catch (Exception e)
			{
				MessageBox.Show("Unable to write into table:" + e.Message,"Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				closeFile();
			}
		}

		public void writeTable(int tableNum,int row,int column,string textToWrite)
		{
			writeTable(tableNum,row,column,System.Drawing.Color.White,textToWrite);
		}

		public void goToHeader()
		{
			m_oDocument.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekPrimaryHeader;
		}

		public void goToFooter()
		{
			m_oDocument.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekPrimaryFooter;
		}

		public void goToMainDocument()
		{
			m_oDocument.ActiveWindow.ActivePane.View.SeekView = WdSeekView.wdSeekMainDocument;
		}

		private void alignSelection(int alignment)
		{
			switch (alignment)
			{
				case 1:
					m_oWD.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
					break;
				case 2:
					m_oWD.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
					break;
				case 3:
					m_oWD.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
					break;
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
	}
}
