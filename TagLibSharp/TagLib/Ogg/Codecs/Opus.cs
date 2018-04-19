using System;
namespace TagLib.Ogg.Codecs
{
	public class Opus:
	Codec,IAudioCodec
	{
#region Private Static Fields
		private static ByteVector magic_signature_base="Opus";
		private static ByteVector magic_signature_header="OpusHead";
		private static ByteVector magic_signature_comment="OpusTags";
		private static int magic_signature_length=8;
#endregion
#region Private Fields
		private HeaderPacket header;
		private ByteVector comment_data;
#endregion
#region Constructors
		private Opus()
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
			ByteVector signature=MagicSignature(packet);
			if(signature!=magic_signature_header&&index==0)
			{
				throw new CorruptFileException("Stream does not begin with opus header.");
			}
			if(comment_data==null)
			{
				if(signature==magic_signature_header)
				{
					header=new HeaderPacket(packet);
				}
				else if(signature==magic_signature_comment)
				{
					comment_data=packet.Mid(magic_signature_length);
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
			return TimeSpan.FromSeconds((double)(lastGranularPosition-firstGranularPosition-2*header.pre_skip)/(double)48000);
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
			ByteVector data=new ByteVector();
			data.Add(magic_signature_comment);
			data.Add(comment.Render(true));
			if(packets.Count>1&&MagicSignature(packets[1])==magic_signature_comment)
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
				return 0;
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return(int)header.input_sample_rate;
			}
		}
		public int AudioChannels
		{
			get
			{
				return(int)header.channel_count;
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
				return string.Format("Opus Version {0} Audio",header.opus_version);
			}
		}
#endregion
#region Public Static Methods
		public static Codec FromPacket(ByteVector packet)
		{
			return(MagicSignature(packet)==magic_signature_header)?new Opus():null;
		}
#endregion
#region Private Static Methods
		private static ByteVector MagicSignature(ByteVector packet)
		{
			if(packet.Count<magic_signature_length)
			{
				return null;
			}
			for(int i=0;i<magic_signature_base.Count;i++)
			{
				if(packet[i]!=magic_signature_base[i])
				{
					return null;
				}
			}
			return packet.Mid(0,magic_signature_length);
		}
#endregion
		private struct HeaderPacket
		{
			public uint opus_version;
			public uint channel_count;
			public uint pre_skip;
			public uint input_sample_rate;
			public uint output_gain;
			public uint channel_map;
			public uint stream_count;
			public uint two_channel_stream_count;
			public uint[]channel_mappings;
			public HeaderPacket(ByteVector data)
			{
				opus_version=data[8];
				channel_count=data[9];
				pre_skip=data.Mid(10,2).ToUInt(false);
				input_sample_rate=data.Mid(12,4).ToUInt(false);
				output_gain=data.Mid(16,2).ToUInt(false);
				channel_map=data[18];
				if(channel_map==0)
				{
					stream_count=1;
					two_channel_stream_count=channel_count-1;
					channel_mappings=new uint[channel_count];
					channel_mappings[0]=0;
					if(channel_count==2)
					{
						channel_mappings[1]=1;
					}
				}
				else
				{
					stream_count=data[19];
					two_channel_stream_count=data[20];
					channel_mappings=new uint[channel_count];
					for(int i=0;i<channel_count;i++)
					{
						channel_mappings[i]=data[21+i];
					}
				}
			}
		}
	}
}
