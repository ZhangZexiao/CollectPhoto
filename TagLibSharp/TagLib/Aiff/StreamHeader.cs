using System;
using System.Globalization;
namespace TagLib.Aiff
{
	public struct StreamHeader:
	IAudioCodec,ILosslessAudioCodec
	{
#region Private Fields
		private ushort channels;
		private ulong total_frames;
		private ushort bits_per_sample;
		private ulong sample_rate;
		private long stream_length;
#endregion
#region Public Static Fields
		public const uint Size=26;
		public static readonly ReadOnlyByteVector FileIdentifier="COMM";
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
			stream_length=streamLength;
			channels=data.Mid(8,2).ToUShort(true);
			total_frames=data.Mid(10,4).ToULong(true);
			bits_per_sample=data.Mid(14,2).ToUShort(true);
			ByteVector sample_rate_indicator=data.Mid(17,1);
			ulong sample_rate_tmp=data.Mid(18,2).ToULong(true);
			sample_rate=44100;
			switch(sample_rate_tmp)
			{
			case 44100:
				if(sample_rate_indicator==0x0E)
				{
					sample_rate=44100;
				}
				else if(sample_rate_indicator==0x0D)
				{
					sample_rate=22050;
				}
				else if(sample_rate_indicator==0x0C)
				{
					sample_rate=11025;
				}
				break;
			case 48000:
				if(sample_rate_indicator==0x0E)
				{
					sample_rate=48000;
				}
				else if(sample_rate_indicator==0x0D)
				{
					sample_rate=24000;
				}
				break;
			case 64000:
				if(sample_rate_indicator==0x0D)
				{
					sample_rate=32000;
				}
				else if(sample_rate_indicator==0x0C)
				{
					sample_rate=16000;
				}
				else if(sample_rate_indicator==0x0B)
				{
					sample_rate=8000;
				}
				break;
			case 44510:
				if(sample_rate_indicator==0x0D)
				{
					sample_rate=22255;
				}
				break;
			case 44508:
				if(sample_rate_indicator==0x0C)
				{
					sample_rate=11127;
				}
				break;
			}
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
				return TimeSpan.FromSeconds((double)total_frames/(double)sample_rate);
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
				return"AIFF Audio";
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
		public int BitsPerSample
		{
			get
			{
				return bits_per_sample;
			}
		}
#endregion
	}
}
