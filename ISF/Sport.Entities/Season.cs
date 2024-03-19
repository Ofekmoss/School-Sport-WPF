using System;
using System.Linq;
using Sport.Data;
using Sport.Types;
using System.Collections.Generic;

namespace Sport.Entities
{
	public class Season : EntityBase
	{
		public static readonly string TypeName = "season";
		public static readonly EntityType Type;

		public enum Fields
		{
			Season = 0,
			Name, 
			Status,
			Start,
			End,
			LastModified,
			FieldCount
		}

		public static readonly int ZeroYear = 1948;

		static Season()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Season));
			
			Type[(int) Fields.Season] = new NumberEntityField(Type, (int) Fields.Season);
			Type[(int) Fields.Season].MustExist = true;
			Type[(int) Fields.Season].CanEdit = false;
			
			Type[(int) Fields.Status] = new LookupEntityField(Type, (int) Fields.Status, new SeasonStatusType());
			Type[(int) Fields.Status].MustExist = true;
			
			Type[(int) Fields.Start] = new DateTimeEntityField(Type, (int) Fields.Start, "dd/MM/yyyy");
			//Type[(int) Fields.Start].MustExist = true;
			
			Type[(int) Fields.End] = new DateTimeEntityField(Type, (int) Fields.End, "dd/MM/yyyy");
			
			Type[(int) Fields.Name].MustExist = true;
			
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;
			
			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public Season(Entity entity)
			: base(entity)
		{
		}

		public Season(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public Season(int season)
			: base(Type, season)
		{
		}

		public new string Name
		{
			get
			{
				object value=GetValue((int) Fields.Name);
				return (value == null)?"":value.ToString();
			}
			set { SetValue((int) Fields.Name, value); }
		}

		public SeasonStatus Status
		{
			get
			{
				object value=GetValue((int) Fields.Status);
				return (value == null)?(SeasonStatus.Closed):((SeasonStatus) value);
			}
			set { SetValue((int) Fields.Status, value); }
		}

		public DateTime Start
		{
			get 
			{ 
				object start = GetValue((int) Fields.Start);
				if (start == null)
					return new DateTime(ZeroYear + Id, 9, 1);
				return (DateTime) start; 
			}
			set { SetValue((int) Fields.Start, value); }
		}

		public DateTime End
		{
			get 
			{ 
				object end = GetValue((int) Fields.End);
				if (end == null)
					return new DateTime(ZeroYear + Id + 1, 8, 31);
				return (DateTime) end;
			}
			set { SetValue((int) Fields.End, value); }
		}

		public static int GetLatestSeason()
		{
			Entity[] allSeasons = Season.Type.GetEntities(null);
			int latestSeason = -1;
			if (allSeasons != null)
			{
				for (int i = 0; i < allSeasons.Length; i++)
				{
					Season curSeason = new Season(allSeasons[i]);
					if (curSeason.Status != SeasonStatus.Closed && curSeason.Id > latestSeason)
						latestSeason = curSeason.Id;
				}
			}
			return latestSeason;
		}

		public static int GetOpenSeason()
		{
			Entity[] allSeasons = Season.Type.GetEntities(null);
			int openSeason = -1;
			if (allSeasons != null)
			{
				List<Season> seasons = allSeasons.ToList().ConvertAll(e => new Season(e));
				seasons.Sort((s1, s2) => s2.Id.CompareTo(s1.Id));
				for (int i = seasons.Count - 1; i >= 0; i--)
				{
					Season curSeason = seasons[i];
					if (curSeason.Status == SeasonStatus.Opened)
					{
						openSeason = curSeason.Id;
						break;
					}
				}
			}
			return openSeason;
		}
	}
}
