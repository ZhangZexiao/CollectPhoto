namespace TagLib.IFD.Entries
{
	public class StringIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public string Value
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public StringIFDEntry(ushort tag,string value)
		{
			Tag=tag;
			Value=value;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Ascii;
			ByteVector data=Value;
			data.Add("\0");
			count=(uint)data.Count;
			return data;
		}
#endregion
	}
}
