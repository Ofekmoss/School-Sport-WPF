using System;
using System.Collections;
using System.Drawing;
using Sport.UI.Controls;

namespace Sportsman.Producer
{
	/// <summary>
	/// Summary description for Tools.
	/// </summary>
	public class Tools
	{
		public static Sport.Documents.TableItem GridToTable(Grid grid, Rectangle bounds)
		{
			Sport.Documents.TableItem tableItem = new Sport.Documents.TableItem();
			tableItem.RelativeColumnWidth = true;

			IGridSource source = grid.Source;
			int visibleColumns = grid.Columns.VisibleCount;

			Sport.Documents.TableItem.TableCell[] header = new Sport.Documents.TableItem.TableCell[visibleColumns];
			
			int i = 0;
			for (int col=0; col < grid.Columns.Count; col++)
			{				
				Sport.UI.Controls.Grid.GridColumn column = grid.Columns[col];
				if (column.Visible)
				{
					Sport.Documents.TableItem.TableColumn tc = new Sport.Documents.TableItem.TableColumn();
					tc.Width = grid.GetColumnWidth(0, col);
					tc.Alignment = (Sport.Documents.TextAlignment) column.Alignment;
					tableItem.Columns.Add(tc);
					header[i] = new Sport.Documents.TableItem.TableCell(column.Title);
					header[i].Border = SystemPens.WindowFrame;
					i++;
				}
			}

			Sport.Documents.TableItem.TableRow headerRow = new Sport.Documents.TableItem.TableRow(header);
			headerRow.BackColor = System.Drawing.Color.SkyBlue;
			headerRow.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			headerRow.Alignment = Sport.Documents.TextAlignment.Center;
			headerRow.LineAlignment = Sport.Documents.TextAlignment.Center;

			tableItem.Rows.Add(headerRow);

			for (int row = 0; row < source.GetRowCount(); row++)
			{
				string[] record = new string[visibleColumns];
				i = 0;
				for (int col=0; col < grid.Columns.Count; col++)
				{
					Sport.UI.Controls.Grid.GridColumn column = grid.Columns[col];
					if (column.Visible)
					{
						record[i] = source.GetText(row, column.Field);
						i++;
					}
				}

				tableItem.Rows.Add(record);
			}

			return tableItem;
		}
		
		/*
		private static int[] GroupRows(IGridSource source, int group)
		{
			ArrayList result=new ArrayList();
			for (int rowIndex=0; rowIndex<source.GetRowCount(); rowIndex++)
			{
				if (source.GetGroup(rowIndex) == group)
					result.Add(rowIndex);
			}
			return (int[]) result.ToArray(typeof(int));
		}
		*/
	}
}
