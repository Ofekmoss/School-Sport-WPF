using System;

namespace Sport.Data
{
	public enum EntityResultCode
	{
		// Success
		Ok,
		NoChange,
		// Failure
		Failed,
		FieldMustExist,
		NotEditing,
		NotConnected,
		UniqueValueFailure,
		ValueOutOfRange,
	}

	public class EntityResult
	{
		public static readonly EntityResult Ok = new EntityResult(EntityResultCode.Ok);
		public static readonly EntityResult Failed = new EntityResult(EntityResultCode.Failed);


		private EntityResultCode _resultCode;
		public EntityResultCode ResultCode
		{
			get { return _resultCode; }
		}

		public bool Succeeded
		{
			get { return _resultCode < EntityResultCode.Failed; }
		}

		public EntityResult(EntityResultCode resultCode)
		{
			_resultCode = resultCode;
		}

		public override string ToString()
		{
			return _resultCode.ToString();
		}

		public virtual string GetMessage()
		{
			return Succeeded ? "הצלחה" : "שגיאה " + ToString();
		}

		public override bool Equals(object obj)
		{
			if (obj is EntityResult)
				return ((EntityResult) obj).ResultCode == _resultCode;
			if (obj is EntityResultCode)
				return ((EntityResultCode) obj) == _resultCode;

			return base.Equals (obj);
		}

		public override int GetHashCode()
		{
			return (int) _resultCode;
		}

		public static bool operator==(EntityResult result, EntityResultCode resultCode)
		{
			return result.Equals(resultCode);
		}

		public static bool operator!=(EntityResult result, EntityResultCode resultCode)
		{
			return !result.Equals(resultCode);
		}
	}

	public class EntityMessageResult : EntityResult
	{
		private string _message;
		public string Message
		{
			get { return _message; }
		}

		public EntityMessageResult(EntityResultCode result, string message)
			: base(result)
		{
			_message = message;
		}

		public override string ToString()
		{
			return base.ToString() + " (message: " + _message + ")";
		}

		public override string GetMessage()
		{
			return Succeeded ? "הצלחה" : _message;
		}

	}

	public class EntityFieldResult : EntityResult
	{
		private EntityField _field;
		public EntityField Field
		{
			get { return _field; }
		}

		public EntityFieldResult(EntityResultCode result, EntityField field)
			: base(result)
		{
			_field = field;
		}

		public override string ToString()
		{
			return base.ToString() + " (type: " + _field.Type.Name + ", field: " + _field.Index + ")";
		}
        
		public override string GetMessage()
		{
			return Succeeded ? "הצלחה" : "שגיאה בשדה " + _field.Index.ToString();
		}

	}
}
