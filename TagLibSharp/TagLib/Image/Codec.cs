using System;
namespace TagLib.Image
{
	public abstract class Codec:
	IPhotoCodec
	{
#region Properties
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
				return MediaTypes.Photo;
			}
		}
		public abstract string Description
		{
			get;
		}
		public int PhotoWidth
		{
			get;
			protected set;
		}
		public int PhotoHeight
		{
			get;
			protected set;
		}
		public int PhotoQuality
		{
			get;
			protected set;
		}
#endregion
#region Constructors
		public Codec(int width,int height):
		this(width,height,0)
		{
		}
		public Codec(int width,int height,int quality)
		{
			PhotoWidth=width;
			PhotoHeight=height;
			PhotoQuality=quality;
		}
#endregion
	}
}
