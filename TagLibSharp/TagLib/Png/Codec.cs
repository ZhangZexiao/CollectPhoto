using System;
namespace TagLib.Png
{
	public class Codec:
	Image.Codec
	{
		public override string Description
		{
			get
			{
				return"PNG File";
			}
		}
		public Codec(int width,int height):
		base(width,height)
		{
		}
	}
}
