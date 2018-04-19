namespace TagLib.IFD.Entries
{
	public class ThumbnailDataIFDEntry:
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
		public ThumbnailDataIFDEntry(ushort tag,ByteVector data)
		{
			Tag=tag;
			Data=data;
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Long;
			count=1;
			return Data;
		}
#endregion
	}
}
