using System.Collections;
using System;
namespace TagLib.Flac
{
	public struct StreamHeader:
	IAudioCodec,ILosslessAudioCodec
	{
#region Private Properties
		private uint flags;
		private uint low_length;
		private long stream_length;
#endregion
#region Constructors
		public StreamHeader(ByteVector data,long streamLength)
		{
			if(data==null)
			{
				throw new ArgumentNullException("data");
			}
			if(data.Count<18)
			{
				throw new CorruptFileException("Not enough data in FLAC header.");
			}
			this.stream_length=streamLength;
			this.flags=data.Mid(10,4).ToUInt(true);
			low_length=data.Mid(14,4).ToUInt(true);
		}
#endregion
#region Public Properties
		public TimeSpan Duration
		{
			get
			{
				return(AudioSampleRate>0&&stream_length>0)?TimeSpan.FromSeconds((double)low_length/(double)AudioSampleRate+(double)HighLength):TimeSpan.Zero;
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
				return(int)(flags>>12);
			}
		}
		public int AudioChannels
		{
			get
			{
				return(int)(((flags>>9)&7)+1);
			}
		}
		public MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.Audio;
			}
		}
		[Obsolete("This property is depreciated, use BitsPerSample instead")]
		public int AudioSampleWidth
		{
			get
			{
				return BitsPerSample;
			}
		}
		public int BitsPerSample
		{
			get
			{
				return(int)(((flags>>4)&31)+1);
			}
		}
		public string Description
		{
			get
			{
				return"Flac Audio";
			}
		}
#endregion
#region Private Properties
		private uint HighLength
		{
			get
			{
				return(uint)(AudioSampleRate>0?(((flags&0xf)<<28)/AudioSampleRate)<<4:0);
			}
		}
#endregion
	}
}
