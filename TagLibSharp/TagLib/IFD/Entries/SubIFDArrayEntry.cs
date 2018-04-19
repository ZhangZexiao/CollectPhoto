using System;
using System.Collections.Generic;
namespace TagLib.IFD.Entries
{
	public class SubIFDArrayEntry:
	IFDEntry
	{
		public ushort Tag
		{
			get;
			set;
		}
		public IFDStructure[]Entries
		{
			get;
			private set;
		}
#region Constructors
		public SubIFDArrayEntry(ushort tag,List<IFDStructure>entries)
		{
			Tag=tag;
			Entries=entries.ToArray();
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			throw new NotImplementedException("Not implemented yet!");
		}
#endregion
	}
}
