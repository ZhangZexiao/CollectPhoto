using System.Collections;
using System;
namespace TagLib.Id3v2
{
	public class ExtendedHeader:
	ICloneable
	{
		private uint size;
		public ExtendedHeader()
		{
		}
		public ExtendedHeader(ByteVector data,byte version)
		{
			Parse(data,version);
		}
		public uint Size
		{
			get
			{
				return size;
			}
		}
		protected void Parse(ByteVector data,byte version)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			size=(version==3?4u:0u)+SynchData.ToUInt(data.Mid(0,4));
		}
#region ICloneable
		public ExtendedHeader Clone()
		{
			ExtendedHeader header=new ExtendedHeader();
			header.size=size;
			return header;
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
#endregion
	}
}
