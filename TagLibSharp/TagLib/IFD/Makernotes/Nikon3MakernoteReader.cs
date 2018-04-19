using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
namespace TagLib.IFD.Makernotes
{
	public class Nikon3MakernoteReader:
	IFDReader
	{
#region Constructors
		public Nikon3MakernoteReader(File file,bool is_bigendian,IFDStructure structure,long base_offset,uint ifd_offset,uint max_offset):
		base(file,is_bigendian,structure,base_offset,ifd_offset,max_offset)
		{
		}
#endregion
#region Protected Methods
		protected override IFDEntry ParseIFDEntry(ushort tag,ushort type,uint count,long base_offset,uint offset)
		{
			if(tag==(ushort)Nikon3MakerNoteEntryTag.Preview)
			{
				type=(ushort)IFDEntryType.IFD;
				IFDStructure ifd_structure=new IFDStructure();
				IFDReader reader=CreateSubIFDReader(file,is_bigendian,ifd_structure,base_offset,offset,max_offset);
				reader.Read(1);
				return new SubIFDEntry(tag,type,(uint)ifd_structure.Directories.Length,ifd_structure);
			}
			return base.ParseIFDEntry(tag,type,count,base_offset,offset);
		}
#endregion
	}
}
