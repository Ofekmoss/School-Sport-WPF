using System;
using System.Drawing;
//using Word=Microsoft.Office.Interop.Word;
using Core=Microsoft.Office.Core;

namespace Sport.Documents
{
	#region TableItem Class

	public class TableItem : PageItem, ITextItemStyle, ISectionObject
	{
		#region TableColumn

		public class TableColumn : DocumentItem, ITextItemStyle
		{
			public TableColumn()
			{
				_alignment = TextAlignment.Default;
				_lineAlignment = TextAlignment.Default;
			}

			#region Cloning
			
			public TableColumn(TableColumn column)
				: base(column)
			{
				_alignment = column._alignment;
				_lineAlignment = column._lineAlignment;
				_width = column._width;
			}

			public override DocumentItem Clone()
			{
				return new TableColumn(this);
			}


			#endregion

            private int _width;
			public int Width
			{
				get { return _width; }
				set { _width = value; }
			}

			private TextAlignment _alignment;
			public TextAlignment Alignment
			{
				get { return _alignment; }
				set { _alignment = value; }
			}

			private TextAlignment _lineAlignment;
			public TextAlignment LineAlignment
			{
				get { return _lineAlignment; }
				set { _lineAlignment = value; }
			}
		}

		#endregion

		#region TableColumnCollection Class

		public class TableColumnCollection : DocumentItemCollection
		{
			public TableColumnCollection(TableItem tableItem)
				: base(tableItem)
			{
			}

			protected override void SetItem(int index, DocumentItem value)
			{
				if (value != null && !(value is TableColumn))
				{
					throw new DocumentException("Child item in table column collection must inherit TableColumn");
				}

				base.SetItem (index, value);
			}

			public TableItem TableItem
			{
				get { return ((TableItem) DocumentItem); }
			}

			public new TableColumn this[int index]
			{
				get { return (TableColumn) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, TableColumn value)
			{
				InsertItem(index, value);
			}

			public void Remove(TableColumn value)
			{
				RemoveItem(value);
			}

			public bool Contains(TableColumn value)
			{
				return base.Contains(value);
			}

			public int IndexOf(TableColumn value)
			{
				return base.IndexOf(value);
			}

			public int Add(TableColumn value)
			{
				return AddItem(value);
			}
		}

		#endregion

		#region TableRow

		public class TableRow : DocumentItem, ITextItemStyle
		{
			public TableRow(object[] values)
			{
				_values = values;
				_alignment = TextAlignment.Default;
				_lineAlignment = TextAlignment.Default;
				_height = 16;
				Border = System.Drawing.SystemPens.WindowFrame;
			}

			public TableRow()
			{
				_values = null;
				_alignment = TextAlignment.Default;
				_lineAlignment = TextAlignment.Default;
				_height = 16;
				_verticalPadding = 0;
				Border = System.Drawing.SystemPens.WindowFrame;
			}

			#region Cloning

			public TableRow(TableRow row)
				: base(row)
			{
				_values = row._values;
				_alignment = row._alignment;
				_lineAlignment = row._lineAlignment;
				_height = row._height;
				_verticalPadding = row._verticalPadding;
			}

			public override DocumentItem Clone()
			{
				return new TableRow(this);
			}


			#endregion

			private int _height;
			public int Height
			{
				get { return _height; }
				set { _height = value; }
			}

			private object[] _values;
			public object[] Values
			{
				get { return _values; }
				set { _values = value; }
			}

			private TextAlignment _alignment;
			public TextAlignment Alignment
			{
				get { return _alignment; }
				set { _alignment = value; }
			}

			private TextAlignment _lineAlignment;
			public TextAlignment LineAlignment
			{
				get { return _lineAlignment; }
				set { _lineAlignment = value; }
			}
			
			private int _verticalPadding = 0;
			public int VerticalPadding
			{
				get { return _verticalPadding; }
				set { _verticalPadding = value; }
			}
		}

		#endregion

		#region TableRowCollection Class

