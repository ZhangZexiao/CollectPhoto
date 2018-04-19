using System;
using System.Collections.Generic;
using System.IO;
using TagLib.Image;
using TagLib.IFD;
using TagLib.IFD.Entries;
using TagLib.Xmp;
namespace TagLib.Jpeg
{
	[SupportedMimeType("taglib/jpg","jpg")]
	[SupportedMimeType("taglib/jpeg","jpeg")]
	[SupportedMimeType("taglib/jpe","jpe")]
	[SupportedMimeType("taglib/jif","jif")]
	[SupportedMimeType("taglib/jfif","jfif")]
	[SupportedMimeType("taglib/jfi","jfi")]
	[SupportedMimeType("image/jpeg")]
	public class File:TagLib.Image.ImageBlockFile
	{
		private static readonly string EXIF_IDENTIFIER="Exif\0\0";
		private static readonly string IPTC_IIM_IDENTIFIER="Photoshop 3.0\u00008BIM\u0004\u0004";
		private static readonly byte[]BASIC_JFIF_HEADER=new byte[]
		{
			0xFF,
			(byte)Marker.APP0,
			0x00,
			0x10,
			0x4A,
			0x46,
			0x49,
			0x46,
			0x00,
			0x01,
			0x01,
			0x01,
			0x00,
			0x48,
			0x00,
			0x48,
			0x00,
			0x00
		};
#region Private Fields
		private Properties properties;
		private ByteVector jfif_header=null;
		ushort width;
		ushort height;
		int quality;
#endregion
#region Constructors
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
#region Public Properties
		public override TagLib.Properties Properties
		{
			get
			{
				return properties;
			}
		}
#endregion
#region Public Methods
		public override TagLib.Tag GetTag(TagLib.TagTypes type,bool create)
		{
			if(type==TagTypes.XMP)
			{
				foreach(Tag tag in ImageTag.AllTags)
				{
					if((tag.TagTypes&type)==type||(tag.TagTypes&TagTypes.IPTCIIM)!=0)
					{
						return tag;
					}
				}
			}
			if(type==TagTypes.IPTCIIM&&create)
			{
				return base.GetTag(type,false);
			}
			return base.GetTag(type,create);
		}
		public override void Save()
		{
			if(!Writeable||PossiblyCorrupt)
			{
				throw new InvalidOperationException("File not writeable. Corrupt metadata?");
			}
			Mode=AccessMode.Write;
			try
			{
				WriteMetadata();
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
				ImageTag=new CombinedImageTag(TagTypes.XMP|TagTypes.TiffIFD|TagTypes.JpegComment|TagTypes.IPTCIIM);
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
		private Properties ExtractProperties()
		{
			if(width>0&&height>0)
			{
				return new Properties(TimeSpan.Zero,new Codec(width,height,quality));
			}
			return null;
		}
		private void ValidateHeader()
		{
			ByteVector segment=ReadBlock(2);
			if(segment.ToUShort()!=0xFFD8)
			{
				throw new CorruptFileException("Expected SOI marker at the start of the file.");
			}
		}
		private Marker ReadSegmentMarker()
		{
			ByteVector segment_header=ReadBlock(2);
			if(segment_header.Count!=2)
			{
				throw new CorruptFileException("Could not read enough bytes for segment maker");
			}
			if(segment_header[0]!=0xFF)
			{
				throw new CorruptFileException("Start of Segment expected at "+(Tell-2));
			}
			return(Marker)segment_header[1];
		}
		private ushort ReadSegmentSize()
		{
			long position=Tell;
			ByteVector segment_size_bytes=ReadBlock(2);
			if(segment_size_bytes.Count!=2)
			{
				throw new CorruptFileException("Could not read enough bytes to determine segment size");
			}
			ushort segment_size=segment_size_bytes.ToUShort();
			if(segment_size<2)
			{
				throw new CorruptFileException(String.Format("Invalid segment size ({0} bytes)",segment_size));
			}
			long length=0;
			try
			{
				length=Length;
			}
			catch(Exception)
			{
			}
			if(length>0&&position+segment_size>=length)
			{
				throw new CorruptFileException("Segment size exceeds file size");
			}
			return segment_size;
		}
		private void ReadMetadata()
		{
			while(true)
			{
				Marker marker=ReadSegmentMarker();
				if(marker==Marker.EOI||marker==Marker.SOS)
				{
					break;
				}
				long position=Tell;
				ushort segment_size=ReadSegmentSize();
				ushort data_size=(ushort)(segment_size-2);
				switch(marker)
				{
				case Marker.APP0:
					ReadJFIFHeader(data_size);
					break;
				case Marker.APP1:
					ReadAPP1Segment(data_size);
					break;
				case Marker.APP13:
					ReadAPP13Segment(data_size);
					break;
				case Marker.COM:
					ReadCOMSegment(data_size);
					break;
				case Marker.SOF0:
				case Marker.SOF1:
				case Marker.SOF2:
				case Marker.SOF3:
				case Marker.SOF9:
				case Marker.SOF10:
				case Marker.SOF11:
					ReadSOFSegment(data_size,marker);
					break;
				case Marker.DQT:
					ReadDQTSegment(data_size);
					break;
				}
				Seek(position+segment_size,SeekOrigin.Begin);
			}
		}
		private void ReadJFIFHeader(ushort length)
		{
			if(Tell!=6)
			{
				return;
			}
			if(ReadBlock(5).ToString().Equals("JFIF\0"))
			{
				Seek(2,SeekOrigin.Begin);
				jfif_header=ReadBlock(length+2+2);
				AddMetadataBlock(2,length+2+2);
			}
		}
		private void ReadAPP1Segment(ushort length)
		{
			long position=Tell;
			ByteVector data=null;
			int exif_header_length=14;
			if((ImageTag.TagTypes&TagLib.TagTypes.TiffIFD)==0x00&&length>=exif_header_length)
			{
				data=ReadBlock(exif_header_length);
				if(data.Count==exif_header_length&&data.Mid(0,6).ToString().Equals(EXIF_IDENTIFIER))
				{
					bool is_bigendian=data.Mid(6,2).ToString().Equals("MM");
					ushort magic=data.Mid(8,2).ToUShort(is_bigendian);
					if(magic!=42)
					{
						throw new Exception(String.Format("Invalid TIFF magic: {0}",magic));
					}
					uint ifd_offset=data.Mid(10,4).ToUInt(is_bigendian);
					var exif=new IFDTag();
					var reader=new IFDReader(this,is_bigendian,exif.Structure,position+6,ifd_offset,(uint)(length-6));
					reader.Read();
					ImageTag.AddTag(exif);
					AddMetadataBlock(position-4,length+4);
					return;
				}
			}
			int xmp_header_length=XmpTag.XAP_NS.Length+1;
			if((ImageTag.TagTypes&TagLib.TagTypes.XMP)==0x00&&length>=xmp_header_length)
			{
				if(data==null)
				{
					data=ReadBlock(xmp_header_length);
				}
				else
				{
					data.Add(ReadBlock(xmp_header_length-exif_header_length));
				}
				if(data.ToString().Equals(XmpTag.XAP_NS+"\0"))
				{
					ByteVector xmp_data=ReadBlock(length-xmp_header_length);
					ImageTag.AddTag(new XmpTag(xmp_data.ToString(),this));
					AddMetadataBlock(position-4,length+4);
				}
			}
		}
		private void ReadAPP13Segment(ushort length)
		{
			var data=ReadBlock(length);
			var iptc_iim_length=IPTC_IIM_IDENTIFIER.Length;
			if(length<iptc_iim_length||data.Mid(0,iptc_iim_length)!=IPTC_IIM_IDENTIFIER)
			{
				return;
			}
			var headerInfoLen=data.Mid(iptc_iim_length,1).ToUShort();
			int lenToSkip;
			if(headerInfoLen>0)
			{
				lenToSkip=1+headerInfoLen+4;
			}
			else
			{
				lenToSkip=6;
			}
			data.RemoveRange(0,iptc_iim_length+lenToSkip);
			var reader=new IIM.IIMReader(data);
			var tag=reader.Process();
			if(tag!=null)
			{
				ImageTag.AddTag(tag);
			}
		}
		private void WriteMetadata()
		{
			ByteVector data=new ByteVector();
			if(jfif_header!=null)
			{
				data.Add(jfif_header);
			}
			else
			{
				data.Add(BASIC_JFIF_HEADER);
			}
			data.Add(RenderExifSegment());
			data.Add(RenderXMPSegment());
			data.Add(RenderCOMSegment());
			SaveMetadata(data,2);
		}
		private ByteVector RenderExifSegment()
		{
			IFDTag exif=ImageTag.Exif;
			if(exif==null)
			{
				return null;
			}
			uint first_ifd_offset=8;
			var renderer=new IFDRenderer(true,exif.Structure,first_ifd_offset);
			ByteVector exif_data=renderer.Render();
			uint segment_size=(uint)(first_ifd_offset+exif_data.Count+2+6);
			if(segment_size>ushort.MaxValue)
			{
				throw new Exception("Exif Segment is too big to render");
			}
			ByteVector data=new ByteVector(new byte[]{0xFF,(byte)Marker.APP1});
			data.Add(ByteVector.FromUShort((ushort)segment_size));
			data.Add("Exif\0\0");
			data.Add(ByteVector.FromString("MM",StringType.Latin1));
			data.Add(ByteVector.FromUShort(42));
			data.Add(ByteVector.FromUInt(first_ifd_offset));
			data.Add(exif_data);
			return data;
		}
		private ByteVector RenderXMPSegment()
		{
			XmpTag xmp=ImageTag.Xmp;
			if(xmp==null)
			{
				return null;
			}
			ByteVector xmp_data=XmpTag.XAP_NS+"\0";
			xmp_data.Add(xmp.Render());
			uint segment_size=(uint)(2+xmp_data.Count);
			if(segment_size>ushort.MaxValue)
			{
				throw new Exception("XMP Segment is too big to render");
			}
			ByteVector data=new ByteVector(new byte[]{0xFF,(byte)Marker.APP1});
			data.Add(ByteVector.FromUShort((ushort)segment_size));
			data.Add(xmp_data);
			return data;
		}
		private void ReadCOMSegment(int length)
		{
			if((ImageTag.TagTypes&TagLib.TagTypes.JpegComment)!=0x00)
			{
				return;
			}
			long position=Tell;
			JpegCommentTag com_tag;
			if(length==0)
			{
				com_tag=new JpegCommentTag();
			}
			else
			{
				ByteVector data=ReadBlock(length);
				int terminator=data.Find("\0",0);
				if(terminator<0)
				{
					com_tag=new JpegCommentTag(data.ToString());
				}
				else
				{
					com_tag=new JpegCommentTag(data.Mid(0,terminator).ToString());
				}
			}
			ImageTag.AddTag(com_tag);
			AddMetadataBlock(position-4,length+4);
		}
		private ByteVector RenderCOMSegment()
		{
			JpegCommentTag com_tag=GetTag(TagTypes.JpegComment)
			as JpegCommentTag;
			if(com_tag==null)
			{
				return null;
			}
			ByteVector com_data=ByteVector.FromString(com_tag.Value+"\0",StringType.Latin1);
			uint segment_size=(uint)(2+com_data.Count);
			if(segment_size>ushort.MaxValue)
			{
				throw new Exception("Comment Segment is too big to render");
			}
			ByteVector data=new ByteVector(new byte[]{0xFF,(byte)Marker.COM});
			data.Add(ByteVector.FromUShort((ushort)segment_size));
			data.Add(com_data);
			return data;
		}
		void ReadSOFSegment(int length,Marker marker)
		{
#pragma warning disable 219 
			byte p=ReadBlock(1)[0];
#pragma warning restore 219
			height=ReadBlock(2).ToUShort();
			width=ReadBlock(2).ToUShort();
		}
		void ReadDQTSegment(int length)
		{
			while(length>0)
			{
				byte pqtq=ReadBlock(1)[0];
				length--;
				byte pq=(byte)(pqtq>>4);
				byte tq=(byte)(pqtq&0x0f);
				int[]table=null;
				switch(tq)
				{
				case 0:
					table=Table.StandardLuminanceQuantization;
					break;
				case 1:
					table=Table.StandardChrominanceQuantization;
					break;
				}
				bool allones=true;
				double cumsf=0.0;
				for(int row=0;row<8;row++)
				{
					for(int col=0;col<8;col++)
					{
						ushort val=ReadBlock(pq==1?2:1).ToUShort();
						length-=(pq+1);
						if(table!=null)
						{
							double x=100.0*(double)val/(double)table[row*8+col];
							cumsf+=x;
							allones=allones&&(val==1);
						}
					}
				}
				if(table!=null)
				{
					double local_q;
					cumsf/=64.0;
					if(allones)
					{
						local_q=100.0;
					}
					else if(cumsf<=100.0)
					{
						local_q=(200.0-cumsf)/2.0;
					}
					else
					{
						local_q=5000.0/cumsf;
					}
					quality=Math.Max(quality,(int)local_q);
				}
			}
		}
#endregion
	}
}
