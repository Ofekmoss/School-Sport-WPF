using System;
using Sport.Data;

namespace Sport.Entities
{
	public class CourtType : EntityBase
	{
		public static readonly string TypeName = "courttype";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			LastModified,
			FieldCount
		}

		static CourtType()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public CourtType(Entity entity)
			: base(entity)
		{
		}

		public CourtType(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public CourtType(int courtTypeId)
			: base(Type, courtTypeId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
		}
		
		public Court[] GetCourts()
		{
			EntityFilter filter=new EntityFilter(
				(int) Court.Fields.CourtType, this.Id);
			Entity[] courts=Court.Type.GetEntities(filter);
			Court[] result=new Court[courts.Length];
			for (int i=0; i<courts.Length; i++)
			{
				result[i] = new Court(courts[i]);
			}
			return result;
		}

		public CourtTypeSport[] GetSports()
		{
			EntityFilter filter=new EntityFilter(
				(int) CourtTypeSport.Fields.CourtType, this.Id);
			Entity[] typeSports=CourtTypeSport.Type.GetEntities(filter);
			CourtTypeSport[] result=new CourtTypeSport[typeSports.Length];
			for (int i=0; i<typeSports.Length; i++)
			{
				result[i] = new CourtTypeSport(typeSports[i]);
			}
			return result;
		}
	}
}
