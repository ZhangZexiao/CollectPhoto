using System.Collections;
using System;
namespace TagLib.Id3v2
{
	public class UnknownFrame:
	Frame
	{
#region Private Properties
		private ByteVector field_data=null;
#endregion
#region Constructors
		public UnknownFrame(ByteVector type,ByteVector data):
		base(type,4)
		{
			field_data=data;
		}
		public UnknownFrame(ByteVector type):
		this(type,null)
		{
		}
		public UnknownFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal UnknownFrame(ByteVector data,int offset,FrameHeader header,byte version):
		base(header)
		{
			SetData(data,offset,version,false);
		}
#endregion
#region Public Properties
		public ByteVector Data
		{
			get
			{
				return field_data;
			}
			set
			{
				field_data=value;
			}
		}
#endregion
#region Public Methods
		public override string ToString()
		{
			return base.ToString();
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			field_data=data;
		}
		protected override ByteVector RenderFields(byte version)
		{
			return field_data??new ByteVector();
		}
#endregion
	}
}
