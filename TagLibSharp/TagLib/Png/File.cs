using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using TagLib;
using TagLib.Image;
using TagLib.Xmp;
namespace TagLib.Png
{
	[SupportedMimeType("taglib/png","png")]
	[SupportedMimeType("image/png")]
	public class File:TagLib.Image.ImageBlockFile
	{
#region GIF specific constants
		private readonly byte[]HEADER=new byte[]
		{
			137,80,78,71,13,10,26,10
		};
		private readonly byte[]IHDR_CHUNK_TYPE=new byte[]
		{
			73,72,68,82
		};
		private readonly byte[]IEND_CHUNK_TYPE=new byte[]
		{
			73,69,78,68
		};
		private readonly byte[]iTXt_CHUNK_TYPE=new byte[]
		{
			105,84,88,116
		};
		private readonly byte[]tEXt_CHUNK_TYPE=new byte[]
		{
			116,69,88,116
		};
		private readonly byte[]zTXt_CHUNK_TYPE=new byte[]
		{
			122,84,88,116
		};
		private readonly byte[]XMP_CHUNK_HEADER=new byte[]
		{
			0x58,
			0x4D,
			0x4C,
			0x3A,
			0x63,
			0x6F,
			0x6D,
			0x2E,
			0x61,
			0x64,
			0x6F,
			0x62,
			0x65,
			0x2E,
			0x78,
			0x6D,
			0x70,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00
		};
#endregion
#region private fields
		private int height;
		private int width;
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
#region private methods
		private void Read(ReadStyle propertiesStyle)
		{
			Mode=AccessMode.Read;
			try
			{
				ImageTag=new CombinedImageTag(TagTypes.XMP|TagTypes.Png);
				ValidateHeader();
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
		private void ValidateHeader()
		{
			ByteVector data=ReadBlock(8);
			if(data.Count!=8)
			{
				throw new CorruptFileException("Unexpected end of header");
			}
			if(!data.Equals(new ByteVector(HEADER)))
			{
				throw new CorruptFileException("PNG Header was expected");
			}
		}
		private int ReadChunkLength()
		{
			ByteVector data=ReadBlock(4);
			if(data.Count!=4)
			{
				throw new CorruptFileException("Unexpected end of Chunk Length");
			}
			uint length=data.ToUInt(true);
			if(length>Int32.MaxValue)
			{
				throw new CorruptFileException("PNG limits the Chunk Length to 2^31-1");
			}
			return(int)length;
		}
		private ByteVector ReadChunkType()
		{
			ByteVector data=ReadBlock(4);
			if(data.Count!=4)
			{
				throw new CorruptFileException("Unexpected end of Chunk Type");
			}
			return data;
		}
		private ByteVector ReadCRC()
		{
			ByteVector data=ReadBlock(4);
			if(data.Count!=4)
			{
				throw new CorruptFileException("Unexpected end of CRC");
			}
			return data;
		}
		private ByteVector ReadChunkData(int data_length)
		{
			ByteVector data=ReadBlock(data_length);
			if(data.Count!=data_length)
			{
				throw new CorruptFileException(String.Format("Chunk Data of Length {0} expected",data_length));
			}
			return data;
		}
		private string ReadTerminatedString(ByteVector data,int start_index,out int terminator_index)
		{
			if(start_index>=data.Count)
			{
				throw new CorruptFileException("Unexpected End of Data");
			}
			terminator_index=data.Find("\0",start_index);
			if(terminator_index<0)
			{
				throw new CorruptFileException("Cannot find string terminator");
			}
			return data.Mid(start_index,terminator_index-start_index).ToString();
		}
		private string ReadKeyword(ByteVector data,int start_index,out int terminator_index)
		{
			string keyword=ReadTerminatedString(data,start_index,out terminator_index);
			if(String.IsNullOrEmpty(keyword))
			{
				throw new CorruptFileException("Keyword cannot be empty");
			}
			return keyword;
		}
		private void SkipChunkData(int data_size)
		{
			long position=Tell;
			if(position+data_size>=Length)
			{
				throw new CorruptFileException(String.Format("Chunk Data of Length {0} expected",data_size));
			}
			Seek(Tell+data_size);
			ReadCRC();
		}
		private void ReadMetadata()
		{
			int data_length=ReadChunkLength();
			ByteVector type=ReadChunkType();
			if(!type.StartsWith(IHDR_CHUNK_TYPE))
			{
				throw new CorruptFileException(String.Format("IHDR Chunk was expected, but Chunk {0} was found",type.ToString()));
			}
			ReadIHDRChunk(data_length);
			while(true)
			{
				data_length=ReadChunkLength();
				type=ReadChunkType();
				if(type.StartsWith(IEND_CHUNK_TYPE))
				{
					return;
				}
				else if(type.StartsWith(iTXt_CHUNK_TYPE))
				{
					ReadiTXtChunk(data_length);
				}
				else if(type.StartsWith(tEXt_CHUNK_TYPE))
				{
					ReadtEXtChunk(data_length);
				}
				else if(type.StartsWith(zTXt_CHUNK_TYPE))
				{
					ReadzTXtChunk(data_length);
				}
				else
				{
					SkipChunkData(data_length);
				}
			}
		}
		private void ReadIHDRChunk(int data_length)
		{
			if(data_length!=13)
			{
				throw new CorruptFileException("IHDR chunk data length must be 13");
			}
			ByteVector data=ReadChunkData(data_length);
			CheckCRC(IHDR_CHUNK_TYPE,data,ReadCRC());
			uint width=data.Mid(0,4).ToUInt(true);
			uint height=data.Mid(4,4).ToUInt(true);
			if(width>Int32.MaxValue||height>Int32.MaxValue)
			{
				throw new CorruptFileException("PNG limits width and heigth to 2^31-1");
			}
			this.width=(int)width;
			this.height=(int)height;
		}
		private void ReadiTXtChunk(int data_length)
		{
			long position=Tell;
			ByteVector data=ReadChunkData(data_length);
			CheckCRC(iTXt_CHUNK_TYPE,data,ReadCRC());
			if(data.StartsWith(XMP_CHUNK_HEADER))
			{
				ImageTag.AddTag(new XmpTag(data.Mid(XMP_CHUNK_HEADER.Length).ToString(StringType.UTF8),this));
				AddMetadataBlock(position-8,data_length+8+4);
				return;
			}
			int terminator_index;
			string keyword=ReadKeyword(data,0,out terminator_index);
			if(terminator_index+2>=data_length)
			{
				throw new CorruptFileException("Compression Flag and Compression Method byte expected");
			}
			byte compression_flag=data[terminator_index+1];
			byte compression_method=data[terminator_index+2];
			ByteVector txt_data=data.Mid(terminator_index+1);
			if(compression_flag!=0x00)
			{
				txt_data=Decompress(compression_method,txt_data);
				if(txt_data==null)
				{
					return;
				}
			}
			string value=txt_data.ToString();
			PngTag png_tag=GetTag(TagTypes.Png,true)
			as PngTag;
			if(png_tag.GetKeyword(keyword)==null)
			{
				png_tag.SetKeyword(keyword,value);
			}
			AddMetadataBlock(position-8,data_length+8+4);
		}
		private void ReadtEXtChunk(int data_length)
		{
			long position=Tell;
			ByteVector data=ReadChunkData(data_length);
			CheckCRC(tEXt_CHUNK_TYPE,data,ReadCRC());
			int keyword_terminator;
			string keyword=ReadKeyword(data,0,out keyword_terminator);
			string value=data.Mid(keyword_terminator+1).ToString();
			PngTag png_tag=GetTag(TagTypes.Png,true)
			as PngTag;
			if(png_tag.GetKeyword(keyword)==null)
			{
				png_tag.SetKeyword(keyword,value);
			}
			AddMetadataBlock(position-8,data_length+8+4);
		}
		private void ReadzTXtChunk(int data_length)
		{
			long position=Tell;
			ByteVector data=ReadChunkData(data_length);
			CheckCRC(zTXt_CHUNK_TYPE,data,ReadCRC());
			int terminator_index;
			string keyword=ReadKeyword(data,0,out terminator_index);
			if(terminator_index+1>=data_length)
			{
				throw new CorruptFileException("Compression Method byte expected");
			}
			byte compression_method=data[terminator_index+1];
			ByteVector plain_data=Decompress(compression_method,data.Mid(terminator_index+2));
			if(plain_data==null)
			{
				return;
			}
			string value=plain_data.ToString();
			PngTag png_tag=GetTag(TagTypes.Png,true)
			as PngTag;
			if(png_tag.GetKeyword(keyword)==null)
			{
				png_tag.SetKeyword(keyword,value);
			}
			AddMetadataBlock(position-8,data_length+8+4);
		}
		private void SaveMetadata()
		{
			ByteVector metadata_chunks=new ByteVector();
			metadata_chunks.Add(RenderXMPChunk());
			metadata_chunks.Add(RenderKeywordChunks());
			SaveMetadata(metadata_chunks,HEADER.Length+13+4+4+4);
		}
		private ByteVector RenderXMPChunk()
		{
			XmpTag xmp=ImageTag.Xmp;
			if(xmp==null)
			{
				return null;
			}
			ByteVector chunk=new ByteVector();
			ByteVector xmp_data=xmp.Render();
			chunk.Add(ByteVector.FromUInt((uint)xmp_data.Count+(uint)XMP_CHUNK_HEADER.Length));
			chunk.Add(iTXt_CHUNK_TYPE);
			chunk.Add(XMP_CHUNK_HEADER);
			chunk.Add(xmp_data);
			chunk.Add(ComputeCRC(iTXt_CHUNK_TYPE,XMP_CHUNK_HEADER,xmp_data));
			return chunk;
		}
		private ByteVector RenderKeywordChunks()
		{
			PngTag png_tag=GetTag(TagTypes.Png,true)
			as PngTag;
			if(png_tag==null)
			{
				return null;
			}
			ByteVector chunks=new ByteVector();
			foreach(KeyValuePair<string,string>keyword in png_tag)
			{
				ByteVector data=new ByteVector();
				data.Add(keyword.Key);
				data.Add("\0");
				data.Add(keyword.Value);
				chunks.Add(ByteVector.FromUInt((uint)data.Count));
				chunks.Add(tEXt_CHUNK_TYPE);
				chunks.Add(data);
				chunks.Add(ComputeCRC(tEXt_CHUNK_TYPE,data));
			}
			return chunks;
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
#region Utility Stuff
		private static void CheckCRC(ByteVector chunk_type,ByteVector chunk_data,ByteVector crc_data)
		{
			ByteVector computed_crc=ComputeCRC(chunk_type,chunk_data);
			if(computed_crc!=crc_data)
			{
				throw new CorruptFileException(String.Format("CRC check failed for {0} Chunk (expected: 0x{1:X4}, read: 0x{2:X4}",chunk_type.ToString(),computed_crc.ToUInt(),crc_data.ToUInt()));
			}
		}
		private static ByteVector ComputeCRC(params ByteVector[]datas)
		{
			uint crc=0xFFFFFFFF;
			if(crc_table==null)
			{
				BuildCRCTable();
			}
			foreach(var data in datas)
			{
				foreach(byte b in data)
				{
					crc=crc_table[(crc^b)&0xFF]^(crc>>8);
				}
			}
			return ByteVector.FromUInt(crc^0xFFFFFFFF);
		}
		private static uint[]crc_table;
		private static void BuildCRCTable()
		{
			uint polynom=0xEDB88320;
			crc_table=new uint[256];
			for(int i=0;i<256;i++)
			{
				uint c=(uint)i;
				for(int k=0;k<8;k++)
				{
					if((c&0x00000001)!=0x00)
					{
						c=polynom^(c>>1);
					}
					else
					{
						c=c>>1;
					}
				}
				crc_table[i]=c;
			}
		}
		private static ByteVector Inflate(ByteVector data)
		{
			using(MemoryStream out_stream=new System.IO.MemoryStream())
			using(var input=new MemoryStream(data.Data))
			{
				input.Seek(2,SeekOrigin.Begin);
				using(var zipstream=new DeflateStream(input,CompressionMode.Decompress))
				{
					byte[]buffer=new byte[1024];
					int written_bytes;
					while((written_bytes=zipstream.Read(buffer,0,1024))>0)out_stream.Write(buffer,0,written_bytes);
					return new ByteVector(out_stream.ToArray());
				}
			}
		}
		private static ByteVector Decompress(byte compression_method,ByteVector compressed_data)
		{
			switch(compression_method)
			{
			case 0:
				return Inflate(compressed_data);
			default:
				return null;
			}
		}
#endregion
	}
}
