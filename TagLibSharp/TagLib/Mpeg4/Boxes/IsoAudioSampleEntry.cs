using System;
using System.Collections.Generic;
using System.Globalization;
namespace TagLib.Mpeg4
{
	public class IsoAudioSampleEntry:
	IsoSampleEntry,IAudioCodec
	{
#region Private Fields
		private ushort channel_count;
		private ushort sample_size;
		private uint sample_rate;
		private IEnumerable<Box>children;
#endregion
#region Constructors
		public IsoAudioSampleEntry(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,file,handler)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			file.Seek(base.DataPosition+8);
			channel_count=file.ReadBlock(2).ToUShort();
			sample_size=file.ReadBlock(2).ToUShort();
			file.Seek(base.DataPosition+16);
			sample_rate=file.ReadBlock(4).ToUInt();
			children=LoadChildren(file);
		}
#endregion
#region Public Properties
		protected override long DataPosition
		{
			get
			{
				return base.DataPosition+20;
			}
		}
		public override IEnumerable<Box>Children
		{
			get
			{
				return children;
			}
		}
#endregion
#region IAudioCodec Properties
		public TimeSpan Duration
		{
			get
			{
				return TimeSpan.Zero;
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
				return string.Format(CultureInfo.InvariantCulture,"MPEG-4 Audio ({0})",BoxType);
			}
		}
		public int AudioBitrate
		{
			get
			{
				AppleElementaryStreamDescriptor esds=GetChildRecursively("esds")
				as AppleElementaryStreamDescriptor;
				if(esds==null)
				{
					return 0;
				}
				return(int)esds.AverageBitrate;
			}
		}
		public int AudioSampleRate
		{
			get
			{
				return(int)(sample_rate>>16);
			}
		}
		public int AudioChannels
		{
			get
			{
				return channel_count;
			}
		}
		public int AudioSampleSize
		{
			get
			{
				return sample_size;
			}
		}
#endregion
	}
}
