using System;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for CalendarView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class CalendarView : Sport.UI.View
	{
		private System.Drawing.Color[] colors = new System.Drawing.Color[]
			{
				System.Drawing.Color.FromArgb(224, 64, 64),
				System.Drawing.Color.FromArgb(0, 128, 224),
				System.Drawing.Color.FromArgb(255, 224, 64),
				System.Drawing.Color.FromArgb(192, 192, 96),
				System.Drawing.Color.FromArgb(64, 192, 224),
				System.Drawing.Color.FromArgb(224, 224, 128),
				System.Drawing.Color.FromArgb(128, 128, 64),
				System.Drawing.Color.FromArgb(224, 128, 64),
				System.Drawing.Color.FromArgb(64, 192, 96),
				System.Drawing.Color.FromArgb(64, 0, 64),
				System.Drawing.Color.FromArgb(255, 255, 255)
			};

		private int[,] colorCombination = new int[,]
			{
				{0, 0}, {1, 1}, {2, 2}, {3, 3}, {4, 4}, {5, 5}, {6, 6}, {7, 7}, {8, 8}, {9, 9}, {10, 10},
				{0, 1}, {0, 2}, {0, 3}, {0, 4}, {0, 5}, {0, 6}, {0, 7}, {0, 8}, {0, 9}, {0, 10},
				{1, 2}, {1, 3}, {1, 4}, {1, 5}, {1, 6}, {1, 7}, {1, 8}, {1, 9}, {1, 10},
				{2, 3}, {2, 4}, /*{2, 5}, */{2, 6}, {2, 7}, {2, 8}, {2, 9}, {2, 10}, 
				/*{3, 4}, {3, 5}, */{3, 6}, {3, 7}, /*{3, 8}, */{3, 9}, {3, 10}, 
				/*{4, 5}, */{4, 6}, {4, 7}, /*{4, 8}, */{4, 9}, {4, 10}, 
				{5, 6}, {5, 7}, {5, 8}, {5, 9}, {5, 10}, 
				/*{6, 7}, */{6, 8}, {6, 9}, {6, 10}, 
				/*{7, 8}, {7, 9}, {7, 10}, */
				{8, 9}, {8, 10}, 
				{9, 10}
			};

		private Sport.UI.Controls.LegendControl legend;
		private Sport.UI.Controls.RightToolBar toolBar;
		private System.Windows.Forms.ToolBarButton tbbCustom;
		private System.Windows.Forms.ToolBarButton tbbPrint;
		private System.Windows.Forms.ToolBarButton tbbCalendar;
		private System.Windows.Forms.ToolBarButton tbbClose;

		private System.Collections.Hashtable championshipBrushes;

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CalendarView));
			this.calendar = new Sport.UI.Controls.CalendarControl();
			this.legend = new Sport.UI.Controls.LegendControl();
			this.toolBar = new Sport.UI.Controls.RightToolBar();
			this.tbbClose = new System.Windows.Forms.ToolBarButton();
			this.tbbCalendar = new System.Windows.Forms.ToolBarButton();
			this.tbbCustom = new System.Windows.Forms.ToolBarButton();
			this.tbbPrint = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// calendar
			// 
			this.calendar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.calendar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.calendar.Calendar = ((System.Globalization.Calendar)(resources.GetObject("calendar.Calendar")));
			this.calendar.CurrentInfo = new System.Globalization.CultureInfo("he-IL");
			this.calendar.End = new System.DateTime(2006, 12, 31, 0, 0, 0, 0);
			this.calendar.Location = new System.Drawing.Point(0, 28);
			this.calendar.Name = "calendar";
			this.calendar.RowHeight = 20;
			this.calendar.Selected = new System.DateTime(((long)(0)));
			this.calendar.SelectedColor = System.Drawing.Color.Blue;
			this.calendar.Size = new System.Drawing.Size(714, 313);
			this.calendar.Start = new System.DateTime(2006, 1, 1, 0, 0, 0, 0);
			this.calendar.Style = Sport.UI.Controls.CalendarControl.CalendarStyle.Year;
			this.calendar.TabIndex = 0;
			this.calendar.TabStop = false;
			this.calendar.Text = "calendar";
			this.calendar.TodayColor = System.Drawing.Color.Red;
			this.calendar.DoubleClick += new System.EventHandler(this.calendar_DoubleClick);
			// 
			// legend
			// 
			this.legend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.legend.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.legend.Direction = Sport.UI.Controls.LegendControlDirection.Horizontal;
			this.legend.Location = new System.Drawing.Point(4, 345);
			this.legend.Name = "legend";
			this.legend.Size = new System.Drawing.Size(706, 66);
			this.legend.TabIndex = 3;
			// 
			// toolBar
			// 
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.tbbClose,
																					   this.tbbCalendar,
																					   this.tbbCustom,
																					   this.tbbPrint});
			this.toolBar.DropDownArrows = true;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(714, 28);
			this.toolBar.TabIndex = 5;
			this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// tbbClose
			// 
			this.tbbClose.Text = "סגור";
			// 
			// tbbCalendar
			// 
			this.tbbCalendar.Text = "לוח עברי";
			// 
			// tbbCustom
			// 
			this.tbbCustom.ImageIndex = 4;
			this.tbbCustom.Text = "התאמה";
			// 
			// tbbPrint
			// 
			this.tbbPrint.ImageIndex = 5;
			this.tbbPrint.Text = "הדפסה";
			// 
			// CalendarView
			// 
			this.Controls.Add(this.calendar);
			this.Controls.Add(this.toolBar);
			this.Controls.Add(this.legend);
			this.Name = "CalendarView";
			this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.Size = new System.Drawing.Size(714, 413);
			this.ResumeLayout(false);

		}
		private Sport.UI.Controls.CalendarControl calendar;
		private System.Globalization.HebrewCalendar hebrewCalendar;
		private System.Globalization.GregorianCalendar gregorianCalendar;

		public CalendarView()
		{
			championshipBrushes = new System.Collections.Hashtable();
			InitializeComponent();

			toolBar.ImageList = Sport.Resources.ImageLists.CreateColorImageList(null);

			tbbClose.ImageIndex = (int)Sport.Resources.ColorImages.DoubleLeft;
			tbbCalendar.ImageIndex = (int)Sport.Resources.ColorImages.CalendarType;
			tbbCustom.ImageIndex = (int)Sport.Resources.ColorImages.Custom;
			tbbCustom.Visible = false;
			tbbPrint.ImageIndex = (int)Sport.Resources.ColorImages.Print;

			Title = "לוח שנה";

			hebrewCalendar = new System.Globalization.HebrewCalendar();
			gregorianCalendar = new System.Globalization.GregorianCalendar();

			SetHebrewCalendar(Sport.Core.Configuration.ReadString("General", "Calendar") == "hebrew");

			Sport.Entities.Season season = new Sport.Entities.Season(Sport.Core.Session.Season);
			if (season.IsValid())
			{
				calendar.Start = season.Start;
				calendar.End = season.End;
			}

			DateTime now = DateTime.Now;
			if ((now >= season.Start) && (now <= season.End))
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

			base.Dispose(disposing);
		}

		public void SetHebrewCalendar(bool hebrew)
		{
			tbbCalendar.Text = hebrew ?
				"לוח לועזי" : "לוח עברי";
			if (hebrew)
				calendar.Calendar = hebrewCalendar;
			else
				calendar.Calendar = gregorianCalendar;

			Sport.Core.Configuration.WriteString("General", "Calendar", hebrew ? "hebrew" : "gregorian");
		}

		private System.Drawing.Brush GetChampionshipBrush(Sport.Entities.Championship championship)
		{
			System.Drawing.Brush brush = (System.Drawing.Brush)championshipBrushes[championship];

			if (brush == null)
			{
				int color1 = colorCombination[championshipBrushes.Count, 0];
				int color2 = colorCombination[championshipBrushes.Count, 1];
				if (color1 == color2)
					brush = new System.Drawing.SolidBrush(colors[color1]);
				else
					brush = new System.Drawing.Drawing2D.HatchBrush(
						System.Drawing.Drawing2D.HatchStyle.WideUpwardDiagonal,
						colors[color1], colors[color2]);
				championshipBrushes[championship] = brush;
				legend.Items.Add(championship, brush);
			}

			return brush;
		}

		private void ReadCalendar()
		{
			championshipBrushes.Clear();

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

				if (cd.Championships.Count > 0)
				{
					Sport.UI.Controls.Calendar.CalendarItem item = new Sport.UI.Controls.Calendar.CalendarItem();
					item.Brushes = new System.Drawing.Brush[cd.Championships.Count];
					for (int n = 0; n < cd.Championships.Count; n++)
					{
						item.Brushes[n] = GetChampionshipBrush(cd.Championships[n].Championship);
					}

					calendar.Items.Add(date, item);
				}

				date = date.AddDays(1);
			}
		}

		private void Calendar_CalendarChanged(object sender, EventArgs e)
		{
			ReadCalendar();
		}

		private void calendar_DoubleClick(object sender, System.EventArgs e)
		{
			//Sport.UI.ViewManager.OpenView(typeof(Views.DayEventsView), "date="+calendar.Selected.Ticks);

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
				{
					dayEventsView.SelectDate(calendar.Selected);
					Sport.UI.ViewManager.SelectView(i);
				}
			}
		}

		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbCalendar)
			{
				SetHebrewCalendar(calendar.Calendar != hebrewCalendar);
			}
			else if (e.Button == tbbClose)
			{
				Sport.UI.ViewManager.CloseView(this);
				Sport.UI.Display.StateItem si = Sport.UI.Display.StateManager.States["calendarbar"];
				si.Checked = true;
			}
			else if (e.Button == tbbCustom)
			{
			}
			else if (e.Button == tbbPrint)
			{
				PrintPreview();
			}
		}

		private Sport.Documents.Document CreateDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			System.Windows.Forms.Cursor cur = System.Windows.Forms.Cursor.Current;
			System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

			Sport.Documents.Document document = new Sport.Documents.Document("לוח שנה");
			document.SetSettings(settings);

			document.Direction = Sport.Documents.Direction.Right;
			document.Font = new System.Drawing.Font("Tahoma", 16, System.Drawing.GraphicsUnit.Pixel);

			Sport.Documents.Page page = new Sport.Documents.Page();

			Sport.Documents.ImageItem calendarItem = new Sport.Documents.ImageItem(
				new System.Drawing.Rectangle(document.DefaultMargins.Left,
				document.DefaultMargins.Top, document.DefaultMargins.Width, document.DefaultMargins.Height - 70),
				calendar.CreateMetafile(document.DefaultMargins.Width, document.DefaultMargins.Height - 70));
			calendarItem.Border = System.Drawing.SystemPens.WindowFrame;
			page.Items.Add(calendarItem);

			Sport.Documents.ImageItem legendItem = new Sport.Documents.ImageItem(
				new System.Drawing.Rectangle(document.DefaultMargins.Left, document.DefaultMargins.Bottom - 60, document.DefaultMargins.Width, 60),
				legend.CreateMetafile(document.DefaultMargins.Width, 60));
			page.Items.Add(legendItem);

			document.Pages.Add(page);

			return document;
		}

		private void PrintPreview()
		{
			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
				return;
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Documents.Document document = CreateDocument(ps);

				if (settingsForm.ShowPreview)
				{
					Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(document, ps);

					if (!printForm.Canceled)
						printForm.ShowDialog();

					printForm.Dispose();
				}
				else
				{
					System.Drawing.Printing.PrintDocument pd = document.CreatePrintDocument(ps);
					pd.PrintController = new Sport.UI.PrintControllerWithPageForm(pd.PrintController, 0);
					pd.Print();
				}
			}
		}
	}
}