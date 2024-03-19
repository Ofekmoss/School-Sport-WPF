using System;
using Sport.Data;

namespace Sport.Entities
{
	public class PracticeCamp : EntityBase
	{
		public static readonly string TypeName = "practicecamp";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Sport,
			DateStart,
			DateFinish,
			BasePrice,
			Remarks,
			LastModified,
			FieldCount
		}

		static PracticeCamp()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Sport] = new EntityEntityField(Type, (int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Sport].MustExist = true;
			Type[(int) Fields.DateStart] = new DateTimeEntityField(Type, (int) Fields.DateStart, "dd/MM/yyyy");
			Type[(int) Fields.DateStart].MustExist = true;
			Type[(int) Fields.DateFinish] = new DateTimeEntityField(Type, (int) Fields.DateFinish, "dd/MM/yyyy");
			Type[(int) Fields.DateFinish].MustExist = true;
			Type[(int) Fields.BasePrice] = new NumberEntityField(Type, (int) Fields.BasePrice, 0, 9999);
			Type[(int) Fields.BasePrice].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "מחנה אימון {0} מ{1} עד {2}", 
				new int[] { (int) Fields.Sport, (int) Fields.DateStart, (int) Fields.DateFinish });
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}
		
		public PracticeCamp(Entity entity)
			: base(entity)
		{
		}

		public PracticeCamp(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public PracticeCamp(int practiceCampID)
			: base(Type, practiceCampID)
		{
		}

		public Sport Sport
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Sport);
				if (entity == null)
					return null;
				return new Sport(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Sport, value.Entity); 
			}
		}
		
		public DateTime DateStart
		{
			get { return Common.Tools.ToDateDef(GetValue((int) Fields.DateStart), DateTime.MinValue); }
			set { SetValue((int) Fields.DateStart, value); }
		}
		
		public DateTime DateFinish
		{
			get { return Common.Tools.ToDateDef(GetValue((int) Fields.DateFinish), DateTime.MinValue); }
			set { SetValue((int) Fields.DateFinish, value); }
		}
		
		public int BasePrice
		{
			get { return Common.Tools.CIntDef(GetValue((int) Fields.BasePrice), 0); }
			set { SetValue((int) Fields.BasePrice, value); }
		}
		
		public string Remarks
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.Remarks), ""); }
			set { SetValue((int) Fields.Remarks, value); }
		}
		
		public PracticeCampParticipant[] GetParticipants()
		{
			Entity[] entities = PracticeCampParticipant.Type.GetEntities(
				new EntityFilter((int) PracticeCampParticipant.Fields.PracticeCamp, this.Id));
			
			if (entities == null)
				return null;
			
			PracticeCampParticipant[] participants = new PracticeCampParticipant[entities.Length];
			for (int t = 0; t < entities.Length; t++)
				participants[t] = new PracticeCampParticipant(entities[t]);
			
			return participants;
		}
		
		public override string CanDelete()
		{
			string result="";
			
			//check for participants:
			PracticeCampParticipant[] participants = this.GetParticipants();
			if ((participants != null)&&(participants.Length > 0))
			{
				string strParticipants = "";
				foreach (PracticeCampParticipant participant in participants)
					strParticipants += participant.Name + "\n";
				if (strParticipants.Length > 0)
				{
					result += "במחנה אימון זה רשומים המשתתפים הבאים: \n";
					result += strParticipants + "יש להסיר משתתפים אלו כדי למחוק את מחנה האימון\n";
				}
			}
			
			return result;
		}
	}
}
