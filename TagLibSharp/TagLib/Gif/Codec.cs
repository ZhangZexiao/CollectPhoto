using System;
namespace TagLib.Gif
{
	public class Codec:
	Image.Codec
	{
		public override string Description
		{
			get
			{
				return"GIF File";
			}
		}
		public Codec(int width,int height):
		base(width,height)
		{
		}
	}
}
