using System;
using System.Linq;
using System.Collections;
using Sport.UI;
using Sport.Data;
using System.Windows.Forms;
using Sportsman.Core;
using Sport.UI.Controls;
using Sport.Types;
using Sportsman.PermissionServices;
using System.Collections.Generic;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for ChampionshipsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Championships)]
	public class ChampionshipsTableView : TableView
	{
		private Sport.UI.TableView.ComboBoxFilter	regionFilter;
		private Sport.UI.TableView.ComboBoxFilter	sportFilter;
		private EntityFilter	filter;
		private EntitySelectionDialog userDialog;
		private Sport.UI.TableView.GridDetailItem gdiCategories;
		private GridDetailItem gdiRegions;
		private ToolBarButton tbbReports;
		private Forms.ChooseAdminReportType dlgChooseAdminReportType = new Forms.ChooseAdminReportType();
	
		private enum ColumnTitles
		{
			Number=0,
			Region,
			Sport,
			Name,
			LastRegisterDate,
			ChampionshipDates,
			FinalDates,
			IsClubs,
			Status,
			Type,
			Ruleset,
			Supervisor,
			Season,
			LastModified,
			Categories,
			Regions,
			Remarks
		}	
		
		private void RefreshFilters()
		{
			if (filter == null)
				filter = new EntityFilter();
			filter.Clear();
			
			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);
			object sport = Core.Tools.GetStateValue(State[Sport.Entities.Sport.TypeName]);
			
			if (region != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Championship.Fields.Region, (int) region));
				regionFilter.Value = Sport.Entities.Region.Type.Lookup((int) region);
			}
			if (sport != null)
			{
				filter.Add(new EntityFilterField((int) Sport.Entities.Championship.Fields.Sport, 
					(int) sport));
				sportFilter.Value = Sport.Entities.Sport.Type.Lookup((int) sport);
			}
		}

		public ChampionshipsTableView()
		{
			// Creating Categories detail grid
			gdiCategories = new GridDetailItem("קטגוריות:", 
				Sport.Entities.ChampionshipCategory.Type, 
				(int) Sport.Entities.ChampionshipCategory.Fields.Championship, 
				new System.Drawing.Size(520, 120));
			gdiCategories.Columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.Index, "#", 50);
			gdiCategories.Columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.Category, "קטגוריה", 180);
			gdiCategories.Columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.RegistrationPrice, "תעריף רישום", 150);
			gdiCategories.Columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.Status, "מצב", 100);
			gdiCategories.Columns.Add((int) Sport.Entities.ChampionshipCategory.Fields.MaxStudentBirthday, "חריג גיל", 120);
			EntityListView.Field field = gdiCategories.EntityListView.Fields[
				(int) Sport.Entities.ChampionshipCategory.Fields.Category];
			field.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			field.Values = Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(Forms.CategorySelectionDialog.ValueSelector));
			gdiCategories.EntityListView.Fields[(int) Sport.Entities.ChampionshipCategory.Fields.Status].CanEdit = false;
			
			gdiCategories.AutoInsert = false;
			gdiCategories.NewClick += new EventHandler(NewCategoryClicked);
			
			gdiCategories.AutoDelete = false;
			gdiCategories.DeleteClick += new EventHandler(gdiCategories_DeleteClick);
			
			gdiCategories.EntityListView.EntityChanged += new EntityChangeEventHandler(CategoryChanged);
			gdiCategories.EntityListView.ValueChanged += new FieldEditEventHandler(CategoryValueChanged);
			gdiCategories.IndexChanged += new IndexChangeEventHandler(gdiCategories_IndexChanged);
			
			gdiCategories.Grid.MouseUp += new MouseEventHandler(Categories_Grid_Mouseup);
			
			// Creating Regions detail grid
			gdiRegions = new GridDetailItem("מחוזות:", 
				Sport.Entities.ChampionshipRegion.Type, 
				(int) Sport.Entities.ChampionshipRegion.Fields.Championship, 
				new System.Drawing.Size(150, 120));
			gdiRegions.Columns.Add((int) Sport.Entities.ChampionshipRegion.Fields.Region, "מחוז", 200);
			gdiRegions.EntityListView.Fields[(int) Sport.Entities.ChampionshipRegion.Fields.Region].CanEdit = false;
			
			gdiRegions.AutoInsert = false;
			gdiRegions.NewClick += new EventHandler(NewRegionClicked);
			
			Items.Add((int) Sport.Entities.Championship.Fields.Number, "מספר אליפות", 100);
			Items.Add((int) Sport.Entities.Championship.Fields.Region, "מחוז", 100);
			Items.Add((int) Sport.Entities.Championship.Fields.Sport, "ענף", 100);
			Items.Add((int) Sport.Entities.Championship.Fields.Name, "שם", 200);
			Items.Add((int) Sport.Entities.Championship.Fields.LastRegistrationDate, "סיום רישום", 120);
			Items.Add((int) Sport.Entities.Championship.Fields.ChampionshipDates, "מועדי אליפות", 120);
			Items.Add((int) Sport.Entities.Championship.Fields.FinalsDates, "מועד גמר", 120);
			Items.Add((int) Sport.Entities.Championship.Fields.IsClubs, "מועדון?", 80);
			Items.Add((int) Sport.Entities.Championship.Fields.Status, "סטטוס", 100);
			Items.Add((int) Sport.Entities.Championship.Fields.IsOpen, "סוג אליפות", 100);
			Items.Add((int) Sport.Entities.Championship.Fields.Ruleset, "תקנון", 200);
			Items.Add((int) Sport.Entities.Championship.Fields.Supervisor, "אחראי", 150);
			Items.Add((int) Sport.Entities.Championship.Fields.Season, "עונה", 150);
			Items.Add((int) Sport.Entities.Championship.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add("קטגוריות", gdiCategories);
			Items.Add("מחוזות", gdiRegions);
			Items.Add((int) Sport.Entities.Championship.Fields.Remarks, "הערות", 220);
			
			//
			// toolBar
			//
			tbbReports = new ToolBarButton();
			tbbReports.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbReports.Text = "דו\"חות";
			tbbReports.Enabled = true;

			toolBar.Buttons.Add(tbbReports);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);
			
			// search (not working)
			//SearchBarEnabled = true;
		}
		
		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbReports)
			{
				Forms.ChooseChampionshipsReportDialog dialog = new Forms.ChooseChampionshipsReportDialog();
				Entity[] selectedChamps = this.GetMarkedEntities();
				if (selectedChamps == null || selectedChamps.Length == 0)
					dialog.DisableRadioButton(Documents.ChampionshipDocumentType.AdministrationReport, "יש לבחור לפחות אליפות אחת");
				if (dialog.ShowDialog(this) == DialogResult.OK)
				{
					Documents.ChampionshipDocumentType? selectedReport;
					if (dialog.TryGetSelectedReport(out selectedReport))
					{
						switch (selectedReport.Value)
						{
							case Documents.ChampionshipDocumentType.ClubReport:
								PrintClubReport(dialog.MinCategoryCellWidthOverride);
								break;
							case Documents.ChampionshipDocumentType.OtherSportsReport:
								PrintOtherSportsReport(dialog.MinCategoryCellWidthOverride);
								break;
							case Documents.ChampionshipDocumentType.AdministrationReport:
								PrintAdministrationReport(selectedChamps);
								break;
						}
					}
				}
			}
		}
		
		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.Championship.TypeName);

			userDialog = new EntitySelectionDialog(new UsersTableView());
			userDialog.View.State[SelectionDialog] = "1";
			userDialog.View.State["UserType"] = ((int) Sport.Types.UserType.Internal).ToString();
			
			//rulesetDialog = new EntitySelectionDialog(new RulesetsTableView());
			//rulesetDialog.View.State[SelectionDialog] = "1";
			
			// Setting sport field
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Sport].CanEdit = false;
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Sport].Values =
				Sport.Entities.Sport.Type.GetEntities(null);

			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Season].CanEdit = false;
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Season].Values =
				Sport.Entities.Season.Type.GetEntities(null);
			
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Region].CanEdit = false;
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Region].Values =
				Sport.Entities.Region.Type.GetEntities(null);

			Fields[(int) Sport.Entities.Championship.Fields.Supervisor].GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			Fields[(int) Sport.Entities.Championship.Fields.Supervisor].Values = Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(userDialog.ValueSelector));
			
			//Fields[(int) Sport.Entities.Championship.Fields.Ruleset].GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			//Fields[(int) Sport.Entities.Championship.Fields.Supervisor].Values = Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(userDialog.ValueSelector));
			
			//rulesetDialog.View.State[SelectionDialog] = "1";
			
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.ChampionshipDates].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.ChampionshipDates].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(ChampionshipsDatesEdit));

			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.FinalsDates].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int) Sport.Entities.Championship.Fields.FinalsDates].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(FinalsDatesEdit));


			Columns = new int[]
			{
				(int) ColumnTitles.Number, (int) ColumnTitles.Region, 
				(int) ColumnTitles.Sport, (int) ColumnTitles.Name, 
				(int) ColumnTitles.IsClubs, (int) ColumnTitles.Status, 
				(int) ColumnTitles.Type, (int) ColumnTitles.Ruleset,
				(int) ColumnTitles.Remarks
			};
			
			Details = new int[]
			{
				(int) ColumnTitles.Name, (int) ColumnTitles.Number, 
				(int) ColumnTitles.LastRegisterDate, (int) ColumnTitles.ChampionshipDates, 
				(int) ColumnTitles.FinalDates, (int) ColumnTitles.Supervisor, 
				(int) ColumnTitles.Categories
			};
			
			Sport.Entities.Championship championship=null;
			if (State["championship"] != null)
			{
				try
				{
					championship = new Sport.Entities.Championship(Int32.Parse(State["championship"]));
				}
				catch {}
				if (championship != null)
				{
					State[Sport.Entities.Region.TypeName] = championship.Region.Id.ToString();
					State[Sport.Entities.Sport.TypeName] = championship.Sport.Id.ToString();
				}
				else
				{
					State["championship"] = null;
				}
			}
			// Creating filters

			EntityType regionType = Sport.Entities.Region.Type;
			Entity[] regions = regionType.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, null, "<כל המחוזות>");
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);
			Filters.Add(regionFilter);

			Entity[] sports = Sport.Entities.Sport.Type.GetEntities(null);
			sportFilter = new ComboBoxFilter("ענף ספורט:", sports, null, "<כל הענפים>", 200);
			sportFilter.FilterChanged += new System.EventHandler(SportFiltered);
			Filters.Add(sportFilter);
			
			//MessageBox.Show(UserManager.CurrentUser.Name);
			
			Requery();
			base.Open();

			if (championship != null)
				this.Current = championship.Entity;
			
			if (State[SelectionDialog] == "1")
				this.Editable = false;
		}

		protected override void OnSelectEntity(Entity entity)
		{
			this.EntityListView.ShowWaitDialog = false;
			if (entity != null)
			{
				Sport.Entities.Championship championship = new Sport.Entities.Championship(entity);

				if (gdiCategories.EntityListView.Count > 0)
				{
					int indexField=(int) Sport.Entities.ChampionshipCategory.Fields.Index;
					ArrayList arrEntityEdits=new ArrayList();
					for (int i=0; i<gdiCategories.EntityListView.Count; i++)
					{
						Entity curCatEnt=gdiCategories.EntityListView[i];
						int curIndex=Sport.Common.Tools.CIntDef(
							curCatEnt.Fields[indexField], -1);
						if (curIndex < 0)
						{
							EntityEdit entEdit=curCatEnt.Edit();
							entEdit.Fields[indexField] = i;
							arrEntityEdits.Add(entEdit);
						}
					}
					foreach (EntityEdit ee in arrEntityEdits)
						ee.Save();
					gdiCategories.EntityListView.Sort = new int[] {
						(int) Sport.Entities.ChampionshipCategory.Fields.Index};
					gdiCategories.EntityListView.CurrentIndex = 0;
				}
				
				//set region and school for user selection dialog:
				Sport.Entities.User supervisor = championship.Supervisor;
				if (supervisor != null)
				{
					userDialog.View.State[Sport.Entities.Region.TypeName] = supervisor.Region.Id.ToString();
					if (supervisor.School != null)
						userDialog.View.State[Sport.Entities.School.TypeName] = supervisor.School.Id.ToString();
					else
						userDialog.View.State[Sport.Entities.School.TypeName] = null;
				}
				else
				{
					userDialog.View.State[Sport.Entities.Region.TypeName] = championship.Region.Id.ToString();
					userDialog.View.State[Sport.Entities.School.TypeName] = null;
				}

				// Disable name for standard championships
				EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Name].CanEdit =
					championship.StandardChampionship == null;
				
				// Set possible patterns of categories according to 
				// championship sport type:
				Sport.Entities.Sport sport = championship.Sport;

				EntityListView.Fields[(int) Sport.Entities.Championship.Fields.Ruleset].Values =
					sport.GetRulesets();
			}
			else
			{
				userDialog.View.State[Sport.Entities.Region.TypeName] = null;
				userDialog.View.State[Sport.Entities.School.TypeName] = null;
			}
			
			this.EntityListView.ShowWaitDialog = true;
			
			base.OnSelectEntity (entity);
		}

		// Overriding new to manualy insert new championship
		private Sport.UI.Dialogs.GenericEditDialog gedCreateChampionship = null;
		private Sport.Entities.Region createChampionshipRegion = null;
		private static readonly string sOther = "אחר";
		private void CreateChampionshipSportRegionChanged(object sender, EventArgs e)
		{
			object[] championships = null;
			if (gedCreateChampionship.Items[0].Value != null)
			{
				Sport.Data.Entity sport = gedCreateChampionship.Items[0].Value as Sport.Data.Entity;
				int regional = createChampionshipRegion.Id == Sport.Entities.Region.CentralRegion ? 0 : 1;
				Sport.Data.EntityFilter filter = new EntityFilter();
				filter.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.StandardChampionship.Fields.Sport, sport.Id));
				filter.Add(new Sport.Data.EntityFilterField((int) Sport.Entities.StandardChampionship.Fields.IsRegional, regional));
				Sport.Data.Entity[] standard = Sport.Entities.StandardChampionship.Type.GetEntities(filter);
				championships = new object[standard.Length + 1];
				Array.Copy(standard, championships, standard.Length);
				championships[standard.Length] = sOther;
			}
			gedCreateChampionship.Items[1].Values = championships;
			gedCreateChampionship.Items[1].Enabled = championships != null;
			gedCreateChampionship.Confirmable = gedCreateChampionship.Items[1].Value != null;
		}

		private void CreateChampionshipChampionshipChanged(object sender, EventArgs e)
		{
			gedCreateChampionship.Confirmable = gedCreateChampionship.Items[1].Value != null;
		}

		protected override void NewEntity()
		{
			if (regionFilter.Value == null)
			{
				Sport.UI.MessageBox.Error("יש לבחור מחוז על מנת להוסיף אליפות", "שגיאה");
				return;
			}
			
			if (gedCreateChampionship == null)
			{
				gedCreateChampionship = new Sport.UI.Dialogs.GenericEditDialog("אליפות חדשה");
				gedCreateChampionship.Items.Add("ענף ספורט:", Sport.UI.Controls.GenericItemType.Selection,
					null, Sport.Entities.Sport.Type.GetEntities(null));
				gedCreateChampionship.Items[0].ValueChanged += new EventHandler(CreateChampionshipSportRegionChanged);
				gedCreateChampionship.Items.Add("אליפות:", Sport.UI.Controls.GenericItemType.Selection);
				gedCreateChampionship.Items[1].ValueChanged += new EventHandler(CreateChampionshipChampionshipChanged);
			}

			createChampionshipRegion = new Sport.Entities.Region(regionFilter.Value as Sport.Data.Entity);

			if (createChampionshipRegion.Id == Sport.Entities.Region.CentralRegion)
			{
				gedCreateChampionship.Title = "אליפות ארצית חדשה";
			}
			else
			{
				gedCreateChampionship.Title = "אליפות מחוזית חדשה - " + createChampionshipRegion.Name;
			}
			
			gedCreateChampionship.Items[0].Value = null;
			gedCreateChampionship.Items[1].Enabled = false;
			gedCreateChampionship.Items[1].Value = null;
			gedCreateChampionship.Confirmable = false;

			if (gedCreateChampionship.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Entities.Sport sport = new Sport.Entities.Sport(gedCreateChampionship.Items[0].Value as Sport.Data.Entity);

				Sport.Data.EntityEdit champEdit = Sport.Entities.Championship.Type.New();
				Sport.Entities.Championship championship = new Sport.Entities.Championship(champEdit);
				championship.Season = new Sport.Entities.Season(Sport.Core.Session.Season);
				championship.Region = createChampionshipRegion;
				championship.Sport = sport;
				championship.Status = Sport.Types.ChampionshipType.InitialPlanning;

				object value = gedCreateChampionship.Items[1].Value;
				if (value == (object) sOther)
				{
					championship.Name = "אליפות " + sport.Name;
					championship.IsClubs = false;
					championship.IsOpen = false;
				}
				else if (value is Sport.Data.Entity)
				{
					Sport.Entities.StandardChampionship standard = new Sport.Entities.StandardChampionship(value as Sport.Data.Entity);

					championship.Name = standard.Name;
					championship.IsClubs = standard.IsClubs;
					championship.IsOpen = standard.IsOpen;
					championship.StandardChampionship = standard;
				}

				if (champEdit.Save().Succeeded)
					Current = champEdit;
			}
		}


		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.Championship champ=new Sport.Entities.Championship(entity);
			
			//begin checkings... first check if the championship has any categories:
			Sport.Entities.ChampionshipCategory[] categories=champ.GetCategories();
			string names="";
			for (int i=0; i<categories.Length; i++)
			{
				names += categories[i].Name+"\n";
				if (i >= 15)
				{
					names += "...\n";
					break;
				}
			}
			
			if (categories.Length > 0)
			{
				Sport.UI.MessageBox.Show("האליפות '"+champ.Name+"' מכילה את הקטגוריות הבאות: "+
					"\n"+names+"יש להסיר קטגוריות אלו מהאליפות", 
					"מחיקת אליפות", MessageBoxIcon.Warning);
				return false;
			}
			
			return Sport.UI.MessageBox.Ask("האם למחוק את האליפות '" + champ.Name + "'?", false);
		}
		
		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			MenuItem[] menuItems = null;
			
			Entity champ=Current;
			if (champ == null)
				return null;

			Sport.Entities.Championship championship=
				new Sport.Entities.Championship(champ);
			
			switch (selectionType)
			{
				case (SelectionType.Single):
					ArrayList arrItems=new ArrayList();
					arrItems.Add(new MenuItem("פתח", new System.EventHandler(ChampionshipDetailsClick)));
					arrItems.Add(new MenuItem("-"));
					arrItems.Add(new MenuItem("קבוצות", new System.EventHandler(ChampTeamsClick)));
					if (gdiCategories.EntityListView.CurrentIndex >= 0)
						arrItems.Add(new MenuItem("בניית אליפות וקביעת תוצאות", new System.EventHandler(ChampBuildClick)));
					menuItems = new MenuItem[arrItems.Count];
					for (int i=0; i<arrItems.Count; i++)
						menuItems[i] = (MenuItem) arrItems[i];
					menuItems[0].DefaultItem = true;
					break;
			}
			
			return menuItems;
		}
		
		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityField entityField;
			//double regPrice=0;
			
			if (regionFilter.Value != null)
			{
				Entity region=(Entity) regionFilter.Value;
				entityField = EntityListView.EntityType.Fields[(int) Sport.Entities.Championship.Fields.Region];
				entityField.SetValue(entityEdit, region);

				//set user selection dialog region as well:
				userDialog.View.State[Sport.Entities.Region.TypeName] = region.Id.ToString();
				userDialog.View.State[Sport.Entities.School.TypeName] = null;
			}
			if (sportFilter.Value != null)
			{
				Sport.Entities.Sport sport=
					new Sport.Entities.Sport((Entity) sportFilter.Value);
				
				entityField = EntityListView.EntityType.Fields[(int) Sport.Entities.Championship.Fields.Sport];
				entityField.SetValue(entityEdit, sport.Entity);
			}
			
			entityField = EntityListView.EntityType.Fields[(int) Sport.Entities.Championship.Fields.Supervisor];
			entityField.SetValue(entityEdit, Sport.Entities.User.Type.Lookup(UserManager.CurrentUser.Id));
			
			entityField = EntityListView.EntityType.Fields[(int) Sport.Entities.Championship.Fields.Season];
			entityField.SetValue(entityEdit, Sport.Entities.Season.Type.Lookup(Sport.Core.Session.Season));
		}

		private void SportFiltered(object sender, EventArgs e)
		{
			if (sportFilter.Value == null)
			{
				State[Sport.Entities.Sport.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.Sport.TypeName] = ((Entity)sportFilter.Value).Id.ToString();
			}
			
			Requery();
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			//throw new Exception("testing");
			EntityFilter entitityFilter = null;
			if (regionFilter.Value == null)
			{
				State[Sport.Entities.Region.TypeName] = null;
			}
			else
			{
				int selectedRegion = ((Entity)regionFilter.Value).Id;
				State[Sport.Entities.Region.TypeName] = selectedRegion.ToString();
				if (selectedRegion != Sport.Entities.Region.CentralRegion)
					entitityFilter = new EntityFilter(new EntityFilterField((int)Sport.Entities.Sport.Fields.CentralRegionOnly, 1, true));
			}
			sportFilter.SetValues(Sport.Entities.Sport.Type.GetEntities(entitityFilter));

			Requery();
		}
		
		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			
			RefreshFilters();			
			EntityListView.Read(filter);
			
			//remove championships of different season... (yeah stupid DAT files)
			ArrayList arrToRemove=new ArrayList();
			for (int i=0; i<EntityListView.EntityList.Count; i++)
			{
				Entity ent=EntityListView.EntityList[i];
				try
				{
					Sport.Entities.Championship champ=new Sport.Entities.Championship(ent);
					if (champ.Season.Id != Sport.Core.Session.Season)
						arrToRemove.Add(ent);
				}
				catch {}
			}
			foreach (Entity ent in arrToRemove)
				EntityListView.EntityList.Remove(ent);
			
			Entity region = (Entity) regionFilter.Value;

			if (region == null)
				Title = "תכנית אליפויות";
			else if (region.Id == Sport.Entities.Region.CentralRegion)
				Title = "תכנית אליפויות ארצית";
			else
				Title = "תכנית אליפויות מחוזית - " + region.Name;

			Cursor.Current = c;
		}
		
		private void ChampionshipDetailsClick(object sender, EventArgs e)
		{
			OpenDetails(Current);
		}
		
		private void ChampTeamsClick(object sender, EventArgs e)
		{	
			Entity entity = Current;
			if (entity != null)
			{
				string state = 
					Sport.Entities.Championship.TypeName + "=" + 
					entity.Fields[(int) Sport.Entities.Championship.Fields.Id].ToString();
				if (gdiCategories.Grid.SelectedIndex >= 0)
				{
					if (gdiCategories.EntityListView.Current != null)
					{
						state += "&" + Sport.Entities.ChampionshipCategory.TypeName + "=" + 
							gdiCategories.EntityListView.Current.Id;
					}
				}
				ViewManager.OpenView(typeof(TeamsTableView), state);
			}
		}

		private void ChampBuildClick(object sender, EventArgs e)
		{
			//abort if no category selected:
			if (gdiCategories.EntityListView.CurrentIndex < 0)
				return;

			//get the id of the selected category:
			int categoryID=gdiCategories.EntityListView.Current.Id;
			
			//load the championship from database or memory:
			Sport.Championships.Championship championship=null;
			try
			{
				championship = Sport.Championships.Championship.GetChampionship(categoryID);
			}
			catch (Exception err)
			{
				//delete dat files and try again:
				System.Diagnostics.Debug.WriteLine("failed to load championship: "+err.Message);
				System.Diagnostics.Debug.WriteLine(err.StackTrace);
				System.Diagnostics.Debug.WriteLine("deleting dat files and trying again...");
				Sport.Entities.Sport.Type.DeleteDatFile();
				Sport.Entities.ChampionshipCategory.Type.DeleteDatFile();
				Sport.Entities.Championship.Type.DeleteDatFile();
				Sport.Entities.Ruleset.Type.DeleteDatFile();
				Sport.Entities.StandardChampionship.Type.DeleteDatFile();
				Sport.Entities.StandardChampionshipCategory.Type.DeleteDatFile();
				try
				{
					championship = Sport.Championships.Championship.GetChampionship(categoryID);
				}
				catch (Exception e2)
				{
					System.Diagnostics.Debug.WriteLine("failed to load championship: "+e2.Message);
					Sport.UI.Dialogs.WaitForm.HideWait();
					Sport.UI.MessageBox.Show("טעינת אליפות נכשלה\nאנא דווח על השגיאה במסך הערות תוך פירוט פעולות אחרונות\nשגיאה: "+
						e2.Message, "טעינת אליפות", MessageBoxIcon.Error);
					return;
				}
			}
			Sport.UI.Dialogs.WaitForm.HideWait();

			//getting here means we have valid championship. check type:
			if (championship is Sport.Championships.MatchChampionship)
			{
				ViewManager.OpenView(typeof(Producer.MatchChampionshipEditorView), 
					"championshipcategory=" + categoryID.ToString());
			}
			else
			{
				ViewManager.OpenView(typeof(Producer.CompetitionChampionshipEditorView), 
					"championshipcategory=" + categoryID.ToString());
			}
		}

		private object ChampionshipsDatesEdit(ButtonBox buttonBox, object value)
		{
			DateSetType dst = value is DateSetType ?
				(DateSetType) ((DateSetType) value).Clone() :
				new DateSetType();

			Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("מועדי אליפות");
			ged.Items.Add("פתיחה:", GenericItemType.DateTime, dst.Start, 
				GenericItem.DateTimeValues("dd/MM/yyyy"));
			ged.Items.Add("סיום:", GenericItemType.DateTime, dst.End, 
				GenericItem.DateTimeValues("dd/MM/yyyy"));
			ged.Items.Add("פתיחה חלופי:", GenericItemType.DateTime, dst.AltStart, 
				GenericItem.DateTimeValues("dd/MM/yyyy"));
			ged.Items.Add("סיום חלופי:", GenericItemType.DateTime, dst.AltEnd, 
				GenericItem.DateTimeValues("dd/MM/yyyy"));

			if (ged.ShowDialog() == DialogResult.OK)
			{
				try
				{
					dst.Start = ged.Items[0].Value;
					dst.End = ged.Items[1].Value;
					dst.AltStart = ged.Items[2].Value;
					dst.AltEnd = ged.Items[3].Value;
				}
				catch
				{
					Sport.UI.MessageBox.Error("תאריכים לא חוקיים, תאריך סיום חייב להיות אחרי תאריך התחלה", "שגיאת מערכת");
					return value;
				}

				return dst;
			}

			return value;
		}

		private object FinalsDatesEdit(ButtonBox buttonBox, object value)
		{
			DateSetType dst = value is DateSetType ?
				(DateSetType) ((DateSetType) value).Clone() :
				new DateSetType();

			Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("מועדי גמר");
			ged.Items.Add("מועד:", GenericItemType.DateTime, dst.Start, 
				GenericItem.DateTimeValues("dd/MM/yyyy"));
			ged.Items.Add("מועד חלופי:", GenericItemType.DateTime, dst.AltStart, 
				GenericItem.DateTimeValues("dd/MM/yyyy"));

			if (ged.ShowDialog() == DialogResult.OK)
			{
				dst.Start = ged.Items[0].Value;
				dst.AltStart = ged.Items[1].Value;

				return dst;
			}

			return value;
		}

		private void CategoryChanged(object sender, EntityChangeEventArgs e)
		{
			//check we got any current championship:
			if ((this.Current == null)||(this.Current.Id < 0))
				return;

			//get current entity:
			Sport.Entities.Championship champ=
				new Sport.Entities.Championship(this.Current);
			
			//put default price if new category added:
			if ((e.Index >= 0)&&(e.Entity != null)&&(e.Entity.Id < 0))
				e.Entity.Fields[(int) Sport.Entities.ChampionshipCategory.Fields.RegistrationPrice] = champ.RegisterPrice;
		}

		private void CategoryValueChanged(object sender, FieldEditEventArgs e)
		{
			
		}

		protected override void OpenDetails(Entity entity)
		{
			string strExecute = "Sportsman.Details.ChampionshipDetailsView,Sportsman" +
				((entity != null) ? "?id=" + entity.Id.ToString() : "");

			new OpenDialogCommand().Execute(strExecute);
		}

		private void NewCategoryClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity current = Current;
			if (current != null)
			{
				Sport.Entities.Championship championship = new Sport.Entities.Championship(current);
				int category = 0;
				if (Forms.CategorySelectionDialog.EditCategory(ref category))
				{
					Sport.Entities.ChampionshipCategory cc = new Sport.Entities.ChampionshipCategory(Sport.Entities.ChampionshipCategory.Type.New());
					cc.Championship = championship;
					cc.Category = category;
					cc.RegistrationPrice = championship.RegisterPrice;
					cc.Index = 0;
					cc.Save();
				}
			}
		}

		private void NewRegionClicked(object sender, EventArgs e)
		{
			Sport.Data.Entity current = Current;
			if (current != null)
			{
				Sport.Entities.Championship championship = new Sport.Entities.Championship(current);
				Sport.UI.Dialogs.GenericEditDialog ged = new Sport.UI.Dialogs.GenericEditDialog("בחר מחוז");
				Sport.Data.Entity[] regions = Sport.Entities.Region.Type.GetEntities(
					new EntityFilter(new Sport.Data.EntityFilterField((int) Sport.Entities.Region.Fields.Id, Sport.Entities.Region.CentralRegion, true))
					);
				ged.Items.Add(Sport.UI.Controls.GenericItemType.Selection, null, regions);
				ged.Items[0].Nullable = false;
				ged.Confirmable = false;
				ged.ValueChanged += new EventHandler(RegionsDialogValueChanged);

				if (ged.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Sport.Data.Entity entity = ged.Items[0].Value as Sport.Data.Entity;

					foreach (Sport.Entities.ChampionshipRegion champRegion in championship.GetRegions())
					{
						if (champRegion.Region.Id == entity.Id)
						{
							Sport.UI.MessageBox.Show("מחוז כבר קיים באליפות");
							return ;
						}
					}

					Sport.Entities.ChampionshipRegion cr = new Sport.Entities.ChampionshipRegion(Sport.Entities.ChampionshipRegion.Type.New());
					cr.Championship = championship;
					cr.Region = new Sport.Entities.Region(entity);
					cr.Save();
				}
			}
		}
		
		private void RegionsDialogValueChanged(object sender, EventArgs e)
		{
			Sport.UI.Dialogs.GenericEditDialog ged = sender as Sport.UI.Dialogs.GenericEditDialog;
			ged.Confirmable = ged.Items[0].Value != null;
		}
		
		private void gdiCategories_IndexChanged(object sender, int change)
		{
			//get selected category:
			Entity entity=gdiCategories.EntityListView.Current;
			
			//got anything?
			if ((entity == null)||(entity.Id < 0))
				return;
			
			//get index of the field:
			int indexField=(int) Sport.Entities.ChampionshipCategory.Fields.Index;
			
			//get current index of the category:
			int curIndex=Sport.Common.Tools.CIntDef(
				entity.Fields[indexField], 0);
			
			//get new index:
			int newIndex=(curIndex+change);
			
			//make valid:
			if (newIndex < 0)
				newIndex = 0;
			if (newIndex >= gdiCategories.EntityListView.Count)
				newIndex = gdiCategories.EntityListView.Count-1;
			
			//any change?
			if (newIndex == curIndex)
				return;
			
			//change the index:
			EntityEdit entEdit=entity.Edit();
			entEdit.Fields[indexField] = newIndex;
			
			//save:
			entEdit.Save();
			
			//change other entities as well?
			if ((gdiCategories.EntityListView.Count > 0)&&
				((change == 1)||(change == -1)))
			{
				//find index of changed entity:
				int indexInGrid=-1;
				for (int i=0; i<gdiCategories.EntityListView.Count; i++)
				{
					if (gdiCategories.EntityListView[i].Id == entity.Id)
					{
						indexInGrid = i;
						break;
					}
				}
				
				//get index in grid of category to replace with:
				int newGridIndex=indexInGrid+change;
				if ((newGridIndex < gdiCategories.EntityListView.Count)&&
					(newGridIndex >= 0))
				{
					entEdit = gdiCategories.EntityListView[newGridIndex].Edit();
					entEdit.Fields[indexField] = curIndex;
					entEdit.Save();
				}
			} //end if has more than one categoris
			
			//reload:
			gdiCategories.EntityListView.Sort = new int[] {
				(int) Sport.Entities.ChampionshipCategory.Fields.Index};
			
			//select original entity:
			gdiCategories.EntityListView.Current = entity;
			gdiCategories.EntityListView.RefreshCurrent();
		}

		private void gdiCategories_DeleteClick(object sender, EventArgs e)
		{
			Entity entity=gdiCategories.EntityListView.Current;
			if ((entity == null)||(entity.Id < 0))
				return;
			
			Sport.Entities.ChampionshipCategory category=
				new Sport.Entities.ChampionshipCategory(entity);
			string strMessage=category.CanDelete();
			if (strMessage.Length > 0)
			{
				Sport.UI.MessageBox.Show(strMessage, "מחיקת קטגורית אליפות", MessageBoxIcon.Warning);
				return;
			}
			gdiCategories.EntityListView.Delete();
		}

		private void Categories_Grid_Mouseup(object sender, MouseEventArgs e)
		{
			//right clicked?
			if (e.Button != MouseButtons.Right)
				return;
			
			//got anything selected?
			if (gdiCategories.EntityListView.Current == null)
				return;
			
			//build menu items:
			MenuItem[] menuItems = new MenuItem[]
				{
					new MenuItem("בניית אליפות", new System.EventHandler(ChampCategoryClicked)), 
					new MenuItem("קבוצות", new System.EventHandler(ChampTeamsClick))
				};
			
			//build and show the menu:
			Sport.UI.Controls.RightContextMenu cm=
				new Sport.UI.Controls.RightContextMenu(menuItems);
			cm.RightToLeft = RightToLeft.Yes;
			cm.Show((Control) sender, new System.Drawing.Point(e.X, e.Y));
		}
		
		private void ChampCategoryClicked(object sender, EventArgs e)
		{
			ChampBuildClick(sender, e);
		}
		
		private void PrintClubReport(int minCategoryCellWidth)
		{
			int regionID = (regionFilter.Value == null) ? -1 : (regionFilter.Value as Sport.Data.Entity).Id;
			
			
			Documents.ChampionshipDocuments champDoc=
				new Documents.ChampionshipDocuments(
				Documents.ChampionshipDocumentType.ClubReport, new int[] { regionID, minCategoryCellWidth });
			
			champDoc.Print();
		}

		private void PrintOtherSportsReport(int minCategoryCellWidth)
		{
			int regionID = (regionFilter.Value == null) ? -1 : (regionFilter.Value as Sport.Data.Entity).Id;


			Documents.ChampionshipDocuments champDoc =
				new Documents.ChampionshipDocuments(
				Documents.ChampionshipDocumentType.OtherSportsReport, new int[] { regionID, minCategoryCellWidth });

			champDoc.Print();
		}

		private void PrintAdministrationReport(Entity[] selectedChamps)
		{
			/*
			int regionID = (regionFilter.Value as Sport.Data.Entity).Id;
			
			*/

			Sport.Core.Data.AdministrationReportType reportType = Sport.Core.Data.AdministrationReportType.Undefined;
			if (dlgChooseAdminReportType.ShowDialog(this) == DialogResult.OK)
				reportType = dlgChooseAdminReportType.ReportType;
			switch (reportType)
			{
				case Sport.Core.Data.AdministrationReportType.Personal:
					object[] data = new object[] { reportType, selectedChamps };
					Documents.ChampionshipDocuments champDoc = new Documents.ChampionshipDocuments(Documents.ChampionshipDocumentType.AdministrationReport, data);
					champDoc.Print();
					break;
				case Sport.Core.Data.AdministrationReportType.Team:
					Sport.UI.MessageBox.Show("אפשרות זאת כרגע לא נתמכת", "דו\"ח מנהל הספורט", MessageBoxIcon.Information);
					break;
			}
		}
	}
}
