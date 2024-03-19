using System;
using Sport.Data;

namespace Sport.Entities
{
	/// <summary>
	/// Implement an EntityType for a City
	/// </summary>
	public class City : EntityBase
	{
		public static readonly string TypeName = "city";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			Region,
			LastModified,
			FieldCount
		}

		static City()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Region].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public City(Entity entity)
			: base(entity)
		{

		}

		public City(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public City(int cityId)
			: base(Type, cityId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
		}

		public Region Region
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Region);
				if (entity == null)
					return null;
				return new Region(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Region, value.Entity); 
			}
		}

		public School[] GetSchools()
		{
			EntityFilter filter = new EntityFilter();
			filter.Add(new EntityFilterField((int) School.Fields.Region, this.Region.Id));
			filter.Add(new EntityFilterField((int) School.Fields.City, this.Id));

			Entity[] entities = School.Type.GetEntities(filter);
			School[] schools = new School[entities.Length];
			for (int i = 0; i < entities.Length; i++)
			{
				schools[i] = new School(entities[i]);
			}
			return schools;
		}
	}
}
