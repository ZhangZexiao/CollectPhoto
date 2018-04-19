using System;
using TagLib.Image;
using TagLib.IFD;
namespace TagLib.Tiff
{
	public abstract class BaseTiffFile:
	TagLib.Image.File
	{
#region Public Properties
		public bool IsBigEndian
		{
			get;
			private set;
		}
#endregion
#region Protected Properties
		protected ushort Magic
		{
			get;
			set;
		}
#endregion
#region Constructors
		protected BaseTiffFile(string path):
		base(path)
		{
			Magic=42;
		}
		protected BaseTiffFile(IFileAbstraction abstraction):
		base(abstraction)
		{
			Magic=42;
		}
#endregion
#region Protected Methods
		protected uint ReadHeader()
		{
			ByteVector header=ReadBlock(8);
			if(header.Count!=8)
			{
				throw new CorruptFileException("Unexpected end of header");
			}
			string order=header.Mid(0,2).ToString();
			if(order=="II")
			{
				IsBigEndian=false;
			}
			else if(order=="MM")
			{
				IsBigEndian=true;
			}
			else
			{
				throw new CorruptFileException("Unknown Byte Order");
			}
			if(header.Mid(2,2).ToUShort(IsBigEndian)!=Magic)
			{
				throw new CorruptFileException(String.Format("TIFF Magic ({0}) expected",Magic));
			}
			uint first_ifd_offset=header.Mid(4,4).ToUInt(IsBigEndian);
			return first_ifd_offset;
		}
		protected void ReadIFD(uint offset)
		{
			ReadIFD(offset,-1);
		}
		protected void ReadIFD(uint offset,int ifd_count)
		{
			long length=0;
			try
			{
				length=Length;
			}
			catch(Exception)
			{
				length=1073741824L*4;
			}
			var ifd_tag=GetTag(TagTypes.TiffIFD,true)
			as IFDTag;
			var reader=CreateIFDReader(this,IsBigEndian,ifd_tag.Structure,0,offset,(uint)length);
			reader.Read(ifd_count);
		}
		protected virtual IFDReader CreateIFDReader(BaseTiffFile file,bool is_bigendian,IFDStructure structure,long base_offset,uint ifd_offset,uint max_offset)
		{
			return new IFDReader(file,is_bigendian,structure,base_offset,ifd_offset,max_offset);
		}
		protected ByteVector RenderHeader(uint first_ifd_offset)
		{
			ByteVector data=new ByteVector();
			if(IsBigEndian)
			{
				data.Add("MM");
			}
			else
			{
				data.Add("II");
			}
			data.Add(ByteVector.FromUShort(Magic,IsBigEndian));
			data.Add(ByteVector.FromUInt(first_ifd_offset,IsBigEndian));
			return data;
		}
#endregion
	}
}
