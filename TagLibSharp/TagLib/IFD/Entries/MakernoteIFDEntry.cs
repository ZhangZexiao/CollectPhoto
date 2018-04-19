namespace TagLib.IFD.Entries
{
	public enum MakernoteType
	{
		Unknown,Canon,Panasonic,Leica,Pentax,Nikon1,Nikon2,Nikon3,Olympus1,Olympus2,Sony
	}
	public class MakernoteIFDEntry:
	IFDEntry
	{
#region Private Fields
		private ByteVector prefix;
		private uint ifd_offset;
		private bool absolute_offset;
		private bool?is_bigendian;
#endregion
#region Properties
		public ushort Tag
		{
			get;
			private set;
		}
		public MakernoteType MakernoteType
		{
			get;
			private set;
		}
		public IFDStructure Structure
		{
			get;
			private set;
		}
#endregion
#region Constructors
		public MakernoteIFDEntry(ushort tag,IFDStructure structure,MakernoteType makernote_type,ByteVector prefix,uint ifd_offset,bool absolute_offset,bool?is_bigendian)
		{
			Tag=tag;
			Structure=structure;
			MakernoteType=makernote_type;
			this.prefix=prefix;
			this.ifd_offset=ifd_offset;
			this.absolute_offset=absolute_offset;
			this.is_bigendian=is_bigendian;
		}
		public MakernoteIFDEntry(ushort tag,IFDStructure structure,MakernoteType makernote_type):
		this(tag,structure,makernote_type,null,0,true,null)
		{
		}
#endregion
#region Public Methods
		public ByteVector Render(bool is_bigendian,uint offset,out ushort type,out uint count)
		{
			type=(ushort)IFDEntryType.Undefined;
			var renderer=new IFDRenderer(this.is_bigendian??is_bigendian,Structure,absolute_offset?offset+ifd_offset:ifd_offset);
			ByteVector data=renderer.Render();
			data.Insert(0,prefix);
			count=(uint)data.Count;
			return data;
		}
#endregion
	}
}
