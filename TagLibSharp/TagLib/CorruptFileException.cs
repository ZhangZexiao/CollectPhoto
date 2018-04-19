using System;
using System.Runtime.Serialization;
namespace TagLib
{
	[Serializable]
	public class CorruptFileException:Exception
	{
		public CorruptFileException(string message):
		base(message)
		{
		}
		public CorruptFileException():
		base()
		{
		}
		public CorruptFileException(string message,Exception innerException):
		base(message,innerException)
		{
		}
		protected CorruptFileException(SerializationInfo info,StreamingContext context):
		base(info,context)
		{
		}
	}
}
