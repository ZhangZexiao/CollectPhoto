namespace TagLib.IFD.Entries
{
	public class LongIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public uint Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public LongIFDEntry(ushort tag,uint value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Long;
			count=1;
			return ByteVector.FromUInt(Value,is_bigendian);
		}
#endregion
	}
}
