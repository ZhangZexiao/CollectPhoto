using System;
using System.IO;
using TagLib;
using TagLib.Image;
using TagLib.Xmp;
namespace TagLib.Gif
{
	[SupportedMimeType("taglib/gif","gif")]
	[SupportedMimeType("image/gif")]
	public class File:TagLib.Image.ImageBlockFile
	{
#region GIF specific constants
		protected static readonly string SIGNATURE="GIF";
		protected static readonly string VERSION_87A="87a";
		protected static readonly string VERSION_89A="89a";
		private static readonly string XMP_IDENTIFIER="XMP Data";
		private static readonly string XMP_AUTH_CODE="XMP";
		private static readonly byte[]XMP_MAGIC_TRAILER=new byte[]
		{
			0x01,
			0xFF,
			0xFE,
			0xFD,
			0xFC,
			0xFB,
			0xFA,
			0xF9,
			0xF8,
			0xF7,
			0xF6,
			0xF5,
			0xF4,
			0xF3,
			0xF2,
			0xF1,
			0xF0,
			0xEF,
			0xEE,
			0xED,
			0xEC,
			0xEB,
			0xEA,
			0xE9,
			0xE8,
			0xE7,
			0xE6,
			0xE5,
			0xE4,
			0xE3,
			0xE2,
			0xE1,
			0xE0,
			0xDF,
			0xDE,
			0xDD,
			0xDC,
			0xDB,
			0xDA,
			0xD9,
			0xD8,
			0xD7,
			0xD6,
			0xD5,
			0xD4,
			0xD3,
			0xD2,
			0xD1,
			0xD0,
			0xCF,
			0xCE,
			0xCD,
			0xCC,
			0xCB,
			0xCA,
			0xC9,
			0xC8,
			0xC7,
			0xC6,
			0xC5,
			0xC4,
			0xC3,
			0xC2,
			0xC1,
			0xC0,
			0xBF,
			0xBE,
			0xBD,
			0xBC,
			0xBB,
			0xBA,
			0xB9,
			0xB8,
			0xB7,
			0xB6,
			0xB5,
			0xB4,
			0xB3,
			0xB2,
			0xB1,
			0xB0,
			0xAF,
			0xAE,
			0xAD,
			0xAC,
			0xAB,
			0xAA,
			0xA9,
			0xA8,
			0xA7,
			0xA6,
			0xA5,
			0xA4,
			0xA3,
			0xA2,
			0xA1,
			0xA0,
			0x9F,
			0x9E,
			0x9D,
			0x9C,
			0x9B,
			0x9A,
			0x99,
			0x98,
			0x97,
			0x96,
			0x95,
			0x94,
			0x93,
			0x92,
			0x91,
			0x90,
			0x8F,
			0x8E,
			0x8D,
			0x8C,
			0x8B,
			0x8A,
			0x89,
			0x88,
			0x87,
			0x86,
			0x85,
			0x84,
			0x83,
			0x82,
			0x81,
			0x80,
			0x7F,
			0x7E,
			0x7D,
			0x7C,
			0x7B,
			0x7A,
			0x79,
			0x78,
			0x77,
			0x76,
			0x75,
			0x74,
			0x73,
			0x72,
			0x71,
			0x70,
			0x6F,
			0x6E,
			0x6D,
			0x6C,
			0x6B,
			0x6A,
			0x69,
			0x68,
			0x67,
			0x66,
			0x65,
			0x64,
			0x63,
			0x62,
			0x61,
			0x60,
			0x5F,
			0x5E,
			0x5D,
			0x5C,
			0x5B,
			0x5A,
			0x59,
			0x58,
			0x57,
			0x56,
			0x55,
			0x54,
			0x53,
			0x52,
			0x51,
			0x50,
			0x4F,
			0x4E,
			0x4D,
			0x4C,
			0x4B,
			0x4A,
			0x49,
			0x48,
			0x47,
			0x46,
			0x45,
			0x44,
			0x43,
			0x42,
			0x41,
			0x40,
			0x3F,
			0x3E,
			0x3D,
			0x3C,
			0x3B,
			0x3A,
			0x39,
			0x38,
			0x37,
			0x36,
			0x35,
			0x34,
			0x33,
			0x32,
			0x31,
			0x30,
			0x2F,
			0x2E,
			0x2D,
			0x2C,
			0x2B,
			0x2A,
			0x29,
			0x28,
			0x27,
			0x26,
			0x25,
			0x24,
			0x23,
			0x22,
			0x21,
			0x20,
			0x1F,
			0x1E,
			0x1D,
			0x1C,
			0x1B,
			0x1A,
			0x19,
			0x18,
			0x17,
			0x16,
			0x15,
			0x14,
			0x13,
			0x12,
			0x11,
			0x10,
			0x0F,
			0x0E,
			0x0D,
			0x0C,
			0x0B,
			0x0A,
			0x09,
			0x08,
			0x07,
			0x06,
			0x05,
			0x04,
			0x03,
			0x02,
			0x01,
			0x00,
			0x00
		};
#endregion
#region private fields
		private int width;
		private int height;
		private Properties properties;
		private string version;
		private long start_of_blocks= -1;
#endregion
#region public Properties
		public override TagLib.Properties Properties
		{
			get
			{
				return properties;
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
			Mode=AccessMode.Write;
			try
			{
				SaveMetadata();
				TagTypesOnDisk=TagTypes;
			}
			finally
			{
				Mode=AccessMode.Closed;
			}
		}
#endregion
#region Private Methods
		private void Read(ReadStyle propertiesStyle)
		{
			Mode=AccessMode.Read;
			try
			{
				ImageTag=new CombinedImageTag(TagTypes.XMP|TagTypes.GifComment);
				ReadHeader();
				ReadMetadata();
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
		private byte ReadByte()
		{
			ByteVector data=ReadBlock(1);
			if(data.Count!=1)
			{
				throw new CorruptFileException("Unexpected end of file");
			}
			return data[0];
		}
		private void ReadHeader()
		{
			ByteVector data=ReadBlock(13);
			if(data.Count!=13)
			{
				throw new CorruptFileException("Unexpected end of Header");
			}
			if(data.Mid(0,3).ToString()!=SIGNATURE)
			{
				throw new CorruptFileException(String.Format("Expected a GIF signature at start of file, but found: {0}",data.Mid(0,3).ToString()));
			}
			var read_version=data.Mid(3,3).ToString();
			if(read_version==VERSION_87A||read_version==VERSION_89A)
			{
				version=read_version;
			}
			else
			{
				throw new UnsupportedFormatException(String.Format("Only GIF versions 87a and 89a are currently supported, but not: {0}",read_version));
			}
			width=data.Mid(6,2).ToUShort(false);
			height=data.Mid(8,2).ToUShort(false);
			SkipColorTable(data[10]);
		}
		private void ReadMetadata()
		{
			start_of_blocks=Tell;
			while(true)
			{
				byte identifier=ReadByte();
				switch(identifier)
				{
				case 0x2c:
					SkipImage();
					break;
				case 0x21:
					ReadExtensionBlock();
					break;
				case 0x3B:
					return;
				default:
					throw new CorruptFileException(String.Format("Do not know what to do with byte 0x{0:X2} at the beginning of a block ({1}).",identifier,Tell-1));
				}
			}
		}
		private void ReadExtensionBlock()
		{
			byte identifier=ReadByte();
			switch(identifier)
			{
			case 0xFE:
				ReadCommentBlock();
				break;
			case 0xFF:
				ReadApplicationExtensionBlock();
				break;
			case 0xF9:
			case 0x01:
			default:
				SkipSubBlocks();
				break;
			}
		}
		private void ReadApplicationExtensionBlock()
		{
			long position=Tell;
			ByteVector data=ReadBlock(12);
			if(data.Count!=12)
			{
				throw new CorruptFileException("");
			}
			if(data.Mid(1,8)==XMP_IDENTIFIER&&data.Mid(9,3)==XMP_AUTH_CODE)
			{
				long data_start=Tell;
				long xmp_trailer_start=Find(new byte[]{0x00},data_start)-XMP_MAGIC_TRAILER.Length+2;
				Seek(data_start,SeekOrigin.Begin);
				if(xmp_trailer_start<=data_start)
				{
					throw new CorruptFileException("No End of XMP data found");
				}
				int data_length=(int)(xmp_trailer_start-data_start);
				ByteVector xmp_data=ReadBlock(data_length);
				ImageTag.AddTag(new XmpTag(xmp_data.ToString(StringType.UTF8),this));
				AddMetadataBlock(position-2,14+data_length+XMP_MAGIC_TRAILER.Length);
				Seek(xmp_trailer_start+XMP_MAGIC_TRAILER.Length,SeekOrigin.Begin);
			}
			else
			{
				SkipSubBlocks();
			}
		}
		private void ReadCommentBlock()
		{
			long position=Tell;
			string comment=ReadSubBlocks();
			if((TagTypes&TagTypes.GifComment)==0x00)
			{
				ImageTag.AddTag(new GifCommentTag(comment));
				AddMetadataBlock(position-2,Tell-position+2);
			}
		}
		private void SkipColorTable(byte packed_data)
		{
			if((packed_data&0x80)==0x80)
			{
				int table_size=3*(1<<((packed_data&0x07)+1));
				ByteVector color_table=ReadBlock(table_size);
				if(color_table.Count!=table_size)
				{
					throw new CorruptFileException("Unexpected end of Color Table");
				}
			}
		}
		private void SkipImage()
		{
			ByteVector data=ReadBlock(9);
			if(data.Count!=9)
			{
				throw new CorruptFileException("Unexpected end of Image Descriptor");
			}
			SkipColorTable(data[8]);
			ReadBlock(1);
			SkipSubBlocks();
		}
		private string ReadSubBlocks()
		{
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			byte length=0;
			do
			{
				if(length>=0)
				{
					builder.Append(ReadBlock(length).ToString());
				}
				length=ReadByte();
			}
			while(length!=0);
			return builder.ToString();
		}
		private void SkipSubBlocks()
		{
			byte length=0;
			do
			{
				if(Tell+length>=Length)
				{
					throw new CorruptFileException("Unexpected end of Sub-Block");
				}
				Seek(Tell+length,SeekOrigin.Begin);
				length=ReadByte();
			}
			while(length!=0);
		}
		private void SaveMetadata()
		{
			ByteVector comment_block=RenderGifCommentBlock();
			ByteVector xmp_block=RenderXMPBlock();
			if(comment_block!=null&&xmp_block!=null&&version!=VERSION_89A)
			{
				Insert(VERSION_89A,3,VERSION_89A.Length);
			}
			ByteVector metadata_blocks=new ByteVector();
			metadata_blocks.Add(comment_block);
			metadata_blocks.Add(xmp_block);
			SaveMetadata(metadata_blocks,start_of_blocks);
		}
		private ByteVector RenderXMPBlock()
		{
			XmpTag xmp=ImageTag.Xmp;
			if(xmp==null)
			{
				return null;
			}
			ByteVector xmp_data=new ByteVector();
			xmp_data.Add(new byte[]{0x21,0xFF,0x0B});
			xmp_data.Add(XMP_IDENTIFIER);
			xmp_data.Add(XMP_AUTH_CODE);
			xmp_data.Add(xmp.Render());
			xmp_data.Add(XMP_MAGIC_TRAILER);
			return xmp_data;
		}
		private ByteVector RenderGifCommentBlock()
		{
			GifCommentTag comment_tag=GetTag(TagTypes.GifComment)
			as GifCommentTag;
			if(comment_tag==null)
			{
				return null;
			}
			string comment=comment_tag.Comment;
			if(comment==null)
			{
				return null;
			}
			ByteVector comment_data=new ByteVector();
			comment_data.Add(new byte[]{0x21,0xFE});
			ByteVector comment_bytes=new ByteVector(comment);
			byte block_max=255;
			for(int start=0;start<comment_bytes.Count;start+=block_max)
			{
				byte block_length=(byte)Math.Min(comment_bytes.Count-start,block_max);
				comment_data.Add(block_length);
				comment_data.Add(comment_bytes.Mid(start,block_length));
			}
			comment_data.Add(new byte[]{0x00});
			return comment_data;
		}
		private Properties ExtractProperties()
		{
			if(width>0&&height>0)
			{
				return new Properties(TimeSpan.Zero,new Codec(width,height));
			}
			return null;
		}
#endregion
	}
}
