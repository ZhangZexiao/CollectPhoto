using System;
namespace TagLib.Ogg.Codecs
{
	public class Theora:
	Codec,IVideoCodec
	{
#region Private Static Fields
		private static ByteVector id="theora";
#endregion
#region Private Fields
		private HeaderPacket header;
		private ByteVector comment_data;
#endregion
#region Constructors
		private Theora()
		{
		}
#endregion
#region Public Methods
		public override bool ReadPacket(ByteVector packet,int index)
		{
			if(packet==null)
			{
				throw new ArgumentNullException("packet");
			}
			if(index<0)
			{
				throw new ArgumentOutOfRangeException("index","index must be at least zero.");
			}
			int type=PacketType(packet);
			if(type!=0x80&&index==0)
			{
				throw new CorruptFileException("Stream does not begin with theora header.");
			}
			if(comment_data==null)
			{
				if(type==0x80)
				{
					header=new HeaderPacket(packet);
				}
				else if(type==0x81)
				{
					comment_data=packet.Mid(7);
				}
				else
				{
					return true;
				}
			}
			return comment_data!=null;
		}
		public override TimeSpan GetDuration(long firstGranularPosition,long lastGranularPosition)
		{
			return TimeSpan.FromSeconds(header.GranuleTime(lastGranularPosition)-header.GranuleTime(firstGranularPosition));
		}
		public override void SetCommentPacket(ByteVectorCollection packets,XiphComment comment)
		{
			if(packets==null)
			{
				throw new ArgumentNullException("packets");
			}
			if(comment==null)
			{
				throw new ArgumentNullException("comment");
			}
			ByteVector data=new ByteVector((byte)0x81);
			data.Add(id);
			data.Add(comment.Render(true));
			if(packets.Count>1&&PacketType(packets[1])==0x81)
			{
				packets[1]=data;
			}
			else
			{
				packets.Insert(1,data);
			}
		}
#endregion
#region Public Properties
		public int VideoWidth
		{
			get
			{
				return header.width;
			}
		}
		public int VideoHeight
		{
			get
			{
				return header.height;
			}
		}
		public override MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.Video;
			}
		}
		public override ByteVector CommentData
		{
			get
			{
				return comment_data;
			}
		}
		public override string Description
		{
			get
			{
				return string.Format("Theora Version {0}.{1} Video",header.major_version,header.minor_version);
			}
		}
#endregion
#region Public Static Methods
		public static Codec FromPacket(ByteVector packet)
		{
			return(PacketType(packet)==0x80)?new Theora():null;
		}
#endregion
#region Private Static Methods
		private static int PacketType(ByteVector packet)
		{
			if(packet.Count<=id.Count||packet[0]<0x80)
			{
				return-1;
			}
			for(int i=0;i<id.Count;i++)
			{
				if(packet[i+1]!=id[i])
				{
					return-1;
				}
			}
			return packet[0];
		}
#endregion
		private struct HeaderPacket
		{
			public byte major_version;
			public byte minor_version;
			public byte revision_version;
			public int width;
			public int height;
			public int fps_numerator;
			public int fps_denominator;
			public int keyframe_granule_shift;
			public HeaderPacket(ByteVector data)
			{
				major_version=data[7];
				minor_version=data[8];
				revision_version=data[9];
				width=(int)data.Mid(14,3).ToUInt();
				height=(int)data.Mid(17,3).ToUInt();
				fps_numerator=(int)data.Mid(22,4).ToUInt();
				fps_denominator=(int)data.Mid(26,4).ToUInt();
				ushort last_bits=data.Mid(40,2).ToUShort();
				keyframe_granule_shift=(last_bits>>5)&0x1F;
			}
			public double GranuleTime(long granularPosition)
			{
				long iframe=granularPosition>>keyframe_granule_shift;
				long pframe=granularPosition-(iframe<<keyframe_granule_shift);
				return(iframe+pframe)*((double)fps_denominator/(double)fps_numerator);
			}
		}
	}
}