		public class TableRowCollection : DocumentItemCollection
		{
			public TableRowCollection(TableItem tableItem)
				: base(tableItem)
			{
			}

			protected override void SetItem(int index, DocumentItem value)
			{
				if (!(value is TableRow))
				{
					throw new DocumentException("Child item in table row collection must inherit TableRow");
				}

				base.SetItem (index, value);
			}

			public TableItem TableItem
			{
				get { return ((TableItem) DocumentItem); }
			}

			public new TableRow this[int index]
			{
				get { return (TableRow) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, TableRow value)
			{
				InsertItem(index, value);
			}

			public void Insert(int index, object[] values)
			{
				InsertItem(index, new TableRow(values));
			}

			public void Remove(TableRow value)
			{
				RemoveItem(value);
			}

			public bool Contains(TableRow value)
			{
				return base.Contains(value);
			}

			public int IndexOf(TableRow value)
			{
				return base.IndexOf(value);
			}

			public int Add(TableRow value)
			{
				return AddItem(value);
			}

			public int Add(object[] values)
			{
				return AddItem(new TableRow(values));
			}
		}

		#endregion

		#region TableCell
		public class TableCell : IDocumentItemStyle, ITextItemStyle
		{
			private string _text;
			private int _innerCells;
			private Sport.Documents.Direction _direction;
			private Font _font;
			private Brush _backBrush;
			private Brush _foreBrush;
			private Pen _border;
			private Sport.Documents.TextAlignment _alignment;
			private Sport.Documents.TextAlignment _lineAlignment;
			private Sport.Documents.Borders _borders;
			
			public TableCell(string text)
			{
				_direction = Direction.Inherit;
				_font = null;
				_backBrush = null;
				_foreBrush = null;
				_border = null;
				_alignment = TextAlignment.Default;
				_lineAlignment = TextAlignment.Default;
				_text = text;
				_innerCells = 0;
				_borders = Sport.Documents.Borders.All;
			}
			
			public TableCell(TableCell cell)
			{
				_direction = cell._direction;
				_font = cell._font;
				_backBrush = cell._backBrush;
				_foreBrush = cell._foreBrush;
				_border = cell._border;
				_alignment = cell._alignment;
				_lineAlignment = cell._lineAlignment;
				_text = cell._text;
				_innerCells = 0;
				_borders = cell._borders;
			}
			
			public TableCell()
				: this((string) null)
			{
			}
			
			public string Text
			{
				get { return _text; }
				set { _text = value; }
			}
			
			public int InnerCells
			{
				get { return _innerCells; }
				set { _innerCells = value; }
			}
			
			public Sport.Documents.Borders Borders
			{
				get { return _borders; }
				set { _borders = value; }
			}
			
			#region IDocumentItemStyle Members
			public Sport.Documents.Direction Direction
			{
				get { return _direction; }
				set { _direction = value; }
			}
			
			public Font Font
			{
				get { return _font; }
				set { _font = value; }
			}
			

			public Brush BackBrush
			{
				get { return _backBrush; }
				set { _backBrush = value; }
			}
			
			public Color BackColor
			{
				get 
				{
					if (_backBrush is SolidBrush)
						return ((SolidBrush) _backBrush).Color;
					return Color.Transparent;
				}
				set
				{
					if (value == Color.Transparent)
						_backBrush = null;
					else
						_backBrush = new SolidBrush(value);
				}
			}
			
			public Brush ForeBrush
			{
				get { return _foreBrush; }
				set { _foreBrush = value; }
			}
			
			public Color ForeColor
			{
				get 
				{
					if (_foreBrush is SolidBrush)
						return ((SolidBrush) _foreBrush).Color;
					return Color.Transparent;
				}
				set
				{
					if (value == Color.Transparent)
						_foreBrush = null;
					else
						_foreBrush = new SolidBrush(value);
				}
			}
			
			public Pen Border
			{
				get { return _border; }
				set { _border = value; }
			}
			#endregion
			
			#region ITextItemStyle Members
			public Sport.Documents.TextAlignment Alignment
			{
				get { return _alignment; }
				set { _alignment = value; }
			}
			
