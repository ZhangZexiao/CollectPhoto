using System.Collections;
using System;
namespace TagLib.Id3v2
{
	public class MusicCdIdentifierFrame:
	Frame
	{
#region Private Properties
		private ByteVector field_data=null;
#endregion
#region Constructors
		public MusicCdIdentifierFrame():
		base(FrameType.MCDI,4)
		{
		}
		public MusicCdIdentifierFrame(ByteVector data,byte version):
		base(data,version)
		{
			SetData(data,0,version,true);
		}
		protected internal MusicCdIdentifierFrame(ByteVector data,int offset,FrameHeader header,byte version):
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
#region Public Static Methods
		public static MusicCdIdentifierFrame Get(Tag tag,bool create)
		{
			MusicCdIdentifierFrame mcdi;
			foreach(Frame frame in tag)
			{
				mcdi=frame as MusicCdIdentifierFrame;
				if(mcdi!=null)
				{
					return mcdi;
				}
			}
			if(!create)
			{
				return null;
			}
			mcdi=new MusicCdIdentifierFrame();
			tag.AddFrame(mcdi);
			return mcdi;
		}
#endregion
#region Protected Methods
		protected override void ParseFields(ByteVector data,byte version)
		{
			field_data=data;
		}
		protected override ByteVector RenderFields(byte version)
		{
			return field_data!=null?field_data:new ByteVector();
		}
#endregion
#region ICloneable
		public override Frame Clone()
		{
			MusicCdIdentifierFrame frame=new MusicCdIdentifierFrame();
			if(field_data!=null)
			{
				frame.field_data=new ByteVector(field_data);
			}
			return frame;
		}
#endregion
	}
}
