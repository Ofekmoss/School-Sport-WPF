using System;

namespace Sportsman.Forms
{
	/// <summary>
	/// Summary description for CalendarBar.
	/// </summary>
	public class CalendarBar : System.Windows.Forms.Panel
	{
		private System.Windows.Forms.Panel panelStyle;
		private Sport.UI.Controls.ThemeButton tbOpenCalendar;
		private Sport.UI.Controls.ThemeButton tbCalendarType;
		private Sport.UI.Controls.ThemeButton tbClose;
		private Sport.UI.Controls.CalendarControl calendar;

		public CalendarBar()
		{
			InitializeComponent();

			this.calendar.HeaderBrushes = new System.Drawing.Brush[]
				{
					new System.Drawing.SolidBrush(System.Drawing.Color.LightBlue),
					new System.Drawing.SolidBrush(System.Drawing.Color.SlateBlue)
				};

			hebrewCalendar = new System.Globalization.HebrewCalendar();
			gregorianCalendar = new System.Globalization.GregorianCalendar();

			SetHebrewCalendar(Sport.Core.Configuration.ReadString("General", "Calendar") == "hebrew");

			if (!Sport.Core.Session.Connected)
				return;
			
			Sport.Entities.Season season = new Sport.Entities.Season(Sport.Core.Session.Season);
			if (season.IsValid())
			{
				calendar.Start = season.Start;
				calendar.End = season.End;
			}
			
			DateTime now=DateTime.Now;
			if ((now >= season.Start)&&(now <= season.End))
			{
				calendar.Selected = now;
				calendar.ScrollTo(now);
			}
			
			Sport.Championships.Calendar.CalendarChanged += new EventHandler(Calendar_CalendarChanged);
			ReadCalendar();
		}

