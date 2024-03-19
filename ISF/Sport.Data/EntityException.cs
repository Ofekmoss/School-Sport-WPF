using System;
using System.Runtime.Serialization;

namespace Sport.Data
{
	public class EntityException : SystemException
	{
		public EntityException()
		{
		}

		public EntityException(string s)
			: base(s)
		{
		}

		public EntityException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
