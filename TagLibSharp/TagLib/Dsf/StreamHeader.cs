using System;
using System.Globalization;
namespace TagLib.Dsf
{
	public struct StreamHeader:
	IAudioCodec,ILosslessAudioCodec
	{
#region Private Fields
		private ushort version;
		private ushort format_id;
		private ushort channel_type;
		private ushort channels;
		private ulong sample_rate;
		private ushort bits_per_sample;
		private ulong sample_count;
		private uint channel_blksize;
		private long stream_length;
#endregion
#region Public Static Fields
		public const uint Size=52;
		public static readonly ReadOnlyByteVector FileIdentifier="fmt ";
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
			version=data.Mid(12,4).ToUShort(false);
			format_id=data.Mid(16,4).ToUShort(false);
			channel_type=data.Mid(20,4).ToUShort(false);
			channels=data.Mid(24,4).ToUShort(false);
			sample_rate=data.Mid(28,4).ToULong(false);
			bits_per_sample=data.Mid(32,4).ToUShort(false);
			sample_count=data.Mid(36,8).ToULong(false);
			channel_blksize=data.Mid(44,4).ToUShort(false);
		}
#endregion
#region Public Properties
		public TimeSpan Duration
		{
			get
			{
				if(sample_rate<=0||sample_count<=0)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromSeconds((double)sample_count/(double)sample_rate);
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
				return"DSF Audio";
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
