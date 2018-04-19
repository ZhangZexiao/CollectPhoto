using System;
namespace TagLib.Tiff
{
	public class Codec:
	Image.Codec
	{
		private string description="TIFF File";
		public override string Description
		{
			get
			{
				return description;
			}
		}
		public Codec(int width,int height):
		base(width,height)
		{
		}
		public Codec(int width,int height,string description):
		base(width,height)
		{
			this.description=description;
		}
	}
}
