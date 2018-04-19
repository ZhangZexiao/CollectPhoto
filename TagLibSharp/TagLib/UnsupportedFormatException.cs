using System;
using System.Runtime.Serialization;
namespace TagLib
{
	[Serializable]
	public class UnsupportedFormatException:Exception
	{
		public UnsupportedFormatException(string message):
		base(message)
		{
		}
		public UnsupportedFormatException():
		base()
		{
		}
		public UnsupportedFormatException(string message,Exception innerException):
		base(message,innerException)
		{
		}
		protected UnsupportedFormatException(SerializationInfo info,StreamingContext context):
		base(info,context)
		{
		}
	}
}
