namespace TagLib.IFD.Entries
{
	public class ByteIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public byte Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public ByteIFDEntry(ushort tag,byte value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Byte;
			count=1;
			return Value;
		}
#endregion
	}
}