			public Sport.Documents.TextAlignment LineAlignment
			{
				get { return _lineAlignment; }
				set { _lineAlignment = value; }
			}
			#endregion
			
			public override string ToString()
			{
				return _text;
			}
		}
		#endregion

		private TableColumnCollection columns;
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public TableColumnCollection Columns
		{
			get { return columns; }
		}

		private bool _relativeColumnWidth;
		public bool RelativeColumnWidth
		{
			get { return _relativeColumnWidth; }
			set { _relativeColumnWidth = value; }
		}

		private TableRowCollection rows;
		[System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Content)]
		public TableRowCollection Rows
		{
			get { return rows; }
		}

		public TableItem(Rectangle bounds)
			: base(bounds)
		{
			_alignment = TextAlignment.Default;
			_lineAlignment = TextAlignment.Default;
			_relativeColumnWidth = true;

			columns = new TableColumnCollection(this);
			rows = new TableRowCollection(this);
		}

		public TableItem()
			: this(new Rectangle(0, 0, 200, 200))
		{
		}

		#region Cloning

		public TableItem(TableItem item)
			: base(item)
		{
			_alignment = item._alignment;
			_lineAlignment = item._lineAlignment;
			_relativeColumnWidth = item._relativeColumnWidth;

			columns = new TableColumnCollection(this);
			rows = new TableRowCollection(this);

			foreach (TableColumn column in item.columns)
			{
				columns.Add(column.Clone() as TableColumn);
			}
			foreach (TableRow row in item.rows)
			{
				rows.Add(row.Clone() as TableRow);
			}
		}

		public override DocumentItem Clone()
		{
			return new TableItem(this);
		}


		#endregion

		private TextAlignment _alignment;
		public TextAlignment Alignment
		{
			get 
			{ 
				if (_alignment == TextAlignment.Default)
					return TextAlignment.Near;
				return _alignment; 
			}
			set { _alignment = value; }
		}

		private TextAlignment _lineAlignment;
		public TextAlignment LineAlignment
		{
			get 
			{ 
				if (_lineAlignment == TextAlignment.Default)
					return TextAlignment.Near;
				return _lineAlignment; 
			}
			set { _lineAlignment = value; }
		}

		private StringFormat paintStringFormat;

