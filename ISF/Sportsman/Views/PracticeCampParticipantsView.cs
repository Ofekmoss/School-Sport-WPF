using System;
using System.Linq;
using Sport.UI;
using System.Collections.Generic;
using Sportsman.Details;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for PracticeCampParticipantsView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.General)]
	public class PracticeCampParticipantsView : TableView
	{
		private enum ColumnTitles
		{
			PracticeCamp = 0,
			ParticipantName,
			ParticipantAddress,
			ParticipantSchool,
			ParticipantBirthDate,
			ParticipantPhone,
			ParticipantCellPhone,
			ParticipantGender,
			IsConfirmed,
			Remarks,
			Charge,
			ParticipantEmail
		}

		#region States
		public Sport.Entities.Sport SelectedSport
		{
			get
			{
				Sport.Entities.PracticeCamp camp = this.SelectedPracticeCamp;
				if (camp != null)
					return camp.Sport;

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

		public Sport.Entities.PracticeCamp SelectedPracticeCamp
		{
			get
			{
				return State[Sport.Entities.PracticeCamp.TypeName] == null ? null :
					new Sport.Entities.PracticeCamp((int)Core.Tools.GetStateValue(State[Sport.Entities.PracticeCamp.TypeName]));
			}
			set
			{
				string sValue = (value == null) ? null : value.Id.ToString();
				State[Sport.Entities.PracticeCamp.TypeName] = sValue;
			}
		}
		#endregion

		private ComboBoxFilter sportFilter;
		private ButtonBoxFilter practiceCampFilter;
		private Sport.Data.EntityFilter _filter;
		private EntitySelectionDialog practiceCampDialog;

		public PracticeCampParticipantsView()
		{
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.PracticeCamp, "מחנה אימון", 220);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantName, "שם משתתף", 150);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantAddress, "כתובת המשתתף", 150);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantSchool, "שם בית ספר", 120);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantBirthday, "תאריך לידה", 120);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantPhone, "טלפון בבית", 110);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.ParticipantCellPhone, "טלפון נייד", 110);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.SexType, "מין", 90);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.IsConfirmed, "סטטוס רישום", 90);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.Remarks, "הערות", 240, 75);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.Charge, "חיוב", 130, 75);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.Email, "אימייל", 150);
			Items.Add((int)Sport.Entities.PracticeCampParticipant.Fields.LastModified, "תאריך שינוי אחרון", 130);
		}

		public override void Open()
		{
			EntityListView = new EntityListView(Sport.Entities.PracticeCampParticipant.TypeName);

			Columns = new int[] { (int) ColumnTitles.PracticeCamp, 
							(int) ColumnTitles.ParticipantName, (int) ColumnTitles.ParticipantEmail, 
							(int) ColumnTitles.ParticipantAddress, (int) ColumnTitles.ParticipantSchool, 
							(int) ColumnTitles.ParticipantBirthDate, (int) ColumnTitles.ParticipantPhone, 
							(int) ColumnTitles.ParticipantCellPhone, (int) ColumnTitles.ParticipantGender, 
							(int) ColumnTitles.IsConfirmed, (int) ColumnTitles.Charge };

			Details = new int[] { (int)ColumnTitles.Remarks };

			practiceCampDialog = new EntitySelectionDialog(new Views.PracticeCampsView());
			practiceCampDialog.View.State[Sport.UI.View.SelectionDialog] = "1";

			//Filters

			//sport filter
			sportFilter = new ComboBoxFilter("ענף ספורט: ", Sport.Entities.Sport.Type.GetEntities(null), null, "<כל ענפי הספורט>", 200);
			sportFilter.FilterChanged += new System.EventHandler(SportFiltered);

			//practice camp filter
			practiceCampFilter = new ButtonBoxFilter("מחנה אימון: ",
				new Sport.UI.Controls.ButtonBox.SelectValue(practiceCampDialog.ValueSelector), null, "<בחר מחנה אימון>", 290);
				//new ComboBoxFilter("מחנה אימון: ", Sport.Entities.PracticeCamp.Type.GetEntities(null), null, "<בחר מחנה אימון>", 290);
			practiceCampFilter.FilterChanged += new System.EventHandler(PracticeCampFiltered);

			//add the filters:
			Filters.Add(sportFilter);
			Filters.Add(practiceCampFilter);

			Sport.Entities.PracticeCamp camp = this.SelectedPracticeCamp;
			if (camp != null)
			{
				practiceCampFilter.Value = camp.Entity;
				PracticeCampFiltered(null, EventArgs.Empty);
			}

			ReloadView();
			base.Open();

			if (camp != null)
				this.Current = camp.Entity;
		}

		#region Filters
		/// <summary>
		/// refresh all filters of this view.
		/// </summary>
		private void RefreshFilters()
		{
			_filter = new Sport.Data.EntityFilter();

			Sport.Entities.Sport sport = this.SelectedSport;
			Sport.Entities.PracticeCamp camp = this.SelectedPracticeCamp;

			if (sport != null)
			{
				sportFilter.StopEvents = true;
				sportFilter.Value = sport.Entity;
				sportFilter.StopEvents = false;
			}

			if (camp != null)
			{
				//practiceCampFilter.StopEvents = true;
				//practiceCampFilter.Value = camp.Entity;
				//practiceCampFilter.StopEvents = false;

				_filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.PracticeCampParticipant.Fields.PracticeCamp, (int)camp.Id));
			}

		}

		/// <summary>
		/// refresh only the practice camp filter using the selected sport.
		/// </summary>
		private void RefreshPracticeCampFilter()
		{
			

			object sport = Core.Tools.GetStateValue(State[Sport.Entities.Sport.TypeName]);
			if (sport != null)
			{
				/*
				//get all practice camps for the selected sport and put in combo:
				object practiceCamp = Core.Tools.GetStateValue(State[Sport.Entities.PracticeCamp.TypeName]);
				Sport.Data.EntityFilter filter = new Sport.Data.EntityFilter();
				filter.Add(new Sport.Data.EntityFilterField((int)Sport.Entities.PracticeCamp.Fields.Sport, (int)sport));
				practiceCampFilter.SetValues(Sport.Entities.PracticeCamp.Type.GetEntities(filter));

				//restore last selected value:
				if (practiceCamp != null)
				{
					practiceCampFilter.StopEvents = true;
					practiceCampFilter.Value = Sport.Entities.PracticeCamp.Type.Lookup((int)practiceCamp);
					practiceCampFilter.StopEvents = false;
				}
				*/
			}
			else
			{
				//practiceCampFilter.SetValues(null);
				practiceCampFilter.Value = null;
			}

			practiceCampDialog.View.State["sport"] = sport == null ? null : sport.ToString();

			//apply null selection in global state:
			if (practiceCampFilter.Value == null)
				State[Sport.Entities.PracticeCamp.TypeName] = null;
		} //end function RefreshChampFilters
		#endregion

		#region Filters Change
		/// <summary>
		/// called when the sport filter (combo box) is changed.
		/// </summary>
		private void SportFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.Sport.TypeName;
			State[identify] = Core.Tools.GetFilterValue(sportFilter.Value);
			State[Sport.Entities.PracticeCamp.TypeName] = null;
			practiceCampFilter.Value = null;
			//apply sport change in championships and teams:
			RefreshPracticeCampFilter();
			ReloadView();
		}

		/// <summary>
		/// called when the region filter (combo box) is changed.
		/// </summary>
		private void PracticeCampFiltered(object sender, EventArgs e)
		{
			string identify = Sport.Entities.PracticeCamp.TypeName;
			State[identify] = Core.Tools.GetFilterValue(practiceCampFilter.Value);
			ReloadView();
		}
		#endregion

		#region Inherited Methods
		protected override bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			if (entity == null)
				return false;

			Sport.Entities.PracticeCampParticipant participant = new Sport.Entities.PracticeCampParticipant(entity);
			return Sport.UI.MessageBox.Ask("האם למחוק את המשתתף '" + participant.Name + "'?", false);
		}

		protected override void OnNewEntity(Sport.Data.EntityEdit entityEdit)
		{
			Sport.Data.EntityField entField;

			//change team value to the selected team in the filter:
			if ((practiceCampFilter != null) && (practiceCampFilter.Value != null))
			{
				//change the team field value:
				entField = EntityListView.EntityType.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.PracticeCamp];
				entField.SetValue(entityEdit, (Sport.Data.Entity)practiceCampFilter.Value);
			}

			//set status of new player to Confirmed:
			entField = EntityListView.EntityType.Fields[(int)Sport.Entities.PracticeCampParticipant.Fields.IsConfirmed];
			entField.SetValue(entityEdit, 1);
		}
		#endregion

		/// <summary>
		/// reload the view according to current filters. read from database.
		/// </summary>
		private void Requery()
		{
			System.Windows.Forms.Cursor c = System.Windows.Forms.Cursor.Current;
			System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

			string strTitle = "משתתפי מחנות אימון";
			if (sportFilter.Value != null)
				strTitle += " - " + (sportFilter.Value as Sport.Data.Entity).Name;
			if (practiceCampFilter.Value != null)
				strTitle += " - " + (practiceCampFilter.Value as Sport.Data.Entity).Name;
			this.Title = strTitle;

			if (practiceCampFilter.Value != null && _filter.Count > 0)
				EntityListView.Read(_filter);
			else
				EntityListView.Clear();
			System.Windows.Forms.Cursor.Current = c;
		} //end function Requery

		private void ReloadView()
		{
			//refresh all filters and requery the information.
			RefreshFilters();
			Requery();
		}
	}
}
