using System;
using System.Collections.Generic;
using TagLib;
using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Tags;
namespace TagLib.Tiff.Cr2
{
	[SupportedMimeType("taglib/cr2","cr2")]
	[SupportedMimeType("image/cr2")]
	[SupportedMimeType("image/x-canon-cr2")]
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
			uint raw_ifd_offset=ReadAdditionalCR2Header();
			ReadIFD(first_ifd_offset,3);
			ReadIFD(raw_ifd_offset,1);
		}
		private uint ReadAdditionalCR2Header()
		{
			ByteVector header=ReadBlock(8);
			if(header.Count!=8)
			{
				throw new CorruptFileException("Unexpected end of CR2 header");
			}
			if(header.Mid(0,2).ToString()!="CR")
			{
				throw new CorruptFileException("CR2 Magic (CR) expected");
			}
			byte major_version=header[2];
			byte minor_version=header[3];
			if(major_version!=2||minor_version!=0)
			{
				throw new UnsupportedFormatException("Only major version 2 and minor version 0 are supported");
			}
			uint raw_ifd_offset=header.Mid(4,4).ToUInt(IsBigEndian);
			return raw_ifd_offset;
		}
		private Properties ExtractProperties()
		{
			int width=0,height=0;
			IFDTag tag=GetTag(TagTypes.TiffIFD)
			as IFDTag;
			width=(int)(tag.ExifIFD.GetLongValue(0,(ushort)ExifEntryTag.PixelXDimension)??0);
			height=(int)(tag.ExifIFD.GetLongValue(0,(ushort)ExifEntryTag.PixelYDimension)??0);
			if(width>0&&height>0)
			{
				return new Properties(TimeSpan.Zero,new Codec(width,height,"Canon RAW File"));
			}
			return null;
		}
#endregion
	}
}