		private void PaintCell(Graphics g, int row, int column, 
			int left, int top, int right, int bottom)
		{
			TableRow tableRow = rows[row];
			TableColumn tableColumn = columns[column];
			IDocumentItemStyle dis = null;
			System.Drawing.Rectangle cellBounds = new System.Drawing.Rectangle(left, top, right - left, bottom - top);
			System.Drawing.Rectangle textBounds = new System.Drawing.Rectangle(
				left, top+tableRow.VerticalPadding, right - left, bottom - top);
			
			if (tableRow.Values != null && column < tableRow.Values.Length)
			{
				object cell = tableRow.Values[column];
				
				if (cell != null)
				{
					if (cell is TableCell)
					{
						int innerCells=(cell as TableCell).InnerCells;
						if (innerCells > 1)
						{
							int curPosX=cellBounds.Left;
							int posY=cellBounds.Top;
							int height=cellBounds.Height;
							int width=(int) (((double) cellBounds.Width)/((double) innerCells));
							for (int i=0; i<innerCells; i++)
							{
								g.DrawRectangle(SystemPens.WindowFrame, curPosX, posY, width, height);
								curPosX += width;
							}
						}
					}
					
					ITextItemStyle tis = cell as ITextItemStyle;
					
					if (tis != null && tis.Alignment != TextAlignment.Default)
						paintStringFormat.Alignment = (StringAlignment) tis.Alignment;
					else if (tableRow.Alignment != TextAlignment.Default)
						paintStringFormat.Alignment = (StringAlignment) tableRow.Alignment;
					else if (tableColumn.Alignment != TextAlignment.Default)
						paintStringFormat.Alignment = (StringAlignment) tableColumn.Alignment;
					else
						paintStringFormat.Alignment = (StringAlignment) Alignment;
					
					if (tis != null && tis.LineAlignment != TextAlignment.Default)
						paintStringFormat.LineAlignment = (StringAlignment) tis.LineAlignment;
					else if (tableRow.LineAlignment != TextAlignment.Default)
						paintStringFormat.LineAlignment = (StringAlignment) tableRow.LineAlignment;
					else if (tableColumn.LineAlignment != TextAlignment.Default)
						paintStringFormat.LineAlignment = (StringAlignment) tableColumn.LineAlignment;
					else
						paintStringFormat.LineAlignment = (StringAlignment) LineAlignment;
					
					Font font = tableColumn.Font;
					Brush foreBrush = tableColumn.ForeBrush;
					
					if (font != tableRow.Font)
						font = tableRow.Font;
					if (foreBrush != tableRow.ForeBrush)
						foreBrush = tableRow.ForeBrush;
					
					dis = cell as IDocumentItemStyle;
					
					if (dis != null)
					{
						if (dis.BackBrush != null)
						{
							g.FillRectangle(dis.BackBrush, cellBounds);
						}
						if (dis.Font != null)
							font = dis.Font;
						if (dis.ForeBrush != null)
							foreBrush = dis.ForeBrush;
					}
					
					string strText=cell.ToString();
					g.DrawString(cell.ToString(), font, foreBrush, 
						textBounds, paintStringFormat);
					
					if ((dis != null)&&(dis.Border != null))
					{
						Pen pen=dis.Border;
						if (dis is TableCell)
						{
							Sport.Documents.Borders borders=(dis as TableCell).Borders;
							if (borders != Borders.All)
							{
								int cellLeft=cellBounds.Left;
								int cellTop=cellBounds.Top;
								int cellBottom=cellBounds.Bottom;
								int cellRight=cellBounds.Right;
								if ((borders & Sport.Documents.Borders.Left) != 0)
									g.DrawLine(pen, cellLeft, cellTop, cellLeft, cellBottom);
								if ((borders & Sport.Documents.Borders.Top) != 0)
									g.DrawLine(pen, cellLeft, cellTop, cellRight, cellTop);
								if ((borders & Sport.Documents.Borders.Right) != 0)
									g.DrawLine(pen, cellRight, cellTop, cellRight, cellBottom);
								if ((borders & Sport.Documents.Borders.Bottom) != 0)
									g.DrawLine(pen, cellLeft, cellBottom, cellRight, cellBottom);
							}
							else
							{
								g.DrawRectangle(pen, cellBounds);
							}
						}
						else
						{
							g.DrawRectangle(pen, cellBounds);
						}
					}
				}
			}
			
			if (dis == null && tableColumn.Border != null)
			{
				g.DrawRectangle(tableColumn.Border, cellBounds);
			}
		}
        
		private void PaintColumns(Graphics g, int row, int top, int bottom)
		{
			int left = Bounds.Right, right = Bounds.Left;

			bool brk = false;

			for (int n = 0; n < columns.Count; n++)
			{
				TableColumn column = columns[n];

				if (Direction == Direction.Right)
				{
					right = left;
					left -= column.Width;
					if (left <= Bounds.Left)
					{
						left = Bounds.Left;
						brk = true;
					}
				}
				else
				{
					left = right;
					right += column.Width;
					if (right >= Bounds.Right)
					{
						right = Bounds.Right;
						brk = true;
					}
				}

				if (column.BackBrush != null)
				{
					g.FillRectangle(column.BackBrush, left, top, right - left, bottom - top);
				}

				PaintCell(g, row, n, left, top, right, bottom);

				if (brk)
					break;
			}
		}

