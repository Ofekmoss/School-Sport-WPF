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
			Name = "�����";
			PluralName = "�������";

			//
			// Fields
			//

			// Id
			EntityFieldView efv = Fields[(int) Sport.Entities.Payment.Fields.Id];
			efv.Name = "���";
			efv.Width = 30;
			// School
			efv = Fields[(int) Sport.Entities.Payment.Fields.Receipt];
			efv.Name = "����";
			efv.Width = 100;
			// Type
			efv = Fields[(int) Sport.Entities.Payment.Fields.Type];
			efv.Name = "����� �����";
			efv.Width = 130;
			// Sum
			efv = Fields[(int) Sport.Entities.Payment.Fields.Sum];
			efv.Name = "����";
			efv.Width = 90;
			// Bank
			efv = Fields[(int) Sport.Entities.Payment.Fields.Bank];
			efv.Name = "���";
			efv.Width = 180;
			// BankBranch
			efv = Fields[(int) Sport.Entities.Payment.Fields.BankBranch];
			efv.Name = "����";
			efv.Width = 60;
			// BankAccount
			efv = Fields[(int) Sport.Entities.Payment.Fields.BankAccount];
			efv.Name = "�����";
			efv.Width = 90;
			// Reference
			efv = Fields[(int) Sport.Entities.Payment.Fields.Reference];
			efv.Name = "������";
			efv.Width = 90;
			// Date
			efv = Fields[(int) Sport.Entities.Payment.Fields.Date];
			efv.Name = "����� �����";
			efv.Width = 110;
			// CreditCardType
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardType];
			efv.Name = "��� ������";
			efv.Width = 130;
			// CreditCardNumber
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardNumber];
			efv.Name = "����� �������";
			efv.Width = 130;
			// CreditCardExpire
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardExpire];
			efv.Name = "���� ��";
			efv.Width = 110;
			// CreditCardPayments
			efv = Fields[(int)Sport.Entities.Payment.Fields.CreditCardPayments];
			efv.Name = "��' �������";
			efv.Width = 110;
			// LastModified
			efv = Fields[(int) Sport.Entities.Payment.Fields.LastModified];
			efv.Name = "�' ����� �����";
			efv.Width = 120;
		}
	}
}
