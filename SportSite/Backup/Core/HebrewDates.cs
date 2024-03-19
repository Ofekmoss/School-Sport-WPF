using System;
using System.Globalization;

namespace SportSite.Core
{
	public class HebrewDates
	{
		private static HebrewDates instance;
		public static HebrewDates Instance { get { return instance; } }

		private HebrewDates()
		{
		}

		static HebrewDates()
		{
			instance = new HebrewDates();
		}

		public DateTime GetGregorianJewishHoliday(string holidayName, int yearOffset)
		{
			HebrewCalendar hebCalendar = new HebrewCalendar();
			int curHebYear = hebCalendar.GetYear(DateTime.Now) + yearOffset;
			int hebHolidayStartDay = 0;
			int hebHolidatStartMonth = 0;
			switch (holidayName)
			{
				case "פסח":
					hebHolidayStartDay = 14;
					hebHolidatStartMonth =  hebCalendar.IsLeapYear(curHebYear) ? 8 : 7;
					break;
				case "סוכות":
					hebHolidayStartDay = 14;
					hebHolidatStartMonth = 1;
					break;
				case "חנוכה":
					hebHolidayStartDay = 25;
					hebHolidatStartMonth = 3;
					break;
			}

			if (hebHolidayStartDay > 0 && hebHolidatStartMonth > 0)
			{
				DateTime hebHolidayDate = new DateTime(curHebYear, hebHolidatStartMonth, hebHolidayStartDay, hebCalendar);
				GregorianCalendar greCalendar = new GregorianCalendar();
				return new DateTime(greCalendar.GetYear(hebHolidayDate), greCalendar.GetMonth(hebHolidayDate), greCalendar.GetDayOfMonth(hebHolidayDate));
			}

			return DateTime.MinValue;
		}

		public DateTime GetGregorianJewishHoliday(string holidayName)
		{
			return GetGregorianJewishHoliday(holidayName, 0);
		}
	}
}
