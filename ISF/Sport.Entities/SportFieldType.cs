using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class SportFieldType : EntityBase
	{
		public static readonly string TypeName = "sportfieldtype";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Sport,
			Name,
			CompetitionType,
			LastModified,
			FieldCount
		}
		static SportFieldType()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.CompetitionType] = new LookupEntityField(Type, (int) Fields.CompetitionType, new CompetitionTypeLookup());
			Type[(int) Fields.CompetitionType].MustExist = true;
			Type[(int) Fields.Sport] = new EntityEntityField(Type, (int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Sport].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public SportFieldType(Entity entity)
			: base(entity)
		{
		}

		public SportFieldType(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public SportFieldType(int sportFieldTypeId)
			: base(Type, sportFieldTypeId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
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

		public CompetitionType CompetitionType
		{
			get { return (CompetitionType) GetValue((int) Fields.CompetitionType); }
			set { SetValue((int) Fields.CompetitionType, (int) value); }
		}

		public SportField[] GetSportFields()
		{
			Entity[] entities = SportField.Type.GetEntities(
				new EntityFilter((int) SportField.Fields.SportFieldType, Id));

			SportField[] sportFields = new SportField[entities.Length];
			for (int n = 0; n < entities.Length; n++)
			{
				sportFields[n] = new SportField(entities[n]);
			}

			return sportFields;
		}
	}
}
