using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class InstantMessageView : EntityView
	{
		private EntitySelectionDialog userDialog;
		
		public InstantMessageView()
			: base (Sport.Entities.InstantMessage.Type)
		{
			//
			// Entity
			//
			Name = "הודעה מיידית";
			PluralName = "הודעות מיידיות";
			
			//
			// Fields
			//
			
			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.InstantMessage.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			efv.CanEdit = false;
			// Sender
			efv = Fields[(int) Sport.Entities.InstantMessage.Fields.Sender];
			efv.Name = "מאת";
			efv.Width = 120;
			efv.CanEdit = false;
			// Recipient
			efv = Fields[(int) Sport.Entities.InstantMessage.Fields.Recipient];
			efv.Name = "לכבוד";
			efv.Width = 120;
			userDialog = new Sport.UI.EntitySelectionDialog(new Views.UsersTableView());
			userDialog.View.State[View.SelectionDialog] = "1";
			efv.GenericItemType = Sport.UI.Controls.GenericItemType.Button;
			efv.Values = Sport.UI.Controls.GenericItem.ButtonValues(new Sport.UI.Controls.ButtonBox.SelectValue(userDialog.ValueSelector));
			// Date Sent
			efv = Fields[(int) Sport.Entities.InstantMessage.Fields.DateSent];
			efv.Name = "תאריך שליחה";
			efv.Width = 160;
			efv.CanEdit = false;
			// Contents
			efv = Fields[(int) Sport.Entities.InstantMessage.Fields.Contents];
			efv.Name = "תוכן ההודעה";
			efv.Size = new System.Drawing.Size(400, 120);
			// Date Read
			efv = Fields[(int) Sport.Entities.InstantMessage.Fields.DateRead];
			efv.Name = "תאריך קריאה";
			efv.Width = 160;
			efv.CanEdit = false;
			// LastModified
			efv = Fields[(int) Sport.Entities.InstantMessage.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}
	}
}
