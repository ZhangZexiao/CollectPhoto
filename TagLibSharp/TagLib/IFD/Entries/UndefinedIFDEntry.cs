namespace TagLib.IFD.Entries
{
	public class UndefinedIFDEntry:
	IFDEntry
	{
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public ByteVector Data
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public UndefinedIFDEntry(ushort tag,ByteVector data)
		{
			Tag=tag;
			Data=data;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Undefined;
			count=(uint)Data.Count;
			return Data;
		}
#endregion
	}
}
