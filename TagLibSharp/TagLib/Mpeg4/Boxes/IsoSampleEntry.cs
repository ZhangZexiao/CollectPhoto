using System;
namespace TagLib.Mpeg4
{
	public class IsoSampleEntry:
	Box
	{
#region Private Fields
		private ushort data_reference_index;
#endregion
#region Constructors
		public IsoSampleEntry(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,handler)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			file.Seek(base.DataPosition+6);
			data_reference_index=file.ReadBlock(2).ToUShort();
		}
#endregion
#region Public Properties
		protected override long DataPosition
		{
			get
			{
				return base.DataPosition+8;
			}
		}
		public ushort DataReferenceIndex
		{
			get
			{
				return data_reference_index;
			}
		}
#endregion
	}
}
