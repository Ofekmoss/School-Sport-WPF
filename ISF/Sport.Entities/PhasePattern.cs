using System;
using Sport.Data;
using Sport.Common;

namespace Sport.Entities
{
	/// <summary>
	/// Implement an EntityType for a PhasePattern
	/// </summary>
	public class PhasePattern : EntityBase
	{
		public static readonly string TypeName = "phasepattern";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			Range,
			LastModified,
			FieldCount
		}

		static PhasePattern()
		{
			Type = new EntityType(TypeName, (int) Fields.FieldCount, (int) Fields.FieldCount, new FieldEntityId((int) Fields.Id));

			Type[(int) Fields.Id] = new NumberEntityField(Type, (int) Fields.Id);
			Type[(int) Fields.Id].CanEdit = false;
			Type[(int) Fields.Name].MustExist = true;
			Type[(int) Fields.Range].CanEdit = false;
			Type[(int) Fields.LastModified] = new DateTimeEntityField(Type, (int) Fields.LastModified, "dd/MM/yyyy HH:mm:ss");
			Type[(int) Fields.LastModified].CanEdit = false;

			Type.NameField = Type[(int) Fields.Name];
			Type.DateLastModified = Type[(int) Fields.LastModified];
		}

		public PhasePattern(Entity entity)
			: base(entity)
		{
		}

		public PhasePattern(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public PhasePattern(int phasePatternId)
			: base(Type, phasePatternId)
		{
		}

		public new string Name
		{
			set { SetValue((int) Fields.Name, value); }
		}

		public string Range
		{
			get { return (string) GetValue((int) Fields.Range); }
		}

		public RangeArray GetRangeArray()
		{
			return RangeArray.Parse(Range);
		}
	}
}
