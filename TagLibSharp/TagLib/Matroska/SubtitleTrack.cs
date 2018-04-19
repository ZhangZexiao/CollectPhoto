using System.Collections.Generic;
using System;
namespace TagLib.Matroska
{
	public class SubtitleTrack:
	Track
	{
#region Private fields
		private List<EBMLElement>unknown_elems=new List<EBMLElement>();
#endregion
#region Constructors
		public SubtitleTrack(File _file,EBMLElement element):
		base(_file,element)
		{
			foreach(EBMLElement elem in base.UnknownElements)
			{
				MatroskaID matroska_id=(MatroskaID)elem.ID;
				switch(matroska_id)
				{
				default:
					unknown_elems.Add(elem);
					break;
				}
			}
		}
#endregion
#region Public fields
		public new List<EBMLElement>UnknownElements
		{
			get
			{
				return unknown_elems;
			}
		}
#endregion
#region Public methods
#endregion
#region ICodec
		public override MediaTypes MediaTypes
		{
			get
			{
				return MediaTypes.Text;
			}
		}
#endregion
	}
}
