using System;
using System.Globalization;
namespace TagLib.Mpeg4
{
	public class IsoVisualSampleEntry:
	IsoSampleEntry,IVideoCodec
	{
#region Private Fields
		private ushort width;
		private ushort height;
#endregion
#region Constructors
		public IsoVisualSampleEntry(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,file,handler)
		{
			file.Seek(base.DataPosition+16);
			width=file.ReadBlock(2).ToUShort();
			height=file.ReadBlock(2).ToUShort();
		}
#endregion
#region Public Properties
		protected override long DataPosition
		{
			get
			{
				return base.DataPosition+62;
			}
		}
#endregion
#region IVideoCodec Properties
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
				return MediaTypes.Video;
			}
		}
		public string Description
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture,"MPEG-4 Video ({0})",BoxType);
			}
		}
		public int VideoWidth
		{
			get
			{
				return width;
			}
		}
		public int VideoHeight
		{
			get
			{
				return height;
			}
		}
#endregion
	}
}
