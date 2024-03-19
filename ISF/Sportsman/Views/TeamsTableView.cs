using System;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using Sport.UI;
using Sport.UI.Controls;
using Sport.Data;
using Sportsman.Core;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for ChampionshipsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class TeamsTableView : TableView
	{
		private readonly string IMPORT_FILE_NAME = "TeamExportFull.txt";
		private ComboBoxFilter schoolFilter;
		private ComboBoxFilter champFilter;
		private ComboBoxFilter cityFilter;
		private ComboBoxFilter regionFilter;
		private ComboBoxFilter sportFilter;
		private ComboBoxFilter categoryFilter;
		private EntityFilter _filter;
		private EntitySelectionDialog schoolDialog;
		private EntitySelectionDialog userDialog;
		private System.Windows.Forms.ToolBarButton tbbImportTeams;

		private string ImportFilePath
		{
			get
			{
				return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), IMPORT_FILE_NAME);
			}
		}

		private enum ColumnTitles
		{
			School = 0,
			SchoolSymbol,
			Index,
			Championship,
			Category,
			Status,
			Supervisor,
			RegisterDate,
			Sport,
			PlayerNumberFrom,
			PlayerNumberTo,
			LastModified,
			Charge,
			PlayersCount,
			City
		}

		#region constructor
		public TeamsTableView()
		{
			EntityType champType = Sport.Entities.Championship.Type;

			Items.Add((int)Sport.Entities.Team.Fields.School, "בית ספר", 220);
			Items.Add((int)Sport.Entities.Team.Fields.SchoolSymbol, "סמל בית ספר", 120);
			Items.Add((int)Sport.Entities.Team.Fields.Index, "מספר סידורי", 80);
			Items.Add((int)Sport.Entities.Team.Fields.Championship, "אליפות", 200);
			Items.Add((int)Sport.Entities.Team.Fields.Category, "קטגוריה", 150);
			Items.Add((int)Sport.Entities.Team.Fields.Status, "סטטוס", 120);
			Items.Add((int)Sport.Entities.Team.Fields.Supervisor, "אחראי", 150);
			Items.Add((int)Sport.Entities.Team.Fields.RegisterDate, "תאריך רישום", 120);
			Items.Add((int)Sport.Entities.Team.Fields.Sport, "ענף ספורט", 150);
			Items.Add((int)Sport.Entities.Team.Fields.PlayerNumberFrom, "ממספר חולצה", 50);
			Items.Add((int)Sport.Entities.Team.Fields.PlayerNumberTo, "עד מספר חולצה", 50);
			Items.Add((int)Sport.Entities.Team.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int)Sport.Entities.Team.Fields.Charge, "חיוב", 150);
			Items.Add((int)Sport.Entities.Team.Fields.TeamPlayerCount, "שחקנים רשומים", 60);
			Items.Add((int)Sport.Entities.Team.Fields.CityId, "רשות", 150);

			//
			// toolBar
			//
			tbbImportTeams = new ToolBarButton();
			tbbImportTeams.ImageIndex = (int)Sport.Resources.ColorImages.ImportPlayers;
			tbbImportTeams.Text = "ייבוא קבוצות";
			tbbImportTeams.Visible = System.IO.File.Exists(this.ImportFilePath);
			toolBar.Buttons.Add(tbbImportTeams);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);
		}
		#endregion

		#region Open View
		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.Team.TypeName);

			SchoolsTableView schoolView = new SchoolsTableView();
			schoolView.State[SelectionDialog] = "1";
			schoolDialog = new EntitySelectionDialog(schoolView);

			userDialog = new EntitySelectionDialog(new UsersTableView());
			userDialog.View.State[SelectionDialog] = "1";
			userDialog.View.State["UserType"] = ((int)Sport.Types.UserType.Internal).ToString();

			EntityFilter filter = new EntityFilter(
				Sport.Entities.Championship.CurrentSeasonFilter());
			EntityListView.Fields[(int)Sport.Entities.Team.Fields.Championship].Values =
				Sport.Entities.Championship.Type.GetEntities(filter);
			EntityListView.Fields[(int)Sport.Entities.Team.Fields.Championship].CanEdit = false;

			Details = new int[] { (int) ColumnTitles.Status, (int) ColumnTitles.Supervisor, 
								(int) ColumnTitles.RegisterDate }; //, (int) ColumnTitles.Charge

			EntityListView.Fields[(int)Sport.Entities.Team.Fields.School].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int)Sport.Entities.Team.Fields.School].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));

			EntityListView.Fields[(int)Sport.Entities.Team.Fields.Supervisor].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int)Sport.Entities.Team.Fields.Supervisor].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(userDialog.ValueSelector));

			Sport.Entities.Team team = null;
			if (State["team"] != null)
			{
				try
				{
					team = new Sport.Entities.Team(Int32.Parse(State["team"]));
				}
				catch (Exception e)
				{
					System.Diagnostics.Debug.WriteLine("failed to create new team entity while opening team view: " + e.Message);
					System.Diagnostics.Debug.WriteLine("team: " + State["team"]);
				}
				if (team != null)
				{
					State[Sport.Entities.Championship.TypeName] = team.Championship.Id.ToString();
					State[Sport.Entities.Sport.TypeName] = team.Championship.Sport.Id.ToString();
					State[Sport.Entities.ChampionshipCategory.TypeName] = team.Category.Id.ToString();
				}
				else
				{
					State["team"] = null;
				}
			}

			//Filters:
			//region filter
			EntityType regionType = Sport.Entities.Region.Type;
			Entity[] regions = regionType.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, null, "<בחר מחוז>", 120);
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);
			Filters.Add(regionFilter);
			switch (this.RequestState())
			{
				case (int)TeamViewState.Championship:
					//define proper columns:
					Columns = new int[] { (int) ColumnTitles.School, (int) ColumnTitles.SchoolSymbol,
											(int) ColumnTitles.Index, (int) ColumnTitles.Sport, 
											(int) ColumnTitles.Category, (int) ColumnTitles.Status, 
											(int) ColumnTitles.Supervisor };
					//add proper filters:
					//sport filter
					EntityType sportType = Sport.Entities.Sport.Type;
					Entity[] sports = sportType.GetEntities(null);
					Sport.Entities.Sport sport = null;
					if (State[Sport.Entities.Championship.TypeName] != null)
					{
						Sport.Entities.Championship championship =
							new Sport.Entities.Championship(Int32.Parse(State[Sport.Entities.Championship.TypeName].ToString()));
						State[Sport.Entities.Sport.TypeName] = championship.Sport.Id.ToString();
						State[Sport.Entities.Region.TypeName] = championship.Region.Id.ToString();
					}
					sportFilter = new ComboBoxFilter("ענף ספורט:", sports, sport, "<כל הענפים>", 200);
					sportFilter.FilterChanged += new System.EventHandler(SportFiltered);
					Filters.Add(sportFilter);
					//-------------------
					//championship filter
					EntityType champType = Sport.Entities.Championship.Type;
					filter = new EntityFilter();
					/*
					if (!Sport.Core.PermissionsManager.IsSuperUser(Core.UserManager.CurrentUser.Id))
					{
						filter.Add(new EntityFilterField(
							(int) Sport.Entities.Championship.Fields.Region, 
							Core.UserManager.CurrentUser.UserRegion));
					}
					*/
					Entity[] champs = champType.GetEntities(filter);
					champFilter = new ComboBoxFilter("אליפות:", champs, null, "<בחר אליפות>", 200);
					champFilter.FilterChanged += new System.EventHandler(ChampFiltered);
					Filters.Add(champFilter);
					//-------------------
					//category filter
					categoryFilter = new ComboBoxFilter("קטגוריה:", null, null, "<כל הקטגוריות>", 200);
					categoryFilter.FilterChanged += new System.EventHandler(CategoryFiltered);
					Filters.Add(categoryFilter);
					//-------------------
					//apply filters:
					RefreshChampFilter();
					if (State[Sport.Entities.Championship.TypeName] != null)
						RefreshCategoryFilter();
					break;
				case (int)TeamViewState.School:
					//define proper columns:
					Columns = new int[] { (int) ColumnTitles.Sport, (int) ColumnTitles.Championship,
											(int) ColumnTitles.Category, (int) ColumnTitles.Index, 
											(int) ColumnTitles.Status, (int) ColumnTitles.Supervisor };

					//define default sort order for the columns:
					Sort = new int[] {(int) ColumnTitles.Sport, (int) ColumnTitles.Championship, 
											((int) ColumnTitles.Index)+Int32.MinValue};

					//add proper filters:
					//-------------------
					//city filter
					cityFilter = new ComboBoxFilter("ישוב:", null, null, "<בחר ישוב>", 120);
					cityFilter.FilterChanged += new System.EventHandler(CityFiltered);
					Filters.Add(cityFilter);
					//-------------------
					//school filter
					schoolFilter = new ComboBoxFilter("בית ספר:", null, null, "<בחר בית ספר>", 180);
					schoolFilter.FilterChanged += new System.EventHandler(SchoolFiltered);
					Filters.Add(schoolFilter);
					//System.Windows.Forms.MessageBox.Show(Sport.Common.Tools.CStrDef(State[Sport.Entities.School.TypeName], ""));
					//-------------------
					//apply filters:
					RefreshCityFilter();
					RefreshSchoolFilter();

					//currently can't add new team in this case:
					this.CanInsert = false;

					//also prevent user from changing the team:
					EntityListView.Fields[(int)Sport.Entities.Team.Fields.Category].CanEdit = false;
					EntityListView.Fields[(int)Sport.Entities.Team.Fields.Championship].CanEdit = false;
					EntityListView.Fields[(int)Sport.Entities.Team.Fields.School].CanEdit = false;
					break;
			}
			//----------------------------

			if (State[Sport.Entities.Region.TypeName] == null)
			{
				regionFilter.Value = new Sport.Entities.Region(Core.UserManager.CurrentUser.UserRegion).Entity;
				RegionFiltered(null, EventArgs.Empty);
			}

			RefreshFilters();
			Requery();

			base.Open();

			if (team != null)
			{
				//this.G
				this.Current = team.Entity;
			}
		} //end function Open
		#endregion

		#region Filters
		/// <summary>
		/// refresh all filters of this view.
		/// </summary>
		private void RefreshFilters()
		{
			_filter = new EntityFilter();

			object school = Core.Tools.GetStateValue(State[Sport.Entities.School.TypeName]);
			object champ = Core.Tools.GetStateValue(State[Sport.Entities.Championship.TypeName]);
			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);
			object city = Core.Tools.GetStateValue(State[Sport.Entities.City.TypeName]);
			object sport = Core.Tools.GetStateValue(State[Sport.Entities.Sport.TypeName]);
			object category = Core.Tools.GetStateValue(State[Sport.Entities.ChampionshipCategory.TypeName]);

			if (school != null)
			{
				_filter.Add(new EntityFilterField((int)Sport.Entities.Team.Fields.School,
					(int)school));
				Core.Tools.SetFilterValue(
					schoolFilter, Sport.Entities.School.Type.Lookup((int)school));
			}

			if (champ != null)
			{
				_filter.Add(new EntityFilterField((int)Sport.Entities.Team.Fields.Championship,
					(int)champ));
				Core.Tools.SetFilterValue(
					champFilter, Sport.Entities.Championship.Type.Lookup((int)champ));
			}

			if (category != null)
			{
				_filter.Add(new EntityFilterField((int)Sport.Entities.Team.Fields.Category,
					(int)category));
				Core.Tools.SetFilterValue(
					categoryFilter, Sport.Entities.ChampionshipCategory.Type.Lookup((int)category));
			}

			if (sport != null)
			{
				Core.Tools.SetFilterValue(sportFilter,
					Sport.Entities.Sport.Type.Lookup((int)sport));
			}

			if (region != null)
			{
				Core.Tools.SetFilterValue(regionFilter,
					Sport.Entities.Region.Type.Lookup((int)region));
			}

			if (city != null)
			{
				Core.Tools.SetFilterValue(cityFilter,
					Sport.Entities.City.Type.Lookup((int)city));
			}
		}

		/// <summary>
		/// refresh only the city filter using the selected region and city.
		/// </summary>
		private void RefreshCityFilter()
		{
			if (cityFilter == null)
				return;

			EntityType typeCity = Sport.Entities.City.Type;
			EntityFilter filter = new EntityFilter();

			//get selected city and region:
			object city = Core.Tools.GetStateValue(State[Sport.Entities.City.TypeName]);
			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);

			//check if any region selected:
			if (region != null)
			{
				//apply the selected region on the city filter:
				filter.Add(new EntityFilterField((int)Sport.Entities.City.Fields.Region, (int)region));
				//get all cities for the selected region and put in combo:
				Entity[] cities = typeCity.GetEntities(filter);
				cityFilter.SetValues(cities);
				//restore last selected value:
				if (city != null)
					Core.Tools.SetFilterValue(cityFilter, typeCity.Lookup((int)city));
			}
			else
			{
				//region is null, clear city filter:
				Core.Tools.SetFilterValue(cityFilter, null);
				cityFilter.SetValues(null);
			}

			//apply in the global state:
			State[Sport.Entities.City.TypeName] = Core.Tools.GetFilterValue(cityFilter.Value);
		} //end function RefreshCityFilter

		/// <summary>
		/// refresh only the school filter using the selected city and school.
		/// </summary>
		private void RefreshSchoolFilter()
		{
			if (schoolFilter == null)
				return;

			EntityType typeSchool = Sport.Entities.School.Type;
			EntityFilter filter = new EntityFilter();

			//get selected school and city:
			object school = Core.Tools.GetStateValue(State[Sport.Entities.School.TypeName]);
			object city = Core.Tools.GetStateValue(State[Sport.Entities.City.TypeName]);

			//refresh combo box only if city selected, otherwise clear all values.
			if (city != null)
			{
				//apply filter:
				filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.City, (int)city));
				//get all schools for this city:
				Entity[] schools = typeSchool.GetEntities(filter);
				//put schools in combo:
				schoolFilter.SetValues(schools);
			}
			else
			{
				Core.Tools.SetFilterValue(schoolFilter, null);
				schoolFilter.SetValues(null);
			}

			//restore last selection:
			if (school != null)
				Core.Tools.SetFilterValue(schoolFilter, typeSchool.Lookup((int)school));

			//apply in the global state:
			State[Sport.Entities.School.TypeName] = Core.Tools.GetFilterValue(schoolFilter.Value);
		} //end function RefreshSchoolFilter

		/// <summary>
		/// refresh only the championship filter using the selected sport.
		/// </summary>
		private void RefreshChampFilter()
		{
			EntityType typeChamp = Sport.Entities.Championship.Type;
			EntityFilter filter = new EntityFilter();

			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);
			object sport = Core.Tools.GetStateValue(State[Sport.Entities.Sport.TypeName]);
			object champ = Core.Tools.GetStateValue(State[Sport.Entities.Championship.TypeName]);

			if (region != null)
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.Championship.Fields.Region, (int)region));
				if (sport != null)
					filter.Add(new EntityFilterField((int)Sport.Entities.Championship.Fields.Sport, (int)sport));
			}

			if (filter.Count > 0)
			{
				//get all championships for the selected sport and put in combo:
				Entity[] champs = typeChamp.GetEntities(filter);
				champFilter.SetValues(champs);
				//restore last selected value:
				if (champ != null)
					Core.Tools.SetFilterValue(champFilter, typeChamp.Lookup((int)champ));
			}
			else
			{
				champFilter.SetValues(null);
			}

			//apply null selection in global state:
			//apply in the global state:
			State[Sport.Entities.Championship.TypeName] = Core.Tools.GetFilterValue(champFilter.Value);
		} //end function RefreshChampFilters

		/// <summary>
		/// refresh only the category filter using the selected championship.
		/// </summary>
		private void RefreshCategoryFilter()
		{
			EntityFilter filter = new EntityFilter();
			object champ = Core.Tools.GetStateValue(State[Sport.Entities.Championship.TypeName]);
			object category = Core.Tools.GetStateValue(State[Sport.Entities.ChampionshipCategory.TypeName]);

			if (champ != null)
				filter.Add(new EntityFilterField((int)Sport.Entities.ChampionshipCategory.Fields.Championship, (int)champ));

			//get all categories for the selected championship and put in combo:
			Entity[] categories = null;
			if (filter.Count > 0)
				categories = Sport.Entities.ChampionshipCategory.Type.GetEntities(filter);
			categoryFilter.SetValues(categories);
			//restore last selected value:
			if (category != null)
				Core.Tools.SetFilterValue(categoryFilter, Sport.Entities.ChampionshipCategory.Type.Lookup((int)category));


			//apply null selection in global state:
			//apply in the global state:
			State[Sport.Entities.ChampionshipCategory.TypeName] = Core.Tools.GetFilterValue(categoryFilter.Value);
		} //end function RefreshCategoryFilter
		#endregion

		#region Filters Change
		/// <summary>
		/// called when the chamionship filter (combo box) is changed.
		/// </summary>
		private void ChampFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.Championship.TypeName;
			State[identify] = Core.Tools.GetFilterValue(champFilter.Value);
			State[Sport.Entities.ChampionshipCategory.TypeName] = null;
			//apply championship change in categories:
			RefreshCategoryFilter();
			ReloadView();
		}

		/// <summary>
		/// called when the category filter (combo box) is changed.
		/// </summary>
		private void CategoryFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.ChampionshipCategory.TypeName;
			State[identify] = Core.Tools.GetFilterValue(categoryFilter.Value);
			ReloadView();
		}

		/// <summary>
		/// called when the sport filter (combo box) is changed.
		/// </summary>
		private void SportFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.Sport.TypeName;
			State[identify] = Core.Tools.GetFilterValue(sportFilter.Value);
			State[Sport.Entities.Championship.TypeName] = null;
			//apply sport change in championships:
			RefreshChampFilter();
			//apply championship change in categories:
			RefreshCategoryFilter();
			ReloadView();
		}

		/// <summary>
		/// called when the region filter (combo box) is changed.
		/// </summary>
		private void RegionFiltered(object sender, EventArgs e)
		{
			EntityFilter entitityFilter = null;
			if (regionFilter.Value != null)
			{
				int selectedRegion = ((Entity)regionFilter.Value).Id;
				if (selectedRegion != Sport.Entities.Region.CentralRegion)
					entitityFilter = new EntityFilter(new EntityFilterField((int)Sport.Entities.Sport.Fields.CentralRegionOnly, 1, true));
			}
			sportFilter.SetValues(Sport.Entities.Sport.Type.GetEntities(entitityFilter));

			string identify = Sport.Entities.Region.TypeName;
			State[identify] = Core.Tools.GetFilterValue(regionFilter.Value);
			State[Sport.Entities.City.TypeName] = null;
			State[Sport.Entities.School.TypeName] = null;
			//apply region change in cities and schools as well:
			RefreshCityFilter();
			RefreshSchoolFilter();
			RefreshChampFilter();
			ReloadView();
		}

		/// <summary>
		/// called when the city filter (combo box) is changed.
		/// </summary>
		private void CityFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.City.TypeName;
			State[identify] = Core.Tools.GetFilterValue(cityFilter.Value);
			State[Sport.Entities.School.TypeName] = null;
			//apply city change in schools:
			RefreshSchoolFilter();
			ReloadView();
		}

		/// <summary>
		/// called when the school filter (combo box) is changed.
		/// </summary>
		private void SchoolFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.School.TypeName;
			State[identify] = Core.Tools.GetFilterValue(schoolFilter.Value);
			ReloadView();
		}
		#endregion

		#region Inherited Methods
		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null || entity.Id <= 0)
				return false;

			Sport.Entities.Team team = new Sport.Entities.Team(entity);
			if (team.Id < 0)
				return false;

			if (team.Category != null)
			{
				Sport.Championships.Championship championship = Sport.Championships.Championship.GetChampionship(team.Category.Id);
				if (championship != null && championship.Phases != null)
				{
					int champCount = 0; //how many championships containing this team?
					string championships = "";
					//search the phases groups for the team:
					foreach (Sport.Championships.Phase phase in championship.Phases)
					{
						if (phase.Groups != null)
						{
							foreach (Sport.Championships.Group group in phase.Groups)
							{
								if (group.Teams != null)
								{
									foreach (Sport.Championships.Team groupTeam in group.Teams)
									{
										Sport.Entities.Team curTeam = groupTeam.TeamEntity;
										if (curTeam != null)
										{
											if (team.Equals(curTeam))
											{
												champCount++;
												championships += phase.Name + " - " + group.Name + "\n";
											}
										}
									}
								}
							}
						}
					}

					if (champCount > 0)
					{
						Sport.UI.MessageBox.Show("הקבוצה '" + team.Name + "' רשומה בשלבי האליפות הבאים: " +
							"\n" + championships + "יש להסיר את הקבוצה משלבים אלו",
							"מחיקת קבוצה", MessageBoxIcon.Warning);
						return false;
					}
				}
			}

			//check if the team has any players registered:
			Sport.Entities.Player[] players = team.Players;
			if (players != null && players.Length > 0)
			{
				string names = "";
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i] != null)
						names += players[i].Name + "\n";

					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Show("הקבוצה '" + team.Name + "' כוללת את השחקנים הבאים: \n" + names +
					"יש להסיר את השחקנים מקבוצה זו", "מחיקת קבוצה", MessageBoxIcon.Warning);
				return false;
			}

			bool result;
			result = Sport.UI.MessageBox.Ask("האם למחוק את הקבוצה '" + team.Name + "'?", false);

			/*
			//delete charages as well...
			if (result == true)
			{
				Sport.Entities.Charge[] charges=team.School.Charges;
				foreach (Sport.Entities.Charge charge in charges)
				{
					charge.Entity.Delete();
				}
				EntityFilter filter=new EntityFilter(
					(int) Sport.Entities.Charge.Fields.School, team.School.Id);
				Sport.Entities.Charge.Type.Reset(filter);
			}
			*/

			return result;
		}

		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			/*
				//get numbers range:
				int pNumFromField = (int) Sport.Entities.Team.Fields.PlayerNumberFrom;
				int pNumToField = (int) Sport.Entities.Team.Fields.PlayerNumberTo;
				int oldPlayerNumberFrom = Sport.Common.Tools.CIntDef(
					entityEdit.Entity.Fields[pNumFromField], -1);
				int oldPlayerNumberTo = Sport.Common.Tools.CIntDef(
					entityEdit.Entity.Fields[pNumToField], -1);
				int newPlayerNumberFrom = Sport.Common.Tools.CIntDef(
					entityEdit.Fields[pNumFromField], -1);
				int newPlayerNumberTo = Sport.Common.Tools.CIntDef(
					entityEdit.Fields[pNumToField], -1);
			*/
			base.OnValueChange(entityEdit, entityField);
		}

		protected override void OnSaveEntity(Entity entity)
		{
			EntityFilter filter = new EntityFilter((int)Sport.Entities.Team.Fields.Id, entity.Id);
			Sport.Entities.Team.Type.Reset(filter);

			//abort if new entity:
			if (entity == null)
				return;

			//build team entity:
			Sport.Entities.Team team = new Sport.Entities.Team(entity);

			//competition type?
			if (team.Championship.Sport.SportType != Sport.Types.SportType.Competition)
				return;

			//get numbers range:
			int pNumFrom = team.PlayerNumberFrom;
			int pNumTo = team.PlayerNumberTo;

			//valid?
			if (pNumFrom < 0)
				return;

			//get team players:
			Sport.Entities.Player[] players = team.GetPlayers();

			//got anything?
			int changedCount = 0;
			if ((players != null) && (players.Length > 0))
			{
				//ask user to confirm
				if (!Sport.UI.MessageBox.Ask("האם לשנות מספר חזה של שחקנים רשומים?", "שינוי טווח מספרים", MessageBoxIcon.Question, false))
					return;

				//sort players by number:
				ArrayList arrPlayers = new ArrayList(players);
				arrPlayers.Sort(new PlayerNumberComparer());

				//iterate over players:
				for (int i = 0; i < arrPlayers.Count; i++)
				{
					//get current player:
					Sport.Entities.Player player = (Sport.Entities.Player)arrPlayers[i];

					//get shirt number:
					int curShirtNumber = player.Number;

					//need new number?
					if ((curShirtNumber < 0) || (curShirtNumber < pNumFrom) || (curShirtNumber > pNumTo))
					{
						//build new number:
						int newShirtNumber = pNumFrom + i;

						//change if already exists
						while (ShirtNumberExists(arrPlayers, newShirtNumber))
						{
							if (newShirtNumber > pNumTo)
							{
								newShirtNumber = -1;
								break;
							}
							newShirtNumber++;
						}

						//assign the new shirt number:
						player.Edit();
						player.Number = newShirtNumber;
						player.Save();
						changedCount++;
					} //end if need new number
				} //end loop over players

				//changed anything?
				if (changedCount > 0)
				{
					Sport.UI.MessageBox.Success("מספר חולצה של " +
						Sport.Common.Tools.BuildOneOrMany("שחקן", "שחקנים", changedCount, true) +
						" שונה בהצלחה", "שמירת קבוצה");
					Sport.Championships.Championship.ResetChampionship(
						team.Category.Id);
				}
			} //end if got any players
		} //end function OnSaveEntity

		private bool ShirtNumberExists(ArrayList arrPlayers, int number)
		{
			foreach (Sport.Entities.Player player in arrPlayers)
			{
				if (player.Number == number)
					return true;
			}
			return false;
		}

		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			MenuItem[] menuItems = null;

			//int champID=(int) team.Fields[(int) Sport.Entities.Team.Fields.Championship];
			//EntityType champType=Sport.Entities.Championship.Type;
			//Entity champ=champType.Lookup(champID);
			ArrayList arrItems = new ArrayList();
			switch (selectionType)
			{
				case (SelectionType.Single):
					Entity team = this.Current;
					if (team != null)
					{
						arrItems.Add(new MenuItem("פתח", new System.EventHandler(TeamDetailsClick)));
						arrItems.Add(new MenuItem("-"));

						//get players for given team.

						//first, define filter:
						EntityFilter filter = new EntityFilter(
							new EntityFilterField((int)Sport.Entities.Player.Fields.Team, team.Id));
						//get players list:
						Entity[] players = Sport.Entities.Player.Type.GetEntities(filter);
						if (players.Length > 0)
							arrItems.Add(new MenuItem("כרטיסי שחקן", new System.EventHandler(PrintPlayerCards)));


						arrItems.Add(new MenuItem("שחקנים", new System.EventHandler(TeamPlayersClick)));
						arrItems.Add(new MenuItem("העתק לאליפות אחרת", new System.EventHandler(CopyTeamClick)));
						arrItems.Add(new MenuItem("פתח בית ספר", new System.EventHandler(OpenSchoolClick)));
					}
					break;
				case (SelectionType.Multiple):
					arrItems.Add(new MenuItem("העתק לאליפות אחרת", new System.EventHandler(CopyTeamClick)));
					arrItems.Add(new MenuItem("הדפס שחקנים", new System.EventHandler(PrintAllPlayers)));
					break;
			}

			if (arrItems.Count > 0)
			{
				menuItems = new MenuItem[arrItems.Count];
				for (int i = 0; i < arrItems.Count; i++)
				{
					menuItems[i] = (MenuItem)arrItems[i];
				}
				menuItems[0].DefaultItem = true;
			}

			return menuItems;
		}

		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			string regionID = null;
			EntityField entityField;

			//set school value with the school filter value, if any selected:
			if ((schoolFilter != null) && (schoolFilter.Value != null))
			{
				//school selected, build new entity for that school:
				Sport.Entities.School school = new Sport.Entities.School((int)schoolFilter.Value);
				if (school.IsValid())
				{
					//extract region id:
					regionID = school.Region.Id.ToString();

					//change the school field value:
					entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Team.Fields.School];
					entityField.SetValue(EntityListView.EntityEdit, ((Entity)schoolFilter.Value));
				}
			}

			//set championship value with the championship filter value, if any selected:
			if ((champFilter != null) && (champFilter.Value != null))
			{
				//championship selected, build new entity for that schampionship:
				Sport.Entities.Championship champ = new Sport.Entities.Championship((Entity)champFilter.Value);
				if (champ.IsValid())
				{
					//extract region id:
					regionID = champ.Region.Id.ToString();

					//change the championship field value:
					entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Team.Fields.Championship];
					entityField.SetValue(EntityListView.EntityEdit, ((Entity)champFilter.Value));
				}
			}

			//set championship category if any selected in the filter:
			if ((categoryFilter != null) && (categoryFilter.Value != null))
			{
				entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Team.Fields.Category];
				entityField.SetValue(EntityListView.EntityEdit, ((Entity)categoryFilter.Value));
			}

			//set championship category if we have only one category:
			if ((champFilter != null) && (champFilter.Value != null))
			{
				Sport.Entities.Championship champ = new Sport.Entities.Championship((Entity)champFilter.Value);
				Sport.Entities.ChampionshipCategory[] categories = champ.GetCategories();
				if (categories.Length == 1)
				{
					entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Team.Fields.Category];
					entityField.SetValue(EntityListView.EntityEdit, categories[0].Entity);
				}
			}

			//change status of new team to Confirmed:
			entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Team.Fields.Status];
			entityField.SetValue(EntityListView.EntityEdit, (int)Sport.Types.TeamStatusType.Confirmed);

			//change Date to the current date:
			entityField = EntityListView.EntityType.Fields[(int)Sport.Entities.Team.Fields.RegisterDate];
			entityField.SetValue(EntityListView.EntityEdit, DateTime.Now);

			//set user selection dialog region as well:
			userDialog.View.State[Sport.Entities.Region.TypeName] = regionID;
			userDialog.View.State[Sport.Entities.School.TypeName] = null;
		}

		protected override void NewEntity()
		{
			if (champFilter == null || champFilter.Value == null)
			{
				Sport.UI.MessageBox.Show("בחר אליפות");
			}
			if (categoryFilter == null || categoryFilter.Value == null)
			{
				Sport.UI.MessageBox.Show("בחר קטגוריה");
				return;
			}

			Sport.Entities.ChampionshipCategory category = new Sport.Entities.ChampionshipCategory((Entity)categoryFilter.Value);

			schoolDialog.View.State[Sport.Entities.Championship.TypeName] = category.Championship.Id.ToString();
			schoolDialog.View.State[Sport.Entities.ChampionshipCategory.TypeName] = category.Id.ToString();
			if (regionFilter.Value != null)
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = (regionFilter.Value as Entity).Id.ToString();
			schoolDialog.Multiple = true;
			schoolDialog.Entities = null;

			if (schoolDialog.ShowDialog() == DialogResult.OK)
			{
				string strErrorMsg = "";
				int dummy = 0;
				Entity[] entities = schoolDialog.Entities;
				if (entities != null)
				{
					foreach (Entity curSchoolEnt in entities)
					{
						try
						{
							strErrorMsg += TeamsTableView.AddTeam(
								curSchoolEnt.Id, category, ref dummy);
						}
						catch (Exception e)
						{
							strErrorMsg += "שגיאה כללית בעת הוספת קבוצה עבור בית הספר '" +
								curSchoolEnt.Name + "': " + e.Message + "\n";
						}
					}
				}
				if (strErrorMsg.Length > 0)
					Sport.UI.MessageBox.Error(strErrorMsg, "שגיאה בעת שמירת נתונים");
			}

			schoolDialog.Multiple = false;
		}

		public static string AddTeam(int schoolID,
			Sport.Entities.ChampionshipCategory category, int index, ref int teamID)
		{
			//generate new team:
			Sport.Entities.Team team = new Sport.Entities.Team(
				Sport.Entities.Team.Type.New());

			//create school entity:
			Sport.Entities.School school = new Sport.Entities.School(schoolID);

			//get championship:
			Sport.Entities.Championship championship = category.Championship;

			//assign championsip and championship category:
			team.Championship = championship;
			team.Category = category;

			//assign school:
			team.School = school;

			//assign index
			if (index > 0)
				team.Index = index;

			//assign shirt numbers range from the school:
			team.PlayerNumberFrom = school.PlayerNumberFrom;
			team.PlayerNumberTo = school.PlayerNumberTo;

			//need to check shirt numbers?
			/*
			if (championship.Sport.SportType == Sport.Types.SportType.Competition)
			{
				//look for conflict
				bool blnConflict=false;
				int pNumFrom=team.PlayerNumberFrom;
				int pNumTo=team.PlayerNumberTo;
				if (!team.School.ValidShirtRange())
					blnConflict = true;
				
				//get all teams:
				Sport.Entities.Team[] arrTeams=category.GetTeams();
				
				//iterate through the teams, look for team with conflicting numbers:
				foreach (Sport.Entities.Team curTeam in arrTeams)
				{
					if ((pNumFrom <= curTeam.PlayerNumberTo)&&
						(pNumTo >= curTeam.PlayerNumberFrom))
					{
						blnConflict = true;
						break;
					}
				}
				
				//got conflict?
				if (blnConflict)
				{
					//find maximum number
					int maxNumberTo=0;
					foreach (Sport.Entities.Team curTeam in arrTeams)
					{
						int curNumberTo=curTeam.School.PlayerNumberTo;
						if (curNumberTo > maxNumberTo)
							maxNumberTo = curNumberTo;
					}
					
					//assign proper numbers
					//team.PlayerNumberFrom = maxNumberTo+1;
					//team.PlayerNumberTo = team.PlayerNumberFrom+30;
				} //end if got conflict
			} //end if championship of type competition
			*/

			//assign status and register date:
			team.Status = Sport.Types.TeamStatusType.Confirmed;
			team.RegisterDate = DateTime.Now;

			//save...
			EntityResult result = team.Save();

			//success?
			if (result != EntityResult.Ok)
			{
				return "שגיאה בעת שמירת הקבוצה '" + school.Name + "':\n" +
					result.GetMessage() + "\n\n";
			}

			//done.
			teamID = team.Id;
			return "";
		} //end function AddTeam

		public static string AddTeam(int schoolID,
			Sport.Entities.ChampionshipCategory category, ref int teamID)
		{
			return AddTeam(schoolID, category, 0, ref teamID);
		}

		protected override void OnSelectEntity(Entity entity)
		{
			//new enitity?
			bool isNewEntity = ((entity is EntityEdit) && ((entity as EntityEdit).Entity == null));

			Sport.Entities.Team team = null;
			if (entity != null)
				team = new Sport.Entities.Team(entity);

			string regionID = "";
			string cityID = "";
			string clubStatus = "";

			//get region and school:
			GetTeamRegionAndCity(team, ref regionID, ref cityID);

			//apply filter:
			ApplyCategoryFilter(team);

			//club?
			clubStatus = CheckTeamClubStatus(team);

			//check if the championship is for clubs only:
			if ((team != null) && (team.Championship != null))
			{
				//also check the championship status to decide category state:
				int champStatus = (int)team.Championship.Status;
				int teamRegStatus = (int)Sport.Types.ChampionshipType.TeamRegister;
				bool canEdit = ((champStatus == teamRegStatus) || (isNewEntity));
				EntityListView.Fields[(int)Sport.Entities.Team.Fields.Category].CanEdit = canEdit;
			}

			//dialogs:
			ChangeDialogState(regionID, cityID, clubStatus);

			//set region and school for user selection dialog:
			SetUserSelectionDialog(entity);
		}

		private void SetUserSelectionDialog(Entity entity)
		{
			if (((entity is EntityEdit) == false) && (entity != null))
			{
				int supervisorID = Sport.Common.Tools.CIntDef(
					entity.Fields[(int)Sport.Entities.Team.Fields.Supervisor], -1);
				if (supervisorID >= 0)
				{
					Sport.Entities.User user = new Sport.Entities.User(supervisorID);
					int schoolID = -1;
					if (user != null)
					{
						userDialog.View.State[Sport.Entities.Region.TypeName] = user.Region.Id.ToString();
						if (user.School != null)
							schoolID = user.School.Id;
					}
					userDialog.View.State[Sport.Entities.School.TypeName] =
						Sport.Common.Tools.CStrDef(Sport.Common.Tools.IIF((schoolID >= 0), schoolID.ToString(), null), null);
				}
				else
				{
					int champID = (int)entity.Fields[(int)Sport.Entities.Team.Fields.Championship];
					Sport.Entities.Championship champ = new Sport.Entities.Championship(champID);
					userDialog.View.State[Sport.Entities.School.TypeName] = null;
					if (champ != null)
						userDialog.View.State[Sport.Entities.Region.TypeName] = champ.Region.Id.ToString();
				}
			}
			else
			{
				userDialog.View.State[Sport.Entities.Region.TypeName] = null;
				userDialog.View.State[Sport.Entities.School.TypeName] = null;
			}
		}

		private void ChangeDialogState(string regionID, string cityID, string clubStatus)
		{
			//change the school selection dialog filters according to the 
			//region and city of the selected team or selected filter.
			schoolDialog.View.State[Sport.Entities.Region.TypeName] =
				Sport.Common.Tools.CStrDef(Sport.Common.Tools.IIF((regionID.Length > 0), regionID, null), null);
			schoolDialog.View.State[Sport.Entities.City.TypeName] =
				Sport.Common.Tools.CStrDef(Sport.Common.Tools.IIF((cityID.Length > 0), cityID, null), null);
			schoolDialog.View.State["IsClub"] =
				Sport.Common.Tools.CStrDef(Sport.Common.Tools.IIF((clubStatus.Length > 0), clubStatus, null), null);
			if ((champFilter != null) && (champFilter.Value != null))
				schoolDialog.View.State[Sport.Entities.Championship.TypeName] = (champFilter.Value as Entity).Id.ToString();
			else
				schoolDialog.View.State[Sport.Entities.Championship.TypeName] = null;
		}

		private string CheckTeamClubStatus(Sport.Entities.Team team)
		{
			//got any team?
			if ((team != null) && (team.Championship != null))
				return team.Championship.IsClubs.ToString();

			//try to read filters:
			if ((champFilter != null) && (champFilter.Value != null))
			{
				Sport.Entities.Championship champEnt = new Sport.Entities.Championship((Entity)champFilter.Value);
				return champEnt.IsClubs.ToString();
			}

			//none.
			return "";
		}

		private void ApplyCategoryFilter(Sport.Entities.Team team)
		{
			//got anything?
			if ((team != null) && (team.Championship != null))
			{
				//make proper filter for championship category:
				EntityType categoryType = Sport.Entities.ChampionshipCategory.Type;
				EntityFilter categoryFilter = new EntityFilter((int)Sport.Entities.ChampionshipCategory.Fields.Championship, team.Championship.Id);

				//apply filter on categorties list:
				EntityListView.Fields[(int)Sport.Entities.Team.Fields.Category].Values =
					Sport.Entities.ChampionshipCategory.Type.GetEntities(categoryFilter);
			}
		}
		private void GetTeamRegionAndCity(Sport.Entities.Team team, ref string regionID,
			ref string cityID)
		{
			//get region and school. try from selected entity, if non selected 
			//get from the selected filter.
			if ((team != null) && (team.School != null))
			{
				//read the region id and city id of the school:
				if (team.School.Region != null)
					regionID = team.School.Region.Id.ToString();
				if (team.School.City != null)
					cityID = team.School.City.Id.ToString();
			}
			else
			{
				if ((champFilter != null) && (champFilter.Value != null))
				{
					Sport.Entities.Championship champ = new Sport.Entities.Championship((Entity)champFilter.Value);
					if (champ.Region != null)
						regionID = champ.Region.Id.ToString();
				}
				if ((schoolFilter != null) && (schoolFilter.Value != null))
				{
					Sport.Entities.School school = new Sport.Entities.School((Entity)schoolFilter.Value);
					if (school.Region != null)
						regionID = school.Region.Id.ToString();
					if (school.City != null)
						cityID = school.City.Id.ToString();
				}
			}
		}
		#endregion

		#region context menu events
		private void TeamDetailsClick(object sender, EventArgs e)
		{
			OpenDetails(Current);
		}

		private void TeamPlayersClick(object sender, EventArgs e)
		{
			//open the players table view if there is any selected team:
			if (Current != null)
			{
				//get actual entity:
				Sport.Entities.Team team = new Sport.Entities.Team(Current);

				//build the state string and open the view:
				string state = Sport.Entities.Team.TypeName + "=";
				state += team.Id.ToString();
				if (team.Category != null)
					state += "&category=" + team.Category.Category.ToString();
				if (team.Championship != null && team.Championship.Region != null)
					state += "&" + Sport.Entities.Region.TypeName + "=" + team.Championship.Region.Id.ToString();
				ViewManager.OpenView(typeof(PlayersTableView), state);
			}
		}

		private void OpenSchoolClick(object sender, EventArgs e)
		{
			if (Current != null)
			{
				Sport.Entities.Team team = new Sport.Entities.Team(Current);
				if (team.School != null)
				{
					int schoolId = team.School.Id;
					if (schoolId >= 0)
						ViewManager.OpenView(typeof(SchoolsTableView), string.Format("{0}={1}", Sport.Entities.School.TypeName, schoolId));
				}
			}
		}

		private void CopyTeamClick(object sender, EventArgs e)
		{
			Entity[] arrSelectedEntities = GetAllSelectedEntitiesWithCurrent();
			if (arrSelectedEntities == null || arrSelectedEntities.Length == 0)
				return;

			Sport.Entities.Team[] teams = new Sport.Entities.Team[arrSelectedEntities.Length];
			for (int i = 0; i < arrSelectedEntities.Length; i++)
				teams[i] = new Sport.Entities.Team(arrSelectedEntities[i]);

			int champID = teams[0].Championship.Id;
			for (int i = 1; i < teams.Length; i++)
			{
				if (teams[i].Championship.Id != champID)
				{
					Sport.UI.MessageBox.Error("בהעתקת קבוצות מרובות על כל הקבוצות להיות באותה אליפות", "העתקת קבוצה");
					return;
				}
			}

			Sport.Entities.Championship teamChamp = teams[0].Championship;
			EntitySelectionDialog objDialog = new EntitySelectionDialog(
				new ChampionshipsTableView());
			objDialog.View.State[SelectionDialog] = "1";
			objDialog.Multiple = false;
			objDialog.View.State[Sport.Entities.Sport.TypeName] = teamChamp.Sport.Id.ToString();
			if (objDialog.ShowDialog() == DialogResult.OK)
			{
				if (objDialog.Entity != null)
				{
					Sport.Entities.Championship champ =
						new Sport.Entities.Championship(objDialog.Entity);

					if (champ.Id == teamChamp.Id)
					{
						Sport.UI.MessageBox.Error("לא ניתן להעתיק לאותה אליפות", "העתקת קבוצה");
						return;
					}

					if (champ.Sport.Id != teamChamp.Sport.Id)
					{
						Sport.UI.MessageBox.Error("לא ניתן להעתיק לאליפות בענף ספורט אחר",
							"העתקת קבוצה");
						return;
					}

					Sport.Entities.ChampionshipCategory[] arrCategories = champ.GetCategories();
					Hashtable tblTargetCategories = new Hashtable();
					for (int i = 0; i < arrCategories.Length; i++)
					{
						Sport.Entities.ChampionshipCategory objChampCat = arrCategories[i];
						tblTargetCategories[objChampCat.Category] = objChampCat;
					}

					int successCount = 0;
					int lastCategoryId = -1;
					foreach (Sport.Entities.Team team in teams)
					{
						int newTeamID = -1;
						string strTeamName = team.TeamName();

						if (tblTargetCategories[team.Category.Category] == null)
						{
							Sport.UI.MessageBox.Error("לאליפות שנבחרה אין קטגוריה מתאימה לקבוצה '" + strTeamName + "'", "העתקת קבוצה");
							continue;
						}

						Sport.Entities.ChampionshipCategory category = (Sport.Entities.ChampionshipCategory)tblTargetCategories[team.Category.Category];
						Sport.UI.Dialogs.WaitForm.ShowWait("מעתיק את הקבוצה '" + strTeamName + "' אנא המתן...");

						int maxIndex = category.GetMaximumIndex(team.School.Id);
						string strErrorMsg = TeamsTableView.AddTeam(team.School.Id, category, maxIndex + 1, ref newTeamID);
						Sport.UI.Dialogs.WaitForm.HideWait();
						if (strErrorMsg.Length == 0)
						{
							Sport.Entities.Team newTeam = new Sport.Entities.Team(newTeamID);
							if (newTeam.Charge != null)
							{
								if (!newTeam.Charge.Entity.Delete())
								{
									Sport.UI.MessageBox.Warn("אזהרה: מחיקת חיוב אוטומטי נכשלה, אנא מחק את החיוב ידנית", "העתקת קבוצה");
								}
							}
							Sport.UI.Dialogs.WaitForm.ShowWait("מעתיק שחקנים של הקבוצה '" + strTeamName + "' אנא המתן...");
							Sport.Entities.Player[] players = team.GetPlayers();
							if ((players != null) && (players.Length > 0))
							{
								foreach (Sport.Entities.Player player in players)
								{
									strErrorMsg += PlayersTableView.AddPlayer(player.Student.Id, player.Number, newTeam);
								}
							}
							Sport.UI.Dialogs.WaitForm.HideWait();
						}

						if (strErrorMsg.Length > 0)
						{
							Sport.UI.MessageBox.Error("שגיאה בעת העתקת הקבוצה '" + strTeamName + "':\n" + strErrorMsg,
								"העתקת קבוצה");
						}
						else
						{
							successCount++;
							lastCategoryId = category.Id;
						}
					}

					if (successCount > 0)
					{
						Sport.UI.MessageBox.Success("העתקת " + Sport.Common.Tools.BuildOneOrMany("קבוצה", "קבוצות", successCount, false) +
							" לאליפות '" + champ.Name + "' הסתיימה בהצלחה", "העתקת קבוצה");

						string state = Sport.Entities.Region.TypeName + "=";
						state += champ.Region.Id.ToString();
						state += "&" + Sport.Entities.Sport.TypeName + "=" + champ.Sport.Id;
						state += "&" + Sport.Entities.Championship.TypeName + "=" + champ.Id;
						state += "&" + Sport.Entities.ChampionshipCategory.TypeName + "=" + lastCategoryId;
						ViewManager.OpenView(typeof(TeamsTableView), state);
					}
				}
			}
		}

		private void PrintAllPlayers(object sender, EventArgs e)
		{
			Entity[] arrSelectedEntities = GetAllSelectedEntitiesWithCurrent();
			if (arrSelectedEntities == null || arrSelectedEntities.Length == 0)
				return;


			System.Drawing.Printing.PrinterSettings ps;
			Sport.UI.Dialogs.PrintSettingsForm settingsForm;
			if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
			{
				return;
			}

			if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Entities.Team[] teams = new Sport.Entities.Team[arrSelectedEntities.Length];
				for (int i = 0; i < arrSelectedEntities.Length; i++)
					teams[i] = new Sport.Entities.Team(arrSelectedEntities[i]);

				Sport.Documents.DocumentBuilder db = new Sport.Documents.DocumentBuilder("רשימת שחקנים");
				db.Direction = Sport.Documents.Direction.Right;
				db.SetSettings(ps);

				db.Direction = Sport.Documents.Direction.Right;
				db.Font = new System.Drawing.Font("Tahoma", 16, System.Drawing.GraphicsUnit.Pixel);

				foreach (Sport.Entities.Team team in teams)
				{
					//build the state string and open the view:
					string state = Sport.Entities.Team.TypeName + "=";
					state += team.Id.ToString();
					state += "&category=" + team.Category.Category.ToString();
					state += "&" + Sport.Entities.Region.TypeName + "=" + team.Championship.Region.Id.ToString();
					Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים עבור הקבוצה '" + team.Name + "' אנא המתן...", true);
					int index = ViewManager.OpenView(typeof(PlayersTableView), state);
					if (index >= 0)
					{
						Views.PlayersTableView ptv = (Views.PlayersTableView)ViewManager.GetView(index);
						db.Sections.Add(ptv.CreatePrintSection(db));
						ViewManager.CloseView(index);
					}
					Sport.UI.Dialogs.WaitForm.HideWait();
				}

				Sport.Documents.Document document = db.CreateDocument();

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
					pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
					pd.Print();
				}
			}
		}

		/// <summary>
		/// print player cards for players in selected team.
		/// </summary>
		private void PrintPlayerCards(object sender, EventArgs e)
		{
			//cancel if there is no selected team:
			if (EntityListView.Current == null)
				return;

			//get selected team:
			Sport.Entities.Team team = new Sport.Entities.Team(EntityListView.Current);

			Producer.PlayerCardBuildForm pcbf = null;
			try
			{
				pcbf = new Producer.PlayerCardBuildForm(team.Id);
			}
			catch
			{
				Sport.UI.MessageBox.Warn("לא ניתן להדפיס כרטיסי שחקן: קבוצה שגויה", "אזהרת מערכת");
				return;
			}

			pcbf.ShowDialog();
		}
		#endregion

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbImportTeams)
			{
				try
				{
					ImportTeamsFromFile();
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Error("שגיאה כללית בעת ייבוא קבוצות:\n" + ex.ToString(), "שגיאה");
				}
			}
		}

		private void ImportTeamsFromFile()
		{
			if (regionFilter.Value == null)
			{
				Sport.UI.MessageBox.Show("יש לבחור מחוז");
				return;
			}
			if (sportFilter.Value == null)
			{
				Sport.UI.MessageBox.Show("יש לבחור ענף ספורט");
				return;
			}
			if (champFilter.Value == null)
			{
				Sport.UI.MessageBox.Show("יש לבחור אליפות");
				return;
			}
			if (categoryFilter.Value == null)
			{
				Sport.UI.MessageBox.Show("יש לבחור קטגורית אליפות");
				return;
			}
			string[] lines = System.IO.File.ReadAllLines(this.ImportFilePath);
			string relevantKey = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", Sport.Core.Session.Season,
				(regionFilter.Value as Entity).Name,
				(sportFilter.Value as Entity).Name,
				(champFilter.Value as Entity).Name,
				(categoryFilter.Value as Entity).Fields[(int)Sport.Entities.ChampionshipCategory.Fields.Category]
				);
			List<string> matchingLines = lines.ToList().FindAll(l => l.StartsWith(relevantKey));
			if (matchingLines.Count == 0)
			{
				Sport.UI.MessageBox.Show("אין קבוצות מתאימות");
				return;
			}
			List<string> arrSchoolSymbols = new List<string>();
			matchingLines.ForEach(currentLine => arrSchoolSymbols.AddRange(currentLine.Split('\t').Last().Split(',').Distinct()));
			List<string> existingSchools = new List<string>();
			for (int i = 0; i < this.EntityListView.Count; i++)
				existingSchools.Add(new Sport.Entities.Team(this.EntityListView[i]).School.Symbol);
			existingSchools.ForEach(curSymbol => arrSchoolSymbols.Remove(curSymbol));
			if (arrSchoolSymbols.Count == 0)
			{
				Sport.UI.MessageBox.Show("כל הקבוצות כבר רשומות");
				return;
			}

			string fullChampName = string.Format("");
			Forms.ConfirmTeamsImportDialog confirmDialog = new Forms.ConfirmTeamsImportDialog(arrSchoolSymbols.ToArray());
			if (confirmDialog.ShowDialog(this) == DialogResult.OK)
			{
				Sport.Entities.ChampionshipCategory category = new Sport.Entities.ChampionshipCategory((Entity)categoryFilter.Value);
				string strErrorMsg = "";
				int dummy = 0, currentSchoolId;
				arrSchoolSymbols.ForEach(currentSymbol =>
				{
					if (confirmDialog.TryMapSymbol(currentSymbol, out currentSchoolId))
					{
						strErrorMsg += TeamsTableView.AddTeam(currentSchoolId, category, ref dummy);
					}
				});

				if (strErrorMsg.Length > 0)
					Sport.UI.MessageBox.Error(strErrorMsg, "שגיאה בעת שמירת נתונים");
			}
		}

		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			//perform proper actions according to the requested state.
			switch (this.RequestState())
			{
				case (int)TeamViewState.Championship:
					RequeryChamps();
					break;
				case (int)TeamViewState.School:
					RequerySchools();
					break;
			} //end switch current request state

			Cursor.Current = c;
		} //end function Requery

		private void RequerySchools()
		{
			if (schoolFilter == null)
				return;

			Entity school = (Entity)schoolFilter.Value;
			if ((_filter == null) || (school == null))
			{
				EntityListView.Clear();
			}
			else
			{
				EntityListView.Read(_filter);
			}

			Title = "קבוצות";
			if (school != null)
				Title = "קבוצות - " + school.Name;
		}

		private void RequeryChamps()
		{
			Entity champ = null;
			if (champFilter != null)
				champ = (Entity)champFilter.Value;
			if ((_filter == null) || (champ == null))
			{
				EntityListView.Clear();
			}
			else
			{
				EntityListView.Read(_filter);
			}

			Title = "קבוצות";
			if (champ != null)
			{
				Entity champcat = (Entity)categoryFilter.Value;
				Title = "קבוצות - " + champ.Name + (champcat == null ? null : " - " + champcat.Name);
			}
		}

		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();
		}

		/// <summary>
		/// returns the view state: viewing by championship or by school.
		/// </summary>
		private int RequestState()
		{
			if (schoolFilter != null)
			{
				return (int)TeamViewState.School;
			}
			return (int)TeamViewState.Championship;
		}

		private Entity[] GetAllSelectedEntitiesWithCurrent()
		{
			ArrayList arrEntities = new ArrayList();
			Entity[] arrSelectedEntities = this.GetSelectedEntities();
			if (arrSelectedEntities != null && arrSelectedEntities.Length > 0)
			{
				arrEntities.AddRange(arrSelectedEntities);
			}
			else if (this.Current != null)
			{
				arrEntities.Add(this.Current);
			}
			return (Entity[])arrEntities.ToArray(typeof(Entity));
		}

		private enum TeamViewState
		{
			Championship = 0,
			School
		}

		protected override void OpenDetails(Entity entity)
		{
			string strExecute = "Sportsman.Details.TeamDetailsView,Sportsman" +
				((entity != null) ? "?id=" + entity.Id.ToString() : "");

			new OpenDialogCommand().Execute(strExecute);
		}

		private class PlayerNumberComparer : System.Collections.IComparer
		{
			public int Compare(object x, object y)
			{
				if ((x == null) && (y == null))
					return 0;
				if (x == null)
					return 1;
				if (y == null)
					return -1;
				Sport.Entities.Player p1 = (Sport.Entities.Player)x;
				Sport.Entities.Player p2 = (Sport.Entities.Player)y;
				return p1.Number.CompareTo(p2.Number);
			}
		}
	} //end class TeamsTableView
}
