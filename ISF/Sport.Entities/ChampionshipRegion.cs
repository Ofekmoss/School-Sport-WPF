using System;
using Sport.Data;
using Sport.Types;
using Sport.Core;

namespace Sport.Entities
{
	public class ChampionshipRegion : EntityBase
	{
		public static readonly string TypeName = "championshipregion";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Championship,
			Region,
			LastModified,
			FieldCount
		}

		static ChampionshipRegion()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id), Session.GetSeasonCache());

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Championship] = new EntityEntityField(Type, (int) Fields.Championship, Championship.TypeName);
			Type[(int) Fields.Championship].MustExist = true;
			Type[(int) Fields.Region] = new EntityEntityField(Type, (int) Fields.Region, Region.TypeName);
			Type[(int) Fields.Region].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Region];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			Type.OwnerField = Type[(int) Fields.Championship];
		}

		public ChampionshipRegion(Entity entity)
			: base(entity)
		{
		}

		public ChampionshipRegion(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public ChampionshipRegion(int championshipRegionId)
			: base(Type, championshipRegionId)
		{
		}

		public Championship Championship
		{
			get
			{
				Entity entity = GetEntityValue((int) Fields.Championship);
				if (entity == null)
					return null;
				return new Championship(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Championship, value.Entity); 
			}
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
	}
}
