using System;
using System.Collections;
using System.Globalization;

namespace Sportsman.Producer
{
	public class EventsGridSource : Sport.UI.Controls.IGridSource
	{

		private Sport.UI.Controls.Grid		_grid;
		private Sport.Championships.Event[] _events=null;
		private Sport.Championships.Event _selEvent=null;
		
		private Hashtable _hebWeekDays=new Hashtable();

		public void Rebuild()
		{
			//CultureInfo hebCulture=CultureInfo.CreateSpecificCulture("he-IL");
			//hebCulture.DateTimeFormat.Calendar = new HebrewCalendar();
			foreach (Sport.Championships.Event e in _events)
			{
				DateTime date=e.Date;
				if (_hebWeekDays[date] == null)
					_hebWeekDays[date] = Sport.Common.Tools.GetHebDayOfWeek(date); //_hebWeekDays[date] = date.ToString("dddd", hebCulture);
			}
			
			_grid.RefreshSource();
		}

		public Sport.Championships.Event[] Events
		{
			get { return _events; }
			set 
			{ 
				_events = value;
				Rebuild();
			}
		}

		public EventsGridSource()
		{
			//constructor logic here.
		}
		
		public void SetGrid(Sport.UI.Controls.Grid grid)
		{
			if (_grid != null)
			{
				_grid.SelectionChanged -= new EventHandler(GridSelectionChanged);
			}

			_grid = grid;

			if (_grid != null)
			{
				_grid.SelectionChanged += new EventHandler(GridSelectionChanged);
			}
		}
		
		public int GetRowCount()
		{
			if (_events == null)
				return 0;
			return _events.Length;
		}

		public int GetFieldCount(int row)
		{
			return (int) EventsGridColumn.ColumnsCount;
		}

		public int GetGroup(int row)
		{
			return 0;
		}
		
		public string GetText(int row, int field)
		{
			//System.Diagnostics.Debug.WriteLine("get text called. row: "+row+", col: "+field);
			
			//verify valid row index:
			if ((row < 0)||(row >= _events.Length))
				return null;
			
			//get proper event:
			Sport.Championships.Event e=_events[row];
			
			//get proper column if possible:
			EventsGridColumn column=EventsGridColumn.Undefined;
			try
			{
				column = (EventsGridColumn) field;
			}
			catch {column = EventsGridColumn.Undefined;}
			
			//decide what is the proper text:
			switch (column)
			{
				case EventsGridColumn.Date:
					if (e.Date.Year < 1900)
						return "---";
					return e.Date.ToString("dd/MM/yyyy");
				case EventsGridColumn.Day:
					if (e.Date.Year < 1900)
						return "---";
					return Sport.Common.Tools.CStrDef(_hebWeekDays[e.Date], "");
				case EventsGridColumn.Hour:
					if (e.Date.Year < 1900)
						return "---";
					return e.Date.ToString("HH:mm");
				case EventsGridColumn.Championship:
					return e.ChampCategory.Championship.Name;
				case EventsGridColumn.Category:
					return e.ChampCategory.Name;
				case EventsGridColumn.Phase:
					return e.PhaseName;
				case EventsGridColumn.Group:
					return e.GroupName;
				case EventsGridColumn.Description:
					return GetEventDescription(e);
				case EventsGridColumn.Place:
					return GetEventPlace(e);
				case EventsGridColumn.Supervisor:
					return e.Supervisor;
				case EventsGridColumn.Phone:
					return e.Phone;
				case EventsGridColumn.Result:
					return GetEventScore(e);
				case EventsGridColumn.Referee:
					return e.Referee;

			}
			
			//getting here means undefined column!
			return null;
		}

		public Sport.UI.Controls.Style GetStyle(int row, int field, Sport.UI.Controls.GridDrawState state)
		{
			return null;
		}

		public string GetTip(int row)
		{
			return null;
		}

		public int[] GetSort(int group)
		{
			return null;
		}

		public void Sort(int group, int[] columns)
		{
		}

		public System.Windows.Forms.Control Edit(int row, int field)
		{
			return null;
		}

		public void EditEnded(System.Windows.Forms.Control control)
		{
		}

		public void Dispose()
		{
			_events = null; //_rows = null;
		}
		
		public Sport.Championships.Event SelectedEvent
		{
			get { return _selEvent; }
		}
		
		public event System.EventHandler SelectionChanged;
		
		private void ResetSelection()
		{
			Sport.Championships.Event e = null;

			if (_grid.Selection.Size == 1)
			{
				int row = _grid.Selection.Rows[0];
				e = _events[row] as Sport.Championships.Event;
			}

			bool selectionChanged = false;

			if (e != _selEvent)
			{
				_selEvent = e;
				selectionChanged = true;
			}

			if (selectionChanged && SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}
		
		private void GridSelectionChanged(object sender, EventArgs e)
		{
			ResetSelection();
		}
		
		private string GetEventDescription(Sport.Championships.Event e)
		{
			return e.GetTeamsOrCompetition();
		}
		
		private string GetEventPlace(Sport.Championships.Event e)
		{
			return e.GetPlace();
		}
		
		private string GetEventScore(Sport.Championships.Event e)
		{
			return e.GetScore(true);
		}
	}
}
