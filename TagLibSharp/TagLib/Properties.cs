using System;
using System.Text;
using System.Collections.Generic;
namespace TagLib
{
	public class Properties:
	IAudioCodec,IVideoCodec,IPhotoCodec
	{
#region Private Fields
		private ICodec[]codecs=new ICodec[0];
		private TimeSpan duration=TimeSpan.Zero;
#endregion
#region Constructors
		public Properties()
		{
		}
		public Properties(TimeSpan duration,params ICodec[]codecs)
		{
			this.duration=duration;
			if(codecs!=null)
			{
				this.codecs=codecs;
			}
		}
		public Properties(TimeSpan duration,IEnumerable<ICodec>codecs)
		{
			this.duration=duration;
			if(codecs!=null)
			{
				this.codecs=new List<ICodec>(codecs).ToArray();
			}
		}
#endregion
#region Public Properties
		public IEnumerable<ICodec>Codecs
		{
			get
			{
				return codecs;
			}
		}
#endregion
#region ICodec
		public TimeSpan Duration
		{
			get
			{
				TimeSpan duration=this.duration;
				if(duration!=TimeSpan.Zero)
				{
					return duration;
				}
				foreach(ICodec codec in codecs)
				if(codec!=null&&codec.Duration>duration)
				{
					duration=codec.Duration;
				}
				return duration;
			}
		}
		public MediaTypes MediaTypes
		{
			get
			{
				MediaTypes types=MediaTypes.None;
				foreach(ICodec codec in codecs)
				if(codec!=null)
				{
					types|=codec.MediaTypes;
				}
				return types;
			}
		}
		public string Description
		{
			get
			{
				StringBuilder builder=new StringBuilder();
				foreach(ICodec codec in codecs)
				{
					if(codec==null)
					{
						continue;
					}
					if(builder.Length!=0)
					{
						builder.Append("; ");
					}
					builder.Append(codec.Description);
				}
				return builder.ToString();
			}
		}
#endregion
#region IAudioCodec
		public int AudioBitrate
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Audio)==0)
					{
						continue;
					}
					IAudioCodec audio=codec as IAudioCodec;
					if(audio!=null&&audio.AudioBitrate!=0)
					{
						return audio.AudioBitrate;
					}
				}
				return 0;
			}
		}
		public int AudioSampleRate
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Audio)==0)
					{
						continue;
					}
					IAudioCodec audio=codec as IAudioCodec;
					if(audio!=null&&audio.AudioSampleRate!=0)
					{
						return audio.AudioSampleRate;
					}
				}
				return 0;
			}
		}
		public int BitsPerSample
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Audio)==0)
					{
						continue;
					}
					ILosslessAudioCodec lossless=codec as ILosslessAudioCodec;
					if(lossless!=null&&lossless.BitsPerSample!=0)
					{
						return lossless.BitsPerSample;
					}
				}
				return 0;
			}
		}
		public int AudioChannels
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Audio)==0)
					{
						continue;
					}
					IAudioCodec audio=codec as IAudioCodec;
					if(audio!=null&&audio.AudioChannels!=0)
					{
						return audio.AudioChannels;
					}
				}
				return 0;
			}
		}
#endregion
#region IVideoCodec
		public int VideoWidth
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Video)==0)
					{
						continue;
					}
					IVideoCodec video=codec as IVideoCodec;
					if(video!=null&&video.VideoWidth!=0)
					{
						return video.VideoWidth;
					}
				}
				return 0;
			}
		}
		public int VideoHeight
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Video)==0)
					{
						continue;
					}
					IVideoCodec video=codec as IVideoCodec;
					if(video!=null&&video.VideoHeight!=0)
					{
						return video.VideoHeight;
					}
				}
				return 0;
			}
		}
#endregion
#region IPhotoCodec
		public int PhotoWidth
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Photo)==0)
					{
						continue;
					}
					IPhotoCodec photo=codec as IPhotoCodec;
					if(photo!=null&&photo.PhotoWidth!=0)
					{
						return photo.PhotoWidth;
					}
				}
				return 0;
			}
		}
		public int PhotoHeight
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Photo)==0)
					{
						continue;
					}
					IPhotoCodec photo=codec as IPhotoCodec;
					if(photo!=null&&photo.PhotoHeight!=0)
					{
						return photo.PhotoHeight;
					}
				}
				return 0;
			}
		}
		public int PhotoQuality
		{
			get
			{
				foreach(ICodec codec in codecs)
				{
					if(codec==null||(codec.MediaTypes&MediaTypes.Photo)==0)
					{
						continue;
					}
					IPhotoCodec photo=codec as IPhotoCodec;
					if(photo!=null&&photo.PhotoQuality!=0)
					{
						return photo.PhotoQuality;
					}
				}
				return 0;
			}
		}
#endregion
	}
}
