using System;
namespace TagLib.WavPack
{
	public struct StreamHeader:
	IAudioCodec,ILosslessAudioCodec,IEquatable<StreamHeader>
	{
#region Constants
		private static readonly uint[]sample_rates=new uint[]
		{
			6000,
			8000,
			9600,
			11025,
			12000,
			16000,
			22050,
			24000,
			32000,
			44100,
			48000,
			64000,
			88200,
			96000,
			192000
		};
		private const int BYTES_STORED=3;
		private const int MONO_FLAG=4;
		private const int SHIFT_LSB=13;
		private const long SHIFT_MASK=(0x1fL<<SHIFT_LSB);
		private const int SRATE_LSB=23;
		private const long SRATE_MASK=(0xfL<<SRATE_LSB);
#endregion
#region Private Fields
		private long stream_length;
		private ushort version;
		private uint flags;
		private uint samples;
#endregion
#region Public Static Fields
		public const uint Size=32;
		public static readonly ReadOnlyByteVector FileIdentifier="wvpk";
#endregion
#region Constructors
		public StreamHeader(ByteVector data,long streamLength)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(!data.StartsWith(FileIdentifier))
			{
				throw new CorruptFileException("Data does not begin with identifier.");
			}
			if(data.Count<Size)
			{
				throw new CorruptFileException("Insufficient data in stream header");
			}
			stream_length=streamLength;
			version=data.Mid(8,2).ToUShort(false);
			flags=data.Mid(24,4).ToUInt(false);
			samples=data.Mid(12,4).ToUInt(false);
		}
#endregion
#region Public Properties
		public TimeSpan Duration
		{
			get
			{
				return AudioSampleRate>0?TimeSpan.FromSeconds((double)samples/(double)AudioSampleRate+0.5):TimeSpan.Zero;
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
				return string.Format(System.Globalization.CultureInfo.InvariantCulture,"WavPack Version {0} Audio",Version);
			}
		}
		public int AudioBitrate
		{
			get
			{
				return(int)(Duration>TimeSpan.Zero?((stream_length*8L)/Duration.TotalSeconds)/1000:0);
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return(int)(sample_rates[(flags&SRATE_MASK)>>SRATE_LSB]);
			}
		}
		public int AudioChannels
		{
			get
			{
				return((flags&MONO_FLAG)!=0)?1:2;
			}
		}
		public int Version
		{
			get
			{
				return version;
			}
		}
		public int BitsPerSample
		{
			get
			{
				return(int)(((flags&BYTES_STORED)+1)*8-((flags&SHIFT_MASK)>>SHIFT_LSB));
			}
		}
#endregion
#region IEquatable
		public override int GetHashCode()
		{
			unchecked
			{
				return(int)(flags^samples^version);
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
			return flags==other.flags&&samples==other.samples&&version==other.version;
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
