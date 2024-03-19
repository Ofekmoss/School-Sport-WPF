using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SportSite.NewRegister
{
	public partial class PagedTable : System.Web.UI.UserControl
	{
		public string NewItemTitle { get; set; }
		public string NewItemUrl { get; set; }
		public string SingularCaption { get; set; }
		public string PluralCaption { get; set; }
		public int CaptionGender { get; set; }
		public string[] AdditionalButtons { get; set; }

		PagedTableData tableData = null;
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("PagedTable"))
				this.Page.ClientScript.RegisterClientScriptInclude("PagedTable", "PagedTable.js");

			hidSingularCaption.Value = this.SingularCaption + "";
			hidPluralCaption.Value = this.PluralCaption + "";
			hidCaptionGender.Value = this.CaptionGender.ToString();
			if (!string.IsNullOrEmpty(this.NewItemTitle))
			{
				lbAddNew.InnerHtml = this.NewItemTitle;
				btnAddNew.Attributes["data-url"] = this.NewItemUrl + "";
				pnlAddNew.Visible = true;
			}
			if (this.AdditionalButtons != null && this.AdditionalButtons.Length > 0)
				hidAdditionalButtons.Value = string.Join(",", this.AdditionalButtons);
		}

		public void SetTableData(PagedTableData data)
		{
			tableData = data;
			int pageSize = 0;
			if (tableData == null)
			{
				rptTableCaptions.Visible = false;
				rptTableRows.Visible = false;
			}
			else
			{
				pageSize = tableData.PageSize;
				rptTableCaptions.DataSource = tableData.Columns;
				rptTableCaptions.DataBind();

				rptTableRows.DataSource = data.Rows;
				rptTableRows.DataBind();
			}
			hidPageSize.Value = pageSize.ToString();
		}

		public void TableCaptions_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
		{
			if (tableData == null || tableData.DefaultSort == null || tableData.DefaultSort.ColumnName == null)
				return;

			// Execute the following logic for Items and Alternating Items.
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				PagedTableData.TableColumn column = (PagedTableData.TableColumn)e.Item.DataItem;
				if (tableData.DefaultSort.ColumnName == column.Name)
				{
					string controlName = "";
					switch (tableData.DefaultSort.Direction)
					{
						case SortDirection.Ascending:
							controlName = "lbAscendingOrderIcon";
							break;
						case SortDirection.Descending:
							controlName = "lbDescendingOrderIcon";
							break;
					}
					if (controlName.Length > 0)
						((HtmlGenericControl)e.Item.FindControl(controlName)).Style["display"] = "";
				}
			}
		}

		public void TableRows_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
		{
			// Execute the following logic for Items and Alternating Items.
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				PagedTableData.TableRow row = (PagedTableData.TableRow)e.Item.DataItem;
				Repeater rowCells = (Repeater)e.Item.FindControl("rptRowCells");
				rowCells.DataSource = row.Cells;
				rowCells.DataBind();
			}
		}

		public void TableRowCells_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
		{
			
		}
	}

	public class PagedTableData
	{
		public class TableColumn
		{
			public string Caption { get; set; }
			public string Name { get; set; }
		}

		public class TableSort
		{
			public string ColumnName { get; set; }
			public SortDirection Direction { get; set; }
		}

		public class TableCell
		{
			public string Contents { get; set; }
			public object Value { get; set; }
		}

		public class TableRow
		{
			public string TargetUrl { get; set; }
			public List<TableCell> Cells { get; set; }
			public bool Visible { get; set; }
			public string Style
			{
				get
				{
					return this.Visible ? "" : "display: none;";
				}
			}
		}

		public List<TableColumn> Columns { get; set; }

		private TableSort defaultSort = null;
		public TableSort DefaultSort
		{
			get { return defaultSort; }
			set
			{
				defaultSort = value;
				ApplyDefaultSort();
			}
		}

		private List<TableRow> rows = null;
		public List<TableRow> Rows
		{
			get
			{
				return rows;
			}
			set
			{
				rows = value;
				ApplyDefaultSort();
				ApplyPageSize();
			}
		}
		private int pageSize = 0;
		public int PageSize
		{
			get { return pageSize; }
			set
			{
				pageSize = value;
				ApplyPageSize();
			}
		}

		protected void ApplyPageSize()
		{
			if (this.Rows != null)
			{
				this.Rows.ForEach(r => r.Visible = true);
				if (this.PageSize > 0 && this.PageSize < this.Rows.Count)
					this.Rows.Skip(this.PageSize).ToList().ForEach(r => r.Visible = false);
			}
		}

		protected void ApplyDefaultSort()
		{
			if (this.Rows != null && this.Rows.Count > 0 && this.Columns != null && this.DefaultSort != null && this.DefaultSort.ColumnName != null)
			{
				int columnIndex = this.Columns.FindIndex(c => c.Name == this.DefaultSort.ColumnName);
				if (columnIndex >= 0)
				{
					rows.Sort((r1, r2) =>
					{
						object v1 = r1.Cells[columnIndex].Value;
						object v2 = r2.Cells[columnIndex].Value;
						if (v1 == null && v2 == null)
							return 0;
						if (v1 == null)
							return this.DefaultSort.Direction == SortDirection.Ascending ? 1 : -1;
						if (v2 == null)
							return this.DefaultSort.Direction == SortDirection.Ascending ? -1 : 1;
						if (this.DefaultSort.Direction == SortDirection.Ascending)
							return (v1 as IComparable).CompareTo(v2 as IComparable);
						return (v2 as IComparable).CompareTo(v1 as IComparable);
					});
				}
			}
		}
	}
}