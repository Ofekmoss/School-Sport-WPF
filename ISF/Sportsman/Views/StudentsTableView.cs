using System;
using System.Linq;
using System.Collections;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using Sport.Data;
using Sport.UI;
using Sport.UI.Controls;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for StudentsTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class StudentsTableView : Sport.UI.TableView
	{
		private System.ComponentModel.IContainer components = null;

		LookupType lookupGrades;

		ComboBoxFilter regionFilter;
		ComboBoxFilter cityFilter;
		ComboBoxFilter schoolFilter;
		ComboBoxFilter gradeFilter;
		ButtonBoxFilter categoryFilter;
		EntityFilter filter;
		EntitySelectionDialog schoolDialog;

		private enum ColumnTitles
		{
			IdNumber = 0,
			LastName,
			FirstName,
			BirthDate,
			Grade,
			LastModified,
			School,
			SchoolSymbol,
			SexType
		}

		public StudentsTableView()
		{
			Items.Add((int)Sport.Entities.Student.Fields.IdNumber, "ת.ז.", 130);
			Items.Add((int)Sport.Entities.Student.Fields.LastName, "שם משפחה", 130);
			Items.Add((int)Sport.Entities.Student.Fields.FirstName, "שם פרטי", 120);
			Items.Add((int)Sport.Entities.Student.Fields.BirthDate, "ת' לידה", 120);
			Items.Add((int)Sport.Entities.Student.Fields.Grade, "כיתה", 80);
			Items.Add((int)Sport.Entities.Student.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int)Sport.Entities.Student.Fields.School, "בית ספר", 180);
			Items.Add((int)Sport.Entities.Student.Fields.SchoolSymbol, "סמל בית ספר", 110);
			Items.Add((int)Sport.Entities.Student.Fields.SexType, "מין", 60);

			// search
			SearchBarEnabled = true;
		}

		public override void Open()
		{
			Title = "תלמידים";

			EntityListView = new EntityListView(Sport.Entities.Student.TypeName);

			SchoolsTableView schoolView = new SchoolsTableView();
			schoolView.State[SelectionDialog] = "1";
			schoolDialog = new EntitySelectionDialog(schoolView);
			EntityListView.Fields[(int)Sport.Entities.Student.Fields.School].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int)Sport.Entities.Student.Fields.School].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new ButtonBox.SelectValue(schoolDialog.ValueSelector));

			if (this.State[SelectionDialog] != "1")
			{
				Columns = new int[] {
										(int) ColumnTitles.IdNumber, (int) ColumnTitles.LastName, 
										(int) ColumnTitles.FirstName, (int) ColumnTitles.BirthDate, 
										(int) ColumnTitles.Grade };
			}
			else
			{
				Columns = new int[] {
										(int) ColumnTitles.IdNumber, (int) ColumnTitles.LastName, 
										(int) ColumnTitles.FirstName, (int) ColumnTitles.BirthDate, 
										(int) ColumnTitles.Grade, (int) ColumnTitles.SexType };
			}

			Details = new int[] {
				(int) ColumnTitles.FirstName, (int) ColumnTitles.LastName, 
				(int) ColumnTitles.School,  (int) ColumnTitles.SchoolSymbol};

			//add search items:
			Searchers.Add(new Searcher("ת.ז.:",
				EntityListView.EntityType.Fields[(int)Sport.Entities.Student.Fields.IdNumber], 100));
			Searchers.Add(new Searcher("שם פרטי:",
				EntityListView.EntityType.Fields[(int)Sport.Entities.Student.Fields.FirstName], 110));
			Searchers.Add(new Searcher("שם משפחה:",
				EntityListView.EntityType.Fields[(int)Sport.Entities.Student.Fields.LastName], 110));

			Sport.Entities.Student student = null;
			if (State["student"] != null)
			{
				try
				{
					student = new Sport.Entities.Student(Int32.Parse(State["student"]));
				}
				catch { }
				if (student != null)
				{
					if (student.School != null)
					{
						State[Sport.Entities.School.TypeName] = student.School.Id.ToString();
						if (student.School.Region != null)
							State[Sport.Entities.Region.TypeName] = student.School.Region.Id.ToString();
						if (student.School.City != null)
							State[Sport.Entities.City.TypeName] = student.School.City.Id.ToString();
					}
				}
				else
				{
					State["student"] = null;
				}
			}

			//filters. add them only if we're not opened in Selection Dialog mode.
			EntityType regionType = Sport.Entities.Region.Type;
			//get all regions:
			Entity[] regions = regionType.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, null, "<בחר מחוז>", 120);
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);

			//empty city filter:
			cityFilter = new ComboBoxFilter("ישוב:", null, null, "<בחר ישוב>", 150);
			cityFilter.FilterChanged += new System.EventHandler(CityFiltered);

			//empty school filter:
			schoolFilter = new ComboBoxFilter("בית ספר:", null, null, "<בחר בית ספר>", 180);
			schoolFilter.FilterChanged += new System.EventHandler(SchoolFiltered);

			//create grades filter. by default, have all grades:
			lookupGrades = new Sport.Types.GradeTypeLookup(true);
			gradeFilter = new ComboBoxFilter("כיתה:", lookupGrades.Items, null, "<כל הכיתות>");
			gradeFilter.FilterChanged += new System.EventHandler(GradeFiltered);

			categoryFilter = new ButtonBoxFilter("קטגוריה:",
				new Sport.UI.Controls.ButtonBox.SelectValue(
				Forms.CategorySelectionDialog.ValueSelector),
				null, "הכל");
			categoryFilter.FilterChanged += new EventHandler(CategoryFiltered);

			//add filters:
			if (this.State[SelectionDialog] != "1")
			{
				Filters.Add(regionFilter);
				Filters.Add(cityFilter);
				Filters.Add(schoolFilter);
				Filters.Add(categoryFilter);
			}
			Filters.Add(gradeFilter);

			RefreshCityFilter();
			RefreshSchoolFilter();
			RefreshFilters();
			Requery();
			if (this.State[SelectionDialog] == "1")
			{
				CategoryFiltered(null, EventArgs.Empty);
			}

			base.Open();

			if (student != null)
				this.Current = student.Entity;

			//remove students if needed...
			if ((State["team"] != null) && (this.EntityListView != null) &&
				(this.EntityListView.EntityList != null))
			{
				Sport.Entities.Team team = new Sport.Entities.Team(Int32.Parse(State["team"]));
				if (team != null)
				{
					Sport.Entities.Player[] arrPlayers = team.Players;
					if (arrPlayers != null)
					{
						foreach (Sport.Entities.Player player in arrPlayers)
						{
							if ((player.Student != null) && (player.Student.Entity != null))
							{
								this.EntityListView.EntityList.Remove(player.Student.Entity);
							}
						}
					}
				}
			}
		}

		protected override void OnSelectEntity(Entity entity)
		{
			if (entity == null)
				return;

			Sport.Entities.Student student = new Sport.Entities.Student(entity);

			if (student.School != null)
			{
				if (student.School.Region != null)
					schoolDialog.View.State[Sport.Entities.Region.TypeName] = student.School.Region.Id.ToString();
				if (student.School.City != null)
					schoolDialog.View.State[Sport.Entities.City.TypeName] = student.School.City.Id.ToString();
				schoolDialog.View.State["school"] = student.School.Id.ToString();
			}
		}


		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			EntityField entField;

			//change school to the selected school in filter:
			if ((schoolFilter != null) && (schoolFilter.Value != null))
			{
				//change the school field value:
				entField = EntityListView.EntityType.Fields[(int)Sport.Entities.Student.Fields.School];
				entField.SetValue(entityEdit, (Entity)schoolFilter.Value);
			}

			//change grade to the selected grade:
			if ((gradeFilter != null) && (gradeFilter.Value != null))
			{
				entField = EntityListView.EntityType.Fields[(int)Sport.Entities.Student.Fields.Grade];
				entField.SetValue(entityEdit, (LookupItem)gradeFilter.Value);
			}
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;
			Sport.Entities.Student student = new Sport.Entities.Student(entity);
			string strMessage = student.CanDelete();
			if (strMessage.Length > 0)
			{
				Sport.UI.MessageBox.Show(strMessage, "מחיקת תלמיד", MessageBoxIcon.Warning);
				return false;
			}

			return Sport.UI.MessageBox.Ask("האם למחוק את התלמיד '" + student.FirstName + " " +
				student.LastName + "'?", "מחיקת תלמיד", false);
		}

		public override MenuItem[] GetContextMenu(Sport.UI.TableView.SelectionType selectionType)
		{
			MenuItem[] menuItems = null;

			Entity student = this.Current;
			if (student == null)
				return null;

			switch (selectionType)
			{
				case (SelectionType.Single):
					ArrayList arrItems = new ArrayList();
					arrItems.Add(new MenuItem("פתח", new System.EventHandler(StudentDetailsClick)));
					arrItems.Add(new MenuItem("-"));
					arrItems.Add(new MenuItem("שינוי בית ספר", new System.EventHandler(ChangeStudentSchool)));
					menuItems = new MenuItem[arrItems.Count];
					for (int i = 0; i < arrItems.Count; i++)
						menuItems[i] = (MenuItem)arrItems[i];
					menuItems[0].DefaultItem = true;
					break;
			}

			return menuItems;
		}

		private void RefreshFilters()
		{
			filter = new EntityFilter();

			object school = Core.Tools.GetStateValue(State[Sport.Entities.School.TypeName]);
			object grade = Core.Tools.GetStateValue(State["grade"]);
			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);
			object city = Core.Tools.GetStateValue(State[Sport.Entities.City.TypeName]);
			object category = Core.Tools.GetStateValue(State["category"]);

			if (school != null)
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.Student.Fields.School, (int)school));
				Core.Tools.SetFilterValue(schoolFilter, Sport.Entities.School.Type.Lookup((int)school));
			}
			if (grade != null)
			{
				object selectedItem = gradeFilter.GenericItem.Values.ToList().Find(o => (o as LookupItem).Id.Equals((int)grade));
				filter.Add(new EntityFilterField((int)Sport.Entities.Student.Fields.Grade, (int)grade));
				Core.Tools.SetFilterValue(gradeFilter, selectedItem); //lookupGrades.Lookup((int)grade)
			}
			if (category != null)
			{
				Core.Tools.SetFilterValue(categoryFilter, Sport.Types.CategoryTypeLookup.ToLookupItem((int)category));
				if (gradeFilter.Value != null)
				{
					//when "all grades" selected, ignore category filter
					filter.Add(new Sport.Types.CategoryFilterField((int)Sport.Entities.Student.Fields.Grade, (int)category,
						Sport.Types.CategoryCompareType.Grade));
				}

			}
			else
			{
				Core.Tools.SetFilterValue(categoryFilter, null);
			}
			if (region != null)
			{
				Core.Tools.SetFilterValue(regionFilter, Sport.Entities.Region.Type.Lookup((int)region));
			}
			if (city != null)
			{
				Core.Tools.SetFilterValue(cityFilter, Sport.Entities.City.Type.Lookup((int)city));
			}
		}

		private void RefreshCityFilter()
		{
			EntityType typeCity = Sport.Entities.City.Type;
			EntityFilter filter = new EntityFilter();

			object city = Core.Tools.GetStateValue(State[Sport.Entities.City.TypeName]);
			object region = Core.Tools.GetStateValue(State[Sport.Entities.Region.TypeName]);

			if (region != null)
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.City.Fields.Region,
					(int)region));
			}

			Entity[] cities = typeCity.GetEntities(filter);

			cityFilter.StopEvents = true;
			cityFilter.SetValues(cities);
			cityFilter.StopEvents = false;

			if (city != null)
			{
				Entity cityEnt = null;
				try
				{
					cityEnt = typeCity.Lookup((int)city);
				}
				catch
				{
				}
				if (cityEnt != null)
					Core.Tools.SetFilterValue(cityFilter, cityEnt);
			}

			if (cityFilter.Value == null)
				State[Sport.Entities.City.TypeName] = null;
		}

		private void RefreshSchoolFilter()
		{
			EntityType typeSchool = Sport.Entities.School.Type;

			EntityFilter filter = new EntityFilter();

			object school = Core.Tools.GetStateValue(State[Sport.Entities.School.TypeName]);
			object city = Core.Tools.GetStateValue(State[Sport.Entities.City.TypeName]);

			if (city != null)
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.School.Fields.City,
					(int)city));

				Entity[] schools = typeSchool.GetEntities(filter);

				schoolFilter.SetValues(schools);
			}
			else
			{
				if (school != null)
				{
					schoolFilter.SetValues(new Sport.Data.Entity[] { typeSchool.Lookup((int)school) });
				}
				else
				{
					schoolFilter.SetValues(null);
				}
			}

			if (school != null)
				Core.Tools.SetFilterValue(schoolFilter, typeSchool.Lookup((int)school));

			if (schoolFilter.Value == null)
				State[Sport.Entities.School.TypeName] = null;
		}

		private void GradeFiltered(object sender, EventArgs e)
		{
			if (gradeFilter.Value == null)
			{
				State["grade"] = null;
			}
			else
			{
				State["grade"] = ((LookupItem)gradeFilter.Value).Id.ToString();
			}
			RefreshFilters();
			Requery();
		}

		private void SchoolFiltered(object sender, EventArgs e)
		{
			ResetGrades();
			if (schoolFilter.Value == null)
			{
				State[Sport.Entities.School.TypeName] = null;
			}
			else
			{
				State[Sport.Entities.School.TypeName] = ((Entity)schoolFilter.Value).Id.ToString();
			}
			RefreshFilters();
			Requery();
		}

		private void CityFiltered(object sender, EventArgs e)
		{
			//State[Sport.Entities.School.TypeName] = null;
			ResetGrades();
			if (cityFilter.Value == null)
			{
				State[Sport.Entities.City.TypeName] = null;
			}
			else
			{
				string cityID = ((Entity)cityFilter.Value).Id.ToString();
				State[Sport.Entities.City.TypeName] = cityID;
				schoolDialog.View.State[Sport.Entities.City.TypeName] = cityID;
			}
			RefreshSchoolFilter();
			RefreshFilters();
			Requery();
		}

		private void RegionFiltered(object sender, EventArgs e)
		{
			ResetGrades();
			if (regionFilter.Value == null)
			{
				State[Sport.Entities.Region.TypeName] = null;
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = null;
			}
			else
			{
				string regionID = ((Entity)regionFilter.Value).Id.ToString();
				State[Sport.Entities.Region.TypeName] = regionID;
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = regionID;
			}
			RefreshCityFilter();
			RefreshSchoolFilter();
			RefreshFilters();
			Requery();
		}

		private void CategoryFiltered(object sender, EventArgs e)
		{
			if (categoryFilter.Value == null || ((LookupItem)categoryFilter.Value).Id == 0)
			{
				State["category"] = null;
				gradeFilter.SetValues(lookupGrades.Items);
			}
			else
			{
				State["category"] = ((LookupItem)categoryFilter.Value).Id.ToString();
				//apply the proper grades for the category:
				LookupItem[] arrGrades = null;
				ArrayList gradesList = new ArrayList();
				for (int i = 0; i < lookupGrades.Items.Length; i++)
				{
					//LookupItem grade=(LookupItem) lookupGrades.Items[i];
					if (Sport.Types.CategoryTypeLookup.Contains((categoryFilter.Value as LookupItem).Id, i))
						gradesList.Add(lookupGrades.Items[i]);
				}
				arrGrades = new LookupItem[gradesList.Count];
				for (int j = 0; j < gradesList.Count; j++)
					arrGrades[j] = (LookupItem)gradesList[j];
				gradeFilter.SetValues(arrGrades);
			}

			RefreshFilters();
			Requery();
		}

		private void ResetGrades()
		{
			//reset grades filter, as well as category lookup filter:
			State["grade"] = null;
			State["category"] = null;
			gradeFilter.SetValues(lookupGrades.Items);
			Core.Tools.SetFilterValue(gradeFilter, null);
			Core.Tools.SetFilterValue(categoryFilter, null);
		}

		private void Requery()
		{
			Cursor c = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			Entity school = (Entity)schoolFilter.Value;
			int grade = (int)(Sport.Common.Tools.CLngDef(State["grade"], -1));

			// Only reading students is at least a school is picked
			if (filter == null ||
				State["school"] == null)
			{
				EntityListView.Clear();
			}
			else
			{
				EntityListView.Read(filter);
			}

			//build page title:
			string title;
			title = (State[SelectionDialog] == "1") ? "בחירת שחקנים" : "תלמידים";
			if (school != null)
			{
				title += " - " + school.Name;
				if (grade >= 0)
				{
					title += " כיתה " + lookupGrades.Lookup(grade);
					if (grade < 10)
						title += "'";
				}
				else
				{
					if ((categoryFilter.Value != null) && ((categoryFilter.Value as LookupItem).Id != 0))
					{
						LookupItem category = (LookupItem)categoryFilter.Value;
						title += " " + category.Text;
					}
				}
			}

			//change view title:
			Title = title;
			Cursor.Current = c;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region context menu events
		private void StudentDetailsClick(object sender, EventArgs e)
		{
			this.OpenDetails(Current);
		}

		private void ChangeStudentSchool(object sender, EventArgs e)
		{
		}

		#endregion

		protected override void NewEntity()
		{
			if ((State["school"] == null) || (State["school"].Length == 0))
				return;
			int schoolID = Int32.Parse(State["school"]);
			Sport.Entities.School school = new Sport.Entities.School(schoolID);
			Forms.AddStudentDialog objDialog = new Forms.AddStudentDialog(school);
			if (objDialog.ShowDialog(this) == DialogResult.OK)
			{
				EntityEdit entEdit = new EntityEdit(Sport.Entities.Student.Type);
				Sport.Entities.Student student = new Sport.Entities.Student(entEdit);
				object[] fields = objDialog.EntityFields;
				for (int i = 0; i < fields.Length; i++)
				{
					student.Entity.Fields[i] = fields[i];
				}

				string errorMsg = "";
				try
				{
					EntityResult er = student.Entity.EntityType.Save(student.Entity as EntityEdit);
					if (!er.Succeeded)
						errorMsg = "כישלון בהוספת שחקן: " + er.GetMessage();
				}
				catch (Exception ex)
				{
					errorMsg = "שגיאה כללית בעת הוספת שחקן: " + ex.ToString();
					
				}

				if (errorMsg.Length > 0)
				{
					Sport.UI.MessageBox.Error(errorMsg, "הוספת תלמיד");
				}
				else
				{
					Current = student.Entity;
				}
			}
		}

		protected override void OpenDetails(Entity entity)
		{
			string strExecute = "Sportsman.Details.StudentDetailsView,Sportsman" +
				((entity != null) ? "?id=" + entity.Id.ToString() : "");

			new OpenDialogCommand().Execute(strExecute);
		}

	}
}
