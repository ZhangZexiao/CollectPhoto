using System;
namespace TagLib
{
	[Flags]
	public enum MediaTypes
	{
		None=0,Audio=1,Video=2,Photo=4,Text=8
	}
	public interface ICodec
	{
		TimeSpan Duration
		{
			get;
		}
		MediaTypes MediaTypes
		{
			get;
		}
		string Description
		{
			get;
		}
	}
	public interface IAudioCodec:
	ICodec
	{
		int AudioBitrate
		{
			get;
		}
		int AudioSampleRate
		{
			get;
		}
		int AudioChannels
		{
			get;
		}
	}
	public interface ILosslessAudioCodec
	{
		int BitsPerSample
		{
			get;
		}
	}
	public interface IVideoCodec:
	ICodec
	{
		int VideoWidth
		{
			get;
		}
		int VideoHeight
		{
			get;
		}
	}
	public interface IPhotoCodec:
	ICodec
	{
		int PhotoWidth
		{
			get;
		}
		int PhotoHeight
		{
			get;
		}
		int PhotoQuality
		{
			get;
		}
	}
}
