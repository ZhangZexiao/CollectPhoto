using System;
using System.Collections.Generic;
namespace TagLib.Mpeg4
{
	public class AppleItemListBox:
	Box
	{
#region Private Fields
		private IEnumerable<Box>children;
#endregion
#region Constructors
		public AppleItemListBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,handler)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			children=LoadChildren(file);
		}
		public AppleItemListBox():
		base("ilst")
		{
			children=new List<Box>();
		}
#endregion
#region Public Properties
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
