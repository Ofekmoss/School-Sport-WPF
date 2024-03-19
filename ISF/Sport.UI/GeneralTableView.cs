using System;

namespace Sport.UI
{
	public class GeneralTableView : TableView2
	{
		public GeneralTableView(string viewName, EntityView entityView, int[] columns, int[] sort,
			int[] details, Type detailsView, Sport.Data.EntityFilter filter)
			: base(entityView)
		{
			Columns = columns;
			Sort = sort;
			Details = details;
			DetailsView = detailsView;
			EntityListView.EntityQuery.BaseFilter = filter;

            ViewName = viewName;
		}

		public GeneralTableView(string viewName, EntityView entityView, int[] columns, int[] sort,
			int[] details, Sport.Data.EntityFilter filter)
			: this(viewName, entityView, columns, sort, details, null, filter)
		{
		}

		public GeneralTableView(string viewName, EntityView entityView, int[] columns, int[] sort,
			Sport.Data.EntityFilter filter)
			: this(viewName, entityView, columns, sort, null, null, filter)
		{
		}

		public GeneralTableView(string viewName, EntityView entityView, int[] columns, int[] sort)
			: this(viewName, entityView, columns, sort, null, null, null)
		{
		}

		public override void Open()
		{
			FilterBarEnabled = Filters.Count > 0;
			SearchBarEnabled = Searchers.Count > 0;
			DetailsBarEnabled = Details.Length > 0;

			base.Open ();
		}

	}
}
