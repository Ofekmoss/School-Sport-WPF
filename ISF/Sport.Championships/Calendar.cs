using System;

namespace Sport.Championships
{
	public class CalendarDate
	{
		#region ChampionshipCollection

		public class ChampionshipCollection : Sport.Common.GeneralCollection
		{
			public ChampionshipCollection(CalendarDate date)
				: base(date)
			{
			}

			public Sport.Entities.ChampionshipCategory this[int index]
			{
				get { return (Sport.Entities.ChampionshipCategory) GetItem(index); }
				set { SetItem(index, value); }
			}

			public void Insert(int index, Sport.Entities.ChampionshipCategory value)
			{
				InsertItem(index, value);
			}

			public void Remove(Sport.Entities.ChampionshipCategory value)
			{
				RemoveItem(value);
			}

			public bool Contains(Sport.Entities.ChampionshipCategory value)
			{
				return base.Contains(value);
			}

			public int IndexOf(Sport.Entities.ChampionshipCategory value)
			{
				return base.IndexOf(value);
			}

			public int Add(Sport.Entities.ChampionshipCategory value)
			{
				return AddItem(value);
			}
		}

		#endregion

		private ChampionshipCollection _championships;
		public ChampionshipCollection Championships
		{
			get { return _championships; }
		}

		public CalendarDate()
		{
			_championships = new ChampionshipCollection(this);
		}
	}

	public class Calendar
	{
		private static CalendarDate[] dates = null;
		private static DateTime firstDate;

		static Calendar()
		{
			Sport.Core.Session.SeasonParameter.ValueChanged += new EventHandler(SeasonParameter_ValueChanged);
			//ReadDates();
		}

		private static void InitializeDates()
		{
			Sport.Entities.Season season = new Sport.Entities.Season(Sport.Core.Session.Season);
			if (season.IsValid())
			{
				firstDate = season.Start;
				if (firstDate.Year > 1900)
				{
					TimeSpan span = season.End - season.Start;

					int seasonDates = span.Days + 1;

					dates = new CalendarDate[seasonDates];

					for (int d = 0; d < seasonDates; d++)
					{
						dates[d] = new CalendarDate();
					}
				}
			}
		}

		private static void ReadDates()
		{
			/*
			if (!Sport.Core.Session.Connected)
				return;
			
			if (dates == null)
				InitializeDates();
			
			if (firstDate.Year < 1900)
				return;
			
			SportServices.ChampionshipService cs = new SportServices.ChampionshipService();
			cs.CookieContainer = Sport.Core.Session.Cookies;
			SportServices.DateChampionship[] dcs = cs.GetDateChampionships(firstDate, firstDate.AddDays(dates.Length));
			
			foreach (SportServices.DateChampionship dc in dcs)
			{
				CalendarDate date = dates[((TimeSpan) (dc.Day - firstDate)).Days];
				date.Championships.Add(new Sport.Entities.ChampionshipCategory(dc.ChampionshipCategory));
			}
			*/
		}

		private static void SeasonParameter_ValueChanged(object sender, EventArgs e)
		{
			//InitializeDates();
			ReadDates();

			if (CalendarChanged != null)
				CalendarChanged(typeof(Calendar), EventArgs.Empty);
		}

		public static event EventHandler CalendarChanged;

		public static CalendarDate GetDate(DateTime date)
		{
			if (firstDate.Year < 1900)
				return null;
			
			int index = ((TimeSpan) (date - firstDate)).Days;

			if (index >= 0 && index < dates.Length)
				return dates[index];

			return null;
		}
	}
}
