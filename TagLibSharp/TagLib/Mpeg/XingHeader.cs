using System.Collections;
using System;
namespace TagLib.Mpeg
{
	public struct XingHeader
	{
#region Private Fields
		private uint frames;
		private uint size;
		private bool present;
#endregion
#region Public Fields
		public static readonly ReadOnlyByteVector FileIdentifier="Xing";
		public static readonly XingHeader Unknown=new XingHeader(0,0);
#endregion
#region Constructors
		private XingHeader(uint frame,uint size)
		{
			this.frames=frame;
			this.size=size;
			this.present=false;
		}
		public XingHeader(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(!data.StartsWith(FileIdentifier))
			{
				throw new CorruptFileException("Not a valid Xing header");
			}
			int position=8;
			if((data[7]&0x01)!=0)
			{
				frames=data.Mid(position,4).ToUInt();
				position+=4;
			}
			else
			{
				frames=0;
			}
			if((data[7]&0x02)!=0)
			{
				size=data.Mid(position,4).ToUInt();
				position+=4;
			}
			else
			{
				size=0;
			}
			present=true;
		}
#endregion
#region Public Properties
		public uint TotalFrames
		{
			get
			{
				return frames;
			}
		}
		public uint TotalSize
		{
			get
			{
				return size;
			}
		}
		public bool Present
		{
			get
			{
				return present;
			}
		}
#endregion
#region Public Static Methods
		public static int XingHeaderOffset(Version version,ChannelMode channelMode)
		{
			bool single_channel=channelMode==ChannelMode.SingleChannel;
			if(version==Version.Version1)
			{
				return single_channel?0x15:0x24;
			}
			else
			{
				return single_channel?0x0D:0x15;
			}
		}
#endregion
	}
}