		private void PaintRows(Graphics g)
		{
			int top, bottom;
			bottom = Bounds.Top;
			for (int n = 0; n < rows.Count; n++)
			{
				TableRow row = rows[n];

				top = bottom;
				bottom += row.Height;
				if (bottom > Bounds.Bottom)
					bottom = Bounds.Bottom;

				if (row.BackBrush != null)
				{
					g.FillRectangle(row.BackBrush, Bounds.Left, top, Bounds.Width, bottom - top);
				}

				PaintColumns(g, n, top, bottom);

				if (row.Border != null)
				{
					g.DrawRectangle(row.Border, Bounds.Left, top, Bounds.Width, bottom - top);
				}

				if (bottom == Bounds.Bottom)
					break;
			}
		}

		public void CalculateColumnWidth()
		{
			int total = 0;
			foreach (TableColumn column in columns)
				total += column.Width;

			int width = Bounds.Width;
			int widthLeft = width;
			if (total != width)
			{
				for (int n = 0; n < columns.Count - 1; n++)
				{
					columns[n].Width = columns[n].Width * width / total;
					widthLeft -= columns[n].Width;
				}

				columns[columns.Count - 1].Width = widthLeft;
			}
		}

		public override void PaintItem(Graphics g)
		{
			base.PaintItem (g);

			paintStringFormat = new StringFormat();

			if (Direction == Direction.Right)
				paintStringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;

			if (_relativeColumnWidth)
			{
				CalculateColumnWidth();
			}

			PaintRows(g);

			if (Border != null)
				g.DrawRectangle(Border, Bounds);
		}

