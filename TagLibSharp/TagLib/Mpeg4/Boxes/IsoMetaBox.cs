using System;
using System.Collections.Generic;
namespace TagLib.Mpeg4
{
	public class IsoMetaBox:
	FullBox
	{
#region Private Fields
		private IEnumerable<Box>children;
#endregion
#region Constructors
		public IsoMetaBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,file,handler)
		{
			children=LoadChildren(file);
		}
		public IsoMetaBox(ByteVector handlerType,string handlerName):
		base("meta",0,0)
		{
			if(handlerType==null)
			{
				throw new ArgumentNullException("handlerType");
			}
			if(handlerType.Count<4)
			{
				throw new ArgumentException("The handler type must be four bytes long.","handlerType");
			}
			children=new List<Box>();
			AddChild(new IsoHandlerBox(handlerType,handlerName));
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
