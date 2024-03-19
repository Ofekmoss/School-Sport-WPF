using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class FunctionarySport : EntityBase
	{
		public static readonly string TypeName = "functionarysport";
		public static readonly EntityType Type;
		
		public enum Fields
		{
			Id = 0,
			Functionary,
			Sport,
			LastModified,
			FieldCount
		}

		static FunctionarySport()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Functionary] = new EntityEntityField(Type, (int) Fields.Functionary, Functionary.TypeName);
			Type[(int) Fields.Functionary].MustExist = true;
			Type[(int) Fields.Sport] = new EntityEntityField(Type, (int) Fields.Sport, Sport.TypeName);
			Type[(int) Fields.Sport].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = new FormatEntityField(Type, "{0} {1}", 
				new int[] { (int) Fields.Functionary, (int) Fields.Sport });
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}
		
		public FunctionarySport(Entity entity)
			: base(entity)
		{
		}
		
		public FunctionarySport(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}
		
		public FunctionarySport(int functionarySportID)
			: base(Type, functionarySportID)
		{
		}
		
		public Functionary Functionary
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Functionary);
				if (entity == null)
					return null;
				return new Functionary(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Functionary, value.Entity); 
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
		
		public override string CanDelete()
		{
			return "";
		}
	}
}
