using System;
using System.Collections.Generic;
namespace TagLib.Mpeg4
{
	public class IsoUserDataBox:
	Box
	{
#region Private Fields
		private IEnumerable<Box>children;
		private BoxHeader[]parent_tree;
#endregion
#region Constructors
		public IsoUserDataBox(BoxHeader header,TagLib.File file,IsoHandlerBox handler):
		base(header,handler)
		{
			if(file==null)
			{
				throw new ArgumentNullException("file");
			}
			children=LoadChildren(file);
		}
		public IsoUserDataBox():
		base("udta")
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
		public BoxHeader[]ParentTree
		{
			get
			{
				return parent_tree;
			}
			set
			{
				parent_tree=value;
			}
		}
#endregion
	}
}
