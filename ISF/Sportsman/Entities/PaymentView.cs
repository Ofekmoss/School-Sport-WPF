using System;
using Sport.UI;

namespace Sportsman.Entities
{
	public class PaymentView : EntityView
	{
		public PaymentView()
			: base (Sport.Entities.Payment.Type)
		{

			//
			// Entity
			//
			Name = "תשלום";
			PluralName = "תשלומים";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Payment.Fields.Id];
			efv.Name = "קוד";
			efv.Width = 30;
			// School
			efv = Fields[(int) Sport.Entities.Payment.Fields.Receipt];
			efv.Name = "קבלה";
			efv.Width = 100;
			// Type
			efv = Fields[(int) Sport.Entities.Payment.Fields.Type];
			efv.Name = "אמצעי תשלום";
			efv.Width = 130;
			// Sum
			efv = Fields[(int) Sport.Entities.Payment.Fields.Sum];
			efv.Name = "סכום";
			efv.Width = 90;
			// Bank
			efv = Fields[(int) Sport.Entities.Payment.Fields.Bank];
			efv.Name = "בנק";
			efv.Width = 180;
			// BankBranch
			efv = Fields[(int) Sport.Entities.Payment.Fields.BankBranch];
			efv.Name = "סניף";
			efv.Width = 60;
			// BankAccount
			efv = Fields[(int) Sport.Entities.Payment.Fields.BankAccount];
			efv.Name = "חשבון";
			efv.Width = 90;
			// Reference
			efv = Fields[(int) Sport.Entities.Payment.Fields.Reference];
			efv.Name = "אסמכתא";
			efv.Width = 90;
			// Date
			efv = Fields[(int) Sport.Entities.Payment.Fields.Date];
			efv.Name = "תאריך פרעון";
			efv.Width = 110;
			// CreditCardType
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardType];
			efv.Name = "סוג הכרטיס";
			efv.Width = 130;
			// CreditCardNumber
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardNumber];
			efv.Name = "ספרות אחרונות";
			efv.Width = 130;
			// CreditCardExpire
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardExpire];
			efv.Name = "תוקף עד";
			efv.Width = 110;
			// CreditCardPayments
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardPayments];
			efv.Name = "מס' תשלומים";
			efv.Width = 110;
			// LastModified
			efv = Fields[(int) Sport.Entities.Payment.Fields.LastModified];
			efv.Name = "ת' שינוי אחרון";
			efv.Width = 120;
		}
	}
}
