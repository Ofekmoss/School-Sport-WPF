using System;
using System.Collections;
using Sport.Data;

namespace Sport.Entities
{
	public class SportField : EntityBase
	{
		public static readonly string TypeName = "sportfield";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			SportFieldType,
			Name,
			LastModified,
			FieldCount
		}
		static SportField()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.SportFieldType] = new EntityEntityField(Type, (int) Fields.SportFieldType, SportFieldType.TypeName);
			Type[(int) Fields.SportFieldType].MustExist = true;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
			Type.OwnerField = Type[(int) Fields.SportFieldType];
		}

		public SportField(Entity entity)
			: base(entity)
		{
		}

		public SportField(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public SportField(int sportFieldId)
			: base(Type, sportFieldId)
		{
		}

		public new string Name
		{
			get { return base.Name; }
			set { SetValue((int) Fields.Name, value); }
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
		
		public override string CanDelete()
		{
			//initialize service:
			DataServices.DataService dataService=new DataServices.DataService();
			
			//begin checkings.. check if part of any competition:
			int[] arrCategoriesID=
				dataService.GetContainingChampionships(this.Id, -1, -1);
			
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
					result += "באליפויות הבאות יש תחרויות עם מקצוע זה:\n"+strChamps+
						"\nיש להסיר תחרויות אלו מהאליפות";
				}
			}
			
			return result;
		}
	}
}
