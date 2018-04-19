using System;
namespace TagLib.Mpeg4
{
	public class IsoFreeSpaceBox:
	Box
	{
#region Private Fields
		private long padding;
#endregion
#region Constructors
		public IsoFreeSpaceBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,handler)
		{
			padding=DataSize;
		}
		public IsoFreeSpaceBox(long padding):
		base("free")
		{
			PaddingSize=padding;
		}
#endregion
#region Public Properties
		public override ByteVector Data
		{
			get
			{
				return new ByteVector((int)padding);
			}
			set
			{
				padding=(value!=null)?value.Count:0;
			}
		}
		public long PaddingSize
		{
			get
			{
				return padding+8;
			}
			set
			{
				padding=value-8;
			}
		}
#endregion
	}
}
