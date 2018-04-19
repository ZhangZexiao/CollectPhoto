using System;
using System.IO;
namespace TagLib.MusePack
{
	public struct StreamHeader:
	IAudioCodec
	{
#region Constants
		private static ushort[]sftable=
		{
			44100,48000,37800,32000
		};
#endregion
#region Private Fields
		private long stream_length;
		private int version;
		private uint header_data;
		private int sample_rate;
		private uint frames;
		private int channels;
		private ulong framecount;
#endregion
#region Public Static Fields
		public const uint SizeSV7=56;
		public static readonly ReadOnlyByteVector FileIdentifierSv7="MP+";
		public static readonly ReadOnlyByteVector FileIdentifierSv8="MPCK";
#endregion
#region Constructors
		public StreamHeader(File file,long streamLength)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			stream_length=streamLength;
			version=7;
			header_data=0;
			frames=0;
			sample_rate=0;
			channels=2;
			framecount=0;
			file.Seek(0);
			ByteVector magic=file.ReadBlock(4);
			if(magic.StartsWith(FileIdentifierSv7))
			{
				ReadSv7Properties(magic+file.ReadBlock((int)SizeSV7-4));
			}
			else if(magic.StartsWith(FileIdentifierSv8))
			{
				ReadSv8Properties(file);
			}
			else
			{
				throw new CorruptFileException("Data does not begin with identifier.");
			}
		}
#endregion
#region Private Methods
		private void ReadSv7Properties(ByteVector data)
		{
			if(data.Count<SizeSV7)
			{
				throw new CorruptFileException("Insufficient data in stream header");
			}
			version=data[3]&15;
			channels=2;
			if(version==7)
			{
				frames=data.Mid(4,4).ToUInt(false);
				uint flags=data.Mid(8,4).ToUInt(false);
				sample_rate=sftable[(int)(((flags>>17)&1)*2+((flags>>16)&1))];
				header_data=0;
			}
			else
			{
				header_data=data.Mid(0,4).ToUInt(false);
				version=(int)((header_data>>11)&0x03ff);
				sample_rate=44100;
				frames=data.Mid(4,version>=5?4:2).ToUInt(false);
			}
		}
		private void ReadSv8Properties(File file)
		{
			bool foundSH=false;
			while(!foundSH)
			{
				ByteVector packetType=file.ReadBlock(2);
				uint packetSizeLength=0;
				bool eof=false;
				ulong packetSize=ReadSize(file,ref packetSizeLength,ref eof);
				if(eof)
				{
					break;
				}
				ulong payloadSize=packetSize-2-packetSizeLength;
				ByteVector data=file.ReadBlock((int)payloadSize);
				if(packetType=="SH")
				{
					foundSH=true;
					if(payloadSize<=5)
					{
						break;
					}
					int pos=4;
					version=data[pos];
					pos+=1;
					frames=(uint)ReadSize(data,ref pos);
					if(pos>(uint)payloadSize-3)
					{
						break;
					}
					ulong beginSilence=ReadSize(data,ref pos);
					if(pos>(uint)payloadSize-2)
					{
						break;
					}
					ushort flags=data.Mid(pos,1).ToUShort(true);
					sample_rate=sftable[(flags>>13)&0x07];
					channels=((flags>>4)&0x0F)+1;
					framecount=frames-beginSilence;
				}
				else if(packetType=="SE")
				{
					break;
				}
				else
				{
					file.Seek((int)payloadSize,SeekOrigin.Current);
				}
			}
		}
		private ulong ReadSize(File file,ref uint packetSizeLength,ref bool eof)
		{
			uint tmp;
			ulong size=0;
			do
			{
				ByteVector b=file.ReadBlock(1);
				if(b.IsEmpty)
				{
					eof=true;
					break;
				}
				tmp=b.ToUInt();
				size=(size<<7)|(tmp&0x7F);
				packetSizeLength++;
			}
			while((tmp&0x80)==1);
			return size;
		}
		private ulong ReadSize(ByteVector data,ref int pos)
		{
			uint tmp;
			ulong size=0;
			do
			{
				tmp=data[pos++];
				size=(size<<7)|(tmp&0x7F);
			}
			while((tmp&0x80)==0x80&&pos<data.Count);
			return size;
		}
#endregion
#region Public Properties
		public TimeSpan Duration
		{
			get
			{
				if(sample_rate<=0&&stream_length<=0)
				{
					return TimeSpan.Zero;
				}
				if(version<=7)
				{
					return TimeSpan.FromSeconds((double)(frames*1152-576)/(double)sample_rate+0.5);
				}
				return TimeSpan.FromMilliseconds((double)(framecount*1000.0)/(double)sample_rate+0.5);
			}
		}
		public MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.Audio;
			}
		}
		public string Description
		{
			get
			{
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,"MusePack Version {0} Audio",Version);
			}
		}
		public int AudioBitrate
		{
			get
			{
				if(header_data!=0)
				{
					return(int)((header_data>>23)&0x01ff);
				}
				if(version<=7)
				{
					return(int)(Duration>TimeSpan.Zero?((stream_length*8L)/Duration.TotalSeconds)/1000:0);
				}
				return(int)(stream_length*8/Duration.TotalMilliseconds+0.5);
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return sample_rate;
			}
		}
		public int AudioChannels
		{
			get
			{
				return channels;
			}
		}
		public int Version
		{
			get
			{
				return version;
			}
		}
#endregion
#region IEquatable
		public override int GetHashCode()
		{
			unchecked
			{
				return(int)(header_data^sample_rate^frames^version);
			}
		}
		public override bool Equals(object other)
		{
			if(!(other is StreamHeader))
			{
				return false;
			}
			return Equals((StreamHeader)other);
		}
		public bool Equals(StreamHeader other)
		{
			return header_data==other.header_data&&sample_rate==other.sample_rate&&version==other.version&&frames==other.frames;
		}
		public static bool operator==(StreamHeader first,StreamHeader second)
		{
			return first.Equals(second);
		}
		public static bool operator!=(StreamHeader first,StreamHeader second)
		{
			return!first.Equals(second);
		}
#endregion
	}
}
