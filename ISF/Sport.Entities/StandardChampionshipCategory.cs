using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class StandardChampionshipCategory : EntityBase
	{
		public static readonly string TypeName = "standardchampionshipcategory";
		public static readonly EntityType Type;

		public enum Fields
		{
			Championship = 0,
			Id,
			Category,
			LastModified,
			FieldCount
		}

		static StandardChampionshipCategory()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Championship] = new EntityEntityField(Type, (int) Fields.Championship, StandardChampionship.TypeName);
			Type[(int) Fields.Championship].MustExist = true;
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Category] = new LookupEntityField(Type, (int) Fields.Category, new CategoryTypeLookup());
			Type[(int) Fields.Category].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Category];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			Type.OwnerField = Type[(int) Fields.Championship];
		}

		public StandardChampionshipCategory(Entity entity)
			: base(entity)
		{
		}

		public StandardChampionshipCategory(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public StandardChampionshipCategory(int standardChampionshipCategoryId)
			: base(Type, standardChampionshipCategoryId)
		{
		}

		public StandardChampionship Championship
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Championship);
				if (entity == null)
					return null;
				return new StandardChampionship(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Championship, value.Entity); 
			}
		}


		public int Category
		{
			get { return (int) GetValue((int) Fields.Category); }
			set { SetValue((int) Fields.Category, value); }
		}
	}
}
