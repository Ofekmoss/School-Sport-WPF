using System;

namespace Sportsman.Commands
{
	/// <summary>
	/// Summary description for DeleteDatFilesCommand.
	/// </summary>
	public class DeleteDatFilesCommand : Sport.UI.Command
	{
		private int _counter=0;
		
		public DeleteDatFilesCommand()
		{
			_counter = 0;
		}

		public override void Execute(string param)
		{
			if (!Sport.UI.MessageBox.Ask("האם למחוק את כל הקבצים הזמניים?", false))
				return;
			
			Sport.UI.Dialogs.WaitForm.ShowWait("אנא המתן מוחק קבצים...");

			DeleteDatFile(Sport.Entities.Bug.Type);
			DeleteDatFile(Sport.Entities.Championship.Type);
			DeleteDatFile(Sport.Entities.ChampionshipCategory.Type);
			DeleteDatFile(Sport.Entities.ChampionshipRegion.Type);
			DeleteDatFile(Sport.Entities.Charge.Type);
			DeleteDatFile(Sport.Entities.City.Type);
			DeleteDatFile(Sport.Entities.Court.Type);
			DeleteDatFile(Sport.Entities.CourtType.Type);
			DeleteDatFile(Sport.Entities.CourtTypeSport.Type);
			DeleteDatFile(Sport.Entities.Equipment.Type);
			DeleteDatFile(Sport.Entities.EquipmentCategory.Type);
			DeleteDatFile(Sport.Entities.EquipmentChampionship.Type);
			DeleteDatFile(Sport.Entities.EquipmentRegion.Type);
			DeleteDatFile(Sport.Entities.EquipmentSport.Type);
			DeleteDatFile(Sport.Entities.EquipmentType.Type);
			DeleteDatFile(Sport.Entities.Facility.Type);
			DeleteDatFile(Sport.Entities.GameBoard.Type);
			DeleteDatFile(Sport.Entities.IsfPayment.Type);
			DeleteDatFile(Sport.Entities.Message.Type);
			DeleteDatFile(Sport.Entities.Payment.Type);
			DeleteDatFile(Sport.Entities.PhasePattern.Type);
			DeleteDatFile(Sport.Entities.Player.Type);
			DeleteDatFile(Sport.Entities.Product.Type);
			DeleteDatFile(Sport.Entities.Sport.Type);
			DeleteDatFile(Sport.Entities.SportField.Type);
			DeleteDatFile(Sport.Entities.SportFieldType.Type);
			DeleteDatFile(Sport.Entities.StandardChampionship.Type);
			DeleteDatFile(Sport.Entities.StandardChampionshipCategory.Type);
			DeleteDatFile(Sport.Entities.Student.Type);
			DeleteDatFile(Sport.Entities.Team.Type);
			DeleteDatFile(Sport.Entities.User.Type);
			
			Sport.UI.Dialogs.WaitForm.HideWait();
			Sport.UI.MessageBox.Success(_counter+" קבצים נמחקו בהצלחה", "מחיקת קבצים זמניים");
		}

		private void DeleteDatFile(Sport.Data.EntityType type)
		{
			type.Reset(null);
			type.DeleteDatFile();
			_counter++;
		}

		public override bool IsPermitted(string param)
		{
			return true;
		}

	}
}
