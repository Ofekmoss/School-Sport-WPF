namespace SportSite.Controls
{
	using System;
	using System.Data;
	using System.Collections;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Web;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using SportSite.Common;
	using Sport.Common;
	using Sport.Data;

	/// <summary>
	///		Summary description for TableView.
	/// </summary>
	public class TableView : System.Web.UI.UserControl
	{
		#region FieldCollection
		/// <summary>
		/// FieldCollection represents collection of view fields and supports 
		/// adding, removing or searching for fields within it. Improved 
		/// version of ArrayList, can hold only ViewField.
		/// </summary>
		public class FieldCollection : GeneralCollection
		{
			/// <summary>
			/// common indexer: returns the Field in given index.
			/// </summary>
			public ViewField this[int index]
			{
				get { return (ViewField)GetItem(index); }
			}

			/// <summary>
			/// search for Field in the collection based on its caption.
			/// </summary>
			public ViewField this[string caption]
			{
				//read only property.
				get
				{
					//search the collection for the desired Field.
					for (int i = 0; i < this.Count; i++)
					{
						ViewField field = this[i];
						//caption is not case sensitive.
						if (field.fieldCaption == caption)
						{
							//found matching Field, return it:
							return field;
						}
					}
					return ViewField.Empty;
				}
			} //end indexer Field[string]

			/// <summary>
			/// insert given Field in given index.
			/// </summary>
			public void Insert(int index, ViewField value)
			{
				//simply call the method from our base class.
				InsertItem(index, value);
			}

			/// <summary>
			/// remove given Field from the collection.
			/// </summary>
			public void Remove(ViewField value)
			{
				//call the method from our base class.
				RemoveItem(value);
			}

			/// <summary>
			/// returns whether the collection contains the given Field or not
			/// </summary>
			public bool Contains(ViewField value)
			{
				//use the method of our base class.
				return base.Contains(value);
			}

			/// <summary>
			/// returns the index of the given Field in the collection, if exist or 
			/// -1 if does not exist.
			/// </summary>
			public int IndexOf(ViewField value)
			{
				//use method from our base class.
				return base.IndexOf(value);
			}

			/// <summary>
			/// add the given Field to the collection.
			/// </summary>
			public int Add(ViewField value)
			{
				return AddItem(value);
			}

			// Overriding += to use Add
			public static FieldCollection operator +(FieldCollection collection, ViewField field)
			{
				collection.Add(field);
				return collection;
			}

			// Overriding += to use Add
			public static FieldCollection operator +(FieldCollection collection, ViewField[] fields)
			{
				for (int i = 0; i < fields.Length; i++)
					collection.Add(fields[i]);
				return collection;
			}
		} //end class FieldCollection
		#endregion

		public string EntityTypeName = "";
		public string TableViewCaption = "";
		public string NoValuesText = "";
		public ArrayList EntitiesToIgnore = null;
		public List<List<string>> Values { get; set; }
		private Controls.MainView IsfMainView = null;

		/// <summary>
		/// collection of ViewField objects to be displayed in the grid.
		/// </summary>
		public FieldCollection ViewFields = null;
		public int GridViewHeight = 100;
		public EntityFilter Filter = null;
		/// <summary>
		/// command to execute (client side) once row is clicked.
		/// leave blank to have the row as usual.
		/// inside the string, put %id to represent id of the row entity.
		/// </summary>
		public string RowClickCommand = null;
		public string RowTooltip = null;

		private readonly string[] RowColors = { "#FFFFFF", "#CDCDCD" };  //{"#0000A5", "#6D70FF", "#FF00FF"};
		private Table ViewGrid = null;
		private int _rowsCount = 0;

		public int RowsCount
		{
			get { return _rowsCount; }
		}

		/// <summary>
		/// default constructor.
		/// </summary>
		public TableView()
		{
			ViewFields = new FieldCollection();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			IsfMainView = (SportSite.Controls.MainView) Common.Tools.FindControlByType(this, "MainView");
			if (EntityTypeName.Length == 0 || ViewFields.Count == 0)
				return;

			Panel objPanel = new Panel();
			objPanel.HorizontalAlign = HorizontalAlign.Center;

			//display external table and captions
			ViewGrid = new Table();
			ViewGrid.Attributes.Add("maxrows", "20");
			ViewGrid.Style["text-align"] = "center";
			ViewGrid.CssClass = SportSite.Common.Style.TableViewTableCss;

			//add caption row
			TableRow objRow = new TableRow();
			objRow.BorderStyle = BorderStyle.None;
			TableCell objCell = new TableCell();
			objCell.HorizontalAlign = HorizontalAlign.Center;
			objCell.ColumnSpan = this.ViewFields.Count;
			objCell.CssClass = SportSite.Common.Style.TableViewCaptionCss;
			objCell.Text = this.TableViewCaption;
			objRow.Cells.Add(objCell);
			ViewGrid.Rows.Add(objRow);

			//add captions:
			objRow = new TableRow();

			for (int i = 0; i < ViewFields.Count; i++)
			{
				ViewField field = ViewFields[i];
				TableHeaderCell objHeader = new TableHeaderCell();
				objHeader.CssClass = SportSite.Common.Style.TableViewHeaderCss;
				objHeader.Style.Add("cursor", "pointer");
				objHeader.ToolTip = "מיין לפי עמודה זו";
				objHeader.Attributes.Add("onclick", "SortTable(this);");
				objHeader.Text = field.fieldCaption;
				objRow.Cells.Add(objHeader);
			}
			ViewGrid.Rows.Add(objRow);

			objPanel.Controls.Add(ViewGrid);
			this.Controls.Add(objPanel);

			//read from database or given values:
			Read();
		}

		public delegate void EntitiesReadHandler(ArrayList entities);
		public event EntitiesReadHandler EntitiesRead = null;

		/// <summary>
		/// read the data from database and fill the view.
		/// </summary>
		private void Read()
		{
			bool blnReadFromDatabase = this.EntityTypeName.Length > 0 && this.Values == null;

			if (blnReadFromDatabase)
			{
				//get entity type:
				EntityType entType = EntityType.GetEntityType(this.EntityTypeName);

				//got anything?
				if (entType == null)
					throw new Exception("Table View: invalid entity type: " + this.EntityTypeName);

				//create array list:
				ArrayList arrEntities = new ArrayList(entType.GetEntities(this.Filter));

				//sort if possible:
				if (EntitiesRead != null)
					EntitiesRead(arrEntities);

				if (this.EntitiesToIgnore != null)
				{
					ArrayList arrTemp = new ArrayList();
					foreach (Entity entity in arrEntities)
					{
						if (this.EntitiesToIgnore.IndexOf(entity.Id) == -1)
							arrTemp.Add(entity);
					}
					arrEntities.Clear();
					foreach (Entity entity in arrTemp)
						arrEntities.Add(entity);
				}
				Entity[] entities = (Entity[])arrEntities.ToArray(typeof(Entity));
				this.Values = new List<List<string>>();
				for (int row = 0; row < entities.Length; row++)
				{
					Entity ent = entities[row];
					List<string> values = new List<string>();
					values.Add(ent.Id.ToString());
					for (int col = 0; col < this.ViewFields.Count; col++)
					{
						string curText = "";
						try
						{
							curText = GetFieldValue(ent, col);
						}
						catch (Exception ex)
						{
							curText = "[שגיאה] <!-- error: " + ex.Message + " -->";
						}
						values.Add(curText);
					}
					this.Values.Add(values);
				}
			}

			if (this.Values == null || this.Values.Count == 0)
			{
				if (this.NoValuesText.Length > 0)
				{
					TableRow objRow = new TableRow();
					TableCell objCell = new TableCell();
					objCell.ColumnSpan = ViewFields.Count;
					objCell.HorizontalAlign = HorizontalAlign.Center;
					objCell.Text = this.NoValuesText;
					objRow.Cells.Add(objCell);
					ViewGrid.Rows.Add(objRow);
				}
				return;
			}

			int rowCounter = 0;
			this.Values.ForEach(cellValues =>
			{
				string strClickCommand = (this.RowClickCommand + "").Replace("%id", cellValues[0]);
				if (strClickCommand.Length > 0 && !strClickCommand.EndsWith(";"))
					strClickCommand += ";";

				//add row for current values
				TableRow objRow = new TableRow();
				objRow.CssClass = "TableViewRow" + ((rowCounter % this.RowColors.Length) + 1).ToString();

				//make it brighter:
				IsfMainView.clientSide.MakeBrighter(objRow, RowColors[rowCounter % RowColors.Length], -50);

				//maybe row is clickable:
				if (strClickCommand.Length > 0)
				{
					objRow.Style.Add("cursor", "pointer");
					objRow.Attributes.Add("onclick", strClickCommand);
				}

				//add tooltip if defined:
				if (RowTooltip != null)
					objRow.ToolTip = RowTooltip;

				//add cells:
				for (int i = 1; i < cellValues.Count; i++)
				{
					string cellText = cellValues[i];
					
					//initialize table cell:
					TableCell objCell = new TableCell();
					objCell.HorizontalAlign = HorizontalAlign.Center;
					objCell.Attributes.Add("dir", "rtl");

					//add onclick if needed:
					if (strClickCommand.Length > 0)
					{
						cellText = "<a href=\"about:blank\" onmouseover=\"return true;\" " +
							"onclick=\"" + strClickCommand + " return false;\" class=\"" + objCell.CssClass + "\" " +
							"style=\"text-decoration: none;\">" + cellText + "</a>";
					}
					objCell.Text = cellText;
					objRow.Cells.Add(objCell);
				}
				
				//add the row to the table:
				if (objRow != null)
				{
					ViewGrid.Rows.Add(objRow);
					_rowsCount++;
				}

				rowCounter++;
			});
		} //end function Read.

		/// <summary>
		/// get field value (string, number) and according to the field type 
		/// decide how to display this value. for example, number will ussually
		/// be translated using entity look up type of some sort.
		/// </summary>
		private string GetFieldValue(Entity ent, int index)
		{
			//get field type:
			ViewFieldType type = ViewFields[index].fieldType;
			int fieldIndex = ViewFields[index].entityFieldIndex;

			//decide how to display:
			switch (type)
			{
				case ViewFieldType.Default:
				case ViewFieldType.CategoryType:
				case ViewFieldType.TeamStatusType:
				case ViewFieldType.TeamName:
				case ViewFieldType.Email:
					string strText = Common.Tools.CStrDef(
						ent.EntityType.Fields[fieldIndex].GetText(ent), "");
					if ((type == ViewFieldType.Email) && (strText.Length > 0))
						strText = "<a href=\"mailto:" + strText + "\">" + strText + "</a>";
					return strText;
				case ViewFieldType.PlayerStatusType:
					return (new Sport.Entities.Player(ent)).StatusText;
			}

			//illegal field type...
			throw new Exception("Table View: illegal field type for field with index " + index.ToString() + ": " + ViewFields[index].fieldType.ToString());
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}

	public enum ViewFieldType
	{
		Default = 0,
		CategoryType,
		TeamStatusType,
		PlayerStatusType,
		TeamName,
		Email
	}

	/// <summary>
	/// ViewField represents one column in the grid, i.e. one data field.
	/// it uses the index of the field in its Entity and enable to define
	/// custom caption and the field type to be used when converting to string.
	/// </summary>
	public struct ViewField
	{
		public int entityFieldIndex;
		public string fieldCaption;
		public ViewFieldType fieldType;

		public static ViewField Empty;

		static ViewField()
		{
			Empty = new ViewField(-1, "", ViewFieldType.Default);
		}

		public ViewField(int index, string caption, ViewFieldType type)
		{
			this.entityFieldIndex = index;
			this.fieldCaption = caption;
			this.fieldType = type;
		}

		public ViewField(int index, string caption)
			: this(index, caption, ViewFieldType.Default)
		{
		}
	}
}
