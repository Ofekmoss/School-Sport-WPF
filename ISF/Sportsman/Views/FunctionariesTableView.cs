using System;
using Sport.Data;
using Sport.UI;
using Sport.UI.Controls;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;


namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for FunctionariesTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class FunctionariesTableView : Sport.UI.TableView
	{
		private Sport.UI.EntitySelectionDialog _sportDialog = null;
		private Sport.UI.EntitySelectionDialog schoolDialog;
		private Sport.UI.TableView.GridDetailItem _gdiFunctionarySports = null;

		private ToolBarButton tbbRefereePayment = null;
		private ToolBarButton tbbStickers = null;

		#region States
		public int SelectedType
		{
			get
			{
				return State["type"] == null ? -1 : (int)Core.Tools.GetStateValue(State["type"]);
			}
			set
			{
				if (value == -1)
				{
					State["type"] = null;
				}
				else
				{
					State["type"] = value.ToString();
				}
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

		#endregion

		#region Filters

		private ComboBoxFilter typeFilter;
		private ComboBoxFilter regionFilter;
		private ComboBoxFilter cityFilter;
		private EntityFilter filter;

		private void ResetEntityFilter()
		{
			filter = new EntityFilter();

			int type = SelectedType;

			if (type != -1)
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.Functionary.Fields.Type, type));
			}

			if (SelectedRegion != null)
			{
				filter.Add(new EntityFilterField((int)Sport.Entities.Functionary.Fields.Region, SelectedRegion.Id));

				if (SelectedCity != null)
				{
					filter.Add(new EntityFilterField((int)Sport.Entities.Functionary.Fields.City, SelectedCity.Id));
				}
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

		public FunctionariesTableView()
		{
			Items.Add((int)Sport.Entities.Functionary.Fields.Name, "שם", 180);
			Items.Add((int)Sport.Entities.Functionary.Fields.Region, "מחוז", 80);
			Items.Add((int)Sport.Entities.Functionary.Fields.City, "ישוב", 100);
			Items.Add((int)Sport.Entities.Functionary.Fields.School, "בית ספר", 130);
			Items.Add((int)Sport.Entities.Functionary.Fields.Address, "כתובת", 150);
			Items.Add((int)Sport.Entities.Functionary.Fields.Phone, "טלפון", 80);
			Items.Add((int)Sport.Entities.Functionary.Fields.Fax, "פקס", 80);
			Items.Add((int)Sport.Entities.School.Fields.LastModified, "תאריך שינוי אחרון", 120);
			Items.Add((int)Sport.Entities.Functionary.Fields.Type, "סוג", 120);
			Items.Add((int)Sport.Entities.Functionary.Fields.ZipCode, "מיקוד", 80);
			Items.Add((int)Sport.Entities.Functionary.Fields.Email, "אימייל", 120);
			Items.Add((int)Sport.Entities.Functionary.Fields.CellPhone, "טלפון נייד", 120);
			Items.Add((int)Sport.Entities.Functionary.Fields.Number, "מספר", 80);
			Items.Add((int)Sport.Entities.Functionary.Fields.ID_Number, "תעודת זהות", 130);
			Items.Add((int)Sport.Entities.Functionary.Fields.Status, "מעמד", 80);
			Items.Add((int)Sport.Entities.Functionary.Fields.HasAnotherJob, "מקום עבודה נוסף", 120);
			Items.Add((int)Sport.Entities.Functionary.Fields.WorkEnviroment, "מסגרת עבודה", 120);
			Items.Add((int)Sport.Entities.Functionary.Fields.SexType, "מין", 60);
			Items.Add((int)Sport.Entities.Functionary.Fields.BirthDate, "תאריך לידה", 130);
			Items.Add((int)Sport.Entities.Functionary.Fields.Seniority, "ותק", 60);
			Items.Add((int)Sport.Entities.Functionary.Fields.Payment, "תשלום", 150);
			Items.Add((int)Sport.Entities.Functionary.Fields.Remarks, "הערות", 150);
			Items.Add((int)Sport.Entities.Functionary.Fields.GotSticker, "מדבקה", 90);

			_sportDialog = new EntitySelectionDialog(new SportsTableView());
			_sportDialog.View.State[SelectionDialog] = "1";

			_gdiFunctionarySports = new GridDetailItem("ענפי התמחות:",
				Sport.Entities.FunctionarySport.Type,
				(int)Sport.Entities.FunctionarySport.Fields.Functionary,
				new System.Drawing.Size(250, 120));
			_gdiFunctionarySports.Columns.Add((int)Sport.Entities.FunctionarySport.Fields.Sport, "ענף ספורט", 250);
			EntityListView.Field field = _gdiFunctionarySports.EntityListView.Fields[
				(int)Sport.Entities.FunctionarySport.Fields.Sport];

			field.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			field.Values = Sport.UI.Controls.GenericItem.ButtonValues(
				new ButtonBox.SelectValue(_sportDialog.ValueSelector));

			Items.Add("ענפי התמחות", _gdiFunctionarySports);

			// search
			SearchBarEnabled = true;

			//
			// toolBar
			//
			tbbRefereePayment = new ToolBarButton();
			tbbRefereePayment.ImageIndex = (int)Sport.Resources.ColorImages.Receipt;
			tbbRefereePayment.Text = "דו\"ח תשלום לשופט";
			tbbRefereePayment.Enabled = false;

			tbbStickers = new ToolBarButton();
			tbbStickers.Text = "מדבקות";

			toolBar.Buttons.Add(tbbRefereePayment);
			toolBar.Buttons.Add(tbbStickers);
			toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(ToolBarButtonClicked);
		}

		public override void Open()
		{
			EntityListView = new Sport.UI.EntityListView(Sport.Entities.Functionary.TypeName);

			EntityListView.Fields[(int)Sport.Entities.Functionary.Fields.Region].Values =
				Sport.Entities.Region.Type.GetEntities(null);

			SchoolsTableView schoolView = new SchoolsTableView();
			schoolView.State[SelectionDialog] = "1";
			schoolDialog = new Sport.UI.EntitySelectionDialog(schoolView);
			EntityListView.Fields[(int)Sport.Entities.Functionary.Fields.School].GenericItemType =
				Sport.UI.Controls.GenericItemType.Button;
			EntityListView.Fields[(int)Sport.Entities.Functionary.Fields.School].Values =
				Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(schoolDialog.ValueSelector));

			List<int> columns = new List<int>(new int[] { 1, 0, 3, 4, 5 });
			if (State[SelectionDialog] == "1")
			{
				Sort = new int[] { int.MinValue + 22 };
			}
			else
			{
				columns.Add(8);
				Details = new int[] { 9, 10 };
			}
			Columns = columns.ToArray();

			Searchers.Add(new Searcher("שם:", EntityListView.EntityType.Fields[(int)Sport.Entities.Functionary.Fields.Name], 150));
			Searchers.Add(new Searcher("מספר:", EntityListView.EntityType.Fields[(int)Sport.Entities.Functionary.Fields.Number], 80));

			typeFilter = new ComboBoxFilter("סוג:", new Sport.Types.FunctionaryTypeLookup().Items, null, "<הכל>");
			typeFilter.FilterChanged += new EventHandler(TypeFiltered);
			EntityType regionType = Sport.Entities.Region.Type;
			Entity[] regions = regionType.GetEntities(null);
			regionFilter = new ComboBoxFilter("מחוז:", regions, SelectedRegion == null ? null : SelectedRegion.Entity, "<כל המחוזות>");
			regionFilter.FilterChanged += new System.EventHandler(RegionFiltered);
			cityFilter = new ComboBoxFilter("ישוב:", null, null, "<כל הישובים>");
			cityFilter.FilterChanged += new System.EventHandler(CityFiltered);

			Filters.Add(regionFilter);
			Filters.Add(cityFilter);

			if (State[SelectionDialog] != "1")
				Filters.Add(typeFilter);

			/*
			if (State[SelectionDialog] == "1")
			{
				//this.EntityListView.Sort = new int[] { (int)Sport.Entities.Functionary.Fields.GotSticker };
				this.EntityListView.EntityList.SortFields = new EntitySortField[] {
					new EntitySortField(new EntityField(Sport.Entities.Functionary.Type, (int)Sport.Entities.Functionary.Fields.GotSticker), 
						EntitySortDirection.Descending)
				};
			}
			*/

			RefreshCityFilter();

			Requery();

			base.Open();
		}

		protected override bool OnDeleteEntity(Entity entity)
		{
			if (entity == null)
				return false;

			Sport.Entities.Functionary func = new Sport.Entities.Functionary(entity);

			string strMessage = func.CanDelete();
			if (strMessage.Length > 0)
			{
				Sport.UI.MessageBox.Show(strMessage, "מחיקת בעל תפקיד", MessageBoxIcon.Warning);
				return false;
			}

			return Sport.UI.MessageBox.Ask("האם למחוק את ה" + (
				new Sport.Types.FunctionaryTypeLookup()).Lookup(
				(int)func.Entity.Fields[(int)Sport.Entities.Functionary.Fields.Type]) + " " +
				func.Name + "?", false);
		}

		Style _gotStickerStyle = new Style(
			new System.Drawing.SolidBrush(System.Drawing.Color.LightGreen), null, null);

		protected override Style GetGridStyle(int row, int field, GridDrawState state)
		{
			if (State[SelectionDialog] == "1")
			{
				Entity funcEnt = this.EntityListView[row];
				if (Sport.Common.Tools.CIntDef(funcEnt.Fields[(int)Sport.Entities.Functionary.Fields.GotSticker], 0) == 1)
					return _gotStickerStyle;
			}

			return base.GetGridStyle(row, field, state);
		}


		private void TypeFiltered(object sender, EventArgs e)
		{
			if (typeFilter.Value == null)
			{
				SelectedType = -1;
			}
			else
			{
				SelectedType = ((LookupItem)typeFilter.Value).Id;
			}

			Requery();
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
				schoolDialog.View.State[Sport.Entities.City.TypeName] = ((Sport.Entities.City)cityFilter.Value).Id.ToString();
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
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = ((Entity)regionFilter.Value).Id.ToString();
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
			string title = "בעלי תפקידים";
			if (SelectedRegion != null)
			{
				title += " - " + SelectedRegion.Name;
				if (SelectedCity != null)
					title += " - " + SelectedCity.Name;
			}

			//change view title:
			Title = title;
			Cursor.Current = c;
		}

		protected override void OnNewEntity(EntityEdit entityEdit)
		{
			Sport.Entities.Functionary functionary = new Sport.Entities.Functionary(entityEdit);
			functionary.Region = SelectedRegion;
			functionary.City = SelectedCity;
		}

		protected override void OnSelectEntity(Entity entity)
		{
			_gdiFunctionarySports.Editable = (entity != null);
			tbbRefereePayment.Enabled = false;

			if (entity == null)
				return;

			Sport.Entities.Functionary functionary = new Sport.Entities.Functionary(entity);

			if (functionary.Region != null)
				schoolDialog.View.State[Sport.Entities.Region.TypeName] = functionary.Region.Id.ToString();
			if (functionary.City != null)
				schoolDialog.View.State[Sport.Entities.City.TypeName] = functionary.City.Id.ToString();
			if (functionary.School != null)
				schoolDialog.View.State["school"] = functionary.School.Id.ToString();

			if (functionary.FunctionaryType == Sport.Types.FunctionaryType.Referee)
				tbbRefereePayment.Enabled = true;
		}

		private void ToolBarButtonClicked(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			if (e.Button == tbbRefereePayment)
				PrintRefereePayments();
			else if (e.Button == tbbStickers)
				PrintFunctionaryStickers();

		}

		private void PrintFunctionaryStickers()
		{
			FunctionariesTableView funcView = new FunctionariesTableView();
			funcView.State[SelectionDialog] = "1";
			Sport.UI.EntitySelectionDialog funcSelection = new Sport.UI.EntitySelectionDialog(funcView);
			funcSelection.Multiple = true;
			if (funcSelection.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Sport.Data.Entity[] entities = funcSelection.Entities;
				if (entities.Length > 0)
				{
					//make functionaries array
					Sport.Entities.Functionary[] arrFuncs = new Sport.Entities.Functionary[entities.Length];
					for (int i = 0; i < entities.Length; i++)
					{
						arrFuncs[i] = new Sport.Entities.Functionary(entities[i]);
					}

					System.Drawing.Printing.PrinterSettings ps;
					Sport.UI.Dialogs.PrintSettingsForm settingsForm;
					if (!Sport.UI.Helpers.GetPrinterSettings(out ps, out settingsForm))
					{
						return;
					}

					//initialize form:
					settingsForm.Landscape = false;

					//ask user to choose the settings:
					if (settingsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						//user confirmed settings, initialize document builder:
						Sport.Documents.DocumentBuilder db = CreateStickersDocument(ps, arrFuncs);

						//show preview if user asked:
						if (settingsForm.ShowPreview)
						{
							//build the preview, let user cancel:
							Sport.UI.Dialogs.PrintForm printForm = new Sport.UI.Dialogs.PrintForm(db.CreateDocument(), ps);

							//show the preview if not canceled:
							if (!printForm.Canceled)
								printForm.ShowDialog();

							//done with preview.
							printForm.Dispose();
						}
						else
						{
							//user don't want preview, print directly:
							System.Drawing.Printing.PrintDocument pd = db.CreateDocument().CreatePrintDocument(ps);
							pd.PrintController = new PrintControllerWithPageForm(pd.PrintController, 0);
							pd.Print();
						}

						foreach (Sport.Data.Entity entity in entities)
						{
							Sport.Data.EntityEdit entEdit = entity.Edit();
							entEdit.Fields[(int)Sport.Entities.Functionary.Fields.GotSticker] = 1;
							entEdit.Save();
						}
					}
				}
			}
		}

		private Sport.Documents.DocumentBuilder CreateStickersDocument(
			System.Drawing.Printing.PrinterSettings settings, Sport.Entities.Functionary[] functionaries)
		{
			//sanity check:
			if (functionaries == null || functionaries.Length == 0)
				return null;

			//set wait cursor:
			Cursor cur = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			//initialize print document:
			Sport.Documents.DocumentBuilder db = new Sport.Documents.DocumentBuilder("מדבקות בעלי תפקידים");
			db.SetSettings(settings);

			//get document size:
			System.Drawing.Size size = db.DefaultPageSize;

			//set printing direction and font:
			db.Direction = Sport.Documents.Direction.Right;
			db.Font = new System.Drawing.Font("Tahoma", 12, System.Drawing.GraphicsUnit.Pixel);

			//initialize section:
			Sport.Documents.Section section = new Sport.Documents.Section();

			//initialize stickers object:
			Producer.PlayerStickersObject pso = new Producer.PlayerStickersObject(new System.Drawing.Rectangle(8, 8, size.Width - 8, size.Height - 8));

			//add stickers:
			foreach (Sport.Entities.Functionary functionary in functionaries)
				pso.Stickers.Add(new Producer.PlayerStickersObject.Sticker(functionary));

			//add the stickers to the section:
			section.Items.Add(pso);

			//add section to the document:
			db.Sections.Add(section);

			//restore cursor:
			Cursor.Current = cur;

			//done.
			return db;
		} //end function CreateStickersDocument

		private void PrintRefereePayments()
		{
			//get selected referee:
			Entity entFunc = this.Current;

			//got anything?
			if (entFunc == null)
				return;

			//get functionary:
			Sport.Entities.Functionary functionary =
				new Sport.Entities.Functionary(entFunc);

			//referee?
			if (functionary.FunctionaryType != Sport.Types.FunctionaryType.Referee)
				return;

			//get championships for this referee:
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתונים אנא המתן...", true);
			Sport.Entities.ChampionshipCategory[] arrCategories =
				functionary.GetChampionships();
			Sport.UI.Dialogs.WaitForm.HideWait();

			//got anything?
			if (arrCategories == null || arrCategories.Length == 0)
			{
				Sport.UI.MessageBox.Error("שופט זה לא משתתף במשחקים כלשהם", "דו\"ח תשלום לשופט");
				return;
			}

			//input date range.
			Sport.UI.Dialogs.GenericEditDialog objDialog =
				new Sport.UI.Dialogs.GenericEditDialog("בחירת טווח תאריכים");
			objDialog.Items.Add("תאריך התחלה",
				Sport.UI.Controls.GenericItemType.DateTime, DateTime.Now,
				GenericItem.DateTimeValues("dd/MM/yyyy"));
			objDialog.Items.Add("תאריך סיום",
				Sport.UI.Controls.GenericItemType.DateTime, DateTime.Now,
				GenericItem.DateTimeValues("dd/MM/yyyy"));
			DateTime dtStart = DateTime.MinValue;
			DateTime dtEnd = DateTime.MinValue;
			if (objDialog.ShowDialog(this) == DialogResult.OK)
			{
				if (objDialog.Items[0].Value != null)
					dtStart = (DateTime)objDialog.Items[0].Value;
				if (objDialog.Items[1].Value != null)
					dtEnd = (DateTime)objDialog.Items[1].Value;
			}

			//valid?
			if ((dtStart.Year < 1900) || (dtStart > dtEnd))
			{
				if (dtStart > dtEnd)
					Sport.UI.MessageBox.Error("תאריך התחלה לא יכול להיות גדול מתאריך סיום", "דו\"ח תשלום לשופט");
				return;
			}

			//fix dates:
			dtStart = Sport.Common.Tools.SetTime(dtStart, 0, 0, 0);
			dtEnd = Sport.Common.Tools.SetTime(dtEnd, 23, 59, 59);

			//get matches data:
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען משחקים אנא המתן...", true);
			Sport.Entities.DataServices.FunctionaryMatchData[] arrMatchData =
				functionary.GetMatchData(dtStart, dtEnd);
			Sport.UI.Dialogs.WaitForm.HideWait();

			//got anything?
			if ((arrMatchData == null) || (arrMatchData.Length == 0))
			{
				Sport.UI.MessageBox.Error("אין משחקים עבור שופט זה בטווח תאריכים נבחר",
					"דו\"ח תשלום לשופט");
				return;
			}

			//load championships.
			ArrayList arrMatches = new ArrayList();
			Hashtable tblChampionships = new Hashtable();
			Sport.UI.Dialogs.WaitForm.ShowWait("טוען נתוני אליפות אנא המתן...", true);
			int noFacilityCount = 0;
			ArrayList arrMatchCategories = new ArrayList();
			ArrayList arrMatchFacilities = new ArrayList();
			foreach (Sport.Entities.DataServices.FunctionaryMatchData data in arrMatchData)
			{
				int categoryID = data.ChampionshipCategoryID;
				if (tblChampionships[categoryID] == null)
					tblChampionships[categoryID] = Sport.Championships.Championship.GetChampionship(categoryID);
				Sport.Championships.MatchChampionship champ =
					(Sport.Championships.MatchChampionship)tblChampionships[categoryID];
				Sport.Championships.MatchPhase phase = champ.Phases[data.Phase];
				Sport.Championships.MatchGroup group = phase.Groups[data.Group];
				Sport.Championships.Round round = (group == null) ? null : group.Rounds[data.Round];
				Sport.Championships.Cycle cycle = (round == null) ? null : round.Cycles[data.Cycle];
				Sport.Championships.Match match = (cycle == null) ? null : cycle.Matches[data.Match];
				Sport.Entities.Facility facility = (match == null) ? null : match.Facility;
				if (facility != null)
				{
					arrMatches.Add(match);
					if (arrMatchFacilities.IndexOf(facility) < 0)
						arrMatchFacilities.Add(facility);
					Sport.Entities.ChampionshipCategory category =
						match.Cycle.Round.Group.Phase.Championship.ChampionshipCategory;
					if (arrMatchCategories.IndexOf(category) < 0)
						arrMatchCategories.Add(category);
				}
				else
				{
					noFacilityCount++;
				}
			}

			arrMatches.Sort(new MatchComparer());
			Sport.UI.Dialogs.WaitForm.HideWait();

			//show alert?
			if (noFacilityCount > 0)
			{
				Sport.UI.MessageBox.Warn("אזהרה: ב" +
					Sport.Common.Tools.BuildOneOrMany("משחק", "משחקים",
					noFacilityCount, true) + " לא מוגדר מתקן", "דו\"ח תשלום לשופט");
			}

			//got anything?
			if (arrMatches.Count == 0)
			{
				Sport.UI.MessageBox.Error("במשחקים בטווח התאריכים הנבחר לא מוגדרים מתקנים",
					"דו\"ח תשלום לשופט");
				return;
			}

			//let user choose rates.
			objDialog = new Sport.UI.Dialogs.GenericEditDialog("קביעת תעריפים");
			objDialog.Width = 350;
			ArrayList arrItemEntities = new ArrayList();
			int[] arrOriginalRates = new int[arrMatchCategories.Count + arrMatchFacilities.Count];
			foreach (Sport.Entities.ChampionshipCategory curCategory in arrMatchCategories)
			{
				string strCategoryName = curCategory.Name;
				string caption = "תעריף משחק " +
					Sport.Common.Tools.GetOnlyGrades(strCategoryName) + " " +
					Sport.Common.Tools.GetOnlySex(strCategoryName);
				int rate = curCategory.GameRate;
				GenericItem item = new GenericItem(caption, GenericItemType.Number,
					rate, new object[] { 0d, 1000d }, new System.Drawing.Size(50, 0));
				objDialog.Items.Add(item);
				arrOriginalRates[arrItemEntities.Count] = rate;
				arrItemEntities.Add(curCategory);
			}
			foreach (Sport.Entities.Facility curFacility in arrMatchFacilities)
			{
				string strFacilityName = curFacility.Name;
				string caption = "תעריף נסיעות " + strFacilityName;
				Sport.Entities.City city = null;
				if (curFacility.City != null)
					city = curFacility.City;
				else if (curFacility.School != null)
					city = curFacility.School.City;
				if ((city != null) && (caption.IndexOf(city.Name) < 0))
					caption += " " + city.Name;
				int rate = curFacility.GetTripRate(functionary);
				GenericItem item = new GenericItem(caption, GenericItemType.Number,
					rate, new object[] { 0d, 500d }, new System.Drawing.Size(50, 0));
				objDialog.Items.Add(item);
				arrOriginalRates[arrItemEntities.Count] = rate;
				arrItemEntities.Add(curFacility);
			}

			//show:
			if (objDialog.ShowDialog(this) == DialogResult.OK)
			{
				//update:
				for (int i = 0; i < arrItemEntities.Count; i++)
				{
					int rate = (int)((double)objDialog.Items[i].Value);
					if (rate == arrOriginalRates[i])
						continue;
					object oEntity = arrItemEntities[i];
					if (oEntity is Sport.Entities.ChampionshipCategory)
						(oEntity as Sport.Entities.ChampionshipCategory).GameRate = rate;
					else if (oEntity is Sport.Entities.Facility)
						(oEntity as Sport.Entities.Facility).SetTripRate(functionary, rate);
				}

				//print:
				Documents.ChampionshipDocuments champDoc = new Documents.ChampionshipDocuments(
					Documents.ChampionshipDocumentType.RefereePaymentReport,
					(Sport.Championships.Match[])arrMatches.ToArray(typeof(Sport.Championships.Match)));
				champDoc.Print();
			}
		}

		private class MatchComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Sport.Championships.Match m1 = (Sport.Championships.Match)x;
				Sport.Championships.Match m2 = (Sport.Championships.Match)y;
				DateTime t1 = m1.Time;
				DateTime t2 = m2.Time;
				Sport.Entities.Facility f1 = m1.Facility;
				Sport.Entities.Facility f2 = m2.Facility;
				if (!Sport.Common.Tools.IsSameDate(t1, t2))
					return t1.CompareTo(t2);
				if ((f1 == null) && (f2 == null))
					return t1.CompareTo(t2);
				if (f1 == null)
					return 1;
				if (f2 == null)
					return -1;
				if (f1.Id != f2.Id)
					return f1.Name.CompareTo(f2.Name);
				return t1.CompareTo(t2);
			}
		}
	}
}