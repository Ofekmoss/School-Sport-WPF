using System;
using Sport.Data;
using System.Collections;

namespace Sport.Entities
{
	public class Court : EntityBase
	{
		public static readonly string TypeName = "court";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			Facility,
			CourtType,
			LastModified,
			FieldCount
		}

		static Court()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Facility] = new EntityEntityField(Type, (int) Fields.Facility, Facility.TypeName);
			Type[(int) Fields.Facility].MustExist = true;
			Type[(int) Fields.CourtType] = new EntityEntityField(Type, (int) Fields.CourtType, CourtType.TypeName);
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Court(Entity entity)
			: base(entity)
		{
		}

		public Court(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Court(int courtId)
			: base(Type, courtId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
		}

		public Facility Facility
		{
			get 
			{
				Entity entity = GetEntityValue((int) Fields.Facility);
				if (entity == null)
					return null;
				return new Facility(entity);
			}
			set 
			{ 
				SetValue((int) Fields.Facility, value.Entity); 
			}
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
		
		public override string CanDelete()
		{
			//initialize service:
			DataServices.DataService dataService=new DataServices.DataService();
			
			//begin checkings.. check if part of any competition:
			int[] arrCategoriesID=
				dataService.GetContainingChampionships(-1, -1, this.Id);
			
			string result="";
			if ((arrCategoriesID != null)&&(arrCategoriesID.Length > 0))
			{
				string strChamps="";
				foreach (int categoryID in arrCategoriesID)
				{
					ChampionshipCategory category=null;
					try
					{
						category = new ChampionshipCategory(categoryID);
					}
					catch {}
					if ((category != null)&&(category.Championship != null))
						strChamps += category.Championship.Name+" "+category.Name+"\n";
				}
				
				if (strChamps.Length > 0)
				{
					result += "באליפויות הבאות יש תחרויות הנערכות במגרש זה:\n"+strChamps+
						"\nיש להסיר תחרויות אלו מהאליפות";
				}
			}
			
			return result;
		}
	}
}
