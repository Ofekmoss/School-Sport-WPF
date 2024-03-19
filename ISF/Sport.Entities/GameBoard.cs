using System;
using Sport.Data;

namespace Sport.Entities
{
	/// <summary>
	/// Implement an EntityType for a GameBoard
	/// </summary>
	public class GameBoard : EntityBase
	{
		public static readonly string TypeName = "gameboard";
		public static readonly EntityType Type;

		public enum Fields
		{
			Id = 0,
			Name,
			Range,
			LastModified,
			FieldCount
		}

		static GameBoard()
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

		public GameBoard(Entity entity)
			: base(entity)
		{
		}

		public GameBoard(EntityEdit entityEdit)
			: base(entityEdit)
		{
		}

		public GameBoard(int gameBoardId)
			: base(Type, gameBoardId)
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
	}
}
