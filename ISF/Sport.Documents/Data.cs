using System;

namespace Sport.Documents
{
	public class Data
	{
		public struct Table
		{
			public static Table Empty;
			static Table()
			{
				Empty = new Table();
				Empty.Headers = null;
				Empty.Rows = null;
			}
			public Column[] Headers;
			public Row[] Rows;
			public int HeatIndex;
			public string Caption;
			
			public double GetRemainingWidth()
			{
				double result=1;
				for (int i=0; i<this.Headers.Length; i++)
				{
					result -= this.Headers[i].RelativeWidth;
				}
				return result;
			}
			
			/*
			public string ToHTML()
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("<table border=\"1\" class=\"CompetitionReportTable\">");
				sb.Append("");
				sb.Append("</table>");
				return sb.ToString();
			}
			*/
		}
		
		public struct Column
		{
			public string Text;
			public double RelativeWidth;
			public bool ShowBorder;
			public bool IsRelativeWidth;
			public int AbsoluteWidth;
			
			public Column(string text, int absoluteWidth, bool showBorder)
			{
				this.Text = text;
				this.RelativeWidth = 0;
				this.ShowBorder = showBorder;
				this.AbsoluteWidth = absoluteWidth;
				this.IsRelativeWidth = false;
			}
			
			public Column(string text, double relativeWidth, bool showBorder)
			{
				this.Text = text;
				this.RelativeWidth = relativeWidth;
				this.ShowBorder = showBorder;
				this.AbsoluteWidth = 0;
				this.IsRelativeWidth = true;
			}
			public Column(string text, double relativeWidth)
				: this(text, relativeWidth, true)
			{
			}
		}
		
		public struct Row
		{
			public Cell[] Cells;
			public string[] GetCellsText()
			{
				string[] result=new string[Cells.Length];
				for (int i=0; i<Cells.Length; i++)
				{
					result[i] = Cells[i].Text;
				}
				return result;
			}
			public void AddCells(string[] arrCellTexts, bool showBorder)
			{
				this.Cells = new Cell[arrCellTexts.Length];
				for (int i=0; i<arrCellTexts.Length; i++)
					this.Cells[i] = new Cell(arrCellTexts[i], showBorder);
			}
		}
		
		public struct Cell
		{
			public string Text;
			public object Data;
			public int InnerCells;
			public bool ShowBorder;
			public Sport.Documents.Borders Borders;
			public Sport.Documents.TextAlignment Alignment;
			public Cell(string text, object data, int innerCells, bool showBorder)
			{
				this.Text = text;
				this.Data = data;
				this.InnerCells = innerCells;
				this.ShowBorder = showBorder;
				this.Borders = Sport.Documents.Borders.All;
				this.Alignment = Sport.Documents.TextAlignment.Default;
			}
			public Cell(string text, object data, int innerCells)
				: this (text, data, innerCells, true)
			{
			}
			public Cell(string text, object data, bool showBorder)
				: this (text, data, 1, showBorder)
			{
			}
			public Cell(string text, object data)
				: this (text, data, 1)
			{
			}
			public Cell(string text, bool showBorder)
				: this(text, null, showBorder)
			{
			}
			public Cell(string text)
				: this(text, null)
			{
			}
		}
		
		public class DataTableRowsComparer : System.Collections.IComparer
		{
			private int _colIndex=0;
			
			public DataTableRowsComparer(int colIndex)
			{
				_colIndex = colIndex;
			}
			
			public int Compare(object x, object y)
			{
				Data.Row r1=(Data.Row) x;
				Data.Row r2=(Data.Row) y;
				Data.Cell c1=r1.Cells[_colIndex];
				Data.Cell c2=r2.Cells[_colIndex];
				return Int32.Parse(c2.Text).CompareTo(Int32.Parse(c1.Text));
			}
		}
	}
}
