using System.IO;
using TagLib.IFD;
namespace TagLib.Tiff.Rw2
{
	public class IFDReader:
	TagLib.IFD.IFDReader
	{
#region Constructors
		public IFDReader(BaseTiffFile file,bool is_bigendian,IFDStructure structure,long base_offset,uint ifd_offset,uint max_offset):
		base(file,is_bigendian,structure,base_offset,ifd_offset,max_offset)
		{
		}
#endregion
		protected override IFDEntry ParseIFDEntry(ushort tag,ushort type,uint count,long base_offset,uint offset)
		{
			if(tag==0x002e&& !seen_jpgfromraw)
			{
				file.Seek(base_offset+offset,SeekOrigin.Begin);
				var data=file.ReadBlock((int)count);
				var mem_stream=new MemoryStream(data.Data);
				var res=new StreamJPGAbstraction(mem_stream);
				(file as Rw2.File).JpgFromRaw=new Jpeg.File(res,ReadStyle.Average);
				seen_jpgfromraw=true;
				return null;
			}
			return base.ParseIFDEntry(tag,type,count,base_offset,offset);
		}
		private bool seen_jpgfromraw=false;
	}
	class StreamJPGAbstraction:
	File.IFileAbstraction
	{
		readonly Stream stream;
		public StreamJPGAbstraction(Stream stream)
		{
			this.stream=stream;
		}
		public string Name
		{
			get
			{
				return"JpgFromRaw.jpg";
			}
		}
		public void CloseStream(System.IO.Stream stream)
		{
			stream.Close();
		}
		public System.IO.Stream ReadStream
		{
			get
			{
				return stream;
			}
		}
		public System.IO.Stream WriteStream
		{
			get
			{
				return stream;
			}
		}
	}
}
