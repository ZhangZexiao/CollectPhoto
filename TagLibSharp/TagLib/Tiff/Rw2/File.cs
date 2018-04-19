using System;
using System.Collections.Generic;
using TagLib;
using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Tags;
namespace TagLib.Tiff.Rw2
{
	[SupportedMimeType("taglib/rw2","rw2")]
	[SupportedMimeType("image/rw2")]
	[SupportedMimeType("taglib/raw","raw")]
	[SupportedMimeType("image/raw")]
	[SupportedMimeType("image/x-raw")]
	[SupportedMimeType("image/x-panasonic-raw")]
	public class File:TagLib.Tiff.BaseTiffFile
	{
#region private fields
		private Properties properties;
#endregion
#region public Properties
		public override TagLib.Properties Properties
		{
			get
			{
				return properties;
			}
		}
		public override bool Writeable
		{
			get
			{
				return false;
			}
		}
		public Jpeg.File JpgFromRaw
		{
			get;
			internal set;
		}
#endregion
#region constructors
		public File(string path,ReadStyle propertiesStyle):
		this(new File.LocalFileAbstraction(path),propertiesStyle)
		{
		}
		public File(string path):
		this(path,ReadStyle.Average)
		{
		}
		public File(File.IFileAbstraction abstraction,ReadStyle propertiesStyle):
		base(abstraction)
		{
			Magic=85;
			Read(propertiesStyle);
		}
		protected File(IFileAbstraction abstraction):
		this(abstraction,ReadStyle.Average)
		{
		}
#endregion
#region Public Methods
		public override void Save()
		{
			throw new NotSupportedException();
		}
		public override TagLib.Tag GetTag(TagLib.TagTypes type,bool create)
		{
			TagLib.Tag tag=base.GetTag(type,false);
			if(tag!=null)
			{
				return tag;
			}
			if(!create||(type&ImageTag.AllowedTypes)==0)
			{
				return null;
			}
			if(type!=TagTypes.TiffIFD)
			{
				return base.GetTag(type,create);
			}
			ImageTag new_tag=new IFDTag(this);
			ImageTag.AddTag(new_tag);
			return new_tag;
		}
#endregion
#region private methods
		private void Read(ReadStyle propertiesStyle)
		{
			Mode=AccessMode.Read;
			try
			{
				ImageTag=new CombinedImageTag(TagTypes.TiffIFD);
				ReadFile();
				TagTypesOnDisk=TagTypes;
				if(propertiesStyle!=ReadStyle.None)
				{
					properties=ExtractProperties();
				}
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
		private void ReadFile()
		{
			uint first_ifd_offset=ReadHeader();
			uint raw_ifd_offset=ReadAdditionalRW2Header();
			ReadIFD(first_ifd_offset,3);
			ReadIFD(raw_ifd_offset,1);
		}
		private uint ReadAdditionalRW2Header()
		{
			ByteVector header=ReadBlock(16);
			if(header.Count!=16)
			{
				throw new CorruptFileException("Unexpected end of RW2 header");
			}
			return(uint)Tell;
		}
		private Properties ExtractProperties()
		{
			int width=0,height=0;
			IFDTag tag=GetTag(TagTypes.TiffIFD)
			as IFDTag;
			IFDStructure structure=tag.Structure;
			width=(int)(structure.GetLongValue(0,0x07)??0);
			height=(int)(structure.GetLongValue(0,0x06)??0);
			var vendor=ImageTag.Make;
			if(vendor=="LEICA")
			{
				vendor="Leica";
			}
			var desc=String.Format("{0} RAW File",vendor);
			if(width>0&&height>0)
			{
				return new Properties(TimeSpan.Zero,new Codec(width,height,desc));
			}
			return null;
		}
		protected override TagLib.IFD.IFDReader CreateIFDReader(BaseTiffFile file,bool is_bigendian,IFDStructure structure,long base_offset,uint ifd_offset,uint max_offset)
		{
			return new IFDReader(file,is_bigendian,structure,base_offset,ifd_offset,max_offset);
		}
#endregion
	}
}
