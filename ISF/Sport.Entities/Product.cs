using System;
using Sport.Data;
using Sport.Types;

namespace Sport.Entities
{
	public class Product : EntityBase
	{
		public static readonly string TypeName = "product";
		public static readonly EntityType Type;

		public enum BasicProducts
		{
			TeamRegister=1,
			PlayerRegister=2,
			ClubRegister=3
		}
		
		public enum Fields
		{
			Id = 0,
			Area,
			Name,
			Price, 
			LastModified,
			FieldCount
		}
		
		static Product()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));
			
			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Area] = new EntityEntityField(Type, (int) Fields.Area, ProductArea.TypeName);
			Type[(int) Fields.Area].MustExist = true;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Price] = new NumberEntityField(Type, (int) Fields.Price, 0, double.MaxValue, 7, 2);
			Type[(int) Fields.Price].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			
			//Type.ValueChanged = new EntityType.EntityValueChange(ValueChanged);
		}
		
		public Product(Entity entity)
			: base(entity)
		{
		}

		public Product(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Product(int productId)
			: base(Type, productId)
		{
		}

		public ProductArea ProductArea
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Area);
				if (entity == null)
					return null;
				return new ProductArea(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Area, value.Entity); 
			}
		}

		public double Price
		{
			get 
			{ 
				object value = GetValue((int) Fields.Price);
				if ((value == null)||(value.ToString().Length == 0))
					return 0;
				else
					return Double.Parse(value.ToString());
			}
			set { SetValue((int) Fields.Price, value); }
		}
	}
}