		/*
		public override void Save(Word.Document document, Sport.Common.ProgressHelper progressHelper)
		{
			object missing = Type.Missing;
			Word.Shape shape = document.Shapes.AddShape((int) Core.MsoAutoShapeType.msoShapeRectangle, 
				(float) Bounds.Left * 0.72f, (float) Bounds.Top * 0.72f, 
				(float) Bounds.Width * 0.72f, (float) Bounds.Height * 0.72f, 
				ref missing);

			if (progressHelper != null && progressHelper.IsCancelled)
				return;

			WordHelper.SetFill(shape.Fill, BackBrush);
			shape.Line.Visible = Core.MsoTriState.msoFalse;

			shape.TextFrame.MarginLeft = 0;
			shape.TextFrame.MarginTop = 0;
			shape.TextFrame.MarginRight = 0;
			shape.TextFrame.MarginBottom = 0;

			Word.Table table = document.Tables.Add(shape.TextFrame.TextRange, rows.Count, columns.Count, ref missing, ref missing);

			WordHelper.SetFont(table.Range.Font, Font);
			WordHelper.SetShading(table.Shading, BackBrush);

			if (_relativeColumnWidth)
			{
				CalculateColumnWidth();
			}

			Word.Column wordColumn = table.Columns.First;
			foreach (TableColumn column in columns)
			{
				wordColumn.SetWidth((float) (column.Width < 16 ? 16 : column.Width) * 0.72f, Word.WdRulerStyle.wdAdjustNone);
				if (column.BackBrush != null)
				{
					WordHelper.SetShading(wordColumn.Shading, column.BackBrush);
				}
				if (column.Border != null)
				{
					WordHelper.SetBorder(wordColumn.Borders[Word.WdBorderType.wdBorderLeft], column.Border);
					WordHelper.SetBorder(wordColumn.Borders[Word.WdBorderType.wdBorderTop], column.Border);
					WordHelper.SetBorder(wordColumn.Borders[Word.WdBorderType.wdBorderRight], column.Border);
					WordHelper.SetBorder(wordColumn.Borders[Word.WdBorderType.wdBorderBottom], column.Border);
				}
				wordColumn = wordColumn.Next;

				if (progressHelper != null && progressHelper.IsCancelled)
					return;
			}

			IDocumentItemStyle dis;
			ITextItemStyle tis;

			Word.Row wordRow = table.Rows.First;
			foreach (TableRow row in rows)
			{
				if (progressHelper != null && progressHelper.IsCancelled)
					return;

				wordRow.SetHeight((float) row.Height * 0.72f, Word.WdRowHeightRule.wdRowHeightExactly);
				if (row.BackBrush != null)
				{
					WordHelper.SetShading(wordRow.Shading, row.BackBrush);
				}

				if (wordRow.Index == 1)
					WordHelper.SetBorder(wordRow.Borders[Word.WdBorderType.wdBorderTop], Border);
				else
					WordHelper.SetBorder(wordRow.Borders[Word.WdBorderType.wdBorderTop], row.Border);
				if (wordRow.Index == rows.Count)
					WordHelper.SetBorder(wordRow.Borders[Word.WdBorderType.wdBorderBottom], Border);
				else		
					WordHelper.SetBorder(wordRow.Borders[Word.WdBorderType.wdBorderBottom], row.Border);

				WordHelper.SetBorder(wordRow.Borders[Word.WdBorderType.wdBorderLeft], Border);
				WordHelper.SetBorder(wordRow.Borders[Word.WdBorderType.wdBorderRight], Border);

				if (progressHelper != null)
					progressHelper.AddProgress(0);

				for (int col = 0; col < columns.Count; col++)
				{
					Word.Cell wordCell = table.Cell(wordRow.Index, col + 1);
					System.Drawing.Pen cellBorder = null;

					if (row.Values != null && col < row.Values.Length)
					{
						object cell = row.Values[col];

						if (cell != null)
						{
							TableColumn column = columns[col];
							
							dis = cell as IDocumentItemStyle;

							// Setting font
							if (dis != null && dis.Font != null)
							{
								WordHelper.SetFont(wordCell.Range.Font, dis.Font);
							}
							else if (row.Font != null)
							{
								WordHelper.SetFont(wordCell.Range.Font, row.Font);
							}
							else if (column.Font != null)
							{
								WordHelper.SetFont(wordCell.Range.Font, column.Font);
							}

							if (dis != null)
							{
								// Setting backgroung
								if (dis.BackBrush != null)
								{
									WordHelper.SetShading(wordCell.Shading, dis.BackBrush);
								}

								cellBorder = dis.Border;
							}

							tis = cell as ITextItemStyle;

							// Setting Alignment
							if (tis != null && tis.Alignment != TextAlignment.Default)
							{
								wordCell.Range.ParagraphFormat.Alignment = WordHelper.GetAlignment(Direction, tis.Alignment);
							}
							else if (row.Alignment != TextAlignment.Default)
							{
								wordCell.Range.ParagraphFormat.Alignment = WordHelper.GetAlignment(Direction, row.Alignment);
							}
							else if (column.Alignment != TextAlignment.Default)
							{
								wordCell.Range.ParagraphFormat.Alignment = WordHelper.GetAlignment(Direction, column.Alignment);
							}

							wordCell.Range.InsertAfter(cell.ToString());
						}
					}

					// Setting border
					if (col == 0)
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderRight], Border);
					else
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderRight], cellBorder);
					if (col == columns.Count - 1)
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderLeft], Border);
					else
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderLeft], cellBorder);
					if (wordRow.Index == 1)
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderTop], Border);
					else
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderTop], cellBorder);
					if (wordRow.Index == rows.Count)
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderBottom], Border);
					else
						WordHelper.SetBorder(wordCell.Borders[Word.WdBorderType.wdBorderBottom], cellBorder);

					if (progressHelper != null)
					{
						if (progressHelper.IsCancelled)
							return;
						progressHelper.AddProgress(0);
					}
				}

				wordRow = wordRow.Next;
			}
		}

		public override void Save(Word.Document document)
		{
			Save(document, null);
		}
		*/

		#region ISectionObject Members

		private int _fixedRows = 1;
		public int FixedRows
		{
			get { return _fixedRows; }
			set { _fixedRows = value; }
		}

		private int _currentRow;

		public void InitializeSave(DocumentBuilder builder)
		{
			paintStringFormat = new StringFormat();

			if (Direction == Direction.Right)
				paintStringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;

			if (_relativeColumnWidth)
			{
				int total = 0;
				foreach (TableColumn column in columns)
					total += column.Width;

				int width = Bounds.Width;
				int widthLeft = width;
				if (total != width)
				{
					for (int n = 0; n < columns.Count - 1; n++)
					{
						columns[n].Width = columns[n].Width * width / total;
						widthLeft -= columns[n].Width;
					}

					columns[columns.Count - 1].Width = widthLeft;
				}
			}

			for (int r = 0; r < rows.Count; r++)
			{
				TableRow row = rows[r];
				MeasureRow(row, builder.MeasureGraphics);
			}

			_currentRow = _fixedRows;
		}

