using System;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using Sport.Data;
using Sport.Common;
using Sport.UI;
using System.Drawing;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for SchoolsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class SchoolsTableView : Sport.UI.TableView
	{
		#region States

		public bool OnlyClubs
		{
			get
			{
				return State["IsClub"] == null ? false :
					State["IsClub"] == "1";
			}
			set
			{
				if (value)
					State["IsClub"] = "1";
				else
					State["IsClub"] = null;
			}
		}

		public Sport.Entities.Region SelectedRegion
		{
			get
			{
				return State[Sport.Entities.Region.TypeName] == null ? null :
					new Sport.Entities.Region((int)Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.Region.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.Region.TypeName] = value.Id.ToString();
				}
			}
		}

		public Sport.Entities.City SelectedCity
		{
			get
			{
				return State[Sport.Entities.City.TypeName] == null ? null :
					new Sport.Entities.City((int)Core.Tools.GetStateValue(State[Sport.Entities.City.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.City.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.City.TypeName] = value.Id.ToString();
				}
			}
		}

		public Sport.Entities.ChampionshipCategory SelectedCategory
		{
			get
			{
				return State[Sport.Entities.ChampionshipCategory.TypeName] == null ? null :
					new Sport.Entities.ChampionshipCategory((int)Core.Tools.GetStateValue(State[Sport.Entities.ChampionshipCategory.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.ChampionshipCategory.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.ChampionshipCategory.TypeName] = value.Id.ToString();
				}
			}
		}

		#endregion

		#region Filters

		private ComboBoxFilter regionFilter;
		private ComboBoxFilter cityFilter;
		private EntityFilter filter;

		private void ResetEntityFilter()
		{
			filter = new EntityFilter();

			Sport.Entities.ChampionshipCategory category = SelectedCategory;
			Sport.Entities.Region region = SelectedRegion;
			Sport.Entities.City city = SelectedCity;

			if (category != null)
			{
				filter.Add(new Sport.Types.CategoryFilterField((int)Sport.Entities.School.Fields.Category,
						category.Category, Sport.Types.CategoryCompareType.Partial));
				Sport.Entities.ChampionshipRegion[] championshipRegions = category.Championship.GetRegions();
				if (championshipRegions.Length > 0)
				{
					if (region != null)
					{
						filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.Region, region.Id));
						if (city != null)
							filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.City, city.Id));
					}
					else
					{
						int[] regions = new int[championshipRegions.Length];
						for (int n = 0; n < regions.Length; n++)
							regions[n] = championshipRegions[n].Region.Id;
						filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.Region, regions));
					}
				}
			}
			else
			{
				if (region != null)
				{
					filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.Region, region.Id));
					if (city != null)
						filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.City, city.Id));
				}
			}

			if (OnlyClubs)
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.ClubStatus, 1));
			}
		}

		private void RefreshCityFilter()
		{
			if (SelectedRegion == null)
			{
				cityFilter.SetValues(null);
			}
			else
			{
				cityFilter.SetValues(SelectedRegion.GetCities());
			}

			SelectedCity = (Sport.Entities.City)cityFilter.Value;
		}

		#endregion

		private System.Windows.Forms.ToolBarButton tbbTeamsReport;
		private Entity selectEntity;

		public SchoolsTableView()
		{
			Items.Add((int)Sport.Entities.School.Fields.Symbol, "סמל", 50);						//index: 0
			Items.Add((int)Sport.Entities.School.Fields.Name, "שם", 180);							//index: 1
			Items.Add((int)Sport.Entities.School.Fields.City, "ישוב", 100);						//index: 2
			Items.Add((int)Sport.Entities.School.Fields.Address, "כתובת", 150);					//index: 3
			Items.Add((int)Sport.Entities.School.Fields.MailAddress, "מען", 150);					//index: 4
			Items.Add((int)Sport.Entities.School.Fields.MailCity, "ישוב מען", 100);				//index: 5
			Items.Add((int)Sport.Entities.School.Fields.ZipCode, "מיקוד", 80);						//index: 6
			Items.Add((int)Sport.Entities.School.Fields.Email, "דוא\"ל", 100);						//index: 7
			Items.Add((int)Sport.Entities.School.Fields.Phone, "טלפון", 80);						//index: 8
			Items.Add((int)Sport.Entities.School.Fields.Fax, "פקס", 80);							//index: 9
			Items.Add((int)Sport.Entities.School.Fields.ManagerName, "מנהל", 120);					//index: 10
			Items.Add((int)Sport.Entities.School.Fields.FromGrade, "משכבה", 60);					//index: 11
			Items.Add((int)Sport.Entities.School.Fields.ToGrade, "עד שכבה", 60);					//index: 12
			Items.Add((int)Sport.Entities.School.Fields.Supervision, "סוג פיקוח", 100);			//index: 13
			Items.Add((int)Sport.Entities.School.Fields.Sector, "מגזר", 100);						//index: 14
			Items.Add((int)Sport.Entities.School.Fields.Region, "מחוז", 150);						//index: 15
			Items.Add((int)Sport.Entities.School.Fields.ClubStatus, "סטטוס מועדון", 120);			//index: 16
			Items.Add((int)Sport.Entities.School.Fields.PlayerNumberFrom, "ממספר חולצה", 50);		//index: 17
			Items.Add((int)Sport.Entities.School.Fields.PlayerNumberTo, "עד מספר חולצה", 50);		//index: 18
			Items.Add((int)Sport.Entities.School.Fields.Category, "קטגוריה", 120);					//index: 19
			Items.Add((int)Sport.Entities.School.Fields.LastModified, "תאריך שינוי אחרון", 120);	//index: 20
			Items.Add((int)Sport.Entities.School.Fields.ManagerCellPhone, "טלפון נייד מנהל", 120); //index: 21

			InitializeComponent();

			// search
			SearchBarEnabled = true;

			//
			// toolBar
			//
			tbbTeamsReport = new ToolBarButton();
			tbbTeamsReport.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbTeamsReport.Text = "דו\"ח רישום קבוצות";
			tbbTeamsReport.Enabled = false;

			toolBar.Buttons.Add(tbbTeamsReport);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			if (selectEntity != null && Height > 0 && Width > 0)
			{
				Current = selectEntity;
				selectEntity = null;
			}
		}

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbTeamsReport)
				PrintTeamsReport();
		}

		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.School.TypeName);

			EntityListView.Fields[(int)Sport.Entities.School.Fields.Region].Values =
				Sport.Entities.Region.Type.GetEntities(null);

			if (SelectedCategory != null) // choosing schools for championship category teams
			{
				Columns = new int[] { 0, 1, 2, 11, 12 };
			}
			else
			{
				if (State[SelectionDialog] == "1")
					Columns = new int[] { 0, 1, 2, 15 };
				else
					Columns = new int[] { 0, 1, 2, 3, 8, 9, 16 };
			}

			Details = new int[] { 5, 6, 7, 8, 13, 15, 17, 18 };


			Searchers.Add(new Searcher("סמל:", EntityListView.EntityType.Fields[(int)Sport.Entities.School.Fields.Symbol], 100));
			Searchers.Add(new Searcher("שם:", EntityListView.EntityType.Fields[(int)Sport.Entities.School.Fields.Name], 200));

			Sport.Entities.School school = null;
			if (State[Sport.Entities.School.TypeName] != null)
			{
				school = new Sport.Entities.School(Int32.Parse(State[Sport.Entities.School.TypeName]));
				if (school != null)
				{
					SelectedRegion = school.Region;
					SelectedCity = school.City;
				}

				State[Sport.Entities.School.TypeName] = null;
			}

			if (State[Sport.Entities.City.TypeName] != null)
				SelectedCity = new Sport.Entities.City(Tools.CIntDef(State[Sport.Entities.City.TypeName], -1));

			EntityType regionType = Sport.Entities.Region.Type;
			Entity[] regions = regionType.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, SelectedRegion == null ? null : SelectedRegion.Entity, "<כל המחוזות>");
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);
			cityFilter = new ComboBoxFilter("ישוב:", null, null, "<כל הישובים>");
			cityFilter.FilterChanged += new System.EventHandler(CityFiltered);

			Sport.Entities.ChampionshipCategory category = SelectedCategory;
			if (category == null)
			{
				if (State["cancel_region_filter"] == null)
					Filters.Add(regionFilter);
				if (State["cancel_city_filter"] == null)
					Filters.Add(cityFilter);
			}
			else
			{
				Filters.Add(regionFilter);
				if (category.Championship.GetRegions().Length > 1)
					Filters.Add(cityFilter);
			}

			RefreshCityFilter();

			Requery();

			base.Open();

			if (school != null)
				selectEntity = school.Entity;
		}

		private void CityFiltered(object sender, EventArgs e)
		{
			if (cityFilter.Value == null)
			{
				SelectedCity = null;
			}
			else
			{
				SelectedCity = (Sport.Entities.City)cityFilter.Value;
			}

			Requery();
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			if (regionFilter.Value == null)
			{
				SelectedRegion = null;
			}
			else
			{
				SelectedRegion = new Sport.Entities.Region((Entity)regionFilter.Value);
			}
			RefreshCityFilter();
			Requery();
		}

		private void Requery()
		{
			//save currrect cursor:
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			ResetEntityFilter();

			//read from database or memory:
			if (filter != null)
				EntityListView.Read(filter);
			else
				EntityListView.Clear();

			//build title:
			string title;
			if (SelectedCategory != null)
			{
				title = "בחירת ";
				title += OnlyClubs ? "מועדונים" : "בתי ספר";
			}
			else if (SelectedRegion != null)
			{
				title = "";
				if (State[SelectionDialog] == "1")
					title = "בחירת ";
				title += "בתי ספר - " + SelectedRegion.Name;
				if (SelectedCity != null)
					title += " - " + SelectedCity.Name;
			}
			else
			{
				title = "בתי ספר";
			}

			//change view title:
			Title = title;
			Cursor.Current = c;
		}

		private int lastRegion = -1;
		private void SetCities(Entity entity)
		{
			if (entity != null)
			{
				Sport.Entities.School school = new Sport.Entities.School(entity);
				int region = school.Region == null ? -1 : school.Region.Id;
				if (region != lastRegion)
				{
					Sport.Data.Entity[] cities = null;
					cities = Sport.Entities.City.Type.GetEntities(
						new EntityFilter((int)Sport.Entities.City.Fields.Region, region));
					EntityListView.Fields[(int)Sport.Entities.School.Fields.City].Values = cities;
					EntityListView.Fields[(int)Sport.Entities.School.Fields.MailCity].Values = cities;
					lastRegion = region;
				}
			}
		}

		protected override void OnSelectEntity(Entity entity)
		{
			tbbTeamsReport.Enabled = ((entity != null) && (entity.Id >= 0));

			SetCities(entity);
		}

		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			SetCities(entityEdit);

			//abort if new entity:
			if ((entityEdit == null) || (entityEdit.Entity == null))
				return;

			//check if club status changed from club to non-club:
			Sport.Entities.School newSchool = new Sport.Entities.School(entityEdit);
			Sport.Entities.School oldSchool = new Sport.Entities.School(entityEdit.Entity);

			if ((oldSchool.ClubStatus == 1) && (newSchool.ClubStatus == 0))
			{
				bool dr = Sport.UI.MessageBox.Ask("שינוי סטטוס המועדון יגרור לביטול החיוב עבור בית הספר.\nהאם לשנות סטטוס מועדון?", "אזהרה", MessageBoxIcon.Warning, false);
				if (dr != true)
				{
					newSchool.ClubStatus = 1;
				}
			}

			Sport.Entities.Product.Type.Reset(null);
		}

		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			//initialize field:
			EntityField entField;

			//got anything?
			if ((EntityListView == null) || (EntityListView.EntityType == null))
				return;

			//change club status to Not Club:
			entField = EntityListView.EntityType.Fields[(int)Sport.Entities.School.Fields.ClubStatus];

			//got anything?
			if (entField != null)
				entField.SetValue(entityEdit, (new Sport.Types.BooleanTypeLookup()).Items[0]);

			//change supervision status to Other:
			entField = EntityListView.EntityType.Fields[(int)Sport.Entities.School.Fields.Supervision];

			//got anything?
			if (entField != null)
				entField.SetValue(entityEdit, (int)Sport.Types.SchoolSupervisionType.Other);

			//change region if any region selected in the filter:
			if ((regionFilter != null) && (regionFilter.Value != null))
			{
				//get field:
				entField = EntityListView.EntityType.Fields[(int)Sport.Entities.School.Fields.Region];

				//got anything here?
				if (entField != null)
					entField.SetValue(entityEdit, (regionFilter.Value as Entity).Id);
			}

			//change city if any city selected in the filter:
			if ((cityFilter != null) && (cityFilter.Value != null))
			{
				//get field:
				entField = EntityListView.EntityType.Fields[(int)Sport.Entities.School.Fields.City];

				//got anything here?
				if (entField != null)
					entField.SetValue(entityEdit, (cityFilter.Value as Sport.Entities.City).Id);
			}
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.School school = new Sport.Entities.School(entity);
			string strMessage = school.CanDelete();
			if (strMessage.Length > 0)
			{
				Sport.UI.MessageBox.Show(strMessage, "מחיקת בית ספר", MessageBoxIcon.Warning);
				return false;
			}
			return Sport.UI.MessageBox.Ask("האם למחוק את בית הספר '" + school.Name +
				"'?", "מחיקת בית ספר", false);
		}

		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			//cancel if this is dialog:
			if (State[SelectionDialog] == "1")
				return null;

			MenuItem[] menuItems = null;
			switch (selectionType)
			{
				case (SelectionType.Single):
					ArrayList arrItems = new ArrayList();
					arrItems.Add(new MenuItem("פתח", new System.EventHandler(SchoolOpenClicked)));
					arrItems.Add(new MenuItem("-"));
					arrItems.Add(new MenuItem("תלמידים", new System.EventHandler(SchoolStudentsClicked)));
					arrItems.Add(new MenuItem("קבוצות", new System.EventHandler(TeamStudentsClicked)));
					arrItems.Add(new MenuItem("חיובים", new System.EventHandler(SchoolChargesClicked)));
					arrItems.Add(new MenuItem("תשלומים", new System.EventHandler(SchoolPaymentsClicked)));
					arrItems.Add(new MenuItem("מתקנים", new System.EventHandler(SchoolFacilitiesClicked)));
					/*
					if (Sport.Entities.User.ViewPermission(
						typeof(StudentsTableView).Name) != Sport.Entities.PermissionServices.PermissionType.None)
					{
						arrItems.Add(new MenuItem("תלמידים", new System.EventHandler(SchoolStudentsClicked)));
					}
					if (Sport.Entities.User.ViewPermission(
						typeof(TeamsTableView).Name) != Sport.Entities.PermissionServices.PermissionType.None)
					{
						arrItems.Add(new MenuItem("קבוצות", new System.EventHandler(TeamStudentsClicked)));
					}
					if (Sport.Entities.User.ViewPermission(
						typeof(ChargesTableView).Name) != Sport.Entities.PermissionServices.PermissionType.None)
					{
						arrItems.Add(new MenuItem("חיובים", new System.EventHandler(SchoolChargesClicked)));
					}
					if (Sport.Entities.User.ViewPermission(
						typeof(PaymentsTableView).Name) != Sport.Entities.PermissionServices.PermissionType.None)
					{
						arrItems.Add(new MenuItem("תשלומים", new System.EventHandler(SchoolPaymentsClicked)));
					}
					if (Sport.Entities.User.ViewPermission(
						typeof(FacilitiesTableView).Name) != Sport.Entities.PermissionServices.PermissionType.None)
					{
						arrItems.Add(new MenuItem("מתקנים", new System.EventHandler(SchoolFacilitiesClicked)));
					}
					*/
					menuItems = new MenuItem[arrItems.Count];
					for (int i = 0; i < arrItems.Count; i++)
						menuItems[i] = (MenuItem)arrItems[i];
					menuItems[0].DefaultItem = true;
					break;
			}

			return menuItems;
		}

		private void SchoolOpenClicked(object sender, EventArgs e)
		{
			OpenDetails(Current);
		}

		private void SchoolStudentsClicked(object senver, EventArgs e)
		{
			Entity entity = Current;

			if (entity != null)
			{
				string state = "region=" + entity.Fields[(int)Sport.Entities.School.Fields.Region].ToString();
				state += "&school=" + entity.Fields[(int)Sport.Entities.School.Fields.Id].ToString();
				if (entity.Fields[(int)Sport.Entities.School.Fields.City] != null)
					state += "&city=" + entity.Fields[(int)Sport.Entities.School.Fields.City].ToString();
				ViewManager.OpenView(typeof(StudentsTableView), state);
			}
		}

		private void TeamStudentsClicked(object senver, EventArgs e)
		{
			Entity entity = Current;

			if (entity != null)
			{
				object oRegion = entity.Fields[(int)Sport.Entities.School.Fields.Region];
				object oCity = entity.Fields[(int)Sport.Entities.School.Fields.City];
				string state = "school=" + entity.Fields[(int)Sport.Entities.School.Fields.Id].ToString();
				if (oRegion != null)
					state += "&region=" + oRegion.ToString();
				if (oCity != null)
					state += "&city=" + oCity.ToString();

				//ViewManager.OpenView(typeof(TeamsTableView), state);
				ViewManager.OpenView(typeof(SchoolTeamsTableView), state);
			}
		}

		private void SchoolChargesClicked(object sender, EventArgs e)
		{
			//get current entity:
			Entity entity = Current;

			//abort if not defined:
			if (entity == null)
				return;

			//jump to charges table view:
			Sport.UI.ViewManager.OpenView(
				typeof(Views.ChargesTableView), "school=" + entity.Id.ToString());
		}

		private void SchoolPaymentsClicked(object sender, EventArgs e)
		{
			//get current entity:
			Entity entity = Current;

			//abort if not defined:
			if (entity == null)
				return;

			Sport.Entities.School school = new Sport.Entities.School(entity);

			//jump to payments table view:
			Sport.UI.ViewManager.OpenView(typeof(Views.PaymentsTableView),
				"region=" + school.Region.Id + "&school=" + school.Id);
		}

		private void SchoolFacilitiesClicked(object sender, EventArgs e)
		{
			//get current entity:
			Entity entity = Current;

			//abort if not defined:
			if (entity == null)
				return;

			Sport.Entities.School school = new Sport.Entities.School(entity);

			//jump to facilities table view:
			Sport.UI.ViewManager.OpenView(typeof(Views.FacilitiesTableView),
				"region=" + school.Region.Id + "&school=" + school.Id);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}


		protected override void OpenDetails(Entity entity)
		{
			string strExecute = "Sportsman.Details.SchoolDetailsView,Sportsman" +
				((entity != null) ? "?id=" + entity.Id.ToString() : "");

			new OpenDialogCommand().Execute(strExecute);
		}

		private void PrintTeamsReport()
		{
			//got any school?
			if ((Current == null) || (Current.Id < 0))
				return;

			//get selected school:
			Sport.Entities.School school = new Sport.Entities.School(Current);

			//get school teams:
			Sport.Entities.Team[] teams = school.GetTeams();

			//got anything?
			if ((teams == null) || (teams.Length == 0))
			{
				Sport.UI.MessageBox.Error("אין קבוצות רשומות עבור בית ספר זה",
					"דו\"ח קבוצות");
				return;
			}

			//System.Windows.Forms.MessageBox.Show(string.Join("\n", teams.ToList().GetRange(25, 5).ConvertAll(t => 
			//	string.Format("{0} - {1} - {2}", t.Id, t.Championship.Id, t.Championship.Name))));

			//print:
			Documents.ChampionshipDocuments champDoc =
				new Documents.ChampionshipDocuments(
				Documents.ChampionshipDocumentType.TeamsReport, teams);
			champDoc.Print();
		}
	}
}
