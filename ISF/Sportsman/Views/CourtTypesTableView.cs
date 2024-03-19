using System;

namespace Sportsman.Views
{
	/// <summary>
	/// Summary description for CourtTypesTableView.
	/// </summary>
	[Sport.UI.ViewPermissions(Sport.Types.Permission.Sports, true)]
	public class CourtTypesTableView : Sport.UI.TableView
	{
		public CourtTypesTableView()
		{
			Items.Add((int) Sport.Entities.CourtType.Fields.Name, "שם", 250);

			gdiCourtTypeSports = new GridDetailItem("ענפי ספורט:", 
				Sport.Entities.CourtTypeSport.Type, 
				(int) Sport.Entities.CourtTypeSport.Fields.CourtType, 
				new System.Drawing.Size(400, 150));
			
			gdiCourtTypeSports.Columns.Add((int) Sport.Entities.CourtTypeSport.Fields.Sport, "ענף", 200);
			gdiCourtTypeSports.Columns.Add((int) Sport.Entities.CourtTypeSport.Fields.SportFieldType, "סוג מקצוע", 200);
			gdiCourtTypeSports.Columns.Add((int) Sport.Entities.CourtTypeSport.Fields.SportField, "מקצוע", 200);

			gdiCourtTypeSports.EntityListView.Fields[(int) Sport.Entities.CourtTypeSport.Fields.Sport].Values =
				Sport.Entities.Sport.Type.GetEntities(null);

			gdiCourtTypeSports.EntityListView.CurrentChanged += new Sport.Data.EntityEventHandler(CourtTypeSportsCurrentChanged);
			gdiCourtTypeSports.EntityListView.ValueChanged += new Sport.UI.FieldEditEventHandler(CourtTypeSportsValueChanged);
			
			Items.Add("ענפי ספורט", gdiCourtTypeSports);
		}

		GridDetailItem gdiCourtTypeSports;

		public override void Open()
		{
			Title = "סוגי מגרשים";

			MoreButtonEnabled = false;

			EntityListView = new Sport.UI.EntityListView(Sport.Entities.CourtType.TypeName);

			Columns = new int[] { 0 };
			Details = new int[] { 1 };

			EntityListView.Read(null);

			base.Open();
		}

		protected override bool OnDeleteEntity(Sport.Data.Entity entity)
		{
			//cancel if no entity selected:
			if (entity == null)
				return false;
			
			//build proper entity:
			Sport.Entities.CourtType courtType=
				new Sport.Entities.CourtType(entity);
			
			//begin checkings... first check for sports for the type:
			Sport.Entities.CourtTypeSport[] courtSports=courtType.GetSports();
			if (courtSports.Length > 0)
			{
				string names="";
				for (int i=0; i<courtSports.Length; i++)
				{
					names += courtSports[i].Name+"\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Warn("סוג המגרש '"+courtType.Name+"' משויך לענפי הספורט הבאים: \n"+names+
					"יש להסיר ענפי ספורט אלו", "מחיקת סוג מגרש");
				return false;
			}

			//check for courts:
			Sport.Entities.Court[] courts=courtType.GetCourts();
			if (courts.Length > 0)
			{
				string names="";
				for (int i=0; i<courts.Length; i++)
				{
					names += courts[i].Name+"\n";
					if (i >= 15)
					{
						names += "...\n";
						break;
					}
				}
				Sport.UI.MessageBox.Warn("סוג המגרש '"+courtType.Name+"' מכיל את המגרשים הבאים: \n"+names+
					"יש להסיר מגרשים אלו", "מחיקת סוג מגרש");
				return false;
			}			
			
			return Sport.UI.MessageBox.Ask("האם למחוק את סוג המגרש '"+courtType.Name+"'?", false);
		}


		private void CourtTypeSportsCurrentChanged(object sender, Sport.Data.EntityEventArgs e)
		{
			if (e.Entity != null)
				SetSportFieldsValues(e.Entity);
		}

		private void CourtTypeSportsValueChanged(object sender, Sport.UI.FieldEditEventArgs e)
		{
			SetSportFieldsValues(e.EntityEdit);
		}

		private int lastSport = -1;
		private int lastFieldType = -1;

		private void SetSportFieldsValues(Sport.Data.Entity entity)
		{
			Sport.Entities.CourtTypeSport cts = new Sport.Entities.CourtTypeSport(entity);

			if (cts.Sport != null)
			{
				if (cts.Sport.Id != lastSport)
				{
					gdiCourtTypeSports.EntityListView.Fields[(int) Sport.Entities.CourtTypeSport.Fields.SportFieldType].Values =
						Sport.Entities.SportFieldType.Type.GetEntities(
						new Sport.Data.EntityFilter((int) Sport.Entities.SportFieldType.Fields.Sport,
						cts.Sport.Id));
					lastSport = cts.Sport.Id;
				}
			}

			if (cts.SportFieldType != null)
			{
				if (cts.SportFieldType.Id != lastFieldType)
				{
					gdiCourtTypeSports.EntityListView.Fields[(int) Sport.Entities.CourtTypeSport.Fields.SportField].Values =
						Sport.Entities.SportField.Type.GetEntities(
						new Sport.Data.EntityFilter((int) Sport.Entities.SportField.Fields.SportFieldType,
						cts.SportFieldType.Id));
					lastFieldType = cts.SportFieldType.Id;
				}
			}
		}
	}
}
