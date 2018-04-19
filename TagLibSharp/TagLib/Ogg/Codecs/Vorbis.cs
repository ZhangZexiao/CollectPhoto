using System;
namespace TagLib.Ogg.Codecs
{
	public class Vorbis:
	Codec,IAudioCodec
	{
#region Private Static Fields
		private static ByteVector id="vorbis";
#endregion
#region Private Fields
		private HeaderPacket header;
		private ByteVector comment_data;
#endregion
#region Constructors
		private Vorbis()
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
			if(type!=1&&index==0)
			{
				throw new CorruptFileException("Stream does not begin with vorbis header.");
			}
			if(comment_data==null)
			{
				if(type==1)
				{
					header=new HeaderPacket(packet);
				}
				else if(type==3)
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
			return header.sample_rate==0?TimeSpan.Zero:TimeSpan.FromSeconds((double)(lastGranularPosition-firstGranularPosition)/(double)header.sample_rate);
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
			ByteVector data=new ByteVector((byte)0x03);
			data.Add(id);
			data.Add(comment.Render(true));
			if(packets.Count>1&&PacketType(packets[1])==0x03)
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
		public int AudioBitrate
		{
			get
			{
				return(int)((float)header.bitrate_nominal/1000f+0.5);
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return(int)header.sample_rate;
			}
		}
		public int AudioChannels
		{
			get
			{
				return(int)header.channels;
			}
		}
		public override MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.Audio;
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
				return string.Format("Vorbis Version {0} Audio",header.vorbis_version);
			}
		}
#endregion
#region Public Static Methods
		public static Codec FromPacket(ByteVector packet)
		{
			return(PacketType(packet)==1)?new Vorbis():null;
		}
#endregion
#region Private Static Methods
		private static int PacketType(ByteVector packet)
		{
			if(packet.Count<=id.Count)
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
			public uint sample_rate;
			public uint channels;
			public uint vorbis_version;
			public uint bitrate_maximum;
			public uint bitrate_nominal;
			public uint bitrate_minimum;
			public HeaderPacket(ByteVector data)
			{
				vorbis_version=data.Mid(7,4).ToUInt(false);
				channels=data[11];
				sample_rate=data.Mid(12,4).ToUInt(false);
				bitrate_maximum=data.Mid(16,4).ToUInt(false);
				bitrate_nominal=data.Mid(20,4).ToUInt(false);
				bitrate_minimum=data.Mid(24,4).ToUInt(false);
			}
		}
	}
}
