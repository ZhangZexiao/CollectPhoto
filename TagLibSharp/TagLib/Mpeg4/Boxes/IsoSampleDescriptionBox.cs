using System;
using System.Collections.Generic;
namespace TagLib.Mpeg4
{
	public class IsoSampleDescriptionBox:
	FullBox
	{
#region Private Fields
		private uint entry_count;
		private IEnumerable<Box>children;
#endregion
#region Constructors
		public IsoSampleDescriptionBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,file,handler)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			entry_count=file.ReadBlock(4).ToUInt();
			children=LoadChildren(file);
		}
#endregion
#region Public Properties
		protected override long DataPosition
		{
			get
			{
				return base.DataPosition+4;
			}
		}
		public uint EntryCount
		{
			get
			{
				return entry_count;
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
	}
}
