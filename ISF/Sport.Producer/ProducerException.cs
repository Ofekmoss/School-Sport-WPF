using System;
using System.Runtime.Serialization;

namespace Sport.Producer
{
	public class ProducerException : Exception
	{
		public ProducerException()
		{
		}

		public ProducerException(string s)
			: base(s)
		{
		}

		public ProducerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