		public bool SavePage(DocumentBuilder builder, Page page)
		{
			// Creating page's table item
			TableItem tableItem = new TableItem(Bounds);
			tableItem.Border = Border;
			tableItem.Alignment = _alignment;
			tableItem.LineAlignment = _lineAlignment;
			tableItem.RelativeColumnWidth = _relativeColumnWidth;

			foreach (TableColumn column in columns)
			{
				tableItem.Columns.Add(column.Clone() as TableColumn);
			}

			page.Items.Add(tableItem);

			int height = 0;

            for (int fr = 0; fr < _fixedRows; fr++)
			{
				TableRow row = rows[fr].Clone() as TableRow;
				tableItem.Rows.Add(row);
				height += row.Height;
			}

			bool nextPage = false;
			for (int r = _currentRow; r < rows.Count && !nextPage; r++)
			{
				TableRow row = rows[r];
				height += row.Height;
				if (height > Bounds.Height)
				{
					nextPage = true;
					_currentRow = r;
				}
				else
				{
					tableItem.Rows.Add(row.Clone() as TableRow);
				}
			}

			return nextPage;
		}

		public void FinalizeSave(DocumentBuilder builder)
		{
		}

		public void MeasureRow(TableRow tableRow, Graphics g)
		{
			int cols = columns.Count;
			if (tableRow.Values.Length < cols)
				cols = tableRow.Values.Length;
			
			int height;
			tableRow.Height = 16+(2*tableRow.VerticalPadding);
			
			for (int c = 0; c < cols; c++)
			{
				TableColumn tableColumn = columns[c];
				IDocumentItemStyle dis = null;
				
				object cell = tableRow.Values[c];
				
				if (cell != null)
				{
					ITextItemStyle tis = cell as ITextItemStyle;
					
					if (paintStringFormat == null)
					{
						paintStringFormat = new StringFormat();
						
						if (Direction == Direction.Right)
							paintStringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
					}
					
					if (tis != null && tis.Alignment != TextAlignment.Default)
						paintStringFormat.Alignment = (StringAlignment) tis.Alignment;
					else if (tableRow.Alignment != TextAlignment.Default)
						paintStringFormat.Alignment = (StringAlignment) tableRow.Alignment;
					else if (tableColumn.Alignment != TextAlignment.Default)
						paintStringFormat.Alignment = (StringAlignment) tableColumn.Alignment;
					else
						paintStringFormat.Alignment = (StringAlignment) Alignment;
					
					if (tis != null && tis.LineAlignment != TextAlignment.Default)
						paintStringFormat.LineAlignment = (StringAlignment) tis.LineAlignment;
					else if (tableRow.LineAlignment != TextAlignment.Default)
						paintStringFormat.LineAlignment = (StringAlignment) tableRow.LineAlignment;
					else if (tableColumn.LineAlignment != TextAlignment.Default)
						paintStringFormat.LineAlignment = (StringAlignment) tableColumn.LineAlignment;
					else
						paintStringFormat.LineAlignment = (StringAlignment) LineAlignment;
					
					Font font = tableColumn.Font;
					
					if (font != tableRow.Font && tableRow.Font != null)
						font = tableRow.Font;
					
					dis = cell as IDocumentItemStyle;
					
					if (dis != null && dis.Font != null)
					{
						font = dis.Font;
					}
					
					height = (int) g.MeasureString(cell.ToString(), font, tableColumn.Width, paintStringFormat).Height;
					
					if (height > (tableRow.Height-(2*tableRow.VerticalPadding)))
						tableRow.Height = height+(2*tableRow.VerticalPadding);
				}
			}
		}

		#endregion
	}

	#endregion
}
