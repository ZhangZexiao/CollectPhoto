namespace TagLib.IFD.Entries
{
	public class SLongIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public int Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public SLongIFDEntry(ushort tag,int value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.SLong;
			count=1;
			return ByteVector.FromInt(Value,is_bigendian);
		}
#endregion
	}
}
