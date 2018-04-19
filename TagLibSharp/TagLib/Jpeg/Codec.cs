using System;
namespace TagLib.Jpeg
{
	public class Codec:
	Image.Codec
	{
		public override string Description
		{
			get
			{
				return"JFIF File";
			}
		}
		public Codec(int width,int height,int quality):
		base(width,height,quality)
		{
		}
	}
}
