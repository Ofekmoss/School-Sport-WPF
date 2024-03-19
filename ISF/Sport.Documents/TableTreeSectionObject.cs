using System;
using System.Drawing;

namespace Sport.Documents
{
	public class TableTreeSectionObject : PageItem, ISectionObject
	{
		#region TableTreeRow Class

		public class TableTreeRow
		{
			private string[] _values;
			public string[] Values
			{
				get { return _values; }
				set { _values = value; }
			}

			private bool _newGroup;
			public bool NewGroup
			{
				get { return _newGroup; }
				set { _newGroup = value; }
			}

			public TableTreeRow(string[] values, bool newGroup)
			{
				_values = values;
				_newGroup = newGroup;
			}

			public TableTreeRow(string[] values)
				: this(values, false)
			{
			}
		}

		#endregion

		#region TableTreeRowCollection Class

		public class TableTreeRowCollection : Sport.Common.GeneralCollection
		{
			public TableTreeRowCollection()
			{
			}

			public TableTreeRow this[int index]
			{
				get { return (TableTreeRow) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, TableTreeRow value)
			{
				InsertItem(index, value);
			}

			public void Remove(TableTreeRow value)
			{
				RemoveItem(value);
			}

			public bool Contains(TableTreeRow value)
			{
				return base.Contains(value);
			}

			public int IndexOf(TableTreeRow value)
			{
				return base.IndexOf(value);
			}

			public int Add(TableTreeRow value)
			{
				return AddItem(value);
			}
		}

		#endregion

		#region TableTreeNode Class

		public class TableTreeNode : Sport.Common.GeneralCollection.CollectionItem
		{
			public TableTreeNode Parent
			{
				get { return (TableTreeNode) base.Owner; }
			}

			private Font _titleFont;
			public Font TitleFont
			{
				get { return _titleFont; }
				set { _titleFont = value; }
			}

			private string _title;
			public string Title
			{
				get { return _title; }
				set { _title = value; }
			}

			private string _continueTitle;
			public string ContinueTitle
			{
				get { return _continueTitle; }
				set { _continueTitle = value; }
			}

			private TableTreeRowCollection _rows;
			public TableTreeRowCollection Rows
			{
				get { return _rows; }
			}

			private TableTreeNodeCollection _children;
			public TableTreeNodeCollection Children
			{
				get { return _children; }
			}

			private bool _breakPage;
			public bool BreakPage
			{
				get { return _breakPage; }
				set { _breakPage = value; }
			}

			public TableTreeNode(Font titleFont, string title, string continueTitle,
				bool breakPage)
			{
				_titleFont = titleFont;
				_title = title;
				_continueTitle = continueTitle;
				_breakPage = breakPage;

				_rows = new TableTreeRowCollection();
				_children = new TableTreeNodeCollection(this);
			}

			public TableTreeNode(Font titleFont, string title, string continueTitle)
				: this(titleFont, title, continueTitle, false)
			{
			}

			public TableTreeNode(Font titleFont, string title)
				: this(titleFont, title, null, false)
			{
			}

			public TableTreeNode(Font titleFont, string title, bool breakPage)
				: this(titleFont, title, null, breakPage)
			{
			}

			public TableTreeNode(string title, bool breakPage)
				: this(null, title, null, breakPage)
			{
			}

			public TableTreeNode(string title)
				: this(null, title, null, false)
			{
			}
		}

		#endregion

		#region TableTreeNodeCollection

		public class TableTreeNodeCollection : Sport.Common.GeneralCollection
		{
			public TableTreeNodeCollection(TableTreeNode owner)
				: base(owner)
			{
			}

