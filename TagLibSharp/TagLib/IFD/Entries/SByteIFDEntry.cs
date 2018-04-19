namespace TagLib.IFD.Entries
{
	public class SByteIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public sbyte Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public SByteIFDEntry(ushort tag,sbyte value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.SByte;
			count=1;
			return(byte)Value;
		}
#endregion
	}
}
