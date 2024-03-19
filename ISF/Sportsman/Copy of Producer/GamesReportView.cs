using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Sport.UI.Display;

namespace Sportsman.Producer
{
	#region public enumerations and structs
	public enum EventsGridColumn
	{
		Undefined=-1,
		Date=0,
		Day,
		Hour,
		Championship,
		Category,
		Phase,
		Group,
		Description,
		Place,
		Supervisor,
		Phone,
		Result,
		Referee,
		ColumnsCount
	}
	
	public struct GridColumnData
	{
		public int Index;
		public string Caption;
		public int Width;
		public GridColumnData(int index, string caption, int width)
		{
			this.Index = index;
			this.Caption = caption;
			this.Width = width;
		}
	}
	#endregion
	
	/// <summary>
	/// Summary description for GamesReportView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class GamesReportView : Sport.UI.View, Sport.UI.ICommandTarget
	{
		#region private data
		private readonly string _baseTitle="דו\"ח אירועי ספורט";
		private readonly string _cfgSection="Events";
		private readonly string _cfgRegion="Region";
		private readonly string _cfgSport="Sport";
		private readonly string _cfgChampionship="Championship";
		private readonly string _cfgCategory="Category";
		private readonly string _cfgDateStart="DateStart";
		private readonly string _cfgDateEnd="DateEnd";
		private readonly string _cfgSortByDate="SortByDate";
		private readonly string _cfgColumnPrefix="Column";
		private readonly GridColumnData[] _gridColumns=new GridColumnData[] {
			new GridColumnData((int) EventsGridColumn.Date, "תאריך", 85),
			new GridColumnData((int) EventsGridColumn.Day, "יום", 50),
			new GridColumnData((int) EventsGridColumn.Hour, "שעה", 50),
			new GridColumnData((int) EventsGridColumn.Championship, "אליפות", 180),
			new GridColumnData((int) EventsGridColumn.Category, "קטגוריה", 180),
			new GridColumnData((int) EventsGridColumn.Phase, "שלב", 90),
			new GridColumnData((int) EventsGridColumn.Group, "בית", 75),
			new GridColumnData((int) EventsGridColumn.Description, "פירוט משחקים", 380),
			new GridColumnData((int) EventsGridColumn.Place, "מקום", 180),
			new GridColumnData((int) EventsGridColumn.Supervisor, "אחראים", 120),
			new GridColumnData((int) EventsGridColumn.Phone, "טלפון", 110),
			new GridColumnData((int) EventsGridColumn.Result, "תוצאה", 80),
			new GridColumnData((int) EventsGridColumn.Referee, "שופטים", 120)
		};
		
		private enum ChooseFilterItem
		{
			Undefined=-1,
			Region=0,
			Sport,
			Championship,
			Category,
			StartDate,
			EndDate,
			Facility,
			City,
			School,
			SortByDate,
			ItemsCount
		}
		#endregion
		
		#region private controls
		private	Sport.UI.Controls.RightToolBar	toolBar;
		private Sport.UI.Controls.Grid	gridEvents;
		private EventsGridSource	sourceEvents;
		private Sport.UI.Dialogs.GenericEditDialog dlgChooseFilters;
		private Sport.UI.Dialogs.GenericEditDialog dlgChooseColumns;
		private System.Windows.Forms.ToolBarButton tbbFilter;
		private System.Windows.Forms.ToolBarButton tbbCustom;
		private System.Windows.Forms.ToolBarButton tbbPrint;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
		private Sport.UI.EntitySelectionDialog facilityDialog;
		private Sport.UI.EntitySelectionDialog cityDialog;
		private Sport.UI.EntitySelectionDialog schoolDialog;
		#endregion

		#region constructors
		public GamesReportView()
		{
			InitializeComponent();
			
			//grid:
			InitializeEventsGrid();
			
			//choose filters dialog:
			InitChooseFiltersDialog();
			
			//choose columns dialog:
			InitChooseColumnsDialog();
		} //end default constructor
		#endregion
		
		#region Initialization
		#region Events Grid
		/// <summary>
		/// build events grid source and events grid columns.
		/// </summary>
		private void InitializeEventsGrid()
		{
			//events grid source:
			sourceEvents = new EventsGridSource();
			sourceEvents.SelectionChanged += new EventHandler(sourceEvents_SelectionChanged);
			
			//read config data:
			string[] arrConfigCols=new string[_gridColumns.Length];
			bool blnUseConfig=true;
			for (int i=0; i<arrConfigCols.Length; i++)
			{
				string strData=Sport.Core.Configuration.ReadString(_cfgSection, _cfgColumnPrefix+i);;
				if ((strData == null)||(strData.Length == 0))
				{
					blnUseConfig = false;
					break;
				}
				arrConfigCols[i] = strData;
			}
			
			//events grid columns:
			if (blnUseConfig)
			{
				char[] delimeters=new char[] {','};
				for (int i=0; i<arrConfigCols.Length; i++)
				{
					GridColumnData colData=_gridColumns[i];
					string strData=arrConfigCols[i];
					string[] arrParts=strData.Split(delimeters);
					int field=Sport.Common.Tools.CIntDef(arrParts[0], -1);
					if (field >= 0)
					{
						for (int j=0; j<_gridColumns.Length; j++)
						{
							if (_gridColumns[j].Index == field)
							{
								colData = _gridColumns[j];
								break;
							}
						}
						int width=-1;
						if (arrParts.Length > 1)
							width = Sport.Common.Tools.CIntDef(arrParts[1], -1);
						int oWidth=width;
						if (width < 0)
							width = colData.Width;
						gridEvents.Columns.Add(field, colData.Caption, width);
						gridEvents.Columns[gridEvents.Columns.Count-1].Visible = (oWidth > 0);
					}
					else
					{
						gridEvents.Columns.Add(colData.Index, colData.Caption, colData.Width);
					}
				}
			}
			else
			{
				foreach (GridColumnData colData in _gridColumns)
					gridEvents.Columns.Add(colData.Index, colData.Caption, colData.Width);
			}
			
			//apply source:
			gridEvents.Source = sourceEvents;
			
			//gridEvents.Columns[5].Field = 0;
		} //end function InitializeEventsGrid
		#endregion
		
		#region Choose Filters Dialog
		/// <summary>
		/// build the choose filters dialog by adding items and fill data.
		/// </summary>
		private void InitChooseFiltersDialog()
		{
			Views.FacilitiesTableView facilityView=new Views.FacilitiesTableView();
			facilityView.State[SelectionDialog] = "1";
			facilityDialog = new Sport.UI.EntitySelectionDialog(facilityView);
			
			Views.CitiesTableView cityView=new Views.CitiesTableView();
			cityView.State[SelectionDialog] = "1";
			cityDialog = new Sport.UI.EntitySelectionDialog(cityView);
			
			Views.SchoolsTableView schoolView=new Views.SchoolsTableView();
			schoolView.State[SelectionDialog] = "1";
			schoolDialog = new Sport.UI.EntitySelectionDialog(schoolView);
			
			//make the datetime picker appear in dd/MM/yyyy format:
			object[] dateValues=Sport.UI.Controls.GenericItem.DateTimeValues("dd/MM/yyyy");
			
			//initialize dialog:
			dlgChooseFilters = new Sport.UI.Dialogs.GenericEditDialog("בניית דו\"ח");
			
			//add items to let user choose region, sport, championship and time range:
			dlgChooseFilters.Items.Add("מחוז:", Sport.UI.Controls.GenericItemType.Selection);
			dlgChooseFilters.Items.Add("ענף ספורט:", Sport.UI.Controls.GenericItemType.Selection);
			dlgChooseFilters.Items.Add("אליפות:", Sport.UI.Controls.GenericItemType.Selection, new Size(180, 25));
			dlgChooseFilters.Items.Add("קטגוריה:", Sport.UI.Controls.GenericItemType.Selection, new Size(180, 25));
			dlgChooseFilters.Items.Add("תאריך התחלה:", Sport.UI.Controls.GenericItemType.DateTime, null, dateValues);
			dlgChooseFilters.Items.Add("תאריך סיום:", Sport.UI.Controls.GenericItemType.DateTime, null, dateValues);
			dlgChooseFilters.Items.Add("מתקן:", Sport.UI.Controls.GenericItemType.Button, null, 
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(facilityDialog.ValueSelector)));
			dlgChooseFilters.Items.Add("יישוב:", Sport.UI.Controls.GenericItemType.Button, null, 
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(cityDialog.ValueSelector)));
			dlgChooseFilters.Items.Add("בית ספר:", Sport.UI.Controls.GenericItemType.Button, null, 
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector)));
			dlgChooseFilters.Items.Add("מיון לפי תאריך?", Sport.UI.Controls.GenericItemType.Check);
			
			//fill regions:
			object[] regions=GetAllRegions();
			dlgChooseFilters.Items[(int) ChooseFilterItem.Region].Values = regions;
			dlgChooseFilters.Items[(int) ChooseFilterItem.Region].Value = regions[0];
			if (Core.UserManager.CurrentUser.UserRegion != Sport.Entities.Region.CentralRegion)
				dlgChooseFilters.Items[(int) ChooseFilterItem.Region].Value = (new Sport.Entities.Region(Core.UserManager.CurrentUser.UserRegion)).Entity;
			((Sport.UI.Controls.NullComboBox) dlgChooseFilters.Items[(int) ChooseFilterItem.Region].Control).Text = "כל המחוזות";
			dlgChooseFilters.Items[(int) ChooseFilterItem.Region].ValueChanged += new EventHandler(FilterSelectionChanged);
			
			//fill sports:
			object[] sports=GetAllSports();
			dlgChooseFilters.Items[(int) ChooseFilterItem.Sport].Values = sports;
			dlgChooseFilters.Items[(int) ChooseFilterItem.Sport].Value = sports[0];
			((Sport.UI.Controls.NullComboBox) dlgChooseFilters.Items[(int) ChooseFilterItem.Sport].Control).Text = "כל ענפי הספורט";
			dlgChooseFilters.Items[(int) ChooseFilterItem.Sport].ValueChanged += new EventHandler(FilterSelectionChanged);
			dlgChooseFilters.Items[(int) ChooseFilterItem.Championship].ValueChanged += new EventHandler(FilterSelectionChanged);
			
			//apply current date:
			dlgChooseFilters.Items[(int) ChooseFilterItem.StartDate].Value = DateTime.Now.Date;
			dlgChooseFilters.Items[(int) ChooseFilterItem.EndDate].Value = DateTime.Now.Date.AddMonths(1);
		}
		#endregion
		
		#region Choose Columns Dialog
		/// <summary>
		/// build the choose columns dialog by adding items.
		/// </summary>
		private void InitChooseColumnsDialog()
		{
			//initialize dialog:
			dlgChooseColumns = new Sport.UI.Dialogs.GenericEditDialog("בחירת עמודות");
			
			//add checkbox for each column:
			foreach (GridColumnData colData in _gridColumns)
			{
				dlgChooseColumns.Items.Add(colData.Caption, Sport.UI.Controls.GenericItemType.Check, true);
				for (int i=0; i<gridEvents.Columns.Count; i++)
				{
					if (gridEvents.Columns[i].Field == colData.Index)
					{
						dlgChooseColumns.Items[dlgChooseColumns.Items.Count-1].Value = gridEvents.Columns[i].Visible;
						break;
					}
				}
			}
		}
		#endregion
		#endregion
		
		#region Reload Report
		/// <summary>
		/// rebuild the report using the given filters.
		/// re-creates the grid rows.
		/// region, sport and championship are the ID of the desired entities.
		/// </summary>
		private void ReloadReport(int region, int sport, int championship, int category, 
			DateTime start, DateTime end, int facility, int city, int school, bool sortByDate)
		{
			//show something while user is waiting:
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...");
			
			//build page title to represent filters:
			string strTitle=_baseTitle;
			if (championship >= 0)
			{
				//add championship name to the title:
				strTitle += " - "+(new Sport.Entities.Championship(championship)).Name;
			}
			else
			{
				//add region and/or sport if available:
				if (region >= 0)
					strTitle += " - מחוז "+(new Sport.Entities.Region(region)).Name;
				if (sport >= 0)
					strTitle += " - "+(new Sport.Entities.Sport(sport)).Name;
			}
			//add the date range and apply:
			if (start.Year > 1900)
				strTitle += " מ-"+start.ToString("dd/MM/yyyy");
			if (end.Year < 2100)
				strTitle += " עד "+end.ToString("dd/MM/yyyy");
			this.Title = strTitle;
			
			//read all events for given region, sport and championship in given time range:
			start = Sport.Common.Tools.SetTime(start, 0, 0, 0);
			end = Sport.Common.Tools.SetTime(end, 23, 59, 59);
			Sport.Championships.Event[] events = new Sport.Championships.Event[0];
			try
			{
				events = Sport.Championships.Events.GetEventsInRange(start, end, region, sport, championship, category, facility, city, school);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("error loading events: "+ex.Message);
				System.Diagnostics.Debug.WriteLine("start: "+start.ToString());
				System.Diagnostics.Debug.WriteLine("end: "+end.ToString());
				System.Diagnostics.Debug.WriteLine("region: "+region.ToString());
				System.Diagnostics.Debug.WriteLine("sport: "+sport.ToString());
				System.Diagnostics.Debug.WriteLine("championship: "+championship.ToString());
				System.Diagnostics.Debug.WriteLine("category: "+category.ToString());
				System.Diagnostics.Debug.WriteLine("facility: "+facility.ToString());
				System.Diagnostics.Debug.WriteLine("city: "+city.ToString());
				System.Diagnostics.Debug.WriteLine("school: "+school.ToString());
			}
			
			//sort by date?
			if (sortByDate)
			{
				ArrayList arrTemp=new ArrayList(events);
				arrTemp.Sort(new Sport.Championships.Events.EventDateComparer(true));
				events = (Sport.Championships.Event[]) arrTemp.ToArray(typeof(Sport.Championships.Event));
			}
			
			//apply events for the grid:
			sourceEvents.Events = events;
			
			//store filters in the configuration file for future use...
			Sport.Core.Configuration.WriteString(_cfgSection, _cfgRegion, region.ToString());
			Sport.Core.Configuration.WriteString(_cfgSection, _cfgSport, sport.ToString());
			Sport.Core.Configuration.WriteString(_cfgSection, _cfgChampionship, championship.ToString());
			Sport.Core.Configuration.WriteString(_cfgSection, _cfgCategory, category.ToString());
			Sport.Core.Configuration.WriteString(_cfgSection, _cfgDateStart, (start.Year<1900)?"0":start.Ticks.ToString());
			Sport.Core.Configuration.WriteString(_cfgSection, _cfgDateEnd, (end.Year>2100)?"0":end.Ticks.ToString());
			Sport.Core.Configuration.WriteString(_cfgSection, _cfgSortByDate, (sortByDate)?"1":"0");
			
			//done.
			Sport.UI.Dialogs.WaitForm.HideWait();
		} //end function ReloadReport
		#endregion
		
		#region Open - inherited from View
		/// <summary>
		/// called when the View is opened.
		/// </summary>
		public override void Open()
		{
			//apply base title:
			this.Title = _baseTitle;
			
			//read configuration data:
			string strRegion=Sport.Core.Configuration.ReadString(_cfgSection, _cfgRegion);
			string strSport=Sport.Core.Configuration.ReadString(_cfgSection, _cfgSport);
			string strChamp=Sport.Core.Configuration.ReadString(_cfgSection, _cfgChampionship);
			string strCategory=Sport.Core.Configuration.ReadString(_cfgSection, _cfgCategory);
			string strDateStart=Sport.Core.Configuration.ReadString(_cfgSection, _cfgDateStart);
			string strDateEnd=Sport.Core.Configuration.ReadString(_cfgSection, _cfgDateEnd);
			string strSortByDate=Sport.Core.Configuration.ReadString(_cfgSection, _cfgSortByDate);
			bool blnSortByDate=(strSortByDate == "1");
			
			//check if user has already chosen filters in the past:
			if ((strRegion != null)&&(strRegion.Length > 0))
			{
				//data is present, parse and apply if possible. careful, user can manually change cfg file!
				int region=-1;
				int sport=-1;
				int championship=-1;
				int category=-1;
				DateTime start=DateTime.MinValue;
				DateTime end=DateTime.MaxValue;
				try
				{
					//parse region, sport and championship as numbers, and start and end as dates:
					region = Sport.Common.Tools.CIntDef(strRegion, -1);
					sport = Sport.Common.Tools.CIntDef(strSport, -1);
					championship = Sport.Common.Tools.CIntDef(strChamp, -1);
					category = Sport.Common.Tools.CIntDef(strCategory, -1);
					long lngStartTicks=Sport.Common.Tools.CLngDef(strDateStart, 0);
					long lngEndTicks=Sport.Common.Tools.CLngDef(strDateEnd, 0);
					start=(lngStartTicks>0)?new DateTime(lngStartTicks):DateTime.Now;
					end=(lngEndTicks>0)?new DateTime(lngEndTicks):DateTime.Now;
					
					//apply region if available:
					if (region >= 0)
					{
						dlgChooseFilters.Items[(int) ChooseFilterItem.Region].Value = (new Sport.Entities.Region(region)).Entity;
						FilterSelectionChanged(null, EventArgs.Empty);
						if (region != Sport.Entities.Region.CentralRegion)
						{
							facilityDialog.View.State[Sport.Entities.Region.TypeName] = region.ToString();
							cityDialog.View.State[Sport.Entities.Region.TypeName] = region.ToString();
							schoolDialog.View.State[Sport.Entities.Region.TypeName] = region.ToString();
						}
						else
						{
							facilityDialog.View.State[Sport.Entities.Region.TypeName] = null;
							cityDialog.View.State[Sport.Entities.Region.TypeName] = null;
							schoolDialog.View.State[Sport.Entities.Region.TypeName] = null;
						}
					}
					
					//apply sport if avilable:
					if (sport >= 0)
					{
						dlgChooseFilters.Items[(int) ChooseFilterItem.Sport].Value = (new Sport.Entities.Sport(sport)).Entity;
						FilterSelectionChanged(null, EventArgs.Empty);
					}
					
					//apply championship, if available:
					if (championship >= 0)
					{
						dlgChooseFilters.Items[(int) ChooseFilterItem.Championship].Value = (new Sport.Entities.Championship(championship)).Entity;
						FilterSelectionChanged(null, EventArgs.Empty);
					}
					
					//apply category, if available:
					if (category >= 0)
						dlgChooseFilters.Items[(int) ChooseFilterItem.Category].Value = (new Sport.Entities.ChampionshipCategory(category)).Entity;
					
					//apply date range if valid:
					if (start.Year >= 1990)
						dlgChooseFilters.Items[(int) ChooseFilterItem.StartDate].Value = start;
					if (end.Year >= 1990)
						dlgChooseFilters.Items[(int) ChooseFilterItem.EndDate].Value = end;
					
					if (strDateStart == "0")
						dlgChooseFilters.Items[(int) ChooseFilterItem.StartDate].Value = null;
					if (strDateEnd == "0")
						dlgChooseFilters.Items[(int) ChooseFilterItem.EndDate].Value = null;
					//(dlgChooseFilters.Items[(int) ChooseFilterItem.StartDate].Control as DateTimePicker).Checked = false;
					
					//apply sort by date:
					dlgChooseFilters.Items[(int) ChooseFilterItem.SortByDate].Value = blnSortByDate;
					
					//don't let user choose invalid range:
					/*
					if (start > end)
						dlgChooseFilters.Confirmable = false;
					*/
					
					if (strDateStart == "0")
						start = DateTime.MinValue;
					
					if (strDateEnd == "0")
						end = DateTime.MaxValue;
				}
				catch
				{
					//something is wrong with the config file values, delete them:
					System.Diagnostics.Debug.WriteLine("invalid ini file values found!");
					System.Diagnostics.Debug.WriteLine("region: "+strRegion);
					System.Diagnostics.Debug.WriteLine("sport: "+strSport);
					System.Diagnostics.Debug.WriteLine("championship: "+strChamp);
					System.Diagnostics.Debug.WriteLine("category: "+strCategory);
					System.Diagnostics.Debug.WriteLine("date start: "+strDateStart);
					System.Diagnostics.Debug.WriteLine("date end: "+strDateEnd);
					System.Diagnostics.Debug.WriteLine("sort by date? "+strSortByDate);
					Sport.Core.Configuration.WriteString(_cfgSection, _cfgRegion, "");
					Sport.Core.Configuration.WriteString(_cfgSection, _cfgSport, "");
					Sport.Core.Configuration.WriteString(_cfgSection, _cfgChampionship, "");
					Sport.Core.Configuration.WriteString(_cfgSection, _cfgCategory, "");
					Sport.Core.Configuration.WriteString(_cfgSection, _cfgDateStart, "");
					Sport.Core.Configuration.WriteString(_cfgSection, _cfgDateEnd, "");
				}
				
				//reload using the given filters:
				ReloadReport(region, sport, championship, category, start, end, -1, -1, -1, blnSortByDate);
			}
		} //end function Open()
		#endregion
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GamesReportView));
			this.gridEvents = new Sport.UI.Controls.Grid();
			this.toolBar = new Sport.UI.Controls.RightToolBar();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.tbbFilter = new System.Windows.Forms.ToolBarButton();
			this.tbbCustom = new System.Windows.Forms.ToolBarButton();
			this.tbbPrint = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// gridEvents
			// 
			this.gridEvents.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.gridEvents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridEvents.Editable = true;
			this.gridEvents.GroupingStyle = Sport.UI.Controls.GridGroupingStyle.Grid;
			this.gridEvents.HorizontalLines = true;
			this.gridEvents.Location = new System.Drawing.Point(0, 28);
			this.gridEvents.Name = "gridEvents";
			this.gridEvents.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.gridEvents.SelectedIndex = -1;
			this.gridEvents.SelectionMode = System.Windows.Forms.SelectionMode.One;
			this.gridEvents.ShowCheckBoxes = false;
			this.gridEvents.ShowRowNumber = true;
			this.gridEvents.Size = new System.Drawing.Size(980, 541);
			this.gridEvents.TabIndex = 2;
			this.gridEvents.VerticalLines = true;
			this.gridEvents.MouseUp += new MouseEventHandler(GridMouseUp);
			// 
			// toolBar
			// 
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					   this.tbbFilter,
																					   this.tbbCustom,
																					   this.tbbPrint});
			this.toolBar.DropDownArrows = true;
			this.toolBar.ImageList = this.imageList1;
			this.toolBar.Location = new System.Drawing.Point(0, 0);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(980, 28);
			this.toolBar.TabIndex = 0;
			this.toolBar.TextAlign = System.Windows.Forms.ToolBarTextAlign.Right;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.ToolBarButtonClicked);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.White;
			// 
			// tbbFilter
			// 
			this.tbbFilter.ImageIndex = 2;
			this.tbbFilter.Text = "חיתוך";
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
			// GamesReportView
			// 
			this.Controls.Add(this.gridEvents);
			this.Controls.Add(this.toolBar);
			this.Name = "GamesReportView";
			this.Size = new System.Drawing.Size(980, 569);
			this.Text = "GamesReportView";
			this.ResumeLayout(false);

		}
		#endregion
		
		#region get regions and sports
		private object[] GetAllRegions()
		{
			ArrayList result=new ArrayList();
			//result.Add(Sport.UI.Controls.NullComboBox.Null); //"כל המחוזות");
			result.AddRange(Sport.Entities.Region.Type.GetEntities(null));
			return (object[]) result.ToArray(typeof(object));
		}
		
		private object[] GetAllSports()
		{
			ArrayList result=new ArrayList();
			//result.Add(Sport.UI.Controls.NullComboBox.Null); //"כל ענפי הספורט");
			result.AddRange(Sport.Entities.Sport.Type.GetEntities(null));
			return (object[]) result.ToArray(typeof(object));
		}
		#endregion
		
		#region Close - inherited from View
		public override void Close()
		{
			//save the current grid state:
			for (int col=0; col<gridEvents.Columns.Count; col++)
			{
				//get current column:
				Sport.UI.Controls.Grid.GridColumn column=gridEvents.Columns[col];

				//put width of -1 if invisible...
				int width=-1;
				if (column.Visible)
					width = gridEvents.GetColumnWidth(0, col);
				string strData=column.Field+","+width.ToString();
				
				//write current data:
				string strKey=_cfgColumnPrefix+col.ToString();
				if (Sport.Core.Configuration.ReadString(_cfgSection, strKey) != strData)
				{
					Sport.Core.Configuration.WriteString(_cfgSection, strKey, strData);
				}
			}

			//done.
			base.Close ();
		}
		#endregion
		
		#region event handlers
		#region filter selection changed
		/// <summary>
		/// called when selection of region or sport in the choose filters dialog is changed.
		/// </summary>
		private void FilterSelectionChanged(object sender, EventArgs e)
		{
			//first, clear championship categories:
			dlgChooseFilters.Items[(int) ChooseFilterItem.Category].Values = null;
			
			int region=-1;
			int sport=-1;
			int champ=-1;
			
			//get selected region, sport and championship:
			object selRegion=dlgChooseFilters.Items[(int) ChooseFilterItem.Region].Value;
			if (selRegion is Sport.Data.Entity)
				region = ((Sport.Data.Entity) selRegion).Id;
			object selSport=dlgChooseFilters.Items[(int) ChooseFilterItem.Sport].Value;
			if (selSport is Sport.Data.Entity)
				sport = ((Sport.Data.Entity) selSport).Id;
			object selChamp=dlgChooseFilters.Items[(int) ChooseFilterItem.Championship].Value;
			if (selChamp is Sport.Data.Entity)
				champ = ((Sport.Data.Entity) selChamp).Id;
			
			//need to clear championships?
			if ((region < 0)||(sport < 0))
				dlgChooseFilters.Items[(int) ChooseFilterItem.Championship].Values = null;
			
			//build entity filter for championships:
			Sport.Data.EntityFilter filter=null;
			ArrayList filterFields=new ArrayList();
			if (region >= 0)
				filterFields.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.Championship.Fields.Region, region));
			if (sport >= 0)
				filterFields.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.Championship.Fields.Sport, sport));
			if (filterFields.Count > 0)
			{
				filter = new Sport.Data.EntityFilter();
				foreach (Sport.Data.EntityFilterField filterField in filterFields)
					filter.Add(filterField);
			}
			
			//apply championships:
			dlgChooseFilters.Items[(int) ChooseFilterItem.Championship].Values = 
				Sport.Entities.Championship.Type.GetEntities(filter);
			
			//got championship?
			if (champ >= 0)
			{
				filter = new Sport.Data.EntityFilter(
					(int) Sport.Entities.ChampionshipCategory.Fields.Championship, champ);
				dlgChooseFilters.Items[(int) ChooseFilterItem.Category].Values = 
					Sport.Entities.ChampionshipCategory.Type.GetEntities(filter);
			}
			
			//check dates:
			DateTime start=DateTime.MinValue;
			if (dlgChooseFilters.Items[(int) ChooseFilterItem.StartDate].Value != null)
				start = (DateTime) dlgChooseFilters.Items[(int) ChooseFilterItem.StartDate].Value;
			DateTime end=DateTime.MaxValue;
			if (dlgChooseFilters.Items[(int) ChooseFilterItem.EndDate].Value != null)
				end = (DateTime) dlgChooseFilters.Items[(int) ChooseFilterItem.EndDate].Value;
			/*
			if (start > end)
				dlgChooseFilters.Confirmable = false;
			*/
			
			//apply region:
			if ((region >= 0)&&(region != Sport.Entities.Region.CentralRegion))
			{
				facilityDialog.View.State[Sport.Entities.Region.TypeName] = region.ToString();
				cityDialog.View.State[Sport.Entities.Region.TypeName] = region.ToString();
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = region.ToString();
			}
			else
			{
				facilityDialog.View.State[Sport.Entities.Region.TypeName] = null;
				cityDialog.View.State[Sport.Entities.Region.TypeName] = null;
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = null;
			}
		} //end function FilterSelectionChanged
		#endregion
		
		#region tool bar button clicked
		/// <summary>
		/// called when tool bar button is being clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbFilter)
			{
				OpenFilterDialog();
			}
			else if (e.Button == tbbCustom)
			{
				OpenColumnsDialog();
			}
			else if (e.Button == tbbPrint)
			{
				PrintPreview();
			}
		}
		
		private void ToolBarButtonClick(ToolBarButton button)
		{
			ToolBarButtonClickEventArgs e=new ToolBarButtonClickEventArgs(button);
			ToolBarButtonClicked(this, e);
		}
		#endregion
		
		#region Printing
		#region Print Preview
		/// <summary>
		/// print events report, showing preview first.
		/// </summary>
		private void PrintPreview()
		{
			//create settings:
			System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
			
			//check if there are any printers installed:
			if (System.Drawing.Printing.PrinterSettings.InstalledPrinters.Count == 0)
			{
				Sport.UI.MessageBox.Show("לא נמצאה מדפסת ברירת מחדל - אנא בדוק הגדרות הדפסה", 
					"שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			//create settings form:
			Sport.UI.Dialogs.PrintSettingsForm settingsForm = new Sport.UI.Dialogs.PrintSettingsForm(ps);
			settingsForm.Landscape = true;
			
			//let user choose printer settings:
			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//build print document:
				Sport.Documents.Document document = CreatePrintDocument(ps);
				
				//preview?
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
		} //end function Print Preview
		#endregion
		
		#region build print document
		/// <summary>
		/// build the print document for events report using the grid.
		/// </summary>
		private Sport.Documents.Document CreatePrintDocument(System.Drawing.Printing.PrinterSettings settings)
		{
			//show the wait cursor while building:
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			Sport.Documents.TableItem ti = Tools.GridToTable(gridEvents, new Rectangle(0, 0, 500, 500));

			Documents.BaseDocumentBuilder baseDocumentBuilder = new Documents.BaseDocumentBuilder(settings);

			Sport.Documents.DocumentBuilder documentBuilder = baseDocumentBuilder.CreateDocumentBuilder(Title, ti, true);

			Sport.Documents.Document document = documentBuilder.CreateDocument();

			Cursor.Current = cur;
			return document;
		} //end function CreatePrintDocument
		#endregion
		#endregion
		
		#region grid mouse up
		private void GridMouseUp(object sender, MouseEventArgs e)
		{
			//get selected event:
			Sport.Championships.Event selEvent=sourceEvents.SelectedEvent;
			
			//got anything?
			if (selEvent == null)
				return;
			
			// Grid was right clicked, lets show menu
			if (e.Button == MouseButtons.Right)
			{
				ArrayList arrItems=new ArrayList();
				if (selEvent.SportField == null)
					arrItems.Add(new MenuItem("הכנס תוצאות", new System.EventHandler(SetResultsClicked)));
				arrItems.Add(new MenuItem("בניית אליפות", new System.EventHandler(OpenChampionshipClicked)));
				MenuItem[] menuItems = (MenuItem[]) arrItems.ToArray(typeof(MenuItem));
				Sport.UI.Controls.RightContextMenu cm = new Sport.UI.Controls.RightContextMenu(menuItems);
				cm.RightToLeft = RightToLeft.Yes;
				cm.Show((Control) sender, new System.Drawing.Point(e.X, e.Y));
			}
		}
		#endregion
		
		#region open championship clicked
		private void OpenChampionshipClicked(object sender, EventArgs e)
		{
			if (sourceEvents.SelectedEvent == null)
				return;
			Sport.Championships.Event selEvent=sourceEvents.SelectedEvent;
			int champCategoryID=selEvent.ChampCategory.Id;
			if (selEvent.SportField == null)
			{
				Sport.UI.ViewManager.OpenView(typeof(Producer.MatchChampionshipEditorView), 
					"championshipcategory=" + champCategoryID);
			}
			else
			{
				Sport.UI.ViewManager.OpenView(typeof(Producer.CompetitionChampionshipEditorView), 
					"championshipcategory=" + champCategoryID);
			}
		}
		#endregion
		
		#region set results clicked
		private void SetResultsClicked(object sender, EventArgs e)
		{
			//get selected event:
			Sport.Championships.Event selEvent=sourceEvents.SelectedEvent;
			
			//got anything?
			if (selEvent == null)
				return;
			
			//get match for selected event:
			string strError="";
			Sport.Championships.Match match=selEvent.GetMatch(ref strError);
			
			//got anything?
			if (match == null)
			{
				Sport.UI.MessageBox.Error("שגיאה: לא ניתן לקרוא את נתוני המשחק ("+strError+")", 
					"הכנסת תוצאות תחרות");
				return;
			}
			
			//show match dialog.
			MatchForm objDialog = new MatchForm(match);
			if (objDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				//Rebuild();
			}
		}
		#endregion
		#endregion

		#region open filter dialog
		/// <summary>
		/// show the choose filters dialog and let user choose desired filters.
		/// </summary>
		private void OpenFilterDialog()
		{
			//show the dialog:
			if (dlgChooseFilters.ShowDialog() == DialogResult.OK)
			{
				//user confirmed. read selections:
				object selRegion=dlgChooseFilters.Items[(int) ChooseFilterItem.Region].Value;
				object selSport=dlgChooseFilters.Items[(int) ChooseFilterItem.Sport].Value;
				object selChamp=dlgChooseFilters.Items[(int) ChooseFilterItem.Championship].Value;
				object selCategory=dlgChooseFilters.Items[(int) ChooseFilterItem.Category].Value;
				object selFacility=dlgChooseFilters.Items[(int) ChooseFilterItem.Facility].Value;
				object selCity=dlgChooseFilters.Items[(int) ChooseFilterItem.City].Value;
				object selSchool=dlgChooseFilters.Items[(int) ChooseFilterItem.School].Value;
				object selSortByDate=dlgChooseFilters.Items[(int) ChooseFilterItem.SortByDate].Value;
				bool blnSortByDate=false;
				if (selSortByDate != null)
					blnSortByDate = (bool) selSortByDate;
				
				//default value is -1 which means all data: (filter: ALL)
				int region=-1;
				int sport=-1;
				int championship=-1;
				int category=-1;
				int facility=-1;
				int city=-1;
				int school=-1;
				
				//check selected region:
				if (selRegion is Sport.Data.Entity)
					region = ((Sport.Data.Entity) selRegion).Id;
				
				//check selected sport:
				if (selSport is Sport.Data.Entity)
					sport = ((Sport.Data.Entity) selSport).Id;
				
				//check selected championship:
				if (selChamp != null)
					championship = ((Sport.Data.Entity) selChamp).Id;
				
				//check selected category:
				if (selCategory != null)
					category = ((Sport.Data.Entity) selCategory).Id;
				
				//get date range selection:
				object oStart=dlgChooseFilters.Items[(int) ChooseFilterItem.StartDate].Value;
				object oEnd=dlgChooseFilters.Items[(int) ChooseFilterItem.EndDate].Value;
				DateTime dtStart=(oStart == null)?(DateTime.MinValue):((DateTime) oStart);
				DateTime dtEnd=(oEnd == null)?(DateTime.MaxValue):((DateTime) oEnd);
				
				//check selected facility:
				if (selFacility is Sport.Data.Entity)
					facility = ((Sport.Data.Entity) selFacility).Id;
				
				//check selected city:
				if (selCity is Sport.Data.Entity)
					city = ((Sport.Data.Entity) selCity).Id;
				
				//check selected school:
				if (selSchool is Sport.Data.Entity)
					school = ((Sport.Data.Entity) selSchool).Id;
				
				//rebuild the report based on the selection:
				ReloadReport(region, sport, championship, category, dtStart, dtEnd, facility, city, school, blnSortByDate);
			}
		} //end function OpenFilterDialog
		#endregion
		
		#region open columns dialog
		/// <summary>
		/// show the choose filters dialog and let user choose desired filters.
		/// </summary>
		private void OpenColumnsDialog()
		{
			//show the dialog:
			if (dlgChooseColumns.ShowDialog() == DialogResult.OK)
			{
				//user confirmed. read selections annd apply column visibility:
				bool changed=false;
				//string strChecked="";
				for (int i=0; i<_gridColumns.Length; i++)
				{
					bool blnChecked=(bool) dlgChooseColumns.Items[i].Value;
					if (gridEvents.Columns[i].Visible != blnChecked)
					{
						changed = true;
					}
					gridEvents.Columns[i].Visible = blnChecked;
					//strChecked += (blnChecked)?"1":"0";
				}
				if (changed)
					gridEvents.Refresh();
			}
		} //end function OpenColumnsDialog
		#endregion
		
		#region general stuff or not in use
		private void sourceEvents_SelectionChanged(object sender, EventArgs e)
		{
			/*
			if (sourceEvents.SelectedEvent == null)
			{
				Sport.UI.MessageBox.Show("null");
			}
			else
			{
				Sport.UI.MessageBox.Show(sourceEvents.SelectedEvent.ChampCategory.Id.ToString());
			}
			*/
		}
		#endregion

		#region ICommandTarget Members

		public bool ExecuteCommand(string command)
		{
			if (command == Sport.Core.Data.KeyCommands.Print)
			{
				ToolBarButtonClick(tbbPrint);
			}
			else if (command == Sport.Core.Data.KeyCommands.CustomizeTable)
			{
				ToolBarButtonClick(tbbCustom);
			}
			else if (command == Sport.Core.Data.KeyCommands.FilterTable)
			{
				ToolBarButtonClick(tbbFilter);
			}
			else
			{
				return false;
			}
			return true;
		}

		#endregion
	}
}
