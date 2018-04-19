namespace TagLib.IFD.Entries
{
	public class ShortIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public ushort Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public ShortIFDEntry(ushort tag,ushort value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Short;
			count=1;
			return ByteVector.FromUShort(Value,is_bigendian);
		}
#endregion
	}
}
