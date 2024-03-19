using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class PracticeCampView : EntityView
	{
		private EntitySelectionDialog sportDialog;
		
		public PracticeCampView()
			: base (Sport.Entities.PracticeCamp.Type)
		{
			//
			// Entity
			//
			Name = "מחנה אימון";
			PluralName = "מחנות אימון";
			
			//
			// Fields
			//
			
			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.PracticeCamp.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			efv.CanEdit = false;
			// Sport
			efv = Fields[(int) Sport.Entities.PracticeCamp.Fields.Sport];
			efv.Name = "ענף ספורט";
			efv.Width = 90;
			efv.MustExist = true;
			sportDialog = new Sport.UI.EntitySelectionDialog(new Views.SportsTableView());
			sportDialog.View.State[View.SelectionDialog] = "1";
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(sportDialog.ValueSelector));
			// Date Start
			efv = Fields[(int) Sport.Entities.PracticeCamp.Fields.DateStart];
			efv.Name = "תאריך התחלה";
			efv.Width = 120;
			efv.MustExist = true;
			// Date Finish
			efv = Fields[(int) Sport.Entities.PracticeCamp.Fields.DateFinish];
			efv.Name = "תאריך סיום";
			efv.Width = 120;
			efv.MustExist = true;
			// Base Price
			efv = Fields[(int) Sport.Entities.PracticeCamp.Fields.BasePrice];
			efv.Name = "תעריף רישום";
			efv.Width = 90;
			efv.MustExist = true;
			// Remarks
			efv = Fields[(int) Sport.Entities.PracticeCamp.Fields.Remarks];
			efv.Name = "הערות";
			efv.Width = 160;
			// LastModified
			efv = Fields[(int) Sport.Entities.PracticeCamp.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}
	}
}