			public TableTreeNode this[int index]
			{
				get { return (TableTreeNode) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, TableTreeNode value)
			{
				InsertItem(index, value);
			}

			public void Remove(TableTreeNode value)
			{
				RemoveItem(value);
			}

			public bool Contains(TableTreeNode value)
			{
				return base.Contains(value);
			}

			public int IndexOf(TableTreeNode value)
			{
				return base.IndexOf(value);
			}

			public int Add(TableTreeNode value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private TableTreeNodeCollection _nodes;
		public TableTreeNodeCollection Nodes
		{
			get { return _nodes; }
		}

		private TableItem _baseTable;
		public TableItem BaseTable
		{
			get { return _baseTable; }
			set { _baseTable = value; }
		}

		private int[] _groupFields;
		public int[] GroupFields
		{
			get { return _groupFields; }
			set { _groupFields = value; }
		}

		public TableTreeSectionObject()
		{
			_nodes = new TableTreeNodeCollection(null);
		}
	
		#region ISectionObject Members

		private TableTreeNode lastNode;
		private int lastRow;
		private int position;
		private bool[] groupingFields;
		private System.Collections.Stack headerStack;

		public void InitializeSave(DocumentBuilder builder)
		{
			lastNode = null;
			lastRow = -1;

			if (_baseTable == null)
			{
				_baseTable = new TableItem();
				_baseTable.RelativeColumnWidth = true;
			}

			_baseTable.Bounds = Bounds;

			_baseTable.InitializeSave(builder);

			groupingFields = new bool[_baseTable.Columns.Count];

			if (_groupFields != null)
			{
				for (int n = 0; n < _groupFields.Length; n++)
				{
					if (_groupFields[n] < groupingFields.Length)
						groupingFields[_groupFields[n]] = true;
				}
			}

			 headerStack = new System.Collections.Stack();
		}

		private bool AddHeader(DocumentBuilder builder, Page page, string text, Font font)
		{
			int height = (int) builder.MeasureString(text, font, Bounds.Width).Height;

			if (position + height > Bounds.Bottom)
				return false;

			TextItem header = new TextItem(text);
			header.Font = font;
			header.Bounds = new Rectangle(Bounds.Left, position, Bounds.Width, height);
			page.Items.Add(header);

			position += height + height / 3;

			headerStack.Push(header);

			return true;
		}

		private void RemoveHeaders(DocumentBuilder builder, Page page)
		{
			while (headerStack.Count > 0)
			{
				TextItem header = (TextItem) headerStack.Pop();
				page.Items.Remove(header);
			}
		}

		private bool SaveNode(DocumentBuilder builder, Page page)
		{
			string header = lastRow == -1 || lastNode.ContinueTitle == null ? lastNode.Title : lastNode.ContinueTitle;
			if (!AddHeader(builder, page, header, lastNode.TitleFont == null ? Font : lastNode.TitleFont))
			{
				RemoveHeaders(builder, page);
				return true;
			}

			if (lastRow == -1)
				lastRow = 0;

			if (lastNode.Rows.Count > 0)
			{
				TableItem tableItem = new TableItem(_baseTable);
				page.Items.Add(tableItem);
				int height = tableItem.Rows[0].Height;

				while (lastRow < lastNode.Rows.Count)
				{
					TableTreeRow treeRow = lastNode.Rows[lastRow];
					string[] record = new string[groupingFields.Length];
					for (int n = 0; n < groupingFields.Length; n++)
					{
						if (tableItem.Rows.Count == 0 || treeRow.NewGroup ||
							!groupingFields[n])
							record[n] = treeRow.Values[n];
					}

					TableItem.TableRow row = new TableItem.TableRow(record);

					tableItem.MeasureRow(row, builder.MeasureGraphics);

					if (height + position + row.Height > Bounds.Bottom)
					{
						if (lastRow == 0) // no rows were added yet
						{
							RemoveHeaders(builder, page);
							page.Items.Remove(tableItem);
							lastRow = -1;
							return true;
						}

						tableItem.Bounds = new Rectangle(Bounds.Left, position, Bounds.Width, height);
						return true;
					}

					if (tableItem.Rows.Count != 0 && !treeRow.NewGroup)
					{
						row.Border = System.Drawing.SystemPens.ControlLight;
					}

					tableItem.Rows.Add(row);

					height += row.Height;

					if (headerStack.Count > 0)
						headerStack.Clear();

					lastRow++;
				}

				tableItem.Bounds = new Rectangle(Bounds.Left, position, Bounds.Width, height);
				position += height + 10;
			}

			lastRow = -1;

			return false;
		}

		public bool SavePage(DocumentBuilder builder, Page page)
		{
			if (lastNode == null)
			{
				if (_nodes.Count == 0)
					return false;

				lastNode = _nodes[0];
			}

			position = Bounds.Top;

			if (lastNode.Parent != null)
			{
				System.Collections.Stack parents = new System.Collections.Stack();
				TableTreeNode parent = lastNode.Parent;
				while (parent != null)
				{
					parents.Push(parent);
					parent = parent.Parent;
				}

				bool succeeded = true;

				while (parents.Count > 0 && succeeded)
				{
					TableTreeNode node = (TableTreeNode) parents.Pop();
					string title = node.ContinueTitle == null ? node.Title : node.ContinueTitle;
					if (!AddHeader(builder, page, title, node.TitleFont == null ? Font : node.TitleFont))
						succeeded = false;
				}

				// Checking if failed to add node parents
				if (!succeeded)
				{
					// Removing headers
					RemoveHeaders(builder, page);
					// Setting position back to top
					position = Bounds.Top;
				}
			}

			bool pageDone = false;

			while (!pageDone)
			{
				pageDone = SaveNode(builder, page);
				
				if (!pageDone)
				{
					TableTreeNode node = lastNode;
					TableTreeNode nextNode = null;
					// Setting next node
					// If node has children than next node is first child
					if (node.Children.Count > 0)
					{
						nextNode = node.Children[0];
					}
					else
					{
						// If not going to a child node, we won't need
						// to remove headers
						headerStack.Clear();

						// Else trying to get parent sibling
						TableTreeNode parent = node.Parent;
						while (parent != null && nextNode == null)
						{
							int index = parent.Children.IndexOf(node);

							// If node has sibling than it is the next node
							if (index < parent.Children.Count - 1)
								nextNode = parent.Children[index + 1];

							// If node don't have sibling, checking parent
							node = parent;
							parent = node.Parent;
						}

						if (nextNode == null)
						{
							// Node don't have parent so it is root node
							int index = _nodes.IndexOf(node);
							// Checking for sibling
							if (index < _nodes.Count - 1)
								nextNode = _nodes[index + 1];
						}
					}

					if (nextNode == null)
						return false;

					lastNode = nextNode;

					// Breaking to next page if needed
					if (lastNode.BreakPage)
					{
						RemoveHeaders(builder, page);
						return true;
					}
				}
			}

			return true;
		}

		public void FinalizeSave(DocumentBuilder builder)
		{
		}

		#endregion
	}
}