		protected override void Dispose(bool disposing)
		{
			Sport.Championships.Calendar.CalendarChanged -= new EventHandler(Calendar_CalendarChanged);

			base.Dispose (disposing);
		}


		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CalendarBar));
			this.calendar = new Sport.UI.Controls.CalendarControl();
			this.panelStyle = new System.Windows.Forms.Panel();
			this.tbClose = new Sport.UI.Controls.ThemeButton();
			this.tbOpenCalendar = new Sport.UI.Controls.ThemeButton();
			this.tbCalendarType = new Sport.UI.Controls.ThemeButton();
			this.panelStyle.SuspendLayout();
			this.SuspendLayout();
			// 
			// calendar
			// 
			this.calendar.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.calendar.Calendar = ((System.Globalization.Calendar)(resources.GetObject("calendar.Calendar")));
			this.calendar.CurrentInfo = new System.Globalization.CultureInfo("he-IL");
			this.calendar.Dock = System.Windows.Forms.DockStyle.Fill;
			//that is quite not smart to put hard coded year.
			this.calendar.End = new System.DateTime(2005, 12, 31, 0, 0, 0, 0);
			this.calendar.Location = new System.Drawing.Point(0, 0);
			this.calendar.Name = "calendar";
			this.calendar.RowHeight = 20;
			this.calendar.Selected = new System.DateTime(((long)(0)));
			this.calendar.SelectedColor = System.Drawing.Color.Blue;
			this.calendar.Size = new System.Drawing.Size(240, 356);
			this.calendar.Start = new System.DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, 0);
			this.calendar.Style = Sport.UI.Controls.CalendarControl.CalendarStyle.List;
			this.calendar.TabIndex = 0;
			this.calendar.TodayColor = System.Drawing.Color.Red;
			this.calendar.DoubleClick += new System.EventHandler(this.calendar_DoubleClick);
			// 
			// panelStyle
			// 
			this.panelStyle.Controls.Add(this.tbClose);
			this.panelStyle.Controls.Add(this.tbOpenCalendar);
			this.panelStyle.Controls.Add(this.tbCalendarType);
			this.panelStyle.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelStyle.Location = new System.Drawing.Point(0, 356);
			this.panelStyle.Name = "panelStyle";
			this.panelStyle.Size = new System.Drawing.Size(240, 24);
			this.panelStyle.TabIndex = 2;
			// 
			// tbClose
			// 
			this.tbClose.Alignment = System.Drawing.StringAlignment.Center;
			this.tbClose.AutoSize = false;
			this.tbClose.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbClose.Hue = 0F;
			this.tbClose.Image = ((System.Drawing.Image)(resources.GetObject("tbClose.Image")));
			this.tbClose.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tbClose.ImageList = null;
			this.tbClose.ImageSize = new System.Drawing.Size(12, 12);
			this.tbClose.Location = new System.Drawing.Point(3, 2);
			this.tbClose.Name = "tbClose";
			this.tbClose.Saturation = 0.8F;
			this.tbClose.Size = new System.Drawing.Size(21, 20);
			this.tbClose.TabIndex = 3;
			this.tbClose.Transparent = System.Drawing.Color.Black;
			this.tbClose.Click += new System.EventHandler(this.tbClose_Click);
			// 
			// tbOpenCalendar
			// 
			this.tbOpenCalendar.Alignment = System.Drawing.StringAlignment.Center;
			this.tbOpenCalendar.AutoSize = false;
			this.tbOpenCalendar.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbOpenCalendar.Hue = 220F;
			this.tbOpenCalendar.Image = ((System.Drawing.Image)(resources.GetObject("tbOpenCalendar.Image")));
			this.tbOpenCalendar.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.tbOpenCalendar.ImageList = null;
			this.tbOpenCalendar.ImageSize = new System.Drawing.Size(12, 12);
			this.tbOpenCalendar.Location = new System.Drawing.Point(29, 2);
			this.tbOpenCalendar.Name = "tbOpenCalendar";
			this.tbOpenCalendar.Saturation = 0.9F;
			this.tbOpenCalendar.Size = new System.Drawing.Size(21, 20);
			this.tbOpenCalendar.TabIndex = 1;
			this.tbOpenCalendar.Transparent = System.Drawing.Color.Black;
			this.tbOpenCalendar.Click += new System.EventHandler(this.tbOpenCalendar_Click);
			// 
			// tbCalendarType
			// 
			this.tbCalendarType.Alignment = System.Drawing.StringAlignment.Center;
			this.tbCalendarType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbCalendarType.AutoSize = false;
			this.tbCalendarType.Font = new System.Drawing.Font("Arial", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
			this.tbCalendarType.Hue = 120F;
			this.tbCalendarType.Image = null;
			this.tbCalendarType.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.tbCalendarType.ImageList = null;
			this.tbCalendarType.ImageSize = new System.Drawing.Size(0, 0);
			this.tbCalendarType.Location = new System.Drawing.Point(142, 2);
			this.tbCalendarType.Name = "tbCalendarType";
			this.tbCalendarType.Saturation = 0.5F;
			this.tbCalendarType.Size = new System.Drawing.Size(88, 20);
			this.tbCalendarType.TabIndex = 2;
			this.tbCalendarType.Text = "לוח שנה עברי";
			this.tbCalendarType.Transparent = System.Drawing.Color.Black;
			this.tbCalendarType.Click += new System.EventHandler(this.tbCalendarType_Click);
			// 
			// CalendarBar
			// 
			this.ClientSize = new System.Drawing.Size(240, 380);
			this.Controls.Add(this.calendar);
			this.Controls.Add(this.panelStyle);
			this.Name = "CalendarBar";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.panelStyle.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		private System.Globalization.HebrewCalendar hebrewCalendar;
		private System.Globalization.GregorianCalendar gregorianCalendar;
		public void SetHebrewCalendar(bool hebrew)
		{
			tbCalendarType.Text = hebrew ?
				"לוח שנה לועזי" : "לוח שנה עברי";
			if (hebrew)
				calendar.Calendar = hebrewCalendar;
			else
				calendar.Calendar = gregorianCalendar;

			Sport.Core.Configuration.WriteString("General", "Calendar", hebrew ? "hebrew" : "gregorian");
		}

		private void tbCalendarType_Click(object sender, System.EventArgs e)
		{
			SetHebrewCalendar(calendar.Calendar != hebrewCalendar);
		}

		private void ReadCalendar()
		{
			Sport.Entities.Season season = new Sport.Entities.Season(Sport.Core.Session.Season);
			if (season.IsValid())
			{
				calendar.Start = season.Start;
				calendar.End = season.End;
			}
			
			calendar.Items.Clear();

			DateTime date = season.Start;
			while (date <= season.End)
			{
				Sport.Championships.CalendarDate cd = Sport.Championships.Calendar.GetDate(date);
				if (cd == null)
					break;
				if (cd.Championships.Count > 0)
				{
					try
					{
						if (cd.Championships.Count > 1)
						{
							calendar.Items.Add(date, cd.Championships.Count.ToString() + " אליפויות");
						}
						else
						{
							calendar.Items.Add(date, cd.Championships[0].Championship.Name);
						}
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine("failed to add calendar item: "+e.Message);
						System.Diagnostics.Debug.WriteLine("count: "+cd.Championships.Count.ToString());
						System.Diagnostics.Debug.WriteLine(e.StackTrace);
					}
				}

				date = date.AddDays(1);
			}
		}

		private void Calendar_CalendarChanged(object sender, EventArgs e)
		{
			ReadCalendar();
		}

		private void tbOpenCalendar_Click(object sender, System.EventArgs e)
		{
			Sport.UI.Display.StateItem si = Sport.UI.Display.StateManager.States["calendarbar"];
			si.Checked = false;
			Sport.UI.ViewManager.OpenView(typeof(Views.CalendarView), null);
		}

		private void calendar_DoubleClick(object sender, System.EventArgs e)
		{
			int i = Sport.UI.ViewManager.FindView(typeof(Views.DayEventsView), null);

			if (i == -1)
			{
				Sport.UI.ViewManager.OpenView(typeof(Views.DayEventsView), null);
				i = Sport.UI.ViewManager.FindView(typeof(Views.DayEventsView), null);
			}

			if (i != -1)
			{
				Views.DayEventsView dayEventsView = Sport.UI.ViewManager.GetView(i) as Views.DayEventsView;

				if (dayEventsView != null)
					dayEventsView.SelectDate(calendar.Selected);
			}
		}

		private void tbClose_Click(object sender, System.EventArgs e)
		{
			Sport.UI.Display.StateItem si = Sport.UI.Display.StateManager.States["calendarbar"];
			si.Checked = false;
		}

	}
}
