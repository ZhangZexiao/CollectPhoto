using System;
using System.Collections.Generic;
namespace TagLib.Mpeg4
{
	public class AppleAnnotationBox:
	Box
	{
#region Private Fields
		private IEnumerable<Box>children;
#endregion
#region Constructors
		public AppleAnnotationBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,handler)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			children=LoadChildren(file);
		}
		public AppleAnnotationBox(ByteVector type):
		base(type)
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
