using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class PracticeCampParticipant : EntityBase
	{
		public static readonly string TypeName = "practicecampparticipant";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			PracticeCamp,
			ParticipantName,
			ParticipantAddress,
			ParticipantSchool,
			ParticipantBirthday,
			ParticipantPhone,
			ParticipantCellPhone,
			Remarks,
			Charge,
			IsConfirmed,
			SexType,
			Email,
			LastModified,
			DataFields,
			Sport = DataFields,
			FieldCount
		}

		static PracticeCampParticipant()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.DataFields, new FieldEntityId((int) Fields.Id));
			
			//Data fields:
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.PracticeCamp] = new EntityEntityField(Type, (int) Fields.PracticeCamp, PracticeCamp.TypeName);
			Type[(int) Fields.PracticeCamp].MustExist = true;
			Type[(int) Fields.ParticipantName].MustExist = true;
			Type[(int) Fields.Charge] = new EntityEntityField(Type, (int) Fields.Charge, Charge.TypeName);
			Type[(int) Fields.IsConfirmed] = new LookupEntityField(Type, (int) Fields.IsConfirmed, new BooleanTypeLookup("מאושר", "רשום"));
			Type[(int) Fields.IsConfirmed].MustExist = true;
			Type[(int) Fields.SexType] = new LookupEntityField(Type, (int) Fields.SexType, new SexTypeLookup());
			Type[(int) Fields.Email].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			// Relative fields
			Type[(int) Fields.Sport] = new EntityRelationEntityField(Type, 
				(int) Fields.Sport,
				(int) Fields.PracticeCamp,
				Sport.TypeName, (int) Sport.Fields.Id);
			
			Type.NameField = new FormatEntityField(Type, "{0} {1}", 
				new int[] { (int) Fields.ParticipantName, (int) Fields.ParticipantAddress });
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}
		
		public PracticeCampParticipant(Entity entity)
			: base(entity)
		{
		}
		
		public PracticeCampParticipant(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}
		
		public PracticeCampParticipant(int practiceCampID)
			: base(Type, practiceCampID)
		{
		}
		
		public PracticeCamp PracticeCamp
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.PracticeCamp);
				if (entity == null)
					return null;
				return new PracticeCamp(entity);
			}
			set 
			{ 
				SetValue((int) Fields.PracticeCamp, value.Entity); 
			}
		}
		
		public string ParticipantName
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.ParticipantName), ""); }
			set { SetValue((int) Fields.ParticipantName, value); }
		}
		
		public string ParticipantAddress
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.ParticipantAddress), ""); }
			set { SetValue((int) Fields.ParticipantAddress, value); }
		}
		
		public string ParticipantSchool
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.ParticipantSchool), ""); }
			set { SetValue((int) Fields.ParticipantSchool, value); }
		}
		
		public string ParticipantBirthday
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.ParticipantBirthday), ""); }
			set { SetValue((int) Fields.ParticipantBirthday, value); }
		}
		
		public string ParticipantPhone
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.ParticipantPhone), ""); }
			set { SetValue((int) Fields.ParticipantPhone, value); }
		}
		
		public string ParticipantCellPhone
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.ParticipantCellPhone), ""); }
			set { SetValue((int) Fields.ParticipantCellPhone, value); }
		}
		
		public string Remarks
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.Remarks), ""); }
			set { SetValue((int) Fields.Remarks, value); }
		}
		
		public Charge Charge
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Charge);
				if (entity == null)
					return null;
				return new Charge(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Charge, value.Entity); 
			}
		}
		
		public bool IsConfirmed
		{
			get { return (Common.Tools.CIntDef(GetValue((int) Fields.IsConfirmed), 0) == 1); }
			set { SetValue((int) Fields.IsConfirmed, ((value == true)?1:0)); }
		}
		
		public Sex SexType
		{
			get 
			{
				object value = GetValue((int) Fields.SexType);
				return (value == null) ? Types.Sex.None : (Sex) value;
			}
			set { SetValue((int) Fields.SexType, (int) value); }
		}
		
		public string Email
		{
			get { return Common.Tools.CStrDef(GetValue((int) Fields.Email), ""); }
			set { SetValue((int) Fields.Email, value); }
		}
	}
}
