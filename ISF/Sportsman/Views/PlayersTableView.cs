using System;
using System.Linq;
using Sport.UI;
using Sport.Data;
using System.Windows.Forms;
using System.Collections;
using Sportsman.Core;
using Sport.UI.Controls;
using System.Drawing;
using System.Collections.Generic;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for PlayersTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class PlayersTableView : TableView
	{
		#region States

		public Sport.Entities.Sport SelectedSport
		{
			get
			{
				if (SelectedChampionship != null)
					return SelectedChampionship.Sport;

				return State[Sport.Entities.Sport.TypeName] == null ? null :
					new Sport.Entities.Sport((int)Core.Tools.GetStateValue(State[Sport.Entities.Sport.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.Sport.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.Sport.TypeName] = value.Id.ToString();
				}
			}
		}

		public Sport.Entities.Championship SelectedChampionship
		{
			get
			{
				if (SelectedTeam != null)
					return SelectedTeam.Championship;

				return State[Sport.Entities.Championship.TypeName] == null ? null :
					new Sport.Entities.Championship((int)Core.Tools.GetStateValue(State[Sport.Entities.Championship.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.Championship.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.Championship.TypeName] = value.Id.ToString();
				}
			}
		}

		public Sport.Entities.ChampionshipCategory SelectedCategory
		{
			get
			{
				Sport.Entities.Team team = this.SelectedTeam;
				if (team != null)
					return team.Category;

				return State[Sport.Entities.ChampionshipCategory.TypeName] == null ? null :
					new Sport.Entities.ChampionshipCategory(
					(int)Core.Tools.GetStateValue(
					State[Sport.Entities.ChampionshipCategory.TypeName]));
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

		public Sport.Entities.Team SelectedTeam
		{
			get
			{
				return State[Sport.Entities.Team.TypeName] == null ? null :
					new Sport.Entities.Team((int)Core.Tools.GetStateValue(State[Sport.Entities.Team.TypeName]));
			}
			set
			{
				if (value == null)
				{
					State[Sport.Entities.Team.TypeName] = null;
				}
				else
				{
					State[Sport.Entities.Team.TypeName] = value.Id.ToString();
				}
			}
		}

		public Sport.Entities.Region SelectedRegion
		{
			get
			{
				if (SelectedChampionship != null)
					return SelectedChampionship.Region;

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
		#endregion


		/*
		--student card added
		DECLARE team_cursor_5 CURSOR FOR
		SELECT del.STUDENT_ID, c.SPORT_ID
		FROM deleted del INNER JOIN inserted ins ON del.PLAYER_ID=ins.PLAYER_ID
		INNER JOIN TEAMS t On del.TEAM_ID=t.TEAM_ID
		INNER JOIN CHAMPIONSHIPS c On t.CHAMPIONSHIP_ID=c.CHAMPIONSHIP_ID
		WHERE del.GOT_STICKER=0 AND ins.GOT_STICKER=1

		OPEN team_cursor_5	
		FETCH NEXT FROM team_cursor_5
		INTO @student_id, @sport_id
	
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF 	(SELECT COUNT(*) FROM STUDENT_CARDS WHERE STUDENT_ID=@student_id AND SPORT_ID=@sport_id)=0
			BEGIN
				INSERT INTO STUDENT_CARDS (
			END
			UPDATE STUDENT_CARDS SET ISSUE_DATE=GETDATE()
			WHERE STUDENT_ID=@student_id AND SPORT_ID=@sport_id
		
			--advance to next record:
			FETCH NEXT FROM team_cursor_5
			INTO @student_id, @sport_id
		END
		CLOSE team_cursor_5
		DEALLOCATE team_cursor_5
		*/

		private ComboBoxFilter sportFilter;
		private ComboBoxFilter champFilter;
		private ComboBoxFilter categoryFilter;
		private ComboBoxFilter teamFilter;
		private ComboBoxFilter regionFilter;
		private EntityFilter _filter;
		private EntitySelectionDialog playerDialog;

		private List<int> _arrDifferentSchool = new List<int>();
		private Style _differentSchoolStyle = new Style(new System.Drawing.SolidBrush(System.Drawing.Color.Red), null, null);
		private Style _overAgeStyle = new Style(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 153, 51)), null, null);

		private Timer _differentSchoolTimer = null;
		private string _differentSchoolName = "";
		private string _differentSchoolPlayer = "";

		private System.Windows.Forms.ToolBarButton tbbPlayersReport;
		private System.Windows.Forms.ToolBarButton tbbConfirmPlayers;
		private System.Windows.Forms.ToolBarButton tbbImportPlayers;

		private int oldGotStudentCard = -1;
		private int newGotStudentCard = -1;

		private enum ColumnTitles
		{
			PlayerNumber = 0,
			Team,
			PlayerName,
			IdNumber,
			BirthDate,
			Grade,
			Status,
			Remarks,
			RegisterDate,
			SexType
		}

		public PlayersTableView()
		{
			Items.Add((int)Sport.Entities.Player.Fields.Number, "מספר שחקן", 80);
			Items.Add((int)Sport.Entities.Player.Fields.Team, "קבוצה", 100);
			Items.Add((int)Sport.Entities.Player.Fields.Student, "שם תלמיד", 120);
			Items.Add((int)Sport.Entities.Player.Fields.IdNumber, "מספר זהות", 120);
			Items.Add((int)Sport.Entities.Player.Fields.BirthDate, "תאריך לידה", 100);
			Items.Add((int)Sport.Entities.Player.Fields.Grade, "כיתה", 80);
			Items.Add((int)Sport.Entities.Player.Fields.Status, "סטטוס", 100);
			Items.Add((int)Sport.Entities.Player.Fields.Remarks, "הערות", 180, 75);
			Items.Add((int)Sport.Entities.Player.Fields.RegisterDate, "תאריך רישום", 120);
			Items.Add((int)Sport.Entities.Player.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int)Sport.Entities.Player.Fields.SexType, "מין", 80);
			Items.Add((int)Sport.Entities.Player.Fields.Got_Sticker, "כרטיס שחקן?", 100);

			//
			// toolBar
			//
			tbbPlayersReport = new ToolBarButton();
			tbbPlayersReport.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbPlayersReport.Text = "דו\"ח שחקנים";
			tbbPlayersReport.Enabled = false;
			tbbConfirmPlayers = new ToolBarButton();
			tbbConfirmPlayers.ImageIndex = (int)Sport.Resources.ColorImages.MassConfirm;
			tbbConfirmPlayers.Text = "אישור גורף";
			tbbImportPlayers = new ToolBarButton();
			tbbImportPlayers.ImageIndex = (int)Sport.Resources.ColorImages.ImportPlayers;
			tbbImportPlayers.Text = "ייבוא שחקנים";


			toolBar.Buttons.Add(tbbPlayersReport);
			toolBar.Buttons.Add(tbbConfirmPlayers);
			toolBar.Buttons.Add(tbbImportPlayers);

			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);
		}

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbPlayersReport)
			{
				PrintPlayersReport();
			}
			else if (e.Button == tbbConfirmPlayers)
			{
				BatchConfirmPlayers();
			}
			else if (e.Button == tbbImportPlayers)
			{
				try
				{
					ImportPlayers();
				}
				catch (Exception ex)
				{
					Sport.UI.MessageBox.Error("שגיאה כללית בעת ייבוא שחקנים: \n" + ex.ToString(), "ייבוא שחקנים");
					Sport.Data.AdvancedTools.ReportExcpetion(ex);
				}
			}
		}

		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.Player.TypeName);

			EntityListView.Fields[(int)Sport.Entities.Player.Fields.Team].CanEdit = false;

			StudentsTableView studentView = new StudentsTableView();
			//define category and selection dialog flag:
			studentView.State["category"] = this.State["category"];
			studentView.State["team"] = this.State[Sport.Entities.Team.TypeName];
			studentView.State[SelectionDialog] = "1";
			playerDialog = new EntitySelectionDialog(studentView);
			((TableView)playerDialog.View).ToolBarEnabled = true;

			EntityListView.Fields[(int)Sport.Entities.Player.Fields.Student].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int)Sport.Entities.Player.Fields.Student].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(playerDialog.ValueSelector));


			Sport.Entities.Player player = null;
			if (State["player"] != null)
			{
				try
				{
					player = new Sport.Entities.Player(Int32.Parse(State["player"]));
				}
				catch { }
				if (player != null)
				{
					State[Sport.Entities.ChampionshipCategory.TypeName] = player.Team.Category.Id.ToString();
					State[Sport.Entities.Championship.TypeName] = player.Team.Championship.Id.ToString();
					State[Sport.Entities.Sport.TypeName] = player.Team.Championship.Sport.Id.ToString();
					State[Sport.Entities.Team.TypeName] = player.Team.Id.ToString();
				}
				else
				{
					State["player"] = null;
				}
			}

			if (State[Sport.Entities.Team.TypeName] == null)
			{
				Columns = new int[] { (int) ColumnTitles.PlayerNumber, 
						(int) ColumnTitles.Team, (int) ColumnTitles.PlayerName, 
						(int) ColumnTitles.IdNumber, (int) ColumnTitles.BirthDate, 
						(int) ColumnTitles.Grade, (int) ColumnTitles.SexType, 
						(int) ColumnTitles.Status };
			}
			else
			{
				Columns = new int[] { (int) ColumnTitles.PlayerNumber, 
						(int) ColumnTitles.PlayerName, (int) ColumnTitles.IdNumber, 
						(int) ColumnTitles.BirthDate, (int) ColumnTitles.Grade, 
						(int) ColumnTitles.SexType, (int) ColumnTitles.Status };
			}

			Details = new int[] { (int) ColumnTitles.Status, (int) ColumnTitles.Remarks, 
									(int) ColumnTitles.RegisterDate};

			//Filters:
			//region filter
			EntityType regionType = Sport.Entities.Region.Type;
			Entity[] regions = regionType.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, null, "<בחר מחוז>", 100);
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);

			//sport filter
			EntityType sportType = Sport.Entities.Sport.Type;
			Entity[] sports = sportType.GetEntities(null);
			sportFilter = new ComboBoxFilter("ענף ספורט: ", sports, null, "<כל הענפים>", 200);
			sportFilter.FilterChanged += new System.EventHandler(SportFiltered);

			//championship filter
			EntityType champType = Sport.Entities.Championship.Type;
			EntityFilter filter = new EntityFilter();
			Entity[] champs = champType.GetEntities(filter);
			champFilter = new ComboBoxFilter("אליפות: ", champs, null, "<בחר אליפות>", 180);
			champFilter.FilterChanged += new System.EventHandler(ChampionshipFiltered);

			//category filter
			categoryFilter = new ComboBoxFilter("קטגורית אליפות: ", null, null, "<בחר קטגוריה>", 150);
			categoryFilter.FilterChanged += new System.EventHandler(CategoryFiltered);

			//team filter
			//EntityType teamType = Sport.Entities.Team.Type;
			Entity[] teams = null; //teamType.GetEntities(null);
			teamFilter = new ComboBoxFilter("קבוצה: ", teams, null, "<בחר קבוצה>", 180);
			teamFilter.FilterChanged += new System.EventHandler(TeamFiltered);

			//add the filters:
			Filters.Add(regionFilter);
			Filters.Add(sportFilter);
			Filters.Add(champFilter);
			Filters.Add(categoryFilter);
			Filters.Add(teamFilter);

			if (SelectedRegion == null)
			{
				regionFilter.Value = new Sport.Entities.Region(Core.UserManager.CurrentUser.UserRegion).Entity;
				RegionFiltered(null, EventArgs.Empty);
			}

			teamFilter.StopEvents = true;
			RefreshTeamFilter();
			RefreshChampFilter();
			teamFilter.StopEvents = false;
			tbbPlayersReport.Enabled = (teamFilter.Value != null);
			ReloadView();

			if ((categoryFilter.Value == null) && (champFilter.Value != null))
			{
				if (teamFilter.Value != null)
				{
					RefreshCategoryFilter();
					categoryFilter.Value = SelectedTeam.Category.Entity;
				}
			}

			_differentSchoolTimer = new Timer();
			_differentSchoolTimer.Interval = 500;
			_differentSchoolTimer.Tick += new EventHandler(DifferentSchoolTimerTick);

			base.Open();

			if (player != null)
				this.Current = player.Entity;
		} //end function Open

		private Sport.Entities.Player[] DifferentSchoolPlayers()
		{
			int teamID = Sport.Common.Tools.CIntDef(State["team"], -1);
			if (teamID < 0)
				return null;

			Sport.Entities.Team team = new Sport.Entities.Team(teamID);
			if (team == null || team.Id < 0 || team.School == null)
				return null;

			int schoolID = team.School.Id;
			State["school"] = schoolID.ToString();

			ArrayList players = new ArrayList();

			for (int row = 0; row < this.EntityListView.Count; row++)
			{
				Entity ent = this.EntityListView[row];
				if (ent != null && IsDifferentSchool(ent.Id))
					players.Add(new Sport.Entities.Player(ent));
			}

			return (Sport.Entities.Player[])
				players.ToArray(typeof(Sport.Entities.Player));
		}

		private Sport.Entities.Player[] OverAgePlayers()
		{
			int teamID = Sport.Common.Tools.CIntDef(State["team"], -1);
			if (teamID < 0)
				return null;

			ArrayList players = new ArrayList();
			for (int row = 0; row < this.EntityListView.Count; row++)
			{
				Sport.Entities.Player curPlayer =
					new Sport.Entities.Player(this.EntityListView[row]);
				if (curPlayer.IsOverAge())
				{
					players.Add(curPlayer);
				}
			}

			return (Sport.Entities.Player[])
				players.ToArray(typeof(Sport.Entities.Player));
		}

		private bool IsDifferentSchool(int playerID)
		{
			if (State["school"] == null)
				return false;

			int schoolID = Int32.Parse(State["school"]);

			Sport.Entities.Player player = null;
			try
			{
				player = new Sport.Entities.Player(playerID);
			}
			catch
			{ }

			if ((player != null) && (player.Id >= 0))
			{
				Sport.Entities.Student student = null;
				try
				{
					student = player.Student;
				}
				catch
				{ }
				if ((student != null) && (student.Id >= 0))
				{
					Sport.Entities.School school = null;
					try
					{
						school = student.School;
					}
					catch
					{ }
					if ((school != null) && (school.Id >= 0))
					{
						if (school.Id != schoolID)
						{
							int curStatus = (int)player.Entity.Fields[
								(int)Sport.Entities.Player.Fields.Status];
							if (curStatus == (int)Sport.Types.PlayerStatusType.Registered)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		#region Filters
		/// <summary>
		/// refresh all filters of this view.
		/// </summary>
		private void RefreshFilters()
		{
			_filter = new EntityFilter();

			Sport.Entities.Region region = this.SelectedRegion;
			Sport.Entities.Sport sport = this.SelectedSport;
			Sport.Entities.Championship championship = this.SelectedChampionship;
			Sport.Entities.ChampionshipCategory category = this.SelectedCategory;
			Sport.Entities.Team team = this.SelectedTeam;

			if (region != null)
			{
				regionFilter.StopEvents = true;
				regionFilter.Value = region.Entity;
				regionFilter.StopEvents = false;
			}

			if (sport != null)
			{
				sportFilter.StopEvents = true;
				sportFilter.Value = sport.Entity;
				sportFilter.StopEvents = false;
			}

			if (championship != null)
			{
				champFilter.StopEvents = true;
				champFilter.Value = championship.Entity;
				champFilter.StopEvents = false;
			}

			if (category != null)
			{
				categoryFilter.StopEvents = true;
				categoryFilter.Value = category.Entity;
				categoryFilter.StopEvents = false;
			}

			if (team != null)
			{
				_filter.Add(new EntityFilterField((int)Sport.Entities.Player.Fields.Team, (int)team.Id));
				teamFilter.StopEvents = true;
				teamFilter.Value = team.Entity;
				teamFilter.StopEvents = false;
				CanInsert = true;
			}
			else
			{
				CanInsert = false;
			}
		}

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
				{
					champFilter.StopEvents = true;
					champFilter.Value = typeChamp.Lookup((int)champ);
					champFilter.StopEvents = false;
				}
			}
			else
			{
				champFilter.SetValues(null);
			}

			//apply null selection in global state:
			if (champFilter.Value == null)
			{
				State[Sport.Entities.Championship.TypeName] = null;
				State[Sport.Entities.ChampionshipCategory.TypeName] = null;
			}
		} //end function RefreshChampFilters

		/// <summary>
		/// refresh only the categories filter using the selected championship
		/// </summary>
		private void RefreshCategoryFilter()
		{
			EntityType typeCategory = Sport.Entities.ChampionshipCategory.Type;
			EntityFilter filter = new EntityFilter();

			Sport.Entities.Championship championship = SelectedChampionship;
			if (championship == null)
			{
				categoryFilter.SetValues(null);
			}
			else
			{
				Sport.Entities.ChampionshipCategory category = SelectedCategory;
				filter.Add(new EntityFilterField((int)Sport.Entities.ChampionshipCategory.Fields.Championship, (int)championship.Id));
				Entity[] categories = typeCategory.GetEntities(filter);
				categoryFilter.SetValues(categories);
				categoryFilter.StopEvents = true;
				categoryFilter.Value = category == null ? null : category.Entity;
				categoryFilter.StopEvents = false;
				if (categoryFilter.Value == null)
				{
					SelectedCategory = null;
				}
			}
		} //end function RefreshCategoryFilter

		/// <summary>
		/// refresh only the teams filter using the selected category
		/// </summary>
		private void RefreshTeamFilter()
		{
			EntityType typeTeam = Sport.Entities.Team.Type;
			EntityFilter filter = new EntityFilter();

			Sport.Entities.ChampionshipCategory category = SelectedCategory;
			if (category == null)
			{
				teamFilter.SetValues(null);
			}
			else
			{
				Sport.Entities.Region region = SelectedRegion;
				if (region == null)
				{
					teamFilter.SetValues(null);
				}
				else
				{
					Sport.Entities.Team team = SelectedTeam;
					filter.Add(new EntityFilterField((int)Sport.Entities.Team.Fields.Category, (int)category.Id));
					//get all teams for the selected sport and put in combo:
					Entity[] teams = typeTeam.GetEntities(filter);
					ArrayList arrTeams = new ArrayList();
					foreach (Entity curTeamEnt in teams)
					{
						Sport.Entities.Team curTeam = new Sport.Entities.Team(curTeamEnt);
						if (curTeam.Championship.Region.Id == region.Id)
							arrTeams.Add(curTeamEnt);
					}
					teamFilter.SetValues((Entity[])arrTeams.ToArray(typeof(Entity)));
					//restore last selected value:
					teamFilter.Value = team == null ? null : team.Entity;

					//apply null selection in global state:
					if (teamFilter.Value == null)
						SelectedTeam = null;
				}
			}
		} //end function RefreshTeamFilter
		#endregion

		#region Filters Change
		/// <summary>
		/// called when the teams filter (combo box) is changed.
		/// </summary>
		private void TeamFiltered(object sender, EventArgs e)
		{
			tbbPlayersReport.Enabled = (teamFilter.Value != null);

			string identify = Sport.Entities.Team.TypeName;
			State[identify] = Core.Tools.GetFilterValue(teamFilter.Value);
			ReloadView();
		}


		/// <summary>
		/// called when the chamionship filter (combo box) is changed.
		/// </summary>
		private void ChampionshipFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.Championship.TypeName;
			State[identify] = Core.Tools.GetFilterValue(champFilter.Value);
			State[Sport.Entities.ChampionshipCategory.TypeName] = null;
			//apply championship change in teams:
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
			State[Sport.Entities.Team.TypeName] = null;
			//apply championship change in teams:
			RefreshTeamFilter();
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
			State[Sport.Entities.Team.TypeName] = null;
			//apply sport change in championships and teams:
			RefreshChampFilter();
			RefreshTeamFilter();
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
			State[Sport.Entities.Championship.TypeName] = null;
			State[Sport.Entities.Team.TypeName] = null;
			//apply region change in cities and schools as well:
			RefreshChampFilter();
			RefreshTeamFilter();
			ReloadView();
		}
		#endregion

		#region Inherited Methods

		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.Player player = new Sport.Entities.Player(entity);

			if (player.Team.Championship.Sport.SportType == Sport.Types.SportType.Competition)
			{
				Sport.Championships.Championship championship = Sport.Championships.Championship.GetChampionship(player.Team.Category.Id);
				if (championship != null)
				{
					//search the phases groups for the team:
					List<string> championships = new List<string>();
					foreach (Sport.Championships.Phase phase in championship.Phases)
					{
						foreach (Sport.Championships.CompetitionGroup group in phase.Groups)
						{
							foreach (Sport.Championships.Competition competition in group.Competitions)
							{
								foreach (Sport.Championships.Competitor competitor in competition.Competitors)
								{
									if ((competitor.Player != null) && (competitor.Player.PlayerEntity != null))
									{
										if (player.Equals(competitor.Player.PlayerEntity))
										{
											championships.Add(competition.Name);
											break;
										}
									}
								}
							}
						}
					}

					if (championships.Count > 0)
					{
						string message = string.Format("השחקן '{0}' רשום לתחרויות:\n{1}\nיש להסיר את השחקן מתחרויות אלו",
							player.Name, string.Join("\n", championships));
						Sport.UI.MessageBox.Warn(message, "מחיקת שחקן");
						return false;
					}
				}
			}

			return Sport.UI.MessageBox.Ask("האם למחוק את השחקן '" + player.Name + "'?", "מחיקת שחקנים", false);
		}

		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			List<MenuItem> menuItems = new List<MenuItem>();
			switch (selectionType)
			{
				case (SelectionType.Single):
					menuItems.Add(new MenuItem("פתח", new System.EventHandler(PlayerDetailsClick)));
					menuItems.Add(new MenuItem("-"));
					menuItems.Add(new MenuItem("קביעת מין שחקן", new System.EventHandler(PlayerChangeSexClick)));
					Entity entity = this.Current;
					if (entity != null && IsDifferentSchool(entity.Id))
						menuItems.Add(new MenuItem("שינוי בית ספר", new System.EventHandler(ChangeSchoolClick)));
					menuItems.Add(new MenuItem("-"));
					menuItems.Add(new MenuItem("יש כרטיס שחקן", new System.EventHandler(BatchGotSticker)));
					menuItems.Add(new MenuItem("-"));
					menuItems.Add(new MenuItem("אין כרטיס שחקן", new System.EventHandler(BatchNoSticker)));
					break;
				case SelectionType.Multiple:
					menuItems.Add(new MenuItem("יש כרטיסי שחקן", new System.EventHandler(BatchGotSticker)));
					menuItems.Add(new MenuItem("-"));
					menuItems.Add(new MenuItem("אין כרטיסי שחקן", new System.EventHandler(BatchNoSticker)));
					break;
			}

			if (menuItems.Count > 0)
				menuItems[0].DefaultItem = true;
			return menuItems.Count == 0 ? null : menuItems.ToArray();
		}

		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityField entField;

			//change team value to the selected team in the filter:
			if ((teamFilter != null) && (teamFilter.Value != null))
			{
				//change the team field value:
				entField = EntityListView.EntityType.Fields[(int)Sport.Entities.Player.Fields.Team];
				entField.SetValue(entityEdit, ((Entity)teamFilter.Value));
			}

			//change Date to the current date:
			entField = EntityListView.EntityType.Fields[(int)Sport.Entities.Player.Fields.RegisterDate];
			entField.SetValue(EntityListView.EntityEdit, DateTime.Now);

			//set status of new player to Confirmed:
			entField = EntityListView.EntityType.Fields[(int)Sport.Entities.Player.Fields.Status];
			entField.SetValue(entityEdit, (object)((int)Sport.Types.PlayerStatusType.Confirmed));

			entField = EntityListView.EntityType.Fields[(int)Sport.Entities.Player.Fields.Got_Sticker];
			entField.SetValue(entityEdit, 0);
		}

		protected override void NewEntity()
		{
			if (teamFilter == null || teamFilter.Value == null)
			{
				Sport.UI.MessageBox.Error("יש לבחור קבוצה", "רישום שחקנים");
				return;
			}

			Sport.Entities.Team team = new Sport.Entities.Team((Entity)teamFilter.Value);
			int minPlayerNumber = team.PlayerNumberFrom;
			int maxPlayerNumber = team.PlayerNumberTo;

			bool blnCompetition = (team.Championship.Sport.SportType == Sport.Types.SportType.Competition);
			List<int> arrPlayerNumbers = new List<int>();
			int curShirtNumber = -1;
			if (blnCompetition)
			{
				arrPlayerNumbers.AddRange(team.GetPlayers().ToList().ConvertAll(p => p.Number).Distinct().ToList());
				curShirtNumber = Sport.Common.Tools.GetAvailableNumber(minPlayerNumber, maxPlayerNumber, arrPlayerNumbers);
				if (curShirtNumber < minPlayerNumber)
					curShirtNumber = -1;
			}

			Sport.Entities.School school = team.School;
			if (school != null)
			{
				playerDialog.View.State[Sport.Entities.School.TypeName] = school.Id.ToString();
				Sport.Entities.City city = school.City;
				if (city != null)
					playerDialog.View.State[Sport.Entities.City.TypeName] = city.Id.ToString();
				Sport.Entities.Region region = school.Region;
				if (region != null)
					playerDialog.View.State[Sport.Entities.Region.TypeName] = region.Id.ToString();
			}
			Sport.Entities.ChampionshipCategory champCategory = team.Category;
			if (champCategory != null)
				playerDialog.View.State["category"] = champCategory.Category.ToString();
			playerDialog.View.State["team"] = this.State[Sport.Entities.Team.TypeName];

			playerDialog.Multiple = true;
			playerDialog.Entities = null;

			if (playerDialog.ShowDialog() == DialogResult.OK)
			{
				List<string> failedPlayers = new List<string>();
				playerDialog.Entities.ToList().ForEach(studentEnt =>
				{
					PlayersTableView.AddPlayer(studentEnt.Id, curShirtNumber, team);
					if (curShirtNumber >= 0 && curShirtNumber >= minPlayerNumber)
					{
						arrPlayerNumbers.Add(curShirtNumber);
						curShirtNumber = Sport.Common.Tools.GetAvailableNumber(minPlayerNumber, maxPlayerNumber, arrPlayerNumbers);
						if (curShirtNumber < minPlayerNumber)
							curShirtNumber = -1;
					}
				});

				if (failedPlayers.Count > 0)
				{
					Sport.UI.MessageBox.Warn("השחקנים הבאים לא נוספו מאחר ואין מספיק מספרי חזה בטווח של בית הספר:\n" +
						string.Join("\n", failedPlayers) + "\n" + "יש להגדיל את טווח המספרים כדי לרשום שחקנים נוספים",
						"רישום שחקנים");
				}
			}

			playerDialog.Multiple = false;
		}

		public static string AddPlayer(int studentID, int shirtNumber,
			Sport.Entities.Team team)
		{
			//create student entity:
			Sport.Entities.Student student = new Sport.Entities.Student(studentID);

			//generate new player:
			Sport.Entities.Player player = new Sport.Entities.Player(Sport.Entities.Player.Type.New());

			//assign team and student:
			player.Team = team;
			player.Student = student;

			//assign status and register date:
			player.Status = (int)Sport.Types.PlayerStatusType.Confirmed;
			player.RegisterDate = DateTime.Now;

			//assign shirt number:
			player.Number = shirtNumber;

			//assign got sticker:
			player.GotSticker = 0;

			//save...
			EntityResult result = player.EntityEdit.Save();

			//success?
			if (result != EntityResult.Ok)
			{
				return "שגיאה בעת שמירת השחקן '" + student.FirstName + " " + student.LastName + "':\n" +
					result.GetMessage() + "\n\n";
			}

			Sport.Championships.CompetitionGroup.ResetPlayerNumbers(team.Id);

			//done.
			return "";
		} //end function AddPlayer

		protected override void OnSelectEntity(Entity entity)
		{
			string regionID = "";
			string cityID = "";
			string schoolID = "";
			string studentID = "";
			string category = "";
			EntityListView.Fields[(int)Sport.Entities.Player.Fields.Remarks].CanEdit = true;
			Sport.Entities.Player player = null;

			_differentSchoolTimer.Enabled = false;

			//try to get real entity:
			if (entity != null)
			{
				player = new Sport.Entities.Player(entity);
				if (player != null)
				{
					if (IsDifferentSchool(entity.Id))
					{
						_differentSchoolName = "-לא מוגדר בית ספר-";
						if ((player.Student != null) && (player.Student.School != null))
							_differentSchoolName = player.Student.School.Name;
						_differentSchoolPlayer = player.Name;
						_differentSchoolTimer.Enabled = true;
					}
				}
			}

			//get region and school. try from selected entity, if non selected get from the selected filter.
			if ((player != null) && (player.Student != null))
			{
				//read the student, region, city and category:
				studentID = player.Student.Id.ToString();
				if (player.Student.School != null)
				{
					schoolID = player.Student.School.Id.ToString();
					if (player.Student.School.Region != null)
						regionID = player.Student.School.Region.Id.ToString();
					if (player.Student.School.City != null)
						cityID = player.Student.School.City.Id.ToString();
				}
				if ((player.Team != null) && (player.Team.Category != null))
					category = player.Team.Category.Category.ToString();
			}
			else
			{
				if ((teamFilter != null) && (teamFilter.Value != null))
				{
					Sport.Entities.Team team = null;
					try
					{
						team = new Sport.Entities.Team((Entity)teamFilter.Value);
					}
					catch
					{
					}
					if (team != null)
					{
						if (team.School != null)
						{
							schoolID = team.School.Id.ToString();
							if (team.School.Region != null)
								regionID = team.School.Region.Id.ToString();
							if (team.School.City != null)
								cityID = team.School.City.Id.ToString();
						}
						if (team.Category != null)
							category = team.Category.Category.ToString();
					}
				}
			}

			//change the school selection dialog filters according to the 
			//region and city of the selected team or selected filter.
			playerDialog.View.State[Sport.Entities.Region.TypeName] = (regionID.Length > 0) ? regionID : null;
			playerDialog.View.State[Sport.Entities.City.TypeName] = (cityID.Length > 0) ? cityID : null;
			playerDialog.View.State[Sport.Entities.School.TypeName] = (schoolID.Length > 0) ? schoolID : null;
			playerDialog.View.State[Sport.Entities.Student.TypeName] = (studentID.Length > 0) ? studentID : null;
			playerDialog.View.State["category"] = (category.Length > 0) ? category : null;
			playerDialog.View.State["team"] = this.State[Sport.Entities.Team.TypeName];
		}

		protected override void OnSaveEntity(Entity entity)
		{
			base.OnSaveEntity(entity);

			//student card has been changed?
			if (oldGotStudentCard != newGotStudentCard)
			{
				if (oldGotStudentCard != 1 && newGotStudentCard == 1)
					StudentCardAdded(entity.Id);
				else if (oldGotStudentCard == 1 && newGotStudentCard != 1)
					StudentCardRemoved(entity.Id);
				oldGotStudentCard = -1;
				newGotStudentCard = -1;
			}
		}

		protected override void OnValueChange(EntityEdit entityEdit, EntityField entityField)
		{
			//check if player status changed to Not Confirmed:
			Sport.Entities.Player newPlayer = new Sport.Entities.Player(entityEdit);
			Sport.Entities.Player oldPlayer = (entityEdit.Entity == null) ? null : new Sport.Entities.Player(entityEdit.Entity);

			//get status:
			int newStatus = Sport.Common.Tools.CIntDef(newPlayer.Entity.Fields[(int)Sport.Entities.Player.Fields.Status], -1);
			int oldStatus = (oldPlayer == null) ? -1 : Sport.Common.Tools.CIntDef(oldPlayer.Entity.Fields[(int)Sport.Entities.Player.Fields.Status], -1);

			//student card:
			newGotStudentCard = Sport.Common.Tools.CIntDef(newPlayer.Entity.Fields[(int)Sport.Entities.Player.Fields.Got_Sticker], -1);
			oldGotStudentCard = (oldPlayer == null) ? -1 : Sport.Common.Tools.CIntDef(oldPlayer.Entity.Fields[(int)Sport.Entities.Player.Fields.Got_Sticker], -1);

			//get remarks for the new player:
			string remarks = Sport.Common.Tools.CStrDef(newPlayer.Entity.Fields[(int)Sport.Entities.Player.Fields.Remarks], "");

			if (remarks.Length == 0 && newStatus != oldStatus && newStatus == (int)Sport.Types.PlayerStatusType.Not_Confirmed)
				Sport.UI.MessageBox.Warn("אנא ציין את סיבת הדחייה בשדה הערות", "דחיית שחקן");
		}

		protected override Style GetGridStyle(int row, int field, GridDrawState state)
		{
			if (state == GridDrawState.Selected)
				return base.GetGridStyle(row, field, state);

			Entity playerEnt = this.EntityListView[row];
			if (_arrDifferentSchool.IndexOf(playerEnt.Id) >= 0)
				return _differentSchoolStyle;

			Sport.Entities.Player player = new Sport.Entities.Player(playerEnt);
			if (player.IsOverAge())
				return _overAgeStyle;

			return base.GetGridStyle(row, field, state);
		}


		#endregion

		private void BatchGotSticker(object sender, EventArgs e)
		{
			BatchChangeStickerState(true);
		}

		private void BatchNoSticker(object sender, EventArgs e)
		{
			BatchChangeStickerState(false);
		}

		private void BatchChangeStickerState(bool blnGotSticker)
		{
			Entity[] arrSelectedPlayers = this.GetMarkedEntities();
			if (arrSelectedPlayers == null || arrSelectedPlayers.Length == 0)
			{
				Sport.UI.MessageBox.Error("יש לבחור לפחות שחקן אחד", "שינוי כרטיסים גורף");
				return;
			}

			int fieldIndex = (int)Sport.Entities.Player.Fields.Got_Sticker;
			string lookFor = blnGotSticker ? "1" : "0";
			List<Sport.Entities.Player> affectedPlayers = arrSelectedPlayers.ToList().FindAll(e => !((e.Fields[fieldIndex] + "").Equals(lookFor)))
				.ConvertAll(e => new Sport.Entities.Player(e));
			string message = "";
			if (affectedPlayers.Count == 0)
			{
				if (arrSelectedPlayers.Length == 1)
					message = string.Format("ל{0} כבר {1} כרטיס שחקן", arrSelectedPlayers[0].Name, blnGotSticker ? "יש" : "אין");
				else
					message = string.Format("לכל השחקנים הנבחרים כבר {0} כרטיסי שחקן", blnGotSticker ? "יש" : "אין");
				Sport.UI.MessageBox.Error(message, "שינוי כרטיסים גורף");
				return;
			}

			message = string.Format("האם {0} כרטיס שחקן עבור ", blnGotSticker ? "להוסיף" : "להסיר");
			message += (arrSelectedPlayers.Length == 1) ? arrSelectedPlayers[0].Name + "?" :
				"השחקנים הבאים?\n" + string.Join("\n", affectedPlayers.ConvertAll(p => p.Name));
			if (Sport.UI.MessageBox.Ask(message, "שינוי כרטיסים גורף", true) == false)
				return;

			affectedPlayers.ForEach(p =>
			{
				EntityEdit entEdit = p.Entity.Edit();
				entEdit.Fields[(int)Sport.Entities.Player.Fields.Got_Sticker] = blnGotSticker ? 1 : 0;
				entEdit.Save();
				if (blnGotSticker)
					StudentCardAdded(p.Id);
				else
					StudentCardRemoved(p.Id);
			});

			this.Refresh();
		}

		private void PlayerDetailsClick(object sender, EventArgs e)
		{
			OpenDetails(Current);
		}

		private void PlayerChangeSexClick(object sender, EventArgs e)
		{
			if (this.Current == null)
				return;

			Sport.Entities.Player player = new Sport.Entities.Player(this.Current);
			Sport.Entities.Student student = player.Student;

			Sport.UI.Dialogs.GenericEditDialog objDialog =
				new Sport.UI.Dialogs.GenericEditDialog("קביעת מין תלמיד - " + student.FirstName + " " + student.LastName);
			Sport.Types.SexTypeLookup sexLookup = new Sport.Types.SexTypeLookup();
			objDialog.Items.Add(
				new Sport.UI.Controls.GenericItem(
					Sport.UI.Controls.GenericItemType.Selection, sexLookup[(int)student.SexType],
					sexLookup.Items));
			if (objDialog.ShowDialog() == DialogResult.OK)
			{
				LookupItem objSex = (LookupItem)objDialog.Items[0].Value;
				int value = (objSex == null) ? (int)Sport.Types.Sex.None : objSex.Id;
				if (value != (int)student.SexType)
				{
					student.Edit();

					student.SexType = (Sport.Types.Sex)objSex.Id;
					student.Save();
				}
			}
		}

		private void ChangeSchoolClick(object sender, EventArgs e)
		{
			if (this.Current == null)
				return;

			Sport.Entities.Player player = new Sport.Entities.Player(this.Current);
			Sport.Entities.Student student = player.Student;
			Sport.Entities.Team team = new Sport.Entities.Team(Int32.Parse(State["team"]));

			string strCaption = "שינוי בית ספר - " + player.Name;
			bool ans = Sport.UI.MessageBox.Ask("אנא אשר שינוי בית ספר:\n" +
				"בית ספר נוכחי: " + student.School.Name + "\n" +
				"בית ספר חדש: " + team.School.Name + "\n" +
				"לאישור לחץ כן, לביטול לחץ לא", strCaption, true);
			if (ans)
			{
				student.Edit();
				student.School = team.School;
				if (student.Save().Succeeded)
				{
					_arrDifferentSchool.Remove(this.Current.Id);
					Sport.UI.MessageBox.Success("מעבר בית ספר בוצע בהצלחה", strCaption);
					this.EntityListView.RefreshCurrent();
				}
				else
				{
					student.Cancel();
				}
			}
		}

		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			Entity te = (Entity)teamFilter.Value;
			if (te == null)
			{
				Title = "שחקנים";
				EntityListView.Clear();
			}
			else
			{
				Sport.Entities.Team team = new Sport.Entities.Team(te);
				EntityListView.Read(_filter);
				Title = "שחקנים - " +
					team.Championship.Name + " - " +
					team.Category.Name + " - " +
					team.Name;
			}

			//remove invalid players
			if ((EntityListView != null) && (EntityListView.EntityList != null))
			{
				ArrayList arrToRemove = new ArrayList();
				for (int i = 0; i < EntityListView.EntityList.Count; i++)
				{
					Entity curEntity = EntityListView.EntityList[i];
					Sport.Entities.Player curPlayer = new Sport.Entities.Player(curEntity);
					if (!curPlayer.IsValid())
					{
						arrToRemove.Add(curEntity);
					}
				}
				foreach (Entity ent in arrToRemove)
					EntityListView.EntityList.Remove(ent);
			}

			Cursor.Current = c;
		} //end function Requery

		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();

			Sport.Entities.Player[] arrPlayers = DifferentSchoolPlayers();
			if (arrPlayers != null && arrPlayers.Length > 0)
			{
				List<string> names = arrPlayers.ToList().ConvertAll(p => string.Format("{0} {1} ({2})",
					p.Student.FirstName, p.Student.LastName, p.Student.School.Name));
				Sport.UI.MessageBox.Warn("השחקנים הבאים רשומים בבית ספר שונה:\n" + string.Join("\n", names) +
					"\nיש לבדוק תקינות הרישום ולהעביר בית ספר במידת הצורך", "עריכת תלמידים");
			}

			_arrDifferentSchool.Clear();
			if (arrPlayers != null)
			{
				_arrDifferentSchool.AddRange(arrPlayers.ToList().ConvertAll(p => p.Id));
				this.Refresh();
			}

			arrPlayers = OverAgePlayers();
			if (arrPlayers != null && arrPlayers.Length > 0)
			{
				List<string> names = arrPlayers.ToList().ConvertAll(p => string.Format("{0} {1} ({2})",
					p.Student.FirstName, p.Student.LastName, p.Student.BirthDate.ToString("dd/MM/yyyy")));
				Sport.UI.MessageBox.Warn("השחקנים הבאים הם חריגי גיל:\n" + string.Join("\n", names) +
					"\n", "עריכת תלמידים");
			}
		}

		private void DifferentSchoolTimerTick(object sender, EventArgs e)
		{
			_differentSchoolTimer.Enabled = false;
			Sport.UI.MessageBox.Warn("שחקן זה רשום בבית הספר הבא:\n" + _differentSchoolName + "\nאנא בדוק את הרישום ושנה בית ספר במידת הצורך",
				"עריכת נתוני שחקן - " + _differentSchoolPlayer);
		}

		protected override void OpenDetails(Entity entity)
		{
			Sport.Entities.Player player = new Sport.Entities.Player(entity);
			string strExecute = "Sportsman.Details.StudentDetailsView,Sportsman" +
				((entity != null) ? "?id=" + player.Student.Id.ToString() : "");

			new OpenDialogCommand().Execute(strExecute);
		}

		private void PrintPlayersReport()
		{
			//got any team?
			if (teamFilter.Value == null)
				return;

			//read team:
			Sport.Entities.Team team = new Sport.Entities.Team(teamFilter.Value as Entity);

			//print:
			Documents.ChampionshipDocuments champDoc =
				new Documents.ChampionshipDocuments(
				Documents.ChampionshipDocumentType.PlayersReport, team);
			champDoc.Print();
		}

		private void BatchConfirmPlayers()
		{
			Entity[] arrSelectedPlayers = this.GetMarkedEntities();
			if (arrSelectedPlayers == null || arrSelectedPlayers.Length == 0)
			{
				Sport.UI.MessageBox.Error("יש לבחור לפחות שחקן אחד", "אישור שחקנים");
				return;
			}

			string names = "";
			foreach (Entity entPlayer in arrSelectedPlayers)
				if (Sport.Common.Tools.CIntDef(entPlayer.Fields[(int)Sport.Entities.Player.Fields.Status], -1) != (int)Sport.Types.PlayerStatusType.Confirmed)
					names += entPlayer.Name + "\n";
			if (names.Length == 0 || !Sport.UI.MessageBox.Ask("האם לאשר את השחקנים הבאים?\n" + names, "אישור שחקנים", true))
				return;

			foreach (Entity entPlayer in arrSelectedPlayers)
			{
				Sport.Entities.Player curPlayer = new Sport.Entities.Player(entPlayer);
				if (curPlayer.Status == (int)Sport.Types.PlayerStatusType.Confirmed)
					continue;
				EntityEdit entEdit = curPlayer.Entity.Edit();
				entEdit.Fields[(int)Sport.Entities.Player.Fields.Status] = (int)Sport.Types.PlayerStatusType.Confirmed;
				entEdit.Save();
			}
			this.Refresh();
		}

		private void ImportPlayers()
		{
			Sport.Entities.Team team = this.GetTeam();
			if (team == null)
			{
				Sport.UI.MessageBox.Error("יש לבחור קבוצה", "ייבוא שחקנים");
				return;
			}

			string[] arrLines = Tools.ReadFileSelectedByUser();
			if (arrLines == null)
				return;

			if (arrLines.Length < 3)
			{
				Sport.UI.MessageBox.Error("על קובץ הטקסט להכיל לפחות שלוש שורות", "ייבוא שחקנים");
				return;
			}


			List<string[]> arrLineItems = arrLines.ToList().ConvertAll(a => a.Split('\t'));
			int maxTabs = arrLineItems.Max(l => l.Length);
			if (maxTabs < 7)
			{
				Sport.UI.MessageBox.Error("מבנה קובץ טקסט שגוי: לפחות שבעה טאבים דרושים", "ייבוא שחקנים");
				return;
			}

			Dictionary<string, List<string[]>> tblPlayersByTeam = ParsePlayersFile(arrLineItems);
			if (tblPlayersByTeam == null || tblPlayersByTeam.Count == 0)
				return;

			string strCorrectTeamName = team.Name.Replace("מרכז מצוינות ", "");
			string strCorrectKey = "";
			string strClosestMatch = null;
			List<string> arrAllTeams = new List<string>();
			int minDifferentCharacters = 999;
			foreach (string strCurrentTeamName in tblPlayersByTeam.Keys)
			{
				if (strCurrentTeamName == strCorrectTeamName)
				{
					strCorrectKey = strCurrentTeamName;
					break;
				}

				int curDifferentCharacters = Sport.Common.Tools.CountDifferentCharacters(strCurrentTeamName, strCorrectTeamName);
				if (curDifferentCharacters < minDifferentCharacters)
				{
					strClosestMatch = strCurrentTeamName;
					minDifferentCharacters = curDifferentCharacters;
				}
				arrAllTeams.Add(strCurrentTeamName);
			}

			//need to have user select a team?
			if (strCorrectKey.Length == 0)
			{
				GenericItem dropDownItem = new GenericItem("בחר קבוצה מתאימה: ", GenericItemType.Selection,
					strClosestMatch, arrAllTeams.ToArray());
				Sport.UI.Dialogs.GenericEditDialog editDialog = new Sport.UI.Dialogs.GenericEditDialog("בחירת קבוצה",
					new GenericItem[] { dropDownItem });
				if (editDialog.ShowDialog(this) == DialogResult.OK)
					strCorrectKey = (string)editDialog.Items[0].Value;
			}

			//got anything?
			if (strCorrectKey.Length == 0)
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("מעבד נתונים אנא המתן...", true);
			List<string[]> arrTeamLines = tblPlayersByTeam[strCorrectKey];
			List<string> arrImportErrors = new List<string>();
			Dictionary<int, Sport.Entities.Student> tblTeamStudents = GetStudentsFromTeamLines(arrTeamLines, arrImportErrors);
			if (arrImportErrors.Count > 0)
			{
				Sport.UI.MessageBox.Warn(string.Join("\n", arrImportErrors), "ייבוא שחקנים");
			}
			foreach (int playerNumber in tblTeamStudents.Keys)
			{
				Sport.Entities.Student student = tblTeamStudents[playerNumber];
				Sport.Data.EntityFilter filter = new EntityFilter((int)Sport.Entities.Player.Fields.Team, team.Id);
				filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.Player.Fields.Student, student.Id));
				Sport.Data.Entity[] arrEntities = Sport.Entities.Player.Type.GetEntities(filter);
				if (arrEntities == null || arrEntities.Length == 0)
					AddPlayer(student.Id, playerNumber, team);
			}
			Sport.UI.Dialogs.WaitForm.HideWait();
		}

		private Sport.Entities.Team GetTeam()
		{
			if (this.teamFilter != null)
			{
				if (this.teamFilter.Value != null)
				{
					return new Sport.Entities.Team(this.teamFilter.Value as Sport.Data.Entity);
				}
			}
			return null;
		}

		private Entity GetStudentByIdNumber(string strIdNumber)
		{
			EntityFilter filter = new EntityFilter((int)Sport.Entities.Student.Fields.IdNumber, strIdNumber);
			Entity[] arrStudents = null;
			try
			{
				arrStudents = Sport.Entities.Student.Type.GetEntities(filter);
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("overflowed an int column") >= 0)
				{
					string[] arrWords = ex.Message.Split(' ');
					string strIdWithError = "";
					for (int i = 0; i < arrWords.Length; i++)
					{
						if (arrWords[i] == "value" && i < (arrWords.Length - 1))
						{
							strIdWithError = arrWords[i + 1];
							break;
						}
					}
					string strMessage = "";
					if (strIdWithError.Length > 0)
						strMessage = "מספר הזהות " + strIdWithError + " ארוך מידי עבור התוכנה. אנא שנה את המספר ונסה שוב לייבא.";
					else
						strMessage = "קיים מספר זהות ארוך מידי עבור התוכנה. יש לשנות מספר זהות זה ולנסות לייבא שוב.";
					Sport.UI.MessageBox.Error(strMessage, "ייבוא תלמידים");
					arrStudents = null;
				}
				else
				{
					throw;
				}
			}
			return arrStudents != null && arrStudents.Length > 0 ? arrStudents[0] : null;
		}

		private Sport.Entities.Student ParsePlayerDetails(string[] arrPlayerDetails, bool blnBoys, bool blnGirls, 
			List<string> errors, Sport.Entities.Team team, out int playerNumber)
		{
			playerNumber = 0;
			
			int firstNonEmptyIndex = arrPlayerDetails.ToList().FindIndex(d => d != null && d.Length > 0);
			if (firstNonEmptyIndex < 0)
				return null;

			playerNumber = Sport.Common.Tools.CIntDef(arrPlayerDetails[firstNonEmptyIndex], 0);
			if (playerNumber <= 0)
				return null;

			string strLastName = arrPlayerDetails[firstNonEmptyIndex + 1];
			string strFirstName = arrPlayerDetails[firstNonEmptyIndex + 2];
			string strIdNumber = arrPlayerDetails[firstNonEmptyIndex + 3];
			string strBirthDate = arrPlayerDetails[firstNonEmptyIndex + 4];
			string strStudentSex = arrPlayerDetails[firstNonEmptyIndex + 5];

			if (strIdNumber == null || strIdNumber.Length == 0)
				return null;

			if (strIdNumber.Length > 9)
			{
				errors.Add("מספר זהות לא תקין, מעל 9 ספרות: " + strIdNumber);
				return null;
			}

			//proper gender?
			if (!((strStudentSex == "ז" && blnBoys) || (strStudentSex == "נ" && blnGirls)))
				return null;

			Sport.Entities.Student student = null;
			Entity entStudent = GetStudentByIdNumber(strIdNumber);
			Sport.Entities.Student actualStudentData = entStudent == null ? null : new Sport.Entities.Student(entStudent);
			strIdNumber = "1" + strIdNumber;
			Entity existingStudentEntity = GetStudentByIdNumber(strIdNumber);
			int studentId = -1;
			if (existingStudentEntity != null)
			{
				studentId = existingStudentEntity.Id;
			}
			else
			{
				int grade = 0;
				DateTime dtBirthDate = DateTime.MinValue;
				try
				{
					dtBirthDate = Sport.Common.Tools.StringToDate(strBirthDate);
				}
				catch
				{
				}

				if (dtBirthDate.Year < 1900)
				{
					dtBirthDate = new DateTime(1970, 1, 1);
					errors.Add("תאריך לידה לא תקין עבור תלמיד בעל מספר זהות " + strIdNumber.Substring(1) + ", משתמש בתאריך ברירת מחדל");
				}

				Sport.Types.Sex sex = (strStudentSex == "ז") ? (Sport.Types.Sex.Boys) : ((strStudentSex == "נ") ? Sport.Types.Sex.Girls : Sport.Types.Sex.None);
				if (actualStudentData != null)
				{
					strLastName = actualStudentData.LastName;
					strFirstName = actualStudentData.FirstName;
					grade = actualStudentData.Grade;
					if (dtBirthDate.Year < 1900)
						dtBirthDate = actualStudentData.BirthDate;
					if (sex.Equals(Sport.Types.Sex.None))
						sex = actualStudentData.SexType;
				}
				else
				{
					grade = 53;
				}

				student = new Sport.Entities.Student(Sport.Entities.Student.Type.New());
				if (dtBirthDate.Year > 1900)
					student.BirthDate = dtBirthDate;
				student.School = new Sport.Entities.School(team.School.Id);
				student.Grade = grade;
				student.FirstName = strFirstName;
				student.LastName = strLastName;
				student.SexType = sex;
				student.IdNumber = strIdNumber;
				EntityResult entResult = student.EntityEdit.Save();
				if (entResult == EntityResult.Ok)
				{
					studentId = GetStudentByIdNumber(strIdNumber).Id;
				}
				else
				{
					errors.Add("שגיאה בעת שמירת נתוני תלמיד בעל מספר זהות " + strIdNumber.Substring(1) + ": " + entResult.GetMessage());
					return null;
				}
			}

			if (student == null && studentId > 0)
				student = new Sport.Entities.Student(studentId);
			return student;
		}

		private Dictionary<int, Sport.Entities.Student> GetStudentsFromTeamLines(List<string[]> arrTeamLines, List<string> errors)
		{
			errors.Clear();
			Sport.Entities.Team team = new Sport.Entities.Team(teamFilter.Value as Sport.Data.Entity);
			string strCategoryName = team.Category.Name;
			bool blnBoys = (strCategoryName.IndexOf("תלמידים") >= 0);
			bool blnGirls = (strCategoryName.IndexOf("תלמידות") >= 0);
			int playerNumber;
			Dictionary<int, Sport.Entities.Student> tblStudents = new Dictionary<int, Sport.Entities.Student>();
			foreach (string[] arrPlayerDetails in arrTeamLines)
			{
				Sport.Entities.Student student = ParsePlayerDetails(arrPlayerDetails, blnBoys, blnGirls, errors, team, out playerNumber);
				if (student != null && !tblStudents.ContainsKey(playerNumber))
					tblStudents.Add(playerNumber, student);

			}

			return tblStudents;
		}

		private Dictionary<string, List<string[]>> ParsePlayersFile(List<string[]> arrLineItems)
		{
			Dictionary<string, List<string[]>> teamPlayerMapping = new Dictionary<string, List<string[]>>();
			int[] arrTeamLocations = GetTeamLocations(arrLineItems);
			foreach (int index in arrTeamLocations)
			{
				string[] items = arrLineItems[index];
				string strTeamName = GetLastItem(items);
				if (strTeamName != null && !teamPlayerMapping.ContainsKey(strTeamName))
				{
					List<string[]> arrTeamLines = new List<string[]>();
					for (int i = index + 1; i < arrLineItems.Count; i++)
					{
						string[] curLineItems = arrLineItems[i];
						if (Sport.Common.Tools.CountPopulatedItems(curLineItems) < 2)
							break;
						arrTeamLines.Add((string[])curLineItems.Clone());
					}
					teamPlayerMapping.Add(strTeamName, arrTeamLines);
				}
			}
			return teamPlayerMapping;
		}

		private int[] GetTeamLocations(List<string[]> arrLineItems)
		{
			List<int> indices = new List<int>();
			indices.Add(1);
			for (int i = 2; i < arrLineItems.Count; i++)
			{
				string[] items = arrLineItems[i];
				if (Sport.Common.Tools.CountPopulatedItems(items) == 1)
					indices.Add(i + 1);
			}
			return indices.ToArray();
		}

		private string GetLastItem(string[] items)
		{
			for (int i = items.Length - 1; i >= 0; i--)
			{
				string item = items[i];
				if (item != null && item.Trim().Length > 0)
					return item;
			}
			return null;
		}

		private void StudentCardAdded(int id)
		{
			Sport.Entities.Player player = new Sport.Entities.Player(id);
			//service.UpdateIssueDate(player.Student.Id, player.Team.Championship.Sport.Id, DateTime.Now);
			//System.Windows.Forms.MessageBox.Show("added: " + player.Name);
			PlayerCardServices.PlayerCardService service = new PlayerCardServices.PlayerCardService();
			service.CookieContainer = Sport.Core.Session.Cookies;
			int[] students = new int[] { player.Student.Id };
			int sport = player.Team.Championship.Sport.Id;
			try
			{
				service.IssuePlayerCards(students, sport);
			}
			catch (Exception ex)
			{
				Sport.UI.MessageBox.Error("שגיאה בעת עדכון כרטיס שחקן עבור " + player.Name + ":\n" + ex.Message, "שמירת נתונים");
			}
		}

		private void StudentCardRemoved(int id)
		{
			//Sport.Entities.Player player = new Sport.Entities.Player(id);
			//System.Windows.Forms.MessageBox.Show("removed: " + player.Name);
		}
	} //end class PlayersTableView
}
