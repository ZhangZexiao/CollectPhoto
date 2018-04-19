namespace TagLib.IFD.Entries
{
	public class SubIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public ushort Type
		{
			get;
			private set;
		}
		public uint Count
		{
			get;
			private set;
		}
		public IFDStructure Structure
		{
			get;
			private set;
		}
		public int ChildCount
		{
			get
			{
				int sum=0;
				foreach(var directory in Structure.Directories)
				sum+=directory.Count;
				return sum;
			}
		}
		public SubIFDEntry(ushort tag,ushort type,uint count,IFDStructure structure)
		{
			Tag=tag;
			Type=type;
			Count=count;
			Structure=structure;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)Type;
			count=1;
			count=Count;
			return new IFDRenderer(is_bigendian,Structure,offset).Render();
		}
#endregion
	}
}
