using System;
using Sport.Data;

namespace Sport.Entities
{
	public class CourtTypeSport : EntityBase
	{
		public static readonly string TypeName = "courttypesport";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			CourtType,
			Sport,
			SportFieldType,
			SportField,
			LastModified,
			FieldCount
		}

		static CourtTypeSport()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.CourtType] = new EntityEntityField(Type, (int) Fields.CourtType, CourtType.TypeName);
			Type[(int) Fields.CourtType].MustExist = true;
			Type[(int) Fields.Sport] = new EntityEntityField(Type, (int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Sport].MustExist = true;
			Type[(int) Fields.SportFieldType] = new EntityEntityField(Type, (int) Fields.SportFieldType, SportFieldType.TypeName);
			Type[(int) Fields.SportField] = new EntityEntityField(Type, (int) Fields.SportField, SportField.TypeName);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.ValueChanged = new EntityType.EntityValueChange(ValueChanged);

			Type.NameField = new FormatEntityField(Type, "{0} {1} {2}", 
				new int[] { (int) Fields.Sport, (int) Fields.SportFieldType, (int) Fields.SportField });
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		private static void ValueChanged(EntityEdit entityEdit, int field)
		{
			if (field == (int) Fields.Sport)
			{
				Type[(int) Fields.SportField].SetValue(entityEdit, null);
				Type[(int) Fields.SportFieldType].SetValue(entityEdit, null);
			}
			else if (field == (int) Fields.SportFieldType)
			{
				Type[(int) Fields.SportField].SetValue(entityEdit, null);
			}
		}

		public CourtTypeSport(Entity entity)
			: base(entity)
		{
		}

		public CourtTypeSport(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public CourtTypeSport(int courtTypeSportId)
			: base(Type, courtTypeSportId)
		{
		}

		public CourtType CourtType
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.CourtType);
				if (entity == null)
					return null;
				return new CourtType(entity);
			}
			set 
			{ 
				SetValue((int) Fields.CourtType, value.Entity); 
			}
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

		public SportFieldType SportFieldType
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.SportFieldType);
				if (entity == null)
					return null;
				return new SportFieldType(entity);
			}
			set 
			{ 
				SetValue((int) Fields.SportFieldType, value.Entity); 
			}
		}

		public SportField SportField
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.SportField);
				if (entity == null)
					return null;
				return new SportField(entity);
			}
			set 
			{ 
				SetValue((int) Fields.SportField, value.Entity); 
			}
		}
	}
}
