using System;
using System.Text;
namespace TagLib.Riff
{
	public abstract class AviStream
	{
		private AviStreamHeader header;
		private ICodec codec;
		protected AviStream(AviStreamHeader header)
		{
			this.header=header;
		}
		public virtual void ParseItem(ByteVector id,ByteVector data,int start,int length)
		{
		}
		public AviStreamHeader Header
		{
			get
			{
				return header;
			}
		}
		public ICodec Codec
		{
			get
			{
				return codec;
			}
			protected set
			{
				this.codec=value;
			}
		}
		public static AviStream ParseStreamList(ByteVector data)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(!data.StartsWith("strl"))
			{
				return null;
			}
			AviStream stream=null;
			int pos=4;
			while(pos+8<data.Count)
			{
				ByteVector id=data.Mid(pos,4);
				int block_length=(int)data.Mid(pos+4,4).ToUInt(false);
				if(id=="strh"&&stream==null)
				{
					AviStreamHeader stream_header=new AviStreamHeader(data,pos+8);
					if(stream_header.Type=="vids")
					{
						stream=new AviVideoStream(stream_header);
					}
					else if(stream_header.Type=="auds")
					{
						stream=new AviAudioStream(stream_header);
					}
				}
				else if(stream!=null)
				{
					stream.ParseItem(id,data,pos+8,block_length);
				}
				pos+=block_length+8;
			}
			return stream;
		}
	}
	public class AviAudioStream:
	AviStream
	{
		public AviAudioStream(AviStreamHeader header):
		base(header)
		{
		}
		public override void ParseItem(ByteVector id,ByteVector data,int start,int length)
		{
			if(id=="strf")
			{
				Codec=new WaveFormatEx(data,start);
			}
		}
	}
	public class AviVideoStream:
	AviStream
	{
		public AviVideoStream(AviStreamHeader header):
		base(header)
		{
		}
		public override void ParseItem(ByteVector id,ByteVector data,int start,int length)
		{
			if(id=="strf")
			{
				Codec=new BitmapInfoHeader(data,start);
			}
		}
	}
	public struct AviStreamHeader
	{
		private ByteVector type;
		private ByteVector handler;
		private uint flags;
		private uint priority;
		private uint initial_frames;
		private uint scale;
		private uint rate;
		private uint start;
		private uint length;
		private uint suggested_buffer_size;
		private uint quality;
		private uint sample_size;
		private ushort left;
		private ushort top;
		private ushort right;
		private ushort bottom;
		[Obsolete("Use WaveFormatEx(ByteVector,int)")]
		public AviStreamHeader(ByteVector data):this(data,0)
		{
		}
		public AviStreamHeader(ByteVector data,int offset)
		{
			if(data==null)
			{
				throw new System.ArgumentNullException("data");
			}
			if(offset<0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if(offset+56>data.Count)
			{
				throw new CorruptFileException("Expected 56 bytes.");
			}
			type=data.Mid(offset,4);
			handler=data.Mid(offset+4,4);
			flags=data.Mid(offset+8,4).ToUInt(false);
			priority=data.Mid(offset+12,4).ToUInt(false);
			initial_frames=data.Mid(offset+16,4).ToUInt(false);
			scale=data.Mid(offset+20,4).ToUInt(false);
			rate=data.Mid(offset+24,4).ToUInt(false);
			start=data.Mid(offset+28,4).ToUInt(false);
			length=data.Mid(offset+32,4).ToUInt(false);
			suggested_buffer_size=data.Mid(offset+36,4).ToUInt(false);
			quality=data.Mid(offset+40,4).ToUInt(false);
			sample_size=data.Mid(offset+44,4).ToUInt(false);
			left=data.Mid(offset+48,2).ToUShort(false);
			top=data.Mid(offset+50,2).ToUShort(false);
			right=data.Mid(offset+52,2).ToUShort(false);
			bottom=data.Mid(offset+54,2).ToUShort(false);
		}
		public ByteVector Type
		{
			get
			{
				return type;
			}
		}
		public ByteVector Handler
		{
			get
			{
				return handler;
			}
		}
		public uint Flags
		{
			get
			{
				return flags;
			}
		}
		public uint Priority
		{
			get
			{
				return priority;
			}
		}
		public uint InitialFrames
		{
			get
			{
				return initial_frames;
			}
		}
		public uint Scale
		{
			get
			{
				return scale;
			}
		}
		public uint Rate
		{
			get
			{
				return rate;
			}
		}
		public uint Start
		{
			get
			{
				return start;
			}
		}
		public uint Length
		{
			get
			{
				return length;
			}
		}
		public uint SuggestedBufferSize
		{
			get
			{
				return suggested_buffer_size;
			}
		}
		public uint Quality
		{
			get
			{
				return quality;
			}
		}
		public uint SampleSize
		{
			get
			{
				return sample_size;
			}
		}
		public ushort Left
		{
			get
			{
				return left;
			}
		}
		public ushort Top
		{
			get
			{
				return top;
			}
		}
		public ushort Right
		{
			get
			{
				return right;
			}
		}
		public ushort Bottom
		{
			get
			{
				return bottom;
			}
		}
	}
}
