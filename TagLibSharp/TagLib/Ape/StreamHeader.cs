using System;
using System.Globalization;
namespace TagLib.Ape
{
	public enum CompressionLevel
	{
		None=0,Fast=1000,Normal=2000,High=3000,ExtraHigh=4000,Insane
	}
	public struct StreamHeader:
	IAudioCodec,ILosslessAudioCodec
	{
#region Private Fields
		private ushort version;
		private CompressionLevel compression_level;
		private uint blocks_per_frame;
		private uint final_frame_blocks;
		private uint total_frames;
		private ushort bits_per_sample;
		private ushort channels;
		private uint sample_rate;
		private long stream_length;
#endregion
#region Public Static Fields
		public const uint Size=76;
		public static readonly ReadOnlyByteVector FileIdentifier="MAC ";
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
			version=data.Mid(4,2).ToUShort(false);
			compression_level=(CompressionLevel)data.Mid(52,2).ToUShort(false);
			blocks_per_frame=data.Mid(56,4).ToUInt(false);
			final_frame_blocks=data.Mid(60,4).ToUInt(false);
			total_frames=data.Mid(64,4).ToUInt(false);
			bits_per_sample=data.Mid(68,2).ToUShort(false);
			channels=data.Mid(70,2).ToUShort(false);
			sample_rate=data.Mid(72,4).ToUInt(false);
		}
#endregion
#region Public Properties
		public TimeSpan Duration
		{
			get
			{
				if(sample_rate<=0||total_frames<=0)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromSeconds((double)((total_frames-1)*blocks_per_frame+final_frame_blocks)/(double)sample_rate);
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
				return string.Format(CultureInfo.InvariantCulture,"Monkey's Audio APE Version {0:0.000}",Version);
			}
		}
		public int AudioBitrate
		{
			get
			{
				TimeSpan d=Duration;
				if(d<=TimeSpan.Zero)
				{
					return 0;
				}
				return(int)((stream_length*8L)/d.TotalSeconds)/1000;
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return(int)sample_rate;
			}
		}
		public int AudioChannels
		{
			get
			{
				return channels;
			}
		}
		public double Version
		{
			get
			{
				return(double)version/(double)1000;
			}
		}
		public int BitsPerSample
		{
			get
			{
				return bits_per_sample;
			}
		}
		public CompressionLevel Compression
		{
			get
			{
				return compression_level;
			}
		}
#endregion
	}
}
